using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace XISystem
{
    namespace XISystem.Class
    {
        public class CommonProperties
        {
            public int StatusTypeID { get; set; }
            public int CreatedBy { get; set; }
            public DateTime CreatedTime { get; set; }
            public string CreatedBySYSID { get; set; }
            public int UpdatedBy { get; set; }
            public DateTime UpdatedTime { get; set; }
            public string UpdatedBySYSID { get; set; }
        }
    }
}