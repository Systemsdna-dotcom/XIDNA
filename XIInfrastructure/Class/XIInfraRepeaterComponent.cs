using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using XICore;
using XIDatabase;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraRepeaterComponent
    {

        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public int OneClickID { get; set; }
        public string sOrgName { get; set; }
        public string sContext { get; set; }
        public int iSectionInstanceID { get; set; }
        public int iStepInstanceID { get; set; }
        public string AttributeName1 { get; set; }
        public string AttributeValue1 { get; set; }
        public string AttributeName2 { get; set; }
        public string AttributeValue2 { get; set; }
        public string sSection { get; set; }
        public string sDisplayMode { get; set; }
        public string sLockGroup { get; set; }

        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);

        XIInfraCache oCache = new XIInfraCache();

        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> nParamsList = new List<CNV>();
                XIDStructure oStructure = new XIDStructure();
                nParamsList = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();

                sDisplayMode = oParams.Where(m => m.sName == "DisplayMode").Select(m => m.sValue).FirstOrDefault();
                sLockGroup = oParams.Where(m => m.sName.ToLower() == "LockGroup".ToLower()).Select(m => m.sValue).FirstOrDefault();
                var WrapperParms = new List<CNV>();
                var WatchParam = oParams.Where(m => m.sName.ToLower().Contains("watchparam".ToLower())).ToList();
                if (WatchParam.Count() > 0)
                {
                    foreach (var items in WatchParam)
                    {
                        if (!string.IsNullOrEmpty(items.sValue))
                        {
                            var Prams = oCache.Get_Paramobject(sSessionID, sGUID, null, items.sValue); //oParams.Where(m => m.sName == items.sValue).FirstOrDefault();
                            if (Prams != null)
                            {
                                WrapperParms = Prams.nSubParams;
                            }
                        }
                    }
                }
                List<CNV> nParams = new List<CNV>();
                sContext = oParams.Where(m => m.sName == "Context").Select(m => m.sValue).FirstOrDefault();
                sSection = oParams.Where(m => m.sName == "Section").Select(m => m.sValue).FirstOrDefault();
                if (oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault() != null && oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                {
                    if (WrapperParms != null)
                    {
                        OneClickID = Convert.ToInt32(WrapperParms.Where(m => m.sName == "{XIP|i1ClickID}").Select(m => m.sValue).FirstOrDefault());
                    }
                }
                else if (oParams.Where(m => m.sName == "1ClickID").FirstOrDefault() != null)
                {
                    OneClickID = Convert.ToInt32(oParams.Where(m => m.sName == "1ClickID").Select(m => m.sValue).FirstOrDefault());
                }
                else
                {
                    OneClickID = 0;
                }
                AttributeName1 = oParams.Where(m => m.sName == "AttributeName1").Select(m => m.sValue).FirstOrDefault();
                AttributeValue1 = oParams.Where(m => m.sName == "AttributeValue1").Select(m => m.sValue).FirstOrDefault();
                if (AttributeValue1 != null && AttributeValue1.ToLower() == "{qsinstance}")
                {
                    AttributeValue1 = oParams.Where(m => m.sName == "FKiQSInstanceID").Select(m => m.sValue).FirstOrDefault();
                }
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                if (AttributeValue1 != null)
                {
                    nParams.Add(new CNV { sName = AttributeName1, sValue = AttributeValue1 });
                    nParams.Add(new CNV { sName = "Section", sValue = sSection });
                }

                nParams.AddRange(oParams.Where(m => m.sType == "Structure").ToList());
                XID1Click o1ClickD = new XID1Click();
                if (OneClickID > 0)
                {
                    //Get 1-Click Defintion             
                    var oOneClick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString());

                    //Get BO Definition
                    //XIDBO oBOD = new XIDBO();
                    //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, o1ClickD.BOID);
                    //o1ClickD.BOD = oBOD;

                    //Get Headings of 1-Click
                    //o1ClickD.Get_1ClickHeadings();

                    //Poovanna 7/9/2018
                    List<XIBODisplay> olBODisplay = new List<XIBODisplay>();
                    //XIDBO oBOD = new XIDBO();
                    //oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, oOneClick.BOID.ToString());
                    XIDBO oXIBOD = new XIDBO();
                    if (oOneClick.BOIDXIGUID != null && oOneClick.BOIDXIGUID != Guid.Empty)
                    {
                        oXIBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, oOneClick.BOIDXIGUID.ToString());
                    }
                    else if (oOneClick.BOID > 0)
                    {
                        oXIBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, oOneClick.BOID.ToString());
                    }
                    int iCmponentID = oOneClick.FKiComponentID;
                    XIDComponent oXIComponent = new XIDComponent();
                    oXIComponent = (XIDComponent)oCache.GetObjectFromCache(XIConstant.CacheComponent, null, oOneClick.RepeaterComponentID.ToString());
                    var oCompC = (XIDComponent)oXIComponent.Clone(oXIComponent);
                    //Get the group from XIComponentNVs
                    //string sGroupName = oXIComponent.NVs.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
                    XIIXI oXIIXI = new XIIXI();

                    //get DataSource
                    XIDXI oDXI = new XIDXI();
                    List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                    Dictionary<string, XIIBO> oDictionaryBOI = null;
                    oDXI.sCoreDatabase = sCoreDatabase;
                    oDXI.sOrgDatabase = sOrgDatabase;
                    var sDataSource = oDXI.GetBODataSource(oXIBOD.iDataSource.ToString(), oXIBOD.FKiApplicationID);
                    o1ClickD.sConnectionString = sDataSource;
                    o1ClickD.BOID = oOneClick.BOID;
                    o1ClickD.Query = oStructure.ReplaceExpressionWithCacheValue(oOneClick.Query, nParamsList);
                    oDictionaryBOI = o1ClickD.OneClick_Execute();
                    oBOInstanceLIst = oDictionaryBOI.Values.ToList();

                    //var GroupFields = oBOD.Groups[sGroupName.ToLower()].BOFieldNames;
                    //List<string> lGroupFields = GroupFields.Split(',').ToList();

                    if (oBOInstanceLIst.Count == 0)
                    {
                        var sGroupName = oParams.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sGroupName))
                        {
                            var GroupFields = oXIBOD.GroupD(sGroupName.ToLower()).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                            if (!string.IsNullOrEmpty(GroupFields))
                            {
                                var oGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                var attributes = oXIBOD.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.Name.ToLower())).ToList();
                                var attrs = attributes.ToDictionary(i => i.Name, i => new XIIAttribute { sName = i.Name, sValue = null, bDirty = true });
                                XIIBO oBOI = new XIIBO();
                                oBOI.Attributes = attrs;
                                oBOI.BOD = oXIBOD;
                                oBOInstanceLIst.Add(oBOI);
                                // oBODisplay.BOInstance = oBOI;
                            }
                        }
                    }

                    for (var i = 0; i < oBOInstanceLIst.Count(); i++)
                    {
                        XIBODisplay oBODisplay = new XIBODisplay();
                        List<XIDropDown> ImagePathDetails = new List<XIDropDown>();
                        //get BO Def


                        foreach (var items in oXIBOD.Attributes)
                        {
                            string sKey = items.Key;
                            string sKeyLabel = items.Value.LabelName;
                            var sAttrDefValue = items.Value;
                            XIDAttribute oDAttr = new XIDAttribute();
                            oDAttr = oXIBOD.Attributes.Where(m => m.Key == sKey.ToLower()).Select(m => m.Value).FirstOrDefault();
                            if (oDAttr.iMasterDataID > 0)
                            {
                                cConnectionString oConString = new cConnectionString();
                                //string sConString = oConString.ConnectionString();
                                XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
                                Dictionary<string, object> Params = new Dictionary<string, object>();
                                Params["Code"] = oDAttr.iMasterDataID;
                                string sMaster = Connection.Select<XIDMasterData>("XIMasterData_T", Params).FirstOrDefault().Name;
                                Dictionary<string, object> MasterParams = new Dictionary<string, object>();
                                MasterParams["Name"] = sMaster;
                                List<XIDMasterData> oXIMaster = new List<XIDMasterData>();
                                oXIMaster = Connection.Select<XIDMasterData>("XIMasterData_T", Params).ToList();
                                var oXIDMasterDDL = oXIMaster.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.Name }).ToList();
                                oXIDMasterDDL.Insert(0, new XIDropDown
                                {
                                    text = "--Select--",
                                    Value = 0
                                });
                                sAttrDefValue.FieldDDL = oXIDMasterDDL;
                                //FieldDDL = dbContext.Types.Where(m => m.Code == items.iMasterDataID).ToList().Select(m => new VMDropDown { ID = m.ID, Expression = m.Expression }).ToList();
                                ////    oBoInstnce.Definition.BOFields.Where(m => m.ID == items.ID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
                                //}
                            }
                            else if (oDAttr.IsOptionList == true)
                            {
                                Dictionary<string, object> Params = new Dictionary<string, object>();
                                Params["BOID"] = oDAttr.BOID;
                                Params["BOFieldID"] = oDAttr.ID;
                                List<XIDOptionList> oXIOption = new List<XIDOptionList>();
                                oXIOption = Connection.Select<XIDOptionList>("XIBOOptionList_T_N", Params).ToList();
                                var oXIDOptionDDL = oXIOption.Select(m => new XIDropDown { Value = Convert.ToInt32(m.ID), text = m.sOptionName }).ToList();
                                oXIDOptionDDL.Insert(0, new XIDropDown
                                {
                                    text = "--Select--",
                                    Value = 0
                                });
                                sAttrDefValue.FieldDDL = oXIDOptionDDL;
                                // var IsOptionalList = oDAttr.OptionList.ToList();
                                //foreach (var item in IsOptionalList)
                                //{
                                //    FieldDDL = dbContext.BOOptionLists.Where(m => m.BOID == item.BOID).Where(m => m.BOFieldID == item.ID).Where(m => m.Name == item.Name).ToList().Select(m => new VMDropDown { Type = m.sValues, Expression = m.sOptionName }).ToList();
                                //    oBoInstnce.Definition.BOFields.Where(m => m.ID == item.ID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
                                //}
                            }
                            else if (oDAttr.FKiType > 0 && oDAttr.sFKBOName != null)
                            {
                                //Dictionary<string, object> Params = new Dictionary<string, object>();
                                //string sBODataSource = string.Empty;
                                //var FKList = oXIBOD.Attributes.Values.Where(m => m.FKiType > 0 && (!string.IsNullOrEmpty(m.FKTableName))).ToList();
                                //foreach (var item in FKList)
                                //{
                                //    var sTableName = item.FKTableName;
                                //    var AllBOs = Connection.Select<XIDBO>("XIBO_T_N", Params).ToList();
                                //    var BO = AllBOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                                //    sBODataSource = oDXI.GetBODataSource(BO.iDataSource);
                                //    SqlConnection Con = new SqlConnection(sBODataSource);
                                //    Con.Open();
                                //    SqlCommand cmd = new SqlCommand();
                                //    cmd.Connection = Con;
                                //    if (BO != null)
                                //    {
                                //        var LabelGroup = oXIBOD.Groups.ToList().Select(m => m.Value).Select(m => m.BOFieldNames).FirstOrDefault();
                                //        cmd.CommandText = "Select * From " + sTableName;
                                //        SqlDataReader reader = cmd.ExecuteReader();
                                //        List<XIDropDown> FKDDL = new List<XIDropDown>();
                                //        while (reader.Read())
                                //        {
                                //            if (reader.FieldCount > 1)
                                //            {
                                //                FKDDL.Add(new XIDropDown
                                //                {
                                //                    text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                                //                    Expression = reader.IsDBNull(1) ? null : reader.GetValue(2).ToString()
                                //                });
                                //            }
                                //            else if (reader.FieldCount > 0)
                                //            {
                                //                FKDDL.Add(new XIDropDown
                                //                {
                                //                    text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                                //                });
                                //            }
                                //        }
                                //        Con.Close();
                                //        sAttrDefValue.FieldDDL = FKDDL;
                                //        oXIBOD.Attributes.Values.Where(m => m.ID == item.ID).FirstOrDefault().sFKBOSize = BO.sSize;
                                //        oXIBOD.Attributes.Values.Where(m => m.ID == item.ID).Select(m => { m.FieldDDL = FKDDL; return m; }).ToList();
                                //    }
                                //}
                            }
                            else if (oDAttr.FKiFileTypeID > 0)
                            {
                                var sFileID = string.Empty;
                                var ImageData = oXIBOD.Attributes.Values.Where(m => m.FKiFileTypeID > 0).ToList();
                                var sImageName = ImageData.ToList().Select(m => m.Name).FirstOrDefault();
                                sFileID = oBOInstanceLIst[i].Attributes.ToList().Where(m => m.Key.ToLower() == sImageName.ToLower()).Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                if (sFileID == null)
                                {
                                    var sLabelName = ImageData.ToList().Select(m => m.LabelName).FirstOrDefault();
                                    sFileID = oBOInstanceLIst[i].Attributes.ToList().Where(m => m.Key == sLabelName).Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                }
                                if (!string.IsNullOrEmpty(sFileID))
                                {
                                    var NewFileID = sFileID.Split(',').Select(Int32.Parse).ToList();
                                    foreach (var item in NewFileID)
                                    {
                                        if (!string.IsNullOrEmpty(item.ToString()))
                                        {
                                            XIInfraDocs oDocs = new XIInfraDocs();
                                            oDocs.ID = item;
                                            var sImagePathDetails = (List<XIDropDown>)oDocs.Get_FilePathDetails().oResult;
                                            if (sImagePathDetails != null && sImagePathDetails.Count() > 0)
                                            {
                                                ImagePathDetails.AddRange(sImagePathDetails);
                                            }
                                        }
                                    }
                                }
                                //oXIBOD.Attributes.Values.Where(x => x.Name == items.Value.Name || x.LabelName == items.Value.Name).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                                oBOInstanceLIst[i].Attributes.Values.Where(x => x.sName == items.Value.Name).ToList().ForEach(x => x.ImagePathDetails = ImagePathDetails);
                            }
                            else if (oDAttr.iOneClickID > 0)
                            {
                                var OneClicks = oXIBOD.Attributes.Values.Where(m => m.iOneClickID > 0).ToList();
                                foreach (var OneClick in OneClicks)
                                {
                                    var iOneClickID = OneClick.iOneClickID;
                                    if (iOneClickID > 0 || (OneClick.iOneClickIDXIGUID != null && OneClick.iOneClickIDXIGUID != Guid.Empty))
                                    {
                                        XID1Click OneClickDef = new XID1Click();
                                        if (OneClick.iOneClickIDXIGUID != null && OneClick.iOneClickIDXIGUID != Guid.Empty)
                                        {
                                            OneClickDef = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClick.iOneClickIDXIGUID.ToString());
                                        }
                                        else
                                        {
                                            OneClickDef = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, iOneClickID.ToString());
                                        }                                        
                                        if(OneClickDef.BOIDXIGUID != null && OneClickDef.BOIDXIGUID != Guid.Empty)
                                        {
                                            oXIBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, OneClickDef.BOIDXIGUID.ToString());
                                        }
                                        else if (OneClickDef.BOID > 0)
                                        {
                                            oXIBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, OneClickDef.BOID.ToString());
                                        }                                        
                                        var sBODataSource = string.Empty;
                                        sBODataSource = oDXI.GetBODataSource(oXIBOD.iDataSource.ToString(),oXIBOD.FKiApplicationID);
                                        List<XIDropDown> FKDDL = new List<XIDropDown>();
                                        using (SqlConnection Con = new SqlConnection(sBODataSource))
                                        {
                                            Con.Open();
                                            SqlCommand cmd = new SqlCommand();
                                            cmd.Connection = Con;
                                            cmd.CommandText = OneClickDef.Query;
                                            SqlDataReader reader = cmd.ExecuteReader();
                                            while (reader.Read())
                                            {
                                                if (reader.FieldCount > 1)
                                                {
                                                    FKDDL.Add(new XIDropDown
                                                    {
                                                        text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                                                        Expression = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString()
                                                    });
                                                }
                                                else if (reader.FieldCount > 0)
                                                {
                                                    FKDDL.Add(new XIDropDown
                                                    {
                                                        text = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString(),
                                                    });
                                                }
                                            }
                                            Con.Close();
                                        }
                                        sAttrDefValue.FieldDDL = FKDDL;
                                        oXIBOD.Attributes.Values.Where(m => m.iOneClickID == iOneClickID).Select(m => { m.FieldDDL = FKDDL; return m; }).ToList();
                                    }
                                }
                            }
                            oXIBOD.Attributes.Where(m => m.Key == sKey).Select(m => m.Value == sAttrDefValue);
                        }
                        oBOInstanceLIst[i].Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                        oBOInstanceLIst[i].BOD = oXIBOD;
                        oBODisplay.BOInstance = oBOInstanceLIst[i];
                        if (!string.IsNullOrEmpty(sLockGroup))
                        {
                            string sPrimaryKey = string.Empty;
                            sPrimaryKey = oXIBOD.sPrimaryKey;
                            var GroupFields = oXIBOD.GroupD(sLockGroup).BOFieldNames;//oXIAPI.GetBOGroupFields(sBOName, sLockGroup, iUserID, sDatabase);
                            if (!string.IsNullOrEmpty(GroupFields))
                            {
                                var oGrpFields = GroupFields.ToLower().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                                oBODisplay.BOInstance.Attributes.Values.Where(m => oGrpFields.Any(n => n == m.sName.ToLower())).ToList().ForEach(c => c.bLock = true);
                                oBODisplay.BOInstance.Attributes.Values.Where(m => m.sName == sPrimaryKey).ToList().ForEach(m => m.bLock = true);
                            }
                        }
                        //sDisplayMode = "Horizontal";
                        if (!string.IsNullOrEmpty(sDisplayMode))
                        {
                            XIVisualisation oVis = new XIVisualisation();
                            oVis.Name = "Mode";
                            XIVisualisationNV NV = new XIVisualisationNV();
                            NV.sName = "DisplayMode";
                            NV.sValue = sDisplayMode;
                            oVis.XiVisualisationNVs = new List<XIVisualisationNV>();
                            oVis.XiVisualisationNVs.Add(NV);
                            oBODisplay.Visualisations = new List<XIVisualisation>();
                            oBODisplay.Visualisations.Add(oVis);
                        }
                        olBODisplay.Add(oBODisplay);
                    }
                    o1ClickD.BOD = new XIDBO();
                    o1ClickD.BOD.Name = oXIBOD.Name;
                    o1ClickD.XIBODisplay = olBODisplay;
                    oCompC.nParams = oParams;
                    o1ClickD.XIComponent = oCompC;
                    o1ClickD.RepeaterComponentID = oOneClick.RepeaterComponentID;

                    //Result = oXIRepo.GetHeadingsForList(OneClickID, null, sDatabase, 0, iUserID, sOrgName);
                    o1ClickD.sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sDisplayMode))
                    {
                        o1ClickD.ActionType = "View";
                    }
                }
                o1ClickD.ID = OneClickID;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = o1ClickD;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Form Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}