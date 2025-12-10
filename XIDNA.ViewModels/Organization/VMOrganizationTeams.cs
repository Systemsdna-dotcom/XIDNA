using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMOrganizationTeams
    {
        public string Role { get; set; }
        public List<string> Users { get; set; }
        public List<int> UserIDs { get; set; }
    }
}
