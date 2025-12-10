using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.ViewModels;
using XIDNA.Models;
using System.Text.RegularExpressions;

namespace XIDNA.Repository
{
    public static class ServiceUtil
    {

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString;
        }
        public static string GetClientConnectionString()
        {

            return ModelDbContext.ConnectionString(ConfigurationManager.AppSettings["CoreDataBase"]).Replace(ConfigurationManager.AppSettings["CoreDataBase"], ConfigurationManager.AppSettings["SharedDataBase"]);
            //return ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString;
        }

        public static string GetCacheStatus()
        {
            return ConfigurationManager.AppSettings["Cache"];
        }

        public static string GetFKLabelGroup(BOs oBO, List<string> FKColumns, string SelectQuery, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> FKLabels = new List<VMDropDown>();
            if (FKColumns != null)
            {
                foreach (var items in FKColumns)
                {
                    if (items.Substring(0, 2) == "FK")
                    {
                        var oBOField = oBO.BOFields.Where(m => m.Name == items).FirstOrDefault();
                        if (oBOField.IsOptionList)
                        {
                            VMDropDown FKTable = new VMDropDown();
                            var sTableName = items;

                            sTableName = sTableName.Substring(2, sTableName.Length - 2);
                            var Character = sTableName.Select(c => char.IsUpper(c)).ToList();
                            var CharPosition = Character.IndexOf(true);
                            if (CharPosition == 1)
                            {
                                char FirstLetter = sTableName[0];
                                if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                                {
                                    sTableName = sTableName.Substring(1, sTableName.Length - 1);
                                }
                            }
                            if (sTableName.Contains("ID"))
                            {
                                sTableName = sTableName.Replace("ID", "");
                            }
                            sTableName = sTableName + "_T";
                            var BO = dbContext.BOs.Where(m => m.TableName == sTableName).FirstOrDefault();
                            var SubQuery = "(Select sOptionName from BOOptionLists Where svalues = " + items + " and boid=" + oBO.BOID + ")";
                            SelectQuery = SelectQuery.Replace(items, SubQuery);
                            //if (BO != null)
                            //{
                            //    var LabelGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.LabelGroup.ToString()).FirstOrDefault().BOSqlFieldNames;
                            //    if (!string.IsNullOrEmpty(LabelGroup))
                            //    {
                            //        var SubQuery = "(Select " + LabelGroup + " from " + sTableName + " Where id = " + items + ")";
                            //        SelectQuery = SelectQuery.Replace(items, SubQuery);
                            //    }
                            //    else
                            //    {
                            //        var DefaultGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.DefaultGroup.ToString()).FirstOrDefault().BOSqlFieldNames;
                            //        if (!string.IsNullOrEmpty(DefaultGroup))
                            //        {
                            //            var SubQuery = "(Select " + DefaultGroup + " from " + sTableName + " Where id = " + items + ")";
                            //            SelectQuery = SelectQuery.Replace(items, SubQuery);
                            //        }
                            //    }
                            //}

                        }
                        else if (oBOField.FKiType > 0)
                        {
                            if (oBOField.FKiType == 10)
                            {
                                VMDropDown FKTable = new VMDropDown();
                                var sTableName = items;
                                sTableName = sTableName.Substring(2, sTableName.Length - 2);
                                var Character = sTableName.Select(c => char.IsUpper(c)).ToList();
                                var CharPosition = Character.IndexOf(true);
                                if (CharPosition == 1)
                                {
                                    char FirstLetter = sTableName[0];
                                    if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                                    {
                                        sTableName = sTableName.Substring(1, sTableName.Length - 1);
                                    }
                                }
                                if (sTableName.Contains("ID"))
                                {
                                    sTableName = sTableName.Replace("ID", "");
                                }
                                sTableName = sTableName + "_T";
                                var BO = dbContext.BOs.Where(m => m.TableName == sTableName).FirstOrDefault();

                                if (BO != null)
                                {
                                    var LabelGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.LabelGroup.ToString()).FirstOrDefault().BOFieldNames;
                                    if (!string.IsNullOrEmpty(LabelGroup))
                                    {
                                        var SubQuery = "(Select " + LabelGroup + " from " + sTableName + " Where id = " + items + ")";
                                        SelectQuery = SelectQuery.Replace(items, SubQuery);
                                    }
                                    else
                                    {
                                        var DefaultGroup = BO.BOGroups.Where(m => m.BOID == BO.BOID && m.GroupName.ToString() == ServiceConstants.DefaultGroup.ToString()).FirstOrDefault().BOSqlFieldNames;
                                        if (!string.IsNullOrEmpty(DefaultGroup))
                                        {
                                            var SubQuery = "(Select " + DefaultGroup + " from " + sTableName + " Where id = " + items + ")";
                                            SelectQuery = SelectQuery.Replace(items, SubQuery);
                                        }
                                    }
                                }
                            }
                            else if (oBOField.FKiType == 20)
                            {

                            }
                        }

                    }
                }
            }
            return SelectQuery;
        }
        public static List<VMDropDown> GetForeginkeyValues(string Query)
        {
            List<VMDropDown> AllVlaues = new List<VMDropDown>();
            var IsClass = Query.IndexOf("FKiLeadClassID", StringComparison.OrdinalIgnoreCase);
            if (IsClass > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "FKiLeadClassID",
                    Value = IsClass
                });
            }
            var IsOrg = Query.IndexOf("FKiOrgID", StringComparison.OrdinalIgnoreCase);
            if (IsOrg > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "FKiOrgID",
                    Value = IsOrg
                });
            }
            var IsTeam = Query.IndexOf("iTeamID", StringComparison.OrdinalIgnoreCase);
            if (IsTeam > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "iTeamID",
                    Value = IsTeam
                });
            }
            var IsSource = Query.IndexOf("FKiSourceID", StringComparison.OrdinalIgnoreCase);
            if (IsSource > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "FKiSourceID",
                    Value = IsSource
                });
            }
            var IsStatus = Query.IndexOf("iStatus", StringComparison.OrdinalIgnoreCase);
            if (IsStatus > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "iStatus",
                    Value = IsStatus
                });
            }
            var IsUserID = Query.IndexOf("UserID", StringComparison.OrdinalIgnoreCase);
            if (IsUserID > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "UserID",
                    Value = IsUserID
                });
            }
            var IsClientID = Query.IndexOf("FKiClientID", StringComparison.OrdinalIgnoreCase);
            if (IsClientID > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "FKiClientID",
                    Value = IsClientID
                });
            }
            var IsOrderClassID = Query.IndexOf("FKiOrderClassID", StringComparison.OrdinalIgnoreCase);
            if (IsOrderClassID > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "FKiOrderClassID",
                    Value = IsOrderClassID
                });
            }
            var iCover = Query.IndexOf("iCover", StringComparison.OrdinalIgnoreCase);
            if (iCover > 0)
            {
                AllVlaues.Add(new VMDropDown
                {
                    text = "iCover",
                    Value = iCover
                });
            }
            List<VMDropDown> SortedList = AllVlaues.OrderBy(o => o.Value).ToList();
            return SortedList;
        }

        /// <purpose>
        /// 
        /// </purpose>
        public static List<VMDropDown> GetGenericEnumList(Type Enumtype)
        {
            try
            {
                if (Enumtype.IsEnum)
                {
                    return Enum.GetValues(Enumtype).Cast<int>().Select(t => new VMDropDown
                    {
                        ID = ((int)t),
                        text = (Enum.GetName(Enumtype, t)).Replace("_", " ").Replace("__", "")
                    }).ToList();
                }
                else
                {
                    return new List<VMDropDown>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string ReplaceQueryContent(string Query, string UserIDs, int UserID, int OrganizationID, int LeadID, int iBOID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string Heading = string.Empty;
            if (Query.IndexOf("'Yesterday'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'Yesterday'", "DATEADD(day,datediff(day,1,GETDATE()),0)");
            }
            if (Query.IndexOf("'Today'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'Today'", "GETDATE()");
            }
            if (Query.IndexOf("'Tomorrow'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'Tomorrow'", "DATEADD(day, 1, GETDATE())");
            }
            if (Query.IndexOf("'7 days'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'7 days'", "DATEADD(day, 7, GETDATE())");
            }
            if (Query.IndexOf("'14 days'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'14 days'", "DATEADD(day, 14, GETDATE())");
            }
            if (Query.IndexOf("'21 days'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'21 days'", "DATEADD(day, 21, GETDATE())");
            }
            if (Query.IndexOf("'90 days'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'90 days'", "DATEADD(day, 90, GETDATE())");
            }
            if (Query.IndexOf("'365 days'", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("'365 days'", "DATEADD(day, 365, GETDATE())");
            }
            if (Query.IndexOf("!= Null", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Regex.Replace(Query, "!= Null", "IS NOT NULL", RegexOptions.IgnoreCase);
            }
            if (Query.IndexOf("= Null", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("= 'Null'", " IS NULL");
            }
            if (Query.IndexOf("= IS Null", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("= IS NULL", " IS NULL");
            }
            if (Query.IndexOf("FROM {Leads}", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("FROM {Leads}", "FROM Leads");
            }
            if (Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) > 0)
            {
                if (Query.IndexOf("UserID = {CurrentUser}", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Query = Regex.Replace(Query, "UserID = {CurrentUser}", "UserID =" + UserID, RegexOptions.IgnoreCase);
                }

                if (Query.IndexOf("iTeamID = {CurrentTeam}", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    string database = dbContext.XIAppUsers.Where(m => m.FKiOrganisationID == OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
                    DataContext Spdb = new DataContext(database);
                    var teams = Spdb.OrganizationTeams.Where(m => m.OrganizationID == OrganizationID).Select(m => m.ID).ToList();
                    string TeamIDs = "";
                    if (teams.Count() > 0)
                    {
                        foreach (var items in teams)
                        {
                            TeamIDs = TeamIDs + items + ",";
                        }
                        TeamIDs = TeamIDs.Substring(0, TeamIDs.Length - 1);
                        Query = Regex.Replace(Query, "iTeamID = {CurrentTeam}", "iTeamID IN(" + TeamIDs + ")", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        Query = Regex.Replace(Query, "iTeamID = {CurrentTeam}", "iTeamID =0", RegexOptions.IgnoreCase);
                    }
                }
                if (Query.IndexOf("FKiOrgID = {CurrentOrganization}", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Query = Regex.Replace(Query, "FKiOrgID = {CurrentOrganization}", "FKiOrgID=" + OrganizationID, RegexOptions.IgnoreCase);
                }

                if (Query.IndexOf("{CurrentEmail}", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (LeadID > 0)
                    {
                        string Email = GetLeadDetails(LeadID, OrganizationID);
                        Query = Regex.Replace(Query, "{CurrentEmail}", Email, RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        Query = Regex.Replace(Query, "{CurrentEmail}", "0", RegexOptions.IgnoreCase);
                    }
                }

                if (Query.IndexOf("{CurrentLead}", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    if (LeadID > 0)
                    {
                        string Email = GetLeadDetails(LeadID, OrganizationID);
                        Query = Regex.Replace(Query, "{CurrentLead}", "" + LeadID + "", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        Query = Regex.Replace(Query, "{CurrentLead}", "0", RegexOptions.IgnoreCase);
                    }
                }

                if (Query.IndexOf("OrgHeirarchyID LIKE 'Org{id}_Loc{id}%'", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    var Locations = dbContext.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sLocation).FirstOrDefault();
                    string OrgHei = "";
                    if (Locations != null)
                    {
                        var AllLocations = Locations.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var items in AllLocations)
                        {
                            OrgHei = OrgHei + "OrgHeirarchyID LIKE 'Org" + OrganizationID + "_Loc" + items + "%' or ";
                        }
                        OrgHei = OrgHei.Substring(0, OrgHei.Length - 4);
                    }
                    else
                    {
                        OrgHei = "OrgHeirarchyID LIKE 'Org1_Loc0%'";
                    }
                    Query = Regex.Replace(Query, "OrgHeirarchyID LIKE 'Org{id}_Loc{id}%'", OrgHei, RegexOptions.IgnoreCase);
                }
            }
            var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var ReplaceQry = Query.Substring(0, FromIndex);
            var NewReplaceQry = ReplaceQry.Replace("SELECT", "").Replace("select", "");
            List<string> Heads = new List<string>();
            Heads = NewReplaceQry.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var BO = dbContext.BOs.Where(m => m.BOID == iBOID).FirstOrDefault();
            if (Heads.Count() > 0)
            {
                foreach (var match in Heads)
                {
                    if (match.Contains("{"))
                    {
                        var sGrup = match;
                        if (match.Contains("."))
                        {
                            sGrup = sGrup.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1];
                        }
                        List<string> oGrpHeadS = new List<string>();
                        var sResGroup = sGrup.ToString().Replace("{", string.Empty).Replace("}", string.Empty);
                        //int id = 0;
                        //if (BO != null && BO.BOGroups.Any(m => m.GroupName.ToLower() == sResGroup.ToLower()))
                        //{
                        //    id = BO.BOGroups.Where(m => m.GroupName.ToLower() == sResGroup.ToLower()).FirstOrDefault().ID;
                        //}
                        var oGrpD = BO.BOGroups.Where(m => m.GroupName.ToLower() == sResGroup.ToLower()).FirstOrDefault();
                        if (!oGrpD.IsMultiColumnGroup)
                        {
                            //ReplaceQry = ReplaceQry.Replace("{" + sResGroup + "}", oGrpD.BOSqlFieldNames);
                            Heading = Heading + oGrpD.BOSqlFieldNames + " AS " + "'" + oGrpD.GroupName + "'" + ", ";
                        }
                        else
                        {
                            var sTabelName = BO.TableName;
                            var sBOFNames = string.Empty;
                            foreach (var item in oGrpD.BOFieldNames.Split(',').ToList())
                            {
                                sBOFNames = sBOFNames + "[" + sTabelName + "]." + item.Trim() + ", ";
                            }
                            Heading = Heading + sBOFNames;
                        }
                    }
                    else if (match.Contains("."))
                    {
                        Heading = Heading + match + ", ";
                    }
                }
                string UpdateQuery = string.Empty;
                if (!string.IsNullOrEmpty(Heading))
                {
                    UpdateQuery = "SELECT " + Heading.Substring(0, Heading.Length - 2);
                }
                Query = Query.Replace(ReplaceQry, UpdateQuery);
            }
            return Query;
        }

        public static string ReplaceGuestUser(string Query, string CurrentGuestUser)
        {
            if (Query.IndexOf("{CurrentGuestUser}", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Regex.Replace(Query, "{CurrentGuestUser}", "'" + CurrentGuestUser + "'", RegexOptions.IgnoreCase);
            }
            return Query;
        }

        public static string GetLeadDetails(int LeadID, int OrgID)
        {
            ModelDbContext dbcontext = new ModelDbContext();
            var Org = dbcontext.Organization.Find(OrgID);
            DataContext Spdb = new DataContext(Org.DatabaseName);
            string EmailID = Spdb.Database.SqlQuery<string>("SELECT sEMail FROM " + EnumLeadTables.Leads.ToString() + " WHERE ID='" + LeadID + "'").FirstOrDefault();
            return EmailID;
        }
        public static string ReplaceForeginKeyValues(VMDropDown Keys, string ID, string database)
        {
            ModelDbContext dbcontext = new ModelDbContext();
            string sDatabase = "XIDemo";
            ModelDbContext dbContext = new ModelDbContext();
            DataContext Spdb = new DataContext(database);
            string KeyValue = "";
            try
            {
                int PID = Convert.ToInt32(ID);
                if (Keys.text.Equals("FKiLeadClassID", StringComparison.InvariantCultureIgnoreCase))
                {
                    KeyValue = Spdb.OrganizationClasses.Where(m => m.ClassID == PID).Select(m => m.Class).FirstOrDefault();
                }
                else if (Keys.text.Equals("iTeamID", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = Spdb.OrganizationTeams.Where(m => m.ID == PID).Select(m => m.Name).FirstOrDefault();
                }
                else if (Keys.text.Equals("FKiSourceID", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = Spdb.OrganizationSources.Where(m => m.ID == PID).Select(m => m.Name).FirstOrDefault();
                }
                else if (Keys.text.Equals("iStatus", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = dbContext.BOOptionLists.Where(m => m.sValues == PID.ToString()).Select(m => m.sOptionName).FirstOrDefault();
                }
                else if (Keys.text.Equals("iCover", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = dbContext.BOOptionLists.Where(m => m.sValues == PID.ToString()).Select(m => m.sOptionName).FirstOrDefault();
                }
                else if (Keys.text.Equals("UserID", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = dbcontext.XIAppUsers.Where(m => m.UserID == PID).Select(m => m.sFirstName).FirstOrDefault();
                }
                else if (Keys.text.Equals("FKiOrgID", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = dbcontext.Organization.Where(m => m.ID == PID).Select(m => m.Name).FirstOrDefault();
                }
                else if (Keys.text.Equals("FKiOrderClassID", StringComparison.OrdinalIgnoreCase))
                {
                    KeyValue = dbcontext.Organization.Where(m => m.ID == PID).Select(m => m.Name).FirstOrDefault();
                }
                //else if (Keys.text.Equals("FKiClientID", StringComparison.OrdinalIgnoreCase))
                //{
                //    KeyValue = dbContext.cClients.Where(m => m.ID == PID).Select(m => m.sName).FirstOrDefault();
                //}
            }
            catch (Exception ex)
            {
                var exp = ex;
                return ID;
            }
            return KeyValue;
        }
        public static string AddSearchParameters(string Query, string SearchText)
        {
            if (SearchText != null && SearchText.Length > 0)
            {
                //Declaration
                string GroupBY = "", Condition = "";
                int pos, Qlength;
                //Query Modification 
                if (Query.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pos = Query.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
                    Qlength = Query.Length;
                    GroupBY = Query.Substring(pos);
                    GroupBY = GroupBY.Insert(0, " ");
                    Query = Query.Substring(0, pos - 1);
                }
                else if (Query.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    pos = Query.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
                    Qlength = Query.Length;
                    GroupBY = Query.Substring(pos);
                    GroupBY = GroupBY.Insert(0, " ");
                    Query = Query.Substring(0, pos - 1);
                }
                if (SearchText.Length > 0)
                {
                    Condition = Condition + SearchText;
                    if (Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        Query = Query + " WHERE " + Condition;
                        Query = string.Concat(Query, GroupBY);
                    }
                    else if (Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) < 0) // WHERE ==0
                    {
                        Query = Query + " WHERE " + Condition;
                        Query = string.Concat(Query, GroupBY);
                    }
                    return Query;
                }
                else
                {
                    return Query;
                }
            }
            else
            {
                return Query;
            }
        }
        public static string GetDynamicSearchStrings(string Fields, string Optrs, string Values)
        {
            string condition = string.Empty;
            var SFields = Fields.Split(',').ToList();
            var SOptrs = Optrs.Split(',').ToList();
            var SValues = Values.Split(',').ToList();
            for (int i = 0; i < SFields.Count(); i++)
            {
                if (SFields[i].ToLower() == "parentboname")
                {
                    if (SValues[i] != "0")
                    {
                        CXiAPI oXIAPI = new CXiAPI();
                        var BODef = oXIAPI.Get_BODefinition(SValues[i], 0, null, null);
                        var ParentBOTableName = BODef.TableName;
                        var CBODef = oXIAPI.Get_BODefinition(SValues[2], 0, null, null);
                        var fk = CBODef.BOFields.Where(m => m.FKTableName == ParentBOTableName).Select(m => m.Name).FirstOrDefault();
                        if (fk != null)
                        {
                            condition = condition + fk + "=" + SValues[i + 1];
                            return condition;
                        }
                    }
                    else
                    {
                        return condition;
                    }
                }
                else if (SValues[i] != "" && SValues[i].Length > 0)
                {
                    condition = condition + SFields[i];
                    if (SOptrs[i] == "STARTS WITH")
                    {
                        condition = condition + " LIKE '" + SValues[i] + "%' AND ";
                    }
                    else if (SOptrs[i] == "NOT STARTS WITH")
                    {
                        condition = condition + " NOT LIKE '" + SValues[i] + "%' AND ";
                    }
                    else if (SOptrs[i] == "ENDS WITH")
                    {
                        condition = condition + " LIKE '%" + SValues[i] + "' AND ";
                    }
                    else if (SOptrs[i] == "NOT ENDS WITH")
                    {
                        condition = condition + " NOT LIKE '%" + SValues[i] + "' AND ";
                    }
                    else if (SOptrs[i] == "CONTAINS")
                    {
                        condition = condition + " LIKE '%" + SValues[i] + "%' AND ";
                    }
                    else if (SOptrs[i] == "Between")
                    {
                        var Dates = SValues[i].Split('_').ToList();
                        condition = condition + " >= '" + Convert.ToDateTime(Dates[0]).ToString("M/dd/yy") + "' AND " + SFields[i] + " <= '" + Convert.ToDateTime(Dates[1]).ToString("M/dd/yy") + "' AND ";
                    }
                    else
                    {
                        condition = condition + " " + SOptrs[i] + " '" + SValues[i] + "' AND ";

                    }
                }
            }
            if (condition.Length > 0)
            {
                condition = condition.Substring(0, condition.Length - 5);
            }
            return condition;
        }
        public static string AddParentWhereConditon(string Query, Guid ReportID)
        {
            ModelDbContext dbcontext = new ModelDbContext();
            var Report = dbcontext.Reports.Where(x => x.XIGUID == ReportID).Select(t => t).FirstOrDefault();
            string ParentWherePart = "";
            var ChildQuery = Query;
            string NewChild = "";
            var ParentQuery = dbcontext.Reports.Where(m => m.ID == Report.ParentID).Select(m => m.Query).FirstOrDefault();
            if (ParentQuery.IndexOf("where", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var WhereIndex = ParentQuery.IndexOf("where", StringComparison.OrdinalIgnoreCase) + 6;
                if (ParentQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var groupIndex = ParentQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase) - 1;
                    ParentWherePart = ParentQuery.Substring(WhereIndex, groupIndex - WhereIndex);
                }
                else if (ParentQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    var orderIndex = ParentQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase) - 1;
                    ParentWherePart = ParentQuery.Substring(WhereIndex, orderIndex - WhereIndex);
                }
                else
                {
                    ParentWherePart = ParentQuery.Substring(WhereIndex, ParentQuery.Length - WhereIndex);
                }
                string GroupPart = "", OrderPart = "", NewQuery = "";

                if (ChildQuery.IndexOf("where", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (ChildQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var groupIndex = ChildQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase);
                        NewQuery = ChildQuery.Substring(0, groupIndex);
                        GroupPart = ChildQuery.Substring(groupIndex, ChildQuery.Length - groupIndex);
                        NewChild = NewQuery + " AND " + ParentWherePart + GroupPart;
                    }
                    else if (ChildQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var orderIndex = ChildQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                        NewQuery = ChildQuery.Substring(0, orderIndex);
                        OrderPart = ChildQuery.Substring(orderIndex, ChildQuery.Length - orderIndex);
                        NewChild = NewQuery + " AND " + ParentWherePart + " " + OrderPart;
                    }
                    else
                    {
                        NewChild = ChildQuery + " AND " + ParentWherePart;
                    }
                }
                else
                {
                    if (ChildQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var groupIndex = ChildQuery.IndexOf("group by", StringComparison.OrdinalIgnoreCase);
                        NewQuery = ChildQuery.Substring(0, groupIndex);
                        GroupPart = ChildQuery.Substring(groupIndex, ChildQuery.Length - groupIndex);
                        NewChild = NewQuery + " Where " + ParentWherePart + GroupPart;
                    }
                    else if (ChildQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        var orderIndex = ChildQuery.IndexOf("order by", StringComparison.OrdinalIgnoreCase);
                        NewQuery = ChildQuery.Substring(0, orderIndex);
                        OrderPart = ChildQuery.Substring(orderIndex, ChildQuery.Length - orderIndex);
                        NewChild = NewQuery + " Where " + ParentWherePart + " " + OrderPart;
                    }
                    else
                    {
                        NewChild = " Where " + ParentWherePart;
                    }
                }
            }
            return NewChild;
        }
        public static List<VMDropDown> GetOrgClasses(int OrgID, string sDatabase)
        {
            ModelDbContext dbcontext = new ModelDbContext(sDatabase);
            List<VMDropDown> AllClasses = new List<VMDropDown>();
            if (OrgID > 0)
            {
                var database = dbcontext.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                DataContext Spdb = new DataContext(database);
                AllClasses = (from c in Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList()
                              select new VMDropDown { Value = c.ClassID, text = c.Class }).ToList();

            }
            else
            {
                AllClasses = (from c in dbcontext.Types.Where(m => m.Name == "Class Type").ToList()
                              select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            }
            return AllClasses;
        }
        public static string ModifyQuery(string Query, int OrganizationID, string UserIDs, int? dropdownValue, int? dropdownDate)
        {
            //Declaration
            string Q1 = "", Condition = "";
            int pos, Qlength;
            DateTime dToday = DateTime.Now, dPastMonth, dPastWeek, dtoday0, dpastweek0, dpastmonth0;
            dtoday0 = new DateTime(dToday.Year, dToday.Month, dToday.Day, 00, 00, 00);
            dPastWeek = DateTime.Now.AddDays(-7);
            dpastweek0 = new DateTime(dPastWeek.Year, dPastWeek.Month, dPastWeek.Day, 00, 00, 00);
            dPastMonth = DateTime.Now.AddMonths(-1);
            dpastmonth0 = new DateTime(dPastMonth.Year, dPastMonth.Month, dPastMonth.Day, 00, 00, 00);
            //Query Modification 
            if (Query.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                pos = Query.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
                Qlength = Query.Length;
                Q1 = Query.Substring(pos);
                Q1 = Q1.Insert(0, " ");
                Query = Query.Substring(0, pos - 1);
            }
            if (dropdownValue != 0 && dropdownDate != 0)//Class=value; Date=value;
            {
                if (dropdownDate == 1)//Today
                {
                    Condition += " FKiLeadClassID=" + dropdownValue + " AND (dImportedOn >='" + dtoday0 + "' AND dImportedOn <='" + dToday + "')";
                }
                else if (dropdownDate == 2)//Past 1 Week
                {
                    Condition += " FKiLeadClassID=" + dropdownValue + " AND (dImportedOn >='" + dpastweek0 + "' AND dImportedOn <='" + dToday + "')";
                }
                else if (dropdownDate == 3)//Past 1 Month
                {
                    Condition += " FKiLeadClassID=" + dropdownValue + " AND (dImportedOn >='" + dpastmonth0 + "' AND dImportedOn <='" + dToday + "')";
                }
            }
            else if (dropdownValue != 0 && dropdownDate == 0) //CLass=value; Date=0;
            {
                Condition += "";
                Condition += " FKiLeadClassID=" + dropdownValue;
            }
            else if (dropdownValue == 0 && dropdownDate != 0) // Class=0; Date=value;
            {
                if (dropdownDate == 1)//Today
                {
                    Condition += " (dImportedOn >='" + dtoday0 + "' AND dImportedOn <='" + dToday + "')";
                }
                else if (dropdownDate == 2)//Past 1 Week
                {
                    Condition += " (dImportedOn >='" + dpastweek0 + "' AND dImportedOn <='" + dToday + "')";
                }
                else if (dropdownDate == 3)//Past 1 Month
                {
                    Condition += " (dImportedOn >='" + dpastmonth0 + "' AND dImportedOn <='" + dToday + "')";
                }
            }

            if (dropdownDate > 0 || dropdownValue > 0)
            {
                if (Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) > 0)
                {
                    Query = Query + " AND" + Condition;
                    Query = string.Concat(Query, Q1);
                }
                else if (Query.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) < 0) // WHERE ==0
                {
                    Query = Query + " WHERE" + Condition;
                    Query = string.Concat(Query, Q1);
                }
            }
            else
            {
                Query = string.Concat(Query, Q1);
            }
            return Query;
        }

        public static string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null)
            {
                return string.Empty;
            }
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }
    }
}
