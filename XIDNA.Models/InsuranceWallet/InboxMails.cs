using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class InboxMails
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public int OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public int ColorCode { get; set; }
        public string Icon { get; set; }
        public int Importance { get; set; }
        public DateTime ReceivedOn { get; set; }
    }
}
