using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;
using System.Web.Mvc;

namespace XIDNA.Models
{
    [Table("XIBOClassAttribute_T")]
    public class BOClassAttributes
    {
        [Key]
        public int ID { get; set; }
        public int BOID { get; set; }
        [Required(ErrorMessage = "Please Enter Class Name")]
        [Remote("IsExistsClassName", "BusinessObjects", AdditionalFields = "BOID", HttpMethod = "POST", ErrorMessage = "Class name already exists. Please enter a different Name.")]
        public string Class { get; set; }
        public int StatusTypeID { get; set; }
        [NotMapped]
        public List<VMDropDown> ddlBOs { get; set; }
    }
}
