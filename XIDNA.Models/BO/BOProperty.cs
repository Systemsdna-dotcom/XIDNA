using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.Models
{
    public class BOProperty
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Enter Property Name")]
        [StringLength(30, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [System.Web.Mvc.Remote("IsExistsPropertyName", "BusinessObjects", AdditionalFields = "CType, ID", HttpMethod = "POST", ErrorMessage = "Property Name Already Exists.")]
        public string Name { get; set; }
        public bool IsWhere { get; set; }
        public bool IsExpression { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public string CType { get; set; }
    }
}

