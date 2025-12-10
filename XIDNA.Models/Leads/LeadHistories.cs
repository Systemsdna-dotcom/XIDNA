using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadHistories
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int StageID { get; set; }
        public int LeadID { get; set; }
    }
}
