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
    //public class Content
    //{
    //    public VMContentEditors ContentEditors { get; set; }
    //    public List<VMNewLeads> VMNewLeads { get; set; }

    //}
    public class VMContentEditors
    { 
        [Key]
        public int ID { get; set; }
        [AllowHtml]
        [StringLength(160, ErrorMessage = "Message cannot be longer than 160 characters.")]
        public string Content { get; set; }
        public int TID { get; set; }
        public string Name { get; set; }
        //[Required(ErrorMessage = "Please Select Type")]
        public int Type { get; set; }
        public string Email { get; set; }
        public string Feilds { get; set; }
        public List<VMDropdowns> ContentList { get; set; }
        public int SID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Class { get; set; }
        public string SEmail { get; set; }
        public string Mobile { get; set; }
        public List<VMNewLeads> Data { get; set; }
        [Required(ErrorMessage = "Please Select Lead")]
        public int LeadID { get; set; }
        [Required(ErrorMessage = "Please Select User Name")]
        public int UserID { get; set; }
        public int OrganizationID { get; set; }
        public string SContent { get; set; }
        public int Category { get; set; }


    }    
}
