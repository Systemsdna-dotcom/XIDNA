using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.Repository;

namespace XIDNA.Components
{
    public class TreeComponent
    {
        XIComponentsRepository oXIComRepo = new XIComponentsRepository();
        public int iUserID { get; set; }
        public string sDatabase { get; set; }
        public string sOrgName { get; set; }
        public List<cTreeView> XILoad(List<cNameValuePairs> oParams)
        {
            var BOLevels = oParams.Where(m => m.sName.ToLower().Contains("BOLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            var FKs = oParams.Where(m => m.sName.ToLower().Contains("FKLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            List<cNameValuePairs> oTreeParams = new List<cNameValuePairs>();
            oTreeParams.AddRange(BOLevels);
            oTreeParams.AddRange(FKs);
            iUserID = Convert.ToInt32(oParams.Where(m => m.sName == "iUserID").Select(m => m.sValue).FirstOrDefault());
            sDatabase = oParams.Where(m => m.sName == "sDatabase").Select(m => m.sValue).FirstOrDefault();
            var Nodes = oXIComRepo.GetTreeStructure(oTreeParams, iUserID, sOrgName, sDatabase);
            return Nodes;
        }
    }
}
