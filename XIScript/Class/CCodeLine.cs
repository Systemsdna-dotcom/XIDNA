using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CCodeLine
    {

        private Dictionary<string, CCodeLine> oNCodeLines = new Dictionary<string, CCodeLine>();

        private string sMyLine = "";

        private string sMyLineOrig = "";

        private long iMyType = 0;

        private string sMyOperator = "";

        private long iMyParamCount = 0;

        private string sMyResolvedValue = "";

        private string sMyResult = "";

        private string sMyKey = "";

        private string sMyName = "";

        private CXIAPI oMyXIAPI = null;

        private CAlgorithmInstance oMyAlgoI = null;

        public Dictionary<string, CCodeLine> NCodeLines
        {
            get
            {
                return oNCodeLines;
            }
            set
            {
                oNCodeLines = value;
            }
        }
        string sOB = mGlobals.sOB;
        string sCB = mGlobals.sCB;
        string sFDelim = mGlobals.sFDelim;
        string sParamDelim = mGlobals.sParamDelim;

        public CResult DeSerialiseMe(string sLine, string sOriginalLine)
        {
            CResult oMyResult = new CResult();
            long iFirstBracket;
            long iLastBracket;
            string sFunction = "";
            bool bSubFunction = false;
            int iOperator = 0;
            string sRemainingLine;
            // Dim AsParameters() As String
            int j;
            string sParam = "";
            long iBracketLevel = 0;
            string sChar = "";
            string sCurrentParam = "";
            string sCurrentOrig = "";
            string sRemainingOrig = "";
            string sOrigChar = "";
            try
            {
                sMyLine = sLine;
                sMyLineOrig = sOriginalLine;
                iFirstBracket = (sMyLine.IndexOf(sOB) + 1);
                iLastBracket = sMyLine.LastIndexOf(sCB, sMyLine.Length - 2) + 1; //InStrRev(sMyLine, sCB);
                if (((iFirstBracket != 0)
                            && (iLastBracket != 0)))
                {
                    // so there is a function to resolve here
                    bSubFunction = true;
                }
                else if (((iFirstBracket != 0)
                            || (iLastBracket != 0)))
                {
                    // bracket mis-match, so error out
                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                    oMyResult.sMessage = (oMyResult.sMessage + ("\r\n" + ("Bracket Mismatch: First Bracket position = "
                                + (iFirstBracket + (", Last Bracket position = "
                                + (iLastBracket + (" [Line = \'"
                                + (sMyLine + "\']"))))))));
                }
                else
                {
                    // no more brackets
                    bSubFunction = false;
                }

                if ((sMyLine.Substring(0, 1) == sOB))
                {
                    sMyLine = sMyLine.Substring(1, (sMyLine.Length - 2));
                    sMyLineOrig = sMyLineOrig.Substring(1, (sMyLineOrig.Length - 2));
                }

                iOperator = (sMyLine.IndexOf(sFDelim) + 1);
                if ((iOperator == 0))
                {
                    // A base value, or in any case, not a function
                    Debug.Print(("XIScripting.CCodeLine.DeSerialiseMe." + sMyLine));
                    // oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError
                    // oMyResult.sMessage = oMyResult.sMessage & vbCrLf & "No operator - looking for delimiter '" & sFDelim & "' [Line = '" & sMyLine & "']"
                }
                else
                {
                    sOperator = sMyLine.Substring(0, (iOperator - 1));
                    sRemainingLine = sMyLine.Substring(iOperator, (sMyLine.Length - iOperator));
                    sRemainingOrig = sMyLineOrig.Substring(iOperator, (sMyLineOrig.Length - iOperator));
                    // warning: do not try to strip out brackets here. for example: {add|{-|9,2},{*|2,3}},{-|7,5} , if you look carefully, this is not a bracket at start and bracket at end, but 2 params which happen to be functions
                    if (bSubFunction)
                    {
                        // Solved? TO DO - a logic problem, the matching bracket for this function may not be the last bracket in the string, if there is
                        //    more than one sub-func on the same level then this will include all of the interior code!!!
                        // DS: solution to this is to have code here which gets next param, so it doesn't just do a 'split' based on commas or whatever
                        //   what it does is to go through the string and find any commas, or if it comes across an open bracket then it will count the open brackets from
                        //   then on and the close brackets and ONLY apply the commas which are at bracketlevel zero
                        for (j = 0; (j
                                    <= (sRemainingLine.Length - 1)); j++)
                        {
                            sChar = sRemainingLine.Substring(j, 1);
                            sOrigChar = sRemainingOrig.Substring(j, 1);
                            sCurrentParam = (sCurrentParam + sChar);
                            sCurrentOrig = (sCurrentOrig + sOrigChar);
                            if ((sChar == sParamDelim))
                            {
                                if ((iBracketLevel == 0))
                                {
                                    sCurrentParam = sCurrentParam.Substring(0, (sCurrentParam.Length - 1));
                                    sCurrentOrig = sCurrentOrig.Substring(0, (sCurrentOrig.Length - 1));
                                    this.AddParam(sCurrentParam, sCurrentOrig);
                                    sCurrentParam = "";
                                    sCurrentOrig = "";
                                }
                                else
                                {
                                    // just a comma or something of a sub-func
                                    // sCurrentParam = sCurrentParam & sChar
                                }

                            }
                            else if ((sChar == sOB))
                            {
                                iBracketLevel = (iBracketLevel + 1);
                            }
                            else if ((sChar == sCB))
                            {
                                iBracketLevel = (iBracketLevel - 1);
                            }

                            if ((((j + 1)
                                        == sRemainingLine.Length)
                                        && (sChar != sParamDelim)))
                            {
                                // got to the end of a param. the char should NEVER be the paramdelim, but you never know
                                this.AddParam(sCurrentParam, sCurrentOrig);
                                sCurrentParam = "";
                                sCurrentOrig = "";
                            }

                        }

                    }
                    else
                    {
                        // TO DO - NO - don't hold these even if it is '3' or 'hello'
                        // sResolvedValue = sRemainingLine
                        // sResult = sRemainingLine
                    }

                    // don't resolve the parameters here, each code line should be resolved only once they have their assigned lines chained up
                    // AsParameters = Split(sRemainingLine, sParamDelim)
                    // For j = 0 To UBound(AsParameters)
                    //     sParam = AsParameters(j)
                    //     If InStr(sParam, sOB) = 0 Then 'this is not a 'sub-function'
                    //     Else
                    //     End If
                    // Next j
                }

            }
            catch (Exception ex)
            {
                Debug.Print(("XIScripting.CCodeLine.DeSerialiseMe." + ("Error: " + ex.Message)));
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage + ("\r\n" + ("Bracket Mismatch: [Line = \'"
                            + (sMyLine + "\']"))));
            }

            return oMyResult;
        }

        public CResult AddParam(string sParameter, string sOriginalParam)
        {
            CResult oMyResult = new CResult();
            CCodeLine oSubCodeLine;
            try
            {
                oSubCodeLine = new CCodeLine();
                oSubCodeLine.DeSerialiseMe(sParameter, sOriginalParam);
                iMyParamCount = (iMyParamCount + 1);
                NCodeLines.Add(("CL" + iParamCount), oSubCodeLine);
            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        public CResult ParsedLine(long iLevel)
        {
            CResult oMyResult = new CResult();
            string sConcat = "";
            string sTabs = "";
            try
            {
                for (int j = 1; (j <= iLevel); j++)
                {
                    sTabs = (sTabs + '\t');
                }

                if ((iParamCount > 0))
                {
                    sConcat = (sTabs
                                + (sOperator + (" "
                                + (sOB + "\r\n"))));
                    foreach (var oLine in NCodeLines.Values)
                    {
                        sConcat = (sConcat
                                    + (oLine.ParsedLine((iLevel + 1)).oResult + "\r\n"));
                    }

                    sConcat = (sConcat + ("\r\n"
                                + (sTabs + sCB)));
                }
                else
                {
                    sConcat = (sTabs + sMyLine);
                }

                oMyResult.oResult = sConcat;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult FormattedLine(long iLevel)
        {
            CResult oMyResult = new CResult();
            string sConcat = "";
            string sTabs = "";
            long k = 0;
            try
            {
                for (int j = 1; (j <= iLevel); j++)
                {
                    sTabs = (sTabs + '\t');
                }

                if ((iParamCount > 0))
                {
                    sConcat = (sTabs
                                + (this.OperatorFormat(sOperator) + (" " + "\r\n")));
                    foreach (var oLine in NCodeLines.Values)
                    {
                        sConcat = (sConcat
                                    + (oLine.FormattedLine((iLevel + 1)).oResult + "\r\n"));
                        k = (k + 1);
                        if ((k < NCodeLines.Count))
                        {
                            sConcat = (sConcat
                                        + (sTabs + this.OperatorGlyph(sOperator)));
                        }

                    }

                    sConcat = (sConcat + "\r\n");
                }
                else
                {
                    sConcat = (sTabs + sMyLine);
                }

                oMyResult.oResult = sConcat;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        private string OperatorFormat(string sOperator)
        {
            try
            {
                switch (sOperator.ToLower())
                {
                    case "-":
                    case "minus":
                        return "Minus";
                        break;
                    case "+":
                    case "plus":
                        return "Plus";
                        break;
                    case "/":
                    case "divide":
                        return "Divide";
                        break;
                    case "*":
                    case "multiply":
                        return "Multiply";
                        break;
                    default:
                        return sOperator;
                        break;
                }
            }
            catch (Exception ex)
            {
                return "Error in Operator Format";
            }

        }

        private string OperatorGlyph(string sOperator)
        {
            try
            {
                switch (sOperator.ToLower())
                {
                    case "-":
                    case "minus":
                        return "\'-\'";
                        break;
                    case "+":
                    case "plus":
                        return "\'+\'";
                        break;
                    case "/":
                    case "divide":
                        return "\'/\'";
                        break;
                    case "*":
                    case "multiply":
                        return "\'*\'";
                        break;
                    default:
                        return sOperator;
                        break;
                }
            }
            catch (Exception ex)
            {
                return "Error in Operator Glyph";
            }

        }

        // TO DO - we may need an OM as a template (ie don't keep resolving the same func all the time)
        //   so we need a way of keeping the unresolved values and then re-resolving everything for each func execute
        //   this means maintaining the unresolved values and om (which this does)
        public CResult ExecuteMe(string sKeyEx, CXIAPI oCXIAPIEx, CAlgorithmInstance oXIAlgoIEx, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            //CCodeLine oCParameter;
            CResult oCR;
            CResult oLastChildResult = null;
            try
            {
                if ((sKeyEx != ""))
                {
                    sMyKey = sKeyEx;
                }

                if (!(oCXIAPIEx == null))
                {
                    oMyXIAPI = oCXIAPIEx;
                }

                if (!(oXIAlgoIEx == null))
                {
                    oMyAlgoI = oXIAlgoIEx;
                }

                if ((iParamCount > 0))
                {
                    foreach (var oCParameter in NCodeLines.Values)
                    {
                        oCR = oCParameter.ExecuteMe(sKeyEx, oCXIAPIEx, oXIAlgoIEx, sGUID, sSessionID);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError))
                        {
                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oMyResult.sMessage = (oMyResult.sMessage
                                        + (oCR.sMessage + "\r\n"));
                        }
                        else
                        {
                            oLastChildResult = oCR;
                        }

                    }

                }

                if ((oMyResult.xiStatus != xiEnumSystem.xiFuncResult.xiError))
                {
                    oCR = this.ResolveMe(sGUID, sSessionID);
                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError))
                    {
                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oMyResult.sMessage = (oMyResult.sMessage
                                    + (oCR.sMessage + "\r\n"));
                        return oMyResult;
                        // TODO: Exit Function: Warning!!! Need to return the value
                        //return;
                    }
                    else if (((sOperator == "")
                                && !(oLastChildResult == null)))
                    {
                        // empty bracket for no reason just contains subtext, not a result. So in this case pass up the last child result. 
                        // BE CAREFUL i don't know if negative consequence
                        oMyResult = oLastChildResult;
                    }
                    else
                    {
                        oMyResult = oCR;
                    }

                }

            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult InitialiseMe(CXIAPI oXIAPI, CAlgorithmInstance oXIAlgoI)
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Dim oCParameter As CCodeLine
            // Dim oCR As CResult
            try
            {
                oMyXIAPI = oXIAPI;
                oMyAlgoI = oXIAlgoI;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult ResolveMe(string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            //CCodeLine oSubResult;
            int j = 0;
            string sCondition = "";
            string sIf1 = "";
            string sIf2 = "";
            string sV1 = "";
            long k = 0;
            string sV2 = "";
            string sV3 = "";
            string sCleanedValue = "";
            CResult oCR;
            try
            {
                // remember everything is already in lower case
                if ((sOperator != ""))
                {
                    sMyResolvedValue = (sOB
                                + (sOperator + sFDelim));
                    foreach (var oSubResult in NCodeLines.Values)
                    {
                        j = (j + 1);
                        // RT_Resolve(oSubResult.sResult)
                        oSubResult.RT_Resolve(oSubResult.sResult);
                        // very odd way of doing so be careful
                        sMyResolvedValue = (sMyResolvedValue + oSubResult.sResult);
                        if ((j < NCodeLines.Count))
                        {
                            sMyResolvedValue = (sMyResolvedValue + sParamDelim);
                        }

                    }

                    sMyResolvedValue = (sMyResolvedValue + sCB);
                    switch (sOperator)
                    {
                        case "add":
                        case "+":
                        case "sub":
                        case "-":
                        case "mul":
                        case "*":
                        case "div":
                        case "\\":
                        case "increment":
                        case "decrement":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                switch (sOperator)
                                {
                                    case "add":
                                    case "+":
                                    case "increment":
                                        sMyResult = (double.Parse(NCodeLines["CL1"].sResult) + double.Parse(NCodeLines["CL2"].sResult)).ToString();
                                        break;
                                    case "sub":
                                    case "-":
                                    case "decrement":
                                        sMyResult = (double.Parse(NCodeLines["CL1"].sResult) - double.Parse(NCodeLines["CL2"].sResult)).ToString();
                                        break;
                                    case "mul":
                                    case "*":
                                        sMyResult = (double.Parse(NCodeLines["CL1"].sResult) * double.Parse(NCodeLines["CL2"].sResult)).ToString();
                                        break;
                                    case "div":
                                    case "\\":
                                        try
                                        {
                                            sMyResult = (double.Parse(NCodeLines["CL1"].sResult) / double.Parse(NCodeLines["CL2"].sResult)).ToString();
                                        }
                                        catch (Exception exDiv)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                            oMyResult.sMessage = ("[Line=\'"
                                                        + (sMyLine + ("\'] - Error:" + exDiv.Message)));
                                        }

                                        // Case "concat", "c"
                                        //     sMyResult = NCodeLines["CL1"].sResult & NCodeLines["CL2"].sResult
                                        // Case "dec"
                                        //     Try
                                        //         sMyResult = Math.Round(CDbl(Val(NCodeLines["CL1"].sResult)), CInt(Val(NCodeLines["CL2"].sResult)))
                                        //     Catch exFunc As Exception
                                        //         oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError
                                        //     End Try
                                        break;
                                }
                            }

                            // conditions
                            break;
                        case "eq":
                        case "=":
                        case "gt":
                        case ">":
                        case "lt":
                        case "<":
                        case "ne":
                        case "<>":
                        case "gteq":
                        case ">=":
                        case "lteq":
                        case "<=":
                        case "or":
                        case "||":
                        case "contains":
                            if ((((iParamCount != 2)
                                        || (NCodeLines.Count != 2))
                                        && ((iParamCount != 3)
                                        || (NCodeLines.Count != 3))))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for condition \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                switch (sOperator)
                                {
                                    case "eq":
                                    case "=":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) == double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                            // case sensitive for now
                                            if ((NCodeLines["CL1"].sResult == NCodeLines["CL2"].sResult))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }

                                        break;
                                    case "gt":
                                    case ">":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) > double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                            // don't do anything
                                        }

                                        break;
                                    case "lt":
                                    case "<":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) < double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                        }

                                        break;
                                    case "ne":
                                    case "<>":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) != double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                            // case sensitive for now
                                            if ((NCodeLines["CL1"].sResult != NCodeLines["CL2"].sResult))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }

                                        break;
                                    case "gteq":
                                    case ">=":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) >= double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                        }

                                        break;
                                    case "lteq":
                                    case "<=":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult) && Utility.IsNumeric(NCodeLines["CL2"].sResult)))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) <= double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                            // case sensitive for now
                                        }

                                        break;
                                    case "or":
                                    case "||":
                                        if ((Utility.IsNumeric(NCodeLines["CL1"].sResult)
                                                    && (Utility.IsNumeric(NCodeLines["CL2"].sResult) && Utility.IsNumeric(NCodeLines["CL3"].sResult))))
                                        {
                                            if ((double.Parse(NCodeLines["CL1"].sResult) == double.Parse(NCodeLines["CL2"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else if ((double.Parse(NCodeLines["CL1"].sResult) == double.Parse(NCodeLines["CL3"].sResult)))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            // non-numeric
                                            // case sensitive for now
                                            if ((NCodeLines["CL1"].sResult == NCodeLines["CL2"].sResult))
                                            {
                                                sMyResult = "true";
                                            }
                                            else if ((NCodeLines["CL1"].sResult == NCodeLines["CL3"].sResult))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }

                                        break;
                                    case "contains":
                                        if (!string.IsNullOrEmpty(NCodeLines["CL1"].sResult))
                                        {
                                            string[] STRarray = Regex.Split(NCodeLines["CL1"].sResult, @"\|\|\|");
                                            if (STRarray.Contains(NCodeLines["CL2"].sResult))
                                            {
                                                sMyResult = "true";
                                            }
                                            else
                                            {
                                                sMyResult = "false";
                                            }

                                        }
                                        else
                                        {
                                            sMyResult = "false";
                                        }

                                        break;
                                }
                            }

                            break;
                        case "if":
                            bool bResult;
                            // Dim oCR As CResult
                            if ((NCodeLines.Count != 3))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Condition count for if statement is "
                                            + (NCodeLines.Count + (" when it should be 3 - [condition][if][else] [Line=\'"
                                            + (sMyLine + "\']"))));
                            }
                            else
                            {
                                sCondition = NCodeLines["CL1"].sResult;
                                sIf1 = NCodeLines["CL2"].sResult;
                                sIf2 = NCodeLines["CL3"].sResult;
                                oCR = Get_BooleanResult(sCondition);
                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                {
                                    bResult = (bool)oCR.oResult;
                                    if ((bResult == true))
                                    {
                                        sMyResult = sIf1;
                                    }
                                    else
                                    {
                                        sMyResult = sIf2;
                                    }

                                }
                                else
                                {
                                    // error to do
                                }

                            }

                            break;
                        case "concat":
                        case "c":
                            switch (sOperator)
                            {
                                case "concat":
                                case "c":
                                    foreach (var oParam in NCodeLines.Values)
                                    {
                                        sMyResult = (sMyResult + oParam.sResult);
                                    }

                                    break;
                            }
                            break;
                        case "dec":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sMyResult = Math.Round(double.Parse(double.Parse(NCodeLines["CL1"].sResult).ToString()), int.Parse(double.Parse(NCodeLines["CL2"].sResult).ToString())).ToString();
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "formatdate":
                        case "fd":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sMyResult = (DateTime.Parse(NCodeLines["CL1"].sResult)).ToString(NCodeLines["CL2"].sResult);
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "select":
                        case "s":
                        case "translate":
                        case "match":
                            if (((iParamCount < 2)
                                        || (NCodeLines.Count < 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be greater than 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sV1 = NCodeLines["CL1"].sResult;
                                    long q = 0;
                                    foreach (var oParam in NCodeLines.Values)
                                    {
                                        // sMyResult = sMyResult & oParam.sResult
                                        k = (k + 1);
                                        if ((k > 1))
                                        {
                                            q = (q + 1);
                                            if ((q == 1))
                                            {
                                                sV2 = oParam.sResult;
                                            }

                                            if ((q == 2))
                                            {
                                                // second of a pair
                                                q = 0;
                                                sV3 = oParam.sResult;
                                                if ((sV2 == sV1))
                                                {
                                                    sMyResult = sV3;
                                                    break;
                                                }

                                                sV2 = "";
                                                sV3 = "";
                                            }

                                        }

                                    }

                                    if (((sMyResult == "")
                                                && ((sV3 == "")
                                                && (sV2 != ""))))
                                    {
                                        // so a default was left at the end (ie otherwise its this)
                                        sMyResult = sV2;
                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "isdate":
                        case "d":
                            if (((iParamCount != 1)
                                        || (NCodeLines.Count != 1)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    DateTime date = new DateTime();
                                    if (DateTime.TryParse(NCodeLines["CL1"].sResult, out date))
                                    {
                                        sMyResult = "true";
                                    }
                                    else
                                    {
                                        sMyResult = "false";
                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "Utility.IsNumeric":
                        case "n":
                            if (((iParamCount != 1)
                                        || (NCodeLines.Count != 1)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    if (Utility.IsNumeric(NCodeLines["CL1"].sResult))
                                    {
                                        sMyResult = "true";
                                    }
                                    else
                                    {
                                        sMyResult = "false";
                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "sum":
                            foreach (var oParam in NCodeLines.Values)
                            {
                                sMyResult = (sMyResult + double.Parse(oParam.sResult));
                            }

                            break;
                        case "replace":
                        case "rp":
                            if (((iParamCount != 3)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 3 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sMyResult = NCodeLines["CL3"].sResult.Replace(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult);
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "left":
                        case "l":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sMyResult = NCodeLines["CL1"].sResult.Substring(0, NCodeLines["CL2"].sResult.Length);
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "right":
                        case "r":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    sMyResult = NCodeLines["CL1"].sResult.Substring((NCodeLines["CL1"].sResult.Length - NCodeLines["CL2"].sResult.Length));
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "age":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                // second param is what to calc age in - years, months, days
                                try
                                {
                                    // sMyResult = Right(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult)
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            // choose, translate, select, in, match, regex, isdate, Utility.IsNumeric, sum, average, biggest, smallest, earliest, latest
                            // Replace, makedate, between, Replace, Mid, Right, Left, InStr, round, And, age, Day, Month, Year, week, LCase, UCase, ascii, abs, Random
                            // Not, double, int, date, boolean, trim, ltrim, rtrim, yeardiff, mondiff, daydiff, between
                            // Case "round", "r"
                            //     Select Case sOperator
                            //         Case "round", "r"
                            //             'For Each oParam In NCodeLines.Values
                            //             '    sMyResult = sMyResult & oParam.sResult
                            //             'Next
                            //             sMyResult = round(NCodeLines["CL1"].sResult)
                            //     End Select
                            // Case 
                            break;
                        case "xi.a":
                        case "xi.a.f":
                        case "xi.a.fk":
                        case "xi.a.fk.f":
                        case "xi.d":
                        case "xi.a.d":
                        case "xi.p":
                        case "xi.r":
                        case "xi.api":
                        case "xi.qsxivalue":
                        case "xi.qsstepxivalue":
                            if ((oMyXIAPI == null))
                            {
                                oMyXIAPI = new CXIAPI();
                            }

                            switch (sOperator)
                            {
                                case "xi.p":
                                    if (((iParamCount != 1)
                                                || (NCodeLines.Count != 1)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 1 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.p":
                                                    oCR = oMyXIAPI.Parameter(NCodeLines["CL1"].sResult, sGUID, sSessionID);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    // xi.qsxivalue
                                    break;
                                case "xi.qsxivalue":
                                    if (((iParamCount < 3)
                                                || (NCodeLines.Count < 3)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 1 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.qsxivalue":
                                                    oCR = oMyXIAPI.QSXIValue(NCodeLines["CL3"].sResult, NCodeLines["CL2"].sResult, sGUID, sSessionID);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                                case "xi.qsstepxivalue":
                                    if (((iParamCount != 2)
                                                || (NCodeLines.Count != 2)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 2 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.qsstepxivalue":
                                                    oCR = oMyXIAPI.QSStepXIValue(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                                case "xi.r":
                                    if (((iParamCount != 1)
                                                || (NCodeLines.Count != 1)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 1 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.r":
                                                    oCR = oMyXIAPI.Reserved(NCodeLines["CL1"].sResult);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                                case "xi.api":
                                    if (((iParamCount != 1)
                                                || (NCodeLines.Count != 1)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 1 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.api":
                                                    oCR = oMyXIAPI.API(NCodeLines["CL1"].sResult);
                                                    // can be anything, just means the script can keep up with the OM
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                                case "xi.a":
                                case "xi.f":
                                    if (((iParamCount < 3)
                                                || (NCodeLines.Count < 3)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 3 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.a":
                                                    if ((iParamCount == 3))
                                                    {
                                                        oCR = oMyXIAPI.Attr(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult, NCodeLines["CL3"].sResult);
                                                    }
                                                    else if ((iParamCount > 3))
                                                    {
                                                        oCR = oMyXIAPI.Attr(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult, NCodeLines["CL3"].sResult, NCodeLines["CL4"].sResult, NCodeLines["CL5"].sResult);
                                                    }

                                                    break;
                                                case "xi.a.f":
                                                    oCR = oMyXIAPI.AttrFormatted(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult, NCodeLines["CL3"].sResult);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                                case "xi.a.fk":
                                    if (((iParamCount != 4)
                                                || (NCodeLines.Count != 4)))
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Param count for function \'"
                                                    + (sOperator + ("\' is "
                                                    + (iParamCount + (" when it should be 4 [Line=\'"
                                                    + (sMyLine + "\']"))))));
                                    }
                                    else
                                    {
                                        try
                                        {
                                            oCR = null;
                                            switch (sOperator)
                                            {
                                                case "xi.a.fk":
                                                    oCR = oMyXIAPI.AttrFK(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult, NCodeLines["CL3"].sResult, NCodeLines["CL4"].sResult);
                                                    break;
                                            }
                                            if (!(oCR == null))
                                            {
                                                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                                {
                                                    sMyResult = oCR.oResult.ToString();
                                                }
                                                else
                                                {

                                                }

                                            }

                                        }
                                        catch (Exception exFunc)
                                        {
                                            oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        }

                                    }

                                    break;
                            }
                            break;
                        case "xi.user":
                            if (((iParamCount != 1)
                                        || (NCodeLines.Count != 1)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 3 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    oCR = oMyXIAPI.UserAttribute(NCodeLines["CL1"].sResult);
                                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                    {
                                        // should return a boolean
                                        sMyResult = oCR.oResult.ToString();
                                    }
                                    else
                                    {

                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "xi.userrole":
                            if (((iParamCount != 1)
                                        || (NCodeLines.Count != 1)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    oCR = oMyXIAPI.UserRole(NCodeLines["CL1"].sResult);
                                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                    {
                                        // should return a boolean
                                        sMyResult = oCR.oResult.ToString();
                                    }
                                    else
                                    {

                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "xi.qs":
                            if (((iParamCount != 2)
                                        || (NCodeLines.Count != 2)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 2 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    oCR = oMyXIAPI.QuestionSetFieldValue(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult);
                                    if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                    {
                                        // should return a boolean
                                        sMyResult = oCR.oResult.ToString();
                                    }
                                    else
                                    {

                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        case "xi.om":
                            if ((oMyXIAPI == null))
                            {
                                oMyXIAPI = new CXIAPI();
                            }

                            if (((iParamCount != 4)
                                        || (NCodeLines.Count != 4)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    string sparam1;
                                    string sparam2;
                                    string sparam3;
                                    string sparam4;
                                    sparam1 = NCodeLines["CL1"].sResult;
                                    // coms.notification.create
                                    sparam2 = NCodeLines["CL2"].sResult;
                                    sparam3 = NCodeLines["CL3"].sResult;
                                    sparam4 = NCodeLines["CL4"].sResult;
                                    if ((sparam1 == "XIInfrastructure.XIInfraNotifications.Create"))
                                    {
                                        oCR = oMyXIAPI.OM(sparam1, sparam2, sparam3, sparam4);
                                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                        {
                                            // should return a boolean
                                            sMyResult = oCR.oResult.ToString();
                                        }
                                        else
                                        {

                                        }

                                        // Create notification using param1 and param2
                                    }

                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            // Case "xi.setattr"
                            //     If oMyXIAPI Is Nothing Then oMyXIAPI = New CXIAPI
                            //     If iParamCount <> 2 Or NCodeLines.Count <> 2 Then
                            //         oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError
                            //         oMyResult.sMessage = "Param count for function '" & sOperator & "' is " & iParamCount & " when it should be 1 [Line='" & sMyLine & "']"
                            //     Else
                            //         Try
                            //             Dim sparam1 As String
                            //             Dim sparam2 As String
                            //             sparam1 = NCodeLines["CL1"].sResult 'coms.notification.create
                            //             sparam2 = NCodeLines["CL2"].sResult
                            //             ''If sparam1 = "XIInfrastructure.XIInfraNotifications.Create" Then
                            //             oCR = oMyXIAPI.setattr(sparam1, sparam2, sGUID)
                            //             If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess Then 'should return a boolean
                            //                     sMyResult = oCR.oResult
                            //                 Else
                            //                 End If
                            //             'Create notification using param1 and param2
                            //             '' End If
                            //         Catch exFunc As Exception
                            //             oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError
                            //         End Try
                            //     End If
                            break;
                        case "xim":
                        case "xis":
                            if (((iParamCount != 1)
                                        || (NCodeLines.Count != 1)))
                            {
                                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oMyResult.sMessage = ("Param count for function \'"
                                            + (sOperator + ("\' is "
                                            + (iParamCount + (" when it should be 1 [Line=\'"
                                            + (sMyLine + "\']"))))));
                            }
                            else
                            {
                                try
                                {
                                    if (!(oMyAlgoI == null))
                                    {
                                        // TO DO - get access to the same lookup used by method line instances. Actually this must have a method line
                                        CMethodStepInstance oMethodStepInstance;
                                        oMethodStepInstance = oMyAlgoI.oSteps[sMyKey];
                                        oCR = oMethodStepInstance.Get_ParameterObject(NCodeLines["CL2"].sResult);
                                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                                        {
                                            sMyResult = oCR.oResult.ToString();
                                        }
                                        else
                                        {
                                            // TO DO - FAIL
                                        }

                                    }
                                    else
                                    {
                                        oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oMyResult.sMessage = ("Algorithm Instance not set on Code Line [Line=\'"
                                                    + (sMyLine + "\']"));
                                    }

                                    // oCR = oMyXIAPI.QuestionSetFieldValue(NCodeLines["CL1"].sResult, NCodeLines["CL2"].sResult)
                                    // If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess Then 'should return a boolean
                                    //     sMyResult = oCR.oResult
                                    // Else
                                    // End If
                                }
                                catch (Exception exFunc)
                                {
                                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                }

                            }

                            break;
                        default:
                            sMyResult = sMyLine;
                            break;
                    }
                }
                else
                {
                    // sMyResolvedValue = sMyLine
                    // sMyResult = sMyLine
                    sCleanedValue = sMyLineOrig;
                    if ((sCleanedValue.Length > 1))
                    {
                        if (((sCleanedValue.Substring(0, 1) == "\'")
                                    || (sCleanedValue.Substring(0, 1) == "\"")))
                        {
                            sCleanedValue = sCleanedValue.Substring(1, (sCleanedValue.Length - 2));
                        }

                    }

                    sMyResolvedValue = sCleanedValue;
                    sMyResult = sCleanedValue;
                }

                oMyResult.oResult = sMyResult;
                // OK, you should look at the sresult value, but this helps
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: " + ex.Message);
                // TO DO - put in func names etc all over this DLL
            }

            return oMyResult;
        }

        private string RT_Resolve(string sValue)
        {
            CMethodStepInstance oMethodStepInstance;
            CResult oCR;
            string sResolved = "";
            try
            {
                sResolved = sValue;
                if ((sValue.Contains("xim") || sValue.Contains("xis")))
                {
                    if (!(oMyAlgoI == null))
                    {
                        // TO DO - get access to the same lookup used by method line instances. Actually this must have a method line
                        oMethodStepInstance = oMyAlgoI.oSteps[sMyKey];
                        oCR = oMethodStepInstance.Get_ParameterObject(sValue);
                        if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiSuccess))
                        {
                            sResolved = oCR.oResult.ToString();
                            sResolvedValue = sResolved;
                            // assumption that this is not used anywhere else as it needs to be reset each time
                        }
                        else
                        {
                            // TO DO - FAIL
                            sResolved = "";
                            sResolvedValue = "";
                        }

                    }
                    else
                    {
                        // oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError
                        // oMyResult.sMessage = "Algorithm Instance not set on Code Line [Line='" & sMyLine & "']"
                    }

                }

            }
            catch (Exception ex)
            {
            }

            return sResolved;
        }

        public string sLine
        {
            get
            {
                return sMyLine;
            }
            set
            {
                sMyLine = value;
            }
        }

        public long iType
        {
            get
            {
                return iMyType;
            }
            set
            {
                iMyType = value;
            }
        }

        public string sName
        {
            get
            {
                return sMyName;
            }
            set
            {
                sMyName = value;
            }
        }

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

        public string sOperator
        {
            get
            {
                return sMyOperator;
            }
            set
            {
                sMyOperator = value;
            }
        }

        private bool bMyResolved;

        public bool bResolved
        {
            get
            {
                return bMyResolved;
            }
            set
            {
                bMyResolved = value;
            }
        }

        public string sResolvedValue
        {
            get
            {
                return sMyResolvedValue;
            }
            set
            {
                sMyResolvedValue = value;
                bResolved = true;
            }
        }

        public string sResult
        {
            get
            {
                if (bResolved)
                {
                    return sResolvedValue;
                }
                else
                {
                    return sMyResult;
                }

            }
            set
            {
                sMyResult = value;
                bResolved = false;
            }
        }

        private string sMyRawValue;

        public string sRawValue
        {
            get
            {
                return sMyResult;
            }
        }

        public long iParamCount
        {
            // Private Property iParamCount() As Long
            get
            {
                return iMyParamCount;
            }
        }


        public CResult Get_BooleanResult(string sValue)
        {
            CResult oMyResult = new CResult();
            string sValLower;
            try
            {
                sValLower = sValue.ToLower();
                if (((sValLower == "y")
                            || (sValLower == "true")))
                {
                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oMyResult.oResult = true;
                }
                else if (((sValLower == "n")
                            || (sValLower == "false")))
                {
                    oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oMyResult.oResult = false;
                }
                else
                {
                    // evaluate the condition
                }

            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public CResult ResetResults()
        {
            CResult oMyResult = new CResult();
            //CCodeLine oLine;
            try
            {
                foreach (var oLine in NCodeLines.Values)
                {
                    oLine.ResetResults();
                }

                sResolvedValue = "";
                sResult = "";
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public CResult AddCodeLine(CCodeLine oNewCodeLine)
        {
            CResult oMyResult = new CResult();
            try
            {
                iMyParamCount = (iMyParamCount + 1);
                NCodeLines.Add(("CL" + iParamCount), oNewCodeLine);
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public CResult TemplateFunction()
        {
            CResult oMyResult = new CResult();
            try
            {
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public string Get_Class()
        {
            System.Reflection.MethodBase mb;
            Type Type;
            string sFullReference = "";
            mb = System.Reflection.MethodBase.GetCurrentMethod();
            Type = mb.DeclaringType;
            sFullReference = Type.FullName;
            return sFullReference;
        }

        public string SerialiseNodeFromXML(System.Xml.XmlNode oXMLNode, CCodeLine oCLine, long iUseKey = 0)
        {
            CCodeLine oCNewNode;
            // Warning!!! Optional parameters not supported
            string sClass = "";
            string sSubKey = "";
            string sNodeID = "";
            string sNodeName = "";
            string sNodeKey = "";
            string sNodeType = "";
            string sNameKey = "";
            string sResult = String.Empty;
            //  ----- Ignore plain text nodes, as they are picked up
            //        by the inner-text code below.
            if ((oXMLNode.NodeType == System.Xml.XmlNodeType.Text))
            {
                return "";
            }

            if (((oXMLNode.NodeType == System.Xml.XmlNodeType.XmlDeclaration)
                        && (oCLine == null)))
            {
                return "";
            }

            //  ----- Add the node itself.
            if ((oCLine == null))
            {
                // baseNode = XMLTree.Nodes.Add(oXMLNode.Name)
            }
            else
            {
                // If oCLine.sSubKey <> "" Then
                //     Try
                //         sNodeID = oXMLNode.Item(oCLine.sSubKey).InnerText
                //     Catch ex As Exception
                //         'tricky, if we are really in a collection then it will fail later as it tries to add nodes with the same key
                //         ' if we are not they it's ok to leave blank
                //     End Try
                // End If
                sNodeID = "";
                try
                {
                    if ((oXMLNode.Attributes.Count > 0))
                    {
                        if (!(oXMLNode.Attributes["name"] == null))
                        {
                            sNodeName = oXMLNode.Attributes["name"].Value;
                        }
                        else
                        {
                            sNodeName = "";
                        }

                        if (!(oXMLNode.Attributes["key"] == null))
                        {
                            sNodeKey = oXMLNode.Attributes["key"].Value;
                        }
                        else
                        {
                            sNodeKey = "";
                        }

                        if (!(oXMLNode.Attributes["type"] == null))
                        {
                            sNodeType = oXMLNode.Attributes["type"].Value;
                        }
                        else
                        {
                            sNodeType = "";
                        }

                    }

                }
                catch (Exception ex)
                {
                    Debug.Print(("XIScripting.CCodeLine.SerialiseNodeFromXML." + ex.Message));
                }

                if ((iUseKey == 10))
                {
                    // use the key for the OM (should do really, but defaults to no)
                    sNameKey = sNodeKey;
                }
                else
                {
                    sNameKey = sNodeName;
                }

                // If oXMLNode.Name.ToLower = "value" Then
                //     oCNewNode = oCLine.AddElement(sNameKey, , , oXMLNode)
                //     oCNewNode.sValue = Replace(oXMLNode.InnerText, Chr(10), "")       'DS - not sure if this is some kind of bug or what, but keeps returning chr 10 (line feed i think). Me or xml, don't know
                //     oCNewNode.sType = sNodeType
                // ElseIf oXMLNode.Name.ToLower = "node" Then
                // oCNewNode = oCLine.AddNode(sNameKey, , , oXMLNode)
                oCNewNode = new CCodeLine();
                // oCNewNode.sOperator
                oCLine.AddCodeLine(oCNewNode);
                oCNewNode.iType = long.Parse(sNodeType);
                // End If
                //  ----- Add the child nodes.
                if (oXMLNode.ChildNodes!=null)
                {
                    foreach (System.Xml.XmlNode subNode in oXMLNode.ChildNodes)
                    {
                        switch (subNode.NodeType)
                        {
                            case System.Xml.XmlNodeType.Attribute:
                                break;
                            case System.Xml.XmlNodeType.CDATA:
                                break;
                            case System.Xml.XmlNodeType.Comment:
                                break;
                            case System.Xml.XmlNodeType.Element:
                                if ((subNode.Name.ToLower() == "Lines"))
                                {
                                    //  Or subNode.Name.ToLower = "values" Then   'so these are just collection markers - jump straight to the next level
                                    SerialiseNodeFromXML(subNode, oCNewNode, iUseKey);
                                    // oCLine)
                                }
                                else if ((subNode.Name.ToLower() == "Line"))
                                {
                                    if (!(oCLine == null))
                                    {
                                        SerialiseNodeFromXML(subNode, oCLine, iUseKey);
                                    }

                                }

                                break;
                        }
                    }

                }

            }
            return sResult;
        }
        public string SerialiseNodeToXML()
        {
            System.Text.StringBuilder oConcat = new System.Text.StringBuilder();
            // 'oConcat.Append("<XIDocument>")
            // oConcat.Append("<Values>")
            // For Each oChildXI In Me.NCodeLines.Values
            //     oConcat.Append("<Value").Append(" key='").Append(oChildXI.sKey).Append("' ").Append(" name='").Append(oChildXI.sName).Append("' ").Append(" type='").Append(oChildXI.iType).Append("'>").Append(vbCrLf).Append(oChildXI.sOperator).Append("</Value>").Append(vbCrLf)
            // Next
            // oConcat.Append("</Values>")
            oConcat.Append("<Lines>");
            foreach (var oChildXI in this.NCodeLines.Values)
            {
                oConcat.Append("<Line").Append(" key=\'").Append(oChildXI.sKey).Append("\' ").Append(" name=\'").Append(oChildXI.sName).Append("\' ").Append(" type=\'").Append(oChildXI.iType).Append("\'>").Append("\r\n").Append(oChildXI.SerialiseNodeToXML()).Append("</Line>").Append("\r\n");
            }

            oConcat.Append("</Lines>");
            // oConcat.Append("</XIDocument>")
            return oConcat.ToString();
        }
    }
}