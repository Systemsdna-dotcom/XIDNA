using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("PaymentGateWay_T")]
    public class PaymentGateWay
    {
        [Key]
        public int ID { get; set; }
        public string sName { get; set; }
        public int OrganizationID { get; set; }
        public string sOrganizationName { get; set; }
        public int ApplicationID { get; set; }
        public string sApplicationName { get; set; }
        public string sMerchantID { get; set; }
        public string sSecret { get; set; }
        public string ResponseUrl { get; set; }
        public string ReturnUrl { get; set; }
        public string Mode { get; set; }
        public int StatusTypeID { get; set; }
    }

    public class Performance
    {
        public double Ram { get; set; }
        public double Cpu { get; set; }
    }
}
