using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMUserDashContents
    {
        public int UserID { get; set; }
        public List<DashboardContent> KPICircle { get; set; }
        public List<DashboardContent> KPIBarChart { get; set; }
        public List<DashboardContent> KPIPieChart { get; set; }
        public List<DashboardContent> ResultList { get; set; }
        public List<DashboardContent> KPICount { get; set; }
        public UserSettings UserSettings { get; set; }
        public GraphData User { get; set; }
        public DashboardContent dashboardcontent { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public int DDLClassValue { get; set; }
        public int DDLDateValue { get; set; }
    }

    public class DashboardContent
    {
        public int ReportID { get; set; }
        public string Type { get; set; }
        public int? ClassID { get; set; }
        public int? dImportedOn { get; set; }
        public string ReportName { get; set; }
    }
    public class GraphData
    {
        public string ID { get; set; }
        public int UserID { get; set; }
        public string Type { get; set; }
        public int? TabID { get; set; }
        public int? DDLClassValue { get; set; }
        public int ReportID { get; set; }
        public string Query { get; set; }
        public string DisplayAs { get; set; }
        public string SectionName { get; set; }
        public int? DDLDateValue { get; set; }
        public List<DashboardContent> KPICount { get; set; }
        public int ClassFilter { get; set; }
        public int DateFilter { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public List<DashBoardGraphs> PieData { get; set; }
        public LineGraph BarData { get; set; }
        public string QueryName { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public int OnClickResultID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
