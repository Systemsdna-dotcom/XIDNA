using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IIOServerDetailsRepository
    {
        VMCustomResponse SaveServerDetails(IOServerDetails model);
        DTResponse ServerDetailsList(jQueryDataTableParamModel param, int Type, int OrgID, int Category, string sDatabase);
        IOServerDetails EditServerDetails(int ID);
        VMCustomResponse SaveSMSServerDetails(IOServerDetails model);
        IOServerDetails EditSMSServerDetails(int ID);
        DTResponse SMSServerDetailsList(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase);
        List<VMDropDown> GetOrganizations(string sDatabase);
        DTResponse SpecificOrgIOServerDetailsGrid(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase);
        DTResponse SpecificOrgSMSServerDetailsGrid(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase);
    }
}
