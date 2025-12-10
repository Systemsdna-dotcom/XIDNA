using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.ViewModels
{
    public class VMKeys
    {
        public string Key { get; set; }
    }
    public class VMForeginKeys : List<VMKeys>
    {
        public VMForeginKeys()
        {
            this.Add(new VMKeys() { Key = "FKiLeadClassID" });
            this.Add(new VMKeys() { Key = "iTeamID" });
            this.Add(new VMKeys() { Key = "FKiSourceID" });
            this.Add(new VMKeys() { Key = "iStatus" });
        }
    }
}
