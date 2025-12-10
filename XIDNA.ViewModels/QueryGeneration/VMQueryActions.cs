using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMQueryActions
    {
        public int QueryID { get; set; }
        public int InnerReportID { get; set; }
        public bool IsRowClick { get; set; }
        public string OnRowClickType { get; set; }
        public int OnRowClickValue { get; set; }
        public bool IsColumnClick { get; set; }
        public string OnClickColumn { get; set; }
        public string OnClickParameter { get; set; }
        public string OnColumnClickType { get; set; }
        public int OnColumnClickValue { get; set; }
        public bool IsCellClick { get; set; }
        public string OnClickCell { get; set; }
        public string OnCellClickType { get; set; }
        public int OnCellClickValue { get; set; }
        public bool IsRowTotal { get; set; }
        public bool IsColumnTotal { get; set; }
        public string EditableFields { get; set; }
        public List<VMDropDown> LeftEditFields { get; set; }
        public List<VMDropDown> RightEditFields { get; set; }

        public int RowXiLinkID { get; set; }
        public string sRowXiLinkType { get; set; }
        public int ColumnXiLinkID { get; set; }
        public int CellXiLinkID { get; set; }

        public bool IsCreate { get; set; }
        public bool IsEdit { get; set; }
        public bool IsDelete { get; set; }
        public string CreateScript { get; set; }
        public string EditScript { get; set; }
        public string DeleteScript { get; set; }
        public int CreateRoleID { get; set; }
        public int EditRoleID { get; set; }
        public int DeleteRoleID { get; set; }
        public int CreateGroupID { get; set; }
        public int EditGroupID { get; set; }
        public int DeleteGroupID { get; set; }
        public int iLayoutID { get; set; }
        public int iCreateXILinkID { get; set; }
        public string sAddLabel { get; set; }
        public string sCreateType { get; set; }
        public bool IsRefresh { get; set; }
        public bool bIsCopy { get; set; }
        public bool bIsView { get; set; }
        public bool bIsCheckbox { get; set; }
        public bool bIsExport { get; set; }
        public string sFileExtension { get; set; }
        public Guid XIGUID { get; set; }
    }
}
