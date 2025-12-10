using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMCustomResponse
    {
        public long ID { get; set; }
        public string PropertyName { get; set; }
        public string ResponseMessage { get; set; }
        public bool IsException { get; set; }
        public bool Status { get; set; }
        public string sID { get; set; }
        public bool IsExceed { get; set; }
    }
}
