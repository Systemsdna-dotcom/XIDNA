using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMPopupLayout
    {
        public int PopupID { get; set; }
        public int DialogID { get; set; }
        public int iInstanceID { get; set; }
        public string PopupName { get; set; }
        public string DialogName { get; set; }
        public int LayoutID { get; set; }
        public string LayoutName { get; set; }
        public string LayoutCode { get; set; }
        public string LayoutType { get; set; }
        public List<VMDropDown> XiLinks { get; set; }
        public List<VMDropDown> XIComponents { get; set; }
        public List<VMDropDown> Popups { get; set; }
        public List<VMPopupLayoutMappings> Mappings { get; set; }
        public List<VMPopupLayoutDetails> Details { get; set; }
        public int XiParameterID { get; set; }
        public int StatusTypeID { get; set; }
        public string sGUID { get; set; }
        public string sNewGUID { get; set; }
        public string LayoutLevel { get; set; }
        public string Authentication { get; set; }
        public int iThemeID { get; set; }
        public string sThemeName { get; set; }
        public string sContext { get; set; }
        public int FKiApplicationID { get; set; }
        public List<VMDropDown> Steps { get; set; }
        public bool bUseParentGUID { get; set; }
        public string sSiloAccess { get; set; }
        public bool bIsTaskBar { get; set; }
        public string sTaskBarPosition { get; set; }
        public List<string> arrSiloAccess { get; set; }
    }

    public class VMPopupLayoutDetails
    {
        public int PlaceHolderID { get; set; }
        public Guid PlaceHolderIDXIGUID { get; set; }
        public int LayoutID { get; set; }
        public Guid LayoutIDXIGUID { get; set; }
        public string PlaceholderName { get; set; }
        public string PlaceholderArea { get; set; }
        public string PlaceholderUniqueName { get; set; }
        public string TDClass { get; set; }
        public string sCssClass { get; set; }
    }
}
