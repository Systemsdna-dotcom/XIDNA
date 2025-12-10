using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Web.Caching;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Globalization;
using System.Data;
using XIDNA.Repository;
using System.IO;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using Dapper;
using System.Net;
using System.Data.Entity;

namespace XIDNA.Repository
{
    public class XIComponentsRepository : IXIComponentsRepository
    {
        protected IDbConnection connection;
        CXiAPI oXiAPI = new CXiAPI();
        CommonRepository Common = new CommonRepository();
        public DTResponse XiComponentsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
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
            IQueryable<cXIComponents> AllXiComponents;
            AllXiComponents = dbContext.XIComponents.Where(m => m.FKiApplicationID == fkiApplicationID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXiComponents = AllXiComponents.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXiComponents.Count();
            AllXiComponents = QuerableUtil.GetResultsForDataTables(AllXiComponents, "", sortExpression, param);
            var clients = AllXiComponents.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName, Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public cXIComponents GetXiComponentsByID(int ComponentID, string sName, string sType, int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cXIComponents oComponent = new cXIComponents();
            if (ComponentID > 0)
            {
                oComponent = dbContext.XIComponents.Find(ComponentID);
            }
            else
            {
                oComponent = dbContext.XIComponents.Where(m => m.sName == sName).FirstOrDefault();
            }

            if (string.IsNullOrEmpty(sType))
            {
                oComponent.XIComponentParams.Clear();
                return oComponent;
            }
            if (sType.ToLower() == "QSStep".ToLower())
            {
                if (ID != 0)
                {
                    var nParams = oComponent.XIComponentParams.Where(m => m.iStepDefinitionID == ID).ToList();
                    var newParams = oComponent.XIComponentNVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new cXIComponentParams();
                        Newparm.ID = 0;
                        Newparm.sName = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oComponent.ID;
                        Newparm.iStepDefinitionID = ID;
                        nParams.Add(Newparm);
                    }
                    oComponent.XIComponentParams = nParams;
                }
                else
                {
                    oComponent.XIComponentParams.Clear();
                }
            }
            else if (sType.ToLower() == "QSStepSection".ToLower())
            {
                if (ID != 0)
                {
                    List<cXIComponentParams> Params = new List<cXIComponentParams>();
                    var nParams = oComponent.XIComponentParams.Where(m => m.iStepSectionID == ID).ToList();
                    var Except = oComponent.XIComponentNVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    Params.AddRange(nParams);
                    foreach (var items in Except)
                    {
                        var NewParam = new cXIComponentParams();
                        NewParam.ID = 0;
                        NewParam.FKiComponentID = oComponent.ID;
                        NewParam.iStepSectionID = ID;
                        NewParam.sName = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        NewParam.sValue = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Params.Add(NewParam);
                    }
                    oComponent.XIComponentParams = Params;
                }
                else
                {
                    oComponent.XIComponentParams.Clear();
                }
            }
            else if (sType.ToLower() == "XiLink".ToLower())
            {
                if (ID > 0)
                {
                    var nParams = oComponent.XIComponentParams.Where(m => m.iXiLinkID == ID).ToList();
                    var newParams = oComponent.XIComponentNVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new cXIComponentParams();
                        Newparm.ID = 0;
                        Newparm.sName = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oComponent.ID;
                        Newparm.iXiLinkID = ID;
                        nParams.Add(Newparm);
                    }
                    oComponent.XIComponentParams = nParams;
                }
                else
                {
                    oComponent.XIComponentParams.Clear();
                }
            }
            else if (sType.ToLower() == "Layout".ToLower())
            {
                if (ID > 0)
                {
                    var nParams = oComponent.XIComponentParams.Where(m => m.iLayoutMappingID == ID).ToList();
                    var newParams = oComponent.XIComponentNVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new cXIComponentParams();
                        Newparm.ID = 0;
                        Newparm.sName = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oComponent.ID;
                        Newparm.iLayoutMappingID = ID;
                        nParams.Add(Newparm);
                    }
                    oComponent.XIComponentParams = nParams;
                }
                else
                {
                    oComponent.XIComponentParams.Clear();
                }
            }
            else if (sType.ToLower() == "Query".ToLower())
            {
                if (ID > 0)
                {
                    var nParams = oComponent.XIComponentParams.Where(m => m.iQueryID == ID).ToList();
                    var newParams = oComponent.XIComponentNVs.Select(m => m.sName.ToLower()).ToList().Except(nParams.Select(m => m.sName.ToLower()).ToList()).ToList();
                    foreach (var items in newParams)
                    {
                        var Newparm = new cXIComponentParams();
                        Newparm.ID = 0;
                        Newparm.sName = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sName).FirstOrDefault();
                        Newparm.sValue = oComponent.XIComponentNVs.Where(m => m.sName.ToLower() == items).Select(m => m.sValue).FirstOrDefault();
                        Newparm.FKiComponentID = oComponent.ID;
                        Newparm.iQueryID = ID;
                        nParams.Add(Newparm);
                    }
                    oComponent.XIComponentParams = nParams;
                }
                else
                {
                    oComponent.XIComponentParams.Clear();
                }
            }
            return oComponent;
        }

        public VMCustomResponse SaveXiComponents(VMXIComponents model, int iUserID, string sOrgName, string sDatabase)
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
            cXIComponents oComponent = new cXIComponents();
            if (model.ID == 0)
            {
                oComponent.FKiApplicationID = FKiAppID;
                oComponent.OrganisationID = iOrgID;
                oComponent.sName = model.sName;
                oComponent.sType = model.sType;
                oComponent.sClass = model.sClass;
                oComponent.sHTMLPage = model.sHTMLPage;
                oComponent.StatusTypeID = model.StatusTypeID;
                oComponent.CreatedBy = model.CreatedBy;
                oComponent.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oComponent.UpdatedBy = model.UpdatedBy;
                oComponent.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oComponent.CreatedTime = DateTime.Now;
                oComponent.UpdatedTime = DateTime.Now;
                dbContext.XIComponents.Add(oComponent);
                dbContext.SaveChanges();
            }
            else
            {
                oComponent = dbContext.XIComponents.Find(model.ID);
                oComponent.FKiApplicationID = FKiAppID;
                oComponent.OrganisationID = iOrgID;
                oComponent.sName = model.sName;
                oComponent.sType = model.sType;
                oComponent.sClass = model.sClass;
                oComponent.sHTMLPage = model.sHTMLPage;
                oComponent.StatusTypeID = model.StatusTypeID;
                oComponent.UpdatedBy = model.UpdatedBy;
                oComponent.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oComponent.UpdatedTime = DateTime.Now;
                dbContext.SaveChanges();
                dbContext.XIComponentsNVs.RemoveRange(dbContext.XIComponentsNVs.Where(m => m.FKiComponentID == oComponent.ID));
                dbContext.SaveChanges();
            }
            if (model.NVPairs != null && model.NVPairs.Count() > 0)
            {
                for (int i = 0; i < model.NVPairs.Count(); i++)
                {
                    cXIComponentsNVs oNVs = new cXIComponentsNVs();
                    var Pairs = model.NVPairs[i].ToString().Split('_').ToList();
                    oNVs.sName = Pairs[0];
                    oNVs.sValue = Pairs[1];
                    oNVs.sType = Pairs[2];
                    oNVs.FKiComponentID = oComponent.ID;
                    oNVs.StatusTypeID = model.StatusTypeID;
                    oNVs.CreatedBy = model.CreatedBy;
                    oNVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oNVs.UpdatedBy = model.UpdatedBy;
                    oNVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oNVs.CreatedTime = DateTime.Now;
                    oNVs.UpdatedTime = DateTime.Now;
                    dbContext.XIComponentsNVs.Add(oNVs);
                    dbContext.SaveChanges();
                }
            }
            if (model.TriggerPairs != null && model.TriggerPairs.Count() > 0)
            {
                for (int i = 0; i < model.TriggerPairs.Count(); i++)
                {
                    cXIComponentTriggers oNVs = new cXIComponentTriggers();
                    var Pairs = model.TriggerPairs[i].ToString().Split('_').ToList();
                    oNVs.sName = Pairs[0];
                    oNVs.sValue = Pairs[1];
                    oNVs.FKiComponentID = oComponent.ID;
                    dbContext.XIComponentTriggers.Add(oNVs);
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oComponent.ID, Status = true };
        }

        public cXIComponents XIInitialise(int iXIComponentID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            cXIComponents oXIComponent = new cXIComponents();
            oXIComponent = dbContext.XIComponents.Find(iXIComponentID);
            return oXIComponent;
        }

        public string SaveLayoutComponentParams(cXIComponents oComponent, string sType, int iLoadID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string IDs = string.Empty;
            ModelDbContext dbContext = new ModelDbContext();
            if (sType.ToLower() == "Layout".ToLower() && iLoadID > 0)
            {
                var Params = dbContext.XIComponentParams.Where(m => m.iLayoutMappingID == iLoadID).ToList();
                dbContext.XIComponentParams.RemoveRange(Params);
                dbContext.SaveChanges();
            }
            if (sType.ToLower() == "XiLink".ToLower() && iLoadID > 0)
            {
                var Params = dbContext.XIComponentParams.Where(m => m.iXiLinkID == iLoadID).ToList();
                dbContext.XIComponentParams.RemoveRange(Params);
                dbContext.SaveChanges();
            }
            if (sType.ToLower() == "QSStep".ToLower() && iLoadID > 0)
            {
                var Params = dbContext.XIComponentParams.Where(m => m.iStepDefinitionID == iLoadID).ToList();
                dbContext.XIComponentParams.RemoveRange(Params);
                dbContext.SaveChanges();
            }
            if (sType.ToLower() == "QSStepSection".ToLower() && iLoadID > 0)
            {
                var Params = dbContext.XIComponentParams.Where(m => m.iStepSectionID == iLoadID).ToList();
                dbContext.XIComponentParams.RemoveRange(Params);
                dbContext.SaveChanges();
            }
            if (sType.ToLower() == "Query".ToLower() && iLoadID > 0)
            {
                var Params = dbContext.XIComponentParams.Where(m => m.iQueryID == iLoadID).ToList();
                dbContext.XIComponentParams.RemoveRange(Params);
                dbContext.SaveChanges();
            }
            if (oComponent.XIComponentNVs != null)
            {
                foreach (var items in oComponent.XIComponentNVs)
                {
                    cXIComponentParams oParams = new cXIComponentParams();
                    oParams.FKiComponentID = oComponent.ID;
                    oParams.sName = items.sName;
                    oParams.sValue = items.sValue;
                    if (sType.ToLower() == "Layout".ToLower())
                    {
                        oParams.iLayoutMappingID = iLoadID;
                    }
                    else if (sType.ToLower() == "XiLink".ToLower())
                    {
                        oParams.iXiLinkID = iLoadID;
                    }
                    else if (sType.ToLower() == "QSStep".ToLower())
                    {
                        oParams.iStepDefinitionID = iLoadID;
                    }
                    else if (sType.ToLower() == "QSStepSection".ToLower())
                    {
                        oParams.iStepSectionID = iLoadID;
                    }
                    else if (sType.ToLower() == "Query".ToLower())
                    {
                        oParams.iQueryID = iLoadID;
                    }
                    dbContext.XIComponentParams.Add(oParams);
                    dbContext.SaveChanges();
                    IDs = IDs + oParams.ID + ",";
                }
            }
            if (oComponent.XIComponentTriggers != null)
            {
                foreach (var items in oComponent.XIComponentTriggers)
                {
                    cXIComponentParams oParams = new cXIComponentParams();
                    oParams.FKiComponentID = oComponent.ID;
                    oParams.sName = items.sName;
                    if (sType.ToLower() == "Layout".ToLower())
                    {
                        oParams.iLayoutMappingID = iLoadID;
                    }
                    else if (sType.ToLower() == "XiLink".ToLower())
                    {
                        oParams.iXiLinkID = iLoadID;
                    }
                    else if (sType.ToLower() == "QSStep".ToLower())
                    {
                        oParams.iXiLinkID = iLoadID;
                    }
                    else if (sType.ToLower() == "Query".ToLower())
                    {
                        oParams.iQueryID = iLoadID;
                    }
                    oParams.sValue = items.sValue;
                    dbContext.XIComponentParams.Add(oParams);
                    dbContext.SaveChanges();
                    IDs = IDs + oParams.ID + ",";
                }
            }
            if (IDs.Length > 0)
            {
                IDs = IDs.Substring(0, IDs.Length - 1);
            }
            return IDs;
        }

        public VMCustomResponse UpdateMappingIDToParams(string sType, int iLoadID, string Params, int iUserID, string sOrgName, string sDatabase)
        {
            string MainDb = sDatabase;
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var iPlaceHolderID = dbContext.PopupLayoutMappings.Where(m => m.ID == iLoadID).Select(m => m.PlaceHolderID).FirstOrDefault();
            var IDs = Params.Split(',').ToList();
            if (sType.ToLower() == "Layout".ToLower())
            {
                foreach (var items in IDs)
                {
                    var Param = dbContext.XIComponentParams.Find(Convert.ToInt32(items));
                    Param.iLayoutMappingID = iPlaceHolderID;
                    dbContext.SaveChanges();
                }
                var Mappings = dbContext.PopupLayoutMappings.Find(iPlaceHolderID);
                if (Mappings != null)
                {
                    Mappings.IsValueSet = true;
                }
                dbContext.SaveChanges();
            }
            if (sType.ToLower() == "XiLink".ToLower())
            {
                foreach (var items in IDs)
                {
                    var Param = dbContext.XIComponentParams.Find(Convert.ToInt32(items));
                    Param.iXiLinkID = iPlaceHolderID;
                    dbContext.SaveChanges();
                }
            }
            if (sType.ToLower() == "Query".ToLower())
            {
                foreach (var items in IDs)
                {
                    var Param = dbContext.XIComponentParams.Find(Convert.ToInt32(items));
                    Param.iQueryID = iPlaceHolderID;
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse() { Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public List<cXIComponentParams> GetComponentParmsByStep(int StepID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<cXIComponentParams> Params = new List<cXIComponentParams>();
            Params = dbContext.XIComponentParams.Where(m => m.iStepSectionID == StepID).ToList();
            return Params;
        }

        #region RunComponent

        public List<cTreeView> GetTreeStructure(List<cNameValuePairs> oTreeParams, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            List<cNameValuePairs> BOLevels = new List<cNameValuePairs>();
            List<cNameValuePairs> FKs = new List<cNameValuePairs>();
            if (oTreeParams.Count() > 0)
            {
                BOLevels = oTreeParams.Where(m => m.sName.ToLower().Contains("BOLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
                FKs = oTreeParams.Where(m => m.sName.ToLower().Contains("FKLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            }
            //else
            //{
            //    BOLevels = oTreeParams.Where(m => m.sName.ToLower().Contains("BOLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            //    FKs = oTreeParams.Where(m => m.sName.ToLower().Contains("FKLevel".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).ToList();
            //}
            List<cTreeView> oTree = new List<cTreeView>();
            foreach (var items in BOLevels)
            {
                //var Params = oXIComponent.XIComponentNVs.Where(m => m.sName == items.sName).FirstOrDefault();
                if (!string.IsNullOrEmpty(items.sValue))
                {
                    var oBO = oXiAPI.Get_BOInstance(items.sValue, "", iUserID, sOrgName, sDatabase);
                    var Group = oBO.Definition.BOGroups.Where(m => m.GroupName.ToLower() == "Label Group".ToLower()).FirstOrDefault();
                    var sTableName = string.Empty;
                    if (!string.IsNullOrEmpty(oBO.Definition.TableName))
                    {
                        sTableName = oBO.Definition.TableName;
                    }
                    else
                    {
                        sTableName = oBO.Definition.Name;
                    }
                    var Level = items.sName.ToLower().Replace("BOLevel".ToLower(), "");
                    if (Level.Length > 0)
                    {
                        string Query = string.Empty;
                        var FKID = FKs.Where(m => m.sName.ToLower() == "FKLevel".ToLower() + Level).Select(m => m.sValue).FirstOrDefault();
                        if (FKID != null && FKID.Length > 0)
                        {
                            Query = "select concat('" + oBO.Definition.Name + "','_', ID) as sID, " + Group.BOFieldNames + " as sText, concat('" + oXiAPI.GetFKTableName(FKID, sDatabase) + "','_'," + FKID + ") as sParentID from " + sTableName;
                        }
                        else
                        {
                            Query = "select concat('" + oBO.Definition.Name + "','_', ID) as sID, " + Group.BOFieldNames + " as sText from " + sTableName;
                        }
                        //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
                        var result = new List<cTreeView>();
                        using (SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
                        {
                            Conn.Open();
                            Conn.ChangeDatabase(sOrgDB);
                            result = Conn.Query<cTreeView>(Query).ToList<cTreeView>();
                            Conn.Close();
                        }
                        oTree.AddRange(result);
                    }
                }
            }
            return oTree;
        }

        public cBODisplay GetFormComponent(cXIComponents oXIComponent, int iUserID, string sOrgName, string sDatabase)
        {
            cBODisplay oBO = new cBODisplay();
            cNameValuePairs BOs = new cNameValuePairs();
            cNameValuePairs Group = new cNameValuePairs();
            if (oXIComponent.XIComponentParams != null && oXIComponent.XIComponentParams.Count() > 0)
            {
                BOs = oXIComponent.XIComponentParams.Where(m => m.sName.ToLower().Contains("BO".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).FirstOrDefault();
                Group = oXIComponent.XIComponentParams.Where(m => m.sName.ToLower().Contains("Group".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).FirstOrDefault();
            }
            else
            {
                BOs = oXIComponent.XIComponentNVs.Where(m => m.sName.ToLower().Contains("BO".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).FirstOrDefault();
                Group = oXIComponent.XIComponentNVs.Where(m => m.sName.ToLower().Contains("Group".ToLower())).Select(m => new cNameValuePairs { sName = m.sName, sValue = m.sValue }).FirstOrDefault();
            }
            string sBOName = string.Empty;
            string sGroup = string.Empty;
            if (!string.IsNullOrEmpty(BOs.sName) && BOs.sName.ToLower() == "BO".ToLower())
            {
                sBOName = BOs.sValue;
            }
            if (!string.IsNullOrEmpty(Group.sName) && Group.sName.ToLower() == "Group".ToLower())
            {
                sGroup = Group.sValue;
            }
            oBO = oXiAPI.GetFormData(sBOName, sGroup, 0, string.Empty, iUserID, sDatabase, sOrgName, null);

            return oBO;
        }

        #endregion RunComponent

        #region XITreeStructure
        public List<cXIStructure> GetXITreeStructure(int iBODID, string sCode, int iUserID, string sOrgName, string sDatabase)
        {
            List<cXIStructure> oTree = new List<cXIStructure>();
            ModelDbContext dbContext = new ModelDbContext();
            List<BOFields> BOFldList = new List<BOFields>();
            var sNodesAdded = new List<string>();
            //Check if the structure table for code
            cXIStructure sMainNode = dbContext.XIStructure.Where(m => m.BOID == iBODID).Where(m => m.sCode == sCode).Where(m => m.FKiParentID == "#").FirstOrDefault();
            List<cXIStructure> oXITree = new List<cXIStructure>();
            oXITree.Add(sMainNode);
            if (sMainNode != null)
            {
                oTree = XIStructTree(oXITree, sCode, sDatabase, oTree, sNodesAdded);
            }
            else
            {
                //oXITree.Add(new cXIStructure { ID = sStructure.ID, FKiParentID = "#", sName = oBO.Name, BOID = iBOID });
            }
            return oTree;
        }
        public List<cXIStructure> XIStructTree(List<cXIStructure> XIStrTrees, string sCode, string sDatabase, List<cXIStructure> XITree, List<string> sNodesAdded)
        {
            ModelDbContext dbContext = new ModelDbContext();
            //List<cXIStructure> Strut = new List<cXIStructure>();
            foreach (var items in XIStrTrees)
            {
                string sCheckNodeName = sNodesAdded.FirstOrDefault(stringToCheck => stringToCheck.Contains(items.sBO));
                if (sCheckNodeName == null)
                {
                    var NodeDetails = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == items.ID).FirstOrDefault();
                    if (NodeDetails != null)
                    {
                        items.i1ClickID = NodeDetails.i1ClickID;
                    }
                    XITree.Add(items);
                    var ID = items.ID;
                    var SubXITreeNodes = dbContext.XIStructure.Where(m => m.FKiParentID == ID.ToString() && m.StatusTypeID == 10 && m.sCode == sCode).ToList();
                    sNodesAdded.Add(items.sBO);
                    if (SubXITreeNodes.Count() > 0)
                    {
                        XIStructTree(SubXITreeNodes, sCode, sDatabase, XITree, sNodesAdded);
                    }
                    else
                    {
                        var XIStructure = dbContext.XIStructure.Where(m => m.FKiParentID == "#" && m.BOID == items.BOID && m.StatusTypeID == 10 && m.sCode == sCode).ToList();
                        if (XIStructure.Count() > 0)
                        {
                            int iReplaceID = Convert.ToInt32(XIStructure.Where(m => m.StatusTypeID == 10 && m.sCode == sCode).Select(m => m.ID).FirstOrDefault());
                            //Replace the ID
                            XITree.Where(m => m.sName == items.sName).ToList().ForEach(s => s.ID = iReplaceID);
                            XIStructTree(XIStructure, sCode, sDatabase, XITree, sNodesAdded);
                        }
                    }
                }
                else
                {
                    var iID = items.ID;
                    var XIStructure = dbContext.XIStructure.Where(m => m.FKiParentID == iID.ToString() && m.StatusTypeID == 10 && m.sCode == sCode).ToList();
                    if (XIStructure.Count() > 0)
                    {
                        XIStructTree(XIStructure, sCode, sDatabase, XITree, sNodesAdded);
                    }
                }
            }
            return XITree;
        }

        public cBOUIDetails GetBOStructure1Click(string StructureName, int NodeID)
        {
            cBOUIDetails oBODetail = new cBOUIDetails();
            ModelDbContext dbContext = new ModelDbContext();
            var BOStructure = dbContext.XIStructure.Where(m => m.sCode.ToLower() == StructureName.ToLower() && m.ID == NodeID).FirstOrDefault();
            if (BOStructure != null)
            {
                if (BOStructure.FKiParentID == "#")
                {
                    oBODetail = dbContext.BOUIDetails.Where(m => m.FKiBOID == BOStructure.BOID).FirstOrDefault();
                    oBODetail.sBOName = BOStructure.sBO;
                }
                else
                {
                    var Structure = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == NodeID).FirstOrDefault();
                    if (Structure != null)
                    {
                        oBODetail.i1ClickID = Structure.i1ClickID;
                    }
                }
            }
            return oBODetail;
        }
        #endregion

        #region QuestionSetComponent

        public List<cXIStructure> GetXIStructureTreeDetails(int BOID, string sCode)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<cXIStructure> XIStrTrees = new List<cXIStructure>();
            List<cXIStructure> Tree = new List<cXIStructure>();
            //XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == iMainID.ToString()).ToList();
            XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == "#" && m.sCode.ToLower() == sCode.ToLower() && m.BOID == BOID).ToList();

            //XIStrTrees.Add(XIParntTrees);
            if (XIStrTrees != null && XIStrTrees.Count() > 0)
            {
                Tree = XITree(XIStrTrees, new List<cXIStructure>());
            }
            else
            {
                Tree = new List<cXIStructure>();
            }
            // XIStrTrees.Add(XIParntTrees);
            return Tree;
        }

        public List<cXIStructure> XITree(List<cXIStructure> XIStrTrees, List<cXIStructure> Tree)
        {
            ModelDbContext dbContext = new ModelDbContext();
            //List<cXIStructure> Strut = new List<cXIStructure>();
            foreach (var items in XIStrTrees)
            {
                Tree.Add(items);
                var ID = items.ID;
                var SubXITreeNodes = dbContext.XIStructure.Where(m => m.FKiParentID == ID.ToString() && m.StatusTypeID == 10).OrderBy(m => m.iOrder).ToList();
                if (SubXITreeNodes.Count() > 0)
                {
                    XITree(SubXITreeNodes, Tree);
                }
            }
            return Tree;
        }


        public cQSDefinition GetQuestionSetComponent(int iBODID, string sCode, string sMode, string sQSName, int iUserID, string OrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cQSDefinition oQS = new cQSDefinition();
            cQSDefinition FixedTemplate = new cQSDefinition();
            FixedTemplate.QSSteps = new List<cQSStepDefiniton>();
            oQS = dbContext.QSDefinition.Where(m => m.sName.ToLower() == sQSName.ToLower()).FirstOrDefault();
            var NewSteps = new List<cQSStepDefiniton>();
            List<cNameValuePairs> oTreeParams = new List<cNameValuePairs>();
            var oTree = GetXIStructureTreeDetails(iBODID, sCode);
            if (oQS != null)
            {
                int i = -1;
                var Steps = dbContext.QSStepDefiniton.Where(m => m.FKiQSDefintionID == oQS.ID).ToList();
                oQS.QSSteps = new List<cQSStepDefiniton>();
                oQS.QSSteps.AddRange(Steps);
                foreach (var items in oTree)
                {
                    if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Single" || items.sMode == "SingleView"))
                    {
                        cQSStepDefiniton oStep = new cQSStepDefiniton();
                        oStep = oQS.QSSteps.Where(m => m.sName.ToLower() == "Single Fixed Step".ToLower()).FirstOrDefault();
                        var ExistingParams = dbContext.XIComponentParams.Where(m => m.iStepDefinitionID == oStep.ID).ToList();
                        if (ExistingParams.Count() > 0)
                        {
                            ExistingParams.ToList().ForEach(m => m.iStepDefinitionID = 0);
                            dbContext.SaveChanges();
                        }
                        var ComponentDefinition = dbContext.XIComponents.Where(m => m.ID == oStep.iXIComponentID).FirstOrDefault();
                        List<cXIComponentParams> nParms = new List<cXIComponentParams>();
                        nParms.Add(new cXIComponentParams() { sName = "BO", sValue = items.sBO });
                        nParms.Add(new cXIComponentParams() { sName = "Group", sValue = "Create" });
                        if (items.sMode == "SingleView")
                        {
                            nParms.Add(new cXIComponentParams() { sName = "DisplayMode", sValue = "View" });
                        }
                        ComponentDefinition.XIComponentParams.Clear();
                        ComponentDefinition.XIComponentParams = nParms;
                        oStep.ComponentDefinition = ComponentDefinition;
                        NewSteps.Add(oStep);
                    }
                    else if (!string.IsNullOrEmpty(items.sMode) && (items.sMode == "Multiple" || items.sMode == "MultipleView"))
                    {
                        var BOUI1ClickID = 0;
                        var BOStructure = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == items.ID).FirstOrDefault();
                        if (BOStructure != null)
                        {
                            BOUI1ClickID = BOStructure.i1ClickID;
                        }
                        cQSStepDefiniton oStep = new cQSStepDefiniton();
                        var StepDef = oQS.QSSteps.Where(m => m.sName.ToLower() == "Multiple Fixed Step".ToLower()).FirstOrDefault();
                        var ComDef = new cXIComponents();
                        ComDef.ID = StepDef.iXIComponentID;
                        var ExistingParams = dbContext.XIComponentParams.Where(m => m.iStepSectionID == i).ToList();
                        if (ExistingParams.Count() > 0)
                        {
                            foreach (var parm in ExistingParams)
                            {
                                using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
                                {
                                    Conn.Open();
                                    SqlCommand SqlCmd = new SqlCommand();
                                    SqlCmd.Connection = Conn;
                                    string cmdText = "delete from XIComponentParams_T where id =" + parm.ID;
                                    SqlCommand cmd = new SqlCommand(cmdText, Conn);
                                    cmd.ExecuteNonQuery();
                                    Conn.Close();
                                }
                            }
                            //ExistingParams.ToList().ForEach(m => m.iStepDefinitionID = 0);

                        }
                        List<cXIComponentParams> nParms = new List<cXIComponentParams>();
                        nParms.Add(new cXIComponentParams() { sName = "1ClickID", sValue = BOUI1ClickID.ToString(), FKiComponentID = StepDef.iXIComponentID, iStepSectionID = StepDef.ID });
                        nParms.Add(new cXIComponentParams() { sName = "Register", sValue = "yes", FKiComponentID = StepDef.iXIComponentID, iStepSectionID = StepDef.ID });
                        if (items.sMode == "MultipleView")
                        {
                            nParms.Add(new cXIComponentParams() { sName = "DisplayMode", sValue = "View" });
                        }
                        foreach (var param in nParms)
                        {
                            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
                            {
                                Conn.Open();
                                SqlCommand SqlCmd = new SqlCommand();
                                SqlCmd.Connection = Conn;
                                string cmdText = "INSERT INTO XIComponentParams_T(sName, sValue, FKiComponentID, iStepSectionID) VALUES('" + param.sName + "','" + param.sValue + "', " + param.FKiComponentID + "," + i + ")";
                                SqlCommand cmd = new SqlCommand(cmdText, Conn);
                                cmd.ExecuteNonQuery();
                                Conn.Close();
                            }
                        }


                        ComDef.ID = StepDef.iXIComponentID;
                        ComDef.XIComponentParams = nParms;
                        oStep.ComponentDefinition = ComDef;
                        oStep.ID = i;
                        NewSteps.Add(oStep);
                        i--;
                    }
                }
            }
            FixedTemplate.QSSteps = NewSteps;
            return FixedTemplate;
        }

        #endregion QuestionSetComponent

        #region BO Components
        public cBODisplay GetBOComponentData(int iBODID, int iBOIID, string sGroupName, string sLoadGroup, string sLockGroup, int iUserID, string sOrgName, string sDatabase)
        {
            CXiAPI oXIAPI = new CXiAPI();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            DataContext Spdb = new DataContext(sOrgDB);
            BOs oBO = dbContext.BOs.Find(iBODID);

            cBODisplay oBODisplay = new cBODisplay();
            var oBOInstance = oXIAPI.Get_BOInstance(oBO.Name, sGroupName, iUserID, sOrgName, sDatabase);//GroupName
            var GroupFields = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == sGroupName.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            var LockFields = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == sLockGroup.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            var lLockFields = LockFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var LoadFields = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == sLoadGroup.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            var lLoadFields = LockFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            string TableName = string.Empty;
            if (!string.IsNullOrEmpty(oBOInstance.Definition.TableName))
            {
                TableName = oBOInstance.Definition.TableName;
            }
            else
            {
                TableName = oBOInstance.Definition.Name;
            }
            List<string[]> Rows = new List<string[]>();
            string sWhereClause = string.Empty;
            //int FKiStepInstanceID = 0;
            //int FKiSectionInstanceID = 0;
            //if (nWCParams != null && nWCParams.Count() > 0)
            //{
            //    string sContext = nWCParams.Where(m => m.sName.ToLower() == "context".ToLower()).Select(m => m.sValue).FirstOrDefault();
            //    if (sContext.ToLower() == "QSStep".ToLower())
            //    {
            //        FKiStepInstanceID = Convert.ToInt32(nWCParams.Where(m => m.sName.ToLower() == "FKiStepInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault());
            //        if (FKiStepInstanceID > 0)
            //        {
            //            sWhereClause = sWhereClause + "FKiStepInstanceID = " + FKiStepInstanceID;
            //        }

            //    }
            //    else if (sContext.ToLower() == "QSStepSection".ToLower())
            //    {
            //        FKiSectionInstanceID = Convert.ToInt32(nWCParams.Where(m => m.sName.ToLower() == "FKiSectionInstanceID".ToLower()).Select(m => m.sValue).FirstOrDefault());
            //        if (FKiSectionInstanceID > 0)
            //        {
            //            sWhereClause = sWhereClause + "FKiSectionInstanceID = " + FKiSectionInstanceID;
            //        }
            //    }
            //}
            //if (sWhereClause.Length > 0)
            //{
            //    sWhereClause = " Where " + sWhereClause;
            //}
            //var Columns = new List<string>();
            // sWhereClause = " Where ID=" + iBOIID;           
            if (iBOIID > 0 || sWhereClause.Length > 0)
            {
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    var SelectQuery = "Select " + GroupFields;
                    var SelectPart = ServiceUtil.GetFKLabelGroup(oBO, null, SelectQuery, iUserID, sOrgName, sDatabase);
                    //if (!string.IsNullOrEmpty(sWhereClause))
                    //{
                    //    cmd.CommandText = SelectPart + " from " + TableName + sWhereClause;
                    //}
                    //else
                    //{
                    cmd.CommandText = SelectPart + " from " + TableName + " Where ID = " + iBOIID;
                    //}
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    Rows = data.Rows.Cast<DataRow>()
      .Select(row => data.Columns.Cast<DataColumn>()
         .Select(col => Convert.ToString(row[col]))
      .ToArray())
   .ToList();
                    Con.Dispose();
                }
                if (Rows.Count() > 0)
                {
                    var RowData = Rows[0];
                    var Fields = GroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    for (int i = 0; i < Fields.Count(); i++)
                    {
                        var Format = oBOInstance.Definition.BOFields.Where(m => m.Name.ToLower() == Fields[i].ToLower()).Select(m => m.Format).FirstOrDefault();
                        if (Format != null)
                        {
                            string sFormattedValue = string.Empty;
                            if (sGroupName != "Save Group")
                            {
                                sFormattedValue = oXIAPI.FormatValue(Fields[i], Rows[0][i], Format);
                            }
                            if (!string.IsNullOrEmpty(sFormattedValue))
                            {
                                oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = sFormattedValue;
                            }
                            else
                            {
                                oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = RowData[i];
                            }
                        }
                        else
                        {
                            oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().sValue = RowData[i];
                        }

                        oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().bDirty = true;
                        //Lock fields
                        if (lLockFields.Contains(Fields[i]))
                        {
                            oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().bLock = true;
                        }
                        //Load fields
                        if (lLoadFields.Contains(Fields[i]))
                        {
                            oBOInstance.NVPairs.Where(m => m.sName.ToLower() == Fields[i].ToLower()).FirstOrDefault().bLoad = true;
                        }
                    }
                    //Lock fields
                    //var lLockFields = LockFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    //for (int i = 0; i < lLockFields.Count(); i++)
                    //{
                    //    oBOInstance.NVPairs.Where(m => m.sName.ToLower() == lLockFields[i].ToLower()).FirstOrDefault().bLock = true;
                    //}
                }
            }
            oBOInstance = oXIAPI.RunGroupFieldsQuery(oBOInstance, iUserID, sOrgName, sDatabase);
            oBODisplay.BOInstance = oBOInstance;
            List<VMDropDown> FieldDDL = new List<VMDropDown>();
            List<VMDropDown> ImagePathDetails = new List<VMDropDown>();
            var iMasterDataID = oBOInstance.Definition.BOFields.Where(m => m.iMasterDataID > 0).ToList();
            foreach (var items in iMasterDataID)
            {
                FieldDDL = dbContext.Types.Where(m => m.Code == items.iMasterDataID).ToList().Select(m => new VMDropDown { ID = m.ID, Expression = m.Expression }).ToList();
                oBODisplay.BOInstance.Definition.BOFields.Where(m => m.ID == items.ID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            }
            var IsOptionalList = oBOInstance.Definition.BOFields.Where(m => m.IsOptionList == true).ToList();
            foreach (var item in IsOptionalList)
            {
                FieldDDL = dbContext.BOOptionLists.Where(m => m.BOID == item.BOID).Where(m => m.BOFieldID == item.ID).Where(m => m.Name == item.Name).ToList().Select(m => new VMDropDown { Type = m.sValues, Expression = m.sOptionName }).ToList();
                oBODisplay.BOInstance.Definition.BOFields.Where(m => m.ID == item.ID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            }
            var FkList = oBOInstance.Definition.BOFields.Where(m => m.FKiType > 0 && m.FKTableName != null).ToList();
            foreach (var item in FkList)
            {
                //var sTableName = item.FKTableName + "_T";
                var sTableName = item.FKTableName;
                var BO = dbContext.BOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    dbContext = new ModelDbContext(sDatabase);
                    if (BO != null)
                    {
                        var LabelGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.LabelGroup.ToString()).FirstOrDefault().BOSqlFieldNames;
                        if (!string.IsNullOrEmpty(LabelGroup))
                        {
                            cmd.CommandText = "(Select ID, " + LabelGroup + " from " + sTableName + ")";
                        }
                    }
                    // cmd.CommandText = "select id,LocationName from "+sTableName+"";
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        FieldDDL.Add(new VMDropDown
                        {
                            ID = reader.GetInt32(0),
                            Expression = reader.GetString(1)
                        });
                    }
                    Con.Close();
                }
                oBODisplay.BOInstance.Definition.BOFields.Where(m => m.FKiType > 0 && m.FKTableName != null).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            }
            var ImageData = oBOInstance.Definition.BOFields.Where(m => m.FKiFileTypeID > 0).ToList();
            foreach (var img in ImageData)
            {
                foreach (var listdata in ImageData)
                {
                    var sName = listdata.Name;
                    var sNewValue = oBOInstance.NVPairs.Where(m => m.sName == sName).Select(m => m.sValue).FirstOrDefault();
                    if (!string.IsNullOrEmpty(sNewValue))
                    {
                        var sPairValues = sNewValue.Split(',').Select(Int32.Parse).ToList();
                        if (iOrgID > 0)
                        {
                            var sDatabases = sOrgDB;
                            dbContext = new ModelDbContext(sDatabases);
                        }
                        ImagePathDetails = dbContext.XIDocs.Where(m => m.FKiDocType == img.FKiFileTypeID && sPairValues.Contains(m.ID)).ToList().Select(m => new VMDropDown { ID = m.ID, text = m.FileName, Type = m.SubDirectoryPath }).ToList();
                        foreach (var imges in ImagePathDetails)
                        {
                            var DocID = Convert.ToInt32(imges.ID);
                            var XIDoc = new XIDocs();
                            if (iOrgID > 0)
                            {
                                var sDatabases = sOrgDB;
                                dbContext = new ModelDbContext(sDatabases);
                                XIDoc = dbContext.XIDocs.Find(DocID);
                            }
                            else
                            {
                                XIDoc = dbContext.XIDocs.Find(DocID);
                            }
                            dbContext = new ModelDbContext(sDatabase);
                            var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                            var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                            imges.Expression = Path;
                        }
                    }
                }
                oBODisplay.BOInstance.Definition.BOFields.Where(m => m.ID == img.ID).Select(m => { m.ImagePathDetails = ImagePathDetails; return m; }).ToList();
            }
            //var iOneClickID = oBOInstance.Definition.BOFields.Where(m => m.iOneClickID > 0).ToList();
            //foreach (var ClickID in iOneClickID)
            //{
            //    var iClickID = ClickID.iOneClickID;
            //    if (iClickID > 0)
            //    {
            //        SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString());
            //        Con.Open();
            //        Con.ChangeDatabase(sOrgDB);
            //        SqlCommand cmd = new SqlCommand();
            //        cmd.Connection = Con;
            //        dbContext = new ModelDbContext(sDatabase);
            //        cmd.CommandText = dbContext.Reports.Where(m => m.ID == iClickID).Select(m => m.Query).FirstOrDefault();
            //        SqlDataReader reader = cmd.ExecuteReader();
            //        while (reader.Read())
            //        {
            //            FieldDDL.Add(new VMDropDown
            //            {
            //                ID = reader.GetInt32(0),
            //                Expression = reader.GetString(1)
            //            });
            //        }
            //        Con.Close();
            //        oBODisplay.BOInstance.Definition.BOFields.Where(m => m.iOneClickID == iClickID).Select(m => { m.FieldDDL = FieldDDL; return m; }).ToList();
            //    }
            //}
            return oBODisplay;
        }

        public cBOInstance GetBOComponentQuickView(int BOIID, string TableName, string GroupName, int iUserID, string sOrgName, string sDatabase)
        {
            CXiAPI oXIAPI = new CXiAPI();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            DataContext Spdb = new DataContext(sOrgDB);
            //Get BO details based on TableName
            string sTableName = TableName;
            int iBOID = dbContext.BOs.Where(m => m.TableName == sTableName).Select(m => m.BOID).FirstOrDefault();
            BOs oBO = dbContext.BOs.Find(iBOID);
            cBOInstance oBOInstance = oXIAPI.Get_BOInstance(oBO.Name, GroupName, iUserID, sOrgName, sDatabase);//GroupName
            var sSummaryGroupFields = oBOInstance.Definition.BOGroups.Where(m => m.GroupName.ToLower() == GroupName.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            var lSummaryGroupFields = sSummaryGroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var sGroupResult = new List<cNameValuePairs>();
            if (GroupName != null)
            {
                //string sTableName = string.Empty;
                List<string[]> Rows = new List<string[]>();
                //if (string.IsNullOrEmpty(oBOInstance.Definition.TableName))
                //{
                //    sTableName = oBOInstance.Definition.Name;
                //}
                //else
                //{
                //    sTableName = oBOInstance.Definition.TableName;
                //}
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    cmd.CommandText = "Select " + sSummaryGroupFields + " from " + sTableName + " WHERE ID=" + BOIID;
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    Rows = data.Rows.Cast<DataRow>()
      .Select(row => data.Columns.Cast<DataColumn>()
         .Select(col => Convert.ToString(row[col]))
      .ToArray())
    .ToList();
                    Con.Dispose();
                    //var GroupFields = Group.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    if (Rows.Count() > 0)
                    {
                        for (int i = 0; i < lSummaryGroupFields.Count(); i++)
                        {
                            cNameValuePairs oNV = new cNameValuePairs();
                            oNV.sName = lSummaryGroupFields[i];
                            oNV.sValue = Rows[0][i];
                            sGroupResult.Add(oNV);
                        }
                    }
                }
            }

            foreach (var items in sGroupResult)
            {
                var Def = oBOInstance.Definition.BOFields.Where(m => m.Name == items.sName).FirstOrDefault();
                if (Def.Format != null)
                {
                    var sFormattedValue = oXIAPI.FormatValue(items.sName, items.sValue, Def.Format);
                    if (!string.IsNullOrEmpty(sFormattedValue))
                    {
                        items.sValue = sFormattedValue;
                    }
                }
                else if (Def.FKiFileTypeID > 0)
                {
                    var ItemFields = items.sValue.Split(',').ToList();
                    var FieldValue = "";
                    for (int i = 0; i < ItemFields.Count(); i++)
                    {
                        FieldValue = ItemFields[i];
                        if (!string.IsNullOrEmpty(FieldValue) && FieldValue.Length > 0 && Convert.ToInt32(FieldValue) > 0)
                        {
                            var DocID = Convert.ToInt32(FieldValue);
                            if (DocID > 0)
                            {
                                if (iOrgID > 0)
                                {
                                    var sNewDatabase = sOrgDB;
                                    dbContext = new ModelDbContext(sNewDatabase);
                                    var XIDoc = dbContext.XIDocs.Find(DocID);
                                    if (XIDoc != null)
                                    {
                                        var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                                        var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                                        items.sValue = Path;
                                    }
                                    else
                                    {
                                        items.sValue = ConfigurationManager.AppSettings["NoImagePath"];
                                    }
                                }
                                else
                                {
                                    var XIDoc = dbContext.XIDocs.Find(DocID);
                                    if (XIDoc != null)
                                    {
                                        var FileFolder = dbContext.XIDocTypes.Where(m => m.ID == XIDoc.FKiDocType).Select(m => m.Type).FirstOrDefault();
                                        var Path = ConfigurationManager.AppSettings["XIDocsPath"] + "//" + FileFolder + "//" + XIDoc.SubDirectoryPath + "//" + XIDoc.FileName;
                                        items.sValue = Path;
                                    }
                                    else
                                    {
                                        items.sValue = ConfigurationManager.AppSettings["NoImagePath"];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            oBOInstance.NVPairs = sGroupResult;
            return oBOInstance;
        }
        #endregion BO Components

    }
}
