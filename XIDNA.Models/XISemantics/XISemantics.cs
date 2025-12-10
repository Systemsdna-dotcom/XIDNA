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
    //Question Set Definition
    [Table("XISemantics_XS_T")]
    public class cXISemantics:CommonProperties
    {
        [Key]
        public int XISemanticID { get; set; }
        public string sName { get; set; }
        public string  sDescription{get;set;}
        [ForeignKey("FKiXISemanticID")]
        public virtual ICollection<cXISemanticsSteps> XISemanticsSteps { get; set; }
    }
}
