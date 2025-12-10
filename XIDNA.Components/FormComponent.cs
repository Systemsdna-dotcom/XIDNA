using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.Repository;

namespace XIDNA.Components
{
    public class FormComponent
    {
        CXiAPI oXIAPI = new CXiAPI();
        public string sBOName { get; set; }
        public string sGroupName { get; set; }
        public int iInstanceID { get; set; }
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }

        public cBODisplay XILoad(List<cNameValuePairs> oParams)
        {
            cBODisplay oBODisplay = new cBODisplay();
            //First set all properties by extracting from oParams
            sBOName = oParams.Where(m => m.sName == "BO").Select(m => m.sValue).FirstOrDefault();
            sGroupName = oParams.Where(m => m.sName == "Group").Select(m => m.sValue).FirstOrDefault();
            iInstanceID = Convert.ToInt32(oParams.Where(m => m.sName == "FC_InstanceID").Select(m => m.sValue).FirstOrDefault());
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            if (!string.IsNullOrEmpty(sBOName))
            {
                oBODisplay = oXIAPI.GetFormData(sBOName, sGroupName, iInstanceID, string.Empty, iUserID, sOrgName, sDatabase, null);
            }
            return oBODisplay;
        }
    }
}
