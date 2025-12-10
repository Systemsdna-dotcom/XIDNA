using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.ViewModels
{
    public class VMStages
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
        //public string EmailTemplate { get; set; }
        //public string SMSTemplate { get; set; }
        public int? LeadID { get; set; }
        public int StageID { get; set; }
        public string SubStages { get; set; }
        public List<VMDropDown> Stages { get; set; }
        public List<VMDropDown> StagesList { get; set; }
        public List<string> SStages { get; set; }
        public bool CheckStage { get; set; }
        public string TypeC { get; set; }
        public int OrganizationID { get; set; }
        public int PopupID { get; set; }
    }
}
