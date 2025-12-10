using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CMethodStepInstance
    {

        private object oMyResult;

        private string sMyKey;

        public string sKey
        {
            get
            {
                return sMyKey;
            }
            set
            {
                sMyKey = value;
            }
        }

        private string sMyMethodTrace;

        public string sMethodTrace
        {
            get
            {
                return sMyMethodTrace;
            }
            set
            {
                sMyMethodTrace = value;
            }
        }

        private CMethodStepDefinition oMyDefinition;

        public CMethodStepDefinition oDefinition
        {
            get
            {
                return oMyDefinition;
            }
            set
            {
                oMyDefinition = value;
                iExecuteStatus = xiAlgoExecuteStatus.xiWaiting;
            }
        }

        private CAlgorithmInstance oMyAlgoI;

        public CAlgorithmInstance oAlgoI
        {
            get
            {
                return oMyAlgoI;
            }
            set
            {
                oMyAlgoI = value;
            }
        }

        public object oMethodResult
        {
            get
            {
                return oMyResult;
            }
            set
            {
                oMyResult = value;
            }
        }

        private xiAlgoExecuteStatus iMyExecuteStatus;

        public xiAlgoExecuteStatus iExecuteStatus
        {
            get
            {
                return iMyExecuteStatus;
            }
            set
            {
                iMyExecuteStatus = value;
            }
        }

        public enum xiAlgoExecuteStatus
        {

            xiWaiting = 10,

            xiProcessing = 20,

            xiComplete = 30,

            xiError = 40,
        }

        public CResult ConcatDebug(ref string sDebug)
        {
            CResult oCResult = new CResult();
            //CMethodStepDefinition oSubMethod = null;
            CMethodStepInstance oSubMethodI = null;
            sDebug = (sDebug + sMyMethodTrace);
            foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
            {
                if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                {
                    oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                    oSubMethodI.ConcatDebug(ref sDebug);
                }

            }

            return oCResult;
        }

        public CResult Execute()
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            CMethodStepDefinition.xiAlgoActionType iAction = CMethodStepDefinition.xiAlgoActionType.xiNothing;
            string sExecute = "";
            CXICorePlaceholder oPlaceholder = new CXICorePlaceholder();
            CBOPlaceholder oBOI1 = null;
            CBOPlaceholder oBOI2 = null;
            CAlgorithmDefinition oAlgoD = null;
            //CMethodStepDefinition oSubMethod = null;
            List<string> oDictionary = new List<string>();
            CMethodStepInstance oSubMethodI = null;
            string oSValue1 = "";
            string oSValue2 = "";
            bool bYes = false;
            // so this class knows its parent definition
            //   and it can execute child lines first. The only reason for having child lines is if this is an iterative method (??)
            //   then, depending on its type it can do certain things
            // ALERT TO DO - the compiler should check that the return of a method is compatible with the action
            // ALERT TO DO - in the compiler, are there parameters matching the expected input params for the action
            // ALERT TO DO - if you reference algo inbound params, are they defined in the algo param set?
            // ALERT TO DO - the compiler has to check that an iterate method uses an input that can be iterated (such as a BOInstanceGrid). This might be relatively wide ranging and could also be numbers - eg 1 to 5 or something
            // ALERT TO DO - compiler to make sure each line is named uniquely. Or it can have no name and use a default
            // TO DO - replace CXI with simple NV with the NNodes method - use NMethodDefinitionas example template
            // need to get the parameters
            // oAlgoI.
            // xi.ag|DiaryTemplate|p.PolicyFK,p.dGoLive,p.TemplateID
            oAlgoD = oAlgoI.Definition;
            Debug.WriteLine(oDefinition.sStepName);
            sMyMethodTrace = (sMyMethodTrace
                        + (oDefinition.sStepName + (" ["
                        + (oDefinition.iActionType.ToString() + ("] " + ": ")))));
            iAction = oDefinition.iActionType;
            if ((oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiMethod))
            {
                switch (iAction)
                {
                    case CMethodStepDefinition.xiAlgoActionType.xiBOLoad:
                        // xi.m|s:-|i:PolicyI|r:BOInstance|e:DBLog,xiFail|a:m.BOLoad|x:Policy_T,PolicyFK
                        // expecting 2 params. These have to be here, the compiler is checking, we can assume
                        // ALERT TO DO - Interpret these inputs - so they might be parameters or some other dynamic value
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue);
                        oSValue2 = oCR.oResult.ToString();
                        oCR = oPlaceholder.BOLoad(oSValue1, oSValue2);
                        // oAlgoD.oXIParameters.Get_NodeByNumber(1).sValue, oAlgoD.oMethodOM.oParamSet.Get_NodeByNumber(2).sValue)
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            // assign result to the instance in the OM
                            sMyMethodTrace = (sMyMethodTrace + ("With \'"
                                        + (oSValue1 + ("\' and \'"
                                        + (oSValue2 + ("\' Loaded BO Successfully" + "\r\n"))))));
                            oMethodResult = oCR.oResult;
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOCopy:
                        // xi.m|s:--|i:CopyDiary|r:xiR|e:DBLog, xiFail|a: m.BOCopy|x:EachDiaryT, xim.CreateDiaryInstance, xig.CopyLive
                        // Need to get hold of 2 BOs. in the above example, one is from the iteration and one is from a method which created
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oBOI1 = (CBOPlaceholder)oCR.oResult;
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue);
                        oBOI2 = (CBOPlaceholder)oCR.oResult;
                        oCR = oPlaceholder.BOCopy(oBOI1, oBOI2);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("With \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' and \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(2).sValue + ("\' Copied Successfully" + "\r\n"))))));
                        }
                        else
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("With \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' and \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(2).sValue + ("\' FAILED" + "\r\n"))))));
                        }

                        oMethodResult = oCR.xiStatus;
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOSave:
                        // xi.m|s:--|i:InsertDiary|r:xiR|e:DBLog, xiFail|a: m.BOSave|x:xim.CreateDiaryInstance
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oBOI1 = (CBOPlaceholder)oCR.oResult;
                        oCR = oPlaceholder.BOSave(oBOI1);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Saved Successfully" + "\r\n"))));
                        }
                        else
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Save FAILED" + "\r\n"))));
                        }

                        oMethodResult = oCR.xiStatus;
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCreateBO:
                        // xi.m|s:--|i:CreateDiaryInstance|r:BOInstance|e:DBLog, xiFail|a: m.CreateBO|x:bod.Diary_T
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = oPlaceholder.BOCreate(oSValue1);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            // assign result to the instance in the OM
                            oMethodResult = oCR.oResult;
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Created Successfully" + "\r\n"))));
                        }
                        else
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Create FAILED" + "\r\n"))));
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiCondition:
                    case CMethodStepDefinition.xiAlgoActionType.xiConditionNot:
                        // xi.m|s:--|i:IfClient7|a:m.Condition|sc:{if|{eq|{xi.p|'-clientid'},'7'},'y','n'}
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            // TO DO - Execute the condition (or get it??)
                            if ((iAction == CMethodStepDefinition.xiAlgoActionType.xiConditionNot))
                            {
                                bYes = !bYes;
                            }

                            if (bYes)
                            {
                                sMyMethodTrace = (sMyMethodTrace + ("\'"
                                            + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Executing" + "\r\n"))));
                                foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                {
                                    if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                    {
                                        sMyMethodTrace = (sMyMethodTrace + ("\'"
                                                    + (oSubMethod.sKey + ("\'" + "\r\n"))));
                                        oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                        oCR = oSubMethodI.Execute();
                                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                        {
                                            // do anything?
                                        }
                                        else
                                        {
                                            oCResult.xiStatus = oCR.xiStatus;
                                            // keep going?
                                        }

                                    }

                                }

                            }
                            else
                            {
                                sMyMethodTrace = (sMyMethodTrace + ("\'"
                                            + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' Not Executed" + "\r\n"))));
                            }

                            // bYes
                        }

                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiIterate:
                        // xi.m|s:-|i:EachDiaryT|r:xiR|e:DBLog, xiFail|a: m.Iterate|x:xim.DiaryTemplate
                        // has to be a dictionary?
                        // 
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            //oDictionary = oCR.oResult;
                            oDictionary = (List<string>)oCR.oResult;
                            foreach (var oIterate in oDictionary)//.oBOInstances.values)
                            {
                                oMethodResult = oIterate;
                                // this cycle, changes each iteration
                                // FOR EACH [GET SUB ENTITY FROM COLLECTION] 'Set the current instance 
                                //  THEN WITHIN EACH ITEM DO THESE STEPS:
                                foreach (CMethodStepDefinition oSubMethod in oDefinition.NMethodSteps.Values)
                                {
                                    if (oAlgoI.oSteps.ContainsKey(oSubMethod.sKey))
                                    {
                                        sMyMethodTrace = (sMyMethodTrace + ("\'"
                                                    + (oSubMethod.sKey + ("\'" + "\r\n"))));
                                        oSubMethodI = oAlgoI.oSteps[oSubMethod.sKey];
                                        oCR = oSubMethodI.Execute();
                                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                        {
                                            // do anything?
                                        }
                                        else
                                        {
                                            oCResult.xiStatus = oCR.xiStatus;
                                            // keep going?
                                        }

                                    }

                                }

                            }

                        }

                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiSetValue:
                        // xi.m|s:--|i:SetValue1|r:xiR|e:DBLog, xiFail|a: m.SetValue|x:xim.CreateDiaryInstance,[dDue=xim.EachDiary.iDaysFromZero+p.dGoLive]
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oBOI1 = (CBOPlaceholder)oCR.oResult;
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(2).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = oPlaceholder.BOSetValue(oBOI1, oSValue1);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("With \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' and \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(2).sValue + ("\' Value Set" + "\r\n"))))));
                        }
                        else
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("With \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' and \'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(2).sValue + ("\' Set FAILED" + "\r\n"))))));
                        }

                        oMethodResult = oCR.xiStatus;
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xi1Click:
                        // xi.m|s:-|i:TemplateDiaries|r:BOInstanceGrid|e:DBLog,xiFail|a:m.1Click|x:templatediaries
                        oCR = this.Get_ParameterObject(oDefinition.oParamSet.Get_NodeByNumber(1).sValue);
                        oSValue1 = oCR.oResult.ToString();
                        oCR = oPlaceholder.BO1Click(oSValue1);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            // assign result to the instance in the OM
                            oMethodResult = oCR.oResult;
                        }

                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' 1 Click Run Successfully" + "\r\n"))));
                        }
                        else
                        {
                            sMyMethodTrace = (sMyMethodTrace + ("\'"
                                        + (oDefinition.oParamSet.Get_NodeByNumber(1).sValue + ("\' 1 Click FAILED" + "\r\n"))));
                        }

                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xiBOToXIValues:
                        // oMethodResult = oCR.xiStatus
                        break;
                    case CMethodStepDefinition.xiAlgoActionType.xixiValuesToBO:
                        // oMethodResult = oCR.xiStatus
                        break;
                }
            }
            else if ((oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiScript))
            {
                // TO DO - REPLACE THIS WITH A MORE GLOBAL API AND ALSO THE PLACEHOLDER - MERGE INTO THIS API
                CXIAPI oCAPI = new CXIAPI();
                oCR = this.Script_Execute(sKey, oCAPI, oAlgoI);
                if ((oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess))
                {
                    // TO DO - handle the error - to log? Check error method and do what it says (abandon or whatever)
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = oCR.sMessage;
                }

                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                {
                    sMyMethodTrace = (sMyMethodTrace + (" Script returned \'"
                                + (oCR.oResult + ("\'" + "\r\n"))));
                }
                else
                {
                    sMyMethodTrace = (sMyMethodTrace + ("Script FAILED: \'"
                                + (oCR.sMessage + ("\'" + "\r\n"))));
                }

            }

            return oCResult;
        }

        public CResult Get_ParameterObject(string sParameterValue)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            string[] AsParam;
            string sValue = "";
            object oValue = null;
            string sPrefix = "";
            string sParamIdentifier = "";
            CMethodStepInstance oStepResult = null;
            CAlgorithmDefinition oAlgoD = null;
            sParameterValue = sParameterValue.ToLower();
            AsParam = sParameterValue.Split('.');
            if (AsParam.Length == 0)
            {
                // just the value
                sValue = sParameterValue;
                oCResult.oResult = sValue;
            }
            else if (AsParam.Length > 0)
            {
                sPrefix = AsParam[0];
                sParamIdentifier = sParameterValue.Substring((sPrefix.Length + 1), (sParameterValue.Length
                                - (sPrefix.Length - 1)));
                switch (sPrefix.ToLower())
                {
                    case "xif":
                        sValue = sParamIdentifier;
                        oCResult.oResult = sValue;
                        break;
                    case "bod":
                        sValue = sParamIdentifier;
                        oCResult.oResult = sValue;
                        break;
                    case "p":
                        oCResult.oResult = oAlgoI.oXIParameters.NNode(sParameterValue).sValue;
                        break;
                    case "xim":
                    case "xis":
                        if (oAlgoI.oSteps.ContainsKey(sParamIdentifier))
                        {
                            oStepResult = oAlgoI.oSteps[sParamIdentifier];
                            if ((oStepResult.oMethodResult == null))
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                // EXIT the whole thing? Maybe there are circumstances when we should not
                                // NO! in my case i am looking up the value in an iteration of the current line, which to start with is null, but i want to count that as zero
                            }
                            else
                            {
                                oCResult.oResult = oStepResult.oMethodResult;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }

                        }

                        break;
                    case "xic":
                        if (oAlgoI.oSteps.ContainsKey(sParamIdentifier))
                        {
                            oStepResult = oAlgoI.oSteps[sParamIdentifier];
                            if ((oStepResult.oMethodResult == null))
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                                // EXIT the whole thing? Maybe there are circumstances when we should not
                            }
                            else
                            {
                                oCResult.oResult = oStepResult.oMethodResult;
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            }

                        }

                        break;
                        if ((sParameterValue.Substring(0, 1) == "["))
                        {
                            // this is some kind of method/script etc, so just return it
                            sValue = sParameterValue;
                            oCResult.oResult = sValue;
                        }

                        break;
                }
            }

            return oCResult;
        }

        public CResult Script_Execute(string sKeyEx, CXIAPI oCXIAPI, CAlgorithmInstance oAlgoI)
        {
            CResult oCResult = new CResult();
            CResult oCR = null;
            string sScript = "";
            CScriptController oXIScript = null;
            if ((oDefinition.iMethodType == CMethodStepDefinition.xiMethodType.xiScript))
            {
                oXIScript = oAlgoI.oScriptController;
                sScript = oDefinition.sScript;
                oCR = oXIScript.API2_Serialise_From_String(sScript);
                oCR = oXIScript.API2_ExecuteMyOM(sKeyEx, oCXIAPI, oAlgoI);
                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError))
                {
                    oCResult.xiStatus = oCR.xiStatus;
                }
                else
                {
                    oCResult.xiStatus = oCR.xiStatus;
                    oCResult.oResult = oCR.oResult;
                    oMethodResult = oCR.oResult;
                }

            }
            else
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                oCResult.sMessage = ("\'"
                            + (oDefinition.sStepName + "\' is not a script"));
            }

            // oCR = oXIScript.API2_Serialise_From_String(txtScript.Text)
            // txtCodeFormatted.Text = oXIScript.API_FormattedFunction.oResult
            // txtParsedFunction.Text = oXIScript.API_ParsedFunction.oResult
            // oCR = oXIScript.API2_ExecuteMyOM
            // If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError Then
            //     txtOutput.Text = oCR.sMessage
            //     txtScript.BackColor = Color.Red
            // Else
            //     txtOutput.Text = oCR.oResult
            //     txtScript.BackColor = Color.Green
            // End If
            return oCResult;
        }
    }
}