using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XISignalRDependencyMaster_T")]
    public class cSignalRQuerySettings
    {
        public int ID { get; set; }
        public string sSelectedFields { get; set; }
        public int iTableID { get; set; }
    }
}
