using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class Reminders
    { 
    [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Select Reminder Time")]
        public DateTime ReminderTime { get; set; }
        [Required(ErrorMessage = "Please Enter Message")]
        [StringLength(512, ErrorMessage = "Message cannot be longer than 512 characters.")]
        public string Message { get; set; }
        public int OrganizationID { get; set; }
        public int UserID { get; set; }
        public int? LeadID { get; set; }
        public int CreatedByID { get; set; }
        public int UpdatedByID { get; set; }
        public string UpdatedBySysID { get; set; }
        public int StatusTypeID { get; set; }
        public int ReportID { get; set; }
        public int ClassID { get; set; }
    }
}
