using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMRequests
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string EmailID { get; set; }
        public int FKiLeadClassID { get; set; }
        public bool IsActivated { get; set; }
        public string Status { get; set; }
        public string ClientID { get; set; }
        public string OrgnizationName { get; set; }
    }
}
