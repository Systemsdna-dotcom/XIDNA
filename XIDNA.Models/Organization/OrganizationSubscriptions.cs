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
    public class OrganizationSubscriptions
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage="Select Source")]
        public int SourceID { get; set; }
        public string SubscriptionID { get; set; }
        [Required(ErrorMessage = "Select Organization")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Select Class")]
        public int ClassID { get; set; }
        public int StatusTypeID { get; set; }
        public string LocationCode { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers sllowed")]
        public int LeadCost { get; set; }
        [Required(ErrorMessage = "Enter Contact Email")]
        [EmailAddress]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please Enter Correct Email ")]
        [MaxLength(128, ErrorMessage = "Email cannot be longer than 128 characters.")]
        public string Email { get; set; }
        [RegularExpression(@"^[A-Z]{1,2}[0-9R][0-9A-Z]? [0-9][ABD-HJLNP-UW-Z]{2}$", ErrorMessage = "Invalid Post Code. Ex: SW1W 0NY OR L1 8JQ")]
        public string PostCode { get; set; }
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only numbers allowed")]
        public int RenewalDate { get; set; }
    }
}
