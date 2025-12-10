using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMPopup
    {
        public int LeadID { get; set; }
        public string ContentType { get; set; }
        public List<VMInlineView> InlineData { get; set; }
    }

    public class VMInlineView
    {
        public string Label { get; set; }
        public string Data { get; set; }
        public string Type { get; set; }
        public string IDs { get; set; }
        public List<string> FilePath { get; set; }
        public List<string> FileName { get; set; }
        public string ImgHeight { get; set; }
        public string ImgWidth { get; set; }
        public string ImageCount { get; set; }
        public string IsPreview { get; set; }
        public int IsDrilldown { get; set; }
        public int DrilldownType { get; set; }
        public int ShowButton { get; set; }
        public string DocFilePath { get; set; }
        public List<string> DrillHeight { get; set; }
        public List<string> DrillWidth { get; set; }
    }

    public class VMSaveInlineEdit
    {
        public int iInstanceID { get; set; }
        public List<VMFormData> FormValues { get; set; }
    }

    public class VMFormData
    {
        public string Label { get; set; }
        public string Data { get; set; }
    }
}
