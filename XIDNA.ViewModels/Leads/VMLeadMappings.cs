using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadMappings
    {
        public string TempName { get; set; }
        public int BOID { get; set; }
        public int ClassID { get; set; }
        public List<string> NonClassFields { get; set; }
        public List<string> ClassFields { get; set; }
        public List<string> DefaultFields { get; set; }
        public List<string> ExtraFieldTypes { get; set; }
        public List<string> ExistingFields { get; set; }
        public List<string> CommonFields { get; set; }
        public List<string> CommonDataTypes { get; set; }
        public List<VMDropDown> ddlBOs { get; set; }
        public List<VMDropDown> Classes { get; set; }
        public List<VMDropDown> MasterTypes { get; set; }
        public List<string> CreatedBy { get; set; }
        public int OrganizationID { get; set; }
        public List<VMDropDown> Subscriptions { get; set; }
        public string SubscriptionID { get; set; }
        public string MappingType { get; set; }
    }    
}
