using XIDNA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IDataRepository
    {
        List<VMDropDown> DataClassList(string database, int OrgID);
        List<VMDropDown> SourceGroupList(string database);
        List<VMDropDown> SourceList(string database);
        int CreateData(List<FormData> Values, string database, int OrgID);
        List<VMLeadsList> GetLeadsList(int OrganizatioID, string database);
        List<UserLeads> GetAssignedLeadsList(int? UserID, int OrganizatioID, string database);
        List<VMDropDown> GetUsersbyOrgID(int OrganizationID, string database);
        List<UserLeads> GetLeadsbyOrgID(int TeamID, int? ClassID, int OrganizationID, string database);
        int AssignUsersToLeads(List<int> selectedleads, int userid, string database, int orgid);
        List<Classes> GetClasses(int OrgID, string sDatabase);
        int ImportLeads(int OrgID, string database);
    }
}
