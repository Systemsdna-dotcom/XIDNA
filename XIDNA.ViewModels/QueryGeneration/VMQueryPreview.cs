using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace XIDNA.ViewModels
{
    public class VMQueryPreview
    {
        public string SectionName { get; set; }
        public int SectionID { get; set; }
        public int? TabID { get; set; }
        public string TabName { get; set; }
        public int Rank { get; set; }
        public int ReportID { get; set; }
        public bool IDExists { get; set; }
        public int Tab1ClickID { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCreate { get; set; }
        public bool IsPopup { get; set; }
        public string Select { get; set; }
        public string Where { get; set; }
        public string GroupBY { get; set; }
        public string OrderBY { get; set; }
        public int QueryID { get; set; }
        public string Query { get; set; }
        public string VisibleQuery { get; set; }
        public int LeadID { get; set; }
        public string IsID { get; set; }
        public string ActionPopUp { get; set; }
        public int UserID { get; set; }
        public string PreviewType { get; set; }
        public string FieldName { get; set; }
        public string Operator { get; set; }
        public string Value { get; set; }
        public string Value2 { get; set; }
        public string DataType { get; set; }
        public string QueryName { get; set; }
        public string QueryIcon { get; set; }
        public int ClassID { get; set; }
        public int BOID { get; set; }
        public List<string> LeadHeadings { get; set; }
        public List<string> LeadVlaues { get; set; }
        public List<string> LeadEditVlaues { get; set; }
        public List<string> SearchFields { get; set; }
        public List<string> LeadLabels { get; set; }
        public List<string> LeadPopupFields { get; set; }
        public List<string> LeadEditableFields { get; set; }
        public List<string> FieldsDatatypes { get; set; }
        public List<string> FieldLengths { get; set; }
        public List<string> Headings { get; set; }
        public List<string> Formats { get; set; }
        public List<string> Scripts { get; set; }
        public List<int> Targets { get; set; }
        public List<string[]> Rows { get; set; }
        public List<int> HeadingReports { get; set; }
        public List<FormData> editpopup { get; set; }
        public VMViewPopup popup { get; set; }
        public ViewRecord ViewRecord { get; set; }
        public List<SingleBOField> SingleBOField { get; set; }
        public List<SectionsData> SectionsData { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public List<VMKPIResult> KpiCircle { get; set; }
        public List<DashBoardGraphs> PieData { get; set; }
        public LineGraph LineGraph { get; set; }
        public LineGraph BarData { get; set; }
        public int ResultListDisplayType { get; set; }
        public string ColumnNames { get; set; }
        public List<string> MouserOverColum { get; set; }
        public List<string> IsMouseOverColumn { get; set; }
        public List<List<string>> MouseOverValues { get; set; }
        public List<string> MouseOverHeadings { get; set; }
        public List<VMDropDown> FKPositions { get; set; }
        public Dictionary<string, string> FilterGroup { get; set; }
        public bool IsFilterSearch { get; set; }
        public bool IsNaturalSearch { get; set; }
        public string SearchText { get; set; }
        public string SearchType { get; set; }
        public string ActionType { get; set; }
        public int ActionReportID { get; set; }
        public bool IsExport { get; set; }
        public int SrchFCount { get; set; }
        public bool IsQueryExists { get; set; }
        public string ShowAs { get; set; }
        public string Message { get; set; }
        public string PopType { get; set; }
        public int StageID { get; set; }
        public string Status { get; set; }
        public Guid XIGUID { get; set; }
    }

    public class SingleBOField
    {
        public int ID { get; set; }
        public int BOID { get; set; }
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string DataType { get; set; }
        public bool IsRunTime { get; set; }
        public bool IsDBValue { get; set; }
        public string DBQuery { get; set; }
        public bool IsExpression { get; set; }
        public string Expression { get; set; }
        public string ExpreValue { get; set; }
        public bool IsDate { get; set; }
        public string DateExpression { get; set; }
        public string DateValue { get; set; }

    }
    public class FormData
    {
        public string Label { get; set; }
        public string Value { get; set; }
    }

    public class ViewRecord
    {
        public string SectionIDs { get; set; }
        public string ViewFields { get; set; }
        public string CreateFields { get; set; }
        public string EditFields { get; set; }
    }

    public class SectionsData
    {
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public List<string> ViewFields { get; set; }
        public List<string> IsViewFK { get; set; }
        public List<int> ViewFKPopuID { get; set; }
        public List<string> CreateFields { get; set; }
        public List<string> EditFields { get; set; }
        public List<string> ViewFieldsData { get; set; }
        public List<string> EditFieldsData { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCreate { get; set; }
        public int? LeadID { get; set; }
        public int? TabID { get; set; }
        public string TabName { get; set; }
        public int? ClassID { get; set; }
        public int Tab1ClickID { get; set; }
        public bool IsBespoke { get; set; }
        public string URL { get; set; }
        public string RefreshType { get; set; }
        public int Rank { get; set; }
        public List<string> EditDataTypes { get; set; }
        public List<string> CreateDataTypes { get; set; }
        public List<string> EditLengths { get; set; }
        public List<string> EditDescs { get; set; }
        public List<string> CreateLengths { get; set; }
        public List<string> CreateDrpts { get; set; }
        public List<string> IsDropDown { get; set; }
        public List<List<VMDropDown>> DropDownValues { get; set; }
        public List<VMDropDown> StatusDDL { get; set; }
        public string PopType { get; set; }
        public int StageID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }

    public class LineGraph
    {
        public List<List<string>> Data { get; set; }
        public int? TabID { get; set; }
        public string SectionName { get; set; }
        public int Rank { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public string Type { get; set; }
        public string QueryName { get; set; }
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public VMViewPopup Popup { get; set; }
        public string PreviewType { get; set; }
        public ViewRecord ViewRecord { get; set; }
        public List<SectionsData> SectionsData { get; set; }
        public int InnerReportID { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public string OnClickParameter { get; set; }
        public int OnClickResultID { get; set; }
        public string OnClickCell { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
