using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.ComponentModel;
namespace XIDNA.Models
{
    //[Table("XIBOAttribute_T")]
    [Table("XIBOAttribute_T_N")]
    public class BOFields
    {
        //[Key]
        public int ID { get; set; }
        public int BOID { get; set; }

        public Guid BOIDXIGUID { get; set; }
        public Guid XIGUID { get; set; }
        public int FieldCreatedID { get; set; }
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        //[Required(ErrorMessage = "* Required")]
        //[StringLength(128, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 0)]
        public string LabelName { get; set; }
        public int TypeID { get; set; }
        public string FKTableName { get; set; }
        public string Script { get; set; }
        public string ScriiptExecutionType { get; set; }
        public string MaxLength { get; set; }
        public bool IsVisible { get; set; }
        public bool IsWhere { get; set; }
        public bool IsTotal { get; set; }
        public bool IsGroupBy { get; set; }
        public bool IsOrderBy { get; set; }
        public bool IsMail { get; set; }
        public bool IsRunTime { get; set; }
        public bool IsDBValue { get; set; }
        public bool bIsScript { get; set; }
        public string DBQuery { get; set; }
        public bool IsWhereExpression { get; set; }
        public string WhereExpression { get; set; }
        public string WhereExpreValue { get; set; }
        public bool IsExpression { get; set; }
        public string ExpressionText { get; set; }
        public string ExpressionValue { get; set; }
        public bool IsDate { get; set; }
        public string DateExpression { get; set; }
        public string DateValue { get; set; }
        public string Description { get; set; }
        public string Format { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FieldClass { get; set; }
        public int FieldClassID { get; set; }
        public string Value { get; set; }
        public string FKWhere { get; set; }
        public string sHelpText { get; set; }
        public string sNarrowBar { get; set; }
        public int FKiType { get; set; }
        public int iMasterDataID { get; set; }
        public bool IsNull { get; set; }
        public string sXMLDataType { get; set; }
        public string sPassword { get; set; }
        public string sPrecision { get; set; }
        public string sVirtualColumn { get; set; }
        public string sLock { get; set; }
        public string sCase { get; set; }
        public string ForeignTable { get; set; }
        public string ForeignColumn { get; set; }
        public int iOutputLength { get; set; }
        public string sPlaceHolder { get; set; }
        [NotMapped]
        public string PKColumnID { get; set; }
        [NotMapped]
        public string FKType { get; set; }
        [NotMapped]
        public string FieldType { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public List<string> GroupNames { get; set; }
        [NotMapped]
        public List<string> GroupSqlFields { get; set; }
        [NotMapped]
        public List<string> GroupFields { get; set; }
        [NotMapped]
        public List<int> GroupIDs { get; set; }
        [NotMapped]
        public string TableName { get; set; }
        [NotMapped]
        public string DataType { get; set; }
        [NotMapped]
        public string FieldMaxLength { get; set; }
        //[ForeignKey("BOID")]
        //public virtual BOs BOs { get; set; }
        [NotMapped]
        public string BOName { get; set; }
        [NotMapped]
        public string FieldValue { get; set; }

        public string DefaultValue { get; set; }
        public bool IsOptionList { get; set; }
        public int iOneClickID { get; set; }
        public int FKiFileTypeID { get; set; }
        public bool bKPI { get; set; }
        public int iSystemType { get; set; }
        [NotMapped]
        public List<VMDropDown> FileDetails { get; set; }
        [ForeignKey("BOFieldID")]
        public virtual ICollection<BOOptionLists> BOOptionLists { get; set; }
        [NotMapped]
        public List<VMDropDown> SysType { get; set; }
        [NotMapped]
        public List<VMDropDown> ImagePathDetails { get; set; }
        [NotMapped]
        public List<VMDropDown> FieldDDL { get; set; }
        [NotMapped]
        public string sOneClickName { get; set; }
        [NotMapped]
        public Dictionary<string, string> ddlOneClicks { get; set; }
        [NotMapped]
        public List<BOOptionLists> AttrBOOptionList { get; set; }
        //1/6/2018
        public string sNotes { get; set; }
        public string sEncrypted { get; set; }
        [NotMapped]
        public string sFKBOSize { get; set; }
        public bool IsTextArea { get; set; }
        public int iAttributeType { get; set; }
    }
    public class FileDetails
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
