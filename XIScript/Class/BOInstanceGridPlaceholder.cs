using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIScript
{
    public class BOInstanceGridPlaceholder
    {

        private Dictionary<string, CBOPlaceholder> oMyBOInstances = new Dictionary<string, CBOPlaceholder>();

        public Dictionary<string, CBOPlaceholder> oBOInstances
        {
            get
            {
                return oMyBOInstances;
            }
            set
            {
                oMyBOInstances = value;
            }
        }

        private string sMyName;

        public string sName
        {
            get
            {
                return sMyName;
            }
            set
            {
                sMyName = value;
            }
        }
    }
}