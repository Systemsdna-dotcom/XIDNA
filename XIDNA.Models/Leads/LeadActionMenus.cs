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
    public class LeadActionMenus
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsActionMenuName", "Lead", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Name already exists.")]
        public string Name { get; set; }
        [Required(ErrorMessage="Select Type")]
        public int Type { get; set; }
        [Required(ErrorMessage = "Select Action Type")]
        public int ActionType { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> ActionTypes { get; set; }
    }
}
