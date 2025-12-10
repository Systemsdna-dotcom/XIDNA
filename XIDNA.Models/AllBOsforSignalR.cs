using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class AllBOsforSignalR
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string sName { get; set; }
        public int Count { get; set; }
        public double rPrice { get; set; }
        public int iQuoteStatus { get; set; }
        public int FKiProductVersionID { get; set; }
        public int iStatus { get; set; }
        public string sGroupQuote { get; set; }
        public int FKiBoid { get; set; }
        public string sAlertType { get; set; }
        public string sAlertMessage { get; set; }
        public int iUserID { get; set; }
        public int iRoleID { get; set; }
        public int iSentMail { get; set; }
        public Guid FKiBoidXIGUID { get; set; }
        public string sBOName { get; set; }
        public Guid XIGUID { get; set; }
        public int iInstanceID { get; set; }
        public Guid iInstanceIDXIGUID { get; set; }

    }
}
