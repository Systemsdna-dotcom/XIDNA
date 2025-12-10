using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMDropDown
    {
        public int ID { get; set; }
        public int Value { get; set; }
        public string text { get; set; }
        public string Expression { get; set; }
        public string Type { get; set; }
        public Guid XIGUID { get; set; }
    }
}
