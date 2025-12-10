using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IUsersRepository
    {
        DTResponse GetUsersList(jQueryDataTableParamModel model, int OrganizationID, string sDatabase);
        DTResponse GetRolesList(jQueryDataTableParamModel model, int OrganizationID, string sDatabase);
        //Users GetUser(int id);
        int UpdateUser(cXIAppUsers model, string sDatabase);
        cXIAppRoles AddRole(cXIAppRoles model, string sDatabase);
        List<VMDropDown> GetReportToUsers(string ID, int UserID, string sDatabase);
        bool IsExistsRoleName(string RoleName, int Id, int OrgID, string sDatabase);
        bool IsExistsEmpEmail(string Email, string Type, int ID, string sDatabase);
        List<VMDropDown> GetOrgLocation(int OrgID, int iUserID, string sOrgName, string sDatabase);
        int DeleteRole(int RoleID, int OrgID, string sDatabase);
        bool GetNoOfUsers(int OrgID, string sDatabase);
        cXIAppRoles GetRoleByID(int ID, string sDatabase);
        cXIAppUsers GetUserDetails(int UserID, string sDatabase);
        List<VMDropDown> GetLayoutsList(string sDatabase);
        List<VMDropDown> GetThemesList(string sDatabase);
    }
}
