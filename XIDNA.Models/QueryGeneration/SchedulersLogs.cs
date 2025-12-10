using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class SchedulersLogs
    {
        [Key]
        public int ID { get; set; }
        public int SchedulerID { get; set; }
        public int OrganizationID { get; set; }
        public string Period { get; set; }
        public string Time { get; set; }
        public DateTime LastExecutedOn { get; set; }
        public string Status { get; set; }
        public string Error { get; set; }
    }
}
