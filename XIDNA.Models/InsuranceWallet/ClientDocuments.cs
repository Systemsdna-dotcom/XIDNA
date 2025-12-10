using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class ClientDocuments
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public int OrganizationID { get; set; }
        public string DocumentName { get; set; }
        public string Message { get; set; }
        public string OriginalName { get; set; }
        public DateTime UploadedOn { get; set; }
        public int StatusTypeID { get; set; }
    }
}
