using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMBOProperties
    {
        public int ID { get; set; }
        public int BOID { get; set; }
        public int FieldCreatedID { get; set; }
        public int OrganizationID { get; set; }
        public string Name { get; set; }
        public string AliasName { get; set; }
        public string Class { get; set; }
        public int TypeID { get; set; }
        public string DataType { get; set; }
        public string FieldType { get; set; }
        public string MaxLength { get; set; }
        public string Description { get; set; }
        public byte StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FieldClass { get; set; }
        public int FieldClassID { get; set; }
        public string TableName { get; set; }
        public string FieldMaxLength { get; set; }
        public string BOName { get; set; }
        public bool IsVisible { get; set; }
        public bool IsWhere { get; set; }
        public bool IsTotal { get; set; }
        public bool IsGroup { get; set; }
        public bool IsOrder { get; set; }
        public bool IsLabel { get; set; }
        public bool IsSummery { get; set; }
        public bool IsGrid { get; set; }
        public bool IsList { get; set; }
        public bool IsSearch { get; set; }
        public bool IsRunTime { get; set; }
        public bool IsDBValue { get; set; }
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
        public string Type { get; set; }
        public List<string> GroupNames { get; set; }
        public List<string> GroupSqlFields { get; set; }
        public List<string> GroupFields { get; set; }
        public List<int> GroupIDs { get; set; }
        public List<BODynamicProperties> Properties { get; set; }
        public List<BODynamicProperties> Headings { get; set; }
    }
    public class BODynamicProperties
    {
        public string Name { get; set; }
        public bool IsExpression { get; set; }
        public bool Value { get; set; }
        public bool IsWhere { get; set; }
    }
}
