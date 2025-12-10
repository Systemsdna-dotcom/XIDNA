using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XIDNA.ViewModels;

namespace XIDNA.ViewModels
{
   public class VMXISemantics
    {
        public int XISemanticID { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Enter Description")]
        public string sDescription { get; set; }
        public int StatusTypeID { get; set; }
    }
}
