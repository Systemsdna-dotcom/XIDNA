using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class sVMValidationDetails
    {
        public List<string> sErMessages { get; set; }
        public List<int> iIDs { get; set; }
        public int iErrorCount { get; set; }
       public List<string> sNullDetails { get; set; }
        public List<string> sErrorCounts { get; set; }
        public string sStatus { get; set; }
        public int FileID { get; set; }
        public int SourceID { get; set; }
        //public int iEmailErrCount { get; set; }
        //public int iPostErrCount { get; set; }
        public List<VMLeadValidation> lErrorDetails { get; set; }

    }
}
