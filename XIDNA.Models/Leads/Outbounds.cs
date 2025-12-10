using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class Outbounds
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int SourceID { get; set; }
        public int LeadID { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public int Type { get; set; }
        public int TemplateID { get; set; }
        public string Attachment { get; set; }
        public string Cc { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public int Users { get; set; }
        public int FileID { get; set; }
        [NotMapped]
        public string csvID { get; set; }
    }
}
