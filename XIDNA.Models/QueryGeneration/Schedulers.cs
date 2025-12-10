using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class Schedulers
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int ReportID { get; set; }
        public int UserID { get; set; }
        public string Period { get; set; }
        public int Date { get; set; }
        public string Day { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public int EmailTemplateID { get; set; }
        public int SMSTemplateID { get; set; }
        public int StatusTypeID { get; set; }
        public DateTime? LastExecutedOn { get; set; }
        public Guid XIGUID { get; set; }
        public Nullable<Guid> FKi1ClickIDXIGUID { get; set; }
    }
}
