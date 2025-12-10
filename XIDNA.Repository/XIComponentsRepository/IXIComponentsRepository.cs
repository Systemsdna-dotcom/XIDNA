using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IXIComponentsRepository
    {
        DTResponse XiComponentsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        cXIComponents GetXiComponentsByID(int ComponentID, string sName, string sType, int ID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXiComponents(VMXIComponents model, int iUserID, string sOrgName, string sDatabase);
        cXIComponents XIInitialise(int iXIComponentID, int iUserID, string sOrgName, string sDatabase);
        List<cTreeView> GetTreeStructure(List<cNameValuePairs> oTreeParams, int iUserID, string sOrgName, string sDatabase);
        string SaveLayoutComponentParams(cXIComponents oComponent, string sType, int iLoadID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse UpdateMappingIDToParams(string sType, int iLoadID, string Params, int iUserID, string sOrgName, string sDatabase);
        cBODisplay GetFormComponent(cXIComponents oComponent, int iUserID, string sOrgName, string sDatabase);
        cBOUIDetails GetBOStructure1Click(string StructureName, int BODID);
        List<cXIComponentParams> GetComponentParmsByStep(int StepID);
    }
}
