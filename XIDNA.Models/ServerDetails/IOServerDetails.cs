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
    [Table("XIIOServerDetails_T")]
    public class IOServerDetails
    {
        public int ID { get; set; }
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Required(ErrorMessage = "Enter Server Name")]
        public string ServerName { get; set; }
        [Required(ErrorMessage = "Select Organization")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Enter port Number")]
        public int Port { get; set; }
        [Required(ErrorMessage = "Enter Security")]
        public string Security { get; set; }
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Required(ErrorMessage = "Enter UserName")]
        //[RegularExpression(@"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{6,15})$", ErrorMessage = "Enter proper username")]
        [EmailAddress]
        public string UserName { get; set; }
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Required(ErrorMessage = "Enter Passsword")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Enter From Address")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string FromAddress { get; set; }
        [NotMapped]
        public string TypeC { get; set; }
        [Required(ErrorMessage = "Enter SMS Path")]
        [StringLength(512, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string SMSPath { get; set; }
        [Required(ErrorMessage = "Enter Sender ID")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string SenderID { get; set; }
        public string SMSAPIKey { get; set; }
        public int Type { get; set; }
        public int Category { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> organizations { get; set; }
        [NotMapped]
        public string Role { get; set; }
        [NotMapped]
        public string MailID { get; set; }
        [NotMapped]
        public List<VMDropDown> MailIDs { get; set; }
        [NotMapped]
        public string UpdatePassword { get; set; }
    }
}
