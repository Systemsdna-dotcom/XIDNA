using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIErrorLog_T")]
    public class ErrorLogs
    {
        [Key]
        public int ID { get; set; }
        public int ObjectID { get; set; }
        public int TableTypeID { get; set; }
        public int TypeID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string sCode { get; set; }
    }
}
