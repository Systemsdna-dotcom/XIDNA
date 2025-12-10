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

namespace XIDNA.Repository
{
    public interface IInboxRepository
    {
        DTResponse GetUsersList(jQueryDataTableParamModel param, int ID, string database);
        int SaveUserQueries(VMUserReports model, string database, int OrgID);
        DTResponse UserQueriesList(jQueryDataTableParamModel model, int RoleID, int iUserID, string sOrgName, string sDatabase);
        List<VMReports> GetQueriesByID(int UserID, int OrgID, string sDatabase, string sOrgName);
        // List<Reports> GetQueriesByID(int UserID, int OrgID, string database);
        List<VMUserReports> GetUserQueryList(int UserID, string database);
        VMResultList RunUserQuery(VMRunUserQuery model, string database, string sOrgName);
        VMUserDashContents GetDashBoardInfo(int UserID, string database, int ClassFilter, int dropdownDate);
        VMUserReports EditUserQuery(int ID, int OrgID, int iUserID, string sOrgName, string sDatabase);
        //List<DashBoardGraphs> GetLeadSourcePolarchart();
        GraphData GetPieChart(VMChart Chart, string sDatabase, string sOrgName);
        LineGraph GetBarChart(VMChart Chart, string database, string sOrgName);
        List<DashBoardGraphs> GetLeadsByClass(int UserID, string sOrgName, string database, int OrganizationID, int ClassFilter, int DateFilter);
        List<DashBoardGraphs> GetLeadsByClassForTab(int UserID, string database, int OrgID, string ReportID, string sOrgName);
        List<VMKPIResult> GetKPICircleResult(string ReportID, int UserID, string database, int OrganizationID, int ClassFilter, int DateFilter, string sOrgName);
        List<DashBoardGraphs> GetLeadsCount(int UserID, string Database);
        int SaveColSettings(string[] ColOrder, int UserID, string database);
        UserReports GetReportType(int ID, int UserID, string Database);
        cXIAppUsers RemoveDashboardGraph(string Type, int UserID, string database);
        cXIAppUsers GetDefaultSettings(int UserID, string database);
        LineGraph GetLineChart(VMChart Chart, string database, string sOrgName);
        List<VMDropDown> GetQueriesByType(int RoleID, int ClassType, int Display, string database, int ClassID, int OrgID);
        DTResponse GetReminderList(jQueryDataTableParamModel param, int LeadID, int iUserID, string sOrgName, string sDatabase);
        int SaveReminder(int userid, int OrgID, string database, string sOrgName, Reminders obj);
        VMReminders GetReminderUserCount(int UserID, int OrgID, string database, string sOrgName);
        List<VMDropDown> GetOrganizations(string database);
        IEnumerable<string[]> GetRolesByOrganization(int OrgID, string database);
        List<VMDropDown> GetAllTabs(int ReportID, string database);
        int DeleteUserReport(int ReportID, string database);
        VMUserReports GetAssignDropdowns(int OrgID, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetClassDropDownList(int OrgID, int UserID, string sOrgName, string database);
        List<VMDropDown> GetDateDropDownList(string database);
        int AssignLeadToUser(int LeadID, int UserID, int OrgID, string database);
        DTResponse ViewReportsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        List<VMDashReports> GetDashboardReports(int iUserID, string sOrgName, string sDatabase);
        VMResultList QuickSearch(VMQuickSearch model, string database, string sOrgName);
        DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch model, string sDatabase, int iUserID, string sCurrentGuestUser, string sOrgName);
        VMResultList GetHeadings(string ReportID, string database, int OrgID, int UserID, string sOrgName);
        List<int> GetLoadingType(string QueryID, string database);
        string GetSearchString(string SearchText, int ReportID, int OrgID, string SearchType, int iUserID, string sOrgName, string sDatabase);
        DataTable ExportDatatableContent(string ReportID, int OrgID, int UserID, string Role, string database, string sOrgName);
        VMOneClicks GetAllOneClicks(string ReportID, string Query, string database);
        int SendMail(string EmailID, int OrgID, string Attachment, string database);
        List<Reports> GetStructuredOneClicks(int OrgID, string ID, string database);
        //List<Reports> GetLeftMenuTree(int UserID, int OrgID, string database, string sOrgName);
        //int GridDeleteTableRow(string InstanceID, int BOID, int iUserID, string sOrgName, string sDatabase);
        VMResultList GetXIOneClickByID(int iOneClickID, int iUserID, string sOrgName, string sDatabase);
        List<RightMenuTrees> GetLeftMenuTrees(int UserID, int OrgID, string sOrgName, string sDatabase);

        int Save1Click(VMUserReports model, string sDatabase, int OrgID, int iUserID);
    }
}
