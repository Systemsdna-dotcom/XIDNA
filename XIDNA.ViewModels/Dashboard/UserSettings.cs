using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class UserSettings
    {
        public string Col0 { get; set; }
        public string Row1 { get; set; }
        public string Row2 { get; set; }
        public string Row3 { get; set; }
        public string Row4 { get; set; }
        public string Row5 { get; set; }
        public List<string> ReportIDs { get; set; }
        public List<string> DisplayType { get; set; }
        public List<string> ReportNames { get; set; }
        public List<string> Visibility { get; set; }
    }
}
