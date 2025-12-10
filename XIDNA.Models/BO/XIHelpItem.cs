using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.DynamicData;

namespace XIDNA.Models
{
    [Table("XIHelpItem_XHI_T")]
    public class XIHelpItem
    {
        [Key]
        public int ID { get; set; }
        public string Description { get; set; }
        public string Default { get; set; }
    }
}
