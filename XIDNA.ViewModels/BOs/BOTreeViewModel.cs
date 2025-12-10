using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class BOTreeViewModel
    {
        public string TableName { get; set; }
        public IEnumerable<string> ColumnName { get; set; }
        public List<string> DataType { get; set; }
        public IEnumerable<string> MaxLength { get; set; }
    }
}
