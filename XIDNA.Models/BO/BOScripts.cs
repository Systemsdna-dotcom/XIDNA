using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.Web.Mvc;

namespace XIDNA.Models
{
    [Table("XIBOScript_T")]
    public class BOScripts : CommonProperties
    {
        [Key]
        public long ID { get; set; }
        public int FKiBOID { get; set; }
        [Required(ErrorMessage = "Enter Script name")]
        //[RegularExpression(@"^[ A-Za-z0-9_@.#+-*/$&]+$", ErrorMessage = "Special characters not allowed")]
        [StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        //[Remote("IsExistsBOName", "BusinessObjects", AdditionalFields = "BOID", HttpMethod = "POST", ErrorMessage = "BO name already exists. Please enter a different Name.")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Enter Description")]
        [StringLength(512, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string sDescription { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Enter Script")]
        public string sScript { get; set; }
        [Required(ErrorMessage = "Select Script")]
        public string sType { get; set; }
        [Required(ErrorMessage = "Select Script")]
        public string sLanguage { get; set; }
        public string sVersion { get; set; }
        public string sMethodName { get; set; }
        [ForeignKey("FKiScriptID")]
        public virtual ICollection<BOScriptResults> ScriptResults { get; set; }
        public List<VMDropDown> ddlStatusTypes { get; set; }
        public List<VMDropDown> ddlScriptTypes { get; set; }
        public List<VMDropDown> ddlLanguages { get; set; }
        [NotMapped]
        public bool IsSuccess { get; set; }
        [NotMapped]
        public string sFieldName { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
    }

    [Table("XIBOScriptResult_T")]
    public class BOScriptResults
    {
        [Key]
        public long ID { get; set; }
        public long FKiScriptID { get; set; }
        [NotMapped]
        public string sResult { get; set; }
        [NotMapped]
        public string sAction { get; set; }
        [NotMapped]
        public string sMessage { get; set; }
        public string sResultCode { get; set; }
        public int iType { get; set; }
        public int iAction { get; set; }
        public string sUserError { get; set; }

    }

}
