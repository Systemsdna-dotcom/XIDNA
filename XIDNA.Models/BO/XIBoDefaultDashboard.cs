using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XIBODashboardCharts_T")]
    public class XIBoDefaultDashboard
    {
        [Key]
        public int ID { get; set; }
        public int FKiOneClickID { get; set; }
        public int FKiBOID { get; set; }
        public string sType { get; set; }
        //public string sQuery { get; set; }
        public bool bFlag { get; set; }
        public int FKiPlaceholderID { get; set; }
        public int XiLinkTypeID { get; set; }
        public int iRowXilinkID { get; set; }
        public int FKiLayoutID { get; set; }
        public int FKiDialogID { get; set; }
        public int FKiAttributeID { get; set; }
        public string sAttrName { get; set; }
        public int FKiComponentTypeID { get; set; }
        public string sContentType { get; set; }
    }
}
