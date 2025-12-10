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
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Xml;

namespace XIDNA.Repository
{
    public class XiLinkRepository : IXiLinkRepository
    {
        CommonRepository Common = new CommonRepository();
        cXICache oCache = new cXICache();
        CXiAPI oXIAPI = new CXiAPI();
        SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString());
        SqlCommand cmd = new SqlCommand();
        #region AddEditXiLink

        public int RemoveXilinkID(int XiLinkID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var model = dbContext.XiLinkNVs.Where(m => m.XiLinkID == XiLinkID);
            dbContext.XiLinkNVs.RemoveRange(model);
            dbContext.SaveChanges();
            return XiLinkID;
        }

        public int RemoveXiVisualID(int iVisualID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var model = dbContext.XiVisualisationNVs.Where(m => m.XiVisualID == iVisualID);
            dbContext.XiVisualisationNVs.RemoveRange(model);
            dbContext.SaveChanges();
            return iVisualID;
        }

        public VMCustomResponse SaveXiLink(VMXiLinks model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetais.FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            XiLinks Link = new XiLinks();
            if (model.XiLinkID == 0)
            {
                Link.FKiApplicationID = FKiAppID;
                Link.OrganisationID = iOrgID;
                Link.Name = model.Name;
                Link.URL = model.URL;
                Link.OneClickID = model.OneClickID;
                Link.FKiComponentID = model.FKiComponentID;
                Link.sActive = model.sActive;
                Link.sType = model.sType;
                Link.StatusTypeID = model.StatusTypeID;
                Link.CreatedBy = model.CreatedBy;
                Link.CreatedBySYSID = Link.CreatedBySYSID; //Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Link.UpdatedBy = model.UpdatedBy;
                Link.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Link.CreatedTime = DateTime.Now;
                Link.UpdatedTime = DateTime.Now;
                dbContext.XiLinks.Add(Link);
                dbContext.SaveChanges();
            }
            else
            {
                Link = dbContext.XiLinks.Find(model.XiLinkID);
                Link.FKiApplicationID = model.FKiApplicationID;
                Link.OrganisationID = iOrgID;
                Link.Name = model.Name;
                Link.URL = model.URL;
                Link.OneClickID = model.OneClickID;
                Link.FKiComponentID = model.FKiComponentID;
                Link.sActive = model.sActive;
                Link.sType = model.sType;
                Link.StatusTypeID = model.StatusTypeID;
                Link.UpdatedBy = model.UpdatedBy;
                Link.UpdatedTime = DateTime.Now;
                Link.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
                dbContext.XiLinkLists.RemoveRange(dbContext.XiLinkLists.Where(m => m.XiLinkID == Link.XiLinkID));
                dbContext.XiLinkNVs.RemoveRange(dbContext.XiLinkNVs.Where(m => m.XiLinkID == Link.XiLinkID));
                dbContext.SaveChanges();
            }
            if (model.NVPairs != null && model.NVPairs.Count() > 0)
            {
                for (int i = 0; i < model.NVPairs.Count(); i++)
                {
                    XiLinkNVs NVs = new XiLinkNVs();
                    var Pairs = model.NVPairs[i].ToString().Split('^').ToList();
                    NVs.Name = Pairs[0];
                    NVs.Value = Pairs[1];
                    NVs.XiLinkID = Link.XiLinkID;
                    NVs.XiLinkListID = 0;
                    NVs.StatusTypeID = model.StatusTypeID;
                    NVs.CreatedBy = model.CreatedBy;
                    NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.UpdatedBy = model.UpdatedBy;
                    NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.CreatedTime = DateTime.Now;
                    NVs.UpdatedTime = DateTime.Now;
                    dbContext.XiLinkNVs.Add(NVs);
                    dbContext.SaveChanges();
                }
            }
            if (model.LNVPairs != null && model.LNVPairs.Count() > 0)
            {
                var AllLists = model.LNVPairs.ToList();
                var ListPairs = model.LNVPairs.Where(m => !m.Contains("^")).Select(m => m).ToList();
                for (int k = 0; k < ListPairs.Count(); k++)
                {
                    XiLinkLists List = new XiLinkLists();
                    List.XiLinkID = Link.XiLinkID;
                    List.ListName = ListPairs[k];
                    List.StatusTypeID = model.StatusTypeID;
                    List.CreatedBy = model.CreatedBy;
                    List.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.UpdatedBy = model.UpdatedBy;
                    List.CreatedTime = DateTime.Now;
                    List.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.UpdatedTime = DateTime.Now;
                    dbContext.XiLinkLists.Add(List);
                    dbContext.SaveChanges();
                    var LNVpairs = AllLists.Where(m => m.StartsWith(ListPairs[k] + "^")).Select(m => m).ToList();
                    for (int i = 0; i < LNVpairs.Count(); i++)
                    {
                        var Pair = LNVpairs[i].ToString().Replace(ListPairs[k] + "^", "").Split('^').ToList();
                        XiLinkNVs NVs = new XiLinkNVs();
                        NVs.Name = Pair[0];
                        NVs.Value = Pair[1];
                        NVs.XiLinkID = Link.XiLinkID;
                        NVs.XiLinkListID = List.XiLinkListID;
                        NVs.StatusTypeID = model.StatusTypeID;
                        NVs.CreatedBy = model.CreatedBy;
                        NVs.UpdatedBy = model.UpdatedBy;
                        NVs.CreatedTime = DateTime.Now;
                        NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        NVs.UpdatedTime = DateTime.Now;
                        dbContext.XiLinkNVs.Add(NVs);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Link.XiLinkID, Status = true };
        }


        public VMXiLinks GetXiLinkByID(int XiLinkID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMXiLinks XiLink = new VMXiLinks();
            XiLinks Link = new XiLinks();
            Link = dbContext.XiLinks.Find(XiLinkID);
            XiLink.FKiApplicationID = Link.FKiApplicationID;
            XiLink.OrganisationID = Link.OrganisationID;
            XiLink.XiLinkID = XiLinkID;
            XiLink.FKiComponentID = Link.FKiComponentID;
            XiLink.Name = Link.Name;
            XiLink.URL = Link.URL;
            XiLink.sType = Link.sType;
            XiLink.sActive = Link.sActive;
            XiLink.OneClickID = Link.OneClickID;
            XiLink.NVs = Link.XiLinkNVs.Where(x => x.XiLinkListID == 0).Select(m => new VMXiLinkNVs { XiLinkID = m.XiLinkID, Name = m.Name, Value = m.Value }).ToList();
            XiLink.Lists = Link.XiLinkLists.Select(m => new VMXiLinkLists { XiLinkID = m.XiLinkID, XiLinkListID = m.XiLinkListID, ListName = m.ListName }).ToList();
            foreach (var items in XiLink.Lists)
            {
                var List = Link.XiLinkLists.Where(m => m.XiLinkListID == items.XiLinkListID).Select(m => m.XiLinkListNVs).FirstOrDefault();
                var NVs = List.Select(m => new VMXiLinkNVs { Name = m.Name, Value = m.Value }).ToList();
                items.NVs = NVs;
            }
            XiLink.StatusTypeID = Link.StatusTypeID;
            return XiLink;
        }

        public DTResponse XiLinksList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            IQueryable<XiLinks> AllXiLinks;
            AllXiLinks = dbContext.XiLinks.Where(m => m.FKiApplicationID == FKiAppID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXiLinks = AllXiLinks.Where(m => m.Name.Contains(param.sSearch) || m.XiLinkID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXiLinks.Count();
            AllXiLinks = QuerableUtil.GetResultsForDataTables(AllXiLinks, "", sortExpression, param);
            var clients = AllXiLinks.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.XiLinkID), c.Name, Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public bool IsExistsXiLinkName(string Name, int XiLinkID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var AllXiLinks = dbContext.XiLinks.ToList();
            var result = AllXiLinks.Where(m => m.Name.ToLower() == Name.ToLower()).FirstOrDefault();
            if (XiLinkID == 0)
            {
                if (result != null)
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
                if (result != null)
                {
                    if (XiLinkID == result.XiLinkID)
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

        //CopyXiLinkByXiLinkID
        public int CopyXiLinkByID(int XiLinkID, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinks CopyXiLink = new XiLinks();
            CopyXiLink = dbContext.XiLinks.Where(m => m.XiLinkID == XiLinkID).FirstOrDefault();
            CopyXiLink.Name = CopyXiLink.Name + " Copy";
            dbContext.XiLinks.Add(CopyXiLink);
            dbContext.SaveChanges();
            var oList = dbContext.XiLinks.Where(s => s.XiLinkID == XiLinkID).FirstOrDefault();
            if (oList.XiLinkNVs.Count() > 0)
            {
                foreach (var LinkNVs in oList.XiLinkNVs.ToList())
                {
                    XiLinkNVs CopyNVs = new XiLinkNVs();
                    CopyNVs = dbContext.XiLinkNVs.Where(m => m.ID == LinkNVs.ID).FirstOrDefault();
                    CopyNVs.XiLinkID = CopyXiLink.XiLinkID;
                    dbContext.XiLinkNVs.Add(CopyNVs);
                    dbContext.SaveChanges();
                }
            }
            if (oList.XiLinkLists.Count() > 0)
            {
                foreach (var LinkLists in oList.XiLinkLists.ToList())
                {
                    XiLinkLists CopyLists = new XiLinkLists();
                    CopyLists = dbContext.XiLinkLists.Where(m => m.XiLinkListID == LinkLists.XiLinkListID).FirstOrDefault();
                    CopyLists.XiLinkID = CopyXiLink.XiLinkID;
                    dbContext.XiLinkLists.Add(CopyLists);
                    dbContext.SaveChanges();
                }
            }
            if (oList.FKiComponentID > 0)
            {
                cXIComponentParams oCopyParams = new cXIComponentParams();
                var ListParams = dbContext.XIComponentParams.Where(m => m.iXiLinkID == oList.XiLinkID).ToList();
                foreach (var Param in ListParams)
                {
                    oCopyParams = dbContext.XIComponentParams.Where(n => n.ID == Param.ID).FirstOrDefault();
                    oCopyParams.iXiLinkID = CopyXiLink.XiLinkID;
                    dbContext.XIComponentParams.Add(oCopyParams);
                    dbContext.SaveChanges();
                }

            }
            return 0;
        }
        #endregion AddEditXiLink


        #region XiParameters
        public VMCustomResponse SaveXiParameter(VMXiParameters model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetais.FKiApplicationID;
            var iOrgID = UserDetais.FKiOrgID;
            XiParameters Parameter = new XiParameters();
            if (model.XiParameterID == 0)
            {
                Parameter.FKiApplicationID = FKiAppID;
                Parameter.OrganisationID = iOrgID;
                Parameter.Name = model.Name;
                Parameter.URL = model.URL;
                Parameter.OneClickID = model.OneClickID;
                Parameter.StatusTypeID = model.StatusTypeID;
                Parameter.CreatedBy = model.CreatedBy;
                Parameter.UpdatedBy = model.UpdatedBy;
                Parameter.CreatedTime = DateTime.Now;
                Parameter.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Parameter.UpdatedTime = DateTime.Now;
                Parameter.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XiParameters.Add(Parameter);
                dbContext.SaveChanges();
            }
            else
            {
                Parameter = dbContext.XiParameters.Find(model.XiParameterID);
                Parameter.FKiApplicationID = model.FKiApplicationID;
                Parameter.OrganisationID = iOrgID;
                Parameter.Name = model.Name;
                Parameter.URL = model.URL;
                Parameter.OneClickID = model.OneClickID;
                Parameter.StatusTypeID = model.StatusTypeID;
                Parameter.UpdatedBy = model.UpdatedBy;
                Parameter.UpdatedTime = DateTime.Now;
                Parameter.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
                dbContext.XiParameterLists.RemoveRange(dbContext.XiParameterLists.Where(m => m.XiParameterID == Parameter.XiParameterID));
                dbContext.XiParameterNVs.RemoveRange(dbContext.XiParameterNVs.Where(m => m.XiParameterID == Parameter.XiParameterID));
                dbContext.SaveChanges();
            }
            if (model.NVPairs != null && model.NVPairs.Count() > 0)
            {
                for (int i = 0; i < model.NVPairs.Count(); i++)
                {
                    XiParameterNVs NVs = new XiParameterNVs();
                    var Pairs = model.NVPairs[i].ToString().Split('^').ToList();
                    NVs.Name = Pairs[0];
                    NVs.Value = Pairs[1];
                    NVs.Type = Pairs[2];
                    NVs.XiParameterID = Parameter.XiParameterID;
                    NVs.XiParameterListID = 0;
                    NVs.StatusTypeID = model.StatusTypeID;
                    NVs.CreatedBy = model.CreatedBy;
                    NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.UpdatedBy = model.UpdatedBy;
                    NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.CreatedTime = DateTime.Now;
                    NVs.UpdatedTime = DateTime.Now;
                    dbContext.XiParameterNVs.Add(NVs);
                    dbContext.SaveChanges();
                }
            }
            if (model.LNVPairs != null && model.LNVPairs.Count() > 0)
            {
                var AllLists = model.LNVPairs.ToList();
                var ListPairs = model.LNVPairs.Where(m => !m.Contains("^")).Select(m => m).ToList();
                for (int k = 0; k < ListPairs.Count(); k++)
                {
                    XiParameterLists List = new XiParameterLists();
                    List.XiParameterID = Parameter.XiParameterID;
                    List.ListName = ListPairs[k];
                    List.StatusTypeID = model.StatusTypeID;
                    List.CreatedBy = model.CreatedBy;
                    List.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.UpdatedBy = model.UpdatedBy;
                    List.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.CreatedTime = DateTime.Now;
                    List.UpdatedTime = DateTime.Now;
                    dbContext.XiParameterLists.Add(List);
                    dbContext.SaveChanges();
                    var LNVpairs = AllLists.Where(m => m.StartsWith(ListPairs[k] + "-")).Select(m => m).ToList();
                    for (int i = 0; i < LNVpairs.Count(); i++)
                    {
                        var Pair = LNVpairs[i].ToString().Replace(ListPairs[k] + "-", "").Split('^').ToList();
                        XiParameterNVs NVs = new XiParameterNVs();
                        NVs.Name = Pair[0];
                        NVs.Value = Pair[1];
                        NVs.Type = Pair[2];
                        NVs.XiParameterID = Parameter.XiParameterID;
                        NVs.XiParameterListID = List.XiParameterListID;
                        NVs.StatusTypeID = model.StatusTypeID;
                        NVs.CreatedBy = model.CreatedBy;
                        NVs.UpdatedBy = model.UpdatedBy;
                        NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        NVs.CreatedTime = DateTime.Now;
                        NVs.UpdatedTime = DateTime.Now;
                        NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        dbContext.XiParameterNVs.Add(NVs);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Parameter.XiParameterID, Status = true };
        }

        public VMXiParameters GetXiParameterByID(int XiParameterID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMXiParameters XiParameter = new VMXiParameters();
            XiParameters Parameter = new XiParameters();
            Parameter = dbContext.XiParameters.Find(XiParameterID);
            XiParameter.FKiApplicationID = Parameter.FKiApplicationID;
            XiParameter.XiParameterID = XiParameterID;
            XiParameter.Name = Parameter.Name;
            XiParameter.URL = Parameter.URL;
            XiParameter.OneClickID = Parameter.OneClickID;
            XiParameter.NVs = Parameter.XiParameterNVs.Where(x => x.XiParameterListID == 0).Select(m => new VMXiParameterNVs { XiParameterID = m.XiParameterID, Name = m.Name, Value = m.Value, Type = m.Type }).ToList();
            XiParameter.Lists = Parameter.XiParameterLists.Select(m => new VMXiParameterLists { XiParameterID = m.XiParameterID, XiParameterListID = m.XiParameterListID, ListName = m.ListName }).ToList();
            foreach (var items in XiParameter.Lists)
            {
                var List = Parameter.XiParameterLists.Where(m => m.XiParameterListID == items.XiParameterListID).Select(m => m.XiParameterListNVs).FirstOrDefault();
                var NVs = List.Select(m => new VMXiParameterNVs { Name = m.Name, Value = m.Value, Type = m.Type }).ToList();
                items.NVs = NVs;
            }
            XiParameter.StatusTypeID = Parameter.StatusTypeID;
            return XiParameter;
        }

        public DTResponse XiParametersList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            IQueryable<XiParameters> AllXiParameters;
            AllXiParameters = dbContext.XiParameters.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXiParameters = AllXiParameters.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXiParameters.Count();
            AllXiParameters = QuerableUtil.GetResultsForDataTables(AllXiParameters, "", sortExpression, param);
            var clients = AllXiParameters.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.XiParameterID), c.Name, Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public XiParameters GetXIParameterDetails(int XiParameterID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiParameters oParam = new XiParameters();
            oParam = dbContext.XiParameters.Find(XiParameterID);
            return oParam;
        }

        #endregion XiParameters

        #region XiVisualisatios

        public VMCustomResponse SaveEditXiVisualisation(VMXiVisualisations model, int iUserID, string sOrgName, string sDatabase)
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
            XiVisualisations Visualisation = new XiVisualisations();
            if (model.XiVisualID == 0)
            {
                Visualisation.FKiApplicationID = FKiAppID;
                Visualisation.OrganisationID = iOrgID;
                Visualisation.Name = model.Name;
                Visualisation.Type = model.Type;
                Visualisation.StatusTypeID = model.StatusTypeID;
                Visualisation.CreatedBy = model.CreatedBy;
                Visualisation.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Visualisation.UpdatedBy = model.UpdatedBy;
                Visualisation.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Visualisation.CreatedTime = DateTime.Now;
                Visualisation.UpdatedTime = DateTime.Now;
                dbContext.XiVisualisations.Add(Visualisation);
                dbContext.SaveChanges();
            }
            else
            {
                Visualisation = dbContext.XiVisualisations.Find(model.XiVisualID);
                Visualisation.FKiApplicationID = FKiAppID;
                Visualisation.OrganisationID = iOrgID;
                Visualisation.Name = model.Name;
                Visualisation.Type = model.Type;
                Visualisation.StatusTypeID = model.StatusTypeID;
                Visualisation.UpdatedBy = model.UpdatedBy;
                Visualisation.UpdatedTime = DateTime.Now;
                Visualisation.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
                dbContext.XiVisualisationLists.RemoveRange(dbContext.XiVisualisationLists.Where(m => m.XiVisualID == Visualisation.XiVisualID));
                dbContext.XiVisualisationNVs.RemoveRange(dbContext.XiVisualisationNVs.Where(m => m.XiVisualID == Visualisation.XiVisualID));
                dbContext.SaveChanges();
            }
            if (model.NVPairs != null && model.NVPairs.Count() > 0)
            {
                for (int i = 0; i < model.NVPairs.Count(); i++)
                {
                    XiVisualisationNVs NVs = new XiVisualisationNVs();
                    var Pairs = model.NVPairs[i].ToString().Split(',').ToList();
                    NVs.sName = Pairs[0];
                    NVs.sValue = Pairs[1];
                    NVs.XiVisualID = Visualisation.XiVisualID;
                    NVs.XiVisualListID = 0;
                    NVs.StatusTypeID = model.StatusTypeID;
                    NVs.CreatedBy = model.CreatedBy;
                    NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.UpdatedBy = model.UpdatedBy;
                    NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    NVs.CreatedTime = DateTime.Now;
                    NVs.UpdatedTime = DateTime.Now;
                    dbContext.XiVisualisationNVs.Add(NVs);
                    dbContext.SaveChanges();
                }
            }
            if (model.LNVPairs != null && model.LNVPairs.Count() > 0)
            {
                var AllLists = model.LNVPairs.ToList();
                var ListPairs = model.LNVPairs.Where(m => !m.Contains(",")).Select(m => m).ToList();
                for (int k = 0; k < ListPairs.Count(); k++)
                {
                    XiVisualisationLists List = new XiVisualisationLists();
                    List.XiVisualID = Visualisation.XiVisualID;
                    List.ListName = ListPairs[k];
                    List.StatusTypeID = model.StatusTypeID;
                    List.CreatedBy = model.CreatedBy;
                    List.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    List.UpdatedBy = model.UpdatedBy;
                    List.CreatedTime = DateTime.Now;
                    List.UpdatedTime = DateTime.Now;
                    dbContext.XiVisualisationLists.Add(List);
                    dbContext.SaveChanges();
                    var LNVpairs = AllLists.Where(m => m.StartsWith(ListPairs[k] + ",")).Select(m => m).ToList();
                    for (int i = 0; i < LNVpairs.Count(); i++)
                    {
                        var Pair = LNVpairs[i].ToString().Replace(ListPairs[k] + ",", "").Split(',').ToList();
                        XiVisualisationNVs NVs = new XiVisualisationNVs();
                        NVs.sName = Pair[0];
                        NVs.sValue = Pair[1];
                        NVs.XiVisualID = Visualisation.XiVisualID;
                        NVs.XiVisualListID = List.XiVisualListID;
                        NVs.StatusTypeID = model.StatusTypeID;
                        NVs.CreatedBy = model.CreatedBy;
                        NVs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        NVs.UpdatedBy = model.UpdatedBy;
                        NVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        NVs.CreatedTime = DateTime.Now;
                        NVs.UpdatedTime = DateTime.Now;
                        dbContext.XiVisualisationNVs.Add(NVs);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Visualisation.XiVisualID, Status = true };
        }

        public VMXiVisualisations GetXiVisualisationByID(int XiVisualID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            VMXiVisualisations XiVisualisation = new VMXiVisualisations();
            XiVisualisations Visualisation = new XiVisualisations();
            Visualisation = dbContext.XiVisualisations.Find(XiVisualID);
            XiVisualisation.FKiApplicationID = Visualisation.FKiApplicationID;
            XiVisualisation.XiVisualID = XiVisualID;
            XiVisualisation.Name = Visualisation.Name;
            XiVisualisation.Type = Visualisation.Type;
            XiVisualisation.NVs = Visualisation.XiVisualisationNVs.Where(x => x.XiVisualListID == 0).Select(m => new VMXiVisualisationNVs { XiVisualID = m.XiVisualID, Name = m.sName, Value = m.sValue }).ToList();
            XiVisualisation.Lists = Visualisation.XiVisualisationLists.Select(m => new VMXiVisualisationLists { XiVisualID = m.XiVisualID, XiVisualListID = m.XiVisualListID, ListName = m.ListName }).ToList();
            foreach (var items in XiVisualisation.Lists)
            {
                var List = Visualisation.XiVisualisationLists.Where(m => m.XiVisualListID == items.XiVisualListID).Select(m => m.XiVisualisationListNVs).FirstOrDefault();
                var NVs = List.Select(m => new VMXiVisualisationNVs { Name = m.sName, Value = m.sValue }).ToList();
                items.NVs = NVs;
            }
            XiVisualisation.StatusTypeID = Visualisation.StatusTypeID;
            return XiVisualisation;
        }

        public DTResponse XiVisualisationsList(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
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
            IQueryable<XiVisualisations> AllXiVisualisations;
            AllXiVisualisations = dbContext.XiVisualisations.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXiVisualisations = AllXiVisualisations.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXiVisualisations.Count();
            AllXiVisualisations = QuerableUtil.GetResultsForDataTables(AllXiVisualisations, "", sortExpression, param);
            var clients = AllXiVisualisations.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.XiVisualID), c.Name,c.Type, Convert.ToString(c.StatusTypeID), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public bool IsExistsXiVisualisationsName(string Name, int XiVisualID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var AllXiVisualisations = dbContext.XiVisualisations.ToList();
            var result = AllXiVisualisations.Where(m => m.Name.ToLower() == Name.ToLower()).FirstOrDefault();
            if (XiVisualID == 0)
            {
                if (result != null)
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
                if (result != null)
                {
                    if (XiVisualID == result.XiVisualID)
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

        #endregion XiVisualisation
        #region CacheObject

        public XiLinks GetXiLinkDetails(int XiLinkID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinks oXilink = new XiLinks();
            var Data = (List<XiLinks>)oCache.GetObjectFromCache("XiLink", null, iUserID, sOrgName, sDatabase, 0);
            oXilink = Data.Where(m => m.XiLinkID == XiLinkID).FirstOrDefault();
            return oXilink;
        }

        #endregion CacheObject

        #region OneClickExecution
        public List<int> GetLoadingType(int QueryID, string sDatabase)
        {
            XmlDocument doc = new XmlDocument();
            ModelDbContext dbContext = new ModelDbContext();
            List<int> Values = new List<int>();
            int Loading = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.ResultListDisplayType).FirstOrDefault();
            Values.Add(Loading);
            var structer = dbContext.Reports.Where(m => m.ParentID == QueryID).ToList();
            if (structer.Count() > 0)
            {
                Values.Add(1);
            }
            else
            {
                Values.Add(0);
            }
            return Values;
        }
        public List<Reports> GetStructuredOneClicks(int OrgID, int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<Reports> OneClicks = new List<Reports>();
            OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.ID == ID).ToList();
            return OneClicks;
            //Reports OneClicks = new Reports();
            //var OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.ParentID>0).ToList();
            //foreach (var items in OneClicks)
            //{
            //    //AllOneClicks.Add(items);
            //    var Added = AllOneClicks.Where(m => m.ID == items.ParentID).FirstOrDefault(); 
            //    if(Added==null){
            //        AllOneClicks.Add(dbContext.Reports.Find(items.ParentID));
            //    }                
            //}
            //return AllOneClicks;
        }
        public VMResultList GetHeadings(int ReportID, string SearchType, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList Preview = new VMResultList();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reports Report = dbContext.Reports.Find(ReportID);
            if (SearchType == EnumSearchType.FilterSearch.ToString())
            {
                Report.IsFilterSearch = true;
                Report.IsNaturalSearch = false;
            }
            else if (SearchType == EnumSearchType.NaturalSearch.ToString())
            {
                Report.IsNaturalSearch = true;
                Report.IsFilterSearch = false;
            }
            var BoFields = dbContext.BOFields.Where(m => m.BOID == Report.BOID).ToList();
            var MapFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == Report.Class).ToList();
            var GroupFields = dbContext.BOGroupFields.Where(m => m.BOID == Report.BOID).ToList();
            if (Report.Query != null)
            {
                int BOID = Report.BOID;
                List<VMDropDown> KeyPositions = new List<VMDropDown>();
                Common Com = new Common();
                var FromIndex = Report.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = Report.Query.Substring(0, FromIndex);
                SelectQuery = SelectQuery.TrimEnd();
                var SelWithGroup = "";
                var regx = new Regex("{.*?}");
                var mathes = regx.Matches(SelectQuery);
                if (mathes.Count > 0)
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            int id = Convert.ToInt32(items.Substring(1, items.Length - 2));
                            var Grp = GroupFields.Where(m => m.ID == id).FirstOrDefault();
                            if (Grp.IsMultiColumnGroup)
                            {
                                SelWithGroup = SelWithGroup + Grp.BOFieldNames + ", ";
                            }
                            else
                            {
                                SelWithGroup = SelWithGroup + Grp.GroupName + ", ";
                            }
                        }
                        else
                        {
                            SelWithGroup = SelWithGroup + items + ", ";
                        }
                    }
                    SelWithGroup = SelWithGroup.Substring(0, SelWithGroup.Length - 2);
                }
                else
                {
                    SelWithGroup = SelectQuery;
                }
                //var Keys = ServiceUtil.GetForeginkeyValues(" " + SelWithGroup);
                var TargetList = Spdb.Targets.Where(m => m.ReportID == ReportID).ToList();
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<int> Targets = new List<int>();
                if (string.IsNullOrEmpty(Report.SelectFields))
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string Head = Regex.Split(items, " as ", RegexOptions.IgnoreCase)[0];
                            Headings.Add(Head);
                        }
                        else
                        {
                            Headings.Add(items);
                        }
                    }
                }
                else
                {
                    Headings = Report.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                List<string> AllHeadings = new List<string>();
                List<string> TableColumns = new List<string>();
                List<int> ColumnReports = new List<int>();
                List<string> MouseOverColumns = new List<string>();
                List<string> Scripts = new List<string>();
                var Columns = new List<string>();
                if (Report.OnClickColumn != null)
                {
                    Columns = Report.OnClickColumn.Split(',').ToList();
                }
                var ColumnIds = new List<string>();
                //if (query.OnColumnClickValue != null)
                //{
                //    ColumnIds = query.OnColumnClickValue;
                //}            
                var str1 = "";
                if (Headings.Contains("ID") == false)
                {
                    //str1 = "No";
                    //Headings.Insert(0, "ID");
                    Preview.IDExists = false;
                }
                else
                {
                    Preview.IDExists = true;
                }
                string allfields = "";
                var groupfieldseditquery2 = "";
                var groupfieldseditquery5 = "";
                if (str1 == "No")
                {
                    //var allfields1 = (query.Query).Insert(7, " ID, ");
                    //allfields = (query.Query).Insert(7, " ID, ");
                    //if (allfields.Contains("ORDER BY") == true && allfields.Contains("GROUP BY") == true)
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //else if (allfields.Contains("GROUP BY") == false)
                    //{
                    //    allfields = allfields1;
                    //}
                    //else
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //if (groupfieldseditquery2 != "")
                    //{
                    //    groupfieldseditquery2 = groupfieldseditquery2 + ", " + "ID" + " ";
                    //    allfields = allfields.Replace(groupfieldseditquery5, groupfieldseditquery2);
                    //}
                    allfields = Report.Query;
                }
                else
                    allfields = Report.Query;
                int i = 0;
                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        string id = items.Substring(1, items.Length - 2);
                        int gid = Convert.ToInt32(id);
                        string groupid = Convert.ToString(gid);
                        BOGroupFields fields = GroupFields.Where(m => m.ID == gid).FirstOrDefault();
                        allfields = allfields.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                        if (fields.IsMultiColumnGroup)
                        {
                            List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                var BoField = BoFields.Where(m => m.Name.ToLower() == names.ToLower()).FirstOrDefault();
                                string aliasname = BoField.LabelName;
                                if (aliasname != null)
                                {
                                    AllHeadings.Add(aliasname);
                                }
                                else
                                {
                                    AllHeadings.Add(names);
                                }
                                TableColumns.Add(names);
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                            }
                        }
                        else
                        {
                            AllHeadings.Add(fields.GroupName);
                            TableColumns.Add(fields.GroupName);
                            Formatting.Add(null);
                            Scripts.Add(null);
                            Targets.Add(0);
                            MouseOverColumns.Add("");
                        }

                    }
                    else
                    {
                        var Fld = items;
                        if (Columns.Contains(items))
                        {
                            int index = Columns.IndexOf(items);
                            //int ID = Convert.ToInt32(ColumnIds[index]);
                            //ColumnReports.Add(ID);
                        }
                        else
                        {
                            //ColumnReports.Add(0);
                        }
                        string aliasname = "";
                        if (OrgID != 0)
                        {
                            aliasname = MapFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                            if (aliasname != null)
                            {
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        else
                        {
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                aliasname = BoField.LabelName;
                            }
                            Formatting.Add(BoField.Format);
                            Scripts.Add(BoField.Script);
                            var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                            if (target != null)
                            {
                                Targets.Add(target.Target);
                            }
                            else
                            {
                                Targets.Add(0);
                            }
                            MouseOverColumns.Add(BoField.FKTableName);
                        }
                        if (aliasname == null)
                        {
                            Fld = items;
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                if (BoField == null && Fld.StartsWith("_"))
                                {
                                    aliasname = Fld.Substring(1, Fld.Length - 1);
                                }
                                else
                                {
                                    aliasname = BoField.LabelName;
                                }

                            }
                            if (BoField != null)
                            {
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                            }
                            else
                            {
                                aliasname = items;
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        //string aliasname = dbContext.BOFields.Where(m => m.Name == items && m.BOID == BOID).Select(m => m.AliasName).FirstOrDefault();                        
                        if (aliasname != null)
                        {
                            AllHeadings.Add(aliasname);
                        }
                        TableColumns.Add(Fld);
                    }
                    i++;
                }
                Preview.BOID = Report.BOID;
                Preview.BO = dbContext.BOs.Where(m => m.BOID == Report.BOID).Select(m => m.Name).FirstOrDefault();
                Preview.Headings = AllHeadings;
                Preview.TableColumns = TableColumns;
                Preview.Formats = Formatting;
                Preview.Scripts = Scripts;
                Preview.Targets = Targets;
                Preview.HeadingReports = ColumnReports;
                Preview.IsRowClick = Report.IsRowClick;
                Preview.XiLinkID = Report.RowXiLinkID;
                Preview.IsExport = Report.IsExport;
                Preview.ResultListDisplayType = Report.ResultListDisplayType;
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
                Preview.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == ReportID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                Preview.QueryName = Report.Name;
                Preview.ShowAs = Report.Title;
                Preview.Query = allfields;
                Preview.FKPositions = KeyPositions;
                var Group = GroupFields.Where(m => m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault();
                if (Group != null)
                {
                    var FilterGroup = Group.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    List<string> FilterFileds = new List<string>();
                    Dictionary<string, string> myDict = new Dictionary<string, string>();
                    foreach (var items in FilterGroup)
                    {
                        myDict[BoFields.Where(m => m.Name.ToLower() == items.ToLower()).Select(m => m.LabelName).FirstOrDefault()] = items;
                    }
                    Preview.FilterGroup = myDict;
                }
                else
                {
                    Preview.FilterGroup = new Dictionary<string, string>();
                }
                Preview.IsFilterSearch = Report.IsFilterSearch;
                if (Report.IsFilterSearch)
                {
                    Preview.SearchType = "FilterSearch";
                }
                Preview.IsNaturalSearch = Report.IsNaturalSearch;
                if (Report.IsNaturalSearch)
                {
                    Preview.SearchType = "NaturalSearch";
                }
                Preview.MouseOverColumns = MouseOverColumns;
                int SFCount = 0;
                if (Report.SearchFields != null && Report.SearchFields.Length > 0)
                {
                    SFCount = Report.SearchFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).Count();
                }
                Preview.SrchFCount = SFCount;
                Preview.Rows = new List<string[]>();
                List<SingleBOField> bofields = new List<SingleBOField>();
                if (Report.IsFilterSearch)
                {
                    if (Report.SearchFields != null && Report.SearchFields.Length > 0)
                    {
                        List<string> SearchFields = Report.SearchFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        if (SearchFields != null)
                        {
                            foreach (var items in SearchFields)
                            {
                                BOFields Field = BoFields.Where(m => m.Name.ToLower() == items.ToLower()).FirstOrDefault();
                                int TypeID = Field.TypeID;
                                string type = ((BODatatypes)TypeID).ToString();
                                bofields.Add(new SingleBOField
                                {
                                    ID = Field.ID,
                                    BOID = Field.BOID,
                                    Name = Field.Name,
                                    AliasName = Field.LabelName,
                                    DataType = type,
                                    IsRunTime = Field.IsRunTime,
                                    IsDBValue = Field.IsDBValue,
                                    DBQuery = Field.DBQuery,
                                    IsExpression = Field.IsExpression,
                                    //Expression = Field.Expression,
                                    //ExpreValue = Field.ExpreValue,
                                    IsDate = Field.IsDate,
                                    DateValue = Field.DateValue,
                                    DateExpression = Field.DateExpression
                                });
                            }
                        }
                    }
                    Preview.SingleBOField = bofields;
                }
                else
                {
                    Preview.SingleBOField = bofields;
                }
                Preview.iLayoutID = Report.iLayoutID;
                Preview.IsQueryExists = true;
                //var AllLeads = Spdb.Database.SqlQuery<VMLeads>("Select * from Leads where FKiOrgID=" + OrgID).ToList();
                //Preview.AllLeads = AllLeads;
                Preview.AllLeads = new List<VMLeads>();
                return Preview;
            }
            else
            {
                VMResultList QPreview = new VMResultList();
                Preview.IsQueryExists = false;
                return QPreview;
            }
        }
        public VMResultList RunUserQuery(VMRunUserQuery model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList Result = new VMResultList();
            List<string> AllHeadings = new List<string>();
            Reports query = dbContext.Reports.Find(model.ReportID);
            if (model.ResultListDisplayType >= 0)
            {
                query.ResultListDisplayType = model.ResultListDisplayType;
            }
            try
            {
                int PageSize = 30;
                int skip = 0;
                if (model.PageIndex == 2)
                {
                    skip = 30;
                    PageSize = 10;
                }
                else if (model.PageIndex >= 2)
                {
                    skip = PageSize + 10 * (model.PageIndex - 2);
                    PageSize = 10;
                }
                if (skip == 0)
                {
                    PageSize = 30;
                }
                //DataContext Spdb = new DataContext(database);
                var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                var sOrgDB = UserDetails.sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                int BOID = query.BOID;
                var oBO = dbContext.BOs.Find(BOID);
                Common Com = new Common();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> Headings = query.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                List<string> Formatting = new List<string>();
                List<int> Targets = new List<int>();
                List<string> MouseOverColumns = new List<string>();
                var Heads = GetHeadings(model.ReportID, model.SearchType, model.database, model.OrgID, model.UserID, sOrgName);
                Formatting = Heads.Formats;
                Targets = Heads.Targets;
                MouseOverColumns = Heads.MouseOverColumns;
                string NewQuery = Heads.Query;
                AllHeadings = Heads.Headings;
                List<VMDropDown> KeyPositions = Heads.FKPositions;
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == model.UserID).Select(m => m.RoleID).FirstOrDefault();
                string UserIDs = Com.GetSubUsers(model.UserID, model.OrgID, sDatabase, sOrgName);
                string Query = ServiceUtil.ReplaceQueryContent(NewQuery, UserIDs, model.UserID, model.OrgID, 0, 0);
                if (model.ClassFilter != 0 || model.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, model.OrgID, UserIDs, model.ClassFilter, model.DateFilter);
                }
                if (model.SearchText != null && model.SearchText.Length > 0)
                {
                    string NewSearchText = GetSearchString(model.SearchText, model.ReportID, model.OrgID, iUserID, model.SearchType, model.database, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (model.SearchType == "FilterSearch")
                {
                    if (model.Fields != null && model.Fields.Length > 0)
                    {
                        var Condition = ServiceUtil.GetDynamicSearchStrings(model.Fields, model.Optrs, model.Values);
                        if (Condition.Length > 0)
                        {
                            Query = ServiceUtil.AddSearchParameters(Query, Condition);
                        }
                    }
                }
                if (model.SearchType == "NaturalSearch")
                {
                    string NewSearchText = GetSearchString(model.SearchText, model.ReportID, model.OrgID, iUserID, model.SearchType, model.database, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                List<string[]> results = new List<string[]>();
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    List<object[]> Res = new List<object[]>();
                    if (query.ResultListDisplayType == 0)
                    {
                        Res = TotalResult.Skip(skip).Take(PageSize).ToList();
                    }
                    var Codings = Spdb.HTMLColorCodings.Where(m => m.OrganizationID == model.OrgID).ToList();
                    for (int i = 0; i < Res.Count(); i++)
                    {
                        List<string> NewRes = new List<string>();
                        for (int j = 0; j < Res[i].Count(); j++)
                        {
                            var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                            if (j == 1)
                            {
                                NewRes.Add("");
                            }
                            else if (pos != null)
                            {
                                var DbValue = Res[i][j];
                                if (DbValue != null)
                                {
                                    var Value = ServiceUtil.ReplaceForeginKeyValues(pos, Res[i][j].ToString(), model.database);
                                    NewRes.Add(Value);
                                }
                                else
                                {
                                    NewRes.Add(null);
                                }
                            }
                            else
                            {
                                if (Formatting[j] != null)
                                {
                                    if (Formatting[j] == "%")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(string.Format("{0}%", Res[i][j]));
                                        }

                                    }
                                    else if (Formatting[j] == "en-GB")
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            if (Convert.ToUInt32(Res[i][j]) > Targets[j])
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetgreencolor'></span>");
                                            }
                                            else
                                            {
                                                NewRes.Add(totalValueCurrency + "<span class='targetredcolor'></span>");
                                            }
                                        }
                                        else
                                        {
                                            CultureInfo rgi = new CultureInfo(Formatting[j]);
                                            string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                            NewRes.Add(totalValueCurrency);
                                        }
                                    }
                                    else
                                    {
                                        if (Targets[j] != 0)
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]) + "<span class='targetcolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]));
                                        }
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                        }
                        results.Add(NewRes.ToArray());
                    }
                    Con.Close();
                }
                if (model.SearchText != null && model.SearchType == "Quick")
                {
                    foreach (var item in results)//Highlight the SearchedText 
                    {
                        for (int i = 0; i <= item.Length - 1; i++)
                        {
                            if (item[i].IndexOf(model.SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                            {
                                string input = item[i];
                                string pattern = model.SearchText;
                                string replacement = string.Format("<strong class='themecolor'>{0}</strong>", "$0");
                                var result = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
                                item[i] = result;
                            }
                        }
                    }
                }
                Result.IsPopup = query.IsRowClick;
                Result.IsExport = query.IsExport;
                Result.ActionType = query.OnRowClickType;
                Result.ActionReportID = query.OnRowClickValue;
                Result.Headings = AllHeadings;
                Result.Rows = results;
                Result.QueryName = query.Name;
                Result.QueryID = model.ReportID;
                Result.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == model.ReportID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
                //vmquery.ClassID = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
                Result.QueryName = query.Name;
                Result.ResultListDisplayType = query.ResultListDisplayType;
                var FilterGroup = dbContext.BOGroupFields.Where(m => m.BOID == BOID && m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault();
                var FilterFields = new List<string>();
                if (FilterGroup != null)
                {
                    FilterFields = FilterGroup.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
                List<string> FilterFileds = new List<string>();
                Dictionary<string, string> myDict = new Dictionary<string, string>();
                foreach (var items in FilterFields)
                {
                    myDict[BOFields.Where(m => m.Name == items).Select(m => m.LabelName).FirstOrDefault()] = items;
                }
                Result.FilterGroup = myDict;
                Result.IsFilterSearch = query.IsFilterSearch;
                Result.IsNaturalSearch = query.IsNaturalSearch;
                Result.HeadingReports = new List<int>();
                Result.MouseOverColumns = MouseOverColumns;
                Result.SingleBOField = Heads.SingleBOField;
                Result.SrchFCount = Heads.SrchFCount;
                return Result;
            }
            catch (Exception ex)
            {
                Result.Headings = AllHeadings;
                Result.Rows = new List<string[]>();
                Result.QueryName = query.Name;
                Result.QueryID = model.ReportID;
                //vmquery.QueryIcon = dbContext.UserReports.Where(m => m.ReportID == QueryID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).SingleOrDefault();
                //vmquery.ClassID = dbContext.Reports.Where(m => m.ID == QueryID).Select(m => m.Class).FirstOrDefault();
                Result.QueryName = query.Name;
                Result.ResultListDisplayType = query.ResultListDisplayType;
                return Result;
            }

        }
        public string GetSearchString(string SearchText, int ReportID, int OrgID, int iUserID, string SearchType, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string NewSearchText = "";
            if (SearchText != null && SearchText.Length > 0)
            {
                var BOID = dbContext.Reports.Where(m => m.ID == ReportID).Select(m => m.BOID).FirstOrDefault();
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                string AndOR = "";
                List<string> AndOr = new List<string>();
                var regex = new Regex(@"[<>]=?|[!=]?=");
                Dictionary<string, bool> myDict = new Dictionary<string, bool>();
                //var FilterGroup = dbContext.BOGroupFields.Where(m => m.BOID == BOID && m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (SearchText.IndexOf(" and ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AndOr.Add("and");
                    var AndSpliting = Regex.Split(SearchText, " and ", RegexOptions.IgnoreCase).ToList();
                    foreach (var items in AndSpliting)
                    {
                        if (items.IndexOf(" or ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            AndOr.Add("or");
                            var OrSpliting = Regex.Split(items, " or ", RegexOptions.IgnoreCase).ToList();
                            foreach (var item in OrSpliting)
                            {
                                AndOR = "or";
                                NewSearchText = NewSearchText + GetModifiedString(item, BOID, iUserID, sDatabase, AndOR, SearchType, sOrgName);
                            }

                        }
                        else
                        {
                            AndOR = "And";
                            NewSearchText = NewSearchText + GetModifiedString(items, BOID, iUserID, sDatabase, AndOR, SearchType, sOrgName);
                        }
                    }
                }
                else if (SearchText.IndexOf(" or ", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    AndOr.Add("or");
                    var AndSpliting = Regex.Split(SearchText, " or ", RegexOptions.IgnoreCase).ToList();
                    foreach (var items in AndSpliting)
                    {

                        if (items.IndexOf(" and ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            AndOr.Add("and");
                            var OrSpliting = Regex.Split(items, " and ", RegexOptions.IgnoreCase).ToList();
                            foreach (var item in OrSpliting)
                            {
                                AndOR = "and";
                                NewSearchText = NewSearchText + GetModifiedString(item, BOID, iUserID, sDatabase, AndOR, SearchType, sOrgName);
                            }
                        }
                        else
                        {
                            AndOR = "or";
                            NewSearchText = NewSearchText + GetModifiedString(items, BOID, iUserID, sDatabase, AndOR, SearchType, sOrgName);

                        }
                    }
                }
                else
                {
                    NewSearchText = GetModifiedString(SearchText, BOID, iUserID, sDatabase, null, SearchType, sOrgName);
                }
                if (AndOr.Count() > 0)
                {
                    if (AndOr.Last() == "and")
                    {
                        NewSearchText = NewSearchText.Substring(0, NewSearchText.Length - 5);
                    }
                    else
                    {
                        NewSearchText = NewSearchText.Substring(0, NewSearchText.Length - 4);
                    }
                }
                return NewSearchText;
            }
            else
            {
                return NewSearchText;
            }
        }
        private string GetModifiedString(string SearchText, int BOID, int iUserID, string sDatabase, string AndOr, string SearchType, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var GroupFields = dbContext.BOGroupFields.Where(m => m.BOID == BOID).ToList();
            if (SearchType == "Quick")
            {
                var SearchString = "";
                var SearchWords = SearchText.Split(' ').ToList();
                //var QuickGroup = GroupFields.Where(m => m.GroupName == "Quick Search").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var QuickGroup = GroupFields.Where(m => m.GroupName == "Label").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (SearchText.Length > 0)
                {
                    foreach (var item in QuickGroup)
                    {
                        SearchString = SearchString + item + " like '%" + SearchText + "%' or ";
                    }
                }
                SearchString = SearchString.Substring(0, SearchString.Length - 4);
                return SearchString;
            }
            else
            {
                var QuickGroup = new List<string>();
                if (SearchType == "Quick")
                {
                    QuickGroup = GroupFields.Where(m => m.GroupName == "Quick Search").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    QuickGroup = GroupFields.Where(m => m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault().Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                string NewSearchText = "";
                var regex = new Regex(@"[<>]=?|[!=]?=");
                var Operators = new List<string>();
                foreach (Match match in regex.Matches(SearchText))
                {
                    Operators.Add(match.Value);
                }
                foreach (var items in Operators)
                {
                    string SValue = "";
                    string ID = "";
                    var value = Regex.Split(SearchText, items, RegexOptions.IgnoreCase).ToList();
                    var TextedName = value[0].TrimStart().TrimEnd();
                    if (TextedName.IndexOf("class", StringComparison.OrdinalIgnoreCase) >= 0 || TextedName.IndexOf("fkileadclassid", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SValue = value[1].TrimStart().TrimEnd();
                        int ClassID = Spdb.OrganizationClasses.Where(m => m.Class == SValue).Select(m => m.ClassID).FirstOrDefault();
                        if (ClassID != 0)
                        {
                            ID = ClassID.ToString();
                        }
                        else
                        {
                            ID = SValue;
                        }
                    }
                    else if (TextedName.IndexOf("FKiSourceID", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        SValue = value[1].TrimStart().TrimEnd();
                        int SourceID = Spdb.OrganizationSources.Where(m => m.Name == SValue).Select(m => m.ID).FirstOrDefault();
                        if (SourceID != 0)
                        {
                            ID = SourceID.ToString();
                        }
                        else
                        {
                            ID = SValue;
                        }
                    }

                    else
                    {
                        ID = value[1].TrimStart().TrimEnd();
                    }
                    var AliasName = value[0].TrimStart().TrimEnd();
                    var OriginalName = dbContext.BOFields.Where(m => m.LabelName == AliasName && m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                    if (OriginalName == null)
                    {
                        OriginalName = value[0].TrimStart().TrimEnd();
                    }
                    if (QuickGroup.Contains(OriginalName))
                    {
                        if (AndOr != null)
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "' " + AndOr + " ";
                        }
                        else
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                        }
                    }
                    else if (SearchType == "Structured")
                    {
                        NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                    }
                    else
                    {
                        if (AndOr != null)
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "' " + AndOr + " ";
                        }
                        else
                        {
                            NewSearchText = NewSearchText + OriginalName + items + "'" + ID.TrimStart().TrimEnd() + "'";
                        }
                    }
                }
                return NewSearchText;
            }

        }
        #endregion OneClickExecution

        #region Popup

        public Popup GetPopupDetails(int PopupID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var Popup = dbContext.Popups.Find(PopupID);
            return Popup;
        }

        public VMLeadPopupLeft GetLeadPopupLeftContent(VMViewPopup popup, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var tablename = EnumLeadTables.Leads.ToString();
            //var Leads = Spdb.Database.SqlQuery<VMLeads>("SELECT * FROM " + tablename + " Where ID = " + popup.LeadID).FirstOrDefault();
            var Leads = Spdb.Database.SqlQuery<VMLeads>("SELECT * FROM " + tablename).ToList();
            var BOFields = dbContext.BOFields.Where(m => m.BOID == 1).ToList();
            //for (int j = 0; j < BOFields.OrderBy(m=>m.Name).Count(); j++)
            //{

            //}
            //Dictionary<BOFields, Leads> d = new Dictionary<BOFields, Leads>();
            //List<Leads> AllLeads = new List<Leads>();
            //for (int i = 0; i < Leads.Count(); i++)
            //{
            //    Leads Lead = new Leads();
            //    Lead.LeadID = Leads[i].ID;
            //    Lead.ID = BOFields.Where(m => m.Name == "ID").FirstOrDefault();
            //    Lead.ID.Value = Leads[i].ID.ToString();
            //    Lead.sForeName = BOFields.Where(m => m.Name == "sForeName").FirstOrDefault();
            //    Lead.sForeName.Value = string.IsNullOrEmpty(Leads[i].sForeName) ? null : Leads[i].sForeName.ToString();
            //    AllLeads.Add(Lead);
            //}





            VMLeadPopupLeft LeftContent = new VMLeadPopupLeft();
            var orgname = dbContext.Organization.Find(OrgID);
            //VMLeads lead = new VMLeads();
            int? ClassID = 0;
            string MobileNo = "";

            var leadData = Spdb.Database.SqlQuery<VMLeads>("SELECT ID, sForeName, sLastName, FKiLeadClassID, sEmail, iCallCount, dtCallSchedule, iCallbackStatus, dImportedOn, sSystemAlert, sMob FROM " + tablename + " Where ID = " + popup.LeadID).FirstOrDefault();
            leadData.ConversionStatus = getconversionstatus(popup.LeadID, sDatabase, iUserID, sOrgName);
            leadData.OrganizationName = orgname.Name;
            leadData.OrganizationID = OrgID;
            leadData.CallBackStatus = (leadData.iCallbackStatus > 0 ? (leadData.iCallbackStatus == 10 ? "Call back pending" : "") : null);
            MobileNo = leadData.sMob;
            ClassID = leadData.FKiLeadClassID;
            var ClassName = dbContext.Types.Where(m => m.ID == ClassID).Select(m => m.Expression).FirstOrDefault();
            leadData.Class = ClassName;
            leadData.StatusTypeID = 10;
            var Request = dbContext.WalletRequests.Where(m => m.EmailID == leadData.sEmail).FirstOrDefault();
            if (Request != null)
            {
                leadData.IsReqSent = true;
            }
            else
            {
                leadData.IsReqSent = false;
            }
            LeftContent.LeadInfo = leadData;
            //Stages
            List<VMStages> AllStages = new List<VMStages>();
            var History = Spdb.LeadTransitions.Where(m => m.LeadID == popup.LeadID).ToList();
            LeadTransitions LeadHistory = new LeadTransitions();
            if (History.Count() > 0)
            {
                LeadHistory = History.Last();
            }
            else
            {
                LeadHistory = null;
            }
            if (LeadHistory != null)
            {
                var NextStages = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StageID == LeadHistory.StageID && m.StatusTypeID == 10).Select(m => m.SubStages).FirstOrDefault();
                if (NextStages != null)
                {
                    var NStages = NextStages.Split(',').ToList();
                    foreach (var items in NStages)
                    {
                        int ID = Convert.ToInt32(items);
                        var Stage = dbContext.Stages.Where(m => m.ID == ID).FirstOrDefault();
                        AllStages.Add(new VMStages
                        {
                            ID = Stage.ID,
                            Name = Stage.Name,
                            LeadID = popup.LeadID,
                            PopupID = Stage.PopupID
                        });
                    }
                }
            }
            else
            {
                StagesFlows SFlow = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).FirstOrDefault();
                if (SFlow != null)
                {
                    var Stage = dbContext.Stages.Where(m => m.ID == SFlow.StageID).FirstOrDefault();
                    AllStages.Add(new VMStages
                    {
                        ID = Stage.ID,
                        Name = Stage.Name,
                        LeadID = popup.LeadID,
                        PopupID = Stage.PopupID
                    });
                }
            }
            LeftContent.LeadStages = AllStages;
            //Lead Actions
            var res = Spdb.LeadActionMenus.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList();
            List<VMLeadActions> AllActions = new List<VMLeadActions>();
            foreach (var items in res)
            {
                int PopupID = 0;
                int StageID = 0;
                var ActionType = Spdb.LeadActions.Where(m => m.ID == items.ActionType).FirstOrDefault();
                if (ActionType.IsPopup)
                {
                    PopupID = ActionType.PopupID;
                }
                if (ActionType.IsStage)
                {
                    StageID = ActionType.StageID;
                }
                AllActions.Add(new VMLeadActions
                {
                    ID = items.ID,
                    Name = items.Name,
                    PopupID = PopupID,
                    LeadID = popup.LeadID,
                    StageID = StageID
                });
            }
            LeftContent.LeadActions = AllActions;
            //Lead Clients
            var ClientData = Spdb.Database.SqlQuery<VMLeads>("SELECT ID, Name, ClassID FROM LeadClients Where Name = '" + leadData.sForeName + " " + leadData.sLastName + "' AND Email='" + leadData.sEmail + "' AND Mobile='" + MobileNo + "'").ToList();
            foreach (var items in ClientData)
            {
                items.Class = getclass(items.ClassID, OrgID, iUserID, sDatabase, sOrgName);
            }
            LeftContent.LeadClients = ClientData;
            return LeftContent;
        }

        private string getconversionstatus(int? LeadID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int StageID = 0;
            string Status = "";
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            StageID = Spdb.Database.SqlQuery<int>("SELECT StageID FROM LeadTransitions Where LeadID = " + LeadID).FirstOrDefault();
            if (StageID > 0)
            {
                Status = getstatusname(StageID, sDatabase);
            }
            else
            {
                Status = "Inbound";
            }
            return Status;
        }

        private string getstatusname(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var status = dbContext.Stages.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return status;
        }

        private string getclass(int p, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var classname = Spdb.OrganizationClasses.Where(m => m.ClassID == p && m.OrganizationID == OrgID).Select(m => m.Class).FirstOrDefault();
            return classname;
        }

        public List<VMDropDown> GetAllTabs(int ReportID, int PopupID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int PopupType = 0;
            if (ReportID > 0 && PopupID == 0)
            {
                PopupType = dbContext.Reports.Where(m => m.ID == ReportID).Select(m => m.OnRowClickValue).FirstOrDefault();
            }
            else
            {
                PopupType = dbContext.Popups.Where(m => m.ID == PopupID).Select(m => m.ID).FirstOrDefault();
            }
            //PopupName = dbContext.Popups.Where(m => m.ID == PopupType).Select(m => m.Name).FirstOrDefault();
            List<VMDropDown> AllTabs = new List<VMDropDown>();
            if (PopupType > 0)
            {
                AllTabs = (from c in dbContext.Tabs.Where(m => m.PopupID == PopupType && m.StatusTypeID == 10).OrderBy(m => m.Rank).ToList()
                           select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
                return AllTabs;
            }
            else
            {
                return AllTabs;
            }
        }

        public int CallAction(VMLeadActions model, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Common Com = new Common();
            var ActionMenu = Spdb.LeadActionMenus.Find(model.ID);
            var ActionTypes = Spdb.LeadActions.Find(ActionMenu.ActionType);
            if (ActionTypes.IsSMS)
            {
                Com.SendSMS(iUserID, OrgID, ActionTypes.SMSTemplateID, sDatabase, sOrgName);
                int val = SaveToOutbounds(model, OrgID, iUserID, sDatabase, sOrgName);
            }
            if (ActionTypes.IsEmail)
            {
                Com.SendMail(iUserID, OrgID, "", ActionTypes.EmailTemplateID, "", sDatabase, sOrgName);
                int val = SaveToOutbounds(model, OrgID, iUserID, sDatabase, sOrgName);
            }
            if (ActionTypes.IsOneClcik)
            {
                var Query = ActionTypes.Query;
                if (Query.IndexOf("{ID}", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    Query = Regex.Replace(Query, "{ID}", model.LeadID.ToString(), RegexOptions.IgnoreCase);
                }
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    cmd.ExecuteNonQuery();
                    Con.Close();
                }
            }
            if (ActionTypes.IsStage)
            {

            }
            return 0;
        }

        private int SaveToOutbounds(VMLeadActions model, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var ActionMenu = Spdb.LeadActionMenus.Find(model.ID);
            var ActionTypes = Spdb.LeadActions.Find(ActionMenu.ActionType);
            Outbounds outbound = new Outbounds();
            VMLeads Lead = Spdb.Database.SqlQuery<VMLeads>("SELECT sMob, sEMail FROM " + EnumLeadTables.Leads.ToString() + " WHERE ID='" + model.LeadID + "'").FirstOrDefault();
            outbound.Email = Lead.sEmail;
            outbound.Mobile = Lead.sMob;
            outbound.OrganizationID = OrgID;
            outbound.LeadID = model.LeadID;
            if (ActionTypes.IsSMS)
            {
                outbound.Type = 2;
                outbound.TemplateID = ActionTypes.SMSTemplateID;
            }
            else if (ActionTypes.IsEmail)
            {
                outbound.Type = 1;
                outbound.TemplateID = ActionTypes.EmailTemplateID;
            }
            Spdb.Outbounds.Add(outbound);
            Spdb.SaveChanges();
            return 0;
        }

        public string SaveLeadTransaction(Stages model, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var lead = Spdb.LeadTransitions.Where(m => m.LeadID == model.LeadID).ToList();
            var AllStages = dbContext.Stages.Where(m => m.OrganizationID == OrgID || m.OrganizationID == 0).ToList();
            LeadTransitions Trans = new LeadTransitions();
            Trans.StageID = model.ID;
            Trans.LeadID = model.LeadID;
            Trans.OrganizationID = OrgID;
            if (lead.Count() == 0)
            {
                Trans.FromStatus = "Inbound";
            }
            else
            {
                Trans.FromStatus = lead.Last().ToStatus;
            }
            Trans.ToStatus = AllStages.Where(m => m.ID == model.ID).Select(m => m.Name).FirstOrDefault();
            Spdb.LeadTransitions.Add(Trans);
            Spdb.SaveChanges();
            Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set istatus='" + model.ID + "' where ID=" + model.LeadID);
            LeadStatus Sta = new LeadStatus();
            Sta.Status = AllStages.Where(m => m.ID == model.ID).Select(m => m.Name).FirstOrDefault();
            Sta.LeadID = model.LeadID;
            Sta.UserName = dbCore.XIAppUsers.Where(m => m.UserID == iUserID).Select(m => m.sFirstName).FirstOrDefault();
            Spdb.LeadStatus.Add(Sta);
            Spdb.SaveChanges();
            var LeadData = Spdb.Database.SqlQuery<VMLeads>("SELECT sMob, sEMail FROM " + EnumLeadTables.Leads.ToString() + " WHERE ID='" + model.LeadID + "'").FirstOrDefault();
            var Stage = dbContext.Stages.Find(model.ID);
            if (Stage.IsEmail == true)
            {
                Outbounds outbound = new Outbounds();
                outbound.OrganizationID = OrgID;
                outbound.LeadID = model.LeadID;
                outbound.Mobile = LeadData.sMob;
                outbound.Email = LeadData.sEmail;
                outbound.Type = 1;
                outbound.TemplateID = 2;
                Spdb.Outbounds.Add(outbound);
                Spdb.SaveChanges();
            }
            if (Stage.IsSMS == true)
            {
                Outbounds outbound = new Outbounds();
                outbound.OrganizationID = OrgID;
                outbound.LeadID = model.LeadID;
                outbound.Mobile = LeadData.sMob;
                outbound.Email = LeadData.sEmail;
                outbound.Type = 2;
                outbound.TemplateID = 2;
                Spdb.Outbounds.Add(outbound);
                Spdb.SaveChanges();
            }
            var stage = getstagename(model.ID, sDatabase);
            return stage;
        }

        private string getstagename(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var stage = dbContext.Stages.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return stage;
        }

        public List<Stages> GetNextStages(int LeadID, int StageID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<Stages> AllStages = new List<Stages>();
            var NextStages = Spdb.StagesFlows.Where(m => m.OrganizationID == OrgID && m.StageID == StageID).Select(m => m.SubStages).FirstOrDefault();
            var NStages = NextStages.Split(',').ToList();
            foreach (var items in NStages)
            {
                int ID = Convert.ToInt32(items);
                var Stage = dbContext.Stages.Where(m => m.ID == ID).FirstOrDefault();
                AllStages.Add(Stage);
            }
            return AllStages;
        }

        public int SaveWalletRequest(WalletRequests Request, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            WalletRequests Req = new WalletRequests();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "Select sEmail from " + EnumLeadTables.Leads.ToString() + " Where ID=" + Request.LeadID;
                Con.Open();
                Con.ChangeDatabase(sDatabase);
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                Req.EmailID = TotalResult[0][0].ToString();
                Con.Dispose();
            }
            //Req.EmailID = Request.EmailID;
            Req.FKiLeadClassID = Request.FKiLeadClassID;
            Req.OrganizationID = Request.OrganizationID;
            dbContext.WalletRequests.Add(Req);
            dbContext.SaveChanges();
            return Req.ID;
        }

        public List<VMQueryPreview> GetTabContent(VMViewPopup Popup, int iUserID, int OrgID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int? TabID = Popup.TabID;
            int? LeadID = Popup.LeadID;
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            int ClassID = Spdb.Database.SqlQuery<int>("Select FKiLeadClassID From " + EnumLeadTables.Leads.ToString() + " Where ID = " + LeadID).FirstOrDefault();
            var Tabs = dbContext.Tabs.ToList();
            var Tab1ClicksList = dbContext.Tab1Clicks.Where(m => m.TabID == TabID).ToList();
            var tab1click = Tab1ClicksList.Where(m => m.ClassID == ClassID && m.StatusTypeID == 10).ToList();
            int Dtype = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString());
            var tab1clickbespoke = Tab1ClicksList.Where(m => m.DisplayAs == Dtype).ToList();
            var AllTab1Clicks = tab1click.Union(tab1clickbespoke).ToList();
            var AllSections = dbContext.Sections.ToList();
            List<VMQueryPreview> TabsResult = new List<VMQueryPreview>();
            foreach (var items in AllTab1Clicks)
            {
                ViewRecord ViewRecord = new ViewRecord();
                ViewRecord.SectionIDs = items.SectionID;
                ViewRecord.ViewFields = items.ViewFields;
                ViewRecord.EditFields = items.EditFields;
                if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ViewRecord.ToString()))
                {
                    if (ViewRecord.ViewFields != null || ViewRecord.EditFields != null)
                    {
                        VMQueryPreview result = new VMQueryPreview();
                        if (Popup.PopType == "Inner")
                        {
                            result = QueryDynamicForm(items.ID, Popup.RowID, sDatabase, OrgID, iUserID, sOrgName);
                        }
                        else
                        {
                            var reportid = Tab1ClicksList.Where(m => m.ID == items.ID).Select(m => m.ReportID).FirstOrDefault();
                            var boid = dbContext.Reports.Where(m => m.ID == reportid).Select(m => m.BOID).FirstOrDefault();
                            var BOName = dbContext.BOs.Where(m => m.BOID == boid).Select(m => m.Name).FirstOrDefault();
                            if (BOName == EnumLeadTables.LeadClients.ToString())
                            {
                                result = QueryDynamicForm(items.ID, Popup.ClientID, sDatabase, OrgID, iUserID, sOrgName);
                            }
                            else
                            {
                                result = QueryDynamicForm(items.ID, LeadID, sDatabase, OrgID, iUserID, sOrgName);
                            }

                        }
                        result.SectionsData.FirstOrDefault().TabID = TabID;
                        result.SectionsData.FirstOrDefault().ClassID = ClassID;
                        result.SectionsData.FirstOrDefault().TabName = Tabs.Where(m => m.ID == TabID).Select(m => m.Name).FirstOrDefault();
                        result.PreviewType = EnumDisplayTypes.ViewRecord.ToString();
                        if (!(items.SectionID.Contains(',')))
                        {
                            var secid = Convert.ToInt32(items.SectionID);
                            int rank = 0;
                            if (secid > 0)
                            {
                                rank = AllSections.Where(m => m.ID == secid).Select(m => m.Rank).FirstOrDefault();
                            }
                            result.Rank = rank;
                        }
                        result.ReportID = items.ReportID;
                        result.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                        result.TabID = items.TabID;
                        TabsResult.Add(result);
                    }
                    else
                    {
                        VMQueryPreview preview = new VMQueryPreview();
                        TabsResult.Add(preview);
                    }
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.Bespoke.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.IsBespoke = items.IsBespoke;
                    data.URL = items.URL;
                    data.RefreshType = items.RefreshType;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    data.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    data.LeadID = Popup.LeadID;
                    data.TabID = Popup.TabID;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    preview.Rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    preview.PreviewType = EnumDisplayTypes.Bespoke.ToString();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ResultList.ToString()))
                {
                    Common Com = new Common();
                    var result = Com.GetHeadings(items.FKi1ClickIDXIGUID, sDatabase, OrgID, iUserID, sOrgName);
                    result.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        result.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                        result.TabID = Popup.TabID;
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                        result.Rank = rank;
                        result.SectionID = sectionid;
                    }
                    result.TabID = TabID;
                    result.ReportID = items.ReportID;
                    result.PreviewType = EnumDisplayTypes.ResultList.ToString();
                    result.Tab1ClickID = items.ID;
                    result.BOID = dbContext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.BOID).FirstOrDefault();
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    result.SectionsData = Data;
                    result.IsView = items.IsView;
                    result.IsEdit = items.IsEdit;
                    result.IsCreate = items.IsCreate;
                    result.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(result);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    var result = KPICirlceForTab(items.ReportID, iUserID, OrgID, sDatabase, sOrgName);
                    preview.KpiCircle = result;
                    preview.PreviewType = EnumDisplayTypes.KPICircle.ToString();
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        result.FirstOrDefault().SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                        result.FirstOrDefault().TabID = Popup.TabID;
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.PieChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.PieChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetPieChartForTab(iUserID, sDatabase, OrgID, items.ReportID, sOrgName);
                    preview.PieData = result;
                    preview.QueryName = dbContext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.BarChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.BarChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetBarChartForTab(iUserID, sDatabase, OrgID, items.ReportID, sOrgName);
                    preview.BarData = result;
                    preview.QueryName = dbContext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else if (items.DisplayAs == (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.LineChart.ToString()))
                {
                    VMQueryPreview preview = new VMQueryPreview();
                    VMViewPopup popup = new VMViewPopup();
                    popup.TabID = TabID;
                    popup.ClassID = ClassID;
                    preview.PreviewType = EnumDisplayTypes.LineChart.ToString();
                    preview.popup = popup;
                    preview.ViewRecord = ViewRecord;
                    var sectionid = Convert.ToInt32(items.SectionID);
                    if (sectionid > 0)
                    {
                        preview.SectionName = AllSections.Where(m => m.ID == sectionid).Select(m => m.Name).FirstOrDefault();
                    }
                    int rank = 0;
                    if (sectionid > 0)
                    {
                        rank = AllSections.Where(m => m.ID == sectionid).Select(m => m.Rank).FirstOrDefault();
                    }
                    preview.Rank = rank;
                    preview.ReportID = items.ReportID;
                    SectionsData data = new SectionsData();
                    List<SectionsData> Data = new List<SectionsData>();
                    data.TabID = TabID;
                    data.SectionID = sectionid;
                    Data.Add(data);
                    preview.SectionsData = Data;
                    var result = GetBarChartForTab(iUserID, sDatabase, OrgID, items.ReportID, sOrgName);
                    preview.LineGraph = result;
                    preview.QueryName = dbContext.Reports.Where(m => m.ID == items.ReportID).Select(m => m.Name).FirstOrDefault();
                    preview.TabName = Tabs.Where(m => m.ID == items.TabID).Select(m => m.Name).FirstOrDefault();
                    TabsResult.Add(preview);
                }
                else
                {
                    return null;
                }
            }
            return TabsResult;
        }

        public VMQueryPreview QueryDynamicForm(int Tab1clickID, int? leadid, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            //string database = dbContext.AspNetUsers.Where(m=>m.OrganizationID==orgid).Select(m=>m.DatabaseName).FirstOrDefault();
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            VMQueryPreview vmquery = new VMQueryPreview();
            Tab1Clicks tab1click = dbContext.Tab1Clicks.Find(Tab1clickID);
            Reports query = dbContext.Reports.Find(tab1click.ReportID);
            List<string> viewfields = new List<string>();
            List<string> editfields = new List<string>();
            List<string> IsDropDown = new List<string>();
            List<List<VMDropDown>> DropDownValues = new List<List<VMDropDown>>();
            BOs BoDetails = dbContext.BOs.Find(query.BOID);
            string tablename = BoDetails.Name;
            var MappedFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == tab1click.ClassID).ToList();
            var BFields = dbContext.BOFields.Where(m => m.BOID == query.BOID).ToList();
            var Popups = dbContext.Popups.Where(m => m.StatusTypeID == 10).ToList();
            vmquery.ActionPopUp = query.ActionFieldValue;
            if (leadid != null)
            {
                vmquery.LeadID = Convert.ToInt32(leadid);
                var sectionids = tab1click.SectionID;
                var vfields = tab1click.ViewFields;
                var efields = tab1click.EditFields;
                List<string> VFields = new List<string>();
                List<string> EFields = new List<string>();
                List<SectionsData> Data = new List<SectionsData>();
                List<string> EditFields = new List<string>();
                List<string> secids = new List<string>();
                List<string> Editdatatypes = new List<string>();
                List<string> EditLengths = new List<string>();
                List<string> EditDescs = new List<string>();
                List<string> ViewDropDowns = new List<string>();
                List<string> ViewDDLTable = new List<string>();
                List<string> IsViewFK = new List<string>();
                List<int> ViewFKPopupID = new List<int>();
                string viewfieldforquery = "";
                if (sectionids != "0")
                {
                    if (sectionids.Contains(','))
                    {
                        secids = sectionids.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    else
                    {
                        secids = sectionids.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                }
                else if (sectionids == "0")
                {
                    secids.Add("0");
                }
                for (int i = 0; i < secids.Count(); i++)
                {
                    SectionsData data = new SectionsData();
                    if (vfields != null)
                    {
                        string aliasname = "";
                        List<string> AliasViewFiels = new List<string>();
                        VFields = vfields.Split('/').ToList();
                        data.ViewFields = VFields[i].Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in data.ViewFields)
                        {
                            if (items.Contains("NE-"))
                            {
                                var editname = items.Split('-')[1];
                                aliasname = MappedFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var BFld = BFields.Where(m => m.Name == editname && m.BOID == query.BOID).FirstOrDefault();
                                    aliasname = BFld.LabelName;
                                    var PopID = dbContext.Popups.Where(m => m.FKColumnID == BFld.ID).FirstOrDefault();
                                    if (PopID != null)
                                    {
                                        IsViewFK.Add("True");
                                        ViewFKPopupID.Add(PopID.ID);
                                    }
                                    else
                                    {
                                        IsViewFK.Add("False");
                                        ViewFKPopupID.Add(0);
                                    }
                                }
                                else
                                {
                                    IsViewFK.Add("False");
                                    ViewFKPopupID.Add(0);
                                }
                                AliasViewFiels.Add("NE-" + aliasname);
                                viewfieldforquery = viewfieldforquery + editname + ", ";
                                ViewDropDowns.Add("False");
                                ViewDDLTable.Add(null);
                            }
                            else
                            {
                                var MapFld = MappedFields.Where(m => m.AddField == items).FirstOrDefault();
                                if (MapFld == null)
                                {
                                    var BFld = BFields.Where(m => m.Name == items && m.BOID == query.BOID).FirstOrDefault();
                                    aliasname = BFld.LabelName;
                                    if (BFld.FKTableName != null && BFld.FKTableName.Length > 0)
                                    {
                                        ViewDropDowns.Add("True");
                                        ViewDDLTable.Add(BFld.FKTableName);
                                    }
                                    else
                                    {
                                        ViewDropDowns.Add("False");
                                        ViewDDLTable.Add(null);
                                    }
                                    var PopID = dbContext.Popups.Where(m => m.FKColumnID == BFld.ID).FirstOrDefault();
                                    if (PopID != null)
                                    {
                                        IsViewFK.Add("True");
                                        ViewFKPopupID.Add(PopID.ID);
                                    }
                                    else
                                    {
                                        IsViewFK.Add("False");
                                        ViewFKPopupID.Add(0);
                                    }
                                }
                                else
                                {
                                    aliasname = MapFld.FieldName;
                                    if (MapFld.IsDropDown)
                                    {
                                        ViewDropDowns.Add("True");
                                        ViewDDLTable.Add("Types");
                                    }
                                    else
                                    {
                                        ViewDropDowns.Add("False");
                                        ViewDDLTable.Add(null);
                                    }
                                    IsViewFK.Add("False");
                                    ViewFKPopupID.Add(0);
                                }
                                AliasViewFiels.Add(aliasname);
                                viewfieldforquery = viewfieldforquery + items + ", ";
                            }
                        }
                        data.ViewFields = AliasViewFiels;
                        data.IsViewFK = IsViewFK;
                        data.ViewFKPopuID = ViewFKPopupID;
                    }
                    int SectionID = Convert.ToInt32(secids[i]);
                    data.SectionID = Convert.ToInt32(secids[i]);
                    data.Tab1ClickID = Tab1clickID;
                    data.LeadID = leadid;
                    data.TabID = tab1click.TabID;
                    data.IsView = tab1click.IsView;
                    data.IsEdit = tab1click.IsEdit;
                    data.IsCreate = tab1click.IsCreate;
                    data.Rank = dbContext.Sections.Where(m => m.ID == SectionID).Select(m => m.Rank).FirstOrDefault();
                    if (SectionID != 0)
                    {
                        var sections = dbContext.Sections.ToList();
                        data.SectionName = sections.Where(m => m.ID == SectionID).Select(m => m.Name).FirstOrDefault();
                        data.IsBespoke = sections.Where(m => m.ID == SectionID).Select(m => m.IsBespoke).FirstOrDefault();
                        if (data.IsBespoke == true)
                        {
                            data.URL = sections.Where(m => m.ID == SectionID).Select(m => m.URL).FirstOrDefault();
                            data.RefreshType = sections.Where(m => m.ID == SectionID).Select(m => m.RefreshType).FirstOrDefault();
                        }
                    }
                    string editfieldforquery = "";
                    if (efields != null && efields.Length > 0)
                    {
                        var EDFields = efields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var field in EDFields)
                        {
                            if (field != null)
                            {
                                if (field.Contains("NE-"))
                                {
                                    string FieldName = "";
                                    var NonEdit = field.Replace("NE-", "");
                                    var fields = MappedFields.Where(m => m.AddField == field).FirstOrDefault();
                                    if (fields == null)
                                    {
                                        var AName = BFields.Where(m => m.Name == NonEdit && m.BOID == query.BOID).Select(m => m.LabelName).FirstOrDefault();
                                        FieldName = NonEdit;
                                        EditFields.Add("NE-" + AName);
                                        EditDescs.Add(null);
                                    }
                                    else
                                    {
                                        FieldName = fields.FieldName;
                                        EditDescs.Add(fields.Description);
                                        EditFields.Add(FieldName);
                                    }
                                    var bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == FieldName).FirstOrDefault();
                                    string type = ((BODatatypes)bofield.TypeID).ToString();
                                    Editdatatypes.Add(type);
                                    EditLengths.Add(bofield.MaxLength);
                                    DropDownValues.Add(new List<VMDropDown>());
                                    editfieldforquery = editfieldforquery + NonEdit + ", ";
                                    IsDropDown.Add("False");
                                }
                                else
                                {
                                    BOFields bofield = new BOFields();
                                    string FieldName = "";
                                    var fields = MappedFields.Where(m => m.AddField == field).FirstOrDefault();
                                    if (fields == null)
                                    {
                                        var AName = BFields.Where(m => m.Name == field && m.BOID == query.BOID).Select(m => m.LabelName).FirstOrDefault();
                                        FieldName = field;
                                        bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == FieldName).FirstOrDefault();
                                        EditFields.Add(AName);
                                        EditDescs.Add(bofield.Description);
                                    }
                                    else
                                    {
                                        FieldName = fields.FieldName;
                                        EditFields.Add(FieldName);
                                        EditDescs.Add(fields.Description);
                                        bofield = BFields.Where(m => m.BOID == query.BOID).Where(m => m.Name == fields.AddField).FirstOrDefault();
                                    }
                                    editfieldforquery = editfieldforquery + field + ", ";
                                    string type = ((BODatatypes)bofield.TypeID).ToString();
                                    Editdatatypes.Add(type);
                                    EditLengths.Add(bofield.MaxLength);
                                    if (bofield.FKTableName != null && bofield.FKTableName.Length > 0)
                                    {
                                        IsDropDown.Add("True");
                                        using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                                        {
                                            Con.Open();
                                            SqlCommand cmd = new SqlCommand("", Con);
                                            if (bofield.FKTableName == "AspNetUsers")
                                            {
                                                int RoleID = 0;
                                                if (bofield.Name == "iSalesUserID")
                                                {
                                                    RoleID = dbCore.XIAppRoles.Where(m => m.sRoleName == "Sales").Select(m => m.RoleID).FirstOrDefault();
                                                }
                                                else
                                                {
                                                    RoleID = dbCore.XIAppRoles.Where(m => m.sRoleName == "Outbound").Select(m => m.RoleID).FirstOrDefault();
                                                }
                                                var Users = dbCore.XIAppUserRoles.Where(m => m.RoleID == RoleID).Select(m => m.UserID).ToList();
                                                var AllUsers = dbCore.XIAppUsers.ToList();
                                                if (Users.Count() > 0)
                                                {
                                                    var DDLVlaues = new List<VMDropDown>();
                                                    foreach (var user in Users)
                                                    {
                                                        DDLVlaues.Add((from c in AllUsers.Where(m => m.UserID == user).ToList()
                                                                       select new VMDropDown { text = c.sFirstName, Value = c.UserID }).FirstOrDefault());
                                                    }
                                                    DropDownValues.Add(DDLVlaues);
                                                }
                                                else
                                                {
                                                    DropDownValues.Add(new List<VMDropDown>());
                                                }
                                            }
                                            else
                                            {
                                                if (bofield.FKTableName == EnumLeadTables.OrganizationClasses.ToString())
                                                {
                                                    cmd.CommandText = "Select ClassID, Class FROM " + bofield.FKTableName + " WHERE OrganizationID=" + OrgID;
                                                }
                                                else
                                                {
                                                    cmd.CommandText = "Select ID, Name FROM " + bofield.FKTableName + " WHERE OrganizationID=" + OrgID;
                                                }
                                                Con.ChangeDatabase(sOrgDB);
                                                SqlDataReader reader = cmd.ExecuteReader();
                                                DataTable Ddata = new DataTable();
                                                Ddata.Load(reader);
                                                List<object[]> TotalResult = Ddata.AsEnumerable().Select(m => m.ItemArray).ToList();
                                                var DDLVlaues = (from c in TotalResult
                                                                 select new VMDropDown { Value = Convert.ToInt32(c[0]), text = c[1].ToString() }).ToList();
                                                DropDownValues.Add(DDLVlaues);
                                            }
                                        }
                                    }
                                    else if (fields != null)
                                    {
                                        if (fields.IsDropDown)
                                        {
                                            IsDropDown.Add("True");
                                            var DDLVlaues = new List<VMDropDown>();
                                            var Type = dbContext.Types.Where(m => m.ID == fields.MasterID).Select(m => m.Expression).FirstOrDefault();
                                            DDLVlaues = (from c in dbContext.Types.Where(m => m.Name == Type) select new VMDropDown { Value = c.ID, text = c.Expression }).ToList();
                                            DropDownValues.Add(DDLVlaues);
                                        }
                                        else
                                        {
                                            IsDropDown.Add("False");
                                            DropDownValues.Add(new List<VMDropDown>());
                                        }
                                    }
                                    else
                                    {
                                        IsDropDown.Add("False");
                                        DropDownValues.Add(new List<VMDropDown>());
                                    }
                                }
                            }
                        }
                    }
                    if (vfields != null && vfields.Length > 0)
                    {
                        viewfieldforquery = viewfieldforquery.Substring(0, viewfieldforquery.Length - 2);
                        var ViewQuery = "Select " + viewfieldforquery + " FROM " + tablename + " WHERE " + " ID= " + leadid;
                        var valueoflead = new List<string>();
                        using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                        {
                            Con.Open();
                            Con.ChangeDatabase(sOrgDB);
                            SqlCommand cmd = new SqlCommand();
                            cmd.Connection = Con;
                            cmd.CommandText = ViewQuery;
                            SqlDataReader reader = cmd.ExecuteReader();
                            DataTable Viewdata = new DataTable();
                            Viewdata.Load(reader);
                            List<object[]> TotalResult = Viewdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                            valueoflead = (from c in TotalResult[0]
                                           select c.ToString()).ToList();
                            data.ViewFieldsData = valueoflead;
                            Con.Close();
                        }
                        for (int h = 0; h < ViewDropDowns.Count(); h++)
                        {
                            if (ViewDropDowns[h] == "True")
                            {
                                if (valueoflead[h] != null && valueoflead[h].Length > 0)
                                {
                                    string Masters = dbContext.Database.SqlQuery<string>("Select Expression from " + ViewDDLTable[h] + " where id=" + valueoflead[h]).FirstOrDefault();
                                    valueoflead[h] = Masters;
                                }
                            }
                        }
                    }
                    else
                    {
                        data.ViewFieldsData = new List<string>();
                    }
                    if (efields != null)
                    {
                        editfieldforquery = editfieldforquery.Substring(0, editfieldforquery.Length - 2);
                        var EditQuery = "Select " + editfieldforquery + " FROM " + tablename + " WHERE " + " ID= " + leadid;
                        using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                        {
                            Conn.Open();
                            Conn.ChangeDatabase(sDatabase);
                            SqlCommand cmmd = new SqlCommand();
                            cmmd.Connection = Conn;
                            cmmd.CommandText = EditQuery;
                            SqlDataReader Editreader = cmmd.ExecuteReader();
                            DataTable Editdata = new DataTable();
                            Editdata.Load(Editreader);
                            List<object[]> TotalResult = Editdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                            var editvalueoflead = (from c in TotalResult[0]
                                                   select c.ToString()).ToList();
                            data.EditFields = EditFields;
                            data.EditFieldsData = editvalueoflead;
                            data.EditDataTypes = Editdatatypes;
                            data.EditLengths = EditLengths;
                            data.EditDescs = EditDescs;
                            data.DropDownValues = DropDownValues;
                            data.IsDropDown = IsDropDown;
                            Conn.Close();
                        }
                    }
                    else
                    {
                        data.EditFieldsData = new List<string>();
                        data.EditDataTypes = new List<string>();
                        data.EditLengths = new List<string>();
                    }

                    List<VMDropDown> StatusDD = new List<VMDropDown>();
                    StatusDD = (from c in dbContext.Stages.Where(m => m.OrganizationID == 0 || m.OrganizationID == OrgID).ToList()
                                select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
                    data.StatusDDL = StatusDD;
                    Data.Add(data);
                    vmquery.SectionsData = Data;
                }
            }
            List<SingleBOField> bofields = new List<SingleBOField>();
            vmquery.SingleBOField = bofields;
            vmquery.Query = query.Query;
            vmquery.QueryID = tab1click.ReportID;
            return vmquery;
        }

        public VMPopupLayout GetPopupLayoutDetails(int PopupID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var LayoutID = dbContext.Popups.Where(m => m.ID == PopupID).Select(m => m.LayoutID).FirstOrDefault();
            VMPopupLayout Layout = new VMPopupLayout();
            Layout = Common.GetLayoutDetails(LayoutID, PopupID, ID, BOID, sNewGUID, iUserID, sOrgName, sDatabase);
            Layout.PopupID = PopupID;
            return Layout;
        }

        public VMPopupLayout GetLayoutDetails(int LayoutID, string sParentGUID, string sSection, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMPopupLayout oLayout = new VMPopupLayout();
            oLayout = dbContext.Layouts.Where(m => m.ID == LayoutID).Select(m => new VMPopupLayout { LayoutID = m.ID, LayoutName = m.LayoutName, LayoutCode = m.LayoutCode, XiParameterID = m.XiParameterID, LayoutType = m.LayoutType, Authentication = m.Authentication, iThemeID = m.iThemeID }).FirstOrDefault();
            var Mappings = new List<VMPopupLayoutMappings>();
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
            var Details = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == LayoutID).Select(m => new VMPopupLayoutDetails { PlaceholderArea = m.PlaceholderArea, PlaceholderName = m.PlaceholderName, LayoutID = m.LayoutID, PlaceHolderID = m.PlaceHolderID, PlaceholderUniqueName = m.PlaceholderUniqueName }).ToList();
            oLayout.Mappings = Mappings;
            oLayout.Details = Details;
            oLayout.sGUID = Guid.NewGuid().ToString();
            oLayout.sNewGUID = oLayout.sGUID;
            if (!string.IsNullOrEmpty(sParentGUID))
            {
                string sSessionID = HttpContext.Current.Session.SessionID;
                //oCache.Init_RuntimeParamSet(sSessionID, oLayout.sNewGUID, sParentGUID, sSection);
            }
            oLayout.sContext = sSection;
            return oLayout;

        }


        //public VMPopupLayout GetInlineLayoutDetails(int InlineID, int ID, int BOID, string sDatabase)
        //{
        //    var LayoutID = dbContext.Popups.Where(m => m.ID == InlineID).Select(m => m.LayoutID).FirstOrDefault();
        //    VMPopupLayout Layout = new VMPopupLayout();
        //    Layout = Common.GetLayoutDetails(LayoutID, ID, BOID, sDatabase);
        //    Layout.PopupID = InlineID;
        //    return Layout;
        //}
        public Dialogs GetDialog(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dialogs App = new Dialogs();
            App = dbContext.Dialogs.Find(ID);
            return App;
        }

        public Popup GetPopupDetailsByID(int ID, string sDatabase)
        {
            Popup App = new Popup();
            ModelDbContext dbContext = new ModelDbContext();
            App = dbContext.Popups.Find(ID);
            return App;
        }

        //public Dialogs GetInlineDetails(int ID, string database)
        //{
        //    var DialogID = ID;
        //    Dialogs App = new Dialogs();
        //    ModeldbContext Spdb = new ModeldbContext();
        //    App = Spdb.Dialogs.Find(DialogID);
        //    return App;
        //}


        public VMPopupLayout GetDialogLayoutDetails(int DialogID, int ID, int BOID, string sNewGUID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var LayoutID = dbContext.Dialogs.Where(m => m.ID == DialogID).Select(m => m.LayoutID).FirstOrDefault();
            VMPopupLayout Layout = new VMPopupLayout();
            Layout = Common.GetLayoutDetails(LayoutID, DialogID, ID, BOID, sNewGUID, iUserID, sOrgName, sDatabase);
            Layout.DialogID = DialogID;
            return Layout;
        }


        #region PopupTabs

        private List<VMKPIResult> KPICirlceForTab(int ReportID, int iUserID, int OrgID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            string Query = "";
            int target = 0;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<VMKPIResult> KPIs = new List<VMKPIResult>();
            KPICircleColors colors = new KPICircleColors();
            KPIIconColors iconscolor = new KPIIconColors();
            List<string> color = new List<string>();
            List<string> iconcolor = new List<string>();
            foreach (var items in colors)
            {
                string str = Convert.ToString(items.KPIColor);
                color.Add(str);
            }
            foreach (var items in iconscolor)
            {
                string str = Convert.ToString(items.KPIColor);
                iconcolor.Add(str);
            }
            int j = 0;
            foreach (var items in Reports)
            {
                Reports report = dbContext.Reports.Find(items.ID);
                Query = report.Query;
                target = 10;
                Common Com = new Common();
                string UserIDs = Com.GetSubUsers(iUserID, OrgID, sDatabase, sOrgName);
                if (Query != null && Query.Length > 0)
                {
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, iUserID, OrgID, 0, 0);
                    VMKPIResult kpi = new VMKPIResult();
                    string[] value = null;
                    using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        Con.Open();
                        Con.ChangeDatabase(sOrgDB);
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        cmd.CommandText = Query;
                        SqlDataReader reader = cmd.ExecuteReader();
                        List<string[]> results = new List<string[]>();
                        int count = reader.FieldCount;
                        string[] rows = new string[count];
                        while (reader.Read())
                        {
                            List<string> values = new List<string>();
                            for (int i = 0; i < count; i++)
                            {
                                values.Add(reader.GetValue(i).ToString());
                            }
                            string[] result = values.ToArray();
                            results.Add(result);
                            value = result;
                        }
                        Con.Close();
                    }
                    int com = Convert.ToInt32(value[0]);
                    double percentage = (double)com / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    kpi.Name = report.Name;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = "fa fa-car";
                    KPIs.Add(kpi);
                    j++;
                }
            }
            return KPIs;
        }

        private List<DashBoardGraphs> GetPieChartForTab(int UserID, string sDatabase, int OrganizationID, int ReportID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            string QueryName = "";
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(UserID, OrganizationID, sDatabase, sOrgName);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Find(items.ID);
                QueryName = model.Name;
                string Query = model.Query;
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrganizationID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    var Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                    if (reader.HasRows == true)
                    {
                        foreach (var items1 in results)
                        {
                            for (int i = 0; i < items1.Count(); i++)
                            {
                                DashBoardGraphs model1 = new DashBoardGraphs();
                                //int ID = Convert.ToInt32(items1[0]);
                                var Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items1[0], sDatabase);
                                if (Name != null)
                                {
                                    model1.label = Name;
                                    model1.value = Convert.ToInt32(items1[1]);
                                    list.Add(model1);
                                }
                            }
                        }
                    }
                    else
                    {
                        DashBoardGraphs model1 = new DashBoardGraphs();
                        list.Add(model1);
                    }
                    Con.Close();
                }
            }

            list.FirstOrDefault().QueryName = QueryName;
            return list;
        }

        private LineGraph GetBarChartForTab(int UserID, string sDatabase, int OrganizationID, int ReportID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string QueryName = "";
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(UserID, OrganizationID, sDatabase, sOrgName);
            LineGraph line = new LineGraph();
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            List<Reports> ids = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var reports in AllReports)
            {
                ids.Add(reports);
            }
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            foreach (var items in ids)
            {
                Reports model = dbContext.Reports.Find(items.ID);
                QueryName = model.Name;
                string Query = model.Query;
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, UserID, OrganizationID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    Con.Close();
                }
                List<List<string>> Chart = new List<List<string>>();
                List<string> XValues = new List<string>();
                List<string> xval = new List<string>();
                XValues.Add("x");
                var Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                if (Keys.Count() > 1)
                {
                    string Name = "";
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    foreach (var item in xval)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], item, sDatabase);
                        XValues.Add(Name);
                    }
                }
                else
                {
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    XValues.AddRange(xval);
                }
                Chart.Add(XValues);
                var types = results.Select(m => m[1]).Distinct();
                foreach (var type in types)
                {
                    List<string> Y = new List<string>();
                    //var ID = Convert.ToInt32(type);
                    string Name = "";
                    if (Keys.Count() > 1)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, sDatabase);
                    }
                    else
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, sDatabase);
                    }
                    if (Name != null)
                    {
                        Y = new List<string> { Name };
                        foreach (var xaxis in xval)
                        {
                            var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2].ToString()).FirstOrDefault();
                            Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                        }
                        Chart.Add(Y);
                    }
                }

                line.Data = Chart;
                line.QueryName = model.Name;
            }
            return line;
        }

        private LineGraph GetLineGraphForTab(int ReportID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            LineGraph line = new LineGraph();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            string Query = "";
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            List<Reports> Reports = new List<Reports>();
            List<Reports> AllReports = dbContext.Reports.Where(m => m.ID == ReportID).ToList();
            foreach (var item in AllReports)
            {
                Reports.Add(item);
            }
            List<string[]> results = new List<string[]>();
            foreach (var items in Reports)
            {
                Reports report = dbContext.Reports.Find(ReportID);
                Query = report.Query;
                Common Com = new Common();
                string UserIDs = Com.GetSubUsers(iUserID, OrgID, sDatabase, sOrgName);
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, iUserID, OrgID, 0, 0);
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    Con.Close();
                }
                List<List<string>> Chart = new List<List<string>>();
                List<string> XValues = new List<string>();
                List<string> xval = new List<string>();
                XValues.Add("x");
                var Keys = ServiceUtil.GetForeginkeyValues(report.Query);
                if (Keys.Count() > 1)
                {
                    string Name = "";
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    foreach (var item in xval)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], item, sOrgDB);
                        XValues.Add(Name);
                    }
                }
                else
                {
                    xval = results.Select(m => m[0]).Distinct().ToList();
                    XValues.AddRange(xval);
                }
                Chart.Add(XValues);
                var types = results.Select(m => m[1]).Distinct();
                foreach (var type in types)
                {
                    List<string> Y = new List<string>();
                    //var ID = Convert.ToInt32(type);
                    string Name = "";
                    if (Keys.Count() > 1)
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, sOrgDB);
                    }
                    else
                    {
                        Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, sOrgDB);
                    }
                    if (Name != null)
                    {
                        Y = new List<string> { Name };
                        foreach (var xaxis in xval)
                        {
                            var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2].ToString()).FirstOrDefault();
                            Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                        }
                        Chart.Add(Y);
                    }
                }
                line.Data = Chart;
                line.QueryName = report.Name;
            }
            return line;
        }

        #endregion PopupTabs

        #endregion Popup


        #region PopupNew

        public VMPopup GetInlineView(string BOName, string Group, int LeadID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BO = dbContext.BOs.Where(m => m.Name == BOName).FirstOrDefault();
            var GroupFields = BO.BOGroups.Where(m => m.GroupName.ToLower() == Group.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            var Fields = GroupFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var BOFields = BO.BOFields.ToList();
            List<VMInlineView> ViewData = new List<VMInlineView>();
            List<string[]> Rows = new List<string[]>();

            if (LeadID > 0)
            {
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = "Select " + GroupFields + " from " + EnumLeadTables.Leads.ToString() + " Where ID=" + LeadID;
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
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
            }
            for (int i = 0; i < Fields.Count(); i++)
            {
                VMInlineView View = new VMInlineView();
                View.Label = BOFields.Where(m => m.Name == Fields[i]).Select(m => m.LabelName).FirstOrDefault();
                if (LeadID > 0)
                {
                    int iFileTypeID = BOFields.Where(m => m.Name == Fields[i]).Where(m => m.BOID == BO.BOID).Select(m => m.FKiFileTypeID).FirstOrDefault();

                    //get BOFieldsID
                    int iBOFieldsID = BOFields.Where(m => m.Name == Fields[i]).Where(m => m.BOID == BO.BOID).Where(m => m.FKiFileTypeID == iFileTypeID).Select(m => m.ID).FirstOrDefault();

                    //Get File settings
                    XIFileTypes Filesettings = new XIFileTypes();
                    string sImgHeight = "";
                    string sImgWidth = "";
                    int iDrillWidth = 0;
                    int iDrillHeight = 0;
                    int iThumbnails = 0;
                    int iPreview = 0;
                    int iMaxImgUploadCount = 0;
                    string sImageCount = "";
                    string sDrilldown = "";
                    string sDrilldownType = "";
                    if (iFileTypeID != 0)
                    {
                        Filesettings = dbContext.XIFileTypes.Find(iFileTypeID);
                        sImgWidth = Filesettings.MaxWidth.ToString() + "px";
                        sImgHeight = Filesettings.MaxHeight.ToString() + "px";
                        iDrillWidth = Filesettings.DrillWidth;
                        iDrillHeight = Filesettings.DrillHeight;
                        iThumbnails = Convert.ToInt32(Filesettings.Thumbnails);
                        iPreview = Convert.ToInt32(Filesettings.Preview);
                        sImageCount = Filesettings.sCount;
                        iMaxImgUploadCount = Convert.ToInt32(Filesettings.MaxCount);
                        sDrilldown = Filesettings.Drilldown;
                        sDrilldownType = Filesettings.DrillDownType;
                    }

                    if (Filesettings.Type == null)
                    {
                        View.IDs = Convert.ToInt32(LeadID) + "_" + Convert.ToInt32(iBOFieldsID);
                        View.Data = Rows[0][i];
                    }
                    else if (Filesettings.Type == "10")
                    {
                        //get file name;
                        List<string> lLabelNames = BOFields.Where(m => m.BOID == BO.BOID).Where(m => m.FKiFileTypeID == iFileTypeID).Select(m => m.Name).ToList();
                        foreach (string Labls in lLabelNames)
                        {
                            int iLblIndex = Fields.IndexOf(Labls);
                            var sFileDetails = new List<string>();
                            var lFilePath = new List<string>();
                            //check if the column is null and also get the IDs if the multiple images are allowed
                            string sDocID = Rows[0][iLblIndex];
                            //Check if ends with","
                            if (sDocID.EndsWith(","))
                            {
                                sDocID = sDocID.Substring(0, sDocID.Length - 1);
                            }
                            if (sDocID != "")
                            {
                                if (sDocID.Contains(","))
                                {

                                    List<string> sFileIDs = sDocID.Split(',').ToList();
                                    foreach (var FleNm in sFileIDs)
                                    {
                                        string sFilePath = "";
                                        XIDocs Docs = dbContext.XIDocs.Find(Convert.ToInt32(FleNm));
                                        //get file path
                                        XIDocTypes DocType = dbContext.XIDocTypes.Find(Docs.FKiDocType);
                                        //if (sFilePath== "")
                                        //{
                                        //check sub directory
                                        string sSubDirectory = DocType.SubDirectory;
                                        if (sSubDirectory.ToLower() == "year/month/day")
                                        {
                                            string sSubDirPath = dbContext.XIDocs.Where(m => m.ID == Docs.ID).Select(m => m.SubDirectoryPath).FirstOrDefault();
                                            sFilePath = DocType.Path + "/" + sSubDirPath;
                                        }
                                        else
                                        {
                                            sFilePath = DocType.Path;
                                        }
                                        //}
                                        lFilePath.Add(sFilePath);
                                        sFileDetails.Add(Docs.FileName);
                                    }
                                }
                                else
                                {
                                    XIDocs Docs = dbContext.XIDocs.Find(Convert.ToInt32(sDocID));
                                    //get file path
                                    XIDocTypes DocType = dbContext.XIDocTypes.Find(Docs.FKiDocType);
                                    //if (sFilePath == "")
                                    //{
                                    string sFilePath = ""; ;
                                    //check sub directory
                                    string sSubDirectory = DocType.SubDirectory;
                                    if (sSubDirectory.ToLower() == "year/month/day")
                                    {
                                        string sSubDirPath = dbContext.XIDocs.Where(m => m.ID == Docs.ID).Select(m => m.SubDirectoryPath).FirstOrDefault();
                                        sFilePath = DocType.Path + "/" + sSubDirPath;
                                    }
                                    else
                                    {
                                        sFilePath = DocType.Path;
                                    }
                                    //}
                                    lFilePath.Add(sFilePath);
                                    sFileDetails.Add(Docs.FileName);
                                }
                                //remove null from the list
                                sFileDetails.RemoveAll(item => item == null);
                            }
                            //check if thumbnails are allowed
                            if (iThumbnails == 10)
                            {
                                var sDupFileDetails = "";
                                if (sFileDetails.Count > 0)
                                {
                                    foreach (var Flnme in sFileDetails)
                                    {
                                        if (Flnme != "")
                                        {
                                            string[] sSplitflName = Flnme.Split('.');
                                            string sName = sSplitflName[0];
                                            string sFormat = sSplitflName[1];
                                            if (sDupFileDetails == "")
                                            {
                                                sDupFileDetails = sName + "_thumb." + sFormat;
                                            }
                                            else
                                            {
                                                sDupFileDetails = sDupFileDetails + "," + sName + "_thumb." + sFormat;
                                            }
                                        }
                                        else
                                        {
                                            //do not consider null;
                                        }
                                    }
                                    View.FileName = sDupFileDetails.Split(',').ToList();
                                }
                            }
                            else
                            {
                                View.FileName = sFileDetails;
                                View.ImgHeight = sImgHeight;
                                View.ImgWidth = sImgWidth;
                            }
                            View.IsPreview = iPreview.ToString();
                            View.IDs = Convert.ToInt32(LeadID) + "_" + Convert.ToInt32(iBOFieldsID);
                            View.FilePath = lFilePath;
                            View.Type = "Image";
                            View.ImageCount = sImageCount;

                            //check whether to show "choose file" button based on the image type.
                            var sGetFileIDs = new List<string>();
                            if (sDocID != "")
                            {
                                sGetFileIDs = sDocID.Split(',').ToList();
                            }

                            int iImageIDCount = sGetFileIDs.Count();
                            if (sImageCount == "10")
                            {
                                if (iImageIDCount == 0)
                                {
                                    View.ShowButton = 1;
                                }
                                else if (iImageIDCount == iMaxImgUploadCount)
                                {
                                    View.ShowButton = 0;
                                }

                                else
                                {
                                    View.ShowButton = 1;
                                }
                            }
                            else if (sImageCount == "20")
                            {
                                if (iImageIDCount == 0)
                                {
                                    View.ShowButton = 1;
                                }
                                else if (iImageIDCount == iMaxImgUploadCount)
                                {
                                    View.ShowButton = 0;
                                }
                                else
                                {
                                    View.ShowButton = 1;
                                }
                            }

                            //Check is drill down allowed.
                            if (sDrilldown == null || sDrilldown == "0" || sDrilldown == "20")
                            {
                                View.IsDrilldown = 0;
                            }
                            else
                            {
                                View.DrilldownType = Convert.ToInt32(sDrilldownType);
                                View.IsDrilldown = 1;
                                //if(iDrillWidth!=0&& iDrillHeight != 0)
                                //{
                                //    var lDrillWidth = new List<string>();
                                //    var lDrillHeight = new List<string>();
                                //    List<string> sFileNames = View.FileName;
                                //    if(sFileNames!=null)
                                //    {
                                //        for (var k = 0; k < sFileNames.Count(); k++)
                                //        {
                                //            List<string> lFlPath = View.FilePath;
                                //            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                                //            string sPath = physicalPath.Substring(0, physicalPath.Length) + lFlPath[k];
                                //            if (sPath.Contains("~/"))
                                //            {
                                //                sPath = sPath.Replace("~/", "");
                                //            }

                                //            if (sPath.Contains('/'))
                                //            {
                                //                sPath = sPath.Replace("/", "\\");
                                //            }
                                //            string sFileNme = sFileNames[k];
                                //            if (sFileNme.Contains("_thumb"))
                                //            {

                                //                sFileNme = sFileNme.Replace("_thumb", "_Org");
                                //            }
                                //            else
                                //            {
                                //                string[] sSplitNme = sFileNme.Split('.');
                                //                sFileNme = sSplitNme[0] + "_Org." + sSplitNme[1];
                                //            }
                                //            Image image = Image.FromFile(sPath + "\\" + sFileNme);
                                //            var ratioX = (double)iDrillWidth / image.Width;
                                //            var ratioY = (double)iDrillHeight / image.Height;
                                //            var ratio = Math.Min(ratioX, ratioY);

                                //            var newWidth = (int)(image.Width * ratio);
                                //            var newHeight = (int)(image.Height * ratio);
                                //            lDrillWidth.Add(newWidth.ToString() + "px");
                                //            lDrillHeight.Add(newHeight.ToString() + "px");
                                //        }
                                //        View.DrillWidth = lDrillWidth;
                                //        View.DrillHeight = lDrillHeight;
                                //    }
                                //    else
                                //    {
                                //        //
                                //    }   
                                //}
                            }
                        }
                    }
                    else if (Filesettings.Type == "20")
                    {
                        List<string> lLabelNames = BOFields.Where(m => m.BOID == BO.BOID).Where(m => m.FKiFileTypeID == iFileTypeID).Select(m => m.Name).ToList();
                        foreach (string Labls in lLabelNames)
                        {
                            int iLblIndex = Fields.IndexOf(Labls);
                            var sDocFileDetails = new List<string>();
                            string sDocFilePath = "";
                            //check if the column is null and also get the IDs if the multiple images are allowed
                            string sDocumentID = Rows[0][iLblIndex];
                            if (sDocumentID != "")
                            {
                                if (sDocumentID.Contains(","))
                                {

                                    //Display icon
                                    List<string> sFileIDs = sDocumentID.Split(',').ToList();
                                    foreach (var FleNm in sFileIDs)
                                    {
                                        string sIcnNme = "";
                                        XIDocs Docs = dbContext.XIDocs.Find(Convert.ToInt32(FleNm));
                                        string sFileNme = Docs.FileName;
                                        string[] sSplitFlNme = sFileNme.Split('.');
                                        string sFrmat = sSplitFlNme[1];
                                        if (sFrmat.ToLower() == "pdf")
                                        {
                                            sIcnNme = "pdf.png";
                                        }
                                        else if (sFrmat.ToLower() == "docx")
                                        {
                                            sIcnNme = "Doc.png";
                                        }
                                        sDocFileDetails.Add(sIcnNme + ":" + sFileNme);
                                    }
                                    sDocFilePath = "~\\Content\\Images";
                                }
                                else
                                {
                                    string sIcnNme = "";
                                    XIDocs Docs = dbContext.XIDocs.Find(Convert.ToInt32(sDocumentID));
                                    string sFileNme = Docs.FileName;
                                    string[] sSplitFlNme = sFileNme.Split('.');
                                    string sFrmat = sSplitFlNme[1];
                                    if (sFrmat.ToLower() == "pdf")
                                    {
                                        sIcnNme = "pdf.png";
                                    }
                                    else if (sFrmat.ToLower() == "docx")
                                    {
                                        sIcnNme = "Doc.png";
                                    }
                                    sDocFileDetails.Add(sIcnNme + ":" + sFileNme);
                                    sDocFilePath = "~\\Content\\Images";
                                }

                                //remove null from the list
                                sDocFileDetails.RemoveAll(item => item == null);
                            }
                            //Check if any preview is allowed else continue
                            View.FileName = sDocFileDetails;
                            View.IDs = Convert.ToInt32(LeadID) + "_" + Convert.ToInt32(iBOFieldsID);
                            View.DocFilePath = sDocFilePath;
                            View.Type = "File";

                            //check whether to show "choose file" button based on the image type.
                            var sGetFileIDs = new List<string>();
                            if (sDocumentID != "")
                            {
                                sGetFileIDs = sDocumentID.Split(',').ToList();
                            }

                            int iFileIDCount = sGetFileIDs.Count();
                            if (sImageCount == "10")
                            {
                                if (iFileIDCount == 0)
                                {
                                    View.ShowButton = 1;
                                }
                                else if (iFileIDCount == iMaxImgUploadCount)
                                {
                                    View.ShowButton = 0;
                                }

                                else
                                {
                                    View.ShowButton = 1;
                                }
                            }
                            else if (sImageCount == "20")
                            {
                                if (iFileIDCount == 0)
                                {
                                    View.ShowButton = 1;
                                }
                                else if (iFileIDCount == iMaxImgUploadCount)
                                {
                                    View.ShowButton = 0;
                                }
                                else
                                {
                                    View.ShowButton = 1;
                                }
                            }
                        }
                    }

                    else
                    {

                    }
                }
                else
                {
                    View.Data = "";
                }
                ViewData.Add(View);
            }
            VMPopup Popup = new VMPopup();
            Popup.InlineData = ViewData;
            return Popup;
        }

        public VMResultList GetHeadingsForList(int ReportID, string SearchType, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            VMResultList Preview = new VMResultList();
            DataContext Spdb = new DataContext(sOrgDB);
            Reports Report = dbContext.Reports.Find(ReportID);
            if (SearchType == EnumSearchType.FilterSearch.ToString())
            {
                Report.IsFilterSearch = true;
                Report.IsNaturalSearch = false;
            }
            else if (SearchType == EnumSearchType.NaturalSearch.ToString())
            {
                Report.IsNaturalSearch = true;
                Report.IsFilterSearch = false;
            }
            var oBO = dbContext.BOs.Find(Report.BOID);
            var BoFields = oBO.BOFields.Where(m => m.BOID == Report.BOID).ToList();
            var MapFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID && m.ClassID == Report.Class).ToList();
            var GroupFields = dbContext.BOGroupFields.Where(m => m.BOID == Report.BOID).ToList();
            if (Report.Query != null)
            {
                int BOID = Report.BOID;
                List<VMDropDown> KeyPositions = new List<VMDropDown>();
                Common Com = new Common();
                var FromIndex = Report.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = Report.Query.Substring(0, FromIndex);
                SelectQuery = SelectQuery.TrimEnd();
                var SelWithGroup = "";
                var regx = new Regex("{.*?}");
                var mathes = regx.Matches(SelectQuery);
                if (mathes.Count > 0)
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.Contains('{'))
                        {
                            int id = Convert.ToInt32(items.Substring(1, items.Length - 2));
                            var Grp = GroupFields.Where(m => m.ID == id).FirstOrDefault();
                            if (Grp.IsMultiColumnGroup)
                            {
                                SelWithGroup = SelWithGroup + Grp.BOFieldNames + ", ";
                            }
                            else
                            {
                                SelWithGroup = SelWithGroup + Grp.GroupName + ", ";
                            }
                        }
                        else
                        {
                            SelWithGroup = SelWithGroup + items + ", ";
                        }
                    }
                    SelWithGroup = SelWithGroup.Substring(0, SelWithGroup.Length - 2);
                }
                else
                {
                    SelWithGroup = SelectQuery;
                }
                var Keys = ServiceUtil.GetForeginkeyValues(" " + SelWithGroup);
                var TargetList = Spdb.Targets.Where(m => m.ReportID == ReportID).ToList();
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<int> Targets = new List<int>();
                if (Report.SelectFields == null)
                {
                    List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in SelectFields)
                    {
                        if (items.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string Head = Regex.Split(items, " as ", RegexOptions.IgnoreCase)[0];
                            Headings.Add(Head);
                        }
                        else
                        {
                            Headings.Add(items);
                        }
                    }
                }
                else
                {
                    Headings = Report.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                List<string> AllHeadings = new List<string>();
                List<string> TableColumns = new List<string>();
                List<int> ColumnReports = new List<int>();
                List<string> MouseOverColumns = new List<string>();
                List<string> Scripts = new List<string>();
                var Columns = new List<string>();
                if (Report.OnClickColumn != null)
                {
                    Columns = Report.OnClickColumn.Split(',').ToList();
                }
                var ColumnIds = new List<string>();
                //if (query.OnColumnClickValue != null)
                //{
                //    ColumnIds = query.OnColumnClickValue;
                //}            
                var str1 = "";
                if (Headings.Contains("ID") == false)
                {
                    //str1 = "No";
                    //Headings.Insert(0, "ID");
                    Preview.IDExists = false;
                }
                else
                {
                    Preview.IDExists = true;
                }
                string allfields = "";
                var groupfieldseditquery2 = "";
                var groupfieldseditquery5 = "";
                if (str1 == "No")
                {
                    //var allfields1 = (query.Query).Insert(7, " ID, ");
                    //allfields = (query.Query).Insert(7, " ID, ");
                    //if (allfields.Contains("ORDER BY") == true && allfields.Contains("GROUP BY") == true)
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //else if (allfields.Contains("GROUP BY") == false)
                    //{
                    //    allfields = allfields1;
                    //}
                    //else
                    //{
                    //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                    //}
                    //if (groupfieldseditquery2 != "")
                    //{
                    //    groupfieldseditquery2 = groupfieldseditquery2 + ", " + "ID" + " ";
                    //    allfields = allfields.Replace(groupfieldseditquery5, groupfieldseditquery2);
                    //}
                    allfields = Report.Query;
                }
                else
                    allfields = Report.Query;
                int FKPosition = 0, i = 0;
                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        string id = items.Substring(1, items.Length - 2);
                        int gid = Convert.ToInt32(id);
                        string groupid = Convert.ToString(gid);
                        BOGroupFields fields = GroupFields.Where(m => m.ID == gid).FirstOrDefault();
                        allfields = allfields.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                        if (fields.IsMultiColumnGroup)
                        {
                            List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                var BoField = BoFields.Where(m => m.Name.ToLower() == names.ToLower()).FirstOrDefault();
                                string aliasname = BoField.LabelName;
                                if (aliasname != null)
                                {
                                    AllHeadings.Add(aliasname);
                                }
                                else
                                {
                                    AllHeadings.Add(names);
                                }
                                TableColumns.Add(names);
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                                KeyPositions.AddRange((from c in Keys.Where(m => m.text == names) select new VMDropDown { text = names, Value = FKPosition }));
                                FKPosition++;
                            }
                        }
                        else
                        {
                            AllHeadings.Add(fields.GroupName);
                            TableColumns.Add(fields.GroupName);
                            Formatting.Add(null);
                            Scripts.Add(null);
                            Targets.Add(0);
                            MouseOverColumns.Add("");
                            FKPosition++;
                        }

                    }
                    else
                    {
                        var Fld = items;
                        if (Columns.Contains(items))
                        {
                            int index = Columns.IndexOf(items);
                            //int ID = Convert.ToInt32(ColumnIds[index]);
                            //ColumnReports.Add(ID);
                        }
                        else
                        {
                            //ColumnReports.Add(0);
                        }
                        string aliasname = "";
                        if (OrgID != 0)
                        {
                            aliasname = MapFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                            if (aliasname != null)
                            {
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        if (string.IsNullOrEmpty(aliasname))
                        {
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                if (BoField != null)
                                {
                                    aliasname = BoField.LabelName;
                                }
                                else
                                {
                                    aliasname = Fld;
                                }
                            }
                            Formatting.Add(BoField.Format);
                            Scripts.Add(BoField.Script);
                            var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                            if (target != null)
                            {
                                Targets.Add(target.Target);
                            }
                            else
                            {
                                Targets.Add(0);
                            }
                            MouseOverColumns.Add(BoField.FKTableName);
                        }
                        if (aliasname == null)
                        {
                            Fld = items;
                            BOFields BoField = new BOFields();
                            if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                            {
                                var OrgName = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                                Fld = OrgName;
                                aliasname = MapFields.Where(m => m.AddField == Fld).Select(m => m.FieldName).FirstOrDefault();
                                if (aliasname == null)
                                {
                                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                                    var regex = new Regex("'(?:''|[^']*)*'");
                                    var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                                    if (matches.Count > 0)
                                    {
                                        fieldname = fieldname.Substring(1, fieldname.Length - 2);
                                    }
                                    BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                    aliasname = fieldname;
                                }
                            }
                            else
                            {
                                BoField = BoFields.Where(m => m.Name.ToLower() == Fld.ToLower()).FirstOrDefault();
                                aliasname = BoField.LabelName;
                            }
                            if (BoField != null)
                            {
                                Formatting.Add(BoField.Format);
                                Scripts.Add(BoField.Script);
                                var target = TargetList.Where(m => m.ColumnID == BoField.ID).FirstOrDefault();
                                if (target != null)
                                {
                                    Targets.Add(target.Target);
                                }
                                else
                                {
                                    Targets.Add(0);
                                }
                                MouseOverColumns.Add(BoField.FKTableName);
                            }
                            else
                            {
                                aliasname = items;
                                Formatting.Add(null);
                                Scripts.Add(null);
                                Targets.Add(0);
                                MouseOverColumns.Add("");
                            }
                        }
                        //string aliasname = dbContext.BOFields.Where(m => m.Name == items && m.BOID == BOID).Select(m => m.AliasName).FirstOrDefault();                        
                        KeyPositions.AddRange((from c in Keys.Where(m => m.text == Fld) select new VMDropDown { text = Fld, Value = FKPosition }));
                        if (aliasname != null)
                        {
                            AllHeadings.Add(aliasname);
                        }
                        FKPosition++;
                        TableColumns.Add(Fld);
                    }
                    i++;
                }
                Preview.BOID = Report.BOID;
                Preview.BO = oBO.Name;
                Preview.IsCreate = Report.IsCreate;
                Preview.IsEdit = Report.IsEdit;
                Preview.IsDelete = Report.IsDelete;
                Preview.Headings = AllHeadings;
                if (Report.CreateGroupID > 0)
                {
                    var CGroup = oBO.BOGroups.Where(m => m.ID == Report.CreateGroupID).FirstOrDefault();
                    if (CGroup != null)
                    {
                        Preview.sCreateGroup = CGroup.GroupName;
                    }
                }
                if (Report.EditGroupID > 0)
                {
                    var EGroup = oBO.BOGroups.Where(m => m.ID == Report.EditGroupID).FirstOrDefault();
                    if (EGroup != null)
                    {
                        Preview.sEditGroup = EGroup.GroupName;
                    }
                }
                Preview.IsRowClick = Report.IsRowClick;
                Preview.XiLinkID = Report.RowXiLinkID;
                Preview.ResultListDisplayType = Report.ResultListDisplayType;
                Preview.TableColumns = TableColumns;
                Preview.iLayoutID = Report.iLayoutID;
                return Preview;
            }
            else
            {
                VMResultList QPreview = new VMResultList();
                Preview.IsQueryExists = false;
                return QPreview;
            }
        }

        public DTResponse GetReportResult(jQueryDataTableParamModel param, VMQuickSearch Search, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser, List<cNameValuePairs> nParams)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            //button1_Click();
            List<string> AllHeadings = new List<string>();
            try
            {
                //DataContext Spdb = new DataContext(database);
                VMResultList vmquery = new VMResultList();
                //var AllLeads = 
                var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                var sOrgDB = UserDetails.sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                Reports query = dbContext.Reports.Find(Search.ReportID);
                int BOID = query.BOID;
                Common Com = new Common();
                var FromIndex = query.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                var SelectQuery = query.Query.Substring(0, FromIndex);
                //var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
                List<string> Headings = new List<string>();
                List<string> Formatting = new List<string>();
                List<string> Scripts = new List<string>();
                List<int> Targets = new List<int>();
                var Heads = GetHeadings(Search.ReportID, null, Search.database, Search.OrgID, Search.UserID, sOrgName);
                Headings = Heads.Headings;
                Formatting = Heads.Formats;
                Targets = Heads.Targets;
                Scripts = Heads.Scripts;
                var oBO = dbContext.BOs.Find(BOID);
                ///var NewSelectQuery = ServiceUtil.GetFKLabelGroup(oBO, Heads.TableColumns, SelectQuery, iUserID, sOrgName, sDatabase);

                string NewQuery = Heads.Query;
                //NewQuery = NewQuery.Replace(SelectQuery, NewSelectQuery);
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Search.UserID).Select(m => m.RoleID).FirstOrDefault();
                //string UserIDs = Com.GetSubUsers(Search.UserID, Search.OrgID, sDatabase, sOrgName);
                string Query = oCache.ReplaceExpressionWithCacheValue(NewQuery, nParams);
                //Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Search.UserID, Search.OrgID, Search.LeadID);
                Query = ServiceUtil.ReplaceGuestUser(Query, sCurrentGuestUser);
                //Query = ReplaceExpressions(NewQuery, Search.OrgID, Search.LeadID);
                if (Search.SearchType == "FilterSearch")
                {
                    if (param.Fields != null && param.Fields.Length > 0)
                    {
                        var Condition = ServiceUtil.GetDynamicSearchStrings(param.Fields, param.Optrs, param.Values);
                        if (Condition.Length > 0)
                        {
                            Query = ServiceUtil.AddSearchParameters(Query, Condition);
                        }
                    }
                }
                //else if (param.Fields != null && param.Fields.Length > 0)
                //{
                //    var Condition = ServiceUtil.GetDynamicSearchStrings(param.Fields, param.Optrs, param.Values);
                //    if (Condition.Length > 0)
                //    {
                //        Query = ServiceUtil.AddSearchParameters(Query, Condition);
                //    }
                //}
                if (Search.SearchType == "NaturalSearch")
                {
                    string NewSearchText = GetSearchString(param.SearchText, Search.ReportID, Search.OrgID, Search.UserID, Search.SearchType, Search.database, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (Search.SearchText != null && Search.SearchText.Length > 0)
                {
                    string NewSearchText = GetSearchString(Search.SearchText, Search.ReportID, Search.OrgID, Search.UserID, Search.SearchType, Search.database, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, NewSearchText);
                }
                if (query.ParentID > 0)
                {
                    Query = ServiceUtil.AddParentWhereConditon(Query, Search.FKi1ClickIDXIGUID);
                }
                if (Search.ReportColumns != null && Search.ReportColumns.Length > 0)
                {
                    var wherecondition = Com.GetReportColumnsWhereCondition(iUserID, Search.BaseID, Search.ReportColumns, Search.OrgID, sDatabase, sOrgName);
                    Query = ServiceUtil.AddSearchParameters(Query, wherecondition);
                }
                var Location = dbCore.XIAppUsers.Find(Search.UserID);

                var BODetails = dbContext.BOs.Where(m => m.BOID == query.BOID).FirstOrDefault();
                string BOName = BODetails.Name;
                if (BODetails.ClassName != null)
                {
                    ExecutePrePersist(BOID, sDatabase);
                }
                if (Search.Role != EnumRoles.SuperAdmin.ToString() && Search.Role != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Search.OrgID);
                    var LocCondition = "";
                    var Locs = Location.sLocation.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in Locs)
                    {
                        LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Search.OrgID + "_" + items + "' or ";
                    }
                    LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                    LocCondition = "(" + LocCondition + ")";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                else if (Search.Role != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Search.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Search.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                List<string[]> results = new List<string[]>();
                List<string[]> fresults = new List<string[]>();
                List<object[]> TotalResult = new List<object[]>();
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    string sortExpression = data.Columns[param.iSortCol].ToString();
                    string sortDirection = param.sSortDir;
                    DataView dv = data.DefaultView;
                    dv.Sort = sortExpression + " " + sortDirection;
                    data = dv.ToTable();
                    reader.Close();
                    //Get Codings

                    TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    List<object[]> Res = new List<object[]>();

                    if (query.ResultListDisplayType == 1 || Search.ShowType == EnumLocations.Dashboard.ToString())
                    {
                        Res = TotalResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                    }
                    for (int i = 0; i < Res.Count(); i++)
                    {
                        List<string> NewRes = new List<string>();
                        for (int j = 0; j < Res[i].Count(); j++)
                        {
                            var oBOField = BODetails.BOFields.Where(m => m.LabelName == Headings[j]).FirstOrDefault();
                            if (Scripts[j] != null && Scripts[j].Length > 0)
                            {
                                //long LeadID = Convert.ToInt64(Res[i][0]);
                                //var ScrpResult = button1_Click(Scripts[j], AllLeads.Where(m => m.ID == Convert.ToInt64(Res[i][0])).FirstOrDefault());
                                //var regex = new Regex(@"(?<=\{)[^}]*(?=\})");
                                //var Fields = new List<string>();
                                //foreach (Match match in regex.Matches(Scripts[j]))
                                //{
                                //    Fields.Add(match.Value);
                                //}
                                //DataTable Cdata = new DataTable();
                                //var Dtypes = new List<List<string>>();
                                //var Script = Scripts[j].ToString();
                                //string columns = "";
                                //foreach (var items in Fields)
                                //{
                                //    var scr = items.Split('.').ToList();
                                //    Dtypes.Add(scr);
                                //    columns = columns + scr[1] + ", ";
                                //}
                                //columns = columns.Substring(0, columns.Length - 2);
                                //var FIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                                //var CodingQuery = "Select " + columns + " " + Query.Substring(FIndex, Query.Length - FIndex);
                                //SqlCommand cmmd = new SqlCommand();
                                //cmmd.Connection = Con;
                                //cmmd.CommandText = CodingQuery;
                                //reader = cmmd.ExecuteReader();
                                //Cdata.Load(reader);
                                //reader.Close();
                                //List<object[]> CodeResult = Cdata.AsEnumerable().Select(m => m.ItemArray).ToList();
                                //List<object[]> Codes = new List<object[]>();
                                //Codes = CodeResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                                //List<string> ScrVals = new List<string>();
                                //for (int h = 0; h < Dtypes.Count(); h++)
                                //{
                                //    if (Dtypes[h][2] == "formattedvalue")
                                //    {
                                //        var Value = Com.ReplaceForeginKeyValues(new VMDropDown { text = Dtypes[h][1] }, Codes[i][h].ToString(), Search.database);
                                //        if (Value != null && Value.Length > 0)
                                //        {
                                //            ScrVals.Add(Value);
                                //        }
                                //        else
                                //        {
                                //            ScrVals.Add(Codes[i][h].ToString());
                                //        }
                                //    }
                                //    else
                                //    {
                                //        ScrVals.Add(Codes[i][h].ToString());
                                //    }
                                //}
                                //for (int m = 0; m < Fields.Count(); m++)
                                //{
                                //    Script = Script.Replace("{" + Fields[m] + "}", ScrVals[m]);
                                //}
                                //string ScrpResult = "";
                                //using (Microsoft.CSharp.CSharpCodeProvider foo = new Microsoft.CSharp.CSharpCodeProvider())
                                //{
                                //    //Script = "if(50==50 && \"NR32 1DR\".Contains(\"DR\")){ return \"<TABLE>XYZ</TABLE>\";}else return \"<TABLE>PQR</TABLE>\"";
                                //    var res = foo.CompileAssemblyFromSource(
                                //        new System.CodeDom.Compiler.CompilerParameters()
                                //        {
                                //            GenerateInMemory = true
                                //        },
                                //        "public class FooClass { string i=\"007\"; public string Execute() {return i;}}"
                                //    );
                                //    var type = res.CompiledAssembly.GetType("FooClass");
                                //    var obj = Activator.CreateInstance(type);
                                //    ScrpResult = type.GetMethod("Execute").Invoke(obj, new object[] { }).ToString();
                                //}
                                //NewRes.Add(ScrpResult);
                            }
                            else if (!string.IsNullOrEmpty(oBOField.FKTableName))
                            {
                                if (!string.IsNullOrEmpty(Res[i][j].ToString()))
                                {
                                    var FKData = oXIAPI.ResolveGroupFieldsWithValues("Label", Convert.ToInt32(Res[i][j]), oBOField.FKTableName, iUserID, sOrgName, sDatabase);
                                    if (!string.IsNullOrEmpty(FKData))
                                    {
                                        NewRes.Add(FKData);
                                    }
                                    else
                                    {
                                        NewRes.Add(Res[i][j].ToString());
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                            else if (oBOField.IsOptionList)
                            {
                                if (!string.IsNullOrEmpty(Res[i][j].ToString()))
                                {
                                    var oVal = Res[i][j].ToString();
                                    var OptionName = dbContext.BOOptionLists.Where(m => m.BOFieldID == oBOField.ID && m.sValues == oVal).FirstOrDefault();
                                    if (OptionName != null)
                                    {
                                        NewRes.Add(OptionName.sOptionName);
                                    }
                                    else
                                    {
                                        NewRes.Add(Res[i][j].ToString());
                                    }
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }


                            }
                            else if (Formatting[j] != null)
                            {
                                if (Formatting[j] == "%")
                                {
                                    if (Targets[j] != 0)
                                    {
                                        NewRes.Add(string.Format("{0}%", Res[i][j]) + "<span class='targetcolor'></span>");
                                    }
                                    else
                                    {
                                        NewRes.Add(string.Format("{0}%", Res[i][j]));
                                    }
                                }
                                else if (Formatting[j] == "en-GB")
                                {
                                    if (Targets[j] != 0)
                                    {
                                        CultureInfo rgi = new CultureInfo(Formatting[j]);
                                        string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                        if (Convert.ToInt32(Res[i][j]) > Targets[j])
                                        {
                                            NewRes.Add(totalValueCurrency + "<span class='targetgreencolor'></span>");
                                        }
                                        else
                                        {
                                            NewRes.Add(totalValueCurrency + "<span class='targetredcolor'></span>");
                                        }
                                    }
                                    else
                                    {
                                        CultureInfo rgi = new CultureInfo(Formatting[j]);
                                        string totalValueCurrency = string.Format(rgi, "{0:c}", Res[i][j]);
                                        NewRes.Add(totalValueCurrency);
                                    }
                                }
                                else
                                {
                                    if (Targets[j] != 0)
                                    {
                                        NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]) + "<span class='targetcolor'></span>");
                                    }
                                    else
                                    {
                                        NewRes.Add(String.Format("{0:" + Formatting[j] + "}", Res[i][j]));
                                    }
                                }
                            }
                            else
                            {
                                if (Targets[j] != 0)
                                {
                                    NewRes.Add(Res[i][j].ToString() + "<span class='targetcolor'></span>");
                                }
                                else
                                {
                                    NewRes.Add(Res[i][j].ToString());
                                }
                            }
                        }

                        results.Add(NewRes.ToArray());
                    }
                    if (Search.SearchText != null && Search.SearchType == "Quick")
                    {
                        foreach (var item in results)//Highlight the SearchedText 
                        {
                            for (int i = 0; i <= item.Length - 1; i++)
                            {
                                if (item[i].IndexOf(Search.SearchText, 0, StringComparison.CurrentCultureIgnoreCase) != -1)
                                {
                                    string input = item[i];
                                    string pattern = Search.SearchText;
                                    string replacement = string.Format("<strong class='themecolor'>{0}</strong>", "$0");
                                    var result = Regex.Replace(input, pattern, replacement, RegexOptions.IgnoreCase);
                                    item[i] = result;
                                }
                            }
                        }
                    }
                    Con.Close();
                }
                if (BODetails.ClassName != null)
                {
                    ExecutePostPersist(BOID, sDatabase);
                }
                if (results.Count() > 0)
                {
                    foreach (var res in results)
                    {
                        List<string> list = new List<string>(res);
                        list.Add(res[0]);
                        if (oBO.Name.ToLower() == "Aggregations".ToLower())
                        {
                            list.Add("");
                            list.Add("");
                        }
                        string[] Arrayres = list.ToArray();
                        fresults.Add(Arrayres);
                    }
                }

                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = TotalResult.Count(),
                    iTotalDisplayRecords = TotalResult.Count(),
                    aaData = fresults,
                    Headings = Headings
                };
            }
            catch (Exception ex)
            {
                return new DTResponse()
                {
                    sEcho = param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = new List<string[]>(),
                    Headings = AllHeadings
                };
            }
        }

        private string ReplaceExpressions(string Query, int OrgID, int LeadID)
        {
            Regex regex = new Regex(@"(?<=\{)[^}]*(?=\})", RegexOptions.IgnoreCase);
            MatchCollection matches = regex.Matches(Query);
            foreach (var match in matches)
            {

            }
            return null;
        }

        private void ExecutePrePersist(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID && m.Script != null).ToList();
            foreach (var Field in BOFields)
            {
                if (Field.Script != null)
                {

                }
            }
        }

        private void ExecutePostPersist(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID && m.Script != null).ToList();
            foreach (var Field in BOFields)
            {
                if (Field.Script != null)
                {

                }
            }
        }

        public int EditData(VMSaveInlineEdit Savedata, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var BO = dbContext.BOs.Where(m => m.Name == "Leads").FirstOrDefault();
            var BoFields = BO.BOFields;
            //var oBOInstance = Get_BOInstance(BO.Name,"")
            //var sResult = MapFormToInstance(Savedata, oBOInstance)

            var MappedFields = Spdb.MappedFields.Where(m => m.OrganizationID == OrgID).ToList();
            foreach (var item in Savedata.FormValues)
            {
                if (item.Data != null)
                {
                    var label = item.Label.Split('-')[0];
                    if (label != "ID")
                    {
                        string FieldName = "";
                        FieldName = MappedFields.Where(m => m.FieldName == label).Select(m => m.AddField).FirstOrDefault();
                        if (FieldName == null)
                        {
                            FieldName = BoFields.Where(m => m.LabelName == label).Select(m => m.Name).FirstOrDefault();
                            if (FieldName == null)
                            {
                                FieldName = label;
                            }
                        }
                        //to do BO prepersist
                        Spdb.Database.ExecuteSqlCommand("UPDATE Leads SET" + " " + FieldName + "=" + "'" + item.Data + "'" + " " + "WHERE" + " ID=" + Savedata.iInstanceID + "");
                        //to do BO Postpersist
                    }
                }
            }
            return 0;
        }

        //public List<RightMenuTrees> GetMenus(List<string> MenuIDs, int OrgID)
        //{
        //    List<RightMenuTrees> Menus = new List<RightMenuTrees>();
        //    if (MenuIDs.Count() > 0)
        //    {
        //        var IDs = new List<int>();
        //        foreach (var items in MenuIDs)
        //        {
        //            IDs.Add(Convert.ToInt32(items));
        //        }
        //        Menus = dbContext.RightMenuTrees.Where(m => IDs.Contains(m.ID) && m.OrgID == OrgID && m.XiLinkID > 0).ToList();
        //    }
        //    else
        //    {
        //        Menus = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID && m.XiLinkID > 0).ToList();
        //    }            
        //    return Menus;
        //}

        //public List<RightMenuTrees> GetMenus(string MenuName, int OrgID,string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    List<RightMenuTrees> Menus = new List<RightMenuTrees>();
        //    if (MenuName != null && MenuName.Length > 0)
        //    {
        //        string MenuID = dbContext.RightMenuTrees.Where(m => m.RootName == MenuName).Where(m => m.ParentID == "#").Select(m => m.ID).FirstOrDefault().ToString();
        //        if (MenuID != null && MenuID.Length > 0)
        //        {
        //            Menus = dbContext.RightMenuTrees.Where(m => m.ParentID == MenuID).ToList();
        //        }
        //    }
        //    //if (MenuName.Count() > 0)
        //    //{
        //    //    List<string> Names = new List<string>();
        //    //    foreach (var items in MenuName)
        //    //    {
        //    //        Names.Add(items);
        //    //    }
        //    //    Menus = dbContext.RightMenuTrees.Where(m => Names.Contains(m.Name) && m.OrgID == OrgID && m.XiLinkID > 0).ToList();
        //    //}
        //    //else
        //    //{
        //    //    Menus = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID && m.XiLinkID > 0).ToList();
        //    //}
        //    return Menus;
        //    //}
        //}

        public List<RightMenuTrees> GetMenus(string MenuName, int iUserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<RightMenuTrees> lMenuTree = new List<RightMenuTrees>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            string MainID = dbContext.RightMenuTrees.Where(m => m.ParentID == "#" && m.RoleID == RoleID && m.RootName == MenuName).Select(m => m.MenuID).FirstOrDefault();
            lMenuTree = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID && m.OrgID == OrgID && m.ParentID == MainID && m.StatusTypeID == 10).OrderBy(m => m.Priority).ToList();
            var Data = Countdata(lMenuTree, sDatabase);
            return lMenuTree;
        }

        public List<RightMenuTrees> Countdata(List<RightMenuTrees> Menus, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            foreach (var items in Menus)
            {
                var ID = items.MenuID;
                items.SubGroups = dbContext.RightMenuTrees.Where(m => m.ParentID == ID).OrderBy(m => m.Priority).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    Countdata(items.SubGroups, sDatabase);
                }
            }
            return Menus;
        }

        public List<RightMenuTrees> GetChildForMenu(int ID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<RightMenuTrees> lMenuTree = new List<RightMenuTrees>();
            var GetMenuID = dbContext.RightMenuTrees.Where(m => m.ID == ID).Select(m => m.MenuID).FirstOrDefault();
            string RootName = dbContext.RightMenuTrees.Where(m => m.ID == ID).Select(m => m.RootName).FirstOrDefault();
            lMenuTree = dbContext.RightMenuTrees.Where(m => m.ParentID == GetMenuID && m.OrgID == OrgID).Where(m => m.RootName == RootName).OrderBy(m => m.Priority).ToList();
            return lMenuTree;
        }

        public string GetLabelData(int iBOID, string Label, int i1ClickID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            if (iBOID == 0)
            {
                var Report = dbContext.Reports.Find(i1ClickID);
                iBOID = Report.BOID;
            }
            var oBO = dbContext.BOs.Find(iBOID);
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            string sOrgDB = sDatabase;
            if (UserDetails.FKiOrgID > 0)
            {
                sOrgDB = UserDetails.sUserDatabase;
            }
            string sTableName = string.Empty;
            if (!string.IsNullOrEmpty(oBO.TableName))
            {
                sTableName = oBO.TableName;
            }
            else
            {
                sTableName = oBO.Name;
            }
            List<string[]> Rows = new List<string[]>();
            var LabelFields = string.Empty;
            var oGroup = oBO.BOGroups.Where(m => m.GroupName.ToLower() == ServiceConstants.LabelGroup.ToLower()).FirstOrDefault();
            if (oGroup != null)
            {
                if (!oGroup.IsMultiColumnGroup)
                {
                    LabelFields = oGroup.BOSqlFieldNames;
                }
                else
                {
                    LabelFields = oGroup.BOFieldNames;
                }
            }
            if (!string.IsNullOrEmpty(LabelFields))
            {
                var LField = string.Empty;
                var NewLabel = LabelFields.Split(',');
                if (NewLabel.Count() > 0)
                {
                    LField = NewLabel[1];
                }
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = "Select " + LField + " from " + sTableName + " Where" + LField + "='" + Label + "'";
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
                if (Rows != null && Rows.Count() > 0)
                {
                    return Rows[0][0].ToString();
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        #endregion PopupNew

        #region UserDialogs

        public int SaveUserDialog(int QueryID, int iUserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            UserDialogs Dialog = new UserDialogs();
            Dialog = dbContext.UserDialogs.Where(m => m.UserID == iUserID && m.OneClickID == QueryID).FirstOrDefault();
            if (Dialog != null)
            {
                Dialog.Status = true;
                dbContext.SaveChanges();
            }
            else
            {
                Dialog = new UserDialogs();
                Dialog.UserID = iUserID;
                Dialog.OrganizationID = OrgID;
                Dialog.OneClickID = QueryID;
                Dialog.Status = true;
                dbContext.UserDialogs.Add(Dialog);
                dbContext.SaveChanges();
            }
            return Dialog.ID;
        }

        public List<VMReports> GetUserDialogs(int iUserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMReports> Dialogs = new List<VMReports>();
            var DialogOneClicks = dbContext.UserDialogs.Where(m => m.UserID == iUserID && m.OrganizationID == OrgID && m.Status == true).Select(m => m.OneClickID).ToList();
            Dialogs = (from r in dbContext.Reports.Where(m => DialogOneClicks.Contains(m.ID))
                       select new VMReports
                       {
                           ID = r.ID,
                           ResultIn = r.ResultIn,
                           PopupType = r.PopupType,
                           PopupWidth = r.PopupWidth,
                           PopupHeight = r.PopupHeight,
                           DialogAt1 = r.DialogAt1,
                           DialogAt2 = r.DialogAt2,
                           DialogMy1 = r.DialogMy1,
                           DialogMy2 = r.DialogMy2
                       }
                               ).ToList();
            return Dialogs;
        }

        #endregion UserDialogs


        #region Graphs

        public LineGraph GetBarChart(VMChart Charts, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Charts.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(Charts.UserID, Charts.OrgID, sDatabase, sOrgName);
            List<DashBoardGraphs> list = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            Reports model = dbContext.Reports.Find(Charts.ReportID);
            string Query = "";
            List<VMDropDown> Keys = new List<VMDropDown>();
            string Type = ((EnumDisplayTypes)model.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.BarChart.ToString())
            {
                if (Charts.Query != null && Charts.Query.Length > 0)
                {
                    Query = Charts.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(Query);
                }
                else
                {
                    Query = model.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(model.Query);
                }
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Charts.UserID, Charts.OrgID, 0, 0);
                if (Charts.ClassFilter != 0 || Charts.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, Charts.OrgID, UserIDs, Charts.ClassFilter, Charts.DateFilter);
                }
                var Location = dbCore.XIAppUsers.Find(Charts.UserID);
                string BOName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
                if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Charts.OrgID);
                    var LocCondition = "";
                    var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in Locs)
                    {
                        LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Charts.OrgID + "_" + items + "' or ";
                    }
                    LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                    LocCondition = "(" + LocCondition + ")";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Charts.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Charts.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();

                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            values.Add("0");
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
            }
            List<List<string>> Graph = new List<List<string>>();
            List<string> XValues = new List<string>();
            List<string> xval = new List<string>();
            XValues.Add("x");
            if (Keys.Count() > 1)
            {
                string Name = "";
                xval = results.Select(m => m[0]).Distinct().ToList();
                foreach (var items in xval)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items, Charts.Database);
                    XValues.Add(Name);
                }
            }
            else
            {
                xval = results.Select(m => m[0]).Distinct().ToList();
                XValues.AddRange(xval);
            }
            Graph.Add(XValues);
            var types = results.Select(m => m[1]).Distinct();
            foreach (var type in types)
            {
                List<string> Y = new List<string>();
                //var ID = Convert.ToInt32(type);
                string Name = "";
                if (Keys.Count() > 1)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, Charts.Database);
                }
                else
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, Charts.Database);
                }
                if (Name != null)
                {
                    Y = new List<string> { Name };
                    foreach (var xaxis in xval)
                    {
                        var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[1].ToString()).FirstOrDefault();
                        Y.Add(string.IsNullOrWhiteSpace(yvalue) ? "0" : yvalue);
                    }
                    Graph.Add(Y);
                }
            }
            LineGraph line = new LineGraph();
            line.Data = Graph;
            line.QueryName = model.Name;
            line.ShowAs = model.Title;
            if (model.IsColumnClick)
            {
                line.IsColumnClick = model.IsColumnClick;
                line.OnClickColumn = model.OnClickColumn;
                line.OnClickResultID = model.OnColumnClickValue;
                line.OnClickParameter = model.OnClickParameter;
                line.OnClickCell = model.OnClickCell;
            }
            return line;
        }

        public GraphData GetPieChart(VMChart Chart, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            //var roels = GetParentRoles(UserRoleID);
            //roels.Add(UserRoleID);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Common Com = new Common();
            string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
            GraphData GraphData = new GraphData();
            List<DashBoardGraphs> PieData = new List<DashBoardGraphs>();
            List<string[]> results = new List<string[]>();
            Reports model = dbContext.Reports.Find(Chart.ReportID);
            string Type = ((EnumDisplayTypes)model.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.PieChart.ToString())
            {
                string Query = "";
                if (Chart.Query != null && Chart.Query.Length > 0)
                {
                    Query = Chart.Query;
                }
                else
                {
                    Query = model.Query;
                }
                Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                {
                    Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                }
                var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                string BOName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
                if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "";
                    if (Location.sLocation != null)
                    {
                        var Locs = Location.sLocation.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + items + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                }
                else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                {
                    Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                    Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                }
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                VMKPIResult kpi = new VMKPIResult();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                string[] value = null;
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            values.Add(reader.GetValue(i).ToString());
                        }
                        else
                        {
                            values.Add("0");
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                    value = result;
                }
                int j = 0;
                var Keys = ServiceUtil.GetForeginkeyValues(Query);
                if (reader.HasRows == true)
                {
                    foreach (var items1 in results)
                    {
                        for (int i = 0; i < items1.Count(); i++)
                        {
                            DashBoardGraphs Values = new DashBoardGraphs();
                            //int ID = Convert.ToInt32(items1[0]);
                            if (Keys.Count() > 0)
                            {
                                var Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items1[0], Chart.Database);
                                if (Name != null)
                                {
                                    Values.label = Name;
                                    Values.value = Convert.ToInt32(items1[1]);
                                    PieData.Add(Values);
                                }
                            }
                            else
                            {
                                Values.label = items1[0];
                                Values.value = Convert.ToInt32(items1[1]);
                                PieData.Add(Values);
                            }

                        }
                        j++;
                    }
                }
                else
                {
                    DashBoardGraphs model1 = new DashBoardGraphs();
                    PieData.Add(model1);
                }
                Con.Close();
                GraphData.ReportID = Chart.ReportID;
                GraphData.PieData = PieData;
                GraphData.QueryName = model.Name;
                GraphData.ShowAs = model.Title;
                if (model.IsColumnClick)
                {
                    GraphData.IsColumnClick = true;
                    GraphData.OnClickColumn = model.OnClickColumn;
                    GraphData.OnClickResultID = model.OnColumnClickValue;
                }
            }
            return GraphData;
        }

        public LineGraph GetLineChart(VMChart Chart, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            //VMForeginKeys Keys = new VMForeginKeys();
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            string Query = "", QueryName = "";
            List<string[]> results = new List<string[]>();
            Common Com = new Common();
            Reports report = dbContext.Reports.Find(Chart.ReportID);
            List<VMDropDown> Keys = new List<VMDropDown>();
            string Type = ((EnumDisplayTypes)report.DisplayAs).ToString();
            if (Type == EnumDisplayTypes.LineChart.ToString())
            {
                if (Chart.Query != null && Chart.Query.Length > 0)
                {
                    Query = Chart.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(Query);
                }
                else
                {
                    Query = report.Query;
                    Keys = ServiceUtil.GetForeginkeyValues(report.Query);
                }
                string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
                if (Query != null && Query.Length > 0)
                {
                    //Send Query For Modification
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                    if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                    {
                        Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                    }
                    var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                    string BOName = dbContext.BOs.Where(m => m.BOID == report.BOID).Select(m => m.Name).FirstOrDefault();
                    if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "";
                        var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + items + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    VMKPIResult kpi = new VMKPIResult();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                    string[] value = null;
                    while (reader.Read())
                    {
                        List<string> values = new List<string>();
                        for (int i = 0; i < count; i++)
                        {
                            if (!reader.IsDBNull(i))
                            {
                                values.Add(reader.GetValue(i).ToString());
                            }
                            else
                            {
                                values.Add("0");
                            }
                        }
                        string[] result = values.ToArray();
                        results.Add(result);
                        value = result;
                    }
                    if (reader.HasRows == false)
                    {
                        //results.Add("");
                    }
                    Con.Close();
                }
                QueryName = report.Name;
            }

            List<List<string>> Graph = new List<List<string>>();
            List<string> XValues = new List<string>();
            List<string> xval = new List<string>();
            XValues.Add("x");
            if (Keys.Count() > 1)
            {
                string Name = "";
                xval = results.Select(m => m[0]).Distinct().ToList();
                foreach (var items in xval)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], items, Chart.Database);
                    XValues.Add(Name);
                }
            }
            else
            {
                xval = results.Select(m => m[0]).Distinct().ToList();
                XValues.AddRange(xval);
            }
            Graph.Add(XValues);
            var types = results.Select(m => m[1]).Distinct();
            foreach (var type in types)
            {
                List<string> Y = new List<string>();
                //var ID = Convert.ToInt32(type);
                string Name = "";
                if (Keys.Count() > 1)
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[1], type, Chart.Database);
                }
                else
                {
                    Name = ServiceUtil.ReplaceForeginKeyValues(Keys[0], type, Chart.Database);
                }

                if (Name != null)
                {
                    Y = new List<string> { Name };
                    foreach (var xaxis in xval)
                    {
                        var grouped = results.Where(m => m[0] == type && m[0] == xaxis).GroupBy(x => x[0])
                     .Select(g => new
                     {
                         Name = g.Key,
                         Sum = g.Sum(x => int.Parse(x[2]))
                     });
                        var Valuess = grouped.Select(m => m.Sum).ToList();
                        int Valuessssss = 0;
                        if (Valuess.Count() > 0)
                        {
                            Valuessssss = Valuess[0];
                        }
                        else
                        {
                            Valuessssss = 0;
                        }
                        //var yvalue = results.Where(m => m[1] == type && m[0] == xaxis).Select(m => m[2]).ToString();
                        Y.Add(string.IsNullOrWhiteSpace(Valuessssss.ToString()) ? "0" : Valuessssss.ToString());
                    }
                    Graph.Add(Y);
                }
            }
            LineGraph line = new LineGraph();
            line.Data = Graph;
            line.QueryName = QueryName;
            line.InnerReportID = report.InnerReportID;
            line.ShowAs = report.Title;
            if (report.IsColumnClick)
            {
                line.IsColumnClick = report.IsColumnClick;
                line.OnClickColumn = report.OnClickColumn;
                line.OnClickResultID = report.OnColumnClickValue;
                line.OnClickParameter = report.OnClickParameter;
                line.OnClickCell = report.OnClickCell;
            }
            return line;
        }

        public List<VMKPIResult> GetKPICircleResult(VMChart Chart, int iUserID, string sDatabase, string sOrgName)
        {

            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            //VMForeginKeys Keys = new VMForeginKeys();
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == Chart.UserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            string Query = "";
            var ReportID = "";
            int target = 0, com;
            List<UserReports> Reports = new List<UserReports>();
            List<string> Vistiblity = new List<string>();
            int DisplayType = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.KPICircle.ToString());
            int DType = (Int32)Enum.Parse(typeof(EnumLocations), EnumLocations.Dashboard.ToString());
            if (Chart.ReportID > 0)
            {
                var userreport = dbContext.UserReports.Where(m => m.RoleID == UserRoleID && m.Location == DType && m.DisplayAs == DisplayType).ToList();
                Reports.AddRange(userreport);
                //Vistiblity.Add(id.Split('-')[1]);
                //var IDs = ReportID.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                //foreach (var id in IDs)
                //{
                //    int ID = Convert.ToInt32(id.Split('-')[0]);

                //}
            }
            else
            {
                List<UserReports> AllReports = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.Location == DType).Where(m => m.DisplayAs == DisplayType).ToList();
                foreach (var item in AllReports)
                {
                    Reports.Add(item);
                    Vistiblity.Add("true");
                }
            }
            List<VMKPIResult> KPIs = new List<VMKPIResult>();
            KPICircleColors colors = new KPICircleColors();
            KPIIconColors iconscolor = new KPIIconColors();
            List<string> color = new List<string>();
            List<string> iconcolor = new List<string>();
            Common Com = new Common();
            foreach (var items in colors)
            {
                string str = Convert.ToString(items.KPIColor);
                color.Add(str);
            }
            foreach (var items in iconscolor)
            {
                string str = Convert.ToString(items.KPIColor);
                iconcolor.Add(str);
            }
            int j = 0;
            var TotReports = dbContext.Reports.ToList();
            var AllUserReports = dbContext.UserReports.ToList();
            foreach (var items in Reports)
            {
                Reports report = TotReports.Where(m => m.ID == items.ReportID).FirstOrDefault();
                Query = report.Query;
                UserReports ureport = AllUserReports.Where(m => m.RoleID == items.RoleID).Where(m => m.ReportID == items.ReportID).SingleOrDefault();
                target = ureport.Target;
                string UserIDs = Com.GetSubUsers(Chart.UserID, Chart.OrgID, sDatabase, sOrgName);
                if (Chart.Query != null && Query.Length > 0)
                {
                    Query = ServiceUtil.ReplaceQueryContent(Query, UserIDs, Chart.UserID, Chart.OrgID, 0, 0);
                    if (Chart.ClassFilter != 0 || Chart.DateFilter != 0)
                    {
                        Query = ServiceUtil.ModifyQuery(Query, Chart.OrgID, UserIDs, Chart.ClassFilter, Chart.DateFilter);
                    }
                    var Location = dbCore.XIAppUsers.Find(Chart.UserID);
                    string BOName = dbContext.BOs.Where(m => m.BOID == report.BOID).Select(m => m.Name).FirstOrDefault();
                    if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "";
                        var Locs = Location.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var Loc in Locs)
                        {
                            LocCondition = LocCondition + "OrgHeirarchyID='ORG" + Chart.OrgID + "_" + Loc + "' or ";
                        }
                        LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                        LocCondition = "(" + LocCondition + ")";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BOName != EnumLeadTables.Reports.ToString() && BOName == EnumLeadTables.Leads.ToString())
                    {
                        Query = ServiceUtil.AddSearchParameters(Query, "FKiOrgID=" + Chart.OrgID);
                        var LocCondition = "OrgHeirarchyID Like 'ORG" + Chart.OrgID + "_%'";
                        Query = ServiceUtil.AddSearchParameters(Query, LocCondition);
                    }
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    cmd.CommandText = Query;
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable data = new DataTable();
                    data.Load(reader);
                    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                    VMKPIResult kpi = new VMKPIResult();
                    if (TotalResult.Count() == 0)
                    {
                        com = 0;
                    }
                    else
                    {
                        com = Convert.ToInt32(TotalResult[0][0]);
                    }
                    Con.Close();
                    double percentage = (double)com / target;
                    int completed = (int)Math.Round(percentage * 100, 0);
                    if (target == 0)
                    {
                        completed = 0;
                    }
                    kpi.Name = report.Name;
                    kpi.ShowAs = report.Title;
                    if (Chart.ReportID > 0)
                    {
                        kpi.Visibility = "true";
                    }
                    else
                    {
                        //kpi.Visibility = "true";
                    }
                    kpi.ReportID = report.ID;
                    kpi.InnerReportID = report.InnerReportID;
                    kpi.KPIPercent = completed;
                    kpi.KPIValue = completed.ToString();// + "%";
                    kpi.KPICircleColor = color[j];
                    kpi.KPIIconColor = iconcolor[j];
                    kpi.KPIIcon = ureport.Icon;
                    KPIs.Add(kpi);
                    j++;
                }
            }
            return KPIs;
        }
        public List<XiLinkNVs> GetTabsDetails(int XiLinkID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<XiLinkNVs> Tabss = new List<XiLinkNVs>();
            var reports = dbContext.XiLinkNVs.Where(m => m.XiLinkID == XiLinkID && m.Value != "Tabs").ToList();
            Tabss = (from r in reports
                     join d in dbContext.XiLinks on Convert.ToInt32(r.Value) equals d.XiLinkID
                     select new XiLinkNVs
                     {
                         Name = d.Name,
                         XiLinkID = Convert.ToInt32(r.Value),
                         Value = r.Value
                     }).ToList();
            return Tabss;
        }

        #endregion Graphs

        #region UpdatedMethods

        public cXISemantics GetXISemanticByID(int iXISemanticID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXISemantics oXISemantic = new cXISemantics();
            oXISemantic = dbContext.XISemantics.Find(iXISemanticID);

            return oXISemantic;
        }

        #endregion UpdatedMethods

        #region UploadFiles

        public VMCustomResponse SaveFiles(int ID, int BOFieldID, int OrgID, List<HttpPostedFileBase> UploadImage, int iUserID, string sOrgName, string sDatabase, string sInstanceID)
        {
            var sNotUploaded = new List<string>();
            int iStatus = 0;
            var XiDocID = 0;
            int iSavedToDoc = 0;
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            BOFields BO = new BOFields();
            string sQuery = "";
            string sValues = string.Empty;
            string sLabelValue = "";
            if (ID > 0 || ID == 0)
            {
                BOFields BODetails = dbContext.BOFields.Find(BOFieldID);
                int FileTypeID = BODetails.FKiFileTypeID;
                int BOID = BODetails.BOID;
                string sFieldName = BODetails.Name;
                string sBOName = dbContext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                string sTableName = dbContext.BOs.Where(m => m.Name == sBOName).Select(m => m.TableName).FirstOrDefault();
                XIFileTypes FileDetails = dbContext.XIFileTypes.Find(FileTypeID);
                string FileType = FileDetails.Type;
                var sDocIDList = new List<string>();
                if (FileType == "10")
                {
                    int iDeletDocIDifNull = 0;
                    foreach (var items in UploadImage)
                    {
                        int iDocID = 0;
                        try
                        {
                            //First save the empty file name and get the docID
                            string sDFileName = "";
                            XIDocs Docs = new XIDocs();
                            Docs.FileName = sDFileName;
                            Docs.FKiDocType = 0;
                            Docs.dCreatedTime = DateTime.Now;
                            Docs.dUpdatedTime = DateTime.Now;
                            Docs.FKiUserID = iUserID;
                            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                            var iOrgID = UserDetais.FKiOrgID;
                            sOrgDB = UserDetais.sUserDatabase;
                            if (iOrgID > 0)
                            {
                                //var sDatabases = sOrgDB;
                                dbContext = new ModelDbContext(sDatabase);
                                dbContext.XIDocs.Add(Docs);
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                dbContext.XIDocs.Add(Docs);
                                dbContext.SaveChanges();
                            }
                            dbContext = new ModelDbContext(sDatabase);
                            //get ID
                            XiDocID = Docs.ID;
                            iDocID = Docs.ID;
                            iDeletDocIDifNull = Docs.ID;
                            sLabelValue = XiDocID.ToString();
                            sQuery = sQuery + XiDocID + ',';
                            //get the details of filename form uploaded file
                            var sImageName = items.FileName;
                            string[] sFormat = sImageName.Split('.');
                            string sImageFormat = sFormat[1];
                            //create a new filename
                            string sNewImageName = "";
                            if (iDocID > 0)
                            {
                                sNewImageName = "images_" + OrgID + "_" + iUserID + "_" + iDocID + "_Org";
                            }
                            // string sFilePath = "";
                            // string sSubDirectory = "";
                            string sNewPathForSubDir = "";
                            int iDocTypeID = 0;
                            //string physicalPath = "";
                            //string sPath = "";
                            string sNewPath = "";
                            List<XIDocTypes> DocTypes = dbContext.XIDocTypes.Where(m => m.Type == sImageFormat).ToList();
                            int iFileTypeID = Convert.ToInt32(FileDetails.FileType);
                            string sFileTypeCheck = dbContext.XIDocTypes.Where(m => m.ID == iFileTypeID).Select(m => m.Type).FirstOrDefault();
                            //Check if the file format matches the doctype details
                            if (sFileTypeCheck.ToLower() != sImageFormat.ToLower())
                            {
                                //do nothing as file format dosnot match
                                sNotUploaded.Add(items.FileName);
                            }
                            else
                            {
                                List<string> sNewPathDetails = CheckAndCreateDirectory(DocTypes, sDatabase);
                                for (var i = 0; i < sNewPathDetails.Count(); i++)
                                {
                                    iDocTypeID = Convert.ToInt32(sNewPathDetails[0]);
                                    sNewPathForSubDir = sNewPathDetails[1];
                                    sNewPath = sNewPathDetails[2];
                                }

                                items.SaveAs(sNewPath + "\\" + sNewImageName + "." + sImageFormat);
                                //Aspect ratio
                                //Get max and min height of image from xi filesetttings
                                var iMaxWidth = Convert.ToInt32(FileDetails.MaxWidth);
                                var iMaxHeight = Convert.ToInt32(FileDetails.MaxHeight);
                                using (var image = Image.FromFile(sNewPath + "\\" + sNewImageName + "." + sImageFormat))
                                using (var newImage = ScaleImage(image, iMaxWidth, iMaxHeight))
                                {
                                    string sImgNme = sNewImageName.Replace("_Org", "");
                                    newImage.Save(sNewPath + "\\" + sImgNme + "." + sImageFormat);
                                }
                                try
                                {
                                    //check DocID and update the details
                                    if (iDocID > 0)
                                    {
                                        UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                                        iOrgID = UserDetais.FKiOrgID;
                                        sOrgDB = UserDetais.sUserDatabase;
                                        XIDocs Doc = new XIDocs();
                                        if (iOrgID > 0)
                                        {
                                            //var sDatabases = sOrgDB;
                                            dbContext = new ModelDbContext(sDatabase);
                                            Doc = dbContext.XIDocs.Find(iDocID);
                                        }
                                        else
                                        {
                                            Doc = dbContext.XIDocs.Find(iDocID);
                                        }
                                        //XIDocs Doc = dbContext.XIDocs.Find(iDocID);
                                        Doc.FileName = sNewImageName.Replace("_Org", "") + "." + sImageFormat;
                                        Doc.FKiDocType = iDocTypeID;
                                        if (sNewPathForSubDir != "")
                                        {
                                            Doc.SubDirectoryPath = sNewPathForSubDir;
                                        }
                                        else
                                        {
                                            Doc.SubDirectoryPath = null;
                                        }
                                        dbContext.SaveChanges();
                                        iSavedToDoc = 1;
                                    }

                                }
                                catch (Exception ex)
                                {

                                }
                                //if (FileType == "10")
                                //{
                                if (iSavedToDoc == 1)
                                {
                                    if (FileDetails.Thumbnails == "10")
                                    {
                                        int iThHeight = Convert.ToInt32(FileDetails.ThumbHeight);
                                        int iThWidth = Convert.ToInt32(FileDetails.ThumbWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + OrgID + "_" + iUserID + "_" + iDocID + "_Org." + sImageFormat);
                                        using (var thumbImage = ThumbImage(image, iThWidth, iThHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            thumbImage.Save(sNewPath + "\\" + sImgNme + "_thumb." + sImageFormat);
                                        }

                                    }

                                    if (FileDetails.Preview == "10")
                                    {
                                        int iPrevHeight = Convert.ToInt32(FileDetails.PreviewHeight);
                                        int iPrevWidth = Convert.ToInt32(FileDetails.PreviewWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + OrgID + "_" + iUserID + "_" + iDocID + "." + sImageFormat);
                                        // Image prev = image.GetThumbnailImage(iPrevHeight, iPrevWidth, () => false, IntPtr.Zero);
                                        //thumb.Save(Path.ChangeExtension(sPath + "\\" + "images_" + OrgID + "_" + ID + "." + sImageFormat, "thumb"));
                                        // prev.Save(sPath + "\\" + sNewImageName + "_prev." + sImageFormat);
                                        using (var newImage = ScaleImage(image, iPrevWidth, iPrevHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            newImage.Save(sNewPath + "\\" + sImgNme + "_prev." + sImageFormat);
                                        }
                                    }

                                    if (FileDetails.Drilldown == "10")
                                    {
                                        int iDrillHeight = Convert.ToInt32(FileDetails.DrillHeight);
                                        int iDrillWidth = Convert.ToInt32(FileDetails.DrillWidth);
                                        Image image = Image.FromFile(sNewPath + "\\" + "images_" + OrgID + "_" + iUserID + "_" + iDocID + "." + sImageFormat);
                                        using (var newImage = ScaleImage(image, iDrillWidth, iDrillHeight))
                                        {
                                            string sImgNme = sNewImageName.Replace("_Org", "");
                                            newImage.Save(sNewPath + "\\" + sImgNme + "_drill." + sImageFormat);
                                        }
                                    }
                                }
                            }
                            //Add Doc ID where the image name is saved to a list.
                            sDocIDList.Add(iDocID.ToString());
                        }//try

                        catch (Exception ex)
                        {
                            if (iDocID > 0)
                            {
                                XIDocs Doc = dbContext.XIDocs.Find(iDocID);
                                dbContext.XIDocs.Remove(Doc);
                                dbContext.SaveChanges();
                            }
                        }
                    }//end upload image for
                    //save the Doc ID where the file name is stored to perticular table
                    //check the not uploaded files
                    if (sNotUploaded.Count == 0)
                    {

                        if (ID != 0)
                        {
                            //check if the table with field has a image ID.
                            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAdbContext"].ConnectionString))
                            {
                                Con.Open();
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = Con;
                                if (sTableName != "Reports")
                                {
                                    Con.ChangeDatabase(sOrgDB);
                                }
                                cmd.CommandText = "SELECT " + sFieldName + " FROM " + sTableName + " WHERE ID=" + ID;
                                SqlDataReader reader = cmd.ExecuteReader();
                                string sDocID = "";
                                while (reader.Read())
                                {
                                    sDocID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                                }
                                Con.Close();

                                string sNewDocID = "";
                                if (sDocID != null)
                                {
                                    string sDocIDs = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                    sNewDocID = sDocID + "," + sDocIDs;
                                }
                                else
                                {
                                    sNewDocID = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                }
                                Con.Open();
                                SqlCommand cmd1 = new SqlCommand();
                                cmd1.Connection = Con;
                                if (sTableName != "Reports")
                                {
                                    Con.ChangeDatabase(sOrgDB);
                                }
                                cmd1.CommandText = "UPDATE " + sTableName + " SET " + sFieldName + "='" + sNewDocID + "' WHERE ID=" + ID;
                                cmd1.ExecuteNonQuery();
                                Con.Close();
                            }
                            iStatus = 1;
                        }
                    }
                    else
                    {
                        iStatus = 0;
                        if (iDeletDocIDifNull != 0)
                        {
                            XIDocs Doc = dbContext.XIDocs.Find(iDeletDocIDifNull);
                            dbContext.XIDocs.Remove(Doc);
                            dbContext.SaveChanges();
                        }
                    }
                }
                else if (FileType == "20")
                {
                    int iDeleteDocIfNull = 0;
                    foreach (var items in UploadImage)
                    {
                        int iDocID = 0;
                        try
                        {
                            //First save the empty file name and get the docID
                            string sDFileName = "";
                            XIDocs Docs = new XIDocs();
                            Docs.FileName = sDFileName;
                            Docs.FKiDocType = 0;
                            Docs.FKiUserID = iUserID;
                            Docs.dCreatedTime = DateTime.Now;
                            Docs.dUpdatedTime = DateTime.Now;
                            dbContext = new ModelDbContext(sDatabase);
                            dbContext.XIDocs.Add(Docs);
                            dbContext.SaveChanges();
                            iDocID = Docs.ID;
                            sQuery = sQuery + iDocID + ',';
                            iDeleteDocIfNull = Docs.ID;
                            //get the details of filename form uploaded file
                            var sFileName = items.FileName;
                            string[] sFormat = sFileName.Split('.');
                            string sImageFormat = sFormat[1];
                            //create a new filename
                            string sNewFileName = "";
                            if (iDocID > 0)
                            {
                                sNewFileName = "file_" + OrgID + "_" + iUserID + "_" + iDocID;
                            }
                            //string sFilePath = "";
                            //string sSubDirectory = "";
                            string sNewPathForSubDir = "";
                            int iDocTypeID = 0;
                            //string physicalPath = "";
                            //string sPath = "";
                            string sNewPath = "";
                            List<XIDocTypes> DocTypes = dbContext.XIDocTypes.Where(m => m.Type == sImageFormat).ToList();
                            int iFileTypeID = Convert.ToInt32(FileDetails.FileType);
                            string sFileTypeCheck = dbContext.XIDocTypes.Where(m => m.ID == iFileTypeID).Select(m => m.Type).FirstOrDefault();
                            //Check if the file format matches the doctype details
                            if (sFileTypeCheck.ToLower() != sImageFormat.ToLower())
                            {
                                //do nothing as file format dosnot match
                                sNotUploaded.Add(items.FileName);
                            }
                            else
                            {
                                //call method to check and create directory
                                List<string> sNewPathDetails = CheckAndCreateDirectory(DocTypes, sDatabase);
                                for (var i = 0; i < sNewPathDetails.Count(); i++)
                                {
                                    iDocTypeID = Convert.ToInt32(sNewPathDetails[0]);
                                    sNewPathForSubDir = sNewPathDetails[1];
                                    sNewPath = sNewPathDetails[2];

                                }
                                items.SaveAs(sNewPath + "\\" + sNewFileName + "." + sImageFormat);


                                try
                                {
                                    //check DocID and update the details
                                    if (iDocID > 0)
                                    {
                                        dbContext = new ModelDbContext(sDatabase);
                                        XIDocs Doc = dbContext.XIDocs.Find(iDocID);
                                        Doc.FileName = sNewFileName + "." + sImageFormat;
                                        Doc.FKiDocType = iDocTypeID;
                                        if (sNewPathForSubDir != "")
                                        {
                                            Doc.SubDirectoryPath = sNewPathForSubDir;
                                            Doc.sFullPath = sNewPathForSubDir + "/" + Doc.FileName;
                                        }
                                        else
                                        {
                                            Doc.SubDirectoryPath = null;
                                        }
                                        dbContext.SaveChanges();
                                        iSavedToDoc = 1;
                                    }

                                }
                                catch (Exception ex)
                                {
                                    //do nothin
                                }
                                if (iSavedToDoc == 1)
                                {
                                    //for now no preview
                                }
                            }
                            //Add Doc ID where the image name is saved to a list.
                            sDocIDList.Add(iDocID.ToString());
                        }//try

                        catch (Exception ex)
                        {
                            if (iDocID > 0)
                            {
                                XIDocs Doc = dbContext.XIDocs.Find(iDocID);
                                dbContext.XIDocs.Remove(Doc);
                                dbContext.SaveChanges();
                            }
                        }
                    }//end upload image for

                    //save the Doc ID where the file name is stored to perticular table
                    //check the not uploaded files
                    if (sNotUploaded.Count == 0)
                    {

                        if (ID != 0)
                        {
                            //check if the table with field has a image ID.
                            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAdbContext"].ConnectionString))
                            {
                                Con.Open();
                                SqlCommand cmd = new SqlCommand();
                                cmd.Connection = Con;
                                if (sTableName != "Reports")
                                {
                                    Con.ChangeDatabase(sOrgDB);
                                }
                                cmd.CommandText = "SELECT " + sFieldName + " FROM " + sTableName + " WHERE ID=" + ID;
                                SqlDataReader reader = cmd.ExecuteReader();
                                string sDocID = "";
                                while (reader.Read())
                                {
                                    sDocID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                                }
                                Con.Close();

                                string sNewDocID = "";
                                if (sDocID != null)
                                {
                                    string sDocIDs = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                    sNewDocID = sDocID + "," + sDocIDs;
                                }
                                else
                                {
                                    sNewDocID = string.Join(",", sDocIDList.Select(x => x.ToString()).ToArray());
                                }

                                Con.Open();
                                SqlCommand cmd1 = new SqlCommand();
                                cmd1.Connection = Con;
                                if (sTableName != "Reports")
                                {
                                    Con.ChangeDatabase(sOrgDB);
                                }
                                cmd1.CommandText = "UPDATE " + sTableName + " SET " + sFieldName + "='" + sNewDocID + "' WHERE ID=" + ID;
                                cmd1.ExecuteNonQuery();
                                Con.Close();
                            }
                            iStatus = 1;
                        }
                    }
                    else
                    {
                        iStatus = 0;
                        if (iDeleteDocIfNull != 0)
                        {
                            XIDocs Doc = dbContext.XIDocs.Find(iDeleteDocIfNull);
                            dbContext.XIDocs.Remove(Doc);
                            dbContext.SaveChanges();
                        }
                    }
                }
                else
                {
                    //save the image as Blob
                }
            }
            if (!string.IsNullOrEmpty(sQuery))
            {
                sQuery = sQuery.Substring(0, sQuery.Length - 1);
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = sQuery, Status = true };
        }
        //Calculate the aspect ratio and recreate the image
        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
        }
        //thumbnail
        public static Image ThumbImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            Image thumb = image.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
            return thumb;
        }

        //Create dir

        public List<string> CheckAndCreateDirectory(List<XIDocTypes> DocTypes, string sDatabase)
        {
            string physicalPath = "";
            string sPath = "";
            string sFilePath = "";
            int iDocTypeID = 0;
            string sSubDirectory = "";
            string sNewPath = "";
            string sNewPathForSubDir = "";
            var lPathDetails = new List<string>();
            foreach (var DTypes in DocTypes)
            {
                sFilePath = DTypes.Path;
                iDocTypeID = DTypes.ID;
                lPathDetails.Add(iDocTypeID.ToString());
                sSubDirectory = DTypes.SubDirectory.ToLower();
                if (sSubDirectory == "year/month/day")
                {
                    //check if sub directory has "/"
                    if ((sSubDirectory.Contains("/")))
                    {
                        sSubDirectory = sSubDirectory.Replace(@"\", "/");
                        string sSubDirCsV = sSubDirectory.Replace("/", ",").TrimStart();
                        List<string> sSubDirList = sSubDirCsV.Split(',').ToList();
                        List<string> sNewSubDirPath = new List<string>(); ;
                        foreach (var DirNames in sSubDirList)
                        {
                            string sVal = "";
                            DateTime DateTme = DateTime.Now;
                            if (DirNames.ToLower() == "year")
                            {
                                sVal = DateTme.Year.ToString();
                            }
                            else if (DirNames.ToLower() == "month")
                            {
                                sVal = DateTme.Month.ToString();
                            }
                            else if (DirNames.ToLower() == "day")
                            {
                                sVal = DateTme.Day.ToString();
                            }
                            else
                            {
                                sVal = DirNames;
                            }
                            sNewSubDirPath.Add(sVal);
                        }//for
                        physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                        sPath = physicalPath.Substring(0, physicalPath.Length) + "\\" + sFilePath;
                        if (sPath.Contains("~/"))
                        {
                            sPath = sPath.Replace("~/", "");
                        }

                        if (sPath.Contains('/'))
                        {
                            sPath = sPath.Replace("/", "\\");
                        }
                        ////Save the new created sub dir path to "XI Doc Settings"
                        string sNewSubDirPathCSV = String.Join(",", sNewSubDirPath.Select(x => x.ToString()).ToArray());
                        sNewPathForSubDir = sNewSubDirPathCSV.Replace(",", "/");
                        //Add subdirpath to list
                        lPathDetails.Add(sNewPathForSubDir);
                        foreach (var sNwSubdir in sNewSubDirPath)
                        {
                            //sNewPath = "";
                            if (sNewPath == "" || sNewPath == null)
                            {
                                sNewPath = sPath + "\\" + sNwSubdir;
                            }
                            else
                            {
                                sNewPath = sNewPath + "\\" + sNwSubdir;
                            }

                            if (Directory.Exists(sNewPath))
                            {

                            }
                            else
                            {
                                System.IO.Directory.CreateDirectory(sNewPath);
                            }
                        }
                    }//sub dir
                    lPathDetails.Add(sNewPath);
                }//sub=10;
                else
                {

                    physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    sPath = physicalPath.Substring(0, physicalPath.Length) + sFilePath;
                    if (sPath.Contains("~/"))
                    {
                        sPath = sPath.Replace("~/", "");
                    }

                    if (sPath.Contains('/'))
                    {
                        sPath = sPath.Replace("/", "\\");
                    }
                    sNewPath = sPath;
                    if (Directory.Exists(sNewPath))
                    {

                    }
                    else
                    {
                        var createDir = System.IO.Directory.CreateDirectory(sNewPath);
                    }
                    lPathDetails.Add(sNewPath);
                }
            }
            return lPathDetails;
        }
        //Delete image
        public int DeleteAttrImage(string sDatabase, int ImgID, int BOFieldID, int LeadID)
        {
            int iStatus = 0;
            int iSuccess = 0;
            //delete ID from the XIDocs
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                XIDocs XIDcx = dbContext.XIDocs.Find(ImgID);
                //Delete from local floder
                XIDocTypes XIDocTyps = dbContext.XIDocTypes.Find(XIDcx.FKiDocType);
                string sNewPath = "";
                if (XIDcx.SubDirectoryPath == "")
                {
                    sNewPath = XIDocTyps.Path;
                }
                else
                {
                    sNewPath = XIDocTyps.Path + "/" + XIDcx.SubDirectoryPath;
                }


                string[] sDiffPattrn = new string[] { "", "_Org", "_prev", "_thumb", "_drill" };
                foreach (var patrn in sDiffPattrn)
                {
                    //get the file name and check if thumbnail or preview exists and if yes delete.
                    string sFileName = XIDcx.FileName;
                    string[] sSplitName = sFileName.Split('.');
                    string sFlName = sSplitName[0];
                    string sFrmt = sSplitName[1];
                    string sNewFileName = sFlName + patrn + "." + sFrmt;
                    string sComplPath = sNewPath + "/" + sNewFileName;
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    string sPath = physicalPath.Substring(0, physicalPath.Length) + sComplPath;
                    if (sPath.Contains("~/"))
                    {
                        sPath = sPath.Replace("~/", "");
                    }

                    if (sPath.Contains('/'))
                    {
                        sPath = sPath.Replace("/", "\\");
                    }
                    bool isFileExists = File.Exists(sPath);
                    if (isFileExists == true)
                    {
                        lock (_lock)
                        {
                            System.IO.File.Delete(sPath);
                        }

                    }
                }
                dbContext.XIDocs.Remove(XIDcx);
                dbContext.SaveChanges();
                iStatus = 1;
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }

            if (iStatus == 1)
            {
                BOFields BODetails = dbContext.BOFields.Find(BOFieldID);
                int FileTypeID = BODetails.FKiFileTypeID;
                int BOID = BODetails.BOID;
                string sFieldName = BODetails.Name;
                string sBOName = dbContext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAdbContext"].ConnectionString))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    if (sBOName != "Reports")
                    {
                        Con.ChangeDatabase(sDatabase);
                    }
                    cmd.CommandText = "SELECT " + sFieldName + " FROM " + sBOName + " WHERE ID=" + LeadID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    string sDocID = "";
                    while (reader.Read())
                    {
                        sDocID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                    }
                    Con.Close();

                    if (sDocID != "")
                    {
                        if (sDocID.Contains(","))
                        {

                            List<string> sFileIDs = sDocID.Split(',').ToList();
                            var sMatchs = sFileIDs.FirstOrDefault(stringToCheck => stringToCheck.Contains(ImgID.ToString()));
                            if (sMatchs != null)
                            {
                                sFileIDs.Remove(ImgID.ToString());
                                string sNewDocIDs = string.Join(",", sFileIDs.Select(x => x.ToString()).ToArray());
                                Con.Open();
                                SqlCommand cmd1 = new SqlCommand();
                                cmd1.Connection = Con;
                                if (sBOName != "Reports")
                                {
                                    Con.ChangeDatabase(sDatabase);
                                }
                                cmd1.CommandText = "UPDATE " + sBOName + " SET " + sFieldName + "='" + sNewDocIDs + "' WHERE ID=" + LeadID;
                                cmd1.ExecuteNonQuery();
                                Con.Close();
                            }
                        }
                        else
                        {
                            Con.Open();
                            SqlCommand cmd1 = new SqlCommand();
                            cmd1.Connection = Con;
                            if (sBOName != "Reports")
                            {
                                Con.ChangeDatabase(sDatabase);
                            }
                            cmd1.CommandText = "UPDATE " + sBOName + " SET " + sFieldName + "=NULL WHERE ID=" + LeadID;
                            cmd1.ExecuteNonQuery();
                            Con.Close();
                        }
                    }
                }
                iSuccess = 1;
            }
            return iSuccess;
        }
        private static object _lock = new object();
        #endregion UploadFiles

        #region QuestionSet  

        public cQSInstance GetQSInstanceByID(int iQSIID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cQSInstance oQSIns = new cQSInstance();
            oQSIns = dbContext.QSInstance.Find(iQSIID);
            return oQSIns;
        }

        public cQSInstance GetQuestionSetInstance(int iQSID, string sGUID, string sMode, int iBODID, int iInstanceID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        {
            //oCache.ClearCache();
            cQSInstance oQSInstance = new cQSInstance();
            oQSInstance = GetQuestionSetInstanceByID(iQSID, sMode, iBODID, iInstanceID, iUserID, sOrgName, sDatabase, sCurrentUserGUID);
            oQSInstance.QSDefinition = (cQSDefinition)oCache.GetObjectFromCache("QuestionSet", sGUID, iUserID, sOrgName, sDatabase, iQSID, sCurrentUserGUID);
            //if (oQSInstance.QSDefinition.iLayoutID > 0)
            //{
            //    oQSInstance.QSDefinition.Layout = (cLayouts)oCache.GetObjectFromCache("Layout", sGUID, iUserID, sOrgName, sDatabase, oQSInstance.QSDefinition.iLayoutID, sCurrentUserGUID);
            //}
            return oQSInstance;
        }

        public cQSDefinition GetQuestionSetDefinitionByID(int iQSID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        {
            var MainDB = sDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
              "left join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
              "left join XIFieldDefinition_T XIFD on QSSDef.ID = XIFD.FKiXIStepDefinitionID " +
              "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
              "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
              "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
              "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
              "WHERE QSDef.ID = @id;";
            var param = new
            {
                id = iQSID,
                CurrentGuestUser = sCurrentUserGUID
            };
            var lookup = new Dictionary<int, cQSDefinition>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup2 = new Dictionary<int, cQSStepDefiniton>();
                var lookup3 = new Dictionary<int, cFieldDefinition>();
                var lookup4 = new Dictionary<int, cFieldOrigin>();
                var lookup5 = new Dictionary<int, cQSNavigations>();
                var lookup6 = new Dictionary<int, cXIFieldOptionList>();
                Conn.Query<cQSDefinition, cQSStepDefiniton, cFieldDefinition, cFieldOrigin, cXIDataTypes, cQSNavigations, cXIFieldOptionList, cQSDefinition>(sQSDefinitionQry,
                    (QS, Step, FieldDefinition, FieldOrigin, DataType, Navigations, OptionList) =>
                    {
                        cQSDefinition oQSDefinition;
                        if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        {
                            lookup.Add(QS.ID, oQSDefinition = QS);
                        }
                        cQSStepDefiniton oStepDefinition;
                        if (Step != null)
                        {
                            if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                            {
                                lookup2.Add(Step.ID, oStepDefinition = Step);
                                if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                                {
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                                else
                                {
                                    oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                            }
                            cFieldDefinition oFieldDefintion;
                            if (FieldDefinition != null)
                            {
                                if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                                {
                                    lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                    if (oStepDefinition.FieldDefinitions != null && oStepDefinition.FieldDefinitions.Count() > 0)
                                    {
                                        oStepDefinition.FieldDefinitions.Add(oFieldDefintion);
                                    }
                                    else
                                    {
                                        oStepDefinition.FieldDefinitions = new List<cFieldDefinition>();
                                        oStepDefinition.FieldDefinitions.Add(oFieldDefintion);
                                    }

                                }
                                cFieldOrigin oXIFieldOrigin;
                                if (FieldOrigin != null)
                                {
                                    if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                    {

                                        if (FieldOrigin.iMasterDataID > 0)
                                        {
                                            FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0)
                                        {
                                            var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                            var oBO = dbContext.BOs.Find(o1Click.BOID);
                                            FieldOrigin.bIsLargeBO = true;
                                            FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                            SqlConnection Con = new SqlConnection(sBODataSource);
                                            Con.Open();
                                            SqlCommand cmd = new SqlCommand();
                                            cmd.Connection = Con;
                                            cmd.CommandText = o1Click.Query;
                                            SqlDataReader reader = cmd.ExecuteReader();
                                            while (reader.Read())
                                            {
                                                FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                                {
                                                    sOptionValue = reader.GetInt32(0).ToString(),
                                                    sOptionName = reader.GetString(1)
                                                });
                                            }
                                            Con.Close();
                                        }
                                        lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                        oXIFieldOrigin = FieldOrigin;
                                    }

                                    cXIFieldOptionList oOptions;
                                    if (OptionList != null)
                                    {
                                        oOptions = OptionList;
                                        if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                        {
                                            lookup6.Add(OptionList.ID, oOptions = OptionList);
                                            if (oXIFieldOrigin.ddlFieldOptionList != null && oXIFieldOrigin.ddlFieldOptionList.Count() > 0)
                                            {
                                                oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                            }
                                            else
                                            {
                                                oXIFieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                                oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                            }
                                        }
                                    }
                                    oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                }

                                FieldOrigin.DataTypes = DataType;

                                //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                                //{
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                                //else
                                //{
                                //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                            }
                            if (Navigations != null)
                            {
                                cQSNavigations nNavs;
                                if (!lookup5.TryGetValue(Navigations.ID, out nNavs))
                                {
                                    lookup5.Add(Navigations.ID, nNavs = Navigations);
                                    if (oStepDefinition.QSNavigations != null && oStepDefinition.QSNavigations.Count() > 0)
                                    {
                                        oStepDefinition.QSNavigations.Add(nNavs);
                                    }
                                    else
                                    {
                                        oStepDefinition.QSNavigations = new List<cQSNavigations>();
                                        oStepDefinition.QSNavigations.Add(nNavs);
                                    }
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }

            var oQSDef = lookup.Values.FirstOrDefault();
            var Sections = GetStepSectionDefinitions(oQSDef.ID, iUserID, sOrgName, MainDB);
            if (Sections != null)
            {
                foreach (var items in Sections.QSSteps)
                {
                    oQSDef.QSSteps.Where(m => m.ID == items.ID).FirstOrDefault().Sections = items.Sections;
                    //oQSInstance.nStepInstances.Where(m => m.FKiQSStepDefinitionID == items.ID).FirstOrDefault().nSections = items.Sections;
                }
            }
            oQSDef.Visualisations = GetQSVisualisations(oQSDef.iVisualisationID, iUserID, sOrgName, MainDB);
            oQSDef.QSFieldVisualisations = GetQSFiledVisualisations(iQSID, iUserID, sOrgName, MainDB);
            if (oQSDef.QSSteps != null)
            {
                foreach (var items in oQSDef.QSSteps)
                {
                    if (items.iLayoutID > 0)
                    {
                        items.Layout = Common.GetLayoutDetails(items.iLayoutID, 0, 0, 0, null, iUserID, sOrgName, sDatabase);
                    }
                }
            }
            return oQSDef;
        }

        public cQSStepDefiniton GetStepDefinition(int iStepID, string sGUID, int iUserID, string sOrgName, string sDatabase)
        {
            cQSStepDefiniton oStep = new cQSStepDefiniton();
            oStep = (cQSStepDefiniton)oCache.GetObjectFromCache("Step", sGUID, iUserID, sOrgName, sDatabase, iStepID, null);
            //if(oStep.iLayoutID > 0)
            //{
            //    oStep.Layout = (VMPopupLayout)oCache.GetObjectFromCache("Layout", sGUID, iUserID, sOrgName, sDatabase, oStep.iLayoutID, null);
            //}
            return oStep;
        }

        //public cQSStepDefiniton GetStepDefinitionByID(int iStepID)
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    cQSStepDefiniton oStep = new cQSStepDefiniton();
        //    oStep = dbContext.QSStepDefiniton.Find(iStepID);
        //    return oStep;
        //}

        public cQSStepDefiniton GetStepDefinitionByID(int iStepID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        {
            var MainDB = sDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
                "left join XIFieldDefinition_T XIFD on QSSDef.ID = XIFD.FKiXIStepDefinitionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIQSNavigation_T NAV on QSSDef.ID = NAV.FKiStepDefinitionID " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSSDef.ID = @id;";
            var param = new
            {
                id = iStepID,
                CurrentGuestUser = sCurrentUserGUID
            };
            var lookup2 = new Dictionary<int, cQSStepDefiniton>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup = new Dictionary<int, cQSDefinition>();
                var lookup3 = new Dictionary<int, cFieldDefinition>();
                var lookup4 = new Dictionary<int, cFieldOrigin>();
                var lookup5 = new Dictionary<int, cQSNavigations>();
                var lookup6 = new Dictionary<int, cXIFieldOptionList>();
                Conn.Query<cQSStepDefiniton, cFieldDefinition, cFieldOrigin, cXIDataTypes, cQSNavigations, cXIFieldOptionList, cQSStepDefiniton>(sQSDefinitionQry,
                    (Step, FieldDefinition, FieldOrigin, DataType, Navigations, OptionList) =>
                    {
                        //cQSDefinition oQSDefinition;
                        //if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        //{
                        //    lookup.Add(QS.ID, oQSDefinition = QS);
                        //}
                        cQSStepDefiniton oStepDefinition;
                        //if (Step != null)
                        //{
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                        }
                        //if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        //{
                        //    lookup2.Add(Step.ID, oStepDefinition = Step);
                        //    if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                        //    {
                        //        oQSDefinition.QSSteps.Add(oStepDefinition);
                        //    }
                        //    else
                        //    {
                        //        oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                        //        oQSDefinition.QSSteps.Add(oStepDefinition);
                        //    }
                        //}
                        cFieldDefinition oFieldDefintion;
                        if (FieldDefinition != null)
                        {
                            if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                            {
                                lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                if (oStepDefinition.FieldDefinitions != null && oStepDefinition.FieldDefinitions.Count() > 0)
                                {
                                    oStepDefinition.FieldDefinitions.Add(oFieldDefintion);
                                }
                                else
                                {
                                    oStepDefinition.FieldDefinitions = new List<cFieldDefinition>();
                                    oStepDefinition.FieldDefinitions.Add(oFieldDefintion);
                                }

                            }
                            cFieldOrigin oXIFieldOrigin;
                            if (FieldOrigin != null)
                            {
                                if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                {

                                    if (FieldOrigin.iMasterDataID > 0)
                                    {
                                        FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                    }
                                    else if (FieldOrigin.FK1ClickID > 0)
                                    {
                                        var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                        var oBO = dbContext.BOs.Find(o1Click.BOID);
                                        FieldOrigin.bIsLargeBO = true;
                                        FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                        var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                        SqlConnection Con = new SqlConnection(sBODataSource);
                                        Con.Open();
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.Connection = Con;
                                        cmd.CommandText = o1Click.Query;
                                        SqlDataReader reader = cmd.ExecuteReader();
                                        while (reader.Read())
                                        {
                                            FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                            {
                                                sOptionValue = reader.GetInt32(0).ToString(),
                                                sOptionName = reader.GetString(1)
                                            });
                                        }
                                        Con.Close();
                                    }
                                    lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                    oXIFieldOrigin = FieldOrigin;
                                }

                                cXIFieldOptionList oOptions;
                                if (OptionList != null)
                                {
                                    oOptions = OptionList;
                                    if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                    {
                                        lookup6.Add(OptionList.ID, oOptions = OptionList);
                                        if (oXIFieldOrigin.ddlFieldOptionList != null && oXIFieldOrigin.ddlFieldOptionList.Count() > 0)
                                        {
                                            oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                        }
                                        else
                                        {
                                            oXIFieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                            oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                        }
                                    }
                                }
                                oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                            }

                            FieldOrigin.DataTypes = DataType;

                            //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                            //{
                            //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                            //}
                            //else
                            //{
                            //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                            //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                            //}
                        }
                        if (Navigations != null)
                        {
                            cQSNavigations nNavs;
                            if (!lookup5.TryGetValue(Navigations.ID, out nNavs))
                            {
                                lookup5.Add(Navigations.ID, nNavs = Navigations);
                                if (oStepDefinition.QSNavigations != null && oStepDefinition.QSNavigations.Count() > 0)
                                {
                                    oStepDefinition.QSNavigations.Add(nNavs);
                                }
                                else
                                {
                                    oStepDefinition.QSNavigations = new List<cQSNavigations>();
                                    oStepDefinition.QSNavigations.Add(nNavs);
                                }
                            }
                        }
                        //}
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }

            var oQSStepDef = lookup2.Values.FirstOrDefault();
            var oStepSections = GetStepSectionDefinitionsByStep(oQSStepDef.ID, iUserID, sOrgName, MainDB);
            if (oStepSections != null)
            {
                oQSStepDef.Sections = oStepSections.Sections;
            }
            if (oQSStepDef.iLayoutID > 0)
            {
                oQSStepDef.Layout = Common.GetLayoutDetails(oQSStepDef.iLayoutID, 0, 0, 0, null, iUserID, sOrgName, sDatabase);
            }
            return oQSStepDef;
        }

        public cLayouts GetLayoutByID(int iLayoutID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cLayouts oLayout = new cLayouts();
            oLayout = dbContext.Layouts.Find(iLayoutID);
            return oLayout;
        }

        private XiVisualisations GetQSVisualisations(int iVisualID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var Visualistation = dbContext.XiVisualisations.Find(iVisualID);
            return Visualistation;
        }

        private List<cQSVisualisations> GetQSFiledVisualisations(int iQSID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var FieldVisualistation = dbContext.QSVisualisations.Where(m => m.FKiQSDefinitionID == iQSID).ToList();
            return FieldVisualistation;

        }

        private cQSInstance GetQuestionSetInstanceByID(int iQSID, string sMode, int iBODID, int iInstanceID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        {//QSStep Definition, Field Origin, XIDataTypes Removed
            var MainDB = sDatabase;
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }

            cQSInstance oQSInstance = new cQSInstance();
            oQSInstance.FKiQSDefinitionID = iQSID;

            //Load QS Instance
            string sQSInstanceQry = "select * from XIQSInstance_T QSI " +
              "inner join XIQSStepInstance_T QSSI on QSI.ID = QSSI.FKiQSInstanceID " +
              "left join XIFieldInstance_T FI on QSSI.FKiQSStepDefinitionID = FI.FKiQSStepDefinitionID " +
                "WHERE (QSI.ID = FI.FKiQSInstanceID or FI.FKiQSInstanceID is null) and QSI.FKiQSDefinitionID = @id";

            var param = new
            {
                id = iQSID,
                CurrentGuestUser = sCurrentUserGUID,
                BODID = iBODID,
                BOIID = iInstanceID
            };
            if (!string.IsNullOrEmpty(sCurrentUserGUID))
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.FKiUserCookieID = @CurrentGuestUser";
            }
            if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iInstanceID > 0)
            {
                if (sMode.ToLower() == "Popup".ToLower())
                {
                    sQSInstanceQry = sQSInstanceQry + " and QSI.FKiBODID = @BODID and QSI.iBOIID = @BOIID";
                }
            }
            var lookupQSIns = new Dictionary<int, cQSInstance>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookupStepIns = new Dictionary<int, cQSStepInstance>();
                var lookupFieldIns = new Dictionary<long, cFieldInstance>();
                Conn.Query<cQSInstance, cQSStepInstance, cFieldInstance, cQSInstance>(sQSInstanceQry,
                    (QS, StepIns, FieldInstance) =>
                    {
                        cQSInstance oQSIns = new cQSInstance();
                        if (QS != null)
                        {
                            if (!lookupQSIns.TryGetValue(QS.ID, out oQSIns))
                            {
                                lookupQSIns.Add(QS.ID, oQSIns = QS);
                            }
                            cQSStepInstance oQSSIns;
                            if (!lookupStepIns.TryGetValue(StepIns.ID, out oQSSIns))
                            {
                                lookupStepIns.Add(StepIns.ID, oQSSIns = StepIns);
                                if (oQSIns.nStepInstances != null && oQSIns.nStepInstances.Count() > 0)
                                {
                                    oQSIns.nStepInstances.Add(oQSSIns);
                                }
                                else
                                {
                                    oQSIns.nStepInstances = new List<cQSStepInstance>();
                                    oQSIns.nStepInstances.Add(oQSSIns);
                                }
                            }
                            if (FieldInstance != null)
                            {
                                cFieldInstance oFVInstance;
                                if (!lookupFieldIns.TryGetValue(FieldInstance.ID, out oFVInstance))
                                {
                                    lookupFieldIns.Add(FieldInstance.ID, oFVInstance = FieldInstance);
                                    if (oQSSIns.nFieldInstances != null && oQSSIns.nFieldInstances.Count() > 0)
                                    {
                                        oQSSIns.nFieldInstances.Add(FieldInstance);
                                    }
                                    else
                                    {
                                        oQSSIns.nFieldInstances = new List<cFieldInstance>();
                                        oQSSIns.nFieldInstances.Add(FieldInstance);
                                    }
                                }
                            }
                        }
                        return oQSIns;
                    },
                    param
                    ).AsQueryable();
            }

            var oQSInsFinal = lookupQSIns.Values.FirstOrDefault();
            if (oQSInsFinal != null && oQSInsFinal.nStepInstances != null && oQSInsFinal.nStepInstances.Count() > 0)
            {
                oQSInstance.iCurrentStepID = oQSInsFinal.iCurrentStepID;
                oQSInstance.nStepInstances = oQSInsFinal.nStepInstances;
                var StepIns = GetSectionInstances(iQSID, sMode, iBODID, iInstanceID, iUserID, sOrgName, MainDB, sCurrentUserGUID).nStepInstances;

                foreach (var items in StepIns)
                {
                    oQSInstance.nStepInstances.Where(m => m.FKiQSStepDefinitionID == items.FKiQSStepDefinitionID).FirstOrDefault().nSectionInstances = items.nSectionInstances;
                }

            }
            return oQSInstance;
        }

        private cQSInstance GetSectionInstances(int iQSID, string sMode, int iBODID, int iInstanceID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        {
            var MainDB = sDatabase;
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }

            cQSInstance oQSInstance = new cQSInstance();
            oQSInstance.FKiQSDefinitionID = iQSID;

            //Load QS Instance
            string sQSInstanceQry = "select * from XIQSInstance_T QSI " +
                "inner join XIQSStepInstance_T QSSI on QSI.ID = QSSI.FKiQSInstanceID " +
                "left join XIStepSectionInstance_T SecI on QSSI.ID = SecI.FKiStepInstanceID " +
                "left join XIFieldInstance_T SFI on SecI.FKiStepSectionDefinitionID = SFI.FKiQSSectionDefinitionID " +
                //"left join XIFullAddress_T FA on SecI.ID = FA.FKiSectionInstanceID " +
                "WHERE (QSI.ID = SFI.FKiQSInstanceID or SFI.FKiQSInstanceID is null) and QSI.FKiQSDefinitionID = @id";

            var param = new
            {
                id = iQSID,
                CurrentGuestUser = sCurrentUserGUID,
                BODID = iBODID,
                BOIID = iInstanceID
            };
            if (!string.IsNullOrEmpty(sCurrentUserGUID))
            {
                sQSInstanceQry = sQSInstanceQry + " and QSI.FKiUserCookieID = @CurrentGuestUser";
            }
            if (!string.IsNullOrEmpty(sMode) && iBODID > 0 && iInstanceID > 0)
            {
                if (sMode.ToLower() == "Popup".ToLower())
                {
                    sQSInstanceQry = sQSInstanceQry + " and QSI.FKiBODID = @BODID and QSI.iBOIID = @BOIID";
                }
            }
            var lookupQSIns = new Dictionary<int, cQSInstance>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookupStepIns = new Dictionary<int, cQSStepInstance>();
                var lookupSectionIns = new Dictionary<int, cStepSectionInstance>();
                var lookupFieldIns = new Dictionary<long, cFieldInstance>();
                Conn.Query<cQSInstance, cQSStepInstance, cStepSectionInstance, cFieldInstance, cQSInstance>(sQSInstanceQry,
                    (QS, StepIns, SectionInstance, FieldInstance) =>
                    {
                        cQSInstance oQSIns = new cQSInstance();
                        if (QS != null)
                        {
                            if (!lookupQSIns.TryGetValue(QS.ID, out oQSIns))
                            {
                                lookupQSIns.Add(QS.ID, oQSIns = QS);
                            }
                            cQSStepInstance oQSSIns;
                            if (!lookupStepIns.TryGetValue(StepIns.ID, out oQSSIns))
                            {
                                lookupStepIns.Add(StepIns.ID, oQSSIns = StepIns);
                                if (oQSIns.nStepInstances != null && oQSIns.nStepInstances.Count() > 0)
                                {
                                    oQSIns.nStepInstances.Add(oQSSIns);
                                }
                                else
                                {
                                    oQSIns.nStepInstances = new List<cQSStepInstance>();
                                    oQSIns.nStepInstances.Add(oQSSIns);
                                }
                            }

                            cStepSectionInstance oStepSectionIns;
                            if (SectionInstance != null)
                            {
                                if (!lookupSectionIns.TryGetValue(SectionInstance.ID, out oStepSectionIns))
                                {
                                    lookupSectionIns.Add(SectionInstance.ID, oStepSectionIns = SectionInstance);
                                    if (oQSSIns.nSectionInstances != null && oQSSIns.nSectionInstances.Count() > 0)
                                    {
                                        oQSSIns.nSectionInstances.Add(SectionInstance);
                                    }
                                    else
                                    {
                                        oQSSIns.nSectionInstances = new List<cStepSectionInstance>();
                                        oQSSIns.nSectionInstances.Add(SectionInstance);
                                    }
                                }

                                cFieldInstance oFVInstance;
                                if (FieldInstance != null)
                                {
                                    if (!lookupFieldIns.TryGetValue(FieldInstance.ID, out oFVInstance))
                                    {
                                        lookupFieldIns.Add(FieldInstance.ID, oFVInstance = FieldInstance);
                                        if (oStepSectionIns.nFieldInstances != null && oStepSectionIns.nFieldInstances.Count() > 0)
                                        {
                                            oStepSectionIns.nFieldInstances.Add(FieldInstance);
                                        }
                                        else
                                        {
                                            oStepSectionIns.nFieldInstances = new List<cFieldInstance>();
                                            oStepSectionIns.nFieldInstances.Add(FieldInstance);
                                        }
                                    }
                                }
                                //if (FullAddress != null)
                                //{
                                //    oStepSectionIns.ComponentInstance = FullAddress;
                                //}

                            }
                        }
                        return oQSIns;
                    },
                    param
                    ).AsQueryable();
            }
            var oQSInsFinal = lookupQSIns.Values.FirstOrDefault();

            return oQSInsFinal;
        }

        public cQSDefinition GetStepSectionDefinitions(int iQSID, int iUserID, string sOrgName, string sDatabase)
        {
            var MainDB = sDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSDef " +
                "inner join XIQSStepDefinition_T QSSDef on QSDef.ID = QSSDef.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSDef.ID = Sec.FKiStepDefinitionID " +
                "left join XIFieldDefinition_T XIFD on Sec.ID = XIFD.FKiStepSectionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSDef.ID = @id;";
            var param = new
            {
                id = iQSID
            };
            var lookup = new Dictionary<int, cQSDefinition>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup2 = new Dictionary<int, cQSStepDefiniton>();
                var lookup3 = new Dictionary<int, cFieldDefinition>();
                var lookup4 = new Dictionary<int, cFieldOrigin>();
                var lookup5 = new Dictionary<int, cQSNavigations>();
                var lookup6 = new Dictionary<int, cXIFieldOptionList>();
                var lookup7 = new Dictionary<int, cStepSectionDefinition>();
                Conn.Query<cQSDefinition, cQSStepDefiniton, cStepSectionDefinition, cFieldDefinition, cFieldOrigin, cXIDataTypes, cXIFieldOptionList, cQSDefinition>(sQSDefinitionQry,
                    (QS, Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
                    {
                        cQSDefinition oQSDefinition;
                        if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        {
                            lookup.Add(QS.ID, oQSDefinition = QS);
                        }
                        cQSStepDefiniton oStepDefinition;
                        if (Step != null)
                        {
                            if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                            {
                                lookup2.Add(Step.ID, oStepDefinition = Step);
                                if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                                {
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                                else
                                {
                                    oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                            }
                            cStepSectionDefinition oSection;
                            if (SectionDef != null)
                            {
                                if (!lookup7.TryGetValue(SectionDef.ID, out oSection))
                                {
                                    lookup7.Add(SectionDef.ID, oSection = SectionDef);
                                    if (oStepDefinition.Sections != null && oStepDefinition.Sections.Count() > 0)
                                    {
                                        oStepDefinition.Sections.Add(SectionDef);
                                    }
                                    else
                                    {
                                        oStepDefinition.Sections = new List<cStepSectionDefinition>();
                                        oStepDefinition.Sections.Add(SectionDef);
                                    }
                                }
                                cFieldDefinition oFieldDefintion;
                                if (FieldDefinition != null)
                                {

                                    if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                                    {
                                        lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                        if (oSection.FieldDefinitions != null && oSection.FieldDefinitions.Count() > 0)
                                        {
                                            oSection.FieldDefinitions.Add(oFieldDefintion);
                                        }
                                        else
                                        {
                                            oSection.FieldDefinitions = new List<cFieldDefinition>();
                                            oSection.FieldDefinitions.Add(oFieldDefintion);
                                        }
                                    }
                                    cFieldOrigin oXIFieldOrigin;
                                    if (FieldOrigin != null)
                                    {
                                        if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                        {

                                            if (FieldOrigin.iMasterDataID > 0)
                                            {
                                                FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                            }
                                            else if (FieldOrigin.FK1ClickID > 0)
                                            {
                                                var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                                var oBO = dbContext.BOs.Find(o1Click.BOID);
                                                FieldOrigin.bIsLargeBO = true;
                                                FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                                var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                                SqlConnection Con = new SqlConnection(sBODataSource);
                                                Con.Open();
                                                SqlCommand cmd = new SqlCommand();
                                                cmd.Connection = Con;
                                                cmd.CommandText = o1Click.Query;
                                                SqlDataReader reader = cmd.ExecuteReader();
                                                while (reader.Read())
                                                {
                                                    FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                                    {
                                                        sOptionValue = reader.GetInt32(0).ToString(),
                                                        sOptionName = reader.GetString(1)
                                                    });
                                                }
                                                Con.Close();
                                            }
                                            lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                            oXIFieldOrigin = FieldOrigin;
                                        }

                                        cXIFieldOptionList oOptions;
                                        if (OptionList != null)
                                        {
                                            oOptions = OptionList;
                                            if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                            {
                                                lookup6.Add(OptionList.ID, oOptions = OptionList);
                                                if (oXIFieldOrigin.ddlFieldOptionList != null && oXIFieldOrigin.ddlFieldOptionList.Count() > 0)
                                                {
                                                    oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                                }
                                                else
                                                {
                                                    oXIFieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                                    oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                                }
                                            }
                                        }
                                        oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                    }

                                    FieldOrigin.DataTypes = DataType;

                                    //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                                    //{
                                    //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                    //}
                                    //else
                                    //{
                                    //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                                    //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                    //}
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var Sections = lookup.Values.FirstOrDefault();

            var SectionContent = GetComponentParams(iQSID, iUserID, sOrgName, MainDB);
            if (SectionContent != null)
            {
                foreach (var items in SectionContent.QSSteps)
                {
                    foreach (var item in items.Sections)
                    {
                        if (item.ComponentDefinition != null)
                        {
                            var AllSections = Sections.QSSteps.Where(m => m.ID == items.ID).FirstOrDefault();
                            AllSections.Sections.Where(m => m.ID == item.ID).FirstOrDefault().ComponentDefinition = item.ComponentDefinition;
                        }
                    }
                }
            }
            return Sections;
        }

        public cQSStepDefiniton GetStepSectionDefinitionsByStep(int iStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var MainDB = sDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSDef " +
                "inner join XIStepSectionDefinition_T Sec on QSSDef.ID = Sec.FKiStepDefinitionID " +
                "left join XIFieldDefinition_T XIFD on Sec.ID = XIFD.FKiStepSectionID " +
                "left join XIFieldOrigin_T XIO on XIFD.FKiXIFieldOriginID = XIO.ID " +
                "left join XIDataType_T XIDT on XIO.FKiDataType = XIDT.id " +
                "left join XIFieldOptionList_T OPT on XIO.ID = OPT.FKiQSFieldID " +
                "WHERE QSSDef.ID = @id;";
            var param = new
            {
                id = iStepID
            };
            var lookup2 = new Dictionary<int, cQSStepDefiniton>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup = new Dictionary<int, cQSDefinition>();
                var lookup3 = new Dictionary<int, cFieldDefinition>();
                var lookup4 = new Dictionary<int, cFieldOrigin>();
                var lookup5 = new Dictionary<int, cQSNavigations>();
                var lookup6 = new Dictionary<int, cXIFieldOptionList>();
                var lookup7 = new Dictionary<int, cStepSectionDefinition>();
                Conn.Query<cQSStepDefiniton, cStepSectionDefinition, cFieldDefinition, cFieldOrigin, cXIDataTypes, cXIFieldOptionList, cQSStepDefiniton>(sQSDefinitionQry,
                    (Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
                    {
                        //cQSDefinition oQSDefinition;
                        //if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        //{
                        //    lookup.Add(QS.ID, oQSDefinition = QS);
                        //}
                        cQSStepDefiniton oStepDefinition;
                        //if (Step != null)
                        //{
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                            //if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                            //{
                            //    oQSDefinition.QSSteps.Add(oStepDefinition);
                            //}
                            //else
                            //{
                            //    oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                            //    oQSDefinition.QSSteps.Add(oStepDefinition);
                            //}
                        }
                        cStepSectionDefinition oSection;
                        if (SectionDef != null)
                        {
                            if (!lookup7.TryGetValue(SectionDef.ID, out oSection))
                            {
                                lookup7.Add(SectionDef.ID, oSection = SectionDef);
                                if (oStepDefinition.Sections != null && oStepDefinition.Sections.Count() > 0)
                                {
                                    oStepDefinition.Sections.Add(SectionDef);
                                }
                                else
                                {
                                    oStepDefinition.Sections = new List<cStepSectionDefinition>();
                                    oStepDefinition.Sections.Add(SectionDef);
                                }
                            }
                            cFieldDefinition oFieldDefintion;
                            if (FieldDefinition != null)
                            {

                                if (!lookup3.TryGetValue(FieldDefinition.ID, out oFieldDefintion))
                                {
                                    lookup3.Add(FieldDefinition.ID, oFieldDefintion = FieldDefinition);
                                    if (oSection.FieldDefinitions != null && oSection.FieldDefinitions.Count() > 0)
                                    {
                                        oSection.FieldDefinitions.Add(oFieldDefintion);
                                    }
                                    else
                                    {
                                        oSection.FieldDefinitions = new List<cFieldDefinition>();
                                        oSection.FieldDefinitions.Add(oFieldDefintion);
                                    }
                                }
                                cFieldOrigin oXIFieldOrigin;
                                if (FieldOrigin != null)
                                {
                                    if (!lookup4.TryGetValue(FieldOrigin.ID, out oXIFieldOrigin))
                                    {

                                        if (FieldOrigin.iMasterDataID > 0)
                                        {
                                            FieldOrigin.ddlFieldOptionList = dbContext.Types.Where(m => m.Code == FieldOrigin.iMasterDataID).ToList().Select(m => new cXIFieldOptionList { sOptionName = m.Expression, sOptionValue = m.ID.ToString() }).ToList();
                                        }
                                        else if (FieldOrigin.FK1ClickID > 0)
                                        {
                                            var o1Click = dbContext.Reports.Where(m => m.ID == FieldOrigin.FK1ClickID).FirstOrDefault();
                                            var oBO = dbContext.BOs.Find(o1Click.BOID);
                                            FieldOrigin.bIsLargeBO = true;
                                            FieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                                            SqlConnection Con = new SqlConnection(sBODataSource);
                                            Con.Open();
                                            SqlCommand cmd = new SqlCommand();
                                            cmd.Connection = Con;
                                            cmd.CommandText = o1Click.Query;
                                            SqlDataReader reader = cmd.ExecuteReader();
                                            while (reader.Read())
                                            {
                                                FieldOrigin.ddlFieldOptionList.Add(new cXIFieldOptionList
                                                {
                                                    sOptionValue = reader.GetInt32(0).ToString(),
                                                    sOptionName = reader.GetString(1)
                                                });
                                            }
                                            Con.Close();
                                        }
                                        lookup4.Add(FieldOrigin.ID, oXIFieldOrigin = FieldOrigin);
                                        oXIFieldOrigin = FieldOrigin;
                                    }

                                    cXIFieldOptionList oOptions;
                                    if (OptionList != null)
                                    {
                                        oOptions = OptionList;
                                        if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
                                        {
                                            lookup6.Add(OptionList.ID, oOptions = OptionList);
                                            if (oXIFieldOrigin.ddlFieldOptionList != null && oXIFieldOrigin.ddlFieldOptionList.Count() > 0)
                                            {
                                                oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                            }
                                            else
                                            {
                                                oXIFieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
                                                oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
                                            }
                                        }
                                    }
                                    oFieldDefintion.FieldOrigin = oXIFieldOrigin;
                                }

                                FieldOrigin.DataTypes = DataType;

                                //if (oFieldInstance.FieldDefinitions != null && oFieldInstance.FieldDefinitions.Count() > 0)
                                //{
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                                //else
                                //{
                                //    oFieldInstance.FieldDefinitions = new List<cXIFieldDefinition>();
                                //    oFieldInstance.FieldDefinitions.Add(FieldDefinition);
                                //}
                            }
                        }
                        //}
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }

            var StepDef = lookup2.Values.FirstOrDefault();

            var SectionContent = GetComponentParamsByStep(iStepID, iUserID, sOrgName, MainDB);
            if (SectionContent != null)
            {
                foreach (var item in StepDef.Sections)
                {
                    if (item.ComponentDefinition != null)
                    {
                        StepDef.Sections.Where(m => m.ID == item.ID).FirstOrDefault().ComponentDefinition = item.ComponentDefinition;
                    }
                }
            }
            return StepDef;
        }


        //private cQSDefinition GetStepSectionDefinitions(int iQSID, int iUserID, string sOrgName, string sDatabase)
        //{
        //    var MainDB = sDatabase;
        //    var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
        //    var iOrgID = UserDetais.FKiOrgID;
        //    var sOrgDB = UserDetais.sUserDatabase;
        //    if (iOrgID > 0)
        //    {
        //        sDatabase = sOrgDB;
        //    }
        //    string sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
        //        "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
        //        "inner join StepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
        //        "left join XIFieldDefinition_T XFID on Sec.ID = XFID.FKiStepSectionID " +
        //        "left join XIFieldOrigin_T XFO on XFID.FKiXIFieldOriginID = XFO.ID " +
        //        "left join XIDataType_T XDT on XFO.FKiDataType = XDT.ID " +
        //        "left join XIFieldOptionList_T Opt on XFO.ID = Opt.FKiQSFieldID " +
        //        "where QSD.ID = @id;";
        //    var param = new
        //    {
        //        id = iQSID
        //    };

        //    SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
        //    Conn.Open();
        //    Conn.ChangeDatabase(sDatabase);
        //    var lookup = new Dictionary<int, cQSDefinition>();
        //    var lookup2 = new Dictionary<int, cQSStepDefiniton>();
        //    var lookup3 = new Dictionary<int, cStepSectionDefinition>();
        //    var lookup4 = new Dictionary<int, cFieldOrigin>();
        //    var lookup5 = new Dictionary<int, cQSNavigations>();
        //    var lookup6 = new Dictionary<int, cXIFieldOptionList>();
        //    var lookup7 = new Dictionary<int, cFieldDefinition>();
        //    Conn.Query<cQSDefinition, cQSStepDefiniton, cStepSectionDefinition, cFieldDefinition, cFieldOrigin, cXIDataTypes, cXIFieldOptionList, cQSDefinition>(sQSDefinitionQry,
        //        (QS, Step, SectionDef, FieldDefinition, FieldOrigin, DataType, OptionList) =>
        //        {
        //            cQSDefinition oQSDefinition;
        //            if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
        //            {
        //                lookup.Add(QS.ID, oQSDefinition = QS);
        //            }
        //            cQSStepDefiniton oStepDefinition;
        //            if (Step != null)
        //            {
        //                if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
        //                {
        //                    lookup2.Add(Step.ID, oStepDefinition = Step);
        //                    if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
        //                    {
        //                        oQSDefinition.QSSteps.Add(oStepDefinition);
        //                    }
        //                    else
        //                    {
        //                        oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
        //                        oQSDefinition.QSSteps.Add(oStepDefinition);
        //                    }
        //                }
        //                cStepSectionDefinition oSection;
        //                if (SectionDef != null)
        //                {
        //                    if (!lookup3.TryGetValue(SectionDef.ID, out oSection))
        //                    {
        //                        lookup3.Add(SectionDef.ID, oSection = SectionDef);
        //                        if (oStepDefinition.Sections != null && oStepDefinition.Sections.Count() > 0)
        //                        {
        //                            oStepDefinition.Sections.Add(SectionDef);
        //                        }
        //                        else
        //                        {
        //                            oStepDefinition.Sections = new List<cStepSectionDefinition>();
        //                            oStepDefinition.Sections.Add(SectionDef);
        //                        }
        //                    }
        //                    cFieldDefinition oFieldDef;
        //                    if (FieldDefinition != null)
        //                    {
        //                        oFieldDef = FieldDefinition;
        //                        if (oSection.FieldDefinitions != null && oSection.FieldDefinitions.Count() > 0)
        //                        {
        //                            oSection.FieldDefinitions.Add(oFieldDef);
        //                        }
        //                        else
        //                        {
        //                            oSection.FieldDefinitions = new List<cFieldDefinition>();
        //                            oSection.FieldDefinitions.Add(oFieldDef);
        //                        }
        //                        cFieldOrigin oXIFieldOrigin;
        //                        if (FieldOrigin != null)
        //                        {
        //                            oXIFieldOrigin = FieldOrigin;
        //                            oFieldDef.FieldOrigin = oXIFieldOrigin;
        //                            oXIFieldOrigin.DataTypes = DataType;
        //                            cXIFieldOptionList oOptions;
        //                            if (OptionList != null)
        //                            {
        //                                oOptions = OptionList;
        //                                if (!lookup6.TryGetValue(OptionList.ID, out oOptions))
        //                                {
        //                                    lookup6.Add(OptionList.ID, oOptions = OptionList);
        //                                    if (oXIFieldOrigin.ddlFieldOptionList != null && oXIFieldOrigin.ddlFieldOptionList.Count() > 0)
        //                                    {
        //                                        oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
        //                                    }
        //                                    else
        //                                    {
        //                                        oXIFieldOrigin.ddlFieldOptionList = new List<cXIFieldOptionList>();
        //                                        oXIFieldOrigin.ddlFieldOptionList.Add(oOptions);
        //                                    }
        //                                }
        //                            }
        //                        }

        //                    }
        //                }
        //            }
        //            return oQSDefinition;
        //        },
        //        param
        //        ).AsQueryable();

        //    var Sections = lookup.Values.FirstOrDefault();


        //    var SectionContent = GetComponentParams(iQSID, iUserID, sOrgName, MainDB);
        //    if (SectionContent != null)
        //    {
        //        foreach (var items in SectionContent.QSSteps)
        //        {
        //            foreach (var item in items.Sections)
        //            {
        //                if (item.ComponentDefinition != null)
        //                {
        //                    var AllSections = Sections.QSSteps.Where(m => m.ID == items.ID).FirstOrDefault();
        //                    AllSections.Sections.Where(m => m.ID == item.ID).FirstOrDefault().ComponentDefinition = item.ComponentDefinition;
        //                }
        //            }
        //        }
        //    }
        //    return Sections;
        //}

        private cQSDefinition GetComponentParams(int iQSID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSDefinition_T QSD " +
                "inner join XIQSStepDefinition_T QSSD on QSD.ID = QSSD.FKiQSDefintionID " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "inner join XIComponentParams_T NVs on sec.ID = NVs.iStepSectionID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentID = XC.ID " +
                "where QSD.ID = @id;";
            var param = new
            {
                id = iQSID
            };

            var lookup = new Dictionary<int, cQSDefinition>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup2 = new Dictionary<int, cQSStepDefiniton>();
                var lookup3 = new Dictionary<int, cStepSectionDefinition>();
                var lookup4 = new Dictionary<int, cXIComponents>();
                Conn.Query<cQSDefinition, cQSStepDefiniton, cStepSectionDefinition, cXIComponentParams, cXIComponents, cQSDefinition>(sQSDefinitionQry,
                    (QS, Step, SectionDef, ComponentParams, Component) =>
                    {
                        cQSDefinition oQSDefinition;
                        if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        {
                            lookup.Add(QS.ID, oQSDefinition = QS);
                        }
                        cQSStepDefiniton oStepDefinition;
                        if (Step != null)
                        {
                            if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                            {
                                lookup2.Add(Step.ID, oStepDefinition = Step);
                                if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                                {
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                                else
                                {
                                    oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                                    oQSDefinition.QSSteps.Add(oStepDefinition);
                                }
                            }
                            cStepSectionDefinition oSection;
                            if (SectionDef != null)
                            {
                                if (!lookup3.TryGetValue(SectionDef.ID, out oSection))
                                {
                                    lookup3.Add(SectionDef.ID, oSection = SectionDef);
                                    if (oStepDefinition.Sections != null && oStepDefinition.Sections.Count() > 0)
                                    {
                                        oStepDefinition.Sections.Add(SectionDef);
                                    }
                                    else
                                    {
                                        oStepDefinition.Sections = new List<cStepSectionDefinition>();
                                        oStepDefinition.Sections.Add(SectionDef);
                                    }
                                }
                                cXIComponents oComponent;
                                if (Component != null)
                                {
                                    if (!lookup4.TryGetValue(SectionDef.ID, out oComponent))
                                    {
                                        lookup4.Add(SectionDef.ID, oComponent = Component);
                                        oSection.ComponentDefinition = Component;
                                    }
                                    if (ComponentParams != null)
                                    {
                                        if (oSection.ComponentDefinition.XIComponentParams != null && oSection.ComponentDefinition.XIComponentParams.Count() > 0)
                                        {
                                            oSection.ComponentDefinition.XIComponentParams.Add(ComponentParams);
                                        }
                                        else
                                        {
                                            oSection.ComponentDefinition.XIComponentParams = new List<cXIComponentParams>();
                                            oSection.ComponentDefinition.XIComponentParams.Add(ComponentParams);
                                        }
                                    }
                                }
                            }
                        }
                        return oQSDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var SectionContent = lookup.Values.FirstOrDefault();
            return SectionContent;
        }

        private cQSStepDefiniton GetComponentParamsByStep(int iStepID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            string sQSDefinitionQry = "select * from XIQSStepDefinition_T QSSD " +
                "inner join XIStepSectionDefinition_T Sec on QSSD.ID = Sec.FKiStepDefinitionID " +
                "inner join XIComponentParams_T NVs on sec.ID = NVs.iStepSectionID " +
                "inner join XIComponents_XC_T XC on NVs.FKiComponentID = XC.ID " +
                "where QSSD.ID = @id;";
            var param = new
            {
                id = iStepID
            };
            var lookup2 = new Dictionary<int, cQSStepDefiniton>();
            using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Conn.Open();
                var lookup = new Dictionary<int, cQSDefinition>();
                var lookup3 = new Dictionary<int, cStepSectionDefinition>();
                var lookup4 = new Dictionary<int, cXIComponents>();
                Conn.Query<cQSStepDefiniton, cStepSectionDefinition, cXIComponentParams, cXIComponents, cQSStepDefiniton>(sQSDefinitionQry,
                    (Step, SectionDef, ComponentParams, Component) =>
                    {
                        //cQSDefinition oQSDefinition;
                        //if (!lookup.TryGetValue(QS.ID, out oQSDefinition))
                        //{
                        //    lookup.Add(QS.ID, oQSDefinition = QS);
                        //}
                        cQSStepDefiniton oStepDefinition;
                        //if (Step != null)
                        //{
                        if (!lookup2.TryGetValue(Step.ID, out oStepDefinition))
                        {
                            lookup2.Add(Step.ID, oStepDefinition = Step);
                            //if (oQSDefinition.QSSteps != null && oQSDefinition.QSSteps.Count() > 0)
                            //{
                            //    oQSDefinition.QSSteps.Add(oStepDefinition);
                            //}
                            //else
                            //{
                            //    oQSDefinition.QSSteps = new List<cQSStepDefiniton>();
                            //    oQSDefinition.QSSteps.Add(oStepDefinition);
                            //}
                        }
                        cStepSectionDefinition oSection;
                        if (SectionDef != null)
                        {
                            if (!lookup3.TryGetValue(SectionDef.ID, out oSection))
                            {
                                lookup3.Add(SectionDef.ID, oSection = SectionDef);
                                if (oStepDefinition.Sections != null && oStepDefinition.Sections.Count() > 0)
                                {
                                    oStepDefinition.Sections.Add(SectionDef);
                                }
                                else
                                {
                                    oStepDefinition.Sections = new List<cStepSectionDefinition>();
                                    oStepDefinition.Sections.Add(SectionDef);
                                }
                            }
                            cXIComponents oComponent;
                            if (Component != null)
                            {
                                if (!lookup4.TryGetValue(SectionDef.ID, out oComponent))
                                {
                                    lookup4.Add(SectionDef.ID, oComponent = Component);
                                    oSection.ComponentDefinition = Component;
                                }
                                if (ComponentParams != null)
                                {
                                    if (oSection.ComponentDefinition.XIComponentParams != null && oSection.ComponentDefinition.XIComponentParams.Count() > 0)
                                    {
                                        oSection.ComponentDefinition.XIComponentParams.Add(ComponentParams);
                                    }
                                    else
                                    {
                                        oSection.ComponentDefinition.XIComponentParams = new List<cXIComponentParams>();
                                        oSection.ComponentDefinition.XIComponentParams.Add(ComponentParams);
                                    }
                                }
                            }
                        }
                        //}
                        return oStepDefinition;
                    },
                    param
                    ).AsQueryable();
            }
            var SectionContent = lookup2.Values.FirstOrDefault();
            return SectionContent;
        }

        //public cQSInstance SaveQSInstance(cQSInstance oQSInstance, int iCurrentStepID, int iUserID, string sOrgName, string sDatabase, string sCurrentUserGUID)
        //{
        //    return null;
        //}
        public cQSInstance SaveQSInstance(cQSInstance oQSInstance, int iCurrentStepID, int iUserID, string sOrgName, string sDatabase, string sCurrentGuestUser)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            var oStepInstances = oQSInstance.nStepInstances.ToList();
            cQSInstance oQSIns;
            if (oQSInstance.FKiBODID > 0)
            {
                oQSIns = dbContext.QSInstance.Where(m => m.FKiQSDefinitionID == oQSInstance.FKiQSDefinitionID && m.FKiBODID == oQSInstance.FKiBODID && m.iBOIID == oQSInstance.iBOIID).FirstOrDefault();
            }
            else
            {
                oQSIns = dbContext.QSInstance.Where(m => m.FKiQSDefinitionID == oQSInstance.FKiQSDefinitionID && m.FKiUserCookieID == sCurrentGuestUser).FirstOrDefault();
            }

            if (oQSIns == null)
            {
                oQSIns = new cQSInstance();
                oQSIns.sQSName = oQSInstance.QSDefinition.sName;
                oQSIns.iCurrentStepID = iCurrentStepID;
                oQSIns.FKiQSDefinitionID = oQSInstance.FKiQSDefinitionID;
                oQSIns.FKiUserCookieID = sCurrentGuestUser;
                oQSIns.FKiBODID = oQSInstance.FKiBODID;
                oQSIns.iBOIID = oQSInstance.iBOIID;
                dbContext.QSInstance.Add(oQSIns);
                dbContext.SaveChanges();
                oQSInstance.ID = oQSIns.ID;
                //InsertIntoAggregations(oQSIns.ID, sDatabase);
            }
            else
            {
                oQSIns.iCurrentStepID = iCurrentStepID;
                dbContext.SaveChanges();
                oQSInstance.ID = oQSIns.ID;
            }
            foreach (var oStep in oQSInstance.nStepInstances.Where(m => m.bIsCurrentStep == true))
            {
                cQSStepInstance oQSStepIns;
                oQSStepIns = dbContext.QSStepInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSStepDefinitionID == oStep.FKiQSStepDefinitionID).FirstOrDefault();
                if (oQSStepIns == null)
                {
                    oQSStepIns = new cQSStepInstance();
                    oQSStepIns.FKiQSInstanceID = oQSIns.ID;
                    oQSStepIns.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;
                    dbContext.QSStepInstance.Add(oQSStepIns);
                    dbContext.SaveChanges();
                    oStep.ID = oQSStepIns.ID;
                }
                //var AllFVlaueInstances = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSStepDefinitionID == oStep.FKiQSStepDefinitionID).ToList();
                //dbContext.XIFieldInstance.RemoveRange(AllFVlaueInstances);
                //dbContext.SaveChanges();

                if (oStep.nSectionInstances != null && oStep.nSectionInstances.Count() > 0)
                {
                    foreach (var sec in oStep.nSectionInstances)
                    {
                        cStepSectionInstance oSecIns = new cStepSectionInstance();
                        oSecIns = dbContext.StepSectionInstance.Where(m => m.FKiStepSectionDefinitionID == sec.FKiStepSectionDefinitionID && m.FKiStepInstanceID == oStep.ID).FirstOrDefault();
                        if (oSecIns == null)
                        {
                            oSecIns = new cStepSectionInstance();
                            oSecIns.FKiStepSectionDefinitionID = sec.FKiStepSectionDefinitionID;
                            oSecIns.FKiStepInstanceID = oQSStepIns.ID;
                            dbContext.StepSectionInstance.Add(oSecIns);
                            dbContext.SaveChanges();
                            sec.ID = oSecIns.ID;
                            sec.FKiStepInstanceID = oQSStepIns.ID;
                        }
                        if (sec.nFieldInstances != null && sec.nFieldInstances.Count() > 0)
                        {
                            //var SecFValueInstances = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSSectionDefinitionID == sec.FKiStepSectionDefinitionID).ToList();
                            //dbContext.XIFieldInstance.RemoveRange(SecFValueInstances);
                            //dbContext.SaveChanges();
                            foreach (var items in sec.nFieldInstances)
                            {
                                cFieldInstance oFIns = new cFieldInstance();
                                var StepDef = oQSInstance.QSDefinition.QSSteps.Where(m => m.ID == oStep.FKiQSStepDefinitionID).FirstOrDefault();
                                var SecDef = StepDef.Sections.Where(m => m.ID == sec.FKiStepSectionDefinitionID).FirstOrDefault();
                                var FieldOrigin = SecDef.FieldDefinitions.Where(m => m.ID == items.FKiFieldDefinitionID).FirstOrDefault().FieldOrigin;
                                oFIns = dbContext.XIFieldInstance.Where(m => m.FKiQSInstanceID == oQSIns.ID && m.FKiQSSectionDefinitionID == sec.FKiStepSectionDefinitionID && m.FKiFieldDefinitionID == items.FKiFieldDefinitionID).FirstOrDefault();
                                if (oFIns != null)
                                {
                                    if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "int")
                                    {
                                        oFIns.iValue = Convert.ToInt32(items.sValue);
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "datetime")
                                    {
                                        if (!string.IsNullOrEmpty(items.sValue))
                                        {
                                            oFIns.dValue = Convert.ToDateTime(items.sValue);
                                        }
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "decimal")
                                    {
                                        oFIns.rValue = Convert.ToDecimal(items.sValue);
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "boolean")
                                    {
                                        if (items.sValue == "on")
                                        {
                                            oFIns.bValue = true;
                                        }
                                        else
                                        {
                                            oFIns.bValue = false;
                                        }
                                    }
                                    oFIns.sValue = items.sValue;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    oFIns = new cFieldInstance();
                                    if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "int")
                                    {
                                        oFIns.iValue = Convert.ToInt32(items.sValue);
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "datetime")
                                    {
                                        if (!string.IsNullOrEmpty(items.sValue))
                                        {
                                            oFIns.dValue = Convert.ToDateTime(items.sValue);
                                        }
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "decimal")
                                    {
                                        oFIns.rValue = Convert.ToDecimal(items.sValue);
                                    }
                                    else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "boolean")
                                    {
                                        if (items.sValue == "on")
                                        {
                                            oFIns.bValue = true;
                                        }
                                        else
                                        {
                                            oFIns.bValue = false;
                                        }
                                    }
                                    oFIns.sValue = items.sValue;
                                    oFIns.FKiQSSectionDefinitionID = sec.FKiStepSectionDefinitionID;//oSecIns.ID;
                                    oFIns.FKiQSInstanceID = oQSIns.ID;
                                    oFIns.FKiFieldDefinitionID = items.FKiFieldDefinitionID;
                                    oFIns.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;// oStep.ID;
                                    dbContext.XIFieldInstance.Add(oFIns);
                                    dbContext.SaveChanges();
                                }
                                items.ID = oFIns.ID;
                                items.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;
                                items.FKiQSSectionDefinitionID = sec.FKiStepSectionDefinitionID;
                            }
                        }
                        else
                        {


                        }
                    }
                }
                else
                {
                    if (oStep.nFieldInstances != null)
                    {
                        foreach (var items in oStep.nFieldInstances)
                        {
                            var StepDef = oQSInstance.QSDefinition.QSSteps.Where(m => m.ID == oStep.FKiQSStepDefinitionID).FirstOrDefault();
                            var FieldOrigin = StepDef.FieldDefinitions.Where(m => m.ID == items.FKiFieldDefinitionID).FirstOrDefault().FieldOrigin;
                            cFieldInstance oFIns = new cFieldInstance();
                            if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "int")
                            {
                                oFIns.iValue = Convert.ToInt32(items.sValue);
                            }
                            else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "datetime")
                            {
                                if (items.sValue != null)
                                {
                                    oFIns.dValue = Convert.ToDateTime(items.sValue);
                                }
                            }
                            else if (FieldOrigin.DataTypes.sBaseDataType.ToLower() == "decimal")
                            {
                                oFIns.rValue = Convert.ToDecimal(items.sValue);
                            }
                            oFIns.sValue = items.sValue;
                            oFIns.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;// oQSStepIns.ID;
                            oFIns.FKiQSInstanceID = oQSIns.ID;
                            oFIns.FKiFieldDefinitionID = items.FKiFieldDefinitionID;
                            dbContext.XIFieldInstance.Add(oFIns);
                            dbContext.SaveChanges();
                            items.ID = oFIns.ID;
                            items.FKiQSStepDefinitionID = oStep.FKiQSStepDefinitionID;// oStep.ID;
                        }
                    }
                    else
                    {

                    }
                }
            }
            return oQSInstance;
        }

        public void InsertIntoAggregations(int iD, string sDatabase, int iUserID, int iCustomerID)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            for (int i = 2; i < 7; i++)
            {
                cAggregations oAggr = new cAggregations();
                if (i == 2)
                {
                    oAggr.sInsurer = "CommercialExpress";
                }
                else if (i == 3)
                {
                    oAggr.sInsurer = "Markerstudy";
                }
                else if (i == 4)
                {
                    oAggr.sInsurer = "PolicyFast";
                }
                else if (i == 5)
                {
                    oAggr.sInsurer = "PolicyPlan";
                }
                else if (i == 6)
                {
                    oAggr.sInsurer = "Tansar";
                }
                oAggr.FKiQSInstanceID = iD;
                Random random = new Random();
                oAggr.rPrice = random.Next(300, 700) + (i * 10);
                oAggr.bLiablityCover = true;
                oAggr.bLiabilityLimit = false;
                oAggr.bLossOfMeteredWater = true;
                oAggr.bLegelExpensesCover = false;
                oAggr.FKiUserID = iUserID;
                oAggr.FKiCustomerID = iCustomerID;
                dbContext.Aggregations.Add(oAggr);
                dbContext.SaveChanges();
            }
        }

        #endregion QuestionSet

        #region DemoXIScripts

        public string XIScripting(int XILinkID, string sGUID, int iInstanceID, int iBOID, int iUserID, string sOrgName, string sDatabase, int iCustomerID)
        {
            //int iBOID = 2; //orders
            //int iInstanceID = 1;
            ModelDbContext dbContext = new ModelDbContext();
            string OrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            object oResult = RunXIScript(iBOID, iInstanceID, sGUID, XILinkID, sDatabase, OrgDB, iUserID, iCustomerID, sOrgName);
            XIResults xiResults = (XIResults)(oResult);
            //Get details from XILink an create a param list using Name value pair to get the BOScriptID
            List<cNameValuePairs> lParam = new List<cNameValuePairs>();
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
            NVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == XILinkID).ToList();
            foreach (var items in NVs)
            {
                cNameValuePairs Params = new cNameValuePairs();
                Params.sName = items.Name;
                Params.sValue = items.Value;
                lParam.Add(Params);
            }

            //get the group and action name to load script.
            string sActionName = lParam.Where(m => m.sName == "ScriptAction").FirstOrDefault().sValue;

            //get the BOScriptID to get Result details based on BOID and Action Name.
            long lScriptID = dbContext.BOScripts.Where(m => m.sName == sActionName).Where(m => m.FKiBOID == iBOID).Select(m => m.ID).FirstOrDefault();
            int iScriptID = Convert.ToInt32(lScriptID);
            //get scriptresult details
            BOScriptResults sScriptResult = dbContext.BOScriptResults.Where(m => m.FKiScriptID == iScriptID).Where(m => m.sResultCode == xiResults.sCode).FirstOrDefault();
            string sUserError = sScriptResult.sUserError;
            Common.SaveErrorLog(sUserError, "XIDNA");
            Common.SaveErrorLog(sScriptResult.sResultCode, "XIDNA");
            Common.SaveErrorLog(xiResults.sMessage, "XIDNA");
            string sGetStringVal = "";
            if (sUserError.Contains('{') && sUserError.Contains('}'))
            {
                Regex Regx = new Regex(@"{(.+?)}");
                MatchCollection MatchCol = Regx.Matches(sUserError);
                for (int i = 0; i < MatchCol.Count; i++)
                {
                    //Split and get string
                    string[] sSplitUserError = MatchCol[i].Value.Split(new Char[] { '{', '}' });
                    sGetStringVal = sSplitUserError[1];
                    var sNVValue = xiResults.NVPairs.Where(m => m.sName == sGetStringVal).Select(m => m.sValue).FirstOrDefault();
                    sUserError = sUserError.Replace("{" + sGetStringVal + "}", sNVValue);
                }
            }

            else
            {
                //show the error to user
                // sUserError = sUserError;
            }
            string sReturn = sScriptResult.iType + "_" + sUserError;
            return sReturn;
        }

        private object RunXIScript(int BOID, int ID, string sGUID, int XILinkID, string sDatabase, string OrgDB, int iUserID, int iCustomerID, string sOrgName)
        {
            CXiAPI oXiAPI = new CXiAPI();
            cBOInstance oBOINstance = new cBOInstance();
            XIResults oXIResults = new XIResults();
            ModelDbContext dbContext = new ModelDbContext();

            //Set parameters....
            var sSessionID = HttpContext.Current.Session.SessionID;
            string sUID = sGUID;
            if (ID > 0)
            {
                string sBOName = string.Empty;
                var oBO = dbContext.BOs.Find(BOID);
                if (oBO != null)
                {
                    sBOName = oBO.Name;
                }
                oXiAPI.Set_ParamVal(sSessionID, sUID, "{XIP|" + sBOName + ".id}", ID.ToString());
                oXiAPI.Set_ParamVal(sSessionID, sUID, "{XIP|BODID}", BOID.ToString());
            }

            //Create a paramlist
            List<cNameValuePairs> lParam = new List<cNameValuePairs>();
            cNameValuePairs Param = new cNameValuePairs();
            Param.sName = "sUID";
            Param.sValue = sUID;
            lParam.Add(Param);
            cNameValuePairs ParamSession = new cNameValuePairs();
            ParamSession.sName = "sSessionID";
            ParamSession.sValue = sSessionID;
            lParam.Add(ParamSession);
            cNameValuePairs param = new cNameValuePairs();
            param.sName = "iInsatnceID";
            param.sValue = ID.ToString();
            lParam.Add(param);
            cNameValuePairs param1 = new cNameValuePairs();
            param1.sName = "iUserID";
            param1.sValue = iUserID.ToString();
            lParam.Add(param1);
            cNameValuePairs param2 = new cNameValuePairs();
            param2.sName = "iCustomerID";
            param2.sValue = iCustomerID.ToString();
            lParam.Add(param2);

            //Get details from XILink an create a param list using Name value pair  
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
            NVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == XILinkID).ToList();
            foreach (var items in NVs)
            {
                cNameValuePairs Params = new cNameValuePairs();
                Params.sName = items.Name;
                Params.sValue = items.Value;
                lParam.Add(Params);
            }

            //get the group and action name to load script.
            //string sBOGroup = lParam.Where(m => m.sName == "Group").FirstOrDefault().sValue;
            string sActionName = lParam.Where(m => m.sName == "ScriptAction").FirstOrDefault().sValue;
            string sMethodName = lParam.Where(m => m.sName.ToLower() == "MethodName".ToLower()).FirstOrDefault().sValue;
            var iBOID = BOID;
            var iOrderID = ID;
            string sScript = oBOINstance.Action_Execute(BOID, ID, "", sActionName, sDatabase, OrgDB);
            //For Now API will have only DB details            
            oXiAPI.DevDB = sDatabase;
            oXiAPI.SharedDB = OrgDB;

            //Create a object to get the script values returned
            object oResult = null;
            ////Test 1:         

            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //for (int i = 0; i <= 1000; i++)
            //{
            //    Call the method to compile the script with script as parameter   
            //    MethodInfo methodInfo = WriteXIMethod(sScript);

            //    if (methodInfo != null)
            //    {
            //        oResult = methodInfo.Invoke(null, new object[] { oXiAPI, lParam });
            //    }
            //}
            // Thread.Sleep(10000);
            //stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            //TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value. 
            //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            //    ts.Hours, ts.Minutes, ts.Seconds,
            //    ts.Milliseconds / 10);

            //// test 2.
            //Call the method to compile the script with script as parameter   
            MethodInfo methodInfoFromCache = null;// oXiAPI.Get_ScriptVal(sSessionID, sUID, "|XI.script");
            if (methodInfoFromCache != null)
            {
                oResult = methodInfoFromCache.Invoke(null, new object[] { oXiAPI, lParam });
            }
            else
            {
                Common.SaveErrorLog("Execution starts WriteXIMethod", "XIDNA");
                MethodInfo methodInfo = WriteXIMethod(sScript, sMethodName);
                oXiAPI.Set_ScriptVal(sSessionID, sUID, "|XI.script", methodInfo);
                //put script in cache
                methodInfoFromCache = oXiAPI.Get_ScriptVal(sSessionID, sUID, "|XI.script");
                oResult = methodInfoFromCache.Invoke(null, new object[] { oXiAPI, lParam });
                Common.SaveErrorLog("script executed successfully", "XIDNA");
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            //for (int i = 0; i <= 1000; i++)
            //{


            //}
            // Thread.Sleep(10000);
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value. 
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            return oResult;
        }

        public MethodInfo WriteXIMethod(string sScript, string sMethodName)
        {
            string sCode = @"  using System;
                  using XIDNA.Models;
                  using XIDNA.ViewModels;
                  using System.Core;
                  using XIDNA.Repository;
                  using System.Linq;
                  using System.Collections.Generic;
                  using System.Threading;
                  using System.Data;
                  using System.Data.SqlClient;
                  using System.Data.Entity; 
                   namespace XIScripting
                   {                
                       public class cXIScripting
                       {                
                          " + sScript + @"
                       }
                   }
               ";
            CompilerParameters loParameters = new CompilerParameters();
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string sPath = physicalPath.Substring(0, physicalPath.Length) + "\\DLL";
            //string sPath = System.Configuration.ConfigurationManager.AppSettings["DLLPath"];
            // *** Start by adding any referenced assemblies
            loParameters.ReferencedAssemblies.Add("System.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Core.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Linq.dll");
            //loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.Linq.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.Models.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.Repository.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\System.Data.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\EntityFramework.SqlServer.dll");
            loParameters.ReferencedAssemblies.Add(sPath + "\\XIDNA.ViewModels.dll");
            // loParameters.ReferencedAssemblies.Add("D:\\TfsProject\\XIDNA\\XIDNA\\XIDNA.Repository\\bin\\Debug\\XIDNA.Repository.dll");
            loParameters.GenerateInMemory = false;
            ICodeCompiler provider = new CSharpCodeProvider().CreateCompiler();
            CompilerResults results = provider.CompileAssemblyFromSource(loParameters, sCode);
            if (results.Errors.HasErrors)
            {

                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    Common.SaveErrorLog(error.ErrorText, "XIDNA");
                    sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }

                throw new InvalidOperationException(sb.ToString());
            }

            Type binaryFunction = results.CompiledAssembly.GetType("XIScripting.cXIScripting");
            return binaryFunction.GetMethod(sMethodName);

        }

        #endregion DemoXIScripts
        public List<VMDropDown> GetAutoCompleteData(int i1ClickID, string sSearchText, int iUserID, string sOrgName, string sDatabase)
        {
            List<VMDropDown> Data = new List<VMDropDown>();
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetails.FKiOrgID;
            var sOrgDB = UserDetails.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            var o1Click = dbContext.Reports.Where(m => m.ID == i1ClickID).FirstOrDefault();
            var oBO = dbContext.BOs.Find(o1Click.BOID);
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = o1Click.Query;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Data.Add(new VMDropDown
                    {
                        Value = Convert.ToInt32(reader.GetInt32(0)),
                        text = reader.GetString(1)
                    });
                }
                Con.Close();
            }
            return Data.Where(m => m.text.ToLower().Contains(sSearchText)).ToList();
        }

        public string ClaimTerms(List<VMFormData> FormValues, string sDatabase, int iUserID, string sOrgName)
        {
            string sMessage = string.Empty;
            string sClaimValue = string.Empty;
            string sClaimCost = string.Empty;
            try
            {
                for (int i = 0; i < FormValues.Count(); i++)
                {
                    var label = FormValues[i].Label;
                    if (label == "Number of Claims")
                    {
                        sClaimValue = FormValues[i].Data;
                    }
                    else if (label == "Claim Cost")
                    {
                        sClaimCost = FormValues[i].Data;
                    }
                }
                if (sClaimValue == "1")
                {
                    if (Convert.ToInt32(sClaimCost) < 10000)
                    {
                        sMessage = "Normal";
                    }
                }
                else if (sClaimValue.ToLower() == "Any".ToLower())
                {
                    if (sClaimCost == "Personal Injury" || sClaimCost == "Damage or Vandalism")
                    {
                        sMessage = "Refer";
                    }
                }
                else if (sClaimValue == "2" || Convert.ToInt32(sClaimValue) > 2)
                {
                    if (Convert.ToInt32(sClaimCost) < 10000)
                    {
                        sMessage = "Refer";
                    }
                }
            }
            catch (Exception ex)
            {
                sMessage = ex.ToString();
            }
            return sMessage;
        }


        public string CliamConvictionData(List<VMFormData> FormValues, string sDatabase, int iUserID, string sOrgName)
        {
            string sMessage = string.Empty;
            string sConvictionValue = string.Empty;
            string sConvictionCode = string.Empty;
            string stringArra = string.Empty;
            string ConStringArray = "";
            string sConviCode = "";
            try
            {
                for (int i = 0; i < FormValues.Count(); i++)
                {
                    var label = FormValues[i].Label;
                    if (label == "Number of Convictions")
                    {
                        sConvictionValue = FormValues[i].Data;
                    }
                    else if (label == "Conviction Code")
                    {
                        sConvictionCode = FormValues[i].Data;
                    }
                }
                if (sConvictionValue.ToLower() == "Any".ToLower() && sConvictionCode.ToLower() == "Ban or Not Listed".ToLower())
                {
                    sMessage = "Refer";
                }
                else if (sConvictionValue.ToLower() == "Any".ToLower())
                {
                    if (sConvictionCode.ToLower() == "S19".ToLower())
                    {
                        sConviCode = sConvictionCode;
                    }
                    else
                    {
                        var Prefix = sConvictionCode.Substring(0, 2);
                        if (Prefix == "DD")
                        {
                            sConviCode = Prefix;
                            for (int i = 20; i <= 40; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            for (int i = 60; i <= 80; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                        }
                        else if (Prefix == "DR")
                        {
                            sConviCode = Prefix;
                            for (int i = 80; i <= 90; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                        }
                        else if (Prefix == "LC")
                        {
                            sConviCode = Prefix;
                            for (int i = 30; i <= 50; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                        }
                        else if (Prefix == "UT")
                        {
                            sConviCode = Prefix;
                            for (int i = 10; i <= 50; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                        }
                        else if (Prefix == "MS")
                        {
                            sConviCode = Prefix;
                            for (int i = 40; i <= 90; i++)
                            {
                                string ConviArray = sConviCode + i;
                                stringArra = '"' + ConviArray + '"' + ",";
                                ConStringArray = ConStringArray + stringArra;
                            }
                            ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                        }
                        else if (Prefix == "AC" || Prefix == "BA" || Prefix == "S19" || Prefix == "TT" || Prefix == "XX")
                        {
                            sConviCode = Prefix;
                            string ConviArray = sConviCode;
                            ConStringArray = '"' + ConviArray + '"';
                        }
                    }
                    List<string> sConviValue = ConStringArray.Split(',').ToList();
                    sConvictionCode = '"' + sConvictionCode + '"';
                    foreach (string s in sConviValue)
                    {
                        if (sConvictionCode.Contains(s))
                        {
                            if (sConviCode == "DD" || sConviCode == "DR" || sConviCode == "LC" || sConviCode == "UT" || sConviCode == "AC" || sConviCode == "BA" || sConviCode == "S19" || sConviCode == "TT" || sConviCode == "XX" || sConviCode == "MS")
                            {
                                sMessage = "Decline";
                            }
                        }
                    }
                }
                else if (sConvictionValue == "1" || sConvictionValue == "2" || Convert.ToInt32(sConvictionValue) > 2)
                {
                    var Prefix = sConvictionCode.Substring(0, 2);
                    if (Prefix == "CD")
                    {
                        sConviCode = Prefix;
                        for (int i = 10; i <= 30; i++)
                        {
                            string ConviArray = sConviCode + i;
                            stringArra = '"' + ConviArray + '"' + ",";
                            ConStringArray = ConStringArray + stringArra;
                        }
                        ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                    }
                    else if (Prefix == "DR")
                    {
                        sConviCode = Prefix;
                        for (int i = 10; i <= 70; i++)
                        {
                            string ConviArray = sConviCode + i;
                            stringArra = '"' + ConviArray + '"' + ",";
                            ConStringArray = ConStringArray + stringArra;
                        }
                        ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                    }
                    else if (Prefix == "IN")
                    {
                        sConviCode = Prefix;
                        for (int i = 10; i <= 10; i++)
                        {
                            string ConviArray = sConviCode + i;
                            stringArra = '"' + ConviArray + '"' + ",";
                            ConStringArray = ConStringArray + stringArra;
                        }
                        ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                    }
                    else if (Prefix == "MS")
                    {
                        sConviCode = Prefix;
                        for (int i = 10; i <= 30; i++)
                        {
                            string ConviArray = sConviCode + i;
                            stringArra = '"' + ConviArray + '"' + ",";
                            ConStringArray = ConStringArray + stringArra;
                        }
                        ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                    }
                    else if (Prefix == "LC")
                    {
                        sConviCode = Prefix;
                        for (int i = 20; i <= 20; i++)
                        {
                            string ConviArray = sConviCode + i;
                            stringArra = '"' + ConviArray + '"' + ",";
                            ConStringArray = ConStringArray + stringArra;
                        }
                        ConStringArray = ConStringArray.Substring(0, ConStringArray.Length - 1);
                    }
                    else if (Prefix == "CU" || Prefix == "LC" || Prefix == "MW" || Prefix == "PC" || Prefix == "PL" || Prefix == "SP" || Prefix == "TS")
                    {
                        sConviCode = Prefix;
                        string ConviArray = sConviCode;
                        ConStringArray = '"' + ConviArray + '"';
                    }
                    List<string> sConviValue = ConStringArray.Split(',').ToList();
                    sConvictionCode = '"' + sConvictionCode + '"';
                    foreach (string s in sConviValue)
                    {
                        if (sConvictionCode.Contains(s))
                        {
                            if (sConviCode == "CD" && sConvictionValue == "1")
                            {
                                sMessage = "25% Load";
                            }
                            else if (sConviCode == "DR" && sConvictionValue == "1")
                            {
                                sMessage = "75% Load";
                            }
                            else if (sConviCode == "IN" && sConvictionValue == "1")
                            {
                                sMessage = "50% Load";
                            }
                            else if (sConviCode == "MS" || sConviCode == "CU" || sConviCode == "LC" || sConviCode == "MW" || sConviCode == "PC" || sConviCode == "PL" || sConviCode == "SP" || sConviCode == "TS")
                            {
                                if (sConvictionValue == "1" || sConvictionValue == "2")
                                {
                                    sMessage = "Normal";
                                }
                                else
                                {
                                    sMessage = "Refer";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sMessage = ex.ToString();
            }
            return sMessage;
        }

    }
}
