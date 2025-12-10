using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("UserCookies_T")]
    public class cUserCookies
    {
        [Key]
        public int ID { get; set; }
        public int FKiUserProfileID { get; set; }
        public string UniqueCookieID { get; set; }

    }
}
