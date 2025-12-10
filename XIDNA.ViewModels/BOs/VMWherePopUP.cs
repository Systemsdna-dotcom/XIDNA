using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMWherePopUP
    {
        public string FieldName { get; set; }
        public int FieldID { get; set; }
        public int BOID { get; set; }
        public bool IsRuntimeValue { get; set; }
        public bool IsDBValue { get; set; }
        [Required(ErrorMessage = "Enter Query")]
        public string DBQuery { get; set; }
        public bool IsWhereExpression { get; set; }
        public bool IsDate { get; set; }
        public string DateExpression { get; set; }
        public string DateValue { get; set; }
        public string DBValueText { get; set; }
        [Required(ErrorMessage="Enter Expression Name")]
        public string WhereExpression { get; set; }
        [Required(ErrorMessage = "Enter Expression Value")]
        public string WhereExpressionValue { get; set; }
        public bool IsExpression { get; set; }
        [Required(ErrorMessage = "Enter Expression Name")]
        public string ExpressionText { get; set; }
        [Required(ErrorMessage = "Enter Expression Value")]
        public string ExpressionValue { get; set; }
        public string FieldDataType { get; set; }
        public string BOName { get; set; }
        public string Type { get; set; }
        public string Script { get; set; }
        public string ScriptExecutionType { get; set; }
    }
}
