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
    public class OrganizationLocations
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Select the Organisation")]
        public int OrganizationID { get; set; }
        [Required(ErrorMessage="Enter Location Name")]
        [StringLength(32, ErrorMessage = "The {0} Name must be at least {2} to {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [Remote("CheckOrgLocation", "Organization", AdditionalFields = "OrganizationID, ID", HttpMethod = "Post", ErrorMessage = "Location already exists!")]
        public string Location { get; set; }
        [Required(ErrorMessage="Enter location code")]
        [StringLength(32, ErrorMessage = "The {0} Name must be at least {2} to {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"^[a-zA-Z0-9_ ]+$", ErrorMessage = "Only Alpha Neumeric Allowed")]
        [Remote("IsExistsLocationCodeSpecific", "Organization", AdditionalFields = "OrganizationID, ID", HttpMethod = "Post", ErrorMessage = "Location code already exists!")]
        public string LocationCode { get; set; }
        public int StatusTypeID { get; set; }
        public int CreatedByID { get; set; }
        public string CreatedByName { get; set; }
        public string CreatedBySYSID { get; set; }
        public DateTime CreatedTime { get; set; }
        public int ModifiedByID { get; set; }
        public string ModifiedByName { get; set; }
        public string ModifiedBySYSID { get; set; }
        public DateTime ModifiedTime { get; set; }
        [NotMapped]
        public List<VMDropDown> OrgDetails { get; set; }
        [NotMapped]
        public string CreationType { get; set; }
        [NotMapped]
        public string Role { get; set; }

    }
    //class used to get the value from the Organisation table(ID,Name)
    public class OrgDetails
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}


