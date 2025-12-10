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
    [Table("XI1ClickLink_T")]
    public class XI1ClickLinks
    {
        public long ID { get; set; }
        public string sName { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiXILinkID { get; set; }
        public int iType { get; set; }
        public string sCode { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
