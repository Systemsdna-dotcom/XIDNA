using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IXiLinkRepository
    {

        #region XiLinks
        int RemoveXilinkID(int XiLinkID);
        int RemoveXiVisualID(int iVisualID);
        DTResponse XiLinksList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXiLink(VMXiLinks model, int iUsreID, string sOrgName, string sDatabase);
        VMXiLinks GetXiLinkByID(int XiLinkID, string sDatabase);
        XiLinks GetXiLinkDetails(int XiLinkID, int iUserID, string sOrgName, string sDatabase);
        //List<XiLinkNVs> GetInlineDetails(int InlineID);
        bool IsExistsXiLinkName(string Name, int XiLinkID, int iUserID, string sOrgName, string sDatabase);
        int CopyXiLinkByID(int XiLinkID, int OrgID, int iUserID, string sDatabase);
        #endregion XiLinks

        #region XiParameters

        DTResponse XiParametersList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXiParameter(VMXiParameters model, int iUserID, string sOrgName, string sDatabase);
        VMXiParameters GetXiParameterByID(int XiParameterID, string sDatabase);
        XiParameters GetXIParameterDetails(int XiParameterID, string sDatabase);

        #endregion XiParameters

        #region OneClickExecution
        List<int> GetLoadingType(int QueryID, string sDatabase);
        List<Reports> GetStructuredOneClicks(int OrgID, int ID, string sDatabase);
        VMResultList GetHeadings(int ReportID, string SearchType, string sDatabase, int OrgID, int iUserID, string sOrgName);
        VMResultList RunUserQuery(VMRunUserQuery model, int iUserID, string sOrgName, string sDatabase);
        #endregion OneClickExecution
        //object DeepClone();

        #region Popup

        Popup GetPopupDetails(int ReportID, string sDatabase);
        VMLeadPopupLeft GetLeadPopupLeftContent(VMViewPopup popup, string sDatabase, int OrgID, int iUserID, string sOrgName);
        List<VMDropDown> GetAllTabs(int ReportID, int PopupID, string sDatabase);
        int CallAction(VMLeadActions model, int OrgID, int iUserID, string sOrgName, string sDatabase);
        string SaveLeadTransaction(Stages model, int OrgID, string sDatabase, int iUserID, string sOrgName);
        List<Stages> GetNextStages(int LeadID, int StageID, int OrgID, int iUserID, string sOrgName, string sDatabase);
        int SaveWalletRequest(WalletRequests Request, string sDatabase);
        List<VMQueryPreview> GetTabContent(VMViewPopup Popup, int iUserID, int OrgID, string sDatabase, string sOrgName);
        VMPopupLayout GetPopupLayoutDetails(int PopupID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase);
        VMPopupLayout GetDialogLayoutDetails(int DialogID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase);
        Dialogs GetDialog(int ID, string sDatabase);
        Popup GetPopupDetailsByID(int ID, string sDatabase);
        VMPopupLayout GetLayoutDetails(int LayoutID, string sParentGUID, string sSection, string sDatabase);
        #endregion Popup

        #region PopupNew

        VMPopup GetInlineView(string BO, string Group, int LeadID, string sDatabase);
        VMResultList GetHeadingsForList(int ReportID, string SearchType, string sDatabase, int OrgID, int iUserID, string sOrgName);
        DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch Search, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser, List<cNameValuePairs> nParams);
        int EditData(VMSaveInlineEdit FormValues, string sDatabase, int OrgID, int iUserID, string sOrgName);
        //cBOInstance CreateFormData(VMSaveInlineEdit Savedata, string sDatabase, int OrgID, int iUserID, string BOName);
        List<RightMenuTrees> GetMenus(string MenuName, int iUserID, int OrgID, string sDatabase);
        List<RightMenuTrees> GetChildForMenu(int ID, int OrgID, string sDatabase);
        string GetLabelData(int iBOID, string Label, int i1ClickID, int iUserID, string sOrgName, string sDatabase);
        #endregion PopupNew

        #region UserDialogs

        int SaveUserDialog(int QueryID, int iUserID, int OrgID, string sDatabase);
        List<VMReports> GetUserDialogs(int iUserID, int OrgID, string sDatabase);
        #endregion UserDialogs


        LineGraph GetBarChart(VMChart Charts, string sDatabase, int iUserID, string sOrgName);
        LineGraph GetLineChart(VMChart Chart, int iUserID, string sOrgName, string sDatabase);
        GraphData GetPieChart(VMChart Chart, int iUserID, string sOrgName, string sDatabase);
        List<VMKPIResult> GetKPICircleResult(VMChart Chart, int iUserID, string sDatabase, string sOrgName);
        List<XiLinkNVs> GetTabsDetails(int XiLinkID, string sDatabase);
        List<VMDropDown> GetAutoCompleteData(int i1ClickID, string sSearchText, int iUserID, string sOrgName, string sDatabase);


        #region UpdatedMethods
        cXISemantics GetXISemanticByID(int iXISemanticID, string sDatabase);
        #endregion UpdatedMethods

        #region XiVisualisation

        VMCustomResponse SaveEditXiVisualisation(VMXiVisualisations model, int iUserID, string sOrgName, string sDatabase);
        VMXiVisualisations GetXiVisualisationByID(int XiVisualID, int iUserID, string sOrgName, string sDatabase);
        DTResponse XiVisualisationsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        bool IsExistsXiVisualisationsName(string Name, int XiVisualID, int iUserID, string sOrgName, string sDatabase);

        #endregion XiVisualisation
        #region UploadFiles
        VMCustomResponse SaveFiles(int ID, int BOFieldID, int OrgID, List<HttpPostedFileBase> UploadImage, int iUserID, string sOrgName, string sDatabase, string sInstanceID);
        #endregion UploadFiles
        //19/02/2018
        int DeleteAttrImage(string sDatabase, int ImgID, int BOFieldID, int LeadID);

        #region QuestionSet

        cQSInstance GetQSInstanceByID(int iQSIID);
        cQSDefinition GetQuestionSetDefinitionByID(int iQSID, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser);
        cQSInstance GetQuestionSetInstance(int iQSID, string sGUID, string sMode, int iBODID, int iInstanceID, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser);
        cQSInstance SaveQSInstance(cQSInstance oQSInstance, int iCurrentStepID, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser);
        cQSStepDefiniton GetStepDefinition(int iStepID, string sGUID, int iUserID, string sOrgName, string sDatabase);
        #endregion QuestionSet

        #region DemoXIScripting
        string XIScripting(int XILinkID, string sGUID, int iInstanceID, int iBOID, int iUserID, string sOrgName, string sDatabase, int iCustomerID);
        #endregion DemoXIScripting

        string ClaimTerms(List<VMFormData> FormValues, string sDatabase, int iUserID, string sOrgName);
        string CliamConvictionData(List<VMFormData> FormValues, string sDatabase, int iUserID, string sOrgName);
    }
}
