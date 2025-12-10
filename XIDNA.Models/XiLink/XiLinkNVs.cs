using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace XIDNA.Models
{
    [Table("XILinkNV_T")]
    public class XiLinkNVs : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int XiLinkID { get; set; }
        public int XiLinkListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
