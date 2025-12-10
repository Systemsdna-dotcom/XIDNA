using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMOrganization
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter organization name")]
        [System.Web.Mvc.Remote("IsExistsOrgName", "Organization", AdditionalFields = "Type, ID", HttpMethod = "POST", ErrorMessage = "Organization name already exists. Please enter a different name.")]
        [StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Organization Type")]
        public int TypeID { get; set; }
        [StringLength(1024, ErrorMessage = "The {0} should not be longer than {1} characters.")]
        public string Description { get; set; }
        [StringLength(1024, ErrorMessage = "The {0} path should not be longer than {1} characters.")]
        public string Logo { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only numerics allowed")]
        public int NoOfUsers { get; set; }
        [StringLength(1024, ErrorMessage = "The {0} should not be longer than {1} characters.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Enter organization phone number")]
        [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Enter organization email")]
        [EmailAddress]
        
        [MaxLength(128, ErrorMessage = "Email cannot be longer than 128 characters.")]
        [System.Web.Mvc.Remote("IsExistsOrgEmail", "Organization", AdditionalFields = "Type, ID, OldEmail", HttpMethod = "POST", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }
        [StringLength(128, ErrorMessage = "The {0} should not be longer than {1} characters.")]
       
        [RegularExpression(@"^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][ABD-HJLNP-UW-Z]{2}$", ErrorMessage = "Invalid Post Code. Ex: SW1W 0NY OR L1 8JQ")]
        public string PostCode { get; set; }
        [StringLength(1024, ErrorMessage = "The {0} should not be longer than {1} characters.")]
        public string Website { get; set; }
        [Required(ErrorMessage = "Enter organization password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[RegularExpression(@"^.(?=.{6,})(?=.[a-z])(?=.[A-Z])(?=.[\d\W]).*$", ErrorMessage = "Password Must have one digit and one upper case letter")]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must be 6 characters long & combination of digits, upper and lower case letters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public String CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string DatabaseType { get; set; }
        public string DatabaseName { get; set; }
        [NotMapped]
        public string Todo { get; set; }

        [NotMapped]
        public int ConID { get; set; }
        [Required(ErrorMessage = "Enter contact name")]
        [StringLength(64, ErrorMessage = "Name must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string ConName { get; set; }
        [Required(ErrorMessage = "Enter contact number")]
        [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string ConPhone { get; set; }
        [NotMapped]
        public int ConOrganizationID { get; set; }
        [Required(ErrorMessage = "Enter contact email")]
        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please enter correct email ")]
        [MaxLength(64, ErrorMessage = "Email cannot be longer than 128 characters.")]
        public string ConEmail { get; set; }
        [Required(ErrorMessage = "Enter contact address")]
        [StringLength(128, ErrorMessage = "Address must be between {2} to {1} characters.", MinimumLength = 2)]
        public string ConAddress { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string OldEmail { get; set; }
        public List<VMDropDown> Classes { get; set; }
        public List<string> ClassIDs { get; set; }
        public int OrganizationID { get; set; }
        public int LocID { get; set; }
        public string CreationType { get; set; }
        //[Required(ErrorMessage = "Select the Organisation.")]
        public int LocOrganizationID { get; set; }
        [Required]
        [StringLength(32, ErrorMessage = "The {0} name must be at least {2} to {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only alpha neumeric allowed")]
        [System.Web.Mvc.Remote("CheckLocation", "Organization", AdditionalFields = "LocOrganizationID, LocID", HttpMethod = "Post", ErrorMessage = "Location already exists!")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Enter location code")]
        [StringLength(32, ErrorMessage = "The {0} Name must be at least {2} to {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9_ ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [System.Web.Mvc.Remote("IsExistsLocationCode", "Organization", AdditionalFields = "LocOrganizationID, LocID", HttpMethod = "Post", ErrorMessage = "Location code already exists!")]
        public string LocationCode { get; set; }
        public List<VMDropDown> OrgDetails { get; set; }
        public int LocStatusTypeID { get; set; }
        public List<string> OrgLocations { get; set; }
        
    }
}
