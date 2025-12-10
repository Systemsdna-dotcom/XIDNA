using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class UserDialogs
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int UserID { get; set; }
        public int OneClickID { get; set; }
        public bool Status { get; set; }
    }
}
