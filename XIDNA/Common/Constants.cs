//using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XICore;
//using XIDataBase.Hubs;
using XIDNA.Common;
using XIDNA.Repository;
using XISystem;

namespace XIDNA
{
    public class Constants
    {
        public const string Admin = "Admin";
        public const int SetupAdminMenuID = 1;
    }

    public class SignalR : iSiganlR
    {
        public void HitSignalR(string InstanceID, int ProductversionID, string sRoleName, string sDatabase, string sGUID, string sSessionID, int iQuoteType)
        {
            //string sDatabase = "XICoreQA";
            CommonRepository Common = new CommonRepository();
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var ConnectionID = oCache.Get_ParamVal(sSessionID, sGUID, "", "SignalRConnectionID");
                var Transtype = oCache.Get_ParamVal(sSessionID, sGUID, "", "-transtype");
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                var oQSInstance = (XIIQS)oCache.Get_QuestionSetCache("QuestionSetCache", sGUID, InstanceID.ToString());
                var oStepD = oQSInstance.QSDefinition.Steps.Values.Where(m => m.XIGUID == oQSInstance.iCurrentStepIDXIGUID).FirstOrDefault();
                var IsStepLock = false;
                if (oStepD.iLockStage != 0 && oStepD.iLockStage <= oQSInstance.iStage)
                {
                    IsStepLock = true;
                }
                Common.SaveErrorLog("Log: SignalREvent Method started Execution" + InstanceID + " " + ProductversionID, sDatabase);
                // NOTE: EXECUTE THE BELOW QUERIES HERE
                // Select ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote AS Yearly from Aggregations_T Where FKiQSInstanceID = 51787 and FKiProductVersionID = 2 and XIDeleted = 0
                //SELECT rBestQuote FROM Lead_T where FKiQSInstanceID=51787
                XIIBO oXiBo = new XIIBO();
                XIIBO oQuoteI = new XIIBO();
                QueryEngine oQE = new QueryEngine();
                //string sWhereCondition = $"FKiQSInstanceID={InstanceID},FKiProductVersionID={ProductversionID},XIDeleted=0";
                string sWhereCondition = "FKiQSInstanceIDXIGUID=" + InstanceID + ",FKiProductVersionID=" + ProductversionID + ",XIDeleted=0, iType=" + iQuoteType;

                var oQResult = oQE.Execute_QueryEngine("Aggregations", "sGUID,ID,iQuoteStatus,rCompulsoryExcess, rVoluntaryExcess, rTotalExcess, rMonthlyPrice, rMonthlyTotal, zDefaultDeposit, rFinalQuote, bIsFlood, bIsApplyFlood", sWhereCondition);
                if (oQResult.bOK && oQResult.oResult != null)
                {
                    oXiBo = ((Dictionary<string, XIIBO>)oQResult.oResult).Values.OrderByDescending(f => f.AttributeI("ID").iValue).FirstOrDefault();
                }
                if (oXiBo == null)
                {
                    string sWhereQuote = "FKiQSInstanceIDXIGUID=" + InstanceID;
                    var oQuoteResult = oQE.Execute_QueryEngine("Aggregations", "ID,iQuoteStatus", sWhereQuote);
                    if (oQuoteResult.bOK && oQuoteResult.oResult != null)
                    {
                        oQuoteI = ((Dictionary<string, XIIBO>)oQuoteResult.oResult).Values.FirstOrDefault();
                    }
                }
                // Get the product id from ProductVersionID
                XIIBO oXiProduct = new XIIBO();
                XIIXI oIXI = new XIIXI();
                oXiProduct = oIXI.BOI("ProductVersion_T", ProductversionID.ToString(), "FKiProductID,bIsIndicativePrice");
                int iProductID = 0; bool bIsIndicativePrice = false;
                if (oXiProduct != null && oXiProduct.Attributes.ContainsKey("FKiProductID"))
                {
                    iProductID = oXiProduct.AttributeI("FKiProductID").iValue;
                    bIsIndicativePrice = oXiProduct.AttributeI("bIsIndicativePrice").bValue;
                }
                // QUERY FOR BEST QUOTE. 
                XIIBO OLeadBO = new XIIBO();

                string sWhereLead = "FKiQSInstanceIDXIGUID=" + InstanceID;

                var oLeadQResult = oQE.Execute_QueryEngine("Lead_T", "rBestQuote", sWhereLead);
                if (oLeadQResult.bOK && oLeadQResult.oResult != null)
                {
                    OLeadBO = ((Dictionary<string, XIIBO>)oLeadQResult.oResult).Values.FirstOrDefault();
                }
                // BUILD ONE STANDARD ANNONYMOUS OBJECT HERE
                if (oXiBo != null && OLeadBO != null)
                {
                    var oAnnonymous = new
                    {
                        IsLockStep = IsStepLock,
                        ProductversionID = ProductversionID,
                        QSInstanceID = InstanceID,
                        ProductID = iProductID,
                        iQuoteStatus = oXiBo.AttributeI("iQuoteStatus").sValue,
                        rCompulsoryExcess = oXiBo.AttributeI("rCompulsoryExcess").rValue,
                        rVoluntaryExcess = oXiBo.AttributeI("rVoluntaryExcess").rValue,
                        rTotalExcess = oXiBo.AttributeI("rTotalExcess").rValue,
                        rMonthlyPrice = oXiBo.AttributeI("rMonthlyPrice").rValue,
                        rMonthlyTotal = oXiBo.AttributeI("rMonthlyTotal").rValue,
                        zDefaultDeposit = oXiBo.AttributeI("zDefaultDeposit").rValue,
                        rFinalQuote = oXiBo.AttributeI("rFinalQuote").rValue,
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QuoteID = oXiBo.AttributeI("sGUID").sValue,
                        iQuoteID = oXiBo.AttributeI("ID").sValue,
                        RoleName = sRoleName,
                        bIsIndicativePrice = bIsIndicativePrice,
                        sQSType = oQSInstance.sQSType,
                        bIsFlood = oXiBo.AttributeI("bIsFlood").sValue,
                        bIsApplyFlood = oXiBo.AttributeI("bIsApplyFlood").sValue,
                        sTranstype = Transtype
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    //ar hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    //hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                    //string ConnectionID = "";
                    //hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
                else if (OLeadBO != null)
                {
                    var oAnnonymous = new
                    {
                        rBestQuote = OLeadBO.AttributeI("rBestQuote").rValue,
                        QSInstanceID = InstanceID,
                        iQuoteStatus = oQuoteI.AttributeI("iQuoteStatus").sValue,
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed rBestQuote addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    ////hubContext.Clients.All.addNewMessageToPage(oAnnonymous);
                    //hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
                else if (oXiBo != null)
                {
                    var oAnnonymous = new
                    {
                        IsLockStep = IsStepLock,
                        ProductversionID = ProductversionID,
                        QSInstanceID = InstanceID,
                        ProductID = iProductID,
                        iQuoteStatus = oXiBo.AttributeI("iQuoteStatus").sValue,
                        rCompulsoryExcess = oXiBo.AttributeI("rCompulsoryExcess").rValue,
                        rVoluntaryExcess = oXiBo.AttributeI("rVoluntaryExcess").rValue,
                        rTotalExcess = oXiBo.AttributeI("rTotalExcess").rValue,
                        rMonthlyPrice = oXiBo.AttributeI("rMonthlyPrice").rValue,
                        rMonthlyTotal = oXiBo.AttributeI("rMonthlyTotal").rValue,
                        zDefaultDeposit = oXiBo.AttributeI("zDefaultDeposit").rValue,
                        rFinalQuote = oXiBo.AttributeI("rFinalQuote").rValue,
                        QuoteID = oXiBo.AttributeI("sGUID").sValue,
                        RoleName = sRoleName,
                        bIsIndicativePrice = bIsIndicativePrice,
                        sQSType = oQSInstance.sQSType,
                        bIsFlood = oXiBo.AttributeI("bIsFlood").sValue,
                        bIsApplyFlood = oXiBo.AttributeI("bIsApplyFlood").sValue,
                        sTranstype = Transtype
                    };
                    Common.SaveErrorLog("Log: SignalREvent Method Executed addNewMessageToPage " + InstanceID + " " + ProductversionID, sDatabase);
                    //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
                    //hubContext.Clients.Client(ConnectionID).addNewMessageToPage(oAnnonymous);
                }
            }
            catch (Exception ex)
            {
                //logger.Error(ex);
                Common.SaveErrorLog("ErrorLog: SignalREvent" + ex.ToString(), sDatabase);
                throw ex;
            }
        }

        public void ShowSignalRMsg(string sMessage)
        {
            XIInfraCache oCache = new XIInfraCache();
            var connid = SessionManager.sSignalRCID;
            if (string.IsNullOrEmpty(connid))
            {
                connid = Guid.NewGuid().ToString();
            }
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            ////hubContext.Clients.Client(connid).addNewMessageToPage(sMessage);
            //hubContext.Clients.All.addNewMessageToPage(sMessage);
        }

        public void ShowSignalRUserMsg(string sMessage)
        {
            XIInfraCache oCache = new XIInfraCache();
            //var ConnectionID = oCache.Get_ParamVal(sSessionID, sGUID, "", "SignalRConnectionID");
            var connid = SessionManager.sSignalRCID;
            if (string.IsNullOrEmpty(connid))
            {
                connid = Guid.NewGuid().ToString();
            }
            //var hubContext = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            ////hubContext.Clients.Client(connid).addNewMessageToPage(sMessage);
            ////hubContext.Clients.All.addNewMessageToPage(sMessage);
            //hubContext.Clients.All.addUserMessage(sMessage);
        }
        public void ListRefresh(dynamic sigRObj)
        {
            //var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            //// Call the addNewMessageToPage method to update clients.
            //context.Clients.All.ListRefresh(sigRObj);
        }
        public void ListAddRefresh(dynamic sigRObj)
        {
           // var context = GlobalHost.ConnectionManager.GetHubContext<NotifyHub>();
            // Call the addNewMessageToPage method to update clients.
           // context.Clients.All.ListAddRefresh(sigRObj);
        }
    }

    public class APIInvoke
    {
        public CResult SetAppToSGMessage(List<CNV> oParams)
        {
            CResult oCR = new CResult();
            var UpdateResponse = API.PostListGetString(oParams, "api/SendGrid/SetAppToSGMessage");
            if (!string.IsNullOrEmpty(UpdateResponse) && UpdateResponse == "Success")
            {
                oCR.oResult = "Success";
            }
            else
            {
                oCR.oResult = "Failure";
            }
            oCR.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
            return oCR;
        }

        public async Task WebhookOutbound(string sIdentifier, string sParam1 = "", string sParam2 = "", string sParam3 = "", string sParam4 = "", string sParam5 = "")
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            CTraceStack oTrace = new CTraceStack();
            XIDefinitionBase oDefBase = new XIDefinitionBase();
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                if (!string.IsNullOrEmpty(sIdentifier))
                {
                    var sParam = string.Empty;
                    XIIXI oXI = new XIIXI();
                    List<CNV> oWhrParams = new List<CNV>();
                    oWhrParams.Add(new CNV { sName = "sIdentifier", sValue = sIdentifier });
                    oWhrParams.Add(new CNV { sName = "iDirection", sValue = "20" });
                    var oWebhookI = new XIIBO();
                    oWebhookI = oXI.BOI("onewebhook", null, null, oWhrParams);
                    if (oWebhookI != null && oWebhookI.Attributes.Count() > 0)
                    {
                        var sConcatenator = oWebhookI.AttributeI("sConcatenator").sValue;
                        var sOutboundURL = oWebhookI.AttributeI("sOutboundURL").sValue;
                        var sHeader = oWebhookI.AttributeI("sHeader").sValue;
                        var sParam1Name = oWebhookI.AttributeI("sParam1").sValue;
                        var sParam2Name = oWebhookI.AttributeI("sParam2").sValue;
                        var sParam3Name = oWebhookI.AttributeI("sParam3").sValue;
                        var sParam4Name = oWebhookI.AttributeI("sParam4").sValue;
                        var sParam5Name = oWebhookI.AttributeI("sParam5").sValue;
                        if (!string.IsNullOrEmpty(sConcatenator) && string.IsNullOrEmpty(sOutboundURL) && string.IsNullOrEmpty(sHeader))
                        {
                            sParam = sParam1 == null ? null : sParam + sParam1Name + "=" + sParam1 + sConcatenator;
                            sParam = sParam2 == null ? null : sParam + sParam2Name + "=" + sParam2 + sConcatenator;
                            sParam = sParam3 == null ? null : sParam + sParam3Name + "=" + sParam3 + sConcatenator;
                            sParam = sParam4 == null ? null : sParam + sParam4Name + "=" + sParam4 + sConcatenator;
                            sParam = sParam5 == null ? null : sParam + sParam5Name + "=" + sParam5 + sConcatenator;
                            if (!string.IsNullOrEmpty(sParam))
                            {
                                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sParam is: " + sParam });
                                sParam = sParam.Substring(0, sParam.Length - sConcatenator.Length);
                                using (var client = new HttpClient())
                                {
                                    //Load Step1
                                    client.BaseAddress = new Uri("http://localhost:63722/");
                                    client.DefaultRequestHeaders.Accept.Clear();
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                                    string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("Test:Test123")); //("Username:Password")  
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                                    #region Consume GET method  
                                    HttpResponseMessage response = await client.GetAsync("api/" + sOutboundURL + "?sParam=" + sParam);
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var httpResponseResult = await response.Content.ReadAsStringAsync();
                                        List<CNV> oResponse = JsonConvert.DeserializeObject<List<CNV>>(httpResponseResult);
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sParam is empty" });
                            }
                        }
                        else
                        {
                            oTraceInfo.Add(new CNV { sValue = "Mandatory parameters sConcatenator :" + sConcatenator + " or sOutboundURL:" + sOutboundURL + " or sHeader:" + sHeader + " are empty" });
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "outbound webhook not found for Identifier:" + sIdentifier });
                    }
                }
                else
                {
                    oTraceInfo.Add(new CNV { sValue = "Mandatory parameter sIdentifier is empty" });
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oTraceInfo.Add(new CNV { sValue = oCResult.sMessage });
                oCResult.oTraceStack = oTraceInfo;
                oDefBase.SaveErrortoDB(oCResult);
            }
        }
    }
}