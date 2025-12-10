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
    [Table("XITemplate_T")]
    public class ContentEditors
    {
        public int ID { get; set; }
        [AllowHtml]
        //[Required(ErrorMessage = " Please Select Content")]
        //[StringLength(160, ErrorMessage = "Message cannot be longer than 160 characters.")]
        public string Content { get; set; }
        //[Required(ErrorMessage = " Please Enter Title")]
        //[System.Web.Mvc.Remote("IsExistsTitle", "Content", AdditionalFields = "TypeC, ID, Name,Type", HttpMethod = "POST", ErrorMessage = "Title already exists.")]
        public string Name { get; set; }
        public string sCSSFileName { get; set; }
        public string sTemplateHeader { get; set; }
        public int? iParentID { get; set; }
        //[Required(ErrorMessage = "Please Select Template")]
        public int Category { get; set; }
        public int OrganizationID { get; set; }
        public int FKiApplicationID { get; set; }
        public int Type { get; set; }
        public int BO { get; set; }
        public bool bIsHavingAttachments { get; set; }
        public bool bIsPaswordProtected { get; set; }
        public int? iSurNamePasswordRange { get; set; }
        public int? iDOBPasswordRange { get; set; }
        public int? iTypeofPDF { get; set; }

        public string sFrom { get; set; }
        public string sBCC { get; set; }
        public string sCC { get; set; }
        public int FkiServerID { get; set; }
        public bool bIsBCCOnly { get; set; }
        public DateTime? dActiveFrom { get; set; }
        public DateTime? dActiveTo { get; set; }
        [NotMapped]
        public string Email { get; set; }
        [NotMapped]
        public string Feilds { get; set; }
        [NotMapped]
        public List<VMDropDown> ContentList { get; set; }
        [NotMapped]
        public List<VMDropDown> BOList { get; set; }
        [NotMapped]
        public string TypeC { get; set; }
        [NotMapped]
        public int SContent { get; set; }
        [NotMapped]
        public int DateField { get; set; }
        [NotMapped]
        public string str { get; set; }
        [NotMapped]
        public List<VMDropDown> Images { get; set; }
        [NotMapped]
        public List<VMDropDown> TypeList { get; set; }
        [NotMapped]
        public string SMSContent { get; set; }
        public List<VMDropDown> ddlApplications { get; set; }
        [NotMapped]
        public Dictionary<int, string> ddlParentList { get; set; }
        [NotMapped]
        public List<VMDropDown> IOServerList { get; set; }
    }
}
