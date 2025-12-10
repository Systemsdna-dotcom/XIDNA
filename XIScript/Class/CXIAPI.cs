using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XISystem;

namespace XIScript
{
    public class CXIAPI
    {

        public CResult LoadBO(string sBODef, string iInstID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult Execute_1Click(long i1ClickID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult LoadBOFK(string sBODef, string iInstID, string sFKAttr)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult Attr(string sBODef, string iInstID, string sAttrName, string sAttrValue = "", string sLoadByAttr = "")
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            try
            {
                // get the attr value
                CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sBOName", sValue = sBODef });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sAttrName", sValue = sAttrName });
                Params.Add(new CNV() { sName = "sLoadByAttr", sValue = sLoadByAttr });
                if (string.IsNullOrEmpty(sAttrValue))
                {
                    oMyResult = ((CResult)(oMyData.API_Load("GetBOAttribute", Params)));
                }
                else
                {
                    Params.Add(new CNV() { sName = "sAttrValue", sValue = sAttrValue });
                    oMyResult = ((CResult)(oMyData.API_Load("SaveBOAttribute", Params)));
                }

                //  If sLoadByAttr="" then load by PK
                // TEMP
                // oMyResult.oResult = "1234"
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        // API
        public CResult API(string sConcatRequest)
        {
            CResult oMyResult = new CResult();
            try
            {
                // TO DO - split the concat request, it should be in square brackets
                // TEMP!!!!!!!!!!!!!
                if ((sConcatRequest == "logintype"))
                {
                    oMyResult.oResult = "private";
                }

                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        // Reserved
        public CResult Reserved(string sReservedWord)
        {
            CResult oMyResult = new CResult();
            try
            {
                CDataLoad oMyData = new CDataLoad();
                switch (sReservedWord)
                {
                    case "currentuser":
                        oMyResult = ((CResult)(oMyData.API_Load("GetUserRole")));
                        if ((oMyResult.oResult == null))
                        {
                            // oMyResult.oResult = ""
                        }

                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentuserid":
                        oMyResult = ((CResult)(oMyData.API_Load("GetUserID")));
                        if ((oMyResult.oResult == null))
                        {
                            // oMyResult.oResult = ""
                        }

                        // oMyResult.oResult = "DAN"
                        break;
                    case "currentyear":
                        oMyResult.oResult = DateTime.Now.Year;
                        if ((oMyResult.oResult == null))
                        {
                            // oMyResult.oResult = ""
                        }

                        // oMyResult.oResult = "DAN"
                        break;
                }
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult AttrFormatted(string sBODef, string iInstID, string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult AttrFK(string sBODef, string iInstID, string sFKAttrName, string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult UserAttribute(string sAttrName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult UserRole(string sRoleName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // in this role or not?
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        // 
        public CResult QuestionSetFieldValue(string sStepID, string sFieldID)
        {
            CResult oMyResult = new CResult();
            try
            {
                // ASSUMES CURRENT QS INSTANCE, but should we allow any QS?
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult Parameter(string sParamName, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sParamName", sValue = sParamName });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                oMyResult = ((CResult)(oMyData.API_Load("Parameter", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        // QSXIValue
        public CResult QSXIValue(string sParamName, string iInstID, string sGUID, string sSessionID)
        {
            CResult oMyResult = new CResult();
            try
            {
                CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sGUID", sValue = sGUID });
                Params.Add(new CNV() { sName = "sAttrName", sValue = sParamName });
                Params.Add(new CNV() { sName = "sInstanceID", sValue = iInstID });
                Params.Add(new CNV() { sName = "sSessionID", sValue = sSessionID });
                //         Params.Add(New CNV With {
                //     .sName = "sBOName",
                //     .sValue = sBODef
                // })
                oMyResult = ((CResult)(oMyData.API_Load("GetQSAttributeValue", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult QSStepXIValue(string sStepName, string sParamName)
        {
            CResult oMyResult = new CResult();
            try
            {
                // GET FROM Questionset - named step
                // oMyResult.oResult =
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }

        public CResult OM(string sMethod = "", string sUser = "", string sNotificationType = "", string sDoc = "")
        {
            CResult oMyResult = new CResult();
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            // Warning!!! Optional parameters not supported
            try
            {
                // get the attr value
                CDataLoad oMyData = new CDataLoad();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV() { sName = "sMethod", sValue = sMethod });
                Params.Add(new CNV() { sName = "sUser", sValue = sUser });
                Params.Add(new CNV() { sName = "sNotificationType", sValue = sNotificationType });
                Params.Add(new CNV() { sName = "sDoc", sValue = sDoc });
                oMyResult = ((CResult)(oMyData.API_Load("OM", Params)));
            }
            catch (Exception ex)
            {
                oMyResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oMyResult.sMessage = (oMyResult.sMessage
                            + (ex.Message + "\r\n"));
            }

            return oMyResult;
        }
    }
}