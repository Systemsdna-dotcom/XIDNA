using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraStepComponent : XIDefinitionBase
    {
        public int iQSDID { get; set; }
        public int iStepDID { get; set; }
        public string sStepName { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCurrentUserGUID { get; set; }
        public string sGUID { get; set; }

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
                if (oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).FirstOrDefault() != null)
                {
                    if (oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                    {
                        iStepDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "{XIP|iStepDID}".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    }
                    else
                    {
                        iStepDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "iStepDID".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    }
                }
                if (oParams.Where(m => m.sName.ToLower() == "sStepName".ToLower()).FirstOrDefault() != null)
                {
                    if (oParams.Where(m => m.sName.ToLower() == "sStepName".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                    {
                        sStepName = oParams.Where(m => m.sName.ToLower() == "{XIP|sStepName}".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    }
                    else
                    {
                        sStepName = oParams.Where(m => m.sName.ToLower() == "sStepName".ToLower()).Select(m => m.sValue).FirstOrDefault();
                    }
                }
                if (oParams.Where(m => m.sName.ToLower() == "iQSDID".ToLower()).FirstOrDefault() != null)
                {
                    if (oParams.Where(m => m.sName.ToLower() == "iQSDID".ToLower()).Select(m => m.sValue).FirstOrDefault().StartsWith("{XIP|"))
                    {
                        iQSDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "{XIP|iQSDID}".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    }
                    else
                    {
                        iQSDID = Convert.ToInt32(oParams.Where(m => m.sName.ToLower() == "iQSDID".ToLower()).Select(m => m.sValue).FirstOrDefault());
                    }
                }
                //iStepDID = Convert.ToInt32(oParams.Where(m => m.sName == "iStepDID").Select(m => m.sValue).FirstOrDefault());
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sCurrentUserGUID = oParams.Where(m => m.sName == "sCurrentUserGUID").Select(m => m.sValue).FirstOrDefault();
                XIDQSStep oQSStep = new XIDQSStep();
                if (iStepDID > 0 || !string.IsNullOrEmpty(sStepName))
                {
                    if (iQSDID > 0)
                    {
                        XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID.ToString());
                        if (iStepDID > 0)
                        {
                            oQSStep = oQSD.Steps.Values.Where(m => m.ID == iStepDID).FirstOrDefault();
                        }
                        else if (!string.IsNullOrEmpty(sStepName))
                        {
                            if (oQSD.Steps.TryGetValue(sStepName, out oQSStep))
                            {

                            }
                        }
                    }
                    else
                    {
                        XIDXI oXID = new XIDXI();
                        oQSStep = oXID.GetStepDefinitionByID(iStepDID.ToString(), sStepName);
                    }
                }
                oCResult.oResult = oQSStep;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Step Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}