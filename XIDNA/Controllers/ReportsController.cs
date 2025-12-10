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
using XIDNA.Common;
using XICore;
using XIDatabase;
using System.Data;
using XISystem;
using System.Web.Script.Serialization;

namespace XIDNA.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IReportsRepository ReportsRepository;

        public ReportsController() : this(new ReportsRepository()) { }

        public ReportsController(IReportsRepository ReportsRepository)
        {
            this.ReportsRepository = ReportsRepository;
        }
        XIInfraUsers oUser = new XIInfraUsers();
        CommonRepository Common = new CommonRepository();
        // GET: Reports
        public ActionResult Index(string ResultIn, string Query, int ReportID = 0)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int UserID = Convert.ToInt32(User.Identity.GetUserId());
                List<UserReports> Reports = new List<UserReports>();
                if (ResultIn == "Run")
                {
                    UserReports Report = new UserReports();
                    Report.ReportID = ReportID;
                    Reports.Add(Report);
                    Reports.ToList().ForEach(m => m.Query = Query);
                    Reports.FirstOrDefault().TypeID = 0;
                }
                else
                {
                    Reports = ReportsRepository.GetAllDashboardReports(UserID, sDatabase);
                    if (Reports != null && Reports.Count > 0)
                    {
                        Reports.FirstOrDefault().TypeID = 1;
                    }
                }
                return View(Reports);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        [AllowAnonymous]
        public ActionResult GetIncome()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sGetResult = ReportsRepository.GetDialyIncome();
                return Json(sGetResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult GetTransLeadLife()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sGetResult = ReportsRepository.GetTransLeadLife();
                return Json(sGetResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        public ActionResult GetLeadLife()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sGetResult = ReportsRepository.GetLeadLife();
                return Json(sGetResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        //public ActionResult GetCLassAndSource()
        //{
        //    var sGetResult = ReportsRepository.GetCLassAndSource();
        //    return Json(sGetResult, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult GetCLassAndSource()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                var sGetResult = ReportsRepository.GetCLassAndSource();
                return Json(sGetResult, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }
        //public ActionResult GetDashboardReports()
        //{
        //    string DbName = SessionManager.CoreDatabase;
        //    string Query = "select FKiSourceID, istatus, count(id) as Total from leads where FKiSourceID IN(select ID from OrganizationSources where OrganizationID=1) group by FKiSourceID, iStatus order by FKiSourceID";
        //    var sGetResult = ReportsRepository.GetDashboardReports(DbName);
        //    return PartialView("GetDashboardReports", sGetResult);
        //}

        //public ActionResult GetClassSource()
        //{
        //    string DbName = SessionManager.CoreDatabase;
        //    string Query = "select FKiSourceID, istatus, count(id) as Total from leads where FKiSourceID IN(select ID from OrganizationSources where OrganizationID=1) group by FKiSourceID, iStatus order by FKiSourceID";
        //    var sGetResult = ReportsRepository.GetClassSource(DbName);
        //    return PartialView("GetDashboardReports", sGetResult);
        //}

        public ActionResult GetOneClickSummary(int ReportID, int Count, string Query)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                string DbName = SessionManager.CoreDatabase;
                int OrgID = 0;
                //string Query = "select FKiSourceID, istatus, count(id) as Total from leads where FKiSourceID IN(select ID from OrganizationSources where OrganizationID=1) group by FKiSourceID, iStatus order by FKiSourceID";
                var sGetResult = ReportsRepository.GetOneClickSummary(ReportID, Query, OrgID, sDatabase);
                sGetResult.FirstOrDefault().Count = Count;
                return PartialView("GetDashboardReports", sGetResult);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult RunScheduler()
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                int iUserID = SessionManager.UserID; string sOrgName = SessionManager.OrganisationName;
                oUser.UserID = iUserID; oUser = (XIInfraUsers)oUser.Get_UserDetails(sDatabase).oResult; int OrgID = oUser.FKiOrganisationID;
                var Result = ReportsRepository.RunScheduler(sDatabase);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                Common.SaveErrorLog(ex.ToString(), sDatabase);
                return null;
            }
        }

        public ActionResult SchedulingLog()
        {
            return View();
        }

        public ActionResult GetSchedulersLogList(jQueryDataTableParamModel param)
        {
            string sDatabase = SessionManager.CoreDatabase;
            try
            {
                //int OrgID = 0;
                param.iSortCol = Convert.ToInt32(Request["iSortCol_0"]);
                param.sSortDir = Request["sSortDir_0"].ToString();
                int OrgID = 0;
                var result = ReportsRepository.GetSchedulersLogList(param, OrgID, sDatabase);
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
        public ActionResult GetReport()
        {
            return View();
        }
        //public ActionResult GetReportData()
        //{
        //    try
        //    {
        //        int ReportID = 3;
        //        XIReports oReport = new XIReports();
        //        var result = oReport.GenerateReport(ReportID);
        //        JavaScriptSerializer serializer = new JavaScriptSerializer();
        //        List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
        //        Dictionary<string, object> row;
        //        DataTable dt = result.FinalResult;
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            row = new Dictionary<string, object>();
        //            foreach (DataColumn col in dt.Columns)
        //            {
        //                row.Add(col.ColumnName, dr[col]);
        //            }
        //            rows.Add(row);
        //        }
        //        return Json(serializer.Serialize(rows), JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}
    }
}