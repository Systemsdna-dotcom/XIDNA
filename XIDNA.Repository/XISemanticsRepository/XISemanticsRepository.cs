using XIDNA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using XIDNA.ViewModels;
using System.Data;
using System.Net;

namespace XIDNA.Repository
{

    public class XISemanticsRepository : IXISemanticsRepository
    {

        #region XISemantics
        CommonRepository Common = new CommonRepository();
        public DTResponse GetXISemanticsDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cQSDefinition> AllSmtcs;
            AllSmtcs = dbContext.QSDefinition.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSmtcs = AllSmtcs.Where(m => m.sName.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSmtcs.Count();
            AllSmtcs = QuerableUtil.GetResultsForDataTables(AllSmtcs, "", sortExpression, param);
            var Smcts = AllSmtcs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in Smcts
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName,c.sDescription,  c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse CreateXISemantics(cQSDefinition model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSDefinition XIScs = new cQSDefinition();
            if (model.ID == 0)
            {
                XIScs.FKiApplicationID = model.FKiApplicationID;
                XIScs.FKiOrganisationID = iOrgID;
                XIScs.sName = model.sName;
                XIScs.SaveType = model.SaveType;
                XIScs.sDescription = model.sDescription;
                XIScs.sMode = model.sMode;
                XIScs.iVisualisationID = model.iVisualisationID;
                XIScs.bIsTemplate = model.bIsTemplate;
                XIScs.sHTMLPage = model.sHTMLPage;
                XIScs.iLayoutID = model.iLayoutID;
                if (model.bIsContextObject == true)
                {
                    XIScs.FKiParameterID = model.FKiParameterID;
                    XIScs.FKiBOStructureID = model.FKiBOStructureID;
                }
                else
                {
                    XIScs.FKiParameterID = 0;
                    XIScs.FKiBOStructureID = 0;
                }
                XIScs.FKiOriginID = model.FKiOriginID;
                XIScs.StatusTypeID = model.StatusTypeID;
                XIScs.CreatedBy = model.CreatedBy;
                XIScs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                XIScs.CreatedTime = model.CreatedTime;
                XIScs.UpdatedBy = model.UpdatedBy;
                XIScs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                XIScs.UpdatedTime = model.UpdatedTime;
                dbContext.QSDefinition.Add(XIScs);
                dbContext.SaveChanges();
            }
            else
            {
                XIScs = dbContext.QSDefinition.Find(model.ID);
                XIScs.FKiApplicationID = model.FKiApplicationID;
                XIScs.FKiOrganisationID = iOrgID;
                XIScs.sName = model.sName;
                XIScs.sDescription = model.sDescription;
                XIScs.SaveType = model.SaveType;
                XIScs.sMode = model.sMode;
                XIScs.iVisualisationID = model.iVisualisationID;
                XIScs.bIsTemplate = model.bIsTemplate;
                XIScs.sHTMLPage = model.sHTMLPage;
                XIScs.iLayoutID = model.iLayoutID;
                if (model.FKiParameterID > 0 || model.FKiBOStructureID > 0)
                {
                    XIScs.FKiParameterID = model.FKiParameterID;
                    XIScs.FKiBOStructureID = model.FKiBOStructureID;
                }
                else
                {
                    XIScs.FKiParameterID = 0;
                    XIScs.FKiBOStructureID = 0;
                }
                XIScs.FKiOriginID = model.FKiOriginID;
                XIScs.StatusTypeID = model.StatusTypeID;
                XIScs.UpdatedBy = model.UpdatedBy;
                XIScs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                XIScs.UpdatedTime = model.UpdatedTime;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = XIScs.ID, Status = true };
        }
        public cQSDefinition EditXISemanticsByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSDefinition oQSDefintion = new cQSDefinition();
            oQSDefintion = dbContext.QSDefinition.Find(ID);
            return oQSDefintion;
        }

        public cQSDefinition GetXISemanticsByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSDefinition oQSDefintion = new cQSDefinition();
            oQSDefintion = dbContext.QSDefinition.Find(ID);
            return oQSDefintion;
        }

        public int DeleteXISemanticsDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 1;
            cQSDefinition oQSDefintion = dbContext.QSDefinition.Find(ID);
            dbContext.QSDefinition.Remove(oQSDefintion);
            dbContext.SaveChanges();
            return iStatus;
        }

        #endregion XISemantics

        #region XISemanticsSteps

        public DTResponse GetStepDetails(jQueryDataTableParamModel param, int iQSID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cQSStepDefiniton> AllSmtcStps;
            AllSmtcStps = dbContext.QSStepDefiniton.Where(m => m.FKiQSDefintionID == iQSID);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSmtcStps = AllSmtcStps.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSmtcStps.Count();
            AllSmtcStps = QuerableUtil.GetResultsForDataTables(AllSmtcStps, "", sortExpression, param);
            var SmctStps = AllSmtcStps.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in SmctStps
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName,Convert.ToString(c.iOrder),c.sCode,((EnumSemanticsDisplayAs)c.iDisplayAs).ToString(),c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse CreateXISemanticsSteps(cQSStepDefiniton model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSStepDefiniton oQSStep = new cQSStepDefiniton();
            if (model.ID == 0)
            {
                oQSStep.sName = model.sName;
                oQSStep.sDisplayName = model.sDisplayName;
                oQSStep.iOrder = model.iOrder;
                oQSStep.sCode = model.sCode;
                oQSStep.iDisplayAs = model.iDisplayAs;
                if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
                {
                    oQSStep.iXIComponentID = model.FKiContentID;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
                {
                    oQSStep.i1ClickID = model.FKiContentID;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.Html.ToString()))
                {
                    oQSStep.HTMLContent = model.HTMLContent;
                }
                oQSStep.FKiQSDefintionID = model.FKiQSDefintionID;
                if (model.bIsHidden)
                {
                    oQSStep.sIsHidden = "on";
                }
                else
                {
                    oQSStep.sIsHidden = "off";
                }
                oQSStep.bInMemoryOnly = model.bInMemoryOnly;
                oQSStep.bIsSaveNext = model.bIsSaveNext;
                oQSStep.bIsSave = model.bIsSave;
                oQSStep.bIsSaveClose = model.bIsSaveClose;
                oQSStep.bIsBack = model.bIsBack;
                oQSStep.sSaveBtnLabel = "";
                if (model.bIsSaveNext == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSaveNext;
                }
                if (model.bIsSave == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSave;
                }
                if (model.bIsSaveClose == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSaveClose;
                }
                oQSStep.sBackBtnLabel = "";
                if (model.bIsBack == true)
                {
                    oQSStep.sBackBtnLabel = model.sBackBtnLabel;
                }
                //if (model.bIsStepTraceStage == true)
                //{
                //    oQSStep.FkiQuoteStageID = model.FKiQSDefintionID;
                //}
                //else
                //{
                //    oQSStep.FKiQSDefintionID = 0;
                //}
                oQSStep.bIsContinue = model.bIsContinue;
                oQSStep.bIsHistory = model.bIsHistory;
                oQSStep.bIsCopy = model.bIsCopy;
                oQSStep.iLayoutID = model.iLayoutID;
                oQSStep.OrganisationID = iOrgID;
                oQSStep.FKiApplicationID = FKiAppID;
                oQSStep.StatusTypeID = model.StatusTypeID;
                oQSStep.CreatedBy = model.CreatedBy;
                oQSStep.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oQSStep.CreatedTime = model.CreatedTime;
                oQSStep.UpdatedBy = model.UpdatedBy;
                oQSStep.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oQSStep.UpdatedTime = model.UpdatedTime;
                dbContext.QSStepDefiniton.Add(oQSStep);
                dbContext.SaveChanges();
                if (!string.IsNullOrEmpty(model.ComponentNVIDs))
                {
                    var IDs = model.ComponentNVIDs.Split(',').ToList();
                    foreach (var ID in IDs)
                    {
                        var id = Convert.ToInt32(ID);
                        var CompNV = dbContext.XIComponentParams.Find(id);
                        CompNV.iStepDefinitionID = oQSStep.ID;
                        dbContext.SaveChanges();
                    }
                }
                if (!string.IsNullOrEmpty(model.SectionIDs))
                {
                    var IDs = model.SectionIDs.Split(',').ToList();
                    foreach (var ID in IDs)
                    {
                        var id = Convert.ToInt32(ID);
                        var Section = dbContext.StepSectionDefinition.Find(id);
                        Section.FKiStepDefinitionID = oQSStep.ID;
                        dbContext.SaveChanges();
                    }
                }
            }
            else
            {
                oQSStep = dbContext.QSStepDefiniton.Find(model.ID);
                oQSStep.sName = model.sName;
                oQSStep.sDisplayName = model.sDisplayName;
                oQSStep.iOrder = model.iOrder;
                oQSStep.sCode = model.sCode;
                oQSStep.iDisplayAs = model.iDisplayAs;
                oQSStep.iLayoutID = model.iLayoutID;
                oQSStep.OrganisationID = iOrgID;
                oQSStep.FKiApplicationID = FKiAppID;
                if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.EditForm.ToString()))
                {
                    oQSStep.iXIComponentID = 0;
                    oQSStep.i1ClickID = 0;
                    oQSStep.HTMLContent = null;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
                {
                    oQSStep.iXIComponentID = model.FKiContentID;
                    oQSStep.FKiContentID = 0;
                    oQSStep.HTMLContent = null;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
                {
                    oQSStep.i1ClickID = model.FKiContentID;
                    oQSStep.iXIComponentID = 0;
                    oQSStep.HTMLContent = null;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.Html.ToString()))
                {
                    oQSStep.HTMLContent = model.HTMLContent;
                    oQSStep.i1ClickID = 0;
                    oQSStep.iXIComponentID = 0;
                }
                else if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.Bespoke.ToString()))
                {
                    oQSStep.i1ClickID = 0;
                    oQSStep.iXIComponentID = 0;
                    oQSStep.HTMLContent = null;
                }
                oQSStep.bInMemoryOnly = model.bInMemoryOnly;
                if (model.bIsHidden)
                {
                    oQSStep.sIsHidden = "on";
                }
                else
                {
                    oQSStep.sIsHidden = "off";
                }
                oQSStep.bIsSaveNext = model.bIsSaveNext;
                oQSStep.bIsSave = model.bIsSave;
                oQSStep.bIsSaveClose = model.bIsSaveClose;
                oQSStep.bIsBack = model.bIsBack;
                oQSStep.sSaveBtnLabel = "";
                if (model.bIsSaveNext == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSaveNext;
                }
                if (model.bIsSave == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSave;
                }
                if (model.bIsSaveClose == true)
                {
                    oQSStep.sSaveBtnLabel = model.sSaveBtnLabelSaveClose;
                }
                oQSStep.sBackBtnLabel = "";
                if (model.bIsBack == true)
                {
                    oQSStep.sBackBtnLabel = model.sBackBtnLabel;
                }
                oQSStep.bIsContinue = model.bIsContinue;
                oQSStep.bIsHistory = model.bIsHistory;
                oQSStep.bIsCopy = model.bIsCopy;
                oQSStep.FkiQuoteStageID = model.FkiQuoteStageID;
                oQSStep.StatusTypeID = model.StatusTypeID;
                oQSStep.UpdatedBy = model.UpdatedBy;
                oQSStep.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oQSStep.UpdatedTime = model.UpdatedTime;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oQSStep.ID, Status = true };
        }

        public cQSStepDefiniton EditXISemanticsStepsByID(string ID, int iUserID, string sOrgName, string sDatabase)
        {
            int iID = 0;
            int.TryParse(ID, out iID);
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSStepDefiniton oQSStep = new cQSStepDefiniton();
            oQSStep = dbContext.QSStepDefiniton.Find(ID);
            oQSStep.QSNavigations = dbContext.QSNavigations.Where(m => m.FKiStepDefinitionID == iID).ToList();
            var SematicSteps = new List<VMDropDown>();
            SematicSteps = dbContext.QSStepDefiniton.ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            oQSStep.QSNavigations.ToList().ForEach(m => m.SematicSteps = SematicSteps);
            var Sections = dbContext.StepSectionDefinition.Where(m => m.FKiStepDefinitionID == iID).ToList();
            if (oQSStep.bIsSaveNext == true)
            {
                oQSStep.sSaveBtnLabelSaveNext = oQSStep.sSaveBtnLabel;
            }
            if (oQSStep.bIsSave == true)
            {
                oQSStep.sSaveBtnLabelSave = oQSStep.sSaveBtnLabel;
            }
            oQSStep.Sections = new List<cStepSectionDefinition>();
            if (Sections != null && Sections.Count() > 0)
            {
                foreach (var items in Sections)
                {
                    if (((EnumSemanticsDisplayAs)items.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Fields.ToString())
                    {
                        var SecFields = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == items.ID).ToList();
                        foreach (var item in SecFields)
                        {
                            var FieldDef = dbContext.XIFieldOrigin.Where(m => m.ID == item.FKiXIFieldOriginID).FirstOrDefault();
                            item.FieldOrigin = FieldDef;
                        }
                        items.FieldDefinitions = SecFields;
                    }
                }
                oQSStep.Sections = Sections.OrderBy(m => m.iOrder).ToList();
            }
            if (oQSStep.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
            {
                oQSStep.FKiContentID = oQSStep.iXIComponentID;
            }
            else if (oQSStep.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
            {
                oQSStep.FKiContentID = oQSStep.i1ClickID;
            }
            if (oQSStep.sIsHidden == "on")
            {
                oQSStep.bIsHidden = true;
            }
            else if (oQSStep.sIsHidden == "off")
            {
                oQSStep.bIsHidden = false;
            }
            return oQSStep;
        }

        public cQSStepDefiniton EditXISemanticsSectionByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSStepDefiniton oQSStep = new cQSStepDefiniton();
            var Sections = dbContext.StepSectionDefinition.Where(m => m.ID == ID).ToList();
            oQSStep.Sections = new List<cStepSectionDefinition>();
            if (Sections != null && Sections.Count() > 0)
            {
                foreach (var items in Sections)
                {
                    if (((EnumSemanticsDisplayAs)items.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Fields.ToString())
                    {
                        var SecFields = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == items.ID && m.XIDeleted ==0).ToList();
                        foreach (var item in SecFields)
                        {
                            var FieldDef = dbContext.XIFieldOrigin.Where(m => m.ID == item.FKiXIFieldOriginID).FirstOrDefault();
                            item.FieldOrigin = FieldDef;
                        }
                        items.FieldDefinitions = SecFields;
                    }
                }
                oQSStep.Sections = Sections.OrderBy(m => m.iOrder).ToList();
            }
            if (oQSStep.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
            {
                oQSStep.FKiContentID = oQSStep.iXIComponentID;
            }
            else if (oQSStep.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
            {
                oQSStep.FKiContentID = oQSStep.i1ClickID;
            }
            if (oQSStep.sIsHidden == "on")
            {
                oQSStep.bIsHidden = true;
            }
            else if (oQSStep.sIsHidden == "off")
            {
                oQSStep.bIsHidden = false;
            }
            return oQSStep;
        }
        public int DeleteXISemanticsStepsDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            cQSStepDefiniton XISemaStepDetails = dbContext.QSStepDefiniton.Find(ID);
            dbContext.QSStepDefiniton.Remove(XISemaStepDetails);
            dbContext.SaveChanges();
            iStatus = 1;
            return iStatus;
        }

        public cQSStepDefiniton GetStepDetailsByID(int iStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSStepDefiniton oStep = new cQSStepDefiniton();
            oStep = dbContext.QSStepDefiniton.Find(iStepID);
            return oStep;
        }
        public List<string> XIFieldValues(int iStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<string> FieldValues = new List<string>();
            var lFieldDefinitions = dbContext.XIFieldDefinition.Where(m => m.FKiXIStepDefinitionID == iStepID && m.FKiStepSectionID == 0).Select(m => m.FKiXIFieldOriginID).ToList();
            foreach (var item in lFieldDefinitions)
            {
                var lXiFields = dbContext.XIFieldOrigin.Where(m => m.ID == item).ToList();
                foreach (var items in lXiFields)
                {
                    FieldValues.Add(items.sName);
                }
            }
            return FieldValues;
        }

        public List<VMDropDown> GetXIFields(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> XIFields = new List<VMDropDown>();

            //Dictionary<string, string> XIFields = new Dictionary<string, string>();
            var lXiFields = dbContext.XIFieldOrigin.Where(m => m.FKiOrganisationID == iOrgID).Where(m => m.FKiApplicationID == UserDetais.FKiApplicationID).ToList();
            XIFields = lXiFields.Select(m => new VMDropDown { text = m.sName, Value = m.ID }).ToList();

            //foreach (var items in lXiFields)
            //{
            //    XIFields[items.sName] = items.sName;
            //}
            return XIFields;
        }

        public List<string> XILinkValues(int SectionID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<string> FieldValues = new List<string>();
            var lFieldDefinitions = dbContext.QSLink.Where(m => m.FKiSectionDefinitionID == SectionID).Select(m => m.FKiXILinkID).ToList();
            foreach (var item in lFieldDefinitions)
            {
                var lXiFields = dbContext.XiLinks.Where(m => m.XiLinkID == item).ToList();
                foreach (var items in lXiFields)
                {
                    FieldValues.Add(items.Name);
                }
            }
            return FieldValues;
        }

        public Dictionary<string, string> GetXILinks(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            Dictionary<string, string> XiLinks = new Dictionary<string, string>();
            var lXiLinks = dbContext.XiLinks.Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiApplicationID == UserDetais.FKiApplicationID).ToList();
            foreach (var items in lXiLinks)
            {
                XiLinks[items.Name] = items.Name;
            }
            return XiLinks;
        }

        public Dictionary<string, string> GetQSXiLinkCodes(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            Dictionary<string, string> QSXiLinkCode = new Dictionary<string, string>();
            var lXiLinks = dbContext.XiLinks.Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiApplicationID == UserDetais.FKiApplicationID).ToList();
            var lXiLinkCodes = dbContext.QSXiLink.ToList().Select(m => m.sCode).Distinct();
            foreach (var items in lXiLinkCodes)
            {
                QSXiLinkCode[items] = items;
            }
            return QSXiLinkCode;
        }
        public List<string> XILinkCodes(string SectionID, int iUserID, string sOrgName, string sDatabase)
        {
            int iSecID = 0;
            int.TryParse(SectionID, out iSecID);
            Guid SecXIGUID = Guid.Empty;
            Guid.TryParse(SectionID, out SecXIGUID);
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<string> LinkCode = new List<string>();
            var lCodeDefinitions = dbContext.QSLink.ToList().Where(m => m.FKiSectionDefinitionIDXIGUID == SecXIGUID && m.XIDeleted == 0).Select(m => m.sCode).ToList();
            foreach (var code in lCodeDefinitions)
            {
                LinkCode.Add(code);
            }
            return LinkCode;
        }

        public VMCustomResponse SaveStepXIFields(int iStepID, string[] XIFields, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            dbContext.Database.ExecuteSqlCommand("Delete from XIFieldDefinition_T where FKiXIStepDefinitionID = " + iStepID);
            if (XIFields != null)
            {
                foreach (var items in XIFields)
                {
                    var FieldID = dbContext.XIFieldOrigin.Where(m => m.sName == items).Select(m => m.ID).FirstOrDefault();
                    cFieldDefinition oFInstance = new cFieldDefinition();
                    oFInstance.FKiXIFieldOriginID = FieldID;
                    oFInstance.FKiXIStepDefinitionID = iStepID;
                    dbContext.XIFieldDefinition.Add(oFInstance);
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = iStepID, Status = true };
        }

        public VMCustomResponse SaveSectionFields(cStepSectionDefinition model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cStepSectionDefinition Sec = new cStepSectionDefinition();
            if (model.ID > 0)
            {
                Sec = dbContext.StepSectionDefinition.Find(model.ID);
                //if (OldContent != null)
                //{
                //    dbContext.StepSectionDefinition.Remove(OldContent);
                //    dbContext.SaveChanges();
                //}
                var OldSecData = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == model.ID).ToList();
                if (OldSecData != null)
                {
                    foreach (var item in OldSecData)
                    {
                        var OldSecID = dbContext.XIFieldDefinition.Find(item.ID);
                        dbContext.XIFieldDefinition.Remove(OldSecID);
                        dbContext.SaveChanges();
                    }
                }
                var OldQSLinks = dbContext.QSLink.Where(m => m.FKiSectionDefinitionID == model.ID).ToList();
                if (OldQSLinks != null)
                {
                    foreach (var item in OldQSLinks)
                    {
                        var oQSLink = dbContext.QSLink.Find(item.ID);
                        dbContext.QSLink.Remove(oQSLink);
                        dbContext.SaveChanges();
                    }
                }
            }
            if (Sec == null)
            {
                Sec = new cStepSectionDefinition();
                
            }
            
            Sec.FKiStepDefinitionID = model.FKiStepDefinitionID;
            var StepD=dbContext.QSStepDefiniton.Find(model.FKiStepDefinitionID);
            if (StepD != null)
            {
                Sec.FKiStepDefinitionIDXIGUID = StepD.XIGUID;
            }
            Sec.FKiApplicationID = FKiAppID;
            Sec.OrganisationID = iOrgID;
            Sec.iDisplayAs = model.iDisplayAs;
            if (model.sName == "")
            {
                Sec.sName = null; ;
            }
            else
            {
                Sec.sName = model.sName;
            }
            Sec.iXIComponentID = 0;
            Sec.bIsGroup = model.bIsGroup;
            Sec.sGroupDescription = model.sGroupDescription;
            Sec.sGroupLabel = model.sGroupLabel;
            if (model.bIsHidden)
            {
                Sec.sIsHidden = "on";
            }
            else
            {
                Sec.sIsHidden = "off";
            }
            Sec.iOrder = model.iOrder;
            Sec.sCode = model.sCode;
            if (model.ID > 0)
            {

            }
            else
            {
                Sec.XIGUID = Guid.NewGuid();
                dbContext.StepSectionDefinition.Add(Sec);
            }
            dbContext.SaveChanges();
            if (model.SecNVPairs != null)
            {
                foreach (var items in model.SecNVPairs)
                {
                    if (model.iDisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.Fields.ToString()))
                    {
                        int iFiledOrgID = 0;
                        int.TryParse(items, out iFiledOrgID);
                        if (iFiledOrgID > 0)
                        {
                            var FieldID = dbContext.XIFieldOrigin.Where(m => m.ID == iFiledOrgID).FirstOrDefault();
                            if (FieldID != null)
                            {
                                cFieldDefinition oFInstance = new cFieldDefinition();
                                oFInstance.FKiXIFieldOriginID = FieldID.ID;
                                oFInstance.FKiXIStepDefinitionID = model.FKiStepDefinitionID;
                                if (StepD != null)
                                {
                                    oFInstance.FKiXIStepDefinitionIDXIGUID = StepD.XIGUID;
                                }
                                oFInstance.FKiStepSectionID = Sec.ID;
                                oFInstance.FKiStepSectionIDXIGUID = Sec.XIGUID;
                                oFInstance.FKiApplicationID = FKiAppID;
                                oFInstance.OrganisationID = iOrgID;
                                dbContext.XIFieldDefinition.Add(oFInstance);
                                dbContext.SaveChanges();
                            }
                        }
                    }
                }
            }
            if (model.QSCodes != null)
            {
                foreach (var items in model.QSCodes)
                {
                    XIQSLinks oQSLink = new XIQSLinks();
                    oQSLink.FKiSectionDefinitionID = Sec.ID;
                    oQSLink.FKiXILinkID = 0;
                    oQSLink.sCode = items;
                    oQSLink.FKiApplicationID = FKiAppID;
                    oQSLink.OrganisationID = iOrgID;
                    oQSLink.FKiStepDefinitionID = model.FKiStepDefinitionID;
                    dbContext.QSLink.Add(oQSLink);
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Sec.ID, Status = true };
        }

        public VMCustomResponse SaveSectionContent(int StepID, int DisplayAs, int ContentID, int SecID, string SectionName, string sParams, bool bIsHidden, decimal iOrder, string sSectionCode, string[] QSCodes, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            if (SecID > 0)
            {
                var oSection = dbContext.StepSectionDefinition.Find(SecID);
                var OldQSLinks = dbContext.QSLink.Where(m => m.FKiSectionDefinitionID == oSection.ID).ToList();
                if (OldQSLinks != null)
                {
                    foreach (var item in OldQSLinks)
                    {
                        var oQSLink = dbContext.QSLink.Find(item.ID);
                        dbContext.QSLink.Remove(oQSLink);
                        dbContext.SaveChanges();
                    }
                }
                if (!string.IsNullOrEmpty(SectionName))
                {
                    oSection.sName = SectionName;
                }
                else
                {
                    oSection.sName = null;
                }
                oSection.iDisplayAs = DisplayAs;
                if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
                {
                    oSection.iXIComponentID = ContentID;
                }
                else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
                {
                    oSection.i1ClickID = ContentID;
                }
                if (bIsHidden)
                {
                    oSection.sIsHidden = "on";
                }
                else
                {
                    oSection.sIsHidden = "off";
                }
                oSection.iOrder = iOrder;
                oSection.sCode = sSectionCode;
                oSection.FKiApplicationID = FKiAppID;
                oSection.OrganisationID = iOrgID;
                dbContext.SaveChanges();
            }
            else
            {
                cStepSectionDefinition Sec = new cStepSectionDefinition();
                Sec.FKiStepDefinitionID = StepID;
                if (SectionName == "")
                {
                    Sec.sName = null;
                }
                else
                {
                    Sec.sName = SectionName;
                }
                Sec.iDisplayAs = DisplayAs;
                if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
                {
                    Sec.iXIComponentID = ContentID;
                }
                else if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.OneClick.ToString()))
                {
                    Sec.i1ClickID = ContentID;
                }
                if (bIsHidden)
                {
                    Sec.sIsHidden = "on";
                }
                else
                {
                    Sec.sIsHidden = "off";
                }
                Sec.iOrder = iOrder;
                Sec.sCode = sSectionCode;
                Sec.OrganisationID = iOrgID;
                Sec.FKiApplicationID = FKiAppID;
                dbContext.StepSectionDefinition.Add(Sec);
                dbContext.SaveChanges();
                SecID = Sec.ID;
            }

            if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.XIComponent.ToString()))
            {
                if (!string.IsNullOrEmpty(sParams))
                {
                    var IDs = sParams.Split(',').ToList();
                    foreach (var PID in IDs)
                    {
                        var id = Convert.ToInt32(PID);
                        var Param = dbContext.XIComponentParams.Find(id);
                        Param.iStepSectionID = SecID;
                        dbContext.SaveChanges();
                    }
                }
            }
            if (QSCodes != null)
            {
                foreach (var items in QSCodes)
                {
                    XIQSLinks oQSLink = new XIQSLinks();
                    oQSLink.FKiSectionDefinitionID = SecID;
                    oQSLink.FKiXILinkID = 0;
                    oQSLink.sCode = items;
                    oQSLink.FKiStepDefinitionID = StepID;
                    oQSLink.FKiApplicationID = FKiAppID;
                    oQSLink.OrganisationID = iOrgID;
                    dbContext.QSLink.Add(oQSLink);
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = SecID, Status = true };
        }


        public VMCustomResponse SaveSectionHTMLContent(int StepID, int DisplayAs, string sHTMLContent, int SecID, string SectionName, bool bIsHidden, decimal iOrder, string sSectionCode, int iUserID, string sOrgName, string sDatabase, string RuleXIGUID="")
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cStepSectionDefinition Sec = new cStepSectionDefinition();
            if (SecID > 0)
            {
                Sec = dbContext.StepSectionDefinition.Find(SecID);
                //if (OldContent != null)
                //{
                //    dbContext.StepSectionDefinition.Remove(OldContent);
                //    dbContext.SaveChanges();
                //}
            }
            if (Sec == null)
            {
                Sec = new cStepSectionDefinition();
            }
            Sec.FKiStepDefinitionID = StepID;
            if (SectionName == "")
            {
                Sec.sName = null;
            }
            else
            {
                Sec.sName = SectionName;
            }
            if (bIsHidden)
            {
                Sec.sIsHidden = "on";
            }
            else
            {
                Sec.sIsHidden = "off";
            }
            var RuleIDXIGUID = Guid.Empty;
            Guid.TryParse(RuleXIGUID, out RuleIDXIGUID);
            Sec.FkiRuleIDXIGUID = RuleIDXIGUID;
            Sec.iOrder = iOrder;
            Sec.sCode = sSectionCode;
            Sec.FKiApplicationID = FKiAppID;
            Sec.OrganisationID = iOrgID;
            Sec.iDisplayAs = DisplayAs;
            if (DisplayAs == (Int32)Enum.Parse(typeof(EnumSemanticsDisplayAs), EnumSemanticsDisplayAs.Html.ToString()))
            {
                Sec.HTMLContent = sHTMLContent;
            }
            if (SecID > 0)
            {

            }
            else
            {
                dbContext.StepSectionDefinition.Add(Sec);
            }
            dbContext.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Sec.ID, Status = true };
        }

        //public VMCustomResponse SaveSectionHtmlCode(int StepID, int DisplayAs, string HtmlCode, int SecID, int iUserID, string sOrgName, string sDatabase)
        //{
        //    var UserDatails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
        //    var iOrgID = UserDatails.FKiOrgID;
        //    var sOrgDB = UserDatails.sUserDatabase;
        //    if (iOrgID > 0)
        //    {
        //        sDatabase = sOrgDB;
        //    }
        //    ModelDbContext dbContext = new ModelDbContext();
        //    if (SecID > 0)
        //    {
        //        var OldHtml = dbContext.StepSectionDefinition.Find(SecID);
        //        if (OldHtml != null)
        //        {
        //            dbContext.StepSectionDefinition.Remove(OldHtml);
        //            dbContext.SaveChanges();
        //        }
        //    }
        //    dbContext.SaveChanges();
        //    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = StepID, Status = true };
        //}

        public int DeleteSectionBySectionID(int SectionID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDatails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDatails.FKiOrgID;
            var sOrgDB = UserDatails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var oSection = dbContext.StepSectionDefinition.Find(SectionID);
            if (oSection != null)
            {
                dbContext.StepSectionDefinition.Remove(oSection);
                dbContext.SaveChanges();                
            }
            return SectionID;
        }

        public int DeleteSectionFieldsBySectionID(int SectionID, int FieldID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDatails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDatails.FKiOrgID;
            var sOrgDB = UserDatails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var oSection = dbContext.StepSectionDefinition.Find(SectionID);
            if (oSection != null)
            {
                //dbContext.StepSectionDefinition.Remove(oSection);
                //dbContext.SaveChanges();
                var FieldDefs = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == SectionID && m.FKiXIFieldOriginID == FieldID).ToList();
                if (FieldDefs != null && FieldDefs.Count() > 0)
                {
                    dbContext.XIFieldDefinition.RemoveRange(FieldDefs);
                    dbContext.SaveChanges();
                }
            }
            //dbContext.SaveChanges();
            return SectionID;
        }

        public cStepSectionDefinition ShowXISemanticsStepBySecID(int StefDefID, int SectionID, int DisplayAs, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var SematicSteps = new List<VMDropDown>();
            var oSection = dbContext.StepSectionDefinition.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiStepDefinitionID == StefDefID && m.ID == SectionID).FirstOrDefault();
            //cStepSectionDefinition oSection = new cStepSectionDefinition();
            if (((EnumSemanticsDisplayAs)oSection.iDisplayAs).ToString() == EnumSemanticsDisplayAs.Fields.ToString())
            {
                var SecFields = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == oSection.ID && m.FKiXIStepDefinitionID == StefDefID).ToList();
                foreach (var item in SecFields)
                {
                    var FieldDef = dbContext.XIFieldOrigin.Where(m => m.ID == item.FKiXIFieldOriginID).FirstOrDefault();
                    item.FieldOrigin = FieldDef;
                }
                oSection.FieldDefinitions = SecFields;
            }
            if (oSection.sIsHidden == "on")
            {
                oSection.bIsHidden = true;
            }
            else if (oSection.sIsHidden == "off")
            {
                oSection.bIsHidden = false;
            }
            return oSection;
        }

        #endregion XISemanticsSteps

        #region XISemanticsNavigation

        public DTResponse GetNavigationDetails(jQueryDataTableParamModel param, int iXIsemanticID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var NavigationData = dbContext.QSStepDefiniton.Where(m => m.FKiQSDefintionID == iXIsemanticID).Select(m => m.ID).ToList();
            IQueryable<cQSNavigations> AllSmtcsNav = dbContext.QSNavigations.Where(c => NavigationData.Contains(c.FKiStepDefinitionID));
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSmtcsNav = AllSmtcsNav.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSmtcsNav.Count();
            AllSmtcsNav = QuerableUtil.GetResultsForDataTables(AllSmtcsNav, "", sortExpression, param);
            var SmctsNav = AllSmtcsNav.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in SmctsNav
                     join d in dbContext.QSStepDefiniton.ToList() on c.FKiStepDefinitionID equals d.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName,Convert.ToString(c.iType),Convert.ToString(d.sName),Convert.ToString(c.iOrder),c.sFunction,  c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse CreateXISemanticsNavigation(cQSStepDefiniton model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSNavigations oXISNav = new cQSNavigations();
            if (model.QSNavigations.Count > 0)
            {
                for (int i = 0; i < model.QSNavigations.Count; i++)
                {
                    var oXISemiNav = model.QSNavigations[i];
                    if (oXISemiNav.IsSave == true)
                    {
                        if (oXISemiNav.ID > 0)
                        {
                            oXISNav = dbContext.QSNavigations.Find(oXISemiNav.ID);
                        }
                        oXISNav.sName = oXISemiNav.sName;
                        oXISNav.iType = oXISemiNav.iType;
                        if (oXISemiNav.iType == 10)
                        {
                            oXISNav.iNextStepID = oXISemiNav.iNextStepID;
                        }
                        else
                        {
                            oXISNav.iNextStepID = 0;
                        }
                        oXISNav.iOrder = oXISemiNav.iOrder;
                        oXISNav.sFunction = oXISemiNav.sFunction;
                        oXISNav.FKiStepDefinitionID = model.ID;
                        oXISNav.sField = oXISemiNav.sField;
                        oXISNav.sOperator = oXISemiNav.sOperator;
                        oXISNav.sValue = oXISemiNav.sValue;
                        oXISNav.StatusTypeID = oXISemiNav.StatusTypeID;
                        if (oXISemiNav.ID == 0)
                        {
                            oXISNav.CreatedBy = model.CreatedBy;
                            oXISNav.CreatedTime = model.CreatedTime;
                            oXISNav.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        }
                        oXISNav.UpdatedBy = model.UpdatedBy;
                        oXISNav.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oXISNav.UpdatedTime = model.UpdatedTime;
                        if (oXISemiNav.ID == 0)
                        {
                            dbContext.QSNavigations.Add(oXISNav);
                        }
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        var iSemiNavID = dbContext.QSNavigations.Where(m => m.ID == oXISemiNav.ID).Select(m => m.ID).FirstOrDefault();
                        dbContext.Database.ExecuteSqlCommand("Delete from QSNavigations_T where ID = " + iSemiNavID);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oXISNav.ID, Status = true };
        }

        public cQSNavigations EditXISemanticsNavigationByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSNavigations oXISNav = new cQSNavigations();
            oXISNav = dbContext.QSNavigations.Find(ID);
            return oXISNav;
        }

        public int DeleteXISemanticsNavigationDetailsByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            cQSNavigations XISemaNavigationDetails = dbContext.QSNavigations.Find(ID);
            dbContext.QSNavigations.Remove(XISemaNavigationDetails);
            dbContext.SaveChanges();
            iStatus = 1;
            return iStatus;
        }
        public List<VMDropDown> AddXISemanticsNavigations(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllSteps = new List<VMDropDown>();
            var StepInfo = dbContext.QSStepDefiniton.ToList();
            foreach (var items in StepInfo)
            {
                AllSteps.Add(new VMDropDown
                {
                    Value = items.ID,
                    text = items.sName
                });
            }
            return AllSteps;
        }

        #endregion XISemanticsNavigation

        #region QuestionSet

        public DTResponse GetQSFieldsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            //GetById(1);
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cFieldOrigin> AllQSFields;
            AllQSFields = dbContext.XIFieldOrigin.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllQSFields = AllQSFields.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllQSFields.Count();
            AllQSFields = QuerableUtil.GetResultsForDataTables(AllQSFields, "", sortExpression, param);
            var Smcts = AllQSFields.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in Smcts
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName, c.sQSCode, GetDataTypeName(c.FKiDataType, sDatabase), c.iLength.ToString(), c.sPlaceHolder, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetDataTypeName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.XIDataTypes.ToList().Where(m => m.ID == p).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }

        public cFieldOrigin GetQSFieldByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext CoredbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cFieldOrigin oQSField = new cFieldOrigin();
            oQSField = dbContext.XIFieldOrigin.Find(ID);
            if (!string.IsNullOrEmpty(oQSField.sIsHidden))
            {
                if (oQSField.sIsHidden == "on")
                {
                    oQSField.bIsHidden = true;
                }
                else if (oQSField.sIsHidden == "off")
                {
                    oQSField.bIsHidden = false;
                }
            }
            oQSField.ddlFieldOptionList = dbContext.XIFieldOptionList.Where(m => m.FKiQSFieldID == ID).ToList();
            if (oQSField.FK1ClickID > 0)
            {
                oQSField.sOneClickName = CoredbContext.Reports.Where(m => m.ID == oQSField.FK1ClickID).Select(m => m.Name).FirstOrDefault();
            }
            if (oQSField.FKiBOID > 0)
            {
                oQSField.sBOName = CoredbContext.BOs.Where(m => m.BOID == oQSField.FKiBOID).Select(m => m.Name).FirstOrDefault();
            }
            return oQSField;
        }

        public VMCustomResponse SaveQSField(cFieldOrigin model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext CoredbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            if (!string.IsNullOrEmpty(model.sOneClickName))
            {
                model.FK1ClickID = CoredbContext.Reports.Where(m => m.Name == model.sOneClickName).Select(m => m.ID).FirstOrDefault();
                model.bIsOptionList = false;
                model.iMasterDataID = 0;
                model.FKiBOID = 0;
            }
            else if (model.iMasterDataID > 0)
            {
                model.FK1ClickID = 0;
                model.bIsOptionList = false;
                model.FKiBOID = 0;
            }
            else if (model.bIsOptionList)
            {
                model.FK1ClickID = 0;
                model.iMasterDataID = 0;
                model.FKiBOID = 0;
            }
            else if (!string.IsNullOrEmpty(model.sBOName))
            {
                model.FK1ClickID = 0;
                model.iMasterDataID = 0;
                model.bIsOptionList = false;
                model.FKiBOID = dbContext.BOs.Where(s => s.Name == model.sBOName).Select(m => m.BOID).FirstOrDefault();
            }
            if (model.bIsCompare == false)
            {
                model.sCompareField = null;
            }
            if (model.ID > 0)
            {
                var OptionList = dbContext.XIFieldOptionList.Where(m => m.FKiQSFieldID == model.ID).ToList();
                if (OptionList != null && OptionList.Count() > 0)
                {
                    dbContext.XIFieldOptionList.RemoveRange(OptionList);
                }
            }
            if (model.bIsHidden)
            {
                model.sIsHidden = "on";
            }
            else
            {
                model.sIsHidden = "off";
            }
            model.FKiApplicationID = FKiAppID;
            model.FKiOrganisationID = iOrgID;
            if (model.ID == 0)
            {
                model.CreatedBy = iUserID;
                model.CreatedTime = DateTime.Now;
                model.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                model.UpdatedBy = iUserID;
                model.UpdatedTime = DateTime.Now;
                model.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XIFieldOrigin.Add(model);
                dbContext.SaveChanges();
            }
            else
            {
                model.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                model.UpdatedBy = iUserID;
                model.UpdatedTime = DateTime.Now;
                model.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = model.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public bool IsExistsFieldName(string sName, int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var AllQSFields = dbContext.XIFieldOrigin.ToList();
            cFieldOrigin QSField = AllQSFields.Where(m => m.sName.Equals(sName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (QSField != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                if (QSField != null)
                {
                    if (ID == QSField.ID)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return true;
                }
            }
        }

        public VMCustomResponse SaveQSFieldOptionList(int ID, string[] NVPairs, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
                //dbContext = new ModelDbContext(sDatabase);
            }
            foreach (var items in NVPairs)
            {
                cXIFieldOptionList OptionList = new cXIFieldOptionList();
                OptionList.FKiQSFieldID = ID;
                var Pair = items.Split('^').ToList();
                OptionList.sOptionName = Pair[0];
                OptionList.sOptionValue = Pair[1];
                OptionList.iType = Convert.ToInt32(Pair[2]);
                if (!string.IsNullOrEmpty(Pair[3]))
                {
                    OptionList.sShowField = Pair[3];
                }
                if (!string.IsNullOrEmpty(Pair[4]))
                {
                    OptionList.sHideField = Pair[4];
                }
                if (!string.IsNullOrEmpty(Pair[5]))
                {
                    OptionList.sOptionCode = Pair[5];
                }
                dbContext.XIFieldOptionList.Add(OptionList);
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public Dictionary<string, string> GetOneClicks(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dictionary<string, string> XIFields = new Dictionary<string, string>();
            var lXiFields = dbContext.Reports.ToList();
            foreach (var items in lXiFields)
            {
                XIFields[items.Name] = items.ID.ToString();
            }
            return XIFields;
        }
        public Dictionary<string, string> GetDDLBOs()
        {
            ModelDbContext dbContext = new ModelDbContext();
            return dbContext.BOs.GroupBy(m => m.Name).ToDictionary(g => g.FirstOrDefault().Name, g => g.FirstOrDefault().BOID.ToString());
        }

        // Copy QuestionSet Field By ID
        public int CopyQSFieldByID(int ID, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cFieldOrigin oCopyField = new cFieldOrigin();
            oCopyField = dbContext.XIFieldOrigin.Where(m => m.ID == ID).FirstOrDefault();
            oCopyField.sName = oCopyField.sName + "Copy";
            oCopyField.CreatedBy = oCopyField.UpdatedBy = iUserID;
            oCopyField.CreatedBySYSID = oCopyField.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.XIFieldOrigin.Add(oCopyField);
            dbContext.SaveChanges();
            var NewCopyFieldID = oCopyField.ID;
            if (oCopyField.bIsOptionList == true)
            {
                cXIFieldOptionList OptionList = new cXIFieldOptionList();
                var oOptionsList = dbContext.XIFieldOptionList.Where(m => m.FKiQSFieldID == ID).ToList();
                if (oOptionsList.Count() > 0)
                {
                    foreach (var item in oOptionsList)
                    {
                        OptionList.FKiQSFieldID = NewCopyFieldID;
                        OptionList.sOptionName = item.sOptionName;
                        OptionList.sOptionValue = item.sOptionValue;
                        OptionList.iType = Convert.ToInt32(item.iType);
                        if (!string.IsNullOrEmpty(item.sShowField))
                        {
                            OptionList.sShowField = item.sShowField;
                        }
                        if (!string.IsNullOrEmpty(item.sHideField))
                        {
                            OptionList.sHideField = item.sHideField;
                        }
                        if (!string.IsNullOrEmpty(item.sHideField))
                        {
                            OptionList.sOptionCode = item.sOptionCode;
                        }
                        dbContext.XIFieldOptionList.Add(OptionList);
                        dbContext.SaveChanges();
                    }
                }
            }
            return NewCopyFieldID;
        }

        #endregion QuestionSet

        #region XIDataTypes
        public DTResponse GetXIDataTypeGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cXIDataTypes> AllXIDataTypes;
            AllXIDataTypes = dbContext.XIDataTypes.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXIDataTypes = AllXIDataTypes.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXIDataTypes.Count();
            AllXIDataTypes = QuerableUtil.GetResultsForDataTables(AllXIDataTypes, "", sortExpression, param);
            var clients = AllXIDataTypes.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName, c.sBaseDataType, c.sStartRange, c.sEndRange, c.sRegex ,c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public cXIDataTypes GetXIDataTypeByID(int XIDataTypeID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cXIDataTypes oGetXIData = new cXIDataTypes();
            oGetXIData = dbContext.XIDataTypes.Find(XIDataTypeID);
            return oGetXIData;
        }
        public VMCustomResponse SaveXIDataType(cXIDataTypes model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cXIDataTypes oXIData = new cXIDataTypes();
            if (model.ID == 0)
            {
                oXIData.FKiApplicationID = model.FKiApplicationID;
                oXIData.FKiOrganisationID = iOrgID;
                oXIData.sName = model.sName;
                oXIData.sBaseDataType = model.sBaseDataType;
                oXIData.sStartRange = model.sStartRange;
                oXIData.sEndRange = model.sEndRange;
                oXIData.sRegex = model.sRegex;
                oXIData.sScript = model.sScript;
                oXIData.sValidationMessage = model.sValidationMessage;
                oXIData.CreatedBy = iUserID;
                oXIData.CreatedTime = DateTime.Now;
                oXIData.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIData.UpdatedBy = iUserID;
                oXIData.UpdatedTime = DateTime.Now;
                oXIData.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIData.StatusTypeID = model.StatusTypeID;
                dbContext.XIDataTypes.Add(oXIData);
                dbContext.SaveChanges();
            }
            else
            {
                oXIData = dbContext.XIDataTypes.Find(model.ID);
                oXIData.FKiApplicationID = model.FKiApplicationID;
                oXIData.FKiOrganisationID = iOrgID;
                oXIData.sName = model.sName;
                oXIData.sBaseDataType = model.sBaseDataType;
                oXIData.sStartRange = model.sStartRange;
                oXIData.sEndRange = model.sEndRange;
                oXIData.sRegex = model.sRegex;
                oXIData.sScript = model.sScript;
                oXIData.sValidationMessage = model.sValidationMessage;
                oXIData.UpdatedBy = iUserID;
                oXIData.UpdatedTime = DateTime.Now;
                oXIData.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIData.StatusTypeID = model.StatusTypeID;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oXIData.ID, Status = true };
        }

        #endregion XIDataTypes

        #region QSVisualisations
        public DTResponse GetQSVisualisationsGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cQSVisualisations> AllXIQSVisuals;
            AllXIQSVisuals = dbContext.QSVisualisations.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXIQSVisuals = AllXIQSVisuals.Where(m => m.sVisualisation.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXIQSVisuals.Count();
            AllXIQSVisuals = QuerableUtil.GetResultsForDataTables(AllXIQSVisuals, "", sortExpression, param);
            var clients = AllXIQSVisuals.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),GetQSName(c.FKiQSDefinitionID,sDatabase),GetQSStepName(c.FKiQSStepDefinitionID,sDatabase),GetFieldName(c.FKiFieldOriginID,sDatabase),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetQSName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.QSDefinition.ToList().Where(m => m.ID == p).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        private string GetQSStepName(int q, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.QSStepDefiniton.ToList().Where(m => m.ID == q).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        private string GetFieldName(int r, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.XIFieldOrigin.ToList().Where(m => m.ID == r).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }

        public VMCustomResponse SaveQSVisualisations(cQSVisualisations model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetais.FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSVisualisations oVis = new cQSVisualisations();
            if (model.ID == 0)
            {
                oVis.FKiApplicationID = FKiAppID;
                oVis.OrganisationID = iOrgID;
                oVis.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oVis.FKiQSStepDefinitionID = model.FKiQSStepDefinitionID;
                oVis.FKiFieldOriginID = model.FKiFieldOriginID;
                oVis.sVisualisation = model.sVisualisation;
                dbContext.QSVisualisations.Add(oVis);
                dbContext.SaveChanges();
            }
            else
            {
                oVis = dbContext.QSVisualisations.Find(model.ID);
                oVis.FKiApplicationID = FKiAppID;
                oVis.OrganisationID = iOrgID;
                oVis.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oVis.FKiQSStepDefinitionID = model.FKiQSStepDefinitionID;
                oVis.FKiFieldOriginID = model.FKiFieldOriginID;
                oVis.sVisualisation = model.sVisualisation;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oVis.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public cQSVisualisations GetQSVisualisationsByID(int iXIQSVisualID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cQSVisualisations oGetXIQSData = new cQSVisualisations();
            oGetXIQSData = dbContext.QSVisualisations.Find(iXIQSVisualID);
            return oGetXIQSData;
        }
        #endregion QSVisualisations



        #region XIQSScripts

        public DTResponse GetXIQSScriptsGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<XIQSScripts> AllXIQSVisuals;
            AllXIQSVisuals = dbContext.XIQSScripts.Where(m => m.FKiApplicationID == FKiAppID);
            string sortExpression = "FKiScriptID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                List<int> Scritpt = dbContext.BOScripts.Where(m => m.sName.Contains(param.sSearch)).Select(m => (int)m.ID).ToList();
                if (Scritpt != null)
                {
                    AllXIQSVisuals = AllXIQSVisuals.Where(m => Scritpt.Contains((int)m.FKiScriptID));
                }
            }
            int displyCount = 0;
            displyCount = AllXIQSVisuals.Count();
            AllXIQSVisuals = QuerableUtil.GetResultsForDataTables(AllXIQSVisuals, "", sortExpression, param);
            var clients = AllXIQSVisuals.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),GetXIQSScriptName(Convert.ToInt32(c.FKiScriptID),sDatabase),GetXIQSName(c.FKiQSDefinitionID,sDatabase),GetXIQSStepName(c.FKiStepDefinitionID,sDatabase),GetSectionName(c.FKiSectionDefinitionID,sDatabase),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetXIQSScriptName(int s, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.BOScripts.ToList().Where(m => m.ID == Convert.ToInt32(s)).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        private string GetXIQSName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.QSDefinition.ToList().Where(m => m.ID == p).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        private string GetXIQSStepName(int q, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.QSStepDefiniton.ToList().Where(m => m.ID == q).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        private string GetSectionName(int r, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.StepSectionDefinition.ToList().Where(m => m.ID == r).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }

        public VMCustomResponse SaveXIQSScripts(XIQSScripts model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, null, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            XIQSScripts oScri = new XIQSScripts();
            if (model.ID == 0)
            {
                oScri.FKiScriptID = model.FKiScriptID;
                oScri.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oScri.FKiStepDefinitionID = model.FKiStepDefinitionID;
                oScri.FKiSectionDefinitionID = model.FKiSectionDefinitionID;
                oScri.FKiApplicationID = FKiAppID;
                oScri.OrganisationID = iOrgID;
                dbContext.XIQSScripts.Add(oScri);
                dbContext.SaveChanges();
            }
            else
            {
                oScri = dbContext.XIQSScripts.Find(model.ID);
                oScri.FKiScriptID = model.FKiScriptID;
                oScri.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oScri.FKiStepDefinitionID = model.FKiStepDefinitionID;
                oScri.FKiSectionDefinitionID = model.FKiSectionDefinitionID;
                oScri.FKiApplicationID = FKiAppID;
                oScri.OrganisationID = iOrgID;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oScri.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public XIQSScripts GetXIQSScriptsByID(int iQSScriptID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            XIQSScripts oGetQSScriptData = new XIQSScripts();
            oGetQSScriptData = dbContext.XIQSScripts.Find(iQSScriptID);
            return oGetQSScriptData;
        }

        #endregion XIQSScripts

        #region Links

        public DTResponse GridQSLinks(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<XIQSLinkDefinition> QSXiLink;
            QSXiLink = (from d in dbContext.QSXiLink
                        select d).GroupBy(k => k.sCode).Select(s => s.FirstOrDefault());
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                QSXiLink = QSXiLink.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = QSXiLink.Count();
            QSXiLink = QuerableUtil.GetResultsForDataTables(QSXiLink, "", sortExpression, param);
            var SmctStps = QSXiLink.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in SmctStps
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName,c.sCode,c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public XIQSLinkDefinition GetQSXiLinkByID(string Code, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XIQSLinkDefinition QSLink = new XIQSLinkDefinition();
            var QSList = dbContext.QSXiLink.Where(m => m.sCode == Code).ToList();
            QSLink.ID = QSList.FirstOrDefault().ID;
            QSLink.sName = QSList.FirstOrDefault().sName;
            QSLink.sCode = Code;
            QSLink.StatusTypeID = QSList.FirstOrDefault().StatusTypeID;
            QSLink.NVs = QSList.ToList().Select(m => new XIQSLinkDefinition
            {
                ID = m.ID,
                sName = m.sName,
                sCode = Code,
                rOrder = m.rOrder,
                FKiXILInkID = m.FKiXILInkID,
                sType = m.sType,
                StatusTypeID = m.StatusTypeID,
                sRunType = m.sRunType,
                FKIXIScriptID = m.FKIXIScriptID
            }).ToList();
            foreach (var items in QSLink.NVs)
            {
                var oXILink = dbContext.XiLinks.Where(m => m.XiLinkID == items.FKiXILInkID).FirstOrDefault();
                if (oXILink != null)
                {
                    items.XiLinkName = oXILink.Name;
                }
            }
            return QSLink;
        }

        public int DeleteXIQSLinkByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDatails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDatails.FKiOrgID;
            var sOrgDB = UserDatails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var oQSLink = dbContext.QSXiLink.Find(ID);
            dbContext.QSXiLink.Remove(oQSLink);
            dbContext.SaveChanges();
            return ID;
        }

        public VMCustomResponse SaveEditQSLinks(XIQSLinkDefinition model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XIQSLinkDefinition Link = new XIQSLinkDefinition();
            if (model.ID > 0)
            {
                Link = dbContext.QSXiLink.Find(model.ID);
                var oQSDefintion = dbContext.QSXiLink.Where(m => m.sCode == Link.sCode).ToList();
                dbContext.QSXiLink.RemoveRange(oQSDefintion);
                dbContext.SaveChanges();
            }
            if (model.NVs != null && model.NVs.Count() > 0)
            {
                foreach (var item in model.NVs)
                {
                    if (!string.IsNullOrEmpty(item.XiLinkName) && !string.IsNullOrEmpty(item.sType))
                    {
                        Link.sName = model.sName;
                        Link.sCode = model.sCode;
                        var oXILink = dbContext.XiLinks.Where(m => m.Name == item.XiLinkName).FirstOrDefault();
                        if (oXILink != null)
                        {
                            Link.FKiXILInkID = oXILink.XiLinkID;
                            Link.sType = item.sType;
                            Link.rOrder = item.rOrder;
                            Link.sRunType = item.sRunType;
                            Link.FKIXIScriptID = item.FKIXIScriptID;
                        }
                        Link.StatusTypeID = model.StatusTypeID;
                        Link.CreatedBy = model.CreatedBy;
                        Link.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        Link.UpdatedBy = model.UpdatedBy;
                        Link.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        Link.CreatedTime = DateTime.Now;
                        Link.UpdatedTime = DateTime.Now;
                        dbContext.QSXiLink.Add(Link);
                        dbContext.SaveChanges();
                    }
                }

            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Link.ID, Status = true };
        }
        public bool IsExistNameOrCode(string sName = "", string sCode = "", int ID = 0)
        {
            ModelDbContext dbContext = new ModelDbContext();
            bool isExist = false;
            XIQSLinkDefinition oQSLinks = new XIQSLinkDefinition();
            try
            {
                if (!string.IsNullOrEmpty(sName))
                {
                    oQSLinks = dbContext.QSXiLink.Where(m => m.sName.ToLower() == sName.ToLower()).FirstOrDefault();

                }
                else if (!string.IsNullOrEmpty(sCode))
                {
                    oQSLinks = dbContext.QSXiLink.Where(m => m.sCode.ToLower() == sCode.ToLower()).FirstOrDefault();

                }
                if (ID == 0)
                {
                    if (oQSLinks == null)
                    {
                        isExist = true;
                    }
                }
                else
                {
                    XIQSLinkDefinition oQSLinksID = new XIQSLinkDefinition();
                    if (!string.IsNullOrEmpty(sName))
                    {
                        oQSLinksID = dbContext.QSXiLink.Where(m => m.sName.ToLower() == sName.ToLower() && m.ID == ID).FirstOrDefault();

                    }
                    else if (!string.IsNullOrEmpty(sCode))
                    {
                        oQSLinksID = dbContext.QSXiLink.Where(m => m.sCode.ToLower() == sCode.ToLower() && m.ID == ID).FirstOrDefault();

                    }
                    if (oQSLinks == null || (oQSLinks != null && oQSLinksID != null))
                    {
                        if (oQSLinks != null)
                        {
                            if (oQSLinks.ID == oQSLinksID.ID)
                            {
                                isExist = true;
                            }
                        }
                        else
                        {
                            isExist = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isExist;
        }

        #endregion Links

        #region XIQSXiLinksMap
        public DTResponse GetXIQSLinksGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<XIQSLinks> AllXIQSXiLinks;
            AllXIQSXiLinks = dbContext.QSLink.Where(m => m.FKiApplicationID == FKiAppID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                List<int> sCode = dbContext.QSXiLink.Where(m => m.sName.Contains(param.sSearch)).Select(m => (int)m.ID).ToList();
                if (sCode != null)
                {
                    AllXIQSXiLinks = AllXIQSXiLinks.Where(m => sCode.Contains((int)m.ID));
                }
            }
            int displyCount = 0;
            displyCount = AllXIQSXiLinks.Count();
            AllXIQSXiLinks = QuerableUtil.GetResultsForDataTables(AllXIQSXiLinks, "", sortExpression, param);
            var clients = AllXIQSXiLinks.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.sCode,GetXIQSName(c.FKiQSDefinitionID,sDatabase),GetXIQSStepName(c.FKiStepDefinitionID,sDatabase),GetSectionName(c.FKiSectionDefinitionID,sDatabase),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetXIQSCodeName(int s, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var TypeName = dbContext.QSXiLink.ToList().Where(m => m.ID == Convert.ToInt32(s)).Select(m => m.sName).FirstOrDefault();
            return TypeName;
        }
        public VMCustomResponse SaveXIQSLinks(XIQSLinks model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var FKiAppID = UserDetais.FKiApplicationID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            XIQSLinks oLink = new XIQSLinks();
            if (model.ID == 0)
            {
                oLink.ID = model.ID;
                oLink.sCode = model.sCode;
                oLink.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oLink.FKiStepDefinitionID = model.FKiStepDefinitionID;
                oLink.FKiSectionDefinitionID = model.FKiSectionDefinitionID;
                oLink.FKiApplicationID = FKiAppID;
                oLink.OrganisationID = iOrgID;
                dbContext.QSLink.Add(oLink);
                dbContext.SaveChanges();
            }
            else
            {
                oLink = dbContext.QSLink.Find(model.ID);
                oLink.ID = model.ID;
                oLink.sCode = model.sCode;
                oLink.FKiQSDefinitionID = model.FKiQSDefinitionID;
                oLink.FKiStepDefinitionID = model.FKiStepDefinitionID;
                oLink.FKiSectionDefinitionID = model.FKiSectionDefinitionID;
                oLink.FKiApplicationID = FKiAppID;
                oLink.OrganisationID = iOrgID;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oLink.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }
        public XIQSLinks GetXIQSXiLinkByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            XIQSLinks oGetQSXiLinkData = new XIQSLinks();
            oGetQSXiLinkData = dbContext.QSLink.Find(ID);
            return oGetQSXiLinkData;
        }

        #endregion XIQSXiLinksMap

        //CopyXISemanticsByXISemanticID
        public int CopyXISemanticsByID(int ID, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, "", sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            cQSDefinition oQSD = new cQSDefinition();
            oQSD = dbContext.QSDefinition.Where(m => m.ID == ID).FirstOrDefault();
            oQSD.sName = oQSD.sName + " Copy";
            dbContext.QSDefinition.Add(oQSD);
            dbContext.SaveChanges();
            var QSStepDef = dbContext.QSStepDefiniton.Where(s => s.FKiQSDefintionID == ID).ToList();
            if (QSStepDef != null && QSStepDef.Count() > 0)
            {
                foreach (var oStepD in QSStepDef)
                {
                    int iMainStepID = oStepD.ID;
                    cQSStepDefiniton oOldStepD = new cQSStepDefiniton();
                    oOldStepD = dbContext.QSStepDefiniton.Where(m => m.ID == oStepD.ID).FirstOrDefault();
                    var Sections = dbContext.StepSectionDefinition.Where(m => m.FKiStepDefinitionID == oStepD.ID).ToList();
                    oOldStepD.FKiQSDefintionID = oQSD.ID;
                    dbContext.QSStepDefiniton.Add(oOldStepD);
                    dbContext.SaveChanges();
                    if (oStepD.iDisplayAs != 20)
                    {
                        var QSStepDefs = dbContext.XIFieldDefinition.Where(s => s.FKiXIStepDefinitionID == iMainStepID).ToList();
                        var oSecCompo = dbContext.XIComponentParams.Where(w => w.iStepDefinitionID == iMainStepID).ToList();
                        if (QSStepDefs != null && QSStepDefs.Count() > 0)
                        {
                            foreach (var oXIField in QSStepDefs)
                            {
                                cFieldDefinition oXIFieldD = new cFieldDefinition();
                                oXIFieldD = dbContext.XIFieldDefinition.Where(r => r.ID == oXIField.ID).FirstOrDefault();
                                oXIFieldD.FKiStepSectionID = 0;
                                oXIFieldD.FKiXIStepDefinitionID = oOldStepD.ID;
                                dbContext.XIFieldDefinition.Add(oXIFieldD);
                                dbContext.SaveChanges();
                            }
                        }
                        if (oSecCompo != null && oSecCompo.Count() > 0)
                        {
                            foreach (var oXICompo in oSecCompo)
                            {
                                cXIComponentParams oCompD = new cXIComponentParams();
                                oCompD = dbContext.XIComponentParams.Where(i => i.iStepDefinitionID == oXICompo.iStepDefinitionID).FirstOrDefault();
                                oCompD.iStepDefinitionID = oOldStepD.ID;
                                oCompD.sName = oXICompo.sName;
                                oCompD.sValue = oXICompo.sValue;
                                dbContext.XIComponentParams.Add(oCompD);
                                dbContext.SaveChanges();
                            }
                        }
                        //if (SecQSLinks != null && SecQSLinks.Count() > 0)
                        //{
                        //    foreach (var QSLink in SecQSLinks)
                        //    {
                        //        XIQSLinks CopyLink = new XIQSLinks();
                        //        CopyLink = dbContext.QSLink.Where(m => m.ID == QSLink.ID).FirstOrDefault();
                        //        CopyLink.FKiStepDefinitionID = CopyStep.ID;
                        //        CopyLink.FKiSectionDefinitionID = CopySec.ID;
                        //        CopyLink.FKiQSDefinitionID = CopyQS.ID;
                        //        dbContext.QSLink.Add(CopyLink);
                        //        dbContext.SaveChanges();
                        //    }
                        //}
                    }
                    else if (oStepD.iDisplayAs == 20 && Sections != null && Sections.Count() > 0)
                    {
                        foreach (var oSecD in Sections)
                        {
                            cStepSectionDefinition oOldSecD = new cStepSectionDefinition();
                            oOldSecD = dbContext.StepSectionDefinition.Where(n => n.ID == oSecD.ID).FirstOrDefault();
                            var oFieldDefs = dbContext.XIFieldDefinition.Where(m => m.FKiStepSectionID == oSecD.ID).ToList();
                            var SecCompo = dbContext.XIComponentParams.Where(m => m.iStepSectionID == oSecD.ID).ToList();
                            var SecQSLinks = dbContext.QSLink.Where(m => m.FKiSectionDefinitionID == oSecD.ID).ToList();
                            oOldSecD.FKiStepDefinitionID = oOldStepD.ID;
                            dbContext.StepSectionDefinition.Add(oOldSecD);
                            dbContext.SaveChanges();
                            if (oFieldDefs != null && oFieldDefs.Count() > 0)
                            {
                                foreach (var XIField in oFieldDefs)
                                {
                                    cFieldDefinition oXIFieldD = new cFieldDefinition();
                                    oXIFieldD = dbContext.XIFieldDefinition.Where(r => r.ID == XIField.ID).FirstOrDefault();
                                    oXIFieldD.FKiStepSectionID = oOldSecD.ID;
                                    oXIFieldD.FKiXIStepDefinitionID = oOldStepD.ID;
                                    oXIFieldD.FKiApplicationID = FKiAppID;
                                    oXIFieldD.OrganisationID = OrgID;
                                    dbContext.XIFieldDefinition.Add(oXIFieldD);
                                    dbContext.SaveChanges();
                                }
                            }
                            if (SecCompo != null && SecCompo.Count() > 0)
                            {
                                foreach (var XICompo in SecCompo)
                                {
                                    cXIComponentParams oCompD = new cXIComponentParams();
                                    oCompD = dbContext.XIComponentParams.Where(i => i.iStepSectionID == XICompo.iStepSectionID).FirstOrDefault();
                                    oCompD.iStepSectionID = oOldSecD.ID;
                                    oCompD.sName = XICompo.sName;
                                    oCompD.sValue = XICompo.sValue;
                                    dbContext.XIComponentParams.Add(oCompD);
                                    dbContext.SaveChanges();
                                }
                            }
                            //if (SecQSLinks != null && SecQSLinks.Count() > 0)
                            //{
                            //    foreach (var QSLink in SecQSLinks)
                            //    {
                            //        XIQSLinks CopyLink = new XIQSLinks();
                            //        CopyLink = dbContext.QSLink.Where(m => m.ID == QSLink.ID).FirstOrDefault();
                            //        CopyLink.FKiStepDefinitionID = CopyStep.ID;
                            //        CopyLink.FKiSectionDefinitionID = CopySec.ID;
                            //        CopyLink.FKiQSDefinitionID = CopyQS.ID;
                            //        dbContext.QSLink.Add(CopyLink);
                            //        dbContext.SaveChanges();
                            //    }
                            //}
                        }
                    }
                }
            }
            return 0;
        }

        #region XIQSDefinitionStages

        public VMCustomResponse SaveEditXIQSStages(cXIQSStages model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIQSStages oStage = new cXIQSStages();
            if (model.SVs != null && model.SVs.Count() > 0)
            {
                foreach (var item in model.SVs)
                {
                    if (item.ID == 0 && item.iChildStage != 0)
                    {
                        oStage.FKiQSDefinitionID = model.FKiQSDefinitionID;
                        oStage.iStage = model.iStage;
                        oStage.iChildStage = item.iChildStage;
                        oStage.iType = item.iType;
                        oStage.StatusTypeID = model.StatusTypeID;
                        oStage.CreatedBy = model.CreatedBy;
                        oStage.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oStage.UpdatedBy = model.UpdatedBy;
                        oStage.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oStage.CreatedTime = DateTime.Now;
                        oStage.UpdatedTime = DateTime.Now;
                        dbContext.XIQSStages.Add(oStage);
                        dbContext.SaveChanges();
                    }
                    else if (item.ID != 0 && item.iChildStage != 0)
                    {
                        oStage = dbContext.XIQSStages.Find(item.ID);
                        oStage.FKiQSDefinitionID = model.FKiQSDefinitionID;
                        oStage.iStage = model.iStage;
                        oStage.iChildStage = item.iChildStage;
                        oStage.iType = item.iType;
                        oStage.StatusTypeID = model.StatusTypeID;
                        oStage.UpdatedBy = model.UpdatedBy;
                        oStage.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oStage.UpdatedTime = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                }

            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oStage.ID, Status = true };
        }

        public cXIQSStages GetStagesByQSDefID(int iQSDefID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIQSStages oStage = new cXIQSStages();
            oStage.SVs = new List<cXIQSStages>();
            var AllStages = dbContext.XIQSStages.Where(m => m.FKiQSDefinitionID == iQSDefID).ToList();
            if (AllStages.Count() > 0)
            {
                oStage.FKiQSDefinitionID = iQSDefID;
                oStage.iStage = AllStages.FirstOrDefault().iStage;
                oStage.StatusTypeID = AllStages.FirstOrDefault().StatusTypeID;
                foreach (var stage in AllStages)
                {
                    cXIQSStages oST = new cXIQSStages();
                    oST.ID = stage.ID;
                    oST.iChildStage = stage.iChildStage;
                    oST.iType = stage.iType;
                    oStage.SVs.Add(oST);
                }
            }
            return oStage;
        }

        public int DeleteXIQSStageByID(int ID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var oQSStage = dbContext.XIQSStages.Find(ID);
            dbContext.XIQSStages.Remove(oQSStage);
            dbContext.SaveChanges();
            return ID;
        }

        #endregion XIQSDefinitionStages       


        public List<VMDropDown> GetQuoteStages(int iUserID, string sOrgName, string sDatabase)
        {
            List<VMDropDown> oQuote = new List<VMDropDown>();
            CXiAPI oXIAPI = new CXiAPI();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int OrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = UserDetails.sUserDatabase;
            var sBODataSource = string.Empty;
            sBODataSource = oXIAPI.GetBODataSource(12, UserDetails.FKiOrgID, "Zeeinsurance", sOrgDB);
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "SELECT ID, sName FROM RefTraceStage_T";
                SqlDataReader Newreader = cmd.ExecuteReader();
                while (Newreader.Read())
                {
                    VMDropDown oDrop = new VMDropDown();
                    var sID = Newreader["ID"].ToString();
                    oDrop.Value = Convert.ToInt32(sID);
                    oDrop.text = Newreader["sName"].ToString();
                    oQuote.Add(oDrop);
                }
                Newreader.Close();
                Con.Close();
            }
            return oQuote;
        }

    }
}
