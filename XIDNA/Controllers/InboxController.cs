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
using System.Data;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.UI;
using NPOI.HSSF.Model; // InternalWorkbook
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel; // HSSFWorkbook, HSSFSheet
using XIDNA.Common;
using XICore;
using XISystem;
using System.Data.SqlClient;
using System.Configuration;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class InboxController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IInboxRepository InboxRepository;

        public InboxController() : this(new InboxRepository()) { }

        public InboxController(InboxRepository InboxRepository)
        {
            this.InboxRepository = InboxRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        public ModelDbContext dbcontext = new ModelDbContext();
        //
        // GET: /Inbox/
        public ActionResult Index()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                if (OrgID != 0)
                {
                    ViewBag.orgid = OrgID;
                    return View("_RolesList", "~/Views/Shared/_XIDynawareLayout.cshtml", OrgID);// Using same View for OrgLogin but with a Layout
                }
                ViewBag.orgid = 0;
                List<VMDropDown> organizations = InboxRepository.GetOrganizations(sDatabase);
                return View(organizations);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetUsersList(jQueryDataTableParamModel param, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = InboxRepository.GetUsersList(param, ID, sDatabase);
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
        public ActionResult GetRolesByOrganization(int OrgID)
        {
            return PartialView("_RolesList", OrgID);
        }
        public ActionResult UserDetails(int RoleID, string Rolename)
        {
            return RedirectToAction("Usercontent", new { RoleID = RoleID, Rolename = Rolename });
        }
        public ActionResult Usercontent(int RoleID, string Rolename)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMUserReports user = new VMUserReports();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var FKiAppID = oUser.FKiApplicationID;
                user = InboxRepository.GetAssignDropdowns(OrgID, iUserID, sOrgName, sDatabase);
                //user.Reports = model;
                user.RoleID = RoleID;
                user.UserName = Rolename;
                if (FKiAppID == 0)
                {
                    user.ddlApplications = Common.GetApplicationsDDL();
                }
                //user.RoleID = User.Identity.GetGroupID();
                user.FKiApplicationID = FKiAppID;
                user.AllBOs = Common.GetBOsDDL(sDatabase);
                user.AllXiLinks = Common.GetXiLinksDDL(sDatabase);
                user.AllOneClicks = Common.GetReportsDDL(sDatabase);
                return PartialView("_UserContentPopUP", user);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditUserQuery(int ID, int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMUserReports model = InboxRepository.EditUserQuery(ID, OrgID, iUserID, sOrgName, sDatabase);
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var FKiAppID = oUser.FKiApplicationID;
                //model.MenuList = InboxRepository.GetAllMenuTitles();
                model.SelectedReports = InboxRepository.GetQueriesByType(RoleID, model.TypeID, model.DisplayAs, sDatabase, model.ClassID, OrgID);
                if (FKiAppID == 0)
                {
                    model.ddlApplications = Common.GetApplicationsDDL();
                }
                model.FKiApplicationID = FKiAppID;
                model.AllBOs = Common.GetBOsDDL(sDatabase);
                model.AllXiLinks = Common.GetXiLinksDDL(sDatabase);
                model.AllOneClicks = Common.GetReportsDDL(sDatabase);
                return PartialView("_EditUserQuery", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveUserQueries(VMUserReports model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Icon = Request["Icon"].ToString();
                if (Icon.Length > 0 && model.Icon == null)
                {
                    Icon = Icon.Substring(1, Icon.Length - 1);
                    model.Icon = Icon;
                }
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                //int id = InboxRepository.SaveUserQueries(model, sDatabase, OrgID);
                model.CreatedByName = model.ModifiedByName = User.Identity.Name;
                int id = InboxRepository.Save1Click(model, sDatabase, OrgID, iUserID);
                if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString()) && model.bSignalR == true)
                {
                    //XiSignalRController SignalRNewOneClick = new XiSignalRController();
                    //SignalRNewOneClick.NewSignalROneClick(model.ReportID, model.RoleID);
                }
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetUserQueries(int UserID = 0)
        {
            return PartialView("_UserQueriesList", UserID);
        }
        public ActionResult UserQueriesList(jQueryDataTableParamModel param, int RoleID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = InboxRepository.UserQueriesList(param, RoleID, iUserID, sOrgName, sDatabase);
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
        public ActionResult GetQueriesByID(int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMReports> model = InboxRepository.GetQueriesByID(UserID, OrgID, sOrgName, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetLeftMenu(int UserID = 0, int Count = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                //List<Reports> model = InboxRepository.GetLeftMenuTree(iUserID, OrgID, sOrgName, sDatabase);
                List<RightMenuTrees> model = InboxRepository.GetLeftMenuTrees(iUserID, OrgID, sOrgName, sDatabase);
                if (Count == 0)
                {
                    return PartialView("_HomeLeftMenu", model);
                }
                else
                {
                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetRightMenu()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMReports> model = InboxRepository.GetQueriesByID(iUserID, OrgID, sOrgName, sDatabase);
                return PartialView("_HomeRightMenu", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetHomeHeader()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                if (oUser.Role.sRoleName.Contains(EnumRoles.XISuperAdmin.ToString()))
                {
                    List<VMReports> Data = new List<VMReports>();
                    ViewBag.UserName = oUser.sUserName;
                    ViewBag.Role = oUser.Role.sRoleName;
                    return PartialView("_HomeHeader", Data);
                }
                List<VMReports> model = InboxRepository.GetQueriesByID(iUserID, OrgID, sDatabase, sOrgName);
                ViewBag.UserName = oUser.sUserName;
                ViewBag.Role = oUser.Role.sRoleName;
                XIInfraCache oCache = new XIInfraCache();
                XIDStructure oXIDStructure = new XIDStructure();
                XID1Click o1ClickD = new XID1Click();
                o1ClickD = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, "A66BA243-50F0-4D6D-A1D1-1B6251773729");
                var o1ClickC = (XID1Click)o1ClickD.Clone(o1ClickD);
                if (o1ClickC.Query != null)
                {                   
                    List<CNV> nParms = new List<CNV>();
                    nParms.Add(new CNV { sName = "{XIP|iUserID}", sValue = SessionManager.UserID.ToString() });
                    o1ClickC.ReplaceFKExpressions(nParms);
                    o1ClickC.Query = oXIDStructure.ReplaceExpressionWithCacheValue(o1ClickC.Query, nParms);
                    var oMessages = o1ClickC.OneClick_Execute();
                    if (oMessages != null && oMessages.Count() > 0)
                    {
                        var projectlist = oMessages.Values
                         .Where(msg => msg.Attributes.ContainsKey("fkiprojectid"))
                         .Select(msg => msg.Attributes["fkiprojectid"].sValue).ToList();
                        SessionManager.ProjectID = string.Join(",", projectlist);
                    }
                }
                return PartialView("_HomeHeader", model);

        }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetMyReports()
        {
            return View("_ViewMyReports");
        }

        public ActionResult ViewReportsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = InboxRepository.ViewReportsList(param, iUserID, sOrgName, sDatabase);
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

        public ActionResult GetUserQueryList()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                List<VMUserReports> model = InboxRepository.GetUserQueryList(iUserID, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #region QueryResults
        [HttpPost]
        public ActionResult RunUserQuery(VMRunUserQuery QValues)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                //VMRunUserQuery UserQuery = new VMRunUserQuery();
                QValues.OrgID = OrgID;
                QValues.UserID = iUserID;
                QValues.database = sDatabase;
                QValues.LeadID = 0;
                QValues.ClassFilter = 0;
                QValues.DateFilter = 0;
                VMResultList model = InboxRepository.RunUserQuery(QValues, sDatabase, sOrgName);
                ViewBag.queryid = QValues.ReportID;
                var AutoLoadResultLlist = model.Rows;
                if (QValues.PageIndex >= 2 || QValues.SearchType == "FilterSearch" || QValues.SearchType == "NaturalSearch" || QValues.SearchType == "Quick")
                {
                    return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                }
                return View(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ReportResult(string QueryID, int PageIndex, string ResultIn, string SearchText, string SearchType, string BO, int BaseID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {

                int iQueryID = 0;
                Guid QueryIDGUID = Guid.Empty;
                int.TryParse(QueryID, out iQueryID);
                Guid.TryParse(QueryID, out QueryIDGUID);
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var LoadingType = InboxRepository.GetLoadingType(QueryID, sDatabase);
                if (LoadingType[1] == 1 && SearchType != "Structured")
                {
                    var Result = InboxRepository.GetStructuredOneClicks(OrgID, QueryID, sDatabase);
                    if (ResultIn == "Run")
                    {
                        Result.FirstOrDefault().ResultIn = ResultIn;
                    }
                    //return null;
                    return PartialView("_StructuredOneClicksTree", Result);
                }
                else
                {
                    if (LoadingType[0] == 1)
                    {
                        var Report = InboxRepository.GetHeadings(QueryID, sDatabase, OrgID, iUserID, sOrgName);
                        if (iQueryID > 0) { Report.ReportID = iQueryID; }
                        else
                        {
                            Report.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        //Report.QueryID = QueryID;
                        Report.BaseReportID = BaseID;
                        Report.PreviewType = ResultIn;
                        Report.SearchText = SearchText;
                        if (SearchType == "Quick")
                        {
                            Report.SearchType = SearchType;
                        }
                        else if (SearchType == "Structured")
                        {
                            Report.PreviewType = "Structured";
                        }
                        //else
                        //{
                        //    Report.SearchType = SearchType;
                        //}
                        //Report.BO = BO;
                        return View(Report);
                        //return Report;
                    }
                    else
                    {
                        int? LeadID = 0;
                        VMRunUserQuery UserQuery = new VMRunUserQuery();
                        UserQuery.OrgID = OrgID;
                        UserQuery.UserID = iUserID;
                        UserQuery.database = sDatabase;
                        UserQuery.PageIndex = PageIndex;
                        if (iQueryID > 0)
                        {
                            UserQuery.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: ReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            UserQuery.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        UserQuery.LeadID = 0;
                        UserQuery.ClassFilter = 0;
                        UserQuery.DateFilter = 0;
                        UserQuery.SearchText = SearchText;
                        UserQuery.BO = BO;
                        UserQuery.SearchType = SearchType;
                        VMResultList model = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                        if (iQueryID > 0)
                        {
                            model.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: ReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            model.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        var AutoLoadResultLlist = model.Rows;
                        if (SearchType == "Structured")
                        {
                            model.PreviewType = "Structured";
                        }
                        else
                        {
                            model.PreviewType = ResultIn;
                        }
                        // model.QueryID = QueryID;
                        model.SearchText = SearchText;
                        model.BO = BO;
                        model.SearchType = SearchType;
                        if (PageIndex >= 2)
                        {
                            return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                        }
                        return View(model);
                        //return model;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }


            //try
            //{
            //    oUser.UserID = iUserID;oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;int OrgID = oUser.FKiOrganisationID;
            //    int UserID = Util.GetUserID();
            //    string sDatabase = SessionManager.CoreDatabase;
            //    int? LeadID = 0;
            //    VMResultList model = InboxRepository.RunUserQuery(QueryID, PageIndex, UserID, sDatabase, Orgid, LeadID, 0, 0);
            //    ViewBag.queryid = QueryID;
            //    var AutoLoadResultLlist = model.Rows;
            //    if (PageIndex >= 2)
            //    {
            //        return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
            //    }
            //    return View(model);
            //}
            //catch (Exception ex)
            //{
            //    logger.Error(ex);
            //    return null;
            //}
        }

        public ActionResult QueryDynamicForm(string OneClickID, string BOID, string sGUID, string sTabCols)//
        {
            ModelDbContext dbContext = new ModelDbContext();
            XIInfraCache oCache = new XIInfraCache();
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIDBO oBODef = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, null, BOID.ToString());
                XID1Click OneClick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClickID.ToString()); //oXID.Get_1ClickDefinition(null, OneClickID.ToString()).oResult;
                //XIDBO oBOD = (XIDBO)oXID.Get_BODefinition("", BOID.ToString()).oResult;
                if (OneClick.bIsXICreatedBy == true || OneClick.bIsXIUpdatedBy == true)
                {
                    if (OneClick.bIsXICreatedBy == true)
                    {
                        var XICreatedBy = "[" + oBODef.TableName + "]" + ".XICreatedBy";
                        if (!(OneClick.SearchFields.Contains(XICreatedBy)))
                        {
                            OneClick.SearchFields = OneClick.SearchFields + ", " + XICreatedBy;
                        }
                    }
                    if (OneClick.bIsXIUpdatedBy == true)
                    {
                        var XIUpdatedBy = "[" + oBODef.TableName + "]" + ".XIUpdatedBy";
                        if (!(OneClick.SearchFields.Contains(XIUpdatedBy)))
                        {
                            OneClick.SearchFields = OneClick.SearchFields + ", " + XIUpdatedBy;
                        }
                    }
                }
                var BOList = new List<string>();
                if (!string.IsNullOrEmpty(OneClick.SearchFields))
                {
                    var sField = OneClick.SearchFields.Split(new string[] { ", " }, StringSplitOptions.None);
                    if (OneClick.bIsMultiSearch == true)
                    {
                        foreach (var item in sField)
                        {
                            var bo = item.Split('.');
                            if (bo.Count() > 0)
                            {
                                var boname = bo[0];
                                boname = boname.Substring(1, boname.Length - 2);
                                if (!BOList.Any(s => s.Contains(boname)))
                                {
                                    BOList.Add(boname);
                                }
                            }
                        }
                    }
                }
                if (OneClick.bIsMultiSearch == true)
                {
                    OneClick.SubAttributes = new List<XID1Click>();
                    foreach (var single in BOList)
                    {
                        var sBOName = dbContext.BOs.Where(m => m.TableName == single).Select(m => m.Name).FirstOrDefault();
                        XIDBO oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO_All, "" + sBOName + "", null);
                        //OneClick.BOD = oBOD;
                        XID1Click oXD = new XID1Click();
                        oXD.BOD = oBOD;
                        OneClick.SubAttributes.Add(oXD);
                        //oXD.SubAttributes.ToList().ForEach(m => m.BOD = oBOD);
                    }
                }
                else
                {
                    OneClick.BOD = oBODef;
                    XIInfraUsers oXIInfra = new XIInfraUsers();
                    oXIInfra.sDatabaseName = sDatabase;
                    var oUsers = new List<XIDropDown>();
                    if (OneClick.bIsXICreatedBy == true)
                    {
                        if (OneClick.FKiCrtd1ClickID > 0)
                        {
                            XID1Click CrtdOneClick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClick.FKiCrtd1ClickID.ToString());
                            oXIInfra.sQuery = CrtdOneClick.Query;
                            oUsers = oXIInfra.GetUserDetails();
                            OneClick.CrtdFieldDDL = oUsers;
                        }
                    }
                    if (OneClick.bIsXIUpdatedBy == true)
                    {
                        if (OneClick.FKiUpdtd1ClickID > 0)
                        {
                            XID1Click UpdtdOneClick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, OneClick.FKiUpdtd1ClickID.ToString());
                            oXIInfra.sQuery = UpdtdOneClick.Query;
                            oUsers = oXIInfra.GetUserDetails();
                            OneClick.UpdtdFieldDDL = oUsers;
                        }
                    }
                    //(XIDBO)oXID.Get_BODefinitionALL("", BOID.ToString()).oResult;
                }
                OneClick.sGUID = sGUID;
                if (!string.IsNullOrEmpty(sTabCols))
                {
                    if (sTabCols.Contains("CONCAT"))
                    {
                        OneClick.TableColumns = sTabCols.Replace("CONCAT(", "").Replace(",'/'", "").Replace(")", "").Split(',').ToList();
                    }
                    else
                    {
                        OneClick.TableColumns = sTabCols.Split(',').ToList();
                    }
                }
                return PartialView("QueryDynamicForm", OneClick);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetOneClickResult(string ReportID, string Query, string PreviewType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Clicks = InboxRepository.GetAllOneClicks(ReportID, Query, sDatabase);
                if (Clicks.ClickType == "MultiClicks")
                {
                    Clicks.PreviewType = PreviewType;
                    return View("OneClickResult", Clicks);
                }
                else
                {
                    object[] Val = new object[1];
                    Val[0] = ReportID;
                    Clicks.Clicks.Add(Val);
                    return View("OneClickResult", Clicks);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult RunInForm(int ID, string Query, int DisplayAs, int BOID, string Fields, string BOName)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (BOName == EnumLeadTables.Reports.ToString())
                {
                    return RedirectToAction("GetOneClickResult", new { ReportID = ID, Query = Query, PreviewType = "ERun" });
                }
                else
                {
                    if (DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString()))
                    {
                        return RedirectToAction("GetPieChart", "XiLink", new { ReportID = ID, ResultIn = "Run", Query = Query, ClassValue = 0, DateValue = 0 });
                    }
                    else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.BarChart.ToString()))
                    {
                        return RedirectToAction("GetBarChart", "XiLink", new { ReportID = ID, ResultIn = "Run", Query = Query, ClassValue = 0, DateValue = 0 });
                    }
                    else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.LineChart.ToString()))
                    {
                        return RedirectToAction("GetLineGraph", "XiLink", new { ReportID = ID, ResultIn = "Run", Query = Query, ClassValue = 0, DateValue = 0 });
                    }
                    else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ResultList.ToString()))
                    {
                        return RedirectToAction("GetPreviewInForm", "QueryGeneration", new { ID = ID, Query = Query, Fields = Fields, BOID = BOID, DisplayAs = "Result List", ResultIn = "Run" });
                    }
                    else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Summary.ToString()))
                    {
                        return RedirectToAction("Index", "Reports", new { ReportID = ID, ResultIn = "Run", Query = Query, });
                    }
                    else
                    {
                        return null;
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
        }

        public ActionResult InnerReportResult(string SearchText, string ReportID, int PageIndex, int? ParentID, bool? IsParent, string ReportColumns, int? BaseID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                if (ParentID == 0 && IsParent == false || ParentID > 0 && IsParent == false || ParentID == null)
                {
                    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                    var LoadingType = InboxRepository.GetLoadingType(ReportID, sDatabase);
                    //PageIndex = 1;
                    oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                    int iQueryID = 0;
                    Guid QueryIDGUID = Guid.Empty;
                    int.TryParse(ReportID, out iQueryID);
                    Guid.TryParse(ReportID, out QueryIDGUID);
                    if (LoadingType[0] == 1)
                    {
                        VMQuickSearch Search = new VMQuickSearch();
                        if (iQueryID > 0)
                        {
                            Search.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: InnerReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            Search.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        Search.UserID = iUserID;
                        Search.OrgID = OrgID;
                        Search.database = sDatabase;
                        Search.PageIndex = PageIndex;
                        Search.SearchText = SearchText;
                        var Result = InboxRepository.GetHeadings(ReportID, sDatabase, OrgID, iUserID, sOrgName);
                        VMResultList model = new VMResultList();
                        model.Headings = Result.Headings;
                        if (iQueryID > 0)
                        {
                            model.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: InnerReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            model.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        model.SearchText = SearchText;
                        model.ResultListDisplayType = Result.ResultListDisplayType;
                        model.ReportColumns = ReportColumns;
                        model.BaseReportID = BaseID;
                        model.IsPopup = Result.IsPopup;
                        model.QueryID = Result.ActionReportID;
                        model.ActionType = Result.ActionType;
                        model.QueryName = Result.QueryName;
                        model.IsQueryExists = Result.IsQueryExists;
                        return PartialView("_InnerReportPagination", model);
                    }
                    else
                    {
                        VMRunUserQuery UserQuery = new VMRunUserQuery();
                        UserQuery.OrgID = OrgID;
                        UserQuery.UserID = iUserID;
                        UserQuery.database = sDatabase;
                        UserQuery.PageIndex = PageIndex;
                        if (iQueryID > 0)
                        {
                            UserQuery.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: InnerReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            UserQuery.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        UserQuery.LeadID = 0;
                        UserQuery.ClassFilter = 0;
                        UserQuery.DateFilter = 0;
                        UserQuery.SearchText = SearchText;
                        UserQuery.SearchType = null;
                        VMResultList model = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                        if (iQueryID > 0)
                        {
                            ViewBag.queryid = iQueryID;
                            model.ReportID = iQueryID;
                            Common.SaveErrorLog("Error at InboxController: InnerReportResult: " + iQueryID, "");
                        }
                        if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                        {
                            ViewBag.queryid = QueryIDGUID;
                            model.FKi1ClickIDXIGUID = QueryIDGUID;
                        }
                        var AutoLoadResultLlist = model.Rows;
                        if (PageIndex >= 2)
                        {
                            return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                        }
                        return PartialView("_InnerReportAutoLoad", model);
                    }
                }
                else if (ParentID == 0 && IsParent == true || ParentID > 0 && IsParent == true)
                {
                    int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                    oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                    var Result = InboxRepository.GetStructuredOneClicks(OrgID, ReportID, sDatabase);
                    return PartialView("_StructuredOneClicksTree", Result);
                    //return RedirectToAction("StructuredOneClicksTree", "QueryGeneration", new { ParentID = ParentID, ReportID = ReportID });
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
        [AllowAnonymous]
        public ActionResult GetReportResult(jQueryDataTableParamModel param, int ReportID, string SearchText, string SearchType, string ShowType, string ReportColumns, string BO, int? BaseID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                string sCurrentGuestUser = null;
                if (SessionManager.UserUniqueID != null)
                {
                    sCurrentGuestUser = SessionManager.UserUniqueID;
                }
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = "desc"; //Request["sSortDir_0"].ToString();
                VMQuickSearch Search = new VMQuickSearch();
                Search.ReportID = ReportID;
                Search.UserID = iUserID;
                Search.OrgID = OrgID;
                Search.database = sDatabase;
                Search.SearchText = SearchText;
                Search.SearchType = SearchType;
                Search.ReportColumns = ReportColumns;
                Search.BaseID = BaseID;
                Search.Role = oUser.Role.sRoleName;
                Search.ShowType = ShowType;
                Search.BO = BO;
                var result = InboxRepository.GetReportResult(param, Search, sDatabase, iUserID, sCurrentGuestUser, sOrgName);
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

        #endregion QueryResults

        //[HttpPost]
        //public ActionResult DashboardGlobalFilter(int dropdownValue = 0, int dropdownDate = 0)
        //{
        //    try
        //    {
        //        string sDatabase = SessionManager.CoreDatabase;
        //        var dp = InboxRepository.GetDropDownDetails(OrgID, sDatabase);
        //        ViewBag.OrgClass = dp;
        //        ViewBag.dropdownValue = dropdownValue;
        //        ViewBag.dropdownDate = dropdownDate;
        //        VMUserDashContents KPIs = InboxRepository.GetDashBoardInfo(Util.GetUserID(), sDatabase, dropdownValue, dropdownDate);
        //        return PartialView("DashboardResult", KPIs);

        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }
        //}
        //[HttpPost]

        #region Dashboard
        public ActionResult DashboardResult(int dropdownValue = 0, int dropdownDate = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var ClassDDLList = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                var DateDDLList = InboxRepository.GetDateDropDownList(sDatabase);
                VMUserDashContents KPIs = InboxRepository.GetDashBoardInfo(iUserID, sDatabase, dropdownValue, dropdownDate);
                KPIs.ClassDDL = ClassDDLList;
                KPIs.DateDDL = DateDDLList;

                return View(KPIs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult FilterDashboardResults(int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var ClassDDLList = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                var DateDDLList = InboxRepository.GetDateDropDownList(sDatabase);
                VMUserDashContents KPIs = InboxRepository.GetDashBoardInfo(iUserID, sDatabase, ClassValue, DateValue);
                KPIs.DDLClassValue = ClassValue;
                KPIs.DDLDateValue = DateValue;
                KPIs.ClassDDL = DateDDLList;
                KPIs.DateDDL = ClassDDLList;
                return PartialView("_DashboardGraphs", KPIs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetDashboardGraphs(VMUserDashContents model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PartialView("_DashboardGraphs", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult GetLeadChart(VMUserDashContents model)
        //{
        //    return PartialView("_GetLeadChart", model);
        //}
        //[HttpPost]
        public ActionResult GetFilteredKPICircles(string ReportID, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMKPIResult> KPIs = InboxRepository.GetKPICircleResult(ReportID, iUserID, sDatabase, OrgID, ClassValue, DateValue, sOrgName);
                var dp = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                ViewBag.OrgClass = dp;
                var ddlDateList = InboxRepository.GetDateDropDownList(sDatabase);
                ViewBag.ddlDateList = ddlDateList;
                KPIs.FirstOrDefault().UserID = iUserID;
                return PartialView("_KPICircles", KPIs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetKPICircleResult(string ReportID, int UserID, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMKPIResult> KPIs = InboxRepository.GetKPICircleResult(ReportID, UserID, sDatabase, OrgID, ClassValue, DateValue, sOrgName);
                var dp = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                ViewBag.OrgClass = dp;
                var ddlDateList = InboxRepository.GetDateDropDownList(sDatabase);
                ViewBag.ddlDateList = ddlDateList;
                KPIs.FirstOrDefault().UserID = iUserID;
                return PartialView("_KPICircles", KPIs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetKPIResults(int ReportID, int DDLClassValue = 0, int DDLDateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                Chart.ClassFilter = DDLClassValue;
                Chart.DateFilter = DDLDateValue;
                GraphData Graph = new GraphData();
                var ddlDateList = InboxRepository.GetDateDropDownList(sDatabase);
                var ddlClassList = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                UserReports report = InboxRepository.GetReportType(ReportID, iUserID, sDatabase);
                if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.PieChart.ToString())
                {
                    Graph = InboxRepository.GetPieChart(Chart, sDatabase, sOrgName);
                    Graph.UserID = iUserID;
                    Graph.ReportID = ReportID;
                    Graph.Type = EnumLocations.Dashboard.ToString();
                    Graph.TabID = 0;
                    Graph.SectionName = "";
                    Graph.DDLClassValue = DDLClassValue;
                    Graph.DDLDateValue = DDLDateValue;
                    Graph.ClassDDL = ddlClassList;
                    Graph.DateDDL = ddlDateList;
                    return PartialView("_KPIPieChart", Graph);
                }
                else if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.BarChart.ToString())
                {
                    LineGraph graph = InboxRepository.GetBarChart(Chart, sDatabase, sOrgName);
                    //here dynamically populating dropdown values
                    graph.ClassDDL = ddlClassList;
                    graph.DateDDL = ddlDateList;
                    graph.Type = EnumLocations.Dashboard.ToString();
                    graph.UserID = iUserID;
                    graph.SectionName = "";
                    graph.ReportID = ReportID;
                    return PartialView("_KPIBarChart", graph);
                }
                else if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.LineChart.ToString())
                {
                    var linechart = InboxRepository.GetLineChart(Chart, sDatabase, sOrgName);
                    //here dynamically populating dropdown values
                    linechart.ClassDDL = ddlClassList;
                    linechart.DateDDL = ddlDateList;
                    linechart.Type = EnumLocations.Dashboard.ToString();
                    linechart.UserID = iUserID;
                    linechart.SectionName = "";
                    linechart.ReportID = ReportID;
                    return PartialView("_LineGraph", linechart);
                }
                else if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.ResultList.ToString())
                {
                    VMRunUserQuery UserQuery = new VMRunUserQuery();
                    UserQuery.OrgID = OrgID;
                    UserQuery.UserID = iUserID;
                    UserQuery.database = sDatabase;
                    UserQuery.PageIndex = 1;
                    UserQuery.ReportID = ReportID;
                    UserQuery.LeadID = 0;
                    UserQuery.ClassFilter = DDLClassValue;
                    UserQuery.DateFilter = DDLDateValue;
                    UserQuery.SearchText = null;
                    UserQuery.SearchType = null;
                    var ResultList = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                    ResultList.ClassDDL = ddlClassList;
                    ResultList.DateDDL = ddlDateList;
                    ResultList.UserID = iUserID;
                    ResultList.ReportID = ReportID;
                    return PartialView("_QueryPreviewGrid", ResultList);
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
        public ActionResult GetDefaultKPIResults(string ReportID, string Type, int UserID, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMChart Chart = new VMChart();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                int iQueryID = 0;
                Guid QueryIDGUID = Guid.Empty;
                int.TryParse(ReportID, out iQueryID);
                Guid.TryParse(ReportID, out QueryIDGUID);
                if (iQueryID > 0)
                {
                    Chart.ReportID = iQueryID;
                    Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    Chart.FKi1ClickIDXIGUID = QueryIDGUID;
                }
                Chart.OrgID = OrgID;
                Chart.UserID = UserID;
                Chart.Database = sDatabase;
                Chart.ClassFilter = ClassValue;
                Chart.DateFilter = DateValue;
                GraphData Graph = new GraphData();
                var ddlDateList = InboxRepository.GetDateDropDownList(sDatabase);
                var ddlClassList = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);

                if (Type == EnumDisplayTypes.PieChart.ToString())
                {
                    Graph = InboxRepository.GetPieChart(Chart, sDatabase, sOrgName);
                    Graph.Type = EnumLocations.Dashboard.ToString();
                    Graph.TabID = 0;
                    Graph.SectionName = "";
                    Graph.ClassDDL = ddlClassList;
                    Graph.DateDDL = ddlDateList;
                    if (iQueryID > 0)
                    {
                        Graph.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Graph.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    Graph.UserID = UserID;
                    return PartialView("_KPIPieChart", Graph);
                }
                else if (Type == EnumDisplayTypes.BarChart.ToString())
                {
                    var graph = InboxRepository.GetBarChart(Chart, sDatabase, sOrgName);
                    //here dynamically populating dropdown values
                    graph.ClassDDL = ddlClassList;
                    graph.DateDDL = ddlDateList;
                    graph.Type = EnumLocations.Dashboard.ToString();
                    if (iQueryID > 0)
                    {
                        graph.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        graph.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    graph.UserID = UserID;
                    graph.SectionName = "";
                    return PartialView("_KPIBarChart", graph);
                }
                else if (Type == EnumDisplayTypes.LineChart.ToString())
                {
                    var linechart = InboxRepository.GetLineChart(Chart, sDatabase, sOrgName);
                    //here dynamically populating dropdown values
                    linechart.ClassDDL = ddlClassList;
                    linechart.DateDDL = ddlDateList;
                    linechart.Type = EnumLocations.Dashboard.ToString();
                    if (iQueryID > 0)
                    {
                        linechart.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        linechart.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    linechart.UserID = UserID;
                    linechart.SectionName = "";
                    return PartialView("_LineGraph", linechart);
                }
                else if (Type == EnumDisplayTypes.ResultList.ToString())
                {
                    VMQuickSearch Search = new VMQuickSearch();
                    if (iQueryID > 0)
                    {
                        Search.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Search.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    Search.UserID = UserID;
                    Search.OrgID = OrgID;
                    Search.database = sDatabase;
                    Search.PageIndex = 1;
                    Search.SearchText = null;
                    var Result = InboxRepository.GetHeadings(ReportID, sDatabase, OrgID, UserID, sOrgName);
                    VMResultList model = new VMResultList();
                    model.Headings = Result.Headings;
                    if (iQueryID > 0)
                    {
                        model.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetDefaultKPIResults: " + QueryIDGUID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        model.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    model.SearchText = null;
                    model.ResultListDisplayType = Result.ResultListDisplayType;
                    model.QueryName = Result.QueryName;
                    model.ClassDDL = ddlClassList;
                    model.DateDDL = ddlDateList;
                    // model.QueryID = ReportID;
                    model.UserID = UserID;
                    model.Rows = Result.Rows;
                    //return PartialView("_InnerReportPagination", model);
                    //var ResultList = InboxRepository.RunUserQuery(ReportID,0, UserID, sDatabase, OrgID, 0, ClassValue, DateValue,null);
                    model.ShowAs = Result.ShowAs;
                    return PartialView("_QueryPreviewGrid", model);
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
        #endregion Dashboard
        public ActionResult GetGridFilteredData(string ReportID, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var ddlDateList = InboxRepository.GetDateDropDownList(sDatabase);
                var ddlClassList = InboxRepository.GetClassDropDownList(OrgID, iUserID, sOrgName, sDatabase);
                VMRunUserQuery UserQuery = new VMRunUserQuery();
                int iQueryID = 0;
                Guid QueryIDGUID = Guid.Empty;
                int.TryParse(ReportID, out iQueryID);
                Guid.TryParse(ReportID, out QueryIDGUID);
                if (iQueryID > 0)
                {
                    UserQuery.ReportID = iQueryID;
                    Common.SaveErrorLog("Error at InboxController: GetGridFilteredData: " + iQueryID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    UserQuery.FKi1ClickIDXIGUID = QueryIDGUID;
                }
                UserQuery.OrgID = OrgID;
                UserQuery.UserID = iUserID;
                UserQuery.database = sDatabase;
                UserQuery.PageIndex = 0;

                UserQuery.LeadID = 0;
                UserQuery.ClassFilter = ClassValue;
                UserQuery.DateFilter = DateValue;
                UserQuery.SearchText = null;
                UserQuery.SearchType = null;
                var ResultList = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                if (iQueryID > 0)
                {
                    ResultList.ReportID = iQueryID;
                    Common.SaveErrorLog("Error at InboxController: GetGridFilteredData: " + iQueryID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    ResultList.FKi1ClickIDXIGUID = QueryIDGUID;
                }
                ResultList.ClassDDL = ddlClassList;
                ResultList.DateDDL = ddlDateList;
                return PartialView("_QueryPreviewGrid", ResultList);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        public ActionResult AllUserQueries()
        {
            ViewBag.UserID = SessionManager.UserID;
            ViewBag.UserName = User.Identity.GetUserName();
            return View();
        }
        public ActionResult GetUserQueriesByID(jQueryDataTableParamModel param, int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = InboxRepository.UserQueriesList(param, UserID, iUserID, sOrgName, sDatabase);
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

        //NewLeadsPopUp
        public ActionResult NewLeadsResultListPopUp(int LeadID, int QueryID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                VMViewPopup popup = new VMViewPopup();
                popup.LeadID = LeadID;
                popup.ReportID = QueryID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var Result = InboxRepository.AssignLeadToUser(LeadID, iUserID, OrgID, sDatabase);
                return View(popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult LeadReminder(int LeadID, int ReportID, int ClassID)
        {
            Reminders model = new Reminders();
            model.LeadID = LeadID;
            model.ReportID = ReportID;
            model.ClassID = ClassID;
            return PartialView("_LeadReminder", model);
        }
        public ActionResult LeadReminderList(int LeadID)
        {
            return PartialView("_LeadReminderList", LeadID);
        }
        public ActionResult GetReminderList(jQueryDataTableParamModel param, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = InboxRepository.GetReminderList(param, LeadID, iUserID, sOrgName, sDatabase.ToString());
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
        public ActionResult GetReminderUserCount(int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var model = InboxRepository.GetReminderUserCount(UserID, OrgID, sDatabase, sOrgName);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //public ActionResult GetViewMessage(int UserID)
        //{
        //    try
        //    {
        //        var model = InboxRepository.GetReminderUserCount(UserID, sDatabase);
        //        return Json(model, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //        return null;
        //    }
        //}
        [HttpPost]
        public ActionResult SaveReminder(Reminders obj)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var model = InboxRepository.SaveReminder(iUserID, OrgID, sDatabase, sOrgName, obj);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult GetLeadSourcePolarchart()
        //{
        //    List<DashBoardGraphs> graph = InboxRepository.GetLeadSourcePolarchart();
        //    return Json(graph, JsonRequestBehavior.AllowGet);
        //}
        //[HttpPost]
        #region Graphs

        public ActionResult GetPieChart(int ReportID, string ResultIn, string Query, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                Chart.Query = Query;
                Chart.ClassFilter = ClassValue;
                Chart.DateFilter = DateValue;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                GraphData Graph = new GraphData();
                Graph = InboxRepository.GetPieChart(Chart, sDatabase, sOrgName);
                if (ResultIn == "Run")
                {
                    Graph.Type = "Run";
                    Graph.SectionName = "";
                    Graph.ClassDDL = new List<VMDropDown>();
                    Graph.DateDDL = new List<VMDropDown>();
                    return PartialView("_KPIPieChart", Graph);
                }
                else
                {
                    return Json(Graph, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json("Failure", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetLineGraph(int ReportID, string ResultIn, string Query, int ClassValue = 0, int DateValue = 0)//mm
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                Chart.DateFilter = DateValue;
                Chart.ClassFilter = ClassValue;
                Chart.Query = Query;
                LineGraph graph = InboxRepository.GetLineChart(Chart, sDatabase, sOrgName);
                graph.ReportID = ReportID;
                if (ResultIn == "Run")
                {
                    graph.Type = "Run";
                    graph.SectionName = "";
                    graph.ClassDDL = new List<VMDropDown>();
                    graph.DateDDL = new List<VMDropDown>();
                    return PartialView("_LineGraph", graph);
                }
                else
                {
                    return Json(graph, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetBarChart(int ReportID, string ResultIn, string Query, int ClassValue = 0, int DateValue = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMChart Chart = new VMChart();
                Chart.ReportID = ReportID;
                Chart.OrgID = OrgID;
                Chart.UserID = iUserID;
                Chart.Database = sDatabase;
                Chart.DateFilter = DateValue;
                Chart.ClassFilter = ClassValue;
                Chart.Query = Query;
                LineGraph graph = InboxRepository.GetBarChart(Chart, sDatabase, sOrgName);
                graph.ReportID = ReportID;
                //model.BarData = graph;
                //model.QueryName = graph.FirstOrDefault().QueryName;
                if (ResultIn == "Run")
                {
                    graph.Type = "Run";
                    graph.SectionName = "";
                    graph.ClassDDL = new List<VMDropDown>();
                    graph.DateDDL = new List<VMDropDown>();
                    return PartialView("_KPIBarChart", graph);
                }
                else
                {
                    return Json(graph, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion Graphs

        public ActionResult GetLeadsByClass(int dropdownValue = 0, int dropdownDate = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                ViewBag.dropdownDate = dropdownDate;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<DashBoardGraphs> graph = InboxRepository.GetLeadsByClass(iUserID, sOrgName, sDatabase, OrgID, dropdownValue, dropdownDate);
                return Json(graph, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetLeadsByClassForTab(string ReportID, int dropdownValue = 0, int dropdownDate = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<DashBoardGraphs> graph = InboxRepository.GetLeadsByClassForTab(iUserID, sDatabase, OrgID, ReportID, sOrgName);
                return Json(graph, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //public ActionResult GetLineChart()
        //{
        //    List<User> model = InboxRepository.GetLineChart();
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
        [HttpPost]
        public ActionResult SaveColSettings(string[] ColOrder, int UserID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int id = InboxRepository.SaveColSettings(ColOrder, UserID, sDatabase);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult RemoveDashboardGraph(string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //AspNetUsers user = InboxRepository.RemoveDashboardGraph(Type, UserID);
                //return Json(user, JsonRequestBehavior.AllowGet);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetDefaultSettings()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                cXIAppUsers user = InboxRepository.GetDefaultSettings(iUserID, sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult LineChart()
        {
            return PartialView("_LineGraph");
        }

        public ActionResult GetQueriesByType(int RoleID, int ClassType, int Display, int ClassID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMDropDown> model = InboxRepository.GetQueriesByType(RoleID, ClassType, Display, sDatabase, ClassID, OrgID);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllTabs(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var tabs = InboxRepository.GetAllTabs(ReportID, sDatabase);
                return Json(tabs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult DeleteUserReport(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var result = InboxRepository.DeleteUserReport(ReportID, sDatabase);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetDashboardReports()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var result = InboxRepository.GetDashboardReports(iUserID, sOrgName, sDatabase);
                return View("_DashboardReports", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult QuickSearch(string QueryID, int PageIndex)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var LoadType = InboxRepository.GetLoadingType(QueryID, sDatabase);
                int iQueryID = 0;
                Guid QueryIDGUID = Guid.Empty;
                int.TryParse(QueryID, out iQueryID);
                Guid.TryParse(QueryID, out QueryIDGUID);
                if (LoadType[0] == 1)
                {
                    VMResultList Report = new VMResultList(); if (iQueryID > 0)
                    {
                        Report.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: QuickSearch: " + QueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Report.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    var Result = InboxRepository.GetHeadings(QueryID, sDatabase, OrgID, iUserID, sOrgName);
                    Report.Headings = Result.Headings;
                    Report.IsPopup = Result.IsPopup;
                    Report.ResultListDisplayType = Result.ResultListDisplayType;
                    Report.QueryName = Result.QueryName;
                    Report.QueryIcon = Result.QueryIcon;
                    Report.FilterGroup = Result.FilterGroup;
                    Report.IsFilterSearch = Result.IsFilterSearch;
                    Report.SearchType = "Filter";
                    return View(Report);
                }
                else
                {
                    VMRunUserQuery UserQuery = new VMRunUserQuery();
                    UserQuery.OrgID = OrgID;
                    UserQuery.UserID = iUserID;
                    UserQuery.database = sDatabase;
                    UserQuery.PageIndex = PageIndex;
                    if (iQueryID > 0)
                    {
                        UserQuery.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: QuickSearch: " + QueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        UserQuery.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    UserQuery.LeadID = 0;
                    UserQuery.ClassFilter = 0;
                    UserQuery.DateFilter = 0;
                    UserQuery.SearchText = null;
                    UserQuery.SearchType = null;
                    VMResultList model = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                    ViewBag.queryid = QueryID;
                    if (iQueryID > 0)
                    {
                        model.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: QuickSearch: " + QueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        model.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    var AutoLoadResultLlist = model.Rows;
                    if (PageIndex >= 2)
                    {
                        return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                    }
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult DisplayQueryResult(VMResultList model)
        {
            return PartialView("_QueryPreviewFromForm", model);
        }

        public ActionResult DisplayAutoLoadResult(VMResultList model)
        {
            return PartialView("_SearchResultList", model);
        }

        public ActionResult GetSearchResult(string SearchText, string ReportID, int PageIndex, string SearchType, string BO)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var LoadingType = InboxRepository.GetLoadingType(ReportID, sDatabase);
                int iQueryID = 0;
                Guid QueryIDGUID = Guid.Empty;
                int.TryParse(ReportID, out iQueryID);
                Guid.TryParse(ReportID, out QueryIDGUID);

                //var IsSearchable = InboxRepository.IsSearchable(SearchText, ReportID);
                //var Searchable = IsSearchable.Select(m => m).Where(k => k.Key.Equals(false)).ToList();
                //PageIndex = 1;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                if (LoadingType[0] == 1)
                {
                    VMQuickSearch Search = new VMQuickSearch();
                    if (iQueryID > 0)
                    {
                        Search.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetSearchResult: " + iQueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Search.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    Search.OrgID = OrgID;
                    Search.database = sDatabase;
                    Search.PageIndex = PageIndex;
                    Search.SearchText = SearchText;
                    var Result = InboxRepository.GetHeadings(ReportID, sDatabase, OrgID, iUserID, sOrgName);
                    if (iQueryID > 0)
                    {
                        Result.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetSearchResult: " + iQueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Result.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    Result.SearchText = SearchText;
                    Result.SearchType = SearchType;
                    Result.PreviewType = "Quick";
                    return View("ReportResult", Result);
                    //if (SearchType == "Structured")
                    //{
                    //    return PartialView("_StructuredResultList", model);
                    //}
                    //else
                    //{
                    //    return PartialView("_QueryPreviewFromForm", model);
                    //}
                }
                else
                {
                    VMRunUserQuery UserQuery = new VMRunUserQuery();
                    UserQuery.OrgID = OrgID;
                    UserQuery.UserID = iUserID;
                    UserQuery.database = sDatabase;
                    UserQuery.PageIndex = PageIndex;
                    if (iQueryID > 0)
                    {
                        UserQuery.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetSearchResult: " + iQueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        UserQuery.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    UserQuery.LeadID = 0;
                    UserQuery.ClassFilter = 0;
                    UserQuery.DateFilter = 0;
                    UserQuery.SearchText = SearchText;
                    UserQuery.SearchType = SearchType;
                    UserQuery.BO = BO;
                    VMResultList model = InboxRepository.RunUserQuery(UserQuery, sDatabase, sOrgName);
                    ViewBag.queryid = ReportID;
                    if (iQueryID > 0)
                    {
                        model.ReportID = iQueryID;
                        Common.SaveErrorLog("Error at InboxController: GetSearchResult: " + iQueryID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        model.FKi1ClickIDXIGUID = QueryIDGUID;
                    }
                    model.SearchType = SearchType;
                    var AutoLoadResultLlist = model.Rows;
                    if (PageIndex >= 2)
                    {
                        return Json(AutoLoadResultLlist, JsonRequestBehavior.AllowGet);
                    }
                    return PartialView("_SearchResultList", model);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult ToDataTable(int i1ClickID, string Name, string FileFormat, string Type, string Fields, string Optrs, string Values, string sGUID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                DataTable dt = new DataTable();
                int iUserID = SessionManager.UserID;
                int iRoleID = SessionManager.iRoleID;
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase, iUserID).oResult;
                int OrgID = oUser.FKiOrganisationID;
                string sSessionID = HttpContext.Session.SessionID;     
                if (Singleton.Instance.oParentGUID.ContainsKey(sGUID))
                {
                    sGUID = Singleton.Instance.oParentGUID[sGUID];
                }
                if(FileFormat==""|| FileFormat == null) { FileFormat = ".xls"; }
                //dt = InboxRepository.ExportDatatableContent(ReportID, OrgID, iUserID, Role, sDatabase, sOrgName);
                XIInfraCache oCache = new XIInfraCache();
                XID1Click oOneclick = (XID1Click)oCache.GetObjectFromCache(XIConstant.Cache1Click, null, i1ClickID.ToString());
                var oOneclickCopy = (XID1Click)oOneclick.Clone(oOneclick);
                XICacheInstance oGUIDParams = oCache.GetAllParamsUnderGUID(sSessionID, sGUID, null);
                List<CNV> nParams = new List<CNV>();
                nParams = oGUIDParams.NMyInstance.Select(m => new CNV { sName = m.Key, sValue = m.Value.sValue, sType = m.Value.sType }).ToList();
                //if (iRoleID == 8033)
                //{
                //    oOneclickCopy.Query = "Select * from sdnaTaskTimeSheet_T order by ID desc";
                //}
                oOneclickCopy.ReplaceFKExpressions(nParams);
                oOneclickCopy.SearchType = Type;
                oOneclickCopy.Fields = Fields;
                oOneclickCopy.Optrs = Optrs;
                oOneclickCopy.Values = Values;
                dt = oOneclickCopy.OneClick_TableResult();
                //put a breakpoint here and check datatable
                string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                string str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\images\\";
                Name = Name + DateTime.Now.TimeOfDay.Hours.ToString() + DateTime.Now.TimeOfDay.Minutes.ToString() + DateTime.Now.TimeOfDay.Seconds.ToString();
                if (Type == "Email")
                {
                    using (FileStream stream = new FileStream(str + Name.Replace(" ", "_") + ".xlsx", FileMode.Create, FileAccess.Write))
                    {
                        IWorkbook wb = new XSSFWorkbook();
                        ISheet sheet = wb.CreateSheet("Sheet1");
                        ICreationHelper cH = wb.GetCreationHelper();
                        IRow rows = sheet.CreateRow(0);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = rows.CreateCell(j);
                            String columnName = dt.Columns[j].ToString();
                            cell.SetCellValue(columnName);
                        }
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            IRow row = sheet.CreateRow(i + 1);
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                ICell cell = row.CreateCell(j);
                                cell.SetCellValue(cH.CreateRichTextString(dt.Rows[i].ItemArray[j].ToString()));
                            }
                        }
                        wb.Write(stream);
                    }
                    var Attachment = str + Name.Replace(" ", "_") + ".xlsx";
                    var Res = InboxRepository.SendMail("raviteja.m@inativetech.com", OrgID, Attachment, sDatabase);
                }
                else
                {
                    IWorkbook workbook;
                    if (FileFormat == ".xlsx")
                    {
                        workbook = new XSSFWorkbook();
                    }
                    else if (FileFormat == ".xls")
                    {
                        workbook = new HSSFWorkbook();
                    }
                    else
                    {
                        throw new Exception("This format is not supported");
                    }
                    ISheet sheet1 = workbook.CreateSheet("Sheet 1");
                    IRow row1 = sheet1.CreateRow(0);

                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        ICell cell = row1.CreateCell(j);
                        String columnName = oOneclickCopy.Headings[j].ToString();
                        cell.SetCellValue(columnName);
                    }

                    //loops through data
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        IRow row = sheet1.CreateRow(i + 1);
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            ICell cell = row.CreateCell(j);
                            String columnName = dt.Columns[j].ToString();
                            //var ResolvedValue = oOneclickCopy.OptionListCols.Contains(columnName.ToLower()) ? oOneclickCopy.CheckOptionList(columnName, dt.Rows[i][columnName].ToString(), oOneclickCopy.BOD) : dt.Rows[i][columnName].ToString();
                            var ResolvedValue = oOneclickCopy.OptionListCols.ContainsKey(columnName.ToLower()) ? oOneclickCopy.CheckOptionList(columnName, dt.Rows[i][columnName].ToString(), oOneclickCopy.OptionListCols[columnName.ToLower()]) : dt.Rows[i][columnName].ToString();
                            cell.SetCellValue(ResolvedValue);
                        }
                    }
                    using (var exportData = new MemoryStream())
                    {
                        string FilePath = str + Name + FileFormat;
                        FileStream file = new FileStream(FilePath, FileMode.CreateNew, FileAccess.Write);
                        workbook.Write(file);
                        if (FileFormat == ".xls")  //xls file format
                        {
                            exportData.WriteTo(file);
                        }
                        file.Close();
                        exportData.Close();
                        //Write to file using file stream  
                        byte[] result = System.IO.File.ReadAllBytes(FilePath);
                        System.IO.File.Delete(FilePath);
                        return File(result.ToArray(), "application/vnd.ms-excel");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
            }
            return null;
        }
        public ActionResult GridDeleteTableRow(string InstanceID, int BOID)
        {
            string sDatabase = Session["CoreDatabase"].ToString();
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                //oUser.UserID = iUserID;
                //oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;

                //var result = InboxRepository.GridDeleteTableRow(InstanceID, BOID, iUserID, sOrgName, sDatabase);

                int iStatus = 0;
                XIInfraUsers oUser = new XIInfraUsers();
                oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult;
                int OrgID = oUser.FKiOrganisationID;
                XIInfraCache oCache = new XIInfraCache();

                try
                {
                    XIDBO oBOD = new XIDBO();
                    oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, null, BOID.ToString());
                    //XIDXI oDXI = new XIDXI();
                    List<XIIBO> oBOInstanceLIst = new List<XIIBO>();
                    //oDXI.sCoreDatabase = oUser.sCoreDatabaseName;
                    //oDXI.sOrgDatabase = oUser.sDatabaseName;
                    //var sDataSource = oCache.GetObjectFromCache(XIConstant.CacheDataSource, null, oBOD.iDataSource.ToString());
                    //var sDataSource = oDXI.GetBODataSource(oBOD.iDataSource);
                    //SqlConnection Con = new SqlConnection(sDataSource);
                    //Con.Open();
                    //SqlCommand cmd = new SqlCommand();
                    //cmd.Connection = Con;
                    //if (oBOD.bUID == true)
                    //{
                    //    cmd.CommandText = "delete from " + oBOD.TableName + " where XGUID=" + InstanceID + "";
                    //}
                    //else
                    //{
                    //    int iInstanceID = Convert.ToInt32(InstanceID);
                    //    cmd.CommandText = "delete from " + oBOD.TableName + " where ID=" + iInstanceID + "";
                    //}
                    //SqlDataReader reader = cmd.ExecuteReader();
                    iStatus = 1;
                }
                catch (Exception ex)
                {
                    iStatus = 0;
                }
                //return iStatus;


                return Json(iStatus, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetXIOneClickByID(int iOneClickID, string sType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                VMResultList Result = InboxRepository.GetXIOneClickByID(iOneClickID, iUserID, sOrgName, sDatabase);
                ViewBag.IsValueSet = "True";
                return PartialView("_LoadXIOneClickParams", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }
        //public string[] GetFiles()
        //{
        //    string rootdir = @"C:\some\path";

        //    // get list of files in root directory and all its subdirectories
        //    string[] files = Directory.GetFiles(rootdir, "*", SearchOption.AllDirectories);
        //    //Console.WriteLine(String.Join(Environment.NewLine, files));

        //    // get list of directories and subdirectories
        //    string[] dirs = Directory.GetDirectories(rootdir, "*", SearchOption.AllDirectories);
        //   // Console.WriteLine(String.Join(Environment.NewLine, dirs));
        //}
    }
}