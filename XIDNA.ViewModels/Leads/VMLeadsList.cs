using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadsList
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public int StatusTypeID { get; set; }
        public string sName { get; set; }
        public string sMob { get; set; }
        public string sEMail { get; set; }
        public string Class { get; set; }
        public string sForeName { get; set; }
        public string sLastName { get; set; }
        public int FKiLeadClassID { get; set; }
    }
}
