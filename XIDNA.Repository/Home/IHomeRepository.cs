using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public interface IHomeRepository
    {
        int SaveMenuTreeDetails(int ID, string RootNode, string ParentNode, string NodeID, string NodeTitle, string Type, int iRoleID, int UserID, int OrgID, string sDatabase, string sOrgName);
        //  List<RightMenuTrees> GetMenuTreeDetails(int UserID, int OrgID, string sDatabase); 
        List<RightMenuTrees> DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type, int iRoleID, int UserID, int OrgID, string sDatabase);
        RightMenuTrees AddDetailsForMenu(string ParentNode, string NodeID, int iRoleID, int OrgID, int UserID, string sDatabase);
        VMCustomResponse SaveAddedDetails(int UserID, RightMenuTrees model, string sDatabase);
        List<RightMenuTrees> ShowMenuTreeDetails(int UserID, int OrgID, string sOrgName, string sDatabase);
        //List<RightMenuTrees> GetChildForMenu(int ID, int OrgID);
        int DragAndDropNodes(string NodeID, string OldParentID, string NewParentID, int UserID, int OrgID, string sDatabase, int Oldposition, int Newposition);

        RightMenuTrees EditRootMenu(int ID, int OrgID, int UserID, string sDatabase);
        int DeleteRootMenu(int ID, int OrgID, int UserID, string sDatabase);

        RightMenuTrees GetOrganisation(int iOrgID, int UserID, string sOrgName, string sDatabase);
        List<VMDropDown> GetRolesForMenu(string OrgName, int OrgID, string sDatabase);
        bool IsExistsRootName(int ID, string RootName, int OrgID, int RoleID, string sDatabase);
        RightMenuTrees SaveMenuDetails(int iUserID, int RoleID, int OrgID, string RootName, string sDatabase);
        List<RightMenuTrees> GetMenuTreeDetails(string RootName, int OrgID, int RoleID, string sDatabase);
        List<RightMenuTrees> GetChildForRootMenu(string NodeID, int UserID, int OrgID, string sDatabase);
        DTResponse GetMenuDetails(jQueryDataTableParamModel param, int iUserID, string sOrgName, int OrgID, string sDatabase);
        int SaveEditedMenuDetails(int RoleID, int OrgID, string NewRootName, string OldRootName, string sDatabase);

        #region UserConfigurations

        DTResponse UserConfigurationGrid(jQueryDataTableParamModel param, int UserID, string sDatabase);
        VMCustomResponse SaveUserConfigurations(VMUserConfigurations model, int UserID, string sDatabase);
        VMUserConfigurations EditUserConfigurations(int ID, string sDatabase);

        #endregion UserConfigurations


        #region UserCookie

        VMCustomResponse SaveUserCookieDetails(string sUniqueID, string sDatabase, string sOrgName);
        int GetLayoutDetails(string sOrgName, string sLayoutName, string sDatabase);

        #endregion UserCookie

        #region SaveRoleMenusAndMappingMenus

        XIMenuMappings SaveRoleMappings(List<XIMenuMappings> oMenu, string RootName, int iRoleID, string RoleName, int iOrgID, int iUserID, string sDatabase);
        int AddTreeNode(XIRoleMenus node, string sOrgName, int iOrgID, int iUserID, string sDatabase);
        int CreateandRenameMenu(XIRoleMenus node, int iOrgID, int iUserID, string sDatabase);
        int DeleteTreeMenu(int ID);

        #endregion SaveRoleMenusAndMappingMenus
    }
}
