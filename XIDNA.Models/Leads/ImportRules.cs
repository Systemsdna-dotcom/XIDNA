using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class ImportRules
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage="Enter Rule Name")]
        [RegularExpression(@"^[ A-Za-z0-9/&(),]*$", ErrorMessage = "No special characters allowed")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Must be between 2 to 50 characters")]
        public string RuleName { get; set; }

        [Required(ErrorMessage = "Enter Rule Value")]
        [RegularExpression(@"^[ A-Za-z0-9/&(),]*$", ErrorMessage = "No special characters allowed")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Must be between 2 to 50 characters")]
        public string RuleValue { get; set; }

        [Required(ErrorMessage = "Enter Rule Type")]
        [RegularExpression(@"^[ A-Za-z0-9/&(),]*$", ErrorMessage = "No special characters allowed")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Must be between 2 to 50 characters")]
        public string RuleType { get; set; }

        [Required(ErrorMessage = "Enter Count")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Only Numbers Allowed")]
        public int Count { get; set; }
    }
}
