using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.SqlClient;
using XIDNA.ViewModels;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using System.Globalization;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using System.Net;
using System.Web.SessionState;

namespace XIDNA.Repository
{
    public class InboxRepository : IInboxRepository
    {
        SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
        SqlCommand cmd = new SqlCommand();
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        CXiAPI oXIAPI = new CXiAPI();
        #region Assign1-Clicks
        public List<VMDropDown> GetOrganizations(string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var orgs = dbContext.Organization.ToList();
            List<VMDropDown> AllOrgs = new List<VMDropDown>();
            AllOrgs = (from c in dbContext.Organization.ToList()
                       select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllOrgs;
        }

        public DTResponse GetUsersList(jQueryDataTableParamModel param, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            IQueryable<VMAspNetRoles> AllReports;
            AllReports = (from c in dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID)
                          join o in dbCore.Organization on c.FKiOrganizationID equals o.ID
                          select new VMAspNetRoles
                          {
                              ID = c.RoleID,
                              RoleName = c.sRoleName,
                              OrgName = o.Name
                          });
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllReports = AllReports.Where(m => m.RoleName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllReports.Count();
            AllReports = QuerableUtil.GetResultsForDataTables(AllReports, "", sortExpression, param);
            var clients = AllReports.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.RoleName, c.OrgName};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public IEnumerable<string[]> GetRolesByOrganization(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            IEnumerable<cXIAppRoles> AllRoles;
            var i = 1;
            AllRoles = dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).OrderBy(m => m.RoleID).ToList();
            var result = from c in AllRoles
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.RoleID), c.sRoleName, OrganizationName(c.FKiOrganizationID)};
            return result;
        }

        private string OrganizationName(int p)
        {
            ModelDbContext dbContext = new ModelDbContext();
            if (p == 0)
            {
                return "Super Admin";
            }
            else
            {
                var organization = dbContext.Organization.Find(p);
                var name = organization.Name;
                return name;
            }

        }

        public int SaveUserQueries(VMUserReports model, string sDatabase, int OrgID)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (model.ID == 0)
            {
                UserReports usermodel = new UserReports();
                usermodel.ReportID = model.ReportID;
                usermodel.RoleID = model.RoleID;
                int OrganizationID = dbCore.XIAppRoles.Where(m => m.RoleID == model.RoleID).Select(m => m.FKiOrganizationID).FirstOrDefault();
                usermodel.OrganizationID = OrganizationID;
                usermodel.Location = model.Location;
                usermodel.Target = model.Target;
                usermodel.FKiApplicationID = model.FKiApplicationID;
                usermodel.StatusTypeID = model.StatusTypeID;
                usermodel.ClassID = model.ClassID;
                if (model.Rank > 0)
                    usermodel.Rank = model.Rank;
                else
                    usermodel.Rank = 0;
                usermodel.Icon = model.Icon;
                usermodel.DisplayAs = model.DisplayAs;
                usermodel.TypeID = model.TypeID;
                usermodel.StatusTypeID = 10;
                usermodel.CreatedByID = usermodel.ModifiedByID = model.RoleID;
                usermodel.CreatedByName = usermodel.ModifiedByName = "User";
                usermodel.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                usermodel.CreatedTime = usermodel.ModifiedTime = DateTime.Now;
                if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString()))
                {
                    usermodel.TargetResultType = model.TargetResultType;
                    usermodel.TargetTemplateID = model.TargetTemplateID;
                }
                else if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.QuickSearch.ToString()))
                {
                    usermodel.BO = model.BO;
                }
                else
                {
                    usermodel.BO = 0;
                    usermodel.TargetResultType = null;
                    usermodel.TargetTemplateID = 0;
                }
                usermodel.FKi1ClickIDXIGUID = model.FKi1ClickIDXIGUID;
                dbContext.UserReports.Add(usermodel);
                dbContext.SaveChanges();
                return usermodel.ID;
            }
            else
            {
                UserReports ur = dbContext.UserReports.Where(x => x.FKi1ClickIDXIGUID == model.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
                ur.ClassID = model.ClassID;
                ur.DisplayAs = model.DisplayAs;
                ur.FKiApplicationID = model.FKiApplicationID;
                ur.ReportID = model.ReportID;
                ur.Target = model.Target;
                if (model.Icon == null)
                    ur.Icon = ur.Icon;
                else
                    ur.Icon = model.Icon;
                ur.Location = model.Location;
                ur.StatusTypeID = model.StatusTypeID;
                if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString()))
                {
                    ur.TargetResultType = model.TargetResultType;
                    ur.TargetTemplateID = model.TargetTemplateID;
                }
                else if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.QuickSearch.ToString()))
                {
                    ur.BO = model.BO;
                }
                else
                {
                    ur.BO = 0;
                    ur.TargetResultType = null;
                    ur.TargetTemplateID = 0;
                }
                ur.ModifiedTime = DateTime.Now;
                dbContext.SaveChanges();
                return model.ID;
            }
        }

        public int Save1Click(VMUserReports model, string sDatabase, int OrgID, int iUserID)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            if (model.ID == 0)
            {
                XIInbox inbox = new XIInbox();
                inbox.FKi1ClickID = model.ReportID;
                inbox.RoleID = model.RoleID;
                int OrganizationID = dbCore.XIAppRoles.Where(m => m.RoleID == model.RoleID).Select(m => m.FKiOrganizationID).FirstOrDefault();
                inbox.OrganizationID = OrganizationID;
                inbox.Location = model.Location;
                inbox.Target = model.Target;
                inbox.FKiApplicationID = model.FKiApplicationID;
                inbox.ClassID = model.ClassID;
                if (model.Rank > 0)
                    inbox.Rank = model.Rank;
                else
                    inbox.Rank = 0;
                inbox.Icon = model.Icon;
                inbox.DisplayAs = model.DisplayAs;
                inbox.TypeID = model.TypeID;
                inbox.bSignalR = model.bSignalR;
                inbox.StatusTypeID = model.StatusTypeID;
                inbox.CreatedByName = inbox.ModifiedByName = model.CreatedByName;
                inbox.CreatedByID = inbox.ModifiedByID = iUserID;
                inbox.CreatedTime = inbox.ModifiedTime = DateTime.Now;
                inbox.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString()))
                {
                    inbox.TargetResultType = model.TargetResultType;
                    inbox.TargetTemplateID = model.TargetTemplateID;
                }
                else if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.QuickSearch.ToString()))
                {
                    inbox.FKiBOID = model.BOID;
                }
                else
                {
                    inbox.FKiBOID = 0;
                    inbox.TargetResultType = null;
                    inbox.TargetTemplateID = 0;
                }
                inbox.FKiXILinkID = model.XILinkID;
                dbContext.XIInbox.Add(inbox);
                dbContext.SaveChanges();
                return inbox.ID;
            }
            else
            {
                XIInbox ur = dbContext.XIInbox.Find(model.ID);
                ur.ClassID = model.ClassID;
                ur.DisplayAs = model.DisplayAs;
                ur.FKiApplicationID = model.FKiApplicationID;
                ur.FKi1ClickID = model.ReportID;
                ur.Target = model.Target;
                if (model.Icon == null)
                    ur.Icon = ur.Icon;
                else
                    ur.Icon = model.Icon;
                ur.Location = model.Location;
                ur.FKiXILinkID = model.XILinkID;
                ur.StatusTypeID = model.StatusTypeID;
                if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString()))
                {
                    ur.TargetResultType = model.TargetResultType;
                    ur.TargetTemplateID = model.TargetTemplateID;
                }
                else if (model.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.QuickSearch.ToString()))
                {
                    ur.FKiBOID = model.BOID;
                }
                else
                {
                    ur.FKiBOID = 0;
                    ur.TargetResultType = null;
                    ur.TargetTemplateID = 0;
                }
                ur.ModifiedTime = DateTime.Now;
                ur.ModifiedByID = iUserID;
                ur.bSignalR = model.bSignalR;
                ur.ModifiedByName = model.ModifiedByName;
                dbContext.SaveChanges();
                return model.ID;
            }
        }
        public VMUserReports EditUserQuery(int ID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            XIInbox UReport = dbContext.XIInbox.Find(ID);
            VMUserReports model = new VMUserReports();
            model.ID = UReport.ID;
            model.FKi1ClickIDXIGUID = UReport.FKi1ClickIDXIGUID;
            model.RoleID = UReport.RoleID;
            model.Icon = UReport.Icon;
            model.TypeID = UReport.TypeID;
            model.FKiApplicationID = UReport.FKiApplicationID;
            model.TargetResultType = UReport.TargetResultType;
            model.Location = UReport.Location;
            model.ClassID = UReport.ClassID;
            model.DisplayAs = UReport.DisplayAs;
            model.ReportID = UReport.FKi1ClickID;
            model.XILinkID = UReport.FKiXILinkID;
            model.BOID = UReport.FKiBOID;
            model.Target = UReport.Target;
            model.bSignalR = UReport.bSignalR;
            model.TargetTemplateID = UReport.TargetTemplateID;
            model.StatusTypeID = (byte)UReport.StatusTypeID;
            model.QueryName = dbContext.Reports.Where(m => m.XIGUID == model.FKi1ClickIDXIGUID).Select(m => m.Name).FirstOrDefault();
            model.UserName = dbCore.XIAppRoles.Where(m => m.RoleID == model.RoleID).Select(m => m.sRoleName).FirstOrDefault();
            var Result = GetAssignDropdowns(OrgID, iUserID, sOrgName, sDatabase);
            model.Locations = Result.Locations;
            model.ReportTypes = Result.ReportTypes;
            model.Classes = Result.Classes;
            model.TemplateList = Result.TemplateList;
            model.QuickSearchBOs = Result.QuickSearchBOs;
            return model;
        }

        public VMUserReports GetAssignDropdowns(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var Masters = dbContext.Types.ToList();
            DataContext Spdb = new DataContext(sOrgDB);
            VMUserReports DropDowns = new VMUserReports();
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            AllClasses = (from c in Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList()
                          select new VMDropDown { text = c.Class, Value = c.ClassID }).ToList();
            List<VMDropDown> AllTypes = new List<VMDropDown>();
            AllTypes = (from c in Masters.Where(m => m.Name == "Report Type" && m.Status == 10).ToList()
                        select new VMDropDown { text = c.Expression, Value = c.TypeID }).ToList();
            List<VMDropDown> AllTemplates = new List<VMDropDown>();
            AllTemplates = (from c in dbContext.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Category == 3).ToList()
                            select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            List<VMDropDown> AllLocations = new List<VMDropDown>();
            AllLocations = (from c in Masters.Where(m => m.Name == "Locations" && m.Status == 10).ToList()
                            select new VMDropDown { text = c.Expression, Value = c.TypeID }).ToList();
            List<VMDropDown> AllQuickBOs = new List<VMDropDown>();
            AllQuickBOs = (from c in Masters.Where(m => m.Name == "QuickSearchBos" && m.Status == 10).ToList()
                           select new VMDropDown { text = c.Expression, Value = c.TypeID }).ToList();
            DropDowns.Classes = AllClasses;
            DropDowns.ReportTypes = AllTypes;
            DropDowns.TemplateList = AllTemplates;
            DropDowns.Locations = AllLocations;
            DropDowns.QuickSearchBOs = AllQuickBOs;
            return DropDowns;
        }

        public int DeleteUserReport(int ReportID, string database)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var RemoveReport = dbContext.XIInbox.Find(ReportID);
            dbContext.XIInbox.Remove(RemoveReport);
            dbContext.SaveChanges();
            return ReportID;
        }

        public DTResponse UserQueriesList(jQueryDataTableParamModel param, int RoleID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            IQueryable<XIInbox> AllReports;
            AllReports = dbContext.XIInbox.Where(m => m.RoleID == RoleID).Where(m => m.FKiApplicationID == FKiAppID || FKiAppID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                List<int> total = dbContext.Reports.Where(m => m.Name.Contains(param.sSearch)).Select(m => m.ID).ToList();
                if (total != null)
                {
                    AllReports = AllReports.Where(m => total.Contains(m.FKi1ClickID));
                }
            }
            int displyCount = 0;
            displyCount = AllReports.Count();
            AllReports = QuerableUtil.GetResultsForDataTables(AllReports, "", sortExpression, param);
            var clients = AllReports.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join r in dbContext.Reports on c.FKi1ClickID equals r.ID
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID),Convert.ToString(c.RoleID), UserName(RoleID, sDatabase), r.Name,((EnumLocations)c.Location).ToString(),((EnumDisplayTypes)c.DisplayAs).ToString(),c.StatusTypeID.ToString(),"Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetClassName(int ClassID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var ClassName = Spdb.OrganizationClasses.Where(m => m.ClassID == ClassID && m.OrganizationID == OrgID).Select(m => m.Class).FirstOrDefault();
            return ClassName;
        }

        string UserName(int RoleID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var rolename = dbCore.XIAppRoles.Where(m => m.RoleID == RoleID).Select(m => m.sRoleName).FirstOrDefault();
            return rolename;
        }
        public List<VMUserReports> GetUserQueryList(int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var settings = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sCol0).FirstOrDefault();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            List<VMUserReports> userqueries = new List<VMUserReports>();
            var AllReports = dbContext.Reports.ToList();
            var AllUserReports = dbContext.UserReports.ToList();
            if (settings == null)
            {
                //var Allroles = GetParentRoles(RoleID);
                List<int> UserReports = AllUserReports.Where(m => m.RoleID == RoleID).OrderBy(m => m.ID).Select(m => m.DisplayAs).ToList();
                UserReports = UserReports.Distinct().ToList();
                foreach (var items in UserReports)
                {
                    int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                    List<UserReports> TypeReports = AllUserReports.Where(m => m.RoleID == RoleID && m.DisplayAs == items && m.Location == DType).ToList();
                    List<string> Names = new List<string>();
                    List<string> Visibility = new List<string>();
                    VMUserReports Report = new VMUserReports();
                    Report.QueryType = ((EnumDisplayTypes)items).ToString();
                    foreach (var item in TypeReports)
                    {
                        var queryname = AllReports.Where(m => m.ID == item.ReportID).Select(m => m.Name).SingleOrDefault();
                        Names.Add(queryname);
                        Visibility.Add("true");
                    }
                    Report.QueryNames = Names;
                    Report.Visibility = Visibility;
                    userqueries.Add(Report);
                }
            }
            else
            {
                var UserSettings = settings.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in UserSettings)
                {
                    if (items.Contains("{"))
                    {
                        List<string> Names = new List<string>();
                        List<string> Visibility = new List<string>();
                        VMUserReports URReport = new VMUserReports();
                        var Row = items.Substring(1, items.Length - 2);
                        var single = Row.Split(',').ToList();
                        foreach (var item in single)
                        {
                            int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                            int ID = Convert.ToInt32(item.Split('-')[0]);
                            UserReports Report = AllUserReports.Where(m => m.RoleID == RoleID && m.Location == DType && m.ReportID == ID).FirstOrDefault();
                            URReport.QueryType = ((EnumDisplayTypes)Report.DisplayAs).ToString();
                            Visibility.Add(item.Split('-')[1]);
                            var queryname = AllReports.Where(m => m.ID == ID).Select(m => m.Name).SingleOrDefault();
                            Names.Add(queryname);
                        }
                        URReport.Visibility = Visibility;
                        URReport.QueryNames = Names;
                        userqueries.Add(URReport);
                    }
                    else
                    {
                        List<string> Names = new List<string>();
                        List<string> Visibility = new List<string>();
                        VMUserReports URReport = new VMUserReports();
                        int ID = Convert.ToInt32(items.Split('-')[0]);
                        int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                        UserReports Report = AllUserReports.Where(m => m.RoleID == RoleID && m.Location == DType && m.ReportID == ID).FirstOrDefault();
                        int IsEsixts = userqueries.Where(m => m.QueryType == ((EnumDisplayTypes)Report.DisplayAs).ToString()).Count();
                        if (IsEsixts > 0)
                        {
                            VMUserReports Rport = userqueries.Where(m => m.QueryType == ((EnumDisplayTypes)Report.DisplayAs).ToString()).FirstOrDefault();
                            var queryname = AllReports.Where(m => m.ID == ID).Select(m => m.Name).SingleOrDefault();
                            Rport.QueryNames.Add(queryname);
                            Rport.Visibility.Add(items.Split('-')[1]);
                        }
                        else
                        {
                            URReport.QueryType = ((EnumDisplayTypes)Report.DisplayAs).ToString();
                            Visibility.Add(items.Split('-')[1]);
                            var queryname = AllReports.Where(m => m.ID == ID).Select(m => m.Name).SingleOrDefault();
                            Names.Add(queryname);
                            URReport.Visibility = Visibility;
                            URReport.QueryNames = Names;
                            userqueries.Add(URReport);
                        }
                    }
                }
            }
            return userqueries;
        }


        public List<VMReports> GetQueriesByID(int iUserID, int OrgID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            //var Allroles = GetParentRoles(RoleID);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var UserInfo = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            DataContext Spdb = new DataContext(sOrgDB);
            List<UserReports> UserReports = new List<UserReports>();
            int IType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString());
            int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
            UserReports = (from c in dbContext.UserReports.Where(m => m.RoleID == RoleID && m.Location != IType && m.Location != DType)
                               //join r in dbContext.Reports on c.ReportID equals r.ID
                           where c.StatusTypeID == 10
                           select c).ToList();

            List<VMReports> Reports = new List<VMReports>();
            Reports = (from d in dbContext.XISignalRUserSettings.Where(r => (r.iUserID == iUserID || r.iUserID == -1 || r.iRoleID == RoleID) && r.iStatus == 10)
                       select new VMReports { ConstantID = d.sConstantID, UserID = d.iUserID, XilinkID = d.FKiXilinkID }).ToList();
            var AllRepots = dbContext.Reports.ToList();
            foreach (var item in UserReports)
            {
                var IsGroupDefined = false;
                if (item.Location == (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.QuickSearch.ToString()))
                {
                    var BO = dbContext.BOs.Find(item.BO);
                    if(BO != null)
                    {
                        var Group = dbContext.BOGroupFields.Where(m => m.BOID == BO.BOID && m.GroupName == "Quick Search").FirstOrDefault();
                        if (Group == null)
                        {
                            IsGroupDefined = false;
                        }
                        else
                        {
                            IsGroupDefined = true;
                        }
                    }                    
                }
                VMReports Report = (from c in AllRepots.Where(m => m.ID == item.ReportID)
                                    join b in dbContext.BOs on c.BOID equals b.BOID
                                    select new VMReports { ID = c.ID, Name = c.Name, ShowAs = c.Title == null ? c.Name : c.Title, Location = ((EnumLocations)item.Location).ToString(), ResultIn = c.ResultIn, BOID = c.BOID, BOName = b.LabelName, IsGroupDefined = IsGroupDefined }
                              ).FirstOrDefault();
                if(Report != null)
                    Reports.Add(Report);
            }
            var Themes = dbContext.Types.Where(m => m.Name == "Themes").ToList();
            Reports.AddRange((from c in Themes select new VMReports { ID = c.ID, Type = c.Expression, Location = "Theme", Name = c.FileName }).ToList());
            var QuickBOs = dbContext.Types.Where(m => m.Name == "QuickSearchBos" && m.FKiApplicationID == UserInfo.FKiApplicationID).ToList();
            if (Reports != null && Reports.Count() > 0)
            {
                Reports.FirstOrDefault().AllBOs = QuickBOs.Select(m => new VMDropDown { Value = m.Value, XIGUID=m.ValueXIGUID, text = m.Expression, Expression = m.Name, Type = m.FileName }).ToList();
            }
            return Reports;
        }

        #endregion Assign1-Clicks

        #region LeftTree
        //public List<Reports> GetLeftMenuTree(int iUserID, int OrgID, string sOrgName, string sDatabase)
        //{
        //    ModelDbContext dbCore = new ModelDbContext(sDatabase);
        //    int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
        //    var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
        //    //var Allroles = GetParentRoles(RoleID);
        //    DataContext Spdb = new DataContext(sOrgDB);
        //    var TotReports = dbContext.Reports.ToList();
        //    List<UserReports> UserReports = new List<UserReports>();
        //    int IType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Inbox.ToString());
        //    UserReports = (from c in dbContext.UserReports.Where(m => m.RoleID == RoleID && m.Location == IType)
        //                   join r in dbContext.Reports on c.ReportID equals r.ID
        //                   where r.StatusTypeID == 10
        //                   select c).ToList();
        //    List<Reports> AllReports = new List<Reports>();
        //    foreach (var items in UserReports)
        //    {
        //        var Rep = TotReports.Where(m => m.ID == items.ReportID).FirstOrDefault();
        //        Rep.Targets = items.Target;
        //        Rep.Type = items.TargetResultType;
        //        if (items.TargetResultType == "HTML")
        //        {
        //            Rep.HTMLCode = Spdb.ContentEditors.Where(m => m.ID == items.TargetTemplateID).Select(m => m.Content).FirstOrDefault();
        //        }
        //        AllReports.Add(Rep);
        //    }
        //    AllReports.Where(c => c.Title == null).ToList().ForEach(cc => cc.Title = cc.Name);
        //    List<Reports> SubReps = new List<Reports>();
        //    foreach (var items in AllReports)
        //    {
        //        var Val = GetInboxCounts(items, sDatabase, iUserID, OrgID);
        //    }
        //    return AllReports;
        //}

        public List<RightMenuTrees> GetLeftMenuTrees(int UserID, int OrgID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<RightMenuTrees> lMenuTree = new List<RightMenuTrees>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            string MainID = dbContext.RightMenuTrees.Where(m => m.ParentID == "#" && m.RoleID == RoleID && (m.RootName.ToLower() == "Zeeinsurance".ToLower() || m.RootName.ToLower() == "Zeeinsurance".ToLower())).Select(m => m.MenuID).FirstOrDefault();
            lMenuTree = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID && m.OrgID == OrgID && m.ParentID == MainID && m.StatusTypeID == 10).OrderBy(m => m.Priority).ToList();
            var Data = Countdata(lMenuTree, sDatabase);
            return lMenuTree;
        }

        public List<RightMenuTrees> Countdata(List<RightMenuTrees> Menus, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            foreach (var items in Menus)
            {
                var ID = items.MenuID;
                items.SubGroups = dbContext.RightMenuTrees.Where(m => m.ParentID == ID && m.StatusTypeID == 10).OrderBy(m => m.Priority).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    Countdata(items.SubGroups, sDatabase);
                }
            }
            return Menus;
        }
        private Reports GetInboxCounts(Reports items, string database, int UserID, int OrgID)
        {
            int Count = GetInboxTargetResult(items, database, UserID, OrgID);
            Reports InboxCount = GetInboxTargetCount(items, Count, UserID);
            items.InboxCount = InboxCount.InboxCount;
            items.Percentage = InboxCount.Percentage;
            if (items.Sub1Clicks.Count() > 0)
            {
                items.Sub1Clicks.ToList().ForEach(m => m.Type = items.Type);
                foreach (var item in items.Sub1Clicks)
                {
                    GetInboxCounts(item, database, UserID, OrgID);
                }
            }
            return items;
        }

        private Reports GetInboxTargetCount(Reports items, int Count, int UserID)
        {
            string InboxCount = "";
            string Percentage = "";
            if (items.Type == "Progress Bar")
            {
                if (Count > 0 && items.Targets > 0)
                {
                    double percentage = (double)Count / items.Targets;
                    InboxCount = Math.Round(percentage * 100, 2).ToString();
                    if (Convert.ToDecimal(InboxCount) > 100)
                    {
                        InboxCount = 100.ToString();
                    }
                }
                else
                {
                    InboxCount = "0";
                }
            }
            else if (items.Type == "Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                }
                else
                {
                    InboxCount = Count + "/0";
                }
            }
            else if (items.Type == "Progress Bar & Number")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
            }
            else if (items.Type == "HTML")
            {
                if (items.Targets > 0)
                {
                    InboxCount = Count + "/" + items.Targets;
                    double percentage = (double)Count / items.Targets;
                    Percentage = Math.Round(percentage * 100, 2).ToString();
                }
                else
                {
                    InboxCount = Count + "/0";
                    Percentage = "0";
                }
                if (items.HTMLCode != null)
                {
                    items.HTMLCode = items.HTMLCode.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "'").Replace("{{Target}}", Percentage);
                }
            }
            items.InboxCount = InboxCount;
            items.Percentage = Percentage;
            return items;
        }

        private int GetInboxTargetResult(Reports items, string database, int UserID, int OrgID)
        {
            var UserDetails = Common.GetUserDetails(UserID, null, database);
            SqlConnection Con = new SqlConnection();
            var Query = items.Query;
            DataTable data = new DataTable();
            string BOName = dbContext.BOs.Where(m => m.BOID == items.BOID).Select(m => m.Name).FirstOrDefault();
            using (Con)
            {
                if (BOName != EnumLeadTables.Reports.ToString())
                {
                    Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Con.Open();
                    Con.ChangeDatabase(UserDetails.sUserDatabase);
                }
                else
                {
                    Con = new SqlConnection(ServiceUtil.GetConnectionString());
                    Con.Open();
                }
                Common Com = new Common();
                Query = ServiceUtil.ReplaceQueryContent(Query, UserID.ToString(), UserID, OrgID, 0, items.BOID);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                data.Load(reader);
                Con.Close();
            }
            return data.Rows.Count;
        }
        #endregion LeftTree

        #region Dashboard
        public VMUserDashContents GetDashBoardInfo(int UserID, string sDatabase, int ClassFilter, int DateFilter)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            //SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString());
            //Con.Open();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Connection = Con;
            //cmd.CommandText = "update reports set Query='select DATENAME(hh, DIMPORTEDON), FKiSourceID, COUNT(*) from leads group by FKiSourceID, DIMPORTEDON order by DATENAME(hh, DIMPORTEDON)' where id=11";
            //cmd.ExecuteNonQuery();
            //Con.Close();
            //string Query = "";
            //int target = 0;
            var val = EnumDisplayTypes.KPICircle.GetTypeCode();
            List<DashboardContent> KPICircles = new List<DashboardContent>();
            List<DashboardContent> KPIPie = new List<DashboardContent>();
            List<DashboardContent> ResultList = new List<DashboardContent>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            List<UserReports> ur = new List<UserReports>();
            VMUserDashContents dashboard = new VMUserDashContents();
            List<string> ReportIDs = new List<string>();
            List<string> DisyplayType = new List<string>();
            List<string> ReportNames = new List<string>();
            List<string> Visibility = new List<string>();
            cXIAppUsers user = dbCore.XIAppUsers.Where(m => m.UserID == UserID).FirstOrDefault();
            var AllReports = dbContext.Reports.ToList();
            List<string> Reports = new List<string>();
            if (user.sCol0 != null)
            {
                Reports = user.sCol0.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            if (user.sCol0 == null)
            {
                int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                List<UserReports> UserReports = (from c in dbContext.UserReports.Where(m => m.RoleID == RoleID).Where(m => m.Location == DType)
                                                 join r in dbContext.Reports on c.ReportID equals r.ID
                                                 where r.StatusTypeID == 10
                                                 select c).ToList();
                foreach (var items in UserReports)
                {
                    ur.Add(items);
                }
                foreach (var items in ur)
                {
                    string Type = ((EnumDisplayTypes)Convert.ToInt32(items.DisplayAs)).ToString();
                    if (Type == EnumDisplayTypes.KPICircle.ToString())
                    {
                        DashboardContent model = new DashboardContent();
                        model.ReportID = items.ReportID;
                        model.Type = ((EnumDisplayTypes)items.DisplayAs).ToString();
                        model.ClassID = ClassFilter;
                        model.dImportedOn = DateFilter;
                        KPICircles.Add(model);
                    }
                    else if (Type == EnumDisplayTypes.PieChart.ToString() || Type == EnumDisplayTypes.BarChart.ToString() || Type == EnumDisplayTypes.LineChart.ToString())
                    {
                        DashboardContent model = new DashboardContent();
                        model.ReportID = items.ReportID;
                        string Name = AllReports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                        model.Type = ((EnumDisplayTypes)items.DisplayAs).ToString();
                        model.ClassID = ClassFilter;
                        model.dImportedOn = DateFilter;
                        model.ReportName = Name;
                        KPIPie.Add(model);
                    }
                    else if (Type == EnumDisplayTypes.ResultList.ToString())
                    {
                        DashboardContent model = new DashboardContent();
                        model.ReportID = items.ReportID;
                        string Name = AllReports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                        model.Type = ((EnumDisplayTypes)items.DisplayAs).ToString();
                        model.ClassID = ClassFilter;
                        model.dImportedOn = DateFilter;
                        model.ReportName = Name;
                        KPIPie.Add(model);
                    }
                }

                dashboard.KPICircle = KPICircles;
                dashboard.KPIPieChart = KPIPie;
                dashboard.ResultList = ResultList;
            }
            else
            {
                List<string> NewReports = new List<string>();
                var circles = Reports.Where(m => m.Contains("{")).Select(m => m).FirstOrDefault();
                circles = circles.Substring(0, circles.Length - 1);
                int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
                List<UserReports> UserReports = dbContext.UserReports.Where(m => m.RoleID == RoleID).Where(m => m.Location == DType).ToList();
                foreach (var items in UserReports)
                {
                    string Type = ((EnumDisplayTypes)items.DisplayAs).ToString();
                    if (Type == EnumDisplayTypes.KPICircle.ToString())
                    {
                        if (circles.Contains(items.ReportID.ToString()))
                        {

                        }
                        else
                        {
                            circles = circles + "," + items.ReportID + "-true";
                        }
                    }
                    else
                    {
                        Boolean isadded = false;
                        foreach (var item in Reports)
                        {
                            if (item.Contains(items.ReportID.ToString()))
                            {
                                isadded = true;
                            }
                        }
                        if (!isadded)
                        {
                            Reports.Add(items.ReportID + "-true");
                        }
                    }
                }
                circles = circles + ",";
                circles = circles.Substring(0, circles.Length - 1) + "}";
                foreach (var items in Reports)
                {
                    if (items.Contains("{"))
                    {
                        NewReports.Add(circles);
                    }
                    else
                    {
                        NewReports.Add(items);
                    }
                }
                foreach (var items in NewReports)
                {
                    string visibility = "";
                    var RID = items;
                    if (items.Contains("{"))
                    {
                        RID = RID.Substring(1, RID.Length - 2);
                        if (RID.Contains(","))
                        {
                            ReportIDs.Add(RID);
                            DisyplayType.Add("KPI Circle");
                            ReportNames.Add("KPI Circle");
                            if (RID.Contains("true"))
                            {
                                Visibility.Add("true");
                            }
                            else
                            {
                                Visibility.Add("false");
                            }
                        }
                        else
                        {
                            ReportIDs.Add(RID);
                            DisyplayType.Add("KPI Circle");
                            ReportNames.Add("KPI Circle");
                            if (RID.Contains("true"))
                            {
                                Visibility.Add("true");
                            }
                            else
                            {
                                Visibility.Add("false");
                            }
                        }
                    }
                    else
                    {
                        RID = items.Split('-')[0];
                        visibility = items.Split('-')[1];
                        int ReportID = Convert.ToInt32(RID);
                        var Report = AllReports.Where(m => m.ID == ReportID).FirstOrDefault();
                        ReportIDs.Add(RID);
                        DisyplayType.Add(((EnumDisplayTypes)Report.DisplayAs).ToString());
                        ReportNames.Add(Report.Name);
                        Visibility.Add(visibility);
                    }
                }
            }
            dashboard.UserID = UserID;
            UserSettings settings = new UserSettings();
            settings.ReportIDs = ReportIDs;
            settings.DisplayType = DisyplayType;
            settings.ReportNames = ReportNames;
            settings.Visibility = Visibility;
            settings.Col0 = user.sCol0;
            dashboard.UserSettings = settings;
            return dashboard;
        }

        public List<VMKPIResult> GetKPICircleResult(string ReportID, int UserID, string sDatabase, int OrgID, int ClassFilter, int DateFilter, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string Query = "";
            int target = 0, com;
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            List<UserReports> Reports = new List<UserReports>();
            List<string> Vistiblity = new List<string>();
            int DisplayType = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString());
            int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
            if (ReportID != "" && ReportID != null)
            {
                var IDs = ReportID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var id in IDs)
                {
                    int ID = Convert.ToInt32(id.Split('-')[0]);
                    UserReports userreport = dbContext.UserReports.Where(m => m.RoleID == UserRoleID && m.ReportID == ID && m.Location == DType && m.DisplayAs == DisplayType).FirstOrDefault();
                    Reports.Add(userreport);
                    Vistiblity.Add(id.Split('-')[1]);
                }
            }
            else
            {
                List<UserReports> AllReports = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.Location == DType).Where(m => m.DisplayAs == DisplayType).ToList();
                foreach (var item in AllReports)
                {
                    Reports.Add(item);
                    Vistiblity.Add("true");
                }
            }
            List<VMKPIResult> KPIs = new List<VMKPIResult>();
            KPICircleColors colors = new KPICircleColors();
            KPIIconColors iconscolor = new KPIIconColors();
            List<string> color = new List<string>();
            List<string> iconcolor = new List<string>();
            Common Com = new Common();
            foreach (var items in colors)
            {
                string str = Convert.ToString(items.KPIColor);
                color.Add(str);
            }
            foreach (var items in iconscolor)
            {
                string str = Convert.ToString(items.KPIColor);
                iconcolor.Add(str);
            }
            int j = 0;
            var TotReports = dbContext.Reports.ToList();
            var AllUserReports = dbContext.UserReports.ToList();
            foreach (var items in Reports)
            {
                Reports report = TotReports.Where(m => m.ID == items.ReportID).FirstOrDefault();
                Query = report.Query;
                UserReports ureport = AllUserReports.Where(m => m.RoleID == items.RoleID).Where(m => m.ReportID == items.ReportID).FirstOrDefault();
                target = ureport.Target;
                string UserIDs = Com.GetSubUsers(UserID, OrgID, sDatabase, sOrgName);
                if (Query != null && Query.Length > 0)
                {
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrgID, 0, 0);
                    if (ClassFilter != 0 || DateFilter != 0)
                    {
                        Query = ServiceUtil.ModifyQuery(Query, OrgID, UserIDs, ClassFilter, DateFilter);
                    }
                    var Location = dbCore.XIAppUsers.Find(UserID);
                    string BOName = dbContext.BOs.Where(m => m.BOID == report.BOID).Select(m => m.Name).FirstOrDefault();
                    if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                        var LocCondition = "";
                        var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var Loc in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + OrgID + "_" + Loc + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                        var LocCondition = "OrgHeirarchyID Like 'ORG" + OrgID + "_%'";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    VMKPIResult kpi = new VMKPIResult();
                    if (TotalResult.Count() == 0)
                    {
                        com = 0;
                    }
                    else
                    {
                        com = Convert.ToInt32(TotalResult[0][0]);
                    }
                    Con.Close();
                    double percentage = (double)com / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    if (target == 0)
                    {
                        completed = 0;
                    }
                    kpi.Name = report.Name;
                    kpi.ShowAs = report.Title;
                    if (ReportID != null)
                    {
                        kpi.Visibility = Vistiblity[j];
                    }
                    else
                    {
                        kpi.Visibility = "true";
                    }
                    kpi.ReportID = report.ID;
                    kpi.InnerReportID = report.InnerReportID;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = ureport.Icon;
                    KPIs.Add(kpi);
                    j++;
                }
            }
            return KPIs;
        }

        public GraphData GetPieChart(VMChart Chart, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            var sOrgDB = Common.GetUserDetails(Chart.UserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
            GraphData GraphData = new GraphData();
            List<DashBoardGraphs> PieData = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            Reports model = dbContext.Reports.Where(x => x.XIGUID == Chart.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
            string Type = ((EnumDisplayTypes)model.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.PieChart.ToString())
            {
                string Query = "";
                if (Chart.Query != null && Chart.Query.Length > 0)
                {
                    Query = Chart.Query;
                }
                else
                {
                    Query = model.Query;
                }
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                }
                var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                string BOName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
                if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "";
                    if (Location.sLocation != null)
                    {
                        var Locs = Location.sLocation.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + items + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                }
                else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                Con.Open();
                Con.ChangeDatabase(Chart.Database);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            values.Add("0");
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
                int j = 0;
                var Keys = ServiceUtil.GetForeginkeyValues(Query);
                if (reader.HasRows == true)
                {
                    foreach (var items1 in results)
                    {
                        for (int i = 0; i < items1.Count(); i++)
                        {
                            DashBoardGraphs Values = new DashBoardGraphs();
                            //int ID = Convert.ToInt32(items1[0]);
                            var Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items1[0], Chart.Database);
                            if (Name != null)
                            {
                                Values.label = Name;
                                Values.value = Convert.ToInt32(items1[1]);
                                PieData.Add(Values);
                            }
                        }
                        j++;
                    }
                }
                else
                {
                    DashBoardGraphs model1 = new DashBoardGraphs();
                    PieData.Add(model1);
                }
                Con.Close();
                GraphData.ReportID = Chart.ReportID;
                GraphData.PieData = PieData;
                GraphData.QueryName = model.Name;
                GraphData.ShowAs = model.Title;
                if (model.IsColumnClick)
                {
                    GraphData.IsColumnClick = true;
                    GraphData.OnClickColumn = model.OnClickColumn;
                    GraphData.OnClickResultID = model.OnColumnClickValue;
                }
            }
            return GraphData;
        }

        public LineGraph GetBarChart(VMChart Chart, string sDatabase, string sOrgName)
        {
            var sOrgDB = Common.GetUserDetails(Chart.UserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            DataContext Spdb = new DataContext(sOrgDB);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            Reports model = dbContext.Reports.Where(x => x.XIGUID == Chart.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
            string Query = "";
            List<VMDropDown> Keys = new List<VMDropDown>();
            string Type = ((EnumDisplayTypes)model.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.BarChart.ToString())
            {
                if (Chart.Query != null && Chart.Query.Length > 0)
                {
                    Query = Chart.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(Query);
                }
                else
                {
                    Query = model.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                }
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                }
                var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                string BOName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
                if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "";
                    var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in Locs)
                    {
                        LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + items + "' or ";
                    }
                    LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                    LocCondition = "(" + LocCondition + ")";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                Con.Open();
                Con.ChangeDatabase(Chart.Database);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();

                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            values.Add("0");
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
            }
            List<List<string>> Graph = new List<List<string>>();
            List<string> XValues = new List<string>();
            List<string> xval = new List<string>();
            XValues.Add("x");
            if (Keys.Count() > 1)
            {
                string Name = "";
                xval = results.Select(m => m[0]).Distinct().ToList();
                foreach (var items in xval)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items, Chart.Database);
                    XValues.Add(Name);
                }
            }
            else
            {
                xval = results.Select(m => m[0]).Distinct().ToList();
                XValues.AddRange(xval);
            }
            Graph.Add(XValues);
            var types = results.Select(m => m[1]).Distinct();
            foreach (var type in types)
            {
                List<string> Y = new List<string>();
                //var ID = Convert.ToInt32(type);
                string Name = "";
                if (Keys.Count() > 1)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, Chart.Database);
                }
                else
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, Chart.Database);
                }
                if (Name != null)
                {
                    Y = new List<string> { Name };
                    foreach (var xaxis in xval)
                    {
                        var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2].ToString()).FirstOrDefault();
                        Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                    }
                    Graph.Add(Y);
                }
            }
            LineGraph line = new LineGraph();
            line.Data = Graph;
            line.QueryName = model.Name;
            line.ShowAs = model.Title;
            if (model.IsColumnClick)
            {
                line.IsColumnClick = model.IsColumnClick;
                line.OnClickColumn = model.OnClickColumn;
                line.OnClickResultID = model.OnColumnClickValue;
                line.OnClickParameter = model.OnClickParameter;
                line.OnClickCell = model.OnClickCell;
            }
            return line;
        }

        public LineGraph GetLineChart(VMChart Chart, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            //VMForeginKeys Keys = new VMForeginKeys();
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            var sOrgDB = Common.GetUserDetails(Chart.UserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            string Query = "", QueryName = "";
            List<string[]> results = new List<string[]>();
            Common Com = new Common();
            Reports report = dbContext.Reports.Where(x => x.XIGUID == Chart.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
            List<VMDropDown> Keys = new List<VMDropDown>();
            string Type = ((EnumDisplayTypes)report.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.LineChart.ToString())
            {
                if (Chart.Query != null && Chart.Query.Length > 0)
                {
                    Query = Chart.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(Query);
                }
                else
                {
                    Query = report.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(report.Query);
                }
                string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
                if (Query != null && Query.Length > 0)
                {
                    //Send Query For Modification
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                    if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                    {
                        Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                    }
                    var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                    string BOName = dbContext.BOs.Where(m => m.BOID == report.BOID).Select(m => m.Name).FirstOrDefault();
                    if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "";
                        var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + items + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            if (!reader.IsDBNull(i))
                            {
                                values.Add(reader.GetValue(i).ToString());
                            }
                            else
                            {
                                values.Add("0");
                            }
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    if (reader.HasRows == false)
                    {
                        //results.Add("");
                    }
                    Con.Close();
                }
                QueryName = report.Name;
            }

            List<List<string>> Graph = new List<List<string>>();
            List<string> XValues = new List<string>();
            List<string> xval = new List<string>();
            XValues.Add("x");
            if (Keys.Count() > 1)
            {
                string Name = "";
                xval = results.Select(m => m[0]).Distinct().ToList();
                foreach (var items in xval)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items, Chart.Database);
                    XValues.Add(Name);
                }
            }
            else
            {
                xval = results.Select(m => m[0]).Distinct().ToList();
                XValues.AddRange(xval);
            }
            Graph.Add(XValues);
            var types = results.Select(m => m[1]).Distinct();
            foreach (var type in types)
            {
                List<string> Y = new List<string>();
                //var ID = Convert.ToInt32(type);
                string Name = "";
                if (Keys.Count() > 1)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, Chart.Database);
                }
                else
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, Chart.Database);
                }

                if (Name != null)
                {
                    Y = new List<string> { Name };
                    foreach (var xaxis in xval)
                    {
                        var grouped = results.Where(m => m[0] == type && m[0] == xaxis).GroupBy(x => x[0])
                     .Select(g => new
                     {
                         Name = g.Key,
                         Sum = g.Sum(x => int.Parse(x[2]))
                     });
                        var Valuess = grouped.Select(m => m.Sum).ToList();
                        int Valuessssss = 0;
                        if (Valuess.Count() > 0)
                        {
                            Valuessssss = Valuess[0];
                        }
                        else
                        {
                            Valuessssss = 0;
                        }
                        //var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2]).ToString();
                        Y.Add(string.IsNullOrWhiteSpace(Valuessssss.ToString()) ? "0" : Valuessssss.ToString());
                    }
                    Graph.Add(Y);
                }
            }
            LineGraph line = new LineGraph();
            line.Data = Graph;
            line.QueryName = QueryName;
            line.InnerReportID = report.InnerReportID;
            line.ShowAs = report.Title;
            line.FKi1ClickIDXIGUID = report.XIGUID;
            if (report.IsColumnClick)
            {
                line.IsColumnClick = report.IsColumnClick;
                line.OnClickColumn = report.OnClickColumn;
                line.OnClickResultID = report.OnColumnClickValue;
                line.OnClickParameter = report.OnClickParameter;
                line.OnClickCell = report.OnClickCell;
            }
            return line;
        }

        public List<DashBoardGraphs> GetLeadsByClass(int iUserID, string sOrgName, string sDatabase, int OrganizationID, int ClassFilter, int DateFilter)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(iUserID, OrganizationID, sDatabase, sOrgName);
            List<UserReports> ids = new List<UserReports>();
            int DisplayType = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString());
            int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
            List<UserReports> AllReports = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.Location == DType).Where(m => m.DisplayAs == DisplayType).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            //List<int> ids = dbContext.UserReports.Where(m => m.RoleID == UserID).Where(m => m.Location == "Dashboard").Where(m => m.DisplayAs == "KPI Pie Chart").Select(m => m.ReportID).ToList();
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            string[] colors = { "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277", "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277" };
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Where(x => x.XIGUID == items.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
                string Query = model.Query;
                //Send Query For Modification
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, iUserID, OrganizationID, 0, 0);
                if (ClassFilter != 0 || DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, OrganizationID, UserIDs, ClassFilter, DateFilter);
                }
                Con.Open();
                Con.ChangeDatabase(sDatabase);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();

                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        values.Add(reader.GetValue(i).ToString());
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
                int j = 0;
                if (reader.HasRows == true)
                {
                    foreach (var items1 in results)
                    {
                        for (int i = 0; i < items1.Count(); i++)
                        {
                            DashBoardGraphs model1 = new DashBoardGraphs();
                            model1.value = Convert.ToInt32(items1[i]);
                            model1.label = "Class" + j;
                            model1.color = colors[j];
                            model1.highlight = "#1ab394";
                            list.Add(model1);

                        }
                        j++;
                    }
                }
                //else if (reader.HasRows == false)
                //{

                //    DashBoardGraphs model1 = new DashBoardGraphs();
                //    model1.value = 0;
                //    model1.label = "Class" + j;
                //    model1.color = colors[j];
                //    model1.highlight = "#1ab394";
                //    list.Add(model1);

                //}
                Con.Close();
            }

            return list;
        }

        public List<DashBoardGraphs> GetLeadsByClassForTab(int UserID, string database, int OrganizationID, string ReportID, string sOrgName)
        {
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(UserID, OrganizationID, database, sOrgName);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = new List<Reports>();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ReportID, out iQueryID);
            Guid.TryParse(ReportID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                AllReports = dbContext.Reports.Where(m => m.ID == iQueryID).ToList();
                Common.SaveErrorLog("Error at InboxRepository: GetLeadsByClassForTab: " + iQueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                AllReports = dbContext.Reports.Where(m => m.XIGUID == QueryIDGUID).ToList();
            }
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            //List<int> ids = dbContext.UserReports.Where(m => m.RoleID == UserID).Where(m => m.Location == "Dashboard").Where(m => m.DisplayAs == "KPI Pie Chart").Select(m => m.ReportID).ToList();
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            string[] colors = { "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277", "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277" };
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Where(m => m.XIGUID == items.XIGUID).Select(t => t).FirstOrDefault();
                string Query = model.Query;
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrganizationID, 0, 0);
                Con.Open();
                Con.ChangeDatabase(database);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();

                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        values.Add(reader.GetValue(i).ToString());
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
                Con.Close();
            }
            int j = 0;
            foreach (var items in results)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    DashBoardGraphs model = new DashBoardGraphs();
                    model.value = Convert.ToInt32(items[i]);
                    model.label = "Class" + j;
                    model.color = colors[j];
                    model.highlight = "#1ab394";
                    list.Add(model);
                }
                j++;
            }
            return list;
        }
        public List<DashBoardGraphs> GetLeadsCount(int UserID, string Database)
        {
            KPIIcon icons = new KPIIcon();
            List<string> icon = new List<string>();
            foreach (var item in icons)
            {
                string str = Convert.ToString(item.KPIColor);
                icon.Add(str);
            }
            string[] leadcolors = { "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277" };
            string[] LeadNames = { "New Imported Leads", "Quoted Leads", "Quoted Reviews", "Last Year Diary", "Admin Leads", "Leads Last Month" };
            List<DashBoardGraphs> AllLeads = new List<DashBoardGraphs>();
            for (int i = 0; i < 6; i++)
            {
                DashBoardGraphs graph = new DashBoardGraphs();
                graph.color = "headicon  cl" + Convert.ToInt32(i + 1);
                graph.highlight = icon[i];
                graph.value = i * 10;
                graph.label = LeadNames[i];
                AllLeads.Add(graph);
            }
            return AllLeads;
        }

        public int SaveColSettings(string[] ColOrder, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string RowIDs = "";
            cXIAppUsers model = dbCore.XIAppUsers.Where(m => m.UserID == UserID).FirstOrDefault();
            for (int i = 0; i < ColOrder.Count(); i++)
            {
                RowIDs = RowIDs + ColOrder[i] + ", ";
            }
            RowIDs = RowIDs.Substring(0, RowIDs.Length - 2);
            model.sCol0 = RowIDs;
            dbContext.SaveChanges();
            return model.UserID;
        }
        public cXIAppUsers RemoveDashboardGraph(string Type, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            cXIAppUsers model = dbCore.XIAppUsers.Where(m => m.UserID == UserID).FirstOrDefault();
            //var rowid = row.Substring(0, 4);
            //if (rowid == "Row1")
            //{
            //    model.Row1 = null;
            //    if (model.Col0.IndexOf(rowid) == 0)
            //    {
            //        model.Col0 = model.Col0.Replace(rowid, "");
            //    }
            //    else
            //    {
            //        model.Col0 = model.Col0.Replace(", " + rowid, "");
            //    }
            //}
            //else if (rowid == "Row2")
            //{
            //    if (model.Row2.IndexOf(row) == 0)
            //    {
            //        if (model.Row2.IndexOf(row) >= 0)
            //        {
            //            model.Row2 = model.Row2.Replace(row, "");
            //        }
            //    }
            //    else
            //    {
            //        model.Row2 = model.Row2.Replace(", " + row, "");
            //    }
            //}
            //else if (rowid == "Row3")
            //{
            //    if (model.Row3.IndexOf(row) == 0)
            //    {
            //        if (model.Row3.IndexOf(row) >= 0)
            //        {
            //            model.Row3 = model.Row3.Replace(row, "");
            //        }
            //    }
            //    else
            //    {
            //        model.Row3 = model.Row3.Replace(", " + row, "");
            //    }
            //}
            //else if (rowid == "Row4")
            //{
            //    if (model.Row4.IndexOf(row) == 0)
            //    {
            //        if (model.Row4.IndexOf(row) >= 0)
            //        {
            //            model.Row4 = model.Row4.Replace(row, "");
            //        }
            //    }
            //    else
            //    {
            //        model.Row4 = model.Row4.Replace(", " + row, "");
            //    }
            //}
            //else if (rowid == "Row5")
            //{
            //    if (model.Row5.IndexOf(row) == 0)
            //    {
            //        if (model.Row5.IndexOf(row) >= 0)
            //        {
            //            model.Row5 = model.Row5.Replace(row, "");
            //        }
            //    }
            //    else
            //    {
            //        model.Row5 = model.Row5.Replace(", " + row, "");
            //    }
            //}
            //dbContext.SaveChanges();
            //if (model.Col0.IndexOf(rowid) == 0)
            //{
            //    if (model.Col0.IndexOf(rowid) == 0 && model.Row2 == "")
            //    {
            //        model.Col0 = model.Col0.Replace(rowid, "");
            //    }
            //    else if (model.Col0.IndexOf(rowid) == 0 && model.Row3 == "")
            //    {
            //        model.Col0 = model.Col0.Replace(rowid, "");
            //    }
            //    else if (model.Col0.IndexOf(rowid) == 0 && model.Row4 == "")
            //    {
            //        model.Col0 = model.Col0.Replace(rowid, "");
            //    }
            //    else if (model.Col0.IndexOf(row) == 0 && model.Row5 == "")
            //    {
            //        model.Col0 = model.Col0.Replace(rowid, "");
            //    }
            //}
            //if (model.Row2 == "")
            //{
            //    model.Row2 = null;
            //    model.Col0 = model.Col0.Replace(", " + rowid, "");
            //}
            //else if (model.Row3 == "")
            //{
            //    model.Row3 = null;
            //    model.Col0 = model.Col0.Replace(", " + rowid, "");
            //}
            //else if (model.Row4 == "")
            //{
            //    model.Row4 = null;
            //    model.Col0 = model.Col0.Replace(", " + rowid, "");
            //}
            //else if (model.Row5 == "")
            //{
            //    model.Row5 = null;
            //    model.Col0 = model.Col0.Replace(", " + rowid, "");
            //}
            //dbContext.SaveChanges();
            return model;
        }
        //public int GetDefault(int UserID)
        //{
        //    ModeldbContext dbContext = new ModeldbContext();
        //    AspNetUsers model = dbContext.AspNetUsers.Where(m => m.Id == UserID).FirstOrDefault();
        //    model.Col0 = null;
        //    model.Row1 = null;
        //    model.Row2 = null;
        //    model.Row3 = null;
        //    model.Row4 = null;
        //    model.Row5 = null;
        //    dbContext.SaveChanges();
        //    return model.Id;
        //}

        public cXIAppUsers GetDefaultSettings(int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            cXIAppUsers model = dbCore.XIAppUsers.Where(m => m.UserID == UserID).FirstOrDefault();
            model.sCol0 = null;
            model.sRow1 = null;
            model.sRow2 = null;
            model.sRow3 = null;
            model.sRow4 = null;
            model.sRow5 = null;
            dbContext.SaveChanges();
            return model;
        }
        #endregion Dashboard

        #region ReportResults

        public VMResultList RunUserQuery(VMRunUserQuery model, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList Result = new VMResultList();
            List<string> AllHeadings = new List<string>();
            Reports query = dbContext.Reports.Where(x => x.XIGUID == model.FKi1ClickIDXIGUID).FirstOrDefault();
            try
            {
                int PageSize = 30;
                int skip = 0;
                if (model.PageIndex == 2)
                {
                    skip = 30;
                    PageSize = 10;
                }
                else if (model.PageIndex >= 2)
                {
                    skip = PageSize + 10 * (model.PageIndex - 2);
                    PageSize = 10;
                }
                if (skip == 0)
                {
                    PageSize = 30;
                }
                //DataContext Spdb = new DataContext(database);
                var sOrgDB = Common.GetUserDetails(model.UserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                int BOID = query.BOID;
                Common Com = new Common();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> Headings = query.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> Formatting = new List<string>();
                List<int> Targets = new List<int>();
                List<string> MouseOverColumns = new List<string>();
                var Heads = GetHeadings(model.FKi1ClickIDXIGUID.ToString(), model.database, model.OrgID, model.UserID, sOrgName);
                Formatting = Heads.Formats;
                Targets = Heads.Targets;
                MouseOverColumns = Heads.MouseOverColumns;
                string NewQuery = Heads.Query;
                AllHeadings = Heads.Headings;
                List<VMDropDown> KeyPositions = Heads.FKPositions;
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == model.UserID).Select(m => m.RoleID).FirstOrDefault();
                string UserIDs = Com.GetSubUsers(model.UserID, model.OrgID, sDatabase, sOrgName);
                string Query = ServiceUtil.ReplaceQueryContent(NewQuery, UserIDs, model.UserID, model.OrgID, 0, 0);
                if (model.ClassFilter != 0 || model.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, model.OrgID, UserIDs, model.ClassFilter, model.DateFilter);
                }
                if (model.SearchText != null && model.SearchText.Length > 0)
                {
                    string NewSearchText = GetSearchString(model.SearchText, model.ReportID, model.OrgID, model.SearchType, model.UserID, sOrgName, model.database);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (model.SearchType == "FilterSearch")
                {
                    if (model.Fields != null && model.Fields.Length > 0)
                    {
                        var Condition = ServiceUtil.GetDynamicSearchStrings(model.Fields, model.Optrs, model.Values);
                        if (Condition.Length > 0)
                        {
                            Query = ServiceUtil.AddSearchParameters(Query, Condition);
                        }
                    }
                }
                if (model.SearchType == "NaturalSearch")
                {
                    string NewSearchText = GetSearchString(model.SearchText, model.ReportID, model.OrgID, model.SearchType, model.UserID, sOrgName, model.database);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                SqlConnection Con = new SqlConnection();
                List<string[]> results = new List<string[]>();
                string BOName = dbContext.BOs.Where(m => m.BOID == query.BOID).Select(m => m.Name).FirstOrDefault();
                using (Con)
                {
                    if (BOName != EnumLeadTables.Reports.ToString())
                    {
                        Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                        Con.Open();
                        Con.ChangeDatabase(model.database);
                    }
                    else
                    {
                        Con = new SqlConnection(ServiceUtil.GetConnectionString());
                        Con.Open();
                    }
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    List<object[]> Res = new List<object[]>();
                    if (query.ResultListDisplayType == 0)
                    {
                        Res = TotalResult.Skip(skip).Take(PageSize).ToList();
                    }
                    var Codings = Spdb.HTMLColorCodings.Where(m => m.OrganizationID == model.OrgID).ToList();
                    for (int i = 0; i < Res.Count(); i++)
                    {
                        List<string> NewRes = new List<string>();
                        for (int j = 0; j < Res[i].Count(); j++)
                        {
                            var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                            if (j == 1)
                            {
                                NewRes.Add("");
                            }
                            else if (pos != null)
                            {
                                var DbValue = Res[i][j];
                                if (DbValue != null)
                                {
                                    var Value = ServiceUtil.ReplaceForeginKeyValues(pos, Res[i][j].ToString(), model.database);
                                    NewRes.Add(Value);
                                }
                                else
                                {
                                    NewRes.Add(null);
                                }
                            }
                            else
                            {
                                if (Formatting[j] != null)
                                {
                                    if (Formatting[j] == "%")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]));
                                        }

                                    }
                                    else if (Formatting[j] == "en-GB")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            if (Convert.ToUInt32(Res[i][j]) > Targets[j])
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetgreencolor'></span>");
                                            }
                                            else
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetredcolor'></span>");
                                            }
                                        }
                                        else
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            NewRes.Add(totalValueCurrency);
                                        }
                                    }
                                    else
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]));
                                        }
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                        }
                        results.Add(NewRes.ToArray());
                    }
                    Con.Close();
                }
                if (model.SearchText != null && model.SearchType == "Quick")
                {
                    foreach (var item in results)//Highlight the SearchedText 
                    {
                        for (int i = 0; i <= item.Length - 1; i++)
                        {
                            if (item[i].IndexOf(model.SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                            {
                                string input = item[i];
                                string pattern = model.SearchText;
                                string replacement = string.Format("<strong style='color:#d0592e !important;'>{0}</strong>", "$0");
                                var result = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
                                item[i] = result;
                            }
                        }
                    }
                }
                Result.IsPopup = query.IsRowClick;
                Result.IsExport = query.IsExport;
                Result.ActionType = query.OnRowClickType;
                Result.ActionReportID = query.OnRowClickValue;
                Result.Headings = AllHeadings;
                Result.Rows = results;
                Result.QueryName = query.Name;
                Result.QueryID = model.ReportID;
                Result.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == model.ReportID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                //vmquery.ClassID = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
                Result.QueryName = query.Name;
                Result.ResultListDisplayType = query.ResultListDisplayType;
                var FilterGroup = dbContext.BOGroupFields.Where(m => m.BOID == BOID && m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault();
                var FilterFields = new List<string>();
                if (FilterGroup != null)
                {
                    FilterFields = FilterGroup.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
                List<string> FilterFileds = new List<string>();
                Dictionary<string, string> myDict = new Dictionary<string, string>();
                foreach (var items in FilterFields)
                {
                    myDict[BOFields.Where(m => m.Name == items).Select(m => m.LabelName).FirstOrDefault()] = items;
                }
                Result.FilterGroup = myDict;
                Result.IsFilterSearch = query.IsFilterSearch;
                Result.IsNaturalSearch = query.IsNaturalSearch;
                Result.HeadingReports = new List<int>();
                Result.MouseOverColumns = MouseOverColumns;
                Result.SingleBOField = Heads.SingleBOField;
                Result.SrchFCount = Heads.SrchFCount;
                return Result;
            }
            catch (Exception ex)
            {
                Result.Headings = AllHeadings;
                Result.Rows = new List<string[]>();
                Result.QueryName = query.Name;
                Result.QueryID = model.ReportID;
                //vmquery.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == QueryID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).SingleOrDefault();
                //vmquery.ClassID = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
                Result.QueryName = query.Name;
                Result.ResultListDisplayType = query.ResultListDisplayType;
                return Result;
            }

        }

        public VMResultList QuickSearch(VMQuickSearch Search, string sDatabase, string sOrgName)
        {
            int PageSize = 30;
            int skip = 0;
            if (Search.PageIndex == 2)
            {
                skip = 30;
                PageSize = 10;
            }
            else if (Search.PageIndex >= 2)
            {
                skip = PageSize + 10 * (Search.PageIndex - 2);
                PageSize = 10;
            }
            if (skip == 0)
            {
                PageSize = 30;
            }
            //DataContext Spdb = new DataContext(database);
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList Result = new VMResultList();
            var sOrgDB = Common.GetUserDetails(Search.UserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reports query = dbContext.Reports.Find(Search.ReportID);
            int BOID = query.BOID;
            Common Com = new Common();
            var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = query.Query.Substring(0, FromIndex);
            var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
            List<string> AllHeadings = new List<string>();
            List<string> Headings = new List<string>();
            var Heads = GetHeadings(Search.FKi1ClickIDXIGUID.ToString(), Search.database, Search.OrgID, Search.UserID, sOrgName);
            List<VMDropDown> KeyPositions = Heads.FKPositions;
            string NewQuery = Heads.Query;
            AllHeadings = Heads.Headings;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Search.UserID).Select(m => m.RoleID).FirstOrDefault();
            string UserIDs = Com.GetSubUsers(Search.UserID, Search.OrgID, sDatabase, sOrgName);
            string Query = ServiceUtil.ReplaceQueryContent(NewQuery, UserIDs, Search.UserID, Search.OrgID, 0, 0);
            if (Search.SearchText.Length > 0)
            {
                Query = ServiceUtil.AddSearchParameters(Query, Search.SearchText);
            }
            Con.Open();
            Con.ChangeDatabase(Search.database);
            cmd.Connection = Con;
            cmd.CommandText = Query;
            SqlDataReader reader = cmd.ExecuteReader();
            DataTable data = new DataTable();
            data.Load(reader);
            List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
            List<object[]> Res = new List<object[]>();
            if (query.ResultListDisplayType == 1)
            {
                Res = TotalResult.ToList();
            }
            List<string[]> results = new List<string[]>();
            for (int i = 0; i < Res.Count(); i++)
            {
                List<string> NewRes = new List<string>();
                for (int j = 0; j < Res[i].Count(); j++)
                {
                    var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                    if (pos != null)
                    {
                        var DbValue = Res[i][j];
                        if (DbValue != null)
                        {
                            var Value = ServiceUtil.ReplaceForeginKeyValues(pos, Res[i][j].ToString(), Search.database);
                            NewRes.Add(Value);
                        }
                        else
                        {
                            NewRes.Add(null);
                        }
                    }
                    else
                    {
                        NewRes.Add(Res[i][j].ToString());
                    }
                }
                results.Add(NewRes.ToArray());
            }
            Con.Close();
            if (query.ActionFields != null)
            {
                Result.IsPopup = true;
            }
            else
            {
                Result.IsPopup = false;
            }
            Result.Headings = AllHeadings;
            if (query.ResultListDisplayType == 0)
            {
                Result.Rows = results.Skip(skip).Take(PageSize).ToList();
            }
            else
            {
                Result.Rows = results;
            }
            Result.QueryName = query.Name;
            Result.ShowAs = query.Title;
            Result.QueryID = Search.ReportID;
            Result.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == Search.ReportID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).SingleOrDefault();
            //vmquery.ClassID = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
            Result.QueryName = query.Name;
            Result.ResultListDisplayType = query.ResultListDisplayType;
            return Result;
        }
        public DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch Search, string sDatabase, int iUserID, string sCurrentGuestUser, string sOrgName)
        {
            //button1_Click();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<string> AllHeadings = new List<string>();
            try
            {
                //DataContext Spdb = new DataContext(database);
                VMResultList vmquery = new VMResultList();
                //var AllLeads = 
                var UserDetails = Common.GetUserDetails(Search.UserID, sOrgName, sDatabase);
                var sOrgDB = UserDetails.sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                Reports query = dbContext.Reports.Where(x => x.XIGUID == Search.FKi1ClickIDXIGUID).Select(t => t).FirstOrDefault();
                int BOID = query.BOID;
                Common Com = new Common();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<string> Scripts = new List<string>();
                List<int> Targets = new List<int>();
                var Heads = GetHeadings(Search.FKi1ClickIDXIGUID.ToString(), Search.database, Search.OrgID, Search.UserID, sOrgName);
                List<VMDropDown> KeyPositions = Heads.FKPositions;
                Headings = Heads.Headings;
                Formatting = Heads.Formats;
                Targets = Heads.Targets;
                Scripts = Heads.Scripts;
                string NewQuery = Heads.Query;
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Search.UserID).Select(m => m.RoleID).FirstOrDefault();
                string UserIDs = Com.GetSubUsers(Search.UserID, Search.OrgID, sDatabase, sOrgName);
                string Query = ServiceUtil.ReplaceQueryContent(NewQuery, UserIDs, Search.UserID, Search.OrgID, 0, 0);
                Query = ServiceUtil.ReplaceGuestUser(Query, sCurrentGuestUser);
                if (Search.SearchType == "FilterSearch")
                {
                    if (param.Fields != null && param.Fields.Length > 0)
                    {
                        var Condition = ServiceUtil.GetDynamicSearchStrings(param.Fields, param.Optrs, param.Values);
                        if (Condition.Length > 0)
                        {
                            Query = ServiceUtil.AddSearchParameters(Query, Condition);
                            if (Query.Contains("WHERE") == true && Query.Contains("{XIP|") == true)
                            {
                                Query = Query.Replace("WHERE", "and");
                            }
                        }
                    }
                }
                if (Search.SearchType == "NaturalSearch")
                {
                    string NewSearchText = GetSearchString(param.SearchText, Search.ReportID, Search.OrgID, Search.SearchType, Search.UserID, sOrgName, Search.database);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (Search.SearchText != null && Search.SearchText.Length > 0)
                {
                    string NewSearchText = GetSearchString(Search.SearchText, Search.ReportID, Search.OrgID, Search.SearchType, Search.UserID, sOrgName, Search.database);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (query.ParentID > 0)
                {
                    Query = ServiceUtil.AddParentWhereConditon(Query, Search.FKi1ClickIDXIGUID);
                }
                if (Search.ReportColumns != null && Search.ReportColumns.Length > 0)
                {
                    var wherecondition = Com.GetReportColumnsWhereCondition(iUserID, Search.BaseID, Search.ReportColumns, Search.OrgID, sDatabase, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, wherecondition);
                }
                var Location = dbCore.XIAppUsers.Find(Search.UserID);
                string BOName = dbContext.BOs.Where(m => m.BOID == query.BOID).Select(m => m.Name).FirstOrDefault();
                if (Search.Role != EnumRoles.SuperAdmin.ToString() && Search.Role != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Search.OrgID);
                    var LocCondition = "";
                    var Locs = Location.sLocation.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in Locs)
                    {
                        LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Search.OrgID + "_" + items + "' or ";
                    }
                    LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                    LocCondition = "(" + LocCondition + ")";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                else if (Search.Role != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Search.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Search.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                if (param.NVPairs != null)
                {
                    var sOneQuery = Regex.Replace(param.NVPairs, " ", "").Replace("/", "").Replace("[", "").Replace("]", "").Replace("\"", "").Split(',').ToList();
                    Dictionary<string, string> QueryInfo = new Dictionary<string, string>();
                    foreach (var item in sOneQuery)
                    {
                        var QueryData = item.Split('-').ToArray();
                        QueryInfo[QueryData[0]] = QueryData[1];
                    }
                    string sFinalString = CheckString(Query);
                    while (!string.IsNullOrEmpty(sFinalString))
                    {
                        foreach (var item in QueryInfo)
                        {
                            var finalquery = sFinalString.Replace("{XIP|", "").Replace("}", "");
                            if (item.Key.ToLower() == finalquery.ToLower())
                            {
                                if (item.Value.ToLower() == "null" || item.Value.ToLower() == "isnull" || item.Value.ToLower() == "isnotnull")
                                {
                                    var sISstring = CheckStringISNULL(Query, sFinalString);
                                    var sNullValue = string.Empty;
                                    if (item.Value.ToLower() == "isnull")
                                    {
                                        sNullValue = " IS NULL";
                                    }
                                    else if (item.Value.ToLower() == "isnotnull")
                                    {
                                        sNullValue = " IS NOT NULL";
                                    }
                                    Query = sISstring.Replace(sFinalString, sNullValue);
                                    break;
                                }
                                else
                                {
                                    if (item.Value == "")
                                    {
                                        var oParamList = dbContext.XI1ClickParameterNDVs.Where(m => m.FKi1ClickID == Search.ReportID).ToList();
                                        var sDefValue = string.Empty;
                                        foreach (var OneParam in oParamList)
                                        {
                                            var sParamName = OneParam.sName;
                                            if (sParamName == finalquery)
                                            {
                                                if (OneParam.sDefault == "")
                                                {
                                                    sDefValue = OneParam.sValue;
                                                }
                                                else
                                                {
                                                    sDefValue = OneParam.sDefault;
                                                }
                                            }
                                        }
                                        Query = Query.Replace(sFinalString, sDefValue);
                                    }
                                    else
                                    {
                                        Query = Query.Replace(sFinalString, "'" + item.Value + "'");
                                        break;
                                    }
                                }
                            }
                        }
                        sFinalString = CheckString(Query);
                    }
                }
                var oBO = dbContext.BOs.Find(BOID);
                var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                List<string[]> results = new List<string[]>();
                List<object[]> TotalResult = new List<object[]>();
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    string sortExpression = data.Columns[param.iSortCol].ToString();
                    string sortDirection = param.sSortDir;
                    DataView dv = data.DefaultView;
                    dv.Sort = sortExpression + " " + sortDirection;
                    data = dv.ToTable();
                    reader.Close();
                    //Get Codings
                    TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    List<object[]> Res = new List<object[]>();

                    if (query.ResultListDisplayType == 1 || Search.ShowType == EnumLocations.Dashboard.ToString())
                    {
                        Res = TotalResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                    }
                    for (int i = 0; i < Res.Count(); i++)
                    {
                        List<string> NewRes = new List<string>();
                        for (int j = 0; j < Res[i].Count(); j++)
                        {
                            var oBOField = oBO.BOFields.Where(m => m.LabelName == Headings[j]).FirstOrDefault();
                            var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                            if (Scripts[j] != null && Scripts[j].Length > 0)
                            {
                                //long LeadID = Convert.ToInt64(Res[i][0]);
                                //var ScrpResult = button1_Click(Scripts[j], AllLeads.Where(m => m.ID == Convert.ToInt64(Res[i][0])).FirstOrDefault());
                                //var regex = new Regex(@"(?<=\{)[^}]*(?=\})");
                                //var Fields = new List<string>();
                                //foreach (Match match in regex.Matches(Scripts[j]))
                                //{
                                //    Fields.Add(match.Value);
                                //}
                                //DataTable Cdata = new DataTable();
                                //var Dtypes = new List<List<string>>();
                                //var Script = Scripts[j].ToString();
                                //string columns = "";
                                //foreach (var items in Fields)
                                //{
                                //    var scr = items.Split('.').ToList();
                                //    Dtypes.Add(scr);
                                //    columns = columns + scr[1] + ", ";
                                //}
                                //columns = columns.Substring(0, columns.Length - 2);
                                //var FIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                                //var CodingQuery = "Select " + columns + " " + Query.Substring(FIndex, Query.Length - FIndex);
                                //SqlCommand cmmd = new SqlCommand();
                                //cmmd.Connection = Con;
                                //cmmd.CommandText = CodingQuery;
                                //reader = cmmd.ExecuteReader();
                                //Cdata.Load(reader);
                                //reader.Close();
                                //List<object[]> CodeResult = Cdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                                //List<object[]> Codes = new List<object[]>();
                                //Codes = CodeResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                                //List<string> ScrVals = new List<string>();
                                //for (int h = 0; h < Dtypes.Count(); h++)
                                //{
                                //    if (Dtypes[h][2] == "formattedvalue")
                                //    {
                                //        var Value = Com.ReplaceForeginKeyValues(new VMDropDown { text = Dtypes[h][1] }, Codes[i][h].ToString(), Search.database);
                                //        if (Value != null && Value.Length > 0)
                                //        {
                                //            ScrVals.Add(Value);
                                //        }
                                //        else
                                //        {
                                //            ScrVals.Add(Codes[i][h].ToString());
                                //        }
                                //    }
                                //    else
                                //    {
                                //        ScrVals.Add(Codes[i][h].ToString());
                                //    }
                                //}
                                //for (int m = 0; m < Fields.Count(); m++)
                                //{
                                //    Script = Script.Replace("{" + Fields[m] + "}", ScrVals[m]);
                                //}
                                //string ScrpResult = "";
                                //using (Microsoft.CSharp.CSharpCodeProvider foo = new Microsoft.CSharp.CSharpCodeProvider())
                                //{
                                //    //Script = "if(50==50 && \"NR32 1DR\".Contains(\"DR\")){ return \"<TABLE>XYZ</TABLE>\";}else return \"<TABLE>PQR</TABLE>\"";
                                //    var res = foo.CompileAssemblyFromSource(
                                //        new System.CodeDom.Compiler.CompilerParameters()
                                //        {
                                //            GenerateInMemory = true
                                //        },
                                //        "public class FooClass { string i=\"007\"; public string Execute() {return i;}}"
                                //    );
                                //    var type = res.CompiledAssembly.GetType("FooClass");
                                //    var obj = Activator.CreateInstance(type);
                                //    ScrpResult = type.GetMethod("Execute").Invoke(obj, new object[] { }).ToString();
                                //}
                                //NewRes.Add(ScrpResult);
                            }
                            else if (!string.IsNullOrEmpty(oBOField.FKTableName))
                            {
                                if (!string.IsNullOrEmpty(Res[i][j].ToString()))
                                {
                                    var FKData = oXIAPI.ResolveGroupFieldsWithValues("Label", Convert.ToInt32(Res[i][j]), oBOField.FKTableName, iUserID, sOrgName, sDatabase);
                                    if (!string.IsNullOrEmpty(FKData))
                                    {
                                        NewRes.Add(FKData);
                                    }
                                    else
                                    {
                                        NewRes.Add(Res[i][j].ToString());
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                            else if (oBOField.IsOptionList)
                            {
                                if (!string.IsNullOrEmpty(Res[i][j].ToString()))
                                {
                                    var oVal = Res[i][j].ToString();
                                    var OptionName = dbContext.BOOptionLists.Where(m => m.BOFieldID == oBOField.ID && m.sValues == oVal).FirstOrDefault();
                                    if (OptionName != null)
                                    {
                                        NewRes.Add(OptionName.sOptionName);
                                    }
                                    else
                                    {
                                        NewRes.Add(Res[i][j].ToString());
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                            else
                            {
                                if (Formatting[j] != null)
                                {
                                    if (Formatting[j] == "%")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]));
                                        }
                                    }
                                    else if (Formatting[j] == "en-GB")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            if (Convert.ToInt32(Res[i][j]) > Targets[j])
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetgreencolor'></span>");
                                            }
                                            else
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetredcolor'></span>");
                                            }
                                        }
                                        else
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            NewRes.Add(totalValueCurrency);
                                        }
                                    }
                                    else
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]));
                                        }
                                    }
                                }
                                else
                                {
                                    if (Targets[j] != 0)
                                    {
                                        NewRes.Add(Res[i][j].ToString() + "<span class='targetcolor'></span>");
                                    }
                                    else
                                    {
                                        NewRes.Add(Res[i][j].ToString());
                                    }
                                }
                            }
                        }
                        results.Add(NewRes.ToArray());
                    }
                    if (Search.SearchText != null && Search.SearchType == "Quick")
                    {
                        foreach (var item in results)//Highlight the SearchedText 
                        {
                            for (int i = 0; i <= item.Length - 1; i++)
                            {
                                if (item[i].IndexOf(Search.SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                                {
                                    string input = item[i];
                                    string pattern = Search.SearchText;
                                    string replacement = string.Format("<strong style='color:#d0592e !important;'>{0}</strong>", "$0");
                                    var result = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
                                    item[i] = result;
                                }
                            }
                        }
                    }
                    Con.Close();
                }
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalResult.Count(),
                    iTotalDisplayRecords = TotalResult.Count(),
                    aaData = results,
                    Headings = Headings
                };
            }
            catch (Exception ex)
            {
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]>(),
                    Headings = AllHeadings
                };
            }
        }
        public string CheckString(string Query)
        {
            var sFinalString = string.Empty;
            if (Query.Contains("}"))
            {
                int iCollectionIndex = Query.IndexOf("}");
                int iStartPosi = Query.LastIndexOf("{", iCollectionIndex);
                int iStringLength = "}".Length;
                sFinalString = Query.Substring(iStartPosi, (iCollectionIndex - iStartPosi) + iStringLength).Replace(" ", "");
            }
            return sFinalString;
        }
        public string CheckStringISNULL(string Query, string FindString)
        {
            var sFinalString = string.Empty;
            if (Query.Contains(FindString))
            {
                int iCollectionIndex = Query.IndexOf(FindString);
                int iStartPosi = Query.LastIndexOf("=", iCollectionIndex);
                sFinalString = Query.Remove(iStartPosi, 1);
            }
            return sFinalString;
        }

        public List<int> GetLoadingType(string QueryID, string database)
        {
            List<int> Values = new List<int>();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            int Loading = 0;
            if (iQueryID > 0)
            {
                Loading = dbContext.Reports.Where(m => m.ID == iQueryID).Select(m => m.ResultListDisplayType).FirstOrDefault();
                Common.SaveErrorLog("Error at InboxRepository: GetLoadingType: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                Loading = dbContext.Reports.Where(m => m.XIGUID == QueryIDGUID).Select(m => m.ResultListDisplayType).FirstOrDefault();
            }
            Values.Add(Loading);
            //var structer = dbContext.Reports.Where(m => m.ParentID == QueryID).ToList();
            //if (structer.Count() > 0)
            //{
            //    Values.Add(1);
            //}
            //else
            //{
            //    Values.Add(0);
            //}
            return Values;
        }

        public string GetSearchString(string SearchText, int ReportID, int OrgID, string SearchType, int iUserID, string sOrgName, string sDatabase)
        {
            string NewSearchText = "";
            if (SearchText != null && SearchText.Length > 0)
            {
                var BOID = dbContext.Reports.Where(m => m.ID == ReportID).Select(m => m.BOID).FirstOrDefault();
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                string AndOR = "";
                List<string> AndOr = new List<string>();
                var regex = new Regex(@"[<>]=?|[!=]?=");
                Dictionary<string, bool> myDict = new Dictionary<string, bool>();
                //var FilterGroup = dbContext.BOGroupFields.Where(m => m.BOID == BOID && m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (SearchText.IndexOf(" and ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AndOr.Add("and");
                    var AndSpliting = Regex.Split(SearchText, " and ", RegexOptions.IgnoreCase).ToList();
                    foreach (var items in AndSpliting)
                    {
                        if (items.IndexOf(" or ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            AndOr.Add("or");
                            var OrSpliting = Regex.Split(items, " or ", RegexOptions.IgnoreCase).ToList();
                            foreach (var item in OrSpliting)
                            {
                                AndOR = "or";
                                NewSearchText = NewSearchText + GetModifiedString(item, BOID, sDatabase, AndOR, SearchType, iUserID, sOrgName);
                            }

                        }
                        else
                        {
                            AndOR = "And";
                            NewSearchText = NewSearchText + GetModifiedString(items, BOID, sDatabase, AndOR, SearchType, iUserID, sOrgName);
                        }
                    }
                }
                else if (SearchText.IndexOf(" or ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AndOr.Add("or");
                    var AndSpliting = Regex.Split(SearchText, " or ", RegexOptions.IgnoreCase).ToList();
                    foreach (var items in AndSpliting)
                    {

                        if (items.IndexOf(" and ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            AndOr.Add("and");
                            var OrSpliting = Regex.Split(items, " and ", RegexOptions.IgnoreCase).ToList();
                            foreach (var item in OrSpliting)
                            {
                                AndOR = "and";
                                NewSearchText = NewSearchText + GetModifiedString(item, BOID, sDatabase, AndOR, SearchType, iUserID, sOrgName);
                            }
                        }
                        else
                        {
                            AndOR = "or";
                            NewSearchText = NewSearchText + GetModifiedString(items, BOID, sDatabase, AndOR, SearchType, iUserID, sOrgName);
                        }
                    }
                }
                else
                {
                    NewSearchText = GetModifiedString(SearchText, BOID, sDatabase, null, SearchType, iUserID, sOrgName);
                }
                if (AndOr.Count() > 0)
                {
                    if (AndOr.Last() == "and")
                    {
                        NewSearchText = NewSearchText.Substring(0, NewSearchText.Length - 5);
                    }
                    else
                    {
                        NewSearchText = NewSearchText.Substring(0, NewSearchText.Length - 4);
                    }
                }
                return NewSearchText;
            }
            else
            {
                return NewSearchText;
            }
        }

        private string GetModifiedString(string SearchText, int BOID, string database, string AndOr, string SearchType, int iUserID, string sOrgName)
        {
            var GroupFields = dbContext.BOGroupFields.Where(m => m.BOID == BOID).ToList();
            if (SearchType == "Quick")
            {
                var SearchString = "";
                var SearchWords = SearchText.Split(' ').ToList();
                var QuickGroup = GroupFields.Where(m => m.GroupName == "Quick Search").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in SearchWords)
                {
                    if (items.Length > 0)
                    {
                        foreach (var item in QuickGroup)
                        {
                            SearchString = SearchString + item + " like '%" + items + "%' or ";
                        }
                    }
                }
                SearchString = SearchString.Substring(0, SearchString.Length - 4);
                return SearchString;
            }
            else
            {
                var QuickGroup = new List<string>();
                if (SearchType == "Quick")
                {
                    QuickGroup = GroupFields.Where(m => m.GroupName == "Quick Search").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    QuickGroup = GroupFields.Where(m => m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, database).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                string NewSearchText = "";
                var regex = new Regex(@"[<>]=?|[!=]?=");
                var Operators = new List<string>();
                foreach (Match match in regex.Matches(SearchText))
                {
                    Operators.Add(match.Value);
                }
                foreach (var items in Operators)
                {
                    string SValue = "";
                    string ID = "";
                    var value = Regex.Split(SearchText, items, RegexOptions.IgnoreCase).ToList();
                    var TextedName = value[0].TrimStart().TrimEnd();
                    if (TextedName.IndexOf("class", StringComparison.OrdinalIgnoreCase) >= 0 || TextedName.IndexOf("fkileadclassid", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SValue = value[1].TrimStart().TrimEnd();
                        int ClassID = Spdb.OrganizationClasses.Where(m => m.Class == SValue).Select(m => m.ClassID).FirstOrDefault();
                        if (ClassID != 0)
                        {
                            ID = ClassID.ToString();
                        }
                        else
                        {
                            ID = SValue;
                        }
                    }
                    else if (TextedName.IndexOf("FKiSourceID", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SValue = value[1].TrimStart().TrimEnd();
                        int SourceID = Spdb.OrganizationSources.Where(m => m.Name == SValue).Select(m => m.ID).FirstOrDefault();
                        if (SourceID != 0)
                        {
                            ID = SourceID.ToString();
                        }
                        else
                        {
                            ID = SValue;
                        }
                    }

                    else
                    {
                        ID = value[1].TrimStart().TrimEnd();
                    }
                    var AliasName = value[0].TrimStart().TrimEnd();
                    var OriginalName = dbContext.BOFields.Where(m => m.LabelName == AliasName && m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                    if (OriginalName == null)
                    {
                        OriginalName = value[0].TrimStart().TrimEnd();
                    }
                    if (QuickGroup.Contains(OriginalName))
                    {
                        if (AndOr != null)
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "' " + AndOr + " ";
                        }
                        else
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                        }
                    }
                    else if (SearchType == "Structured")
                    {
                        NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                    }
                    else
                    {
                        if (AndOr != null)
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "' " + AndOr + " ";
                        }
                        else
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                        }
                    }
                }
                return NewSearchText;
            }

        }

        public VMResultList GetHeadings(string ReportID, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList Preview = new VMResultList();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reports Report = new Reports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ReportID, out iQueryID);
            Guid.TryParse(ReportID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                Report = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at InboxRepository: GetHeadings: " + iQueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                Report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            var oBO = dbContext.BOs.Find(Report.BOID);
            var BoFields = oBO.BOFields.Where(m => m.BOID == Report.BOID).ToList();
            var MapFields = new List<MappedFields>();// Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == Report.Class).ToList();
            var GroupFields = dbContext.BOGroupFields.Where(m => m.BOID == Report.BOID).ToList();
            if (Report.Query != null)
            {
                int BOID = Report.BOID;
                List<VMDropDown> KeyPositions = new List<VMDropDown>();
                Common Com = new Common();
                var FromIndex = Report.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = Report.Query.Substring(0, FromIndex);
                SelectQuery = SelectQuery.TrimEnd();
                var SelWithGroup = "";
                var regx = new Regex("{.*?}");
                var mathes = regx.Matches(SelectQuery);
                if (mathes.Count > 0)
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            int id = Convert.ToInt32(items.Substring(1, items.Length - 2));
                            var Grp = GroupFields.Where(m => m.ID == id).FirstOrDefault();
                            if (Grp.IsMultiColumnGroup)
                            {
                                SelWithGroup = SelWithGroup + Grp.BOFieldNames + ", ";
                            }
                            else
                            {
                                SelWithGroup = SelWithGroup + Grp.GroupName + ", ";
                            }
                        }
                        else
                        {
                            SelWithGroup = SelWithGroup + items + ", ";
                        }
                    }
                    SelWithGroup = SelWithGroup.Substring(0, SelWithGroup.Length - 2);
                }
                else
                {
                    SelWithGroup = SelectQuery;
                }
                var Keys = ServiceUtil.GetForeginkeyValues(" " + SelWithGroup);
                List<Targets> TargetList = new List<Targets>();
                if (iQueryID > 0)
                {
                    TargetList = Spdb.Targets.Where(m => m.ReportID == iQueryID).ToList();
                    Common.SaveErrorLog("Error at QueryGenerationController: QueryEditPopUP: " + iQueryID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    TargetList = Spdb.Targets.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).ToList();
                }                
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<int> Targets = new List<int>();
                if (string.IsNullOrEmpty(Report.SelectFields))
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string Head = Regex.Split(items, " as ", RegexOptions.IgnoreCase)[0];
                            Headings.Add(Head);
                        }
                        else
                        {
                            Headings.Add(items);
                        }
                    }
                }
                else
                {
                    Headings = Report.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                List<string> AllHeadings = new List<string>();
                List<string> TableColumns = new List<string>();
                List<int> ColumnReports = new List<int>();
                List<string> MouseOverColumns = new List<string>();
                List<string> Scripts = new List<string>();
                var Columns = new List<string>();
                if (Report.OnClickColumn != null)
                {
                    Columns = Report.OnClickColumn.Split(',').ToList();
                }
                var ColumnIds = new List<string>();
                //if (query.OnColumnClickValue != null)
                //{
                //    ColumnIds = query.OnColumnClickValue;
                //}            
                var str1 = "";
                if (Headings.Contains("ID") == false)
                {
                    //str1 = "No";
                    //Headings.Insert(0, "ID");
                    Preview.IDExists = false;
                }
                else
                {
                    Preview.IDExists = true;
                }
                string allfields = "";
                var groupfieldseditquery2 = "";
                var groupfieldseditquery5 = "";
                if (str1 == "No")
                {
                    //var allfields1 = (query.Query).Insert(7, " ID, ");
                    //allfields = (query.Query).Insert(7, " ID, ");
                    //if (allfields.Contains("ORDER BY") == true && allfields.Contains("GROUP BY") == true)
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //else if (allfields.Contains("GROUP BY") == false)
                    //{
                    //    allfields = allfields1;
                    //}
                    //else
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //if (groupfieldseditquery2 != "")
                    //{
                    //    groupfieldseditquery2 = groupfieldseditquery2 + ", " + "ID" + " ";
                    //    allfields = allfields.Replace(groupfieldseditquery5, groupfieldseditquery2);
                    //}
                    allfields = Report.Query;
                }
                else
                    allfields = Report.Query;
                int FKPosition = 0, i = 0;
                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        string id = items.Substring(1, items.Length - 2);
                        int gid = Convert.ToInt32(id);
                        string groupid = Convert.ToString(gid);
                        BOGroupFields fields = GroupFields.Where(m => m.ID == gid).FirstOrDefault();
                        allfields = allfields.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                        if (fields.IsMultiColumnGroup)
                        {
                            List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                var BoField = BoFields.Where(m => m.Name == names).FirstOrDefault();
                                string aliasname = BoField.LabelName;
                                if (aliasname != null)
                                {
                                    AllHeadings.Add(aliasname);
                                }
                                else
                                {
                                    AllHeadings.Add(names);
                                }
                                TableColumns.Add(names);
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                                KeyPositions.AddRange((from c in Keys.Where(m => m.text == names) select new VMDropDown { text = names, Value = FKPosition }));
                                FKPosition++;
                            }
                        }
                        else
                        {
                            AllHeadings.Add(fields.GroupName);
                            TableColumns.Add(fields.GroupName);
                            Formatting.Add(null);
                            Scripts.Add(null);
                            Targets.Add(0);
                            MouseOverColumns.Add("");
                            FKPosition++;
                        }

                    }
                    else
                    {
                        var Fld = items;
                        if (Columns.Contains(items))
                        {
                            int index = Columns.IndexOf(items);
                            //int ID = Convert.ToInt32(ColumnIds[index]);
                            //ColumnReports.Add(ID);
                        }
                        else
                        {
                            //ColumnReports.Add(0);
                        }
                        string aliasname = "";
                        if (OrgID != 0)
                        {
                            aliasname = MapFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                            if (aliasname != null)
                            {
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        else
                        {
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name == Fld).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                aliasname = BoField.LabelName;
                            }
                            Formatting.Add(BoField.Format);
                            Scripts.Add(BoField.Script);
                            var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                            if (target != null)
                            {
                                Targets.Add(target.Target);
                            }
                            else
                            {
                                Targets.Add(0);
                            }
                            MouseOverColumns.Add(BoField.FKTableName);
                        }
                        if (aliasname == null)
                        {
                            Fld = items;
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name == Fld).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                aliasname = BoField.LabelName;
                            }
                            if (BoField != null)
                            {
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                            }
                            else
                            {
                                aliasname = items;
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        //string aliasname = dbContext.BOFields.Where(m => m.Name == items && m.BOID == BOID).Select(m => m.AliasName).FirstOrDefault();                        
                        KeyPositions.AddRange((from c in Keys.Where(m => m.text == Fld) select new VMDropDown { text = Fld, Value = FKPosition }));
                        if (aliasname != null)
                        {
                            AllHeadings.Add(aliasname);
                        }
                        FKPosition++;
                        TableColumns.Add(Fld);
                    }
                    i++;
                }
                Preview.BO = oBO.Name;
                Preview.BOID = Report.BOID;
                Preview.Headings = AllHeadings;
                Preview.TableColumns = TableColumns;
                Preview.Formats = Formatting;
                Preview.Scripts = Scripts;
                Preview.Targets = Targets;
                Preview.HeadingReports = ColumnReports;
                Preview.IsPopup = Report.IsRowClick;
                Preview.IsExport = Report.IsExport;
                Preview.ActionType = Report.OnRowClickType;
                Preview.ActionReportID = Report.OnRowClickValue;
                Preview.ResultListDisplayType = Report.ResultListDisplayType;
                Preview.IsRowClick = Report.IsRowClick;
                Preview.XiLinkID = Report.RowXiLinkID;
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
                if (iQueryID > 0)
                {
                    Preview.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == iQueryID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                    Common.SaveErrorLog("Error at InboxController: GetHeadings: " + iQueryID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    Preview.QueryIcon = dbContext.UserReports.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                }               
                Preview.QueryName = Report.Name;
                Preview.ShowAs = Report.Title;
                Preview.Query = allfields;
                Preview.FKPositions = KeyPositions;
                var Group = GroupFields.Where(m => m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault();
                if (Group != null)
                {
                    var FilterGroup = Group.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> FilterFileds = new List<string>();
                    Dictionary<string, string> myDict = new Dictionary<string, string>();
                    foreach (var items in FilterGroup)
                    {
                        myDict[BoFields.Where(m => m.Name == items).Select(m => m.LabelName).FirstOrDefault()] = items;
                    }
                    Preview.FilterGroup = myDict;
                }
                else
                {
                    Preview.FilterGroup = new Dictionary<string, string>();
                }
                Preview.IsFilterSearch = Report.IsFilterSearch;
                if (Report.IsFilterSearch)
                {
                    Preview.SearchType = "FilterSearch";
                }
                Preview.IsNaturalSearch = Report.IsNaturalSearch;
                if (Report.IsNaturalSearch)
                {
                    Preview.SearchType = "NaturalSearch";
                }
                Preview.MouseOverColumns = MouseOverColumns;
                int SFCount = 0;
                if (Report.SearchFields != null && Report.SearchFields.Length > 0)
                {
                    SFCount = Report.SearchFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Count();
                }
                Preview.SrchFCount = SFCount;
                Preview.Rows = new List<string[]>();
                List<SingleBOField> bofields = new List<SingleBOField>();
                if (Report.IsFilterSearch)
                {
                    if (Report.SearchFields != null && Report.SearchFields.Length > 0)
                    {
                        List<string> SearchFields = Report.SearchFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (SearchFields != null)
                        {
                            foreach (var items in SearchFields)
                            {
                                BOFields Field = BoFields.Where(m => m.Name.ToLower() == items.ToLower()).FirstOrDefault();
                                int TypeID = Field.TypeID;
                                string type = ((BODatatypes)TypeID).ToString();
                                bofields.Add(new SingleBOField
                                {
                                    ID = Field.ID,
                                    BOID = Field.BOID,
                                    Name = Field.Name,
                                    AliasName = Field.LabelName,
                                    DataType = type,
                                    IsRunTime = Field.IsRunTime,
                                    IsDBValue = Field.IsDBValue,
                                    DBQuery = Field.DBQuery,
                                    IsExpression = Field.IsExpression,
                                    //Expression = Field.Expression,
                                    //ExpreValue = Field.ExpreValue,
                                    IsDate = Field.IsDate,
                                    DateValue = Field.DateValue,
                                    DateExpression = Field.DateExpression
                                });
                            }
                        }
                    }
                    Preview.SingleBOField = bofields;
                }
                else
                {
                    Preview.SingleBOField = bofields;
                }

                Preview.IsQueryExists = true;
                //var AllLeads = Spdb.Database.SqlQuery<VMLeads>("Select * from Leads where FKiOrgID=" + OrgID).ToList();
                //Preview.AllLeads = AllLeads;
                Preview.AllLeads = new List<VMLeads>();
                return Preview;
            }
            else
            {
                VMResultList QPreview = new VMResultList();
                Preview.IsQueryExists = false;
                return QPreview;
            }
        }
        public DataTable ExportDatatableContent(string ReportID, int OrgID, int iUserID, string Role, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList vmquery = new VMResultList();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reports query = dbContext.Reports.Find(ReportID);
            Common Com = new Common();
            var Heads = GetHeadings(ReportID, sDatabase, OrgID, iUserID, sOrgName);
            string UserIDs = Com.GetSubUsers(iUserID, OrgID, sDatabase, sOrgName);
            string Query = ServiceUtil.ReplaceQueryContent(query.Query, UserIDs, iUserID, OrgID, 0, 0);
            var Location = dbCore.XIAppUsers.Find(iUserID);
            string BOName = dbContext.BOs.Where(m => m.BOID == query.BOID).Select(m => m.Name).FirstOrDefault();
            if (Role != EnumRoles.SuperAdmin.ToString() && Role != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
            {
                Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                var LocCondition = "";
                var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in Locs)
                {
                    LocCondition = LocCondition + "OrgHeirarchyID='ORG" + OrgID + "_" + items + "' or ";
                }
                LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                LocCondition = "(" + LocCondition + ")";
                Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
            }
            else if (Role != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
            {
                Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + OrgID);
                var LocCondition = "OrgHeirarchyID Like 'ORG" + OrgID + "_%'";
                Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
            }
            DataTable data = new DataTable();
            SqlConnection Con = new SqlConnection();
            using (Con)
            {
                if (BOName != EnumLeadTables.Reports.ToString())
                {
                    Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                }
                else
                {
                    Con = new SqlConnection(ServiceUtil.GetConnectionString());
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                data.Load(reader);
                for (int i = 0; i < Heads.Headings.Count(); i++)
                {
                    if (data.Columns[i].ToString() != Heads.Headings[i])
                    {
                        data.Columns[i].ColumnName = Heads.Headings[i];
                    }
                }
                Con.Close();
            }
            return data;
        }

        public VMOneClicks GetAllOneClicks(string ReportID, string Query, string database)
        {
            VMOneClicks Clicks = new VMOneClicks();
            List<int> Ids = new List<int>();
            Reports Report = new Reports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ReportID, out iQueryID);
            Guid.TryParse(ReportID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                Report = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at InboxRepository: GetAllOneClicks: " + iQueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                Report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            var BOName = dbContext.BOs.Where(m => m.BOID == Report.BOID).Select(m => m.Name).FirstOrDefault();
            var AllReports = dbContext.Reports.ToList();
            List<object[]> Res = new List<object[]>();
            string NewQuery = "";
            if (Query != null && Query.Length > 0)
            {
                NewQuery = Query;
            }
            else
            {
                NewQuery = Report.Query;
            }
            if (BOName == EnumLeadTables.Reports.ToString())
            {
                List<string> RTypes = new List<string>();
                List<string> RNames = new List<string>();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString()))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = NewQuery;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    Res = TotalResult.ToList();
                    Con.Close();
                }
                Clicks.Clicks = Res;
                Clicks.ClickType = "MultiClicks";
                foreach (var items in Res)
                {
                    int ID = Convert.ToInt32(items[0].ToString());
                    var Rep = AllReports.Where(m => m.ID == ID).FirstOrDefault();
                    RTypes.Add(((EnumDisplayTypes)Rep.DisplayAs).ToString());
                    RNames.Add(Rep.Name);
                }
                Clicks.ReportTypes = RTypes;
                Clicks.ReportNames = RNames;
            }
            else
            {
                Clicks.Clicks = Res;
                Clicks.ClickType = "SingleClick";
                Clicks.ReportTypes = new List<string> { ((EnumDisplayTypes)Report.DisplayAs).ToString() };
                Clicks.ReportNames = new List<string> { Report.Name };
            }
            Clicks.XIComponentID = Report.RepeaterComponentID;
            List<cNameValuePairs> lNVs = new List<cNameValuePairs>();
            lNVs.Add(new cNameValuePairs { sName = "1ClickID", sValue = ReportID.ToString() });
            Clicks.nParams = lNVs.Select(m => new VMNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            return Clicks;
        }

        public int SendMail(string EmailID, int OrgID, string Attachment, string database)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Common Com = new Common();
            //var Res = Com.SendMail(EmailID, OrgID, Attachment, 0);
            return 0;
        }

        public List<Reports> GetStructuredOneClicks(int OrgID, string ID, string database)
        {
            List<Reports> OneClicks = new List<Reports>();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ID, out iQueryID);
            Guid.TryParse(ID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.ID == iQueryID).ToList();
                Common.SaveErrorLog("Error at InboxRepository: GetStructuredOneClicks: " + iQueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.XIGUID == QueryIDGUID).ToList();
            }
            
            return OneClicks;
            //Reports OneClicks = new Reports();
            //var OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.ParentID>0).ToList();
            //foreach (var items in OneClicks)
            //{
            //    //AllOneClicks.Add(items);
            //    var Added = AllOneClicks.Where(m => m.ID == items.ParentID).FirstOrDefault(); 
            //    if(Added==null){
            //        AllOneClicks.Add(dbContext.Reports.Find(items.ParentID));
            //    }                
            //}
            //return AllOneClicks;
        }
        #endregion ReportResults

        #region miscellaneous
        public int SaveReminder(int iUserID, int OrgID, string sDatabase, string sOrgName, Reminders obj)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reminders model = new Reminders();
            model.ID = obj.ID;
            model.ReminderTime = obj.ReminderTime;
            model.Message = obj.Message;
            model.OrganizationID = OrgID;
            model.StatusTypeID = 10;
            model.UserID = iUserID;
            model.LeadID = obj.LeadID;
            model.ReportID = obj.ReportID;
            model.ClassID = obj.ClassID;
            Spdb.Reminders.Add(model);
            Spdb.SaveChanges();
            return 0;
        }
        public DTResponse GetReminderList(jQueryDataTableParamModel param, int LeadID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<Reminders> AllReminders, FilteredReminders;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            // string date = DateTime.Today.Date.ToString("yyyy/MM/dd HH:MM");
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredReminders = Spdb.Reminders.ToList();
                AllReminders = FilteredReminders.OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredReminders.Count();
            }
            else
            {
                displyCount = Spdb.Reminders.Where(m => m.LeadID == LeadID).OrderBy(m => m.ID).Count();
                AllReminders = Spdb.Reminders.Where(m => m.LeadID == LeadID).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllReminders
                         select new[] { 
                            // (i++).ToString(), Convert.ToString(c.OrganizationID), c.Message,  Convert.ToString(c.StatusTypeID)};
                               (i++).ToString(), Convert.ToString(c.ID),Convert.ToString(c.ReminderTime), c.Message, Convert.ToString(c.StatusTypeID)};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMReminders GetReminderUserCount(int iUserID, int OrgID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string Details = "";
            int ID = 0;
            int count = 0;
            List<string> LeadDetails = new List<string>();
            List<int> LeadIDs = new List<int>();
            VMReminders Reminders = new VMReminders();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            int NType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Notifications.ToString());
            var Notifications = dbContext.UserReports.Where(m => m.OrganizationID == OrgID).Where(m => m.RoleID == RoleID && m.Location == NType).Select(m => m.ReportID).FirstOrDefault();
            Reports report = dbContext.Reports.Where(m => m.ID == Notifications).FirstOrDefault();
            if (report != null)
            {
                string UID = iUserID.ToString();
                Common Com = new Common();
                string Query = ServiceUtil.ReplaceQueryContent(report.Query, UID, iUserID, OrgID, 0, 0);
                Con.Open();
                Con.ChangeDatabase(sDatabase);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    count = reader.GetInt32(0);
                }
                reader.Close();
                string Select = "";
                string From = "";
                int f = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                Select = "Select ID, sForeName, sLastName, sMob, sEmail";
                int length = Query.Length - f;
                From = Query.Substring(f, length);
                Query = Select + " " + From;
                cmd.CommandText = Query;
                SqlDataReader readr = cmd.ExecuteReader();
                while (readr.Read())
                {
                    ID = readr.GetInt32(0);
                    Details = readr.IsDBNull(1) ? null : readr.GetString(1) + " " + (readr.IsDBNull(2) ? null : readr.GetString(2)) + "-" + (readr.IsDBNull(3) ? null : readr.GetString(3)) + "-" + (readr.IsDBNull(4) ? null : readr.GetString(4));
                    LeadIDs.Add(ID);
                    LeadDetails.Add(Details);
                }
                reader.Close();
                Con.Close();
                Reminders.InnerReportID = report.OnRowClickValue;
            }

            Reminders.Count = count;
            Reminders.LeadDetails = LeadDetails;
            Reminders.LeadIDs = LeadIDs;
            return Reminders;
        }
        public UserReports GetReportType(int ID, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int ReportID = ID;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            UserReports model = dbContext.UserReports.Where(m => m.RoleID == RoleID).Where(m => m.ReportID == ReportID).FirstOrDefault();
            return model;
        }

        public List<VMDropDown> GetQueriesByType(int RoleID, int ClassType, int Display, string database, int ClassID, int OrgID)
        {
            List<UserReports> UserReports = dbContext.UserReports.Where(m => m.RoleID == RoleID).ToList();
            List<Reports> Reports = new List<Reports>();
            List<int> Assinged = new List<int>();
            //foreach (var ids in UserReports)
            //{
            //    Assinged.Add(ids.ReportID);
            //}
            if (OrgID > 0)
            {
                if (ClassType == 1)
                {
                    Reports = dbContext.Reports.Where(m => m.DisplayAs == Display && m.StatusTypeID == 10).Where(m => m.TypeID == ClassType && m.Class == ClassID && (m.OrganizationID == OrgID || m.OrganizationID == 0)).ToList();
                }
                else
                {
                    Reports = dbContext.Reports.Where(m => m.DisplayAs == Display && m.StatusTypeID == 10).Where(m => m.TypeID == ClassType && (m.OrganizationID == OrgID || m.OrganizationID == 0)).ToList();
                }

            }
            else
            {
                if (ClassType == 1)
                {
                    Reports = dbContext.Reports.Where(m => m.DisplayAs == Display && m.StatusTypeID == 10).Where(m => m.TypeID == ClassType && m.Class == ClassID && m.OrganizationID == OrgID).ToList();
                }
                else
                {
                    Reports = dbContext.Reports.Where(m => m.DisplayAs == Display && m.StatusTypeID == 10).Where(m => m.TypeID == ClassType && m.OrganizationID == OrgID).ToList();
                }
            }
            List<VMDropDown> Classes = new List<VMDropDown>();
            Classes = (from c in Reports.ToList()
                       select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            //List<int> AllReports = new List<int>();
            //foreach (var id in Reports)
            //{
            //    AllReports.Add(id.ID);
            //}
            //var remaining = AllReports.Except(Assinged).ToList();

            //List<Reports> RemainingReports = new List<Reports>();
            //foreach (var remain in remaining)
            //{
            //    Reports report = dbContext.Reports.Find(remain);
            //    RemainingReports.Add(report);
            //}
            //foreach (var items in RemainingReports)
            //{
            //    Classes.Add(new Classes
            //    {
            //        value = items.ID,
            //        text = items.Name
            //    });
            //}
            return Classes;
        }

        public List<VMDropDown> GetAllTabs(int ReportID, string database)
        {
            List<Tabs> Tabs = new List<Tabs>();
            List<VMDropDown> AllTabs = new List<VMDropDown>();
            var popuptype = dbContext.Reports.Where(m => m.ID == ReportID).Select(m => m.ActionFieldValue).FirstOrDefault();
            if (popuptype != null)
            {
                AllTabs = (from c in dbContext.Tabs.Where(m => m.PopupID == 0).OrderBy(m => m.Rank).ToList()
                           select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
                return AllTabs;
            }
            else
            {
                return null;
            }
        }

        public List<VMDropDown> GetClassDropDownList(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var list = ServiceUtil.GetOrgClasses(OrgID, sDatabase);
            return list;
        }
        public List<VMDropDown> GetDateDropDownList(string database)
        {
            List<VMDropDown> ddlDateList = new List<VMDropDown> { new VMDropDown { Value = 1, text = "Today" }, new VMDropDown { Value = 2, text = "Past 1 Week" }, new VMDropDown { Value = 3, text = "Past 1 Month" } };
            return ddlDateList;
        }

        public int AssignLeadToUser(int LeadID, int UserID, int OrgID, string database)
        {
            int PrevUser = 0;
            Con.Open();
            Con.ChangeDatabase(database);
            using (SqlCommand cmd = new SqlCommand("", Con))
            {
                cmd.CommandText = "SELECT UserID FROM " + EnumLeadTables.Leads.ToString() + " Where ID = " + LeadID;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PrevUser = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                }
                reader.Close();
                if (PrevUser == 0)
                {
                    cmd.CommandText = "UPDATE " + EnumLeadTables.Leads.ToString() + " SET UserID= " + UserID + " WHERE ID=" + LeadID;
                    cmd.ExecuteNonQuery();
                    Con.Dispose();
                }

            }
            return 0;
        }
        public DTResponse ViewReportsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            List<UserReports> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = dbContext.UserReports.Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                AllDetails = FilteredDetails.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            else
            {
                displyCount = dbContext.UserReports.Where(m => m.RoleID == RoleID).Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0).Count();
                AllDetails = dbContext.UserReports.Where(m => m.RoleID == RoleID).Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            IEnumerable<string[]> result;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                result = from c in AllDetails
                         join r in dbContext.Reports.Where(m => m.Name.Contains(param.sSearch)) on c.ReportID equals r.ID
                         select new[] {
                             (i++).ToString(),Convert.ToString(r.ID) ,UserName(c.RoleID, sDatabase),ReportName(c.ReportID,sDatabase),r.Description, ((EnumDisplayTypes)r.DisplayAs).ToString(),((EnumLocations)c.Location).ToString(),""};
            }
            else
            {
                result = from c in AllDetails
                         join r in dbContext.Reports on c.ReportID equals r.ID
                         select new[] {
                             (i++).ToString(),Convert.ToString(r.ID) ,UserName(c.RoleID, sDatabase),ReportName(c.ReportID,sDatabase),r.Description, ((EnumDisplayTypes)r.DisplayAs).ToString(),((EnumLocations)c.Location).ToString(),""};
            }

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        string ReportName(int ReportID, string database)
        {
            var Report = dbContext.Reports.Where(m => m.ID == ReportID).Select(m => m.Name).FirstOrDefault();
            return Report;
        }

        public List<VMDashReports> GetDashboardReports(int iUserID, string sOrgName, string sDatabase)
        {
            Reports Report = dbContext.Reports.Find(27);
            Common Com = new Common();
            var Keys = ServiceUtil.GetForeginkeyValues(Report.Query);
            foreach (var items in Keys)
            {

            }
            List<string[]> results = new List<string[]>();
            Con.Open();
            Con.ChangeDatabase(sDatabase);
            cmd.Connection = Con;
            cmd.CommandText = Report.Query;
            //cmd.ExecuteNonQuery();
            SqlDataReader reader = cmd.ExecuteReader();
            int count = reader.FieldCount;
            string[] rows = new string[count];
            string[] value = null;
            while (reader.Read())
            {
                List<string> values = new List<string>();

                for (int i = 0; i < count; i++)
                {
                    if (!reader.IsDBNull(i))
                    {
                        values.Add(reader.GetValue(i).ToString());
                    }
                    else
                    {
                        values.Add("0");
                    }
                }
                string[] result = values.ToArray();
                results.Add(result);
                value = result;
            }
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<string> Headings = new List<string>();
            List<VMDashReports> DReports = new List<VMDashReports>();
            foreach (var items in results)
            {
                VMDashReports Rep = new VMDashReports();
                int SorID = Convert.ToInt32(items[0]);
                string SorceName = Spdb.OrganizationSources.Where(m => m.ID == SorID).Select(m => m.Name).FirstOrDefault();
                Rep.Heading = SorceName;
                Headings.Add(SorceName);
                int SgID = Convert.ToInt32(items[1]);
                string SgName = dbContext.Stages.Where(m => m.ID == SgID).Select(m => m.Name).FirstOrDefault();
                Rep.Status = SgName;
                //Left.Add(SgName);
                Rep.TCount = Convert.ToInt32(items[2]);
                //Counts.Add(Convert.ToInt32(items[2]));
                DReports.Add(Rep);
            }
            List<VMDashReports> Final = new List<VMDashReports>();
            List<string> Stats = new List<string>();
            Headings = Headings.Distinct().ToList();
            foreach (var items in DReports)
            {

                if (Stats.Contains(items.Status))
                {

                }
                else
                {
                    VMDashReports repp = new VMDashReports();
                    List<string> Heads = new List<string>();
                    List<int> Counts = new List<int>();
                    var res = DReports.Where(m => m.Status == items.Status).ToList();
                    int i = 0;
                    foreach (var item in res)
                    {
                        if (i == 0)
                        {
                            foreach (var head in Headings)
                            {
                                var xxss = res.Where(m => m.Heading == head).FirstOrDefault();
                                if (xxss != null)
                                {
                                    Counts.Add(xxss.TCount);
                                }
                                else
                                {
                                    Counts.Add(0);
                                }
                                i++;
                            }
                        }
                    }
                    repp.Status = items.Status;
                    repp.Heads = Heads;
                    //repp.Counts = Counts;
                    Final.Add(repp);
                }
                Stats.Add(items.Status);
            }
            Final.FirstOrDefault().Headings = Headings.Distinct().ToList();
            Con.Close();
            return Final;
        }

        private string button1_Click(string Script, VMLeads Lead)
        {
            VMLeads ll = new VMLeads() { iStatus = 10, FKiLeadClassID = 51, sPostCode = "CDS" };
            // *** Example form input has code in a text box
            Script = Script.Replace("{", "");
            Script = Script.Replace("}", "");
            Script = Script.Replace(",", "+\",\"+");
            string lcCode = "return " + Script + ";";


            ICodeCompiler loCompiler = new CSharpCodeProvider().CreateCompiler();
            CompilerParameters loParameters = new CompilerParameters();

            // *** Start by adding any referenced assemblies
            loParameters.ReferencedAssemblies.Add("System.dll");
            loParameters.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            loParameters.ReferencedAssemblies.Add("D:\\TfsProjects\\XIDNA\\XIDNA.Repository\\bin\\Debug\\XIDNA.ViewModels.dll");

            // *** Must create a fully functional assembly as a string
            //            lcCode = @"using System;
            //using System.IO;
            //using System.Windows.Forms;
            //using XIDNA.ViewModels; 
            //namespace MyNamespace {
            //public class MyClass:VMLeads {
            //  public object DynamicCode(params object[] Parameters) { VMLeads Lead = new VMLeads(){ ID=" + Lead.ID + ", InBoundID=" + Lead.InBoundID + ", UserID=" + Lead.UserID + ",iTeamID=" + Lead.iTeamID + ",OrgHeirarchyID=\"" + Lead.OrgHeirarchyID + "\",sName=\"" + Lead.sName + "\",sForeName=\"" + Lead.sForeName + "\",sLastName=\"" + Lead.sLastName + "\",dDateRenewal=\"" + Lead.dDateRenewal + "\",FKiLeadClassID=" + Lead.FKiLeadClassID + ",FKiSourceGroupID=" + Lead.FKiSourceGroupID + ",FKiSourceID=" + Lead.FKiSourceID + ",sMob=\"" + Lead.sMob + "\",sTel=\"" + Lead.sTel + "\",sEmail=\"" + Lead.sEmail + "\",sAddress1=\"" + Lead.sAddress1 + "\",sAddress2=\"" + Lead.sAddress2 + "\",sAddress3=\"" + Lead.sAddress3 + "\",sAddress4=\"" + Lead.sAddress4 + "\",sAddress5=\"" + Lead.sAddress5 + "\",sPostCode=\"" + Lead.sPostCode + "\",sCompany=\"" + Lead.sCompany + "\",dDOB=\"" + Lead.dDOB + "\",sNotes=\"" + Lead.sNotes + "\",sBestTimeToCall=\"" + Lead.sBestTimeToCall + "\",iBestDayToCall=" + Lead.iBestDayToCall + ",iEMailOptOut=" + Lead.iEMailOptOut + ",iSMSOptOut=" + Lead.iSMSOptOut + ",iCallOptOut=" + Lead.iCallOptOut + ",FKiOrgID=" + Lead.FKiOrgID + ",FKiClientID=" + Lead.FKiClientID + ",AddField1=" + Lead.AddField1 + ",AddField2=" + Lead.AddField2 + ",AddField3=" + Lead.AddField3 + ",AddField4=" + Lead.AddField4 + ",AddField5=" + Lead.AddField5 + ",AddField6=\"" + Lead.AddField6 + "\",AddField7=\"" + Lead.AddField7 + "\",AddField8=\"" + Lead.AddField8 + "\",AddField9=\"" + Lead.AddField9 + "\",AddField10=\"" + Lead.AddField10 + "\",AddField11=\"" + Lead.AddField11 + "\",AddField12=\"" + Lead.AddField12 + "\",AddField13=\"" + Lead.AddField13 + "\",AddField14=\"" + Lead.AddField14 + "\",AddField15=\"" + Lead.AddField15 + "\",AddField16=\"" + Lead.AddField16 + "\",AddField17=\"" + Lead.AddField17 + "\",AddField18=\"" + Lead.AddField18 + "\",AddField19=\"" + Lead.AddField19 + "\",AddField20=\"" + Lead.AddField20 + "\",XICreatedBy=\"" + Lead.XICreatedBy + "\",XICreatedWhen=\"" + Lead.XICreatedWhen + "\",XIUpdatedBy=\"" + Lead.XIUpdatedBy + "\",XIUpdatedWhen=\"" + Lead.XIUpdatedWhen + "\",dImportedOn=\"" + Lead.dImportedOn + "\",iPriority=" + Lead.iPriority + ",iFinance=" + Lead.iFinance + ",iStatus=" + Lead.iStatus + ",BrokerRefID=" + Lead.BrokerRefID + ",AssignedTime=\"" + Lead.AssignedTime + "\",iCallCount=" + Lead.iCallCount + ",dtCallSchedule=\"" + Lead.dtCallSchedule + "\",iCallbackStatus=" + Lead.iCallbackStatus + ",sSystemAlert=\"" + Lead.sSystemAlert + "\",iOutboundUserID=" + Lead.iOutboundUserID + ",iSalesUserID=" + Lead.iSalesUserID + ",sDairyReason=\"" + Lead.sDairyReason + "\",sCoding=\"" + Lead.sCoding + "\"}; " + lcCode +
            //         "}   }    }";
            lcCode = @"using System;
using System.IO;
using System.Windows.Forms;
using XIDNA.ViewModels; 
namespace MyNamespace {
public class MyClass:VMLeads {
  public object DynamicCode(params object[] Parameters) { VMLeads Lead = new VMLeads(){ iStatus=" + Lead.iStatus + ", sPostCode=\"" + Lead.sPostCode + "\", FKiLeadClassID=" + Lead.FKiLeadClassID + "}; " + lcCode +
         "}   }    }";

            // *** Load the resulting assembly into memory
            loParameters.GenerateInMemory = false;

            // *** Now compile the whole thing
            CompilerResults loCompiled =
                    loCompiler.CompileAssemblyFromSource(loParameters, lcCode);

            if (loCompiled.Errors.HasErrors)
            {
                string lcErrorMsg = "";

                lcErrorMsg = loCompiled.Errors.Count.ToString() + " Errors:";
                for (int x = 0; x < loCompiled.Errors.Count; x++)
                    lcErrorMsg = lcErrorMsg + "\r\nLine: " +
                                 loCompiled.Errors[x].Line.ToString() + " - " +
                                 loCompiled.Errors[x].ErrorText;

                //MessageBox.Show(lcErrorMsg + "\r\n\r\n" + lcCode,
                //"Compiler Demo");
                return lcErrorMsg;
            }

            Assembly loAssembly = loCompiled.CompiledAssembly;

            // *** Retrieve an obj ref – generic type only
            object loObject = loAssembly.CreateInstance("MyNamespace.MyClass");
            if (loObject == null)
            {
                //MessageBox.Show("Couldn't load class.");
                return "";
            }

            VMLeads Leads = new VMLeads();
            Leads.iStatus = 0;

            object[] loCodeParms = new object[1];
            loCodeParms[0] = "West Wind Technologies";

            try
            {
                string loResult = loObject.GetType().InvokeMember(
                                 "DynamicCode", BindingFlags.InvokeMethod,
                                 null, loObject, loCodeParms).ToString();
                return loResult;
                //DateTime ltNow = (DateTime)loResult;
                //MessageBox.Show("Method Call Result:\r\n\r\n" +
                //loResult.ToString(), "Compiler Demo");
            }
            catch (Exception loError)
            {
                return null;
                //MessageBox.Show(loError.Message, "Compiler Demo");
            }
        }

        #endregion miscellaneous

        #region RepeaterComponent   

        //public int GridDeleteTableRow(string InstanceID, int BOID, int iUserID, string sOrgName, string sDatabase)
        //{

        //}

        #endregion RepeaterComponent

        public VMResultList GetXIOneClickByID(int iOneClickID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            VMResultList oComponent = new VMResultList();
            oComponent.XIClickParams = new List<VMXI1ClickParameter>();
            XI1ClickParameterNDVs oNewCompo = new XI1ClickParameterNDVs();
            if (iOneClickID > 0)
            {
                var CompoResult = dbContext.XI1ClickParameterNDVs.Where(m => m.FKi1ClickID == iOneClickID).ToList();
                foreach (var item in CompoResult)
                {
                    VMXI1ClickParameter oXIClick = new VMXI1ClickParameter();
                    oXIClick.sName = item.sName;
                    oComponent.XIClickParams.Add(oXIClick);
                }
            }
            else
            {
                oComponent.XIClickParams.Clear();
            }
            return oComponent;
        }
    }
}
