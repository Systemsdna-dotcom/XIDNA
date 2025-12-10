using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class MasterTemplates
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public int ClassID { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string DataFieldName { get; set; }
        public string FieldLength { get; set; }
    }
}
