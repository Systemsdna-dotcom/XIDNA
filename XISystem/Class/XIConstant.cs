using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XISystem
{
    public static class XIConstant
    {
        //XI Component Names
        public const string FormComponent = "Form Component";
        public const string HTMLComponent = "HTML Component";
        public const string QSComponent = "QSComponent";
        public const string CheckboxComponent = "CheckboxComponent";
        public const string QuestionSetComponent = "Question Set Component";
        public const string OneClickComponent = "OneClickComponent";
        public const string XITreeStructure = "XITreeStructure";
        public const string TabComponent = "Tab Component";
        public const string MenuComponent = "MenuComponent";
        public const string GridComponent = "Grid Component";
        public const string ListComponent = "List Component";
        public const string GroupComponent = "GroupComponent";
        public const string XilinkComponent = "XilinkComponent";
        public const string ScriptComponent = "ScriptComponent";
        public const string LayoutComponent = "LayoutComponent";
        public const string LayoutMappingComponent = "LayoutMappingComponent";
        public const string LayoutDetailsComponent = "LayoutDetailsComponent";
        public const string XIApplicationComponent = "XIApplicationComponent";
        public const string MenuNodeComponent = "MenuNodeComponent";
        public const string InboxComponent = "InboxComponent";
        public const string XIUrlMappingComponent = "XIUrlMappingComponent";
        public const string DialogComponent = "DialogComponent";
        public const string ReportComponent = "ReportComponent";
        public const string PieChartComponent = "PieChartComponent";
        public const string GaugeChartComponent = "GaugeChartComponent";
        public const string DashBoardChartComponent = "DashBoardChartComponent";
        public const string CombinationChartComponent = "CombinationChartComponent";
        public const string DailerComponent = "DailerComponent";
        public const string FieldOriginComponent = "FieldOriginComponent";
        public const string XIParameterComponent = "XIParameterComponent";
        public const string DataTypeComponent = "DataTypeComponent";
        public const string XIComponentComponent = "XIComponentComponent";
        public const string XIBOComponent = "XIBOComponent";
        public const string XIBOAttributeComponent = "XIBOAttributeComponent";
        public const string XIBOScriptComponent = "XIBOScriptComponent";
        public const string XIBOStructureComponent = "XIBOStructureComponent";
        public const string XIDataSourceComponent = "XIDataSourceComponent";
        public const string QueryManagementComponent = "QueryManagementComponent";
        public const string QSConfigComponent = "QSConfigComponent";
        public const string QSStepConfigComponent = "QSStepConfigComponent";
        public const string QSSectionConfigComponent = "QSSectionConfigComponent";
        public const string VisualisationComponent = "VisualisationComponent";
        public const string QSLinkComponent = "QSLinkComponent";
        public const string QSLinkDefinationComponent = "QSLinkDefinationComponent";
        public const string XiGUIDColumn = "XIGUID";
        public const string TreeStructureComponet = "XITreeStructure";
        public const string MappingComponent = "MappingComponent";
        public const string MultiRowComponent = "MultiRowComponent";
        public const string XIInfraXIBOUIComponent = "XIInfraXIBOUIComponent";
        public const string QuoteReportDataComponent = "QuoteReportDataComponent";
        public const string XIBOUIDetailsComponent = "XIBOUIDetailsComponent";
        public const string FeedComponent = "FeedComponent";
        public const string DocumentTreeComponent = "DocumentTreeComponent";
        public const string AccountComponent = "AccountComponent";
        public const string UserCreationComponent = "UserCreationComponent";
        public const string DynamicTreeComponent = "DynamicTreeComponent";
        public const string KPICircleComponent = "KPICircleComponent";
        public const string XIValuesComponent = "XIValuesComponent";
        public const string RulesComponent = "RulesComponent";
        public const string SmartSearchComponent = "SmartSearchComponent";
        public const string CodeEditorComponent = "CodeEditorComponent";
        public const string AccordionComponent = "AccordionComponent";

        //XI Definition Cache Keys
        public const string CacheBO = "bo";
        public const string CacheBO_All = "bo_all";
        public const string CacheXILink = "xilink";
        public const string CacheURL = "url";
        public const string CacheApplication = "application";
        public const string CacheApplicationOTP = "applicationotpsetting";
        public const string CacheOrganisation = "organisation";
        public const string CacheLayout = "layout";
        public const string CacheQuestionSet = "questionset";
        public const string CacheQuestionSetLite = "questionsetlite";
        public const string CacheQSStep = "qsstep";
        public const string CacheQSSection = "qssection";
        public const string Cache1Click = "oneclick";
        public const string CacheComponent = "component";
        public const string CacheDataSource = "datasource";
        public const string CacheStructure = "structure";
        public const string CacheStructureCode = "structurecode";
        public const string CacheXIParamater = "xiparamater";
        public const string CacheDialog = "dialog";
        public const string CachePopup = "popup";
        public const string CacheTemplate = "template";
        public const string CacheVisualisation = "visualisation";
        public const string CacheRefData = "refdata";
        public const string CacheDocType = "doctypes";
        public const string CacheMenu = "menu";
        public const string CacheMenuNode = "menunode";
        public const string CacheIOServer = "ioserverinfo";
        public const string CacheClass = "class";
        public const string CacheSource = "source";
        public const string CacheBOGroup = "bogroup";
        public const string CacheBOScript = "boscript";
        public const string CacheInbox = "inbox";
        public const string CacheFieldOrigin = "fieldorigin";
        public const string CacheScript = "script";
        public const string CacheBOAction = "boaction";
        public const string CacheBODefault = "bodefault";
        public const string CacheDataType = "datatype";
        public const string CacheWidget = "widget";
        public const string CacheXIAlgorithm = "algorithm";
        public const string CacheWhiteList = "whitelist";
        public const string CacheConfig = "configsetting";
        public const string CacheLinkAccess = "linkaccess";
        public const string CacheQueryAccess = "queryaccess";
        public const string CacheSendGridAccount = "sendgridaccount";
        public const string CacheSendGridTemplate = "sendgridtemplate";
        public const string CacheDistribution = "distribute";
        public const string CacheXIAccount = "xiaccount";
        public const string CacheXIrefComType = "xirefcomtype";
        public const string CacheXIAPISetting = "xiapisetting";
        public const string CacheXIRule = "xirule";
        public const string CacheXIFormat = "format";
        public const string CacheXIDocType = "doctype";
        public const string CacheBOSignalR = "bosignalR";

        //XI Result Codes
        public const string Success = "Success";
        public const string Error = "Error";
        public const string SqlDateFormat = "yyyy-MM-dd HH:mm:ss.fff";
        public const string SqlDefaultDate = "1/1/1900 12:00:00 AM";
        public const string Date_Format = "dd-MMM-yyyy";
        public const string DateTime_Format = "dd-MMM-yyyy HH:mm";
        public const string DateTimeFull_Format = "dd-MMM-yyyy hh:mm:ss tt";
        public const string Time_Format = "HH:mm:ss";

        //XIContent
        public const string ContentXILink = "xilink";
        public const string ContentXIComponent = "xicomponent";
        public const string ContentQuestionSet = "questionset";
        public const string ContentHTML = "html";
        public const string ContentStep = "step";
        public const string ContentSection = "section";
        public const string ContentLayout = "layout";
        public const string ContentDialog = "dialog";
        public const string ContentPopup = "popup";
        public const string ContentFields = "fields";

        public static String sGUID = "";

        //Component Params
        public const string Param_InstanceID = "iInstanceID";
        public const string Param_InstanceIDM = "{-iInstanceID}";
        public const string Param_BOIID = "iBOIID";
        public const string Param_BO = "BO";
        public const string Param_BOAttrID = "BOFieldID";
        public const string Param_BOName = "sBOName";
        public const string Param_Group = "Group";
        public const string Param_LockGroup = "LockGroup";
        public const string Param_1ClickID = "1ClickID";
        public const string Param_1ClickGUID = "XIGUID";
        public const string Param_i1ClickID = "i1ClickID";//For Mappipng 1Click
        public const string Param_StructureName = "sStructureName";
        public const string Param_StructureCode = "StructureCode";
        public const string Param_DisplayMode = "DisplayMode";
        public const string Param_BODID = "iBODID";
        public const string Param_Code = "sCode";
        public const string Param_Mode = "sMode";
        public const string Param_OutputContent = "OutputContent";
        public const string Param_QSDID = "iQSDID";
        public const string Param_QSName = "sQSName";
        public const string Param_MenuName = "MenuName";
        public const string Param_Layout = "sLayout";
        public const string Param_sTemplate = "sTemplate";
        public const string Param_ParentBO = "sParentBO";
        public const string Param_ParentBOIID = "iParentBOIID";
        public const string Param_ParentName = "sParentName";
        public const string Param_ParentFKCol = "sParentFKColumn";
        public const string Param_ParentFKColumn = "ParentFKColumn";
        public const string Param_ParentInsID = "ParentInsID";
        public const string Param_ParentID = "ParentID";
        public const string Param_StrName = "StructureName";
        public const string Param_SearchText = "sSearchText";
        public const string Param_SearchType = "sSearchType";
        public const string Param_OverrideGroup = "sOverrideGroup";
        public const string Param_HiddenGroup = "HiddenGroup";
        public const string Param_AttrName = "sAttrName";
        public const string Param_AttrValue = "sAttrValue";
        public const string Param_ParentAttr = "sParentAttr";
        public const string Param_ParentAttrVal = "sParentAttrVal";
        public const string Param_ChildAttr = "sChildAttr";
        public const string Param_ChildAttrVal = "sChildAttrVal";
        public const string Param_FKiVisualisationID = "{XIP|FKiVisualisationID}";
        public const string Param_BOActionCode = "BOActionCode";
        public const string Param_PreviousValue = "PreviousValue";
        public const string Param_LoadType = "sLoadType";
        public const string Param_TreeGUID = "sTreeGUID";
        public const string Param_NodeID = "iNodeID";
        public const string Param_OrgID = "iOrgID";
        public const string Param_AspectWhere = "sAspectWhere";
        public const string Param_DataSource = "iDataSource";
        public const string Param_BOUpdateAction = "UpdateAction";
        public const string Param_SendGridAccountID = "SendGridAccountID";
        public const string Param_SendGridTemplateName = "SendGridTemplateName";
        public const string Param_SendGridTemplateID = "SendGridTemplateID";

        //Cache Merge Params
        public const string XIP_ActiveBO = "{XIP|ActiveBO}";
        public const string XIP_InstanceID = "{XIP|iInstanceID}";
        public const string XIP_BOName = "{XIP|sBOName}";
        public const string XIP_QSInstanceID = "{XIP|iQSInstanceID}";
        public const string XIP_1ClickID = "{XIP|i1ClickID}";
        public const string XIP_XILinkID = "{XIP|iXILinkID}";
        public const string XIP_BOAttributeID = "{XIP|iBOAttributeID}";
        public const string XIP_ActiveID = "{XIP|iActiveID}";
        public const string XIP_VisualisationID = "{XIP|iVisualID}";
        public const string XIP_iUserID = "{XIP|iUserID}";

        //Common Params
        public const string Param_ApplicationID = "iApplicationID";
        public const string Param_ApplicationXIGUID = "ApplicationXIGUID";
        public const string Param_ApplicationName = "sApplicationName";
        public const string Param_ApplicationType = "sApplicationType";
        public const string Param_UserID = "iUserID";
        public const string Param_CoreDatabase = "sDatabase";
        public const string Param_OrgDatabase = "sOrgDatabase";
        public const string Param_OrgName = "sOrgName";
        public const string Param_SessionID = "sSessionID";
        public const string Param_GUID = "sGUID";
        public const string Param_RoleID = "iRoleID";
        
        public const string Param_Hierarchy = "Hierarchy";
        public const string Param_InsertHierachy = "InsertHierarchy";
        public const string Param_UpdateHierarchy = "UpdateHierarchy";
        public const string Param_ViewHierarchy = "ViewHierarchy";
        public const string Param_DeleteHierarchy = "DeleteHierarchy";
        public const string Param_CallHierarchy = "sCallHierarchy";
        public const string Param_UserLevel = "iUserLevel";

        //Query
        public const string AgeDebtQuery = "select (select Convert(varchar(256), Customer_T.ID)+' '+Convert(varchar(256), Customer_T.sName) from Customer_T where id=[ACPolicy_T].FKiCustomerID) as 'Client', (select Convert(varchar(256), Supplier_T.ID)+' '+Convert(varchar(256), Supplier_T.sName) from Supplier_T where id=[ACPolicy_T].FKiSupplierID) as 'Supplier', [ACPolicy_T].ID as 'Policy', (select Convert(varchar(256), Enterprise_T.id)+' '+Convert(varchar(256), Enterprise_T.sName) from Enterprise_T where id=[ACPolicy_T].FKiEnterpriseID) as 'Enterprise', (select Convert(varchar(256), refAccountCategory_T.id)+' '+Convert(varchar(256), refAccountCategory_T.sName) from refAccountCategory_T where id=[ACPolicy_T].refAccountCategory) as 'AccountCategory', iif([ACPolicy_T].iStatus=5,'Pending Finance',iif([ACPolicy_T].iStatus=10,'New Business',iif([ACPolicy_T].iStatus=15,'Pending MTA',iif([ACPolicy_T].iStatus=25,'Renewal',iif([ACPolicy_T].iStatus=30,'Renewal Invited',iif([ACPolicy_T].iStatus=40,'Lapsed+Rebroke',iif([ACPolicy_T].iStatus=50,'Rebroke',iif([ACPolicy_T].iStatus=190,'Pending Cancellation',iif([ACPolicy_T].iStatus=200,'Cancelled','Lapsed'))))))))) as 'Status', [ACPolicy_T].dCoverStart, [ACPolicy_T].rPolicyBalance, [ACPolicy_T].FKiCustomerID, [ACPolicy_T].FKiSupplierID, [ACPolicy_T].id, [ACPolicy_T].sName from ACPolicy_T where [ACPolicy_T]."+ XIConstant.Key_XIDeleted + "=0";
        public const string AgeCreditQuery = "select distinct([ACPolicy_T].id) as 'Policy', (select Convert(varchar(256), Customer_T.ID)+' '+Convert(varchar(256), Customer_T.sName) from Customer_T where id=[ACPolicy_T].FKiCustomerID) as 'Client', (select Convert(varchar(256), Supplier_T.ID)+' '+Convert(varchar(256), Supplier_T.sName) from Supplier_T where id=[ACPolicy_T].FKiSupplierID) as 'Supplier', (select Convert(varchar(256), Enterprise_T.id)+' '+Convert(varchar(256), Enterprise_T.sName) from Enterprise_T where id=[ACPolicy_T].FKiEnterpriseID) as 'Enterprise', (select Convert(varchar(256), refAccountCategory_T.id)+' '+Convert(varchar(256), refAccountCategory_T.sName) from refAccountCategory_T where id=[ACPolicy_T].refAccountCategory) as 'AccountCategory', iif([ACPolicy_T].iStatus=5,'Pending Finance',iif([ACPolicy_T].iStatus=10,'New Business',iif([ACPolicy_T].iStatus=15,'Pending MTA',iif([ACPolicy_T].iStatus=25,'Renewal',iif([ACPolicy_T].iStatus=30,'Renewal Invited',iif([ACPolicy_T].iStatus=40,'Lapsed+Rebroke',iif([ACPolicy_T].iStatus=50,'Rebroke',iif([ACPolicy_T].iStatus=190,'Pending Cancellation',iif([ACPolicy_T].iStatus=200,'Cancelled','Lapsed'))))))))) as 'Status', [ACPolicy_T].dCoverStart, [ACPolicy_T].rPolicyBalance, [ACPolicy_T].FKiCustomerID, [ACPolicy_T].FKiSupplierID, [ACPolicy_T].id, [ACPolicy_T].sName, [ACPolicy_T].FKiProductID from ACPolicy_T LEFT JOIN ACJournalEntry_T on [ACPolicy_T].id=[ACJournalEntry_T].FKiACPolicyID INNER JOIN Supplier_T on [ACPolicy_T].FKiSupplierID=[Supplier_T].id where [ACJournalEntry_T].iReconciled=10 and [ACJournalEntry_T].FKiAccountID=[Supplier_T].FKiACAccountID and [ACJournalEntry_T]."+ XIConstant.Key_XIDeleted + " = 0";
        //Keywords
        public const string Key_XIDeleted = "XIDeleted";
        public const string Key_XICrtdWhn = "XICreatedWhen";
        public const string Key_XIUpdtdWhn = "XIUpdatedWhen";
        public const string Key_XICrtdBy = "XICreatedBy";
        public const string Key_XIUpdtdBy = "XIUpdatedBy";
        public const string Key_XIUpdtdWh = "XIUpdatedWhen";
        public const string Key_XIGUID = "XIGUID";
        public const string Key_Hierarchy = "sHierarchy";
        public const string Key_ID = "ID";
        public const string Key_Name = "sName";
        public const string Key_Order = "XIfOrder";

        //Email Template
        public const string Email_ForgotPassword = "Forgot Password";
        public const string Email_ResetPassword = "Reset Password";
        public const string Email_Welcome_Online = "Welcome_Online";
        public const string Email_Welcome_Internal = "Welcome_Internal";
        public const string Email_Welcome_InternalManual = "Welcome_InternalManual";
        public const string Email_OnCover = "OnCover";
        public const string Email_BrokerOnCover = "OnCover_Broker";
        public const string Email_InternalNewUserOnCover = "Internal_NewUserOnCover";
        public const string Email_InternalNewUserBrokerOnCover = "Internal_NewUserOnCover_Broker";
        public const string Email_OnCoverMTA = "OnCoverMTA";
        public const string Email_BrokerOnCoverMTA = "OnCoverMTA_Broker";
        public const string Email_OnCoverRenewal = "OnCoverRenewal";
        public const string Email_BrokerOnCoverRenewal = "OnCoverRenewal_Broker";
        public const string Email_InternalNewUserOnCoveRenewal = "Internal_NewUserOnCoverRenewal";
        public const string Email_InternalNewUserBrokerOnCoverRenewal = "Internal_NewUserOnCoverRenewal_Broker";
        public const string Email_InternalNewUserOnCoverMTA = "Internal_NewUserOnCoverMTA";
        public const string Email_InternalNewUserBrokerOnCoverMTA = "Internal_NewUserOnCoverMTA_Broker";
        public const string Email_SendQuotesToLead = "SendQuotesToLead";
        public const string Email_RequirementChase = "RequirementChase";
        public const string Email_MTARequest = "MTARequest";
        public const string Email_NoSequence = "No Sequence";
        public const string Email_BrokerNoSequence = "No Sequence_Broker";
        public const string Email_OnCover_Manual_PolicyPopup = "OnCover_Manual_PolicyPopup";
        public const string Email_OnCover_Manual_Lead = "OnCover_Manual_Lead";
        public const string Email_Internal_DisableClientEmail = "Internal_DisableClientEmail";
        public const string Email_Internal_DisableClientEmail_Broker = "Internal_DisableClientEmail_broker";
        public const string Email_OnCoverIndicative = "OnCover Indicative";
        public const string Lead_Transfer = "Lead_Transfer";

        //Other Constants
        public const string HierarchyNull = "XI.H|NULL";
        public const string HierarchyAll = "XI.H|ALL";

        //DefaultBO and BOAttributes Creation
        public const string DefaultTableName = "Sample_T";
        //public const string DefaultTableAttributes = "ID,sName,FKiOrganizationID,FKiApplicationID,sDescription,iStatus";
        public const string Auto3LayoutHTML = "<div class=\"row\"> <div class=\"col-md-2\" id=\"1\"></div> <div class=\"col-md-8\" id=\"2\"></div> <div class=\"col-md-2\" id=\"3\"></div> </div>";
        public const string DefaultLayout = @"<div class=""row""><div class=""col-md-12"" id=""1""></div></div>";
        public const string DefaultThemeID = "116";
        public const string DefaultQSThemeID = "169";
        public const string DefaultQSVisualisationID = "13";
        public const string DefaultTableAttributes = "ID\r\nsName\r\nsDescription\r\nsCode\r\niStatus\r\niType\r\nXIGUID\r\nsHierarchy";
        public const string DefaultAttributes = "ID, sName, sDescription, sCode, iStatus, iType";
        //public const string Auto5LayoutHTML = "<div class=\"row\"> <div class=\"col-md-12\" id=\"1\"></div> <div class=\"col-md-12\" id=\"2\"></div> <div class=\"col-md-12\" id=\"3\"></div> <div class=\"col-md-12\" id=\"4\"></div> <div class=\"col-md-12\" id=\"5\"></div> </div>";
        public const string Auto1LayoutHTML = "<div class=\"row\"> <div class=\"col-md-12\" id=\"1\"></div> </div>";

        //Groups
        public const string SummaryGroup = "Summary";
        public const string LabelGroup = "Label";
        public const string ShowGroup = "Show Group";
        public const string SaveGroup = "Save Group";
        public const string DefaultGroup = "Default";
        public const string DescriptionGroup = "Description";
        public const string Details1Group = "Details1";
        public const string CreateGroup = "Create";
        public const string SearchGroup = "Search";
        public const string EditGroup = "Create";
        public const string ThemeColor = "#d0592e";
        public const string FormLayoutTemplate = @"<div class=""row""><div class=""col-md-12"" id=""1""></div></div>";
        public const string MainGroup = "Main";

        //Auto Create Variables for popup
        public const string PopupLayoutName = "TabLayoutTemplate";
        public const string PopupTabArea = "PopupTabArea";
        public const string PopupOutputArea = "PopupTabContentArea";
        public const string PopupLeftPaneArea = "LeftPane";

        public const string FormLayoutName = "FormLayoutTemplate";
        public const string FormLayoutAtea = "FormContent";

        public const string Level1Layout = "Level1 Layout";
        public const string QSComponentHolder = "QSComponentHolder";

        //Left Tree Layout
        public const string LeftTreeLayout = "Left Tree Layout";
        public const string LeftTreeArea = "Tree Structure";
        public const string LeftTreeOutput = "LeftTreeOutput";
        public const string SummeryHolder = "SummeryHolder";

        //Sub Entity Layout
        public const string SubEntityLayout = "SubEntityLayout";
        public const string SubEntityListArea = "SubEntityListArea";
        public const string SubEntityDetailsArea = "SubEntityDetailsArea";

        //DB Type
        public const string DB_Type = "sDBType";
        public const string DB_Core = "Core";
        public const string DB_Nanno = "Nanno";
        public const string DB_Shared = "Shared";
        public const string DB_NameLength = "16";

        //Database Compare Related Constants
        public const string DataType = "DataType", MaxLength = "MaxLength", IsNull = "IsNull", IsIdentity = "IsIdentity", DefaultValue = "DefaultValue", GUID = "xiguid";
        //Database Compare Script Types
        public const string Source = "Source", Difference = "Difference", SourceDifference = "SourceDiff", TargetDifference = "TargetDiff", Target = "Target";
        public const string RecordsCacheKey = "DBRecords";
        //Simple1Click InlineXilink
        public const string SInlineXilink = "Simple 1Click Inline XiLink";


        //XI BO Action
        public const string Action_StructureCopy = "structurecopy";
        public const string Action_XIDelete = "xidelete";
        public const string Action_XI = "xi";
        public const string Action_ActionChain = "actionchain";
        public const string Action_XIAlgorithm = "xialgorithm";
        public const string Action_Script = "script";
        public const string Action_questionset = "questionset";
        public const string Action_DefaultPopup = "defaultpopup";
        public const string Action_ConfirmationDialog = "confirmationdialog";
        public const string Action_XILink = "xilink";

        //Bo Dashboards Default Constant Values
        public const string OrganisationLevel = "OrganisationLevel";
        public const string PlaceHolderClass = "col-md-12";
        public const string PlaceHolderArea = "div1";
        public const string DefaultURL = "XILink";
        public const string DefaultActiveXilink = "Y";
        public const string DefaultXiLinkType = "Content";
        public const int DefaultOneClickComponentID = 3;
        public const int DefaultFormComponentID = 2;
        public const int DefaultQSComponentID = 8;
        public const int DefaultPieChartComponentID = 30;
        public const int DefaultBarChartComponentID = 56;
        public const int DefaultAM4PieChartComponentID = 65;
        public const int DefaultAM4BarChartComponentID = 66;
        public const int DefaultAM4LineChartComponentID = 64;
        public const int DefaultStatusTypeID = 10;
        public const string DefaultStartAction = "StartAction";
        public const string DefaultDialog = "Dialog";
        public const string DefaultDialogID = "DialogID";
        public const string DefaultXiComponent = "XIComponent";
        public const string DefaultAuthenticated = "Authenticated";
        //Dashboards AM4 Charts
        public const string AM4PriceChartComponent = "AM4PriceChartComponent";
        public const string AM4PieChartComponent = "AM4PieChartComponent";
        public const string AM4HeatChartComponent = "AM4HeatChartComponent";
        public const string AM4LineChartComponent = "AM4LineChartComponent";
        public const string AM4BarChartComponent = "AM4BarChartComponent";
        public const string AM4SemiPieChartComponent = "AM4SemiPieChartComponent";
        public const string AM4GaugeChartComponent = "AM4GaugeChartComponent";
        public const string ReportDataComponent = "ReportDataComponent";
        public const string AM4ColumnBarChartComponent = "AM4ColumnBarChartComponent";
        public const string AM4ColumnLineChartComponent = "AM4ColumnLineChartComponent";
        public const string AM4FunnelChartComponent = "AM4FunnelChartComponentnew";

        public const string Param_RowXilinkID = "RowXilinkID";

        public const string IDRef_internal = "internal id reference";
        public const string IDRef_public = "public id reference";
        public const string Chosen_Plugin = "Chosen";
    }
}