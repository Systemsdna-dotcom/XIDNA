using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMActionTypes
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Select Organization")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsActionName", "Lead", AdditionalFields = "ID, OrganizationID", HttpMethod = "POST", ErrorMessage = "Action Name already exists.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Type")]
        public int Type { get; set; }
        public int StatusTypeID { get; set; }
        public bool IsStage { get; set; }
        [Required(ErrorMessage = "Select Stage")]
        public int StageID { get; set; }
        public bool IsSMS { get; set; }
        [Required(ErrorMessage = "Select Template")]
        public int SMSTemplateID { get; set; }
        public bool IsEmail { get; set; }
        [Required(ErrorMessage = "Select Template")]
        public int EmailTemplateID { get; set; }
        public bool IsPopup { get; set; }
        [Required(ErrorMessage = "Select Popup")]
        public int PopupID { get; set; }
        [Required(ErrorMessage = "Enter 1-Click")]
        public string Query { get; set; }
        public bool IsOneClick { get; set; }
        [NotMapped]
        public List<VMDropDown> Templates { get; set; }
        [NotMapped]
        public List<VMDropDown> SMSTemplates { get; set; }
        [NotMapped]
        public List<VMDropDown> EmailTemplates { get; set; }
        [NotMapped]
        public List<VMDropDown> Stages { get; set; }
        [NotMapped]
        public List<VMDropDown> OneClicks { get; set; }
        [NotMapped]
        public List<VMDropDown> Popups { get; set; }
        [NotMapped]
        public List<VMDropDown> organizations { get; set; }
        [NotMapped]
        public int OrgID { get; set; }
    }
}
