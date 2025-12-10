using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraTreeStructureComponent : XIDefinitionBase
    {
        XIInfraCache oCache = new XIInfraCache();
        public string sOrgName { get; set; }
        public string sCode { get; set; }
        public int iBODID { get; set; }
        public string sMode { get; set; }
        public string sSessionID { get; set; }
        public string sGUID { get; set; }

        XIDStructure oXIStruct = new XIDStructure();

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
                if (!string.IsNullOrEmpty(sGUID))
                {
                    oCache.sSessionID = sSessionID;
                    var ParentGUID = oCache.GetParentGUIDRecurrsive(sGUID);
                    sGUID = ParentGUID;
                }
                var WrapperParms = new List<CNV>();
                var TreeParams = new List<CNV>();
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
                iBODID = Convert.ToInt32(oParams.Where(m => m.sName == "iBODID").Select(m => m.sValue).FirstOrDefault());
                sCode = oParams.Where(m => m.sName == "sCode").Select(m => m.sValue).FirstOrDefault();
                sMode = oParams.Where(m => m.sName == "sMode").Select(m => m.sValue).FirstOrDefault();
                var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}"); //oParams.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                var Prm = "{XIP|" + ActiveBO + ".id}";
                var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                //var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, "iBOIID");// oParams.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                XIDXI oXID = new XIDXI();
                List<XIDStructure> oStruct = new List<XIDStructure>();
                var oTree = oCache.Get_ObjectSetCache("Tree_" + sGUID, sGUID, sSessionID);
                if (oTree != null)
                {
                    oStruct = (List<XIDStructure>)oTree;
                }
                if (oTree == null)
                {
                    var oStructD = (List<XIDStructure>)oCache.GetObjectFromCache(XIConstant.CacheStructure, sCode, iBODID.ToString());
                    oStruct = (List<XIDStructure>)oXID.Clone(oStructD);
                    foreach (var items in oStruct)
                    {
                        if (items.sMode != null && items.sMode.ToLower() == "single")
                        {
                            var iID = oCache.Get_ParamVal(sSessionID, sGUID, null, items.sBO.ToLower() + ".id");//oParams.Where(m => m.sName.ToLower() == items.sName.ToLower() + ".id").Select(m => m.sValue).FirstOrDefault();
                            if (!string.IsNullOrEmpty(iID))
                            {
                                items.sInsID = iID.ToString();
                            }
                        }
                    }
                    if (oStruct != null)
                    {
                        if (oStruct.Count() > 0)
                        {
                            oStruct.ToList().ForEach(m => m.sContext = sMode);
                        }
                    }
                    oCache.Set_ObjectSetCache(sSessionID, "Tree_" + sGUID, sGUID, oStruct);
                }
                //var Nodes = oXIStruct.GetXITreeStructure(iBODID, sCode);


                if (WrapperParms != null && WrapperParms.Count() > 0)
                {
                    ActiveBO = WrapperParms.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                    iBOIID = WrapperParms.Where(m => m.sName == "iBOIID").Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(ActiveBO) && !string.IsNullOrEmpty(iBOIID))
                    {
                        var BOD = (XIDBO)(oCache.GetObjectFromCache(XIConstant.CacheBO, ActiveBO, "0"));
                        var sNameAttribute = BOD.Get_BOAttributeValue(ActiveBO, Convert.ToInt32(iBOIID));

                        //var sNameAttribute = oXIAPI.GetBONameAttributeValue(ActiveBO, Convert.ToInt32(iBOIID), iUserID, sDatabase);


                        if (!string.IsNullOrEmpty(sNameAttribute))
                        {
                            if (oStruct.Count() > 0)
                            {
                                var NodeName = oStruct.Where(m => m.sBO == ActiveBO).FirstOrDefault().sName;
                                if (NodeName.Contains('('))
                                {
                                    var index = NodeName.IndexOf('(');
                                    NodeName = NodeName.Substring(0, index + 1);
                                }
                                oStruct.Where(m => m.sBO == ActiveBO).FirstOrDefault().sName = NodeName + " (" + sNameAttribute + ")";
                                oStruct.Where(m => m.sBO == ActiveBO).FirstOrDefault().sInsID = iBOIID;
                            }
                            oCache.Set_ObjectSetCache(sSessionID, "Tree_" + sGUID, sGUID, oStruct);
                        }
                    }
                }
                oCResult.oResult = oStruct;

                //var QSIns = oStructobj.oChildBOI("qs instance");



                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tree Structure Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }

            return oCResult;
        }
    }
}