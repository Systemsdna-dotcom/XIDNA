using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace XIDNA.Models
{
    public class Calls
    {
        [Key]
        public int ID { get; set; }
        public int FKiLeadID { get; set; }
        public int FKiOrganizationID { get; set; }
        public string Description { get; set; }
    }
}
