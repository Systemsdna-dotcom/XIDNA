using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMOneClicks
    {
        public List<object[]> Clicks { get; set; }
        public List<string> ReportTypes { get; set; }
        public List<string> ReportNames { get; set; }
        public string ClickType { get; set; }
        public string PreviewType { get; set; }
        public int XIComponentID { get; set; }
        public List<VMNameValuePairs> nParams { get; set; }  
    }
}
