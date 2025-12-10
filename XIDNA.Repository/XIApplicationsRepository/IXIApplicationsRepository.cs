using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IXIApplicationsRepository
    {
        DTResponse GetXIApplicationsGrid(jQueryDataTableParamModel param);
        VMCustomResponse SaveXIApplication(cXIApplications oXIApp, int iUserID, string sOrgName, string sDatabase);
        cXIApplications GetXIApplicationByID(int XIAppID);
        VMCustomResponse SaveXIAppLogo(cXIApplications oXIApp);
        int AddOrgRoles(int orgid);

        #region XIUrlMapping
        DTResponse GetXIUrlMappingGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXIUrlMappingDetails(cXIUrlMappings model, int iUserID, string sOrgName, string sDatabase);
        cXIUrlMappings GetXIUrlMappingByID(int ID);
        bool IsExistsUrlMappingName(string sUrlName, int ID, string sDatabase);
        #endregion XIUrlMapping
    }
}
