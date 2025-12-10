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
    public class VMXISemanticsSteps
    {
        public int XIStepID { get; set; }
        public int FKiXISemanticID { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string sName { get; set; }
        [Required(ErrorMessage = "Please Select Order")]
        public int iOrder { get; set; }
        [Required(ErrorMessage = "Please Enter Code")]
        public string sCode { get; set; }
        [Required(ErrorMessage = "Please Select DisplayAs")]
        public int iDisplayAs { get; set; }
        public int StatusTypeID { get; set; }
        public int XILinkID { get; set; }
        public int FKiContentID { get; set; }
        public List<VMDropDown> ddlContent { get; set; }
        public List<VMDropDown> ddlXILinks { get; set; }
        public int XIComponentID { get; set; }
        public List<VMDropDown> ddlXIComponents { get; set; }
        public Dictionary<string, string> XIFields { get; set; }
    }
}
