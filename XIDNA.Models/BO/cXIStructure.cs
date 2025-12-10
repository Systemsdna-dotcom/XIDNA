using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
//using System.Web.DynamicData;

namespace XIDNA.Models
{
    [Table("XIBOStructure_T")]
    public class cXIStructure : CommonProperties
    {
        [Key]
        public long ID { get; set; }
        public string FKiParentID { get; set; }
        public  Nullable<Guid> FKiParentIDXIGUID { get; set; }
        public string sStructureName { get; set; }
        public string sName { get; set; }
        public string sMode { get; set; }
        public int BOID { get; set; }
        public string sBO { get; set; }
        public int FKi1ClickID { get; set; }
        public int FKiXIApplicationID { get; set; }
        public bool bMasterEntity { get; set; }
        public bool bIsAutoCreateDone { get; set; }
        public string sCode { get; set; }
        public string sType { get; set; }
        public bool bIsVisible { get; set; }
        public string sParentFKColumn { get; set; }
        public string sLinkingType { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public string sOutputArea { get; set; }
        public Guid XIGUID { get; set; }
        [NotMapped]
        public List<cXIStructure> SubXITreeNodes { get; set; }
        [NotMapped]
        public bool bIsExists { get; set; }
        [NotMapped]
        public string FKiStepDefinitionName { get; set; }
        [NotMapped]
        public string sSavingType { get; set; }
        [NotMapped]
        public List<VMDropDown> BOList { get; set; }
        [NotMapped]
        public string sInsName { get; set; }
        [NotMapped]
        public string sInsID { get; set; }
        public int iOrder { get; set; }
        [NotMapped]
        public int i1ClickID { get; set; }
        [NotMapped]
        public Dictionary<string, string> AllQSSteps { get; set; }
        public int OrganisationID { get; set; }
        [NotMapped]
        public cXIStructureDetails StructureDetail { get; set; }
    }

    [Table("XIBOStructureDetail_T")]
    public class cXIStructureDetails
    {
        [Key]
        public long ID { get; set; }
        public long iParentStructureID { get; set; }
        public long FKiStructureID { get; set; }
        public int iTabXiLinkID { get; set; }
        public int i1ClickID { get; set; }
        public int iCreateDialogID { get; set; }
        public int iEditDialogID { get; set; }
        public int iCreateFormXiLinkID { get; set; }
        public int iEditFormXiLinkID { get; set; }
        public int iCreateLayoutID { get; set; }
        public int iEditLayoutID { get; set; }
        public Guid i1ClickIDXIGUID { get; set; }
        public Guid FKiStructureIDXIGUID { get; set; }
    }
}

