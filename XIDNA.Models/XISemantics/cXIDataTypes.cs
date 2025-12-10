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
    [Table("XIDataType_T")]
    public class cXIDataTypes : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please Enter Name")]
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Please Select Base Datatype")]
        public string sBaseDataType { get; set; }
        public int FKiOrganisationID { get; set; }
        public string sStartRange { get; set; }
        public string sEndRange { get; set; }
        [StringLength(256, ErrorMessage = "Maximum length should be 256 only")]
        public string sRegex { get; set; }
        [StringLength(64, ErrorMessage = "Maximum length should be 64 only")]
        public string sScript { get; set; }
        public string sValidationMessage { get; set; }
        public int FKiApplicationID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
    }
}
