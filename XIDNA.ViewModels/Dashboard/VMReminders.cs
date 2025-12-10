using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMReminders
    {
        public int Count { get; set; }
        public List<int> LeadIDs { get; set; }
        public List<string> LeadDetails { get; set; }
        public int ReportID { get; set; }
        public int InnerReportID { get; set; }
    }
}
