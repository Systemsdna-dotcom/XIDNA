using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using log4net;
using XIDNA.Common;
using XICore;

namespace XIDNA.Controllers
{
    [Authorize]
    public class IOServerDetailsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IIOServerDetailsRepository IOServerDetailsRepository;

        public IOServerDetailsController() : this(new IOServerDetailsRepository()) { }

        public IOServerDetailsController(IIOServerDetailsRepository IOServerDetailsRepository)
        {
            this.IOServerDetailsRepository = IOServerDetailsRepository;
        }
        CommonRepository Common = new CommonRepository();
        XIInfraUsers oUser = new XIInfraUsers();
        //
        // GET: /IOServerDetails/
        public ActionResult Index()
        {
            return View();
        }
        //opening of serverDetails form
        public ActionResult ServerDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int Type = 1;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                IOServerDetails Details = new IOServerDetails();
                Details.OrganizationID = OrgID;
                Details.Type = Type;
                return View("IOServerDetailsList", Details);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //opening of IOServer Details Form
        public ActionResult CreateServerDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                IOServerDetails IOSD = new IOServerDetails();
                List<VMDropDown> organizations = IOServerDetailsRepository.GetOrganizations(sDatabase);
                IOSD.organizations = organizations;
                IOSD.OrganizationID = 0;
                IOSD.Role = "Role";//later chage to rolename
                return View("IOServerDetails", IOSD);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //Saving IOServer Details Form
        [HttpPost]
        public ActionResult SaveServerDetails(IOServerDetails model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sd = IOServerDetailsRepository.SaveServerDetails(model);
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }
        //For  Displaying the grid
        public ActionResult ServerDetailsList(jQueryDataTableParamModel param, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = IOServerDetailsRepository.ServerDetailsList(param, Type, OrgID, 0, sDatabase);
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
        //Editing of IOServer Details form
        public ActionResult EditServerDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var IOSD = IOServerDetailsRepository.EditServerDetails(ID);
                List<VMDropDown> organizations = IOServerDetailsRepository.GetOrganizations(sDatabase);
                IOSD.organizations = organizations;
                IOSD.Role = "Role";//later chage to rolename
                //IOSD.OrganizationID = 0;
                return View("IOServerDetails", IOSD);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        public ActionResult SMSServerDetails()
        {
            IOServerDetails Details = new IOServerDetails();
            Details.OrganizationID = 0;
            Details.Type = 2;
            return View("SMS_ServerDetailsList", Details);
        }
        public ActionResult CreateSMSServerDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                IOServerDetails IOSD = new IOServerDetails();
                List<VMDropDown> organizations = IOServerDetailsRepository.GetOrganizations(sDatabase);
                IOSD.organizations = organizations;
                IOSD.OrganizationID = 0;
                IOSD.Role = "Role";//later chage to rolename
                return View("SMS_ServerDetails", IOSD);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SaveSMSServerDetails(IOServerDetails model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sd = IOServerDetailsRepository.SaveSMSServerDetails(model);
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult EditSMSServerDetails(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var IOSD = IOServerDetailsRepository.EditSMSServerDetails(ID);
                List<VMDropDown> organizations = IOServerDetailsRepository.GetOrganizations(sDatabase);
                IOSD.organizations = organizations;
                //IOSD.OrganizationID = 0;
                IOSD.Role = "Role";//later chage to rolename
                return View("SMS_ServerDetails", IOSD);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;

            }
        }
        public ActionResult SMSServerDetailsList(jQueryDataTableParamModel param, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = IOServerDetailsRepository.SMSServerDetailsList(param, Type, OrgID, sDatabase);
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

        public ActionResult OrgIOServerDetailsGrid(int OrgID, int Type, int Category)
        {
            var lDetails = new List<int>();
            lDetails.Add(OrgID);
            lDetails.Add(Type);
            lDetails.Add(Category);
            return PartialView("_SpecificIOServerDetailsList", lDetails);
        }
        public ActionResult SpecificOrgIOServerDetailsGrid(jQueryDataTableParamModel param, int OrgID, int Type, int Category)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //int OrgID = 0;
                //string database = Util.GetDatabaseName();
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = IOServerDetailsRepository.ServerDetailsList(param, Type, OrgID, Category, sDatabase);
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
        public ActionResult OrgSMSServerDetailsGrid(int OrgID, int Type)
        {
            var lDetails = new List<int>();
            lDetails.Add(OrgID);
            lDetails.Add(Type);
            return PartialView("_SpecificSMSServerDetailsList", lDetails);
        }
        public ActionResult SpecificOrgSMSServerDetailsGrid(jQueryDataTableParamModel param, int OrgID, int Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = IOServerDetailsRepository.SMSServerDetailsList(param, Type, OrgID, sDatabase);
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
    }
}