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
   public class Stages
    {
      
       [Key]
       public int ID { get; set; }
       [Required(ErrorMessage = "Enter Stage Name")]
       [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
       [StringLength(50, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
       [System.Web.Mvc.Remote("IsExistsStageName", "Popup", AdditionalFields = "ID, Name", HttpMethod = "POST", ErrorMessage = "Name already exists.")]
       public string Name { get; set; }
       public int StatusTypeID { get; set; }
       public bool IsSMS { get; set; }
       public bool IsEmail { get; set; }
       public bool IsSQLJob { get; set; }
       public bool IsReminders { get; set; }
       public bool IsDashboardRefresh { get; set; }
       public bool IsAlerts { get; set; }
       public string EmailTemplate { get; set; }
       public string SMSTemplate { get; set; }
       public bool IsPopup { get; set; }
       public int PopupID { get; set; }
       public int OrganizationID { get; set; }
       [NotMapped]
       public int LeadID { get; set; }
       [NotMapped]
       public int StageID { get; set; }
       [NotMapped]
       public string Type { get; set; }
       [NotMapped]
       public List<VMDropDown> PopupList { get; set; }
     }
}
