using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace XIDNA.ViewModels
{
    public class VMMailExtractStrings
    {

        public int ID { get; set; }
        [Required(ErrorMessage = "Select Subscription")]
        public string SubscriptionID { get; set; }
        public string sSource { get; set; }
        public string sCategory { get; set; }
        [Required(ErrorMessage = "Enter Start String")]
        public string sStartString { get; set; }
        [Required(ErrorMessage = "Enter End String")]
        public string sEndString { get; set; }
        public int StatusTypeID { get; set; }
        public int SourceID { get; set; }
        public List<VMDropDown> SubscriptionList { get; set; }
        public string SubscriptionIDtext { get; set; }
        public int OrganizationID { get; set; }
    }
}
