using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XIDNA.ViewModels;
namespace XIDNA.Models
{
    [Table("XI1ClickParameter_T")]
    public class XI1ClickParameterNDVs
    {
        [Key]
        public int ID { get; set; }
        public int FKi1ClickID { get; set; }
        public string sName { get; set; }
        public string sDefault { get; set; }
        public string sValue { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBySYSID { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedBySYSID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
