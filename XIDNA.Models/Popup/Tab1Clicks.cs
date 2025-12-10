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
    public class Tab1Clicks
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Select Tab")]
        public int TabID { get; set; }
        public string SectionID { get; set; }
        public string ViewFields { get; set; }
        public string CreateFields { get; set; }
        public string EditFields { get; set; }
        [Required(ErrorMessage = "Select Report")]
        public int ReportID { get; set; }
        [Required(ErrorMessage = "Select Class")]
        public int ClassID { get; set; }
        [Required(ErrorMessage = "Select Display Type")]
        public int DisplayAs { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCreate { get; set; }
        public int OrganizationID { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedBySYSID { get; set; }
        public DateTime ModifiedTime { get; set; }
        public bool IsBespoke { get; set; }
        [Required(ErrorMessage = "Enter URL")]
        public string URL { get; set; }
        public string RefreshType { get; set; }
        [NotMapped]
        public int TabValue { get; set; }
        [NotMapped]
        public int Category { get; set; }
        [NotMapped]
        public string BespokeDisplayAs { get; set; }
        [NotMapped]
        public List<VMDropDown> TabSections { get; set; }
        [NotMapped]
        public List<VMDropDown> Tabs { get; set; }
        [NotMapped]
        public List<VMDropDown> Classes { get; set; }
        [NotMapped]
        public string ClassName { get; set; }
        [NotMapped]
        public int UserID { get; set; }
        [NotMapped]
        public int OrgID { get; set; }
        [NotMapped]
        public Sections Section { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
