using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class XiParameterLists : CommonProperties
    {
        [Key]
        public int XiParameterListID { get; set; }
        public int XiParameterID { get; set; }
        public string ListName { get; set; }
        [ForeignKey("XiParameterListID")]
        public virtual List<XiParameterNVs> XiParameterListNVs { get; set; }
    }
}
