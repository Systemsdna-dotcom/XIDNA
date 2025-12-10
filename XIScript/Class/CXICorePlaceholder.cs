using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CXICorePlaceholder
    {

        public CResult BOCreate(string sBOName)
        {
            CResult oCResult = new CResult();
            CBOPlaceholder oNewBOPlaceholder = new CBOPlaceholder();
            oNewBOPlaceholder.sName = "BO CREATE";
            oCResult.oResult = oNewBOPlaceholder;
            return oCResult;
        }

        public CResult BOLoad(string sBOName, string sUID)
        {
            CResult oCResult = new CResult();
            CBOPlaceholder oNewBOPlaceholder = new CBOPlaceholder();
            oNewBOPlaceholder.sName = "BO LOAD";
            oCResult.oResult = oNewBOPlaceholder;
            return oCResult;
        }

        public CResult BOSave(CBOPlaceholder oBO, string sGroup="")
        {
            CResult oCResult = new CResult();
            // Warning!!! Optional parameters not supported
            oBO.sName = ("SAVE: " + oBO.sName);
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }

        // BO1Click
        public CResult BO1Click(string s1ClickName)
        {
            CResult oCResult = new CResult();
            long j;
            CBOPlaceholder oBOI = null;
            BOInstanceGridPlaceholder oNewBOInstPlaceholder = new BOInstanceGridPlaceholder();
            oNewBOInstPlaceholder.sName = "1 Click";
            for (j = 1; (j <= 2); j++)
            {
                oBOI = new CBOPlaceholder();
                oBOI.sName = ("Sub Item " + j);
                oNewBOInstPlaceholder.oBOInstances.Add(("Item" + j), oBOI);
            }

            oCResult.oResult = oNewBOInstPlaceholder;
            return oCResult;
        }

        public CResult BOCopy(CBOPlaceholder oBOFrom, CBOPlaceholder oBOTo, string sCopyGroup="")
        {
            CResult oCResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Dim oNewBOPlaceholder As New CBOPlaceholder
            // oNewBOPlaceholder.sName = "BO COPY"
            // oCResult.oResult = oNewBOPlaceholder
            oBOTo.sName = oBOTo.sName + ": COPY FROM: " + oBOFrom.sName;
            oCResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiSuccess;
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }

        public CResult BOSetValue(CBOPlaceholder oBO, string sValueMethod)
        {
            CResult oCResult = new CResult();
            oBO.sName = oBO.sName + ": S:" + sValueMethod;
            oCResult.xiStatus = (xiEnumSystem.xiFuncResult)xiEnum.xiFuncResult.xiSuccess;
            oCResult.oResult = "xiSuccess";
            return oCResult;
        }
    }
}