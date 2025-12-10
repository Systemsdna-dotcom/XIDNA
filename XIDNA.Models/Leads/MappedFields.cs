using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using XIDNA.ViewModels;

namespace XIDNA.Models
{
    public class MappedFields
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public int BOID { get; set; }
        [Required(ErrorMessage = "Select Class")]
        public int ClassID { get; set; }
        public string SubscriptionID { get; set; }
        [Required(ErrorMessage="Enter Description")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Enter Field Name")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Use Alphabets only")]
        [Remote("IsExistsFieldName", "Fields", AdditionalFields = "ID, ClassID", HttpMethod = "POST", ErrorMessage = "Field Name already exists. Please enter a different Name.")]
        public string FieldName { get; set; }
        public string AddField { get; set; }
        public bool IsDropDown { get; set; }
        public int MasterID { get; set; }
        [Required(ErrorMessage = "Select Type")]
        [Remote("IsTypeChangable", "Fields", AdditionalFields = "ID, CreationType", HttpMethod = "POST", ErrorMessage = "This Type Changing Is Not Possible")]
        public string FieldType { get; set; }
        [Required(ErrorMessage="Select Length")]
        [Remote("IsLengthChangable", "Fields", AdditionalFields = "ID, CreationType", HttpMethod = "POST", ErrorMessage = "Decreasing Size Is Not Allowed")]
        public string Length { get; set; }
        [NotMapped]
        public string CreationType { get; set; }
        [NotMapped]
        public string Type { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlBOs { get; set; }
        [NotMapped]
        public List<Classes> Classes { get; set; }
        [NotMapped]
        public List<VMDropDown> Subscriptions { get; set; }
        [NotMapped]
        public string ClassSpecific { get; set; }
    }
}
