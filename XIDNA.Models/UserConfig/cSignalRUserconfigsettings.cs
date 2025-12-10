using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XISignalRUsersSettings_AU_T")]
   public class cSignalRUserconfigsettings
    {
        public int ID { get; set; }
        public int iUserID { get; set; }
        public string sConfig { get; set; }
        public DateTime? dtTurnOFFFrom { get; set; }
        public DateTime? dtTurnOFFTo { get; set; }
        public bool bFlag { get; set; }
        public int iStatus { get; set; }
        public int iShowCount { get; set; }
        public string sConstantID { get; set; }
        public int iNotificationBO { get; set; }
        public string sSelectedFields { get; set; }
        public int FkiNewSignalrQueryID { get; set; }
        public int fkiOneClick { get; set; }
        public int fkidepOneClick { get; set; }
        public string sWhereFields { get; set; }
        public string sBOSelectedFields { get; set; }
        public string sOneClickOrBO { get; set; }
        public string sAlertText { get; set; }
        public int fkiBOID { get; set; }
        [NotMapped]
        public int TableCount { get; set; }
        [NotMapped]
        public List<string> PopMessages { get; set; }
        public string sSentMail { get; set; }
        public int iRoleID { get; set; }
        public int FKiXilinkID { get; set; }
    }
}
