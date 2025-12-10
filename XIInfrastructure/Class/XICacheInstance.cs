using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using XICore;
using XISystem;

namespace XIInfrastructure
{
    public class XICacheInstance
    {
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sType { get; set; }
        public MethodInfo Script { get; set; }
        public List<CNV> Registers { get; set; }
        public List<CNV> nSubParams { get; set; }
        public XIBOInstance oBOInstance { get; set; }
        public Dictionary<string, XICacheInstance> NMyInstance = new Dictionary<string, XICacheInstance>(StringComparer.InvariantCultureIgnoreCase);

        public XICacheInstance NInstance(string sKey)
        {
            XICacheInstance oInstance;


            if (NMyInstance.ContainsKey(sKey))
            {
                oInstance = NMyInstance[sKey];
            }
            else
            {
                oInstance = new XICacheInstance();
                NMyInstance.Add(sKey, oInstance);
            }

            return oInstance;
        }
        //method called NInstance
        //what it does is, look in the dict NMyInstance for this key, if not exists then add it in

        public void RemoveUserCache(string sSessionId)
        {
            if (!string.IsNullOrEmpty(sSessionId))
            {
                sSessionId = "SS_" + sSessionId;
                if (NMyInstance.ContainsKey("XISession"))
                {
                    if (NMyInstance["XISession"].NMyInstance.ContainsKey(sSessionId))
                    {
                        NMyInstance["XISession"].NMyInstance.Remove(sSessionId);
                    }
                }
            }
        } // Method is used to Remove Logoff User Cache

    }
}