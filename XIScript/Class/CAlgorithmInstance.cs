using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Specialized;
using XISystem;

namespace XIScript
{
    public class CAlgorithmInstance
    {

        // Private oMySteps As New OrderedDictionary
        // Public Property oSteps() As OrderedDictionary
        //     Get
        //         Return oMySteps
        //     End Get
        //     Set(ByVal value As OrderedDictionary)
        //         oMySteps = value
        //     End Set
        // End Property
        private Dictionary<string, CMethodStepInstance> oMySteps = new Dictionary<string, CMethodStepInstance>();

        public Dictionary<string, CMethodStepInstance> oSteps
        {
            get
            {
                return oMySteps;
            }
            set
            {
                oMySteps = value;
            }
        }

        private CNodeItem oMyXIParams = new CNodeItem();

        public CNodeItem oXIParameters
        {
            get
            {
                return oMyXIParams;
            }
            set
            {
                oMyXIParams = value;
            }
        }

        private CScriptController oMyScriptController = new CScriptController();

        public CScriptController oScriptController
        {
            get
            {
                return oMyScriptController;
            }
            set
            {
                oMyScriptController = value;
            }
        }

        public CResult Execute_OM(string sRunTimeParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            //CMethodStepDefinition oMethodStepD = null;
            CMethodStepInstance oMethodStepI = null;
            string sFirstStepKey = "";
            CMethodStepDefinition oFirstStep = null;
            //CMethodStepDefinition oThisStep = null;
            string sStepKey = "";
            string[] AsRTParams;
            //CNodeItem oXIParam = null;
            long j = 0;
            bool bAtLeastOneIssue = false;
            string sDebug = "";
            AsRTParams = sRunTimeParams.Split(',');
            foreach (CNodeItem oXIParam in Definition.oXIParameters.NNodeItems.Values)
            {
                if (AsRTParams.Length >= j)
                {
                    oXIParameters.NNode(oXIParam.sKey).sValue = AsRTParams[j];
                }

                j = (j + 1);
            }

            // first build a flat collection of method instances so at least each step is represented
            oSteps.Clear();
            // if running through again
            foreach (CMethodStepDefinition oMethodStepD in Definition.oMethodDefinition.NMethodSteps.Values)
            {
                oMethodStepI = new CMethodStepInstance();
                oMethodStepI.sKey = oMethodStepD.sKey;
                oMethodStepI.oAlgoI = this;
                oMethodStepI.oDefinition = oMethodStepD;
                oSteps.Add(oMethodStepD.sKey, oMethodStepI);
            }

            // now execute the OM from the definition and assign the result to this method instance
            // 'sFirstStepKey = Definition.oMethodOM.NMethodSteps.Keys(0)
            // 'oFirstStep = Definition.oMethodOM '.NMethodSteps(sFirstStepKey)
            // 'oSteps(sFirstStepKey).Execute()
            foreach (CMethodStepDefinition oThisStep in Definition.oMethodOM.NMethodSteps.Values)
            {
                // oSteps.Values
                sStepKey = oThisStep.sKey;
                oCR = oSteps[sStepKey].Execute();
                if ((oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess))
                {
                    bAtLeastOneIssue = true;
                }

            }

            if (bAtLeastOneIssue)
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                // SET THE MESSAGE
            }
            else
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }

            // if debug:
            foreach (CMethodStepDefinition oThisStep in Definition.oMethodOM.NMethodSteps.Values)
            {
                // oSteps.Values
                sStepKey = oThisStep.sKey;
                oCR = oSteps[sStepKey].ConcatDebug(ref sDebug);
            }

            oCResult.oResult = sDebug;
            return oCResult;
        }

        public CResult NextStep()
        {
            CResult oCResult = new CResult();
            CMethodStepDefinition oCStep = null;
            CMethodStepDefinition oNextStep = null;
            string sKey = "";
            if ((sCurrentStepKey == ""))
            {
                // first step
                sCurrentStepKey = Definition.oMethodOM.NMethodSteps[0].ToString();
                // sCurrentStepKey = oCStep.sKey
                oNextStep = (CMethodStepDefinition)Definition.oMethodDefinition.NMethodSteps[sCurrentStepKey];
            }
            else
            {
                // get the next step
                oCStep = (CMethodStepDefinition)Definition.oMethodDefinition.NMethodSteps[sCurrentStepKey];
                if ((oCStep.NMethodSteps.Count > 0))
                {
                    oNextStep = (CMethodStepDefinition)oCStep.NMethodSteps[oCStep.NMethodSteps[0].ToString()];
                }
                else
                {
                    // next sibling
                    if (!(oCStep.oParentMethod == null))
                    {
                        if ((oCStep.oParentMethod.NMethodSteps.Count
                                    > (oCStep.iChildIndex + 1)))
                        {
                            oNextStep = (CMethodStepDefinition)oCStep.oParentMethod.NMethodSteps[oCStep.oParentMethod.NMethodSteps[(oCStep.iChildIndex + 1)]];
                        }

                    }

                }

            }

            if (!(oNextStep == null))
            {
                sCurrentStepKey = oNextStep.sKey;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            else
            {
                sCurrentStepKey = "END";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiWarning;
            }

            return oCResult;
        }

        private CAlgorithmDefinition oMyAlgoDef;

        public CAlgorithmDefinition Definition
        {
            get
            {
                return oMyAlgoDef;
            }
            set
            {
                oMyAlgoDef = value;
            }
        }

        private string sMyCurrentStepKey;

        public string sCurrentStepKey
        {
            get
            {
                return sMyCurrentStepKey;
            }
            set
            {
                sMyCurrentStepKey = value;
            }
        }
    }
}