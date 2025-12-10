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
    public class LeadInbounds
    {
        [Key]
        public int ID { get; set; }
        public int SourceID { get; set; }
        public int FileID { get; set; }
        public DateTime ImportedOn { get; set; }
        public string ImportedBy { get; set; }
        public string Content { get; set; }
        public int StatusTypeID { get; set; }
        public string SubscriptionID { get; set; }
        [NotMapped]
        public List<VMDropDown> SourceDetails { get; set; }
    }
    //public class SourceDetails
    //{
    //    public string text { get; set; }
    //    public int Value { get; set; }
    //}
}
