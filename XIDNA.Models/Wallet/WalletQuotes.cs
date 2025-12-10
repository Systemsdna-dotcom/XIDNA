using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
   public class WalletQuotes
    {
       [Key]
       public int ID { get; set; }
       public int LeadID { get; set; }
       public int OrganizationID { get; set; }
       [Required(ErrorMessage = "Enter Name")]
       [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
       [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
       [System.Web.Mvc.Remote("IsExistsWalletQuoteName", "Wallet", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Quote name exists")]
       public string Name { get; set; }
       public int Product { get; set; }
       public DateTime QuoteValidTo { get; set; }
       public decimal GrossPremium { get; set; }
       public decimal Commission { get; set; }
       public decimal AdminCharges { get; set; }
       public decimal AddOnCommission { get; set; }
       public decimal AddOnCharges { get; set; }
       public decimal PolicySetupCharges { get; set; }
       public decimal Income { get; set; }
       public int FinancialQuote { get; set; }
       public string Notes { get; set; }
       public string SentBy { get; set; }
       public DateTime SentOn { get; set; }
       public string Document { get; set; }
       public string Image { get; set; }
       public int Type { get; set; }
       public int ClassID { get; set; }
       public string Content { get; set; }
       public string Status { get; set; }
       public int TemplateID { get; set; }
       public bool IsPosted { get; set; }
       [NotMapped]
       public string OrgName { get; set; }
       [NotMapped]
       public string ClassName { get; set; }
       [NotMapped]
       public List<VMDropDown> ProductList { get; set; }
       [NotMapped]
       public List<VMDropDown> Classes { get; set; }
    }
}

