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
    public class OrganizationTeams
    {
        [Key]
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        [Required(ErrorMessage = "Enter Team Name")]
        [StringLength(64, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        [System.Web.Mvc.Remote("IsExistsTeamName", "Organization", AdditionalFields = "ID, Name", HttpMethod = "POST", ErrorMessage = "Team Name already exists.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Users")]
        public string Users { get; set; }
        public int StatusTypeID { get; set; }
        public string UserIDs { get; set; }
        //[NotMapped]
        //public string TypeC { get; set; }
        [NotMapped]
        public List<string> UsersList { get; set; }
        [NotMapped]
        public List<string> sUsers { get; set; }
        [NotMapped]
        public List<string> sUserIDs { get; set; }
        [NotMapped]
        public List<string> aUsers { get; set; }
        [NotMapped]
        public List<string> aUserIDs { get; set; }
        //[NotMapped]
        //public List<VMOrganizationTeams> AllUsers { get; set; }
    }
}
