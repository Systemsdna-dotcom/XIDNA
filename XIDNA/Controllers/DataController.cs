using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XIDNA.Models;
using System.Web.Mvc;
using XIDNA.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.ViewModels;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
//using XIDataBase.Hubs;
using XIDNA.Common;
namespace XIDNA.Controllers
{
    [Authorize]
    public class DataController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IDataRepository DataRepository;
        internal static SqlCommand command = null;
        internal static SqlDependency dependency = null;
        public DataController() : this(new DataRepository()) { }

        public DataController(DataRepository DataRepository)
        {
            this.DataRepository = DataRepository;
        }

        [HttpGet]
        public ActionResult CreateData()
        {
            try
            {
                int OrgID = 0;
                Datas model = new Datas();
                model.ClassLeadList = DataRepository.DataClassList(SessionManager.CoreDatabase, OrgID);
                //model.SourceGroupList = DataRepository.SourceGroupList(SessionManager.CoreDatabase);
                //model.SourceList = DataRepository.SourceList(SessionManager.CoreDatabase);
                return PartialView("_CreateData", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        [HttpPost]
        public ActionResult CreateData(List<FormData> Values)
        {
            try
            {
                int orgid = 0;
                int id = DataRepository.CreateData(Values, SessionManager.CoreDatabase, orgid);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        public ActionResult LeadsList()
        {
            try
            {
                var LeadsList = DataRepository.GetLeadsList(0, SessionManager.CoreDatabase);
                return View(LeadsList);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        [HttpGet]
        public ActionResult GetAssignedLeadsList(int? UserID)
        {
            try
            {
                var model = DataRepository.GetAssignedLeadsList(UserID, 0, SessionManager.CoreDatabase);
                //return Json(model, JsonRequestBehavior.AllowGet);
                return PartialView("AssignedLeadsList", model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        public ActionResult AssignLeads()
        {
            return RedirectToAction("AssignUsertoLeads");
        }

        public ActionResult AssignUsertoLeads()
        {
            try
            {
                int OrgID = 0;
                NewLeads model = new NewLeads();
                List<Classes> Classes = DataRepository.GetClasses(OrgID, SessionManager.CoreDatabase);
                model.ALLTeams = DataRepository.GetUsersbyOrgID(0, SessionManager.CoreDatabase);
                //model.newleads = DataRepository.GetLeadsbyOrgID(0, SessionManager.CoreDatabase);
                model.Classes = Classes;
                return PartialView(model);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        public ActionResult GetleadsListByClass(int? ClassID, int TeamID)
        {
            try
            {
                NewLeads model = new NewLeads();
                model.newleads = DataRepository.GetLeadsbyOrgID(TeamID, ClassID, 0, SessionManager.CoreDatabase);
                return Json(model.newleads, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetUserID(int? UserID)
        {
            return Json(UserID, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AssignUsersToLeads(int[] selectedleads, int selecteduser)
        {
            try
            {
                int orgid = 0;
                var lst = new List<int>();
                lst.AddRange(selectedleads);
                var res = DataRepository.AssignUsersToLeads(lst, selecteduser, SessionManager.CoreDatabase, orgid);
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public ActionResult ImportLeads()
        {
            try
            {
                int orgid = 0;
                string database = SessionManager.CoreDatabase;
                var leads = DataRepository.ImportLeads(orgid, database);
                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        //[HttpPost]
        //public ActionResult GetData()
        //{
        //    string database = SessionManager.CoreDatabase;
        //    using (var connection = new SqlConnection(ServiceUtil.GetClientConnectionString()))
        //    {
        //        connection.Open();

        //        connection.ChangeDatabase(database);
        //        using (SqlCommand command = new SqlCommand(@"SELECT Count(*) FROM [dbo].[Leads]", connection))
        //        {
        //            // Make sure the command object does not already have
        //            // a notification object associated with it.
        //            command.Notification = null;
        //            if (dependency == null)
        //            {
        //                dependency = new SqlDependency(command);
        //                dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
        //            }

        //            if (connection.State == ConnectionState.Closed)
        //                connection.Open();

        //            using (var reader = command.ExecuteReader())
        //                if (reader.HasRows)
        //                {
        //                    return Json(1, JsonRequestBehavior.AllowGet);
        //                }
        //                else
        //                {
        //                    return Json(0, JsonRequestBehavior.AllowGet);
        //                }
        //        }
        //    }
        //}
        //private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        //{
        //    if (dependency != null)
        //    {
        //        dependency.OnChange -= dependency_OnChange;
        //        dependency = null;
        //    }
        //    if (e.Type == SqlNotificationType.Change)
        //    {
        //        NotifyHub Notify = new NotifyHub();
        //        Notify.Send("Message");
        //    }
        //}     

        readonly string _connString = ServiceUtil.GetClientConnectionString();

        [HttpPost]
        public ActionResult GetAllMessages()
        {
            string database = SessionManager.CoreDatabase;
            //var messages = new List<Messages>();
            using (var connection = new SqlConnection(_connString))
            {
                connection.Open();
                connection.ChangeDatabase(database);
                var queries = new[] { @"SELECT [iSalesUserID] FROM [dbo].[Leads]" };
                using (var command = new SqlCommand(string.Join("; ", queries), connection))
                {
                    command.Notification = null;
                    if (dependency == null)
                    {
                        dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
                    }


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {

                    }
                }
            }
            return Json(1, JsonRequestBehavior.AllowGet);
        }

        private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (dependency != null)
            {
                dependency.OnChange -= dependency_OnChange;
                dependency = null;
            }
            if (e.Type == SqlNotificationType.Change)
            {
                //NotifyHub Notify = new NotifyHub();
                //Notify.Send("Message");
            }
        }
    }
}