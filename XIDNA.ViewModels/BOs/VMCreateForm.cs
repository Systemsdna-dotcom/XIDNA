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
    public class VMCreateForm
    {
        public string sBOName { get; set; }
        public List<List<string>> sBOAttrDetails { get; set; }
        public string sValue { get; set; }
        public List<List<VMDropDown>> sFKDropdwn { get; set; }
        public List<List<VMDropDown>> sOptionsDropdwn { get; set; }
        //public List<string> sDefaultValues { get; set; }
        public string FormName { get; set; }

    }
}
