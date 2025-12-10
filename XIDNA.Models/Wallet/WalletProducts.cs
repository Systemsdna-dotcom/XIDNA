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
    public class WalletProducts
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Required(ErrorMessage="Enter Name")]
        [System.Web.Mvc.Remote("IsExistsWalletProductName", "Wallet", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "product name exists")]
        public string Name { get; set; }
        public int ClassID { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public string Document { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required(ErrorMessage = "Select product type")]
        public int Type { get; set; }
        public int StatusTypeID { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int TemplateID { get; set; }
        [NotMapped]
        public string OrgName { get; set; }
        [NotMapped]
        public List<VMDropDown> Classes { get; set; }
        [NotMapped]
        public List<VMDropDown> Templates { get; set; }
    }
}
