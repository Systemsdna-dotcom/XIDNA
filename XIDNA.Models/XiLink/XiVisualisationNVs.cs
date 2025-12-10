using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class XiVisualisationNVs : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int XiVisualID { get; set; }
        public int XiVisualListID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
    }
}
