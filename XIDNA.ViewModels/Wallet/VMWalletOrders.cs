using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
  public  class VMWalletOrders
    {
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
        public string OrganizationName { get; set; }
        public string ProductName { get; set; }
    }
}
