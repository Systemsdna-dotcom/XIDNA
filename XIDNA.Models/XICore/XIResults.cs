using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
   public class XIResults
    {
       public string sResult { get; set; }
        public string sCode { get; set; }
        public string sMessage { get; set; }
        public string Stack { get; set; }
        public List<cInformationNVPairs> NVPairs { get; set; }
    }
    public class cInformationNVPairs
    {
        public string sName { get; set; }
        public string sValue { get; set; }
    }
    }
