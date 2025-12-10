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
    [Table("XIBOUIDetails_T")]
    public class cBOUIDetails : CommonProperties
    {
        [Key]
        public long ID { get; set; }
        public int FKiBOID { get; set; }
        public int i1ClickID { get; set; }
        public int iLayoutID { get; set; }
        public long FKiStructureID { get; set; }
        public int FKiQSTemplateID { get; set; }
        public int FKiQSStepTemplateID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        [NotMapped]
        public string sBOName { get; set; }
        [NotMapped]
        public string sSavingType { get; set; }
        [NotMapped]
        public List<VMDropDown> ddl1Clicks { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlLayouts { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSTemplates { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSStepTemplates { get; set; }
    }

    [Table("XIBOUIDefault_T")]
    public class cBOUIDefaults : CommonProperties
    {
        [Key]
        public long ID { get; set; }
        public int FKiBOID { get; set; }
        public int i1ClickID { get; set; }
        public int iLayoutID { get; set; }
        public int iXIComponentID { get; set; }
        public int iStructureID { get; set; }
        public int iPopupID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
        [NotMapped]
        public string sBOName { get; set; }
        [NotMapped]
        public string sSavingType { get; set; }
        [NotMapped]
        public List<VMDropDown> ddl1Clicks { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlLayouts { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIComponents { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlStructures { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlPopups { get; set; }
    }
}
