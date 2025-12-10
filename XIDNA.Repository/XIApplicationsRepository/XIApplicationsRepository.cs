using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class XIApplicationsRepository : IXIApplicationsRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        public DTResponse GetXIApplicationsGrid(jQueryDataTableParamModel param)
        {
            IQueryable<cXIApplications> AllXIApplications;
            AllXIApplications = dbContext.XIApplications;
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXIApplications = AllXIApplications.Where(m => m.sApplicationName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXIApplications.Count();
            AllXIApplications = QuerableUtil.GetResultsForDataTables(AllXIApplications, "", sortExpression, param);
            var clients = AllXIApplications.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sApplicationName, c.sDescription, c.sDatabaseName, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse SaveXIApplication(cXIApplications model, int iUserID, string sOrgName, string sDatabase)
        {
            cXIApplications oXIApp = new cXIApplications();
            if (model.ID == 0)
            {
                oXIApp.sApplicationName = model.sApplicationName;
                oXIApp.sDescription = model.sDescription;
                oXIApp.StatusTypeID = model.StatusTypeID;
                oXIApp.CreatedBy = model.CreatedBy;
                oXIApp.CreatedTime = DateTime.Now;
                oXIApp.UpdatedBy = model.UpdatedBy;
                oXIApp.UpdatedTime = DateTime.Now;
                oXIApp.sUserName = model.XIAppUserName;
                oXIApp.sType = "Add";
                //My Code change it later as login dosn't work for organization specific DB
                oXIApp.sConnectionString = model.sConnectionString;
                //if (model.sDatabaseName != null)
                //{
                //    oXIApp.sDatabaseName = model.sDatabaseName;
                //}
                //else
                //{
                oXIApp.sDatabaseName = model.sApplicationName + "_Core"; //model.sApplicationName + "_Core";
                                                                         // }
                dbContext.XIApplications.Add(oXIApp);
                dbContext.SaveChanges();

                //My Code Add UrlMapping by default
                cXIUrlMappings oXIUrls = new cXIUrlMappings();
                int iXIUrlID = dbContext.XIUrlMappings.Where(m => m.FKiApplicationID == oXIApp.ID).Select(m => m.ID).FirstOrDefault();
                if (iXIUrlID > 0)
                {
                    oXIUrls.ID = iXIUrlID;
                }
                else
                {
                    oXIUrls.ID = 0;
                }
                oXIUrls.sUrlName = model.sApplicationName;
                oXIUrls.sActualUrl = model.sApplicationName;
                oXIUrls.FKiApplicationID = oXIApp.ID;
                oXIUrls.sType = "Application";
                oXIUrls.StatusTypeID = 10;
                SaveXIUrlMappingDetails(oXIUrls, iUserID, sOrgName, sDatabase);
                //Create Application Core DataBase
                CreateOrganizationDatabase(oXIApp.sDatabaseName);
                // Create Application Shared DataBase
                CreateOrganizationDatabase(model.sApplicationName + "_Shared");
            }
            else
            {
                oXIApp.sApplicationName = model.sApplicationName;
                oXIApp.sDescription = model.sDescription;
                oXIApp.sUserName = model.sUserName;
                oXIApp.sType = "Edit";
                oXIApp.sConnectionString = model.sConnectionString;
                oXIApp.StatusTypeID = model.StatusTypeID;
                oXIApp = dbContext.XIApplications.Find(model.ID);
                oXIApp.UpdatedBy = model.UpdatedBy;
                oXIApp.UpdatedTime = DateTime.Now;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oXIApp.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage, sID = oXIApp.sDatabaseName, PropertyName = oXIApp.sType };
        }
        private void CreateOrganizationDatabase(string sDatabaseName)
        {
            string DBName = sDatabaseName;
            using (SqlConnection DBCon = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAAdminContext"].ConnectionString))
            {
                DBCon.Open();
                SqlCommand dbcmd = new SqlCommand();
                dbcmd.Connection = DBCon;
                string CreateDatabase = " CREATE DATABASE " + DBName;
                dbcmd.CommandText = CreateDatabase;
                dbcmd.ExecuteNonQuery();
                DBCon.Close();
            }
            //Create User For Database
            Server sqlInstance = new Server(
 new Microsoft.SqlServer.Management.Common.ServerConnection(
 new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAAdminContext"].ConnectionString))); //connects to the local server
            //initialize new object from database  adventureworks
            Database database = sqlInstance.Databases[DBName];
            //creates user RamyMahrous on database adventureworks
            string DataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
            if (DataBaseUser != null)
            {
                CreateDatabaseUser(database, DataBaseUser);
            }
        }

        private void CreateDatabaseUser(Database database, String username)
        {
            // initializes new User object and we say to which database it belongs
            // and its name
            Microsoft.SqlServer.Management.Smo.User sqlServerUser = new Microsoft.SqlServer.Management.Smo.User(database, username);
            sqlServerUser.UserType = UserType.SqlLogin; //SqlLogin not anything else
            //associated the user to login name, login name should be valid login name
            string DataBaseUser = ConfigurationManager.AppSettings["DataBaseUser"];
            sqlServerUser.Login = DataBaseUser;
            // here's we create the user on the database and till now the user
            // don't have any permission on database objects
            sqlServerUser.Create();
            //or any role like db_databasereader, db_databasewriter,...
            sqlServerUser.AddToRole("db_owner");
        }
        public int AddOrgRoles(int orgid)
        {
            cXIAppRoles group = new cXIAppRoles();
            group.iParentID = 2;
            group.sRoleName = "SuperAdmin";
            // group.Discriminator = "ApplicationRole";
            group.FKiOrganizationID = orgid;
            dbContext.XIAppRoles.Add(group);
            dbContext.SaveChanges();
            return group.RoleID;
        }

        public cXIApplications GetXIApplicationByID(int XIAppID)
        {
            cXIApplications oXIApp = new cXIApplications();
            oXIApp = dbContext.XIApplications.Find(XIAppID);
            return oXIApp;
        }
        public VMCustomResponse SaveXIAppLogo(cXIApplications model)
        {
            cXIApplications oXIApp = new cXIApplications();
            oXIApp = dbContext.XIApplications.Find(model.ID);
            oXIApp.sLogo = model.sLogo;
            dbContext.SaveChanges();
            return new VMCustomResponse() { ID = model.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        #region XIUrlMappings
        public DTResponse GetXIUrlMappingGrid(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            IQueryable<cXIUrlMappings> AllXIUrlMappings;
            AllXIUrlMappings = dbContext.XIUrlMappings.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXIUrlMappings = AllXIUrlMappings.Where(m => m.sUrlName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXIUrlMappings.Count();
            AllXIUrlMappings = QuerableUtil.GetResultsForDataTables(AllXIUrlMappings, "", sortExpression, param);
            var clients = AllXIUrlMappings.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.sType, c.sUrlName, c.sActualUrl, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public VMCustomResponse SaveXIUrlMappingDetails(cXIUrlMappings model, int iUserID, string sOrgName, string sDatabase)
        {
            cXIUrlMappings oXIUrls = new cXIUrlMappings();
            var FKiAppID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            if (model.ID == 0)
            {
                oXIUrls.FKiApplicationID = model.FKiApplicationID;
                oXIUrls.OrganisationID = model.OrganisationID;
                oXIUrls.sType = model.sType;
                oXIUrls.sUrlName = model.sUrlName;
                oXIUrls.sActualUrl = model.sActualUrl;
                oXIUrls.StatusTypeID = model.StatusTypeID;
                oXIUrls.CreatedBy = oXIUrls.UpdatedBy = model.CreatedBy;
                oXIUrls.CreatedTime = oXIUrls.UpdatedTime = DateTime.Now;
                oXIUrls.CreatedBySYSID = oXIUrls.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XIUrlMappings.Add(oXIUrls);
                dbContext.SaveChanges();
            }
            else
            {
                oXIUrls = dbContext.XIUrlMappings.Find(model.ID);
                oXIUrls.sType = model.sType;
                oXIUrls.FKiApplicationID = model.FKiApplicationID;
                oXIUrls.OrganisationID = model.OrganisationID;
                oXIUrls.sUrlName = model.sUrlName;
                oXIUrls.sActualUrl = model.sActualUrl;
                oXIUrls.StatusTypeID = model.StatusTypeID;
                oXIUrls.UpdatedBy = model.UpdatedBy;
                oXIUrls.UpdatedTime = DateTime.Now;
                oXIUrls.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oXIUrls.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }
        public cXIUrlMappings GetXIUrlMappingByID(int ID)
        {
            cXIUrlMappings oXIUrl = new cXIUrlMappings();
            oXIUrl = dbContext.XIUrlMappings.Find(ID);
            return oXIUrl;
        }
        public bool IsExistsUrlMappingName(string sUrlName, int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var AllUrls = dbContext.XIUrlMappings.ToList();
            cXIUrlMappings XIUrl = AllUrls.Where(m => m.sUrlName.Equals(sUrlName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (XIUrl != null)
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
                if (XIUrl != null)
                {
                    if (ID == XIUrl.ID)
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
        #endregion XIUrlMappings
    }
}
