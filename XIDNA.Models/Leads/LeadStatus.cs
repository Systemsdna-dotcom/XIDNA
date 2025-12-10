using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadStatus
    {
        [Key]
        public int ID { get; set; }
        public int LeadID { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
    }
}
