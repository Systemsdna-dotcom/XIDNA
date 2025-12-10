using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class CommonProperties
    {
        public int StatusTypeID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string CreatedBySYSID { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime
        {
            get { return DateTime.Now; }
            set { }
        }
        public string UpdatedBySYSID { get; set; }
    }
}
