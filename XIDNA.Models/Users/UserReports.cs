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
    public class UserReports
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Select report")]
        public int ReportID { get; set; }
        public int FKiApplicationID { get; set; }
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
        //[Required(ErrorMessage = "Select menu name")]
        public int BO { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        [ForeignKey("ReportID")]
        public virtual Reports Report { get; set; }
        [NotMapped]
        public string Query { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        public Guid FKi1ClickIDXIGUID {get; set;}
    }
}
