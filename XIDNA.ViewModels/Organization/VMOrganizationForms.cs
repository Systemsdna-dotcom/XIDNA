using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace XIDNA.ViewModels
{
    public class VMOrganizationForms
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Select organization")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Select type")]
        public int Type { get; set; }
        [Required(ErrorMessage = "Enter source name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only Alphabets allowed")]
        [System.Web.Mvc.Remote("IsExistsSourceName", "Organization", AdditionalFields = "ID, OrganizationID", HttpMethod = "POST", ErrorMessage = "Source Name already exists.")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        public int StatusTypeID { get; set; }
        public List<VMDropDown> OrgList { get; set; }
        public string CreationType { get; set; }
        public List<VMDropDown> SourceTypes { get; set; }
        public List<string> SourceFields { get; set; }
        public int SubID { get; set; }
        [Required(ErrorMessage = "Select source")]
        public int SourceID { get; set; }
        public string SubsriptionID { get; set; }
        [Required(ErrorMessage = "Select organization")]
        public int SubOrganizationID { get; set; }
        [Required(ErrorMessage = "Select class")]
        public int ClassID { get; set; }
        public string Icon { get; set; }
        [Required(ErrorMessage = "Select location code")]
        public string LocationCode { get; set; }
        [Required(ErrorMessage = "Select provider")]
        public int Provider { get; set; }
        public int SubStatusTypeID { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers allowed")]
        public int LeadCost { get; set; }
        [Required(ErrorMessage = "Enter contact email")]
        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email ")]
        [MaxLength(128, ErrorMessage = "Email cannot be longer than 128 characters.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter post code")]
        [RegularExpression(@"^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][ABD-HJLNP-UW-Z]{2}$", ErrorMessage = "Invalid Post Code. Ex: SW1W 0NY OR L1 8JQ")]
        public string PostCode { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers allowed")]
        public int RenewalDate { get; set; }
        public List<VMDropDown> SubOrgList { get; set; }
        public List<VMDropDown> ProviderList { get; set; }
        public List<VMDropDown> SourcesList { get; set; }
        public List<VMDropDown> ClassesList { get; set; }
        public List<VMDropDown> LocationCodes { get; set; }
        public int SorFieldID { get; set; }
        [Required(ErrorMessage = "Select organization")]
        public int SorFieldOrganizationID { get; set; }
        [Required(ErrorMessage = "Select subscription")]
        public string SorFieldSubscriptionID { get; set; }
        [Required(ErrorMessage="Select field")]
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public List<VMDropDown> SorFieldOrgsList { get; set; }
        public List<VMDropDown> Subscriptions { get; set; }
        public string SorFieldSubscription { get; set; }
        public int OrgID { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Enter email")]
        public string EmailID { get; set; }
        [Required(ErrorMessage = "Enter mobile number")]
        [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string MobileNumber { get; set; }
        public string Role { get; set; }
        public int CopySubID { get; set; }
        [Required]
        public string FieldValue { get; set; }
        public List<VMDropDown> ColumnsList { get; set; }
    }
}
