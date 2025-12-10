using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace XIDNA.Models
{
    [Table("XIQSLinkDefinition_T")]
    public class XIQSLinkDefinition
    {
        public long ID { get; set; }
        [Required(ErrorMessage = "Enter QS Link Name")]
        [Remote("IsExistNameOrCode", "XISemantics", AdditionalFields = "ID", ErrorMessage = "QS Link Name Already Exists")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Enter Link Code")]
        [Remote("IsExistNameOrCode", "XISemantics", AdditionalFields = "ID", ErrorMessage = "QS Link Code Already Exists")]
        public string sCode { get; set; }
        public int FKiXILInkID { get; set; }
        public string sType { get; set; }
        public decimal rOrder { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedBySYSID { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UpdatedBySYSID { get; set; }
        public string sRunType { get; set; }
        public int FKIXIScriptID { get; set; }
        [NotMapped]
        public string[] NVPairs { get; set; }
        [NotMapped]
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields
        [NotMapped]
        public List<XIQSLinkDefinition> NVs { get; set; }
        [NotMapped]
        public string XiLinkName { get; set; }

    }
}
