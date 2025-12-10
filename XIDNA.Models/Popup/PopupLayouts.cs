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
    [Table("XILayout_T")]
    public class cLayouts : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Please enter layout name ")]
        [StringLength(512, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 2)]
        public string LayoutName { get; set; }
        [Required(ErrorMessage = "Please enter layout structure code ")]
        public string LayoutCode { get; set; }
        [Required(ErrorMessage = "Select Layout Type")]
        public string LayoutType { get; set; }
        [Required(ErrorMessage = "Select Layout Level")]
        public string LayoutLevel { get; set; }
        public string Authentication { get; set; }
        public int XiParameterID { get; set; }
        public int iThemeID { get; set; }
        public string sSiloAccess { get; set; }
        public bool bIsTaskBar { get; set; }
        public string sTaskBarPosition { get; set; }
        [ForeignKey("LayoutID")]
        public virtual ICollection<PopupLayoutDetails> LayoutDetails { get; set; }
        [ForeignKey("PopupLayoutID")]
        public virtual ICollection<PopupLayoutMappings> LayoutMappings { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXiParameters { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIThemes { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        public bool bUseParentGUID { get; set; }
        [NotMapped]
        public List<string> arrSiloAccess { get; set; }
        public Nullable<Guid> XIGUID { get; set; }
    }

    [Table("XILayoutDetail_T")]
    public class PopupLayoutDetails
    {
        [Key]
        public int PlaceHolderID { get; set; }
        public int FKiApplicationID { get; set; }
        public int LayoutID { get; set; }
        public string PlaceholderName { get; set; }
        public string PlaceholderArea { get; set; }
        public string PlaceholderUniqueName { get; set; }
        public string PlaceholderClass { get; set; }
        public Nullable<Guid> LayoutIDXIGUID { get; set; }
    }
}
