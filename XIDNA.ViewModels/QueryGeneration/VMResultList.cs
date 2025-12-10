using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;

namespace XIDNA.ViewModels
{
    public class VMResultList
    {
        public List<string> Headings { get; set; }
        public List<string> TableColumns { get; set; }
        public List<int> HeadingReports { get; set; }
        public List<string[]> Rows { get; set; }
        public bool IDExists { get; set; }
        public int QueryID { get; set; }
        public bool IsPopup { get; set; }
        public string QueryName { get; set; }
        public string QueryIcon { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public int UserID { get; set; }
        public int ClassID { get; set; }
        public ViewRecord ViewRecord { get; set; }
        public string SectionName { get; set; }
        public int? TabID { get; set; }
        public int Rank { get; set; }
        public int ReportID { get; set; }
        public int Tab1ClickID { get; set; }
        public int BOID { get; set; }
        public string PreviewType { get; set; }
        public List<SectionsData> SectionsData { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCreate { get; set; }
        public bool IsDelete { get; set; }
        public int ResultListDisplayType { get; set; }
        public string SearchText { get; set; }
        public Dictionary<string, string> FilterGroup { get; set; }
        public bool IsFilterSearch { get; set; }
        public bool IsNaturalSearch { get; set; }
        public string SearchType { get; set; }
        public string BO { get; set; }
        public string ReportColumns { get; set; }
        public int? BaseReportID { get; set; }
        public string ActionType { get; set; }
        public int ActionReportID { get; set; }
        public bool IsExport { get; set; }
        public List<SingleBOField> SingleBOField { get; set; }
        public int SrchFCount { get; set; }
        public bool IsQueryExists { get; set; }
        public List<string> MouseOverColumns { get; set; }
        public string ShowAs { get; set; }
        public List<VMDropDown> FKPositions { get; set; }
        public string Query { get; set; }
        public List<string> Formats { get; set; }
        public List<string> Scripts { get; set; }
        public List<int> Targets { get; set; }
        public List<VMLeads> AllLeads { get; set; }
        public bool IsRowClick { get; set; }
        public int XiLinkID { get; set; }
        public int LeadID { get; set; }
        public string sGUID { get; set; }
        public string sCreateGroup { get; set; }
        public string sEditGroup { get; set; }
        public List<VMNameValuePairs> nParams { get; set; }
        public int iLayoutID { get; set; }
        [ForeignKey("FKi1ClickID")]
        public virtual List<VMXI1ClickParameter> XIClickParams { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
    public class VMNameValuePairs
    {
        public int ID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sContext { get; set; }
        public string sType { get; set; }
    }
}
