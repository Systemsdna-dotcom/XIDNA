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
    public class Tabs
    {
        public Tabs()
        {
            this.Sections = new HashSet<Sections>();
        }
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage="Enter Tab Name")]
        [StringLength(30, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [System.Web.Mvc.Remote("IsExistsTabName", "Popup", AdditionalFields = "ID, PopupID", HttpMethod = "POST", ErrorMessage = "Tab Name already exists.")]
        public string Name { get; set; }
        [Required(ErrorMessage="Select Popup")]
        public int PopupID { get; set; }
        public int Rank { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedBySYSID { get; set; }
        public DateTime ModifiedTime { get; set; }        
        [NotMapped]
        public string TypeC { get; set; }
        [NotMapped]
        public List<VMDropDown> Ranks { get; set; }
        [NotMapped]
        public List<VMDropDown> SecRanks { get; set; }
        [NotMapped]
        public Sections Section { get; set; }
        [ForeignKey("TabID")]
        public virtual ICollection<Sections> Sections { get; set; }
        [NotMapped]
        public string CreationType { get; set; }
        [NotMapped]
        public List<VMDropDown> PopupList { get; set; }        
    }
    public class Status
    {
        public int Value { get; set; }
        public string Text { get; set; }
    }
    
}
