using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
     public class VMNewLeads
     {
         [Key]
         public int ID { get; set; }
         public string FirstName { get; set; }
         public string LastName { get; set; }
         public string Class { get; set; }
         public string Email { get; set; }
         public string Mobile { get; set; }
         [NotMapped]
         public List<VMNewLeads> Data { get; set; }

         [Required(ErrorMessage = "Please Select Lead")]
         public int LeadID { get; set; }
         [Required(ErrorMessage = "Please Select User Name")]
         public int UserID { get; set; }
         public int OrganizationID { get; set; }
         public string Content { get; set; }
    }
}
