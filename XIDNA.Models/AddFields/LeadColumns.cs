using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadColumns
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string TableName { get; set; }
        public string ClassName { get; set; }
    }
}
