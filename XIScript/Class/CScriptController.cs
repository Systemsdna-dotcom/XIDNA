using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CScriptController
    {

        private string sMyGUID;

        private string sMySessionID;

        // eg:
        //     func.{div|{add|{-|9,2},{getattr|'BO','attr1'}}},{-|7,3}}
        // func.{
        //    div|
        //        {add|{-|9,2},
        //            {getattr|'BO','attr1'}
        //        },
        //        {-|7,3}
        // }
        private CCodeLine oTopLine = new CCodeLine();

        public CResult API_ExecuteScript(string sScript)
        {
            CResult oMyResult = new CResult();
            CResult oCR;
            try
            {
                oCR = this.API2_Serialise_From_String(sScript);
                // so OM should now be built
                if ((oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError))
                {
                    return oCR;
                    // TODO: Exit Function: Warning!!! Need to return the value
                    //return;
                }
                else
                {
                    oCR = this.API2_ExecuteMyOM();
                    return oCR;
                    // TODO: Exit Function: Warning!!! Need to return the value
                    //return;
                }

            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        public CResult API2_Serialise_From_String(string sSerialFunction)
        {
            CResult oMyResult = new CResult();
            string sOriginal;
            try
            {
                // ok, so the logic is to work inwards and match up brackets from either end
                //   every time we locate a bracket we will get the end one and then that substring is the function code
                //   and then that line de-codes itself
                sOriginal = sSerialFunction;
                // sOriginal is used for extracting the original strings between quotes
                sSerialFunction = sSerialFunction.ToLower();
                if ((sSerialFunction.Substring(0, 5) == "func."))
                {
                    sSerialFunction = sSerialFunction.Substring(5, (sSerialFunction.Length - 5));
                    sOriginal = sOriginal.Substring(5, (sOriginal.Length - 5));
                }
                else if ((sSerialFunction.Substring(0, 4) == "xi.s"))
                {
                    sSerialFunction = sSerialFunction.Substring(4, (sSerialFunction.Length - 4));
                    sOriginal = sOriginal.Substring(4, (sOriginal.Length - 4));
                }
                else if ((sSerialFunction.Substring(0, 3) == "xi."))
                {
                    sSerialFunction = sSerialFunction.Substring(3, (sSerialFunction.Length - 3));
                    sOriginal = sOriginal.Substring(3, (sOriginal.Length - 3));
                }

                // use whatever brackets you like
                sSerialFunction = sSerialFunction.Replace("(", "{");
                sSerialFunction = sSerialFunction.Replace(")", "}");
                sSerialFunction = sSerialFunction.Replace("[", "{");
                sSerialFunction = sSerialFunction.Replace("]", "}");
                // 2018.05.16
                oTopLine = new CCodeLine();
                oTopLine.DeSerialiseMe(sSerialFunction, sOriginal);
            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        // Public Function API2_Serialise_To_String() As CResult
        //     Dim oMyResult As New CResult
        //     Try
        //     Catch ex As Exception
        //     End Try
        //     Return oMyResult
        // End Function
        public CResult API2_ExecuteMyOM(string sKeyEx = "", CXIAPI oCXIAPIEx = null, CAlgorithmInstance oXIAlgoIEx = null, string sGUID = "", string sSessionID = "" )
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            CResult oCR;
            try
            {
                sMyGUID = sGUID;
                sMySessionID = sSessionID;
                // so the OM is built. At each level there should be the function name (operator)
                //   and then the sub-objects as parameters. If they don't have an operator and they have no sub-params then
                //   that is an actual value which needs to be resolved
                // so this is recursive. execute each level, which checks sub-levels and executes them first
                // TO DO - THERE IS NO SINGLE POINT OF REFERENCE FOR THE LINES, EACH ONLY HAS WHAT YOU PASS DOWN
                oCR = oTopLine.ExecuteMe(sKeyEx, oCXIAPIEx, oXIAlgoIEx, sMyGUID, sSessionID);
                oMyResult = oCR;
                // If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError Then
                //     oMyResult = oCR
                // Else
                //     oMyResult.oResult=oCR.
                // End If
                // If oCR.xiStatus = xiEnumSystem.xiFuncResult.xiError Then
                // Else
                // End If
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (this.Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public CResult API_ParsedFunction()
        {
            CResult oMyResult = new CResult();
            try
            {
                oMyResult = oTopLine.ParsedLine(0);
            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        public CResult API_FormattedFunction()
        {
            CResult oMyResult = new CResult();
            try
            {
                oMyResult = oTopLine.FormattedLine(0);
            }
            catch (Exception ex)
            {
            }

            return oMyResult;
        }

        public CResult API_ResetResults()
        {
            CResult oMyResult = new CResult();
            try
            {
                // TO DO 
                oTopLine.ResetResults();
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = ("ERROR: ["
                            + (this.Get_Class() + ("."
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
                            + (this.Get_Class() + ("."
                            + (System.Reflection.MethodBase.GetCurrentMethod().Name + ("] - "
                            + (ex.Message + (" - Trace: "
                            + (ex.StackTrace + "\r\n"))))))));
            }

            return oMyResult;
        }

        public string Switch_sOpenBracket
        {
            get
            {
                return mGlobals.sOB;
            }
            set
            {
                mGlobals.sOB = value;
            }
        }

        public string Switch_sCloseBracket
        {
            get
            {
                return mGlobals.sCB;
            }
            set
            {
                mGlobals.sCB = value;
            }
        }

        public string Switch_sFunctionDelimiter
        {
            get
            {
                return mGlobals.sFDelim;
            }
            set
            {
                mGlobals.sFDelim = value;
            }
        }

        public string Switch_sParameterDelimiter
        {
            get
            {
                return mGlobals.sParamDelim;
            }
            set
            {
                mGlobals.sParamDelim = value;
            }
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
    }
}