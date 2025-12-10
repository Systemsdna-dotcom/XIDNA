using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadClients
    {
        [Key]
        public int ID { get; set; }
        public int InBoundID { get; set; }
        public string OrgHeirarchyID { get; set; }
        public string Name { get; set; }
        public int ClassID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int OrganizationID { get; set; }
    }
}
