using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class BObjects
    {
        public string name { get; set; }
        public string version { get; set; }
        public string label { get; set; }
        public string description { get; set; }
        public string size { get; set; }
        public string table { get; set; }
        public string sequence { get; set; }
        public string classname { get; set; }
        public string primarykey { get; set; }
        public string auditable { get; set; }
        public string deleterule { get; set; }
        public string security { get; set; }
    }
}
