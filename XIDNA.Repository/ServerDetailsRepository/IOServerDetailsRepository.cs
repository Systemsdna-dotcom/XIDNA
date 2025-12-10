using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;


namespace XIDNA.Repository
{
    public class IOServerDetailsRepository : IIOServerDetailsRepository
    {
        ModelDbContext dbContext = new ModelDbContext();
        public VMCustomResponse SaveServerDetails(IOServerDetails model)
        {
            try
            {
                IOServerDetails IOSD = new IOServerDetails();
                if (model.ID == 0)
                {
                    IOSD.Category = model.Category;
                    if (IOSD.Category == 1)
                    {
                        IOSD.OrganizationID = model.OrganizationID;
                        IOSD.ServerName = model.ServerName;
                        IOSD.Security = model.Security;
                        IOSD.FromAddress = model.FromAddress;
                        IOSD.UserName = model.UserName;
                        IOSD.Password = model.Password;
                        IOSD.Port = model.Port;
                        IOSD.Type = model.Type;
                        IOSD.SenderID = "NULL";
                        IOSD.SMSPath = "NULL";
                        IOSD.StatusTypeID = model.StatusTypeID;
                    }
                    else
                    {
                        IOSD.OrganizationID = model.OrganizationID;
                        IOSD.ServerName = model.ServerName;
                        IOSD.Security = "NULL";
                        IOSD.FromAddress = "NULL";
                        IOSD.UserName = model.UserName;
                        IOSD.Password = model.Password;
                        IOSD.Port = model.Port;
                        IOSD.Type = model.Type;
                        IOSD.SenderID = "NULL";
                        IOSD.SMSPath = "NULL";
                        IOSD.StatusTypeID = model.StatusTypeID;
                    }
                    dbContext.IOServerDetails.Add(IOSD);
                    dbContext.SaveChanges();
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = IOSD.OrganizationID, Status = true };
                }
                else
                {
                    IOSD = dbContext.IOServerDetails.Find(model.ID);
                    //IOSD.Category = model.Category;
                    if (IOSD.Category == 1)
                    {
                        IOSD.ServerName = model.ServerName;
                        //IOSD.OrganizationID = model.OrganizationID;
                        IOSD.Port = model.Port;
                        IOSD.FromAddress = model.FromAddress;
                        IOSD.Security = model.Security;
                        IOSD.UserName = model.UserName;
                        // IOSD.Password = model.Password;
                        if (model.UpdatePassword != null)
                        {
                            IOSD.Password = model.UpdatePassword;
                        }
                        IOSD.SenderID = "NULL";
                        IOSD.SMSPath = "NULL";
                        IOSD.Type = model.Type;
                        IOSD.StatusTypeID = model.StatusTypeID;
                    }
                    else
                    {
                        IOSD.ServerName = model.ServerName;
                        //IOSD.OrganizationID = model.OrganizationID;
                        IOSD.Port = model.Port;
                        IOSD.FromAddress = "NULL";
                        IOSD.Security = "NULL";
                        IOSD.UserName = model.UserName;
                        if (model.UpdatePassword != null)
                        {
                            IOSD.Password = model.UpdatePassword;
                        }
                        IOSD.SenderID = "NULL";
                        IOSD.SMSPath = "NULL";
                        IOSD.Type = model.Type;
                        IOSD.StatusTypeID = model.StatusTypeID;
                    }
                    dbContext.SaveChanges();
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = IOSD.OrganizationID, Status = true };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string GetOrgName(int p, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var orgname = dbCore.Organization.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return orgname;
        }

        public DTResponse ServerDetailsList(jQueryDataTableParamModel param, int Type, int OrgID, int Category, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            IQueryable<IOServerDetails> AllSrvrs;
            if (OrgID == 0)
            {
                if (Category == 0)
                {
                    AllSrvrs = dbContext.IOServerDetails.Where(m => m.Type == Type);
                }
                else
                {
                    AllSrvrs = dbContext.IOServerDetails.Where(m => m.Type == Type && m.Category == Category);
                }

            }
            else
            {
                if (Category == 0)
                {
                    AllSrvrs = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID).Where(m => m.Type == Type);
                }
                else
                {
                    AllSrvrs = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID).Where(m => m.Type == Type && m.Category == Category);
                }

            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSrvrs = AllSrvrs.Where(m => m.ServerName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSrvrs.Count();
            AllSrvrs = QuerableUtil.GetResultsForDataTables(AllSrvrs, "", sortExpression, param);
            var clients = AllSrvrs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),GetOrgName(c.OrganizationID,sDatabase), c.Category.ToString(), c.ServerName,c.FromAddress, Convert.ToString(c.Port),  (c.Security),c.UserName,c.StatusTypeID.ToString(),"Edit" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public IOServerDetails EditServerDetails(int ID)
        {
            IOServerDetails IOSD = new IOServerDetails();
            IOSD = dbContext.IOServerDetails.Find(ID);
            return IOSD;
        }

        public VMCustomResponse SaveSMSServerDetails(IOServerDetails model)
        {
            IOServerDetails IOSD = new IOServerDetails();
            if (model.ID == 0)
            {
                IOSD.ServerName = "NULL";
                IOSD.Security = "NULL";
                IOSD.FromAddress = "NULL";
                IOSD.SenderID = model.SenderID;
                IOSD.SMSPath = model.SMSPath;
                IOSD.Type = model.Type;
                IOSD.Port = 0;
                IOSD.OrganizationID = model.OrganizationID;
                IOSD.UserName = model.UserName;
                IOSD.Password = model.Password;
                IOSD.SMSAPIKey = model.SMSAPIKey;
                dbContext.IOServerDetails.Add(IOSD);
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = IOSD.OrganizationID, Status = true };
            }
            else
            {
                IOSD = dbContext.IOServerDetails.Find(model.ID);
                IOSD.ServerName = "NULL";
                IOSD.Security = "NULL";
                IOSD.FromAddress = "NULL";
                IOSD.Port = 0;
                //IOSD.OrganizationID = model.OrganizationID;
                IOSD.SenderID = model.SenderID;
                IOSD.SMSPath = model.SMSPath;
                IOSD.Type = model.Type;
                IOSD.UserName = model.UserName;
                IOSD.Password = model.Password;
                IOSD.SMSAPIKey = model.SMSAPIKey;
                IOSD.Type = model.Type;
                dbContext.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = IOSD.OrganizationID, Status = true };
            }
        }
        public IOServerDetails EditSMSServerDetails(int ID)
        {
            IOServerDetails IOSD = new IOServerDetails();
            IOSD = dbContext.IOServerDetails.Find(ID);
            return IOSD;
        }

        public DTResponse SMSServerDetailsList(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            IQueryable<IOServerDetails> AllDetails;
            if (OrgID == 0)
            {
                AllDetails = dbContext.IOServerDetails.Where(m => m.Type == Type);
            }
            else
            {
                AllDetails = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID).Where(m => m.Type == Type);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllDetails = AllDetails.Where(m => m.SenderID.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllDetails.Count();
            AllDetails = QuerableUtil.GetResultsForDataTables(AllDetails, "", sortExpression, param);
            var clients = AllDetails.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),GetOrgName(c.OrganizationID,sDatabase),c.SMSPath, c.SenderID,c.UserName,c.Password,"Edit" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<VMDropDown> GetOrganizations(string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<VMDropDown> AllOrgs = new List<VMDropDown>();
            AllOrgs = (from c in dbCore.Organization.Where(m => m.StatusTypeID == 10).ToList()
                       select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            //AllOrgs.Insert(0, new VMDropDown
            //{
            //    text = "--Select--",
            //    Value = 0
            //});
            return AllOrgs;
        }
        public DTResponse SpecificOrgIOServerDetailsGrid(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase)
        {
            IEnumerable<IOServerDetails> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).Where(m => m.ServerName.Contains(param.sSearch.ToUpper())).ToList();
                AllDetails = FilteredDetails.Where(m => m.Type == Type).OrderBy(m => m.Type == Type).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDetails.Count();
            }
            else
            {
                displyCount = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).Count();
                AllDetails = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).OrderBy(m => m.Type == Type).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }

            var result = from c in AllDetails
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID),GetOrgName(c.OrganizationID,sDatabase),c.Category.ToString(), c.ServerName,c.FromAddress, Convert.ToString(c.Port),  (c.Security),c.UserName };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse SpecificOrgSMSServerDetailsGrid(jQueryDataTableParamModel param, int Type, int OrgID, string sDatabase)
        {
            IEnumerable<IOServerDetails> AllDetails, FilteredDetails;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredDetails = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).Where(m => m.ServerName.Contains(param.sSearch.ToUpper())).ToList();
                AllDetails = FilteredDetails.Where(m => m.Type == Type).OrderBy(m => m.Type == Type).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredDetails.Count();
            }
            else
            {
                displyCount = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).Count();
                AllDetails = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).OrderBy(m => m.Type == Type).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }

            var result = from c in AllDetails
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID),GetOrgName(c.OrganizationID,sDatabase),c.SMSPath, c.SenderID,c.UserName,c.Password };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
    }
}
