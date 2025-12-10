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
    public class Sections
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter Section Name")]
        [StringLength(30, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [System.Web.Mvc.Remote("IsExistsSectionName", "Popup", AdditionalFields = "ID, TabID", HttpMethod = "POST", ErrorMessage = "Section Name already exists.")]
        public string Name { get; set; }
        public int TabID { get; set; }
        public int Rank { get; set; }
        public bool IsBespoke { get; set; }
        public string RefreshType { get; set; }
        [Required(ErrorMessage="Enter URL")]
        public string URL { get; set; }
        [NotMapped]
        public bool IsView { get; set; }
        [NotMapped]
        public bool IsEdit { get; set; }
        [NotMapped]
        public bool IsCreate { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public string[] SectionIDs { get; set; }
        [NotMapped]
        public string[] Fields { get; set; }
        [NotMapped]
        public string[] ViFields { get; set; }
        [NotMapped]
        public string[] EdFields { get; set; }
        [NotMapped]
        public string[] CrFields { get; set; }

        [NotMapped]
        [Required(ErrorMessage="Select Class")]
        public int ClassID { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Select Display Type")]
        public int DisplayAs { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "Select Report")]
        public int ReportID { get; set; }
        [NotMapped]
        public int SecNames { get; set; }
        [NotMapped]
        public List<VMDropDown> TabSections { get; set; }
        [NotMapped]
        public List<VMDropDown> SectionRanks { get; set; }
        [NotMapped]
        public List<VMDropDown> Classes { get; set; }
        [NotMapped]
        public List<VMDropDown> ReportTypes { get; set; }
        [NotMapped]
        public List<Classes> Reports { get; set; }
        [NotMapped]
        public string ViewFields { get; set; }
        [NotMapped]
        public string EditFields { get; set; }
        [NotMapped]
        public string CreateFields { get; set; }
        [NotMapped]
        public string ReportType { get; set; }
        [NotMapped]
        public VMAssignReports TabPreview { get; set; }
    }
}
