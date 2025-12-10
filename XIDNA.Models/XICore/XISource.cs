using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XISource_T")]
    public class XISource
    {
        [Key]
        public int ID { get; set; }
        public string sName { get; set; }
        public string sDescription { get; set; }
        public int iStatus { get; set; }
        public string sCode { get; set; }
        public string XICreatedBy { get; set; }
        public DateTime XICreatedWhen { get; set; }
        public string XIUpdatedBy { get; set; }
        public DateTime XIUpdatedWhen { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
    }
}