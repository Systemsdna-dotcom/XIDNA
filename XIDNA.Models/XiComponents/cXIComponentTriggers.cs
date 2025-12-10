using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIComponentTriggers_XCT_T")]
    public class cXIComponentTriggers
    {
        [Key]
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
    }
}
