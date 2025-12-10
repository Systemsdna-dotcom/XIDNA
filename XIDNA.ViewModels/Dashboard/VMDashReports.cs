using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMDashReports
    {
        public List<string> Heads { get; set; }
        //public List<string> Left { get; set; }
        public List<string> Counts { get; set; }
        public List<int> IntCounts { get; set; }
        public string Heading { get; set; }
        public List<string> Headings { get; set; }
        public List<string> HeadNames { get; set; }
        public string Status { get; set; }
        public int TCount { get; set; }
        public string ReportName { get; set; }
        public int ReportID { get; set; }
        public int PopupID { get; set; }
        public int Count { get; set; }
        public int BaseReportID { get; set; }
        public string RowClickType { get; set; }
        public int RowClickValue { get; set; }
        public string ColumnClickType { get; set; }
        public string Column { get; set; }
        public int ColumnClickValue { get; set; }
        public string CellClickType { get; set; }
        public int CellClickValue { get; set; }
        public string RowName { get; set; }
        public string ColumnName { get; set; }
        public string FinaceColumn { get; set; }
    }
}
