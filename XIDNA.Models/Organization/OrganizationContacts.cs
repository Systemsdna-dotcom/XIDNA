using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class OrganizationContacts
    {
        [Key]
       public int ID {get;set;}
       public int OrganizationID {get;set;}
       [Required(ErrorMessage = "Enter Contact Name")]
       [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
       public string Name{get;set;}
       [Required(ErrorMessage = "Enter Contact Number")]
       [RegularExpression(@"^(\+44\s?7\d{3}|\(?07\d{3}\)?)\s?\d{3}\s?\d{3}$", ErrorMessage = "Ex: 07222 555555 OR (07222) 555555 OR +44 7222 555 555")]
        public string Phone{get;set;}
       [Required(ErrorMessage = "Enter Contact Email")]
       [EmailAddress]
       [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email ")]
       [MaxLength(128, ErrorMessage = "Email cannot be longer than 128 characters.")]
        public string Email{get;set;}
       [Required(ErrorMessage = "Enter Contact Address")]
       [StringLength(128, ErrorMessage = "The {0} must be below {1} characters.")]
        public string Address{get;set;}
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public String CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}


