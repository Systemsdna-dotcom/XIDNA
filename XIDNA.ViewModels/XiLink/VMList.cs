using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMList
    {
        public List<string> Headings { get; set; }
        public string XIGUID { get; set; }
        public string Value { get; set; }
        public string OneClickID { get; set; }
    }
}
