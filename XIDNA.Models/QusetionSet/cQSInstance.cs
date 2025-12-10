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

    //Question Set Definition
    [Table("XIQSDefinition_T")]
    public class cQSDefinition : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiOrganisationID { get; set; }

        [Required(ErrorMessage = "Please Enter Name")]
        [StringLength(128, ErrorMessage = "Maximum length should be 32 only")]
        public string sName { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        [StringLength(256, ErrorMessage = "Maximum length should be 256 only")]
        public string sDescription { get; set; }
        public int iVisualisationID { get; set; }
        public string SaveType { get; set; }
        public string sMode { get; set; }
        public bool bIsTemplate { get; set; }
        public string sHTMLPage { get; set; }
        public int FKiApplicationID { get; set; }
        public int iLayoutID { get; set; }
        public bool bInMemoryOnly { get; set; }
        //public bool bIsQSTraceStage { get; set; }
        [NotMapped]
        public List<cQSStepDefiniton> QSSteps { get; set; }
        [NotMapped]
        public cLayouts Layout { get; set; }
        [NotMapped]
        public XiVisualisations Visualisations { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIVisualisations { get; set; }
        [NotMapped]
        public List<cQSVisualisations> QSFieldVisualisations { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlLayouts { get; set; }
        [NotMapped]
        public bool bIsContextObject { get; set; }
        public int FKiParameterID { get; set; }
        public int FKiBOStructureID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIParameters { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIStructures { get; set; }
        public int FKiOriginID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlSourceList { get; set; }
        public int FKiSourceID { get; set; }
    }


    //Step Definition
    [Table("XIQSStepDefinition_T")]
    public class cQSStepDefiniton : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSDefintionID { get; set; }
        //[Required(ErrorMessage = "Please Enter Name")]
        [StringLength(128, ErrorMessage = "Maximum length should be 128 characters only")]
        public string sName { get; set; }
        [StringLength(512, ErrorMessage = "Maximum length should be 512 characters only")]
        public string sDisplayName { get; set; }
        public decimal iOrder { get; set; }
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sCode { get; set; }
        [Required(ErrorMessage = "Please Select atleast one")]
        public int iDisplayAs { get; set; }
        public int XILinkID { get; set; }
        [Required(ErrorMessage = "Please select")]
        public int FKiContentID { get; set; }
        public int iXIComponentID { get; set; }
        public int i1ClickID { get; set; }
        public string HTMLContent { get; set; }
        public bool bIsSaveNext { get; set; }
        public bool bIsSave { get; set; }
        public bool bIsBack { get; set; }
        public bool bIsSaveClose { get; set; }
        public bool bInMemoryOnly { get; set; }
        public int iLayoutID { get; set; }
        public string sIsHidden { get; set; }
        public string sSaveBtnLabel { get; set; }
        public string sBackBtnLabel { get; set; }
        [NotMapped]
        public string sSaveBtnLabelSaveNext { get; set; }
        [NotMapped]
        public string sSaveBtnLabelSaveClose { get; set; }
        [NotMapped]
        public string sSaveBtnLabelSave { get; set; }
        [NotMapped]
        public bool bIsHidden { get; set; }
        public bool bIsContinue { get; set; }
        public bool bIsHistory { get; set; }
        public bool bIsCopy { get; set; }
        public Guid XIGUID { get; set; }
        [NotMapped]
        public VMPopupLayout Layout { get; set; }
        [NotMapped]
        public List<VMDropDown> XIFields { get; set; }//auto complete fields
        [NotMapped]
        public Dictionary<string, string> XILinks { get; set; }//auto complete fields
        [NotMapped]
        public List<VMDropDown> ddlContent { get; set; }
        [NotMapped]
        public List<string> XIFieldValues { get; set; }//auto complete fields for edit
        [NotMapped]
        public string ComponentNVIDs { get; set; }
        [NotMapped]
        public string SecComponentNVIDs { get; set; }
        [NotMapped]
        public string SectionIDs { get; set; }
        [NotMapped]
        public List<cQSNavigations> QSNavigations { get; set; }
        [NotMapped]
        public List<cFieldDefinition> FieldDefinitions { get; set; }
        [NotMapped]
        public List<cStepSectionDefinition> Sections { get; set; }
        [NotMapped]
        public cXIComponents ComponentDefinition { get; set; }
        [NotMapped]
        public Reports OneClickDefinition { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlLayouts { get; set; }
        [NotMapped]
        public Dictionary<string, string> XICodes { get; set; }  //Auto Complete XILink Codes
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        //public bool bIsStepTraceStage { get; set; }
        public int FkiQuoteStageID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQuoteStage { get; set; }
    }

    //Navigation Definition
    [Table("XIQSNavigation_T")]
    public class cQSNavigations : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sName { get; set; }
        public int iType { get; set; }
        public int iNextStepID { get; set; }
        public decimal iOrder { get; set; }
        [StringLength(64, ErrorMessage = "Maximum length should be 64 only")]
        public string sFunction { get; set; }
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sField { get; set; }
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sOperator { get; set; }
        [StringLength(32, ErrorMessage = "Maximum length should be 32 only")]
        public string sValue { get; set; }
        //public int Order { get; set; }
        [NotMapped]
        public List<VMDropDown> SematicSteps { get; set; }
        [NotMapped]
        public bool IsSave { get; set; }
    }

    //Field Values
    [Table("XIFieldInstance_T")]
    public class cFieldInstance
    {
        [Key]
        public long ID { get; set; }
        public string sValue { get; set; }
        public int FKiFieldDefinitionID { get; set; }
        public int FKiQSInstanceID { get; set; }
        public int FKiQSStepDefinitionID { get; set; }
        public int FKiQSSectionDefinitionID { get; set; }
        public int iValue { get; set; }
        public DateTime? dValue { get; set; }
        public decimal rValue { get; set; }
        public bool bValue { get; set; }
        //[NotMapped]
        //public cFieldDefinition FieldDefinition { get; set; }
    }

    //Field Definition
    [Table("XIFieldOrigin_T")]
    public class cFieldOrigin : CommonProperties
    {
        [Key]
        public int ID { get; set; }
        public int FKiOrganisationID { get; set; }
        public int FKiApplicationID { get; set; }
        [RegularExpression(@"^\S*$", ErrorMessage = "Space is not allowed")]
        [Required(ErrorMessage = "Enter Field Name")]
        //[Remote("IsExistsFieldName", "XISemantics", AdditionalFields = "ID", HttpMethod = "POST", ErrorMessage = "Field name already exists. Please enter a different Name.")]
        [StringLength(128, ErrorMessage = "Maximum length should be 128 only")]
        public string sName { get; set; }

        [StringLength(512, ErrorMessage = "Maximum length should be 512 only")]
        public string sDisplayName { get; set; }

        public string sAdditionalText { get; set; }
        public int iLength { get; set; }
        public string sFieldDefaultValue { get; set; }

        [Required(ErrorMessage = "Please select atleast one DataType")]
        public int FKiDataType { get; set; }
        public int FK1ClickID { get; set; }
        public bool bIsOptionList { get; set; }
        public int iMasterDataID { get; set; }
        public string sIsHidden { get; set; }
        [StringLength(1024, ErrorMessage = "Maximum length should be 1024 only")]
        public string sDefaultValue { get; set; }

        [StringLength(1024, ErrorMessage = "Maximum length should be 1024 only")]
        public string sDisplayHelp { get; set; }

        [StringLength(128, ErrorMessage = "Maximum length should be 128 only")]
        public string sPlaceHolder { get; set; }
        [AllowHtml]
        [StringLength(1024, ErrorMessage = "Maximum length should be 1024 only")]
        public string sScript { get; set; }
        public int iValidationType { get; set; }
        public int iValidationDisplayType { get; set; }
        public bool bIsMandatory { get; set; }
        public int FKiBOID { get; set; }
        [NotMapped]
        public bool bIsHidden { get; set; }
        [NotMapped]
        public Dictionary<string, string> ddlOneClicks { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlMasterTypes { get; set; }
        [NotMapped]
        public List<cXIFieldOptionList> ddlFieldOptionList { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlDataTypes { get; set; }
        [NotMapped]
        public cXIDataTypes DataTypes { get; set; }
        [NotMapped]
        public string sOneClickName { get; set; }
        [NotMapped]
        public bool bIsLargeBO { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlApplications { get; set; }
        [NotMapped]
        public List<VMDropDown> XIFields { get; set; }//auto complete fields
        [NotMapped]
        public Dictionary<string, string> ddlBOs { get; set; }
        [NotMapped]
        public string sBOName { get; set; }
        public string sMinDate { get; set; }
        public string sMaxDate { get; set; }
        public bool bIsDisable { get; set; }
        public bool bIsMerge { get; set; }
        public string sMergeField { get; set; }
        public bool bIsCompare { get; set; }
        public string sCompareField { get; set; }
        public string sValidationMessage { get; set; }
        public string sMergeBo { get; set; }
        public string sMergeBoField { get; set; }
        public bool bIsUpperCase { get; set; }
        public bool bIsLowerCase { get; set; }
        public bool bIsHelpIcon { get; set; }
        public string sMergeVariable { get; set; }
        public string sCode { get; set; }
        public string sQSCode { get; set; }
        public bool bIsDisplay { get; set; }
        public string sDependentValue { get; set; }
    }

    //Field Option List
    [Table("XIFieldOptionList_T")]
    public class cXIFieldOptionList
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSFieldID { get; set; }
        public string sOptionName { get; set; }
        public string sOptionValue { get; set; }
        public string sOptionCode { get; set; }
        public int iType { get; set; }
        public string sShowField { get; set; }
        public string sHideField { get; set; }
    }

    //Field Instance
    [Table("XIFieldDefinition_T")]
    public class cFieldDefinition
    {
        [Key]
        public int ID { get; set; }
        public int FKiXIFieldOriginID { get; set; }
        public int FKiXIStepDefinitionID { get; set; }
        public int FKiStepSectionID { get; set; }
        [NotMapped]
        public cFieldOrigin FieldOrigin { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public int XIDeleted { get; set; }
        public Guid FKiStepSectionIDXIGUID { get; set; }
        public Guid FKiXIStepDefinitionIDXIGUID { get; set; }
    }

    //QS Instance
    [Table("XIQSInstance_T")]
    public class cQSInstance
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public int iCurrentStepID { get; set; }
        public string sQSName { get; set; }
        public string FKiUserCookieID { get; set; }
        public int FKiBODID { get; set; }
        public int iBOIID { get; set; }
        [NotMapped]
        public List<cQSStepInstance> nStepInstances { get; set; }
        [NotMapped]
        public virtual cQSDefinition QSDefinition { get; set; }
    }

    //Step Instance
    [Table("XIQSStepInstance_T")]
    public class cQSStepInstance
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSInstanceID { get; set; }
        public int FKiQSStepDefinitionID { get; set; }
        [NotMapped]
        public bool bIsCurrentStep { get; set; }
        public string sMessage { get; set; }
        [NotMapped]
        public List<cFieldInstance> nFieldInstances { get; set; }
        [NotMapped]
        public List<cStepSectionInstance> nSectionInstances { get; set; }
        //[NotMapped]
        //public cFullAddress ComponentInstance { get; set; }
        //[NotMapped]
        //public virtual cQSStepDefiniton QSStepDefiniton { get; set; }
    }


    [Table("XIStepSectionDefinition_T")]
    public class cStepSectionDefinition
    {
        [Key]
        public int ID { get; set; }
        public int FKiStepDefinitionID { get; set; }
        public int iDisplayAs { get; set; }
        public string sName { get; set; }
        public string sCode { get; set; }
        public string sIsHidden { get; set; }
        public bool bIsGroup { get; set; }
        public string sGroupDescription { get; set; }
        public string sGroupLabel { get; set; }
        public int iXIComponentID { get; set; }
        public int i1ClickID { get; set; }
        public string HTMLContent { get; set; }
        public decimal iOrder { get; set; }
        public Guid FKiStepDefinitionIDXIGUID { get; set; }
        public Guid? FkiRuleIDXIGUID { get; set; }
        public int FkiRuleID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlRules { get; set; }
        public Guid XIGUID { get; set; }
        [NotMapped]
        public bool bIsHidden { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlXIComponents { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlOneClicks { get; set; }
        [NotMapped]
        public List<cFieldDefinition> FieldDefinitions { get; set; }
        //[NotMapped]
        //public List<cFieldInstance> nFieldInstances { get; set; }
        [NotMapped]
        public cXIComponents ComponentDefinition { get; set; }
        [NotMapped]
        public Reports OneClickDefinition { get; set; }
        [NotMapped]
        public string[] SecNVPairs { get; set; }
        [NotMapped]
        public string[] QSLinks { get; set; }
        [NotMapped]
        public List<string> QSLink { get; set; }//auto complete fields
        [NotMapped]
        public List<string> QSLinkCodes { get; set; }//auto complete XiLink Codes
        [NotMapped]
        public string[] QSCodes { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        public int FKiParentSectionID { get; set; }
        public List<VMDropDown> ddlSections { get; set; }
    }

    [Table("XIStepSectionInstance_T")]
    public class cStepSectionInstance
    {
        [Key]
        public int ID { get; set; }
        public int FKiStepSectionDefinitionID { get; set; }
        public int FKiStepInstanceID { get; set; }
        public int FKiFieldInstance { get; set; }
        public int FKiXIComponentInstance { get; set; }
        public int FKiOneClickInstance { get; set; }
        [NotMapped]
        public List<cFieldInstance> nFieldInstances { get; set; }
        //[NotMapped]
        //public cFullAddress ComponentInstance { get; set; }
    }

    [Table("FullAddress_T")]
    public class cFullAddress
    {
        public int ID { get; set; }
        public string sFullAddress { get; set; }
        public int FKiSectionInstanceID { get; set; }
        public int FKiStepInstanceID { get; set; }
    }

    [Table("XIQSVisualisation_T")]
    public class cQSVisualisations
    {
        [Key]
        public int ID { get; set; }
        public int FKiQSDefinitionID { get; set; }
        public int FKiQSStepDefinitionID { get; set; }
        public int FKiSectionDefinitionID { get; set; }
        public int FKiFieldOriginID { get; set; }
        public string sVisualisation { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQuestionSets { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlQSStteps { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlSections { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlFields { get; set; }
        public int FKiApplicationID { get; set; }
        public List<VMDropDown> ddlApplications { get; set; }
        public int OrganisationID { get; set; }
    }
}
