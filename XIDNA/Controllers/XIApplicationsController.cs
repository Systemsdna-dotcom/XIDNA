using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using XIDNA.Common;
using XICore;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using System.IO;
using XISystem;
using System.Net;
using System.Globalization;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Entity;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class XIApplicationsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IXIApplicationsRepository XIApplicationsRepository;

        public XIApplicationsController() : this(new XIApplicationsRepository()) { }

        public XIApplicationsController(IXIApplicationsRepository XIApplicationsRepository)
        {
            this.XIApplicationsRepository = XIApplicationsRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        XIInfraCache oCache = new XIInfraCache();
        CXiAPI oXIAPI = new CXiAPI();
        // GET: XIApplications
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetXIApplicationsGrid(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = XIApplicationsRepository.GetXIApplicationsGrid(param);
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

        public ActionResult AddEditXIApplication(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                cXIApplications oXIApp = new cXIApplications();
                if (ID > 0)
                {
                    oXIApp = XIApplicationsRepository.GetXIApplicationByID(ID);
                }
                return PartialView("_AddEditXIApplication", oXIApp);
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
        public ActionResult SaveXIApplication(cXIApplications oXIApp)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                var Response = XIApplicationsRepository.SaveXIApplication(oXIApp, iUserID, sOrgName, sDatabase);
                if (Response.ID > 0)
                {
                    ModelDbContext dbContext = new ModelDbContext(Response.sID);

                    //Create Login for newly created Application
                    cXIAppUsers oUser = new cXIAppUsers();
                    oUser.sUserName = oXIApp.XIAppUserName;
                    oUser.sDatabaseName = Response.sID;
                    oUser.sAppName = oXIApp.sApplicationName;
                    oUser.FKiApplicationID = Convert.ToInt32(Response.ID);
                    oUser.LockoutEndDateUtc = DateTime.Now;
                    dbContext.XIAppUsers.Add(oUser);
                    dbContext.SaveChanges();
                    var EncryptedPwd = oXIAPI.EncryptData(oXIApp.XIAppPassword, true, oUser.UserID.ToString());
                    oUser = dbContext.XIAppUsers.Find(oUser.UserID);
                    oUser.sPasswordHash = EncryptedPwd;
                    dbContext.SaveChanges();

                    //Create Role for newly created Application
                    cXIAppRoles Role = new cXIAppRoles();
                    Role.iParentID = 2;
                    Role.sRoleName = "SuperAdmin";
                    Role.FKiOrganizationID = iOrgID;
                    dbContext.XIAppRoles.Add(Role);
                    dbContext.SaveChanges();

                    //Assign Role to User
                    cXIAppUserRoles oURole = new cXIAppUserRoles();
                    oURole.UserID = oUser.UserID;
                    oURole.RoleID = Role.RoleID;
                    dbContext.XIAppUserRoles.Add(oURole);
                    dbContext.SaveChanges();
                }
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse() { ID = 0, Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IDESaveXIApplication(XIDApplication oXIApp)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                int iApplicationID = 0;
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                XID1Click oXID = new XID1Click();
                var iRoleID = "0";
                int iCoreDBSoruceID = 0;
                var oAPPI = new XIIBO();
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                var oCR = oConfig.Save_Application(oXIApp);
                if (oXIApp.ID == 0)
                {
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oAPPI = (XIIBO)oCR.oResult;
                        var sApplictionID = oAPPI.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                        var sThemeID = oAPPI.Attributes.Where(m => m.Key.ToLower() == "stheme").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                        int.TryParse(sApplictionID, out iApplicationID);
                        var sScriptsList = oAPPI.BOD.sScripts.ToList().Where(m => m.IsSuccess == false).ToList();
                        if (sScriptsList.Count() == 0 && iApplicationID > 0)
                        {
                            XIIBO oBO = new XIIBO();
                            oConfig.iAppID = iApplicationID;
                            oConfig.iUserID = iUserID;
                            oConfig.sAppName = oXIApp.sApplicationName;
                            List<CNV> oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = XIConstant.DB_Type, sValue = XIConstant.DB_Core });
                            oCR = oConfig.Save_DataSource(oParams);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oDS = (XIIBO)oCR.oResult;
                                var iDataSourceID = oDS.Attributes.Where(m => m.Key.ToLower() == "xiguid").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                var DataSourceXIGUID = Guid.Empty;
                                Guid.TryParse(iDataSourceID, out DataSourceXIGUID);
                                oConfig.CoreDataSourceXIGUID = DataSourceXIGUID;
                                iCoreDBSoruceID = Convert.ToInt32(iDataSourceID);
                            }
                            else
                            {

                            }
                            //Inserting Shared Datasource in XIDataSource_XID_T Table
                            oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = XIConstant.DB_Type, sValue = XIConstant.DB_Shared });
                            oCR = oConfig.Save_DataSource(oParams);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oDS = (XIIBO)oCR.oResult;
                                var iDataSourceID = oDS.Attributes.Where(m => m.Key.ToLower() == "xiguid").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                var DataSourceXIGUID = Guid.Empty;
                                Guid.TryParse(iDataSourceID, out DataSourceXIGUID);
                                oConfig.SharedDataSourceXIGUID = DataSourceXIGUID;
                            }
                            else
                            {

                            }
                            oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = XIConstant.DB_Type, sValue = XIConstant.DB_Core });
                            oParams.Add(new CNV { sName = XIConstant.Param_ApplicationName, sValue = oXIApp.sApplicationName });
                            oCR = oConfig.Create_Database(oParams);
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                            else
                            {

                            }
                            oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = XIConstant.DB_Type, sValue = XIConstant.DB_Shared });
                            oParams.Add(new CNV { sName = XIConstant.Param_ApplicationName, sValue = oXIApp.sApplicationName });
                            oCR = oConfig.Create_Database(oParams);
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                            else
                            {

                            }
                            oCR = oConfig.Create_CoreDBTables();
                            if (oCR.bOK && oCR.oResult != null)
                            {

                            }
                            else
                            {

                            }
                            //var sAppUsersSharedTBCreation = oConfig.CreateShareTable(oXIApp.sApplicationName + "_Shared"); //Shared Database Creation
                            //Create Login for Application User
                            oCR = oConfig.Save_ApplicationUser(oXIApp, null);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oRoleDef = (XIIBO)oCR.oResult;
                                iRoleID = oRoleDef.Attributes.Where(m => m.Key.ToLower() == "roleid").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                            }
                            //Create Layout for Application User Login
                            oParams = new List<CNV>();
                            oParams.Add(new CNV { sName = XIConstant.Param_Layout, sValue = XIConstant.Auto3LayoutHTML });
                            XIDLayout oLayout = new XIDLayout();
                            oLayout.LayoutName = "AppDefaultLayout";
                            oLayout.LayoutType = "Inline";
                            oLayout.LayoutCode = XIConstant.Auto3LayoutHTML;
                            oLayout.FKiApplicationID = iApplicationID;
                            oLayout.Authentication = "Authenticated";
                            oLayout.LayoutLevel = "OrganisationLevel";
                            oLayout.StatusTypeID = 10;
                            oCR = oConfig.Save_ApplicationDefaultLayout(oLayout, "");
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oLayoutDef = (XIIBO)oCR.oResult;
                                var iLayoutID = oLayoutDef.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                XIIBO oBOI1 = new XIIBO();
                                XIDBO oBOD1 = new XIDBO();
                                oBOD1 = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XIAppRoles_AR_T");
                                oBOI1.BOD = oBOD1;
                                oBOD1.iDataSourceXIGUID = oConfig.CoreDataSourceXIGUID;
                                oBOI1.SetAttribute("RoleID", iRoleID.ToString());
                                oBOI1.SetAttribute("iLayoutID", iLayoutID.ToString());
                                oBOI1.SetAttribute("iThemeID", sThemeID);
                                oCR = oBOI1.Save(oBOI1);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    //oConfig.AutoMenuCreationForApplication();
                                }
                                else
                                {

                                }
                            }
                            XIMenu oMenu = new XIMenu();
                            oMenu.FKiApplicationID = iApplicationID;
                            oMenu.Name = oXIApp.sApplicationName;
                            oMenu.RootName = oXIApp.sApplicationName;
                            oMenu.ParentID = "#";
                            oMenu.ActionType = 30;
                            oMenu.XiLinkID = 0;
                            oCR = oConfig.Save_Menu(oMenu);
                            if (oCR.bOK && oCR.oResult != null)
                            {
                                var oMenuDef = (XIIBO)oCR.oResult;
                                var iParentID = oMenuDef.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();

                                //Create Reference Data Menu
                                oParams = new List<CNV>();
                                oParams.Add(new CNV { sName = "LayoutName", sValue = oXIApp.sApplicationName + " Reference Data Layout" });
                                oParams.Add(new CNV { sName = "DialogName", sValue = oXIApp.sApplicationName + " Reference Data Dialog" });
                                oParams.Add(new CNV { sName = "XilinkName", sValue = oXIApp.sApplicationName + " Reference Data Xilink" });
                                oParams.Add(new CNV { sName = "sParentID", sValue = iParentID });
                                oParams.Add(new CNV { sName = "sApplicationName", sValue = oXIApp.sApplicationName });
                                oParams.Add(new CNV { sName = "IsRowClick", sValue = "0" });
                                oParams.Add(new CNV { sName = XIConstant.Param_ApplicationID, sValue = iApplicationID.ToString() });
                                oCR = oConfig.Save_ReferenceData(oParams);
                                if (oCR.bOK && oCR.oResult != null)
                                {
                                    var o1ClickDef = (XIIBO)oCR.oResult;
                                    var i1ClickID = o1ClickDef.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                                    //Create Reference Data RowClick
                                    oParams = new List<CNV>();
                                    oParams.Add(new CNV { sName = "LayoutName", sValue = oXIApp.sApplicationName + " Reference Data RowClick Layout" });
                                    oParams.Add(new CNV { sName = "DialogName", sValue = oXIApp.sApplicationName + " Reference Data RowClick Dialog" });
                                    oParams.Add(new CNV { sName = "XilinkName", sValue = oXIApp.sApplicationName + " Reference Data RowClick Xilink" });
                                    oParams.Add(new CNV { sName = "IsRowClick", sValue = "1" });
                                    oParams.Add(new CNV { sName = "i1ClickID", sValue = i1ClickID.ToString() });
                                    oParams.Add(new CNV { sName = XIConstant.Param_ApplicationID, sValue = iApplicationID.ToString() });
                                    oCR = oConfig.Save_ReferenceData(oParams);
                                }
                            }
                        }
                    }
                }
                else
                {
                    iApplicationID = oXIApp.ID;
                    if (oCR.bOK && oCR.oResult != null)
                    {
                        oAPPI = (XIIBO)oCR.oResult;
                    }
                }
                return Json(oAPPI, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse() { ID = 0, Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveXIAppLogo(HttpPostedFileBase UploadImage, int XIAppID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (UploadImage != null)
                {
                    var oXIApp = XIApplicationsRepository.GetXIApplicationByID(XIAppID);
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images";
                    UploadImage.SaveAs(str + "\\" + oXIApp.sApplicationName + "Logo_" + oXIApp.ID + ".png");
                    oXIApp.sLogo = oXIApp.sApplicationName + "Logo_" + oXIApp.ID + ".png";
                    var res = XIApplicationsRepository.SaveXIAppLogo(oXIApp);
                    return Content(res.ID.ToString());
                }
                else
                {
                    return Content("0");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Content("0");
            }
        }

        [HttpPost]
        public JsonResult ValidateUser(string Uname)
        {
            var data = "False";
            //var db = new PetaPoco.Database("cnstr");
            //var tempUser = new UserRepository().GetUsers();
            //Users usr = (from u in tempUser where u.UserName.Equals(Uname) select u).FirstOrDefault();
            //if (usr != null)
            //{
            //    data = "True";
            //}
            //else
            //{
            //    data = "False";
            //}
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #region XIUrlMappings

        public ActionResult XIUrlMapping()
        {
            return View();
        }
        public ActionResult GetXIUrlMappingGrid(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = XIApplicationsRepository.GetXIUrlMappingGrid(param, iUserID, sOrgName, sDatabase);
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
        public ActionResult AddEditXIUrlMapping(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                cXIUrlMappings oXIUrl = new cXIUrlMappings();
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                if (ID > 0)
                {
                    oXIUrl = XIApplicationsRepository.GetXIUrlMappingByID(ID);
                }
                if (fkiApplicationID == 0)
                {
                    oXIUrl.ddlApplications = Common.GetApplicationsDDL();
                }
                oXIUrl.FKiApplicationID = fkiApplicationID;
                oXIUrl.SourceList = Common.GetSourceDDL(iUserID, sOrgName, sDatabase);
                return PartialView("_AddEditXIUrlMapping", oXIUrl);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveXIUrlMapping(cXIUrlMappings oXIUrl)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                oXIUrl.CreatedBy = oXIUrl.UpdatedBy = iUserID; oXIUrl.OrganisationID = OrgID;
                var Response = XIApplicationsRepository.SaveXIUrlMappingDetails(oXIUrl, iUserID, sOrgName, sDatabase);
                return Json(Response, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Save_Url
        [HttpPost]
        public ActionResult Save_Url(XIURLMappings oXIUrl)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                var iUrlID = "";
                int iUserID = SessionManager.UserID;
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                var Response = oConfig.Save_URLMapping(oXIUrl);
                if (Response.bOK && Response.oResult != null)
                {
                    var UrlDef = (XIIBO)Response.oResult;
                    iUrlID = UrlDef.Attributes.Where(m => m.Key.ToLower() == "id").Select(m => m.Value).Select(m => m.sValue).FirstOrDefault();
                }
                return Json(XIConstant.Success, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult IsExistsUrlMappingName(string sUrlName, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return XIApplicationsRepository.IsExistsUrlMappingName(sUrlName, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion XIUrlMappings

        #region CACHE_CONFIG
        public ActionResult ManageCache(string sType)
        {
            try
            {
                var Model = CacheList(EnumCacheTypes.None);
                ViewBag.sType = sType;
                return View(Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public ActionResult CachePartial(EnumCacheTypes cachetype)
        {
            try
            {
                var Model = CacheList(cachetype);
                return PartialView("_CacheList", Model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult RemoveCacheByKey(string skey, EnumCacheTypes cachetype)
        {
            try
            {
                if (!string.IsNullOrEmpty(skey))
                {
                    if (cachetype == EnumCacheTypes.Application)
                    {
                        //if (HttpRuntime.Cache[skey] != null)
                        //{
                        //    HttpRuntime.Cache.Remove(skey);
                        //}
                        XIInfraCache.RemoveCacheWithKey(skey);
                    }
                    else if (cachetype == EnumCacheTypes.User)
                    {
                        // Remove from cache User Level
                        var oUserCache = HttpRuntime.Cache["XICache"];
                        var oCacheobj = (XICacheInstance)oUserCache;
                        if (oCacheobj != null)
                            oCacheobj.RemoveUserCache(skey);
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /* NOTE: Cache List*/
        [NonAction]
        public VM_CacheData CacheList(EnumCacheTypes cachetype)
        {
            string sDatabase = SessionManager.CoreDatabase;
            var AppName = SessionManager.AppName;
            try
            {

                VM_CacheData result = new VM_CacheData();
                List<VM_CacheConfig> Model = new List<VM_CacheConfig>();
                if (cachetype == EnumCacheTypes.None || cachetype == EnumCacheTypes.Application)
                {
                    string sSystemdefined = AppName +"_Definition_";
                    // get cache Application level and User level  here
                    var cacheobj = HttpRuntime.Cache.GetEnumerator();
                    //long bytes = 0;
                    while (cacheobj.MoveNext())
                    {
                        var pair = (DictionaryEntry)cacheobj.Current;
                        if (pair.Key.ToString().Contains(sSystemdefined))
                        {
                            using (Stream s = new MemoryStream())
                            {
                                string json = JsonConvert.SerializeObject(pair.Value, Newtonsoft.Json.Formatting.Indented);
                                BinaryFormatter formatter = new BinaryFormatter();
                                formatter.Serialize(s, json);
                                Model.Add(new VM_CacheConfig { sKey = pair.Key.ToString(), Size = (s.Length / 1024), CacheType = EnumCacheTypes.Application });
                            }
                        }
                    }
                }
                if (cachetype == EnumCacheTypes.None || cachetype == EnumCacheTypes.User)
                {
                    // User level Cache here
                    var oUserCache = HttpRuntime.Cache["XICache"];
                    var oCacheobj = (XICacheInstance)oUserCache;
                    if (oCacheobj != null)
                    {
                        foreach (KeyValuePair<string, XICacheInstance> item in oCacheobj.NMyInstance)
                        {
                            // XiSessionsLevels
                            if (item.Key.Contains("XISession"))
                            {
                                foreach (var sessionids in item.Value.NMyInstance)
                                {
                                    using (Stream s = new MemoryStream())
                                    {
                                        string json = JsonConvert.SerializeObject(sessionids.Value, Newtonsoft.Json.Formatting.Indented);
                                        BinaryFormatter formatter = new BinaryFormatter();
                                        formatter.Serialize(s, json);
                                        var userinfocache = HttpRuntime.Cache["S" + sessionids.Key.ToString()];
                                        if (userinfocache != null)
                                        {

                                        }
                                        var requsercacheddata = (VM_UserLoginCache)userinfocache;
                                        requsercacheddata = requsercacheddata ?? new VM_UserLoginCache();
                                        Model.Add(new VM_CacheConfig { sRole = requsercacheddata.sRole, sUserName = requsercacheddata.sUserName, sKey = sessionids.Key, Size = (s.Length / 1024), CacheType = EnumCacheTypes.User });
                                    }
                                }
                            }
                        }
                    }
                }
                result.CacheList = Model.OrderByDescending(s => s.Size).ToList();
                result.Cachefilteredtype = cachetype;
                return result;
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                throw ex;
            }
        }
        #endregion

    }
}