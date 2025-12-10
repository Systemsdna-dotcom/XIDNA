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
namespace XIDNA.Repository
{
    public class DataRepository : IDataRepository
    {
        ModelDbContext dbcontext = new ModelDbContext();
        public List<VMLeadsList> GetLeadsList(int OrganizationID, string database)
        {
            var AllUsers = dbcontext.XIAppUsers.ToList();
            DataContext Spdb = new DataContext(database);
            List<VMLeadsList> UserLeads = new List<VMLeadsList>();
            UserLeads = Spdb.Database.SqlQuery<VMLeadsList>("Select ID, sForeName, sLastName, UserID, sMob, sEmail, FKiLeadClassID From " + EnumLeadTables.Leads.ToString() + " Where UserID != 0 and UserID is not null and FKiOrgID=" + OrganizationID).ToList();
            foreach (var items in UserLeads)
            {
                int UserID = items.UserID;
                int classid = items.FKiLeadClassID;
                items.sName = items.sForeName + " " + items.sLastName;
                items.Class = Spdb.OrganizationClasses.Where(m => m.ClassID == classid).Select(m => m.Class).FirstOrDefault();
                items.UserName = AllUsers.Where(m => m.UserID == UserID).Select(m => m.sFirstName).FirstOrDefault();
            }
            return UserLeads;
        }
        public List<int> GetSubRoles(int RoleID)
        {
            List<int> SubRolesIDs = new List<int>();
            cXIAppRoles Role = dbcontext.XIAppRoles.Where(s => s.RoleID == RoleID).FirstOrDefault();
            List<cXIAppRoles> Rolelist = Role.SubGroups.ToList();
            List<cXIAppRoles> SubRoles = new List<cXIAppRoles>();
            //SubCategoriesIDs.Add(RoleID);
            do
            {
                SubRoles.Clear();
                foreach (var item in Rolelist)
                {
                    if (item.IsLeaf)
                    {
                        SubRolesIDs.Add(item.RoleID);
                    }
                    else
                    {
                        SubRolesIDs.Add(item.RoleID);
                        SubRoles.AddRange(item.SubGroups);
                    }
                }
                Rolelist.Clear();
                Rolelist.AddRange(SubRoles);
            } while (Rolelist.Count != 0);

            return SubRolesIDs;
        }
        public List<UserLeads> GetAssignedLeadsList(int? UserID, int OrganizationID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var orgname = dbCore.Organization.Where(m => m.ID == OrganizationID).Select(m => m.Name).FirstOrDefault();
            var tablename = EnumLeadTables.Leads.ToString();
            List<UserLeads> AllLeads = new List<UserLeads>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sDatabase);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "Select ID, FirstName, StatusTypeID From " + tablename + " Where UserID = " + UserID;
                SqlDataReader reader = cmd.ExecuteReader();
                int i = 1;
                while (reader.Read())
                {
                    AllLeads.Add(new UserLeads
                    {
                        SNo = i,
                        ID = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        StatusTypeID = reader.GetInt32(2),
                    });
                    i++;
                }
                Con.Close();
            }
            return AllLeads;

        }
        public List<VMDropDown> DataClassList(string database, int OrgID)
        {
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            DataContext Spdb = new DataContext(database);
            AllClasses = (from c in Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList()
                          select new VMDropDown { text = c.Class, Value = c.ClassID }).ToList();
            return AllClasses;
        }
        public List<VMDropDown> SourceGroupList(string database)
        {
            DataContext Spdb = new DataContext(database);
            List<VMDropDown> list = new List<VMDropDown>();
            //var DataClassList = dbContext.DataSourceGroups.ToList();
            //foreach (var item in DataClassList)
            //{
            //    VMDropDown model = new VMDropDown();
            //    model.Value = item.id;
            //    model.text = item.sName;
            //    list.Add(model);
            //}
            return list;
        }
        public List<VMDropDown> SourceList(string database)
        {
            DataContext Spdb = new DataContext(database);
            List<VMDropDown> list = new List<VMDropDown>();
            //var DataClassList = dbContext.DataSources.ToList();
            //foreach (var item in DataClassList)
            //{
            //    VMDropDown model = new VMDropDown();
            //    model.Value = item.id;
            //    model.text = item.sName;
            //    list.Add(model);
            //}
            return list;
        }
        public int CreateData(List<FormData> Values, string sDatabase, int OrgID)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var db = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            LeadEngine Engine = new LeadEngine();
            VMLeadEngine result = Engine.GetLeadDetails(Values, OrgID);
            try
            {
                var InboundTable = EnumLeadTables.LeadInbounds.ToString();
                var InstanceTable = EnumLeadTables.LeadInstances.ToString();
                var ClientsTable = EnumLeadTables.LeadClients.ToString();
                var orgname = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.Name).FirstOrDefault();
                var LeadsTable = EnumLeadTables.Leads.ToString();
                string RawData = "";
                var ClientID = 0;
                foreach (var items in Values)
                {
                    RawData = RawData + items.Label + ":" + items.Value + ", ";
                }
                string Columns = "SourceID, ImportedOn, ImportedBy, Content, StatusTypeID";
                string Value = "" + result.SourceID + ", '" + DateTime.Now + "', 'Admin', '" + RawData + "', 10";
                using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = "INSERT INTO " + InboundTable + " (" + Columns + ") VALUES (" + Value + ")";
                    Con.Open();
                    Con.ChangeDatabase(db);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "Select ID From " + InboundTable + " Where Content = '" + RawData + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    int InboundID = 0;
                    while (reader.Read())
                    {
                        InboundID = reader.GetInt32(0);
                    }
                    reader.Close();
                    cmd.CommandText = "INSERT INTO " + InstanceTable + " (InBoundID) VALUES (" + InboundID + ")";
                    cmd.ExecuteNonQuery();
                    for (int i = 0; i < Values.Count(); i++)
                    {
                        if (Values[i].Value != null)
                        {
                            cmd.CommandText = "UPDATE " + InstanceTable + " SET " + Values[i].Label + "=" + "'" + Values[i].Value + "'" + " " + "WHERE" + " InBoundID=" + InboundID + "";
                            cmd.ExecuteNonQuery();
                        }
                    }
                    if (result.ClientID == 0)
                    {
                        cmd.CommandText = "INSERT INTO " + ClientsTable + " (Name, ClassID, Email, Mobile) VALUES ('" + Values[0].Value + " " + Values[1].Value + "'," + result.ClassID + ",'" + Values[8].Value + "','" + Values[6].Value + "'); SELECT SCOPE_IDENTITY()";
                        var client = cmd.ExecuteScalar();
                        ClientID = Convert.ToInt32(client);
                    }
                    //for (int i = 0; i < Values.Count(); i++)
                    //{
                    //    cmd.CommandText = "UPDATE " + InstanceTable + " SET " + Values[i].Label + "=" + "'" + Values[i].Value + "'" + " " + "WHERE" + " InBoundID=" + InboundID + "";
                    //    cmd.ExecuteNonQuery();
                    //}
                    Con.Dispose();
                }

                if (result.bInsertLead)
                {
                    Values.Add(new FormData
                    {
                        Label = "dImportedOn",
                        Value = DateTime.Now.ToString()
                    });
                    Values.Add(new FormData
                    {
                        Label = "FKiOrgID",
                        Value = OrgID.ToString()
                    });
                    Values.Add(new FormData
                    {
                        Label = "FKiClientID",
                        Value = ClientID.ToString()
                    });
                    using (SqlConnection MyCon = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmmd = new SqlCommand("", MyCon);
                        cmmd.CommandText = "INSERT INTO " + LeadsTable + " (sName) VALUES ('" + Values[0].Value + " " + Values[1].Value + "'); SELECT SCOPE_IDENTITY()";
                        MyCon.Open();
                        MyCon.ChangeDatabase(db);
                        var latestid = cmmd.ExecuteScalar();
                        for (int i = 0; i < Values.Count(); i++)
                        {
                            if (Values[i].Value != null)
                            {
                                cmmd.CommandText = "UPDATE " + LeadsTable + " SET " + Values[i].Label + "=" + "'" + Values[i].Value + "'" + " " + "WHERE" + " id=" + latestid + "";
                                cmmd.ExecuteNonQuery();
                            }
                        }
                        MyCon.Dispose();
                    }
                }
                else
                {
                    using (SqlConnection Conn = new SqlConnection(ServiceUtil.GetClientConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Conn;
                        Conn.Open();
                        Conn.ChangeDatabase(db);
                        for (int i = 0; i < Values.Count(); i++)
                        {
                            cmd.CommandText = "UPDATE " + LeadsTable + " SET " + Values[i].Label + "=" + "'" + Values[i].Value + "'" + " " + "WHERE" + " ID=" + result.LeadID + "";
                            cmd.ExecuteNonQuery();
                        }
                        if (result.bUpdateSource)
                        {
                            cmd.CommandText = "UPDATE " + LeadsTable + " SET FKiSourceID =" + "'" + result.SourceID + "'" + " " + "WHERE" + " ID=" + result.LeadID + "";
                            cmd.ExecuteNonQuery();
                        }
                        else
                        {
                            cmd.CommandText = "UPDATE " + LeadsTable + " SET FKiSourceID =" + "'" + result.OldSourceID + "'" + " " + "WHERE" + " ID=" + result.LeadID + "";
                            cmd.ExecuteNonQuery();
                        }
                        Conn.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
        public List<VMDropDown> GetUsersbyOrgID(int OrganizationID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            List<VMDropDown> AllTeams = new List<VMDropDown>();
            string db = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(db);
            //var Teams = Spdb.OrganizationTeams.Where(m => m.OrganizationID == OrganizationID).ToList();
            //foreach (var items in Teams)
            //{
            //    AllTeams.Add(new VMDropDown
            //    {
            //        text = items.Name,
            //        Value = items.ID
            //    });
            //}
            //return AllTeams;
            List<VMDropDown> Leads1 = new List<VMDropDown>();
            List<cXIAppUsers> Leads = new List<cXIAppUsers>();
            Leads1 = (from c in dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).ToList()
                      select new VMDropDown { Value = c.UserID, text = c.sFirstName }).ToList();
            return Leads1;
        }

        public List<UserLeads> GetLeadsbyOrgID(int TeamID, int? ClassID, int OrganizationID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var Org = dbCore.Organization.Where(m => m.ID == OrganizationID).FirstOrDefault();
            DataContext Spdb = new DataContext(Org.DatabaseName);
            var AllLeads = Spdb.Database.SqlQuery<VMLeads>("Select ID, sForeName, sLastName, sMob, sEmail From " + EnumLeadTables.Leads.ToString() + " Where FKiLeadClassID = " + ClassID + " AND FKiOrgID=" + OrganizationID + " AND (UserID =0 OR UserID IS NULL)").ToList();
            List<UserLeads> Leads = new List<UserLeads>();
            Leads = AllLeads.Select(m => new UserLeads
            {
                ID = m.ID,
                FirstName = m.sForeName == null ? null : m.sForeName + " " + (m.sLastName == null ? null : m.sLastName) + "-" + (m.sMob == null ? null : m.sMob) + "-" + (m.sEmail == null ? null : m.sEmail),
            }).ToList();
            return Leads;
        }
        public int AssignUsersToLeads(List<int> selectedleads, int userid, string database, int orgid)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var Org = dbCore.Organization.Where(m => m.ID == orgid).FirstOrDefault();
            DataContext Spdb = new DataContext(Org.DatabaseName);
            foreach (var item in selectedleads)
            {
                Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set UserID = " + userid + " Where ID =" + item);
            }
            return userid;
        }
        public List<Classes> GetClasses(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            List<Classes> Classes = new List<Classes>();
            var classnames = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList();
            foreach (var items in classnames)
            {
                Classes.Add(new Classes
                {
                    text = items.Class,
                    value = items.ClassID
                });
            }
            return Classes;
        }

        public int ImportLeads(int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var db = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            var orgname = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.Name).FirstOrDefault();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(db);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                var tablename = EnumLeadTables.Leads.ToString();
                cmd.CommandText = "truncate table " + tablename;
                cmd.ExecuteNonQuery();
                cmd.CommandText = "Insert into " + tablename + " Select * from Leads";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "Update " + tablename + " Set UserID = 0";
                cmd.ExecuteNonQuery();
                Con.Close();
            }
            return 0;
        }

        private List<UserLeads> AutoAssignLeads(int TeamID, int? ClassID, int OrganizationID, string database)
        {
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                Con.ChangeDatabase(database);
                SqlDataReader reader = cmd.ExecuteReader();
                cmd.CommandText = "Select ID From " + EnumLeadTables.Leads.ToString() + " Where iTeamID = " + TeamID + " AND (UserID =0 OR UserID IS NULL)";
                List<int> LeadIDs = new List<int>();
                while (reader.Read())
                {
                    LeadIDs.Add(reader.GetInt32(0));
                }
                reader.Close();
                DataContext Spdb = new DataContext(database);
                int j = 0, LastAssignedEmployee = 0;
                var UserIDs = Spdb.OrganizationTeams.Where(m => m.ID == TeamID).Select(m => m.UserIDs).FirstOrDefault().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                for (int i = 0; i < LeadIDs.Count(); i++)
                {
                    int UserID = Convert.ToInt32(UserIDs[j]);

                    if (i == LeadIDs.Count() - 1)
                    {
                        LastAssignedEmployee = UserID;
                    }
                    cmd.CommandText = "update " + EnumLeadTables.Leads.ToString() + " set userid =" + UserID + " where id=" + LeadIDs[i];
                    cmd.ExecuteNonQuery();
                    if (j == UserIDs.Count() - 1)
                    {
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                }
                Con.Close();
            }
            return null;
        }
    }
}
