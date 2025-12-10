using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;
using Limilabs.Mail;
using Limilabs.Client.IMAP;
using Limilabs.Mail.MIME;
using System.Collections;

namespace ZeeInsurance
{
    public class LeadImport
    {
        XIInfraCache oCache = new XIInfraCache();

        public LeadImport()
        {

        }
        List<string> ValidateMessages = new List<string>();
        /// <summary>
        /// Parse Lead data and save to DB
        /// </summary>
        /// <returns></returns>
        public CResult Import_Lead(List<CNV> Params)
        {
            //Source
            //Motorhome
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Pasrse the Lead Info against source, class and build QS Instance";//expalin about this method logic
            try
            {
                XIIXI oXI = new XIIXI();
                int FKiOriginID = 0; int FKiSourceID = 0; int FKiClassID = 0; string sInfo = string.Empty;
                var sID = Params.Where(m => m.sName.ToLower() == XIConstant.Param_InstanceID.ToLower()).Select(m => m.sValue).FirstOrDefault();
                int iID = 0;
                int.TryParse(sID, out iID);
                if (iID > 0)
                {

                    var oBOI = oXI.BOI("xileadimport", iID.ToString());
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        var iOriginID = oBOI.AttributeI("FKiOriginID").sValue;
                        var iSourceID = oBOI.AttributeI("FKiSourceID").sValue;
                        var iClassID = oBOI.AttributeI("FKiClassID").sValue;
                        sInfo = oBOI.AttributeI("sLeadData").sValue;
                        int.TryParse(iOriginID, out FKiOriginID);
                        int.TryParse(iSourceID, out FKiSourceID);
                        int.TryParse(iClassID, out FKiClassID);
                    }
                }

                oTrace.oParams.Add(new CNV { sName = "FKiOriginID", sValue = FKiOriginID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "FKiSourceID", sValue = FKiSourceID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "FKiClassID", sValue = FKiClassID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "sInfo", sValue = sInfo });
                List<CNV> Mapping = new List<CNV>();
                List<CNV> Data = new List<CNV>();
                List<string> IDs = new List<string>();
                List<CNV> oParams = new List<CNV>();
                int iQSIID = 0;
                if (FKiOriginID > 0 && FKiSourceID > 0 && FKiClassID > 0 && !string.IsNullOrEmpty(sInfo))//check mandatory params are passed or not
                {
                    oCR = Save_QSInstance(FKiOriginID, FKiSourceID, FKiClassID);
                    oTrace.oTrace.Add(oCR.oTrace);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var sQSIID = (string)oCR.oResult;
                        int.TryParse(sQSIID, out iQSIID);
                        if (iQSIID > 0)
                        {
                            oCR = Parse_Lead(sInfo);
                            oTrace.oTrace.Add(oCR.oTrace);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                Data = (List<CNV>)oCR.oResult;
                                List<CNV> oWhr = new List<CNV>();
                                oWhr.Add(new CNV { sName = "FKiOriginID", sValue = FKiOriginID.ToString() });
                                oWhr.Add(new CNV { sName = "FKiSourceID", sValue = FKiSourceID.ToString() });
                                oWhr.Add(new CNV { sName = "FKiClassID", sValue = FKiClassID.ToString() });
                                XID1Click o1Click = new XID1Click();
                                var sQuery = string.Format("select * from XILeadImportMapping_T where FKiOriginID ={0} and FKiSourceID={1} and FKiClassID={2}", FKiOriginID, FKiSourceID, FKiClassID);
                                o1Click.Query = sQuery;
                                o1Click.BOID = 1305;
                                var Result = o1Click.OneClick_Run();
                                if (Result != null && Result.Values.Count() > 0)
                                {
                                    foreach (var items in Result.Values.ToList())
                                    {
                                        IDs.Add(items.AttributeI("fkifieldoriginid").sValue);
                                        Mapping.Add(new CNV { sName = items.AttributeI("sName").sValue, sValue = items.AttributeI("fkifieldoriginid").sValue });
                                    }
                                    var sWhere = string.Join(",", IDs);
                                    sWhere = "ID IN (" + sWhere + ")";
                                    XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                    var FieldDefs = Connection.SelectIN<XIDFieldOrigin>("XIFieldOrigin_T", sWhere).ToList();
                                    if (FieldDefs != null && FieldDefs.Count() > 0)
                                    {
                                        oCR = Save_XIValues(iQSIID, Data, Mapping, FieldDefs);
                                        oTrace.oTrace.Add(oCR.oTrace);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            oParams.Add(new CNV { sName = "iQSInstanceID", sValue = iQSIID.ToString() });
                                            Policy oPolicy = new Policy();
                                            oCR = oPolicy.InsertLead(oParams);
                                            if (oCR.bOK && oCR.oResult != null)
                                            {
                                                oCResult.oResult = "Success";
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                            }
                                            else
                                            {
                                                var oLeadI = (XIIBO)oCR.oResult;
                                                var FailedScripts = oLeadI.BOD.sScripts.Where(m => m.IsSuccess == false).ToList();
                                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                            }
                                        }
                                        else
                                        {
                                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        }
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                        oTrace.sMessage = "Error while getting FieldOrigin definitions";
                                    }
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oTrace.sMessage = "No mappings found in leadimport table";
                                }
                            }
                            else
                            {
                                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                            }
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Params missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Parse_Lead(string sInfo)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Parse string information and bulid the name value pairs";//expalin about this method logic
            try
            {
                List<CNV> oNV = new List<CNV>();
                oTrace.oParams.Add(new CNV { sName = "sInfo", sValue = sInfo });
                if (!string.IsNullOrEmpty(sInfo))//check mandatory params are passed or not
                {
                    var Rows = Regex.Split(sInfo, "\r\n").ToList();
                    foreach (var Row in Rows)
                    {
                        if (!string.IsNullOrEmpty(Row))
                        {
                            var Data = Row.Split(':').ToList();
                            if (Data != null && Data.Count() == 2 && !string.IsNullOrEmpty(Data[1]) && !string.IsNullOrEmpty(Data[1]))
                            {
                                oNV.Add(new CNV { sName = Data[0].Trim(), sValue = Data[1].Trim() });
                            }
                        }
                    }
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                    oCResult.oResult = oNV;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: sInfo is missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_QSInstance(int FKiOriginID, int FKiSourceID, int FKiClassID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {

                oTrace.oParams.Add(new CNV { sName = "FKiOriginID", sValue = FKiOriginID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "FKiSourceID", sValue = FKiSourceID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "FKiClassID", sValue = FKiClassID.ToString() });
                if (FKiOriginID > 0 && FKiSourceID > 0 && FKiClassID > 0)//check mandatory params are passed or not
                {
                    XIInfraCache oCache = new XIInfraCache();
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "QS Instance");
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = oBOD;
                    oBOI.sBOName = "QS Instance";
                    oBOI.SetAttribute("FKiClassID", FKiClassID.ToString());
                    oBOI.SetAttribute("FKiOriginID", FKiOriginID.ToString());
                    oBOI.SetAttribute("FKiSourceID", FKiSourceID.ToString());
                    oBOI.SetAttribute("iStage", "10");
                    oCR = oBOI.SaveV2(oBOI);
                    oTrace.oTrace.Add(oCR.oTrace);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oBOI = (XIIBO)oCR.oResult;
                        var iQSIID = oBOI.AttributeI("id").sValue;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = iQSIID;
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Params missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Save_XIValues(int iQSIID, List<CNV> Data, List<CNV> Mapping, List<XIDFieldOrigin> FieldDefs)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO BulkInsert = new XIIBO();
                List<XIIBO> oBulkBO = new List<XIIBO>();
                XIIValue oFIns = new XIIValue();
                oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                foreach (var NV in Data)
                {
                    var sValue = NV.sValue;
                    string sReslovedValue = sValue;
                    XIIBO oBOI = new XIIBO();
                    int iFieldOriginID = 0;
                    var FieldOriginID = Mapping.Where(m => m.sName.ToLower() == NV.sName.ToLower()).Select(m => m.sValue).FirstOrDefault();
                    if (FieldOriginID != null)
                    {
                        int.TryParse(FieldOriginID, out iFieldOriginID);
                        if (iFieldOriginID > 0)
                        {
                            var Def = FieldDefs.Where(m => m.ID == iFieldOriginID).FirstOrDefault();
                            if (Def != null)
                            {
                                if (Def.bIsOptionList || (Def.FKiBOID > 0 ||(Def.FKiBOIDXIGUID != null && Def.FKiBOIDXIGUID != Guid.Empty)))
                                {
                                    oCR = Resolve_XIValue(sValue, Def);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        sReslovedValue = (string)oCR.oResult;
                                        //sReslovedValue = sValue;
                                    }
                                }
                                if (Def.FKiDataType > 0)
                                {
                                    var oDataTypeD = (XIDDataType)oCache.GetObjectFromCache(XIConstant.CacheDataType, null, Def.FKiDataType.ToString());
                                    if (oDataTypeD.sBaseDataType.ToLower() == "int")
                                    {
                                        int ival;
                                        if (int.TryParse(sValue, out ival))
                                        {
                                            oFIns.iValue = ival;
                                        }
                                        else
                                        {
                                            oFIns.iValue = 0;
                                        }
                                    }
                                    else if (oDataTypeD.sBaseDataType.ToLower() == "datetime")
                                    {
                                        //if (!string.IsNullOrEmpty(items.Value.sValue))
                                        //{
                                        //    oFIns.dValue = Convert.ToDateTime(items.Value.sValue);
                                        //}
                                    }
                                    else if (oDataTypeD.sBaseDataType.ToLower() == "decimal")
                                    {
                                        decimal rval;
                                        if (decimal.TryParse(sValue, out rval))
                                        {
                                            oFIns.rValue = rval;
                                        }
                                        else
                                        {
                                            oFIns.rValue = 0;
                                        }
                                    }
                                    else if (oDataTypeD.sBaseDataType.ToLower() == "boolean")
                                    {
                                        if (sValue == "on")
                                        {
                                            oFIns.bValue = true;
                                        }
                                        else
                                        {
                                            oFIns.bValue = false;
                                        }

                                    }
                                    bool bSave = true;
                                    if (!string.IsNullOrEmpty(oDataTypeD.sRegex))
                                    {
                                        var OCR = Validate_Data(oDataTypeD.sRegex, sReslovedValue);
                                        if (OCR.oResult != null)
                                        {
                                            ValidateMessages.Add(Def.sValidationMessage + "/r/n");
                                            bSave = false;
                                        }
                                    }
                                    oBOI.SetAttribute("rValue", oFIns.rValue.ToString());
                                    oBOI.SetAttribute("bValue", oFIns.bValue.ToString());
                                    oBOI.SetAttribute("iValue", oFIns.iValue.ToString());
                                    oBOI.SetAttribute("sDerivedValue", sValue);
                                    oBOI.SetAttribute("sValue", sReslovedValue);
                                    oBOI.SetAttribute("FKiQSInstanceID", iQSIID.ToString());
                                    oBOI.SetAttribute("FKiFieldOriginID", Def.ID.ToString());
                                    oBOI.SetAttribute("dValue", DateTime.Now.ToString());
                                    if (bSave)
                                    {
                                        oBulkBO.Add(oBOI);
                                    }
                                }
                            }
                        }
                    }
                }
                if (oBulkBO != null && oBulkBO.Count() > 0)
                {
                    var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIFieldInstance_T", null);
                    oBulkBO.ForEach(f => f.BOD = oBOD);
                    var MakeDatatble = BulkInsert.MakeBulkSqlTable(oBulkBO);
                    oCR = BulkInsert.SaveBulk(MakeDatatble, oBOD.iDataSourceXIGUID.ToString(), "XIFieldInstance_T");
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = "Success";
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult Resolve_XIValue(string sValue, XIDFieldOrigin oDef)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "sName", sValue = oDef.sName });
                oTrace.oParams.Add(new CNV { sName = "sValue", sValue = sValue });
                string sResolvedValue = string.Empty;
                if (!string.IsNullOrEmpty(sValue))//check mandatory params are passed or not
                {
                    if (oDef.bIsOptionList)
                    {
                        Dictionary<string, object> Params = new Dictionary<string, object>();
                        Params["fkiqsfieldid"] = oDef.ID;
                        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                        var Options = Connection.Select<XIDFieldOptionList>("XIFieldOptionList_T", Params);
                        var Option = Options.Where(m => m.sOptionName.ToLower() == sValue.ToLower()).FirstOrDefault();
                        sResolvedValue = Option.sOptionValue;
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        oCResult.oResult = sResolvedValue;
                    }
                    else if (oDef.FKiBOID > 0 ||(oDef.FKiBOIDXIGUID != null && oDef.FKiBOIDXIGUID != Guid.Empty))
                    {
                        XIDXI oXID = new XIDXI();
                        if(oDef.FKiBOIDXIGUID != null && oDef.FKiBOIDXIGUID != Guid.Empty)
                        {
                            oCR = oXID.Get_AutoCompleteList("bo-" + oDef.FKiBOIDXIGUID, "");
                        }
                        else if (oDef.FKiBOID > 0)
                        {
                            oCR = oXID.Get_AutoCompleteList("bo-" + oDef.FKiBOID, "");
                        }
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            List<XIDFieldOptionList> FKDDL = new List<XIDFieldOptionList>();
                            FKDDL = (List<XIDFieldOptionList>)oCR.oResult;
                            var value = FKDDL.Where(m => m.sOptionName.Contains(sValue)).FirstOrDefault();
                            if (value != null)
                            {
                                sResolvedValue = value.sOptionValue;
                            }
                            oCResult.oResult = sResolvedValue;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }

                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Params missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Get_EmailFullDetails(int ID, int iUID, string sFolder)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic

            try
            {
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iUID", sValue = iUID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "sFolder", sValue = sFolder.ToString() });
                if (ID > 0 && !string.IsNullOrEmpty(sFolder))
                {
                    string lEmailDetails = string.Empty;
                    string sFromAddress = "";
                    string sFromName = "";
                    string sEmailDate;
                    var mMonth = "";
                    var dDate = "";
                    var yYear = "";
                    int i = 0;
                    //string database = dbcontext.AspNetUsers.Where(m => m.OrganizationID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
                    //DataContext Spdb = new DataContext(database);
                    //string SubscrptionID = 
                    string sAttachmentName = "";
                    int iAttachmentCount = 0;

                    ArrayList sAttachmentDetails = new ArrayList();
                    ArrayList aItemList = new ArrayList();

                    string sCSVAttachmentName = "";
                    // XIIBO Details = new XIIBO();
                    string UserName = string.Empty;// = Details.UserName;
                    string Password = string.Empty;// = Details.Password;
                    string Server = string.Empty;//;= Details.ServerName;
                    int Port = 0;//= Details.Port;
                    oCR = Get_EmailCredentials(ID);
                    oTrace.oTrace.Add(oCR.oTrace);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var oServer = (XIIBO)oCR.oResult;
                        UserName = oServer.AttributeI("UserName").sValue;
                        Password = oServer.AttributeI("Password").sValue;
                        Server = oServer.AttributeI("ServerName").sValue;
                        Port = Convert.ToInt32(oServer.AttributeI("Port").sValue);

                        using (Imap imap = new Imap())
                        {
                            imap.ConnectSSL(Server, Port);   // or ConnectSSL
                            imap.UseBestLogin(UserName, Password);

                            //Select folder
                            imap.Select(sFolder);

                            //Get details on selected UID
                            IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(iUID));

                            //Email UID
                            int UID = iUID;

                            //Email date for attachments
                            sEmailDate = (email.Date).Value.ToString();
                            var dSplitToDateTime = sEmailDate.Split(' ');
                            var dGetDate = dSplitToDateTime[0];
                            var dSplitToDate = dGetDate.Split('/');
                            mMonth = dSplitToDate[0];
                            dDate = dSplitToDate[1];
                            yYear = dSplitToDate[2];

                            //Email details
                            foreach (var sAddress in email.From)
                            {
                                sFromAddress = sAddress.Address;
                                sFromName = sAddress.Name;
                            }
                            string sSubject = email.Subject;
                            string sText = email.Text;

                            //Email Attachment
                            //check for number of attachemnts in the mail..
                            if (email.NonVisuals.Count == 0)
                            {
                                sAttachmentName = "";
                            }
                            else
                            {
                                //Get the count of attachments.
                                iAttachmentCount = email.NonVisuals.Count;

                                foreach (MimeData mime in email.Attachments)
                                {
                                    sAttachmentName = mime.SafeFileName;

                                    //Arraylist to insert values if tha email contains n attachments
                                    aItemList.Insert(i, sAttachmentName);

                                    //increment the i value to insert attachment name.
                                    i = i + 1;
                                }

                                //Convert the output in the form of arraylist to CSV
                                sCSVAttachmentName = string.Join(",", (string[])aItemList.ToArray(Type.GetType("System.String")));
                            }

                            //Seperate method call to save to DB
                            //SaveDetailsToDatabase(UID, sEmailDate, sFromAddress, sSubject, sText, sAttachmentPath, sCSVAttachmentName);
                            //ExtractEmailData(sText);
                            lEmailDetails = sFromName + "<" + sFromAddress + "> <> " + sSubject + " <> " + sText + " <> " + email.Html + " <> " + sCSVAttachmentName;
                            imap.Close();
                            oCResult.oResult = lEmailDetails;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory params missing";
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            }
            oCResult.oTrace = oTrace;
            return oCResult;
        }
        public CResult Get_EmailCredentials(int ID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                XIIXI oIXI = new XIIXI();
                var oBOI = oIXI.BOI("IOServerDetail", Convert.ToString(ID));
                if (oBOI != null && oBOI.Attributes.Count() > 0)
                {
                    oCResult.oResult = oBOI;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
            // var ServerDetails = dbContext.IOServerDetails.Where(m => m.ID == ID).FirstOrDefault();
        }
        public CResult Save_MailContent(int ID, int iUID, string sFolder)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIConfigs config = new XIConfigs();
            try
            {
                oTrace.oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "iUID", sValue = iUID.ToString() });
                oTrace.oParams.Add(new CNV { sName = "sFolder", sValue = sFolder.ToString() });
                if (ID > 0 && iUID > 0 && !string.IsNullOrEmpty(sFolder))
                {
                    XIIBO oBOI = new XIIBO();
                    XIDBO oBOD = new XIDBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XILeadImport");
                    oBOI.BOD = oBOD;
                    oCR = Get_EmailFullDetails(ID, iUID, sFolder);
                    oTrace.oTrace.Add(oCR.oTrace);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var sEmailDetails = oCR.oResult.ToString();
                        string[] sDetailsSplit = sEmailDetails.Split(new string[] { "<>" }, StringSplitOptions.None);
                        var sFrom = sDetailsSplit[0];
                        var sSubject = sDetailsSplit[1];
                        var sText = sDetailsSplit[2];
                        var sHtml = sDetailsSplit[3];
                        var sAttachment = sDetailsSplit[4];
                        oBOI.SetAttribute("ID", 0.ToString());
                        oBOI.SetAttribute("FKiOriginID", 2.ToString());
                        oBOI.SetAttribute("FKiSourceID", 2.ToString());
                        oBOI.SetAttribute("FKiClassID", 1.ToString());
                        oBOI.SetAttribute("iType", 30.ToString());
                        oBOI.SetAttribute("sLeadData", sText);
                        oBOI.SetAttribute("sFrom", sFrom);
                        oBOI.SetAttribute("sSubject", sSubject);
                        oCR = oBOI.SaveV2(oBOI);
                        oTrace.oTrace.Add(oCR.oTrace);
                        if (oCR.bOK && oCR.oResult != null)
                        {
                            List<CNV> Params = new List<CNV>();
                           int iLeadImportID = Convert.ToInt32(oBOI.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault());
                            if (iLeadImportID > 0)
                            {
                                Params.Add(new CNV { sName = XIConstant.Param_InstanceID, sValue = iLeadImportID.ToString() });
                                oCR = Import_Lead(Params);
                                oTrace.oTrace.Add(oCR.oTrace);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    string ValidateMessage = string.Empty;
                                    if (ValidateMessages.Count > 0)
                                    {
                                        foreach (var item in ValidateMessages)
                                        {
                                            ValidateMessage = ValidateMessage + item;
                                        }
                                        oBOI.SetAttribute("iStatus", 10.ToString());
                                        oBOI.SetAttribute("sMessage", ValidateMessage);
                                    }
                                    else
                                    {
                                        oBOI.SetAttribute("iStatus", 20.ToString());
                                    }
                                    oCR = oBOI.SaveV2(oBOI);
                                    oTrace.oTrace.Add(oCR.oTrace);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                        oCResult.oResult = "Success";
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                                else
                                {
                                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    oBOI.SetAttribute("iStatus", 20.ToString());
                                    oCR = oBOI.SaveV2(oBOI);
                                    if (oCR.bOK && oCR.oResult != null)
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                                        oCResult.oResult = "Success";
                                    }
                                    else
                                    {
                                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                    }
                }


                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Params missing";
                }

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error while Inserting XILeadImport" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            oTrace.sProcessID = Guid.NewGuid().ToString().Substring(0, 12);
            oTrace.sParentID = "#";
            config.Save_CodeLog(oTrace);
            return oCResult;
        }
        public CResult Get_EmailSubjects(int ID, string sFolder, int OrgID,string Flag)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            XIIXI oIXI = new XIIXI();
            try
            {
                oCR = Get_EmailCredentials(ID);
                oTrace.oTrace.Add(oCR.oTrace);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var Details = (XIIBO)oCR.oResult;
                    string UserName = Details.AttributeI("UserName").sValue;
                    string Password = Details.AttributeI("Password").sValue;
                    string Server = Details.AttributeI("ServerName").sValue;
                    int Port = Convert.ToInt32(Details.AttributeI("Port").sValue);
                    var lListSubjects = new List<string>();
                    string sFromAddress = "";
                    string sFromName = "";
                    using (Imap imap = new Imap())
                    {
                        imap.ConnectSSL(Server, Port);   // or ConnectSSL
                        imap.UseBestLogin(UserName, Password);
                        imap.Select(sFolder);
                        List<long> uids = imap.GetAll();
                        uids = uids.OrderByDescending(m => m).ToList();
                        List<MessageInfo> infos = imap.GetMessageInfoByUID(uids);
                        infos = infos.OrderByDescending(m => m.UID).ToList();
                        if (Flag == "S")
                        {
                            var ScheduleTime = string.Empty;
                            List<CNV> oWhereParams = new List<CNV>();
                            oWhereParams.Add(new CNV { sName = "sKey", sValue = "TIGPWD" });
                            var oBOI = oIXI.BOI("XIConfig_T", null, null, oWhereParams);
                            if (oBOI != null && oBOI.Attributes != null)
                            {
                                ScheduleTime = oBOI.AttributeI("ScheduleTime").sValue;
                            }
                            var Date = DateTime.Now.AddMinutes(Convert.ToDouble("-" + ScheduleTime));
                            infos = infos.Where(a => a.Envelope.Date >= Date && a.Envelope.Date <= DateTime.Now).ToList();
                        }
                        foreach (MessageInfo info in infos)
                        {
                            string uid = info.Envelope.UID.ToString();
                            string subject = info.Envelope.Subject;
                            string sEmailDate = (info.Envelope.Date).Value.ToString();
                            //string sEmailDate=info.Envelope.Date.Value.
                            var dSplitToDateTime = sEmailDate.Split(' ');
                            var dGetDate = dSplitToDateTime[0];
                            foreach (var sAddress in info.Envelope.From)
                            {
                                sFromAddress = sAddress.Address;
                                sFromName = sAddress.Name;
                            }
                            lListSubjects.Add(uid + " <> " + sFolder + " <> " + subject + " <> " + sFromAddress + " <> " + dGetDate + " <> " + ID);
                        }
                        imap.Close();
                    }
                    oCResult.oResult = lListSubjects;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            }
            return oCResult;
        }
        public CResult Select_FoldersWithIMAP(int ID, int OrgID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                oCR = Get_EmailCredentials(ID);
                oTrace.oTrace.Add(oCR.oTrace);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var Details = (XIIBO)oCR.oResult;
                    string UserName = Details.AttributeI("UserName").sValue;
                    string Password = Details.AttributeI("Password").sValue;
                    string Server = Details.AttributeI("ServerName").sValue;
                    int Port = Convert.ToInt32(Details.AttributeI("Port").sValue);
                    var lFolderList = new List<string>();
                    using (Imap imap = new Imap())
                    {
                        imap.ConnectSSL(Server, Port);   // or ConnectSSL
                        imap.UseBestLogin(UserName, Password);
                        //Get list of folders...
                        foreach (FolderInfo folder in imap.GetFolders())
                        {
                            lFolderList.Add(folder.Name);
                        }
                        imap.Close();
                    }
                    oCResult.oResult = lFolderList;
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                }

            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            }
            return oCResult;
        }
        public CResult Validate_Data(string sRegex, string sReslovedValue)
        {

            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "";//expalin about this method logic
            try
            {
                Regex regxppattern = new Regex(sRegex);
                var match = Regex.Match(sReslovedValue, sRegex.Trim());
                if (!match.Success && match.Value != sReslovedValue)
                {
                    oCResult.oResult = true;
                }
            }

            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

    }
}