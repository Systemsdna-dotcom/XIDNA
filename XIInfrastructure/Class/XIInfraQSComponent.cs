using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XIInfraQSComponent : XIDefinitionBase
    {
        public string iQSDID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgDatabase { get; set; }
        public string sOrgName { get; set; }
        public string sCurrentUserGUID { get; set; }
        public string sGUID { get; set; }

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
                string sSessionID = HttpContext.Current.Session.SessionID;
                iQSDID = oParams.Where(m => m.sName == "iQSDID").Select(m => m.sValue).FirstOrDefault();
                iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
                sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
                sOrgDatabase = oParams.Where(m => m.sName == "sOrgDatabase").Select(m => m.sValue).FirstOrDefault();
                sGUID = oParams.Where(m => m.sName == "sGUID").Select(m => m.sValue).FirstOrDefault();
                sCurrentUserGUID = oParams.Where(m => m.sName == "sCurrentUserGUID").Select(m => m.sValue).FirstOrDefault();
                XIIQS oQSInstance = new XIIQS();
                if (!string.IsNullOrEmpty(iQSDID))
                {
                    XIIXI oXII = new XIIXI();
                    //Get Question Set Object
                    XIIQS oQSI = new XIIQS();
                    XIDQS oQSD = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, iQSDID);
                    //var oQSD = oDXI.Get_QSDefinition(null, iQSDID);
                    if (!oQSD.bInMemoryOnly)
                    {
                        CResult oQSResult = oXII.CreateQSI(null, iQSDID, null, null, 0, 0, sCurrentUserGUID);
                        if (!oQSResult.bOK)
                        {
                            return oCResult;
                        }
                        oQSInstance = (XIIQS)oQSResult.oResult;
                    }
                    else
                    {
                        oQSInstance.FKiQSDefinitionID = oQSD.ID;
                    }
                    //oQSD.FKiBOStructureID = 0;
                    if (oQSD.FKiBOStructureID > 0 && oQSD.FKiParameterID > 0)
                    {
                        var oStruct = (XIDStructure)oCache.GetObjectFromCache(XIConstant.CacheStructureCode, null, oQSD.FKiBOStructureID.ToString());
                        XIIXI oIXI = new XIIXI();
                        var ActiveBO = oCache.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}"); //oParams.Where(m => m.sName == "{XIP|ActiveBO}").Select(m => m.sValue).FirstOrDefault();
                        var Prm = "{XIP|" + ActiveBO + ".id}";
                        var iBOIID = oCache.Get_ParamVal(sSessionID, sGUID, null, Prm);
                        if (!string.IsNullOrEmpty(ActiveBO) && !string.IsNullOrEmpty(iBOIID))
                        {
                            var oStructobj = oIXI.BOI(ActiveBO, iBOIID).Structure(oStruct.sCode).XILoad("partial");
                            var oXIParams = oCache.GetObjectFromCache(XIConstant.CacheXIParamater, null, oQSD.FKiParameterID.ToString());
                            if (oXIParams != null)
                            {
                                var oXIP = (XIParameter)oXIParams;
                                var oCopy = (XIParameter)oXIP.Clone(oXIP);
                                foreach (var param in oCopy.XiParameterNVs)
                                {
                                    if (param.Value.ToString().Contains('.'))
                                    {
                                        var Splitdot = param.Value.Split('.').ToList();
                                        var NodeData = oStructobj.oChildBOI(Splitdot[0]);
                                        XIIAttribute oAttrI = null;
                                        if (NodeData.FirstOrDefault().Attributes.TryGetValue(Splitdot[1], out oAttrI))
                                        {
                                            if (oAttrI != null)
                                            {
                                                param.Value = oAttrI.sValue;
                                            }
                                        }
                                    }
                                }
                                List<CNV> nSubParams = new List<CNV>();
                                nSubParams.AddRange(oCopy.XiParameterNVs.Select(m => new CNV { sName = m.Name, sValue = m.Value, sType = m.Type }).ToList());
                                oCache.SetXIParams(nSubParams, sGUID, sSessionID);
                                oCache.Set_ParamVal(sSessionID, sGUID, null, oCopy.Name, null, null, nSubParams);
                            }
                        }
                    }
                    oQSInstance.QSDefinition = oQSD;
                    //oQSInstance.oDefintion = oQSD;
                    XIIQSStep oStepI = new XIIQSStep();
                    oStepI.sGUID = sGUID;
                    oStepI.oDefintion = oQSD.Steps.Values.OrderBy(m => m.ID).FirstOrDefault();
                    var oStepIns = oStepI.Load();
                    if (oStepIns.bOK && oStepIns.oResult != null)
                    {
                        oQSInstance.Steps[oQSD.Steps.Values.FirstOrDefault().sName] = (XIIQSStep)oStepIns.oResult;// (((XIInstanceBase)oStepIns.oResult).oContent[XIConstant.ContentStep]);
                        oQSInstance.iCurrentStepID = oQSD.Steps.Values.FirstOrDefault().ID;
                        oQSInstance.sCurrentStepName = oQSD.Steps.Values.FirstOrDefault().sName;
                        oQSInstance.Steps[oQSInstance.sCurrentStepName].bIsCurrentStep = true;
                        oQSInstance.sMode = oQSD.sMode;
                        var Secs = oQSInstance.Steps.Values.Where(m => m.FKiQSStepDefinitionID == oQSInstance.iCurrentStepID).Select(m => m.Sections).ToList();
                        foreach (var sec in Secs)
                        {
                            var XiValues = sec.Values.Select(m => m.XIValues).ToList();
                            foreach (var xivalue in XiValues)
                            {
                                foreach (var item in xivalue)
                                {
                                    oQSInstance.XIValues.Add(item.Key, item.Value);
                                }
                            }
                        }

                    }
                    //oQSInstance = oQSInstance.LoadStepInstance(oQSInstance, 0);

                    oCache.Set_ParamVal(sSessionID, sGUID, null, "{XIP|iQSInstanceID}", Convert.ToString(oQSInstance.XIGUID), "autoset", null);
                    //oQSInstance = oXIRepo.GetQuestionSetInstance(iQSDID, sGUID, null, 0, 0, iUserID, sOrgName, sDatabase, sCurrentUserGUID);
                }
                if (oQSInstance.sMode == null || oQSInstance.sMode.ToLower() == "QuestionSet".ToLower())
                {
                    oCache.Set_QuestionSetCache("QuestionSetCache", sGUID, oQSInstance.XIGUID.ToString(), oQSInstance);
                    oQSInstance.sGUID = sGUID;
                    if (oQSInstance.History == null)
                    {
                        oQSInstance.History = new List<int>();
                    }
                    bool IsHistory = oQSInstance.QSDefinition.Steps.Values.Where(m => m.ID == oQSInstance.iCurrentStepID).Select(m => m.bIsHistory).FirstOrDefault();
                    if (oQSInstance.History.IndexOf(oQSInstance.iCurrentStepID) == -1 && IsHistory)
                    {
                        oQSInstance.History.Add(oQSInstance.iCurrentStepID);
                    }
                }
                else
                {
                    oQSInstance.QSDefinition = null;
                }
                oCResult.oResult = oQSInstance;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing QS Component" });
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.LogToFile();
                SaveErrortoDB(oCResult);
            }
            return oCResult;
        }
    }
}