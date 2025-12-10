using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadActions
    {
        public int ID { get; set; }
        public int LeadID { get; set; }
        public int Type { get; set; }
        public string Name { get; set; }
        public int TemplateID { get; set; }
        public int PopupID { get; set; }
        public int StageID { get; set; }
        public string PopType { get; set; }
        public List<VMDropDown> Tabs { get; set; }
        public int ClientID { get; set; }
    }
}
