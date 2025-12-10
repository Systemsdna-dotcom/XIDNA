using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XI1ClickPermission_T")]
    public class XI1ClickPermissions : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiRoleID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
