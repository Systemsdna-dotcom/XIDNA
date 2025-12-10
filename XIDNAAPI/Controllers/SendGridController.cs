using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using XICore;
using XIDatabase;
using XIDNA.Models;
using xiEnumSystem;
using XISystem;

namespace XIAPI.Controllers
{
    [RoutePrefix("api/SendGrid")]
    public class SendGridController : ApiController
    {
        public SendGridController()
        {

        }
        [HttpPost]
        [Route("Events_Campaign")]
        public IHttpActionResult Events_Campaign(SendGridEvents[] events)
        {
            string sMessageID = string.Empty;
            int StatusID = 0;
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            XIIBO oBOI = new XIIBO();
            XIIXI oXII = new XIIXI();
            XIInfraCache oCache = new XIInfraCache();
            XIDBO BOD = new XIDBO();
            foreach (SendGridEvents item in events)
            {
                List<CNV> oWhrParams = new List<CNV>();
                sMessageID = item.sg_message_id.Split('.')[0];
                oWhrParams.Add(new CNV { sName = "sReference", sValue = sMessageID });
                oBOI = oXII.BOI("communicationinstance", null, null, oWhrParams);
                if (oBOI?.Attributes != null)
                {
                    StatusID = (int)Enum.Parse(typeof(EnumEmailStatus), item.@event);
                    string sEventDate = oBOI.TimeStampToDateTime(Convert.ToDouble(item.timestamp));
                    bool bIsOpened = oBOI.Attributes["bIsOpened"].sValue == "0" ? true : false;
                    if (item.@event == EnumEmailStatus.open.ToString() && bIsOpened)
                    {
                        oBOI.SetAttribute("bIsOpened", "1");
                        oBOI.SetAttribute("OpenDT", sEventDate);
                    }
                    oBOI.SetAttribute("iInteractionStatus", StatusID.ToString()); //Save Email Activity in CommunicationInstance
                    var oResult = oBOI.Save(oBOI);
                    if (oResult.bOK)
                    {
                        int ID = 0;
                        int.TryParse(oBOI.Attributes["ID"].sValue, out ID);
                        if (ID > 0)
                        {
                            var CampaignID = oBOI.Attributes["FKiCampaignID"].sValue;
                            var LeadID = oBOI.Attributes["FKiLeadID"].sValue;


                            var ClickedUrl = item.url;
                            BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsTransaction"); //Save Email activity in commsTransaction
                            oBOI = new XIIBO();
                            oBOI.BOD = BOD;
                            oBOI.SetAttribute("FKiCommInstanceID", ID.ToString());
                            oBOI.SetAttribute("iType", StatusID.ToString());
                            oBOI.SetAttribute("CreateDate", sEventDate);
                            oBOI.SetAttribute("FKiCampaignID", CampaignID);
                            if (!string.IsNullOrEmpty(ClickedUrl))
                            {
                                oBOI.SetAttribute("sClickedUrl", ClickedUrl);
                            }

                            oCR = oBOI.Save(oBOI);
                            if (oCR != null)
                            {
                                //XIIBO CheckLead = new XIIBO();
                                //XIIXI CampaignedLead = new XIIXI();
                                XIIBO XLeadStatus = new XIIBO();
                                XIIXI XILeadCampaign = new XIIXI();
                                List<CNV> oWhrLeadParams = new List<CNV>();
                                //sMessageID = item.sg_message_id.Split('.')[0];
                                oWhrLeadParams.Add(new CNV { sName = "FKiLeadID", sValue = LeadID });
                                oWhrLeadParams.Add(new CNV { sName = "FKiCampaignID", sValue = CampaignID });
                                //CheckLead = CampaignedLead.BOI("CampLeadAssign", null, null, oWhrLeadParams);
                                //var iEmailSendStatus = CheckLead.Attributes["iEmailStatus"].sValue;
                                //if(iEmailSendStatus == "10")
                                //{
                                XLeadStatus = XILeadCampaign.BOI("XLeadCampaign", null, null, oWhrLeadParams);
                                XLeadStatus.SetAttribute("iLeadCampaignStatus", StatusID.ToString());
                                oCR = XLeadStatus.Save(XLeadStatus);
                                var XLeadCampainedID = XLeadStatus.Attributes["id"].sValue;

                                //}
                                //else
                                //{
                                // XLeadStatus = XILeadCampaign.BOI("XLeadCampaign", null, null, oWhrParams);
                                // XLeadStatus.SetAttribute("iLeadCampaignStatus", StatusID.ToString());
                                //oCR = XLeadStatus.Save(XLeadStatus);
                                //}
                                // LeadStatus1.SetAttribute("iExternalStatus", StatusID.ToString());
                                //oCR = LeadStatus1.Save(LeadStatus1);





                                if (oCR.bOK)
                                {
                                    var response = LoadFunnelCampaign(XLeadCampainedID, CampaignID, StatusID.ToString(), ClickedUrl, LeadID); //passing params to get Funnel based on Campaign
                                    if (response != null)
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "Lead : " + LeadID + "is not saved Funnel Stages and xLeadCampaign : " + CampaignID + "";
                                        oXID.SaveErrortoDB(oCResult);
                                    }
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Lead : " + LeadID + "is not saved in the xLeadCampaign with CampaignID : " + CampaignID + "";
                                    oXID.SaveErrortoDB(oCResult);
                                }

                            }
                            else
                            {
                                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                oCResult.sMessage = "Lead : " + LeadID + "is not saved in the commsTransaction with Campaign : " + CampaignID + "";
                                oXID.SaveErrortoDB(oCResult);
                            }
                        }
                    }
                }
            }
            // return Ok();
            return Content(HttpStatusCode.Accepted, "Url is Working");
        }

        [HttpPost]
        [Route("Events")]
        public IHttpActionResult Events(SendGridEvents[] events)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Write the send grid response to DB";//expalin about this method logic
            XIDefinitionBase oDef = new XIDefinitionBase();
            int iTrace = 0;
            string sMessage = string.Empty;
            string sCode = "SendGrid";
            try
            {
                iTrace = 100;
                oCResult.sMessage = "Send grid API Events method is working " + DateTime.Now + " - Events Data: " + JsonConvert.SerializeObject(events);
                oCResult.LogToFile();
                oDef.SaveErrortoDB(oCResult);
                //Save Email activity in commsTransaction object
                string sMessageID = string.Empty;
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsTransaction");
                var oBOI = new XIIBO();
                oBOI = new XIIBO();
                var sClickedUrl = string.Empty;
                foreach (SendGridEvents item in events)
                {
                    iTrace = 200;
                    List<CNV> oWhrParams = new List<CNV>();
                    sMessageID = item.sg_message_id.Split('.')[0];
                    int iEventType = (int)Enum.Parse(typeof(EnumEmailStatus), item.@event);
                    DateTime sEventDate = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    sEventDate = sEventDate.AddSeconds(Convert.ToDouble(item.timestamp)).ToLocalTime();
                    //string sEventDate = oBOI.TimeStampToDateTime(Convert.ToDouble(item.timestamp));
                    oTrace.oParams.Add(new CNV { sName = "Send Grid MessageID", sValue = sMessageID });
                    if (!string.IsNullOrEmpty(sMessageID))
                    {
                        oBOI = new XIIBO();
                        oBOI.BOD = BOD;
                        oBOI.SetAttribute("sSendGridReference", sMessageID.ToString());
                        oBOI.SetAttribute("iType", iEventType.ToString());
                        oBOI.SetAttribute("dtEvent", sEventDate.ToString());
                        oBOI.SetAttribute("iStatus", "10");
                        sClickedUrl = item.url;
                        if (!string.IsNullOrEmpty(sClickedUrl))
                        {
                            oBOI.SetAttribute("sClickedUrl", sClickedUrl);
                        }
                        oBOI.SetAttribute("sEvent", item.@event);
                        oBOI.SetAttribute("sEmail", item.email);
                        oBOI.SetAttribute("sCategory", item.category);
                        oBOI.SetAttribute("sResponse", item.response);
                        oBOI.SetAttribute("sAttempt", item.attempt);
                        oBOI.SetAttribute("sTimeStamp", item.timestamp);
                        oBOI.SetAttribute("sReason", item.response);
                        oBOI.SetAttribute("sStatus", item.status);
                        oBOI.SetAttribute("sType", item.type);
                        oBOI.SetAttribute("sUserAgent", item.useragent);
                        oBOI.SetAttribute("sIP", item.ip);
                        oBOI.SetAttribute("sg_event_id", item.sg_event_id);
                        oBOI.SetAttribute("sg_message_id", item.sg_message_id);
                        oCR = oBOI.Save(oBOI);
                        if (oCR != null && oCR.oResult != null)
                        {
                            iTrace = 300;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                            oCResult.oResult = "Success";
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - Send grid message reference is not resolved";
                        oDef.SaveErrortoDB(oCResult);
                    }
                }
            }
            catch (Exception ex)
            {
                iTrace = 400;
                sMessage = ex.ToString();
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + " - " + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oCResult.LogToFile();
                oDef.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return Content(HttpStatusCode.Accepted, "Events method called successfully " + iTrace + " " + sMessage);
        }

        [HttpPost]
        [Route("OneSendGridEvents")]
        public List<XIIBO> OneSendGridEvents(string sAppGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get Send grid events from remote DB to local DB";//expalin about this method logic
            Dictionary<string, XIIBO> oResult = new Dictionary<string, XIIBO>();
            XIDefinitionBase oXIDef = new XIDefinitionBase();
            string sCode = "SendGrid";
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XID1Click o1ClickD = new XID1Click();
                XIDStructure oStrD = new XIDStructure();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "RemoteCommsTransaction");
                if (o1ClickD != null && o1ClickD.ID > 0)
                {
                    List<CNV> nParams = new List<CNV>();
                    nParams.Add(new CNV { sName = "{XIP|AppID}", sValue = sAppGUID });
                    o1ClickD.Query = oStrD.ReplaceExpressionWithCacheValue(o1ClickD.Query, nParams);
                    oResult = o1ClickD.OneClick_Execute();
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + " - " + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXIDef.SaveErrortoDB(oCResult);
            }
            return oResult.Values.ToList();
        }

        [HttpPost]
        [Route("Tesst_ConnectionString")]
        public bool Tesst_ConnectionString(string sDBName)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get Send grid events from remote DB to local DB";//expalin about this method logic
            XIDefinitionBase oXIDef = new XIDefinitionBase();
            var bIsConnectable = false;
            string sCode = "SendGrid";
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                XIDXI oXID = new XIDXI();
                string sConnString = string.Empty;
                var oDataSrcD = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sDBName);
                XIEncryption oXIAPI = new XIEncryption();
                sConnString = oXIAPI.DecryptData(oDataSrcD.sConnectionString, true, oDataSrcD.XIGUID.ToString());
                XIDBAPI oDB = new XIDBAPI();
                bIsConnectable = oDB.Test_ConnectionString<SqlConnection>(sConnString);
                if (!bIsConnectable)
                {
                    oCResult.sMessage = "Unable to connect to Remote connection for Connection string: " + sConnString;
                    oCResult.sCategory = sCode;
                    oXID.SaveErrortoDB(oCResult);
                }
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + "-" + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXIDef.SaveErrortoDB(oCResult);
            }
            return bIsConnectable;
        }

        [HttpPost]
        [Route("OneSendGrid_UpdateStatus")]
        public IHttpActionResult OneSendGrid_UpdateStatus(List<string> RemoteGUIDs)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Update the import status of CommsTransaction instance in remote DB";//expalin about this method logic
            Dictionary<string, XIIBO> oResult = new Dictionary<string, XIIBO>();
            XIDefinitionBase oXID = new XIDefinitionBase();
            string sCode = "SendGrid";
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                string sStatus = string.Empty;
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsTransaction");
                string FailedIDs = string.Empty;
                string FailedGUIDs = string.Empty;
                foreach (var items in RemoteGUIDs)
                {
                    var Spilt = items.Split('_').ToList();
                    var sGUID = Spilt[0];
                    var Status = Spilt[1];
                    List<CNV> WhrParams = new List<CNV>();
                    WhrParams.Add(new CNV { sName = "xiguid", sValue = sGUID });
                    var oBOI = oXI.BOI("commsTransaction", null, null, WhrParams);
                    if (oBOI != null && oBOI.Attributes.Count() > 0)
                    {
                        var iCommID = oBOI.AttributeI("id").sValue;
                        oBOI.BOD = BOD;
                        oBOI.SetAttribute("iStatus", Status);
                        oCR = oBOI.Save(oBOI);
                        if (oCR.bOK && oCR.oResult != null)
                        {

                        }
                        else
                        {
                            //Log status updating issue
                            FailedIDs = FailedIDs + iCommID + ",";
                            sStatus = "Failure";
                        }
                    }
                    else
                    {
                        FailedGUIDs = FailedGUIDs + sGUID + ",";
                        sStatus = "Failure";
                    }
                }
                if (sStatus != "Failure")
                {
                    sStatus = "Success";
                }
                else
                {
                    if (!string.IsNullOrEmpty(FailedIDs))
                    {
                        oCResult.sMessage = "Communcation Transaction status updation failed for instances:" + FailedIDs.Substring(0, FailedIDs.Length - 1);
                        oCResult.sCategory = sCode;
                        oXID.SaveErrortoDB(oCResult);
                    }
                    if (!string.IsNullOrEmpty(FailedGUIDs))
                    {
                        oCResult.sMessage = "Communcation Transaction loading failed for instances:" + FailedGUIDs.Substring(0, FailedGUIDs.Length - 1);
                        oCResult.sCategory = sCode;
                        oXID.SaveErrortoDB(oCResult);
                    }
                }
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(sStatus, System.Text.Encoding.UTF8, "text/plain")
                };
                return ResponseMessage(httpResponseMessage);
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + "-" + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Failure", System.Text.Encoding.UTF8, "text/plain")
                };
                return ResponseMessage(httpResponseMessage);
            }
        }

        [HttpPost]
        [Route("SetAppToSGMessage")]
        public IHttpActionResult SetAppToSGMessage(List<CNV> oParams)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Map the application guid to CommsTransaction instance in remote DB";//expalin about this method logic
            Dictionary<string, XIIBO> oResult = new Dictionary<string, XIIBO>();
            XIDefinitionBase oXID = new XIDefinitionBase();
            string sCode = "SendGrid";
            List<CNV> oTraceInfo = new List<CNV>();
            try
            {
                string sStatus = string.Empty;
                XIIXI oXI = new XIIXI();
                XIInfraCache oCache = new XIInfraCache();
                var BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsAppMapping");
                var SGMessageID = oParams.Where(m => m.sName == "SGMessageID").Select(m => m.sValue).FirstOrDefault();
                var CommID = oParams.Where(m => m.sName == "CommID").Select(m => m.sValue).FirstOrDefault();
                var AppXIGUID = oParams.Where(m => m.sName == "AppXIGUID").Select(m => m.sValue).FirstOrDefault();
                if (!string.IsNullOrEmpty(SGMessageID) && !string.IsNullOrEmpty(CommID) && !string.IsNullOrEmpty(AppXIGUID))
                {
                    XIIBO oBOI = new XIIBO();
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("sSGMessageID", SGMessageID);
                    oBOI.SetAttribute("FKiCommID", CommID);
                    oBOI.SetAttribute("FKiAppIDXIGUID", AppXIGUID);
                    oCR = oBOI.Save(oBOI);
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        sStatus = "Success";
                    }
                    else
                    {
                        sStatus = "Failure";
                    }
                }
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(sStatus, System.Text.Encoding.UTF8, "text/plain")
                };
                return ResponseMessage(httpResponseMessage);
            }
            catch (Exception ex)
            {
                oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiError;
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oTrace.sMessage = "Line No:" + line + " - " + ex.ToString();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = sCode + "-" + ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oXID.SaveErrortoDB(oCResult);
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("Failure", System.Text.Encoding.UTF8, "text/plain")
                };
                return ResponseMessage(httpResponseMessage);
            }
        }

        public CResult LoadFunnelCampaign(string XLeadCampainedID, string CampaignID, string StatusID, string ClickedUrl, string LeadID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "This method is used to Load Funnel structure";//expalin about this method logic
            try
            {
                //oTrace.oParams.Add(new CNV { sName = "", sValue = "" });
                if (!string.IsNullOrEmpty(XLeadCampainedID) && !string.IsNullOrEmpty(StatusID) && !string.IsNullOrEmpty(CampaignID) && !string.IsNullOrEmpty(LeadID))//check mandatory params are passed or not
                {
                    string sCondition = string.Empty;
                    string sTableName = string.Empty;
                    XIInfraDynamicTreeComponent Tree = new XIInfraDynamicTreeComponent();
                    sCondition = "FKiCampaignID=" + CampaignID;
                    if (!string.IsNullOrEmpty(sCondition))
                    {
                        var sBOName = "Funnel";
                        oTrace.oParams.Add(new CNV { sName = "sBOName", sValue = sBOName });
                        if (!string.IsNullOrEmpty(sBOName))//check mandatory params are passed or not
                        {
                            XIInfraCache oCache = new XIInfraCache();
                            var oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, sBOName);
                            sTableName = oBOD.TableName;
                            XIIXI oXI = new XIIXI();
                            XID1Click oD1Click = new XID1Click();
                            string sQuery = string.Empty;
                            oD1Click.sParentWhere = sCondition;
                            sQuery = "select * from " + sTableName + " WHERE iParentID ='0' and " + XIConstant.Key_XIDeleted + " = 0";
                            oD1Click.Query = sQuery;
                            oD1Click.Name = sBOName;
                            var ParentFunnel = oD1Click.OneClick_Execute();
                            var TreeStructure = Tree.TreeBuilding(ParentFunnel, sBOName, sTableName);      //Get All funnel structure based on CampaignID
                            var ListOfTree = (List<XIIBO>)TreeStructure.oResult;
                            bool IsSaved = false;
                            foreach (var item in ListOfTree)
                            {
                                var FunnelID = "";
                                var CampaignStatus = item.Attributes["icampaignstatus"].sValue;
                                var Url = item.Attributes["sUrl"].sValue; //get the url from funnel stage if exist
                                int i1ClickID = 0;
                                int.TryParse(item.Attributes["FKi1ClickID"].sValue, out i1ClickID);
                                if (i1ClickID > 0)
                                {
                                    string sClickCondition = string.Empty;
                                    XID1Click o1ClickD = new XID1Click();
                                    XID1Click o1ClickC = new XID1Click();
                                    XIInfraCache oMyCache = new XIInfraCache();
                                    o1ClickD = (XID1Click)oMyCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                                    o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                                    sClickCondition = "ID=" + LeadID;
                                    o1ClickC.sParentWhere = sClickCondition;
                                    var oCRes = o1ClickC.OneClick_Execute();
                                    if (oCRes.Count > 0)
                                    {
                                        if (!string.IsNullOrEmpty(CampaignStatus))
                                        {
                                            if (CampaignStatus == StatusID)          //if 1Click satisfy then compare the status id 
                                            {
                                                if (string.IsNullOrEmpty(Url))
                                                {
                                                    FunnelID = item.Attributes["id"].sValue;
                                                }
                                                else if (!string.IsNullOrEmpty(Url) && Url == ClickedUrl)   //comparing the clicked url with funnel url
                                                {
                                                    FunnelID = item.Attributes["id"].sValue;
                                                }
                                                oCR = SaveFunnelInXLeadCampaign(XLeadCampainedID, FunnelID);
                                                if (oCR.bOK)
                                                {
                                                    IsSaved = true;
                                                }
                                            }
                                        }
                                        if (!IsSaved)
                                        {
                                            FunnelID = item.Attributes["id"].sValue;        // if no status ID and 1Click satify's
                                            oCR = SaveFunnelInXLeadCampaign(XLeadCampainedID, FunnelID);
                                            if (oCR.bOK)
                                            {
                                                IsSaved = true;
                                            }
                                            //break;
                                        }

                                    }
                                }
                                if (!IsSaved)
                                {
                                    if (CampaignStatus == StatusID)          //compare the status id 
                                    {
                                        if (string.IsNullOrEmpty(Url))
                                        {
                                            FunnelID = item.Attributes["id"].sValue;
                                        }
                                        else if (!string.IsNullOrEmpty(Url) && Url == ClickedUrl)   //comparing the clicked url with funnel url
                                        {
                                            FunnelID = item.Attributes["id"].sValue;
                                        }
                                        if (!string.IsNullOrEmpty(FunnelID))
                                        {
                                            oCR = SaveFunnelInXLeadCampaign(XLeadCampainedID, FunnelID);
                                            if (oCR.bOK)
                                            {
                                                IsSaved = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (oCR != null)
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Lead : " + LeadID + " is not saved in the Campaign : " + CampaignID + "";
                        oXID.SaveErrortoDB(oCResult);
                    }
                }

                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: " + XLeadCampainedID + " or " + StatusID + " or " + CampaignID + " are missing";
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
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }

        public CResult SaveFunnelInXLeadCampaign(string XLeadCampainedID, string FunnnelID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "This Method is Used to Save the Lead in XLeadCampaign Table based on Message status";//expalin about this method logic
            try
            {
                if (!string.IsNullOrEmpty(FunnnelID) && !string.IsNullOrEmpty(XLeadCampainedID))//check mandatory params are passed or not
                {
                    XIIBO XLeadStatus = new XIIBO();
                    XIIXI XILeadCampaign = new XIIXI();
                    List<CNV> oWhrParams = new List<CNV>();
                    //oWhrParams.Add(new CNV { sName = "sReference", sValue = sMessageID });
                    XLeadStatus = XILeadCampaign.BOI("XLeadCampaign", XLeadCampainedID);
                    XLeadStatus.SetAttribute("FKiFunnelID", FunnnelID);
                    //var XLeadCampID = XLeadStatus.Attributes["ID"].sValue;
                    oCR = XLeadStatus.Save(XLeadStatus); //Saving funnelID and Email status in XLeadCampaign
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        var LeadID = XLeadStatus.Attributes["FKiLeadID"].sValue;
                        var CampaignID = XLeadStatus.Attributes["FKiCampaignID"].sValue;
                        var response = LeadTransferInFunnelCycle(FunnnelID, LeadID, CampaignID, XLeadCampainedID); // Recording all Lead transactions in FunnelLifeCycle
                        if (response != null)
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Lead : " + LeadID + " is Not saved in XLeadCampaign with the Campaign ID : " + CampaignID + "";
                            oXID.SaveErrortoDB(oCResult);
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: " + FunnnelID + " or " + XLeadCampainedID + " are missing";
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
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;

        }

        public CResult LeadTransferInFunnelCycle(string FunnelID, string LeadID, string CampaignID, string XLeadCampainedID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIDefinitionBase oXID = new XIDefinitionBase();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Recording the lead life cycle of the funnel stages";//expalin about this method logic
            try
            {
                if (!string.IsNullOrEmpty(FunnelID) && !string.IsNullOrEmpty(LeadID) && !string.IsNullOrEmpty(CampaignID) && !string.IsNullOrEmpty(XLeadCampainedID))//check mandatory params are passed or not
                {
                    XIIBO oBOI = new XIIBO();
                    XIIXI oXII = new XIIXI();
                    XIInfraCache oCache = new XIInfraCache();
                    XIDBO BOD = new XIDBO();
                    BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "FunnelLifeCycle");
                    oBOI.BOD = BOD;
                    oBOI.SetAttribute("FKiLeadID", LeadID);
                    oBOI.SetAttribute("FKiFunnelID", FunnelID);
                    oBOI.SetAttribute("FKiCampaignID", CampaignID);
                    oBOI.SetAttribute("FKiXLeadCampID", XLeadCampainedID);
                    oCR = oBOI.Save(oBOI);              //Save all funnel transactions of a Lead
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Lead : " + LeadID + " is Not saved in FunneLifeCycle with the Campaign ID : " + CampaignID + "";
                        oXID.SaveErrortoDB(oCResult);
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: " + LeadID + " or " + FunnelID + " or " + CampaignID + " or " + XLeadCampainedID + " is missing";
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
                oXID.SaveErrortoDB(oCResult);
            }
            watch.Stop();
            oTrace.iLapsedTime = TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).TotalSeconds;
            oCResult.oTrace = oTrace;
            return oCResult;
        }


        //[Route("Events")]
        //[HttpPost]
        //public IHttpActionResult Events([FromBody]SendGridEvents[] events)
        //{
        //    string connection = ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString;
        //    List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
        //    CResult oCResult = new CResult();
        //    CResult oCR = new CResult();
        //    long iTraceLevel = 10;
        //    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
        //    if (iTraceLevel > 0)
        //    {
        //        oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
        //    }
        //    if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
        //    {
        //        oCResult.xiStatus = oCR.xiStatus;
        //        //oCResult.oTraceStack.Trace("Stage",sError)
        //    }
        //    //in the case of
        //    //xiEnumSystem.xiFuncResult.xiLogicalError
        //    oCResult.sMessage = "someone tried to do something they shouldnt";
        //    //var DetailRequest = Request.Content;

        //    //var DetailReuqest = JsonConvert.SerializeObject(Request, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        //    using (SqlConnection con = new SqlConnection(connection))
        //    {
        //        con.Open();
        //        try
        //        {
        //            foreach (var data in events)
        //            {
        //                tbl_sendGrid grid = new tbl_sendGrid();
        //                grid.sEvent = data.@event;
        //                grid.sAttempt = data.attempt;
        //                grid.sCategory = data.category;
        //                grid.sEmailAddress = data.email;
        //                grid.sEventDate = TimeStampToDateTime(Convert.ToDouble(data.timestamp));
        //                grid.sReason = data.reason;
        //                grid.sResponse = data.response;
        //                //grid.SendGridEventID = data.;
        //                grid.sStatus = data.status;
        //                grid.sUrl = data.url;
        //                grid.sSG_Event_ID = data.sg_event_id;
        //                grid.sSG_Message_ID = data.sg_message_id;
        //                grid.sUserAgent = data.useragent;
        //                grid.sIP = data.ip;


        //                values.Clear();
        //                foreach (var item in grid.GetType().GetProperties())
        //                {
        //                    values.Add(new KeyValuePair<string, string>(item.Name, item?.GetValue(grid)?.ToString()));
        //                }

        //                string xQry = getInsertCommand("SendGridEvents", values);
        //                SqlCommand cmdi = new SqlCommand(xQry, con);
        //                cmdi.ExecuteNonQuery();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            oCResult.sMessage = ex.Message + "____Inner___" + ex.InnerException + "____trace__" + ex.StackTrace;
        //            XIDefinitionBase xibase = new XIDefinitionBase();
        //            oCResult.LogToFile();
        //            xibase.SaveErrortoDB(oCResult);
        //        }
        //    }
        //    return Ok();
        //}
        //private static string getInsertCommand(string table, List<KeyValuePair<string, string>> values)
        //{
        //    string query = null;
        //    query += "INSERT INTO " + table + " ( ";
        //    foreach (var item in values)
        //    {
        //        query += item.Key;
        //        query += ", ";
        //    }
        //    query = query.Remove(query.Length - 2, 2);
        //    query += ") VALUES ( ";
        //    foreach (var item in values)
        //    {
        //        if (item.Key.GetType().Name == "System.Int") // or any other numerics
        //        {
        //            query += item.Value;
        //        }
        //        else
        //        {
        //            query += "'";
        //            query += item.Value;
        //            query += "'";
        //        }
        //        query += ", ";
        //    }
        //    query = query.Remove(query.Length - 2, 2);
        //    query += ")";
        //    return query;
        //}
        //private string TimeStampToDateTime(double timeStamp)
        //{
        //    DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        //    dateTime = dateTime.AddSeconds(timeStamp).ToLocalTime();
        //    return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        //}
    }

    public class SendGridEvents
    {
        public string @event { get; set; }
        public string email { get; set; }
        public string category { get; set; }
        public string response { get; set; }
        public string attempt { get; set; }
        public string timestamp { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string reason { get; set; }
        public string type { get; set; }
        public string useragent { get; set; }
        public string ip { get; set; }
        public string sg_event_id { get; set; }
        public string sg_message_id { get; set; }
    }
    public class tbl_sendGrid
    {

        //public int SendGridEventID { get; set; }

        public string sEvent { get; set; }

        public string sEmailAddress { get; set; }

        public string sCategory { get; set; }

        public string sResponse { get; set; }

        public string sAttempt { get; set; }

        public string sEventDate { get; set; }

        public string sUrl { get; set; }

        public string sStatus { get; set; }

        public string sReason { get; set; }

        public string sType { get; set; }
        public string sUserAgent { get; set; }
        public string sIP { get; set; }
        public string sSG_Event_ID { get; set; }
        public string sSG_Message_ID { get; set; }
        public string sDetailRequest { get; set; }
    }
}

