using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMMappedFields
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ClassID { get; set; }
        public string Class { get; set; }
        public string SubscriptionID { get; set; }
        public string Description { get; set; }
        public string FieldName { get; set; }
        public string AddField { get; set; }
        public bool IsDropDown { get; set; }
        public int MasterID { get; set; }
        public string FieldType { get; set; }
        public string Length { get; set; }
        public string CreationType { get; set; }
        public string Type { get; set; }
        public List<VMDropDown> Subscriptions { get; set; }
        public string ClassSpecific { get; set; }
        public int BOID { get; set; }
    }
}
