using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;
using System.Net.Mail;
using System.IO;
using XIDatabase;
using System.Configuration;

namespace XIInfrastructure
{
    public class XIInfraDocumentComponent : XIDefinitionBase
    {
        public long iInstanceID;
        public string sDocumentType { get; set; }
        public string sBOName { get; set; }
        public string sStructureName { get; set; }
        public int iDocumentID { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCoreDatabase { get; set; }
        public int iUserID { get; set; }
        public int iOrgID { get; set; }
        public string sOutput { get; set; }
        public string sSubject { get; set; }
        public int iAddonID { get; set; }

        public XIBOInstance oXIBOInstance { get; set; }

        XIInfraCache oCache = new XIInfraCache();
        XIContentEditors oXIContent = new XIContentEditors();
        XIInfraDocs oXIDocs = new XIInfraDocs();
        XIDXI oXIDX = new XIDXI();
        XIInfraNotifications oNotifications = new XIInfraNotifications();
        XIDBAPI Connection = new XIDBAPI(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
        public CResult XILoad(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            List<CNV> oNVPairsList = new List<CNV>();
            CNV oNVPairs = new CNV();
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            try
            {
                XIInfraEmail oEmail = new XIInfraEmail();
                XIIQS oXIQS = new XIIQS();
                XIIXI oIXI = new XIIXI();
                object oStructureresult = ""; int iProductID = 0; string sEmail = string.Empty; string sUserID = string.Empty; int iBOID = 0; string sProductName = string.Empty;
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sBOName = oParams.Where(m => m.sName == "Object").Select(m => m.sValue).FirstOrDefault();
                //iPolicyID = Convert.ToInt32(oParams.Where(m => m.sName == "PolicyID").Select(m => m.sValue).FirstOrDefault());
                sDocumentType = oParams.Where(m => m.sName == "Document Type").Select(m => m.sValue).FirstOrDefault();
                sStructureName = oParams.Where(m => m.sName == "Structure Name").Select(m => m.sValue).FirstOrDefault();
                iDocumentID = Convert.ToInt32(oParams.Where(m => m.sName == "DocumentID").Select(m => m.sValue).FirstOrDefault());
                //sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                //sCoreDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                iOrgID = Convert.ToInt32(oParams.Where(m => m.sName == "iOrganizationID").Select(m => m.sValue).FirstOrDefault());
                sOutput = oParams.Where(m => m.sName == "Output").Select(m => m.sValue).FirstOrDefault();
                sSubject = oParams.Where(m => m.sName == "Subject").Select(m => m.sValue).FirstOrDefault();
                iAddonID = Convert.ToInt32(oParams.Where(m => m.sName == "iAddonID").Select(m => m.sValue).FirstOrDefault());
                //var sInstanceID = oParams.Where(m => m.sName.ToLower() == "PolicyID".ToLower()).Select(m => m.sValue).FirstOrDefault();

                var sInstanceID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{-iInstanceID}");

                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);

                long.TryParse(sInstanceID, out iInstanceID);
                if (iInstanceID == 0)
                {
                    iInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault());
                }
                //var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}");
                //var Prm = "{XIP|" + ActiveBO + ".id}";
                //var XIPBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                //if (XIPBOIID != null)
                //{
                //    iInstanceID = Convert.ToInt32(XIPBOIID);
                //}
                //else
                //{
                //    iInstanceID = 0;
                //}
                //if (sBOName == null && !(string.IsNullOrEmpty(ActiveBO)))
                //{
                //    sBOName = ActiveBO;
                //}

                var oDocContent = (List<XIContentEditors>)oCache.GetObjectFromCache(XIConstant.CacheTemplate, sDocumentType, iDocumentID.ToString()); //oXIDX.Get_ContentDefinition(iDocumentID, sDocumentType);
                //var oInstance = oXIQS.QSI(oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).oSubStruct("Policy Version").oSubStruct("QS Instance"));
                //if(oContentDef.Category==1)
                var oContentDef = oDocContent.FirstOrDefault();
                //{
                if (oContentDef != null)
                {
                int iMainTemplateID = oContentDef.ID;
                var oLIst = oIXI.BOI(sBOName, iInstanceID.ToString());
                iBOID = oLIst.BOD.BOID;
                XIIBO oDocumentTemplate = new XIIBO();
                //if (string.IsNullOrEmpty(sSubject))
                //{
                if (oLIst != null && oLIst.Attributes.ContainsKey("FKiProductID"))
                {
                    sEmail = oLIst.Attributes.Where(x => x.Key.ToLower() == "sEmail".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    sUserID = oLIst.Attributes.Where(x => x.Key.ToLower() == "FKiUserID".ToLower()).Select(x => x.Value.sValue).FirstOrDefault();
                    iProductID = Convert.ToInt32(oLIst.Attributes["FKiProductID"].sValue);
                    sProductName = oLIst.AttributeI("FKiProductID").ResolveFK("display");
                    if (iProductID != 0)
                    {
                        List<CNV> oNVParams = new List<CNV>();
                        CNV oCNV = new CNV();
                        if (iAddonID != 0)
                        {
                            oCNV.sName = "FKiProductAddonID";
                            oCNV.sValue = iAddonID.ToString();
                            oNVParams.Add(oCNV);
                        }
                        else
                        {
                            oCNV.sName = "FKiProductID";
                            oCNV.sValue = iProductID.ToString();
                            oNVParams.Add(oCNV);
                        }
                        oCNV = new CNV();
                        oCNV.sName = "FKiTemplateID";
                        oCNV.sValue = iMainTemplateID.ToString();
                        oNVParams.Add(oCNV);
                        oDocumentTemplate = oIXI.BOI("DocumentTemplate", null, null, oNVParams);
                        if (oDocumentTemplate != null)
                        {
                            if (oDocumentTemplate.Attributes.ContainsKey("sDefaultSubject"))
                            {
                                sSubject = oDocumentTemplate.Attributes["sDefaultSubject"].sValue;
                            }
                        }
                    }
                }
                //}
                if (oDocumentTemplate != null)
                {
                    //var oInstance = oIXI.BOI(sBOName, Convert.ToString(iPolicyID)).Structure(sStructureName).XILoad();
                    XIBOInstance oInstance = new XIBOInstance();
                    if (oXIBOInstance == null)
                    {
                        oInstance = oLIst.Structure(sStructureName).XILoad();
                    }
                    else
                    {
                        oInstance = oXIBOInstance;
                    }
                    //if (oContentDef != null)
                    //{
                        XIContentEditors oDocumentContent = oContentDef;
                        oXIContent.sSessionID = sSessionID;
                        oXIContent.FKiProductID = iProductID;
                        if (oContentDef.Category == 1)//Email with Attachments
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oDocumentContent.Name + " Template merging Started" });
                        //var sStructureresult = oXIContent.MergeTemplateContent(oDocumentContent, oInstance);//Get template content with dynamic data
                        var sStructureresult = oXIContent.MergeContentTemplate(oDocumentContent, oInstance);//Get template content with dynamic data
                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oDocumentContent.Name + " Template merging Completed" });

                            if (sStructureresult.bOK && sStructureresult.oResult != null)
                            {
                                string sContent = (string)sStructureresult.oResult;

                                XIBOInstance oStructureI = new XIBOInstance();
                                oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                //var oPolicyInfo = oStructureI.oSubStructureI("QS Instance");
                                var oPolicyInfo = oStructureI.oSubStructureI("ACPolicy_T");
                                if (oPolicyInfo != null && oPolicyInfo.oBOIList != null)
                                {
                                    SavetoEDITransaction(iProductID, iBOID, oDocumentContent, sContent, iInstanceID.ToString());
                                }
                                //if (oContentDef.Category == 1)//Email with Attachments
                              //{
                                if (oContentDef.bIsHavingAttachments)
                                {
                                    var oAttachmnetContentDef = oDocContent.Where(x => x.ID != iMainTemplateID).ToList();
                                    List<Attachment> oAttachments = new List<Attachment>();
                                    foreach (var oContentD in oAttachmnetContentDef)
                                    {
                                        string sTemplateContent = oContentD.Content; int iTemplateType = oContentD.Category;
                                        if (iTemplateType != 0)
                                        {
                                            switch (iTemplateType)
                                            {
                                                case 30://pdf
                                                    if (!string.IsNullOrEmpty(sTemplateContent))
                                                    {
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = ""+ oContentD.Name + " Template merging Started" });
                                                        //var sMergeTempResult1 = oXIContent.MergeTemplateContent(oContentD, oInstance);//Get template content with dynamic data
                                                        var sMergeTempResult = oXIContent.MergeContentTemplate(oContentD, oInstance);//Get template content with dynamic data
                                                        oCResult.oTraceStack.Add(new CNV { sName = "Template merging", sValue = "" + oContentD.Name + " Template merging Completed" });
                                                        if (sMergeTempResult.bOK && sMergeTempResult.oResult != null)
                                                        {
                                                            string sMergeTempContent = (string)sMergeTempResult.oResult;
                                                            if (!string.IsNullOrEmpty(sMergeTempContent))
                                                            {
                                                                SavetoEDITransaction(iProductID, iBOID, oContentD, sMergeTempContent, iInstanceID.ToString());
                                                                Dictionary<string, object> AttrNvs = new Dictionary<string, object>();
                                                                AttrNvs["BOFieldID"] = 17609;
                                                                AttrNvs["sValues"] = oContentD.iSurNamePasswordRange;
                                                                //AttrNvs["StatusTypeID"] = "10";
                                                                var optionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();
                                                                string sSurNamePasswordRange = string.Empty;
                                                                if (optionList != null)
                                                                {
                                                                    sSurNamePasswordRange = optionList.sOptionName;
                                                                }

                                                                AttrNvs = new Dictionary<string, object>();
                                                                AttrNvs["BOFieldID"] = 17610;
                                                                AttrNvs["sValues"] = oContentD.iDOBPasswordRange;
                                                                //AttrNvs["StatusTypeID"] = "10";
                                                                var oOptionList = Connection.Select<XIDOptionList>("XIBOOptionList_T_N ", AttrNvs).FirstOrDefault();
                                                                string sDOBPasswordRange = string.Empty;
                                                                if (oOptionList != null)
                                                                {
                                                                    sDOBPasswordRange = oOptionList.sOptionName;
                                                                }

                                                                if (!string.IsNullOrEmpty(sSurNamePasswordRange))
                                                                {
                                                                    oEmail.sSurName = oInstance.oSubStructureI("QS Instance").Item(0).XIIValue("sLastName").sResolvedValue;
                                                                }
                                                                if (!string.IsNullOrEmpty(sDOBPasswordRange))
                                                                {
                                                                    oEmail.sDOB = oInstance.oSubStructureI("Driver_T").Item(0).AttributeI("dDOB").sResolvedValue;
                                                                }
                                                                CResult oResult = new CResult();
                                                                if(oContentD.iTypeofPDF==20)//using ironPDF
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oDocumentContent.Name + " PDF generation Started" });
                                                                    oResult = oEmail.IronPdf(sMergeTempContent, oContentD.sCSSFileName, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "ironPDF Generation", sValue = "" + oDocumentContent.Name + " PDF generated" });
                                                                }
                                                                else//using iTextsharp
                                                                {
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oDocumentContent.Name + " PDF generation Started" });
                                                                    oResult = oEmail.PDFGenerate(sMergeTempContent, oContentD.bIsPaswordProtected, sSurNamePasswordRange, sDOBPasswordRange); //pdf generation
                                                                    oCResult.oTraceStack.Add(new CNV { sName = "iTextsharpPDF Generation", sValue = "" + oDocumentContent.Name + " PDF generated" });
                                                                }
                                                                if (oResult.bOK && oResult.oResult != null)
                                                                {
                                                                    string sDocumentName = oContentD.Name;
                                                                    oEmail.sDocumentName = sDocumentName;
                                                                    var oAttachment = oEmail.GeneratePDFFile((MemoryStream)oResult.oResult);
                                                                    if (oAttachment.bOK && oAttachment.oResult != null)
                                                                    {
                                                                        Attachment data = (Attachment)oAttachment.oResult;
                                                                        MemoryStream file = (MemoryStream)oResult.oResult;
                                                                        oAttachments.Add(data);
                                                                        oXIDocs.FKiUserID = Convert.ToInt32(sUserID);
                                                                        oXIDocs.sOrgName = sOrgName;
                                                                        oXIDocs.iOrgID = iOrgID;
                                                                        oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                                                                        var oResponse = oXIDocs.SaveDocuments(file, data.Name);//save documents to folder
                                                                        if (oResponse.bOK && oResponse.oResult != null)
                                                                        {
                                                                            string sDocID = (string)oResponse.oResult;
                                                                            oNotifications.iDocumentID = Convert.ToInt32(sDocID);
                                                                            oCResult.oTraceStack.Add(new CNV { sName = "Document saving", sValue = "" + oDocumentContent.Name + " Document saved successfully,DocID:"+ sDocID + "" });
                                                                        }
                                                                        oNotifications.iUserID = Convert.ToInt32(sUserID);
                                                                        oNotifications.iStatus = 10;
                                                                        oNotifications.sOrgName = sOrgName;
                                                                        oNotifications.iOrgID = iOrgID;
                                                                        //Dan Explained
                                                                        //oxii.xiinfra.coms.notifications.create("CLIENTDOC", sDocID)
                                                                        oNotifications.Create(sUserID, "CLIENTDOC", oNotifications.iDocumentID.ToString());
                                                                    }
                                                                }

                                                            }
                                                        }

                                                    }
                                                    break;
                                                case 40://Nonmerged Document
                                                    var oCResponse = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentD.Name, sProductName);
                                                    if (oCResponse.bOK && oCResponse.oResult != null)
                                                    {
                                                        var oRes = (MemoryStream)oCResponse.oResult;
                                                        string sFileName = oContentD.Name + ".pdf";
                                                        oAttachments.Add(new Attachment(new MemoryStream(oRes.ToArray()), sFileName));
                                                    }
                                                    break;
                                            }
                                        }
                                    }

                                    if (sContent.Contains("<html"))
                                    {
                                        switch (oContentDef.Category)
                                        {
                                            case 1:
                                                oEmail.EmailID = sEmail;
                                                oEmail.sSubject = sSubject;
                                                oCResult.oTraceStack.Add(new CNV { sName = "Mail Sending started", sValue = "Sending mail started, email:" + sEmail + "" });
                                                oEmail.Sendmail(iOrgID, sContent, oAttachments);//send mail with attachment
                                                oCResult.oTraceStack.Add(new CNV { sName = "Mail send successfully", sValue = "Mail send successfully, email:" + sEmail + "" });
                                                break;
                                        }
                                    }
                                }
                           // }
                            
                        }
                        }
                        else if (oContentDef.Category == 20)//EDI
                        {
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oDocumentContent.Name + " Template merging Started" });
                            //var sStructureresult = oXIContent.MergeTemplateContent(oDocumentContent, oInstance);//Get template content with dynamic data
                            var sStructureresult = oXIContent.MergeContentTemplate(oDocumentContent, oInstance);//Get template content with dynamic data
                            oCResult.oTraceStack.Add(new CNV { sName = "EDITemplate merging", sValue = "" + oDocumentContent.Name + " Template merging Completed" });

                            if (sStructureresult.bOK && sStructureresult.oResult != null)
                            {
                                string sContent = (string)sStructureresult.oResult;

                                XIBOInstance oStructureI = new XIBOInstance();
                                oStructureI.oStructureInstance = oInstance.oStructureInstance;
                                //var oPolicyInfo = oStructureI.oSubStructureI("QS Instance");
                                var oPolicyInfo = oStructureI.oSubStructureI("ACPolicy_T");
                                if (oPolicyInfo != null && oPolicyInfo.oBOIList != null)
                                {
                                    SavetoEDITransaction(iProductID, iBOID, oDocumentContent, sContent, iInstanceID.ToString());
                                }
                            }
                            }
                        else if (oContentDef.Category == 40)
                        {
                            var oCResponse = SaveProductNonMergedDocuments(Convert.ToInt32(iInstanceID), Convert.ToInt32(sUserID), sOrgName, iOrgID, oContentDef.Name, sProductName);
                        }
                        // }
                    }
                }
                //}
              
            }
            catch (Exception ex)
            {
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                SaveErrortoDB(oCResult);
                oCResult.LogToFile();
            }
            return oCResult;
        }

        private void SavetoEDITransaction(int iProductID, int iBOID, XIContentEditors oDocumentContent, string sContent, string sInstanceID)
        {
            XIIBO oBOI = new XIIBO();
            XIDXI oXID = new XIDXI();
            XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "EDI_Transaction"); //oXID.Get_BODefinition("EDI_Transaction").oResult;
            oBOI.BOD = oBOD;
            oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
            oBOI.Attributes["sMergedText"] = new XIIAttribute { sName = "sMergedText", sValue = sContent.Replace("'", "''"), bDirty = true };
            oBOI.Attributes["FkiBOID"] = new XIIAttribute { sName = "FkiBOID", sValue = iBOID.ToString(), bDirty = true };
            oBOI.Attributes["FkiTemplateID"] = new XIIAttribute { sName = "FkiTemplateID", sValue = oDocumentContent.ID.ToString(), bDirty = true };
            oBOI.Attributes["iStatus"] = new XIIAttribute { sName = "iStatus", sValue = "0", bDirty = true };
            oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };
            oBOI.Attributes["iType"] = new XIIAttribute { sName = "iType", sValue = oDocumentContent.Category.ToString(), bDirty = true };
            oBOI.Attributes["FKiProductID"] = new XIIAttribute { sName = "FKiProductID", sValue = iProductID.ToString(), bDirty = true };
            if (!string.IsNullOrEmpty(oDocumentContent.sTemplateHeader))
            {
                oBOI.Attributes["sMergedTextHeader"] = new XIIAttribute { sName = "sMergedTextHeader", sValue = oDocumentContent.sTemplateHeader, bDirty = true };
            }
            oBOI.Attributes["FKiAddonID"] = new XIIAttribute { sName = "FKiAddonID", sValue = iAddonID.ToString(), bDirty = true };
            var response = oBOI.Save(oBOI);
        }
        public CResult SaveProductNonMergedDocuments(int iInstanceID, int iUserID, string sOrgName, int iOrgID, string sCopyDocumentName,string sProductName)
        {
            CResult oCResult = new CResult(); string sInstanceID = string.Empty;
            try
            {
                XIInfraDocs oXIDocs = new XIInfraDocs();
                string sCopyDocumentPath = string.Empty;
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\ProductNonMergedDocuments\\"+sProductName+"";
                string[] filePaths = Directory.GetFiles(@"" + sPath + "");
                foreach (var item in filePaths)
                {
                    int ipos = item.LastIndexOf("\\") + 1;
                    string sFileName = item.Substring(ipos, item.Length - ipos);
                    if (sCopyDocumentName == sFileName.Split('.')[0])
                    {
                        sCopyDocumentPath = item;
                    }
                }
                int pos = sCopyDocumentPath.LastIndexOf("\\") + 1;
                string sCopyDocFileName = sCopyDocumentPath.Substring(pos, sCopyDocumentPath.Length - pos);
                //if(sCopyDocFileName!="" && File.Exists(sCopyDocumentPath))
                if (!string.IsNullOrEmpty(sCopyDocFileName))
                {
                    FileStream fs = new FileStream(sCopyDocumentPath, FileMode.Open, FileAccess.Read);
                    byte[] tmpBytes = new byte[fs.Length];
                    fs.Read(tmpBytes, 0, Convert.ToInt32(fs.Length));
                    MemoryStream mystream = new MemoryStream(tmpBytes);
                    StreamReader reader = new StreamReader(mystream);
                    oXIDocs.iInstanceID = iInstanceID;
                    oXIDocs.FKiUserID = iUserID;
                    oXIDocs.sOrgName = sOrgName;
                    oXIDocs.iOrgID = iOrgID;
                    oXIDocs.iInstanceID = Convert.ToInt32(iInstanceID);
                    var oResponse = oXIDocs.SaveDocuments(mystream, sCopyDocFileName);//save documents to folder
                    if (oResponse.bOK && oResponse.oResult!=null)
                    {
                        sInstanceID = (string)oResponse.oResult;
                        oCResult.oTraceStack.Add(new CNV { sName = "Non-Merging Document saving", sValue = "" + sCopyDocFileName + " Document saved successfully,DocID:" + sInstanceID + "" });
                    }
                    oCResult.oResult = mystream;
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Non-Merging Documents" });
                oCResult.sMessage = "ERROR: [ Copy Documenst:" + sCopyDocumentName + "-" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}