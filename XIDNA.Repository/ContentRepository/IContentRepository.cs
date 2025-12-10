using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;


namespace XIDNA.Repository
{
    public interface IContentRepository
    {
        VMCustomResponse PostContent(ContentEditors model, int iUserID, string sOrgName, string sDatabase);
        ContentEditors GetContent(int ID, string sDatabase, int iUserID, string sOrgName);
        List<VMNewLeads> GetUsersList(int Type, string sDatabase, int OrgID, int iUserID, string sOrgName);
        List<VMDropdowns> ContentList(string sDatabase, int OrgID, int iUserID, string sOrgName);
        //List<ContentEditors> GetContentList(string sDatabase);
        List<VMDropDown> LeadFieldsList(int BOID, string sDatabase);
        List<VMLeadEmail> GetLeadsData(string Users, List<string> Columns, string sDatabase, int iUseID, string sOrgName);
        List<VMLeadEmail> GetLead(int? LeadID, string Users, List<string> Columns, string sDatabase, int iUserID, string sOrgName);
        List<VMDropDown> Contentdropdown(int Category, string sDatabase, int OrgID, int iUserID, string sOrgName);
        int SaveOutbounds(Outbounds Content, string sDatabase, int OrgID, int iUserID, string sOrgName);
        bool IsExistsTitle(string Name, int ID, int Category, int OrgID, int iUserID, string sOrgName, string sDatabase);
        List<IOServerDetails> ServerDetails(int Type, int OrgID, string sDatabase);
        List<VMDropDown> GetOrgImages(int OrgID, int iUserID, string sOrgName, string sDatabase);
        DTResponse GetListOfTemplates(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase);
        Outbounds GetAttachment(int ID, string sDatabase, int iUserID, string sOrgName);
        List<VMDropDown> TypesList(string sDatabase);
        Organizations GetOrganizations(int ID, string sDatabase, int iUserID, string sOrgName);
    }
}
