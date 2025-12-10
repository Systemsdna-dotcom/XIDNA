using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CTypeConvertor
    {

        public CResult EnumStringToOp(string sOperator)
        {
            CResult oMyResult = new CResult();
            // Dim oCR As CResult
            xiEnum.xiQryOperator xiOperator= new xiEnum.xiQryOperator();
            try
            {
                Enum.Parse(typeof(xiEnum.xiQryOperator), sOperator);
                oMyResult.oResult = xiOperator;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + "ERROR: ["  + this.Get_Class() + "."
                            + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            }
            return oMyResult;
        }

        public CResult EnumOpToString(xiEnum.xiQryOperator xiOperator)
        {
            CResult oFuncResult = new CResult();
            string sOperator = "";
            string sOutput = String.Empty;
            try
            {
                sOutput = xiOperator.ToString();
                // sOutput = CInt(xiOperator).ToString()
                // Select Case xiOperator
                //     Case xiEnum.xiQryOperator.xiEQ
                //         sOperator = "="
                //     Case xiEnum.xiQryOperator.xiGT
                //         sOperator = ">"
                //     Case xiEnum.xiQryOperator.xiGTEQ
                //         sOperator = ">="
                //     Case xiEnum.xiQryOperator.xiLT
                //         sOperator = "<"
                //     Case xiEnum.xiQryOperator.xiLTEQ
                //         sOperator = "<="
                //         'TO DO - HOW TO HANDLE??????
                //     Case xiEnum.xiQryOperator.xiLike
                //         sOperator = ""
                //     Case xiEnum.xiQryOperator.xiStarts
                //         sOperator = ""
                // End Select
            }
            catch (Exception ex)
            {
            }
            oFuncResult.oResult = sOperator;
            return oFuncResult;
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