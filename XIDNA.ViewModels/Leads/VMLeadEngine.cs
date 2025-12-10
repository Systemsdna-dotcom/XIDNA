using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadEngine
    {
        public bool bInsertLead { get; set; }
        public long LeadID { get; set; }
        public int? Interval { get; set; }
        public bool bUpdateSource { get; set; }
        public int SourceID { get; set; }
        public int OldSourceID { get; set; }
        public int ClassID { get; set; }
        public int ClientID { get; set; }
        public bool AlwaysInsert { get; set; }
        public bool InsertWithClassAndRenewalDate { get; set; }
        public bool InsertByClass { get; set; }
        public bool InsertToLead { get; set; }
        public bool InsertToInstance { get; set; }
    }
}
