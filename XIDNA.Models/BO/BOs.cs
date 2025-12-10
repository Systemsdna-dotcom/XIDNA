using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    //[Table("XIBO_T")]
    [Table("XIBO_T_N")]
    public class BOs : CommonProperties
    {
        public BOs()
        {
            this.BOFields = new HashSet<BOFields>();
        }
        [Key]
        public int BOID { get; set; }
        [Required(ErrorMessage = "Enter BO name")]
        //[RegularExpression(@"^[ A-Za-z0-9_@.#+-*/$&]+$", ErrorMessage = "Special characters not allowed")]
        [StringLength(512, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 1)]
        [Remote("IsExistsBOName", "BusinessObjects", AdditionalFields = "BOID", HttpMethod = "POST", ErrorMessage = "BO name already exists. Please enter a different Name.")]
        public string Name { get; set; }
        public string sNameAttribute { get; set; }
        public int OrganizationID { get; set; }
        public int FKiApplicationID { get; set; }
        public int TypeID { get; set; }
        [Required(ErrorMessage = "Enter description")]
        //[StringLength(256, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 1)]
        public string Description { get; set; }
        [NotMapped]
        public int ClassAttribute { get; set; }
        public bool IsClassEnabled { get; set; }
        //[Required(ErrorMessage = "Enter BO field count")]
        //[RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers allowed")]
        public int FieldCount { get; set; }
        public string LabelName { get; set; }
        public string TableName { get; set; }
        public string ClassName { get; set; }
        [ForeignKey("BOID")]
        public virtual ICollection<BOFields> BOFields { get; set; }
        public virtual ICollection<BOGroupFields> BOGroups { get; set; }
        [ForeignKey("FKiBOID")]
        public virtual ICollection<BOScripts> BOScripts { get; set; }
        [NotMapped]
        public List<VMDropDown> StatusTypes { get; set; }
        [NotMapped]
        public string ColName { get; set; }

        public bool IsLeaf
        {
            get
            {
                return this.BOFields.Count == 0;
            }
        }
        public string sType { get; set; }
        public string sSection { get; set; }

        public string sVersion { get; set; }
        public string sSize { get; set; }
        public bool bUID { get; set; }
        public string sPrimaryKey { get; set; }
        public string sTimeStamped { get; set; }
        public string sDeleteRule { get; set; }
        public string sSearchType { get; set; }
        public int iDataSource { get; set; }
        public Guid? iDataSourceXIGUID { get; set; }
        public string sNotes { get; set; }
        public string sHelpItem { get; set; }
        public int iTransactionEnable { get; set; }
        [NotMapped]
        public List<VMDropDown> HelpTypes { get; set; }
        public string sUpdateVersion { get; set; }
        public string sAudit { get; set; }
        public int iUpdateCount { get; set; }
        [NotMapped]
        public List<VMDropDown> DataSources { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlBOFieldAttributes { get; set; }
        [NotMapped]
        public string AuditType { get; set; }
        public string sAuditBOName { get; set; }
        public bool bIsAutoIncrement { get; set; }
        public bool bIsHierarchy { get; set; }
        public string sColumns { get; set; }
        public string sDashBoardType { get; set; }
        public string sLastUpdate { get; set; }
        public string KPI { get; set; }
        public bool bIsEncrypt { get; set; }
        public Guid XIGUID { get; set; }
    }
}
