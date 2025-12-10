using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.Models
{
    public class AddFields
    {
        public int ID { get; set; }
        public int BOID { get; set; }
        [Required(ErrorMessage="Select Class Specific")]
        public string ClassSpecific { get; set; }
        [Required(ErrorMessage = "Select Class")]
        public int Class { get; set; }
        [Required(ErrorMessage = "Enter Field Name")]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Use Alphabets only")]
        //[Remote("IsExistsFieldName", "Fields", AdditionalFields = "CreationType", HttpMethod = "POST", ErrorMessage = "Field Name already exists. Please enter a different Name.")]
        public string FieldName { get; set; }
        [Required(ErrorMessage = "Select Field Type")]
        //[Remote("IsTypeChangable", "Fields", AdditionalFields = "ID, CreationType", HttpMethod = "POST", ErrorMessage = "This Type Changing Is Not Possible")]
        public string FieldType { get; set; }
        [Required(ErrorMessage = "Select Length")]
        //[Remote("IsLengthChangable", "Fields", AdditionalFields = "ID, CreationType", HttpMethod = "POST", ErrorMessage = "Decreasing Size Is Not Allowed")]
        public string Length { get; set; }
        public string OrgName { get; set; }
        public int OrganizationID { get; set; }
        public string OldFieldName { get; set; }
        public string TableName { get; set; }
        public List<Classes> Classes { get; set; }
        public string ClassName { get; set; }
        public int ClassID { get; set; }
        public string Type { get; set; }
        public Nullable<int> BID { get; set; }
        public string CreationType { get; set; }
        public string Name { get; set; }
    }

    public class Classes
    {
        public int value { get; set; }
        public string text { get; set; }
    }
}
