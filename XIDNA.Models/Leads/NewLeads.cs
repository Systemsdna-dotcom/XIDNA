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
    public class NewLeads
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
        public int OrganizationID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get;set;}
        public string Mobile { get; set; }
        public int Class { get; set; }
        public int StatusTypeID { get; set; }
        public List<Users1> users { get; set; }
        public List<UserLeads> newleads { get; set; }
        [NotMapped]
        public List<Classes> Classes { get; set; }

        [ForeignKey("Class")]
        public virtual Types types {get; set;}

        [NotMapped]
        public List<VMDropDown> ALLTeams { get; set; }

    }
    public class Users1
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
    public class UserLeads
    {
        public int SNo { get; set; }
        public long ID { get; set; }
        public string FirstName { get; set; }
        public int StatusTypeID { get; set; }
    }
}


