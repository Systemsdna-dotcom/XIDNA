using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMViewRecord
    {
        public string ActionPopUp { get; set; }
        public int LeadID { get; set; }
        public List<SectionsData> SectionsData { get; set; }
        public int QueryID { get; set; }
        public string Query { get; set; }
        public List<SingleBOField> SingleBOField { get; set; }
        public string PreviewType { get; set; }
        public int Rank { get; set; }
        public int ReportID { get; set; }
        public VMResultList ResultList { get; set; }
    }
}
