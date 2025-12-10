using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;


namespace XIDNA.ViewModels
{
    public class VMMessages
    {
        public int ID { get; set; }
        public int Sender { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string ClientID { get; set; }
        public int OrganizationID { get; set; }
        public int Type { get; set; }
        public int MailType { get; set; }
        public string OrganizationName { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        //public int ColorCode { get; set; }
        public string Icon { get; set; }
        public int Importance { get; set; }
        public int ProductID { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string Attachments { get; set; }
        public virtual VMMessages MasterMail { get; set; }
        public virtual ICollection<VMMessages> SubMails { get; set; }
        [NotMapped]
        public int PageSize { get; set; }
        [NotMapped]
        public int PageNumber { get; set; }
        [NotMapped]
        public int Total { get; set; }
        [NotMapped]
        public int MailNumber { get; set; }

    }
}
