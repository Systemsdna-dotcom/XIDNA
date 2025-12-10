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
    [Table("XIPopup_T")]
    public class Popup
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Select BO")]
        public int BOID { get; set; }
        [Required(ErrorMessage = "Select Foregin Key column")]
        public int FKColumnID { get; set; }
        public bool IsFKPopup { get; set; }
        public bool IsLeftMenu { get; set; }
        public bool IsGrouping { get; set; }
        public int ParentID { get; set; }
        public string BarPosition { get; set; }
        [Required(ErrorMessage = "Select Popup Size")]
        public string PopupSize { get; set; }
        public int PopupWidth { get; set; }
        public int PopupHeight { get; set; }
        [Required(ErrorMessage = "Enter Popup Name")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [StringLength(50, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsPopupName", "Popup", AdditionalFields = "ID, Name", HttpMethod = "POST", ErrorMessage = "Popup Name already exists.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Layout Type")]
        public int LayoutID { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> Popups { get; set; }
        [NotMapped]
        public List<VMDropDown> AllBOs { get; set; }
        [NotMapped]
        public List<VMDropDown> AllColumns { get; set; }
        [NotMapped]
        public List<VMDropDown> Layouts { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }


    }
}
