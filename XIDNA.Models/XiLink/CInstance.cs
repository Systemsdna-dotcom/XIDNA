using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class CInstance
    {
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sType { get; set; }
        public MethodInfo Script { get; set; }
        public List<cNameValuePairs> Registers { get; set; }
        public List<cNameValuePairs> nSubParams { get; set; }
        public Dictionary<string, CInstance> NMyInstance = new Dictionary<string, CInstance>(StringComparer.InvariantCultureIgnoreCase);

        public CInstance NInstance(string sKey)
        {
            CInstance oInstance;


            if (NMyInstance.ContainsKey(sKey))
            {
                oInstance = NMyInstance[sKey];
            }
            else
            {
                oInstance = new CInstance();
                NMyInstance.Add(sKey, oInstance);
            }
            
            return oInstance;
        }

        //method called NInstance
        //what it does is, look in the dict NMyInstance for this key, if not exists then add it in

    }

    // CInstance   oInstance =new CInstance()
    //oInstance.NInstance("BOs").NInstance("Policy").sName="My Pol";
}


