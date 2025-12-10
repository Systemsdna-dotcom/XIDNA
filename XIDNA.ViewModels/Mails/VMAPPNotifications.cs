using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XIDNA.ViewModels
{
    public class VMAppNotifications
    {
        public int ID { get; set; }
        public int OrganizationID { get; set; }
        public string Icon { get; set; }
        public string ImageName { get; set; }
        public string Image { get; set; }
        [Required(ErrorMessage = "Enter Message")]
        [StringLength(1000, ErrorMessage = "The {0} must be between {2} to {1} characters long.", MinimumLength = 3)]
        public string Message { get; set; }
        public int RoleID { get; set; }
        public int UserID { get; set; }
        public int StatusTypeID { get; set; }
        public List<VMDropDown> Roles { get; set; }
        public int Role { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public List<VMDropDown> GetUsers{ get; set; }
    }
}
