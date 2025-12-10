using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMQuickSearch
    {
        public int ReportID { get; set; }
        public int PageIndex { get; set; }
        public int OrgID { get; set; }
        public int UserID { get; set; }
        public string database { get; set; }
        public string SearchText { get; set; }
        public List<string> Headings { get; set; }
        public string SearchType { get; set; }
        public string ReportColumns { get; set; }
        public int? BaseID { get; set; }
        public string Role { get; set; }
        public int RoleID { get; set; }
        public int TabID { get; set; }
        public int LeadID { get; set; }
        public string ShowType { get; set; }
        public string BO { get; set; }
        public string sGUID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
