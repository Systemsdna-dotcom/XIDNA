using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    [Table("XIMenu_T")]
    public class RightMenuTrees : CommonProperties
    {
        //public RightMenuTrees()
        //{
        //    this.SubGroups = new HashSet<RightMenuTrees>();
        //}
        [Key]
        public int ID { get; set; }
        public string RootName { get; set; }
        public string MenuID { get; set; }
        [Required(ErrorMessage = "Enter Group name")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Special characters not allowed")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Name { get; set; }
        public string ParentID { get; set; }
        [Required(ErrorMessage = "Select Organisation")]
        public int OrgID { get; set; }
        [Required(ErrorMessage = "Select the Role")]
        public int RoleID { get; set; }
        public int ActionType { get; set; }
        public int Priority { get; set; }
        //[Required(ErrorMessage = "* Required")]
        //[StringLength(64, ErrorMessage = "The field cannaot be left empty.")]
        public string MenuController { get; set; }
        //[Required(ErrorMessage = "* Required")]
        //[StringLength(64, ErrorMessage = "The field cannaot be left empty.")]
        public string MenuAction { get; set; }
        [NotMapped]
        public List<VMDropDown> VMXILink { get; set; }
        public int XiLinkID { get; set; }
        [NotMapped]
        public string XiLinkName { get; set; }
        [NotMapped]
        public List<VMDropDown> MenuGroup { get; set; }
        [NotMapped]
        public List<VMDropDown> Organisations { get; set; }
        [NotMapped]
        public List<VMDropDown> Roles { get; set; }
        [NotMapped]
        public List<RightMenuTrees> SubGroups { get; set; }
        public int FKiApplicationID { get; set; }
        [NotMapped]
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields

    }
}

