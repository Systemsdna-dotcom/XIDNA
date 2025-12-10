using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class WalletOrders
    {
        [Key]
        public int ID { get; set; }
        public int ProductID { get; set; }
        public string ClientID { get; set; }
        public DateTime OrderedOn { get; set; }
        public int StatusTypeID { get; set; }
        public int OrganizationID { get; set; }
        public string EmailID { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int Type { get; set; }
        [NotMapped]
        public string ClientEmailID { get; set; }
        [NotMapped]
        public int DaysRemaining { get; set; }
        [NotMapped]
        public string ProductName { get; set; }
        [NotMapped]
        public string OrganizationName { get; set; }

    }
}
