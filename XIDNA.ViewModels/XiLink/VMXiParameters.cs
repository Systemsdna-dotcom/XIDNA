using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMXiParameters
    {
        public int XiParameterID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int OneClickID { get; set; }
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
        public List<VMXiParameterNVs> NVs { get; set; }
        public List<VMXiParameterLists> Lists { get; set; }
        public List<VMDropDown> ddlApplications { get; set; }
    }

    public class VMXiParameterNVs
    {
        public int XiParameterID { get; set; }
        public int XiParameterListID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
    }

    public class VMXiParameterLists
    {
        public int XiParameterID { get; set; }
        public int XiParameterListID { get; set; }
        public string ListName { get; set; }
        public List<VMXiParameterNVs> NVs { get; set; }
    }
}
