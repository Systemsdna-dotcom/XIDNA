using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class CommonRepository
    {
        BOs oBO = new BOs();
        cXICache Cacheobj = new cXICache();
        string sDatabaseName = "";

        #region UserDetails

        public VM_cXIAppUserDetails GetUserDetails(int UserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            VM_cXIAppUserDetails oUser = new VM_cXIAppUserDetails();
            if (UserID > 0)
            {
                var User = dbCore.XIAppUsers.Find(UserID);
                var RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
                var Role = dbCore.XIAppRoles.Find(RoleID);
                oUser.UserID = UserID;
                if (User != null)
                {
                    oUser.sCoreDatabase = User.sCoreDatabaseName;
                    oUser.sUserDatabase = User.sDatabaseName;
                    oUser.iRoleID = RoleID;
                    oUser.FKiOrgID = User.FKiOrganisationID;
                    oUser.sUserName = User.sUserName;
                    oUser.sAppName = User.sAppName;
                    oUser.FKiApplicationID = User.FKiApplicationID;
                    if (Role != null)
                    {
                        oUser.sRoleName = Role.sRoleName;
                        oUser.iThemeID = Role.iThemeID;
                        oUser.iLayoutID = Role.iLayoutID;
                        oUser.sThemeName = dbContext.Types.Where(m => m.ID == Role.iThemeID).Select(m => m.FileName).FirstOrDefault();
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(sOrgName))
                {
                    var OrgDetails = dbCore.Organization.Where(m => m.Name == sOrgName).FirstOrDefault();
                    oUser.FKiOrgID = OrgDetails.ID;
                    oUser.sUserDatabase = OrgDetails.DatabaseName;
                }
                return oUser;
            }
            return oUser;
        }

        #endregion UserDetails

        #region ErrorLog
        public void SaveErrorLog(string Error, string sDatabase, string sCode = "")
        {
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                ErrorLogs Errors = new ErrorLogs();
                Errors.Description = Error;
                Errors.CreatedBySYSID = ServiceUtil.GetIPAddress(); //Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Errors.CreatedTime = DateTime.Now;
                if (!string.IsNullOrEmpty(sCode))
                {
                    Errors.sCode = sCode;
                }
                int iUserID = 0;
                if (HttpContext.Current != null)
                {
                    if (HttpContext.Current.Session["UserID"] != null)
                    {
                        int.TryParse(HttpContext.Current.Session["UserID"].ToString(), out iUserID);
                    }
                    string sUser = string.Empty;
                    if (iUserID > 0)
                    {
                        sUser = HttpContext.Current.Session["sUserName"].ToString();
                    }
                    if (string.IsNullOrEmpty(sUser))
                    {
                        sUser = "Public without Login";
                    }
                    Errors.CreatedByID = iUserID;
                    Errors.CreatedByName = sUser;

                }
                if (!string.IsNullOrEmpty(Errors.Description))
                {
                    dbContext.ErrorLogs.Add(Errors);
                    dbContext.SaveChanges();
                }                
            }
            catch (Exception ex)
            {
                ErrorLogs Errors = new ErrorLogs();
                Errors.Description = Error;
                Errors.CreatedBySYSID = ServiceUtil.GetIPAddress();
                Errors.CreatedTime = DateTime.Now;
                if (!string.IsNullOrEmpty(Errors.Description))
                {
                    dbContext.ErrorLogs.Add(Errors);
                    dbContext.SaveChanges();
                }                    
            }
        }

        public DTResponse GetErrorsList(jQueryDataTableParamModel param, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<ErrorLogs> AllTypes;
            AllTypes = dbContext.ErrorLogs;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTypes = AllTypes.Where(m => m.Description.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTypes.Count();
            AllTypes = QuerableUtil.GetResultsForDataTables(AllTypes, "", sortExpression, param);
            var clients = AllTypes.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Description, c.CreatedTime.ToString("dd-MMM-yyyy"),"" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public ErrorLogs GetErrorByID(int ErrorID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ErrorLogs Error = new ErrorLogs();
            Error = dbContext.ErrorLogs.Find(ErrorID);
            return Error;
        }
        #endregion ErrorLog

        #region LayoutDetails

        public VMPopupLayout GetLayoutDetails(int LayoutID, int PopupOrDialogID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase)
        {
            //XIDNA
            ModelDbContext dbContext = new ModelDbContext();
            VMPopupLayout Layout = new VMPopupLayout();
            string sGUID = string.Empty;
            if (LayoutID > 0)
            {
                Layout = dbContext.Layouts.Where(m => m.ID == LayoutID).Select(m => new VMPopupLayout { LayoutID = m.ID, LayoutName = m.LayoutName, LayoutCode = m.LayoutCode, XiParameterID = m.XiParameterID, LayoutType = m.LayoutType, Authentication = m.Authentication, iThemeID = m.iThemeID }).FirstOrDefault();
                if (Layout.iThemeID > 0)
                {
                    Layout.sThemeName = dbContext.Types.Where(m => m.ID == Layout.iThemeID).FirstOrDefault().FileName;
                }
                //var Mappings = dbContext.PopupLayoutMappings.Where(m=>m.PopupLayoutID==LayoutID).Select(m=> new VMPopupLayoutMappings{})
                var Mappings = new List<VMPopupLayoutMappings>();
                if (PopupOrDialogID > 0)
                {
                    Mappings = (from pm in dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == LayoutID && m.PopupID == PopupOrDialogID && m.StatusTypeID == 10)
                                join pd in dbContext.PopupLayoutDetails on pm.PlaceHolderID equals pd.PlaceHolderID
                                select new VMPopupLayoutMappings
                                {
                                    MappingID = pm.ID,
                                    XiLinkID = pm.XiLinkID,
                                    PlaceHolderID = pd.PlaceHolderID,
                                    PlaceholderName = pd.PlaceholderName,
                                    HTMLCode = pm.HTMLCode,
                                    ContentType = pm.ContentType,
                                    IsValueSet = pm.IsValueSet == true ? "True" : "False"
                                }).ToList();
                }
                else
                {
                    Mappings = (from pm in dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == LayoutID && m.StatusTypeID == 10)
                                join pd in dbContext.PopupLayoutDetails on pm.PlaceHolderID equals pd.PlaceHolderID
                                select new VMPopupLayoutMappings
                                {
                                    MappingID = pm.ID,
                                    XiLinkID = pm.XiLinkID,
                                    PlaceHolderID = pd.PlaceHolderID,
                                    PlaceholderName = pd.PlaceholderName,
                                    HTMLCode = pm.HTMLCode,
                                    ContentType = pm.ContentType,
                                    IsValueSet = pm.IsValueSet == true ? "True" : "False"
                                }).ToList();
                }


                var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == LayoutID).Select(m => new VMPopupLayoutDetails { PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, LayoutID = m.LayoutID, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
                Layout.Mappings = Mappings;
                Layout.Details = Details;
                //NInstance();
                if (Layout.XiParameterID > 0)
                {
                    if (BOID > 0)
                    {
                        oBO = dbContext.BOs.Where(m => m.BOID == BOID).FirstOrDefault();
                        sDatabaseName = sDatabase;
                    }
                    sGUID = AddXiParametersToCache(LayoutID, Layout.XiParameterID, ID, BOID, sNewGUID, iUserID, sOrgName, sDatabase);
                }
                if (Layout.LayoutType.ToLower() == "inline" || Layout.LayoutType.ToLower() == "template")
                {
                    Layout.sGUID = Guid.NewGuid().ToString();
                }
            }
            if (!string.IsNullOrEmpty(sGUID))
            {
                Layout.sGUID = sGUID;
            }

            return Layout;
        }

        private string AddXiParametersToCache(int LayoutID, int XiParameterID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiParameters Parameter = new XiParameters();
            Parameter = dbContext.XiParameters.Where(m => m.XiParameterID == XiParameterID).FirstOrDefault();
            var ParameterNVs = Parameter.XiParameterNVs;
            var sSessionID = HttpContext.Current.Session.SessionID;
            string sGUID = sNewGUID;
            if (!string.IsNullOrEmpty(sGUID))
            {
                var sParentGUID = Cacheobj.Get_ParamVal(sSessionID, sGUID, null, "{XIP|ParentGUID}");
                if (!string.IsNullOrEmpty(sParentGUID))
                {
                    sGUID = sParentGUID;
                }
                else
                {
                    sParentGUID = Cacheobj.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ParentGUID}", sGUID.ToString(), null, null);
                }
            }

            //if (ID > 0)
            //{
            string sBOName = string.Empty;
            var oBO = dbContext.BOs.Find(BOID);
            if (oBO != null)
            {
                sBOName = oBO.Name;
            }
            CInstance oCache = Cacheobj.Get_XICache();

            var BOFields = dbContext.BOFields.Where(m => m.FKTableName == sBOName).ToList();
            if (BOFields != null && BOFields.Count() > 0)
            {
                var ActiveFK = BOFields.FirstOrDefault().Name;
                Cacheobj.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveFK}", ActiveFK.ToString(), null, null);
            }
            Cacheobj.Set_ParamVal(sSessionID, sGUID, null, "{XIP|ActiveBO}", sBOName.ToString(), null, null);
            if (ID > 0)
            {
                Cacheobj.Set_ParamVal(sSessionID, sGUID, null, "{XIP|" + sBOName + ".id}", ID.ToString(), null, null);
            }

            Cacheobj.Set_ParamVal(sSessionID, sGUID, null, "{XIP|BODID}", BOID.ToString(), null, null);

            //}
            //CInstance oCacheobj = Get_XICache();
            return sGUID;
        }


        public List<VMDropDown> GetXiParametersDDL(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            List<VMDropDown> XiParameters = new List<VMDropDown>();
            XiParameters = dbContext.XiParameters.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).ToList().Where(m => m.StatusTypeID == ServiceConstants.StatusTypeID).Select(m => new VMDropDown { Value = m.XiParameterID, text = m.Name }).ToList();
            XiParameters.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return XiParameters;
        }

        #endregion LayoutDetails

        #region DropDowns

        public List<VMDropDown> GetScriptLanguageDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Languages = new List<VMDropDown>();
            Languages = dbContext.Types.Where(m => m.Name == "ScriptLanguages").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return Languages;
        }

        public List<VMDropDown> GetScriptTypeDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> ScriptTypes = new List<VMDropDown>();
            ScriptTypes = dbContext.Types.Where(m => m.Name == "ScriptTypes").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return ScriptTypes;
        }

        public List<VMDropDown> GetStatusTypeDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> StatusTypes = new List<VMDropDown>();
            StatusTypes = dbContext.Types.Where(m => m.Name == "Status Type").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return StatusTypes;
        }

        public List<VMDropDown> GetScriptLevelDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> ScriptTypes = new List<VMDropDown>();
            ScriptTypes = dbContext.Types.Where(m => m.Name == "ScriptLevel").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return ScriptTypes;
        }

        public List<VMDropDown> GetScriptCategoryDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> ScriptTypes = new List<VMDropDown>();
            ScriptTypes = dbContext.Types.Where(m => m.Name == "ScriptCategory").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return ScriptTypes;
        }
        public List<VMDropDown> GetScriptClassificationDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> ScriptTypes = new List<VMDropDown>();
            ScriptTypes = dbContext.Types.Where(m => m.Name == "ScriptClassification").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return ScriptTypes;
        }

        public List<VMDropDown> GetSystemTypesDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> SystemTypes = new List<VMDropDown>();
            SystemTypes = dbContext.Types.Where(m => m.Name == "Sys Type").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return SystemTypes;
        }

        public List<VMDropDown> GetBOsDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> BOs = new List<VMDropDown>();
            BOs = dbContext.BOs.Where(m => m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.BOID, text = m.Name }).ToList();
            BOs.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return BOs;
        }
        public List<VMDropDown> GetIOServerDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> IOServerList = new List<VMDropDown>();
            IOServerList = dbContext.IOServerDetails.Where(m => m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.ServerName }).ToList();
            IOServerList.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return IOServerList;
        }

        public List<VMDropDown> GetBOClassesDDL(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> BOClassess = new List<VMDropDown>();
            BOClassess = dbContext.BOClassAttributes.Where(m => m.BOID == BOID && m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Class }).ToList();
            return BOClassess;
        }

        public List<VMDropDown> GetXiLinksDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> XiLinks = new List<VMDropDown>();
            XiLinks = dbContext.XiLinks.Where(m => m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.XiLinkID, text = m.Name }).ToList();
            XiLinks.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return XiLinks;
        }

        public List<VMDropDown> GetXIComponentsDDL(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> XIComponents = new List<VMDropDown>();
            XIComponents = dbContext.XIComponents.Where(m => m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            XIComponents.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return XIComponents;
        }

        public List<VMDropDown> GetReportsDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Reports = new List<VMDropDown>();
            Reports = dbContext.Reports.Where(m => m.StatusTypeID == 10).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Name }).ToList();
            Reports.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return Reports;
        }

        public List<VMDropDown> PopulateDDL(string sDDLType, int iUserID, string sOrgName, string sDatabase)
        {
            List<VMDropDown> DDL = new List<VMDropDown>();
            if (sDDLType.ToLower() == "XI Component".ToLower())
            {
                DDL = GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
            }
            else if (sDDLType.ToLower() == "1-Click".ToLower())
            {
                DDL = GetReportsDDL(sDatabase);
            }
            return DDL;
        }

        public List<VMDropDown> GetXIDataTypesDDL(string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> DataTypes = new List<VMDropDown>();
            DataTypes = dbContext.XIDataTypes.ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            return DataTypes;
        }
        public List<VMDropDown> GetXIVisualisationsDDL(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllXIVisualisations = new List<VMDropDown>();
            var oVisualisationList = dbContext.XiVisualisations.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).ToList();
            AllXIVisualisations.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            foreach (var items in oVisualisationList)
            {
                AllXIVisualisations.Add(new VMDropDown
                {
                    Value = items.XiVisualID,
                    text = items.Name.ToString()
                });
            }
            return AllXIVisualisations;
        }

        public List<VMDropDown> GetThemesDDL(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            List<VMDropDown> ThemeTypes = new List<VMDropDown>();
            ThemeTypes = dbContext.Types.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).Where(m => m.Name.ToLower() == "Themes".ToLower()).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            ThemeTypes.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return ThemeTypes;
        }

        public List<VMDropDown> GetLayoutsDDL(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Layouts = new List<VMDropDown>();
            Layouts = dbContext.Layouts.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganizationID == iOrgID).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.LayoutName }).ToList();
            Layouts.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return Layouts;
        }

        public List<VMDropDown> GetTemplateLayoutsDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Layouts = new List<VMDropDown>();
            Layouts = dbContext.Layouts.Where(m => m.LayoutType.ToLower() == "template").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.LayoutName }).ToList();
            Layouts.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return Layouts;
        }

        public List<VMDropDown> GetLayoutMappingsDDL(int iLayoutID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Layouts = new List<VMDropDown>();
            Layouts = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == iLayoutID).ToList().Select(m => new VMDropDown { Value = m.PlaceHolderID, text = m.PlaceholderName }).ToList();
            Layouts.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return Layouts;
        }

        public List<VMDropDown> GetQuestionsetsDDL(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> QuestionSets = new List<VMDropDown>();
            QuestionSets = dbContext.QSDefinition.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.FKiOrganisationID == iOrgID).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            //QuestionSets.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return QuestionSets;
        }

        public List<VMDropDown> GetQSStepsDDL(int iQSDefinitionID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Steps = new List<VMDropDown>();
            Steps = dbContext.QSStepDefiniton.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.FKiQSDefintionID == iQSDefinitionID).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            //Steps.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return Steps;
        }

        public List<VMDropDown> GetQSSectionsDDL(int iQSStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Sections = new List<VMDropDown>();
            Sections = dbContext.StepSectionDefinition.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.FKiStepDefinitionID == iQSStepID).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            Sections.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return Sections;
        }

        public List<VMDropDown> GetQSFieldsDDL(int iQSStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Fields = new List<VMDropDown>();
            var StepFields = dbContext.XIFieldDefinition.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiXIStepDefinitionID == iQSStepID).Select(m => m.FKiXIFieldOriginID).ToList();
            Fields = dbContext.XIFieldOrigin.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.FKiOrganisationID == iOrgID).Where(m => StepFields.Contains(m.ID)).ToList().Select(m => new VMDropDown { Value = m.ID, text = m.sName }).ToList();
            //Fields.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return Fields;
        }

        public List<VMDropDown> GetBOStructuresDDL(int iBOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Structures = new List<VMDropDown>();
            Structures = dbContext.XIStructure.Where(m => m.BOID == iBOID && m.FKiParentID == "#").ToList().Select(m => new VMDropDown { Value = Convert.ToInt32(m.ID), text = m.sStructureName }).ToList();
            return Structures;
        }

        public List<VMDropDown> GetDialogsDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Dialogs = new List<VMDropDown>();
            Dialogs = dbContext.Dialogs.ToList().Select(m => new VMDropDown { Value = m.ID, text = m.DialogName }).ToList();
            return Dialogs;
        }
        public List<VMDropDown> GetApplicationsDDL()
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllBOs = new List<VMDropDown>();
            AllBOs = (from c in dbContext.XIApplications.ToList()
                      select new VMDropDown { text = c.sApplicationName, Value = c.ID }).ToList();
            return AllBOs;
        }
        public List<VMDropDown> GetBOFieldAttributesDDL(int BOID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllBOFields = new List<VMDropDown>();
            AllBOFields = (from c in dbContext.BOFields.Where(m => m.BOID == BOID).ToList()
                           select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return AllBOFields;
        }

        public List<VMDropDown> GetQSTemplatesDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> QSTemplates = new List<VMDropDown>();
            QSTemplates = (from c in dbContext.QSDefinition.Where(m => m.bIsTemplate == true).ToList()
                           select new VMDropDown { text = c.sName, Value = c.ID }).ToList();
            return QSTemplates;
        }

        public List<VMDropDown> GetQSStepTemplatesDDL(int iQSDID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> QSStepTemplates = new List<VMDropDown>();
            QSStepTemplates = (from c in dbContext.QSStepDefiniton.Where(m => m.FKiQSDefintionID == iQSDID).ToList()
                               select new VMDropDown { text = c.sName, Value = c.ID }).ToList();
            return QSStepTemplates;
        }

        public List<VMDropDown> GetStepsDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> QSStepTemplates = new List<VMDropDown>();
            QSStepTemplates = (from c in dbContext.QSStepDefiniton.ToList()
                               select new VMDropDown { text = c.sName, Value = c.ID }).ToList();
            return QSStepTemplates;
        }

        public List<VMDropDown> GetQSScriptsDDL(int FKiScriptID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> ScriptList = new List<VMDropDown>();
            ScriptList = dbContext.BOScripts.ToList().Select(m => new VMDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
            //ScriptList.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return ScriptList;
        }
        public List<VMDropDown> GetQSXiLinksDDL(int ID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> oXiLinkList = new List<VMDropDown>();
            //oXiLinkList = dbContext.QSXiLink.ToList().Select(m => new VMDropDown { Expression = m.sCode, text = m.sName }).ToList();
            var oCodeList = dbContext.QSXiLink.ToList().Select(m => m.sCode).Distinct();
            foreach (var code in oCodeList)
            {
                oXiLinkList.Insert(0, new VMDropDown
                {
                    text = code,
                    Expression = code
                });
            }
            return oXiLinkList;
        }
        public List<VMDropDown> GetXIBOStructuresDDL(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var FKiAppID = GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            List<VMDropDown> Structures = new List<VMDropDown>();
            Structures = dbContext.XIStructure.Where(m => m.FKiXIApplicationID == FKiAppID).Where(m => m.OrganisationID == iOrgID).Where(m => m.FKiParentID == "#").ToList().Select(m => new VMDropDown { Value = Convert.ToInt32(m.ID), text = m.sCode }).ToList();
            Structures.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return Structures;
        }

        public List<VMDropDown> GetSourceDDL(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var FKiAppID = GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            List<VMDropDown> Sources = new List<VMDropDown>();
            Sources = dbContext.XISource.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganisationID == FKiAppID).ToList().Select(m => new VMDropDown { Value = Convert.ToInt32(m.ID), text = m.sName }).ToList();
            Sources.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            return Sources;
        }

        public List<VMDropDown> GetAllUsersDDL(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            List<VMDropDown> Users = new List<VMDropDown>();
            Users = (from c in dbContext.XIAppUsers.ToList()
                     select new VMDropDown { text = c.sUserName, Value = c.UserID }).ToList();
            return Users;
        }

        #endregion DropDowns

        #region ApplicationInfo

        public cXIApplications GetApplicationInfo(string UrlName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIApplications result = new cXIApplications();
            string StrUrlName = GetUrlName(UrlName);
            //if (StrUrlName.ToLower().Trim() == EnumDefaultUrls.CorporateUser.ToString().ToLower().Trim())
            //{
            //    result = new VMOrganizations { OrganizationID = -1 };
            //}
            //else
            //{
            result = dbContext.XIApplications.Where(Q => Q.sApplicationName.ToLower().Trim() == StrUrlName.ToLower().Trim()).FirstOrDefault();
            //}
            result = result ?? new cXIApplications { ID = -1 };
            return result;
        }

        public string GetUrlName(string InputUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(InputUrl))
                {
                    //remove Last special characters
                    string result = string.Empty;
                    char[] chars = { '/', '\\', '@', ':', '*', '&' };
                    result = InputUrl.TrimEnd(chars);
                    int Index_ = result.LastIndexOf('/');
                    string strfinal = result.Substring(Index_, result.Length - Index_);
                    return RemoveSpecialCharacters(strfinal);
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string RemoveSpecialCharacters(string str)
        {
            return Regex.Replace(str, "[^a-zA-Z0-9]+", "", RegexOptions.Compiled);

        }

        public cXIUrlMappings GetURLInfo(string sURL)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIUrlMappings oURL = new cXIUrlMappings();
            oURL = dbContext.XIUrlMappings.Where(m => m.sUrlName.ToLower() == sURL.ToLower()).FirstOrDefault();
            return oURL;
        }

        public cXIApplications GetAppInfo(string sAppName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIApplications oAPP = new cXIApplications();
            oAPP = dbContext.XIApplications.Where(m => m.sApplicationName.ToLower() == sAppName.ToLower()).FirstOrDefault();
            return oAPP;
        }
        public VMCustomResponse ResponseMessage()
        {
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, Status = true };
        }
        #endregion ApplicationInfo
    }
}