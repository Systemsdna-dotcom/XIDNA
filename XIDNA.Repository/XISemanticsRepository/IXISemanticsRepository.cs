using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IXISemanticsRepository
    {

        #region XISemantics

        DTResponse GetXISemanticsDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string database);
        VMCustomResponse CreateXISemantics(cQSDefinition model, int iUserID, string sOrgName, string sDatabase);
        cQSDefinition EditXISemanticsByID(int ID, int iUserID, string sOrgName, string sDatabase);
        cQSDefinition GetXISemanticsByID(int ID, int iUserID, string sOrgName, string sDatabase);
        int DeleteXISemanticsDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase);
        int CopyQSFieldByID(int ID, int OrgID, int iUserID, string sDatabase);
        #endregion XISemantics

        #region XISemanticsSteps
        cQSStepDefiniton EditXISemanticsSectionByID(int ID, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetQuoteStages(int iUserID, string sOrgName, string sDatabase);
        DTResponse GetStepDetails(jQueryDataTableParamModel param, int iXISemanticID, int iUserID, string sOrgName, string database);
        VMCustomResponse CreateXISemanticsSteps(cQSStepDefiniton XISmtcsStps, int iUserID, string sOrgName, string sDatabase);
        cQSStepDefiniton EditXISemanticsStepsByID(string ID, int iUserID, string sOrgName, string sDatabase);
        int DeleteXISemanticsStepsDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase);
        cQSStepDefiniton GetStepDetailsByID(int iStepID, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetXIFields(int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveStepXIFields(int iStepID, string[] XIFields, int iUsreID, string sOrgName, string sDatabase);
        List<string> XIFieldValues(int iStepID, int iUserID, string sOrgName, string sDatabase);
        List<string> XILinkValues(int SectionID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveSectionFields(cStepSectionDefinition model, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveSectionContent(int StepID, int DisplayAs, int ContentID, int SecID, string SectionName, string sParams, bool bIsHidden, decimal iOrder, string sSectionCode, string[] QSLinks, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveSectionHTMLContent(int StepID, int DisplayAs, string sHTMLContent, int SecID, string SectionName, bool bIsHidden, decimal iOrder, string sSectionCode, int iUserID, string sOrgName, string sDatabase, string RuleXIGUID = "");
        int DeleteSectionBySectionID(int SectionID, int iUserID, string sOrgName, string sDatabase);
        int DeleteSectionFieldsBySectionID(int SectionID, int FieldID, int iUserID, string sOrgName, string sDatabase);
        cStepSectionDefinition ShowXISemanticsStepBySecID(int ID, int SectionID, int DisplayAs, int iUserID, string sOrgName, string sDatabase);
        Dictionary<string, string> GetXILinks(int iUserID, string sOrgName, string sDatabase);
        Dictionary<string, string> GetQSXiLinkCodes(int iUserID, string sOrgName, string sDatabase);
        List<string> XILinkCodes(string SectionID, int iUserID, string sOrgName, string sDatabase);
        #endregion XISemanticsSteps

        #region XISemanticNavigation

        DTResponse GetNavigationDetails(jQueryDataTableParamModel param, int iXIsemanticID, int iUserID, string sOrgName, string database);
        VMCustomResponse CreateXISemanticsNavigation(cQSStepDefiniton XISmtcsNav, int iUserID, string sOrgName, string sDatabase);
        cQSNavigations EditXISemanticsNavigationByID(int ID, int iUserID, string sOrgName, string sDatabase);
        int DeleteXISemanticsNavigationDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase);
        List<VMDropDown> AddXISemanticsNavigations(int ID, int iUserID, string sOrgName, string sDatabase);

        #endregion XISemanticNavigation


        #region QuestionSet

        DTResponse GetQSFieldsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        cFieldOrigin GetQSFieldByID(int ID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveQSFieldOptionList(int ID, string[] NVPairs, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveQSField(cFieldOrigin model, int iUserID, string sOrgName, string sDatabase);
        bool IsExistsFieldName(string sName, int ID, int iUserID, string sOrgName, string sDatabase);
        Dictionary<string, string> GetOneClicks(string sDatabase);
        Dictionary<string, string> GetDDLBOs();
        //cStepInstance GetNextStepInstance(int iStepID, string sDatabase);

        #endregion QuestionSet

        #region XIDataTypes
        DTResponse GetXIDataTypeGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        cXIDataTypes GetXIDataTypeByID(int XIDataTypeID, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXIDataType(cXIDataTypes model, int iUserID, string sOrgName, string sDatabase);

        #endregion XIDataTypes


        #region QSVisualisations

        DTResponse GetQSVisualisationsGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveQSVisualisations(cQSVisualisations model, int iUserID, string sOrgName, string sDatabase);
        cQSVisualisations GetQSVisualisationsByID(int iXIQSVisualID, int iUserID, string sOrgName, string sDatabase);
        #endregion QSVisualisations

        #region XIQSScripts

        DTResponse GetXIQSScriptsGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXIQSScripts(XIQSScripts model, int iUserID, string sOrgName, string sDatabase);
        XIQSScripts GetXIQSScriptsByID(int iQSScriptID, int iUserID, string sOrgName, string sDatabase);

        #endregion XIQSScripts

        #region Links
        DTResponse GridQSLinks(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveEditQSLinks(XIQSLinkDefinition model, string sDatabase);
        XIQSLinkDefinition GetQSXiLinkByID(string Code, int iUserID, string sOrgName, string sDatabase);
        int DeleteXIQSLinkByID(int ID, int iUserID, string sOrgName, string sDatabase);
        bool IsExistNameOrCode(string sName = "", string sCode = "", int ID = 0);
        #endregion Links

        #region XIQSXiLinksMap

        DTResponse GetXIQSLinksGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase);
        VMCustomResponse SaveXIQSLinks(XIQSLinks model, int iUserID, string sOrgName, string sDatabase);
        XIQSLinks GetXIQSXiLinkByID(int ID, int iUserID, string sOrgName, string sDatabase);

        #endregion XIQSXiLinksMap

        //CopyXISemanticsByXISemanticID
        int CopyXISemanticsByID(int ID, int OrgID, int iUserID, string sDatabase);

        #region XIQSDefinitionStages

        VMCustomResponse SaveEditXIQSStages(cXIQSStages model, string sDatabase);
        cXIQSStages GetStagesByQSDefID(int iQSDefID);
        int DeleteXIQSStageByID(int ID);

        #endregion XIQSDefinitionStages
    }
}
