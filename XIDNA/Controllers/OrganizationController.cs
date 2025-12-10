using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Collections;
using System.Net;
using log4net;
using System.Configuration;
using XIDNA.Common;
using System.Web.SessionState;
using XICore;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class OrganizationController : Controller, IRequiresSessionState
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IOrganizationRepository OrganizationRepository;

        public OrganizationController() : this(new OrganizationRepository()) { }

        public OrganizationController(IOrganizationRepository OrganizationRepository)
        {
            this.OrganizationRepository = OrganizationRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        CXiAPI oXIAPI = new CXiAPI();
        //
        // GET: /Organization/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetOrganizationList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.GetOrganizationList(param, sDatabase);
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
        public ActionResult GetOrganizationForm()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return RedirectToAction("AddOrganization");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult AddOrganization()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization org = new VMOrganization();
                org.Classes = OrganizationRepository.GetClasses(SessionManager.CoreDatabase);
                org.OrgDetails = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                return View(org);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrganization(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var sAppName = oUser.sAppName;
                var CommonDB = sAppName + "_Shared";
                model.CreatedByName = oUser.sUserName;
                var organizationid = OrganizationRepository.AddOrganization(model, CommonDB, sDatabase);
                string OrgDb = "";
                if (model.ID == 0)
                {
                    if (model.DatabaseType == "Specific")
                    {
                        OrgDb = ServiceConstants.ClientDBName + organizationid;
                    }
                    else
                    {
                        OrgDb = SessionManager.CoreDatabase;
                    }


                    ModelDbContext dbContext = new ModelDbContext(sDatabase);

                    //Create Login for newly created Application
                    cXIAppUsers oXIUser = new cXIAppUsers();
                    oXIUser.sUserName = model.Email;
                    oXIUser.sDatabaseName = CommonDB;
                    oXIUser.sCoreDatabaseName = sDatabase;
                    oXIUser.LockoutEndDateUtc = DateTime.Now;
                    oXIUser.FKiOrganisationID = organizationid;
                    oXIUser.FKiApplicationID = oUser.FKiApplicationID;
                    oXIUser.sAppName = sAppName;
                    dbContext.XIAppUsers.Add(oXIUser);
                    dbContext.SaveChanges();
                    var EncryptedPwd = oXIAPI.EncryptData(model.Password, true, oXIUser.UserID.ToString());
                    oXIUser = dbContext.XIAppUsers.Find(oUser.UserID);
                    oXIUser.sPasswordHash = EncryptedPwd;
                    dbContext.SaveChanges();

                    //Create Role for newly created Application
                    cXIAppRoles Role = new cXIAppRoles();
                    Role.iParentID = 2;
                    Role.sRoleName = "Admin";
                    Role.FKiOrganizationID = organizationid;
                    Role.CreatedBy = iUserID;
                    Role.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Role.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Role.CreatedTime = DateTime.Now;
                    Role.UpdatedTime = DateTime.Now;
                    Role.UpdatedBy = iUserID;
                    dbContext.XIAppRoles.Add(Role);
                    dbContext.SaveChanges();

                    //Assign Role to User
                    cXIAppUserRoles oURole = new cXIAppUserRoles();
                    oURole.UserID = oUser.UserID;
                    oURole.RoleID = Role.RoleID;
                    dbContext.XIAppUserRoles.Add(oURole);
                    dbContext.SaveChanges();
                }
                else
                {
                    int UserID = OrganizationRepository.GetUserID(model, sDatabase);
                    ModelDbContext dbContext = new ModelDbContext(model.DatabaseName);
                    cXIAppUsers oXIUser = new cXIAppUsers();
                    oXIUser = dbContext.XIAppUsers.Find(oUser.UserID);
                    oXIUser.sUserName = model.Email;
                    oXIUser.sEmail = model.Email;
                    oXIUser.sFirstName = model.Name;
                    oXIUser.sLastName = model.Name;
                    oXIUser.sPhoneNumber = model.PhoneNumber;
                    dbContext.SaveChanges();
                }
                return Json(organizationid, JsonRequestBehavior.AllowGet);
                //return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                logger.Info("Exception Occured");
                string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~");
                string str = physicalPath + "\\Content/images";
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(ex.Message + "--------" + str + "\\" + model.Name + ".png");
            }

        }
        [HttpPost]
        public ActionResult SaveLogo(int id, HttpPostedFileBase UploadImage)
        {
            string sDatabase = SessionManager.CoreDatabase;
            VMOrganization model = OrganizationRepository.GetOrganizationDetails(id, sDatabase);
            try
            {
                if (UploadImage != null)
                {
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    UploadImage.SaveAs(str + "\\" + model.Name + "_" + id + ".png");
                    model.Logo = model.Name + "_" + id + ".png";
                    var res = OrganizationRepository.SaveLogo(model, sDatabase);
                }
                return Json(true, "text/html");
            }

            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        public ActionResult GetOrganizationDetails(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization model = OrganizationRepository.GetOrganizationDetails(OrgID, sDatabase);
                return View("EditOrgGeneralDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetOrganizationDetail(int orgid)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization model = OrganizationRepository.GetOrganizationDetails(orgid, sDatabase);
                return PartialView("ViewOrgGeneralDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrganizationDetailtoView(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization model = OrganizationRepository.GetOrganizationDetails(OrgID, sDatabase);

                return View("ViewOrgGeneralDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrgEditDetails(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PartialView("_EditOrgGenDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrgViewDetails(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PartialView("_ViewOrgGenDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrgEditUpdatedData(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization model = OrganizationRepository.GetOrganizationDetails(OrgID, sDatabase);
                return PartialView("_EditOrgGenDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrgUpdatedData(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMOrganization model = OrganizationRepository.GetOrganizationDetails(OrgID, sDatabase);
                return PartialView("_ViewOrgGenDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        [HttpGet]
        public ActionResult GetOrganizationContacttoView(int orgid)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMOrganization model = OrganizationRepository.GetOrgContactDetails(orgid, iUserID, sOrgName, sDatabase);
                //return Json(model, JsonRequestBehavior.AllowGet);
                return PartialView("ViewOrgContactDetails", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetOrganizationContactView(int ido)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                VMOrganization model = OrganizationRepository.GetOrgContactDetails(ido, iUserID, sOrgName, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult AddOrganizationContact(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            var orgid = 0;
            try
            {
                orgid = OrganizationRepository.AddOrganizationContact(model, sDatabase);
                return Json(orgid, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            //return RedirectToAction("AddOrgContactDetails", new { orgid = orgid });
            //return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateOrganizationContact(OrganizationContacts model)
        {
            var orgid = 0;
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                orgid = OrganizationRepository.UpdateOrganizationContact(model, iUserID, sOrgName, sDatabase);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
            //return RedirectToAction("AddOrgContactDetails", new { orgid = orgid });
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult IsExistsOrgName(string Name, string Type, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsOrgName(Name, Type, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsOrgEmail(string Email, string Type, int ID, string OldEmail)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsOrgEmail(Email, Type, ID, OldEmail, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //poovanna......
        //GET
        public ActionResult SaveOrgWithLocation()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                OrganizationLocations model = new OrganizationLocations();
                var OrgNames = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                model.OrgDetails = OrgNames;
                model.OrganizationID = OrgID;
                return View(model);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        //Both get and post has the same parameter so it produces error.
        [ActionName("SaveOrgWithLocation")]
        public JsonResult SaveOrgWithLocations(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                model.OrganizationID = model.LocOrganizationID;
                int OrgID = OrganizationRepository.SaveOrgDetails(model, sDatabase);
                return Json(OrgID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        //Both get and post has the same parameter so it produces error.
        public JsonResult SaveOrganizationLocation(OrganizationLocations model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var OrgID = OrganizationRepository.SaveOrgLocDetails(model, SessionManager.CoreDatabase);
                return Json(OrgID, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //Get 1: First display this layout and then display the table inside i.e its easy to refresh when the modification is done to the table.
        public ActionResult OrganizationLocations()
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            return View(OrgID);
        }
        public ActionResult DisplayOrgLocDetails(jQueryDataTableParamModel param, int LocOrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                LocOrgID = OrgID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.DisplayOrganizations(param, LocOrgID, iUserID, sOrgName, sDatabase);
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
        //Get Edit
        public ActionResult EditOrgLocDetails(int ID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                string role = oUser.Role.sRoleName;
                if (ID == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                OrganizationLocations OrgDetails = OrganizationRepository.EditOrgLocation(ID, OrgID, sDatabase);
                if (OrgDetails == null)
                {
                    return null;
                }
                OrgDetails.Role = role;
                return View(OrgDetails);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        // Delete
        [HttpPost]
        public JsonResult DeleteOrgLocation(int ColumnID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                OrganizationLocations OrgDetails = OrganizationRepository.DeleteOrgLocation(ColumnID, OrgID, sDatabase);

                if (OrgDetails == null)
                {
                    return null;
                }
                return Json(ColumnID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        //Check uniqueness of location entered   
        public ActionResult CheckLocation(string Location, int LocOrganizationID, int LocID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsOrgLocation(Location, LocOrganizationID, LocID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        //Check uniqueness of location entered   
        public ActionResult CheckOrgLocation(string Location, int OrganizationID, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsOrgLocation(Location, OrganizationID, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        //Check uniqueness of location entered   
        public ActionResult IsExistsLocationCode(string LocationCode, int LocOrganizationID, int LocID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsLocationCode(LocationCode, LocOrganizationID, LocID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        //Check uniqueness of location entered   
        public ActionResult IsExistsLocationCodeSpecific(string LocationCode, int OrganizationID, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsLocationCode(LocationCode, OrganizationID, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddOrganizationClasses(VMOrganization model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = OrganizationRepository.AddOrganizationClasses(model, OrgID, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrganizationClasses(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = OrganizationRepository.GetOrganizationClasses(OrgID, iUserID, sOrgName, sDatabase);
                return PartialView("_OrgClassesDetails", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //OrganizationLoactionGrid
        public ActionResult OrgLocationGrid(int LocOrgID)
        {
            //int LocOrgID = Util.GetOrganisationID();
            return PartialView("_OrgLocationGrid", LocOrgID);
        }

        public ActionResult SpecificOrgLocationGrid(jQueryDataTableParamModel param, int LocOrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.SpecificOrgLocationGrid(param, LocOrgID, sDatabase);
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

        public ActionResult SpecificOrgLocation(int LocOrgID)
        {
            return PartialView("_SpecificOrgLocation", LocOrgID);
        }
        public ActionResult SpecificOrgLocationList(jQueryDataTableParamModel param, int LocOrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.SpecificOrgLocationList(param, LocOrgID, sDatabase);
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
        //Organization Sources
        public ActionResult OrganizationSources()
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            return View(OrgID);
        }

        public ActionResult GetOrgSources(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.GetOrgSources(param, OrgID, iUserID, sOrgName, sDatabase);
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

        public ActionResult OrganizationForms()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                VMOrganizationForms forms = new VMOrganizationForms();
                var orglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                forms.OrgList = orglist;
                forms.OrganizationID = OrgID;
                var suborglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                var providerlist = OrganizationRepository.GetProviders(SessionManager.CoreDatabase);
                var sourcetypes = OrganizationRepository.GetSourceTypes(SessionManager.CoreDatabase);
                forms.SourceTypes = sourcetypes;
                forms.OrgList = suborglist;
                forms.ProviderList = providerlist;
                forms.OrganizationID = OrgID;
                forms.SubOrganizationID = OrgID;
                forms.Role = oUser.Role.sRoleName;
                forms.SourcesList = OrganizationRepository.GetOrgSources(OrgID, sDatabase);
                forms.ClassesList = OrganizationRepository.GetOrgClasses(OrgID, sDatabase);
                forms.SorFieldOrganizationID = OrgID;
                return PartialView("_OrganizationForms", forms);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult CreateOrgSource()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                OrganizationSources source = new OrganizationSources();
                var orglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                source.OrgList = orglist;
                source.OrganizationID = OrgID;
                return PartialView("_OrgSourceForm", source);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveSource(VMOrganizationForms Source)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = OrganizationRepository.SaveSource(Source, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditOrgSource(int SourceID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var result = OrganizationRepository.EditOrgSource(SourceID, OrgID, sDatabase);
                var orglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                var suborglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                var providerlist = OrganizationRepository.GetProviders(SessionManager.CoreDatabase);
                var sourcetypes = OrganizationRepository.GetSourceTypes(SessionManager.CoreDatabase);
                result.SourceTypes = sourcetypes;
                result.OrgList = suborglist;
                result.ProviderList = providerlist;
                result.OrgList = orglist;
                result.OrganizationID = OrgID;
                result.OrgID = OrgID;
                result.SubOrganizationID = OrgID;
                result.Role = oUser.Role.sRoleName;
                result.SourcesList = OrganizationRepository.GetOrgSources(OrgID, sDatabase);
                result.ClassesList = OrganizationRepository.GetOrgClasses(OrgID, sDatabase);
                return PartialView("_OrganizationForms", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult SpecificSorces(int OrgID)
        {
            return PartialView("_SpecificSourcesList", OrgID);
        }
        public ActionResult SpecSourcesList(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.SpecificSourcesList(param, OrgID, sDatabase);
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
        public ActionResult IsExistsSrcEmail(string Email, string Type, int ID, string OldEmail)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsSrcEmail(Email, Type, ID, OldEmail, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //Organization Subscriptions
        public ActionResult OrganizationSubscriptions()
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            return View(OrgID);
        }

        public ActionResult GetOrgSubscriptions(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.GetOrgSubscriptions(param, OrgID, iUserID, sOrgName, sDatabase);
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
        public ActionResult OrgColumnDetails(string Name, string SubscriptionID, int OrgID)//37953
        {
            string sDatabase = SessionManager.CoreDatabase;
            var result = OrganizationRepository.OrgColumnDetails(Name, SubscriptionID, OrgID, sDatabase);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CreateOrgSubscription(int ID, int OrganizationID, string CreationType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                if (ID != 0)
                {
                    //int orgID = Convert.ToInt32(OrgID);
                    var Details = OrganizationRepository.GetOrgSubscriptionDetails(ID, OrganizationID, sDatabase);
                    VMOrganizationForms sub = new VMOrganizationForms();
                    var orglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                    var providerlist = OrganizationRepository.GetProviders(SessionManager.CoreDatabase);
                    //oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                    sub.SubOrgList = orglist;
                    sub.ProviderList = providerlist;
                    sub.SubOrganizationID = OrganizationID;
                    sub.SourcesList = OrganizationRepository.GetOrgSources(OrganizationID, sDatabase);
                    sub.ClassesList = OrganizationRepository.GetOrgClasses(OrganizationID, sDatabase);
                    sub.LocationCodes = OrganizationRepository.GetLocationCodesList(OrganizationID, sDatabase);
                    sub.ColumnsList = OrganizationRepository.GetColumnsList(SessionManager.CoreDatabase);
                    sub.ID = ID;
                    sub.SubsriptionID = Details.SubscriptionID;
                    sub.OrganizationID = OrgID;
                    sub.SourceID = Details.SourceID;
                    sub.ClassID = Details.ClassID;
                    sub.LeadCost = Details.LeadCost;
                    sub.Email = Details.Email;
                    sub.PostCode = Details.PostCode;
                    sub.RenewalDate = Details.RenewalDate;
                    sub.SubStatusTypeID = Details.StatusTypeID;
                    sub.CreationType = CreationType;
                    sub.CopySubID = ID;
                    sub.LocationCode = Details.LocationCode;
                    if (CreationType == "Copy")
                    {
                        sub.ID = 0;
                    }
                    return PartialView("_OrgSubscriptionForm", sub);
                }
                else
                {
                    VMOrganizationForms sub = new VMOrganizationForms();
                    var orglist = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                    var providerlist = OrganizationRepository.GetProviders(SessionManager.CoreDatabase);
                    //oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                    sub.SubOrgList = orglist;
                    sub.ProviderList = providerlist;
                    sub.SubOrganizationID = OrgID;
                    sub.SourcesList = OrganizationRepository.GetOrgSources(OrgID, sDatabase);
                    sub.ClassesList = OrganizationRepository.GetOrgClasses(OrgID, sDatabase);
                    sub.ColumnsList = OrganizationRepository.GetColumnsList(SessionManager.CoreDatabase);
                    sub.LocationCodes = OrganizationRepository.GetLocationCodesList(OrganizationID, sDatabase);
                    return PartialView("_OrgSubscriptionForm", sub);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //[HttpPost]
        public ActionResult Sources(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var orgsources = OrganizationRepository.GetOrgSources(OrgID, sDatabase);
                return Json(orgsources, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOrgClasses(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var orgsources = OrganizationRepository.GetOrgClasses(OrgID, sDatabase);
                return Json(orgsources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetSourceTypes(int SourceID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var orgsources = OrganizationRepository.GetSourceTypes(SourceID, OrgID, sDatabase);
                return Json(orgsources, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetSourceClasses(int SourceID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var orgsources = OrganizationRepository.GetSourceClasses(SourceID, OrgID, sDatabase);
                return Json(orgsources, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult GetProviders( int OrgID, int SourceID, string Type, int Clas)
        //{
        //    var orgsources = OrganizationRepository.GetProviders(OrgID, SourceID, Type, Clas);
        //    return Json(orgsources, JsonRequestBehavior.AllowGet);
        //}

        [HttpPost]
        public ActionResult SaveSubscription(VMOrganizationForms model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = OrganizationRepository.SaveSubscription(model, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SpecificSubscriptions(int OrgID, int SourceID)
        {
            List<int> Values = new List<int>();
            Values.Add(OrgID);
            Values.Add(SourceID);
            return PartialView("_SpecificSubscriptionsList", Values);
        }

        public ActionResult SpecificSubscriptionsList(jQueryDataTableParamModel param, int OrgID, int SourceID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.SpecificSubscriptionsList(param, OrgID, SourceID, sDatabase);
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
        //Check uniqueness of source entered   
        public ActionResult IsExistsSourceName(string Name, int ID, int OrganizationID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return OrganizationRepository.IsExistsSourceName(Name, ID, OrganizationID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult OrganizationSourceFields()
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            return View(OrgID);
        }

        public ActionResult GetOrgSourceFields(jQueryDataTableParamModel param, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.GetOrgSourceFields(param, OrgID, sDatabase);
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

        public ActionResult OrganizationSourceSpecFields(string SubID)
        {
            return PartialView("_OrgSpecSourceFields", SubID);
        }

        public ActionResult GetOrgSpecSourceFields(jQueryDataTableParamModel param, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.GetOrgSpecSourceFields(param, OrgID, SubID, sDatabase);
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
        public ActionResult SourceFieldsForm()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = OrganizationRepository.GetSubscriptions(sDatabase, OrgID);
                VMOrganizationForms model = new VMOrganizationForms();
                model.Subscriptions = result;
                model.SorFieldOrgsList = OrganizationRepository.GetOrgNames(SessionManager.CoreDatabase);
                model.SorFieldOrganizationID = OrgID;
                return PartialView("_SourceFields", model);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetSubscriptions(int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = OrganizationRepository.GetSubscriptions(sDatabase, OrgID);
                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult AddSourceFields(VMOrganizationForms model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = OrganizationRepository.AddSourceFields(model, OrgID, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult DeleteSourceField(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var result = OrganizationRepository.DeleteSourceField(ID, OrgID, sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult OrgImages()
        {
            return View("OrganizationImagesList");
        }

        //organization Logos
        public ActionResult OrgLogo()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                //String[] filenames = System.IO.Directory.GetFiles("D:\\TfsProject\\XIDynaware\\XIDynaware\\XIDynaware\\Content\\images", "images_0" + "_*.png");
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var filenames = OrganizationRepository.GetOrgImages(OrgID, sDatabase);

                //var FilePath = new List<string>();
                //foreach (var items in filenames)
                //{
                //    string a = items.Replace("D:\\TfsProject\\XIDynaware\\XIDynaware\\XIDynaware", "");
                //    FilePath.Add(a);
                //}           
                return View("OrganizationImages", filenames);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //saving the Logo
        [HttpPost]
        public ActionResult SaveOtherImages(List<HttpPostedFileBase> UploadImage)
        {
            string sDatabase = SessionManager.CoreDatabase;
            //VMOrganization model = OrganizationRepository.GetOrganizationDetails(id);
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                if (UploadImage != null)
                {
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    logger.Info(str);
                    foreach (var items in UploadImage)
                    {
                        OrganizationImages model = new OrganizationImages();
                        oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                        model.OrganizationID = OrgID;
                        model.FileName = "";
                        var res = OrganizationRepository.SaveOtherImages(model, OrgID, sDatabase);
                        items.SaveAs(str + "\\" + "images_" + OrgID + "_" + res + ".png");
                        string sFilePath = "images_" + OrgID + "_" + res + ".png";
                        model.ID = res;
                        model.FileName = sFilePath;
                        var Img = OrganizationRepository.SaveOtherImages(model, OrgID, sDatabase);
                    }
                    //String[] filenames = System.IO.Directory.GetFiles("D:\\TfsProject\\XIDynaware\\XIDynaware\\XIDynaware\\Content\\images", "images_0" + "_*.png");                   
                    //model.Logo = model.Name + ".png";
                    //var res = OrganizationRepository.SaveLogo(model);                  
                }
                return Json(true, "text/html");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(false, "text/html");
                //string physicalPath = System.Web.HttpContext.Current.Server.MapPath("~");
                //string str = physicalPath + "\\Content/images";
                //return Json(ex.Message + "--------" + str + "\\" + model.Name + ".png");
            }
        }
        //Deleting the Logo

        [HttpPost]
        public ActionResult DeleteLogo(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                OrganizationImages OrgImages = OrganizationRepository.DeleteLogo(ID, iUserID, sOrgName, sDatabase);

                if (OrgImages == null)
                {
                    return null;
                }
                return Json(ID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //For Displaying the Images Grid
        public ActionResult OrganizationImagesGrid(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.OrganizationImagesGrid(param, OrgID, sDatabase);
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

        //opening of OrgTeamsGrid Form
        public ActionResult OrgTeams()
        {
            return View("OrgTeamsGrid");
        }
        //Opening of CreateOrgTeamsForm
        public ActionResult CreateOrgTeams()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                OrganizationTeams OT = new OrganizationTeams();
                OT = OrganizationRepository.GetUsersList(OrgID, sDatabase);
                OT.OrganizationID = OrgID;
                return View("CreateOrgTeamsForm", OT);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveOrganizationTeams(OrganizationTeams model, string Name, string[] SelectedUsers, string[] UserIDs, int ID, int StatusTypeID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                model.Name = Name;
                model.StatusTypeID = StatusTypeID;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var sd = OrganizationRepository.SaveOrganizationTeams(model, SelectedUsers, UserIDs, OrgID, sDatabase);
                model.ID = ID;
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult OrgTeamsGrid(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.OrgTeamsGrid(param, OrgID, sDatabase);
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
        public ActionResult EditOrgTeams(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var OT = OrganizationRepository.EditOrgTeams(ID, OrgID, sDatabase);
                OT.OrganizationID = OrgID;
                return View("CreateOrgTeamsForm", OT);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsTeamName(string Name, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                return OrganizationRepository.IsExistsTeamName(Name, ID, OrgID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveSubscriptionColumns(VMOrganizationForms model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = OrganizationRepository.SaveSubscriptionColumns(model, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SpecificSubCoulmns(string SubID)
        {
            return PartialView("_SpecificSubCoulmnsList", SubID);
        }

        public ActionResult SpecificSubCoulmnsList(jQueryDataTableParamModel param, string SubID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = OrganizationRepository.SpecificSubCoulmnsList(param, SubID, OrgID, sDatabase);
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

        public ActionResult ChangeTheme(string Theme, int ID, string returnUrl)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
                var Res = OrganizationRepository.ChangeTheme(ID, OrgID, sDatabase);
                //User.AddUpdateClaim("ThemeID", Theme.ToString());
                SessionManager.Theme = Theme;
                return Redirect(returnUrl);
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