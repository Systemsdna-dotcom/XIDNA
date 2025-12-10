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
    public class VMXISemanticsNavigations
    {
        public int XINavigationID { get; set; }
        public int FKiXIStepID { get; set; }
        [Required(ErrorMessage = "Please enter a name")]
        public string sName { get; set; }
        [Required(ErrorMessage="Please Select Type")]
        public int iType { get; set; }
        public int iNextStepID { get; set; }
        [Required(ErrorMessage = "Please Enter Order")]
        public int iOrder { get; set; }
        [Required(ErrorMessage = "Please Enter Function")]
        public string sFunction { get; set; }
        public string sField { get; set; }
        public string sOperator { get; set; }
        public string sValue { get; set; }
        public int StatusTypeID { get; set; }
        public List<VMDropDown> SematicSteps { get; set; }
        public List<VMDropDown> XISemaEdit { get; set; }
    }
}
