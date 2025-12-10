using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class OrganizationSetup
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Columns { get; set; }
        public int StatusTypeID { get; set; }
    }
}
