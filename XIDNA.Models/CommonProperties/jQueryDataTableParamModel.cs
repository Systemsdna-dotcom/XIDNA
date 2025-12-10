using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class jQueryDataTableParamModel
    {
        /// <summary>
        /// Request sequence number sent by DataTable,
        /// same value must be returned in response
        /// </summary>       
        public string sEcho { get; set; }

        /// <summary>
        /// Text used for filtering
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Number of records that should be shown in table
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// First record that should be shown(used for paging)
        /// </summary>
        public int iDisplayStart { get; set; }

        /// <summary>
        /// Number of columns in table
        /// </summary>
        public int iColumns { get; set; }

        /// <summary>
        /// Number of columns that are used in sorting
        /// </summary>
        public int iSortingCols { get; set; }

        /// <summary>
        /// Comma separated list of column names
        /// </summary>
        public string sColumns { get; set; }

        public int iSortCol { get; set; }

        public string sSortDir { get; set; }
        public string EmailID { get; set; }
        public string ClientID { get; set; }
        public int OrganizationID { get; set; }
        public List<int> Orgs { get; set; }
        public string Fields { get; set; }
        public string Optrs { get; set; }
        public string Values { get; set; }
        public string SearchText { get; set; }
        public string Type { get; set; }
        public bool bFromBO { get; set; }
        public string NVPairs { get; set; }
        public string sParentWhere { get; set; }
    }
}
