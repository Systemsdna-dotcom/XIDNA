using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class XiParameters : CommonProperties
    {
        [Key]
        public int XiParameterID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        [ForeignKey("XiParameterID")]
        public virtual List<XiParameterNVs> XiParameterNVs { get; set; }
        public virtual List<XiParameterLists> XiParameterLists { get; set; }
    }
}
