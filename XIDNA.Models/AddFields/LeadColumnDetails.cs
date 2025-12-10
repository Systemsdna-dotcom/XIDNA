using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class LeadColumnDetails
    {
        [Key]
        public int ID { get; set; }
        public int LeadColumnID { get; set; }
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public int Length { get; set; }
        public int StatusTypeID { get; set; }
        public int FieldTypeID { get; set; }
        [ForeignKey("LeadColumnID")]
        public virtual LeadColumns LeadColumns { get; set; }
    }
}
