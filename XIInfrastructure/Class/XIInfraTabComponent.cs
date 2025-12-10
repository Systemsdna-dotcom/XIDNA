using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraTabComponent : XIDefinitionBase
    {
        public string sStructureCode { get; set; }
        public int iBODID { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }
        public string sOutputContent { get; set; }

        public CResult XILoad(List<CNV> oParams)
        {
            XIInfraCache oCache = new XIInfraCache();
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
                var BODID = oParams.Where(m => m.sName == "iBODID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(BODID))
                {
                    iBODID = Convert.ToInt32(BODID);
                }
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                sStructureCode = oParams.Where(m => m.sName == "StructureCode").Select(m => m.sValue).FirstOrDefault();
                sOutputContent = oParams.Where(m => m.sName == "OutputContent").Select(m => m.sValue).FirstOrDefault();
                var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "sBOName");
                var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "{-iInstanceID}");
                if (iBODID > 0 && !string.IsNullOrEmpty(sStructureCode))
                {
                    XIDStructure oStrct = new XIDStructure();
                    var oTree = oStrct.GetXIStructureTreeDetails(iBODID.ToString(), sStructureCode);
                    XIIXI oIXI = new XIIXI();
                    var oStructobj = oIXI.BOI(ActiveBO, iBOIID).Structure(sStructureCode).XILoad("partial");                   
                    foreach (var items in oTree)
                    {
                        if (items.sMode != null && items.sMode.ToLower() == "single")
                        {
                            if (!string.IsNullOrEmpty(items.sBO))
                            {
                                var NodeData = oStructobj.oChildBOI(items.sBO);
                                if (NodeData != null)
                                {
                                    if (NodeData.FirstOrDefault().BOD != null)
                                    {
                                        var sPrimaryKey = NodeData.FirstOrDefault().BOD.sPrimaryKey.ToLower();
                                        var PKValue = NodeData.FirstOrDefault().Attributes[sPrimaryKey].sValue;
                                        items.iInstanceID = Convert.ToInt32(PKValue);
                                    }
                                }
                            }                            
                        }
                        else if(items.sMode != null && items.sMode.ToLower() == "Multiple".ToLower())
                        {
                            //var NodeData = oStructobj.oChildBOI(items.sBO);
                            //if (NodeData != null)
                            //{
                            //    if (NodeData.FirstOrDefault().BOD != null)
                            //    {
                            //        int iOneClickID = NodeData.FirstOrDefault().BOD.iOneClickID;
                            //        items.i1ClickID = iOneClickID;
                            //    }
                            //}
                        }
                    }
                    oCResult.oResult = oTree;
                    if (oTree != null && oTree.Count() > 0)
                    {
                        oTree.ToList().ForEach(m => m.sOutputContent = sOutputContent);
                    }
                }
                else
                {
                    oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Params loading error for Tab Component" });
                }
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tab Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }

            return oCResult;
        }
    }
}