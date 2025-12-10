using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("Aggregations_T")]
    public class cAggregations
    {
        [Key]
        public int ID { get; set; }
        public string sInsurer { get; set; }
        public decimal rPrice { get; set; }
        public bool bLiablityCover { get; set; }
        public bool bLiabilityLimit { get; set; }
        public bool bLossOfMeteredWater { get; set; }
        public bool bLegelExpensesCover { get; set; }
        public int FKiQSInstanceID { get; set; }
        public int FKiCustomerID { get; set; }
        public int FKiUserID { get; set; }
    }
}
