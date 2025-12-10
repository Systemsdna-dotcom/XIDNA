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
    [Table("XIComponents_XC_T")]
    public class cXIComponents : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public string sName { get; set; }
        public string sType { get; set; }
        public string sClass { get; set; }
        public string sHTMLPage { get; set; }
        public int FKiApplicationID { get; set; }
        [ForeignKey("FKiComponentID")]
        public virtual List<cXIComponentsNVs> XIComponentNVs { get; set; }
        [ForeignKey("FKiComponentID")]
        public virtual List<cXIComponentTriggers> XIComponentTriggers { get; set; }
        [ForeignKey("FKiComponentID")]
        public virtual List<cXIComponentParams> XIComponentParams { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        public int OrganisationID { get; set; }
    }
}
