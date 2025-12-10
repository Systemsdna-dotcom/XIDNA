using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMXIComponents
    {
        public int ID { get; set; }
        public string sType { get; set; }
        public string sName { get; set; }
        public string sClass { get; set; }
        public string sHTMLPage { get; set; }
        public int StatusTypeID { get; set; }
        public List<string> Names { get; set; }
        public List<string> Values { get; set; }
        public List<string> ListNames { get; set; }
        public List<string> ListValues { get; set; }
        public string[] NVPairs { get; set; }
        public string[] TriggerPairs { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public List<VMXIComponentsNVs> NVs { get; set; }
        public List<VMXIComponentTriggers> Triggers { get; set; }
        public int FKiApplicationID { get; set; }
    }

    public class VMXIComponentsNVs
    {
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string sType { get; set; }
        public string ExposedFilter { get; set; }
        public int StatusTypeID { get; set; }
    }

    public class VMXIComponentTriggers
    {
        public int ID { get; set; }
        public int FKiComponentID { get; set; }
        public string sName { get; set; }
        public string sValue { get; set; }
    }
}
