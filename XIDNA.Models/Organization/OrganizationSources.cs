using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    public class OrganizationSources
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage="Select Organization")]
        public int OrganizationID { get; set; }
        public int Type { get; set; }
        [Required(ErrorMessage = "Enter Source Name")]
        [System.Web.Mvc.Remote("IsExistsSourceName", "Organization", AdditionalFields = "CreationType, ID, OrganizationID", HttpMethod = "POST", ErrorMessage = "Source Name already exists.")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Provider")]
        public int Provider { get; set; }
         //[Required(ErrorMessage = "Select Icon")]
        public string Icon { get; set; }
        [Required(ErrorMessage = "Enter Email ID")]
        [EmailAddress]
        [MaxLength(128, ErrorMessage = "Email cannot be longer than 128 characters.")]
        //[System.Web.Mvc.Remote("IsExistsSrcEmail", "Organization", AdditionalFields = "Type, ID, OldEmail", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Enter Mobile Number")]
        [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string MobileNumber { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> OrgList { get; set; }
        [NotMapped]
        public string CreationType { get; set; }
        [NotMapped]
        public List<string> SourceFields { get; set; }
    }
}
