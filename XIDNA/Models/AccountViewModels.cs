using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using XICore;

namespace XIDNA.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Usre Name")]
        //[EmailAddress]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string sTheme { get; set; }
        //[Required]
        //[Display(Name = "OTP")]
        //public string OTP { get; set; }
    }

    //public class RegisterViewModel
    //{
    //    [Required]
    //    //[EmailAddress]
    //    [Display(Name = "UserName")]
    //    public string UserName { get; set; }

    //    [Required]
    //    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    //    [DataType(DataType.Password)]
    //    [Display(Name = "Password")]
    //    public string Password { get; set; }

    //    [DataType(DataType.Password)]
    //    [Display(Name = "Confirm password")]
    //    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    //    public string ConfirmPassword { get; set; }
    //}

    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Enter New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must Be 8 Characters long & Combination Of Digits, Upper And Lower Case Letters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Please enter verification code")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [NotMapped]
        public string Error { get; set; }
        [NotMapped]
        public int Value { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Please enter valid Email address")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginWithOtp
    {
        //[Display(Name = "UserName")]
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "OTP")]
        public List<string> OTP { get; set; }
        //[Display(Name = "Password")]
        public string Password { get; set; }
        public int iUserID { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int sLength { get; set; }
        public bool Identifier { get; set; }
    }
    public class RegisterViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Special characters not allowed")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Special characters not allowed")]
        public string LastName { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Enter Email Address")]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Email")]
        [System.Web.Mvc.Remote("IsExistsEmpEmail", "Users", HttpMethod = "POST", AdditionalFields = "Type,Id", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        [RegularExpression(@"^[0][1-9]\d{9}$|^[1-9]\d{10}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string PhoneNumber { get; set; }   
        [Required(ErrorMessage = "Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must Be 6 Characters long & Combination Of Digits, Upper And Lower Case Letters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public List<XIInfraRoles> Group { get; set; }
        [Required]
        public string GroupID { get; set; }
        [Required(ErrorMessage = "Select Primary Role")]
        public int RoleID { get; set; }
        [Required(ErrorMessage = "Select Reporting Authority")]
        public int ReportTo { get; set; }
        public string Location { get; set; }
        public string sHierarchy { get; set; }
        public string sInsertDefaultCode { get; set; }
        public string sUpdateHierarchy { get; set; }
        public string sViewHierarchy { get; set; }
        public string sDeleteHierarchy { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public List<VMDropDown> DropDown { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Select Location")]
        public List<string> Locs { get; set; }
        [NotMapped]
        public int OrganizationID { get; set; }
        [NotMapped]
        public List<VMDropDown> HierarchyDropDown { get; set; }
        [NotMapped]
        //[Required(ErrorMessage = "Select Hierarchy")]
        public List<string> Hierarchy { get; set; }
        public int FKiTeamID { get; set; }
        public List<VMDropDown> TeamsDropDown { get; set; }
    }
    public class EditUserViewModel
    {
        [Required(ErrorMessage = "Enter First Name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Special characters not allowed")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Special characters not allowed")]
        public string LastName { get; set; }
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Enter Email Address")]
        [Display(Name = "Email")]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsEmpEmail", "Users", HttpMethod = "POST", AdditionalFields = "Type ,Id", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Enter Phone Number")]
        [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string PhoneNumber { get; set; }
        public List<XIInfraRoles> Group { get; set; }
        public string GroupID { get; set; }
        [Required(ErrorMessage = "Select Primary Role")]
        public int RoleID { get; set; }
        [Required(ErrorMessage = "Select Reporting Authority")]
        public int ReportTo { get; set; }
        public string Location { get; set; }
        public int PaginationCount { get; set; }
        public string Menu { get; set; }
        public string sHierarchy { get; set; }
        public string sInsertDefaultCode { get; set; }
        public string sUpdateHierarchy { get; set; }
        public string sViewHierarchy { get; set; }
        public string sDeleteHierarchy { get; set; }
        public int StatusTypeID { get; set; }
        [Range(1, 10000, ErrorMessage = "Inbox refresh time must be greater than 0")]
        public int InboxRefreshTime { get; set; }
        [NotMapped]
        public string LeftMenu { get; set; }
        [NotMapped]
        public string RightMenu { get; set; }
        public List<VMDropDown> DropDown { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Enter Location")]
        public List<string> Locs { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Enter Hierarchy")]
        public List<string> Hierarchy { get; set; }
        public int FKiTeamID { get; set; }
    }

    public class RegisterWMUserModel
    {

        [Required(ErrorMessage = "Enter Email Address")]
        [EmailAddress]
        [StringLength(256, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        [Display(Name = "Email")]
        [System.Web.Mvc.Remote("IsExistsEmpEmail", "Users", HttpMethod = "POST", AdditionalFields = "Type,Id", ErrorMessage = "Email already exists. Please enter a different Email.")]
        public string Email { get; set; }

        //[Required(ErrorMessage = "Enter Phone Number")]
        //[RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string MobileNumber { get; set; }

        public string UniqueNumber { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must Be 6 Characters long & Combination Of Digits, Upper And Lower Case Letters")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}