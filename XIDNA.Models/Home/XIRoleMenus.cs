using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    [Table("XIRoleMenus_T")]
    public class XIRoleMenus : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public string RootName { get; set; }
        public int MenuID { get; set; }
        public Guid MenuIDXIGUID { get; set; }
        public string FKiInboxID { get; set; }
        public string Name { get; set; }
        public string ParentID { get; set; }
        public int OrgID { get; set; }
        public int RoleID { get; set; }
        public int ActionType { get; set; }
        public int Priority { get; set; }
        public string MenuController { get; set; }
        public string MenuAction { get; set; }
        public int FKiApplicationID { get; set; }
        public string[] MenuParams { get; set; }
        [NotMapped]
        public List<XIRoleMenus> oMenuParams { get; set; }
        [NotMapped]
        public string Root { get; set; }
        public int XiLinkID { get; set; }
        [NotMapped]
        public string[] SelectedNodes { get; set; }
        [NotMapped]
        public string Type { get; set; }
    }
}
