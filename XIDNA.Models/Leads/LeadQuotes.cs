using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadQuotes
    {
        [Key]
        public int ID { get; set; }
        public int QuoteID { get; set; }
        public DateTime PostedOn { get; set; }
        public int LeadID { get; set; }
        public int OrganizationID { get; set; }
    }
}
