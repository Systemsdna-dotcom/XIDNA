using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraQuestionSetComponent:XIDefinitionBase
    {
        public int iBODID { get; set; }
        public string sMode { get; set; }
        public string sStructureCode { get; set; }
        public string sQSName { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        //XIComponentsRepository oXIComRepo = new XIComponentsRepository();
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
                XIInfraCache oCache = new XIInfraCache();
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                var sParentGUID = oCache.Get_ParamVal(sSessionID, sGUID, null, "|XIParent");
                //if (!string.IsNullOrEmpty(sParentGUID))
                //{
                //    sGUID = sParentGUID;
                //}
                iBODID = Convert.ToInt32(oParams.Where(m => m.sName == "BODID").Select(m => m.sValue).FirstOrDefault());
                if (iBODID == 0)
                {
                    //XIInfraCache oCache = new XIInfraCache();
                    iBODID = Convert.ToInt32(oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}"));
                    //iBODID = Convert.ToInt32(oXIAPI.Get_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}"));
                }
                sQSName = oParams.Where(m => m.sName == "QSFixedTemplate").Select(m => m.sValue).FirstOrDefault();
                sMode = oParams.Where(m => m.sName == "Mode").Select(m => m.sValue).FirstOrDefault();
                sStructureCode = oParams.Where(m => m.sName == "StructureCode").Select(m => m.sValue).FirstOrDefault();
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgName = oParams.Where(m => m.sName == "sOrgName").Select(m => m.sValue).FirstOrDefault();
                XIDQS oQSD = new XIDQS();
                var oQSDefintion = oQSD.GetQuestionSetComponent(iBODID, sStructureCode, sMode, sQSName, iUserID, sOrgName, sDatabase);
                oCResult.oResult = oQSDefintion;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;

            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Question Set Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            
            return oCResult;
        }
    }
}