using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class OrganizationReports
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage="Choose Query")]
        public int ReportID { get; set; }
        public int OrganizationID { get; set; }
        public int UserID { get; set; }
        public int RoleTypeID { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
    }
}
