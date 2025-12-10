using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XIDNA.Repository;
using XIDNA.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Net;
using XIDNA.Common;
using XIDNA.Models;
using XICore;
using XISystem;
using System.Web.Script.Serialization;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class UsersController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IUsersRepository UsersRepository;

        public UsersController() : this(new UsersRepository()) { }

        public UsersController(UsersRepository UsersRepository)
        {
            this.UsersRepository = UsersRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        CXiAPI oXIAPI = new CXiAPI();
        XIInfraRoles oXIInfraRoles = new XIInfraRoles();
        XIDropDown XIDrop = new XIDropDown();
        XIInfraHierarchy oXIInfraHierarchy = new XIInfraHierarchy();
        XIInfraTeams oXIInfraTeams = new XIInfraTeams();
        // GET: /Users/
        public async Task<ActionResult> Index(string sType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var RoleID = oUser.Role.RoleID;
                ModelDbContext dbContext = new ModelDbContext(sDatabase);
                var GropMenus = dbContext.XIAppRoles.Where(m => m.RoleID == RoleID).ToList();
                ViewBag.sType = sType;
                return View(GropMenus);
                //await UserManager.Users.ToListAsync()
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public async Task<ActionResult> GetUsersList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ViewBag.GroupID = OrgID;
                XIInfraUsers xifuser = new XIInfraUsers();
                XIInfraRoles xifroles = new XIInfraRoles();
                XIInfraUserRoles xifuserroles = new XIInfraUserRoles();
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = UsersRepository.GetUsersList(param, OrgID, sDatabase);
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


        //public ActionResult Edit(int userid)
        //{
        //    var res = UsersRepository.GetUser(userid);
        //    return PartialView(res);
        //}

        public ActionResult CreateEmployeeWindow()
        {
            return RedirectToAction("Create");
        }
        public ActionResult Create()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                XIInfraUsers xifuser = new XIInfraUsers();
                xifuser.UserID = iUserID;
                xifuser = (XIInfraUsers)xifuser.Get_UserDetails(sDatabase).oResult;
                var RoleID = oUser.Role.RoleID;
                var RolesTree = oUser.Role.Get_RolesTree(sDatabase, OrgID);
                List<XIInfraRoles> GropMenus = (List<XIInfraRoles>)RolesTree.oResult;// dbCore.XIAppRoles.Where(m => m.RoleID == RoleID).ToList();
                //GropMenus.Add(xifuser.Role);
                List<VMDropDown> orgLocation = UsersRepository.GetOrgLocation(OrgID, iUserID, sOrgName, sDatabase);
                //var data = oXIInfraHierarchy.GetHierarchiesList();
                RegisterViewModel models = new RegisterViewModel();
                models.DropDown = orgLocation;
                //models.HierarchyDropDown = data.ToList().Select(c => new VMDropDown { Expression = c.Value.ToString(), text = c.text }).ToList();
                //models.TeamsDropDown = oXIInfraTeams.GetTeamsList().Select(c => new VMDropDown { Expression = c.Value.ToString(), text = c.text }).ToList();
                models.Group = GropMenus;
                return PartialView(models);
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
        [AllowAnonymous]
        public ActionResult Create(RegisterViewModel userViewModel)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInfraUsers xifUser = new XIInfraUsers();
                XIInfraEncryption xifEncrypt = new XIInfraEncryption();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                userViewModel.UserName = userViewModel.Email;
                int iOrgID = oUser.FKiOrganisationID;
                List<RegisterViewModel> OrgLocation = new List<RegisterViewModel>();
                string LocIDs = "";
                foreach (var user in userViewModel.Locs)
                {
                    LocIDs = LocIDs + user + ",";
                }
                LocIDs = LocIDs.Substring(0, LocIDs.Length - 1);
                if (userViewModel.GroupID == null)
                {
                    return Json(new { Response = false, Error = "Please select role from left tree" });
                }

                XIDXI oDXI = new XIDXI();
                oDXI.sOrgDatabase = sDatabase;
                var xiOrgData = oDXI.Get_OrgDefinition(null, iOrgID.ToString());
                var noofusers = (int)oUser.Get_NoOfUsers(sDatabase).oResult;// UsersRepository.GetNoOfUsers(iOrgID, sDatabase);
                XIDOrganisation xiOrg = (XIDOrganisation)xiOrgData.oResult;
                if (noofusers < xiOrg.NoOfUsers)
                {
                    if (ModelState.IsValid)
                    {
                        //DateTime dt = DateTime.ParseExact(System.DateTime.Now.ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
                        // dt = dt.Date.AddMonths(6);
                        //ModelDbContext dbContext = new ModelDbContext();
                        //ModelDbContext dbCore = new ModelDbContext(sDatabase);
                        //cXIAppUsers oUser = new cXIAppUsers();
                        xifUser.sUserName = userViewModel.Email;
                        xifUser.sEmail = userViewModel.Email;
                        xifUser.sFirstName = userViewModel.FirstName;
                        xifUser.sLastName = userViewModel.LastName;
                        xifUser.FKiOrganisationID = iOrgID;
                        xifUser.FKiApplicationID = oUser.FKiApplicationID;
                        //oUser.sDatabaseName = sDatabase;
                        xifUser.sDatabaseName = oUser.sDatabaseName; //dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == iOrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                        xifUser.sCoreDatabaseName = sDatabase;
                        xifUser.sPhoneNumber = userViewModel.PhoneNumber;
                        xifUser.iReportTo = userViewModel.ReportTo;
                        xifUser.LockoutEndDateUtc = DateTime.Now;
                        xifUser.sLocation = LocIDs;
                        xifUser.iPaginationCount = 10;
                        xifUser.sMenu = "Open,Open";
                        xifUser.iInboxRefreshTime = 0;
                        xifUser.CreatedBy = xifUser.UpdatedBy = iUserID;
                        xifUser.CreatedTime = DateTime.Now;
                        xifUser.CreatedBySYSID = xifUser.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        xifUser.StatusTypeID = userViewModel.StatusTypeID;
                        xifUser.sHierarchy = userViewModel.sHierarchy; //string.IsNullOrEmpty(userViewModel.sHierarchy) ? "" : GetHierarchies(userViewModel.sHierarchy);
                        xifUser.sInsertDefaultCode = xifUser.sInsertDefaultCode;
                        xifUser.sUpdateHierarchy = userViewModel.sUpdateHierarchy;
                        xifUser.sViewHierarchy = userViewModel.sViewHierarchy;
                        xifUser.sDeleteHierarchy = userViewModel.sDeleteHierarchy;
                        xifUser.FKiTeamID = userViewModel.FKiTeamID;
                        var oUserData = xifUser.Save_User(sDatabase);
                        if (oUserData.xiStatus == 0 && oUserData.oResult != null)
                        {
                            xifUser = (XIInfraUsers)oUserData.oResult;
                            var EncryptedPwd = xifEncrypt.EncryptData(userViewModel.Password, true, xifUser.UserID.ToString());
                            xifUser.sPasswordHash = EncryptedPwd;
                            oUserData = xifUser.Update_User(sDatabase);
                            if (oUserData.xiStatus == 0 && oUserData.oResult != null)
                            {
                                xifUser = (XIInfraUsers)oUserData.oResult;
                                if (xifUser.UserID > 0)
                                {
                                    var Roles = userViewModel.GroupID.Split(',').ToList();
                                    foreach (var items in Roles)
                                    {
                                        XIInfraRoles xifRole = new XIInfraRoles();
                                        xifRole.RoleID = Convert.ToInt32(items);
                                        var RoleData = xifRole.Get_RoleDefinition(sDatabase);
                                        if (RoleData.xiStatus == 0 && RoleData.oResult != null)
                                        {
                                            xifRole = (XIInfraRoles)RoleData.oResult;
                                            XIInfraUserRoles xifURole = new XIInfraUserRoles();
                                            xifURole.UserID = xifUser.UserID;
                                            xifURole.RoleID = xifRole.RoleID;
                                            var URoleData = xifURole.Save_UserRole(sDatabase);
                                            if (URoleData.xiStatus == 0 && URoleData.oResult != null)
                                            {

                                            }
                                            else
                                            {
                                                return Json(URoleData, JsonRequestBehavior.AllowGet);
                                            }
                                        }
                                        else
                                        {
                                            return Json(RoleData, JsonRequestBehavior.AllowGet);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                return Json(oUserData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(oUserData, JsonRequestBehavior.AllowGet);
                        }

                        return Json(new { Response = true });
                        //return RedirectToAction("Index");
                    }
                    else
                    {
                        var errors = ModelState.Select(x => x.Value.Errors)
                                     .Where(y => y.Count > 0)
                                     .ToList();
                        return Json(new { Response = false, Error = errors.First().First().ErrorMessage });
                    }

                }
                else
                {
                    return Json(new { Response = false, Error = "Users limit reached. Unable to create new user" });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new { Response = false, Error = "Error while creating employee!!!<br/> Please try again" });
            }
        }
        [HttpPost]
        public ActionResult UpdateUser(cXIAppUsers model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var res = UsersRepository.UpdateUser(model, sDatabase);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpGet]
        public ActionResult AddRole(string sType)
        {
            ViewBag.sType = sType;
            return View();
        }
        public async Task<ActionResult> GetRolesList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                ViewBag.GroupID = OrgID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = UsersRepository.GetRolesList(param, OrgID, sDatabase);
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
        [HttpGet]
        public ActionResult CreateRoles()
        {
            return RedirectToAction("AddRoles");
        }

        public ActionResult AddRoles()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName; string sConfigDatabase = SessionManager.ConfigDatabase;
                XIInfraRoles xifRole = new XIInfraRoles();
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var RolesTree = oUser.Role.Get_RolesTree(sDatabase, oUser.FKiOrganisationID);
                List<XIInfraRoles> GropMenus = (List<XIInfraRoles>)RolesTree.oResult;
                ViewBag.Group = GropMenus;
                XIDrop.FKiAppID = oUser.FKiApplicationID;
                var oXILayoutList = (List<XIDropDown>)XIDrop.Get_XILayoutsDDL(sConfigDatabase).oResult;
                var oXIThemesList = (List<XIDropDown>)XIDrop.Get_XIThemesDDL(sConfigDatabase).oResult;
                xifRole.bSignalR = false;
                xifRole.LayoutsList = oXILayoutList.ToList().Select(m => new XIDropDown { text = m.text, Value = m.Value }).ToList();
                xifRole.ThemesList = oXIThemesList.ToList().Select(m => new XIDropDown { text = m.text, Value = m.Value }).ToList();
                return View("CreateRoles", xifRole);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GroupTree(List<XIInfraRoles> treedata)
        {
            ViewBag.Disable = "No";
            return PartialView(treedata);
        }
        [HttpPost]
        public ActionResult CreateRole(XIInfraRoles xifRole)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIInfraCache oCache = new XIInfraCache();
                var oDatasource = (XIDataSource)oCache.GetObjectFromCache(XIConstant.CacheDataSource, sDatabase);
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                int RoleID = 0;
                if (xifRole.iParentID == 0)
                {
                    return Json(new { Response = false, Error = "Select role from left tree" });
                }
                xifRole.iParentID = xifRole.iParentID;
                xifRole.FKiOrganizationID = OrgID;
                XIConfigs oConfig = new XIConfigs();
                var xifRoleData = oConfig.Save_Role(xifRole, oDatasource.XIGUID.ToString());
                //var xifRoleData = xifRole.Save_Role(sDatabase);
                if (xifRoleData.bOK)
                {
                    var oRoleI = (XIIBO)xifRoleData.oResult;
                    var sRoleID = oRoleI.AttributeI("roleid").sValue;
                    int.TryParse(sRoleID, out RoleID);
                    xifRole.RoleID = RoleID;
                }
                //var res = UsersRepository.AddRole(xifRole, sDatabase);
                if (xifRole.RoleID > 0)
                {
                    return Json(new { Response = true, Id = xifRole.RoleID, RoleName = xifRole.sRoleName, Type = "Edit" });
                }
                else
                {
                    return Json(new { Response = true, Id = xifRole.RoleID, RoleName = xifRole.sRoleName, Type = "Create" });
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new { Response = false, Error = "Error while creating role. Please try again" });
            }
        }

        public ActionResult EditRole(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string sConfigDatabase = SessionManager.ConfigDatabase;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                XIInfraRoles model = new XIInfraRoles();
                model = (XIInfraRoles)oXIInfraRoles.Get_RoleByID(ID, sDatabase).oResult;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                var RolesTree = oUser.Role.Get_RolesTree(sDatabase, oUser.FKiOrganisationID);
                List<XIInfraRoles> GropMenus = (List<XIInfraRoles>)RolesTree.oResult;
                ViewBag.Group = GropMenus;
                model.RoleID = ID;
                XIDrop.FKiAppID = oUser.FKiApplicationID;
                var oXILayoutList = (List<XIDropDown>)XIDrop.Get_XILayoutsDDL(sConfigDatabase).oResult;
                var oXIThemesList = (List<XIDropDown>)XIDrop.Get_XIThemesDDL(sConfigDatabase).oResult;
                model.LayoutsList = oXILayoutList.ToList().Select(m => new XIDropDown { text = m.text, Value = m.Value }).ToList();
                model.ThemesList = oXIThemesList.ToList().Select(m => new XIDropDown { text = m.text, Value = m.Value }).ToList();
                return View("CreateRoles", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new { Response = false, Error = "Error while creating role. Please try again" });
            }
        }

        [HttpGet]
        public ActionResult GetUser(int userid)
        {
            return RedirectToAction("Edit", new { id = userid });
        }
        //
        // GET: /Users/Edit/1
        public async Task<ActionResult> Edit(int id)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                if (id > 0)
                {
                    XIInfraUsers xifUser = new XIInfraUsers();
                    xifUser.UserID = id;
                    var oUserData = xifUser.Get_UserDetails(sDatabase);
                    if (oUserData.bOK)
                    {
                        xifUser = (XIInfraUsers)oUserData.oResult;
                        List<string> Loc = new List<string>();
                        if (xifUser.sLocation != null)
                        {
                            List<string> LocIDS = xifUser.sLocation.Split(',').ToList();
                            foreach (var Item in LocIDS)
                            {
                                string LocID = Item;
                                Loc.Add(LocID);
                            }
                        }
                        var RoleID = xifUser.Role.RoleID;
                        var RolesTree = oUser.Role.Get_RolesTree(sDatabase, OrgID);
                        List<XIInfraRoles> GropMenus = (List<XIInfraRoles>)RolesTree.oResult;
                        List<VMDropDown> orgLocation = UsersRepository.GetOrgLocation(OrgID, iUserID, sOrgName, sDatabase);
                        var userRoles = xifUser.RoleID;
                        var Role = xifUser.Role;
                        return View(new EditUserViewModel()
                        {
                            Id = id,
                            Email = xifUser.sEmail,
                            FirstName = xifUser.sFirstName,
                            LastName = xifUser.sLastName,
                            PhoneNumber = xifUser.sPhoneNumber,
                            GroupID = Role.RoleID.ToString(),
                            RoleID = Role.RoleID,
                            Group = GropMenus,
                            Locs = Loc,
                            DropDown = orgLocation,
                            ReportTo = xifUser.iReportTo
                        });
                    }
                    else
                    {

                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

            //var res = UsersRepository.GetUser(id);
            //return View(res);

        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel editUser)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                ModelDbContext dbCore = new ModelDbContext(sDatabase);
                string LocIDs = "";
                foreach (var users in editUser.Locs)
                {
                    LocIDs = LocIDs + users + ",";
                }
                LocIDs = LocIDs.Substring(0, LocIDs.Length - 1);
                //cXIAppUsers oUser = new cXIAppUsers();
                //oUser = dbCore.XIAppUsers.Find(editUser.Id);
                //var user = await UserManager.FindByIdAsync(editUser.Id);
                //if (user == null)
                //{
                //    return HttpNotFound();
                //}
                XIInfraUsers xifUser = oUser;
                var LogggedinUserID = User.Identity.GetUserId<int>();
                xifUser.UserID = editUser.Id;
                xifUser.sUserName = editUser.Email;
                xifUser.sEmail = editUser.Email;
                xifUser.sFirstName = editUser.FirstName;
                xifUser.sLastName = editUser.LastName;
                xifUser.sPhoneNumber = editUser.PhoneNumber;
                xifUser.sLocation = LocIDs;
                xifUser.iReportTo = editUser.ReportTo;
                xifUser.CreatedBy = xifUser.UpdatedBy = iUserID;
                xifUser.UpdatedTime = DateTime.Now;
                xifUser.CreatedBySYSID = xifUser.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                xifUser.sHierarchy = xifUser.sHierarchy;
                xifUser.sInsertDefaultCode = xifUser.sInsertDefaultCode;
                xifUser.sUpdateHierarchy = xifUser.sUpdateHierarchy;
                xifUser.sViewHierarchy = xifUser.sViewHierarchy;
                xifUser.sDeleteHierarchy = xifUser.sDeleteHierarchy;
                xifUser.StatusTypeID = editUser.StatusTypeID;
                xifUser.FKiTeamID = xifUser.FKiTeamID;
                var oUserData = xifUser.Update_User(sDatabase);
                if (oUserData.xiStatus == 0 && oUserData.oResult != null)
                {
                    xifUser = (XIInfraUsers)oUserData.oResult;
                    XIInfraUserRoles xifURole = new XIInfraUserRoles();
                    xifURole.UserID = xifUser.UserID;

                    var UserRoles = xifURole.Get_UserRoles(sDatabase);// dbCore.XIAppUserRoles.Where(m => m.UserID == oUser.UserID);
                    if (UserRoles.xiStatus == 0 && UserRoles.oResult != null)
                    {
                        var oURoles = (List<XIInfraUserRoles>)UserRoles.oResult;
                        foreach (var items in oURoles)
                        {
                            xifURole.ID = items.ID;
                            var oURoleData = xifURole.Delete_UserRole(sDatabase);
                            if (oURoleData.xiStatus == 0 && oURoleData.oResult != null)
                            {

                            }
                            //else
                            //{
                            //    return Json(oURoleData, JsonRequestBehavior.AllowGet);
                            //}
                        }
                        //var Roles = editUser.GroupID.Split(',').ToList();
                        //foreach (var items in Roles)
                        //{
                        //    XIInfraRoles xifRole = new XIInfraRoles();
                        //    xifRole.RoleID = Convert.ToInt32(items);
                        //    var RoleData = xifRole.Get_RoleDefinition(sDatabase);
                        //    if (RoleData.xiStatus == 0 && RoleData.oResult != null)
                        //    {
                        //        xifRole = (XIInfraRoles)RoleData.oResult;
                        //        xifURole.UserID = xifUser.UserID;
                        //        xifURole.RoleID = xifRole.RoleID;
                        //        var URoleData = xifURole.Save_UserRole(sDatabase);
                        //        if (URoleData.xiStatus == 0 && URoleData.oResult != null)
                        //        {

                        //        }
                        //        else
                        //        {
                        //            return Json(URoleData, JsonRequestBehavior.AllowGet);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        return Json(RoleData, JsonRequestBehavior.AllowGet);
                        //    }
                        //}

                        // Naresh
                        XIInfraRoles xifRole = new XIInfraRoles();
                        xifRole.RoleID = editUser.RoleID;
                        var RoleData = xifRole.Get_RoleDefinition(sDatabase);
                        if (RoleData.xiStatus == 0 && RoleData.oResult != null)
                        {
                            xifRole = (XIInfraRoles)RoleData.oResult;
                            xifURole.UserID = xifUser.UserID;
                            xifURole.RoleID = xifRole.RoleID;
                            var URoleData = xifURole.Save_UserRole(sDatabase);
                            if (URoleData.xiStatus == 0 && URoleData.oResult != null)
                            {

                            }
                            else
                            {
                                return Json(URoleData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            return Json(RoleData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        return Json(UserRoles, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(oUserData, JsonRequestBehavior.AllowGet);
                }

                return Json(new VMCustomResponse() { Status = true, ResponseMessage = ServiceConstants.SuccessMessage, ID = editUser.Id }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse() { Status = false, ResponseMessage = ServiceConstants.ErrorMessage, ID = 0 }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetReportToUsers(string ID, int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<VMDropDown> ids = UsersRepository.GetReportToUsers(ID, UserID, sDatabase);
                return Json(ids, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult IsExistsRoleName(string sRoleName, int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
            try
            {
                return UsersRepository.IsExistsRoleName(sRoleName, RoleID, OrgID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult IsExistsEmpEmail(string Email, string Type, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return UsersRepository.IsExistsEmpEmail(Email, Type, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult DeleteRole(int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var result = UsersRepository.DeleteRole(RoleID, OrgID, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetUserProfile()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var RoleID = oUser.Role.RoleID;
                EditUserViewModel Users = new EditUserViewModel();
                Users.Id = oUser.UserID;
                Users.Email = oUser.sUserName;
                Users.FirstName = oUser.sFirstName;
                Users.LastName = oUser.sLastName;
                Users.PhoneNumber = oUser.sPhoneNumber;
                Users.PaginationCount = oUser.iPaginationCount;
                return PartialView("_UserProfile", Users);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult LoadHierarchyTree(string sBO)
        {
            CResult oCResult = new CResult();
            CResult oCR = new CResult(); // always
            long iTraceLevel = 10;

            //get iTraceLevel from ??somewhere fast - cache against user??

            oCResult.sClassName = oCResult.Get_Class(); //AUTO-DERIVE
            oCResult.sFunctionName = System.Reflection.MethodBase.GetCurrentMethod().Name;

            oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiInProcess;

            if (iTraceLevel > 0)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Started Execution" });
            }
            //if iTraceLevel>0 then 
            //oCResult.oTraceStack.Trace("Stage", "Begin query build",milliseconds)
            //oCResult.oTraceStack.Trace("Stage",sError)
            //end if

            if (oCR.xiStatus == xiEnumSystem.xiFuncResult.xiError)
            {
                oCResult.xiStatus = oCR.xiStatus;
                //oCResult.oTraceStack.Trace("Stage",sError)
            }
            string oJsonResult = string.Empty;
            try
            {
                XIInfraDynamicTreeComponent oDynamicTree = new XIInfraDynamicTreeComponent();
                List<CNV> oParams = new List<CNV>();
                oParams.Add(new CNV { sName = "bo", sValue = sBO });
                oCResult = oDynamicTree.XILoad(oParams);
                oJsonResult = new JavaScriptSerializer().Serialize(oCResult);
            }
            catch (Exception ex)
            {
                oCResult.oTraceStack.Add(new CNV { sName = "Stage", sValue = "Error While Executing Tree Structure Component" });
                oCResult.sMessage = "ERROR: " + oCResult.Get_Class() + "." + System.Reflection.MethodBase.GetCurrentMethod().Name + "] - " + ex.Message + " - Trace: " + ex.StackTrace + "\r\n";
                oCResult.xiStatus = xiEnumSystem.xiFuncResult.xiError;
                oCResult.oResult = "Failed";
                oCResult.LogToFile();
            }
            return Json(oJsonResult, JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public async Task<ActionResult> SaveUserSettings(EditUserViewModel editUser)
        //{
        //    try
        //    {
        //        var user = await UserManager.FindByIdAsync(editUser.Id);
        //        if (user == null)
        //        {
        //            return HttpNotFound();
        //        }
        //        var LogggedinUserID = User.Identity.GetUserId<int>();
        //        user.PaginationCount = editUser.PaginationCount;
        //        string Menu = editUser.LeftMenu + "," + editUser.RightMenu;
        //        user.Menu = Menu;
        //        user.InboxRefreshTime = editUser.InboxRefreshTime;
        //        var UserResult = await UserManager.UpdateAsync(user);
        //        User.AddUpdateClaim("PaginationCount", editUser.PaginationCount.ToString());
        //        return Json(editUser.Id, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        Common.SaveErrorLog(ex.ToString(), sDatabase);
        //        return Json(0, JsonRequestBehavior.AllowGet);
        //    }
        //}
       

        public string GetHierarchies(string sHierarchy)
        {
            XIIXI oXI = new XIIXI();
            if (sHierarchy.Contains('|'))
            {
                var Hierarchies = sHierarchy.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                sHierarchy = string.Empty;
                foreach (var item in Hierarchies)
                {
                    var Hierarchy = oXI.BOI("XIHierarchy_T", item);
                    sHierarchy = sHierarchy == "" ? Hierarchy.AttributeI("sHierarchy").sValue : sHierarchy + "|" + Hierarchy.AttributeI("sHierarchy").sValue;
                }
            }
            else
            {
                var Hierarchy = oXI.BOI("XIHierarchy_T", sHierarchy);
                sHierarchy = Hierarchy.AttributeI("sHierarchy").sValue;
            }
            return sHierarchy;
        }
    }
}