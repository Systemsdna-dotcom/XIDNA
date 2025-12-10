using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class AppNotifications : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string Icon { get; set; }
        public string Message { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public string ImageName { get; set; }
        
    }
}
