using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XIInbox_T")]
    public class XIInbox
    {
        public int ID { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiBOID { get; set; }
        public int ClassID { get; set; }
        public int TypeID { get; set; }
        public int OrganizationID { get; set; }
        public int RoleID { get; set; }
        public int Rank { get; set; }
        public string Icon { get; set; }
        public int Target { get; set; }
        public int Location { get; set; }
        public int DisplayAs { get; set; }
        public string TargetResultType { get; set; }
        public int TargetTemplateID { get; set; }
        public int MenuID { get; set; }
        public int FKiApplicationID { get; set; }
        public int FKiXILinkID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime ModifiedTime { get; set; }
        public int StatusTypeID { get; set; }
        public bool bSignalR { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
