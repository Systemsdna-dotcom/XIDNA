using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.Web.Mvc;

namespace XIDNA.Models
{
    [Table("XIUrlMappings_T")]
    public class cXIUrlMappings : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        [Required(ErrorMessage = "Enter URL name")]
        [StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 1)]
        [Remote("IsExistsUrlMappingName", "XIApplications", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Url name already exists. Please enter a different Name.")]
        public string sUrlName { get; set; }
        [Required(ErrorMessage = "Enter Actual Url Name")]
        public string sActualUrl { get; set; }
        public string sType { get; set; }
        public int FKiSourceID { get; set; }
        [NotMapped]
        public List<VMDropDown> SourceList { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        public int OrganisationID { get; set; }
    }
}
