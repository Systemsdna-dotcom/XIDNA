using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    //Step Definition
    [Table("XISemanticsSteps_XSS_T")]
    public class cXISemanticsSteps : CommonProperties
    {
        [Key]
        public int XIStepID { get; set; }
        public int FKiXISemanticID { get; set; }
        public string sName { get; set; }
        public int iOrder { get; set; }
        public string sCode { get; set; }
        public int iDisplayAs { get; set; }
        public int XILinkID { get; set; }
        [Required(ErrorMessage = " Please select")]
        public int FKiContentID { get; set; }
        //[ForeignKey("FKiXIStepID")]
        //public virtual ICollection<cXISemanticsNavigations> XISemanticsNavigations { get; set; }
        //[ForeignKey("FKiXIFieldDefinitionID")]
        //public virtual ICollection<cFieldDefinition> XIFieldInstance { get; set; }
    }
}
