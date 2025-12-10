using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class EmailDetails
    {
        [Key]
        public int id { get; set; }

        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DOB { get; set; }
        public string HomeTelephoneNo { get; set; }
        public string DaytimeTelephoneNo { get; set; }
        public string MobileNo { get; set; }
        public string EmailAddress { get; set; }
        public string ContactTelephoneNumber { get; set; }
        public string FullName { get; set; }
        public string TelephoneNumber { get; set; }
    }
}
