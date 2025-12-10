using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    public class cBODisplay
    {
        public cBOInstance BOInstance { get; set; }
        public List<cVisualisations> Visualisations { get; set; }
    }

    public class cBOInstance
    {
        public List<cNameValuePairs> NVPairs { get; set; }
        public BOs Definition { get; set; }
        public string sScripts { get; set; }
        //Get Script
        public string Action_Execute(int iBOID, int iInstanceID, string sBOGroup, string sActionName, string sDatabase, string sOrgDB)
        {
            List<string[]> Rows = new List<string[]>();
            //ModelDbContext dbContext = new ModelDbContext(sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            var BO = dbContext.BOs.Where(m => m.BOID == iBOID).FirstOrDefault();
            string sScript = BO.BOScripts.Where(m => m.sName == sActionName).Select(m => m.sScript).FirstOrDefault();
            return sScript;
        }
    }

    public class cNameValuePairs
    {
        public string sName { get; set; }
        public string sValue { get; set; }
        public bool bDirty { get; set; }
        public string sGroupValue { get; set; }
        public string sType { get; set; }
        public string sContext { get; set; }
        public bool bLock { get; set; }
        public bool bLoad { get; set; }
        public List<cNameValuePairs> nSubParams { get; set; }
        //bDirty
        //bLoaded
        //sOldValue
    }

    public class cVisualisations
    {
        public string sName { get; set; }
        public string sValue { get; set; }
        public string sAttribute { get; set; }
        public int iDrillDownXiLinkID { get; set; }
        public string sPreviewGroup { get; set; }
        public string sResolveGroup { get; set; }
        public string sHelpText { get; set; }
        public string sNarrowBar { get; set; }
        public string sPreviewData { get; set; }
    }

    public class cOptionaList
    {
        public string sOptionName { get; set; }
        public string sOptionValue { get; set; }
    }
}
