using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XISemanticsNavigations_XSN_T")]
    public class cXISemanticsNavigations : CommonProperties
    {
        [Key]
        public int XINavigationID { get; set; }
        public int FKiXIStepID { get; set; }
        public string sName { get; set; }
        public int iType { get; set; }
        public int iNextStepID { get; set; }
        public int iOrder { get; set; }
        public string sFunction { get; set; }
        public string sField { get; set; }
        public string sOperator { get; set; }
        public string sValue { get; set; }
    }
}
