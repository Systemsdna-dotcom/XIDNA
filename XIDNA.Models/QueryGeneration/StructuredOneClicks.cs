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
    public class StructuredOneClicks
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ReportID { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage="Enter Condition")]
        public string WhereCondition { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> Parent1Clicks { get; set; }
    }
}
