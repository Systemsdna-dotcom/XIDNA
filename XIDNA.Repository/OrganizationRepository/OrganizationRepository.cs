using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.SqlClient;
using XIDNA.ViewModels;
using System.Text.RegularExpressions;
using System.Configuration;
//using System.Data.Entity;
//using Microsoft.SqlServer.Smo;
using Microsoft.SqlServer.Management.Smo;

namespace XIDNA.Repository
{
    public class OrganizationRepository : IOrganizationRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        #region OrgCreation

        public DTResponse GetOrganizationList(jQueryDataTableParamModel param, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            IQueryable<Organizations> AllOrgs;
            AllOrgs = dbCore.Organization;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllOrgs = AllOrgs.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllOrgs.Count();
            AllOrgs = QuerableUtil.GetResultsForDataTables(AllOrgs, "", sortExpression, param);
            var clients = AllOrgs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.Name, c.PhoneNumber, c.Email, c.StatusTypeID.ToString(),"Actions"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public int AddOrganization(VMOrganization model, string CommonDB, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            Organizations org = new Organizations();
            if (model.ID == 0)
            {
                //Create Organization
                org.Name = model.Name;
                org.TypeID = model.TypeID;
                org.Description = model.Description;
                org.NoOfUsers = model.NoOfUsers;
                org.DatabaseType = model.DatabaseType;
                org.Address = model.Address;
                if (model.Logo == null)
                    org.Logo = null;
                else
                    org.Logo = model.Logo;
                org.PostCode = model.PostCode;
                org.Email = model.Email;
                org.PhoneNumber = model.PhoneNumber;
                org.Website = model.Website;
                org.Password = model.Password;
                org.StatusTypeID = model.StatusTypeID;
                org.StatusTypeID = 10;
                org.CreatedByID = org.ModifiedByID = 1;
                org.CreatedByName = org.ModifiedByName = model.Email;
                org.CreatedBySYSID = "1";
                org.CreatedTime = org.ModifiedTime = DateTime.Now;
                if (model.DatabaseType != "Specific")
                {
                    org.DatabaseName = CommonDB;
                }
                var ThemeName = ConfigurationManager.AppSettings["ThemeFile"].ToString();
                var ThemeID = dbContext.Types.Where(m => m.FileName == ThemeName).Select(m => m.ID).FirstOrDefault();
                org.ThemeID = ThemeID;
                dbCore.Organization.Add(org);
                dbCore.SaveChanges();
                string DBName = "XIDNAOrg" + org.ID;
                if (model.DatabaseType == "Specific")
                {
                    var Org = dbContext.Organization.Find(org.ID);
                    org.DatabaseName = DBName;
                    dbContext.SaveChanges();
                    //Create Organization DataBase
                    CreateOrganizationDatabase(org.ID);
                    //create Organization tables 
                    //CreateOrganizationTables(org.ID, DBName, model.Name, model.DatabaseType, database);
                }
            }
            else
            {
                org = dbCore.Organization.Where(m => m.ID == model.ID).SingleOrDefault();
                org.Name = model.Name;
                org.TypeID = model.TypeID;
                org.Description = model.Description;
                org.NoOfUsers = model.NoOfUsers;
                org.Address = model.Address;
                org.PostCode = model.PostCode;
                org.Email = model.Email;
                org.PhoneNumber = model.PhoneNumber;
                org.Website = model.Website;
                org.StatusTypeID = model.StatusTypeID;
                org.ModifiedByID = 1;
                org.ModifiedTime = DateTime.Now;
                org.ModifiedByName = model.Email;
                dbCore.SaveChanges();
            }
            return org.ID;
        }
        public OrganizationSubscriptions GetOrgSubscriptionDetails(int ID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);

            OrganizationSubscriptions GetOrgDetails = new OrganizationSubscriptions();
            GetOrgDetails = Spdb.OrganizationSubscriptions.Find(ID);
            return GetOrgDetails;
        }

        private void CreateOrganizationTables(int OrgID, string DBName, string OrgName, string DatabaseType, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            List<OrganizationSetup> Setup = dbContext.OrganizationSetups.ToList();
            List<string> Tables = Setup.Select(m => m.Name).ToList();
            List<string> ColumTypes = Setup.Select(m => m.Columns).ToList();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                if (DatabaseType == "Specific")
                {
                    Con.ChangeDatabase(DBName);
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                string CreateTable = "";
                for (int i = 0; i < Tables.Count(); i++)
                {
                    CreateTable = "CREATE TABLE " + Tables[i] + " (" + ColumTypes[i] + ")";
                    cmd.CommandText = CreateTable;
                    cmd.ExecuteNonQuery();
                }
                Con.Close();
            }
        }

        private void CreateOrganizationDatabase(int OrgID)
        {
            string DBName = "XIDNAOrg" + OrgID;
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
            CreateDatabaseUser(database, DataBaseUser);
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
        public int AddOrgRoles(int orgid, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            cXIAppRoles group = new cXIAppRoles();
            group.sRoleName = Guid.NewGuid().ToString();
            group.iParentID = 2;
            group.sRoleName = "Admin";
            // group.Discriminator = "ApplicationRole";
            group.FKiOrganizationID = orgid;
            dbCore.XIAppRoles.Add(group);
            dbCore.SaveChanges();
            return group.RoleID;
        }

        public VMOrganization GetOrganizationDetails(int orgid, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            VMOrganization model = new VMOrganization();
            OrganizationContacts orgcon = new OrganizationContacts();
            List<OrganizationClasses> Orgclasses = new List<OrganizationClasses>();
            Organizations org = dbCore.Organization.Find(orgid);
            model.ID = org.ID;
            model.Name = org.Name;
            model.Email = org.Email;
            model.TypeID = org.TypeID;
            model.Description = org.Description;
            model.Logo = org.Logo;
            model.NoOfUsers = org.NoOfUsers;
            model.Address = org.Address;
            model.PhoneNumber = org.PhoneNumber;
            model.PostCode = org.PostCode;
            model.StatusTypeID = org.StatusTypeID;
            model.Website = org.Website;
            model.OldEmail = org.Email;
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == org.ID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            orgcon = Spdb.OrganizationContacts.Where(m => m.OrganizationID == orgid).SingleOrDefault();
            if (orgcon != null)
            {
                model.ConID = orgcon.ID;
                model.ConAddress = orgcon.Address;
                model.ConPhone = orgcon.Phone;
                model.ConEmail = orgcon.Email;
                model.ConOrganizationID = orgid;
                model.ConName = orgcon.Name;
            }
            model.OrganizationID = org.ID;
            model.ConOrganizationID = org.ID;
            Orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == org.ID).ToList();
            List<string> Classes = new List<string>();
            foreach (var items in Orgclasses)
            {
                Classes.Add(items.ClassID + "-" + items.Class);
            }
            model.ClassIDs = Classes;
            model.Classes = GetClasses(database);
            List<string> Locations = new List<string>();
            Locations = Spdb.OrganizationLocations.Where(m => m.OrganizationID == org.ID).Select(m => m.Location).ToList();
            if (Locations != null)
            {
                model.OrgLocations = Locations;
            }
            return model;
        }
        public int AddOrganizationContact(VMOrganization model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.ConOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationContacts org = new OrganizationContacts();
            try
            {
                if (model.ConID == 0)
                {
                    org.Name = model.ConName;
                    org.OrganizationID = model.ConOrganizationID;
                    org.Phone = model.ConPhone;
                    org.Email = model.ConEmail;
                    org.Address = model.ConAddress;
                    org.CreatedByID = org.ModifiedByID = 1;
                    org.CreatedByName = org.ModifiedByName = "Admin";
                    org.CreatedBySYSID = "1";
                    org.CreatedTime = org.ModifiedTime = DateTime.Now;
                    Spdb.OrganizationContacts.Add(org);
                    Spdb.SaveChanges();
                }
                else
                {
                    org = Spdb.OrganizationContacts.Where(m => m.ID == model.ConID).SingleOrDefault();
                    org.Name = model.ConName;
                    org.OrganizationID = model.ConOrganizationID;
                    org.Phone = model.ConPhone;
                    org.Email = model.ConEmail;
                    org.Address = model.ConAddress;
                    org.CreatedTime = org.ModifiedTime = DateTime.Now;
                    Spdb.SaveChanges();
                }
            }
            catch (Exception ex)
            {
            }
            return org.OrganizationID;
        }
        public int UpdateOrganizationContact(OrganizationContacts model, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            OrganizationContacts org = new OrganizationContacts();
            DataContext Spdb = new DataContext(sOrgDB);
            if (model.ID == 0)
            {
                org.Name = model.Name;
                org.OrganizationID = model.OrganizationID;
                org.Phone = model.Phone;
                org.Email = model.Email;
                org.Address = model.Address;
                org.CreatedByID = org.ModifiedByID = 1;
                org.CreatedByName = org.ModifiedByName = "Admin";
                org.CreatedBySYSID = "1";
                org.CreatedTime = org.ModifiedTime = DateTime.Now;
                Spdb.OrganizationContacts.Add(org);
                Spdb.SaveChanges();
            }
            else
            {
                org = Spdb.OrganizationContacts.Where(m => m.ID == model.ID).SingleOrDefault();
                org.Name = model.Name;
                org.OrganizationID = model.OrganizationID;
                org.Phone = model.Phone;
                org.Email = model.Email;
                org.Address = model.Address;
                org.CreatedTime = org.ModifiedTime = DateTime.Now;
                Spdb.SaveChanges();
            }
            return org.OrganizationID;
        }
        public VMOrganization GetOrgContactDetails(int organizationid, int iUserID, string sOrgName, string sDatabase)
        {
            OrganizationContacts model = new OrganizationContacts();
            VMOrganization orgcon = new VMOrganization();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            model = Spdb.OrganizationContacts.Where(m => m.OrganizationID == organizationid).SingleOrDefault();
            orgcon.ConID = model.ID;
            orgcon.ConName = model.Name;
            orgcon.ConEmail = model.Email;
            orgcon.ConPhone = model.Phone;
            orgcon.ConAddress = model.Address;
            orgcon.ConOrganizationID = model.OrganizationID;
            return orgcon;
        }
        public int SaveLogo(VMOrganization model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            if (model.ID > 0)
            {
                Organizations org = dbCore.Organization.Find(model.ID);
                org.Logo = model.Logo;
                dbCore.SaveChanges();
            }
            return model.ID;
        }
        public bool IsExistsOrgName(string Name, string Type, int ID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var Organization = dbCore.Organization.ToList();
            Organizations org = Organization.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (org != null)
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
                if (org != null)
                {
                    if (ID == org.ID)
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
        public bool IsExistsOrgEmail(string Email, string Type, int ID, string OldEmail, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var Organization = dbCore.Organization.ToList();
            if (Type == "Create")
            {
                var user = dbCore.XIAppUsers.Where(m => m.sEmail == Email).FirstOrDefault();
                if (user != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
                //var orgs = dbContext.Organization.ToList();
                //Organizations org = Organization.Where(m => m.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //if (org != null)
                //{
                //    orgexists = true;
                //}
                //else
                //{
                //    orgexists = false;
                //}
                //if (userexists || orgexists)
                //{
                //    return false;
                //}
                //else
                //{
                //    return true;
                //}
            }
            else
            {
                if (OldEmail == Email)
                {
                    return true;
                }
                else
                {
                    var user = dbCore.XIAppUsers.Where(m => m.sEmail == Email).FirstOrDefault();
                    if (user != null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }

                //Organizations org = Organization.Where(m => m.ID == ID).FirstOrDefault();
                //if (org != null)
                //{
                //    if (org.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase))
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        Organizations orgemail = Organization.Where(m => m.Email.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                //        if (orgemail != null)
                //        {
                //            return false;
                //        }
                //        else
                //        {
                //            return true;
                //        }
                //    }
                //}
                //else
                //{
                //    return true;
                //}
            }
        }
        public int GetUserID(VMOrganization model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            int userid = dbCore.XIAppUsers.Where(m => m.sEmail == model.OldEmail).Select(m => m.UserID).FirstOrDefault();
            return userid;
        }
        public List<VMDropDown> GetOrgNames(string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var OrgDetails = new List<VMDropDown>();
            OrgDetails = (from c in dbCore.Organization.Where(m => m.StatusTypeID == 10).ToList()
                          select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return OrgDetails;
        }

        #endregion OrgCreation

        #region OrgLocations
        public int SaveOrgDetails(VMOrganization model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations Orgloc = new OrganizationLocations();
            Orgloc.OrganizationID = model.LocOrganizationID;
            Orgloc.Location = model.Location;
            Orgloc.LocationCode = model.LocationCode;
            Orgloc.StatusTypeID = model.LocStatusTypeID;
            Orgloc.CreatedByID = 1;
            Orgloc.CreatedByName = "Admin";
            Orgloc.CreatedBySYSID = "1";
            Orgloc.CreatedTime = DateTime.Now;
            Orgloc.ModifiedByID = 1;
            Orgloc.ModifiedByName = "Admin";
            Orgloc.ModifiedBySYSID = "1";
            Orgloc.ModifiedTime = DateTime.Now;
            Spdb.OrganizationLocations.Add(Orgloc);
            Spdb.SaveChanges();
            return model.OrganizationID;
        }
        public VMCustomResponse SaveOrgLocDetails(OrganizationLocations model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations Orgloc = new OrganizationLocations();
            if (model.ID == 0)
            {
                Orgloc.OrganizationID = model.OrganizationID;
                Orgloc.Location = model.Location;
                Orgloc.LocationCode = model.LocationCode;
                Orgloc.StatusTypeID = model.StatusTypeID;
                Orgloc.CreatedByID = 1;
                Orgloc.CreatedByName = "Admin";
                Orgloc.CreatedBySYSID = "1";
                Orgloc.CreatedTime = DateTime.Now;
                Orgloc.ModifiedByID = 1;
                Orgloc.ModifiedByName = "Admin";
                Orgloc.ModifiedBySYSID = "1";
                Orgloc.ModifiedTime = DateTime.Now;
                Spdb.OrganizationLocations.Add(Orgloc);
                Spdb.SaveChanges();
            }
            else
            {
                Orgloc = Spdb.OrganizationLocations.Find(model.ID);
                Orgloc.Location = model.Location;
                Orgloc.LocationCode = model.LocationCode;
                Orgloc.StatusTypeID = model.StatusTypeID;
                Orgloc.ModifiedTime = DateTime.Now;
                Spdb.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = model.OrganizationID, Status = true };
        }

        private string GetOrgName(int p, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var orgname = dbCore.Organization.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return orgname;
        }

        public DTResponse DisplayOrganizations(jQueryDataTableParamModel param, int LocOrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            IQueryable<OrganizationLocations> AllOrgLocs;
            List<OrganizationLocations> AllLocs = new List<OrganizationLocations>();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            if (LocOrgID == 0)
            {
                //AllLocs = Spdb.OrganizationLocations.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in dbs)
                {
                    if (items == sOrgDB)
                    {
                        AllLocs.AddRange(Spdb.OrganizationLocations);
                    }
                }
                AllOrgLocs = AllLocs.AsQueryable();
            }
            else
            {
                AllOrgLocs = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllOrgLocs = AllOrgLocs.Where(m => m.Location.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllOrgLocs.Count();
            AllOrgLocs = QuerableUtil.GetResultsForDataTables(AllOrgLocs, "", sortExpression, param);
            var clients = AllOrgLocs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(), c.Location, c.LocationCode, GetOrgName(c.OrganizationID,sDatabase), c.StatusTypeID.ToString(), ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }


        public OrganizationLocations EditOrgLocation(int ColumnID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations OrgDet = Spdb.OrganizationLocations.Find(ColumnID);
            var OrgDetails = new List<VMDropDown>();
            Organizations Org = new Organizations();
            var Details = dbCore.Organization.ToList();
            foreach (var item in Details)
            {
                OrgDetails.Add(new VMDropDown
                {
                    text = item.Name,
                    Value = item.ID
                }

                    );
            }
            OrgDet.OrgDetails = OrgDetails;
            return OrgDet;
        }

        public OrganizationLocations DeleteOrgLocation(int ColumnID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations OrgDet = Spdb.OrganizationLocations.Find(ColumnID);
            Spdb.OrganizationLocations.Remove(OrgDet);
            Spdb.SaveChanges();
            return OrgDet;
        }

        public bool IsExistsOrgLocation(string Location, int OrganizationID, int ID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations orgloc = Spdb.OrganizationLocations.Where(m => m.Location.Equals(Location, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.OrganizationID == OrganizationID).FirstOrDefault();
            if (ID == 0)
            {
                if (orgloc != null)
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
                if (orgloc != null)
                {
                    if (ID == orgloc.ID)
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

        public bool IsExistsLocationCode(string LocationCode, int OrganizationID, int ID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationLocations orgloc = Spdb.OrganizationLocations.Where(m => m.LocationCode.Equals(LocationCode, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.OrganizationID == OrganizationID).FirstOrDefault();
            if (ID == 0)
            {
                if (orgloc != null)
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
                if (orgloc != null)
                {
                    if (ID == orgloc.ID)
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
        //LocationGrid

        public DTResponse SpecificOrgLocationGrid(jQueryDataTableParamModel param, int LocOrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == LocOrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationLocations> AllLocs, FilteredLocs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredLocs = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID && m.Location.Contains(param.sSearch)).ToList();
                AllLocs = FilteredLocs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredLocs.Count();

            }
            else
            {
                displyCount = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID).Count();
                AllLocs = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllLocs
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID),c.Location,c.LocationCode,c.StatusTypeID.ToString()}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse SpecificOrgLocationList(jQueryDataTableParamModel param, int LocOrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == LocOrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationLocations> AllLoc, FilteredLocs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredLocs = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID).ToList();
                AllLoc = FilteredLocs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredLocs.Count();

            }
            else
            {
                displyCount = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID).Count();
                AllLoc = Spdb.OrganizationLocations.Where(m => m.OrganizationID == LocOrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllLoc
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID),c.Location,c.LocationCode, c.StatusTypeID.ToString()}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<VMDropDown> GetClasses(string database)
        {
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            Common Com = new Common();
            AllClasses = (from c in dbContext.Types.Where(m => m.Name == "Class Type").ToList()
                          select new VMDropDown { Expression = c.ID + "-" + c.Expression, text = c.Expression }).ToList();
            //AllClasses = Com.GetOrgClasses(0);
            return AllClasses;
        }
        public int AddOrganizationClasses(VMOrganization model, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            List<OrganizationClasses> orgclasses = new List<OrganizationClasses>();
            orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == model.OrganizationID).ToList();
            if (orgclasses.Count() > 0)
            {
                OrganizationClasses org = new OrganizationClasses();
                foreach (var items in orgclasses)
                {
                    org = Spdb.OrganizationClasses.Find(items.ID);
                    Spdb.OrganizationClasses.Remove(org);
                }

            }
            foreach (var classes in model.ClassIDs)
            {
                OrganizationClasses org = new OrganizationClasses();
                var details = classes.Split('-').ToList();
                org.Class = details[1];
                org.ClassID = Convert.ToInt32(details[0]);
                org.OrganizationID = model.OrganizationID;
                Spdb.OrganizationClasses.Add(org);
                Spdb.SaveChanges();
            }
            return model.OrganizationID;
        }
        public VMOrganization GetOrganizationClasses(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<OrganizationClasses> Orgclasses = new List<OrganizationClasses>();
            Orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
            List<string> Classes = new List<string>();
            foreach (var items in Orgclasses)
            {
                Classes.Add(items.ClassID + "-" + items.Class);
            }
            VMOrganization model = new VMOrganization();
            model.ClassIDs = Classes;
            model.Classes = GetClasses(sDatabase);
            return model;
        }
        public List<VMDropDown> GetLocationCodesList(int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> AllLocCodes = new List<VMDropDown>();
            var LocCodes = (from c in Spdb.OrganizationLocations.Where(m => m.OrganizationID == OrgID).ToList()
                            select new VMDropDown { text = c.LocationCode, Expression = c.LocationCode }).ToList();
            AllLocCodes.AddRange(LocCodes);
            return AllLocCodes;
        }
        #endregion OrgLocations

        #region OrgSources       

        public DTResponse GetOrgSources(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var sOrgDB = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            DataContext Spdb = new DataContext(sOrgDb);
            IQueryable<OrganizationSources> AllSources;
            List<OrganizationSources> AllSrcs = new List<OrganizationSources>();
            if (OrgID == 0)
            {
                //AllTypes = Spdb.OrganizationSources.OrderBy(m => m.Name).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in sOrgDB)
                {
                    DataContext OrgDb = new DataContext(items);
                    AllSrcs.AddRange(OrgDb.OrganizationSources.ToList());
                }
                AllSources = AllSrcs.AsQueryable();
            }
            else
            {
                AllSources = Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSources = AllSources.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSources.Count();
            AllSources = QuerableUtil.GetResultsForDataTables(AllSources, "", sortExpression, param);
            var clients = AllSources.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), c.OrganizationID.ToString(), GetOrgName(c.OrganizationID,sDatabase), c.Icon, c.Name, GetType(c.Type,sDatabase), GetProvider(c.Provider,sDatabase),c.EmailID,c.MobileNumber, c.StatusTypeID.ToString(),"" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetType(int p, string database)
        {
            var typename = dbContext.Types.Where(m => m.ID == p).Select(m => m.Expression).FirstOrDefault();
            return typename;
        }

        private string GetProvider(int p, string database)
        {
            var typename = dbContext.Types.Where(m => m.ID == p).Select(m => m.Expression).FirstOrDefault();
            return typename;
        }
        public VMCustomResponse SaveSource(VMOrganizationForms Source, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == Source.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            if (Source.ID == 0)
            {
                OrganizationSources model = new OrganizationSources();
                model.Name = Source.Name;
                model.OrganizationID = Source.OrganizationID;
                model.Type = Source.Type;
                model.Provider = Source.Provider;
                model.Icon = Source.Icon;
                model.EmailID = Source.EmailID;
                model.MobileNumber = Source.MobileNumber;
                model.StatusTypeID = Source.StatusTypeID;
                Spdb.OrganizationSources.Add(model);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Source.OrganizationID, Status = true };
            }
            else
            {
                OrganizationSources model = Spdb.OrganizationSources.Find(Source.ID);
                model.Name = Source.Name;
                model.Type = Source.Type;
                model.Provider = Source.Provider;
                model.EmailID = Source.EmailID;
                model.MobileNumber = Source.MobileNumber;
                if (Source.Icon == null)
                    model.Icon = model.Icon;
                else
                    model.Icon = Source.Icon;
                model.StatusTypeID = Source.StatusTypeID;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = model.OrganizationID, Status = true };
            }
        }

        public VMOrganizationForms EditOrgSource(int SourceID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationSources source = Spdb.OrganizationSources.Find(SourceID);
            VMOrganizationForms sor = new VMOrganizationForms();
            sor.ID = source.ID;
            sor.Name = source.Name;
            sor.StatusTypeID = source.StatusTypeID;
            sor.Provider = source.Provider;
            sor.Type = source.Type;
            sor.Icon = source.Icon;
            sor.EmailID = source.EmailID;
            sor.MobileNumber = source.MobileNumber;
            return sor;
        }
        public bool IsExistsSourceName(string Name, int ID, int OrganizationID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationSources orgsrc = Spdb.OrganizationSources.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).Where(m => m.OrganizationID == OrganizationID).FirstOrDefault();
            if (ID == 0)
            {
                if (orgsrc != null)
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
                if (orgsrc != null)
                {
                    if (ID == orgsrc.ID)
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
        public bool IsExistsSrcEmail(string Email, string Type, int ID, string OldEmail, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var Organization = dbCore.Organization.ToList();
            if (Type == "Create")
            {
                var user = dbCore.XIAppUsers.Where(m => m.sEmail == Email).FirstOrDefault();
                if (user != null)
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
                if (OldEmail == Email)
                {
                    return true;
                }
                else
                {
                    var user = dbCore.XIAppUsers.Where(m => m.sEmail == Email).FirstOrDefault();
                    if (user != null)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
        }
        public DTResponse SpecificSourcesList(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationSources> AllSrcs, FilteredSrcs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredSrcs = Spdb.OrganizationSources.Where(m => m.Name.Contains(param.sSearch.ToUpper())).ToList();
                AllSrcs = FilteredSrcs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredSrcs.Count();
            }
            else
            {
                displyCount = Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID).Count();
                AllSrcs = Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllSrcs
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Icon, c.Name, GetType(c.Type,database), GetProvider(c.Provider,database), c.EmailID,c.MobileNumber}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        #endregion OrgSources

        #region OrgSubscriptions        

        public DTResponse GetOrgSubscriptions(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var sOrgDB = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            DataContext Spdb = new DataContext(sOrgDb);
            IQueryable<OrganizationSubscriptions> AllSubs;
            List<OrganizationSubscriptions> SpecificSubs = new List<OrganizationSubscriptions>();
            if (OrgID == 0)
            {
                //AllTypes = Spdb.OrganizationSources.OrderBy(m => m.Name).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                foreach (var items in sOrgDB)
                {
                    DataContext AllDb = new DataContext(items);
                    SpecificSubs.AddRange(AllDb.OrganizationSubscriptions.ToList());
                }
                AllSubs = SpecificSubs.AsQueryable();

            }
            else
            {
                AllSubs = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID);
            }
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllSubs = AllSubs.Where(m => m.SubscriptionID.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllSubs.Count();
            AllSubs = QuerableUtil.GetResultsForDataTables(AllSubs, "", sortExpression, param);
            var clients = AllSubs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), c.OrganizationID.ToString(), c.SubscriptionID.ToString(), GetOrgName(c.OrganizationID,sDatabase), c.LocationCode, GetSourceName(c.SourceID, c.OrganizationID,sDatabase), GetClassName(c.ClassID,c.OrganizationID,sDatabase), c.LeadCost.ToString(),c.Email,c.PostCode,c.RenewalDate.ToString(),c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetSourceName(int p, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            string source = Spdb.OrganizationSources.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return source;
        }

        private string GetClassName(int p, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            string classname = Spdb.OrganizationClasses.Where(m => m.ClassID == p).Select(m => m.Class).FirstOrDefault();
            return classname;
        }

        public List<VMDropDown> GetOrgSources(int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> AllSources = new List<VMDropDown>();
            AllSources = (from c in Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList()
                          select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return AllSources;
        }
        public List<VMDropDown> GetOrgClasses(int OrgID, string database)
        {
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            AllClasses = ServiceUtil.GetOrgClasses(OrgID, database);
            return AllClasses;
        }

        public List<VMDropDown> GetSourceTypes(int SourceID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var orgsource = Spdb.OrganizationSources.Where(m => m.ID == SourceID).Select(m => m.Type).FirstOrDefault();
            //var types = orgsource.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
            List<VMDropDown> AllTypes = new List<VMDropDown>();
            //foreach (var items in types)
            //{
            //    AllTypes.Add(new VMDropDown
            //    {
            //        text = items
            //    });
            //}
            return AllTypes;
        }

        public List<VMDropDown> GetSourceClasses(int SourceID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var orgclasses = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).Select(m => m.ClassID).ToList();
            var existingclass = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.SourceID == SourceID).Select(m => m.ClassID).ToList();
            var remainingclass = orgclasses.Except(existingclass).ToList();
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            foreach (var items in remainingclass)
            {
                var classes = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID && m.ClassID == items).FirstOrDefault();
                AllClasses.Add(new VMDropDown
                {
                    text = classes.Class,
                    Value = classes.ClassID
                });
            }
            return AllClasses;
        }
        public List<VMDropDown> GetProviders(string database)
        {
            List<VMDropDown> AllProviders = new List<VMDropDown>();
            AllProviders = (from c in dbContext.Types.Where(m => m.Name == "Provider Type").ToList()
                            select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            return AllProviders;
        }
        public List<VMDropDown> GetSourceTypes(string database)
        {
            List<VMDropDown> AllSourceTypes = new List<VMDropDown>();
            AllSourceTypes = (from c in dbContext.Types.Where(m => m.Name == "Source Type").ToList()
                              select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            return AllSourceTypes;
        }

        public VMCustomResponse SaveSubscription(VMOrganizationForms model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            if (model.ID == 0)
            {
                if (model.CopySubID > 0)
                {
                    var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.SubOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(sOrgDB);
                    OrganizationSubscriptions sub = new OrganizationSubscriptions();
                    sub = Spdb.OrganizationSubscriptions.Find(model.CopySubID);
                    sub.OrganizationID = model.SubOrganizationID;
                    sub.SourceID = model.SourceID;
                    sub.LocationCode = model.LocationCode;
                    sub.ClassID = model.ClassID;
                    sub.SubscriptionID = RandomString(8, database);
                    sub.LeadCost = model.LeadCost;
                    sub.Email = model.Email;
                    sub.PostCode = model.PostCode;
                    sub.RenewalDate = model.RenewalDate;
                    sub.StatusTypeID = model.SubStatusTypeID;
                    Spdb.OrganizationSubscriptions.Add(sub);
                    Spdb.SaveChanges();
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = sub.SubscriptionID + "-" + sub.OrganizationID, Status = true };
                }
                else
                {
                    OrganizationSubscriptions sub = new OrganizationSubscriptions();
                    var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.SubOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(sOrgDB);
                    sub.OrganizationID = model.SubOrganizationID;
                    sub.SourceID = model.SourceID;
                    sub.ClassID = model.ClassID;
                    sub.LeadCost = model.LeadCost;
                    sub.Email = model.Email;
                    sub.PostCode = model.PostCode;
                    sub.LocationCode = model.LocationCode;
                    sub.RenewalDate = model.RenewalDate;
                    sub.SubscriptionID = RandomString(8, database);
                    sub.StatusTypeID = model.SubStatusTypeID;
                    Spdb.OrganizationSubscriptions.Add(sub);
                    Spdb.SaveChanges();
                    return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = sub.SubscriptionID + "-" + sub.OrganizationID, Status = true };
                }
            }
            else
            {
                var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.SubOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
                DataContext Spdb = new DataContext(sOrgDB);
                OrganizationSubscriptions sub = new OrganizationSubscriptions();
                sub = Spdb.OrganizationSubscriptions.Find(model.ID);
                sub.OrganizationID = model.SubOrganizationID;
                //sub.SourceID = model.SourceID;
                //sub.ClassID = model.ClassID;
                sub.LeadCost = model.LeadCost;
                sub.Email = model.Email;
                sub.PostCode = model.PostCode;
                sub.RenewalDate = model.RenewalDate;
                sub.StatusTypeID = model.SubStatusTypeID;
                sub.LocationCode = model.LocationCode;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = sub.SubscriptionID + "-" + sub.OrganizationID, Status = true };
            }

        }
        private static Random random = new Random();
        public static string RandomString(int length, string database)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string subid = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            DataContext Spdb = new DataContext(database);
            OrganizationSubscriptions sub = new OrganizationSubscriptions();
            sub = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == subid).FirstOrDefault();
            if (sub == null)
            {
                return subid;
            }
            return RandomString(8, database);
        }
        public DTResponse SpecificSubscriptionsList(jQueryDataTableParamModel param, int OrgID, int SourceID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationSubscriptions> AllSubs, FilteredSubs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredSubs = Spdb.OrganizationSubscriptions.ToList();
                AllSubs = FilteredSubs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredSubs.Count();

            }
            else
            {
                if (SourceID > 0)
                {
                    displyCount = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.SourceID == SourceID).Count();
                    AllSubs = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.SourceID == SourceID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }
                else
                {
                    displyCount = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Count();
                    AllSubs = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                }

            }
            var result = (from c in AllSubs
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID.ToString(), GetOrgName(c.OrganizationID,database), c.LocationCode, GetSourceName(c.SourceID, c.OrganizationID,database), GetClassName(c.ClassID, c.OrganizationID,database), c.LeadCost.ToString(),c.Email,c.PostCode,c.RenewalDate.ToString()}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<VMDropDown> GetSubscriptions(string database, int OrgID)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            List<OrganizationSubscriptions> list = new List<OrganizationSubscriptions>();
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            if (OrgID == 0)
            {
                AllClasses = (from s in Spdb.OrganizationSubscriptions.Where(m => m.StatusTypeID == 10)
                              join r in Spdb.OrganizationSources on s.SourceID equals r.ID
                              join c in Spdb.OrganizationClasses on s.ClassID equals c.ClassID
                              select new VMDropDown { text = r.Name + "-" + c.Class, Expression = s.SubscriptionID }).ToList();
            }
            else
            {
                AllClasses = (from s in Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Where(m => m.StatusTypeID == 10)
                              join r in Spdb.OrganizationSources on s.SourceID equals r.ID
                              join c in Spdb.OrganizationClasses on s.ClassID equals c.ClassID
                              select new VMDropDown { text = r.Name + "-" + c.Class, Expression = s.SubscriptionID }).ToList();
            }
            return AllClasses;
        }
        #endregion OrgSubscriptions

        #region OrgSourceFields
        public string AddSourceFields(VMOrganizationForms model, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.SorFieldOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);

            var field = model.FieldName.Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();
            foreach (var items in field)
            {
                if (items != "")
                {
                    OrganizationSourceFields fields = new OrganizationSourceFields();
                    fields.FieldName = items;
                    fields.FieldType = "varchar(32)";
                    fields.SubscriptionID = model.SorFieldSubscriptionID;
                    Spdb.OrganizationSourceFields.Add(fields);
                    Spdb.SaveChanges();
                }
            }
            return model.SorFieldSubscriptionID;

        }
        public DTResponse GetOrgSourceFields(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationSourceFields> AllTypes, FilteredTypes;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredTypes = Spdb.OrganizationSourceFields.ToList();
                AllTypes = FilteredTypes.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredTypes.Count();

            }
            else
            {
                if (OrgID == 0)
                {
                    AllTypes = (from c in Spdb.OrganizationSourceFields
                                select c).ToList();
                }
                else
                {
                    AllTypes = (from c in Spdb.OrganizationSourceFields
                                join s in Spdb.OrganizationSubscriptions on c.SubscriptionID equals s.SubscriptionID
                                where s.OrganizationID == OrgID
                                select c).ToList();
                }
                displyCount = AllTypes.Count();
                AllTypes = AllTypes.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                //AllTypes = Spdb.OrganizationSourceFields.OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllTypes
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID,  c.FieldName,""}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }


        public int DeleteSourceField(int ID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var res = Spdb.OrganizationSourceFields.Find(ID);
            Spdb.OrganizationSourceFields.Remove(res);
            Spdb.SaveChanges();
            return 0;
        }
        public DTResponse GetOrgSpecSourceFields(jQueryDataTableParamModel param, int OrgID, string SubID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<OrganizationSourceFields> AllTypes, FilteredTypes;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredTypes = Spdb.OrganizationSourceFields.ToList();
                AllTypes = FilteredTypes.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredTypes.Count();

            }
            else
            {
                if (OrgID == 0)
                {
                    AllTypes = (from c in Spdb.OrganizationSourceFields
                                where c.SubscriptionID == SubID
                                select c).ToList();
                }
                else
                {
                    AllTypes = (from c in Spdb.OrganizationSourceFields
                                where c.SubscriptionID == SubID
                                join s in Spdb.OrganizationSubscriptions on c.SubscriptionID equals s.SubscriptionID
                                where s.OrganizationID == OrgID
                                select c).ToList();
                }
                displyCount = AllTypes.Count();
                AllTypes = AllTypes.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                //AllTypes = Spdb.OrganizationSourceFields.OrderBy(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllTypes
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID,  c.FieldName,""}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        #endregion OrgSourceFields

        #region OrgImages
        //saving of Logo

        public int SaveOtherImages(OrganizationImages model, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationImages OI = new OrganizationImages();
            if (model.ID == 0)
            {
                OI.OrganizationID = model.OrganizationID;
                OI.FileName = model.FileName;
                Spdb.OrganizationImages.Add(OI);
                Spdb.SaveChanges();
                return OI.ID;
            }
            else
            {
                OI = Spdb.OrganizationImages.Find(model.ID);
                OI.OrganizationID = model.OrganizationID;
                OI.FileName = model.FileName;
                Spdb.SaveChanges();
                return OI.ID;
            }


        }
        //Getting the Logo

        public List<OrganizationImages> GetOrgImages(int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var SDetails = Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).ToList();
            return SDetails;
        }

        //Deleting the Logo
        public OrganizationImages DeleteLogo(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationImages OI = Spdb.OrganizationImages.Find(ID);
            Spdb.OrganizationImages.Remove(OI);
            Spdb.SaveChanges();
            return OI;
        }
        // For Displaying the Images Grid
        public DTResponse OrganizationImagesGrid(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IList<OrganizationImages> AllImages, FilteredImages;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredImages = Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).Where(m => m.FileName.Contains(param.sSearch.ToUpper())).ToList();
                AllImages = FilteredImages.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredImages.Count();
            }
            else
            {
                displyCount = Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).OrderBy(m => m.FileName).Count();
                AllImages = Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = from c in AllImages
                         select new[] {
                             (i++).ToString(),Convert.ToString(c.ID), c.FileName ,""};
            var OrgLogo = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.Logo).FirstOrDefault();
            var NewList = result.ToList();
            if (OrgLogo != null)
            {
                List<string> Logo = new List<string>();
                Logo.Add((i++).ToString());
                Logo.Add(Convert.ToString(0));
                Logo.Add(OrgLogo);
                Logo.Add("");
                NewList.Insert(0, Logo.ToArray());
            }
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = NewList
            };
        }

        #endregion OrgImages        

        #region OrgTeams
        public VMCustomResponse SaveOrganizationTeams(OrganizationTeams model, string[] SelectedUsers, string[] UserIDs, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationTeams OT = new OrganizationTeams();
            if (model.ID == 0)
            {
                OT.OrganizationID = OrgID;
                OT.Name = model.Name;
                var UserNames = string.Join(",", SelectedUsers);
                OT.Users = UserNames;
                var IDs = string.Join(",", UserIDs);
                OT.UserIDs = IDs;
                OT.StatusTypeID = model.StatusTypeID;
                Spdb.OrganizationTeams.Add(OT);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = OT.ID, Status = true };
            }
            else
            {
                OT = Spdb.OrganizationTeams.Find(model.ID);
                OT.OrganizationID = OrgID;
                OT.Name = model.Name;
                var result = string.Join(",", SelectedUsers);
                OT.Users = result;
                var Ids = string.Join(",", UserIDs);
                OT.UserIDs = Ids;
                OT.StatusTypeID = model.StatusTypeID;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = OT.ID, Status = true };
            }
        }
        public OrganizationTeams GetUsersList(int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            OrganizationTeams AllUsers = new OrganizationTeams();
            var Users = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).ToList();
            var lUsersList = new List<string>();
            var lUsersIds = new List<string>();
            foreach (var items in Users)
            {
                lUsersList.Add(items.sFirstName);
                lUsersIds.Add(items.UserID.ToString());
            }
            AllUsers.aUsers = lUsersList;
            AllUsers.aUserIDs = lUsersIds;
            AllUsers.sUsers = new List<string>();
            AllUsers.sUserIDs = new List<string>();
            return AllUsers;
            //var Users = dbContext.AspNetGroups.Where(m => m.OrganizationID == OrgID).ToList();
            //List<VMOrganizationTeams> AllUsers = new List<VMOrganizationTeams>();
            //foreach (var items in Users)
            //{
            //    var lUsersList = new List<string>();
            //    var UserIDs = new List<int>();
            //    VMOrganizationTeams Team = new VMOrganizationTeams();
            //    Team.Role = items.RoleName;
            //    var RoleUsers = dbContext.AspNetUserGroups.Where(m => m.RoleId == items.Id).ToList();
            //    foreach (var item in RoleUsers)
            //    {
            //        var UserName = dbContext.AspNetUsers.Where(m => m.Id == item.UserId).FirstOrDefault();
            //        lUsersList.Add(UserName.FirstName);
            //        UserIDs.Add(UserName.Id);
            //    }
            //    Team.Users = lUsersList;
            //    Team.UserIDs = UserIDs;
            //    AllUsers.Add(Team);
            //}
            //return AllUsers;
        }

        public DTResponse OrgTeamsGrid(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IQueryable<OrganizationTeams> AllTeams;
            AllTeams = Spdb.OrganizationTeams.Where(m => m.OrganizationID == OrgID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTeams = AllTeams.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTeams.Count();
            AllTeams = QuerableUtil.GetResultsForDataTables(AllTeams, "", sortExpression, param);
            var clients = AllTeams.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), c.Name, c.Users, c.StatusTypeID.ToString(), "Edit"};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public OrganizationTeams EditOrgTeams(int ID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            OrganizationTeams OT = new OrganizationTeams();
            OT = Spdb.OrganizationTeams.Find(ID);
            if (OT.Users != "")
            {
                List<string> ids = OT.UserIDs.Split(',').ToList();
                List<string> Names = new List<string>();
                OT.sUserIDs = ids;
                foreach (var items in ids)
                {
                    int UserID = Convert.ToInt32(items);
                    var Name = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sFirstName).FirstOrDefault();
                    Names.Add(Name);
                }
                OT.sUsers = Names;
            }
            var Users = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).ToList();
            var lUsersList = new List<string>();
            var lUserIds = new List<string>();
            foreach (var items in Users)
            {
                lUsersList.Add(items.sFirstName);
                lUserIds.Add(items.UserID.ToString());
            }
            OT.aUsers = lUsersList;
            OT.aUserIDs = lUserIds;
            var RemainingUsers = OT.aUsers.Except(OT.sUsers).ToList();
            var RemainingIds = OT.aUserIDs.Except(OT.sUserIDs).ToList();
            OT.aUsers = RemainingUsers;
            OT.aUserIDs = RemainingIds;
            return OT;
        }
        public bool IsExistsTeamName(string Name, int ID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var OrgTeams = Spdb.OrganizationTeams.ToList();
            OrganizationTeams OT = OrgTeams.Where(m => m.OrganizationID == OrgID).Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (OT != null)
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
                if (OT != null)
                {
                    if (ID == OT.ID)
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

        #endregion OrgImages

        #region OrgSubColumns

        public VMCustomResponse SaveSubscriptionColumns(VMOrganizationForms model, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.SubOrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            var Field = model.FieldName.Split('-')[0];
            var OrgCol = Spdb.SubscriptionColumns.Where(m => m.OrganizationID == model.SubOrganizationID && m.SubscriptionID == model.SubsriptionID && m.FieldName == Field).FirstOrDefault();
            if (OrgCol == null)
            {
                SubscriptionColumns SubCol = new SubscriptionColumns();
                SubCol.OrganizationID = model.OrganizationID;
                SubCol.SubscriptionID = model.SubsriptionID;
                SubCol.FieldName = model.FieldName.Split('-')[0];
                SubCol.FieldValue = model.FieldValue;
                SubCol.OrganizationID = model.SubOrganizationID;
                Spdb.SubscriptionColumns.Add(SubCol);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = SubCol.SubscriptionID + "-" + SubCol.OrganizationID, Status = true };
            }
            else
            {
                OrgCol.FieldValue = model.FieldValue;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = OrgCol.SubscriptionID + "-" + OrgCol.OrganizationID, Status = true };
            }
        }
        public SubscriptionColumns OrgColumnDetails(string Name, string SubscriptionID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            SubscriptionColumns sub = new SubscriptionColumns();
            sub = Spdb.SubscriptionColumns.Where(m => m.FieldName == Name && m.SubscriptionID == SubscriptionID).FirstOrDefault();
            return sub;
        }

        public DTResponse SpecificSubCoulmnsList(jQueryDataTableParamModel param, string SubID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var sOrgDB = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<SubscriptionColumns> AllSubs, FilteredSubs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredSubs = Spdb.SubscriptionColumns.ToList();
                AllSubs = FilteredSubs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredSubs.Count();
            }
            else
            {
                displyCount = Spdb.SubscriptionColumns.Where(m => m.OrganizationID == OrgID && m.SubscriptionID == SubID).Count();
                AllSubs = Spdb.SubscriptionColumns.Where(m => m.OrganizationID == OrgID && m.SubscriptionID == SubID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = (from c in AllSubs
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID.ToString(), GetOrgName(c.OrganizationID,database), c.FieldName,c.FieldValue}).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        public List<VMDropDown> GetColumnsList(string database)
        {
            List<VMDropDown> AllColumns = new List<VMDropDown>();
            var ColumnsList = dbContext.BOGroupFields.Where(m => m.GroupName == "Mapping Group").FirstOrDefault();
            if (ColumnsList != null)
            {
                int BOID = ColumnsList.BOID;
                var MappingColumns = ColumnsList.BOFieldNames;
                if (MappingColumns != null)
                {
                    var ColumnsLists = MappingColumns.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                    foreach (string items in ColumnsLists)
                    {
                        int TypeID = dbContext.BOFields.Where(m => m.Name == items && m.BOID == BOID).Select(m => m.TypeID).FirstOrDefault();
                        string type = ((BODatatypes)TypeID).ToString();
                        AllColumns.Add(new VMDropDown
                        {
                            text = items,
                            Expression = items + "-" + type
                        });
                    }
                }
            }
            return AllColumns;
        }
        #endregion OrgSubColumns

        public int ChangeTheme(int ID, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var OrgDetails = dbCore.Organization.Find(OrgID);
            OrgDetails.ThemeID = ID;
            dbCore.SaveChanges();
            return OrgID;
        }

    }
}
