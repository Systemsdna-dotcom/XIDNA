using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace XIDNA.Repository
{
    public class PopupRepository : IPopupRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        #region Tabs&Sections
        public VMCustomResponse SaveTab(Tabs model, string sDatabase)
        {
            if (model.ID == 0)
            {
                Tabs tab = new Tabs();
                tab.Name = model.Name;
                tab.StatusTypeID = model.StatusTypeID;
                tab.CreatedByID = 1;
                tab.CreatedByName = "Admin";
                tab.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.CreatedTime = DateTime.Now;
                tab.ModifiedByID = 1;
                tab.ModifiedByName = "Admin";
                tab.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.ModifiedTime = DateTime.Now;
                tab.PopupID = model.PopupID;
                tab.Rank = model.Rank;
                int rank = model.Rank;
                int maxrank = 0;
                var Tabs = dbContext.Tabs.ToList();
                if (Tabs.Count() > 0)
                {
                    maxrank = Tabs.Max(m => m.Rank);
                }
                var tabrank = dbContext.Tabs.Where(m => m.Rank == rank).FirstOrDefault();
                if (tabrank != null)
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Tabs] SET [Rank] = [Rank] + 1  WHERE [Rank] in (select [Rank] from[Tabs] where [Rank] BETWEEN " + rank + " and " + maxrank + ")");
                }
                dbContext.Tabs.Add(tab);
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = tab.ID, Status = true };
            }
            else
            {
                Tabs tab = dbContext.Tabs.Find(model.ID);
                tab.Name = model.Name;
                tab.StatusTypeID = model.StatusTypeID;
                tab.ModifiedByID = 1;
                tab.ModifiedByName = "Admin";
                tab.PopupID = model.PopupID;
                tab.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.ModifiedTime = DateTime.Now;
                var oldrank = tab.Rank;
                var newrank = model.Rank;
                if (oldrank > newrank)
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Tabs] SET [Rank] = [Rank] + 1  WHERE [Rank] in (select [Rank] from[Tabs] where [Rank] BETWEEN " + newrank + " and " + oldrank + ")");
                }
                else
                {
                    dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Tabs] SET [Rank] = [Rank] - 1  WHERE [Rank] in (select [Rank] from[Tabs] where [Rank] BETWEEN " + oldrank + " and " + newrank + ")");
                }
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = tab.ID, Status = true };
            }
        }

        public VMAssignReports SaveSection(Sections model, string sDatabase)
        {
            try
            {
                Sections Section = new Sections();
                Section = model;
                if (model.ID == 0)
                {
                    Sections Sec = new Sections();
                    int TabID = Section.TabID;
                    Sec.TabID = Section.TabID;
                    Sec.Name = Section.Name;
                    int rank = Section.Rank;
                    var maxrank = dbContext.Sections.Where(m => m.TabID == TabID).ToList();
                    int MaxValue = 0;
                    if (maxrank.Count() > 0)
                    {
                        MaxValue = maxrank.Max(m => m.Rank);
                    }
                    var tabrank = dbContext.Sections.Where(m => m.TabID == TabID).Where(m => m.Rank == rank).FirstOrDefault();
                    if (tabrank != null)
                    {
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Sections] SET [Rank] = [Rank] + 1  WHERE [Rank] in (select [Rank] from[Sections] where [Rank] BETWEEN " + rank + " and " + MaxValue + ") AND [TabID] = " + TabID);
                    }
                    Sec.Rank = Section.Rank;
                    if (Section.IsBespoke != false)
                    {
                        Sec.IsBespoke = Section.IsBespoke;
                        Sec.URL = Section.URL;
                        Sec.RefreshType = Section.RefreshType;
                        Sec.DisplayAs = 0;
                        Sec.ClassID = 0;
                        Sec.ReportID = 0;
                    }
                    else
                    {
                        Sec.IsBespoke = Section.IsBespoke;
                        Sec.URL = "Null";
                        Sec.RefreshType = null;
                        Sec.DisplayAs = 0;
                        Sec.ClassID = 0;
                        Sec.ReportID = 0;

                    }
                    Sec.StatusTypeID = Section.StatusTypeID;
                    dbContext.Sections.Add(Sec);
                    dbContext.SaveChanges();
                    Tab1Clicks tab1click = new Tab1Clicks();
                    tab1click.SectionID = Sec.ID.ToString();
                    tab1click.TabID = model.TabID;
                    if (Section.IsBespoke != false)
                    {
                        tab1click.IsBespoke = Section.IsBespoke;
                        tab1click.URL = Section.URL;
                        tab1click.RefreshType = Section.RefreshType;
                        tab1click.ClassID = 0;
                        tab1click.ReportID = 0;
                        tab1click.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
                    }
                    else
                    {
                        tab1click.IsBespoke = false;
                        tab1click.URL = "Null";
                        tab1click.RefreshType = Section.RefreshType;
                        tab1click.ClassID = Section.ClassID;
                        tab1click.ReportID = Section.ReportID;
                        tab1click.DisplayAs = Section.DisplayAs;
                    }
                    tab1click.IsView = model.IsView;
                    tab1click.IsCreate = model.IsCreate;
                    tab1click.IsEdit = model.IsEdit;
                    tab1click.StatusTypeID = model.StatusTypeID;
                    tab1click.CreatedByID = 1;
                    tab1click.CreatedByName = "Admin";
                    tab1click.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    tab1click.CreatedTime = DateTime.Now;
                    tab1click.ModifiedByID = 1;
                    tab1click.ModifiedByName = "Admin";
                    tab1click.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    tab1click.ModifiedTime = DateTime.Now;
                    dbContext.Tab1Clicks.Add(tab1click);
                    dbContext.SaveChanges();
                    Sec.Type = model.Type;
                    var result = GetTabPreview(tab1click, sDatabase);
                    result.TabID = model.TabID;
                    result.ReportID = model.ReportID;
                    result.Type = model.Type;
                    result.IsView = tab1click.IsView;
                    result.IsEdit = tab1click.IsEdit;
                    result.IsCreate = tab1click.IsCreate;
                    return result;
                }
                else
                {
                    Sections Sec = dbContext.Sections.Find(model.ID);
                    string secid = model.ID.ToString();
                    Tab1Clicks tab1click = dbContext.Tab1Clicks.Where(m => m.IsBespoke == Sec.IsBespoke).Where(m => m.SectionID == secid).FirstOrDefault();
                    Sec.Name = model.Name;
                    var oldrank = Sec.Rank;
                    var newrank = model.Rank;
                    if (oldrank > newrank)
                    {
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Sections] SET [Rank] = [Rank] + 1  WHERE [Rank] in (select [Rank] from[Sections] where [Rank] BETWEEN " + newrank + " and " + oldrank + ") AND [TabID] = " + model.TabID);
                    }
                    else
                    {
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Sections] SET [Rank] = [Rank] - 1  WHERE [Rank] in (select [Rank] from[Sections] where [Rank] BETWEEN " + oldrank + " and " + newrank + ") AND [TabID] = " + model.TabID);
                    }
                    dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Sections] SET [Rank] = " + newrank + " WHERE [ID] =" + model.ID + " AND [TabID] = " + model.TabID);
                    Sec.Rank = model.Rank;
                    if (model.IsBespoke != false)
                    {
                        Sec.IsBespoke = Section.IsBespoke;
                        Sec.URL = Section.URL;
                        Sec.RefreshType = Section.RefreshType;
                        Sec.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());

                    }
                    else
                    {
                        Sec.IsBespoke = Section.IsBespoke;
                        Sec.URL = "Null";
                        Sec.RefreshType = null;
                        Sec.DisplayAs = Section.DisplayAs;
                    }
                    Sec.StatusTypeID = Section.StatusTypeID;
                    dbContext.SaveChanges();
                    if (tab1click == null && model.IsBespoke == true)
                    {
                        Tab1Clicks click = new Tab1Clicks();
                        click.TabID = model.TabID;
                        click.SectionID = model.ID.ToString();
                        if (model.IsBespoke == true)
                        {
                            click.IsBespoke = Section.IsBespoke;
                            click.URL = Section.URL;
                            click.RefreshType = Section.RefreshType;
                            click.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
                            click.IsView = false;
                            click.IsEdit = false;
                            click.IsCreate = false;
                        }
                        else
                        {
                            click.IsBespoke = Section.IsBespoke;
                            click.URL = null;
                            click.RefreshType = null;
                            click.DisplayAs = 0;
                            click.ClassID = 0;
                            click.ReportID = 0;
                            click.IsView = model.IsView;
                            click.IsEdit = model.IsEdit;
                            click.IsCreate = model.IsCreate;
                        }
                        click.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
                        click.StatusTypeID = model.StatusTypeID;
                        click.CreatedByID = 1;
                        click.CreatedByName = "Admin";
                        click.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        click.CreatedTime = DateTime.Now;
                        click.ModifiedByID = 1;
                        click.ModifiedByName = "Admin";
                        click.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        click.ModifiedTime = DateTime.Now;
                        dbContext.Tab1Clicks.Add(click);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        if (model.IsBespoke == true)
                        {
                            tab1click.IsBespoke = Section.IsBespoke;
                            tab1click.URL = Section.URL;
                            tab1click.RefreshType = Section.RefreshType;
                            tab1click.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
                            tab1click.ClassID = 0;
                            tab1click.ReportID = 0;
                            tab1click.IsView = false;
                            tab1click.IsEdit = false;
                            tab1click.IsCreate = false;
                            tab1click.ViewFields = null;
                            tab1click.CreateFields = null;
                            tab1click.EditFields = null;
                            tab1click.StatusTypeID = model.StatusTypeID;
                        }
                        else
                        {
                            tab1click.IsBespoke = Section.IsBespoke;
                            tab1click.URL = "Null";
                            tab1click.RefreshType = null;
                            tab1click.ClassID = Section.ClassID;
                            tab1click.ReportID = Section.ReportID;
                            tab1click.DisplayAs = Section.DisplayAs;
                            tab1click.IsView = model.IsView;
                            tab1click.StatusTypeID = model.StatusTypeID;
                            if (!(model.IsView))
                            {
                                tab1click.ViewFields = null;
                            }
                            tab1click.IsEdit = model.IsEdit;
                            if (!(model.IsEdit))
                            {
                                tab1click.EditFields = null;
                            }
                            tab1click.IsCreate = model.IsCreate;
                            if (!(model.IsCreate))
                            {
                                tab1click.CreateFields = null;
                            }
                        }
                    }
                    dbContext.SaveChanges();
                    Sec.Type = model.Type;
                    var result = GetTabPreview(tab1click, sDatabase);
                    result.Type = model.Type;
                    result.TabID = model.TabID;
                    result.ReportID = model.ReportID;
                    result.IsView = tab1click.IsView;
                    result.IsEdit = tab1click.IsEdit;
                    result.IsCreate = tab1click.IsCreate;
                    result.ViewFields = tab1click.ViewFields;
                    result.CreateFields = tab1click.CreateFields;
                    result.EditFields = tab1click.EditFields;
                    return result;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DTResponse TabsList(jQueryDataTableParamModel param, string sDatabase)
        {
            IQueryable<Tabs> AllTabs;
            AllTabs = dbContext.Tabs;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTabs = AllTabs.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTabs.Count();
            AllTabs = QuerableUtil.GetResultsForDataTables(AllTabs, "", sortExpression, param);
            var clients = AllTabs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join p in dbContext.Popups on c.PopupID equals p.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.Name, p.Name, c.PopupID.ToString(), Convert.ToString(c.Rank), Convert.ToString(c.StatusTypeID), "Edit", "Delete"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<Sections> SectionsList(int TabID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sections = dbContext.Sections.Where(m => m.TabID == TabID).OrderBy(m => m.Rank).ToList();
            return sections;
        }
        public DTResponse TabSectionsList(jQueryDataTableParamModel param, int TabID, string sDatabase)
        {
            IEnumerable<Sections> AllSections, FilteredSections;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredSections = dbContext.Sections.Where(m => m.TabID == TabID).Where(m => m.Name.Contains(param.sSearch.ToUpper())).ToList();
                AllSections = FilteredSections.OrderBy(m => m.Rank).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredSections.Count();
            }
            else
            {
                displyCount = dbContext.Sections.Where(m => m.TabID == TabID).OrderBy(m => m.Rank).Count();
                AllSections = dbContext.Sections.Where(m => m.TabID == TabID).OrderBy(m => m.Rank).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllSections
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.TabID.ToString(), c.Name, Convert.ToString(c.Rank), Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<Tabs> GetTabsReports(int PopupID, string sDatabase)
        {
            var Tabs = dbContext.Tabs.Where(m => m.PopupID == PopupID).ToList();
            return Tabs;
        }
        public List<VMDropDown> GetCategoryDetails(string sDatabase)
        {
            List<VMDropDown> AllPopups = new List<VMDropDown>();
            AllPopups = (from c in dbContext.Popups.Where(m => m.StatusTypeID == 10).ToList() select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllPopups;
        }
        public Tabs EditTab(int TabID, string sDatabase)
        {
            Tabs tab = dbContext.Tabs.Find(TabID);
            return tab;
        }
        public int DeleteTab(int TabID, string sDatabase)
        {
            Tabs tab = dbContext.Tabs.Find(TabID);
            tab.StatusTypeID = 20;
            //dbContext.Tabs.Remove(tab);
            dbContext.SaveChanges();
            return TabID;
        }
        public DTResponse TabsReportsList(jQueryDataTableParamModel param, string sDatabase)
        {
            IEnumerable<Tab1Clicks> All1Clicks, Filtered1Clicks;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                Filtered1Clicks = dbContext.Tab1Clicks.ToList();
                All1Clicks = (from c in Filtered1Clicks
                              join t in dbContext.Tabs on c.TabID equals t.ID
                              where t.Name.ToUpper().Contains(param.sSearch.ToUpper())
                              select c).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                //AllTabs = FilteredTabs.Where(m => m.Name.Contains(param.sSearch.ToUpper())).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = All1Clicks.Count();
            }
            else
            {
                displyCount = dbContext.Tab1Clicks.OrderBy(m => m.ID).Count();
                All1Clicks = dbContext.Tab1Clicks.OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in All1Clicks
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), TabName(c.TabID,sDatabase), SectionName(c.SectionID,sDatabase), ((EnumDisplayTypes)c.DisplayAs).ToString(), ClassName(c.ClassID,sDatabase), ReportName(c.ReportID,sDatabase), Convert.ToString(c.StatusTypeID), "Edit", "Delete"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string ClassName(int p, string sDatabase)
        {
            var type = dbContext.Types.Where(m => m.ID == p).Where(m => m.Name == "Class Type").ToList().Select(m => m.Expression).FirstOrDefault();
            return type;
        }

        private string TabName(int p, string sDatabase)
        {
            var report = dbContext.Tabs.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return report;
        }
        private string ReportName(int p, string sDatabase)
        {
            var Report = dbContext.Reports.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return Report;
        }

        public List<VMDropDown> GetTabs(string Category, string sDatabase)
        {
            List<Tabs> tabs = new List<Tabs>();
            if (Category != null)
            {
                tabs = dbContext.Tabs.Where(m => m.PopupID == 0).ToList();
            }
            else
            {
                tabs = dbContext.Tabs.Where(m => m.PopupID == 0).ToList();
            }

            List<VMDropDown> AllTabs = new List<VMDropDown>();
            foreach (var items in tabs)
            {
                AllTabs.Add(new VMDropDown
                {
                    Value = items.ID,
                    text = items.Name
                });
            }
            return AllTabs;
        }
        public List<VMDropDown> GetClasses(int OrgID, string sDatabase)
        {
            List<VMDropDown> Classes = new List<VMDropDown>();
            Classes = ServiceUtil.GetOrgClasses(OrgID, sDatabase);
            return Classes;
        }
        public List<Classes> GetQueriesByType(int TabID, int DisplayAs, int ClassType, string SectionID, string sDatabase)
        {
            if ((SectionID == null) || (SectionID == "-1"))
            {
                var reports = dbContext.Reports.Where(m => m.Class == ClassType).Where(m => m.DisplayAs == DisplayAs).ToList();
                var tabreports = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).ToList();
                List<int> Assigned = new List<int>();
                foreach (var item in tabreports)
                {
                    Assigned.Add(item.ReportID);
                }
                List<int> AllReports = new List<int>();
                foreach (var items in reports)
                {
                    AllReports.Add(items.ID);
                }
                List<int> Remaining = new List<int>();
                Remaining = AllReports.Except(Assigned).ToList();
                List<Classes> Classes = new List<Classes>();
                foreach (var items in Remaining)
                {
                    var report = dbContext.Reports.Where(m => m.ID == items).FirstOrDefault();
                    Classes.Add(new Classes
                    {
                        value = report.ID,
                        text = report.Name
                    });
                }
                return Classes;
            }
            else
            {
                var reports = dbContext.Reports.Where(m => m.Class == ClassType).Where(m => m.DisplayAs == DisplayAs).ToList();
                var tabreports = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).Where(m => m.SectionID == SectionID).ToList();
                List<int> Assigned = new List<int>();
                foreach (var item in tabreports)
                {
                    Assigned.Add(item.ReportID);
                }
                List<int> AllReports = new List<int>();
                foreach (var items in reports)
                {
                    AllReports.Add(items.ID);
                }
                List<int> Remaining = new List<int>();
                Remaining = AllReports.Except(Assigned).ToList();
                List<Classes> Classes = new List<Classes>();
                foreach (var items in Remaining)
                {
                    var report = dbContext.Reports.Where(m => m.ID == items).FirstOrDefault();
                    Classes.Add(new Classes
                    {
                        value = report.ID,
                        text = report.Name
                    });
                }
                return Classes;
            }

        }
        public List<Classes> GetQueriesByTypeForEdit(int Tab1ClickID, string sDatabase)
        {
            var tab1clicks = dbContext.Tab1Clicks.Find(Tab1ClickID);
            var assignedReport = tab1clicks.ReportID;
            var classid = tab1clicks.ClassID;
            var display = tab1clicks.DisplayAs;
            var tabid = tab1clicks.TabID;
            var reports = dbContext.Reports.Where(m => m.Class == classid).Where(m => m.DisplayAs == display).ToList();
            var tabreports = dbContext.Tab1Clicks.Where(m => m.TabID == tabid).ToList();
            if (tab1clicks.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString()))
            {
                return null;
            }
            else
            {
                List<int> Assigned = new List<int>();
                foreach (var item in tabreports)
                {
                    Assigned.Add(item.ReportID);
                }
                List<int> AllReports = new List<int>();
                foreach (var items in reports)
                {
                    AllReports.Add(items.ID);
                }
                List<int> Remaining = new List<int>();
                Remaining = AllReports.Except(Assigned).ToList();
                Remaining.Add(assignedReport);
                List<Classes> Classes = new List<Classes>();
                foreach (var items in Remaining)
                {
                    var report = dbContext.Reports.Where(m => m.ID == items).FirstOrDefault();
                    Classes.Add(new Classes
                    {
                        value = report.ID,
                        text = report.Name
                    });
                }
                return Classes;
            }
        }

        public List<Classes> GetQueriesByTypeForSection(int DisplayAs, int ClassType, string sDatabase)
        {
            var reports = dbContext.Reports.Where(m => m.Class == ClassType).Where(m => m.DisplayAs == DisplayAs).ToList();
            //var tabreports = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).ToList();
            List<int> Assigned = new List<int>();
            //foreach (var item in tabreports)
            //{
            //    Assigned.Add(item.ReportID);
            //}
            List<int> AllReports = new List<int>();
            foreach (var items in reports)
            {
                AllReports.Add(items.ID);
            }
            List<int> Remaining = new List<int>();
            Remaining = AllReports.Except(Assigned).ToList();
            List<Classes> Classes = new List<Classes>();
            foreach (var items in Remaining)
            {
                var report = dbContext.Reports.Where(m => m.ID == items).FirstOrDefault();
                Classes.Add(new Classes
                {
                    value = report.ID,
                    text = report.Name
                });
            }
            return Classes;
        }
        public VMAssignReports SaveReportToTab(Tab1Clicks model, string sDatabase)
        {
            if (model.ID == 0)
            {
                Tab1Clicks tab = new Tab1Clicks();
                if (model.TabValue > 0)
                {
                    tab.TabID = model.TabValue;
                }
                else
                {
                    tab.TabID = model.TabID;
                }
                tab.ReportID = model.ReportID;
                tab.ClassID = model.ClassID;
                tab.OrganizationID = 0;
                tab.SectionID = model.SectionID;
                tab.DisplayAs = model.DisplayAs;
                tab.StatusTypeID = model.StatusTypeID;
                tab.CreatedByID = 1;
                tab.CreatedByName = "Admin";
                tab.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.CreatedTime = DateTime.Now;
                tab.ModifiedByID = 1;
                tab.ModifiedByName = "Admin";
                tab.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.ModifiedTime = DateTime.Now;
                tab.URL = "Null";
                dbContext.Tab1Clicks.Add(tab);
                dbContext.SaveChanges();
                var result = GetTabPreview(tab, sDatabase);
                return result;
            }
            else
            {
                Tab1Clicks tab = dbContext.Tab1Clicks.Find(model.ID);

                tab.StatusTypeID = model.StatusTypeID;
                tab.ModifiedByID = 1;
                tab.ModifiedByName = "Admin";
                tab.ModifiedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                tab.ModifiedTime = DateTime.Now;
                if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString()))
                {
                    tab.DisplayAs = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
                    tab.ReportID = 0;
                    tab.ClassID = 0;
                    tab.URL = model.URL;
                    tab.RefreshType = model.RefreshType;
                    tab.IsBespoke = model.IsBespoke;
                }
                else
                {
                    tab.IsBespoke = false;
                    tab.DisplayAs = model.DisplayAs;
                    tab.ReportID = model.ReportID;
                    tab.ClassID = model.ClassID;
                    tab.URL = "Null";
                    tab.RefreshType = null;
                }
                dbContext.SaveChanges();
                var result = GetTabPreview(tab, sDatabase);
                return result;
            }
        }

        public VMAssignReports GetTabPreview(Tab1Clicks tab, string sDatabase)
        {
            VMAssignReports TabReport = new VMAssignReports();
            string[] result;
            if (tab.SectionID == "-1" && tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ViewRecord.ToString()))
            {
                Reports report = dbContext.Reports.Find(tab.ReportID);
                string OrgSelectFields = "";
                List<string> SelectFields = new List<string>();
                if (report.EditableFields != null)
                {
                    OrgSelectFields = report.EditableFields;
                    SelectFields = report.EditableFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            result = items.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields groupname = dbContext.BOGroupFields.Where(m => m.ID == id).SingleOrDefault();
                            OrgSelectFields.Replace("{" + id + "}", groupname.BOFieldNames);
                        }
                    }
                    var SelFields = OrgSelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    TabReport.Fields = SelFields;
                    var sections = dbContext.Sections.Where(m => m.TabID == tab.TabID).ToList();
                    List<VMDropDown> AllSections = new List<VMDropDown>();
                    foreach (var items in sections)
                    {
                        AllSections.Add(new VMDropDown
                        {
                            Value = items.ID,
                            text = items.Name
                        });
                    }
                    TabReport.ID = tab.ID;
                    TabReport.TabSections = AllSections;
                    TabReport.TabID = tab.TabID;
                    TabReport.ReportType = EnumDisplayTypes.ViewRecord.ToString();
                    TabReport.IsView = tab.IsView;
                    TabReport.IsEdit = tab.IsEdit;
                    TabReport.IsCreate = tab.IsCreate;
                    return TabReport;
                }
                else { return TabReport; }
            }
            else if (tab.SectionID != null && tab.SectionID != "-1" && tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ViewRecord.ToString()))
            {
                Reports report = dbContext.Reports.Find(tab.ReportID);
                string OrgSelectFields = "";
                List<string> SelectFields = new List<string>();
                if (report.EditableFields != null)
                {
                    OrgSelectFields = report.EditableFields;
                    SelectFields = report.EditableFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            result = items.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields groupname = dbContext.BOGroupFields.Where(m => m.ID == id).SingleOrDefault();
                            OrgSelectFields.Replace("{" + id + "}", groupname.BOFieldNames);
                        }
                    }
                    var SelFields = OrgSelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    VMAssignReports TabsReport = new VMAssignReports();
                    var secid = Convert.ToInt32(tab.SectionID);
                    var sections = dbContext.Sections.Where(m => m.ID == secid).FirstOrDefault();
                    List<VMDropDown> AllSections = new List<VMDropDown>();
                    AllSections.Add(new VMDropDown
                    {
                        Value = sections.ID,
                        text = sections.Name
                    });
                    TabsReport.ID = tab.ID;
                    TabsReport.Fields = SelFields;
                    TabsReport.TabSections = AllSections;
                    TabsReport.TabID = tab.TabID;
                    TabsReport.ReportType = EnumDisplayTypes.ViewRecord.ToString();
                    TabReport.IsView = tab.IsView;
                    TabReport.IsEdit = tab.IsEdit;
                    TabReport.IsCreate = tab.IsCreate;
                    return TabsReport;
                }
                else { return TabReport; }
            }
            else if (tab.SectionID == null && tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ViewRecord.ToString()))
            {
                Reports report = dbContext.Reports.Find(tab.ReportID);
                string OrgSelectFields = "";
                List<string> SelectFields = new List<string>();
                if (report.EditableFields != null)
                {
                    OrgSelectFields = report.EditableFields;
                    SelectFields = report.EditableFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            result = items.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields groupname = dbContext.BOGroupFields.Where(m => m.ID == id).SingleOrDefault();
                            OrgSelectFields.Replace("{" + id + "}", groupname.BOFieldNames);
                        }
                    }
                    var SelFields = OrgSelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    VMAssignReports TabsReport = new VMAssignReports();
                    var secid = Convert.ToInt32(tab.SectionID);
                    List<VMDropDown> AllSections = new List<VMDropDown>();
                    var tabname = dbContext.Tabs.Where(m => m.ID == tab.TabID).Select(m => m.Name).FirstOrDefault();
                    //if (tab.SectionID == null)
                    //{
                    //    AllSections.Add(new VMDropDown
                    //    {
                    //        Value = tab.ID,
                    //        text = tabname +" Tab"
                    //    });
                    //}
                    TabReport.IsView = tab.IsView;
                    TabReport.IsEdit = tab.IsEdit;
                    TabReport.IsCreate = tab.IsCreate;
                    TabsReport.ID = tab.ID;
                    TabsReport.Fields = SelFields;
                    TabsReport.TabSections = AllSections;
                    TabsReport.TabID = tab.TabID;
                    TabsReport.ReportType = EnumDisplayTypes.ViewRecord.ToString();
                    return TabsReport;
                }
                else
                {
                    return TabReport;
                }
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ResultList.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = "Result List";
                TabsList.ReportID = tab.ReportID;
                Reports report = dbContext.Reports.Find(tab.ReportID);
                string OrgSelectFields = "";
                List<string> SelectFields = new List<string>();
                if (report.EditableFields != null)
                {
                    OrgSelectFields = report.EditableFields;
                    SelectFields = report.EditableFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            result = items.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields groupname = dbContext.BOGroupFields.Where(m => m.ID == id).SingleOrDefault();
                            OrgSelectFields.Replace("{" + id + "}", groupname.BOFieldNames);
                        }
                    }
                    var SelFields = OrgSelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    VMAssignReports TabsReport = new VMAssignReports();
                    var secid = Convert.ToInt32(tab.SectionID);
                    var sections = dbContext.Sections.Where(m => m.ID == secid).FirstOrDefault();
                    List<VMDropDown> AllSections = new List<VMDropDown>();
                    AllSections.Add(new VMDropDown
                    {
                        Value = sections.ID,
                        text = sections.Name
                    });
                    TabsReport.ID = tab.ID;
                    TabsReport.Fields = SelFields;
                    TabsReport.TabSections = AllSections;
                    TabsReport.TabID = tab.TabID;
                    TabsReport.ReportType = "Result List";
                    TabReport.IsView = tab.IsView;
                    TabReport.IsEdit = tab.IsEdit;
                    TabReport.IsCreate = tab.IsCreate;
                    return TabsReport;
                }
                else
                {
                    return TabsList;
                }
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = EnumDisplayTypes.KPICircle.ToString();
                TabsList.ReportID = tab.ReportID;
                return TabsList;
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = EnumDisplayTypes.PieChart.ToString();
                TabsList.ReportID = tab.ReportID;
                TabsList.ClassID = tab.ClassID;
                return TabsList;
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.BarChart.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = EnumDisplayTypes.BarChart.ToString();
                TabsList.ReportID = tab.ReportID;
                TabsList.ClassID = tab.ClassID;
                return TabsList;
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.LineChart.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = EnumDisplayTypes.LineChart.ToString();
                TabsList.ReportID = tab.ReportID;
                TabsList.ClassID = tab.ClassID;
                return TabsList;
            }
            else if (tab.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString()))
            {
                VMAssignReports TabsList = new VMAssignReports();
                TabsList.ReportType = EnumDisplayTypes.Bespoke.ToString();
                return TabsList;
            }
            else
            {
                return null;
            }
        }
        public VMQueryPreview GetQueryPreview(int QueryID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            Reports query = dbContext.Reports.Find(QueryID);
            string UserIDs = GetSubUsers(iUserID, OrganizationID, sDatabase);
            var Query = ReplaceQueryContent(query.Query, UserIDs, OrganizationID, sDatabase);
            string allfields = query.Query;
            List<string> AllHeadings = new List<string>();
            Reports model = new Reports();
            string Headings = query.Query;
            if (query.SelectFields != null)
            {
                model = GetHeadingsOfQuery(query, sDatabase);
                Query = model.Query;
                AllHeadings = null;
            }
            else if (Headings.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                Headings = Query.Substring(0, Query.IndexOf(" FROM", StringComparison.InvariantCultureIgnoreCase));
                Headings = Headings.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                model.SelectFields = Headings;
                model.Query = Query;
                model.BOID = query.BOID;
                model = GetHeadingsOfQuery(model, sDatabase);
                Query = model.Query;
                AllHeadings = null;
            }
            List<string[]> results = new List<string[]>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sDatabase);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                int count = reader.FieldCount;
                string[] rows = new string[count];

                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        values.Add(reader.GetValue(i).ToString());
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                }
                Con.Close();
            }
            VMQueryPreview vmquery = new VMQueryPreview();
            vmquery.Headings = AllHeadings;
            vmquery.Rows = results;
            return vmquery;
        }

        public Tab1Clicks EditTabReport(int ID, string sDatabase)
        {
            Tab1Clicks model = dbContext.Tab1Clicks.Find(ID);
            var type = dbContext.Types.Where(m => m.ID == model.ClassID).Select(m => m.Expression).FirstOrDefault();
            model.ClassName = type;
            var tabsections = dbContext.Sections.Where(m => m.TabID == model.TabID).ToList();
            List<VMDropDown> AllSections = new List<VMDropDown>();
            foreach (var items in tabsections)
            {
                AllSections.Add(new VMDropDown
                {
                    text = items.Name,
                    Value = items.ID
                });
            }
            AllSections.Insert(0, new VMDropDown
            {
                text = "All",
                Value = -1
            });
            model.Category = dbContext.Tabs.Where(m => m.ID == model.TabID).Select(m => m.PopupID).FirstOrDefault();
            model.TabSections = AllSections;

            return model;
        }
        public int DeleteTabReport(int ID, string sDatabase)
        {
            Tab1Clicks model = dbContext.Tab1Clicks.Find(ID);
            dbContext.Tab1Clicks.Remove(model);
            dbContext.SaveChanges();
            return ID;
        }
        public bool IsExistsTabName(string Name, int ID, int PopupID, string sDatabase)
        {
            var Tabs = dbContext.Tabs.ToList();
            Tabs tab = Tabs.Where(m => m.PopupID == PopupID).Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (tab != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (tab != null)
                {
                    if (ID == tab.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsExistsSectionName(string Name, int ID, int TabID, string sDatabase)
        {
            var Sections = dbContext.Sections.ToList();
            Sections section = Sections.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.TabID == TabID).FirstOrDefault();
            if (ID == 0)
            {
                if (section != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (section != null)
                {
                    if (ID == section.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public DTResponse TabSpecificList(jQueryDataTableParamModel param, int Tab, string sDatabase)
        {
            IEnumerable<Tab1Clicks> AllTabReports, FilteredTabReports;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredTabReports = dbContext.Tab1Clicks.ToList();
                AllTabReports = (from c in FilteredTabReports
                                 join t in dbContext.Tabs on c.TabID equals t.ID
                                 where t.Name.ToUpper().Contains(param.sSearch.ToUpper())
                                 select c).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = AllTabReports.Count();
            }
            else
            {
                displyCount = dbContext.Tab1Clicks.Where(m => m.TabID == Tab).OrderBy(m => m.ID).Count();
                AllTabReports = dbContext.Tab1Clicks.Where(m => m.TabID == Tab).OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllTabReports
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), TabName(c.TabID,sDatabase), SectionName(c.SectionID,sDatabase), ((EnumDisplayTypes)c.DisplayAs).ToString(), ClassName(c.ClassID,sDatabase), ReportName(c.ReportID,sDatabase), Convert.ToString(c.StatusTypeID)};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string SectionName(string SecID, string sDatabase)
        {
            if (SecID == null)
            {
                return "";
            }
            else if (SecID.Contains(','))
            {
                return "All";
            }
            else
            {
                var id = Convert.ToInt32(SecID);
                var secname = dbContext.Sections.Where(m => m.ID == id).Select(m => m.Name).FirstOrDefault();
                return secname;
            }
        }
        public List<Classes> GetAssignedClasses(int Tab, string sDatabase)
        {
            var tabclasses = dbContext.Tab1Clicks.Where(m => m.TabID == Tab).ToList();
            List<Classes> classes = new List<Classes>();
            foreach (var items in tabclasses)
            {
                var type = dbContext.Types.Where(m => m.ID == items.ClassID).Where(m => m.Name == "Class Type").ToList().Select(m => m.Expression).FirstOrDefault();
                classes.Add(new Classes
                {
                    value = items.ClassID,
                    text = type
                });
            }
            return classes;
        }
        public List<Classes> GetAssignedSecClasses(int SectionID, int TabID, string sDatabase)
        {
            var tab1clicks = dbContext.Tab1Clicks.ToList();
            var types = dbContext.Types.ToList();
            var sectionid = SectionID.ToString();
            List<Classes> classes = new List<Classes>();
            if (SectionID == -1)
            {
                var ClassID = tab1clicks.Where(m => m.TabID == TabID).Where(m => m.SectionID.Contains(',')).Select(m => m.ClassID).ToList();
                if (ClassID.Count() > 0)
                {
                    foreach (var items in ClassID)
                    {
                        var type = types.Where(m => m.ID == items).Where(m => m.Name == "Class Type").Select(m => m.Expression).FirstOrDefault();
                        classes.Add(new Classes
                        {
                            value = items,
                            text = type
                        });
                    }
                }
                else
                {
                    var tabclasses = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).Select(m => m.ClassID).ToList();
                    foreach (var items in tabclasses)
                    {
                        var type = types.Where(m => m.ID == items).Where(m => m.Name == "Class Type").Select(m => m.Expression).FirstOrDefault();
                        classes.Add(new Classes
                        {
                            value = items,
                            text = type
                        });
                    }
                }
            }
            else
            {
                var ClassID = tab1clicks.Where(m => m.TabID == TabID).Where(m => m.SectionID.Contains(',')).Select(m => m.ClassID).ToList();
                if (ClassID.Count() > 0)
                {
                    foreach (var items in ClassID)
                    {
                        var type = types.Where(m => m.ID == items).Where(m => m.Name == "Class Type").Select(m => m.Expression).FirstOrDefault();
                        classes.Add(new Classes
                        {
                            value = items,
                            text = type
                        });
                    }
                }
                else
                {
                    var tabclasses = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).Where(m => m.SectionID == sectionid).ToList();
                    foreach (var items in tabclasses)
                    {
                        var type = types.Where(m => m.ID == items.ClassID).Where(m => m.Name == "Class Type").Select(m => m.Expression).FirstOrDefault();
                        classes.Add(new Classes
                        {
                            value = items.ClassID,
                            text = type
                        });
                    }
                }
            }

            return classes;

        }
        public List<VMDropDown> GetAllRanks(int PopupID, string sDatabase)
        {
            List<VMDropDown> AllTabs = new List<VMDropDown>();
            var ranks = dbContext.Tabs.Where(m => m.PopupID == PopupID).OrderByDescending(m => m.Rank).ToList();
            if (ranks.Count() == 0)
            {
                AllTabs.Insert(0, new VMDropDown
                {
                    Value = 1,
                    text = "1"
                });
                return AllTabs;
            }
            foreach (var items in ranks)
            {
                AllTabs.Add(new VMDropDown
                {
                    Value = items.Rank,
                    text = items.Rank.ToString()
                });
            }
            if (PopupID == 0)
            {
                var maxrank = ranks.Max(m => m.Rank);
                maxrank = maxrank + 1;
                AllTabs.Insert(0, new VMDropDown
                {
                    Value = maxrank,
                    text = maxrank.ToString()
                });
            }
            return AllTabs;
        }
        public List<VMDropDown> GetAllSecRanks(int TabID, string sDatabase)
        {
            List<VMDropDown> AllRanks = new List<VMDropDown>();
            if (TabID != 0)
            {
                var ranks = dbContext.Sections.Where(m => m.TabID == TabID).OrderByDescending(m => m.Rank).ToList();
                if (ranks.Count != 0)
                {
                    foreach (var items in ranks)
                    {
                        AllRanks.Add(new VMDropDown
                        {
                            Value = items.Rank,
                            text = items.Rank.ToString()
                        });
                    }
                    var maxrank = dbContext.Sections.Where(m => m.TabID == TabID).Max(m => m.Rank);
                    maxrank = maxrank + 1;
                    AllRanks.Insert(0, new VMDropDown
                    {
                        Value = maxrank,
                        text = maxrank.ToString()
                    });
                }
                else
                {
                    AllRanks.Insert(0, new VMDropDown
                    {
                        Value = 1,
                        text = "1"
                    });
                }
                return AllRanks;
            }
            else
            {
                AllRanks.Insert(0, new VMDropDown
                {
                    Value = 1,
                    text = "1"
                });
                return AllRanks;
            }
        }
        public List<VMDropDown> GetRanksByCategory(int Category, string sDatabase)
        {
            List<VMDropDown> AllRanks = new List<VMDropDown>();
            if (Category != 0)
            {
                var ranks = dbContext.Tabs.Where(m => m.PopupID == Category).OrderByDescending(m => m.Rank).ToList();
                if (ranks.Count != 0)
                {
                    foreach (var items in ranks)
                    {
                        AllRanks.Add(new VMDropDown
                        {
                            Value = items.Rank,
                            text = items.Rank.ToString()
                        });
                    }
                    var maxrank = dbContext.Tabs.Where(m => m.PopupID == Category).Max(m => m.Rank);
                    maxrank = maxrank + 1;
                    AllRanks.Insert(0, new VMDropDown
                    {
                        Value = maxrank,
                        text = maxrank.ToString()
                    });
                }
                else
                {
                    AllRanks.Insert(0, new VMDropDown
                    {
                        Value = 1,
                        text = "1"
                    });
                }
                return AllRanks;
            }
            else
            {
                AllRanks.Insert(0, new VMDropDown
                {
                    Value = 1,
                    text = "1"
                });
                return AllRanks;
            }
        }
        public List<VMDropDown> GetTabsByCategory(int Category, string sDatabase)
        {
            List<VMDropDown> AllRanks = new List<VMDropDown>();
            if (Category != 0)
            {
                var Tabs = dbContext.Tabs.Where(m => m.PopupID == Category).OrderBy(m => m.Rank).ToList();
                if (Tabs.Count != 0)
                {
                    foreach (var items in Tabs)
                    {
                        AllRanks.Add(new VMDropDown
                        {
                            Value = items.ID,
                            text = items.Name
                        });
                    }
                }
            }
            return AllRanks;
        }
        public List<VMDropDown> GetCategoryTabs(int TabID, string sDatabase)
        {
            List<VMDropDown> AllRanks = new List<VMDropDown>();
            var Category = dbContext.Tabs.Where(m => m.ID == TabID).Select(m => m.PopupID).FirstOrDefault();
            if (Category != 0)
            {
                var Tabs = dbContext.Tabs.Where(m => m.PopupID == Category).OrderBy(m => m.Rank).ToList();
                if (Tabs.Count != 0)
                {
                    foreach (var items in Tabs)
                    {
                        AllRanks.Add(new VMDropDown
                        {
                            Value = items.ID,
                            text = items.Name
                        });
                    }
                }
            }
            return AllRanks;
        }
        public List<VMDropDown> GetAllSectionRanks(int TabID, string sDatabase)
        {
            var sections = dbContext.Sections.Where(m => m.TabID == TabID).ToList();
            List<VMDropDown> AllSecRanks = new List<VMDropDown>();
            foreach (var items in sections)
            {
                AllSecRanks.Add(new VMDropDown
                {
                    text = items.Rank.ToString(),
                    Value = items.Rank
                });
            }
            return AllSecRanks;
        }
        public List<VMDropDown> GetAllTabSections(int ID, string sDatabase)
        {
            var sections = dbContext.Sections.Where(m => m.TabID == ID).ToList();
            List<VMDropDown> AllSections = new List<VMDropDown>();
            foreach (var items in sections)
            {
                AllSections.Add(new VMDropDown
                {
                    text = items.Name,
                    Value = items.ID
                });
            }
            return AllSections;
        }

        public Sections GetSectionDetails(int SectionID, string sDatabase)
        {
            var section = SectionID.ToString();
            Sections model = dbContext.Sections.Find(SectionID);
            Tab1Clicks tab1click = dbContext.Tab1Clicks.Where(m => m.SectionID == section).FirstOrDefault();
            model.ClassID = tab1click.ClassID;
            model.ReportID = tab1click.ReportID;
            model.DisplayAs = tab1click.DisplayAs;
            model.IsView = tab1click.IsView;
            model.IsCreate = tab1click.IsCreate;
            model.IsEdit = tab1click.IsEdit;
            model.Reports = GetQueriesByTypeForSection(tab1click.DisplayAs, tab1click.ClassID, sDatabase);
            model.ViewFields = tab1click.ViewFields;
            model.CreateFields = tab1click.CreateFields;
            model.EditFields = tab1click.EditFields;
            model.ReportType = ((EnumDisplayTypes)tab1click.DisplayAs).ToString();
            var result = GetTabPreview(tab1click, sDatabase);
            model.TabPreview = result;
            return model;
        }

        public List<VMDropDown> GetTabSections(int TabID, string sDatabase)
        {
            List<Sections> model = dbContext.Sections.Where(m => m.TabID == TabID).ToList();
            List<VMDropDown> AllSections = new List<VMDropDown>();
            foreach (var items in model)
            {
                AllSections.Add(new VMDropDown
                {
                    text = items.Name,
                    Value = items.ID
                });
            }
            return AllSections;
        }
        public string SaveSectionFields(Sections model, string sDatabase)
        {
            var Fields = model.Fields;
            var sectionids = model.SectionIDs;
            //var sections = ""; 
            string vfields = "", cfields = "", efields = "";
            if (model.ViFields != null && model.ViFields.Count() > 0)
            {
                vfields = string.Join(", ", model.ViFields);
            }
            if (model.EdFields != null && model.EdFields.Count() > 0)
            {
                efields = string.Join(", ", model.EdFields);
            }
            if (model.CrFields != null && model.CrFields.Count() > 0)
            {
                cfields = string.Join(", ", model.CrFields);
            }


            //for (int i = 0; i < sectionids.Count(); i++)
            //{
            //    if (sectionids[i] == "View")
            //    {
            //        vfields = Fields[i];
            //    }
            //    else if (sectionids[i] == "Create")
            //    {
            //        cfields = Fields[i];
            //    }
            //    else if (sectionids[i] == "Edit")
            //    {
            //        efields = Fields[i];
            //    }
            //}
            var tab1click = dbContext.Tab1Clicks.Find(model.ID);
            if (vfields != "" && vfields != null)
            {
                tab1click.ViewFields = vfields;
            }
            else
            {
                tab1click.ViewFields = null;
            }
            if (cfields != "" && cfields != null)
            {
                tab1click.CreateFields = cfields;
            }
            else
            {
                tab1click.CreateFields = null;
            }
            if (efields != "" && efields != null)
            {
                tab1click.EditFields = efields;
            }
            else
            {
                tab1click.EditFields = null;
            }
            dbContext.SaveChanges();
            return model.Type;
        }
        #endregion Tabs&Sections

        #region PreviewGraphs

        public List<VMKPIResult> GetKPICircleResult(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            string Query = "";
            int target = 0;
            //int RoleID = dbContext.AspNetUserGroups.Where(m => m.UserId == UserID).Select(m => m.RoleId).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).Where(m => m.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString())).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<VMKPIResult> KPIs = new List<VMKPIResult>();
            KPICircleColors colors = new KPICircleColors();
            KPIIconColors iconscolor = new KPIIconColors();
            List<string> color = new List<string>();
            List<string> iconcolor = new List<string>();
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
            foreach (var items in Reports)
            {
                Reports report = dbContext.Reports.Find(items.ID);
                Query = report.Query;
                //UserReports ureport = dbContext.UserReports.Where(m => m.RoleID == items.RoleID).Where(m => m.ReportID == items.ReportID).SingleOrDefault();
                target = 10;
                string UserIDs = GetSubUsers(iUserID, OrganizationID, sDatabase);
                if (Query != null && Query.Length > 0)
                {
                    var NewQuery = ReplaceQueryContent(Query, UserIDs, OrganizationID, sDatabase);
                    string[] value = null;
                    VMKPIResult kpi = new VMKPIResult();
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        Con.Open();
                        Con.ChangeDatabase(sDatabase);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        cmd.CommandText = NewQuery;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<string[]> results = new List<string[]>();
                        int count = reader.FieldCount;
                        string[] rows = new string[count];
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
                    int com = Convert.ToInt32(value[0]);
                    double percentage = (double)com / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    kpi.Name = report.Name;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = "fa fa-car";
                    KPIs.Add(kpi);
                    j++;
                }
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
                KPIs.FirstOrDefault().AllLeadsCount = AllLeads;
            }
            return KPIs;
        }
        public List<DashBoardGraphs> GetLeadsBySource(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            string UserIDs = GetSubUsers(iUserID, OrganizationID, sDatabase);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).Where(m => m.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString())).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            string[] colors = { "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277", "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277" };
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Find(items.ID);
                string Query = model.Query;
                var NewQuery = ReplaceQueryContent(Query, UserIDs, OrganizationID, sDatabase);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = NewQuery;
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
            }
            int j = 0;
            foreach (var items in results)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    DashBoardGraphs model = new DashBoardGraphs();
                    model.value = Convert.ToInt32(items[i]);
                    model.label = "Source" + j;
                    model.color = colors[j];
                    model.highlight = "#1ab394";
                    list.Add(model);
                }
                j++;
            }
            return list;
        }
        public List<DashBoardGraphs> GetLeadsByClass(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            string UserIDs = GetSubUsers(iUserID, OrganizationID, sDatabase);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).Where(m => m.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString())).ToList();
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
                Reports model = dbContext.Reports.Find(items.ID);
                string Query = model.Query;
                var NewQuery = ReplaceQueryContent(Query, UserIDs, OrganizationID, sDatabase);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = NewQuery;
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
        public List<DashBoardGraphs> GetBarChartResult(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            string UserIDs = GetSubUsers(iUserID, OrganizationID, sDatabase);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).Where(m => m.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.BarChart.ToString())).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            string[] colors = { "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277", "#a3e1d4", "#dedede", "#D67913", "#b5b8cf", "#85B786", "#CAA277", "#a3e1d4", "#dedede", "#b5b8cf", "#85B786", "#D67913", "#CAA277" };
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Find(items.ID);
                string Query = model.Query;
                var NewQuery = ReplaceQueryContent(Query, UserIDs, OrganizationID, sDatabase);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = NewQuery;
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
            }
            int j = 0;
            foreach (var items in results)
            {
                for (int i = 0; i < items.Count(); i++)
                {
                    DashBoardGraphs model = new DashBoardGraphs();
                    model.value = Convert.ToInt32(items[i]);
                    model.label = "Source" + j;
                    model.color = colors[j];
                    model.highlight = "#1ab394";
                    list.Add(model);
                }
                j++;
            }
            return list;
        }
        public LineGraph GetLineGraphForTab(int ReportID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            string Query = "";
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<string[]> results = new List<string[]>();
            foreach (var items in Reports)
            {
                Reports report = dbContext.Reports.Find(ReportID);
                Query = report.Query;
                string UserIDs = GetSubUsers(iUserID, OrgID, sDatabase);
                if (Query != null && Query.Length > 0)
                {
                    var NewQuery = ReplaceQueryContent(Query, UserIDs, OrgID, sDatabase);
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        Con.Open();
                        Con.ChangeDatabase(database);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        cmd.CommandText = NewQuery;
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
                }
            }
            LineGraph line = new LineGraph();
            //line.Data = results;
            return line;
        }
        #endregion PreviewGraphs

        #region miscellaneous
        public Tab1Clicks GetSectionType(int SectionID, string sDatabase)
        {
            var section = dbContext.Sections.Where(m => m.ID == SectionID).Select(m => m.IsBespoke).FirstOrDefault();
            if (section == true)
            {
                string id = SectionID.ToString();
                var tab1click = dbContext.Tab1Clicks.Where(m => m.SectionID == id).Where(m => m.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString())).FirstOrDefault();
                return tab1click;
            }
            else
            {
                Tab1Clicks tab1click = new Tab1Clicks();
                return tab1click;
            }
        }

        private string ReplaceQueryContent(string Query, string UserID, int OrganizationID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            if (Query.IndexOf("'Yesterday'") > 0)
            {
                Query = Query.Replace("'Yesterday'", "DATEADD(day,datediff(day,1,GETDATE()),0)");
            }
            if (Query.IndexOf("'Today'") > 0)
            {
                Query = Query.Replace("'Today'", "GETDATE()");
            }
            if (Query.IndexOf("'Tomorrow'") > 0)
            {
                Query = Query.Replace("'Tomorrow'", "DATEADD(day, 1, GETDATE())");
            }
            if (Query.IndexOf("WHERE") > 0)
            {
                if (Query.Contains("FROM {Leads}"))
                {
                    Query = Query.Replace("FROM {Leads}", "FROM Leads");
                }
                if (Query.Contains("UserID = {CurrentUser}"))
                {
                    Query = Query.Replace("UserID = {CurrentUser}", "UserID IN(" + UserID + ")");
                }
                if (Query.Contains("FKiOrgID = {CurrentOrganization}"))
                {
                    Query = Query.Replace("FKiOrgID = {CurrentOrganization}", "FKiOrgID=" + OrganizationID);
                }
            }
            return Query;
        }

        private Reports GetHeadingsOfQuery(Reports Report, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            Reports model = new Reports();
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == Report.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext SpDb = new DataContext(database);
            List<string> AllHeadings = new List<string>();
            string Query = Report.Query;
            if (Report.SelectFields.Length > 0)
            {
                List<string> Headings = new List<string>();
                if (Report.SelectFields.IndexOf(", ") >= 0)
                {
                    Headings = Report.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    Headings = Report.SelectFields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        string id = items.Substring(1, items.Length - 2);
                        int gid = Convert.ToInt32(id);
                        string groupid = Convert.ToString(gid);
                        BOGroupFields fields = dbContext.BOGroupFields.Find(gid);
                        Query = Query.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                        if (fields.IsMultiColumnGroup == true)
                        {
                            List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                string aliasname = dbContext.BOFields.Where(m => m.Name.Equals(names, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.BOID == Report.BOID).Select(m => m.LabelName).FirstOrDefault();
                                AllHeadings.Add(aliasname);
                            }
                        }
                        else
                        {
                            AllHeadings.Add(fields.GroupName);
                        }
                    }
                    else if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        var fieldname = Regex.Split(items, "AS ", RegexOptions.IgnoreCase)[1];
                        //fieldname = fieldname.Substring(1, fieldname.Length - 1);
                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                        AllHeadings.Add(fieldname);
                    }
                    else if (items.Contains('.'))
                    {
                        var fieldname = items.Split('.')[1];
                        string aliasname = dbContext.BOFields.Where(m => m.Name.Equals(fieldname, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.BOID == Report.BOID).Select(m => m.LabelName).FirstOrDefault();
                        if (aliasname == null)
                        {
                            AllHeadings.Add(fieldname);
                        }
                        else
                        {
                            AllHeadings.Add(aliasname);
                        }
                    }
                    else
                    {
                        string aliasname = "";
                        if (Report.OrganizationID != 0)
                        {
                            aliasname = SpDb.MappedFields.Where(m => m.AddField == items && m.OrganizationID == Report.OrganizationID).Select(m => m.FieldName).FirstOrDefault();
                        }
                        else
                        {
                            aliasname = dbContext.BOFields.Where(m => m.Name.Equals(items, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.BOID == Report.BOID).Select(m => m.LabelName).FirstOrDefault();
                        }
                        if (aliasname == null)
                        {
                            aliasname = dbContext.BOFields.Where(m => m.Name.Equals(items, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.BOID == Report.BOID).Select(m => m.LabelName).FirstOrDefault();
                        }
                        if (aliasname == null)
                        {
                            AllHeadings.Add(items);
                        }
                        else
                        {
                            AllHeadings.Add(aliasname);
                        }
                    }
                }
            }
            model.Query = Query;
            //model.selectrightfields = AllHeadings;
            return model;
        }

        private string GetSubUsers(int UserID, int OrganizationID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string database = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            string Locations = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sLocation).FirstOrDefault();
            var rolename = dbCore.XIAppRoles.Where(m => m.RoleID == UserRoleID).Select(m => m.sRoleName).FirstOrDefault();
            if (rolename != EnumRoles.Admin.ToString())
            {
                Locations = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sLocation).FirstOrDefault();
            }
            else
            {
                var allLocations = Spdb.OrganizationLocations.Select(m => m.ID).ToList();
                foreach (var items in allLocations)
                {
                    Locations = Locations + items + ", ";
                }
                Locations = Locations.Substring(0, Locations.Length - 2);
            }
            var roles = GetSubRoles(UserRoleID, sDatabase);
            string UserIDs = Convert.ToString(UserID) + ", ";
            foreach (var role in roles)
            {
                List<int> ChildUsers = dbCore.XIAppUserRoles.Where(m => m.RoleID == role).Select(m => m.UserID).ToList();
                foreach (var user in ChildUsers)
                {
                    string userloc = dbCore.XIAppUsers.Where(m => m.UserID == user).Select(m => m.sLocation).FirstOrDefault();
                    if (rolename == EnumRoles.Admin.ToString())
                    {
                        if (Locations.Contains(userloc))
                        {
                            UserIDs = UserIDs + user + ", ";
                        }
                    }
                    else
                    {
                        if (Locations.Contains(userloc))
                        {
                            UserIDs = UserIDs + user + ", ";
                        }

                    }
                }
            }
            UserIDs = UserIDs.Substring(0, UserIDs.Length - 2);
            return UserIDs;
        }

        public List<int> GetSubRoles(int RoleID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<int> SubRolesIDs = new List<int>();
            cXIAppRoles Role = dbCore.XIAppRoles.Where(s => s.RoleID == RoleID).FirstOrDefault();
            List<cXIAppRoles> Rolelist = Role.SubGroups.ToList();
            List<cXIAppRoles> SubRoles = new List<cXIAppRoles>();
            //SubCategoriesIDs.Add(RoleID);
            do
            {
                SubRoles.Clear();
                foreach (var item in Rolelist)
                {
                    if (item.IsLeaf)
                    {
                        SubRolesIDs.Add(item.RoleID);
                    }
                    else
                    {
                        SubRolesIDs.Add(item.RoleID);
                        SubRoles.AddRange(item.SubGroups);
                    }
                }
                Rolelist.Clear();
                Rolelist.AddRange(SubRoles);
            } while (Rolelist.Count != 0);

            return SubRolesIDs;
        }

        public int DeleteSection(int TabID, int SectionID, string sDatabase)
        {
            var AllSections = dbContext.Sections.ToList();
            Sections section = AllSections.Where(m => m.TabID == TabID).Where(m => m.ID == SectionID).FirstOrDefault();
            var rank = section.Rank;
            dbContext.Sections.Remove(section);
            dbContext.SaveChanges();
            var newsections = AllSections.Where(m => m.Rank > rank && m.TabID == TabID).ToList();
            if (newsections.Count() > 0)
            {
                dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[Sections] SET [Rank] = [Rank] - 1  WHERE [Rank] in (select [Rank] from[Sections] where [Rank] >" + rank + ")");
            }
            dbContext.SaveChanges();
            var secid = SectionID.ToString();
            Tab1Clicks click = dbContext.Tab1Clicks.Where(m => m.TabID == TabID && m.SectionID == secid).FirstOrDefault();
            dbContext.Tab1Clicks.Remove(click);
            dbContext.SaveChanges();
            return TabID;
        }
        public int SaveCallsContent(string Desc, int LeadID, int orgid, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == orgid).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            Calls call = new Calls();
            call.Description = Desc;
            call.FKiLeadID = LeadID;
            call.FKiOrganizationID = orgid;
            Spdb.Calls.Add(call);
            Spdb.SaveChanges();
            return call.ID;
        }
        public VMLeadRecord EditLeadRecord(int LeadID, int ReportID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            VMLeadRecord Record = new VMLeadRecord();
            List<string> Datatypes = new List<string>();
            List<string> Lengths = new List<string>();
            var report = dbContext.Reports.Find(ReportID);
            var selectfields = report.SelectFields;
            var Labels = selectfields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (var items in Labels)
            {
                var bofield = dbContext.BOFields.Where(m => m.BOID == report.BOID).Where(m => m.Name == items).FirstOrDefault();
                string type = ((BODatatypes)bofield.TypeID).ToString();
                Datatypes.Add(type);
                Lengths.Add(bofield.MaxLength);
            }
            var Query = "Select " + selectfields + " From " + EnumLeadTables.Leads.ToString() + " Where ID=" + LeadID;
            List<string> results = new List<string>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(database);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                while (reader.Read())
                {
                    string values = "";
                    for (int i = 0; i < count; i++)
                    {
                        values = reader.GetValue(i).ToString();
                        results.Add(values);
                    }
                }
                Con.Close();
            }
            Record.LeadID = LeadID;
            Record.Labels = Labels;
            Record.Values = results;
            Record.DataTypes = Datatypes;
            Record.Lengths = Lengths;
            Record.ReportID = ReportID;
            return Record;
        }
        public VMLeadRecord SaveLeadRecord(VMLeadRecord model, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            var orgname = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.Name).FirstOrDefault();
            var tablename = EnumLeadTables.Leads.ToString() + orgname.Substring(0, 3) + OrgID;
            for (int i = 0; i < model.Labels.Count(); i++)
            {
                if (model.Labels[i] != "ID")
                {
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        cmd.CommandText = "UPDATE " + tablename + " SET" + " " + model.Labels[i] + "=" + "'" + model.Values[i] + "'" + " " + "WHERE" + " ID=" + model.LeadID + "";
                        cmd.Parameters.Add("@ID", SqlDbType.Int).Value = model.LeadID;
                        Con.Open();
                        Con.ChangeDatabase(database);
                        cmd.Parameters.AddWithValue(model.Labels[i], model.Values[i]);
                        cmd.ExecuteNonQuery();
                        Con.Dispose();
                    }
                }
            }

            var result = EditLeadRecord(model.LeadID, model.ReportID, OrgID, sDatabase);
            //return result;
            return result;
        }

        #endregion miscellaneous

        #region Popups
        //creation of popup repository
        public VMCustomResponse CreatePopup(Popup model, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            Popup p = new Popup();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            if (model.ID == 0)
            {
                p.FKiApplicationID = FKiAppID;
                p.OrganizationID = iOrgID;
                p.ParentID = model.ParentID;
                p.Name = model.Name;
                p.LayoutID = model.LayoutID;
                p.StatusTypeID = model.StatusTypeID;
                p.IsGrouping = model.IsGrouping;
                p.BarPosition = model.BarPosition;
                p.PopupSize = model.PopupSize;
                if (model.PopupSize == "Specific")
                {
                    p.PopupWidth = model.PopupWidth;
                    p.PopupHeight = model.PopupHeight;
                }
                if (model.IsFKPopup)
                {
                    p.IsFKPopup = model.IsFKPopup;
                    p.BOID = model.BOID;
                    p.FKColumnID = model.FKColumnID;
                }
                else
                {
                    p.IsFKPopup = model.IsFKPopup;
                    p.BOID = 0;
                    p.FKColumnID = 0;
                }
                if (model.IsLeftMenu)
                {
                    p.IsLeftMenu = model.IsLeftMenu;
                }
                else
                {
                    p.IsLeftMenu = false;
                }
                dbContext.Popups.Add(p);
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = p.ID, Status = true };
            }
            else
            {
                p = dbContext.Popups.Find(model.ID);
                p.FKiApplicationID = model.FKiApplicationID;
                p.OrganizationID = model.OrganizationID;
                int OldLayoutID = p.LayoutID;
                int NewLayoutID = model.LayoutID;
                p.Name = model.Name;
                p.ParentID = model.ParentID;
                p.LayoutID = model.LayoutID;
                p.StatusTypeID = model.StatusTypeID;
                p.IsGrouping = model.IsGrouping;
                if (p.IsGrouping == false)
                {
                    p.BarPosition = null;
                }
                else
                {
                    p.BarPosition = model.BarPosition;
                }
                p.PopupSize = model.PopupSize;
                if (model.PopupSize == "Specific")
                {
                    p.PopupWidth = model.PopupWidth;
                    p.PopupHeight = model.PopupHeight;
                }
                else
                {
                    p.PopupWidth = 0;
                    p.PopupHeight = 0;
                }

                if (p.IsFKPopup)
                {
                    p.BOID = model.BOID;
                    p.FKColumnID = model.FKColumnID;
                }
                else
                {
                    p.BOID = 0;
                    p.FKColumnID = 0;
                }
                if (model.IsLeftMenu)
                {
                    p.IsLeftMenu = model.IsLeftMenu;
                }
                else
                {
                    p.IsLeftMenu = false;
                }
                dbContext.SaveChanges();
                if (OldLayoutID != NewLayoutID)
                {
                    DeleteExistingMappings(p.ID, sDatabase);
                }
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = p.ID, Status = true };
            }
        }

        private void DeleteExistingMappings(int p, string sDatabase)
        {
            var Mappings = dbContext.PopupLayoutMappings.Where(m => m.PopupID == p).ToList();
            foreach (var items in Mappings)
            {
                dbContext.PopupLayoutMappings.Remove(items);
                dbContext.SaveChanges();
            }
        }
        //Editing of Popup Repository
        public Popup EditPopup(int PopupID, string sDatabase)
        {
            Popup p = new Popup();
            p = dbContext.Popups.Find(PopupID);
            return p;
        }

        //getting popup values
        public Popup GetPopupValues(int iUserID, string sOrgName, string sDatabase)
        {
            Popup model = new Popup();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            List<VMDropDown> Layouts = new List<VMDropDown>();
            Layouts = dbContext.Layouts.Where(m => m.FKiApplicationID == FKiAppID).ToList().Where(m => m.StatusTypeID == 10).Where(m => m.LayoutType == "Popup").Select(m => new VMDropDown { Value = m.ID, text = m.LayoutName }).ToList();
            model.Layouts = Layouts;
            List<VMDropDown> BOs = new List<VMDropDown>();
            BOs = dbContext.BOs.Where(m => m.FKiApplicationID == FKiAppID).ToList().Where(m => m.StatusTypeID == 10).Select(m => new VMDropDown { Value = m.BOID, text = m.Name }).ToList();
            model.AllBOs = BOs;
            return model;
        }

        // Creating PopupList
        public DTResponse PopupList(jQueryDataTableParamModel param, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            var fkiApplilcationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            IQueryable<Popup> AllPopups;
            AllPopups = dbContext.Popups.Where(m => m.FKiApplicationID == fkiApplilcationID || m.FKiApplicationID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllPopups = AllPopups.Where(m => m.Name.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllPopups.Count();
            AllPopups = QuerableUtil.GetResultsForDataTables(AllPopups, "", sortExpression, param);
            var clients = AllPopups.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join p in dbContext.Layouts on c.LayoutID equals p.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.LayoutID.ToString(), c.Name, p.LayoutName, Convert.ToString(c.ParentID), Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        //validation for Popupname
        public bool IsExistsPopupName(string Name, int ID, string sDatabase)
        {
            var Popups = dbContext.Popups.ToList();
            Popup popup = Popups.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (popup != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (popup != null)
                {
                    if (ID == popup.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        //BO Columns
        public List<VMDropDown> GetBOColumns(int BOID, string sDatabase)
        {
            List<VMDropDown> AllColumns = new List<VMDropDown>();
            AllColumns = (from c in dbContext.BOFields.Where(m => m.BOID == BOID).ToList() select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllColumns;
        }

        #endregion Popups

        #region Dialogs
        public VMCustomResponse CreateDialog(Dialogs model, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            Dialogs s = new Dialogs();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            if (model.ID == 0)
            {
                s.FKiApplicationID = FKiAppID;
                s.OrganizationID = iOrgID;
                s.DialogName = model.DialogName;
                s.LayoutID = model.LayoutID;
                s.IsResizable = model.IsResizable;
                s.IsCloseIcon = model.IsCloseIcon;
                s.IsMinimiseIcon = model.IsMinimiseIcon;
                s.IsPinIcon = model.IsPinIcon;
                s.IsMaximiseIcon = model.IsMaximiseIcon;
                s.IsGrouping = model.IsGrouping;
                s.BarPosition = model.BarPosition;
                s.PopupSize = model.PopupSize;
                if (model.PopupSize == "Specific")
                {
                    s.DialogMy1 = model.DialogMy1;
                    s.DialogMy2 = model.DialogMy2;
                    s.DialogAt1 = model.DialogAt1;
                    s.DialogAt2 = model.DialogAt2;
                    s.DialogWidth = model.DialogWidth;
                    s.DialogHeight = model.DialogHeight;
                }
                s.Icon = model.Icon;
                s.iTransparency = model.iTransparency;
                s.StatusTypeID = model.StatusTypeID;
                s.CreatedBy = model.CreatedBy;
                s.CreatedTime = s.UpdatedTime = DateTime.Now;
                s.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                s.UpdatedBy = model.UpdatedBy;
                dbContext.Dialogs.Add(s);
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = s.ID, Status = true };
            }
            else
            {
                s = dbContext.Dialogs.Find(model.ID);
                s.FKiApplicationID = FKiAppID;
                s.OrganizationID = iOrgID;
                int OldLayoutID = s.LayoutID;
                int NewLayoutID = model.LayoutID;
                s.DialogName = model.DialogName;
                s.LayoutID = model.LayoutID;
                s.IsResizable = model.IsResizable;
                s.IsCloseIcon = model.IsCloseIcon;
                s.IsMinimiseIcon = model.IsMinimiseIcon;
                s.IsGrouping = model.IsGrouping;
                if (s.IsGrouping == false)
                {
                    s.BarPosition = null;
                }
                else
                {
                    s.BarPosition = model.BarPosition;
                }
                s.IsPinIcon = model.IsPinIcon;
                s.IsMaximiseIcon = model.IsMaximiseIcon;

                s.PopupSize = model.PopupSize;
                if (model.PopupSize == "Specific")
                {
                    s.DialogMy1 = model.DialogMy1;
                    s.DialogMy2 = model.DialogMy2;
                    s.DialogAt1 = model.DialogAt1;
                    s.DialogAt2 = model.DialogAt2;
                    s.DialogWidth = model.DialogWidth;
                    s.DialogHeight = model.DialogHeight;
                }
                else
                {
                    s.DialogWidth = 0;
                    s.DialogHeight = 0;
                    s.DialogMy1 = null;
                    s.DialogMy2 = null;
                    s.DialogAt1 = null;
                    s.DialogAt2 = null;
                }
                s.Icon = model.Icon;
                s.iTransparency = model.iTransparency;
                s.StatusTypeID = model.StatusTypeID;
                s.UpdatedBy = model.UpdatedBy;
                s.UpdatedTime = DateTime.Now;
                s.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = s.ID, Status = true };
            }
        }
        public Dialogs EditDialog(int ID, string sDatabase)
        {
            Dialogs p = new Dialogs();
            p = dbContext.Dialogs.Find(ID);
            return p;
        }
        public Dialogs GetDialogValues(int iUserID, string sOrgName, string sDatabase)
        {
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            Dialogs model = new Dialogs();
            List<VMDropDown> Layouts = new List<VMDropDown>();
            Layouts = dbContext.Layouts.Where(m => m.FKiApplicationID == FKiAppID).ToList().Where(m => m.StatusTypeID == 10).Select(m => new VMDropDown { Value = m.ID, text = m.LayoutName }).ToList();
            model.Layouts = Layouts;
            return model;
        }
        public DTResponse DialogList(jQueryDataTableParamModel param, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            IQueryable<Dialogs> AllPopups;
            AllPopups = dbContext.Dialogs.Where(m => m.FKiApplicationID == fkiApplicationID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //List<int> total = dbContext.Layouts.Where(m => m.LayoutName.Contains(param.sSearch)).Select(m => m.ID).ToList();
                //if (total != null)
                //{
                //    AllPopups = AllPopups.Where(m => total.Contains(m.LayoutID));
                //}
                //else
                //{
                AllPopups = AllPopups.Where(m => m.DialogName.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
                //}
            }
            int displyCount = 0;
            displyCount = AllPopups.Count();
            AllPopups = QuerableUtil.GetResultsForDataTables(AllPopups, "", sortExpression, param);
            var clients = AllPopups.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join p in dbContext.Layouts on c.LayoutID equals p.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.LayoutID.ToString(), c.DialogName, p.LayoutName, Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public bool IsExistsDialogName(string Name, int ID, string sDatabase)
        {
            var AllDialogs = dbContext.Dialogs.ToList();
            Dialogs popup = AllDialogs.Where(m => m.DialogName.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (popup != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (popup != null)
                {
                    if (ID == popup.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        //CopyDialog
        public int CopyDialogByID(int DialogID, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dialogs CopyDialog = new Dialogs();
            CopyDialog = dbContext.Dialogs.Where(m => m.ID == DialogID).FirstOrDefault();
            CopyDialog.DialogName = CopyDialog.DialogName + " Copy";
            dbContext.Dialogs.Add(CopyDialog);
            dbContext.SaveChanges();
            return CopyDialog.ID;
        }

        #endregion Dialogs

        #region Stages
        //creation of Stages repository
        public int CreateStage(Stages model, string sDatabase)
        {
            Stages s = new Stages();
            if (model.ID == 0)
            {
                s.Name = model.Name;
                s.OrganizationID = model.OrganizationID;
                s.StatusTypeID = model.StatusTypeID;
                dbContext.Stages.Add(s);
                dbContext.SaveChanges();
                return s.ID;
            }
            else
            {
                s = dbContext.Stages.Find(model.ID);
                s.Name = model.Name;
                s.StatusTypeID = model.StatusTypeID;
                dbContext.SaveChanges();
                return s.ID;
            }
        }

        //Adding Opertions to database
        public int AddOperations(Stages model, string sDatabase)
        {
            Stages s = new Stages();
            if (model.ID == 0)
            {
                s.IsSMS = model.IsSMS;
                s.IsEmail = model.IsEmail;
                s.IsSQLJob = model.IsSQLJob;
                s.IsReminders = model.IsReminders;
                s.IsDashboardRefresh = model.IsDashboardRefresh;
                s.IsAlerts = model.IsAlerts;
                s.IsPopup = model.IsPopup;
                if (model.IsPopup)
                {
                    s.PopupID = model.PopupID;
                }
                else
                {
                    s.PopupID = 0;
                }
                dbContext.Stages.Add(s);
                dbContext.SaveChanges();
                return s.ID;
            }
            else
            {
                s = dbContext.Stages.Find(model.ID);
                s.IsSMS = model.IsSMS;
                s.IsEmail = model.IsEmail;
                s.IsSQLJob = model.IsSQLJob;
                s.IsReminders = model.IsReminders;
                s.IsDashboardRefresh = model.IsDashboardRefresh;
                s.IsAlerts = model.IsAlerts;
                //s.EmailTemplate = model.EmailTemplate;
                //s.SMSTemplate = model.SMSTemplate;
                s.IsPopup = model.IsPopup;
                if (model.IsPopup)
                {
                    s.PopupID = model.PopupID;
                }
                else
                {
                    s.PopupID = 0;
                }
                dbContext.SaveChanges();
                return 0;
            }


        }

        public Stages GetStageDetails(int StageID, string sDatabase)
        {
            Stages model = new Stages();
            model = dbContext.Stages.Find(StageID);
            return model;
        }

        //Editing of StagesRepository
        public Stages EditStage(int StageID, string sDatabase)
        {
            Stages s = new Stages();
            s = dbContext.Stages.Find(StageID);
            return s;
        }

        // Creating StageList
        public DTResponse StagesList(jQueryDataTableParamModel param, int OrgID, string sDatabase)
        {
            IQueryable<Stages> AllStages;
            if (OrgID == 0)
            {
                AllStages = dbContext.Stages.Where(m => m.OrganizationID == OrgID);
            }
            else
            {
                AllStages = dbContext.Stages.Where(m => m.OrganizationID == 0 || m.OrganizationID == OrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllStages = AllStages.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllStages.Count();
            AllStages = QuerableUtil.GetResultsForDataTables(AllStages, "", sortExpression, param);
            var clients = AllStages.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.Name, GetPopupName(c.PopupID,sDatabase), c.IsSMS.ToString(),c.IsEmail.ToString(),c.IsSQLJob.ToString(),c.IsReminders.ToString(),c.IsDashboardRefresh.ToString(),c.IsAlerts.ToString(), Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetPopupName(int p, string sDatabase)
        {
            if (p > 0)
            {
                var PName = dbContext.Popups.Find(p);
                return PName.Name;
            }
            else
            {
                return "";
            }
        }
        //validation for Stagename
        public bool IsExistsStageName(string Name, int ID, string sDatabase)
        {
            var Stages = dbContext.Stages.ToList();
            Stages stage = Stages.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (stage != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (stage != null)
                {
                    if (ID == stage.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        public List<VMDropDown> GetStages(int ID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            List<Stages> AStages = new List<Stages>();
            if (OrgID == 0)
            {
                AStages = dbContext.Stages.Where(m => m.StatusTypeID == 10 && m.OrganizationID == OrgID).ToList();
            }
            else
            {
                AStages = dbContext.Stages.Where(m => m.StatusTypeID == 10 && (m.OrganizationID == 0 || m.OrganizationID == OrgID)).ToList();
            }
            List<VMDropDown> AllStages = new List<VMDropDown>();
            foreach (var items in AStages)
            {
                if (ID > 0)
                {
                    AllStages.Add(new VMDropDown
                    {
                        Value = items.ID,
                        text = items.Name
                    });
                }
                else
                {
                    int id = items.ID;
                    int StagesExists = 0;
                    if (OrgID == 0)
                    {
                        StagesExists = dbContext.StagesFlows.Where(m => m.StageID == id && m.OrganizationID == OrgID).Select(m => m.ID).FirstOrDefault();
                    }
                    else
                    {
                        StagesExists = Spdb.StagesFlows.Where(m => m.StageID == id && m.OrganizationID == OrgID).Select(m => m.ID).FirstOrDefault();
                    }

                    if (StagesExists == 0)
                    {
                        AllStages.Add(new VMDropDown
                        {
                            text = items.Name,
                            Value = items.ID
                        });
                    }
                }

            }
            //AllOrgs.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return AllStages;
        }
        public List<VMDropDown> GetAllStages(int StageID, int OrgID, string sDatabase)
        {
            var Stages = dbContext.Stages.Where(m => m.ID != StageID).Where(m => m.StatusTypeID == 10 && (m.OrganizationID == OrgID || m.OrganizationID == 0)).ToList();
            List<VMDropDown> AllStages = new List<VMDropDown>();
            AllStages = (from c in Stages
                         select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllStages;
        }

        public VMCustomResponse SaveStagesFlow(VMStages model, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext db = new DataContext(sOrgDB);
            StagesFlows SFlow = new StagesFlows();
            Stages s = new Stages();
            if (model.ID == 0)
            {
                if (model.Name == null)
                {
                    SFlow.StageID = model.StageID;
                    List<string> Stages = new List<string>();
                    foreach (var items in model.SStages)
                    {
                        Stages.Add(items);
                    }
                    string combindedString = string.Join(",", Stages.ToArray());
                    SFlow.SubStages = combindedString;
                    SFlow.StatusTypeID = model.StatusTypeID;
                    SFlow.OrganizationID = model.OrganizationID;
                    if (model.OrganizationID == 0)
                    {
                        dbContext.StagesFlows.Add(SFlow);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.StagesFlows.Add(SFlow);
                        dbContext.SaveChanges();
                    }
                    //return SFlow.ID;
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = SFlow.ID, Status = true };
                }
                else
                {
                    s.Name = model.Name;
                    s.OrganizationID = model.OrganizationID;
                    s.StatusTypeID = model.StatusTypeID;
                    dbContext.Stages.Add(s);
                    dbContext.SaveChanges();

                    SFlow.StageID = model.StageID;
                    List<string> Stages = new List<string>();
                    if (s.ID > 0)
                    {
                        Stages.Add(s.ID.ToString());
                    }
                    foreach (var items in model.SStages)
                    {
                        Stages.Add(items);
                    }
                    string combindedString = string.Join(",", Stages.ToArray());
                    SFlow.SubStages = combindedString;
                    SFlow.StatusTypeID = model.StatusTypeID;
                    SFlow.OrganizationID = model.OrganizationID;
                    if (model.OrganizationID == 0)
                    {
                        dbContext.StagesFlows.Add(SFlow);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.StagesFlows.Add(SFlow);
                        dbContext.SaveChanges();
                    }
                    //return s.ID;
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = s.ID, Status = true };
                }
            }
            else
            {
                if (model.Name == null)
                {
                    if (model.OrganizationID == 0)
                    {
                        SFlow = dbContext.StagesFlows.Find(model.ID);
                    }
                    else
                    {
                        SFlow = dbContext.StagesFlows.Find(model.ID);
                    }

                    List<string> Stages = new List<string>();
                    foreach (var items in model.SStages)
                    {
                        Stages.Add(items);
                    }
                    string combindedString = string.Join(",", Stages.ToArray());
                    SFlow.SubStages = combindedString;
                    SFlow.StatusTypeID = model.StatusTypeID;
                    if (model.OrganizationID == 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.SaveChanges();
                    }
                    //return SFlow.ID;
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = SFlow.ID, Status = true };
                }
                else
                {
                    if (model.OrganizationID == 0)
                    {
                        SFlow = dbContext.StagesFlows.Find(model.ID);
                    }
                    else
                    {
                        SFlow = db.StagesFlows.Find(model.ID);
                    }
                    List<string> Stages = new List<string>();
                    foreach (var items in model.SStages)
                    {
                        Stages.Add(items);
                    }
                    string combindedString = string.Join(",", Stages.ToArray());
                    SFlow.SubStages = combindedString;
                    SFlow.StatusTypeID = model.StatusTypeID;
                    dbContext.SaveChanges();

                    s = dbContext.Stages.Find(model.ID);
                    s.Name = model.Name;
                    s.StatusTypeID = model.StatusTypeID;
                    if (model.OrganizationID == 0)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        dbContext.SaveChanges();
                    }
                    //return s.ID;
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = s.ID, Status = true };
                }
            }

        }

        public DTResponse StagesFlowList(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext db = new DataContext(sOrgDB);
            //if (OrgID != 0)
            //{
            //    var SAStageFlows = dbContext.StagesFlows.ToList();
            //    var AStageFlows = db.StagesFlows.ToList();
            //    var RemainingFlows = SAStageFlows.Where(x => !AStageFlows.Any(y => y.StageID == x.StageID)).ToList();
            //    foreach (var items in RemainingFlows)
            //    {
            //        items.OrganizationID = OrgID;
            //        db.StagesFlows.Add(items);
            //        db.SaveChanges();
            //    }
            //}

            IQueryable<StagesFlows> AllFlows;
            if (OrgID == 0)
            {
                AllFlows = dbContext.StagesFlows.Where(m => m.OrganizationID == OrgID);
            }
            else
            {
                AllFlows = db.StagesFlows.Where(m => m.OrganizationID == OrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //AllFlows = AllFlows.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllFlows.Count();
            AllFlows = QuerableUtil.GetResultsForDataTables(AllFlows, "", sortExpression, param);
            var clients = AllFlows.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join s in dbContext.Stages on c.StageID equals s.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), c.StageID.ToString(), s.Name, GetSubStages(c.SubStages,sDatabase), Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetSubStages(string Stages, string sDatabase)
        {
            string s = Stages;
            int[] nums = Array.ConvertAll(s.Split(','), int.Parse);
            var typename = "";
            List<string> SubStages = new List<string>();
            var AllStages = dbContext.Stages.ToList();
            foreach (var items in nums)
            {
                typename = AllStages.Where(m => m.ID == items).Select(m => m.Name).FirstOrDefault();
                SubStages.Add(typename);
            }
            string combindedString = string.Join(",", SubStages.ToArray());
            return combindedString;
        }

        //private string GetStageName(int p)
        //{
        //    var typename = dbContext.Stages.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
        //    return typename;
        //}

        public VMStages EditStageFlow(int ID, int iUserID, string sOrgName, string sDatabase, int OrgID)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext db = new DataContext(sOrgDB);
            VMStages VS = new VMStages();
            StagesFlows s = new StagesFlows();
            if (OrgID == 0)
            {
                s = dbContext.StagesFlows.Find(ID);
            }
            else
            {
                s = db.StagesFlows.Find(ID);
            }
            VS.ID = s.ID;
            VS.StageID = s.StageID;
            VS.StatusTypeID = s.StatusTypeID;
            string sNumbers = s.SubStages;
            s.SStages = sNumbers.Split(new[] { ',' }).ToList<string>();
            VS.SStages = s.SStages;
            return VS;
        }

        public List<VMDropDown> GetPopupsList(int OrgID, string sDatabase)
        {
            List<VMDropDown> AllPopups = new List<VMDropDown>();
            AllPopups = (from c in dbContext.Popups.Where(m => m.StatusTypeID == 10).ToList()
                         select new VMDropDown { text = c.Name, Value = c.ID }
                             ).ToList();
            return AllPopups;
        }


        public List<VMDropDown> GetDialogsList(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var Org = dbCore.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            List<VMDropDown> AllPopups = new List<VMDropDown>();
            AllPopups = (from c in dbContext.Dialogs.Where(m => m.StatusTypeID == 10).ToList()
                         select new VMDropDown { text = c.DialogName, Value = c.ID }
                             ).ToList();
            return AllPopups;
        }


        #endregion Stages

        #region Layouts

        public VMCustomResponse SavePopupLayout(cLayouts model, int i1ClickID, string sBO, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            cLayouts Layout = new cLayouts();
            XiLinks oXiLink = new XiLinks();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            if (model != null)
            {
                if (model.ID == 0)
                {
                    Layout.FKiApplicationID = FKiAppID;
                    Layout.OrganizationID = iOrgID;
                    Layout.LayoutType = model.LayoutType;
                    Layout.LayoutName = model.LayoutName;
                    Layout.LayoutCode = model.LayoutCode;
                    Layout.XiParameterID = model.XiParameterID;
                    Layout.LayoutLevel = model.LayoutLevel;
                    Layout.Authentication = model.Authentication;
                    Layout.iThemeID = model.iThemeID;
                    Layout.bUseParentGUID = model.bUseParentGUID;
                    Layout.bIsTaskBar = model.bIsTaskBar;
                    if (Layout.bIsTaskBar == true)
                    {
                        Layout.sTaskBarPosition = model.sTaskBarPosition;
                    }
                    else
                    {
                        Layout.sTaskBarPosition = null;
                    }
                    Layout.sTaskBarPosition = model.sTaskBarPosition;
                    Layout.StatusTypeID = model.StatusTypeID;
                    if (model.arrSiloAccess != null)
                    {
                        Layout.sSiloAccess = string.Join("|", model.arrSiloAccess);
                    }
                    else
                    {
                        Layout.sSiloAccess = "";
                    }
                    Layout.bIsTaskBar = model.bIsTaskBar;
                    Layout.sTaskBarPosition = model.sTaskBarPosition;
                    Layout.CreatedBy = model.CreatedBy;
                    Layout.CreatedTime = DateTime.Now;
                    Layout.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Layout.UpdatedBy = model.UpdatedBy;
                    Layout.UpdatedTime = DateTime.Now;
                    dbContext.Layouts.Add(Layout);
                }
                else
                {
                    Layout = dbContext.Layouts.Find(model.ID);
                    Layout.FKiApplicationID = model.FKiApplicationID;
                    Layout.OrganizationID = iOrgID;
                    Layout.LayoutType = model.LayoutType;
                    Layout.LayoutName = model.LayoutName;
                    Layout.LayoutCode = model.LayoutCode;
                    Layout.XiParameterID = model.XiParameterID;
                    Layout.LayoutLevel = model.LayoutLevel;
                    Layout.iThemeID = model.iThemeID;
                    if (model.LayoutLevel == "OrganisationLevel")
                    {
                        Layout.Authentication = model.Authentication;
                    }
                    else
                    {
                        Layout.Authentication = null;
                    }
                    Layout.bUseParentGUID = model.bUseParentGUID;
                    Layout.bIsTaskBar = model.bIsTaskBar;
                    if (Layout.bIsTaskBar == true)
                    {
                        Layout.sTaskBarPosition = model.sTaskBarPosition;
                    }
                    else
                    {
                        Layout.sTaskBarPosition = null;
                    }
                    Layout.StatusTypeID = model.StatusTypeID;
                    if (model.arrSiloAccess != null)
                    {
                        Layout.sSiloAccess = string.Join("|", model.arrSiloAccess);
                    }
                    else
                    {
                        Layout.sSiloAccess = "";
                    }
                    Layout.bIsTaskBar = model.bIsTaskBar;
                    Layout.sTaskBarPosition = model.sTaskBarPosition;
                    Layout.UpdatedBy = model.UpdatedBy;
                    Layout.UpdatedTime = DateTime.Now;
                    Layout.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                }
                dbContext.SaveChanges();
            }
            else
            {
                Layout.LayoutName = "Default";
                Layout.LayoutType = "Dialog";
                Layout.LayoutLevel = "OrganisationLevel";
                Layout.Authentication = "Authenticated";
                Layout.iThemeID = 116;
                Layout.OrganizationID = iOrgID;
                Layout.FKiApplicationID = FKiAppID;
                Layout.LayoutCode = ServiceConstants.AutoLayoutHTML;
                Layout.StatusTypeID = 10;
                Layout.bIsTaskBar = true;
                Layout.sTaskBarPosition = "Left";
                Layout.bUseParentGUID = true;
                Layout.CreatedBy = Layout.UpdatedBy = 1;
                Layout.CreatedTime = Layout.UpdatedTime = DateTime.Now;
                Layout.CreatedBySYSID = Layout.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.Layouts.Add(Layout);
                dbContext.SaveChanges();
                Layout.LayoutName = "1CS " + i1ClickID + " Default";
                dbContext.SaveChanges();
                PopupLayoutDetails Details = new PopupLayoutDetails();
                Details.LayoutID = Layout.ID;
                Details.PlaceholderName = Layout.LayoutName + " Content";
                Details.PlaceholderUniqueName = Details.PlaceholderName.Replace(" ", "");
                Details.PlaceholderArea = "div1";
                dbContext.PopupLayoutDetails.Add(Details);
                dbContext.SaveChanges();
                Dialogs oDialog = AutocreateDialog(Layout.ID, FKiAppID, iOrgID, iUserID, sDatabase); //Auto Dialog Creation
                oXiLink = AutocreateXiLink(oDialog.ID, FKiAppID, iOrgID, iUserID, sDatabase); //Auto XiLink Creation
                MapXiComponentToLayout(Layout.ID, Details.PlaceHolderID, oDialog.ID, Convert.ToInt32(oXiLink.XiLinkID), iUserID, sDatabase, FKiAppID, iOrgID); //Mapping Component to Layout
                AutoCreateComponentParams(i1ClickID, sBO, Convert.ToInt32(oXiLink.XiLinkID), Details.PlaceHolderID, sDatabase, FKiAppID, iOrgID); //Auto XIComponentparams Creation
                //AutocreateDialog(Layout.ID, sDatabase);
                //var Result = AutoCreateXILink(sBO, sDatabase);
                //MapXiLinkToLayout(Layout.ID, Details.PlaceHolderID, Convert.ToInt32(oXiLink.XiLinkID), sDatabase);
            }

            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Layout.ID, Status = true, PropertyName = Layout.LayoutName, sID = oXiLink.XiLinkID.ToString() };
        }

        public Dialogs AutocreateDialog(int iLayoutID, int FKiAppID, int iOrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dialogs oDialog = new Dialogs();
            oDialog.DialogName = "1CS Dialog";
            oDialog.LayoutID = iLayoutID;
            oDialog.StatusTypeID = 10;
            oDialog.PopupSize = "Default";
            if (oDialog.PopupSize == "Default")
            {
                oDialog.DialogWidth = oDialog.DialogHeight = 0;
                oDialog.DialogMy1 = oDialog.DialogMy2 = oDialog.DialogAt1 = oDialog.DialogAt2 = null;
            }
            oDialog.IsResizable = oDialog.IsCloseIcon = oDialog.IsMinimiseIcon = oDialog.IsMaximiseIcon = true;
            oDialog.IsGrouping = false;
            if (oDialog.IsGrouping == false)
            {
                oDialog.BarPosition = null;
            }
            oDialog.IsPinIcon = false;
            oDialog.FKiApplicationID = FKiAppID; oDialog.OrganizationID = iOrgID;
            oDialog.CreatedBy = oDialog.UpdatedBy = iUserID;
            oDialog.CreatedTime = oDialog.UpdatedTime = DateTime.Now;
            oDialog.CreatedBySYSID = oDialog.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.Dialogs.Add(oDialog);
            dbContext.SaveChanges();
            return oDialog;
        }

        public XiLinks AutocreateXiLink(int iDialogID, int FKiAppID, int iOrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinks oXiLink = new XiLinks();
            oXiLink.Name = "1CS Dialog XiLink";
            oXiLink.URL = "XiLink";
            oXiLink.sActive = "Y";
            oXiLink.sType = "Content";
            oXiLink.StatusTypeID = 10;
            oXiLink.FKiApplicationID = FKiAppID; oXiLink.OrganisationID = iOrgID;
            oXiLink.CreatedBy = oXiLink.UpdatedBy = iUserID;
            oXiLink.CreatedTime = oXiLink.UpdatedTime = DateTime.Now;
            oXiLink.CreatedBySYSID = oXiLink.UpdatedBySYSID = "1";
            dbContext.XiLinks.Add(oXiLink);
            dbContext.SaveChanges();
            XiLinkNVs oNvs = new XiLinkNVs();
            oNvs.XiLinkID = oXiLink.XiLinkID;
            oNvs.Name = "StartAction";
            oNvs.Value = "Dialog";
            oNvs.StatusTypeID = 10;
            oNvs.CreatedBy = oNvs.UpdatedBy = 1;
            oNvs.CreatedTime = oNvs.UpdatedTime = DateTime.Now;
            oNvs.CreatedBySYSID = oNvs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oNvs.XiLinkListID = 0;
            dbContext.XiLinkNVs.Add(oNvs);
            dbContext.SaveChanges();
            XiLinkNVs oNvs1 = new XiLinkNVs();
            oNvs1.XiLinkID = oXiLink.XiLinkID;
            oNvs1.Name = "DialogID";
            oNvs1.Value = iDialogID.ToString();
            oNvs1.StatusTypeID = 10;
            oNvs1.CreatedBy = oNvs1.UpdatedBy = iUserID;
            oNvs1.CreatedTime = oNvs1.UpdatedTime = DateTime.Now;
            oNvs1.CreatedBySYSID = oNvs1.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oNvs1.XiLinkListID = 0;
            dbContext.XiLinkNVs.Add(oNvs1);
            dbContext.SaveChanges();
            return oXiLink;
        }

        private void MapXiComponentToLayout(int iLayoutID, int placeHolderID, int DialogID, int xiLinkID, int iUserID, string sDatabase, int FKiAppID, int iOrgID)
        {
            PopupLayoutMappings oMap = new PopupLayoutMappings();
            oMap.Type = "Dialog";
            if (oMap.Type == "Dialog")
            {
                oMap.PopupID = DialogID;
            }
            var Map = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == iLayoutID && m.PlaceHolderID == placeHolderID && m.PopupID == DialogID).FirstOrDefault();
            if (Map == null)
            {
                oMap.PlaceHolderID = placeHolderID;
                oMap.PopupLayoutID = iLayoutID;
                oMap.XiLinkID = 2;
                oMap.ContentType = "XIComponent";
                if (DialogID > 0)
                {
                    oMap.PopupID = DialogID;
                }
                oMap.Type = "Dialog";
                oMap.StatusTypeID = 10;
                oMap.FKiApplicationID = FKiAppID; oMap.OrganisationID = iOrgID;
                oMap.CreatedBy = oMap.UpdatedBy = iUserID;
                oMap.CreatedTime = oMap.UpdatedTime = DateTime.Now;
                oMap.CreatedBySYSID = oMap.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.PopupLayoutMappings.Add(oMap);
                dbContext.SaveChanges();
            }
        }
        private void AutoCreateComponentParams(int i1ClickID, string sBOName, int xiLinkID, int PlaceHolderID, string sDatabase, int FKiAppID, int iOrgID)
        {
            cXIComponentParams oCompo = new cXIComponentParams();
            oCompo.sName = "BO";
            oCompo.sValue = sBOName;
            oCompo.FKiComponentID = 2;
            oCompo.iLayoutMappingID = PlaceHolderID;
            oCompo.iXiLinkID = oCompo.iStepDefinitionID = oCompo.iStepSectionID = oCompo.iQueryID = 0;
            dbContext.XIComponentParams.Add(oCompo);
            dbContext.SaveChanges();
            oCompo.sName = "Group";
            oCompo.sValue = "Create";
            oCompo.FKiComponentID = 2;
            oCompo.iLayoutMappingID = PlaceHolderID;
            oCompo.iXiLinkID = oCompo.iStepDefinitionID = oCompo.iStepSectionID = oCompo.iQueryID = 0;
            dbContext.XIComponentParams.Add(oCompo);
            dbContext.SaveChanges();
        }

        //private void AutocreateDialog(int iLayoutID, string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext(sDatabase);
        //    Dialogs oDialog = new Dialogs();
        //    oDialog.DialogName = "1CS Dialog";
        //    oDialog.LayoutID = iLayoutID;
        //    dbContext.Dialogs.Add(oDialog);
        //    dbContext.SaveChanges();

        //    AutocreateXiLink(oDialog.ID, sDatabase);
        //}

        //private void AutocreateXiLink(int iDialogID, string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext(sDatabase);
        //    XiLinks oXiLink = new XiLinks();
        //    oXiLink.Name = "1CS Dialog XiLink";
        //    oXiLink.URL = "XiLink";
        //    dbContext.XiLinks.Add(oXiLink);
        //    dbContext.SaveChanges();
        //    XiLinkNVs oNvs = new XiLinkNVs();
        //    oNvs.XiLinkID = oXiLink.XiLinkID;
        //    oNvs.Name = "StartAction";
        //    oNvs.Value = "Dialog";
        //    dbContext.XiLinkNVs.Add(oNvs);
        //    XiLinkNVs oNvs1 = new XiLinkNVs();
        //    oNvs1.XiLinkID = oXiLink.XiLinkID;
        //    oNvs1.Name = "DialogID";
        //    oNvs1.Value = iDialogID.ToString();
        //    dbContext.XiLinkNVs.Add(oNvs);
        //    dbContext.SaveChanges();
        //}


        public VMCustomResponse AutoCreateXILink(string sBO, string sDatabase)
        {
            XiLinks oXiLink = new XiLinks();
            oXiLink.Name = "1CS XiLink";
            oXiLink.URL = "XiLink";
            oXiLink.CreatedTime = DateTime.Now;
            oXiLink.UpdatedTime = DateTime.Now;
            dbContext.XiLinks.Add(oXiLink);
            dbContext.SaveChanges();
            XiLinkNVs oNvs = new XiLinkNVs();
            oNvs.XiLinkID = oXiLink.XiLinkID;
            oNvs.Name = "StartAction";
            oNvs.Value = "CreateFrom";
            oNvs.CreatedTime = DateTime.Now;
            oNvs.UpdatedTime = DateTime.Now;
            dbContext.XiLinkNVs.Add(oNvs);
            XiLinkNVs oNvs1 = new XiLinkNVs();
            oNvs1.XiLinkID = oXiLink.XiLinkID;
            oNvs1.Name = "BO";
            oNvs1.Value = sBO;
            oNvs1.CreatedTime = DateTime.Now;
            oNvs1.UpdatedTime = DateTime.Now;
            dbContext.XiLinkNVs.Add(oNvs1);
            XiLinkNVs oNvs2 = new XiLinkNVs();
            oNvs2.XiLinkID = oXiLink.XiLinkID;
            oNvs2.Name = "Group";
            oNvs2.Value = "Edit";
            oNvs2.CreatedTime = DateTime.Now;
            oNvs2.UpdatedTime = DateTime.Now;
            dbContext.XiLinkNVs.Add(oNvs2);
            dbContext.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oXiLink.XiLinkID, Status = true };
        }

        private void MapXiLinkToLayout(int iLayoutID, int placeHolderID, int xiLinkID, string sDatabase)
        {
            PopupLayoutMappings oMap = new PopupLayoutMappings();
            oMap.PopupLayoutID = iLayoutID;
            oMap.PlaceHolderID = placeHolderID;
            oMap.XiLinkID = xiLinkID;
            oMap.Type = "Dialog";
            oMap.ContentType = "XiLink";
            oMap.StatusTypeID = 10;
            oMap.CreatedTime = DateTime.Now;
            oMap.UpdatedTime = DateTime.Now;
            dbContext.PopupLayoutMappings.Add(oMap);
            dbContext.SaveChanges();
        }

        //CopyLayout
        public int CopyPopupLayoutByID(int ID, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cLayouts CopyLayout = new cLayouts();
            PopupLayoutDetails Detail = new PopupLayoutDetails();
            CopyLayout = dbContext.Layouts.Where(m => m.ID == ID).FirstOrDefault();
            CopyLayout.LayoutName = CopyLayout.LayoutName + " Copy";
            dbContext.Layouts.Add(CopyLayout);
            dbContext.SaveChanges();
            var oLayoutDetails = dbContext.Layouts.Where(m => m.ID == ID).FirstOrDefault();
            if (oLayoutDetails.LayoutDetails.Count() > 0)
            {
                foreach (var details in oLayoutDetails.LayoutDetails.ToList())
                {
                    Detail.LayoutID = CopyLayout.ID;
                    Detail.PlaceholderName = details.PlaceholderName;
                    Detail.PlaceholderArea = details.PlaceholderArea;
                    var sPlaceholderUniqueName = details.PlaceholderUniqueName;
                    Detail.PlaceholderUniqueName = sPlaceholderUniqueName + "Copy";
                    dbContext.PopupLayoutDetails.Add(Detail);
                    dbContext.SaveChanges();
                }
            }
            return CopyLayout.ID;
        }

        public VMPopupLayout GetLayoutByID(int PopupID, int DialogID, int LayoutID, string sDatabase)
        {
            VMPopupLayout Layout = new VMPopupLayout();
            if (PopupID > 0)
            {
                var PopupDetails = dbContext.Popups.Find(PopupID);
                Layout.PopupName = PopupDetails.Name;
                Layout.PopupID = PopupID;
            }
            if (DialogID > 0)
            {
                var DialogDetails = dbContext.Dialogs.Find(DialogID);
                Layout.PopupName = DialogDetails.DialogName;
                Layout.DialogID = DialogID;
                PopupID = DialogID;
            }
            Layout.LayoutID = LayoutID;
            var LayoutStruct = dbContext.Layouts.Find(LayoutID);
            Layout.LayoutCode = LayoutStruct.LayoutCode;
            Layout.LayoutName = LayoutStruct.LayoutName;
            var Mappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == LayoutID && m.PopupID == PopupID).Select(m => new VMPopupLayoutMappings { MappingID = m.ID, PopupLayoutID = m.PopupLayoutID, PlaceHolderID = m.PlaceHolderID, XiLinkID = m.XiLinkID, PopupID = m.PopupID, ContentType = m.ContentType, HTMLCode = m.HTMLCode, StatusTypeID = m.StatusTypeID }).ToList();
            Layout.Mappings = Mappings;
            var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == LayoutID).Select(m => new VMPopupLayoutDetails { LayoutID = LayoutID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID }).ToList();
            Layout.Details = Details;
            Layout.LayoutType = LayoutStruct.LayoutType;
            return Layout;
        }
        public VMPopupLayout GetDialogLayoutByID(int ID, string sDatabase)
        {
            VMPopupLayout Layout = new VMPopupLayout();
            var PopupDetails = dbContext.Dialogs.Find(ID);
            Layout.LayoutName = PopupDetails.DialogName;
            Layout.PopupID = ID;
            Layout.LayoutID = PopupDetails.LayoutID;
            var LayoutStruct = dbContext.Layouts.Find(PopupDetails.LayoutID);
            Layout.LayoutCode = LayoutStruct.LayoutCode;
            Layout.LayoutName = LayoutStruct.LayoutName;
            var Mappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == PopupDetails.LayoutID && m.PopupID == ID).Select(m => new VMPopupLayoutMappings { PopupLayoutID = m.PopupLayoutID, PlaceHolderID = m.PlaceHolderID, XiLinkID = m.XiLinkID, PopupID = m.PopupID }).ToList();
            Layout.Mappings = Mappings;
            var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == PopupDetails.LayoutID).Select(m => new VMPopupLayoutDetails { LayoutID = PopupDetails.LayoutID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID }).ToList();
            Layout.Details = Details;
            Layout.PopupName = PopupDetails.DialogName;
            return Layout;
        }

        public VMPopupLayout GetPopupLayoutByID(int LayoutID, string sDatabase)
        {
            VMPopupLayout Layout = new VMPopupLayout();
            var LayoutStruct = dbContext.Layouts.Find(LayoutID);
            Layout.LayoutID = LayoutID;
            Layout.LayoutType = LayoutStruct.LayoutType;
            Layout.LayoutCode = LayoutStruct.LayoutCode;
            Layout.LayoutName = LayoutStruct.LayoutName;
            Layout.LayoutLevel = LayoutStruct.LayoutLevel;
            if (LayoutStruct.LayoutLevel == "OrganisationLevel")
            {
                Layout.Authentication = LayoutStruct.Authentication;
            }
            Layout.XiParameterID = LayoutStruct.XiParameterID;
            Layout.iThemeID = LayoutStruct.iThemeID;
            Layout.FKiApplicationID = LayoutStruct.FKiApplicationID;
            Layout.bUseParentGUID = LayoutStruct.bUseParentGUID;
            Layout.bIsTaskBar = LayoutStruct.bIsTaskBar;
            Layout.sTaskBarPosition = LayoutStruct.sTaskBarPosition;
            var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == LayoutID).Select(m => new VMPopupLayoutDetails { LayoutID = LayoutID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
            Layout.Details = Details;
            if (!string.IsNullOrEmpty(LayoutStruct.sSiloAccess))
            {
                string strSilo = LayoutStruct.sSiloAccess;
                var arrSilo = strSilo.Split('|');
                List<string> liStrSilo = new List<string>();
                foreach (var item in arrSilo)
                {
                    liStrSilo.Add(item.ToString());
                }
                Layout.arrSiloAccess = liStrSilo;
            }
            Layout.bIsTaskBar = LayoutStruct.bIsTaskBar;
            Layout.sTaskBarPosition = LayoutStruct.sTaskBarPosition;

            return Layout;
        }

        public DTResponse PopupLayoutsList(jQueryDataTableParamModel param, string Type, int iUserID, string sOrgName, int iOrgID, string sDatabase)
        {
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            IQueryable<cLayouts> AllLayouts;
            if (Type == "Inline")
            {
                AllLayouts = dbContext.Layouts.Where(m => m.LayoutType == Type || m.LayoutType.ToLower() == "template".ToLower()).Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0);
            }
            else
            {
                AllLayouts = dbContext.Layouts.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            }
            int displyCount = 0;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllLayouts = AllLayouts.Where(m => m.LayoutName.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
            }
            displyCount = AllLayouts.Count();
            AllLayouts = QuerableUtil.GetResultsForDataTables(AllLayouts, "", sortExpression, param);
            List<cLayouts> Layouts = new List<cLayouts>();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            Layouts = AllLayouts.ToList();
            result = from c in Layouts
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.LayoutName,c.LayoutType ,Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse SaveLayoutMapping(VMPopupLayoutMappings Mappings, string sDatabase)
        {
            PopupLayoutMappings Mapping = new PopupLayoutMappings();
            if (Mappings.Type == "Dialog")
            {
                Mappings.PopupID = Mappings.DialogID;
                Mapping = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Mappings.LayoutID && m.PlaceHolderID == Mappings.PlaceHolderID && m.PopupID == Mappings.PopupID).FirstOrDefault();
            }
            if (Mappings.Type == "Inline")
            {
                Mapping = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Mappings.LayoutID && m.PlaceHolderID == Mappings.PlaceHolderID).FirstOrDefault();
                //Mappings.PopupID = Mappings.LayoutID;
            }
            //var TypeName = dbContext.PopupLayouts.Where(m => m.ID == Mappings.PopupID).Select(m => m.LayoutType).FirstOrDefault();

            //var Map = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Mappings.LayoutID && m.PlaceHolderID == Mappings.PlaceHolderID && m.PopupID == Mappings.PopupID).FirstOrDefault();
            if (Mapping != null && Mapping.ID > 0)
            {
                if (Mappings.ContentType == "HTML")
                {
                    Mapping.XiLinkID = 0;
                    Mapping.ContentType = Mappings.ContentType;
                    Mapping.HTMLCode = Mappings.HTMLCode;
                }
                else
                {
                    Mapping.ContentType = Mappings.ContentType;
                    Mapping.XiLinkID = Mappings.XiLinkID;
                    Mapping.HTMLCode = null;
                }
                if (Mappings.PopupID > 0)
                {
                    Mapping.PopupID = Mappings.PopupID;
                }
                if (Mappings.DialogID > 0)
                {
                    Mapping.PopupID = Mappings.DialogID;
                }
                if (Mappings.LayoutID > 0)
                {
                    //Map.PopupID = Mappings.LayoutID;
                }
                Mapping.StatusTypeID = Mappings.StatusTypeID;
                Mapping.UpdatedTime = DateTime.Now;
                Mapping.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
            }
            else
            {
                Mapping = new PopupLayoutMappings();
                Mapping.PlaceHolderID = Mappings.PlaceHolderID;
                Mapping.PopupLayoutID = Mappings.LayoutID;
                Mapping.XiLinkID = Mappings.XiLinkID;
                Mapping.ContentType = Mappings.ContentType;
                if (Mappings.PopupID > 0)
                {
                    Mapping.PopupID = Mappings.PopupID;
                }
                if (Mappings.DialogID > 0)
                {
                    Mapping.PopupID = Mappings.DialogID;
                }
                if (Mappings.LayoutID > 0)
                {
                    //Mapping.PopupID = Mappings.LayoutID;
                }
                if (Mappings.ContentType == "HTML")
                {
                    Mapping.XiLinkID = 0;
                    Mapping.HTMLCode = Mappings.HTMLCode;
                }
                else
                {
                    Mapping.XiLinkID = Mappings.XiLinkID;
                }
                Mapping.Type = Mappings.Type;
                Mapping.StatusTypeID = Mappings.StatusTypeID;
                Mapping.CreatedBy = 1;
                Mapping.CreatedTime = DateTime.Now;
                Mapping.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Mapping.UpdatedBy = 1;
                Mapping.UpdatedTime = DateTime.Now;
                Mapping.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.PopupLayoutMappings.Add(Mapping);
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { Status = true, ID = Mapping.ID, ResponseMessage = "Data Saved Successfully" };
        }

        public VMCustomResponse SaveLayoutDetails(VMPopupLayoutDetails Details, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            PopupLayoutDetails Detail = new PopupLayoutDetails();
            if (Details.PlaceHolderID == 0)
            {
                Detail.LayoutID = Details.LayoutID;
                Detail.PlaceholderName = Details.PlaceholderName;
                Detail.PlaceholderArea = Details.PlaceholderArea;
                Detail.PlaceholderUniqueName = Details.PlaceholderUniqueName;
                Detail.PlaceholderClass = Details.TDClass;
                var Layout=dbContext.Layouts.Find(Details.LayoutID);
                Detail.LayoutIDXIGUID = Layout.XIGUID;
                Detail.FKiApplicationID = FKiAppID;
                dbContext.PopupLayoutDetails.Add(Detail);
                dbContext.SaveChanges();
            }
            else
            {
                Detail = dbContext.PopupLayoutDetails.Find(Details.PlaceHolderID);
                Detail.LayoutID = Details.LayoutID;
                Detail.PlaceholderName = Details.PlaceholderName;
                Detail.PlaceholderArea = Details.PlaceholderArea;
                Detail.FKiApplicationID = FKiAppID;
                Detail.PlaceholderUniqueName = Details.PlaceholderUniqueName;
                Detail.PlaceholderClass = Details.TDClass;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { Status = true, ID = Detail.PlaceHolderID, ResponseMessage = "Data Saved Successfully" };
        }

        public int CheckUniqueness(string UniqueName, int LayoutID, string sDatabase)
        {
            var Name = dbContext.PopupLayoutDetails.Where(m => m.PlaceholderUniqueName.Equals(UniqueName, StringComparison.OrdinalIgnoreCase)).Where(m => m.LayoutID == LayoutID).FirstOrDefault();
            if (Name != null)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public VMPopupLayout GetLayoutDetailsByID(int LayoutID, string sDatabase)
        {
            VMPopupLayout Layout = new VMPopupLayout();
            Layout = dbContext.Layouts.Where(m => m.ID == LayoutID).Select(m => new VMPopupLayout { LayoutCode = m.LayoutCode, LayoutID = m.ID, LayoutName = m.LayoutName, LayoutType = m.LayoutType }).FirstOrDefault();
            var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == LayoutID).Select(m => new VMPopupLayoutDetails { LayoutID = LayoutID, PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, PlaceHolderID = m.PlaceHolderID }).ToList();
            Layout.Details = Details;
            var Mappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == LayoutID).Select(m => new VMPopupLayoutMappings { PopupLayoutID = m.PopupLayoutID, PlaceHolderID = m.PlaceHolderID, XiLinkID = m.XiLinkID, StatusTypeID = m.StatusTypeID, ContentType = m.ContentType, HTMLCode = m.HTMLCode }).ToList();
            Layout.Mappings = Mappings;
            return Layout;
        }

        #endregion Layouts


    }
}
