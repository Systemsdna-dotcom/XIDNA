using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.ViewModels;
//using Microsoft.AspNet.SignalR;
//using XIDataBase.Hubs;
using System.Data.SqlClient;
using System.Data.Entity;
using System.IO;
using XIDNA.Common;
using ZeeInsurance;
using System.Configuration;
using XICore;
using XISystem;
using System.Diagnostics;
using System.Reflection;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace XIDNA.Controllers
{
    // [System.Web.Mvc.Authorize]
    public class MailController : Controller
    {
        LeadImport LeadImport = new LeadImport();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IMailRepository MailRepository;

        public MailController() : this(new MailRepository()) { }

        public MailController(MailRepository MailRepository)
        {
            this.MailRepository = MailRepository;
        }
        CommonRepository Common = new CommonRepository();
        XIInfraCache oCache = new XIInfraCache();
        //
        // GET: /Mail/
        public ActionResult Index()
        {
            string sDatabase = SessionManager.ConfigDatabase;
            try
            {
                //int OrgID = 0;
                //var Mail = MailRepository.GetEmailCredentials(OrgID);
                IOServerDetails Mail = new IOServerDetails();
                XID1Click PV1Click = new XID1Click();
                XIDStructure oStructure = new XIDStructure();
                List<CNV> nParams = new List<CNV>();

                var o1ClickIO = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "IOServerDetails", null);
                PV1Click = (XID1Click)o1ClickIO.Clone(o1ClickIO);
                var oQueryIO = oStructure.ReplaceExpressionWithCacheValue(o1ClickIO.Query, nParams);
                PV1Click.Query = oQueryIO;
                PV1Click.Name = "XIIOServerDetails_T";
                var oOneClickIO = PV1Click.OneClick_Execute();

                var o1ClickAPP = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, "AppSettings", null);
                PV1Click = (XID1Click)o1ClickAPP.Clone(o1ClickAPP);
                var oQueryAPP = oStructure.ReplaceExpressionWithCacheValue(o1ClickAPP.Query, nParams);
                PV1Click.Query = oQueryAPP;
                PV1Click.Name = "XIAPISettings_T";
                var oOneClickAPP = PV1Click.OneClick_Execute();
                List<string> ListApp = new List<string>();
                ListApp = oOneClickAPP.ToList().Select(m => m.Value.Attributes.Where(s => s.Key == "scode").Select(s => s.Value.sValue).FirstOrDefault()).ToList();
                Dictionary<int, string> ListIO = new Dictionary<int, string>();
                for (int j = 0; j < oOneClickIO.Values.Count(); j++)
                {
                    var Value = oOneClickIO.Values.ElementAt(j).Attributes.Values.Where(s => s.sName == "ID").Select(s => s.iValue).FirstOrDefault();
                    var text = oOneClickIO.Values.ElementAt(j).Attributes.Values.Where(s => s.sName.ToLower() == "fromaddress").Select(s => s.sPreviousValue).FirstOrDefault();
                    ListIO.Add(Value, text);
                }
                VMDropDown Mails = new VMDropDown();
                Mail.MailIDs = new List<VMDropDown>();
                var APPIO = ListApp.Where(b => ListIO.Any(a => b.Contains(a.Value)));
                if (APPIO != null)
                {
                    foreach (var item in APPIO)
                    {
                        int Value = ListIO.Where(s => s.Value == item).Select(s => s.Key).FirstOrDefault();
                        var text = item;
                        Mails.Value = Value;
                        Mails.text = text;
                        Mail.MailIDs.Add(Mails);
                    }
                }
                return View(Mail);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Display partial view for email to display on button click 
        public ActionResult ImportEmails()
        {
            return View();
        }

        //Getting folder list
        [HttpPost]
        public ActionResult SelectFoldersWithIMAP(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var sFolderList = LeadImport.Select_FoldersWithIMAP(ID, OrgID);
                return Json(sFolderList.oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Getting Subject
        public ActionResult GetSubjectWithIMAP(int ID, string sFolder)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                string Flag = string.Empty;
                var sSubjects = LeadImport.Get_EmailSubjects(ID, sFolder, OrgID, Flag);
                return Json(sSubjects.oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //On selecting subject get email details which is saved to DB
        public ActionResult GetEmailDetailsByUID(int ID, int iUID, string sFolder, string sSubject)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var sEmailDetails = LeadImport.Get_EmailFullDetails(ID, iUID, sFolder);
                return Json(sEmailDetails.oResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SaveSelectedEmailByUID(int ID, int iUID, string sFolder)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {

                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                var Response = LeadImport.Save_MailContent(ID, iUID, sFolder);
                if (Response.bOK && Response.oResult != null)
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failure", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                var LeadDetails = new List<List<string>>();
                List<string> NoData = new List<string>();
                NoData.Add("Error while saving mail");
                LeadDetails.Add(NoData);
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(LeadDetails, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MailExtractStrings()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int orgid = 0;
                return PartialView("_MailExtractStrings", orgid);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public ActionResult AddEditMailExtractStrings(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (ID == 0)
                {
                    int orgid = 0;
                    VMMailExtractStrings model = new VMMailExtractStrings();
                    model.SubscriptionList = MailRepository.AddEditMailExtractStrings(orgid, sDatabase);
                    return View("AddEditMailExtractStrings", model);
                }
                else
                {
                    int orgid = 0;

                    var row = MailRepository.GetMailExtractStringsRow(ID, orgid, sDatabase);
                    VMMailExtractStrings model = new VMMailExtractStrings();
                    model.SubscriptionList = MailRepository.AddEditMailExtractStrings(orgid, sDatabase);

                    model.ID = ID;
                    model.SubscriptionID = row.SubscriptionID;
                    model.sStartString = row.sStartString;
                    model.sEndString = row.sEndString;
                    model.SourceID = row.SourceID;
                    model.StatusTypeID = row.StatusTypeID;
                    model.OrganizationID = row.OrganizationID;
                    return View("AddEditMailExtractStrings", model);
                }
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        [HttpPost]
        public ActionResult SaveMailExtractStrings(VMMailExtractStrings model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int orgid = 0;
                model.OrganizationID = orgid;
                var result = MailRepository.SaveMailExtractStrings(model, orgid, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult MailExtractStringsGrid(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = MailRepository.MailExtractStringsGrid(param, OrgID, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult MailExtractStringsPopUp(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PartialView("_MailExtractStringsPopUp", OrgID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        public ActionResult AppNotifications()
        {
            return View();
        }

        public ActionResult AddAppNotification()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMAppNotifications model = new VMAppNotifications();
                model.Roles = MailRepository.GetOrgRoles(0, sDatabase);
                model.GetUsers = MailRepository.GetUsers(0, sDatabase);
                return PartialView("_AddNotificationForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AppNotificationsGrid(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = MailRepository.AppNotificationsGrid(param, OrgID, sDatabase);
                return Json(new
                {
                    sEcho = result.sEcho,
                    iTotalRecords = result.iTotalRecords,
                    iTotalDisplayRecords = result.iTotalDisplayRecords,
                    aaData = result.aaData
                },
                JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        [HttpPost]
        public ActionResult SaveAppNotification(VMAppNotifications model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                model.OrganizationID = 0;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                var Res = MailRepository.SaveAppNotification(model, UserID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult EditAppNotification(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var row = MailRepository.GetOrgRoles(OrgID, sDatabase);
                var obj = MailRepository.EditAppNotifications(ID, sDatabase);
                obj.GetUsers = MailRepository.GetUsers(obj.OrganizationID, sDatabase);
                obj.Roles = row;
                return View("_AddNotificationForm", obj);
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        //public ActionResult UploadFiles(int id, HttpPostedFileBase ImageName)
        //{
        //    try
        //    {
        //        if (ImageName != null)
        //        {
        //            string ext = Path.GetExtension(ImageName.FileName);
        //            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
        //            string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
        //            var Image = "Notification_" + id + ext;
        //            ImageName.SaveAs(str + "\\" + Image);
        //            var res = MailRepository.SaveNotificationImage(id, Image, Util.GetDatabaseName());
        //        }
        //        return Content(id.ToString(), "text/plain");
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return Content("0", "text/plain");
        //    }
        //}

        public ActionResult GetUsers(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Name = MailRepository.GetUsers(OrgID, sDatabase);
                return Json(Name, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SendNotificationForAndroid(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var row = MailRepository.GetOrgRoles(OrgID, sDatabase);
                var Res = MailRepository.SendNotification(ID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
                //obj.GetUsers = MailRepository.GetUsers(obj.OrganizationID);
                //obj.Roles = row;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SendMail(List<CNV> Params, string sBOName)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            try
            {
                int ID = 0;
                XIIXI oXII = new XIIXI();
                XIIBO oBOI = new XIIBO();
                XIIXI LeadData = new XIIXI();
                List<CNV> oParams = new List<CNV>();
                XIDefinitionBase oXID = new XIDefinitionBase();
                //XIIBO oBOI1 = new XIIBO();
                XIInfraSendGridComponent oSendGrid = new XIInfraSendGridComponent();

                foreach (var item in Params)
                {
                    int.TryParse(item.sValue, out ID);
                    if (ID > 0)
                    {
                        //oParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                        var LeadInfo = LeadData.BOI(sBOName, ID.ToString());
                        var LeadID = LeadInfo.Attributes["FKiLeadID"].sValue;
                        var CampaignID = LeadInfo.Attributes["FKiCampaignID"].sValue;
                        //List<CNV> oWhrParams = new List<CNV>();
                        //oWhrParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                        //oWhrParams.Add(new CNV { sName = "ID", sValue = LeadID });
                        //oBOI = oXII.BOI(sBOName, ID.ToString());
                        oBOI = oXII.BOI("CampLead", LeadID);
                        if (oBOI.Attributes?.Count() > 0)
                        {
                            oSendGrid.sTo = oBOI.Attributes["sEmail"].sValue;
                            oSendGrid.sName = oBOI.Attributes["sName"].sValue;
                            //var CampaignID = oBOI.Attributes["FKiCampaignID"].sValue;
                            oSendGrid.oDynamicData = new
                            {
                                subject = "Send Grid Using Dynamic Template",
                                header = "dynamic header",
                                name = oSendGrid.sName,
                                address = "west godavari",
                                emailID = oSendGrid.sTo,
                                //url = "https://www.google.com/",
                                //buttonName = "Go to Google"
                            };

                            var listParams = new List<CNV>();
                            listParams.Add(new CNV { sName = "LeadID", sValue = LeadID });
                            listParams.Add(new CNV { sName = "CampaignID", sValue = CampaignID });
                            listParams.Add(new CNV { sName = "SendGridAccountID", sValue = "2" });
                            listParams.Add(new CNV { sName = "SendGridTemplateName", sValue = "MyTemplate" });
                            oCR = oSendGrid.Load(listParams);
                            if (oCR.bOK)
                            {
                                var MessageResponse = oCR.oResult.ToString().Split(',').ToList();
                                string sMessageID = MessageResponse[0];
                                string sSingleSender = MessageResponse[1];
                                if (!string.IsNullOrEmpty(sMessageID))
                                {
                                    XIIBO LeadEmailStatus = new XIIBO();
                                    XIIXI LeadStatus = new XIIXI();
                                    List<CNV> LeadParams = new List<CNV>();
                                    LeadParams.Add(new CNV { sName = "FKiLeadID", sValue = LeadID });
                                    LeadParams.Add(new CNV { sName = "FKiCampaignID", sValue = CampaignID });
                                    LeadEmailStatus = LeadStatus.BOI("CampLeadAssign", null, null, LeadParams);
                                    LeadEmailStatus.SetAttribute("iEmailStatus", "10");
                                    oCR = LeadEmailStatus.Save(LeadEmailStatus);
                                    if (oCR.bOK)
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                    }
                                    else
                                    {
                                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                        oCResult.sMessage = "Lead : " + LeadID + " is Not Saved in CampLeadAssign";
                                        oXID.SaveErrortoDB(oCResult);
                                    }
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Mail Not sent to Lead : " + LeadID + "";
                                    oXID.SaveErrortoDB(oCResult);
                                }
                                //int iSGADID = 0;
                                //var oSendGridInfo = (XIDSendGridAccountDetails)oCache.GetObjectFromCache(XIConstant.CacheSendGridAccount, null, "2");
                                //var sSingleSender = oSendGridInfo?.sSingleSender;
                                oCR = oSendGrid.SaveEmailActivity(sMessageID, sSingleSender, oSendGrid.sTo, LeadID, CampaignID);
                                //oCR = SaveEmailActivity(sMessageID.ToString(), sSingleSender, oSendGrid.sTo, LeadID, CampaignID);
                                if (oCR.bOK)
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Lead : " + LeadID + " of Email Activity is Not Saved in communicationinstance";
                                    oXID.SaveErrortoDB(oCResult);
                                }
                            }
                            else
                            {
                                XIDBO BOD = new XIDBO();
                                BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XLeadCampaign");
                                XIIBO oLeadBOI = new XIIBO();
                                oLeadBOI.BOD = BOD;
                                oLeadBOI.SetAttribute("fkileadid", LeadID);
                                oLeadBOI.SetAttribute("fkicampaignid", CampaignID);
                                oLeadBOI.SetAttribute("sReference", "Email not Sent Due to Technical issues");
                                //oBOI.SetAttribute("FKiCommInstanceID","");
                                oLeadBOI.SetAttribute("ileadcampaignstatus", "20");
                                oLeadBOI.SetAttribute("iStatus", "20");
                                //oBOI.SetAttribute("fkifunnelid", "");
                                oLeadBOI.Save(oBOI);
                                if (oLeadBOI != null)
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                                }
                                else
                                {
                                    oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                                    oCResult.sMessage = "Lead : " + LeadID + " is Not Saved in CampLeadAssign";
                                    oXID.SaveErrortoDB(oCResult);
                                }
                            }
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Lead info Bo is not loaded for the lead id: " + LeadID + "";
                            oXID.SaveErrortoDB(oCResult);
                        }
                    }
                    else
                    {
                        oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                        oCResult.sMessage = "Lead : " + ID + " is Missing to SendMail";
                        oXID.SaveErrortoDB(oCResult);
                    }
                }
            }
            catch (Exception ex)
            {
                XIInstanceBase oInstanceBase = new XIInstanceBase();
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oInstanceBase.SaveErrortoDB(oCResult);

            }

            return Json(oCResult.oResult, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public CResult AddLeadToCampaign(List<CNV> Params, string sGUID)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            XIIBO LeadDetails = new XIIBO();
            XIIXI LeadCampaign = new XIIXI();
            XIDefinitionBase oXID = new XIDefinitionBase();
            XIIBO oBOI = new XIIBO();
            XIDBO BOD = new XIDBO();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Campaigning the Leads";//expalin about this method logic
            try
            {

                if (Params != null && !string.IsNullOrEmpty(sGUID))//check mandatory params are passed or not
                {
                    var sSessionID = HttpContext.Session.SessionID;
                    XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                    string CampaignID = oGUIDParams.NMyInstance.Where(m => m.Key == "{XIP|iInstanceID}").Select(m => m.Value.sValue).FirstOrDefault();
                    foreach (var Lead in Params)
                    {
                        XIIBO ExistingLead = new XIIBO();
                        XIIXI XLead = new XIIXI();
                        List<CNV> oMyWhrParams = new List<CNV>();
                        oMyWhrParams.Add(new CNV { sName = "FKiLeadID", sValue = Lead.sValue });
                        oMyWhrParams.Add(new CNV { sName = "FKiCampaignID", sValue = CampaignID });
                        ExistingLead = XLead.BOI("CampLeadAssign", null, null, oMyWhrParams);
                        if (ExistingLead == null)
                        {
                            BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "CampLeadAssign"); //Save Email activity in commsTransaction
                            oBOI = new XIIBO();
                            oBOI.BOD = BOD;
                            oBOI.SetAttribute("FKiLeadID", Lead.sValue);
                            oBOI.SetAttribute("FKiCampaignID", CampaignID);
                            oCResult.oResult = oBOI.Save(oBOI);
                        }
                        //XIIBO LeadInfo = new XIIBO();
                        //XIIXI XILead = new XIIXI();
                        //List<CNV> oWhrParams = new List<CNV>();
                        ////oWhrParams.Add(new CNV { sName = "FKiLeadID", sValue = Lead.sValue });
                        ////oWhrParams.Add(new CNV { sName = "FKiCampaignID", sValue = CampaignID });
                        //LeadInfo = XILead.BOI("CampLead", Lead.sValue);
                        //var LeadGuidID = LeadInfo.Attributes["XIGUID"].sValue;
                        //var LeadEmail = LeadInfo.Attributes["sEmail"].sValue;
                        //var LeadName = LeadInfo.Attributes["sName"].sValue;
                        //BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XICommunicationI"); //Save Email activity in commsTransaction
                        //oBOI = new XIIBO();
                        //oBOI.BOD = BOD;
                        //oBOI.SetAttribute("XIInstanceOrigin", LeadGuidID);
                        //oBOI.SetAttribute("FKiTemplateID", "2");
                        //oBOI.SetAttribute("sTo", LeadEmail);
                        //oBOI.SetAttribute("sName", LeadName);
                        //oBOI.SetAttribute("FKiComTypeID", "2");
                        //oBOI.SetAttribute("iDirection", "20");
                        //oBOI.SetAttribute("iComType", "20");
                        //oBOI.SetAttribute("XIOrigin", "");
                        //oCResult.oResult = oBOI.Save(oBOI);
                        if (oCResult.oResult != null)
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiSuccess;
                        }
                        else
                        {
                            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                            oCResult.sMessage = "Lead : " + Lead.sValue + "is Not Assigned to Campaign";
                            oXID.SaveErrortoDB(oCResult);
                        }
                    }
                }
                else
                {
                    oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiLogicalError;
                    oTrace.sMessage = "Mandatory Param: " + sGUID + " or " + Params + " is missing";
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

        public ActionResult SendMail_Test()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            long iTraceLevel = 10;
            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;
            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            //in the case of
            //xiEnumSystem.xiFuncResult.xiLogicalError
            oCResult.sMessage = "someone tried to do something they shouldnt";

            try
            {
                int ID = 0;
                XIIXI oXII = new XIIXI();
                XIIBO oBOI = new XIIBO();
                XIInfraSendGridComponent oSendGrid = new XIInfraSendGridComponent();
                List<CNV> Params = new List<CNV>();
                Params.Add(new CNV { sName = "ID", sValue = "1" });
                string sBOName = "CampLead";
                foreach (var item in Params)
                {
                    int.TryParse(item.sValue, out ID);
                    if (ID > 0)
                    {
                        List<CNV> oWhrParams = new List<CNV>();
                        oWhrParams.Add(new CNV { sName = "ID", sValue = ID.ToString() });
                        oBOI = oXII.BOI(sBOName, ID.ToString());
                        if (oBOI.Attributes?.Count() > 0)
                        {
                            oSendGrid.sTo = oBOI.Attributes["sEmail"].sValue;
                            oSendGrid.sName = oBOI.Attributes["sName"].sValue;
                            oSendGrid.oDynamicData = new
                            {
                                subject = "Send Grid Using Dynamic Template",
                                header = "dynamic header",
                                name = oSendGrid.sName,
                                address = "west godavari",
                                emailID = oSendGrid.sTo,
                                url = "www.google.com",
                                buttonName = "Go to Google"
                            };

                            var listParams = new List<CNV>();
                            listParams.Add(new CNV { sName = "SendGridAccountID", sValue = "1" });
                            listParams.Add(new CNV { sName = "SendGridTemplateName", sValue = "test" });
                            var result = oSendGrid.Load(listParams);
                            if (result.bOK)
                            {
                                oCResult = result;
                            }
                        }
                    }
                }

                //these code testing purpose

                //oSendGrid.sTo = "raviteja.m@inativetech.com";
                //oSendGrid.sName = "sarvesh";
                //oSendGrid.sCC = "sarveswararao.s@inativetech.com";
                //oSendGrid.sCCName = "sarveswararao";

                //oSendGrid.sTemplateID = "d-467b33cf4fbc43238f748f90b068b308";
                //oSendGrid.iSGADID = 1;

            }
            catch (Exception ex)
            {
                XIInstanceBase oInstanceBase = new XIInstanceBase();
                int line = (new StackTrace(ex, true)).GetFrame(0).GetFileLineNumber();
                oCResult.sMessage = "ERROR: [" + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.sCategory = ex.GetType().ToString();
                oCResult.iCriticality = (int)xiEnumSystem.EnumXIErrorCriticality.Exception;
                oInstanceBase.SaveErrortoDB(oCResult);

            }

            return Json(oCResult.oResult, JsonRequestBehavior.AllowGet);
        }
        public async Task Test_API()
        {
            using (var client = new HttpClient())
            {
                //Load Step1
                client.BaseAddress = new Uri("https://oneplatformfactory.com/whatsapp_api/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("Test:Test123")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                #region Consume GET method  
                HttpResponseMessage response = await client.GetAsync("api/QSAPI/CreateQS?QSD=23386EA5-4E05-4CBA-8FAB-083E6B0005A1&sUniqueRef=9032781899");
                if (response.IsSuccessStatusCode)
                {
                    var httpResponseResult = await response.Content.ReadAsStringAsync();
                    var JsonSer = new JavaScriptSerializer();
                    CNV oResponse = JsonConvert.DeserializeObject<CNV>(httpResponseResult);
                    //CNV oResponse = (CNV)JsonSer.DeserializeObject(httpResponseResult);
                    oResponse.NNVs["Step-Step1"].NNVs["40477"].NNVs["iNoOfNights"].sValue = "10";
                    oResponse.NNVs["Step-Step1"].NNVs["40477"].NNVs["iNoOfGuests"].sValue = "100";
                    //Load Step2
                    var clientRes = new HttpClient();
                    clientRes.BaseAddress = new Uri("https://oneplatformfactory.com/whatsapp_api/");
                    clientRes.DefaultRequestHeaders.Accept.Clear();
                    clientRes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var requeststring = JsonConvert.SerializeObject(oResponse);
                    HttpContent content = new StringContent(requeststring, Encoding.UTF8, "application/json");
                    var resp = await clientRes.PostAsync("api/QSAPI/ResponseQS", content);
                    if (response.IsSuccessStatusCode)
                    {
                        var contentResponse = await resp.Content.ReadAsStringAsync();
                        CNV oResponse2 = JsonConvert.DeserializeObject<CNV>(contentResponse);
                        //oResponse2.NNVs["Step-Insurance Cover"].NNVs["19736"].NNVs["IORiskPostcode"].sValue = "TA1 1AA";
                        //oResponse2.NNVs["Step-Insurance Cover"].NNVs["19736"].NNVs["IOBuildingssuminsured"].sValue = "10";
                        //Load Step3'
                        var clientRes2 = new HttpClient();
                        clientRes2.BaseAddress = new Uri("http://localhost:63722/");
                        clientRes2.DefaultRequestHeaders.Accept.Clear();
                        clientRes2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        var requeststring2 = JsonConvert.SerializeObject(oResponse2);
                        HttpContent content2 = new StringContent(requeststring2, Encoding.UTF8, "application/json");
                        var resp2 = await clientRes2.PostAsync("api/QSAPI/ResponseQS", content2);
                        if (resp2.IsSuccessStatusCode)
                        {
                            var contentResponse2 = await resp2.Content.ReadAsStringAsync();
                            CNV oResponse3 = JsonConvert.DeserializeObject<CNV>(contentResponse2);
                            oResponse3.NNVs["Step-Contact Details"].NNVs["19742"].NNVs["sFirstName"].sValue = "Ravi";
                            oResponse3.NNVs["Step-Contact Details"].NNVs["19742"].NNVs["sLastName"].sValue = "teja";
                        }
                        //var respObject = JsonConvert.DeserializeObject<CreateOrderResponse>(contentResponse);
                        //return respObject;
                    }
                }
                else
                {
                }
                #endregion
            }
            //return null;
        }

        public async Task Test_1Q()
        {
            using (var client = new HttpClient())
            {
                //Load Step1
                client.BaseAddress = new Uri("http://localhost:63722/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                string authInfo = Convert.ToBase64String(Encoding.Default.GetBytes("Test:Test123")); //("Username:Password")  
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authInfo);
                #region Consume GET method  
                HttpResponseMessage response = await client.GetAsync("api/QSAPI/OneQ?QID=9B3FE15E-797E-4BFF-AA64-29708738200F");
                if (response.IsSuccessStatusCode)
                {
                    var httpResponseResult = await response.Content.ReadAsStringAsync();
                    List<CNV> oResponse = JsonConvert.DeserializeObject<List<CNV>>(httpResponseResult);
                }
                #endregion
            }
        }

        #region SendGridRemote

        public ActionResult Get_SendGridTransactions()
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            CTraceStack oTrace = new CTraceStack();
            oTrace.sClass = this.GetType().Name;
            oTrace.sMethod = MethodBase.GetCurrentMethod().Name;
            oTrace.iStatus = (int)xiEnumSystem.xiFuncResult.xiInProcess;
            oTrace.sTask = "Get Communication transactions from remote DB to Local DB";//expalin about this method logic
            XIDefinitionBase oXID = new XIDefinitionBase();
            string sCode = "SendGrid";
            oCResult.sCategory = sCode;
            List<CNV> oTraceInfo = new List<CNV>();
            XIInfraUsers oUser = new XIInfraUsers();
            CUserInfo oInfo = new CUserInfo();
            oInfo = oUser.Get_UserInfo();
            var AppID = oInfo.iApplicationID;
            var sTraceLog = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "TraceLog");
            var sRemoteDB = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, AppID + "_" + "RemoteDB");
            string sDBName = sRemoteDB;
            try
            {
                oTraceInfo.Add(new CNV { sValue = "Started Get_SendGridTransactions method to import Communication Transactions for DB " + sDBName + " on " + DateTime.Now });
                var bIsConnectable = API.GetString("api/SendGrid/Tesst_ConnectionString?sDBName=" + sDBName);
                if (bIsConnectable == "true")
                {
                    oTraceInfo.Add(new CNV { sValue = "Remote connection established successfully" });
                    var sSessionID = HttpContext.Session.SessionID;
                    oTraceInfo.Add(new CNV { sValue = "Started OneSendGridEvents method to import Communication Transactions on " + DateTime.Now });
                    var Result = API.PostStringGetList<XIIBO>("api/SendGrid/OneSendGridEvents?sAppGUID=" + oInfo.iApplicationIDXIGUID);
                    if (Result != null && Result.Count() > 0)
                    {
                        oTraceInfo.Add(new CNV { sValue = "Received Communication Transactions are " + Result.Count() });
                        XIIXI oXI = new XIIXI();
                        List<CNV> oWhrPrms = new List<CNV>();
                        List<string> RemoteGUIDs = new List<string>();
                        foreach (var trans in Result)
                        {
                            if (trans.Attributes != null && trans.Attributes.Count() > 0)
                            {
                                var iTransID = trans.AttributeI("id").sValue;
                                var sMessageID = trans.AttributeI("sSendGridReference").sValue;
                                if (!string.IsNullOrEmpty(sMessageID))
                                {
                                    string sRemoteGUID = trans.AttributeI("xiguid").sValue;
                                    oWhrPrms = new List<CNV>();
                                    oWhrPrms.Add(new CNV { sName = "sSendGridReference", sValue = sMessageID });
                                    var CommI = oXI.BOI("XICommunicationI", null, null, oWhrPrms);
                                    if (CommI != null && CommI.Attributes.Count() > 0)
                                    {
                                        Guid PCGUID = Guid.Empty;
                                        var FKiComTypeID = CommI.AttributeI("FKiCommunicationTypeIDXIGUID").sValue;
                                        var ComType = (XIIBO)oCache.GetObjectFromCache(XIConstant.CacheXIrefComType, "XICommunicationType", FKiComTypeID);
                                        if (ComType != null && ComType.Attributes.Count() > 0)
                                        {
                                            var PC = ComType.AttributeI("FKiResponsePCIDXIGUID").sValue;
                                            Guid.TryParse(PC, out PCGUID);
                                        }
                                        var FKiCommIID = CommI.AttributeI("id").sValue;
                                        trans.BOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "commsTransaction");
                                        trans.Attributes.Values.ToList().ForEach(m => m.bDirty = true);
                                        trans.SetAttribute("fkicomminstanceid", FKiCommIID);
                                        trans.SetAttribute("id", "");
                                        trans.SetAttribute("xiguid", "");
                                        oCR = trans.Save(trans);
                                        if (oCR.bOK && oCR.oResult != null)
                                        {
                                            var oTransI = (XIIBO)oCR.oResult;
                                            var iCommTransID = oTransI.AttributeI("id").sValue;
                                            sRemoteGUID = sRemoteGUID + "_" + "20";
                                            if (PCGUID != null && PCGUID != Guid.Empty)
                                            {
                                                XIDAlgorithm oAlgoD = new XIDAlgorithm();
                                                string sNewGUID = Guid.NewGuid().ToString();
                                                List<CNV> oNVsList = new List<CNV>();
                                                oNVsList.Add(new CNV { sName = "-iBOIID", sValue = iCommTransID });
                                                //oNVsList.Add(new CNV { sName = "-OriginType", sValue = "XILink" });
                                                //oNVsList.Add(new CNV { sName = "-Origin", sValue = XiLink.XiLinkID.ToString() });
                                                oCache.SetXIParams(oNVsList, sNewGUID, sSessionID);
                                                oAlgoD = (XIDAlgorithm)oCache.GetObjectFromCache(XIConstant.CacheXIAlgorithm, null, PCGUID.ToString());
                                                oCR = oAlgoD.Execute_XIAlgorithm(sSessionID, sNewGUID);
                                                if (oCR.bOK && oCR.oResult != null)
                                                {

                                                }
                                                else
                                                {
                                                    oTraceInfo.Add(new CNV { sValue = "Process Controller execution failed for communication instance " + FKiCommIID });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sRemoteGUID = sRemoteGUID + "_" + "30";
                                            oTraceInfo.Add(new CNV { sValue = "Importing failed for communication instance for MessageID:" + sMessageID + " and Communication Instance:" + FKiCommIID });
                                        }
                                        RemoteGUIDs.Add(sRemoteGUID);
                                    }
                                    else
                                    {
                                        oTraceInfo.Add(new CNV { sValue = "Instance loading failed for communication instance for MessageID:" + sMessageID });
                                    }
                                }
                                else
                                {
                                    oTraceInfo.Add(new CNV { sValue = "There are no messageid for communication transaction:" + iTransID });
                                }
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "There are no attributes for communication transaction" });
                            }
                        }
                        oTraceInfo.Add(new CNV { sValue = "Completed importing the communication transactions :" + DateTime.Now });
                        if (RemoteGUIDs != null && RemoteGUIDs.Count() > 0)
                        {
                            oTraceInfo.Add(new CNV { sValue = "Started OneSendGrid_UpdateStatus method to update status on remote DB" });
                            //Update Status of CommsTransaction instance in remote DB
                            var UpdateResponse = API.PostListGetString(RemoteGUIDs, "api/SendGrid/OneSendGrid_UpdateStatus");
                            if (UpdateResponse == "Success")
                            {
                                oTraceInfo.Add(new CNV { sValue = "Completed OneSendGrid_UpdateStatus method sucessfully " + DateTime.Now });
                            }
                            else
                            {
                                oTraceInfo.Add(new CNV { sValue = "Completed OneSendGrid_UpdateStatus method with Failure " + DateTime.Now });
                            }
                        }
                    }
                    else
                    {
                        oTraceInfo.Add(new CNV { sValue = "There are no Communication Transactions to import" });
                    }
                }
                else
                {
                    oTraceInfo.Add(new CNV { sValue = "Unable to connect to Remote connection for DB " + sDBName });
                    oCResult.sMessage = "Unable to connect to Remote connection for DB " + sDBName;
                    oCResult.sCategory = "SendGrid";
                    oCResult.oTraceStack = oTraceInfo;
                    oXID.SaveErrortoDB(oCResult);
                }
                if (!string.IsNullOrEmpty(sTraceLog) && sTraceLog.ToLower() == "yes")
                {
                    oCResult.oTraceStack = oTraceInfo;
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
                oCResult.oTraceStack = oTraceInfo;
                oXID.SaveErrortoDB(oCResult);
            }
            var Messages = oCResult.oTraceStack.Select(m => m.sValue).ToArray();
            return Content(string.Join("->", Messages), "application/json");
        }

        public ActionResult SetAppToSGMessage(List<CNV> oParams)
        {
            return null;
        }

        #endregion SendGridRemote
    }
}