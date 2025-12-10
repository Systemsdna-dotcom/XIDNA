using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    //[Table("XIBOGroup_T")]
    [Table("XIBOGroup_T_N")]
    public class BOGroupFields
    {
        [Key]
        public int ID { get; set; }
        public int BOID { get; set; }
        public string BOFieldIDs { get; set; }
        public string BOSqlFieldNames { get; set; }
        public string BOFieldNames { get; set; }
        public string GroupName { get; set; }
        public int TypeID { get; set; }
        public bool IsMultiColumnGroup { get; set; }
        public string Description { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public Guid BOIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
    }
}
