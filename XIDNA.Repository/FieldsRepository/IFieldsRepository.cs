using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IFieldsRepository
    {
        int SaveField(AddFields model, string sDatabase, int iUserID, string sOrgName);
        VMCustomResponse SaveOrgField(MappedFields model, string sDatabase, int iUserID, string sOrgName);
        VMCustomResponse SaveOrgEditedField(MappedFields model, string sDatabase, int iUserID, string sOrgName);
        DTResponse GetFieldsList(jQueryDataTableParamModel param, string UserName, string sDatabase, int OrgID, int iUserID, string sOrgName);
        DTResponse GetOrgNonClassFieldsList(jQueryDataTableParamModel param, string UserName, string sDatabase, int OrgID, int iUserID, string sOrgName);
        AddFields EditField(int ColumnID, int OrgID, string sDatabase, int iUserID, string sOrgName);
        List<Classes> GetClasses(int OrgID, string sDatabase, int iUserID, string sOrgName);
        List<VMDropDown> GetAllOrgClasses(int OrgID, string sDatabase, int iUserID, string sOrgName);
        List<VMDropDown> GetSubscriptions(int OrgID, string sDatabase, int iUserID, string sOrgName);
        MappedFields EditOrgField(int ColumnID, int OrgID, string sDatabase, int iUserID, string sOrgName);
        string GetOrganization(int OrgID, string sDatabase);
        bool IsExistsFieldName(string FieldName, string sDatabase, int ID, int ClassID, int OrgID, int iUserID, string sOrgName);
        DTResponse GetSelectedFields(jQueryDataTableParamModel param, int ID, string sDatabase, int OrgID);
        DTResponse GetSelectedOrgFields(jQueryDataTableParamModel param, int ID, string sDatabase, int OrgID, int iUserID, string sOrgName);
        DTResponse GetNonSelectedField(jQueryDataTableParamModel param, string sDatabase, int OrgID);
        DTResponse GetOrgNonSelectedField(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName);
        bool IsLengthChangable(string FieldName, int BOID, string sDatabase, int OrgID, string CreationType, int iUserID, string sOrgName);
        bool IsTypeChangable(string FieldName, int BOID, string sDatabase, int OrgID, string CreationType, int iUserID, string sOrgName);
        DTResponse GetOrgClassFields(jQueryDataTableParamModel param, string Type, int ClassID, string sDatabase, int OrgID, int iUserID, string sOrgName);
        DTResponse DisplayOrgMappedFields(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName);
        List<VMDropDown> GetOrgSubscriptions(int OrgID, string sDatabase, int iUserID, string sOrgName);
        LeadMappings ViewOrgMappedFields(int ClassID, string sDatabase, int OrgID, int iUserID, string sOrgName);
        VMLeadMappings GetOrgLeadFields(int ClassID, string Type, string Category, int OrgID, string sDatabase, int iUserID, string sOrgName);
        VMCustomResponse SaveOrgMappedLeadFields(int ClassID, string LeadField, string OrgField, int MasterID, int OrgID, string sDatabase, int iUserID, string sOrgName);
        DTResponse OrgMappedFieldsGrid(jQueryDataTableParamModel param, int ClassID, string Type, string sDatabase, int OrgID, int iUserID, string sOrgName);
        int DeleteMappedField(int ID, string sDatabase, int iUserID, string sOrgName);
        DTResponse DisplayFields(jQueryDataTableParamModel param, string sDatabase, int OrgID, int iUserID, string sOrgName);
        VMLeadMappings GetLeadFields(int ClassID, string Type, int OrgID, string sDatabase);
        LeadMappings MappedFieldsGrid(LeadMappings model, int ClsID, string sDatabase, int OrgID);
        bool IsExistsTemplateTitle(string Type, string Name, int ID, string sDatabase);
        string SaveMappedLeadFields(string Name, int ClsID, string DataField, string ColumnField, int OrgID, string sDatabase);
        List<VMDropDown> GetClassTypes(int OrgID, string sDatabase);
        DTResponse GetMappedMasterFieldsList(jQueryDataTableParamModel param, int ClassID, string sDatabase);
        int DeleteMasterMappedField(int ID, string sDatabase);
        List<VMDropDown> GetAllMasterTypes(string sDatabase);
    }
}
