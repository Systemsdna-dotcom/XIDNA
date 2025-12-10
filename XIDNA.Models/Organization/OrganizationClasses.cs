using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class OrganizationClasses
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ClassID { get; set; }
        public string Class { get; set; }
    }
}
