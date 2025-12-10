using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace XIDNA.ViewModels
{
   public class VMUsers
    {
       [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int UserID { get; set; }
        public int Role { get; set; }
        public string RoleName { get; set; }
        public int OrganizationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailID { get; set; }
        public string Mobile { get; set; }
        public int StatusTypeID { get; set; }
        public int PaginationCount { get; set; }
        public int ParentID { get; set; }
        public string Email { get; set; }
    }
}
