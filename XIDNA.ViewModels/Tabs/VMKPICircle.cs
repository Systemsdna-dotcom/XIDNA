using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMKPICircle
    {
        public List<VMKPIResult> KpiCircle { get; set; }
        public string PreviewType { get; set; }
        public ViewRecord ViewRecord { get; set; }
        public int Rank { get; set; }
        public int ReportID { get; set; }
        public List<SectionsData> SectionsData { get; set; }
    }
}
