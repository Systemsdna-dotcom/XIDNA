using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace XIDNA.ViewModels
{
    public class VMXiVisualisations
    {
        public int XiVisualID { get; set; }
        [StringLength(32, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [Remote("IsExistsXiVisualisationsName", "XiLink", AdditionalFields = "XiVisualID", HttpMethod = "Post", ErrorMessage = "Name already exists! Choose another name")]
        public string Name { get; set; }
        public string Type { get; set; }
        public string List { get; set; }
        public List<string> Names { get; set; }
        public List<string> Values { get; set; }
        public List<string> ListNames { get; set; }
        public List<string> ListValues { get; set; }
        public string[] NVPairs { get; set; }
        public string[] LNVPairs { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public int FKiApplicationID { get; set; }
        public List<VMXiVisualisationNVs> NVs { get; set; }
        public List<VMXiVisualisationLists> Lists { get; set; }
        public List<VMDropDown> ddlApplications { get; set; }
    }
    public class VMXiVisualisationNVs
    {
        public int XiVisualID { get; set; }
        public int XiVisualListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class VMXiVisualisationLists
    {
        public int XiVisualID { get; set; }
        public int XiVisualListID { get; set; }
        public string ListName { get; set; }
        public List<VMXiVisualisationNVs> NVs { get; set; }
    }
}
