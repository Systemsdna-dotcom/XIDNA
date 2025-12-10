using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class SubscriptionColumns
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string SubscriptionID { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
