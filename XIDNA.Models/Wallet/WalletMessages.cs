using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.Models
{

    public class WalletMessages
    {
        //public WalletMessages()
        //{
        //    this.SubMails = new HashSet<WalletMessages>();
        //}
        [Key]
        public int ID { get; set; }
        public int Sender { get; set; }
        public Nullable<int> ParentID { get; set; }
        public string ClientID { get; set; }
        public string EmailID { get; set; }
        public int OrganizationID { get; set; }
        public string MailType { get; set; }
        public string OrganizationName { get; set; }
        public string Subject { get; set; }
        [Required(ErrorMessage = "Enter Message")]
        public string Message { get; set; }
        public bool IsRead { get; set; }
        //public int ColorCode { get; set; }
        public string Icon { get; set; }
        public int Importance { get; set; }
        public int ProductID { get; set; }
        public DateTime ReceivedOn { get; set; }
        public string Attachments { get; set; }
        public string Type { get; set; }
        public string MailFrom { get; set; }
        [ForeignKey("ParentID")]
        public virtual WalletMessages MasterMail { get; set; }
        //public virtual ICollection<WalletMessages> SubMails { get; set; }
        //public bool IsLeaf
        //{
        //    get
        //    {
        //        return this.SubMails.Count == 0;
        //    }
        //}
        [NotMapped]
        public string MailTime { get; set; }
        [NotMapped]
        public List<VMDropDown> OrgsList { get; set; }
        [NotMapped]
        public int Unread { get; set; }
        [NotMapped]
        public string ClientName { get; set; }
    }    
}
