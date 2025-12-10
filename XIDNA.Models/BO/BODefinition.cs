using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class BODefinition
    {
        public BOs BODetails { get; set; }
        public List<BOFields> BOFields { get; set; }
        public List<BOFields> BOFieldsWithOpt { get; set; }
        public List<BOGroupFields> BOGroups { get; set; }
    }
}
