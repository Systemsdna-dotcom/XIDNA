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
    public class LeadConfigurations
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Select Organization")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Select Class")]
        public int Class { get; set; }
        [Required(ErrorMessage = "Select Settings")]
        public string Settings { get; set; }
        [Required(ErrorMessage = "Enter Interval")]
        public int? Interval { get; set; }
        [NotMapped]
        public string TypeC { get; set; }
        [NotMapped]
        public List<VMDropDown> ClassesList { get; set; }
        [NotMapped]
        public List<VMDropDown> organizations { get; set; }
    }
}
