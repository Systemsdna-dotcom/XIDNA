using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class WalletRequests
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string EmailID { get; set; }
        public int FKiLeadClassID { get; set; }
        public bool IsActivated { get; set; }
        public string Status { get; set; }
        public string ClientID { get; set; }
        [NotMapped]
        public string OrgnizationName { get; set; }
        [NotMapped]
        public int LeadID { get; set; }
    }
}
