using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    public class LeadActions
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }public string Name { get; set; }
        public int Type { get; set; }        
        public int StatusTypeID { get; set; }
        public bool IsStage { get; set; }
        public int StageID { get; set; }
        public bool IsSMS { get; set; }
        public int SMSTemplateID { get; set; }
        public bool IsEmail { get; set; }
        public int EmailTemplateID { get; set; }
        public bool IsPopup { get; set; }
        public int PopupID { get; set; }
        public string Query { get; set; }
        public bool IsOneClcik { get; set; }
    }
}
