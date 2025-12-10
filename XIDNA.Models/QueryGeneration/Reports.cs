using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XIDNA.ViewModels;
namespace XIDNA.Models
{
    [Table("XI1Click_T")]
    public class Reports : CommonProperties
    {
        public Reports()
        {
            this.Sub1Clicks = new HashSet<Reports>();
        }
        //[Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int? ParentID { get; set; }
        public bool IsParent { get; set; }
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "Select business object")]
        public int BOID { get; set; }
        public Guid BOIDXIGUID { get; set; }
        [Required(ErrorMessage = "Enter query name")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special characters not allowed")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        
        //commenting out as XI1Click_T has lot of duplicat names
        //[Remote("IsExistsQueryName", "QueryGeneration", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Query name already exists. Please enter a different Name.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter show as name")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Title { get; set; }
        //[StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Code { get; set; }
        [Required(ErrorMessage = "Select type of query")]
        public int TypeID { get; set; }
        public string Query { get; set; }
        public string VisibleQuery { get; set; }
        [Required(ErrorMessage = "Select display type")]
        public int DisplayAs { get; set; }
        public int ResultListDisplayType { get; set; }
        public string ResultIn { get; set; }
        public string PopupType { get; set; }
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
        public bool bIsAddBottom { get; set; }
        public bool bIsAddTop { get; set; }
        public int ColumnXiLinkID { get; set; }
        public int CellXiLinkID { get; set; }
        //not in use for now
        //[Required(ErrorMessage = "Select class")]
        public int Class { get; set; }
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
        public string SearchFields { get; set; }
        public int InnerReportID { get; set; }
        public bool IsRowClick { get; set; }
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
        public int iPaginationCount { get; set; }
        public bool IsColumnTotal { get; set; }
        public bool bIsMultiBO { get; set; }
        public bool bIsExport { get; set; }
        public string sFileExtension { get; set; }
        [ForeignKey("ParentID")]
        public virtual ICollection<Reports> Sub1Clicks { get; set; }
        public bool IsLeaf
        {
            get
            {
                return this.Sub1Clicks.Count == 0;
            }
        }
        [NotMapped]
        public int Targets { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string HTMLCode { get; set; }
        [NotMapped]
        public string InboxCount { get; set; }
        [NotMapped]
        public string Percentage { get; set; }

        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public int CreateRoleID { get; set; }
        public int EditRoleID { get; set; }
        public int DeleteRoleID { get; set; }
        public int CreateGroupID { get; set; }
        public int EditGroupID { get; set; }
        public int iLayoutID { get; set; }
        public string sSystemType { get; set; }
        public int FKiComponentID { get; set; }
        public int FKiVisualisationID { get; set; }
        public int RepeaterType { get; set; }
        public int RepeaterComponentID { get; set; }
        public int iCreateXILinkID { get; set; }
        public string sAddLabel { get; set; }
        public string sCreateType { get; set; }
        public bool IsRefresh { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsView { get; set; }
        public bool bIsCheckbox { get; set; }
        public bool bIsMultiSearch { get; set; }
        public bool bIsLockToUser { get; set; }
        public bool bIsXICreatedBy { get; set; }
        [NotMapped]
        public bool bIsXICreatedWhen { get; set; }
        public bool bIsXIUpdatedBy { get; set; }
        [NotMapped]
        public bool bIsXIUpdatedWhen { get; set; }
        public int FKiCrtd1ClickID { get; set; }
        public int FKiUpdtd1ClickID { get; set; }
        public string sLog { get; set; }
        public int FKiApplicationID { get; set; }
        public Guid XIGUID { get; set; }
        public string sCode { get; set; }
        public Guid RowXiLinkIDXIGUID { get; set; }
    }

    public class IntOperatorOptions : List<DisplayName>
    {
        public IntOperatorOptions()
        {
            this.Add(new DisplayName() { Type = "EqualTo", DisplayValue = "=", Value = "=" });
            this.Add(new DisplayName() { Type = "NotEqualTo", DisplayValue = "!=", Value = "!=" });
            this.Add(new DisplayName() { Type = "Greaterthan", DisplayValue = ">", Value = ">" });
            this.Add(new DisplayName() { Type = "Lessthan", DisplayValue = "<", Value = "<" });
            this.Add(new DisplayName() { Type = "GreaterthanorEqualTo", DisplayValue = ">=", Value = ">=" });
            this.Add(new DisplayName() { Type = "LessthanorEqualTo", DisplayValue = "<=", Value = "<=" });
            //this.Add(new DisplayName() { Type = "IN", DisplayValue = "IN", Value = "60" });
            //this.Add(new DisplayName() { Type = "NOTIN", DisplayValue = "NOT IN", Value = "70" });
            //this.Add(new DisplayName() { Type = "ISNULL", DisplayValue = "ISNULL", Value = "80" });
            //this.Add(new DisplayName() { Type = "ISNOTNULL", DisplayValue = "NOT NULL", Value = "90" });
        }
    }

    public class BITOperatorOptions : List<DisplayName>
    {
        public BITOperatorOptions()
        {
            this.Add(new DisplayName() { Type = "EqualTo", DisplayValue = "=", Value = "=" });
            this.Add(new DisplayName() { Type = "NotEqualTo", DisplayValue = "!=", Value = "!=" });
        }
    }

    public class StringOperatorOptions : List<DisplayName>
    {
        public StringOperatorOptions()
        {
            this.Add(new DisplayName() { Type = "EqualTo", DisplayValue = "=", Value = "=" });
            this.Add(new DisplayName() { Type = "NotEqualTo", DisplayValue = "!=", Value = "!=" });
            this.Add(new DisplayName() { Type = "STARTSWITH", DisplayValue = "STARTS WITH", Value = "LIKE" });
            this.Add(new DisplayName() { Type = "NOTSTARTSWITH", DisplayValue = "NOT STARTS WITH", Value = "NOT LIKE" });
            this.Add(new DisplayName() { Type = "EndsWith", DisplayValue = "ENDS WITH", Value = "LIKE" });
            this.Add(new DisplayName() { Type = "NotEndsWith", DisplayValue = "NOT ENDS WITH", Value = "NOT LIKE" });
            this.Add(new DisplayName() { Type = "CONTAINS", DisplayValue = "CONTAINS", Value = "LIKE" });
            //this.Add(new DisplayName() { Type = "NOTCONTAINS", DisplayValue = "NOT CONTAINS", Value = "50" });
            //this.Add(new DisplayName() { Type = "LIKE", DisplayValue = "LIKE", Value = "Like" });
            //this.Add(new DisplayName() { Type = "IN", DisplayValue = "IN", Value = "70" });
            //this.Add(new DisplayName() { Type = "NOTIN", DisplayValue = "NOT IN", Value = "80" });
            //this.Add(new DisplayName() { Type = "ISNULL", DisplayValue = "ISNULL", Value = "90" });
            //this.Add(new DisplayName() { Type = "ISNOTNULL", DisplayValue = "NOT NULL", Value = "100" });

        }
    }
    public class AppendOptions : List<DisplayName>
    {
        public AppendOptions()
        {
            this.Add(new DisplayName() { Type = "AND", DisplayValue = "AND", Value = "10" });
            this.Add(new DisplayName() { Type = "OR", DisplayValue = "OR", Value = "20" });
            this.Add(new DisplayName() { Type = "ANDNOT", DisplayValue = "AND NOT", Value = "30" });
            this.Add(new DisplayName() { Type = "ORNOT", DisplayValue = "OR NOT", Value = "40" });
        }
    }
    public class DisplayName
    {
        public string Type { get; set; }
        public string DisplayValue { get; set; }
        public string Value { get; set; }
    }


}
