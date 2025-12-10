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
    public class HTMLColorCodings
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage="Select Column")]
        public int ColumnID { get; set; }
        [Required(ErrorMessage="Select Value")]
        public int Value { get; set; }
        [Required(ErrorMessage="Enter result")]
        public string Result { get; set; }
        public int StatusTypeID { get; set; }
        public int Type { get; set; }
        [NotMapped]
        public List<VMDropDown> Columns { get; set; }
        [NotMapped]
        public List<VMDropDown> Values { get; set; }
        [NotMapped]
        public string Icon { get; set; }
    }
}
