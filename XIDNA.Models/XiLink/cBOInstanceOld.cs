using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class cBOInstanceOld
    {
        public List<cNameValuePairs> NVPairs { get; set; }
        public BOs Definition { get; set; }
        public XiVisualisations Visualisations { get; set; }
    }

    public class cNameValuePairsOld
    {
        public string sName { get; set; }
        public string sValue { get; set; }
        public bool bDirty { get; set; }
        public string sGroupValue { get; set; }
        public string sHelptext { get; set; }
        public string sNarrowBar { get; set; }
        //bDirty
        //bLoaded
        //sOldValue
    }
}
