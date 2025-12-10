using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using xiEnumSystem;

namespace XISystem
{
    public class CTypeConvertor
    {
        public CResult EnumStringToOp(string sOperator)
        {
            CResult oMyResult = new CResult();
            // Dim oCR As CResult
            xiQryOperator xiOperator;

            try
            {
                xiOperator = (xiQryOperator)Enum.Parse(typeof(xiQryOperator), sOperator);

                oMyResult.oResult = xiOperator;
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiFuncResult.xiError;
                oMyResult.sMessage = oMyResult.sMessage + "ERROR: [" + Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
            }


            return oMyResult;
        }

        public CResult EnumOpToString(xiQryOperator xiOperator)
        {
            CResult oFuncResult = new CResult();
            string sOperator = "";
            string sOutput = string.Empty;

            try
            {
                sOutput = xiOperator.ToString();
            }
            // sOutput = CInt(xiOperator).ToString()

            // Select Case xiOperator
            // Case xiQryOperator.xiEQ
            // sOperator = "="
            // Case xiQryOperator.xiGT
            // sOperator = ">"
            // Case xiQryOperator.xiGTEQ
            // sOperator = ">="
            // Case xiQryOperator.xiLT
            // sOperator = "<"
            // Case xiQryOperator.xiLTEQ
            // sOperator = "<="

            // 'TO DO - HOW TO HANDLE??????
            // Case xiQryOperator.xiLike
            // sOperator = ""
            // Case xiQryOperator.xiStarts
            // sOperator = ""
            // End Select

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