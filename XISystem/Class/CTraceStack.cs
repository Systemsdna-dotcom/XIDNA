using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XISystem
{
    public class CTraceStack
    {
        public string sName { get; set; }
        public string sClass { get; set; }
        public string sMethod { get; set; }
        public string sCode { get; set; }
        public string sMessage { get; set; }
        public string sQuery { get; set; }
        public string sQueryParams { get; set; }
        public double iLapsedTime { get; set; }
        public int iStatus { get; set; }
        public int iAlert { get; set; }
        public string sParentID { get; set; }
        public string sProcessID { get; set; }
        public string sParams { get; set; }
        public string sTask { get; set; }

        public List<CNV> oParams = new List<CNV>();

        public List<CTraceStack> oTrace = new List<CTraceStack>(); 
    }
}