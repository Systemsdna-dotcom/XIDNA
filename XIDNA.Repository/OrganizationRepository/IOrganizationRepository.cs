using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.SqlClient;
using XIDNA.ViewModels;
using System.Text.RegularExpressions;

namespace XIDNA.Repository
{
    public interface IOrganizationRepository
    {
        DTResponse GetOrganizationList(jQueryDataTableParamModel model, string database);
        int AddOrganization(VMOrganization model, string CommonDB, string database);
        VMOrganization GetOrganizationDetails(int orgid, string database);
        int AddOrganizationContact(VMOrganization model, string database);
        int UpdateOrganizationContact(OrganizationContacts model, int iUserID, string sOrgName, string database);
        VMOrganization GetOrgContactDetails(int organizationid, int iUserID, string sOrgName, string database);
        int AddOrgRoles(int orgid, string database);
        int SaveLogo(VMOrganization model, string database);
        bool IsExistsOrgName(string Name, string Type, int ID, string database);
        bool IsExistsOrgEmail(string Email, string Type, int ID, string OldEmail, string database);
        int GetUserID(VMOrganization model, string database);
        List<VMDropDown> GetOrgNames(string database);
        int SaveOrgDetails(VMOrganization model, string database);
        VMCustomResponse SaveOrgLocDetails(OrganizationLocations model, string database);
        DTResponse SpecificOrgLocationList(jQueryDataTableParamModel param, int LocOrgID, string database);
        DTResponse SpecificOrgLocationGrid(jQueryDataTableParamModel param, int LocOrgID, string database);
        DTResponse DisplayOrganizations(jQueryDataTableParamModel param, int LocOrgID, int iUserID, string sOrgName, string database);
        OrganizationLocations EditOrgLocation(int ColumnID, int OrgID, string database);
        OrganizationLocations DeleteOrgLocation(int ColumnID, int OrgID, string database);
        bool IsExistsOrgLocation(string Location, int OrganizationID, int ID, string database);
        bool IsExistsLocationCode(string Location, int OrganizationID, int ID, string database);
        List<VMDropDown> GetClasses(string database);
        int AddOrganizationClasses(VMOrganization model, int OrgID, string database);
        VMOrganization GetOrganizationClasses(int OrgID, int iUserID, string sOrgName, string database);
        DTResponse GetOrgSources(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string database);
        List<VMDropDown> GetProviders(string database);
        List<VMDropDown> GetSourceTypes(string database);
        VMCustomResponse SaveSource(VMOrganizationForms Source, string database);
        VMOrganizationForms EditOrgSource(int SourceID, int OrgID, string database);
        DTResponse GetOrgSubscriptions(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string database);
        List<VMDropDown> GetOrgSources(int OrgID, string database);
        List<VMDropDown> GetOrgClasses(int OrgID, string database);
        List<VMDropDown> GetSourceTypes(int SourceID, int OrgID, string database);
        List<VMDropDown> GetSourceClasses(int SourceID, int OrgID, string database);
        VMCustomResponse SaveSubscription(VMOrganizationForms model, string database);
        DTResponse SpecificSubscriptionsList(jQueryDataTableParamModel param, int OrgID, int SourceID, string database);
        bool IsExistsSourceName(string Name, int ID, int OrganizationID, string database);
        DTResponse SpecificSourcesList(jQueryDataTableParamModel param, int OrgID, string database);
        //List<VMDropDown> GetProviders(int OrgID, int SourceID, string Type, int Clas);
        string AddSourceFields(VMOrganizationForms model, int OrgID, string database);
        DTResponse GetOrgSourceFields(jQueryDataTableParamModel param, int OrgID, string database);
        List<VMDropDown> GetSubscriptions(string database, int OrgID);
        int DeleteSourceField(int ID, int OrgID, string database);
        DTResponse GetOrgSpecSourceFields(jQueryDataTableParamModel param, int OrgID, string SubID, string database);
        int SaveOtherImages(OrganizationImages model, int OrgID, string database);
        List<OrganizationImages> GetOrgImages(int OrgID, string database);
        OrganizationImages DeleteLogo(int ID, int iUserID, string sOrgName, string database);
        DTResponse OrganizationImagesGrid(jQueryDataTableParamModel param, int OrgID, string database);
        bool IsExistsSrcEmail(string Email, string Type, int ID, string OldEmail, string database);
        OrganizationTeams GetUsersList(int OrgID, string database);
        VMCustomResponse SaveOrganizationTeams(OrganizationTeams model, string[] SelectedUsers, string[] UserIDs, int OrgID, string database);
        DTResponse OrgTeamsGrid(jQueryDataTableParamModel param, int OrgID, string database);
        OrganizationTeams EditOrgTeams(int ID, int OrgID, string database);
        bool IsExistsTeamName(string Name, int ID, int OrgID, string database);
        OrganizationSubscriptions GetOrgSubscriptionDetails(int ID, int OrgID, string database);
        VMCustomResponse SaveSubscriptionColumns(VMOrganizationForms model, string database);
        DTResponse SpecificSubCoulmnsList(jQueryDataTableParamModel param, string SubID, int OrgID, string database);
        List<VMDropDown> GetColumnsList(string database);
        List<VMDropDown> GetLocationCodesList(int OrganizationID, string database);
        SubscriptionColumns OrgColumnDetails(string Name, string SubscriptionID, int OrgID, string database);
        int ChangeTheme(int ID, int OrgID, string database);
    }
}
