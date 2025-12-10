using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.Models;
using System.Text.RegularExpressions;
using XIDNA.ViewModels;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
//using RestSharp;
using Newtonsoft.Json;
using XIDNA.Common;
using XICore;

namespace XIDNA.Controllers
{
    [Authorize]
    public class LeadController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILeadRepository LeadRepository;

        public LeadController() : this(new LeadRepository()) { }

        public LeadController(ILeadRepository LeadRepository)
        {
            this.LeadRepository = LeadRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        // GET: /Lead/Email
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LeadUploadFile()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //Get the Source details i.e. Subscriptions
                int OrgID = 0;
                LeadInbounds LeadInbound = new LeadInbounds();
                var sSourceDetails = LeadRepository.GetSourceProvider(OrgID, sDatabase);
                LeadInbound.SourceDetails = sSourceDetails;
                return PartialView("LeadUploadFile", LeadInbound);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public List<string> GetDatabase()
        {
            List<string> Databases = LeadRepository.GetDatabases();
            return Databases;
        }

        public ActionResult GetSourceExtractData()
        {
            return PartialView("GetSourceExtractData");
        }

        [HttpPost]
        public ActionResult ExtractEmailData(int SourceID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var sData = LeadRepository.ExtractEmailData(SourceID, OrgID, sDatabase);
                if (sData != null)
                {
                    return Json(sData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult DisplayLeadDetails(jQueryDataTableParamModel param, string database)
        //{
        //    try
        //    {
        //        param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
        //        param.sSortDir = Request["sSortDir_0"].ToString();
        //        var result = LeadRepository.DisplayLeadDetails(param, sDatabase);
        //        return Json(new
        //        {
        //            sEcho = result.sEcho,
        //            iTotalRecords = result.iTotalRecords,
        //            iTotalDisplayRecords = result.iTotalDisplayRecords,
        //            aaData = result.aaData
        //        },
        //        JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }

        //}

        //Excel
        [HttpPost]
        public ActionResult ImportExcelData(HttpPostedFileBase UploadExcel, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ImportHistories model = new ImportHistories();
                int OrgID = 0;
                int UserID = 0;
                model.OrganizationID = OrgID;
                model.UserID = UserID;
                model.FileType = "Excel";
                model.ImportedOn = DateTime.Now;
                model.StatusTypeID = 0;
                model.FileName = "";
                string OriginalName = Path.GetFileName(UploadExcel.FileName);
                model.OriginalName = OriginalName;
                var res = LeadRepository.SaveImportHistories(model, sDatabase);
                var FilePath = "";
                string sGetExtension = Path.GetExtension(UploadExcel.FileName);
                string sExtension = sGetExtension.Replace(".", "").ToLower();
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";

                model.ID = res;
                if (sExtension == "xlsx")
                {
                    UploadExcel.SaveAs(str + "Excel_" + 0 + "_" + res + ".xlsx");
                    string sFilePath = "Excel_" + 0 + "_" + res + ".xlsx";
                    model.FileName = sFilePath;
                    FilePath = str + "Excel_" + 0 + "_" + res + ".xlsx";
                }
                else
                {
                    UploadExcel.SaveAs(str + "Excel_" + 0 + "_" + res + ".xls");
                    string sFilePath = "Excel_" + 0 + "_" + res + ".xls";
                    model.FileName = sFilePath;
                    FilePath = str + "Excel_" + 0 + "_" + res + ".xls";
                }
                var save = LeadRepository.SaveImportHistories(model, sDatabase);
                logger.Info(FilePath);
                var sData = LeadRepository.ImportExcelData(UploadExcel, FilePath, OrgID, res, SubID);
                //return Json(sData,JsonRequestBehavior.AllowGet );
                if (sData != null)
                {
                    if (sData[0] == "Success")
                    {
                        sData.Add("FIleID-" + res);
                        string sExcel = string.Join(",", sData);
                        // sExcel= string.Join(",", "FIleID-"+res);
                        return Content(sExcel);
                    }
                    else
                    {
                        sData.Insert(0, "Failure");
                        //SendErrorAlert(FilePath, OrgID);
                        //return Content("Oops!! Looks like something went wrong.Uploaded file status has been sent through Email.Please check.");
                        sData.Add("FIleID-" + res);
                        string sExcel = string.Join(",", sData);
                        return Content(sExcel);
                    }
                }
                else
                {
                    //foreach (var items in sData)
                    //{
                    //    logger.Info(items);
                    //}
                    return Content(ServiceConstants.LeadImportingError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }
        }

        public ActionResult ImportJSONData(HttpPostedFileBase UploadJson, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ImportHistories model = new ImportHistories();
                int OrgID = 0;
                int UserID = 0;
                model.OrganizationID = OrgID;
                model.UserID = UserID;
                model.FileType = "Json";
                model.ImportedOn = DateTime.Now;
                model.StatusTypeID = 0;
                model.FileName = "";
                string OriginalName = Path.GetFileName(UploadJson.FileName);
                model.OriginalName = OriginalName;
                var res = LeadRepository.SaveImportHistories(model, sDatabase);

                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                logger.Info(str);
                UploadJson.SaveAs(str + "\\" + "Json_" + 0 + "_" + res + ".json");
                string sFilePath = "Json_" + 0 + "_" + res + ".json";

                model.ID = res;
                model.FileName = sFilePath;
                var save = LeadRepository.SaveImportHistories(model, sDatabase);
                string FilePath = str + "Json_" + 0 + "_" + res + ".json";
                var sData = LeadRepository.ImportJSONData(FilePath, OrgID, res, SubID);
                if (sData != null)
                {
                    //var sErrorCount= sData[0].Split(':');
                    // if(Convert.ToInt16(sErrorCount[1]) > 1)
                    // { }

                    if (sData[0] == "Success")
                    {
                        sData.Add("FIleID-" + res);
                        string sJSON = String.Join(",", sData);
                        return Content(sJSON);
                    }
                    else
                    {
                        sData.Insert(0, "Failure");
                        //SendErrorAlert(FilePath, OrgID);
                        //return Content("Oops!! Looks like something went wrong.Uploaded file status has been sent through Email.Please check.");
                        sData.Add("FIleID-" + res);
                        string sJSON = String.Join(",", sData);
                        return Content(sJSON);
                    }
                }
                else
                {
                    return Content(ServiceConstants.LeadImportingError);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }
        }

        [HttpPost]
        public ActionResult ImportXMLData(HttpPostedFileBase UploadXML, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ImportHistories model = new ImportHistories();
                int OrgID = 0;
                int UserID = 0;
                model.OrganizationID = OrgID;
                model.UserID = UserID;
                model.FileType = "XML";
                model.ImportedOn = DateTime.Now;
                model.StatusTypeID = 0;
                model.FileName = "";
                string OriginalName = Path.GetFileName(UploadXML.FileName);
                model.OriginalName = OriginalName;
                var res = LeadRepository.SaveImportHistories(model, sDatabase);

                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                logger.Info(str);
                UploadXML.SaveAs(str + "\\" + "XML_" + 0 + "_" + res + ".xml");
                string sFilePath = "XML_" + 0 + "_" + res + ".xml";
                model.ID = res;
                model.FileName = sFilePath;
                var save = LeadRepository.SaveImportHistories(model, sDatabase);            //var sFilePath = UploadXML.FileName;
                string FilePath = str + "XML_" + 0 + "_" + res + ".xml";

                var sData = LeadRepository.ImportXMLData(FilePath, OrgID, res, SubID);
                if (sData != null)
                {
                    if (sData[0] == "Success")
                    {
                        sData.Add("FIleID-" + res);
                        string sXML = String.Join(",", sData);
                        return Content(sXML);
                    }
                    else
                    {
                        sData.Insert(0, "Failure");
                        //SendErrorAlert(FilePath, OrgID);
                        //return Content("Oops!! Looks like something went wrong.Uploaded file status has been sent through Email.Please check.");
                        sData.Add("FIleID-" + res);
                        string sXML = String.Join(",", sData);
                        return Content(sXML);
                    }

                }
                else
                {
                    return Content(ServiceConstants.LeadImportingError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }
        }
        [HttpPost]
        public ActionResult ImportTabDelimitedData(HttpPostedFileBase UploadTab, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ImportHistories model = new ImportHistories();
                int OrgID = 0;
                int UserID = 0;
                model.OrganizationID = OrgID;
                model.UserID = UserID;
                model.FileType = "Tab";
                model.ImportedOn = DateTime.Now;
                model.StatusTypeID = 0;
                model.FileName = "";
                string OriginalName = Path.GetFileName(UploadTab.FileName);
                model.OriginalName = OriginalName;
                var res = LeadRepository.SaveImportHistories(model, sDatabase);

                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                logger.Info(str);
                UploadTab.SaveAs(str + "\\" + "Tab_" + 0 + "_" + res + ".txt");
                string sFilePath = "Tab_" + 0 + "_" + res + ".txt";
                model.ID = res;
                model.FileName = sFilePath;
                var save = LeadRepository.SaveImportHistories(model, sDatabase);
                string FilePath = str + "Tab_" + 0 + "_" + res + ".txt";
                //var sFilePath = UploadTab.FileName;

                var sData = LeadRepository.ImportTabDelimitedData(FilePath, OrgID, res, SubID);
                if (sData != null)
                {
                    if (sData[0] == "Success")
                    {
                        sData.Add("FIleID-" + res);
                        string sTabText = String.Join(",", sData);
                        return Content(sTabText);
                    }
                    else
                    {
                        sData.Insert(0, "Failure");
                        //SendErrorAlert(FilePath, OrgID);
                        // return Content("Oops!! Looks like something went wrong.Uploaded file status has been sent through Email.Please check.");
                        sData.Add("FIleID-" + res);
                        string sTabText = String.Join(",", sData);
                        return Content(sTabText);
                    }
                }
                else
                {
                    return Content(ServiceConstants.LeadImportingError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }
        }

        [HttpPost]
        public ActionResult ImportCSVData(HttpPostedFileBase UploadCSV, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ImportHistories model = new ImportHistories();
                int OrgID = 0;
                int UserID = 0;
                model.OrganizationID = OrgID;
                model.UserID = UserID;
                model.FileType = "CSV";
                model.ImportedOn = DateTime.Now;
                model.StatusTypeID = 0;
                model.FileName = "";
                string OriginalName = Path.GetFileName(UploadCSV.FileName);
                model.OriginalName = OriginalName;
                var res = LeadRepository.SaveImportHistories(model, sDatabase);
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                logger.Info(str);
                UploadCSV.SaveAs(str + "\\" + "CSV_" + 0 + "_" + res + ".csv");
                string sFilePath = "CSV_" + 0 + "_" + res + ".csv";
                model.ID = res;
                model.FileName = sFilePath;
                var save = LeadRepository.SaveImportHistories(model, sDatabase);
                string FilePath = str + "CSV_" + 0 + "_" + res + ".csv";
                //var sFilePath = UploadCSV.FileName;

                var sData = LeadRepository.ImportCSVData(FilePath, OrgID, res, SubID);
                if (sData != null)
                {
                    if (sData[0] == "Success")
                    {
                        sData.Add("FIleID-" + res);
                        string sCSV = string.Join(",", sData);
                        return Content(sCSV);
                    }
                    else
                    {
                        sData.Insert(0, "Failure");
                        //SendErrorAlert(FilePath, OrgID);
                        // return Content("Oops!! Looks like something went wrong.Uploaded file status has been sent through Email.Please check.");
                        sData.Add("FIleID-" + res);
                        string sCSV = String.Join(",", sData);
                        return Content(sCSV);
                    }
                }
                else
                {
                    return Content(ServiceConstants.LeadImportingError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }
        }
        [HttpPost]
        public ActionResult GetValidANDInvalidData(string iID, int FileID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<string> Ids = iID.Split(',').ToList();
                Ids.RemoveAt(0);
                Ids.RemoveAt(Ids.Count - 1);
                List<long> LeadIDs = (from c in Ids
                                      select Convert.ToInt64(c)).ToList();
                List<int> Id = (from c in Ids
                                select Convert.ToInt32(c)).ToList();
                int OrgID = 0;
                var sData = LeadRepository.GetValidANDInvalidDetails(Id, sDatabase, OrgID, FileID);
                return Json(sData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetInValidData(List<int> iID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var sData = LeadRepository.GetInValidData(iID, sDatabase, OrgID);
                return Json(sData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        [HttpPost]
        public ActionResult GetValidData(List<int> iID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var sData = LeadRepository.GetInValidData(iID, sDatabase, OrgID);
                return Json(sData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        //public ActionResult ContactSRCTemplate(string csvID, int FileID)
        //{
        //    Outbounds oModel = new Outbounds();
        //    oModel.csvID = csvID;
        //    oModel.FileID = FileID;
        //    return PartialView("LeadGetTemplate", oModel);
        //}


        //public ActionResult GetTemplate(int iType)
        //{
        //    string database = sDatabase;
        //    int OrgID = 0;
        //    List<VMDropDown> list = LeadRepository.GetTemplateList(iType, database, OrgID);
        //    return Json(list, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult ReportSourceProvider(List<int> iID, int FileID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                DataContext DbContext = new DataContext();
                int OrgID = 0;
                var sData = LeadRepository.ReportSourceProvider(iID, sDatabase, OrgID, FileID);
                return Json(sData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        [HttpPost]
        public ActionResult ExtractLeadData(string iID, int FileID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Ids = iID.Split(',').ToList();
                List<int> LeadIDs = new List<int>();
                LeadIDs = (from c in Ids
                           select Convert.ToInt32(c)).ToList();
                int OrgID = 0;
                var sData = LeadRepository.ExtractLeadData(LeadIDs, sDatabase, OrgID, FileID);
                if (sData != null)
                {
                    return Json(sData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Content(ServiceConstants.LeadImportingError);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content(ServiceConstants.LeadImportingException);
            }

        }
        //public ActionResult SendErrorAlert(string FilePath, int OrgID)
        //{
        //    try
        //    {
        //        int iType = 1;
        //        if (iType == 1)
        //        {
        //            //Getting the Server Details from the Database
        //            var sDetails = LeadRepository.ServerDetails(iType, OrgID);
        //            string usern = "", pass = "", sender = "", security = "", serverName = "";
        //            int port = 0;
        //            foreach (var items in sDetails)
        //            {
        //                usern = items.UserName;
        //                pass = items.Password;
        //                sender = items.FromAddress;
        //                serverName = items.ServerName;
        //                port = items.Port;
        //                security = items.Security;
        //            }
        //            string username = HttpUtility.UrlEncode(usern);
        //            string password = HttpUtility.UrlEncode(pass);
        //            string emailSubject = "Error occured in the uploaded file";
        //            string messageBody = "Hi Sir/Madam, <br/> We are unable to process your uploaded file. Please check the file and try again.<br/> Regards,<br/> Team XIDNA. <br/>";
        //            MailMessage msg = new MailMessage();


        //            msg.To.Add("raviteja.m@inativetech.com");
        //            msg.From = new MailAddress(sender);
        //            msg.Subject = emailSubject;
        //            string html = @"<html><body>" + messageBody + "</body></html>";
        //            string Attach = "";
        //            msg.Body = html;

        //            Attach = FilePath;
        //            msg.Attachments.Add(new Attachment(Attach));

        //            msg.IsBodyHtml = true;
        //            SmtpClient smtp = new SmtpClient();
        //            smtp.Host = serverName;
        //            smtp.Port = port;
        //            //for gmail
        //            smtp.EnableSsl = false;
        //            smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
        //            smtp.Send(msg);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        return Content("Oops!! Looks like error has been encountered while sending email.Please check.");
        //    }
        //    return null;
        //}

        public ActionResult DisplayCompleteLeadDetails(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.DisplayCompleteLeadDetails(param, OrgID, sDatabase);
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

        //[HttpPost]
        //public ActionResult DisplayCompleteLeadDetails()
        //{
        //    try
        //    {
        //        int OrgID = 0;
        //        string DbName = sDatabase;
        //        var sData = LeadRepository.DisplayCompleteLeadDetails(OrgID, DbName);
        //        if (sData != null)
        //        {
        //            return Json(sData, JsonRequestBehavior.AllowGet);
        //        }
        //        else
        //        {
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }
        //    return null;
        //}


        public ActionResult GetOrgClassTypes()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var Model = LeadRepository.GetOrgClassTypes(OrgID, sDatabase);
                return Json(Model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult LeadActionTypes()
        {
            int OrgID = 0;
            return View("LeadActions", OrgID);
        }
        //Displaying the Grid
        public ActionResult GetActionsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.GetActionsList(param, OrgID, sDatabase);
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
        //Opening the _LeadAction View
        public ActionResult CreateLeadAction()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                VMActionTypes action = new VMActionTypes();
                List<VMDropDown> organizations = LeadRepository.GetOrganizations();
                action.organizations = organizations;
                action.OrganizationID = 0;
                var Result = LeadRepository.GetAllTemplates(OrgID, 1, sDatabase);
                action.SMSTemplates = Result.SMSTemplates;
                action.EmailTemplates = Result.EmailTemplates;
                action.Popups = Result.Popups;
                action.Stages = Result.Stages;
                action.OneClicks = Result.OneClicks;
                action.OrgID = 0;
                return View("_LeadActionsForm", action);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        //Saving into LeadAction Table
        [HttpPost]
        public ActionResult SaveAction(VMActionTypes model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = LeadRepository.SaveAction(model);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CallAction(VMLeadActions model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = LeadRepository.CallAction(model, OrgID, iUserID, sOrgName, sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetActionPopup(int PopupID, int LeadID, string PopType, int ClientID = 0, int StageID = 0, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMLeadActions Action = new VMLeadActions();
                Action.PopupID = PopupID;
                Action.ID = ID;
                Action.LeadID = LeadID;
                var tabs = LeadRepository.GetAllTabs(0, PopupID, sDatabase);
                Action.Tabs = tabs;
                Action.PopType = PopType;
                Action.StageID = StageID;
                Action.ClientID = ClientID;
                return PartialView("_ActionPopupContent", Action);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Checking the Uniquness of the Action Name
        [HttpPost]
        public ActionResult IsExistsActionName(string Name, int ID, int OrganizationID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return LeadRepository.IsExistsActionName(Name, ID, OrganizationID) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        //Editing of the LeadAction Form
        public ActionResult EditAction(int ActionID, int OrganizationID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMActionTypes action = new VMActionTypes();
                action = LeadRepository.EditAction(ActionID, OrganizationID);
                List<VMDropDown> organizations = LeadRepository.GetOrganizations();
                action.organizations = organizations;
                var Result = LeadRepository.GetAllTemplates(OrganizationID, 1, sDatabase);
                action.SMSTemplates = Result.SMSTemplates;
                action.EmailTemplates = Result.EmailTemplates;
                action.Popups = Result.Popups;
                action.Stages = Result.Stages;
                action.OneClicks = Result.OneClicks;
                action.OrgID = 0;
                return PartialView("_LeadActionsForm", action);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Getting the Templates DropDown
        public ActionResult GetAllTemplates(int OrgID, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Templates = LeadRepository.GetAllTemplates(OrgID, Type, null);
                return Json(Templates, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveLeadTransaction(Stages model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                var result = LeadRepository.SaveLeadTransaction(model, OrgID, sDatabase, UserID);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        //Import Rules
        public ActionResult ImportRulesDetails()
        {
            int Type = 2;
            return View("ImportRulesDetailsList", Type);
        }
        //5
        public ActionResult ImportRulesDetailsList(jQueryDataTableParamModel param, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.ImportRulesDetailsList(param, Type);
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
        //2
        public ActionResult CreateImportRulesDetails()
        {
            ImportRules importrules = new ImportRules();
            //ModelDbContext modeldb = new ModelDbContext();
            //var list = modeldb.ImportRules.ToList();
            return View("ImportRulesDetails", importrules);

        }

        //3

        public ActionResult SaveImportRulesDetails(ImportRules model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sd = LeadRepository.SaveImportRulesDetails(model);
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //4
        public ActionResult EditImportRulesDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var importrules = LeadRepository.EditImportRulesDetails(ID);
                return View("ImportRulesDetails", importrules);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;

            }

        }
        public ActionResult ImportHistory()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;

            }

        }
        public ActionResult ErrorDetailsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.ErrorDetailsList(param, OrgID, sDatabase);
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
        public ActionResult GetFileErrorDetails(int ID, string FileName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgId = 0;
                var Result = LeadRepository.GetFileErrorDetails(ID, OrgId, sDatabase);
                ViewBag.FileName = FileName;
                return PartialView("_FileErrorsList", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;

            }

        }

        public ActionResult SendMail()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                logger.Info("Controller");
                var Result = LeadRepository.SendMail(iUserID, sOrgName);
                logger.Info("Mail Sent");
                return null;
            }
            catch (Exception ex)
            {
                logger.Info("Error");
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Opening of the LeadConfigurationList View
        public ActionResult LeadConfigurations()
        {
            int OrgID = 0;
            return View("LeadConfigurationsList", OrgID);
        }
        //opening of the form
        public ActionResult CreateLeadConfigurations()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                LeadConfigurations LC = new LeadConfigurations();
                List<VMDropDown> organizations = LeadRepository.GetOrganizations();
                LC.organizations = organizations;
                LC.ClassesList = LeadRepository.ClassesList(OrgID, 0, sDatabase, iUserID, sOrgName);
                LC.OrganizationID = 0;
                return View("LeadConfigurationForm", LC);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Getting Dropdown List for the Classes
        public ActionResult ClassesList(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var sListClasses = LeadRepository.ClassesList(OrgID, 0, null, iUserID, sOrgName);
                return Json(sListClasses, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Saving into Leads Configurations Table
        [HttpPost]
        public ActionResult SaveLeadConfigurations(LeadConfigurations model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                if (model.OrganizationID != OrgID)
                {
                    model.OrganizationID = OrgID;
                }
                var sd = LeadRepository.SaveLeadConfigurations(model);
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        //Data Table
        public ActionResult LeadConfigurationsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.LeadConfigurationsList(param, OrgID, sDatabase);
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
        //Editing of the Form
        public ActionResult EditLeadConfigurations(int ID, int OrganizationID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var LC = LeadRepository.EditLeadConfigurations(ID, OrganizationID);
                List<VMDropDown> organizations = LeadRepository.GetOrganizations();
                LC.organizations = organizations;
                List<VMDropDown> ClassesList = LeadRepository.ClassesList(OrgID, ID, sDatabase, iUserID, sOrgName);
                LC.ClassesList = ClassesList;
                LC.OrganizationID = OrgID;
                return View("LeadConfigurationForm", LC);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        //Action Menu
        public ActionResult LeadActionMenu()
        {
            return View("ActionMenu");
        }

        public ActionResult CreateLeadActionMenu()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                LeadActionMenus Menu = new LeadActionMenus();
                Menu.ActionTypes = LeadRepository.GetActionTypes(OrgID, sDatabase);
                return PartialView("_LeadActionMenuForm", Menu);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditActionMenu(int ID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                LeadActionMenus Menu = new LeadActionMenus();
                var Result = LeadRepository.GetActionMenuByID(ID, OrgID, sDatabase);
                Result.ActionTypes = LeadRepository.GetActionTypes(OrgID, sDatabase);
                return PartialView("_LeadActionMenuForm", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveActionMenu(LeadActionMenus model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                model.OrganizationID = OrgID;
                var Response = LeadRepository.SaveActionMenu(model, OrgID, sDatabase);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IsExistsActionMenuName(string Name, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                return LeadRepository.IsExistsActionMenuName(Name, ID, OrgID) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetActionMenusList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.GetActionMenusList(param, OrgID, sDatabase);
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
        //Lead Popup-- Open Popup Window
        public ActionResult GetLeadPopup(int LeadID, int PopupID, string PopType, int ClientID = 0, int StageID = 0, int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = LeadID;
                popup.PopupID = PopupID;
                //popup.UserName = Util.GetUserName();
                popup.StageID = StageID;
                popup.PopType = PopType;
                popup.ClientID = ClientID;
                popup.RowID = ID;
                var Res = LeadRepository.GetPopupDetails(PopupID);
                if (Res.IsLeftMenu)
                {
                    popup.IsLeftMenu = true;
                }
                else
                {
                    popup.IsLeftMenu = false;
                }
                popup.LayoutType = Res.LayoutID;
                return View("LeadPopup", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Lead Popup - Left Content
        public ActionResult GetLeadPopupLeftContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                VMLeadPopupLeft model = LeadRepository.GetLeadPopupLeftContent(Popup, sDatabase, OrgID);
                return PartialView("_LeadPopupLeftContent", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult LeadContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = Popup.LeadID;
                popup.PopupID = Popup.ReportID;
                popup.Tabs = LeadRepository.GetAllTabs(Popup.ReportID, Popup.PopupID, sDatabase);
                popup.ClientID = Popup.ClientID;
                popup.PopType = Popup.PopType;
                popup.StageID = Popup.StageID;
                popup.RowID = Popup.RowID;
                popup.LayoutType = Popup.LayoutType;
                return PartialView("_LeadPopupContent", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Lead Popup-- Get Popup Tabs
        public ActionResult GetAllTabs(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var tabs = LeadRepository.GetAllTabs(ReportID, 0, sDatabase);
                return Json(tabs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Lead Popup-- Get Tab Content
        [HttpPost]
        public ActionResult GetTabContent(VMViewPopup Popup)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                int OrgID = 0;
                if (Popup.Name == "Reminders")
                {
                    Reminders model = new Reminders();
                    model.ReportID = Popup.ReportID;
                    model.LeadID = Popup.LeadID;
                    return PartialView("_LeadReminder", model);
                }
                else if (Popup.Name == "Sent Documents")
                {
                    return PartialView("_TabUploadDocument", Popup.LeadID);
                }
                else if (Popup.Name == "Received Documents")
                {
                    return PartialView("_TabClientDocument", Popup.LeadID);
                }
                else
                {
                    List<VMQueryPreview> content = LeadRepository.GetTabContent(Popup, UserID, OrgID, sDatabase, sOrgName);
                    if (content.Count() > 0 && content != null)
                    {
                        content.ToList().ForEach(m => m.PopType = Popup.PopType);
                        content.ToList().ForEach(m => m.StageID = Popup.StageID);
                        VMViewPopup popup = new VMViewPopup();
                        popup.TabID = Popup.TabID;
                        popup.ClassID = Popup.ClassID;
                        content.FirstOrDefault().popup = popup;
                        content.FirstOrDefault().LeadID = Popup.LeadID;
                        return PartialView("_LeadPopupTabContent", content);
                    }
                    else
                    {
                        return null;
                    }
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
        //public ActionResult GetTabContent(VMViewPopup Popup)
        //{
        //    try
        //    {
        //        int UserID = Convert.ToInt32(User.Identity.GetUserId());
        //        int OrgID = 0;
        //        string Database = sDatabase;
        //        if (Popup.Name == "Reminders")
        //        {
        //            Reminders model = new Reminders();
        //            model.ReportID = Popup.ReportID;
        //            model.LeadID = Popup.LeadID;
        //            return PartialView("_LeadReminder", model);
        //        }
        //        else if (Popup.Name == "Sent Documents")
        //        {
        //            return PartialView("_TabUploadDocument", Popup.LeadID);
        //        }
        //        else if (Popup.Name == "Received Documents")
        //        {                    
        //            return PartialView("_TabClientDocument", Popup.LeadID);
        //        }
        //        else
        //        {
        //            VMQueryPreview content = LeadRepository.GetTabContent(Popup, UserID, OrgID, Database);
        //            if (content != null)
        //            {
        //                VMViewPopup popup = new VMViewPopup();
        //                popup.TabID = Popup.TabID;
        //                popup.ClassID = Popup.ClassID;

        //                content.popup = popup;
        //                content.LeadID = Popup.LeadID;
        //                if (content.PreviewType == "View Record")
        //                {
        //                    if (content.SectionsData[0].ViewFieldsData != null && content.SectionsData[0].ViewFieldsData.Count() > 0)
        //                    {
        //                        return PartialView("_TabViewForm", content.SectionsData);
        //                    }
        //                    if (content.SectionsData[0].EditFieldsData != null && content.SectionsData[0].EditFieldsData.Count() > 0)
        //                    {
        //                        return PartialView("_TabEditForm", content.SectionsData);
        //                    }
        //                }
        //                else if (content.PreviewType == "KPI Circle")
        //                {
        //                    return PartialView("_TabKPICircles", content.KpiCircle);
        //                }
        //                else if (content.PreviewType == "KPI Pie Chart")
        //                {
        //                    List<VMKPIResult> KpiResult = new List<VMKPIResult>();
        //                    User models = new User();
        //                    models.Type = "Tabs";
        //                    models.TabID = content.popup.TabID;
        //                    models.DDLClassValue = content.popup.ClassID;
        //                    models.SectionName = content.SectionName;
        //                    models.ReportID = content.ReportID;
        //                    models.PieData = content.PieData;
        //                    models.QueryName = content.QueryName;
        //                    return PartialView("_TabKPIPieChart", models);
        //                }
        //                else if (content.PreviewType == "KPI Bar Chart")
        //                {
        //                    LineGraph BarData = new LineGraph();
        //                    BarData = content.BarData;
        //                    BarData.ReportID = content.ReportID;
        //                    BarData.TabID = content.popup.TabID;
        //                    BarData.SectionName = content.SectionName;
        //                    BarData.QueryName = content.QueryName;
        //                    BarData.Type = "Tabs";
        //                    return PartialView("_TabKPIBarChart", BarData);
        //                }
        //                else if (content.PreviewType == "Bespoke")
        //                {
        //                    return PartialView("_Bespoke", content.SectionsData);
        //                }
        //                else if (content.PreviewType == "KPI Line Graph")
        //                {
        //                    LineGraph BarData = new LineGraph();
        //                    BarData = content.LineGraph;
        //                    BarData.ReportID = content.ReportID;
        //                    BarData.TabID = content.popup.TabID;
        //                    BarData.SectionName = content.SectionName;
        //                    BarData.QueryName = content.QueryName;
        //                    BarData.Type = "Tabs";
        //                    return PartialView("_TabLineGraph", content.LineGraph);
        //                }
        //                else if (content.PreviewType == "Result List")
        //                {
        //                    content.LeadID = content.LeadID;
        //                    return PartialView("_TabsGrid", content);
        //                }
        //                return null;
        //                //return PartialView("_LeadPopupTabContent", content);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }
        //}
        public ActionResult GetViewRecordDetails(List<SectionsData> Data)
        {
            return PartialView("_TabViewForm", Data);
        }
        public ActionResult GetEditRecordDetails(List<SectionsData> Data)
        {
            return PartialView("_TabEditForm", Data);
        }
        public ActionResult GetCreateRecordDetails(List<SectionsData> Data)
        {
            return PartialView("_TabCreateForm", Data);
        }

        public ActionResult QueryDynamicForm(int ReportID, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var result = LeadRepository.QueryDynamicForm(ReportID, LeadID, sDatabase, OrgID);
                return PartialView("_TabViewForm", result.SectionsData);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditData(List<FormData> FormValues, int Tab1ClickID, int LeadID, int ID, string EditType, int BOID, string PopType, int StageID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = ID;
                popup.Tab1ClickID = Tab1ClickID;
                popup.BOID = BOID;
                int OrgID = 0;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                popup.OrganizationID = OrgID;
                popup.UserID = UserID;
                popup.FormType = EditType;
                popup.PopType = PopType;
                popup.StageID = StageID;
                popup.Database = sDatabase;
                VMQueryPreview Data = LeadRepository.EditData(FormValues, popup, sDatabase, sOrgName);
                Data.LeadID = LeadID;
                if (EditType == "FromGrid")
                {
                    return PartialView("_ResultListTab", Data);
                }
                else if (EditType == "CallBack" || EditType == "ManageOverride")
                {
                    return Json(Data, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return PartialView("_TabViewForm", Data.SectionsData);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllNewLeads(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int orgid = 0;
                //int InnrerReportID = LeadRepository.GetInnerReportID(ReportID);
                int LeadID = 0;
                VMQueryPreview model = LeadRepository.RunUserQuery(ReportID, Convert.ToInt32(Convert.ToInt32(User.Identity.GetUserId())), sDatabase, orgid, LeadID, sOrgName);
                ViewBag.queryid = ReportID;
                return PartialView("_NotificationLeads", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult LeadViewOnNotification(int LeadID, int ReportID)
        {
            VMViewPopup popup = new VMViewPopup();
            popup.LeadID = LeadID;
            popup.ReportID = ReportID;
            popup.UserName = "User";
            //popup.ClassID = ClassID;
            //popup.TabName = "ReminderTab";
            return View("LeadPopup", popup);
        }
        public ActionResult GetNextStages(int LeadID, int StageID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var result = LeadRepository.GetNextStages(LeadID, StageID, OrgID, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //[HttpPost]
        public ActionResult GetEditRowDetails(int ID, int Tab1ClickID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var result = LeadRepository.GetEditRowDetails(ID, Tab1ClickID, OrgID, sDatabase);
                return PartialView("_TabEditForm", result.SectionsData);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetCreateRowDetails(int Tab1ClickID = 0, int BOID = 0, int LeadID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var result = LeadRepository.GetCreateRowDetails(Tab1ClickID, BOID, OrgID, sDatabase);
                result.FirstOrDefault().LeadID = LeadID;
                result.FirstOrDefault().Tab1ClickID = Tab1ClickID;
                return PartialView("_TabCreateForm", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult DeleteRowDetails(int LeadID, int Tab1ClickID, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int OrgID = 0;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                var result = LeadRepository.DeleteRowDetails(LeadID, Tab1ClickID, BOID, OrgID, UserID, sDatabase, sOrgName);
                result.LeadID = LeadID;
                return PartialView("_ResultListTab", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult CreateRowFromGrid(List<FormData> FormValues, int Tab1ClickID, int LeadID, string EditType, int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int OrgID = 0;
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = LeadID;
                popup.Tab1ClickID = Tab1ClickID;
                popup.BOID = BOID;
                popup.OrganizationID = OrgID;
                popup.UserID = UserID;
                VMQueryPreview Data = LeadRepository.CreateRowFromGrid(FormValues, popup, sDatabase, sOrgName);
                Data.LeadID = LeadID;
                if (EditType == "FromGrid")
                {
                    return PartialView("_ResultListTab", Data);
                }
                else
                {
                    return PartialView("_TabViewForm", Data.SectionsData);
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
        public ActionResult TableRowValues(string ID, int BOID, string BOName, string ColumnName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var tab = LeadRepository.TableRowValues(ID, BOID, BOName, ColumnName, sDatabase, OrgID);
                return Json(tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult UploadOrgDocuments(string Message, int LeadID, HttpPostedFileBase Upload)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (Upload != null)
                {
                    int OrgID = 0;
                    var EmailID = LeadRepository.GetLeadEmailID(LeadID, OrgID);
                    //var ClientID = APICalls.GetString("api/Documents/GetClientID?EmailID=" + EmailID);
                    WalletDocuments Doc = new WalletDocuments();
                    Doc.Message = Message;
                    Doc.OriginalName = Upload.FileName;
                    Doc.OrganizationID = OrgID;
                    Doc.Type = 20;
                    var res = LeadRepository.SaveOrganizationDocument(Doc, LeadID);
                    string ext = Path.GetExtension(Upload.FileName);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    var DocName = "Doc_" + res + ext;
                    Upload.SaveAs(str + "\\Doc_" + res + ext);
                    Doc.DocumentName = DocName;
                    Doc.ID = res;
                    var Result = LeadRepository.SaveOrganizationDocument(Doc, LeadID);
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetClientDocsList(jQueryDataTableParamModel param, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var EmailID = LeadRepository.GetLeadEmailID(LeadID, OrgID);
                param.EmailID = EmailID[0];
                param.OrganizationID = OrgID;
                var Documents = LeadRepository.GetClientDocuments(param, LeadID, OrgID, "Sent");
                return Json(new
                {
                    sEcho = Documents.sEcho,
                    iTotalRecords = Documents.iTotalRecords,
                    iTotalDisplayRecords = Documents.iTotalDisplayRecords,
                    aaData = Documents.aaData
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

        public ActionResult UploadedDocsGrid(int LeadID)
        {
            return PartialView("_TabUploadedDocsGrid", LeadID);
        }

        public ActionResult GetUploadedDocsGrid(jQueryDataTableParamModel param, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var EmailID = LeadRepository.GetLeadEmailID(LeadID, OrgID);
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = LeadRepository.GetClientDocuments(param, LeadID, OrgID, "Received");
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

        public ActionResult GetTabReportResult(jQueryDataTableParamModel param, int ReportID, int TabID, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                VMQuickSearch Search = new VMQuickSearch();
                Search.ReportID = ReportID;
                Search.UserID = iUserID;
                Search.OrgID = OrgID;
                Search.database = sDatabase;
                Search.RoleID = 0;
                Search.TabID = TabID;
                Search.LeadID = LeadID;
                var result = LeadRepository.GetReportResult(param, Search, sDatabase, iUserID, sOrgName);
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
        public ActionResult PostQuoteToLead(int LeadID, int QuoteID, int BOID, int Tab1ClickID, int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sOrgName = SessionManager.OrganisationName;
                int OrgID = 0;
                VMViewPopup Popup = new VMViewPopup();
                Popup.BOID = BOID;
                Popup.Tab1ClickID = Tab1ClickID;
                Popup.ReportID = ReportID;
                Popup.LeadID = LeadID;
                Popup.QuoteID = QuoteID;
                Popup.OrganizationID = OrgID;
                Popup.UserID = Convert.ToInt32(User.Identity.GetUserId());
                var Res = LeadRepository.PostQuoteToLead(Popup, sOrgName);
                var EmailID = LeadRepository.GetLeadEmailID(LeadID, OrgID);
                var ClientID = EmailID[1];
                WalletMessages Inbox = new WalletMessages();
                Inbox.ClientID = ClientID;
                Inbox.OrganizationID = OrgID;
                //Inbox.Message = "New quote is posted. Please <button class='QteBtn' id='" + Popup.QuoteID + "-"+OrgID+"'>see</button> the details";
                if (Res.TabName == "Policies")
                {
                    Inbox.Message = "New policy is posted";
                    Inbox.MailType = "Insurance";
                }
                else if (Res.TabName == "Quotes")
                {
                    Inbox.Message = "New quote is posted";
                    Inbox.MailType = "Quote";
                }
                var Mesg = LeadRepository.PostMessage(Inbox);
                //var InboxItem = APICalls.Post<InboxMails>(Inbox, "api/Inbox/PostInboxItem");
                Res.LeadID = LeadID;
                return PartialView("_ResultListTab", Res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ShowViewImport(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var Data = LeadRepository.ShowViewImport(ID, OrgID);
                return PartialView("_ViewImport", Data);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #region HTMLColorCodings

        public ActionResult HTMLColorCoding()
        {
            return View();
        }

        public ActionResult GetHTMLColorCodingsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var Documents = LeadRepository.GetHTMLColorCodingsList(param, OrgID, sDatabase);
                return Json(new
                {
                    sEcho = Documents.sEcho,
                    iTotalRecords = Documents.iTotalRecords,
                    iTotalDisplayRecords = Documents.iTotalDisplayRecords,
                    aaData = Documents.aaData
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

        public ActionResult AddHTMLColorCoding()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                HTMLColorCodings model = new HTMLColorCodings();
                model.Columns = LeadRepository.GetAllBoFields();
                model.Values = new List<VMDropDown>();
                return PartialView("_AddHTMLColorCodingForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetColumnValues(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var Data = LeadRepository.GetColumnValues(ID, OrgID);
                return Json(Data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SaveHTMLColorCoding(HTMLColorCodings model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                var Res = LeadRepository.SaveHTMLColorCoding(model, OrgID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditHTMLCoding(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                HTMLColorCodings model = LeadRepository.GetHTMLCodingByID(ID, OrgID, sDatabase);
                return PartialView("_AddHTMLColorCodingForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion HTMLColorCodings
    }
}