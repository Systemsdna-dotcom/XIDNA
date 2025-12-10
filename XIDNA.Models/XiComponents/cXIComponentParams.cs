using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIComponentParams_T")]
    public class cXIComponentParams
    {
        [Key]
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
        public int iLayoutMappingID { get; set; }
        public int iXiLinkID { get; set; }
        public int iStepDefinitionID { get; set; }
        public int iStepSectionID { get; set; }
        public int iQueryID { get; set; }
    }
}
