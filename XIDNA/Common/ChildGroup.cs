using XIDNA.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIDNA.Common
{
    //public static class ChildGroup
    //{
    //    public static List<int> GetChildGroupID(List<XIDNA.Models.AspNetGroups> nodes)
    //    {
    //        List<int> Groups = new List<int>();
    //        foreach (var node in nodes)
    //        {


    //            if (node.IsLeaf)
    //            {

    //                Groups.Add(node.Id);

    //            }
    //            else
    //            {
    //                Groups.Add(node.Id);
    //                Groups.AddRange(GetChildGroupID(node.SubGroups.ToList()));
    //            }
    //        }
    //        return Groups;
    //    }
    //    //public static List<VMMenu> GetChildMenus(List<TataSteel.Models.Menus> nodes)
    //    //{
    //    //    List<VMMenu> Groups = new List<VMMenu>();
    //    //    foreach (var node in nodes)
    //    //    {
    //    //        if (node.Forms!=null)
    //    //        {
    //    //            Groups.Add(new VMMenu{ID= node.ID,MenuName=node.MenuName,FormName=node.Forms.Name});
    //    //        }
    //    //        if(!node.IsLeaf )
    //    //        {
    //    //            Groups.AddRange(GetChildMenus(node.SubMenus.ToList()));
    //    //        }
    //    //    }
    //    //    return Groups;
    //    //}
    //    public static List<string> GetChildGroupName(List<XIDNA.Models.AspNetGroups> nodes)
    //    {
    //        List<string> Groups = new List<string>();
    //        foreach (var node in nodes)
    //        {
    //            if (node.IsLeaf)
    //            {
    //                Groups.Add(node.Name);
    //            }
    //            else
    //            {
    //                Groups.Add(node.Name);
    //                Groups.AddRange(GetChildGroupName(node.SubGroups.ToList()));
    //            }
    //        }
    //        return Groups;
    //    }
    //}
}