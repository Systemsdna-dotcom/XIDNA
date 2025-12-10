using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIDNA.Providers
{
    public class TOTPAccountDto 
    {
        public string QRCodeUrl { get; set; } 

        public string ManualAccountKey { get; set; }
    }
}