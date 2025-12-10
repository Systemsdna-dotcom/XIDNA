using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XILinkList_T")]
    public class XiLinkLists : CommonProperties
    {
        [Key]
        public int XiLinkListID { get; set; }
        public int XiLinkID { get; set; }
        public string ListName { get; set; }
        [NotMapped]
        [ForeignKey("XiLinkListID")]
        public virtual List<XiLinkNVs> XiLinkListNVs { get; set; }
    }
}
