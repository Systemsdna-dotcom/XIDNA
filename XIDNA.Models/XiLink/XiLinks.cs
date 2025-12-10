using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XILink_T")]
    public class XiLinks : CommonProperties
    {
        [Key]
        public int XiLinkID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
        public int FKiComponentID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public string sActive { get; set; }
        public string sType { get; set; }
        [ForeignKey("XiLinkID")]
        public virtual List<XiLinkNVs> XiLinkNVs { get; set; }
        public virtual List<XiLinkLists> XiLinkLists { get; set; }
        public Nullable<Guid> FKi1ClickIDXIGUID { get; set; }
    }
}
