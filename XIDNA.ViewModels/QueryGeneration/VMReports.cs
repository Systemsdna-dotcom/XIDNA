using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.ViewModels
{
    public class VMReports
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string ShowAs { get; set; }
        public string Location { get; set; }
        public string BO { get; set; }
        public string BOName { get; set; }
        public int ClassID { get; set; }
        public int Status { get; set; }
        public bool IsGroupDefined { get; set; }
        public int? ParentID { get; set; }
        public bool IsParent { get; set; }
        public bool IsMultipleBo { get; set; }
        public bool bIsMultiBO { get; set; }
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Select business object")]
        public int BOID { get; set; }

        public Guid BOIDXIGUID { get; set; }
        [Required(ErrorMessage = "Enter query name")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special characters not allowed")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        //[Remote("IsExistsQueryName", "QueryGeneration", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Query name already exists. Please enter a different Name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter show as name")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Title { get; set; }
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        //[Required(ErrorMessage = "Select type of query")]
        public int TypeID { get; set; }
        public string Query { get; set; }
        public string VisibleQuery { get; set; }
        [Required(ErrorMessage = "Select display type")]
        public int DisplayAs { get; set; }
        public int ResultListDisplayType { get; set; }
        public string ResultIn { get; set; }
        public string PopupType { get; set; }
        public string DialogType { get; set; }
        [Required(ErrorMessage = "Enter left position value")]
        public int? PopupLeft { get; set; }
        [Required(ErrorMessage = "Enter top position value")]
        public int? PopupTop { get; set; }
        [Required(ErrorMessage = "Enter width value")]
        public int? PopupWidth { get; set; }
        [Required(ErrorMessage = "Enter height value")]
        public int? PopupHeight { get; set; }

        public string DialogMy1 { get; set; }
        public string DialogMy2 { get; set; }
        public string DialogAt1 { get; set; }
        public string DialogAt2 { get; set; }
        public int RowXiLinkID { get; set; }
        public string sRowXiLinkType { get; set; }
        public int ColumnXiLinkID { get; set; }
        public int CellXiLinkID { get; set; }

        //[Required(ErrorMessage = "Select class")]
        public int Class { get; set; }
        public string ClassName { get; set; }
        public int StatusTypeID { get; set; }
        public bool IsFilterSearch { get; set; }
        public bool IsNaturalSearch { get; set; }
        public bool IsExport { get; set; }
        public bool IsDynamic { get; set; }
        public bool IsStoredProcedure { get; set; }
        public string SelectFields { get; set; }
        public string FromBos { get; set; }
        public string WhereFields { get; set; }
        public string GroupFields { get; set; }
        public string OrderFields { get; set; }
        public string ActionFields { get; set; }
        public string ActionFieldValue { get; set; }
        public string ViewFields { get; set; }
        public string EditableFields { get; set; }
        public string NonEditableFields { get; set; }
        [Required(ErrorMessage = "Enter description")]
        [StringLength(250, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Description { get; set; }
        //public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public String CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string SearchFields { get; set; }
        public int InnerReportID { get; set; }
        public bool IsRowClick { get; set; }
        public bool bIsMultiSearch { get; set; }
        public string OnRowClickType { get; set; }
        public int OnRowClickValue { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public string OnClickParameter { get; set; }
        public string OnColumnClickType { get; set; }
        public int OnColumnClickValue { get; set; }
        public bool IsCellClick { get; set; }
        public string OnClickCell { get; set; }
        public string OnCellClickType { get; set; }
        public int OnCellClickValue { get; set; }
        public bool IsRowTotal { get; set; }
        public bool IsColumnTotal { get; set; }
        public string parent { get; set; }
        public int TargetID { get; set; }
        [Required(ErrorMessage = "Select user")]
        public int TargetUsers { get; set; }
        [Required(ErrorMessage = "Select column")]
        public int TargetColumns { get; set; }
        [Required(ErrorMessage = "Enter target value")]
        public int Targets { get; set; }
        public string TargetPeriod { get; set; }
        public string Colour { get; set; }
        public bool IsSMS { get; set; }
        public bool IsEmail { get; set; }
        public bool IsNotification { get; set; }
        public int QueryID { get; set; }
        public int TarQueryID { get; set; }
        public int SchQueryID { get; set; }
        public string OtherPopup { get; set; }
        public int ClassType { get; set; }
        public List<VMDropDown> Classes { get; set; }
        public List<string> selectrightfields { get; set; }
        //[NotMapped]
        //public List<string> BOs { get; set; }
        public string generalquery { get; set; }
        public string generalselectedfields { get; set; }
        public string selectedgroupfields { get; set; }
        public List<groups> groupallfields { get; set; }
        public string selectedfields { get; set; }
        public string SelWithTypes { get; set; }
        public string WhereWithTypes { get; set; }
        public string PopType { get; set; }
        public string SelectWithAlias { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Select type")]
        public string AlertType { get; set; }
        public List<VMDropDown> AllBOs { get; set; }
        public Dictionary<string, string> AllBOss { get; set; }//auto complete fields
        public List<VMDropDown> ReportTypes { get; set; }
        public List<VMDropDown> InnerReports { get; set; }
        public List<VMDropDown> TargetUsersList { get; set; }
        public List<VMDropDown> PopupsList { get; set; }
        public List<VMDropDown> XiLinksList { get; set; }
        public int SchedulerID { get; set; }
        [Required(ErrorMessage = "Select user")]
        public int UserID { get; set; }
        public int XilinkID { get; set; }
        public string Time { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int EmailTemplateID { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int SchEmailTemplateID { get; set; }
        public List<VMDropDown> EmailTemplates { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int SMSTemplateID { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int SchSMSTemplateID { get; set; }
        public List<VMDropDown> SMSTemplates { get; set; }
        public string Day { get; set; }
        public int Date { get; set; }
        public List<string> Parent1Clicks { get; set; }
        public string InboxCount { get; set; }
        public string Percentage { get; set; }
        public string HTMLCode { get; set; }
        public List<VMDropDown> StatusTypes { get; set; }
        [NotMapped]
        public bool bIsXICreatedBy { get; set; }
        [NotMapped]
        public bool bIsXICreatedWhen { get; set; }
        [NotMapped]
        public bool bIsXIUpdatedBy { get; set; }
        [NotMapped]
        public bool bIsXIUpdatedWhen { get; set; }
        [NotMapped]
        public int FKiCrtd1ClickID { get; set; }
        [NotMapped]
        public int FKiUpdtd1ClickID { get; set; }
        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public List<VMDropDown> ddlRoles { get; set; }
        public List<VMDropDown> ddlBOGroups { get; set; }
        public string CreateScript { get; set; }
        public string EditSscript { get; set; }
        public string DeleteScript { get; set; }
        public int CreateRoleID { get; set; }
        public int EditRoleID { get; set; }
        public int DeleteRoleID { get; set; }
        public int CreateGroupID { get; set; }
        public int EditGroupID { get; set; }
        public List<VMDropDown> ddlLayouts { get; set; }
        public List<VMDropDown> ddlLayoutMappings { get; set; }
        public int iLayoutID { get; set; }
        public int XIComponent { get; set; }
        public int RepeaterType { get; set; }
        public int RepeaterComponent { get; set; }
        public int FKiComponentID { get; set; }
        public List<VMDropDown> XIComponentList { get; set; }
        public List<VMNameValuePairs> NVs { get; set; }
        public List<VMNameValuePairs> NDVs { get; set; }
        [NotMapped]
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields
        [NotMapped]
        public List<VMDropDown> ddlOneClicks { get; set; } // Auto Complete OneClicks
        public string sAddLabel { get; set; }
        public string sCreateType { get; set; }
        public int iCreateXILinkID { get; set; }
        public int iType { get; set; }
        public string sCode { get; set; }
        public bool bIsCreate { get; set; }
        public bool bIsEdit { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsDelete { get; set; }
        public bool IsRefresh { get; set; }
        public bool bIsCheckbox { get; set; }
        public bool bIsView { get; set; }
        public string sFileExtension { get; set; }
        public int iPaginationCount { get; set; }
        public Guid XIGUID { get; set; }
        [NotMapped]
        public string[] XI1ClickLinks { get; set; }
        [NotMapped]
        public int OneClickID { get; set; }
        [NotMapped]
        public List<VMXI1ClickLinks> OneClickXILinks { get; set; }
        [NotMapped]
        public List<int> GroupIDs { get; set; }
        public bool bIsLockToUser { get; set; }
        public int FKiVisualisationID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlVisualisations { get; set; }
        //[NotMapped]
        //public List<VMSignalRsettings> SiganlRUsersSettings { get; set; }
        public string sLog { get; set; }
        public int FKiApplicationID { get; set; }
        public string ConstantID { get; set; }
    }

    public class groups
    {
        public string singlefield { get; set; }
        public string singlefieldtype { get; set; }
        public string singlealiasname { get; set; }
        public Nullable<int> groupid { get; set; }
        public string groupname { get; set; }
        public string bofieldnames { get; set; }
        public string bosqlfieldnames { get; set; }
    }
}
