using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadPopupLeft
    {
        public VMLeads LeadInfo { get; set; }
        public List<VMStages> LeadStages { get; set; }
        public List<VMLeadActions> LeadActions { get; set; }
        public List<VMLeads> LeadClients { get; set; }
    }
}
