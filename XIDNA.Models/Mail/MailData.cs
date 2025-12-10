using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class MailData
    {
        public int ID { get; set; }
        public int UID { get; set; }
        public DateTime dEmailDate { get; set; }
        public string sEmailFrom { get; set; }
        public string sEmailSubject { get; set; }
        public string sEmailText { get; set; }
        public string sAttachmentPath { get; set; }
        public string sAttachmentName { get; set; }
    }
}
