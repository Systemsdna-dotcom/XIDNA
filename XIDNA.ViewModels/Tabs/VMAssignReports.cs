using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMAssignReports
    {
        public int ID { get; set; }
        public int TabID { get; set; }
        public List<string> Fields { get; set; }
        public List<VMDropDown> TabSections { get; set; }
        public string ReportType { get; set; }
        public VMQueryPreview QueryPreview { get; set; }
        public int ReportID { get; set; }
        public int ClassID { get; set; }
        public int SectionID { get; set; }
        public string Type { get; set; }
        public string ViewFields { get; set; }
        public string CreateFields { get; set; }
        public string EditFields { get; set; }
        public bool IsView { get; set; }
        public bool IsEdit { get; set; }
        public bool IsCreate { get; set; }
    }
}
