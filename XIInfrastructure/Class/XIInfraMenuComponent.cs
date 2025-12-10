using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraMenuComponent : XIDefinitionBase
    {
        public string sMenuName { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public int iUserID { get; set; }
        public string sCoreDatabase { get; set; }
        public int iOrgID { get; set; }


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
                CUserInfo oInfo = new CUserInfo();
                oInfo = oInfo.GetUserInfo();
                sSessionID = oParams.Where(m => m.sName == "sSessionID").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sMenuName = oParams.Where(m => m.sName == "MenuName").Select(m => m.sValue).FirstOrDefault();
                iUserID = oInfo.iUserID; //Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sCoreDatabase = oInfo.sCoreDataBase;
                iOrgID = oInfo.iOrganizationID; //Convert.ToInt32(oParams.Where(m => m.sName == "iOrganizationID").Select(m => m.sValue).FirstOrDefault());
                List<XIMenu> oRightMenus = new List<XIMenu>();
                XIDXI oXID = new XIDXI();
                oXID.sCoreDatabase = sCoreDatabase;
                oXID.iOrgID = iOrgID;
                oXID.sUserID = iUserID.ToString();
                if (!string.IsNullOrEmpty(sMenuName))
                {
                    oRightMenus = (List<XIMenu>)oCache.GetObjectFromCache(XIConstant.CacheMenu, sMenuName, null);
                    //var oResult = oXID.Get_RightMenuDefinition(sMenuName);
                    //if (oResult.bOK && oResult.oResult != null)
                    //{
                    //    oRightMenus = (List<XIMenu>)oResult.oResult;
                    //}
                }
                oCResult.oResult = oRightMenus;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Menu Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}