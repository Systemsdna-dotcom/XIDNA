using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    [Table("XI1ClickLink_T")]
    public class VMXI1ClickLinks
    {
        public long ID { get; set; }
        public string sName { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiXILinkID { get; set; }
        public int iType { get; set; }
        public string sCode { get; set; }
        public bool bIsCreate { get; set; }
        public bool bIsEdit { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsDelete { get; set; }
        public bool bIsRefresh { get; set; }
        public string XILinkName { get; set; }
    }
}
