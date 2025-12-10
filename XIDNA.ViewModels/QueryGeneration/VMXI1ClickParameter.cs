using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    [Table("XI1ClickParameter_T")]
    public class VMXI1ClickParameter
    {
        public int ID { get; set; }
        public int FKi1ClickID { get; set; }
        public string sName { get; set; }
        public string sDefault { get; set; }
        public string sValue { get; set; }
    }
}
