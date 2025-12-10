using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMTabContent
    {
        public VMResultList ResultList { get; set; }
        public VMViewRecord ViewRecord { get; set; }
        public VMBespoke Bespoke { get; set; }
        public VMKPICircle KPICircle { get; set; }
        public VMChart PieChart { get; set; }
        public LineGraph LineGraph { get; set; }
        public LineGraph BarGraph { get; set; }
        public VMViewPopup Popup { get; set; }
    }
}
