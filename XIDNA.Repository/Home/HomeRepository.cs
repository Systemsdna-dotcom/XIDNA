using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;


namespace XIDNA.Repository
{
    public class HomeRepository : IHomeRepository
    {
        CommonRepository Common = new CommonRepository();
        public DTResponse GetMenuDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            IQueryable<RightMenuTrees> AllMenu;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            AllMenu = dbContext.RightMenuTrees.Where(m => m.FKiApplicationID == fkiApplicationID || m.FKiApplicationID == 0).Where(m => m.ParentID == "#" && (m.StatusTypeID == 0 || m.StatusTypeID == 10));
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllMenu = AllMenu.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllMenu.Count();
            AllMenu = QuerableUtil.GetResultsForDataTables(AllMenu, "", sortExpression, param);
            var Groups = AllMenu.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in Groups
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.RootName,GetRoleName(c.RoleID,sDatabase),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }
        private string GetRoleName(int RoleID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var rolename = dbCore.XIAppRoles.Where(m => m.RoleID == RoleID).Select(m => m.sRoleName).FirstOrDefault();
            return rolename;
        }
        public RightMenuTrees EditRootMenu(int ID, int OrgID, int UserID, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(UserID, null, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            RightMenuTrees rghtMenudetails = new RightMenuTrees();
            List<RightMenuTrees> rghtMenuTree = new List<RightMenuTrees>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            int iRoleID = dbContext.RightMenuTrees.Where(m => m.ID == ID).Select(m => m.RoleID).FirstOrDefault();
            string RootNode = dbContext.RightMenuTrees.Where(m => m.ID == ID).Select(m => m.RootName).FirstOrDefault();
            var lID = new List<int>();
            lID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RootName == RootNode).Select(m => m.ID).ToList();
            for (var i = 0; i < lID.Count(); i++)
            {
                int DelID = lID[i];
                RightMenuTrees GetDetails = dbContext.RightMenuTrees.Find(DelID);
                rghtMenuTree.Add(GetDetails);
            }
            rghtMenudetails.OrgID = OrgID;
            rghtMenudetails.RoleID = iRoleID;
            rghtMenudetails.Name = dbContext.RightMenuTrees.Where(m => m.ID == ID).Select(m => m.Name).FirstOrDefault();
            List<VMDropDown> Roles = new List<VMDropDown>();
            Roles = (from c in dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).ToList()
                     select new VMDropDown { text = c.sRoleName, Value = c.RoleID }).ToList();
            rghtMenudetails.Roles = Roles;
            List<VMDropDown> Organisations = new List<VMDropDown>();
            if (UserDetails.sRoleName != EnumRoles.XISuperAdmin.ToString())
            {
                Organisations = (from c in dbCore.Organization.ToList()
                                 select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            }
            rghtMenudetails.Organisations = Organisations;
            rghtMenudetails.RootName = RootNode;
            return rghtMenudetails;
        }


        public int DeleteRootMenu(int ID, int OrgID, int UserID, string sDatabase)
        {
            int iStatus = 0;
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                ModelDbContext dbCore = new ModelDbContext(sDatabase);
                List<RightMenuTrees> LMenuTrees = new List<RightMenuTrees>();
                int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
                RightMenuTrees UpNodes = dbContext.RightMenuTrees.Find(ID);
                UpNodes.StatusTypeID = 20;
                dbContext.SaveChanges();
                //var oMenuList = dbContext.RightMenuTrees.Where(m => m.ID == ID).ToList();
                //string MenuName = oMenuList.Select(m => m.Name).FirstOrDefault();
                //int iRoleID = oMenuList.Select(m => m.RoleID).FirstOrDefault();
                //int iOrgID = oMenuList.Select(m => m.OrgID).FirstOrDefault();                
                //var lID = new List<int>();
                //lID = dbContext.RightMenuTrees.Where(m => m.OrgID == iOrgID).Where(m => m.RoleID == iRoleID).Where(m => m.RootName == MenuName).Select(m => m.ID).ToList();
                //for (var i = 0; i < lID.Count(); i++)
                //{
                //    int DelID = lID[i];
                //    RightMenuTrees DelNodes = dbContext.RightMenuTrees.Find(DelID);
                //    dbContext.RightMenuTrees.Remove(DelNodes);
                //    dbContext.SaveChanges();
                //}
                iStatus = 1;
            }
            catch (Exception ex)
            {

            }
            return iStatus;
        }
        public int SaveMenuTreeDetails(int MenuID, string RootNode, string ParentNode, string NodeID, string NodeTitle, string Type, int iRoleID, int UserID, int OrgID, string sDatabase, string sOrgName)
        {
            int ID = 0;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            RightMenuTrees MenuTree = new RightMenuTrees();
            int Status = 0;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            var fkiApplicationID = Common.GetUserDetails(UserID, sOrgName, sDatabase).FKiApplicationID;
            RightMenuTrees CheckMenu = dbContext.RightMenuTrees.Where(m => m.ParentID == ParentNode).Where(m => m.RootName == RootNode).Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).FirstOrDefault();
            if (Type == "create")
            {
                if (MenuID > 0)
                {
                    var Menu = dbContext.RightMenuTrees.Find(MenuID);
                    if (ParentNode == null)
                    {
                        var iMainMenuID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == iRoleID).Where(m => m.ParentID == "#").Select(m => m.MenuID).FirstOrDefault();
                        Menu.ParentID = iMainMenuID;
                    }
                    else
                    {
                        Menu.ParentID = ParentNode;
                    }
                    var NewMenuID = MenuID;
                    var ListOfMenu = dbContext.RightMenuTrees.Where(m => m.ParentID == Menu.ParentID).Select(m => m.Priority).ToList();
                    if (ListOfMenu.Count() > 0)
                    {
                        var MaxPriority = ListOfMenu.Max();
                        Menu.Priority = MaxPriority + 1;
                    }
                    else
                    {
                        Menu.Priority = 1;
                    }
                    dbContext.SaveChanges();
                    return MenuID;
                }
                if (CheckMenu == null)
                {
                    MenuTree.RootName = RootNode;
                    //MenuTree.MenuID = NodeID;
                    MenuTree.MenuID = NodeID;
                    MenuTree.Name = NodeTitle;
                    MenuTree.ParentID = ParentNode;
                    MenuTree.RoleID = iRoleID;
                    MenuTree.OrgID = OrgID;
                    MenuTree.Priority = 1;
                    // if validation is added then this throws error
                    //MenuTree.MenuController = null;
                    //MenuTree.MenuAction = null;
                    //MenuTree.XiLinkID = 0;
                    //MenuTree.ActionType = 0;
                    MenuTree.StatusTypeID = 10;
                    MenuTree.FKiApplicationID = fkiApplicationID;
                    MenuTree.CreatedBy = MenuTree.UpdatedBy = UserID;
                    MenuTree.CreatedTime = MenuTree.UpdatedTime = DateTime.Now;
                    MenuTree.CreatedBySYSID = MenuTree.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    dbContext.RightMenuTrees.Add(MenuTree);
                    dbContext.SaveChanges();
                    Status = 1;
                    ID = MenuTree.ID;
                    if (Status == 1)
                    {
                        RightMenuTrees UpdateMenuTree = dbContext.RightMenuTrees.Find(ID);
                        UpdateMenuTree.MenuID = (ID).ToString();
                        dbContext.SaveChanges();
                    }
                }
            }
            else if (Type == "rename")
            {
                ID = Convert.ToInt32(NodeID);
                RightMenuTrees EditMenuTree = dbContext.RightMenuTrees.Find(ID);
                EditMenuTree.Name = NodeTitle;
                dbContext.SaveChanges();
                Status = 1;
            }
            return ID;
        }
        public List<RightMenuTrees> DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type, int iRoleID, int UserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            RightMenuTrees MenuTree = new RightMenuTrees();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            //var AllIDs = new List<int>();
            //AllIDs= dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).Select(m=>m.ID).ToList();
            //Delete Main node.
            int MainID = dbContext.RightMenuTrees.Where(m => m.RoleID == iRoleID).Where(m => m.OrgID == OrgID).Where(m => m.MenuID == NodeID).Select(m => m.ID).FirstOrDefault();
            RightMenuTrees DelMainNode = dbContext.RightMenuTrees.Find(MainID);
            if (DelMainNode != null)
            {
                dbContext.RightMenuTrees.Remove(DelMainNode);
                dbContext.SaveChanges();
            }
            //Delete Chidrens
            if (ChildrnIDs != "")
            {
                List<string> TargetdIDs = ChildrnIDs.Split(',').ToList();
                for (var i = 0; i < TargetdIDs.Count(); i++)
                {
                    string TrgID = TargetdIDs[i];
                    int IID = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).Where(m => m.MenuID == TrgID).Select(m => m.ID).FirstOrDefault();
                    RightMenuTrees DelNode = dbContext.RightMenuTrees.Find(IID);
                    dbContext.RightMenuTrees.Remove(DelNode);
                    dbContext.SaveChanges();
                }
            }
            //return the updated values
            List<RightMenuTrees> lRightMenu = new List<RightMenuTrees>();
            lRightMenu = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).ToList();
            return lRightMenu;
        }

        public RightMenuTrees AddDetailsForMenu(string ParentNode, string NodeID, int iRoleID, int OrgID, int UserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            RightMenuTrees MenuTree = new RightMenuTrees();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            List<VMDropDown> XILink = new List<VMDropDown>();
            //XILink = (from c in dbContext.XiLinks.ToList()
            //          select new VMDropDown { text = c.Name, Value = c.XiLinkID }).ToList();
            MenuTree = dbContext.RightMenuTrees.Where(m => m.RoleID == iRoleID).Where(m => m.OrgID == OrgID).Where(m => m.MenuID == NodeID).Where(m => m.ParentID == ParentNode).FirstOrDefault();
            MenuTree.XiLinkName = dbContext.XiLinks.Where(m => m.XiLinkID == MenuTree.XiLinkID).Select(m => m.Name).FirstOrDefault();
            //MenuTree.VMXILink = XILink;
            //List<VMDropDown> MenuGroup = new List<VMDropDown>();
            //MenuGroup = (from c in dbContext.MenuGroups
            //             select new VMDropDown { text = c.sName, Value = c.ID }).ToList();

            //MenuTree.MenuGroup = MenuGroup;
            return MenuTree;
        }

        //public RightMenuTrees AddDetailsForMenu(string ParentNode, string NodeID, int OrgID, int UserID, string sDatabase)
        //{
        //    ModeldbContext dbContext = new ModeldbContext();
        //    RightMenuTrees MenuTree = new RightMenuTrees();
        //    int RoleID = dbContext.AspNetUserGroups.Where(m => m.UserId == UserID).Select(m => m.RoleId).FirstOrDefault();
        //    List<VMDropDown> XILink = new List<VMDropDown>();
        //    XILink = (from c in dbContext.XiLinks
        //              select new VMDropDown { text = c.Name, Value = c.XiLinkID }).ToList();
        //    MenuTree = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).Where(m => m.MenuID == NodeID).Where(m => m.ParentID == ParentNode).FirstOrDefault();
        //    MenuTree.VMXILink = XILink;
        //    List<VMDropDown> MenuGroup = new List<VMDropDown>();
        //    MenuGroup = (from c in dbContext.MenuGroups
        //                 select new VMDropDown { text = c.sName, Value = c.ID }).ToList();

        //    MenuTree.MenuGroup = MenuGroup;
        //    return MenuTree;
        //}

        public VMCustomResponse SaveAddedDetails(int UserID, RightMenuTrees model, string sDatabase)
        {
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                RightMenuTrees EditMenuTree = dbContext.RightMenuTrees.Find(model.ID);
                EditMenuTree.Name = model.Name;
                if (model.ActionType == 10)
                {
                    EditMenuTree.ActionType = 10;
                    EditMenuTree.MenuController = model.MenuController;
                    EditMenuTree.MenuAction = model.MenuAction;
                    EditMenuTree.XiLinkID = 0;
                    EditMenuTree.StatusTypeID = model.StatusTypeID;
                    EditMenuTree.UpdatedBy = UserID;
                    EditMenuTree.UpdatedTime = DateTime.Now;
                    EditMenuTree.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                }
                else if (model.ActionType == 20)
                {
                    EditMenuTree.ActionType = 20;
                    //string GetUrl = dbContext.XiLinks.Where(m => m.XiLinkID==model.XiLinkID).Select(m => m.URL).FirstOrDefault();
                    //string[] SplitGetUrl = GetUrl.Split('/');
                    //string sControllerName = SplitGetUrl[0];
                    //string sActionName = SplitGetUrl[1];

                    //If validation is alloved this throws error.
                    EditMenuTree.MenuController = "";
                    EditMenuTree.MenuAction = "";
                    EditMenuTree.XiLinkID = model.XiLinkID;
                    EditMenuTree.StatusTypeID = model.StatusTypeID;
                    EditMenuTree.UpdatedBy = UserID;
                    EditMenuTree.UpdatedTime = DateTime.Now;
                    EditMenuTree.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                }
                else
                {
                    EditMenuTree.StatusTypeID = model.StatusTypeID;
                    EditMenuTree.UpdatedBy = UserID;
                    EditMenuTree.UpdatedTime = DateTime.Now;
                    EditMenuTree.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    EditMenuTree.ActionType = model.ActionType;
                    EditMenuTree.XiLinkID = model.XiLinkID;
                }
                dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            return new VMCustomResponse() { Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        //public List<RightMenuTrees> ShowMenuTreeDetails(int UserID, int OrgID, string sDatabase)
        //{
        //    ModeldbContext dbContext = new ModeldbContext();
        //    List<RightMenuTrees> lMenuTree = new List<RightMenuTrees>();
        //    int RoleID = dbContext.AspNetUserGroups.Where(m => m.UserId == UserID).Select(m => m.RoleId).FirstOrDefault();
        //    string MainID = dbContext.RightMenuTrees.Where(m => m.ParentID == "#" && m.RoleID == RoleID && (m.RootName.ToLower() == "HomeMenu".ToLower() || m.RootName.ToLower()=="home menu".ToLower())).Select(m => m.MenuID).FirstOrDefault();
        //    lMenuTree = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID && m.OrgID == OrgID && m.ParentID == MainID && m.StatusTypeID==10).OrderBy(m => m.Priority).ToList();
        //    return lMenuTree;
        //}

        public List<RightMenuTrees> ShowMenuTreeDetails(int UserID, int OrgID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<RightMenuTrees> lMenuTree = new List<RightMenuTrees>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            var FKiAppID = Common.GetUserDetails(UserID, sOrgName, sDatabase).FKiApplicationID;
            //string MainID = dbContext.RightMenuTrees.where(m => m.).Where(m => m.ParentID == "#" && m.RoleID == RoleID && (m.RootName.ToLower() == "HomeMenu".ToLower() || m.RootName.ToLower() == "home menu".ToLower())).Select(m => m.MenuID).FirstOrDefault();
            string MainID = dbContext.RightMenuTrees.Where(m => m.ParentID == "#").Where(m => m.OrgID == OrgID).Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.RoleID == RoleID).Where(m => m.RootName.ToLower() == "HomeMenu".ToLower() || m.RootName.ToLower() == "home menu".ToLower()).Select(m => m.MenuID).FirstOrDefault();
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
                items.SubGroups = dbContext.RightMenuTrees.Where(m => m.ParentID == ID && m.StatusTypeID == 10).OrderBy(m => m.Priority).ToList();
                if (items.SubGroups.Count() > 0)
                {
                    Countdata(items.SubGroups, sDatabase);
                }
            }
            return Menus;
        }
        public int DragAndDropNodes(string NodeID, string OldParentID, string NewParentID, int UserID, int OrgID, string sDatabase, int Oldposition, int Newposition)
        {
            int TabID = 0;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            int iGetID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).Where(m => m.ParentID == OldParentID).Where(m => m.MenuID == NodeID).Select(m => m.ID).FirstOrDefault();
            RightMenuTrees DAndDMenuTree = dbContext.RightMenuTrees.Find(iGetID);
            int NoOfPostionsChanged = Oldposition - Newposition;
            if (DAndDMenuTree != null)
            {
                if (DAndDMenuTree.ID == 0)
                {
                    RightMenuTrees tab = new RightMenuTrees();
                    tab.MenuID = DAndDMenuTree.MenuID;
                    tab.ParentID = DAndDMenuTree.ParentID;
                    tab.OrgID = DAndDMenuTree.OrgID;
                    tab.RoleID = DAndDMenuTree.RoleID;
                    tab.Name = DAndDMenuTree.Name;
                    tab.ActionType = DAndDMenuTree.ActionType;
                    tab.MenuController = DAndDMenuTree.MenuController;
                    tab.MenuAction = DAndDMenuTree.MenuAction;
                    tab.XiLinkID = DAndDMenuTree.XiLinkID;
                    tab.Priority = DAndDMenuTree.Priority;
                    int rank = DAndDMenuTree.Priority;
                    int maxrank = 0;
                    var RightMenuTrees = dbContext.RightMenuTrees.ToList();
                    if (RightMenuTrees.Count() > 0)
                    {
                        maxrank = RightMenuTrees.Max(m => m.Priority);
                    }
                    var tabrank = dbContext.RightMenuTrees.Where(m => m.Priority == rank).FirstOrDefault();
                    if (tabrank != null)
                    {
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[RightMenuTrees] SET [Priority] = [Priority] + 1  WHERE [Priority] in (select [Priority] from[RightMenuTrees] where [Priority] BETWEEN " + rank + " and " + maxrank + ")");
                    }
                    dbContext.RightMenuTrees.Add(tab);
                    dbContext.SaveChanges();
                    TabID = tab.ID;
                }
                else
                {
                    RightMenuTrees tab = dbContext.RightMenuTrees.Find(DAndDMenuTree.ID);
                    tab.ParentID = DAndDMenuTree.ParentID;
                    var oldrank = tab.Priority;
                    var newrank = Newposition;
                    if (Oldposition != 0 && Newposition != 0)
                    {
                        if (NoOfPostionsChanged > 0)
                        {
                            var NewPos = tab.Priority - NoOfPostionsChanged;
                            dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[RightMenuTrees] SET [Priority] = [Priority] + 1  WHERE [Priority] BETWEEN " + NewPos + " and " + tab.Priority + " and ParentID='" + OldParentID + "' and OrgID= " + OrgID);
                            tab.Priority = tab.Priority - NoOfPostionsChanged;
                        }
                        else
                        {
                            var NewPos = tab.Priority - NoOfPostionsChanged;
                            dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[RightMenuTrees] SET [Priority] = [Priority] - 1  WHERE [Priority] BETWEEN " + tab.Priority + " and " + NewPos + " and ParentID='" + OldParentID + "' and OrgID= " + OrgID);
                            tab.Priority = tab.Priority - NoOfPostionsChanged;
                        }
                    }
                    if (Oldposition > 0 && Newposition == 0)
                    {
                        if (OldParentID == NewParentID)
                        {
                            if (NoOfPostionsChanged > 0)
                            {
                                var NewPos = tab.Priority - NoOfPostionsChanged;
                                dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[RightMenuTrees] SET [Priority] = [Priority] + 1  WHERE [Priority] BETWEEN " + NewPos + " and " + tab.Priority + " and ParentID='" + OldParentID + "' and OrgID= " + OrgID);
                                tab.Priority = tab.Priority - NoOfPostionsChanged;
                            }
                            else
                            {
                                var NewPos = tab.Priority - NoOfPostionsChanged;
                                dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[RightMenuTrees] SET [Priority] = [Priority] - 1  WHERE [Priority] BETWEEN " + tab.Priority + " and " + NewPos + " and ParentID='" + OldParentID + "' and OrgID= " + OrgID);
                                tab.Priority = tab.Priority - NoOfPostionsChanged;
                            }
                        }
                        else
                        {
                            var oMenusList = dbContext.RightMenuTrees.Where(m => m.ParentID == NewParentID).ToList();
                            if (oMenusList.Count() > 0)
                            {
                                var ListOfChildMenu = oMenusList.Select(m => m.Priority).ToList();
                                if (ListOfChildMenu.Count() > 0)
                                {
                                    var MaxPriority = ListOfChildMenu.Max();
                                    tab.Priority = MaxPriority + 1;
                                }
                            }
                            else
                            {
                                tab.Priority = 1;
                            }
                        }
                    }
                    if (Oldposition == 0 && Newposition > 0)
                    {
                        var oMenusList = dbContext.RightMenuTrees.Where(m => m.ParentID == NewParentID).ToList();
                        if (oMenusList.Count() > 0)
                        {
                            var ListOfChildMenu = oMenusList.Select(m => m.Priority).ToList();
                            if (ListOfChildMenu.Count() > 0)
                            {
                                var MaxPriority = ListOfChildMenu.Max();
                                tab.Priority = MaxPriority + 1;
                            }
                        }
                        else
                        {
                            tab.Priority = 1;
                        }
                    }
                    if (Oldposition == 0 && Newposition == 0)
                    {
                        var oMenusList = dbContext.RightMenuTrees.Where(m => m.ParentID == NewParentID).ToList();
                        if (oMenusList.Count() > 0)
                        {
                            var ListOfChildMenu = oMenusList.Select(m => m.Priority).ToList();
                            if (ListOfChildMenu.Count() > 0)
                            {
                                var MaxPriority = ListOfChildMenu.Max();
                                tab.Priority = MaxPriority + 1;
                            }
                        }
                        else
                        {
                            tab.Priority = 1;
                        }
                    }
                    tab.ParentID = NewParentID;
                    dbContext.SaveChanges();
                    TabID = tab.ID;
                }
            }
            return TabID;
        }

        //25/11/2017
        public RightMenuTrees GetOrganisation(int Orgid, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (UserDetails.sRoleName == EnumRoles.XISuperAdmin.ToString())
            {
                RightMenuTrees MenuTree = new RightMenuTrees();
                List<VMDropDown> Organisations = new List<VMDropDown>();
                MenuTree.Organisations = Organisations;
                MenuTree.ID = 0;
                return MenuTree;
            }
            else
            {
                RightMenuTrees MenuTree = new RightMenuTrees();
                List<VMDropDown> Organisations = new List<VMDropDown>();
                Organisations = (from c in dbCore.Organization.Where(m => m.ID == Orgid).ToList()
                                 select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
                MenuTree.Organisations = Organisations;
                MenuTree.ID = 0;
                return MenuTree;
            }

        }

        public List<VMDropDown> GetRolesForMenu(string OrgName, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int UserID = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.UserID).FirstOrDefault();
            List<VMDropDown> Roles = new List<VMDropDown>();

            Roles = (from c in dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).ToList()
                     select new VMDropDown { text = c.sRoleName, Value = c.RoleID }).ToList();
            return Roles;
        }

        public bool IsExistsRootName(int ID, string RootName, int OrgID, int RoleID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var Menu = dbContext.RightMenuTrees.Where(m => m.RootName.Equals(RootName, StringComparison.OrdinalIgnoreCase)).Where(m => m.ParentID == "#").Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).FirstOrDefault();
            if (ID == 0)
            {
                if (Menu == null)
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
                if (Menu != null)
                {
                    if (ID == Menu.ID)
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
        public RightMenuTrees SaveMenuDetails(int iUserID, int RoleID, int OrgID, string RootName, string sDatabase)
        {
            int iStatus = 0;
            RightMenuTrees RgtMenuTree = new RightMenuTrees();
            RightMenuTrees RgtMenuTrees = new RightMenuTrees();
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                //Check if Root name already exists
                int iRootID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).Where(m => m.Name == RootName).Where(m => m.ParentID == "#").Select(m => m.ID).FirstOrDefault();
                if (iRootID == 0)
                {
                    //string ReplaceSpace = RootName.Replace(" ", "_");
                    //string CreateID = ReplaceSpace + "_" + OrgID + "_" + RoleID;
                    RgtMenuTree.RootName = RootName;
                    //RgtMenuTree.MenuID = CreateID;
                    RgtMenuTree.MenuID = "";
                    RgtMenuTree.Name = RootName;
                    RgtMenuTree.ParentID = "#";
                    RgtMenuTree.OrgID = OrgID;
                    RgtMenuTree.RoleID = RoleID;
                    RgtMenuTree.Priority = 0;
                    RgtMenuTree.XiLinkID = 0;
                    RgtMenuTree.CreatedBy = RgtMenuTree.UpdatedBy = iUserID;
                    RgtMenuTree.CreatedTime = RgtMenuTree.UpdatedTime = DateTime.Now;
                    RgtMenuTree.CreatedBySYSID = RgtMenuTree.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    //RgtMenuTree.StatusTypeID = 10;
                    dbContext.RightMenuTrees.Add(RgtMenuTree);
                    dbContext.SaveChanges();
                    iStatus = 1;

                }
                else
                {
                    //already exists....
                }
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }

            if (iStatus == 1)
            {
                RightMenuTrees UpdateMenuTree = dbContext.RightMenuTrees.Find(RgtMenuTree.ID);
                UpdateMenuTree.MenuID = (RgtMenuTree.ID).ToString();
                dbContext.SaveChanges();

                RgtMenuTrees = dbContext.RightMenuTrees.Where(m => m.ID == RgtMenuTree.ID).FirstOrDefault();
            }
            return RgtMenuTrees;
        }

        public List<RightMenuTrees> GetMenuTreeDetails(string RootName, int OrgID, int RoleID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<RightMenuTrees> lMenuDetails = new List<RightMenuTrees>();
            lMenuDetails = dbContext.RightMenuTrees.Where(m => m.RoleID == RoleID).Where(m => m.OrgID == OrgID).Where(m => m.RootName == RootName).OrderBy(m => m.Priority).ToList();
            return lMenuDetails;
        }

        public List<RightMenuTrees> GetChildForRootMenu(string NodeID, int UserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<RightMenuTrees> lMenuTrees = new List<RightMenuTrees>();
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            lMenuTrees = dbContext.RightMenuTrees.Where(m => m.ParentID == NodeID).Where(m => m.RoleID == RoleID).ToList();
            return lMenuTrees;
        }

        public int SaveEditedMenuDetails(int RoleID, int OrgID, string NewRootName, string OldRootName, string sDatabase)
        {
            int iStatus = 0;
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                //Check if group name already exists
                int iRootID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).Where(m => m.Name == NewRootName).Where(m => m.ParentID == "#").Select(m => m.ID).FirstOrDefault();
                if (iRootID == 0)
                {
                    //replace the menu with parent "#"
                    int iOldRootID = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).Where(m => m.Name == OldRootName).Where(m => m.ParentID == "#").Select(m => m.ID).FirstOrDefault();
                    RightMenuTrees EditMenuTree = dbContext.RightMenuTrees.Find(iOldRootID);
                    EditMenuTree.Name = NewRootName;
                    EditMenuTree.RootName = NewRootName;
                    dbContext.SaveChanges();

                    var iIDs = new List<int>();
                    iIDs = dbContext.RightMenuTrees.Where(m => m.OrgID == OrgID).Where(m => m.RoleID == RoleID).Where(m => m.RootName == OldRootName).Select(m => m.ID).ToList();
                    for (var i = 0; i < iIDs.Count(); i++)
                    {
                        int iEditID = iIDs[i];
                        RightMenuTrees EditChildMenuTree = dbContext.RightMenuTrees.Find(iEditID);
                        EditChildMenuTree.RootName = NewRootName;
                        dbContext.SaveChanges();
                    }
                    iStatus = 1;
                }
                else
                {
                    //already exists....                  
                }
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }
            return iStatus;
        }
        #region UserConfiguration

        public DTResponse UserConfigurationGrid(jQueryDataTableParamModel param, int UserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<UserConfigurations> AllTypes;
            AllTypes = dbContext.UserConfigurations;
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTypes = AllTypes.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTypes.Count();
            AllTypes = QuerableUtil.GetResultsForDataTables(AllTypes, "", sortExpression, param);
            var clients = AllTypes.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.Name, c.StatusTypeID.ToString(),"" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse SaveUserConfigurations(VMUserConfigurations model, int UserID, string sDatabase)
        {
            UserConfigurations user = new UserConfigurations();
            ModelDbContext dbContext = new ModelDbContext();
            if (model.ID == 0)
            {
                user.Name = model.Name;
                user.StatusTypeID = model.StatusTypeID;
                user.CreatedBy = UserID;
                user.CreatedTime = DateTime.Now;
                user.UpdatedBy = UserID;
                user.UpdatedTime = DateTime.Now;
                dbContext.UserConfigurations.Add(user);
            }
            else
            {
                user = dbContext.UserConfigurations.Find(model.ID);
                user.Name = model.Name;
                user.StatusTypeID = model.StatusTypeID;
                user.UpdatedBy = UserID;
                user.UpdatedTime = DateTime.Now;
            }
            dbContext.SaveChanges();
            //return user.ID;
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = user.ID, Status = true };
        }

        public VMUserConfigurations EditUserConfigurations(int ID, string sDatabase)
        {
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                var model = dbContext.UserConfigurations.Find(ID);
                var obj = new VMUserConfigurations();
                obj.ID = model.ID;
                obj.Name = model.Name;
                obj.StatusTypeID = model.StatusTypeID;
                return obj;
            }
            catch (Exception ex)
            {
                return new VMUserConfigurations();
            }
        }

        #endregion UserConfiguration


        #region UserCookie

        public VMCustomResponse SaveUserCookieDetails(string sUniqueID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            if (!string.IsNullOrEmpty(sOrgName) && sOrgName.ToLower() != "org")
            {
                var UserDetails = Common.GetUserDetails(0, sOrgName, sDatabase);
                dbContext = new ModelDbContext();
            }
            cUserCookies oCookie = new cUserCookies();
            oCookie.UniqueCookieID = sUniqueID;
            dbContext.UserCookies.Add(oCookie);
            dbContext.SaveChanges();
            return new VMCustomResponse() { ID = oCookie.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public int GetLayoutDetails(string sOrgName, string sLayoutName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var OrgDetails = dbCore.Organization.Where(m => m.Name.ToLower() == sOrgName.ToLower()).FirstOrDefault();
            if (OrgDetails != null)
            {
                var LayoutDetails = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == sLayoutName.ToLower()).FirstOrDefault();
                if (LayoutDetails != null)
                {
                    return LayoutDetails.ID;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        #endregion UserCookie

        #region SaveRoleMenusAndMappingMenus

        public XIMenuMappings SaveRoleMappings(List<XIMenuMappings> oMenu, string RootName, int iRoleID, string RoleName, int iOrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<XIMenuMappings> oRM = new List<XIMenuMappings>();
            string iNewMapID = "";
            XIMenuMappings oRS = new XIMenuMappings();
            if (oMenu != null || oMenu.Count() > 0)
            {
                var All = dbContext.XIMenuMappings.Where(m => m.RootName == RootName && m.RoleID == iRoleID).ToList();
                dbContext.XIMenuMappings.RemoveRange(All);
                dbContext.SaveChanges();
            }
            //var RemoveHash = oMenu.Where(m => m.ParentID == "#").ToList();
            //if (RemoveHash != null && RemoveHash.Count() > 0)
            //{
            //    foreach (var Rem in RemoveHash)
            //    {
            //        oMenu.Remove(Rem);
            //    }
            //}
            var Exists = dbContext.XIMenuMappings.Where(m => m.RootName == RootName && m.RoleID == iRoleID).ToList().FirstOrDefault();
            if (Exists != null)
            {
                if (Exists.RootName != RootName)
                {
                    oRS.Name = RootName;
                    oRS.RootName = RootName;
                    oRS.ParentID = "#";
                    oRS.RoleID = iRoleID; oRM.FirstOrDefault().OrgID = iOrgID;
                    oRS.CreatedBy = oRM.FirstOrDefault().UpdatedBy = iUserID;
                    oRS.CreatedTime = oRM.FirstOrDefault().UpdatedTime = DateTime.Now;
                    oRS.CreatedBySYSID = oRM.FirstOrDefault().UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oRS.StatusTypeID = 10;
                    dbContext.XIMenuMappings.Add(oRS);
                    dbContext.SaveChanges();
                    if (oRS.ID > 0)
                    {
                        iNewMapID = oRS.ID.ToString();
                        XIMenuMappings UpdateMenuTree = dbContext.XIMenuMappings.Find(oRS.ID);
                        UpdateMenuTree.MenuID = (oRS.ID).ToString();
                        dbContext.SaveChanges();
                    }
                }
                else
                {
                    iNewMapID = Exists.ID.ToString();
                }
            }
            else
            {
                oRS.RootName = RootName;
                oRS.ParentID = "#";
                oRS.Name = RootName;
                oRS.RoleID = iRoleID; oRS.OrgID = iOrgID;
                oRS.CreatedBy = oRS.UpdatedBy = iUserID;
                oRS.CreatedTime = oRS.UpdatedTime = DateTime.Now;
                oRS.CreatedBySYSID = oRS.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oRS.StatusTypeID = 10;
                dbContext.XIMenuMappings.Add(oRS);
                dbContext.SaveChanges();
                if (oRS.ID > 0)
                {
                    iNewMapID = oRS.ID.ToString();
                    XIMenuMappings UpdateMenuTree = dbContext.XIMenuMappings.Find(oRS.ID);
                    UpdateMenuTree.MenuID = (oRS.ID).ToString();
                    dbContext.SaveChanges();
                }
            }
            if (oMenu.Count() > 0)
            {
                var Data = CheckChilds(oMenu);

                if (Data != null && Data.Count() > 0)
                {
                    AddChilds(Data, iNewMapID, iRoleID, RootName, iOrgID, iUserID);
                }
            }
            return oRS;
        }

        private List<XIMenuMappings> CheckChilds(List<XIMenuMappings> oMP)
        {
            var oNewMP = new List<XIMenuMappings>();
            oNewMP = oMP.FindAll(m => m.ParentID == "#").ToList();
            foreach (var items in oNewMP.ToList())
            {
                var iParID = items.ID;
                items.oMenuParams = oMP.FindAll(m => m.ParentID == iParID.ToString()).ToList();
                if (items.oMenuParams.Count() > 0)
                {
                    oCheckChilds(items.oMenuParams, oMP);
                }
            }
            return oNewMP;
        }

        private List<XIMenuMappings> oCheckChilds(List<XIMenuMappings> oNewMP, List<XIMenuMappings> oMP)
        {
            foreach (var items in oNewMP.ToList())
            {
                var iParID = items.ID;
                items.oMenuParams = oMP.FindAll(m => m.ParentID == iParID.ToString()).ToList();
                if (items.oMenuParams.Count() > 0)
                {
                    oCheckChilds(items.oMenuParams, oMP);
                }
            }
            return oNewMP;
        }

        private List<XIMenuMappings> AddChilds(List<XIMenuMappings> Data, string iParentID, int iRoleID, string RootName, int iOrgID, int iUserID)
        {
            XIMenuMappings oRS = new XIMenuMappings();
            ModelDbContext dbContext = new ModelDbContext();
            string oChildID = "";
            if (Data.Count() > 0)
            {
                foreach (var item in Data)
                {
                    oRS.Name = item.Name;
                    oRS.RootName = RootName;
                    oRS.ParentID = iParentID;
                    oRS.RoleID = iRoleID; oRS.OrgID = iOrgID;
                    oRS.CreatedBy = oRS.UpdatedBy = iUserID;
                    oRS.CreatedTime = oRS.UpdatedTime = DateTime.Now;
                    oRS.CreatedBySYSID = oRS.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oRS.StatusTypeID = 10;
                    dbContext.XIMenuMappings.Add(oRS);
                    dbContext.SaveChanges();
                    if (oRS.ID > 0)
                    {
                        oChildID = oRS.ID.ToString();
                        XIMenuMappings UpdateMenuTree = dbContext.XIMenuMappings.Find(oRS.ID);
                        UpdateMenuTree.MenuID = (oRS.ID).ToString();
                        dbContext.SaveChanges();
                    }
                    if (item.oMenuParams != null && item.oMenuParams.Count() > 0)
                    {
                        AddChilds(item.oMenuParams, oChildID, iRoleID, RootName, iOrgID, iUserID);
                    }
                }
            }
            return Data;
        }

        public int AddTreeNode(XIRoleMenus node, string sOrgName, int iOrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            if (!string.IsNullOrEmpty(node.Name))
            {
                XIRoleMenus XiRM = new XIRoleMenus();
                XiRM.ParentID = node.ParentID;
                XiRM.OrgID = iOrgID;
                XiRM.Name = node.Name;
                XiRM.RoleID = node.RoleID;
                XiRM.FKiApplicationID = node.FKiApplicationID;
                XiRM.XiLinkID = node.XiLinkID;
                XiRM.RootName = node.Name;
                XiRM.ActionType = 0;
                XiRM.StatusTypeID = 10;
                XiRM.CreatedBy = XiRM.UpdatedBy = iUserID;
                XiRM.CreatedTime = XiRM.UpdatedTime = DateTime.Now;
                XiRM.CreatedBySYSID = XiRM.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XIRoleMenus.Add(XiRM);
                dbContext.SaveChanges();
                int UpdatedID = XiRM.ID;
                int iMaxPriority = 0;
                var oPrioMenu = dbContext.XIRoleMenus.Where(m => m.ParentID == "#").OrderBy(m => m.Priority).ToList();
                XiRM.FKiInboxID = UpdatedID.ToString();
                if (oPrioMenu.Count() > 0 && oPrioMenu != null)
                {
                    iMaxPriority = oPrioMenu.Select(m => m.Priority).Max();
                    iMaxPriority = iMaxPriority + 1;
                }
                XiRM.Priority = iMaxPriority;
                dbContext.SaveChanges();
                if (node.SelectedNodes != null && node.SelectedNodes.Count() > 0)
                {
                    foreach (var item in node.SelectedNodes)
                    {
                        int iRoleMenuID = Convert.ToInt32(item);
                        var Selectrole = dbContext.XIRoleMenus.Where(m => m.ID == iRoleMenuID).FirstOrDefault();
                        Selectrole.ParentID = XiRM.ID.ToString();
                        Selectrole.RootName = XiRM.Name;
                        dbContext.XIRoleMenus.Add(Selectrole);
                        dbContext.SaveChanges();
                        var iMenuID = Selectrole.ID;
                        Selectrole.FKiInboxID = iMenuID.ToString();
                        dbContext.SaveChanges();
                    }
                }
            }
            return 1;
        }

        public int CreateandRenameMenu(XIRoleMenus node, int iOrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int id = 0;
            if (node.Type == "Create")
            {
                if (!string.IsNullOrEmpty(node.Name))
                {
                    XIRoleMenus XiRM = new XIRoleMenus();
                    XiRM.ParentID = node.ParentID;
                    XiRM.OrgID = iOrgID;
                    XiRM.Name = node.Name;
                    XiRM.RoleID = node.RoleID;
                    XiRM.FKiApplicationID = node.FKiApplicationID;
                    XiRM.XiLinkID = node.XiLinkID;
                    XiRM.ActionType = 0;
                    XiRM.RoleID = node.RoleID;
                    XiRM.StatusTypeID = 10;
                    XiRM.CreatedBy = XiRM.UpdatedBy = iUserID;
                    XiRM.CreatedTime = XiRM.UpdatedTime = DateTime.Now;
                    XiRM.CreatedBySYSID = XiRM.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    dbContext.XIRoleMenus.Add(XiRM);
                    dbContext.SaveChanges();
                    id = XiRM.ID;
                    int maxOrder = 0;
                    var xiroles = dbContext.XIRoleMenus.Where(m => m.ParentID == node.ParentID).Where(m => m.StatusTypeID == 10).OrderBy(m => m.Priority).ToList();
                    XiRM.FKiInboxID = id.ToString();
                    int nodeid = Convert.ToInt32(node.ParentID);
                    XiRM.RootName = dbContext.XIRoleMenus.Where(m => m.ID == nodeid).FirstOrDefault().RootName;
                    if (xiroles.Count() > 1 && xiroles != null)
                    {
                        maxOrder = xiroles.Select(m => m.Priority).Max();
                        maxOrder = maxOrder + 1;
                    }
                    XiRM.Priority = maxOrder;
                    dbContext.SaveChanges();
                }
            }
            if (node.Type == "Rename")
            {
                var oMR = dbContext.XIRoleMenus.Where(m => m.ID == node.ID).FirstOrDefault();
                oMR.Name = node.Name;
                dbContext.SaveChanges();
            }
            return id;
        }

        public int DeleteTreeMenu(int ID)
        {
            ModelDbContext dbcontext = new ModelDbContext();
            var oMenu = dbcontext.XIRoleMenus.Where(m => m.ID == ID).FirstOrDefault();
            oMenu.StatusTypeID = 20;
            dbcontext.SaveChanges();
            return ID;
        }

        #endregion SaveRoleMenusAndMappingMenus
    }
}