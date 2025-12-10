using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMPopupLayoutMappings
    {
        public int PopupLayoutID { get; set; }
        public Guid PopupLayoutIDXIGUID { get; set; }
        public int XiLinkID { get; set; }
        public Guid XiLinkIDXIGUID { get; set; }
        public string sXiLinkID { get; set; }
        public int PlaceHolderID { get; set; }
        public Guid PlaceHolderIDXIGUID { get; set; }
        public string AreaName { get; set; }
        public int PopupID { get; set; }
        public Guid DialogIDXIGUID { get; set; }
        public int DialogID { get; set; }
        public string TdID { get; set; }
        public int LayoutID { get; set; }
        public string PlaceholderName { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public string HTMLCode { get; set; }
        public int StatusTypeID { get; set; }
        public string IsValueSet { get; set; }
        public int MappingID { get; set; }
        public int ID { get; set; }
        public Guid XIGUID { get; set; }
    }
}
