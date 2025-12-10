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
using XISystem;
using System.Net;

namespace XIDNA.Controllers
{
    [Authorize]
    [SessionTimeout]
    public class PopupController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPopupRepository PopupRepository;

        public PopupController() : this(new PopupRepository()) { }

        public PopupController(IPopupRepository PopupRepository)
        {
            this.PopupRepository = PopupRepository;
        }
        CommonRepository Common = new CommonRepository();
        XIInfraUsers oUser = new XIInfraUsers();
        XIInfraCache oCache = new XIInfraCache();
        ModelDbContext dbContext = new ModelDbContext();
        //
        // GET: /Popup/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetTabsForm()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                Tabs model = new Tabs();
                List<VMDropDown> ranks = new List<VMDropDown>();
                ranks = PopupRepository.GetAllRanks(0, sDatabase);
                var secranks = PopupRepository.GetAllSecRanks(model.ID, sDatabase);
                model.Ranks = ranks;
                model.SecRanks = secranks;
                //model.Category = PopupType;
                model.PopupList = PopupRepository.GetCategoryDetails(sDatabase);
                return PartialView("_TabsForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetTabsReports(int PopupID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Tabs = PopupRepository.GetTabsReports(PopupID, sDatabase);
                return PartialView("_TabsReports", Tabs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }


        public ActionResult GetCategoryDetails()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Tabs = PopupRepository.GetCategoryDetails(sDatabase);
                return Json(Tabs, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveTab(Tabs model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Tab = PopupRepository.SaveTab(model, sDatabase);
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SaveSection(Sections model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var section = PopupRepository.SaveSection(model, sDatabase);
                return Json(section, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AddSection(int SectionID, string Type, int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                Sections section = new Sections();
                if (Type == "Create" || Type == "FormCreate" || Type == "EditCreate")
                {
                    section.SectionRanks = PopupRepository.GetAllSecRanks(TabID, sDatabase);
                    section.Classes = PopupRepository.GetClasses(OrgID, sDatabase);
                    section.TabID = TabID;
                    section.Type = Type;
                    section.TabPreview = new VMAssignReports();
                }
                else
                {
                    section = PopupRepository.GetSectionDetails(SectionID, sDatabase);
                    section.SectionRanks = PopupRepository.GetAllSectionRanks(section.TabID, sDatabase);
                    section.Type = "Edit";
                    section.Classes = PopupRepository.GetClasses(OrgID, sDatabase);
                }

                return PartialView("_EditSection", section);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult TabsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.TabsList(param, sDatabase);
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

        public ActionResult GetAllRanks(int Category)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var ranks = PopupRepository.GetAllRanks(Category, sDatabase);
                return Json(ranks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetSectionsByTab(int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<Sections> Section = PopupRepository.SectionsList(TabID, sDatabase);
                return PartialView("_SectionsGrid", Section);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetSectionsForEdit(int TabID)
        {
            return PartialView("_SectionsGridEdit", TabID);
        }
        public ActionResult TabSectionsList(jQueryDataTableParamModel param, int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.TabSectionsList(param, TabID, sDatabase);
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
        public ActionResult EditTab(int TabID, int PopupID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var tab = PopupRepository.EditTab(TabID, sDatabase);
                var ranks = PopupRepository.GetAllRanks(PopupID, sDatabase);
                var secranks = PopupRepository.GetAllSecRanks(TabID, sDatabase);
                var tabsections = PopupRepository.GetAllTabSections(TabID, sDatabase);
                tab.PopupList = PopupRepository.GetCategoryDetails(sDatabase);
                tab.SecRanks = secranks;
                tab.Ranks = ranks;
                Sections section = new Sections();
                section.TabSections = tabsections;
                tab.Section = section;
                return PartialView("_EditTab", tab);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult DeleteTab(int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var tab = PopupRepository.DeleteTab(TabID, sDatabase);
                return Json(TabID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult AssignReportToTab()
        {
            return View();
        }
        public ActionResult TabsReportsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.TabsReportsList(param, sDatabase);
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
        public ActionResult AssignReportsForm()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                string Category = null;
                var tabs = PopupRepository.GetTabs(Category, sDatabase);
                var classes = PopupRepository.GetClasses(OrgID, sDatabase);
                Tab1Clicks model = new Tab1Clicks();
                model.Tabs = tabs;
                model.Classes = classes;
                return PartialView("_AssignReportsForm", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult AssignReportsFromTab(int TabID, int SectionID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var tabs = PopupRepository.GetCategoryTabs(TabID, sDatabase);
                var classes = PopupRepository.GetClasses(OrgID, sDatabase);
                var section = PopupRepository.GetSectionDetails(SectionID, sDatabase);
                Tab1Clicks model = new Tab1Clicks();
                model.TabID = TabID;
                model.Tabs = tabs;
                model.Classes = classes;
                model.Section = section;
                return PartialView("_AssignReportsFromTab", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetTabSections(int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var tabsections = PopupRepository.GetAllTabSections(TabID, sDatabase);
                return Json(tabsections, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetQueriesByType(int TabID, int DisplayAs, int ClassType, string SectionID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<Classes> model = PopupRepository.GetQueriesByType(TabID, DisplayAs, ClassType, SectionID, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetQueriesByTypeForSection(int DisplayAs, int ClassType)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<Classes> model = PopupRepository.GetQueriesByTypeForSection(DisplayAs, ClassType, sDatabase);
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetQueriesByTypeForEdit(int Tab1ClickID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                List<Classes> model = PopupRepository.GetQueriesByTypeForEdit(Tab1ClickID, sDatabase);
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
        public ActionResult SaveReportToTab(Tab1Clicks model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                model.UserID = iUserID;
                model.OrgID = OrgID;
                var id = PopupRepository.SaveReportToTab(model, sDatabase);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditTabReport(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var classes = PopupRepository.GetClasses(OrgID, sDatabase);
                Tab1Clicks model = PopupRepository.EditTabReport(ID, sDatabase);
                var tabs = new List<VMDropDown>(); //PopupRepository.GetTabs(model.Category);
                model.Tabs = tabs;
                model.Classes = classes;
                return PartialView("_EditTabReport", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult DeleteReportToTab(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var id = PopupRepository.DeleteTabReport(ID, sDatabase);
                return Json(ID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult IsExistsTabName(string Name, int ID, int PopupID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PopupRepository.IsExistsTabName(Name, ID, PopupID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
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
        public ActionResult IsExistsSectionName(string Name, int ID, int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PopupRepository.IsExistsSectionName(Name, ID, TabID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult TabSpecificReports(int Tab)
        {
            return PartialView("_TabSpecificReports", Tab);
        }
        public ActionResult TabSpecificList(jQueryDataTableParamModel param, int Tab)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.TabSpecificList(param, Tab, sDatabase);
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

        public ActionResult GetAssignedClasses(int Tab)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var classes = PopupRepository.GetAssignedClasses(Tab, sDatabase);
                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAssignedSecClasses(int SectionID, int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var classes = PopupRepository.GetAssignedSecClasses(SectionID, TabID, sDatabase);
                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllClasses()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var classes = PopupRepository.GetClasses(OrgID, sDatabase);
                return Json(classes, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllSecRanks(int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var secranks = PopupRepository.GetAllSecRanks(TabID, sDatabase);
                return Json(secranks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetRanksByCategory(int Category)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var secranks = PopupRepository.GetRanksByCategory(Category, sDatabase);
                return Json(secranks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetTabsByCategory(int Category)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var secranks = PopupRepository.GetTabsByCategory(Category, sDatabase);
                return Json(secranks, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetAllTabSections(int TabID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var tabsections = PopupRepository.GetAllTabSections(TabID, sDatabase);
                return Json(tabsections, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetSectionDetails(int SectionID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                Sections section = PopupRepository.GetSectionDetails(SectionID, sDatabase);
                section.SectionRanks = PopupRepository.GetAllSectionRanks(section.TabID, sDatabase);
                return Json(section, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveSectionFields(Sections model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var section = PopupRepository.SaveSectionFields(model, sDatabase);
                return Json(section, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetQueryPreview(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMQueryPreview res = PopupRepository.GetQueryPreview(ReportID, iUserID, sOrgName, sDatabase, OrgID);
                return PartialView("_TabPreview", res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetKPICircleResult(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMKPIResult> KPIs = PopupRepository.GetKPICircleResult(ReportID, iUserID, sOrgName, sDatabase, OrgID);
                return PartialView("_KPICircles", KPIs);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetPieChartResult(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                GraphData models = new GraphData();
                models.ID = "10";
                //models.value = "20";
                models.ReportID = ReportID;
                models.Type = EnumLocations.Dashboard.ToString();
                return PartialView("_KPIPieChart", models);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetLeadsBySource(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<DashBoardGraphs> graph = PopupRepository.GetLeadsBySource(ReportID, iUserID, sOrgName, sDatabase, OrgID);
                return Json(graph, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetLeadsByClass(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<DashBoardGraphs> graph = PopupRepository.GetLeadsByClass(ReportID, iUserID, sOrgName, sDatabase, OrgID);
                return Json(graph, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetBarChartResult(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                GraphData models = new GraphData();
                models.ID = "10";
                //models.value = "20";
                models.ReportID = ReportID;
                models.Type = EnumLocations.Dashboard.ToString();
                return PartialView("_KPIBarChart", models);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetLineGrpahResult(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                LineGraph line = PopupRepository.GetLineGraphForTab(ReportID, OrgID, iUserID, sOrgName, sDatabase);
                return PartialView("_LineGraph", line);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetBarChart(int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<DashBoardGraphs> graph = PopupRepository.GetBarChartResult(ReportID, iUserID, sOrgName, sDatabase, OrgID);
                return Json(graph, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetSectionType(int SectionID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var type = PopupRepository.GetSectionType(SectionID, sDatabase);
                return Json(type, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult DeleteSection(int TabID, int SectionID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var id = PopupRepository.DeleteSection(TabID, SectionID, sDatabase);
                return Json(TabID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveCallsContent(string Desc, int LeadID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var id = PopupRepository.SaveCallsContent(Desc, LeadID, OrgID, sDatabase);
                return Json(id, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult EditLeadRecord(int LeadID, int ReportID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMLeadRecord result = PopupRepository.EditLeadRecord(LeadID, ReportID, OrgID, sDatabase);
                return PartialView("_EditLeadRecord", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult SaveLeadRecord(VMLeadRecord model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var result = PopupRepository.SaveLeadRecord(model, OrgID, sDatabase);
                return PartialView("_EditLeadRecord", result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #region Popups

        public ActionResult Popups()
        {
            return PartialView("PopupList");
        }

        public ActionResult PopupsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.PopupList(param, iUserID, sOrgName, iOrgID, sDatabase);
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
        //creation of popup
        public ActionResult Popup()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID; var iOrgID = oUser.FKiOrganisationID;
                Popup popup = new Popup();
                var Dropdowns = PopupRepository.GetPopupValues(iUserID, sOrgName, sDatabase);
                popup.Layouts = Dropdowns.Layouts;
                popup.AllBOs = Dropdowns.AllBOs;
                popup.AllColumns = new List<VMDropDown>();
                if (fkiApplicationID == 0)
                {
                    popup.ddlApplications = Common.GetApplicationsDDL();
                }
                popup.FKiApplicationID = fkiApplicationID;
                popup.OrganizationID = iOrgID;
                return View("PopupForm", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetBOColumns(int BOID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Columns = PopupRepository.GetBOColumns(BOID, sDatabase);
                return Json(Columns, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreatePopup(Popup model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                var Tab = PopupRepository.CreatePopup(model, iUserID, sOrgName, iOrgID, sDatabase);
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }

        }

        //Editing of PopupList
        public ActionResult EditPopup(int PopupID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var popup = PopupRepository.EditPopup(PopupID, sDatabase);
                var Dropdowns = PopupRepository.GetPopupValues(iUserID, sOrgName, sDatabase);
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID; var iOrgID = oUser.FKiOrganisationID;
                popup.Layouts = Dropdowns.Layouts;
                popup.AllBOs = Dropdowns.AllBOs;
                if (fkiApplicationID == 0)
                {
                    popup.ddlApplications = Common.GetApplicationsDDL();
                }
                popup.AllColumns = PopupRepository.GetBOColumns(popup.BOID, sDatabase);
                popup.FKiApplicationID = fkiApplicationID;
                popup.OrganizationID = iOrgID;
                return View("PopupForm", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        //Validations for PopupName
        [HttpPost]
        public ActionResult IsExistsPopupName(string Name, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PopupRepository.IsExistsPopupName(Name, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion Popups

        #region Dialogs

        public ActionResult Dialog()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                Dialogs dialog = new Dialogs();
                var Dropdowns = PopupRepository.GetDialogValues(iUserID, sOrgName, sDatabase);
                dialog.Layouts = Dropdowns.Layouts;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                var iOrgID = oUser.FKiOrganisationID;
                if (fkiApplicationID == 0)
                {
                    dialog.ddlApplications = Common.GetApplicationsDDL();
                }
                dialog.FKiApplicationID = fkiApplicationID;
                dialog.OrganizationID = iOrgID;
                return View("DialogForm", dialog);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreateDialog(Dialogs model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                model.CreatedBy = model.UpdatedBy = SessionManager.UserID;
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var iOrgID = oUser.FKiOrganisationID;
                var Tab = PopupRepository.CreateDialog(model, iUserID, sOrgName, iOrgID, sDatabase);
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //Save_Dialog
        [HttpPost]
        public ActionResult Save_Dialog(XIDDialog model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iDialogID = 0;
                int iUserID = SessionManager.UserID;
                XIInfraCache oCache = new XIInfraCache();
                SignalR oSignalR = new SignalR();
                XIConfigs oConfig = new XIConfigs(oSignalR);
                oConfig.iUserID = iUserID;
                oConfig.sSessionID = HttpContext.Session.SessionID;
                var oCR = oConfig.Save_Dialog(model);
                if (oCR.bOK && oCR.oResult != null)
                {
                    var BOI = (XIIBO)oCR.oResult;
                    var DialogID = BOI.AttributeI("id").sValue;
                    int.TryParse(DialogID, out iDialogID);
                    var oLayoutD = new XIDLayout();
                    if (model.LayoutIDXIGUID != null && model.LayoutIDXIGUID != Guid.Empty)
                    {
                        oLayoutD = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, model.LayoutIDXIGUID.ToString());
                    }
                    else if(model.LayoutID > 0)
                    {
                        oLayoutD = (XIDLayout)oCache.GetObjectFromCache(XIConstant.CacheLayout, null, model.LayoutID.ToString());
                    }                    
                    var Mappings = oLayoutD.LayoutMappings;
                    if (Mappings != null && Mappings.Count() > 0)
                    {
                        foreach (var oMap in Mappings)
                        {
                            oMap.PopupID = iDialogID;
                            oMap.PopupIDXIGUID = model.XIGUID;
                            oCR = oConfig.Save_LayoutMapping(oMap);
                        }
                    }
                }
                var Tab = Common.ResponseMessage();
                Tab.sID = iDialogID.ToString();
                return Json(Tab, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult EditDialog(int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var popup = PopupRepository.EditDialog(ID, sDatabase);
                var Dropdowns = PopupRepository.GetDialogValues(iUserID, sOrgName, sDatabase);
                popup.Layouts = Dropdowns.Layouts;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                var iOrgID = oUser.FKiOrganisationID;
                if (fkiApplicationID == 0)
                {
                    popup.ddlApplications = Common.GetApplicationsDDL();
                }
                popup.FKiApplicationID = fkiApplicationID;
                popup.OrganizationID = iOrgID;
                return View("DialogForm", popup);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        public ActionResult Dialogs(string sType)
        {
            ViewBag.sType = sType;
            return PartialView("DialogList");
        }

        public ActionResult DialogsList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.DialogList(param, iUserID, sOrgName, iOrgID, sDatabase);
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
        public ActionResult IsExistsDialogName(string DialogName, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PopupRepository.IsExistsDialogName(DialogName, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetXiLayoutListByOrg(string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                if (Type == "Popup")
                {
                    Popup Result = PopupRepository.GetPopupValues(iUserID, sOrgName, sDatabase);
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Dialogs Result = PopupRepository.GetDialogValues(iUserID, sOrgName, sDatabase);
                    return Json(Result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }
        }

        //CopyDialog
        public ActionResult CopyDialogByID(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                var CopyDialogID = PopupRepository.CopyDialogByID(ID, OrgID, iUserID, sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        #endregion Dialogs

        #region Stages
        //creation of Stages
        public ActionResult Stages()
        {
            Stages s = new Stages();
            //var Users = PopupRepository.GetUsers();
            return View("StagesForm", s);
        }
        [HttpPost]
        public ActionResult CreateStage(Stages model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                model.OrganizationID = OrgID;
                var Stage = PopupRepository.CreateStage(model, sDatabase);
                return Json(Stage, JsonRequestBehavior.AllowGet);
                //return View ("StagesForm");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        //Operations 
        public ActionResult Operations(int StageID, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
            oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
            try
            {
                if (Type == "Create")
                {
                    Stages s = new Stages();
                    s.ID = StageID;
                    s.Type = Type;
                    s.PopupList = PopupRepository.GetPopupsList(OrgID, sDatabase);
                    return View("_Operations", s);
                }
                else
                {
                    ModelDbContext dbContext = new ModelDbContext();
                    Stages s = new Stages();
                    s = PopupRepository.GetStageDetails(StageID, sDatabase);
                    s.Type = "Edit";
                    s.PopupList = PopupRepository.GetPopupsList(OrgID, sDatabase);
                    return View("_Operations", s);
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
        public ActionResult AddOperations(Stages model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var operations = PopupRepository.AddOperations(model, sDatabase);
                return Json(operations, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        //Editing of StagesList
        public ActionResult EditStage(int StageID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Stage = PopupRepository.EditStage(StageID, sDatabase);
                Stage.ID = StageID;
                //Stage.TypeC = TypeC;
                return View("StagesForm", Stage);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AllStages()
        {
            return PartialView("StagesList");
        }

        //Creation Of Stages List
        public ActionResult StagesList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.StagesList(param, OrgID, sDatabase);
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
        //Validations for stageName
        [HttpPost]
        public ActionResult IsExistsStageName(string Name, int ID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return PopupRepository.IsExistsStageName(Name, ID, sDatabase) ? Json(true, JsonRequestBehavior.AllowGet)
                     : Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion Stages

        #region StagesFlow
        //Creation of stages flow

        public ActionResult StagesFlow()
        {
            return View("StagesFlow");
        }
        public ActionResult CreateStagesFlow()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                //StagesFlows SFlow = new StagesFlows();
                VMStages SFlow = new VMStages();
                List<VMDropDown> Stages = PopupRepository.GetStages(0, OrgID, sDatabase);
                SFlow.Stages = Stages;
                List<VMDropDown> StagesList = PopupRepository.GetAllStages(0, OrgID, sDatabase);
                SFlow.StagesList = StagesList;
                return View("StagesFlowForm", SFlow);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetSpecificStages(int StageID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                List<VMDropDown> StagesList = PopupRepository.GetAllStages(StageID, OrgID, sDatabase);
                return Json(StagesList, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [HttpPost]
        public ActionResult SaveStagesFlow(VMStages model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                model.OrganizationID = OrgID;
                var sd = PopupRepository.SaveStagesFlow(model, iUserID, sOrgName, sDatabase);
                return Json(sd, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(0, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult StagesFlowList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.StagesFlowList(param, OrgID, iUserID, sOrgName, sDatabase);
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
        public ActionResult EditStageFlow(int ID, int StageID, int OrgID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                //VMStages model = new VMStages();
                var sFlow = PopupRepository.EditStageFlow(ID, iUserID, sOrgName, sDatabase, OrgID);
                List<VMDropDown> Stages = PopupRepository.GetStages(ID, OrgID, sDatabase);
                sFlow.Stages = Stages;
                List<VMDropDown> StagesList = PopupRepository.GetAllStages(StageID, OrgID, sDatabase);
                sFlow.StagesList = StagesList;
                return View("StagesFlowForm", sFlow);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }
        #endregion StagesFlow

        #region Layouts

        public ActionResult PopupLayouts(string sType)
        {
            ViewBag.sType = sType;
            return View("PopupLayoutsGrid");
        }

        public ActionResult PopupLayoutsList(jQueryDataTableParamModel param, string Type)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                var result = PopupRepository.PopupLayoutsList(param, Type, iUserID, sOrgName, iOrgID, sDatabase);
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

        public ActionResult AddEditPopupLayout(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                cLayouts Layouts = new cLayouts();
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                if (fkiApplicationID == 0)
                {
                    Layouts.ddlApplications = Common.GetApplicationsDDL();
                }
                ViewBag.OrgID = OrgID;
                if (ID == 0)
                {
                    Layouts.ddlXiParameters = Common.GetXiParametersDDL(iUserID, sOrgName, sDatabase);
                    Layouts.ddlXIThemes = Common.GetThemesDDL(iUserID, sOrgName, sDatabase);
                    Layouts.FKiApplicationID = fkiApplicationID;
                    Layouts.OrganizationID = OrgID;
                    return PartialView("_AddEditPopupLayout", Layouts);
                }
                else
                {
                    var LayoutDetails = PopupRepository.GetPopupLayoutByID(ID, sDatabase);
                    Layouts.ID = ID;
                    Layouts.LayoutType = LayoutDetails.LayoutType;
                    Layouts.LayoutCode = LayoutDetails.LayoutCode;
                    Layouts.LayoutName = LayoutDetails.LayoutName;
                    Layouts.LayoutLevel = LayoutDetails.LayoutLevel;
                    if (LayoutDetails.LayoutLevel == "OrganisationLevel")
                    {
                        Layouts.Authentication = LayoutDetails.Authentication;
                    }
                    Layouts.XiParameterID = LayoutDetails.XiParameterID;
                    Layouts.iThemeID = LayoutDetails.iThemeID;
                    Layouts.ddlXiParameters = Common.GetXiParametersDDL(iUserID, sOrgName, sDatabase);
                    Layouts.ddlXIThemes = Common.GetThemesDDL(iUserID, sOrgName, sDatabase);
                    Layouts.FKiApplicationID = fkiApplicationID;
                    Layouts.OrganizationID = OrgID;
                    Layouts.bUseParentGUID = LayoutDetails.bUseParentGUID;
                    Layouts.bIsTaskBar = LayoutDetails.bIsTaskBar;
                    Layouts.sTaskBarPosition = LayoutDetails.sTaskBarPosition;
                    Layouts.arrSiloAccess = LayoutDetails.arrSiloAccess;
                    return PartialView("_AddEditPopupLayout", Layouts);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SavePopupLayout(cLayouts model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                model.CreatedBy = model.UpdatedBy = iUserID;
                var Res = PopupRepository.SavePopupLayout(model, 0, null, iUserID, sOrgName, iOrgID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        //IDE
        [HttpPost, ValidateInput(false)]
        public ActionResult Save_Layout(XIDLayout model)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {

                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                int FKiAppID = oUser.FKiApplicationID;
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XI Layout");
                oBOI.BOD = oBOD;
                oBOI.SetAttribute("ID", model.ID.ToString());
                oBOI.SetAttribute("OrganizationID", iOrgID.ToString());
                oBOI.SetAttribute("FKiApplicationID", model.FKiApplicationID.ToString());
                oBOI.SetAttribute("LayoutName", model.LayoutName);
                oBOI.SetAttribute("LayoutType", model.LayoutType);
                oBOI.SetAttribute("LayoutCode", model.LayoutCode);
                oBOI.SetAttribute("XiParameterID", model.XiParameterID.ToString());
                oBOI.SetAttribute("LayoutLevel", model.LayoutLevel);
                oBOI.SetAttribute("iThemeID", model.iThemeID.ToString());
                oBOI.SetAttribute("bUseParentGUID", model.bUseParentGUID.ToString());
                oBOI.SetAttribute("StatusTypeID", model.StatusTypeID.ToString());
                oBOI.SetAttribute("bIsTaskBar", model.bIsTaskBar.ToString());
                if (model.ID == 0)
                {
                    oBOI.SetAttribute("CreatedBy", iUserID.ToString());
                    oBOI.SetAttribute("CreatedTime", DateTime.Now.ToString());
                    oBOI.SetAttribute("CreatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                }
                oBOI.SetAttribute("UpdatedBy", iUserID.ToString());
                oBOI.SetAttribute("UpdatedTime", DateTime.Now.ToString());
                oBOI.SetAttribute("UpdatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                if (model.bIsTaskBar == true)
                {
                    oBOI.SetAttribute("sTaskBarPosition", model.sTaskBarPosition.ToString());
                }
                else
                {
                    oBOI.SetAttribute("sTaskBarPosition", null);
                }
                if (model.arrSiloAccess != null)
                {
                    oBOI.SetAttribute("sSiloAccess", string.Join("|", model.arrSiloAccess));
                }
                else
                {
                    oBOI.SetAttribute("sSiloAccess", "");
                }
                if (model.ID != 0)
                {
                    if (model.LayoutLevel == "OrganisationLevel")
                    {
                        oBOI.SetAttribute("Authentication", model.Authentication);
                    }
                    else
                    {
                        oBOI.SetAttribute("Authentication", null);
                    }
                }
                else
                {
                    oBOI.SetAttribute("Authentication", model.Authentication);
                }
                var PopupLayout = oBOI.Save(oBOI);
                var Res = Common.ResponseMessage();
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = ServiceConstants.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult AddEditPopupLayoutDetails(int LayoutID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = PopupRepository.GetPopupLayoutByID(LayoutID, sDatabase);
                return PartialView("_AddEditPopupLayoutDetails", Result);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult CheckUniqueness(string UniqueName, int LayoutID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = PopupRepository.CheckUniqueness(UniqueName, LayoutID, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //CopyLayout
        public ActionResult CopyPopupLayout(int ID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; var fkiApplicationID = oUser.FKiApplicationID;
                var CopyLayoutID = PopupRepository.CopyPopupLayoutByID(ID, OrgID, iUserID, sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult PopupLayoutMappings()
        {
            return View("PopupLayoutMapping");
        }

        public ActionResult AddEditPopupLayoutMapping(int PopupID = 0, int DialogID = 0, int LayoutID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                VMPopupLayout Layout = new VMPopupLayout();
                Layout = PopupRepository.GetLayoutByID(PopupID, DialogID, LayoutID, sDatabase);
                Layout.XiLinks = Common.GetXiLinksDDL(sDatabase);
                Layout.XIComponents = Common.GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
                Layout.Steps = Common.GetStepsDDL(sDatabase);
                Layout.Popups = PopupRepository.GetPopupsList(OrgID, sDatabase);
                Layout.LayoutType = Layout.LayoutType;
                Layout.DialogID = DialogID;
                Layout.PopupID = PopupID;
                return PartialView("_AddEditPopupLayoutMapping", Layout);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaveLayoutMapping(VMPopupLayoutMappings Mappings)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Result = PopupRepository.SaveLayoutMapping(Mappings, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = "Error Occured" }, JsonRequestBehavior.AllowGet);
            }
        }
        //IDE
        [HttpPost]
        public ActionResult IDESaveLayoutMapping(VMPopupLayoutMappings Mappings)
        {
            //PopupLayoutMappings Mapping = new PopupLayoutMappings();
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                XIIXI oXI = new XIIXI();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XI LayoutMapping");
                oBOI.BOD = oBOD;
                if (Mappings.ID > 0 || (Mappings.XIGUID != null && Mappings.XIGUID != Guid.Empty))
                {
                    oBOI.SetAttribute("ID", Mappings.ID.ToString());
                    oBOI.SetAttribute("XIGUID", Mappings.XIGUID.ToString());
                    oBOI.SetAttribute("PlaceHolderIDXIGUID", Mappings.PlaceHolderIDXIGUID.ToString());
                    oBOI.SetAttribute("PopupLayoutIDXIGUID", Mappings.PopupLayoutIDXIGUID.ToString());
                    if (Mappings.ContentType == "HTML")
                    {
                        oBOI.SetAttribute("XiLinkID", "0");
                        oBOI.SetAttribute("ContentType", Mappings.ContentType);
                        oBOI.SetAttribute("HTMLCode", Mappings.HTMLCode);
                    }
                    else
                    {
                        oBOI.SetAttribute("ContentType", Mappings.ContentType);
                        int iXiLinkID; Guid guidOutput;
                        bool isValid = Guid.TryParse(Mappings.sXiLinkID, out guidOutput);
                        if (int.TryParse(Mappings.sXiLinkID, out iXiLinkID) && iXiLinkID > 0)
                        {
                            if (Mappings.ContentType == "XiLink")
                            {
                                var oXiLinkI = oXI.BOI("XI Link", iXiLinkID.ToString(), "", null, false, true);
                                if (oXiLinkI != null && oXiLinkI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oXiLinkI.AttributeI("xiguid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "XIComponent")
                            {
                                var oComponentI = oXI.BOI("XI Components", iXiLinkID.ToString(), "", null, false, true);
                                if (oComponentI != null && oComponentI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oComponentI.AttributeI("xiguid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "Step")
                            {
                                var oStepI = oXI.BOI("XIQSStepDefinition", iXiLinkID.ToString(), "", null, false, true);
                                if (oStepI != null && oStepI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oStepI.AttributeI("xiguid").sValue);
                                }
                            }
                            oBOI.SetAttribute("XiLinkID", iXiLinkID.ToString());
                        }
                        else if (isValid && guidOutput != Guid.Empty)
                        {
                            if (Mappings.ContentType == "XiLink")
                            {
                                var oXiLinkI = oXI.BOI("XI Link", guidOutput.ToString(), "", null, false, false);
                                if (oXiLinkI != null && oXiLinkI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oXiLinkI.AttributeI("xilinkid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "XIComponent")
                            {
                                var oComponentI = oXI.BOI("XI Components", guidOutput.ToString(), "", null, false, false);
                                if (oComponentI != null && oComponentI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oComponentI.AttributeI("id").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "Step")
                            {
                                var oStepI = oXI.BOI("XIQSStepDefinition", guidOutput.ToString(), "", null, false, false);
                                if (oStepI != null && oStepI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oStepI.AttributeI("id").sValue);
                                }
                            }
                            oBOI.SetAttribute("XiLinkIDXIGUID", guidOutput.ToString());
                        }
                        oBOI.SetAttribute("HTMLCode", "");

                    }
                    if (Mappings.PopupID > 0)
                    {
                        oBOI.SetAttribute("PopupID", Mappings.PopupID.ToString());
                    }
                    if (Mappings.DialogID > 0)
                    {
                        oBOI.SetAttribute("PopupID", Mappings.DialogID.ToString());
                    }
                    if (Mappings.DialogIDXIGUID != null && Mappings.DialogIDXIGUID != Guid.Empty)
                    {
                        oBOI.SetAttribute("PopupIDXIGUID", Mappings.DialogIDXIGUID.ToString());
                    }
                    if (Mappings.LayoutID > 0)
                    {
                        //Map.PopupID = Mappings.LayoutID;
                    }
                    oBOI.SetAttribute("StatusTypeID", Mappings.StatusTypeID.ToString());
                    oBOI.SetAttribute("UpdatedBySYSID", Dns.GetHostAddresses(Dns.GetHostName())[1].ToString());
                    oBOI.SetAttribute("UpdatedTime", DateTime.Now.ToString());
                }
                else
                {
                    oBOI.SetAttribute("PopupLayoutID", Mappings.LayoutID.ToString());
                    oBOI.SetAttribute("PopupLayoutIDXIGUID", Mappings.PopupLayoutIDXIGUID.ToString());
                    oBOI.SetAttribute("PlaceHolderID", Mappings.PlaceHolderID.ToString());
                    oBOI.SetAttribute("PlaceHolderIDXIGUID", Mappings.PlaceHolderIDXIGUID.ToString());
                    oBOI.SetAttribute("XiLinkID", "0");
                    oBOI.SetAttribute("ContentType", Mappings.ContentType);
                    if (Mappings.PopupID > 0)
                    {
                        oBOI.SetAttribute("PopupID", Mappings.PopupID.ToString());
                    }
                    if (Mappings.DialogID > 0) 
                    {
                        oBOI.SetAttribute("PopupID", Mappings.DialogID.ToString());
                    }
                    if (Mappings.DialogIDXIGUID != null && Mappings.DialogIDXIGUID != Guid.Empty)
                    {
                        oBOI.SetAttribute("PopupIDXIGUID", Mappings.DialogIDXIGUID.ToString());
                    }
                    if (Mappings.LayoutID > 0)
                    {
                        //Mapping.PopupID = Mappings.LayoutID;
                    }
                    if (Mappings.ContentType == "HTML")
                    {
                        oBOI.Attributes["XiLinkID".ToLower()] = new XIIAttribute { sName = "XiLinkID", sValue = "0", bDirty = true };
                        oBOI.Attributes["HTMLCode".ToLower()] = new XIIAttribute { sName = "HTMLCode", sValue = Mappings.HTMLCode.ToString(), bDirty = true };
                    }
                    else
                    {
                        int iXiLinkID; Guid guidOutput;
                        bool isValid = Guid.TryParse(Mappings.sXiLinkID, out guidOutput);
                        if (int.TryParse(Mappings.sXiLinkID, out iXiLinkID) && iXiLinkID > 0)
                        {
                            if (Mappings.ContentType == "XiLink")
                            {
                                var oXiLinkI = oXI.BOI("XI Link", iXiLinkID.ToString(), "", null, false, true);
                                if (oXiLinkI != null && oXiLinkI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oXiLinkI.AttributeI("xiguid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "XIComponent")
                            {
                                var oComponentI = oXI.BOI("XI Components", iXiLinkID.ToString(), "", null, false, true);
                                if (oComponentI != null && oComponentI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oComponentI.AttributeI("xiguid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "Step")
                            {
                                var oStepI = oXI.BOI("XIQSStepDefinition", iXiLinkID.ToString(), "", null, false, true);
                                if (oStepI != null && oStepI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkIDXIGUID", oStepI.AttributeI("xiguid").sValue);
                                }
                            }
                            oBOI.SetAttribute("XiLinkID", iXiLinkID.ToString());
                        }
                        else if (isValid && guidOutput != Guid.Empty)
                        {
                            if (Mappings.ContentType == "XiLink")
                            {
                                var oXiLinkI = oXI.BOI("XI Link", guidOutput.ToString(), "", null, false, false);
                                if (oXiLinkI != null && oXiLinkI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oXiLinkI.AttributeI("xilinkid").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "XIComponent")
                            {
                                var oComponentI = oXI.BOI("XI Components", guidOutput.ToString(), "", null, false, false);
                                if (oComponentI != null && oComponentI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oComponentI.AttributeI("id").sValue);
                                }
                            }
                            else if (Mappings.ContentType == "Step")
                            {
                                var oStepI = oXI.BOI("XIQSStepDefinition", guidOutput.ToString(), "", null, false, false);
                                if (oStepI != null && oStepI.Attributes.Count() > 0)
                                {
                                    oBOI.SetAttribute("XiLinkID", oStepI.AttributeI("id").sValue);
                                }
                            }
                            oBOI.SetAttribute("XiLinkIDXIGUID", guidOutput.ToString());
                        }
                    }
                    oBOI.Attributes["Type".ToLower()] = new XIIAttribute { sName = "Type", sValue = Mappings.Type.ToString(), bDirty = true };
                    oBOI.Attributes["CreatedTime".ToLower()] = new XIIAttribute { sName = "CreatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["CreatedBy".ToLower()] = new XIIAttribute { sName = "CreatedBy", sValue = 1.ToString(), bDirty = true };
                    oBOI.Attributes["CreatedBySYSID".ToLower()] = new XIIAttribute { sName = "CreatedBySYSID", sValue = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString(), bDirty = true };
                    oBOI.Attributes["UpdatedBy".ToLower()] = new XIIAttribute { sName = "UpdatedBy", sValue = 1.ToString(), bDirty = true };
                    oBOI.Attributes["UpdatedBySYSID".ToLower()] = new XIIAttribute { sName = "UpdatedBySYSID", sValue = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString(), bDirty = true };
                    oBOI.Attributes["UpdatedTime".ToLower()] = new XIIAttribute { sName = "UpdatedTime", sValue = DateTime.Now.ToString(), bDirty = true };
                    oBOI.Attributes["StatusTypeID".ToLower()] = new XIIAttribute { sName = "StatusTypeID", sValue = Mappings.StatusTypeID.ToString(), bDirty = true };
                    oBOI.Attributes["IsValueSet".ToLower()] = new XIIAttribute { sName = "IsValueSet", sValue = Mappings.IsValueSet.ToString(), bDirty = true };
                }
                var PopupMapping = oBOI.Save(oBOI);
                var iMappingID = Mappings.PlaceHolderIDXIGUID.ToString();
                //var Result = Common.ResponseMessage();
                return Json(iMappingID, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = "Error Occured" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult SaveLayoutDetails(VMPopupLayoutDetails Details)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                var Result = PopupRepository.SaveLayoutDetails(Details, iUserID, sOrgName, sDatabase);
                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return Json(new VMCustomResponse { Status = false, ResponseMessage = "Error Occured" }, JsonRequestBehavior.AllowGet);
            }
        }
        //IDE
        [HttpPost]
        public ActionResult IDESaveLayoutDetails(VMPopupLayoutDetails Details)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                int FKiAppID = oUser.FKiApplicationID;

                XIIBO oBOI = new XIIBO();
                XIDBO oBOD = new XIDBO();
                oBOD = (XIDBO)oCache.GetObjectFromCache(XIConstant.CacheBO, "XILayoutDetails");
                oBOI.BOD = oBOD;
                oBOI.Attributes["XIGUID".ToLower()] = new XIIAttribute { sName = "XIGUID", sValue = Details.PlaceHolderIDXIGUID.ToString(), bDirty = true };
                oBOI.Attributes["FKiApplicationID".ToLower()] = new XIIAttribute { sName = "FKiApplicationID", sValue = FKiAppID.ToString(), bDirty = true };
                oBOI.Attributes["LayoutIDXIGUID".ToLower()] = new XIIAttribute { sName = "LayoutIDXIGUID", sValue = Details.LayoutIDXIGUID.ToString(), bDirty = true };
                oBOI.Attributes["PlaceholderName".ToLower()] = new XIIAttribute { sName = "PlaceholderName", sValue = Details.PlaceholderName, bDirty = true };
                oBOI.Attributes["PlaceholderArea".ToLower()] = new XIIAttribute { sName = "PlaceholderArea", sValue = Details.PlaceholderArea, bDirty = true };
                oBOI.Attributes["PlaceholderUniqueName".ToLower()] = new XIIAttribute { sName = "PlaceholderUniqueName", sValue = Details.PlaceholderUniqueName, bDirty = true };
                oBOI.Attributes["PlaceholderClass".ToLower()] = new XIIAttribute { sName = "PlaceholderClass", sValue = Details.TDClass, bDirty = true };
                oBOI.Attributes["sCssClass".ToLower()] = new XIIAttribute { sName = "sCssClass", sValue = Details.sCssClass, bDirty = true };
                var PopupMapping = oBOI.Save(oBOI);
                if (PopupMapping.bOK && PopupMapping.oResult != null)
                {
                    return Json(new VMCustomResponse { Status = true, ResponseMessage = "Data Saved Successfully" }, JsonRequestBehavior.AllowGet);
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
                return Json(new VMCustomResponse { Status = false, ResponseMessage = "Error Occured" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult HelpLayoutCode()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                return View("HelpLayoutCode");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult AutoCreateLayout(string sBO, int i1ClickID)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int iOrgID = oUser.FKiOrganisationID;
                var Res = PopupRepository.SavePopupLayout(null, i1ClickID, sBO, iUserID, sOrgName, iOrgID, sDatabase);
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
        public ActionResult GetLayoutMappings(int iLayoutID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var Res = Common.GetLayoutMappingsDDL(iLayoutID, sDatabase);
                return Json(Res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        #endregion Layouts

        #region InlineLayouts

        public ActionResult LayoutManagement(string sType)
        {
            ViewBag.sType = sType;
            return View("InlineLayoutsList");
        }

        public ActionResult AddEditLayoutDetails(int LayoutID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID;
                string sOrgName = SessionManager.OrganisationName;
                VMPopupLayout Layout = new VMPopupLayout();
                Layout = PopupRepository.GetLayoutDetailsByID(LayoutID, sDatabase);
                Layout.XiLinks = Common.GetXiLinksDDL(sDatabase);
                Layout.XIComponents = Common.GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
                Layout.Steps = Common.GetStepsDDL(sDatabase);
                //return PartialView("_AddEditLayoutMappings", Layout);
                Layout.LayoutType = Layout.LayoutType;
                Layout.LayoutID = LayoutID;
                return PartialView("_AddEditPopupLayoutMapping", Layout);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }

        }

        #endregion InlineLayouts
    }
}