using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XISystem
{
    public class CDataTable
    {
        public List<string> FileHeadings { get; set; }
        public List<string> TransactionHeadings { get; set; }
        public List<List<string>> FileRows { get; set; }
        public List<List<string>> TransactionRows { get; set; }
        public List<string> FileDBColumns { get; set; }
        public List<string> TransactionDBColumns { get; set; }
    }
}