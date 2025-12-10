using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XIMenuMappings_T")]
    public class XIMenuMappings : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int RoleID { get; set; }
        public string RootName { get; set; }
        public string MenuID { get; set; }
        public string ParentID { get; set; }
        public int OrgID { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public List<XIMenuMappings> oMenuParams { get; set; }
    }
}