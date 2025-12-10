using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class Targets
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public int ColumnID { get; set; }
        public int Target { get; set; }
        public string Period { get; set; }
        public string Colour { get; set; }
        public bool IsSMS { get; set; }
        public int SMSTemplateID { get; set; }
        public bool IsEmail { get; set; }
        public int EmailTemplateID { get; set; }
        public bool IsNotification { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
