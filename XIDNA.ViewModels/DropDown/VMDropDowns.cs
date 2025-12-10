using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMDropdowns
    {
        [Key]
        public int ID { get; set; }
        public string Value { get; set; }
    }
}
