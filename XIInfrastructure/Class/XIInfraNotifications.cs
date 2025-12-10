using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraNotifications
    {
        public int iOrgID { get; set; }
        public string sSubject { get; set; }
        public string sMessage { get; set; }
        public string sOrgName { get; set; }
        public int iDocumentID { get; set; }
        public string sMailType { get; set; }
        public int iUserID { get; set; }
        public string sIsRead { get; set; }
        public string sIcon { get; set; }
        public string sInstanceID { get; set; }
        public int iStatus { get; set; }
        public string NotifyType { get; set; }
        public CResult SaveNotifications(List<CNV> oParams)
        {
            CResult oCResult = new CResult(); // always
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;
            string sParGUID = string.Empty;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIIBO oBOI = new XIIBO();
                XIDXI oXID = new XIDXI();
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "Notifications", null);
                iOrgID = Convert.ToInt32(oParams.Where(m => m.sName == "sOrgID").Select(m => m.sValue).FirstOrDefault());
                sSubject = oParams.Where(m => m.sName == "sSubject").Select(m => m.sValue).FirstOrDefault();
                sMessage = oParams.Where(m => m.sName == "sMessage").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                iDocumentID = Convert.ToInt32(oParams.Where(m => m.sName == "sDocumentID").Select(m => m.sValue).FirstOrDefault());
                sMailType = oParams.Where(m => m.sName == "sMailType").Select(m => m.sValue).FirstOrDefault();
                sIsRead = oParams.Where(m => m.sName == "IsRead").Select(m => m.sValue).FirstOrDefault();
                sIcon = oParams.Where(m => m.sName == "sIcon").Select(m => m.sValue).FirstOrDefault();
                sInstanceID = oParams.Where(m => m.sName == "iInstanceID").Select(m => m.sValue).FirstOrDefault();
                iStatus = Convert.ToInt32(oParams.Where(m => m.sName == "iStatus").Select(m => m.sValue).FirstOrDefault());

                oBOI.BOD = oBOD;
                oBOI.SetAttribute("ID",null);
                oBOI.SetAttribute("iOrganizationID", iOrgID.ToString());
                oBOI.SetAttribute("sSubject", sSubject);
                oBOI.SetAttribute("sMessage", sMessage);
                oBOI.SetAttribute("dReceivedOn", DateTime.Now.ToString());
                oBOI.SetAttribute("sOrganizationName", sOrgName);
                oBOI.SetAttribute("FKiUserID", iUserID.ToString());
                oBOI.SetAttribute("sAttachments", iDocumentID.ToString());
                oBOI.SetAttribute("sOrganizationName", sOrgName);
                oBOI.SetAttribute("sMailType", sMailType);
                oBOI.SetAttribute("bIsRead", sIsRead);
                oBOI.SetAttribute("sIcon", sIcon);
                oBOI.SetAttribute("iInstanceID", sInstanceID);
                oBOI.SetAttribute("iStatus", iStatus.ToString());

                //oBOI.Attributes["ID"] = new XIIAttribute { sName = "ID", sValue = null, bDirty = true };
                //oBOI.Attributes["iOrganizationID"] = new XIIAttribute { sName = "iOrganizationID", sValue = iOrgID.ToString(), bDirty = true };
                //oBOI.Attributes["sSubject"] = new XIIAttribute { sName = "sSubject", sValue = sSubject, bDirty = true };
                //oBOI.Attributes["sMessage"] = new XIIAttribute { sName = "sMessage", sValue = sMessage, bDirty = true };
                //oBOI.Attributes["dReceivedOn"] = new XIIAttribute { sName = "dReceivedOn", sValue = DateTime.Now.ToString(), bDirty = true };
                //oBOI.Attributes["sOrganizationName"] = new XIIAttribute { sName = "sOrganizationName", sValue = sOrgName, bDirty = true };
                //oBOI.Attributes["FKiUserID"] = new XIIAttribute { sName = "FKiUserID", sValue = iUserID.ToString(), bDirty = true };
                //oBOI.Attributes["sAttachments"] = new XIIAttribute { sName = "sAttachments", sValue = iDocumentID.ToString(), bDirty = true };
                //oBOI.Attributes["sOrganizationName"] = new XIIAttribute { sName = "sOrganizationName", sValue = sOrgName, bDirty = true };
                //oBOI.Attributes["sMailType"] = new XIIAttribute { sName = "sMailType", sValue = sMailType, bDirty = true };
                //oBOI.Attributes["bIsRead"] = new XIIAttribute { sName = "bIsRead", sValue = sIsRead, bDirty = true };
                //oBOI.Attributes["sIcon"] = new XIIAttribute { sName = "sIcon", sValue = sIcon, bDirty = true };
                //oBOI.Attributes["iInstanceID"] = new XIIAttribute { sName = "iInstanceID", sValue = sInstanceID, bDirty = true };

                var Response = oBOI.Save(oBOI);//to save Notifications
                XIIBO oBOInstance = new XIIBO();
                if (Response.bOK && Response.oResult != null)
                {
                    oBOInstance = (XIIBO)Response.oResult;
                }
                var sID = oBOInstance.Attributes.Values.Where(m => m.sName.ToLower() == oBOD.sPrimaryKey.ToLower()).FirstOrDefault().sValue;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                oCResult.oResult = sID;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Saving Notifications" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;

        }
        public CResult Create(string sUser, string sNotificationType, string sDoc)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            string sParGUID = string.Empty;
            //get iTraceLevel from ??somewhere fast - cache against user??
            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = oCResult.Get_MethodName();

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            try
            {
                //CUserInfo oInfo = new CUserInfo();
                //oInfo = oInfo.GetUserInfo();
                List<CNV> oParams = new List<CNV>();
                switch (sNotificationType)
                {
                    case "Document":
                    case "Document Status":
                    case "CLIENTDOC":
                        CNV Params = new CNV();
                        Params.sName = "sOrgID";
                        Params.sValue = iOrgID.ToString();
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sSubject";
                        Params.sValue = "Document recieved";
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sMessage";
                        Params.sValue = "You have recieved new document";
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sDocumentID";
                        Params.sValue = sDoc;
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sOrgName";
                        Params.sValue = sOrgName;
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sMailType";
                        Params.sValue = sNotificationType;
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "iUserID";
                        Params.sValue = sUser;
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "IsRead";
                        Params.sValue = "0";
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "sIcon";
                        Params.sValue = "fa fa-file";
                        oParams.Add(Params);
                        Params = new CNV();
                        Params.sName = "iStatus";
                        Params.sValue = "20";
                        oParams.Add(Params);

                        var oResult = SaveNotifications(oParams);
                        return oResult;

                    case "Document Request":
                        //List<CNV> nParamsList = new List<CNV>();
                        CNV nParams = new CNV();
                        nParams.sName = "sOrgID";
                        nParams.sValue = iOrgID.ToString();
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "sSubject";
                        nParams.sValue = "Document Request";
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "sMessage";
                        nParams.sValue = "Please provide the Proof of no claims bonus";
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "sOrgName";
                        nParams.sValue = sOrgName;
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "sMailType";
                        nParams.sValue = sNotificationType;
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "iUserID";
                        nParams.sValue = sUser;
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "IsRead";
                        nParams.sValue = "0";
                        oParams.Add(nParams);
                        nParams = new CNV();
                        nParams.sName = "sIcon";
                        nParams.sValue = "fa fa-file";
                        oParams.Add(nParams);
                        Params = new CNV();
                        Params.sName = "iStatus";
                        Params.sValue = iStatus.ToString();
                        oParams.Add(nParams);

                        var ocResult = SaveNotifications(oParams);
                        return ocResult;

                    case "Document Recieved":
                        //List<CNV> oParamsList = new List<CNV>();
                        CNV nParam = new CNV();
                        nParam.sName = "sOrgID";
                        nParam.sValue = iOrgID.ToString();
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sSubject";
                        nParam.sValue = "Document Recieved";
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sMessage";
                        nParam.sValue = "Find the document";
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sDocumentID";
                        nParam.sValue = sDoc;
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sOrgName";
                        nParam.sValue = sOrgName;
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sMailType";
                        nParam.sValue = sNotificationType;
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "iUserID";
                        nParam.sValue = sUser;
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "IsRead";
                        nParam.sValue = "0";
                        oParams.Add(nParam);
                        nParam = new CNV();
                        nParam.sName = "sIcon";
                        nParam.sValue = "fa fa-file";
                        oParams.Add(nParam);
                        Params = new CNV();
                        Params.sName = "iStatus";
                        Params.sValue = iStatus.ToString();
                        oParams.Add(nParam);

                        var oRes = SaveNotifications(oParams);
                        return oRes;
                    case "Make a Change":
                    case "Make a Claim":
                        List<CNV> oParamsList = new List<CNV>();
                        CNV nvParam = new CNV();
                        nvParam.sName = "sOrgID";
                        nvParam.sValue = iOrgID.ToString();
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sSubject";
                        nvParam.sValue = sNotificationType;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sMessage";
                        nvParam.sValue = sNotificationType;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sDocumentID";
                        nvParam.sValue = sDoc;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sOrgName";
                        nvParam.sValue = sOrgName;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sMailType";
                        nvParam.sValue = sNotificationType;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "iUserID";
                        nvParam.sValue = sUser;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "IsRead";
                        nvParam.sValue = "0";
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "sIcon";
                        nvParam.sValue = "fa fa-edit";
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "iInstanceID";
                        nvParam.sValue = sInstanceID;
                        oParams.Add(nvParam);
                        nvParam = new CNV();
                        nvParam.sName = "iStatus";
                        nvParam.sValue = iStatus.ToString();
                        oParams.Add(nvParam);

                        var oResponse = SaveNotifications(oParams);
                        return oResponse;

                    default:
                        return null;
                }

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Notification Saving" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
            }
            return oCResult;
        }
    }
}