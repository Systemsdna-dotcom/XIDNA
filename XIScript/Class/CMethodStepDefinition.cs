using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;
using System.Collections.Specialized;
using System.Diagnostics;

namespace XIScript
{
    public class CMethodStepDefinition
    {

        public enum xiAlgoReturnType
        {

            xiNothing = 0,

            xiResult = 10,

            xiBOInstanceGrid = 20,

            xiBOInstance = 30,
        }

        public enum xiMethodType
        {

            xiMethod = 0,

            xiScript = 10,
        }

        public enum xiAlgoErrorType
        {

            xiLogFail = 10,
        }

        public enum xiAlgoActionType
        {

            xiSetValue = 10,

            xiBOCopy = 20,

            xiBOSave = 30,

            xiBOLoad = 40,

            xiCreateBO = 50,

            xi1Click = 100,

            xiBOToXIValues = 200,

            xixiValuesToBO = 210,

            xiIterate = 1000,

            xiCondition = 1100,

            xiConditionNot = 1110,

            xiNothing = 0,
        }

        private xiAlgoActionType iMyActionType;

        public xiAlgoActionType iActionType
        {
            get
            {
                return iMyActionType;
            }
            set
            {
                iMyActionType = value;
            }
        }

        private xiAlgoReturnType iMyReturnType;

        public xiAlgoReturnType iReturnType
        {
            get
            {
                return iMyReturnType;
            }
            set
            {
                iMyReturnType = value;
            }
        }

        private xiAlgoErrorType iMyErrorType;

        public xiAlgoErrorType iErrorType
        {
            get
            {
                return iMyErrorType;
            }
            set
            {
                iMyErrorType = value;
            }
        }

        private xiMethodType iMyMethodType;

        public xiMethodType iMethodType
        {
            get
            {
                return iMyMethodType;
            }
            set
            {
                iMyMethodType = value;
            }
        }

        private OrderedDictionary oMethodSteps = new OrderedDictionary();
        public OrderedDictionary NMethodSteps
        {
            get
            {
                return oMethodSteps;
            }
            set
            {
                // (Of String, CMethodStepDefinition))
                oMethodSteps = value;
            }
        }// (Of String, CMethodStepDefinition)

        private CMethodStepDefinition oMyParentMethod;
        public CMethodStepDefinition oParentMethod
        {
            get
            {
                return oMyParentMethod;
            }
            set
            {
                oMyParentMethod = value;
            }
        }

        private long iMyChildIndex;

        public long iChildIndex
        {
            get
            {
                return iMyChildIndex;
            }
            set
            {
                iMyChildIndex = value;
            }
        }

        public CResult AddMethodObject(CMethodStepDefinition oMethodToAdd)
        {
            CResult oResult = new CResult();
            try
            {
                oMethodToAdd.iChildIndex = NMethodSteps.Count;
                oMethodSteps.Add(oMethodToAdd.sKey, oMethodToAdd);
                oMethodToAdd.oParentMethod = this;
                oMethodToAdd.oAlgoD = oAlgoD;
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            }
            catch (Exception ex)
            {
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.sMessage = (ex.Message + (" - Stack: " + ex.StackTrace));
            }

            return oResult;
        }

        public CResult NMethodAdd(string sNodeName, string sKey = "")
        {
            // essentially we build the key up in here to make it easier
            // Warning!!! Optional parameters not supported
            string sConcatKey = "";
            CMethodStepDefinition oNewM = null;
            CResult oResult = new CResult();
            try
            {
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
                if ((sKey == ""))
                {
                    sConcatKey = sNodeName;
                }
                else
                {
                    sConcatKey = sNodeName;
                }

                if (oMethodSteps.Contains(sConcatKey.ToLower()))
                {
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oResult.sMessage = ("Key: \'"
                                + (sConcatKey + "\' already exists in dictionary"));
                }
                else
                {
                    oNewM = NMethod(sNodeName, sKey);
                    oResult.oResult = oNewM;
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                }

            }
            catch (Exception ex)
            {
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
            }

            return oResult;
        }

        public CMethodStepDefinition NMethod(string sNodeName, string sKey = "")
        {
            // essentially we build the key up in here to make it easier
            // Warning!!! Optional parameters not supported
            string sConcatKey = "";
            string sNewKey = "";
            CMethodStepDefinition oNewM = null;
            try
            {
                if ((sKey == ""))
                {
                    sConcatKey = sNodeName;
                }
                else
                {
                    sConcatKey = sNodeName;
                }

                if ((sConcatKey != ""))
                {
                    try
                    {
                        if (oMethodSteps.Contains(sConcatKey.ToLower()))
                        {
                            NMethod(oMethodSteps[sConcatKey.ToLower()].ToString());
                        }
                        else
                        {
                            oNewM = AddMethod(sConcatKey.ToLower());
                            sNewKey = oNewM.sKey;
                            // sNewKey = AddMethod(sConcatKey.ToLower).sKey
                            try
                            {
                                NMethod(oMethodSteps[sNewKey].ToString());
                            }
                            catch (Exception ex2)
                            {
                                //NMethod = null;
                            }

                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.Print("XIGenerics.CXI.NNodeMeta.Error");
                        //NMethod = null;
                    }

                }
                else
                {
                    //NMethod = null;
                }

            }
            catch (Exception ex)
            {
                //NMethod = null;
            }
            return oNewM;
        }

        CMethodStepDefinition AddMethod(string sMethodName)
        {
            CMethodStepDefinition oNew = new CMethodStepDefinition();
            string sKey;
            string sFirstChar = "";
            string sName;
            bool bDataTypeAssigned = false;
            string sGlobalKey = "";
            try
            {
                sName = sMethodName;
                sKey = sMethodName.ToLower();
                if ((oMethodSteps.Contains(sKey)
                            || (sKey == "")))
                {
                    // TO DO - error or can we use a generated key??
                    // sKey = Get_UID()
                }

                oNew = new CMethodStepDefinition();
                // Try
                //     sFirstChar = sName.Substring(0, 1)
                // Catch exName As Exception
                // End Try
                // If bHungarian Then
                //     Select Case sFirstChar
                //         Case "s"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString
                //             bDataTypeAssigned = True
                //         Case "i"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiLong
                //         Case "f"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiFloat
                //         Case "o"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiObject
                //         Case "d"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiDate
                //         Case "r"
                //             oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiCurrency
                //     End Select
                // End If
                // If oNew.tMyBaseDataType = xiEnum.tBaseDataType.xiString And bDataTypeAssigned = False Then  'if this is dan notation then chop of first char of name
                // Else
                sName = sName.Substring(1, (sName.Length - 1));
                // End If
                //     oNew.oParent = Me
                // oNew.oBaseClass = Me.oBaseClass
                // oNew.sClass = sMethodName
                // oNew.iLevel = Me.iLevel + 1
                // oNew.sUID = Get_UID()
                // oNew.sName = sName
                // If sSpecifiedGivenKey = "" Then
                //     oNew.sGivenKey = NextKey()
                // Else
                //     oNew.sGivenKey = sSpecifiedGivenKey
                // End If
                // 'oNew.sGivenKey = NextKey()
                // oNew.xiBaseDataType = tBaseType.xiMeta
                // iMyMetaCount = iMyMetaCount + 1
                // If oCopyFrom Is Nothing Then
                //     'sKey = oNew.oBaseClass.Get_Key(sMethodName, Me, oXMLNode)       ', sNodeId
                //     sGlobalKey = "M." & oNew.sUID
                // Else
                //     'be careful then - why keep keys the same??
                //     sGlobalKey = oCopyFrom.sKey
                // End If
                // oNew.sUID = sGlobalKey        'do this in the get_uid itself
                oNew.sKey = sKey;
                oMethodSteps.Add(sKey, oNew);
                // keep to the original - in the local collection this is the reference
                // Try
                //     oNew.oBaseClass.oRootDictionary.Add(sGlobalKey, oNew)
                // Catch exKey As Exception
                //     Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Key for Meta: " & sName)
                // End Try
                // Try
                //     oNew.oBaseClass.oRootKeyDictionary.Add(oNew.sGivenKey, oNew)
                // Catch exKey2 As Exception
                //     Debug.Print("XIGenerics.CXI.AddMeta." & "Duplicate Given Key for Meta: " & sName)
                // End Try
                // oNew.sKey = sKey
                // If oCopyFrom Is Nothing Then
                // Else
                //     'Debug.Print("Copying")
                // End If
                return oNew;
            }
            catch (Exception ex)
            {
                // Debug.Print("XIGenerics.CXI.AddMeta." & "ERROR IN ADDMETA: " & Err.Description)
                return oNew;
                // really needs to be xiResult
            }

        }

        public CResult NMethodIndex(long iIndex)
        {
            CResult oCResult = new CResult();
            // If 
            return oCResult;
        }

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

        private string sMyStepName;

        public string sStepName
        {
            get
            {
                return sMyStepName;
            }
            set
            {
                sMyStepName = value;
            }
        }

        private string sMyScript;

        public string sScript
        {
            get
            {
                return sMyScript;
            }
            set
            {
                sMyScript = value;
            }
        }

        private string sMyIndent;

        public string sIndent
        {
            get
            {
                return sMyIndent;
            }
            set
            {
                sMyIndent = value;
            }
        }

        private long iMyIndent;

        public long iIndent
        {
            get
            {
                return iMyIndent;
            }
            set
            {
                iMyIndent = value;
            }
        }

        private string sMyActionType;

        public string sActionType
        {
            get
            {
                return sMyActionType;
            }
            set
            {
                sMyActionType = value;
                switch (sMyActionType.ToLower())
                {
                    case "setvalue":
                    case "m.setvalue":
                    case "sv":
                        iActionType = xiAlgoActionType.xiSetValue;
                        break;
                    case "createbo":
                    case "m.createbo":
                    case "boc":
                        iActionType = xiAlgoActionType.xiCreateBO;
                        break;
                    case "1click":
                    case "m.1click":
                    case "1cs":
                    case "1c":
                        iActionType = xiAlgoActionType.xi1Click;
                        break;
                    case "iterate":
                    case "m.iterate":
                    case "it":
                    case "loop":
                        iActionType = xiAlgoActionType.xiIterate;
                        break;
                    case "boload":
                    case "m.boload":
                    case "bol":
                        iActionType = xiAlgoActionType.xiBOLoad;
                        break;
                    case "bosave":
                    case "m.bosave":
                    case "bos":
                        iActionType = xiAlgoActionType.xiBOSave;
                        break;
                    case "bocopy":
                    case "m.bocopy":
                    case "copy":
                        iActionType = xiAlgoActionType.xiBOCopy;
                        break;
                    case "botoxivalues":
                    case "m.botoxivalues":
                    case "2vals":
                        iActionType = xiAlgoActionType.xiBOToXIValues;
                        break;
                    case "xivaluestobo":
                    case "m.xivaluestobo":
                    case "2bo":
                        iActionType = xiAlgoActionType.xixiValuesToBO;
                        break;
                    case "condition":
                    case "m.condition":
                    case "if":
                    case "c":
                        iActionType = xiAlgoActionType.xiCondition;
                        break;
                    case "conditionnot":
                    case "m.conditionnot":
                    case "ifnot":
                    case "cn":
                        iActionType = xiAlgoActionType.xiConditionNot;
                        break;
                    default:
                        iActionType = xiAlgoActionType.xiNothing;
                        break;
                }
            }
        }

        private string sMyReturnType;

        public string sReturnType
        {
            get
            {
                return sMyReturnType;
            }
            set
            {
                sMyReturnType = value;
                switch (value.ToLower())
                {
                    case "boinstance":
                        iReturnType = xiAlgoReturnType.xiBOInstance;
                        break;
                    case "boinstancegrid":
                        iReturnType = xiAlgoReturnType.xiBOInstanceGrid;
                        break;
                    default:
                        iReturnType = xiAlgoReturnType.xiResult;
                        break;
                }
            }
        }

        private string sMyErrorType;

        public string sErrorType
        {
            get
            {
                return sMyErrorType;
            }
            set
            {
                sMyErrorType = value;
                switch (sMyErrorType.ToLower())
                {
                    default:
                        iErrorType = xiAlgoErrorType.xiLogFail;
                        break;
                }
            }
        }

        private string sMyExecute;

        public string sExecute
        {
            get
            {
                return sMyExecute;
            }
            set
            {
                string[] AsParams;
                long j;
                CNodeItem oCXI = null;
                sMyExecute = value;
                // set the params
                // a:m.BOLoad|x:fx.Policy_T,in.PolicyFK  
                //   so in the above, params are not the names but what they are, one is a reference to an inbound param and one is a specific value
                oMyParamSet = new CNodeItem();
                AsParams = sMyExecute.Split(',');
                for (j = 0; j <= AsParams.Length; j++)
                {
                    oCXI = oMyParamSet.NNode(("Param_"
                                    + (j + 1)));
                    oCXI.sValue = AsParams[j];
                    // get the type from the prefix
                }

            }
        }

        public string Imprint()
        {
            string sConcat = "";
            //CMethodStepDefinition oMethod;
            foreach (CMethodStepDefinition oMethod in NMethodSteps.Values)
            {
                sConcat = (sConcat + oMethod.ImprintSelf());
                sConcat = (sConcat + oMethod.Imprint());
            }

            return sConcat;
        }

        public string ImprintSelf()
        {
            string sImprint = "";
            long j;
            for (j = 1; (j <= iIndent); j++)
            {
                sImprint = (sImprint + '\t');
            }

            // TO DO - change to return the interpreted values for error, action etc, not the strings
            sImprint = (sImprint
                        + (sStepName + (" - Return=\'"
                        + (sReturnType + ("\' - Error = \'"
                        + (sErrorType + ("\' - Action=\'"
                        + (sActionType + ("\' - Execute=\'"
                        + (sExecute + ("\'" + "\r\n")))))))))));
            return sImprint;
        }

        private CNodeItem oMyParamSet = new CNodeItem();

        public CNodeItem oParamSet
        {
            get
            {
                return oMyParamSet;
            }
            set
            {
                oMyParamSet = value;
            }
        }

        private CAlgorithmDefinition oMyAlgoDef;

        public CAlgorithmDefinition oAlgoD
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

        public CResult Validate()
        {
            CResult oCResult = new CResult();
            //CMethodStepDefinition oCMethod;
            CResult oCR = null;
            string sValidationError = "";
            bool bType = false;
            bool bParams = false;
            long iParamCount = 0;
            string sParamOrigin = "";
            try
            {
                // so check various things
                switch (iActionType)
                {
                    case xiAlgoActionType.xi1Click:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiBOInstanceGrid;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiBOInstanceGrid))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOCopy:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOLoad:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiBOInstance;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiBOInstance))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOSave:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOToXIValues:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiCreateBO:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiBOInstance;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiBOInstance))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiIterate:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiNothing:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xiSetValue:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        break;
                    case xiAlgoActionType.xixiValuesToBO:
                        if ((iReturnType == xiAlgoReturnType.xiNothing))
                        {
                            iReturnType = xiAlgoReturnType.xiResult;
                        }

                        // set the default, which is usual
                        if ((iReturnType != xiAlgoReturnType.xiResult))
                        {
                            bType = true;
                        }

                        // Case xiAlgoActionType.xiIterate
                        // If iReturnType <> xiAlgoReturnType.xiBOInstanceGrid Then
                        //     bType = True
                        // End If
                        // Case xiAlgoActionType.xiIterate
                        // If iReturnType <> xiAlgoReturnType.xiBOInstanceGrid Then
                        //     bType = True
                        // End If
                        break;
                }
                if (bType)
                {
                    sValidationError = (sValidationError + ("Action Type is "
                                + (iActionType.ToString() + (" but the return type is "
                                + (iReturnType.ToString() + "\r\n")))));
                }

                // input params
                //  for now, we are just counting how many. But in fact we could potentially
                //   work out whether the input params are of the correct type also
                switch (iActionType)
                {
                    case xiAlgoActionType.xi1Click:
                        iParamCount = 1;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOCopy:
                        iParamCount = 3;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOLoad:
                        iParamCount = 2;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOSave:
                        iParamCount = 1;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiBOToXIValues:
                        iParamCount = 2;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiCreateBO:
                        iParamCount = 1;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiIterate:
                        iParamCount = 1;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiNothing:
                        iParamCount = 0;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xiSetValue:
                        iParamCount = 2;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                    case xiAlgoActionType.xixiValuesToBO:
                        iParamCount = 2;
                        if ((oParamSet.iChildCount != iParamCount))
                        {
                            bParams = true;
                        }

                        break;
                }
                if (bParams)
                {
                    sValidationError = (sValidationError + ("Action Type is "
                                + (iActionType.ToString() + (" but the parameter count is "
                                + (oParamSet.iChildCount + (" and should be "
                                + (iParamCount + "\r\n")))))));
                }

                bool bValidInParam;
                foreach (var oXIParam in oParamSet.NNodeItems.Values)
                {
                    bValidInParam = false;
                    sParamOrigin = oXIParam.ToString().ToLower();
                    if ((sParamOrigin.Length > 1))
                    {
                        if ((sParamOrigin.Substring(0, 2) == "p."))
                        {
                            foreach (var oInParam in oAlgoD.oXIParameters.NNodeItems.Values)
                            {
                                if ((oInParam.ToString().ToLower() == sParamOrigin))
                                {
                                    bValidInParam = true;
                                }

                            }

                        }
                        else
                        {
                            // for now, assume it is valid
                            bValidInParam = true;
                        }

                    }

                    if ((bValidInParam == false))
                    {
                        sValidationError = (sValidationError + ("Parameter "
                                    + (oXIParam.ToString() + (" expected but does not exist in algorithm inbound parameters" + "\r\n"))));
                    }

                }

                if ((sValidationError != ""))
                {
                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oCResult.sMessage = ("Method: "
                                + (sStepName + (" - Validation Errors: " + ("\r\n" + sValidationError))));
                }

                // children
                foreach (CMethodStepDefinition oCMethod in oMethodSteps.Values)
                {
                    oCR = oCMethod.Validate();
                    if ((oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess))
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = (oCResult.sMessage
                                    + (oCR.sMessage + "\r\n"));
                    }

                }

            }
            catch (Exception ex)
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = (ex.Message + (" - Stack: " + ex.StackTrace));
            }

            return oCResult;
        }
    }
}