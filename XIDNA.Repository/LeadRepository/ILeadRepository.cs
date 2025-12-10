using XIDNA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
using System.Web;

namespace XIDNA.Repository
{
    public interface ILeadRepository
    {
        List<List<string>> ExtractEmailData(int SourceID, int OrgID, string DbName);
        //DTResponse DisplayLeadDetails(jQueryDataTableParamModel param,string database);
        List<VMDropDown> GetOrgClassTypes(int OrgID, string database);
        VMActionTypes GetAllTemplates(int OrgID, int Type, string database);
        int SaveAction(VMActionTypes model);
        int CallAction(VMLeadActions model, int OrgID, int iUserID, string sOrgName, string sDatabase);
        DTResponse GetActionsList(jQueryDataTableParamModel param, int OrgID, string database);
        DTResponse GetActionMenusList(jQueryDataTableParamModel param, int OrgID, string database);
        bool IsExistsActionName(string Name, int ID, int OrganizationID);
        bool IsExistsActionMenuName(string Name, int ID, int OrganizationID);
        VMActionTypes EditAction(int ActionID, int OrganizationID);
        string SaveLeadTransaction(Stages model, int OrgID, string database, int UserID);
        List<string> ImportExcelData(HttpPostedFileBase fileName, string FilePath, int OrgID, int FileID, string SubID);
        List<string> ImportJSONData(string fileName, int OrgID, int FileID, string SubID);
        List<string> ImportXMLData(string fileName, int OrgID, int FileID, string SubID);
        List<string> ImportTabDelimitedData(string fileName, int OrgID, int FileID, string SubID);
        List<string> ImportCSVData(string fileName, int OrgID, int FileID, string SubID);
        List<List<int>> GetValidANDInvalidDetails(List<int> ID, string database, int OrgID, int FileID);
        List<List<string>> GetInValidData(List<int> ID, string database, int OrgID);
        List<object[]> ExtractLeadData(List<int> ID, string database, int OrgID, int FileID);
        int SaveImportHistories(ImportHistories model, string database);
        DTResponse ImportRulesDetailsList(jQueryDataTableParamModel param, int ID);
        int SaveImportRulesDetails(ImportRules model);
        ImportRules EditImportRulesDetails(int ID);
        DTResponse DisplayCompleteLeadDetails(jQueryDataTableParamModel param, int OrgID, string database);
        DTResponse ErrorDetailsList(jQueryDataTableParamModel param, int OrgID, string Database);
        List<ImportingErrorDetails> GetFileErrorDetails(int ID, int OrgID, string database);
        List<IOServerDetails> ServerDetails(int Type, int OrgID);
        int ReportSourceProvider(List<int> iID, string database, int OrgID, int FileID);
        //List<VMDropDown> GetTemplateList(int Type, string database, int OrgID);
        string SendMail(int iUserID, string sOrgName);
        int SaveLeadConfigurations(LeadConfigurations model);
        DTResponse LeadConfigurationsList(jQueryDataTableParamModel param, int OrgID, string database);
        LeadConfigurations EditLeadConfigurations(int ID, int OrganizationID);
        List<VMDropDown> ClassesList(int OrgID, int ID, string sDatabse, int iUserID, string sOrgName);
        List<VMDropDown> GetOrganizations();
        List<VMDropDown> GetSourceProvider(int OrgID, string DbName);
        List<VMDropDown> GetActionTypes(int OrgID, string database);
        VMCustomResponse SaveActionMenu(LeadActionMenus model, int OrgID, string database);
        LeadActionMenus GetActionMenuByID(int ID, int OrgID, string database);
        List<VMDropDown> GetAllTabs(int ReportID, int PopupID, string database);
        List<VMQueryPreview> GetTabContent(VMViewPopup Popup, int UserID, int OrgID, string database, string sOrgName);
        VMQueryPreview QueryDynamicForm(int Tab1clickID, int? leadid, string database, int orgid);
        VMQueryPreview EditData(List<FormData> FormValues, VMViewPopup Popup, string database, string sOrgName);
        VMLeadPopupLeft GetLeadPopupLeftContent(VMViewPopup popup, string database, int OrgID);
        Popup GetPopupDetails(int ReportID);
        int GetLeadStage(int LeadID, string database);
        List<Stages> GetNextStages(int LeadID, int StageID, int OrgID, string database);
        VMQueryPreview GetEditRowDetails(int LeadID, int ClickID, int OrgID, string database);
        List<SectionsData> GetCreateRowDetails(int Tab1ClickID, int BOID, int OrgID, string database);
        VMQueryPreview CreateRowFromGrid(List<FormData> FormValues, VMViewPopup popup, string database, string sOrgName);
        ViewDetails TableRowValues(string ID, int BOID, string BOName, string ColumnName, string database, int OrgID);
        VMQueryPreview DeleteRowDetails(int LeadID, int Tab1ClickID, int BOID, int OrgID, int UserID, string database, string sOrgName);
        VMQueryPreview RunUserQuery(int QueryID, int UserID, string database, int OrganizationID, int LeadID, string sOrgName);
        int GetInnerReportID(int ReportID);
        List<string> GetDatabases();
        int SendRegisterMail(int iUserID, string Email, string Type, int OrgID, string database, int ClassID, string sOrgName);
        DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch model, string sDatabase, int iUserID, string sOrgName);

        #region Documents
        int SaveOrganizationDocument(WalletDocuments Document, int LeadID);
        DTResponse GetClientDocuments(jQueryDataTableParamModel param, int LeadID, int OrgID, string Type);
        #endregion Documents
        List<string> GetLeadEmailID(int LeadID, int OrgID);
        DTResponse GetUploadedDocsGrid(jQueryDataTableParamModel param, string ClientID, int OrgID);
        VMQueryPreview PostQuoteToLead(VMViewPopup model, string sOrgName);
        int PostMessage(WalletMessages Message);
        LeadInbounds ShowViewImport(int ID, int OrgID);
        DTResponse GetHTMLColorCodingsList(jQueryDataTableParamModel param, int OrgID, string database);
        List<VMDropDown> GetAllBoFields();
        List<VMDropDown> GetColumnValues(int ID, int OrgID);
        int SaveHTMLColorCoding(HTMLColorCodings model, int OrgID, string Database);
        HTMLColorCodings GetHTMLCodingByID(int ID, int OrgID, string Database);
    }
}
