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
    public class MailExtractStrings
    {
        [Key]
        public int ID { get; set; }
        public string SubscriptionID { get; set; }
        public string sSource { get; set; }
        public string sCategory { get; set; }
        public string sStartString { get; set; }
        public string sEndString { get; set; }
        public int SourceID { get; set; }
        public int StatusTypeID { get; set; }
        public int OrganizationID { get; set; }
    }
}
