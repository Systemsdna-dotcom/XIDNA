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
    [Table("XIMasterData_T")]
    public class Types
    {
        [Key]
        public int ID { get; set; }
        public int Code { get; set; }
        [Required(ErrorMessage = "Enter Type ID")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Only Numbers allowed")]
        public int TypeID { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        [Required(ErrorMessage = "Enter name")]
        [StringLength(30, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        //[RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only alpha numeric allowed")]
        [System.Web.Mvc.Remote("IsExistsDataName", "Master", AdditionalFields = "ID, Code", HttpMethod = "POST", ErrorMessage = "Name already exists.")]
        public string Expression { get; set; }
        public string Icon { get; set; }
        public int Status { get; set; }
        public string FileName { get; set; }
        [NotMapped]
        public List<VMDropDown> Names { get; set; }
        public int FKiApplicationID { get; set; }
        public int OrganisationID { get; set; }
        [NotMapped]
        public Guid ValueXIGUID { get; set; }
    }
}
