using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadRecord
    {
        public int LeadID { get; set; }
        public int ReportID { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Values { get; set; }
        public List<string> DataTypes { get; set; }
        public List<string> Lengths { get; set; }
    }
}
