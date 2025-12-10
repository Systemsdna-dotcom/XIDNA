using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMChart
    {
        public int ReportID { get; set; }
        public string Query { get; set; }
        public string DisplayAs { get; set; }
        public int ClassFilter { get; set; }
        public int DateFilter { get; set; }
        public int BOID { get; set; }
        public string ResultIn { get; set; }
        public int OrgID { get; set; }
        public string Database { get; set; }
        public int UserID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
