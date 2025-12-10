using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMViewPopup
    {
        public int? TabID { get; set; }
        public int LeadID { get; set; }
        public int PopupID { get; set; }
        public int ReportID { get; set; }
        public int? ClassID { get; set; }
        public string UserName { get; set; }
        public string TabName { get; set; }
        public string Name { get; set; }
        public List<VMDropDown> Users { get; set; }
        public int Tab1ClickID { get; set; }
        public int BOID { get; set; }
        public List<VMDropDown> Tabs { get; set; }
        public int QuoteID { get; set; }
        public int OrganizationID { get; set; }
        public int UserID { get; set; }
        public string FormType { get; set; }
        public string PopType { get; set; }
        public int StageID { get; set; }
        public string Database { get; set; }
        public int RowID { get; set; }
        public int ClientID { get; set; }
        public bool IsLeftMenu { get; set; }
        public int LayoutType { get; set; }
        public int DailogID { get; set; }
        public Guid FKi1ClickIDXIGUID { get; set; }
    }
}
