using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.ComponentModel;
namespace XIDNA.Models
{
    public class XIFileTypes : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string sCount { get; set; }
        public int? MaxCount { get; set; }
        public string Type { get; set; }
        public int? MaxWidth { get; set; }
        public int? MaxHeight { get; set; }
        public string FileType { get; set; }
        public string Thumbnails { get; set; }
        public int ThumbWidth { get; set; }
        public int ThumbHeight { get; set; }
        public string Preview { get; set; }
        [DefaultValue(200)]
        public int PreviewWidth { get; set; }
        [DefaultValue(400)]
        public int PreviewHeight { get; set; }
        public string Drilldown { get; set; }
        public int DrillWidth { get; set; }
        public int DrillHeight { get; set; }
        public string DrillDownType { get; set; }
        [NotMapped]
        public List<VMDropDown> FileTypes { get; set; }
    }
    public class FileTypes
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
