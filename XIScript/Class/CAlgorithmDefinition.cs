using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CAlgorithmDefinition
    {

        // Private oParameters As New Dictionary(Of String, Cxi)

        // Public Property NParameters() As Dictionary(Of String, Cxi)
        // Get
        // Return oParameters
        // End Get
        // Set(ByVal value As Dictionary(Of String, Cxi))
        // oParameters = value
        // End Set
        // End Property

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

        private CMethodStepDefinition oMyMethodDefinition = new CMethodStepDefinition();
        public CMethodStepDefinition oMethodDefinition
        {
            get
            {
                return oMyMethodDefinition;
            }
            set
            {
                oMyMethodDefinition = value;
            }
        }

        private CMethodStepDefinition oMyMethodOM = new CMethodStepDefinition();
        public CMethodStepDefinition oMethodOM
        {
            get
            {
                return oMyMethodOM;
            }
            set
            {
                oMyMethodOM = value;
            }
        }

        private string sMyName;
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

        private bool bMyInitialised;
        public bool bInitialised
        {
            get
            {
                return bMyInitialised;
            }
            set
            {
                bMyInitialised = value;
            }
        }

        private bool bMyCompiled;
        public bool bCompiled
        {
            get
            {
                return bMyCompiled;
            }
            set
            {
                bMyCompiled = value;
            }
        }

        private bool bMyOM;
        public bool bCOM
        {
            get
            {
                return bMyOM;
            }
            set
            {
                bMyOM = value;
            }
        }

        public string Imprint()
        {
            string sConcat = "";

            sConcat = oMethodOM.Imprint();

            return sConcat;
        }



        public CResult CompileOM()
        {
            CResult oCResult = new CResult();
            //CMethodStepDefinition oCMethod;
            CMethodStepDefinition oParentMethod;
            long iIndentDiff;
            long j;

            try
            {
                // so the 2 base nodes are the same, now add on any sub nodes which are side by side in the methoddef
                oMethodOM.oAlgoD = this;
                oParentMethod = oMethodOM;
                foreach (CMethodStepDefinition oCMethod in oMethodDefinition.NMethodSteps.Values)  // Make sure we are not associated with the base node oMethodDefinition, we are just iterating through all his methods
                {
                    iIndentDiff = (oCMethod.iIndent - oParentMethod.iIndent);
                    if (iIndentDiff == 1)
                        oParentMethod.AddMethodObject(oCMethod);
                    else if (iIndentDiff > 1)
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiLogicalError;
                        oCResult.sMessage = "Cannot add line: '" + oCMethod.sStepName + "' - Indent differential is: " + iIndentDiff;
                        return oCResult;
                    }
                    else
                    {
                        // chain back up the tree
                        for (j = 0; j <= (iIndentDiff * -1); j++)
                        {
                            if (oParentMethod != null)
                            {
                                if (oParentMethod.oParentMethod != null)
                                    oParentMethod = oParentMethod.oParentMethod;
                            }
                        }
                        if (oParentMethod != null)
                            oParentMethod.AddMethodObject(oCMethod);
                        else
                        {
                            // Assume a base method (but should we?
                            oParentMethod = oMethodOM;
                            oParentMethod.AddMethodObject(oCMethod);
                        }
                    }

                    oParentMethod = oCMethod;
                }
            }
            catch (Exception ex)
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = ex.Message + " - Stack: " + ex.StackTrace;
            }


            return oCResult;
        }

        public CResult ValidateOM()
        {
            CResult oCResult = new CResult();
            //CMethodStepDefinition oCMethod;
            CResult oCR = null/* TODO Change to default(_) if this is not a reference type */;

            try
            {
                foreach (CMethodStepDefinition oCMethod in oMethodDefinition.NMethodSteps.Values)  // Make sure we are not associated with the base node oMethodDefinition, we are just iterating through all his methods
                {
                    oCR = oCMethod.Validate();
                    if (oCR.xiStatus != xiEnumSystem.xiFuncResult.xiSuccess)
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;

                        oCResult.sMessage = "Errors in Compilation: Validation" + ("\r\n" + oCR.sMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.sMessage = ex.Message + " - Stack: " + ex.StackTrace;
            }


            return oCResult;
        }
    }

}
