using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IPopupRepository
    {
        VMCustomResponse SaveTab(Tabs model, string sDatabase);
        VMAssignReports SaveSection(Sections model, string sDatabase);
        DTResponse TabsList(jQueryDataTableParamModel param, string sDatabase);
        List<Sections> SectionsList(int TabID, string sDatabase);
        DTResponse TabSectionsList(jQueryDataTableParamModel param, int TabID, string sDatabase);
        Tabs EditTab(int TabID, string sDatabase);
        int DeleteTab(int TabID, string sDatabase);
        DTResponse TabsReportsList(jQueryDataTableParamModel param, string sDatabase);
        List<VMDropDown> GetTabs(string Category, string sDatabase);
        List<VMDropDown> GetClasses(int OrgID, string sDatabase);
        List<Classes> GetQueriesByType(int TabID, int DisplayAs, int ClassType, string SectionID, string sDatabase);
        List<Classes> GetQueriesByTypeForSection(int DisplayAs, int ClassType, string sDatabase);
        List<Classes> GetQueriesByTypeForEdit(int Tab1ClickID, string sDatabase);
        VMAssignReports SaveReportToTab(Tab1Clicks model, string sDatabase);
        Tab1Clicks EditTabReport(int ID, string sDatabase);
        int DeleteTabReport(int ID, string sDatabase);
        bool IsExistsTabName(string Name, int ID, int PopupID, string sDatabase);
        bool IsExistsSectionName(string Name, int ID, int TabID, string sDatabase);
        DTResponse TabSpecificList(jQueryDataTableParamModel param, int Tab, string sDatabase);
        List<Classes> GetAssignedClasses(int Tab, string sDatabase);
        List<Classes> GetAssignedSecClasses(int SectionID, int TabID, string sDatabase);
        List<VMDropDown> GetAllRanks(int Category, string sDatabase);
        List<VMDropDown> GetAllSecRanks(int ID, string sDatabase);
        List<VMDropDown> GetRanksByCategory(int Category, string sDatabase);
        List<VMDropDown> GetTabsByCategory(int Category, string sDatabase);
        List<VMDropDown> GetAllTabSections(int ID, string sDatabase);
        List<VMDropDown> GetAllSectionRanks(int ID, string sDatabase);
        List<VMDropDown> GetCategoryTabs(int TabID, string sDatabase);
        Sections GetSectionDetails(int SectionID, string sDatabase);
        string SaveSectionFields(Sections model, string sDatabase);
        VMQueryPreview GetQueryPreview(int QueryID, int iUserID, string sOrgName, string sDatabase, int orgid);
        List<VMKPIResult> GetKPICircleResult(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrgID);
        List<DashBoardGraphs> GetLeadsBySource(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrgID);
        List<DashBoardGraphs> GetLeadsByClass(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrgID);
        List<DashBoardGraphs> GetBarChartResult(int ReportID, int iUserID, string sOrgName, string sDatabase, int OrgID);
        Tab1Clicks GetSectionType(int TabID, string sDatabase);
        List<Tabs> GetTabsReports(int Category, string sDatabase);
        List<VMDropDown> GetCategoryDetails(string sDatabase);
        LineGraph GetLineGraphForTab(int ReportID, int OrgID, int iUserID, string sOrgName, string sDatabase);
        int DeleteSection(int TabID, int SectionID, string sDatabase);
        int SaveCallsContent(string Desc, int LeadID, int orgid, string sDatabase);
        VMLeadRecord EditLeadRecord(int LeadID, int ReportID, int OrgID, string sDatabase);
        VMLeadRecord SaveLeadRecord(VMLeadRecord model, int OrgID, string sDatabase);
        int CreateStage(Stages model, string sDatabase);
        Stages EditStage(int StageID, string sDatabase);
        DTResponse StagesList(jQueryDataTableParamModel param, int OrgID, string sDatabase);
        //List<Stages> StagesList();
        bool IsExistsStageName(string Name, int ID, string sDatabase);
        int AddOperations(Stages model, string sDatabase);
        Stages GetStageDetails(int StageID, string sDatabase);
        List<VMDropDown> GetStages(int ID, int OrgID, string sDatabase);
        List<VMDropDown> GetAllStages(int StageID, int OrgID, string sDatabase);
        VMCustomResponse SaveStagesFlow(VMStages model, int iUserID, string sOrgName, string sDatabase);
        DTResponse StagesFlowList(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase);
        VMStages EditStageFlow(int ID, int iUserID, string sOrgName, string sDatabase, int OrgID);


        #region Popups
        VMCustomResponse CreatePopup(Popup model, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        Popup GetPopupValues(int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetBOColumns(int BOID, string sDatabase);
        DTResponse PopupList(jQueryDataTableParamModel param, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        Popup EditPopup(int PopupID, string sDatabase);
        bool IsExistsPopupName(string Name, int ID, string sDatabase);
        List<VMDropDown> GetPopupsList(int OrgID, string sDatabase);
        #endregion Popups

        #region Dialogs
        VMCustomResponse CreateDialog(Dialogs model, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        Dialogs EditDialog(int ID, string sDatabase);
        Dialogs GetDialogValues(int iUserID, string sOrgName, string sDatabase);
        DTResponse DialogList(jQueryDataTableParamModel param, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        List<VMDropDown> GetDialogsList(int OrgID, string sDatabase);
        bool IsExistsDialogName(string Name, int ID, string sDatabase);
        #endregion Dialogs

        #region Layouts
        VMCustomResponse SavePopupLayout(cLayouts model, int i1ClickID, string sBO, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        VMPopupLayout GetLayoutByID(int PopupID, int DialogID, int LayoutID, string sDatabase);
        VMPopupLayout GetDialogLayoutByID(int ID, string sDatabase);
        VMPopupLayout GetPopupLayoutByID(int PopupID, string sDatabase);
        DTResponse PopupLayoutsList(jQueryDataTableParamModel param, string Type, int iUserID, string sOrgName, int iOrgID, string sDatabase);
        VMCustomResponse SaveLayoutMapping(VMPopupLayoutMappings Mappings, string sDatabase);
        VMCustomResponse SaveLayoutDetails(VMPopupLayoutDetails Mappings, int iUserID, string sOrgName, string sDatabase);
        int CheckUniqueness(string UniqueName, int LayoutID, string sDatabase);
        VMPopupLayout GetLayoutDetailsByID(int LayoutID, string sDatabase);
        int CopyPopupLayoutByID(int ID, int OrgID, int iUserID, string sDatabase);
        int CopyDialogByID(int DialogID, int OrgID, int iUserID, string sDatabase);
        #endregion Layouts
    }
}
