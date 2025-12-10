using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
namespace XIDNA.Repository
{
    public interface IBusinessObjectsRepository
    {
        DTResponse GetBusinessObjects(jQueryDataTableParamModel model, int iUserID, string sOrgName, string sDatabase);
        List<BOFields> GetBOFields(int BOID, int OrgID, int iUserID, string sOrgName, string sDatabase);
        int SaveBO(BOs model, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveExtractedBO(BOs model, int iUserID, string sOrgName, string sDatabase);
        BOs GetBOByID(int ID, string sDatabase);
        int AddBOAttributes(List<BOFields> model, string sDatabase, int iUserID);
        List<BOs> GetAllBos(int iUserID, string sOrgName, string sDatabase);
        int AddAttributeGroup(BOGroupFields model, string sDatabase, int iUserID, string sOrgName);
        DTResponse GetAttributeGroups(jQueryDataTableParamModel model, int BOID, string sDatabase);
        BOAttributeVIewModel GetBOFieldsByID(int BOID, string sDatabase);
        bool IsExistsGroupName(string GroupName, int BOGroupId, int BOID, string sDatabase);
        bool IsExistsBOName(string Name, int ID, string sDatabase);
        BOAttributeVIewModel EditBOAttributeGroup(int GroupID, int BOID, string sDatabase);
        int RemoveGroup(int GroupID, string sDatabase);
        int CreateAttributeForField(List<string> values, List<string> Checkbox, string FieldName, string sDatabase);
        int CreatePopUpItems(VMWherePopUP model, string sDatabase);
        BOFields GetPopUpDataByID(int FieldID, string sDatabase);
        int SaveScript(int ID, string Script, string ExecutionType, string sDatabase);
        List<string> ValidateScript(string Script);

        //2/12/2017
        BOFields GetBOAtrributesForm(string FieldName, int BOID, int iUserID, string sOrgName, string sDatabase);
        int SaveBoOptionList(string Values, int BOID, int iID, string AtrName, string sDatabase);
        VMCustomResponse SaveFormBOAttributes(BOFields model, int iUserID, string sOrgName, string sDatabase);
        int DeleteBoOptionList(int BOID, int iID, string AtrName, string sDatabase);
        bool CheckBoOptionList(int BOID, string AtrName, string sDatabase);
        List<List<string>> EditBoOptionList(int BOID, string AtrName, string sDatabase);
        Dictionary<string, string> GetOneClicks(string sDatabase);
        //int CreateTableWithDetails(CreateTable model, string database, int CreatedByID, string CreatedByName);

        //15/12/2017
        int DeleteAttribute(int BOID, string AttrName, int iUserID, string sOrgName, string sDatabase);
        BOs CopyBOByID(int ID, int iUserID, string sOrgName, string sDatabase);

        //18/12/2017
        VMCustomResponse CreateTableFromBO(BOs model, int OrgID, int CreatedByID, string CreatedByName, int UserID, string sOrgName, string sDatabase);

        int DeleteBO(int BOID, string sDatabase);
        List<VMDropDown> GetHelpItems(string sDatabase);

        #region DynamicForm
        int SaveFormData(List<FormData> FormValues, string sTableName, int iUserID, string sOrgName, string sDatabase);
        List<string> GetDefaultValues(string sAttrNames, string BOName, string sDatabase);
        VMCreateForm CreateDynamicForm(int XiLinkID, int iUserID, string sOrgName, string sDatabase);
        #endregion DynamicForm

        #region Scripts

        DTResponse GetBOScriptsList(jQueryDataTableParamModel model, int BOID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveBOScript(BOScripts model, int iUserID, string sOrgName, string sDatabase);
        BOScripts GetScriptByID(int ID, string sDatabase);
        long CopyScript(int ID, int iUserID, string sOrgName, string sDatabase);

        #endregion Scripts

        #region BOClassAttributes

        DTResponse GetClassAttributesGrid(jQueryDataTableParamModel model, int BOID, string sDatabase);
        VMCustomResponse SaveBOClassAttibute(BOClassAttributes model, string sDatabase);
        BOClassAttributes EditClassAttribute(int ID, string sDatabase);
        bool IsExistsClassName(string Name, int ID, string sDatabase);

        #endregion BOClassAttributes        

        #region ImportBO

        string ImportBOInXML(string sFilePath, int iUserID, string sOrgName, string sDatabase);

        #endregion ImportBO

        #region EditableGrid
        int DeleteFormData(int iBOID, string BOName, string Group, int iInstanceID, string sVisualisation, int iUserID, string sOrgName, string sDatabase, List<cNameValuePairs> nWCParams);
        #endregion

        #region Data Source
        DTResponse GetDataSource(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        XIDataSources GetDataSourceDetails(int ID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse CreateDataSource(XIDataSources model, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetDataSources(int iUserID, string sOrgName, string sDatabase);
        string CheckConnectionString(string sConnectionString);
        List<VMDropDown> GetAppOrganisations(int iAppID);

        #endregion Data Source

        #region XIStructure
        DTResponse GetBOStructuresList(jQueryDataTableParamModel model, int BOID, string sDatabase);
        List<cXIStructure> GetBOStructureTree(int iBOID, string sDatabase);
        VMCustomResponse SaveBOStructure(List<cXIStructure> model, int iStructureID, string sType, int iUserID, string sDatabase);
        List<cXIStructure> SaveBODetailsToXIStructure(int iBOID, string BOName, int iUserID, string sOrgName, string sDatabase);
        List<cXIStructure> GetXIStructureTreeDetails(int BOID, string iStructureID, string sDatabase);
        long CreateAndSaveTreeNode(string ParentNode, string NodeID, string NodeTitle, int iUserID, int OrgID, string sDatabase);
        List<cXIStructure> DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type, int UserID, int OrgID, string sDatabase);
        int RenameTreeNode(string ParentNode, string NodeID, string NodeTitle, int UserID, int OrgID, string sDatabase);
        cXIStructure AddDetailsForStructure(string ParentNode, string NodeID, int OrgID, int UserID, string sDatabase);
        VMCustomResponse SaveAddedDetails(int UserID, cXIStructure model, string sDatabase);
        cBOUIDetails GetBOUIDetails(int ID, string sDatabase);
        VMCustomResponse SaveBOUIDetails(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveBOUIDetailsWithQuestionSet(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveBOUI(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveBODefaults(cBOUIDefaults model, string sDatabase);
        cBOUIDefaults GetBODefaults(int iBOID, int iUserID, string sOrgName, string sDatabase);
        int DragAndDropNodes(string NodeID, string OldParentID, int UserID, string sDatabase, int Oldposition, int Newposition);
        #endregion XIStructure

        #region Audit
        string CreateAuditTable(int BOID, int iOrgID, int iUserID, string sOrgName, string sDatabase, string CreatedByName);
        #endregion Audit
    }
}
