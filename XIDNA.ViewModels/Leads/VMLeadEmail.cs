using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMLeadEmail
    {
        public string Email { get; set; }
        public List<List<string>> Data { get; set; }
        public List<string> Columns { get; set; }
        public string Content { get; set; }
    }
}
