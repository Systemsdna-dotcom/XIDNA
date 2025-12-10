using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;


namespace XIDNA.Models
{
    [Table("XIDialog_T")]
    public class Dialogs : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganizationID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        [Required(ErrorMessage = "Enter Dailog Name")]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsDialogName", "Popup", AdditionalFields = "DialogName, ID", HttpMethod = "POST", ErrorMessage = "Dailog Name already exists.")]
        public string DialogName { get; set; }
        [Required(ErrorMessage = "Select Layout")]
        public int LayoutID { get; set; }
        public bool IsResizable { get; set; }
        public bool IsCloseIcon { get; set; }
        public bool IsMinimiseIcon { get; set; }
        public bool IsGrouping { get; set; }
        public bool IsPinIcon { get; set; }
        public bool IsMaximiseIcon { get; set; }
        //[Required(ErrorMessage = "Select Bar Position")]
        public string BarPosition { get; set; }
        [Required(ErrorMessage = "Select Dialog Size")]
        public string PopupSize { get; set; }
        [Required(ErrorMessage = "Enter Dialog Width")]
        public int DialogWidth { get; set; }
        [Required(ErrorMessage = "Enter Dialog Height")]
        public int DialogHeight { get; set; }
        public string Icon { get; set; }
        [NotMapped]
        public List<VMDropDown> Layouts { get; set; }
        public string DialogMy1 { get; set; }
        public string DialogMy2 { get; set; }
        public string DialogAt1 { get; set; }
        public string DialogAt2 { get; set; }
        public int iTransparency { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }

    }
}
