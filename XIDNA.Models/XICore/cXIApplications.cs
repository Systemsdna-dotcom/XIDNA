using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIApplication_T")]
    public class cXIApplications : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public string sApplicationName { get; set; }
        public string sLogo { get; set; }
        public string sDatabaseName { get; set; }
        public string sDescription { get; set; }
        [NotMapped]
        public string XIAppUserName { get; set; }
        [NotMapped]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).*$", ErrorMessage = "Must be 6 characters long & combination of digits, upper and lower case letters")]
        [DataType(DataType.Password)]
        public string XIAppPassword { get; set; }
        [NotMapped]
        [Compare("XIAppPassword", ErrorMessage = "The password and confirm password do not match.")]
        [DataType(DataType.Password)]
        public string XIAppConfirmPassword { get; set; }
        public string sConnectionString { get; set; }
        public string sTheme { get; set; }
        public string sUserName { get; set; }
        [NotMapped]
        public string sType { get; set; }
    }
}
