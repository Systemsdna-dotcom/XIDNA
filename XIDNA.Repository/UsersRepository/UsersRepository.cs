using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.Repository;
using XIDNA.ViewModels;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Net;
using System.Data;

namespace XIDNA.Repository
{
    public class UsersRepository : IUsersRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        public DTResponse GetUsersList(jQueryDataTableParamModel param, int OrganizationID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            IQueryable<VMUsers> AllUsers;
            AllUsers = (from r in dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID)
                        join t in dbCore.XIAppUserRoles on r.UserID equals t.UserID
                        join s in dbCore.XIAppRoles on t.RoleID equals s.RoleID into q
                        from rt in q.DefaultIfEmpty()
                        select new VMUsers
                        {
                            Id = r.UserID,
                            OrganizationID = r.FKiOrganisationID,
                            FirstName = r.sFirstName,
                            LastName = r.sLastName,
                            UserName = r.sUserName,
                            UserID = t.UserID,
                            RoleId = t.RoleID,
                            Role = t.RoleID,
                            RoleName = rt.sRoleName,
                            PhoneNumber = r.sPhoneNumber
                        });
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllUsers = AllUsers.Where(m => m.UserName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllUsers.Count();
            AllUsers = QuerableUtil.GetResultsForDataTables(AllUsers, "", sortExpression, param);
            var clients = AllUsers.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.Id), c.FirstName,c.LastName, c.UserName, RoleName(c.Id,sDatabase), c.PhoneNumber, ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string RoleName(int p, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var roleid = dbCore.XIAppUserRoles.Where(m => m.UserID == p).Select(m => m.RoleID).FirstOrDefault();
            var rolename = dbCore.XIAppRoles.Where(m => m.RoleID == roleid).Select(m => m.sRoleName).FirstOrDefault();
            return rolename;
        }
        //public EditUserViewModel GetUser(int id)
        //{
        //    edituser model = dbContext.Users.Find(id);
        //    //Id = user.Id,
        //    //    Email = user.Email,
        //    //    FirstName = user.FirstName,
        //    //    LastName = user.LastName,
        //    //    GroupID = Role == null ? 0 : Role.Id,
        //    //    Group = dbContext.AspNetGroups.Where(m => m.Name == XIDNA.Common.Constants.SuperAdmin).OrderBy(m => m.Name).Take(1).ToList(),

        //    return model;
        //}
        public int UpdateUser(cXIAppUsers model, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            cXIAppUsers user = new cXIAppUsers();
            if (model.UserID == 0)
            {
                user.sUserName = model.sUserName;
                user.sPhoneNumber = model.sPhoneNumber;
                // user.Mobile = model.Mobile;
                user.sEmail = model.sEmail;
                dbCore.XIAppUsers.Add(user);
                dbCore.SaveChanges();
            }
            else
            {
                user = dbCore.XIAppUsers.Where(m => m.UserID == model.UserID).SingleOrDefault();
                user.sUserName = model.sUserName;
                user.sPhoneNumber = model.sPhoneNumber;
                // user.Mobile = model.Mobile;
                user.sEmail = model.sEmail;
                dbCore.SaveChanges();

            }
            return user.UserID;
        }
        public cXIAppRoles AddRole(cXIAppRoles model, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (model.RoleID > 0)
            {
                cXIAppRoles groups = new cXIAppRoles();
                groups = dbCore.XIAppRoles.Find(model.RoleID);
                groups.sRoleName = model.sRoleName;
                groups.iLayoutID = model.iLayoutID;
                if (model.iThemeID > 0)
                {
                    groups.iThemeID = model.iThemeID;
                }
                else
                {
                    groups.iThemeID = 0;
                }
                groups.UpdatedBy = 1;
                groups.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                groups.UpdatedTime = DateTime.Now;
                dbCore.SaveChanges();
                return groups;
            }
            else
            {
                cXIAppRoles groups = new cXIAppRoles();
                groups.FKiOrganizationID = model.FKiOrganizationID;
                groups.iParentID = model.iParentID;
                groups.sRoleName = model.sRoleName;
                //groups.sRoleName = Guid.NewGuid().ToString();
                groups.iLayoutID = model.iLayoutID;
                if (model.iThemeID > 0)
                {
                    groups.iThemeID = model.iThemeID;
                }
                else
                {
                    groups.iThemeID = 0;
                }
                groups.CreatedBy = 1;
                groups.CreatedTime = DateTime.Now;
                groups.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                groups.UpdatedBy = 1;
                groups.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                groups.UpdatedTime = DateTime.Now;
                dbCore.XIAppRoles.Add(groups);
                dbCore.SaveChanges();
                return groups;
            }

        }

        public DTResponse GetRolesList(jQueryDataTableParamModel param, int OrganizationID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            IQueryable<cXIAppRoles> AllRoles;
            AllRoles = dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrganizationID && m.iParentID != 2);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllRoles = AllRoles.Where(m => m.sRoleName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllRoles.Count();
            AllRoles = QuerableUtil.GetResultsForDataTables(AllRoles, "", sortExpression, param);
            var clients = AllRoles.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join l in dbContext.Layouts on c.iLayoutID equals l.ID
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.RoleID), c.sRoleName,c.groups.sRoleName, l.LayoutName,""};
            //displyCount = result.Count();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<VMDropDown> GetReportToUsers(string RoleID, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var AllRoles = RoleID.Split(',').ToList();
            List<VMDropDown> DropDown = new List<VMDropDown>();
            foreach (var items in AllRoles)
            {
                int RID = Convert.ToInt32(items);
                int ParentID = dbCore.XIAppRoles.Where(m => m.RoleID == RID).Select(m => m.iParentID).FirstOrDefault();
                List<cXIAppRoles> Role = dbCore.XIAppRoles.Where(m => m.RoleID == ParentID).ToList();
                foreach (var item in Role)
                {
                    var Exists = DropDown.Where(m => m.Value == item.RoleID).FirstOrDefault();
                    if (Exists == null)
                    {
                        DropDown.Add(new VMDropDown
                        {
                            Value = item.RoleID,
                            text = item.sRoleName
                        });
                    }
                }
            }
            return DropDown;
        }
        public List<VMDropDown> GetOrgLocation(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sCoreDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> list = new List<VMDropDown>();
            list = (from c in Spdb.OrganizationLocations.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList()
                    select new VMDropDown { Expression = c.LocationCode, text = c.Location }).ToList();
            return list;
        }

        public bool IsExistsRoleName(string RoleName, int Id, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var Roles = dbCore.XIAppRoles.ToList();
            cXIAppRoles Role = dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).Where(m => m.sRoleName.Equals(RoleName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (Id == 0)
            {
                if (Role != null)
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
                if (Role != null)
                {
                    if (Id == Role.RoleID)
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
        public bool IsExistsEmpEmail(string Email, string Type, int ID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var AspNetUsers = dbCore.XIAppUsers.ToList();
            if (Type == "Create")
            {
                cXIAppUsers user = AspNetUsers.Where(m => m.sEmail == Email).FirstOrDefault();
                //cXIAppUsers user = AspNetUsers.Where(m => m.sEmail.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
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
                cXIAppUsers user = AspNetUsers.Where(m => m.UserID == ID).FirstOrDefault();
                if (user != null)
                {
                    if (user.sEmail == Email)
                    {
                        return true;
                    }
                    else
                    {
                        cXIAppUsers useremail = AspNetUsers.Where(m => m.sEmail.Equals(Email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                        if (useremail != null)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        public int DeleteRole(int RoleID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<cXIAppUserRoles> users = new List<cXIAppUserRoles>();
            users = dbCore.XIAppUserRoles.Where(m => m.RoleID == RoleID).ToList();
            foreach (var items in users)
            {
                dbCore.XIAppUserRoles.Remove(items);
                dbCore.SaveChanges();
            }
            cXIAppRoles model = dbCore.XIAppRoles.Where(m => m.RoleID == RoleID && m.FKiOrganizationID == OrgID).FirstOrDefault();
            var entry = dbCore.Entry(model);
            if (entry.State == EntityState.Detached)
                dbCore.XIAppRoles.Attach(model);
            dbCore.XIAppRoles.Remove(model);
            dbCore.SaveChanges();

            return RoleID;
        }

        public bool GetNoOfUsers(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var users = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Count();
            var limit = dbCore.Organization.Where(m => m.ID == OrgID).Select(m => m.NoOfUsers).FirstOrDefault();
            if (limit == 0)
            {
                return true;
            }
            else if (limit > users)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public cXIAppRoles GetRoleByID(int ID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            cXIAppRoles Role = dbCore.XIAppRoles.Find(ID);
            return Role;
        }

        public cXIAppUsers GetUserDetails(int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            cXIAppUsers User = new cXIAppUsers();
            User = dbCore.XIAppUsers.Find(UserID);
            return User;
        }

        public List<VMDropDown> GetLayoutsList(string sDatabase)
        {
            List<VMDropDown> AllLayouts = new List<VMDropDown>();
            AllLayouts = dbContext.Layouts.Where(m => m.LayoutType.ToLower() == "inline").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.LayoutName }).ToList();
            return AllLayouts;
        }

        public List<VMDropDown> GetThemesList(string sDatabase)
        {
            List<VMDropDown> AllLayouts = new List<VMDropDown>();
            AllLayouts = dbContext.Types.Where(m => m.Name.ToLower() == "Themes").ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Expression }).ToList();
            return AllLayouts;
        }
    }
}
