using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMRunUserQuery
    {
        public int ReportID { get; set; }
        public int PageIndex { get; set; }
        public int UserID { get; set; }
        public string database { get; set; }
        public int OrgID { get; set; }
        public int LeadID { get; set; }
        public int ClassFilter { get; set; }
        public int DateFilter { get; set; }
        public string SearchText { get; set; }
        public string SearchType { get; set; }
        public string BO { get; set; }
        public string Fields { get; set; }
        public string Optrs { get; set; }
        public string Values { get; set; }
        public string Type { get; set; }
        public int ResultListDisplayType { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
