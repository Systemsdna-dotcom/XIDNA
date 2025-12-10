using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMUserReports
    {
        public string QueryType { get; set; }
        public List<string> QueryNames { get; set; }
        public List<string> Visibility { get; set; }
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        [Required(ErrorMessage = "Select report")]
        public int ReportID { get; set; }
        [Required(ErrorMessage = "Select type")]
        public int TypeID { get; set; }
        [Required(ErrorMessage = "Select class")]
        public int ClassID { get; set; }
        public int OrganizationID { get; set; }
        public int RoleID { get; set; }
        public int Rank { get; set; }
        //[Required(ErrorMessage = "Select Icon")]
        public string Icon { get; set; }
        [Required(ErrorMessage = "Enter target")]
        public int Target { get; set; }
        public string TargetResultType { get; set; }
        [Required(ErrorMessage = "Select Template")]
        public int TargetTemplateID { get; set; }
        [Required(ErrorMessage = "Select location")]
        public int Location { get; set; }
        [Required(ErrorMessage = "Select display")]
        public int DisplayAs { get; set; }
        public int BO { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public string QueryName { get; set; }
        public int Category { get; set; }
        public List<VMDropDown> Classes { get; set; }
        public string UserName { get; set; }
        public List<VMDropDown> TemplateList { get; set; }
        public List<VMDropDown> ReportTypes { get; set; }
        public string MenuName { get; set; }
        public string ClassName { get; set; }
        public List<VMDropDown> SelectedReports { get; set; }
        public List<VMDropDown> Locations { get; set; }
        public List<VMDropDown> QuickSearchBOs { get; set; }
        public string Query { get; set; }
        public List<VMDropDown> ddlApplications { get; set; }
        [Required(ErrorMessage = "Select Business Object")]
        public int BOID { get; set; }
        public List<VMDropDown> AllBOs { get; set; }
        public int XILinkID { get; set; }
        public List<VMDropDown> AllXiLinks { get; set; }
        public List<VMDropDown> AllOneClicks { get; set; }
        public bool bSignalR { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
