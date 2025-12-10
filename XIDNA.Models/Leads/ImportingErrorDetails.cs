using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class ImportingErrorDetails
    {
        [Key]
        public int ID { get; set; }
        public int SourceID { get; set; }
        public int FileID { get; set; }
        public int InboundID { get; set; }
        public int OrganizationID { get; set; }
        public string SubscriptionID { get; set; }
        public string TypeOfData { get; set; }
        public DateTime LoggedOn { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
        [NotMapped]
        public string FileName { get; set; }
    }
}
