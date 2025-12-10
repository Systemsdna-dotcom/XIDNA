using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using XICore;
//using XIDatabase;
using XIDNA.Models;
using XIDNA.Repository;
using XISystem;
using System.Threading;
using System.Diagnostics;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.IO;

namespace XIDNA.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IXiLinkRepository XiLinkRepository;

        public PaymentController() : this(new XiLinkRepository()) { }

        public PaymentController(IXiLinkRepository XiLinkRepository)
        {
            this.XiLinkRepository = XiLinkRepository;
        }
        CommonRepository Common = new CommonRepository();
        // GET: Payment
        public ActionResult Payment(string ID, string QuoteID, string sLayoutGUID)
        {
            CResult oResult = new CResult();
            try
            {
                XIIXI oXII = new XIIXI();
                List<CNV> oNV = new List<CNV>();
                XIInfraCache oCache = new XIInfraCache();
                var sSessionID = HttpContext.Session.SessionID;
                string sQuote = oCache.Get_ParamVal(sSessionID, sLayoutGUID, null, "{XIP|iQuoteID}");
                int iQuoteID = 0;
                if(!string.IsNullOrEmpty(QuoteID))
                {
                    if(int.TryParse(sQuote, out iQuoteID))
                    {
                        oNV.Add(new CNV { sName = "ID", sValue = iQuoteID.ToString() });
                    }
                    else
                    {
                        oNV.Add(new CNV { sName = "sGUID", sValue = QuoteID });
                    }
                }
                var oQuoteI = oXII.BOI("Aggregations", "", "", oNV);
                if (oQuoteI != null && !string.IsNullOrEmpty(oQuoteI.AttributeI("rPayableAmount").sValue))
                {
                    var QSIID = oQuoteI.AttributeI("FKiQSInstanceID").sValue;
                    int iQSInstanceID = 0;
                    if (int.TryParse(QSIID,out iQSInstanceID))
                    { }
                    int iGlobalPaymentID = 0;
                    XIIQS oQSI = oXII.GetQSXIValuesByQSIID(iQSInstanceID.ToString());
                    var oQSDefinition = (XIDQS)oCache.GetObjectFromCache(XIConstant.CacheQuestionSet, null, oQSI.FKiQSDefinitionID.ToString());//oDXI.GetQuestionSetDefinitionByID(null, iQSID.ToString());
                    var OriginI = oXII.BOI("XIOrigin", oQSDefinition.FKiOriginID.ToString());
                    if(OriginI !=null && OriginI.Attributes.ContainsKey("FkiPaymentID"))
                    {
                        var PaymentID = OriginI.Attributes["FkiPaymentID"].sValue;
                        if(int.TryParse(PaymentID,out iGlobalPaymentID))
                        { }
                    }
                    var sQuoteID = oQuoteI.AttributeI("ID").sValue;
                    var PayableAmount = oQuoteI.AttributeI("rPayableAmount").sValue;
                    //ID = "589";
                    //Amount = "660";
                    //XIBOBuilding oBO = new XIBOBuilding();
                    //oBO.OrderID = "589";
                    //oBO.iPaymentID = Convert.ToInt32(26);
                    //oBO.rGrossPremium = Convert.ToDecimal(748);
                    //oBO.PolicySequenceNumber = "66000999999";
                    //oBO.BuildPolicyObject();
                    string sDatabase = SessionManager.CoreDatabase;
                    decimal dAmount = Convert.ToDecimal(PayableAmount);
                    int OrganizationID = SessionManager.OrganizationID;
                    var sServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];
                    XIInfraPayment XIInfraPayment = new XIInfraPayment();
                    var oPaymentGateWay = XIInfraPayment.Get_PaymentGatway(OrganizationID, sDatabase, sServerKey, iGlobalPaymentID);
                    if (oPaymentGateWay.xiStatus == 0 && oPaymentGateWay.oResult != null)
                    {
                        XIInfraPayment = (XIInfraPayment)oPaymentGateWay.oResult;
                        if (XIInfraPayment.ID > 0)
                        {
                            if (XIInfraPayment.sName.ToLower() == "GlobalPayment".ToLower())
                            {
                                if (string.IsNullOrEmpty(XIInfraPayment.sHPPVerion))
                                {
                                    XIInfraPayment.sHPPVerion = "1";
                                }
                                PayWithGlobalPayments(XIInfraPayment, ID, dAmount, sQuoteID, sLayoutGUID);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
            }
            return null;
        }
        public void PayWithGlobalPayments(XIInfraPayment oPaymentGateWay, string ID, decimal Amount, string sQuoteID, string sLayoutGUID)
        {
            CResult oResult = new CResult();
            try
            {
                var timestamp = DateTime.Now.AddHours(1).ToString("yyyyMMddHHmmss");
                var currency = "GBP";
                var orderid = ID;
                var amount = Convert.ToInt32(Amount * 100);
                var sha = SHA1HashStringForUTF8String(timestamp + "." + oPaymentGateWay.sMerchantID + "." + orderid + "." + amount + "." + currency);
                var sha1 = SHA1HashStringForUTF8String(sha + "." + oPaymentGateWay.sSecret);
                RemotePost myremotepost = new RemotePost();
                myremotepost.Url = oPaymentGateWay.ReturnUrl;
                myremotepost.Add("MERCHANT_ID", oPaymentGateWay.sMerchantID);
                myremotepost.Add("SECRET_ID", oPaymentGateWay.sSecret);
                myremotepost.Add("TIMESTAMP", timestamp);
                myremotepost.Add("ACCOUNT", oPaymentGateWay.sAccount);
                myremotepost.Add("ORDER_ID", orderid.ToString());
                myremotepost.Add("AMOUNT", amount.ToString());
                myremotepost.Add("CURRENCY", currency);
                myremotepost.Add("SHA1HASH", sha1);
                myremotepost.Add("HPP_VERSION", oPaymentGateWay.sHPPVerion);
                myremotepost.Add("AUTO_SETTLE_FLAG", "1");
                //string sSRD = RandomString(9);
                //myremotepost.Add("SRD", sSRD);
                //Common.SaveErrorLog("Payment Request SRD:" + sSRD + "_QuoteGUID:"+ orderid.Split('_')[0], "XIDNA");
                //sID=232&sSID=xyz
                var sServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];
                var sSessionID = HttpContext.Session.SessionID;
                oPaymentGateWay.ResponseUrl = oPaymentGateWay.ResponseUrl + "sID=" + sServerKey + "&sSID=" + sSessionID + "&sLayoutGUID=" + sLayoutGUID;
                myremotepost.Add("MERCHANT_RESPONSE_URL", oPaymentGateWay.ResponseUrl);

                Dictionary<string, string> oRequest = new Dictionary<string, string>();
                oRequest.Add("MERCHANT_ID", oPaymentGateWay.sMerchantID);
                oRequest.Add("SECRET_ID", oPaymentGateWay.sSecret);
                oRequest.Add("TIMESTAMP", timestamp);
                oRequest.Add("ACCOUNT", oPaymentGateWay.sAccount);
                oRequest.Add("ORDER_ID", orderid);
                oRequest.Add("AMOUNT", amount.ToString());
                oRequest.Add("CURRENCY", currency);
                oRequest.Add("AUTO_SETTLE_FLAG", "1");
                oRequest.Add("HPP_VERSION", oPaymentGateWay.sHPPVerion);
                oRequest.Add("MERCHANT_RESPONSE_URL", oPaymentGateWay.ResponseUrl);

                var oRequestObject = JsonConvert.SerializeObject(oRequest);
                XIIXI oXII = new XIIXI();
                List<XIWhereParams> oWhrParams = new List<XIWhereParams>();
                QueryEngine oQE = new QueryEngine();
                oWhrParams.Add(new XIWhereParams { sField = "FKiQuoteID", sOperator = "=", sValue = sQuoteID.ToString() });
                oWhrParams.Add(new XIWhereParams { sField = XIConstant.Key_XIDeleted, sOperator = "=", sValue = "0" });
                oQE.AddBO("PaymentRequest_T", "", oWhrParams);
                CResult oCresult1 = oQE.BuildQuery();
                if (oCresult1.bOK && oCresult1.oResult != null)
                {
                    var sSql1 = (string)oCresult1.oResult;
                    ExecutionEngine oEE = new ExecutionEngine();
                    oEE.XIDataSource = oQE.XIDataSource;
                    oEE.sSQL = sSql1;
                    var oQResult = oEE.Execute();
                    if (oQResult.bOK && oQResult.oResult != null)
                    {
                        var oBOIList1 = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.ToList();
                        var oBOD1 = oQE.QParams.FirstOrDefault().BOD;
                        oBOIList1.ForEach(x => x.BOD = oBOD1);
                        if (oBOIList1 != null && oBOIList1.Count > 0)
                        {
                            foreach (var oPaymentI in oBOIList1)
                            {
                                XIIBO oBO = new XIIBO();
                                oBO.Delete(oPaymentI);
                            }
                        }
                    }
                }


                XIIBO oBOI = new XIIBO();
                XIInfraCache oCache = new XIInfraCache();
                XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "PaymentRequest_T", null);
                oBOI.BOD = oBOD;
                oBOI.LoadBOI("create");
                if (oBOI.Attributes.ContainsKey("fkiquoteid"))
                {
                    oBOI.Attributes["fkiquoteid"].sValue = sQuoteID;
                }
                if (oBOI.Attributes.ContainsKey("fksquoteid"))
                {
                    oBOI.Attributes["fksquoteid"].sValue = orderid.Split('_')[0];
                }
                if (oBOI.Attributes.ContainsKey("sguid"))
                {
                    oBOI.Attributes["sguid"].sValue = orderid.Split('_')[1];
                }
                if (oBOI.Attributes.ContainsKey("sorderid"))
                {
                    oBOI.Attributes["sorderid"].sValue = orderid;
                }
                if (oBOI.Attributes.ContainsKey("sresponseurl"))
                {
                    oBOI.Attributes["sresponseurl"].sValue = oPaymentGateWay.ResponseUrl;
                }
                if (oBOI.Attributes.ContainsKey("ramount"))
                {
                    oBOI.Attributes["ramount"].sValue = String.Format("{0:0.00}", Amount);
                }
                if (oBOI.Attributes.ContainsKey("ssha1hash"))
                {
                    oBOI.Attributes["ssha1hash"].sValue = sha1;
                }
                if (oBOI.Attributes.ContainsKey("ssecret"))
                {
                    oBOI.Attributes["ssecret"].sValue = oPaymentGateWay.sSecret;
                }
                if (oBOI.Attributes.ContainsKey("srequestcontent"))
                {
                    oBOI.Attributes["srequestcontent"].sValue = oRequestObject;
                }
                oBOI.Attributes.ToList().ForEach(m => m.Value.bDirty = true);
                oBOI.Save(oBOI, false);

                myremotepost.Post();
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oResult.LogToFile();
            }
        }
        public string SHA1HashStringForUTF8String(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public class RemotePost
        {
            private System.Collections.Specialized.NameValueCollection Inputs = new System.Collections.Specialized.NameValueCollection();
            public string Url = "";
            public string Method = "post";
            public string FormName = "form1";
            public void Add(string name, string value)
            {
                Inputs.Add(name, value);
            }
            public void Post()
            {
                System.Web.HttpContext.Current.Response.Clear();
                System.Web.HttpContext.Current.Response.Write("<html><head>");
                System.Web.HttpContext.Current.Response.Write(string.Format("</head><body onload=\"document.{0}.submit()\">", FormName));
                System.Web.HttpContext.Current.Response.Write(string.Format("<form name=\"{0}\" method=\"{1}\" action=\"{2}\" >", FormName, Method, Url));
                for (int i = 0; i < Inputs.Keys.Count; i++)
                {
                    System.Web.HttpContext.Current.Response.Write(string.Format("<input name=\"{0}\" type=\"hidden\" value=\"{1}\">", Inputs.Keys[i], Inputs[Inputs.Keys[i]]));
                }
                System.Web.HttpContext.Current.Response.Write("</form>");
                System.Web.HttpContext.Current.Response.Write("</body></html>");
                System.Web.HttpContext.Current.Response.End();
            }
        }
        [AllowAnonymous]
        public ActionResult PayWithGlobalPaymentResponse(string sID, string sSID, string sLayoutGUID)
        {
            XIInfraCache oCache = new XIInfraCache();
            //XIInfraCache oCache = new XIInfraCache();
            CResult oResult = new CResult();
            //XIIQS oQSInstance = new XIIQS();
            try
            {
                var sServerKey = System.Configuration.ConfigurationManager.AppSettings["ServerKey"];
                Common.SaveErrorLog("Returned to PaymetResponse Method_sID " + sID + " and _sSID " + sSID + " and Sever Key " + sServerKey + " and GUID: " + sLayoutGUID, "XIDNA");
                string sResponseContent;
                using (Stream receiveStream = Request.InputStream)
                {
                    using (StreamReader readStream = new StreamReader(receiveStream, Request.ContentEncoding))
                    {
                        sResponseContent = readStream.ReadToEnd();
                    }
                }
                ViewBag.sLayoutGUID = sLayoutGUID;
                Common.SaveErrorLog("Response Object " + sResponseContent, "XIDNA");
                var Orderid = Request.Params["ORDER_ID"];
                //var sResponseContent = Request.Form["hppResponse"];
                string[] ResList = Orderid.Split('_');
                string sQuoteID = ResList[0];
                var Result = Request.Params["RESULT"];
                var MerchantId = Request.Params["MERCHANT_ID"];
                var acthcode = Request.Params["AUTHCODE"];
                var message = Request.Params["MESSAGE"];
                var parsef = Request.Params["PASREF"];
                var timestamp = Request.Params["TIMESTAMP"];
                var amount = Request.Params["AMOUNT"];
                var amount1 = String.Format("{0:0.00}", (Convert.ToDecimal(amount) / 100));
                var sha1hash = Request.Params["SHA1HASH"];
                XIIXI oXII = new XIIXI();
                List<CNV> oWhrParams = new List<CNV>();
                oWhrParams.Add(new CNV { sName = "FKsQuoteID", sValue = sQuoteID.ToString() });
                oWhrParams.Add(new CNV { sName = XIConstant.Key_XIDeleted, sValue = "0" });
                var oPaymentRequestI = oXII.BOI("PaymentRequest_T", null, "create", oWhrParams);
                string sSecret = string.Empty;
                if (oPaymentRequestI.Attributes != null && oPaymentRequestI.Attributes.ContainsKey("ssecret"))
                {
                    sSecret = oPaymentRequestI.Attributes["ssecret"].sValue;
                }
                if (oPaymentRequestI.Attributes != null && oPaymentRequestI.Attributes.ContainsKey("ssecret"))
                {
                    oPaymentRequestI.Attributes["ssecret"].sValue = "";
                    oPaymentRequestI.Attributes["ssecret"].bDirty = true;
                    oPaymentRequestI.Save(oPaymentRequestI);
                }
                var sha = SHA1HashStringForUTF8String(timestamp + "." + MerchantId + "." + Orderid + "." + Result + "." + message + "." + parsef + "." + acthcode);
                var sha1 = SHA1HashStringForUTF8String(sha + "." + sSecret);
                var SRD = Request.Params["SRD"];
                Common.SaveErrorLog("Payment Response SRD:" + SRD + "_QuoteGUID:" + sQuoteID, "XIDNA");
                ViewBag.amount = amount1;
                ViewBag.Orderid = Orderid;
                ViewBag.QuoteID = ResList[0];
                Common.SaveErrorLog("Payment result:" + Result, "XIDNA");
                Common.SaveErrorLog("sha1hash:" + sha1hash, "XIDNA");
                Common.SaveErrorLog("sha1:" + sha1, "XIDNA");
                Common.SaveErrorLog("sGUID:" + ResList[1], "XIDNA");
                ViewBag.sGUID = ResList[1];
                //var sSessionID = HttpContext.Session.SessionID;
                //Common.SaveErrorLog("sSessionID:" + sSessionID, "XIDNA");
                var sQuoteCache = oCache.Get_ParamVal(sSID, ResList[1], null, "{XIP|iPaymentQuoteID}");
                if (string.IsNullOrEmpty(sQuoteCache))
                {
                    if (Result == "00" && sha1hash == sha1)
                    {
                        ViewBag.Status = "Success";
                        oCache.Set_ParamVal(sSID, ResList[1], "", "{XIP|iPaymentQuoteID}", sQuoteID, "", null);
                        var oPaymentData = CreatePaymentDetails(acthcode, parsef, sha1hash, amount1, ResList[0], ResList[1], 10, sSID, SRD, sResponseContent);
                        Common.SaveErrorLog("oPaymentDetails.oResult: " + oPaymentData.oResult, "XIDNA");
                        Common.SaveErrorLog("oPaymentDetails.xiStatus: " + oPaymentData.xiStatus, "XIDNA");
                        if (oPaymentData.oResult != null && oPaymentData.xiStatus == 0)
                        {
                            Common.SaveErrorLog("Success:  Payment Details Created Sucessfully", "XIDNA");
                            var oresult = (CResult)oPaymentData.oResult;
                            var oPayment = (XIIBO)oresult.oResult;
                            var id = "";
                            if (oPayment.Attributes.ContainsKey(oPayment.BOD.sPrimaryKey))
                            {
                                id = oPayment.Attributes[oPayment.BOD.sPrimaryKey].sValue;
                            }
                            Common.SaveErrorLog("PaymentDetails_T  id:" + id, "XIDNA");
                        }
                        return View("TestPaymentSuccess");
                        //return null;
                    }
                    else
                    {
                        ViewBag.Status = "Failure";
                        var oPaymentData = CreatePaymentDetails(acthcode, parsef, sha1hash, amount1, ResList[0], ResList[1], 20, sSID, SRD, sResponseContent);
                        var sGUID = Guid.NewGuid();
                        ViewBag.sGUID = sGUID;
                        Common.SaveErrorLog("Payment new guid: " + sGUID, "XIDNA");
                        Common.SaveErrorLog("oPaymentDetails.oResult: " + oPaymentData.oResult, "XIDNA");
                        Common.SaveErrorLog("oPaymentDetails.xiStatus: " + oPaymentData.xiStatus, "XIDNA");
                        //Payment(Orderid, amount1);
                        return View("Failure");
                    }
                }
                else
                {
                    ViewBag.Status = "Repeated";
                    Common.SaveErrorLog("sQuote:already existed in cache" + sQuoteID, "XIDNA");
                    if (Result == "00" && sha1hash == sha1)
                    {
                        return View("TestPaymentSuccess");
                    }
                    else
                    {
                        ViewBag.Status = "Failure";
                        var oPaymentData = CreatePaymentDetails(acthcode, parsef, sha1hash, amount1, ResList[0], ResList[1], 20, sSID, SRD, sResponseContent);
                        var sGUID = Guid.NewGuid();
                        ViewBag.sGUID = sGUID;
                        Common.SaveErrorLog("Payment new guid: " + sGUID, "XIDNA");
                        Common.SaveErrorLog("oPaymentDetails.oResult: " + oPaymentData.oResult, "XIDNA");
                        Common.SaveErrorLog("oPaymentDetails.xiStatus: " + oPaymentData.xiStatus, "XIDNA");
                        //Payment(Orderid, amount1);
                        return View("Failure");
                    }
                }
            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                Common.SaveErrorLog(ex.ToString(), "XIDNA");
                ViewBag.Failurer1 = ex.ToString();
                return View("Failure1");
            }
        }
        public CResult CreatePaymentDetails(string sActhCode, string sPasRef, string sSha1hash, string amount1, string sQuoteID, string sGUID, int iPaymentStatus, string sSessionID, string sSRD, string sResponseContent)
        {
            CResult oResult = new CResult();
            XIInfraCache oCache = new XIInfraCache();
            XIIXI oIXI = new XIIXI();
            try
            {
                int iNoOfAttempts = 1;
                Common.SaveErrorLog("CreatePaymentDetails Method Started executing", "XIDNA");
                //DateTime dDate = DateTime.Now.AddYears(-20).Year;
                XIIBO oBOI = new XIIBO();
                Dictionary<string, CNV> OQSD = new Dictionary<string, CNV>(StringComparer.CurrentCultureIgnoreCase);
                CNV oNV1 = new CNV();
                oNV1.sName = "sAuthCode";
                oNV1.sValue = sActhCode;
                OQSD.Add(oNV1.sName, oNV1);
                CNV oNV2 = new CNV();
                oNV2.sName = "sPasRef";
                oNV2.sValue = sPasRef;
                OQSD.Add(oNV2.sName, oNV2);
                CNV oNV3 = new CNV();
                oNV3.sName = "sSha1hash";
                oNV3.sValue = sSha1hash;
                OQSD.Add(oNV3.sName, oNV3);
                CNV oNV4 = new CNV();
                oNV4.sName = "rAmount";
                oNV4.sValue = amount1;
                OQSD.Add(oNV4.sName, oNV4);
                CNV oNV5 = new CNV();
                oNV5.sName = "FKsQuoteID";
                oNV5.sValue = sQuoteID;
                OQSD.Add(oNV5.sName, oNV5);
                List<CNV> oNV = new List<CNV>();
                oNV.Add(new CNV { sName = "sGUID", sValue = sQuoteID });
                var oQuoteI = oIXI.BOI("Aggregations", "", "", oNV);
                Common.SaveErrorLog("Aggregations loded successfully", "XIDNA");
                if (oQuoteI.Attributes != null && oQuoteI.Attributes.ContainsKey("id"))
                {
                    CNV oNV7 = new CNV();
                    oNV7.sName = "FKiQuoteID";
                    oNV7.sValue = oQuoteI.Attributes["id"].sValue;
                    OQSD.Add(oNV7.sName, oNV7);
                }
                CNV oNV6 = new CNV();
                oNV6.sName = "sGUID";
                oNV6.sValue = sGUID;
                OQSD.Add(oNV6.sName, oNV6);
                CNV oNV8 = new CNV();
                oNV8.sName = "iNoOfAttempts";
                oNV8.sValue = iNoOfAttempts.ToString();
                OQSD.Add(oNV8.sName, oNV8);
                OQSD.Add("sSRD", new CNV { sName = "sSRD", sValue = sSRD });
                OQSD.Add("sResponseContent", new CNV { sName = "sResponseContent", sValue = sResponseContent });

                XIIXI oXII = new XIIXI();
                List<CNV> oWhrParams = new List<CNV>();
                oWhrParams.Add(new CNV { sName = "FKsQuoteID", sValue = sQuoteID.ToString() });
                oWhrParams.Add(new CNV { sName = XIConstant.Key_XIDeleted, sValue = "0" });
                var oPaymentRequestI = oXII.BOI("PaymentRequest_T", null, "create", oWhrParams);
                if (oPaymentRequestI.Attributes != null && oPaymentRequestI.Attributes.ContainsKey("id"))
                {
                    CNV oNV9 = new CNV();
                    oNV9.sName = "FKiPaymentRequestID";
                    oNV9.sValue = oPaymentRequestI.Attributes["id"].sValue;
                    OQSD.Add(oNV9.sName, oNV9);
                }

                OQSD.Add("iStatus", new CNV { sName = "iStatus", sValue = iPaymentStatus.ToString() });
                var oPaymentResult = oBOI.BuildBoObject("PaymentDetails_T", "Create", OQSD, "");
                if (oPaymentResult.oResult != null && oPaymentResult.xiStatus == 0)
                {
                    List<CNV> oPaymentNV = new List<CNV>();
                    oPaymentNV.Add(new CNV { sName = "-PaymentQuoteID", sValue = sQuoteID });
                    oCache.SetXIParams(oPaymentNV, sGUID, sSessionID);
                    string sScript = "xi.s|{xi.count|'PaymentDetails_T',{xi.p|-PaymentQuoteID},'FKsQuoteID'}";
                    string sNoAttempts = "";
                    XIDScript oXIScript = new XIDScript();
                    oXIScript.sScript = sScript.ToString();
                    oResult = oXIScript.Execute_Script(sGUID, sSessionID);
                    if (oResult.bOK && oResult.oResult != null)
                    {
                        sNoAttempts = (string)oResult.oResult;
                        Common.SaveErrorLog("Payment No Of Attempts" + sNoAttempts, "XIDNA");
                    }
                    if (int.TryParse(sNoAttempts, out iNoOfAttempts))
                    { }
                    ViewBag.NoOfAttempts = iNoOfAttempts;

                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oResult.oResult = oPaymentResult.oResult;
                    oResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    oResult.oTraceStack.Add(new CNV { sName = "Create Payment Details", sValue = "Success: Payment Details Created Sucessfully" });
                }

            }
            catch (Exception ex)
            {
                oResult.sMessage = "ERROR: [" + oResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                Common.SaveErrorLog(ex.ToString(), "XIDNA");
                oResult.LogToFile();
            }
            return oResult;
        }


        //static PerformanceCounter ramCounter;
        //static PerformanceCounter cpuCounter;

        //static Timer timer;
        //static ManualResetEvent waiter = new ManualResetEvent(false);

        //static Performance lastMeasure = new Performance(); // the Model (in Mvc)

        //static PaymentController()
        //{
        //    // Get the current process
        //    using (var p = Process.GetCurrentProcess())
        //    {
        //        ramCounter = new PerformanceCounter("Process", "Working Set", p.ProcessName);
        //        cpuCounter = new PerformanceCounter("Process", "% Processor Time", p.ProcessName);
        //    }
        //    // make sure some time has passed before first NextValue call
        //    timer = new Timer(s =>
        //    {
        //        waiter.Set();
        //    }, null, 500, Timeout.Infinite);

        //    // clean-up
        //    AppDomain.CurrentDomain.DomainUnload += (s, e) => {
        //        var time = (IDisposable)timer;
        //        if (time != null) time.Dispose();
        //        var wait = (IDisposable)waiter;
        //        if (wait != null) wait.Dispose();
        //        var rc = (IDisposable)ramCounter;
        //        if (rc != null) rc.Dispose();
        //        var cc = (IDisposable)cpuCounter;
        //        if (cc != null) cc.Dispose();
        //    };
        //}

        //private static Performance GetReading()
        //{
        //    // wait for the first reading 
        //    waiter.WaitOne();
        //    // maybe cache its values for a few seconds
        //    lastMeasure.Cpu = cpuCounter.NextValue();
        //    lastMeasure.Ram = ramCounter.NextValue();
        //    return lastMeasure;
        //}

        ////
        //// GET: /Performance/
        //public ActionResult Performance()
        //{
        //    return View(GetReading());
        //}
    }
}