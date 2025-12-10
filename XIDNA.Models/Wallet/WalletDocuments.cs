using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class WalletDocuments
    {
        [Key]
        public int ID { get; set; }
        public string ClientID { get; set; }
        public int OrganizationID { get; set; }
        public string DocumentName { get; set; }
        public string Message { get; set; }
        public string OriginalName { get; set; }
        public DateTime UploadedOn { get; set; }
        public int StatusTypeID { get; set; }
        public int Type { get; set; }
    }
}
