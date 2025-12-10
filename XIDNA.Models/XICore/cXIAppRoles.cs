using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    [Table("XIAppRoles_AR_T")]
    public class cXIAppRoles : CommonProperties
    {
        public cXIAppRoles()
        {
            this.SubGroups = new HashSet<cXIAppRoles>();
        }
        [Key]
        public int RoleID { get; set; }
        public int iParentID { get; set; }
        //public string sRoleName { get; set; }
        [Required(ErrorMessage = "Please Enter Role Name")]
        [StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only alphabet allowed")]
        [Remote("IsExistsRoleName", "Users", HttpMethod = "POST", AdditionalFields = "RoleID", ErrorMessage = "Role Name Already Exists. Please Enter A Different Name.")]
        public string sRoleName { get; set; }
        public int FKiOrganizationID { get; set; }
        [Required(ErrorMessage = "Select Layout")]
        public int iLayoutID { get; set; }
        [Required(ErrorMessage = "Please Select Theme")]
        public int iThemeID { get; set; }
        [ForeignKey("iParentID")]
        public virtual ICollection<cXIAppRoles> SubGroups { get; set; }
        [ForeignKey("iParentID")]
        public virtual cXIAppRoles groups { get; set; }
        public bool IsLeaf
        {
            get
            {
                return this.SubGroups.Count == 0;
            }
        }
        [NotMapped]
        public int ReportToID { get; set; }
        [NotMapped]
        public int UserID { get; set; }
        [NotMapped]
        public List<VMDropDown> LayoutsList { get; set; }
        [NotMapped]
        public List<VMDropDown> ThemesList { get; set; }
        public bool bSignalR { get; set; }
    }
}
