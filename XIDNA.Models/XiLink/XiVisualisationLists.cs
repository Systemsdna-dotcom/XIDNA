using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class XiVisualisationLists : CommonProperties
    {
        [Key]
        public int XiVisualListID { get; set; }
        public int XiVisualID { get; set; }
        public string ListName { get; set; }
        [ForeignKey("XiVisualListID")]
        public virtual List<XiVisualisationNVs> XiVisualisationListNVs { get; set; }
    }
}
