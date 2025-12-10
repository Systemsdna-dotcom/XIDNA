using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class DashBoardGraphs
    {
        public string color { get; set; }
        public string highlight { get; set; }
        public string label { get; set; }
        public int value { get; set; }
        public List<VMDropDown> ClassDDL { get; set; }
        public List<VMDropDown> DateDDL { get; set; }
        public string QueryName { get; set; }
        public string ShowAs { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public int OnClickResultID { get; set; }
    }
}
