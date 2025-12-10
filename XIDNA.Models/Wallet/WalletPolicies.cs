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
    public class WalletPolicies
    {
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter name")]
        [System.Web.Mvc.Remote("IsExistsWalletPolicyName", "Wallet", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "policy name already exists")]
        public string PolicyName { get; set; }
        public int OrganizationID { get; set; }
        public int LeadID { get; set; }
        public string BrokerName { get; set; }
        public string ProductName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; }
        public int Type { get; set; }
        [Required(ErrorMessage = "Enter gross premium")]
        public decimal? GrossPremium { get; set; }
        [Required(ErrorMessage = "Enter commission")]
        public decimal? Commission { get; set; }
        [Required(ErrorMessage = "Enter charges")]
        public decimal? Charges { get; set; }
        [Required(ErrorMessage = "Enter add-on-commission")]
        public decimal? AddOnCommission { get; set; }
        [Required(ErrorMessage = "Enter add-on-charges")]
        public decimal? AddOnCharges { get; set; }
        [Required(ErrorMessage = "Enter policy-setup-charges")]
        public decimal? PolicySetupCharges { get; set; }
        public DateTime Date { get; set; }
        //[Required(ErrorMessage = "Enter notes")]
        public string Notes { get; set; }
        public string Document { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Select product")]
        public int ProductID { get; set; }
        [Required(ErrorMessage = "Select product type")]
        public int ProductType { get; set; }
        [Required(ErrorMessage = "Select template")]
        public int TemplateID { get; set; }
        public bool IsPosted { get; set; }
        [NotMapped]
        public string OrgName { get; set; }
        [NotMapped]
        public List<VMDropDown> ProductList { get; set; }
        [NotMapped]
        public List<VMDropDown> Templates { get; set; }

    }
}
