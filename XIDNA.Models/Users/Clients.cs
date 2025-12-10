using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class Clients
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ClientID { get; set; }
        public bool IsActivated { get; set; }
    }
}
