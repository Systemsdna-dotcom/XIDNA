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
using System.Net;
using System.Data;

namespace XIDNA.Repository
{
    public class GenerateQueryRepository : IGenerateQueryRepository
    {
        CommonRepository Common = new CommonRepository();
        CXiAPI oXIAPI = new CXiAPI();
        public DTResponse GetQueryList(jQueryDataTableParamModel param, int iUserID, string sOrgName, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            IQueryable<VMReports> AllReports;
            AllReports = (from r in dbContext.Reports.Where(m => m.FKiApplicationID == FKiAppID)
                          join t in dbContext.Types on r.Class equals t.ID into p
                          join b in dbContext.BOs on r.BOID equals b.BOID
                          from rt in p.DefaultIfEmpty()
                          select new VMReports
                          {
                              ID = r.ID,
                              OrganizationID = r.OrganizationID,
                              FKiApplicationID = r.FKiApplicationID,
                              Name = r.Name,
                              FromBos = r.FromBos,
                              Description = r.Description,
                              Status = r.StatusTypeID,
                              IsParent = r.IsParent,
                              ClassID = r.Class,
                              ClassName = rt.Expression,
                              Type = ((EnumDisplayTypes)r.DisplayAs).ToString(),
                              BOID = r.BOID,
                              ParentID = r.ParentID == null ? 0 : r.ParentID,
                              ShowAs = r.Title,
                              BO = b.Name
                          });
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllReports = AllReports.Where(m => m.Name.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllReports.Count();
            AllReports = QuerableUtil.GetResultsForDataTables(AllReports, "", sortExpression, param);
            var clients = AllReports.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.OrganizationID.ToString(),c.ParentID.ToString(),c.BO, c.Name, c.ShowAs, c.Description, c.IsParent.ToString(), c.ClassName, c.Type, c.Status.ToString(), "","" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetClassName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            if (p > 0)
            {
                var ClassName = dbContext.Types.Where(m => m.ID == p).Select(m => m.Expression).FirstOrDefault();
                return ClassName;
            }
            else
            {
                return "";
            }
        }

        public string UpdateQuery(VMReports model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Reports report = dbContext.Reports.Where(m => m.XIGUID == model.XIGUID).FirstOrDefault();
            string Query = model.Query;
            string VisibleQuery = model.Query;
            report.Query = model.Query;
            report.VisibleQuery = model.Query;
            report.bIsMultiBO = model.bIsMultiBO;
            report.FromBos = model.FromBos;
            if (model.SelectFields == null)
                report.SelectFields = null;
            else
                report.SelectFields = model.SelectFields;
            if (model.WhereFields == null)
                report.WhereFields = null;
            else
                report.WhereFields = model.WhereFields;
            if (model.GroupFields == null)
                report.GroupFields = null;
            else
                report.GroupFields = model.GroupFields;
            if (model.OrderFields == null)
                report.OrderFields = null;
            else
                report.OrderFields = model.OrderFields;
         
             dbContext.SaveChanges();
            if (model.ActionFieldValue != null)
            {
                Popup popup = new Popup();
                popup = dbContext.Popups.Where(m => m.Name == model.ActionFieldValue).FirstOrDefault();
                if (popup == null)
                {
                    Popup Newpopup = new Popup();
                    string PopupName = model.ActionFieldValue;
                    Newpopup.Name = PopupName;
                    Newpopup.StatusTypeID = 10;
                    dbContext.Popups.Add(Newpopup);
                    dbContext.SaveChanges();
                }
            }
            if (report.IsStoredProcedure == true)
            {
                model.Query = report.Query;
                List<string> SelectFields = new List<string>();
                List<string> WhereFields = new List<string>();
                if (model.SelWithTypes != null)
                {
                    string selfields = model.SelWithTypes.Substring(0, model.SelWithTypes.Length - 1);
                    SelectFields = selfields.Split(',').ToList();
                }
                if (model.WhereWithTypes != null)
                {
                    if (model.WhereWithTypes.IndexOf('-') > 0)
                    {
                        string wherefields = model.WhereWithTypes.Substring(0, model.WhereWithTypes.Length - 1);
                        WhereFields = wherefields.Split(',').ToList();
                    }
                    else
                    {
                        string wherefields = model.WhereWithTypes.Substring(0, model.WhereWithTypes.Length - 1);
                        WhereFields = wherefields.Split(new string[] { ", " }, StringSplitOptions.None).ToList();
                    }
                }
                string proccondition = "", wherecondition = "";
                if (WhereFields.Count() > 0)
                {
                    WhereFields = WhereFields.Distinct().ToList();
                    foreach (var items in WhereFields)
                    {
                        if (items.IndexOf('-') > 0)
                        {
                            var item = items.Split('-').ToList();
                            if (item[1] == "VARCHAR")
                                proccondition = proccondition + "@" + item[0] + " " + item[1].ToLower() + "(max)" + ", ";
                            else
                                proccondition = proccondition + "@" + item[0] + " " + item[1].ToLower() + ", ";
                        }
                        else
                        {
                            int TypeID = dbContext.BOFields.Where(m => m.BOID == model.BOID).Where(m => m.Name == items).Select(m => m.TypeID).FirstOrDefault();
                            string type = ((BODatatypes)TypeID).ToString();
                            if (type == "VARCHAR")
                                proccondition = proccondition + "@" + items + " " + type.ToLower() + "(max)" + ", ";
                            else
                                proccondition = proccondition + "@" + items + " " + type.ToLower() + ", ";
                        }

                    }
                    proccondition = proccondition.Substring(0, proccondition.Length - 2);
                }
                int ands = 0, ors = 0;
                if (model.Query != null)
                {
                    if (model.Query.IndexOf("WHERE") > 0)
                    {
                        if (model.WhereFields.IndexOf("AND") > 0)
                            ands = model.WhereFields.Split(new string[] { "AND" }, StringSplitOptions.None).Length - 1;
                        else
                            ands = 0;
                        if (model.WhereFields.IndexOf("OR") > 0)
                            ors = model.WhereFields.Split(new string[] { "OR" }, StringSplitOptions.None).Length - 1;
                        else
                            ors = 0;
                    }
                }
                int totoprts = ands + ors;
                string wherestring = model.WhereFields;
                List<string> whererows = new List<string>();
                if (totoprts > 0)
                {
                    for (int i = 0; i < totoprts; i++)
                    {
                        var andindex = 0; var orindex = 0;
                        if (wherestring.IndexOf("AND") > 0)
                        {
                            andindex = wherestring.IndexOf("AND");
                        }
                        if (wherestring.IndexOf("OR") > 0)
                        {
                            orindex = wherestring.IndexOf("OR");
                        }
                        if (andindex > 0 && orindex > 0)
                        {
                            if (andindex < orindex)
                            {
                                var andstring = wherestring.Substring(0, andindex - 1);
                                wherestring = wherestring.Substring(andindex + 4);
                                whererows.Add(andstring);
                                whererows.Add("AND");
                            }
                            else
                            {
                                var andstring = wherestring.Substring(0, orindex - 1);
                                wherestring = wherestring.Substring(orindex + 3);
                                whererows.Add(andstring);
                                whererows.Add("OR");
                            }
                        }
                        else if (andindex > 0)
                        {
                            var andstring = wherestring.Substring(0, andindex - 1);
                            wherestring = wherestring.Substring(andindex + 4);
                            whererows.Add(andstring);
                            whererows.Add("AND");
                        }
                        else if (orindex > 0)
                        {
                            var andstring = wherestring.Substring(0, orindex - 1);
                            wherestring = wherestring.Substring(orindex + 3);
                            whererows.Add(andstring);
                            whererows.Add("OR");
                        }
                        if (i == totoprts - 1)
                        {
                            whererows.Add(wherestring);
                        }
                    }
                }
                if (ands == 0 && ors == 0 && wherestring != null)
                {
                    whererows.Add(wherestring);
                }
                if (whererows.Count() > 0)
                {
                    for (int j = 0; j < whererows.Count(); j++)
                    {
                        if (whererows[j].IndexOf(' ') > 0)
                        {
                            List<string> conditon = whererows[j].Split(' ').ToList();
                            wherecondition = wherecondition + conditon[0] + " " + conditon[1] + " @" + conditon[0] + " ";
                        }
                        else
                        {
                            wherecondition = wherecondition + whererows[j] + " ";
                        }
                    }
                    wherecondition = wherecondition.Substring(0, wherecondition.Length - 1);
                }
                string query = "";
                if (model.Query.IndexOf("SELECT") >= 0)
                {
                    query = query + "SELECT " + model.SelectFields;
                }
                query = query + " FROM " + model.FromBos;
                if (model.Query.IndexOf("WHERE") > 0)
                {
                    query = query + " WHERE " + wherecondition;
                }
                if (model.Query.IndexOf("GROUP BY") > 0)
                {
                    query = query + " GROUP BY " + model.GroupFields;
                }
                if (model.Query.IndexOf("ORDER BY") > 0)
                {
                    query = query + " ORDER BY " + model.OrderFields;
                }
                string sp = "CREATE PROCEDURE" + " " + model.Name.Replace(" ", "_") + " " + proccondition + " AS " + query + " RETURN";
                string dropsp = "DROP PROCEDURE " + model.Name.Replace(" ", "_");
                var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                var sOrgDB = UserDetails.sUserDatabase;
                int iBOID = report.BOID;
                var oBO = dbContext.BOs.Find(iBOID);
                var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    string procedure = sp;
                    cmd.Connection = Con;
                    try
                    {
                        cmd.CommandText = dropsp;
                        cmd.ExecuteNonQuery();
                        cmd.CommandText = procedure;
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        cmd.CommandText = procedure;
                        cmd.ExecuteNonQuery();
                    }
                    Con.Close();
                }
            }
            return model.XIGUID.ToString();
        }
        public VMReports GetQueryByID(string QueryID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMReports Rep = new VMReports();
            Reports model = new Reports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                model = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: GetQueryByID: " + QueryID, "");
            }
            if (QueryIDGUID != null && QueryIDGUID != Guid.Empty)
            {
                model = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            BOs oBO = new BOs();
            if (model.BOID > 0)
            {
                oBO = dbContext.BOs.Find(model.BOID);
            }
            else
            {
                oBO = dbContext.BOs.Where(k => k.XIGUID == model.BOIDXIGUID).Select(n => n).FirstOrDefault();//Find(model.BOIDXIGUID);
                Rep.BOIDXIGUID = model.BOIDXIGUID==null?Guid.Empty:model.BOIDXIGUID;
            }
            Rep.ID = model.ID;
            Rep.OrganizationID = model.OrganizationID;
            Rep.FKiApplicationID = model.FKiApplicationID;
            Rep.CategoryID = model.CategoryID;
            Rep.BOID = model.BOID;
            Rep.BO = oBO.Name;
            Rep.ParentID = model.ParentID;
            Rep.Name = model.Name;
            Rep.Code = model.Code;
            Rep.Title = model.Title;
            Rep.TypeID = model.TypeID;
            Rep.Query = model.Query;
            Rep.bIsMultiBO = model.bIsMultiBO;
            Rep.bIsMultiSearch = model.bIsMultiSearch;
            Rep.VisibleQuery = model.VisibleQuery;
            Rep.DisplayAs = model.DisplayAs;
            Rep.ResultListDisplayType = model.ResultListDisplayType;
            Rep.ResultIn = model.ResultIn;
            Rep.Class = model.Class;
            Rep.IsDynamic = model.IsDynamic;
            Rep.IsStoredProcedure = model.IsStoredProcedure;
            Rep.SelectFields = model.SelectFields;
            Rep.FromBos = model.FromBos;
            Rep.WhereFields = model.WhereFields;
            Rep.GroupFields = model.GroupFields;
            Rep.OrderFields = model.OrderFields;
            Rep.SearchFields = model.SearchFields;
            Rep.InnerReportID = model.InnerReportID;
            Rep.ActionFields = model.ActionFields;
            Rep.ActionFieldValue = model.ActionFieldValue;
            Rep.EditableFields = model.EditableFields;
            Rep.Description = model.Description;
            Rep.StatusTypeID = model.StatusTypeID;
            Rep.IsFilterSearch = model.IsFilterSearch;
            Rep.IsNaturalSearch = model.IsNaturalSearch;
            Rep.IsParent = model.IsParent;
            Rep.IsExport = model.IsExport;
            Rep.bIsLockToUser = model.bIsLockToUser;
            Rep.sLog = model.sLog;
            Rep.IsRowClick = model.IsRowClick;
            Rep.iPaginationCount = model.iPaginationCount;
            if (Rep.IsRowClick == true)
            {
                Rep.RowXiLinkID = model.RowXiLinkID;
                Rep.sRowXiLinkType = model.sRowXiLinkType;
            }
            Rep.OnRowClickType = model.OnRowClickType;
            Rep.OnRowClickValue = model.OnRowClickValue;
            Rep.IsColumnClick = model.IsColumnClick;
            if (Rep.IsColumnClick == true)
            {
                Rep.ColumnXiLinkID = model.ColumnXiLinkID;
            }
            Rep.OnClickColumn = model.OnClickColumn;
            Rep.OnColumnClickType = model.OnColumnClickType;
            Rep.OnClickParameter = model.OnClickParameter;
            Rep.OnColumnClickValue = model.OnColumnClickValue;
            Rep.IsCellClick = model.IsCellClick;
            if (Rep.IsCellClick == true)
            {
                Rep.CellXiLinkID = model.CellXiLinkID;
            }
            Rep.IsCreate = model.IsCreate;
            if (Rep.IsCreate == true)
            {
                Rep.CreateRoleID = model.CreateRoleID;
                Rep.CreateGroupID = model.CreateGroupID;
                Rep.iCreateXILinkID = model.iCreateXILinkID;
                Rep.sAddLabel = model.sAddLabel;
                Rep.sCreateType = model.sCreateType;
            }
            Rep.IsRefresh = model.IsRefresh;
            Rep.bIsCheckbox = model.bIsCheckbox;
            Rep.bIsCopy = model.bIsCopy;
            Rep.bIsView = model.bIsView;
            Rep.IsEdit = model.IsEdit;
            if (Rep.IsEdit == true)
            {
                Rep.EditRoleID = model.EditRoleID;
                Rep.EditGroupID = model.EditGroupID;
            }
            Rep.IsDelete = model.IsDelete;
            if (Rep.IsDelete == true)
            {
                Rep.DeleteRoleID = model.DeleteRoleID;
            }
            Rep.OnClickCell = model.OnClickCell;
            Rep.OnCellClickType = model.OnCellClickType;
            Rep.OnCellClickValue = model.OnCellClickValue;
            Rep.IsRowTotal = model.IsRowTotal;
            Rep.IsColumnTotal = model.IsColumnTotal;
            Rep.PopupType = model.PopupType;
            Rep.PopupLeft = model.PopupLeft;
            Rep.PopupTop = model.PopupTop;
            Rep.PopupWidth = model.PopupWidth;
            Rep.PopupHeight = model.PopupHeight;
            if (model.ResultIn == "Dialog")
            {
                Rep.DialogType = model.PopupType;
            }
            Rep.DialogAt1 = model.DialogAt1;
            Rep.DialogAt2 = model.DialogAt2;
            Rep.DialogMy1 = model.DialogMy1;
            Rep.DialogMy2 = model.DialogMy2;
            Rep.ViewFields = model.ViewFields;
            Rep.RepeaterComponent = model.RepeaterComponentID;
            Rep.FKiComponentID = model.FKiComponentID;
            Rep.FKiVisualisationID = model.FKiVisualisationID;
            Rep.RepeaterType = model.RepeaterType;
            Rep.bIsXICreatedBy = model.bIsXICreatedBy;
            Rep.FKiCrtd1ClickID = model.FKiCrtd1ClickID;
            Rep.bIsXIUpdatedBy = model.bIsXIUpdatedBy;
            Rep.FKiUpdtd1ClickID = model.FKiUpdtd1ClickID;
            Rep.XIGUID = model.XIGUID;
            var GroupFields = oBO.BOGroups.ToList();
            List<BOFields> BoFields = new List<BOFields>();
            if (oBO.BOFields != null)
            {
                 BoFields = oBO.BOFields.Where(m => m.BOID == model.BOID).ToList();
            }
            var VisibleQuery = model.Query;
            if (string.IsNullOrEmpty(Rep.SelectFields))
            {
                string sSelectFields = string.Empty;
                if (!string.IsNullOrEmpty(Rep.Query))
                {
                    var FromIndex = Rep.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
                    var SelectQuery = Rep.Query.Substring(0, FromIndex);
                    SelectQuery = SelectQuery.TrimEnd();
                    sSelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase);
                    Rep.SelectFields = sSelectFields;
                    model.SelectFields = sSelectFields;
                }
            }
            var res2 = model.SelectFields;
            var SelForGroup = model.SelectFields;
            var res4 = model.SelectFields;
            var splitfields = new List<string>();
            if (res2 != null)
            {
                if (res2.Length != 0)
                {
                    splitfields = res2.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

            }
            var regex = new Regex("{.*?}");
            //var matches = regex.Matches(model.Query); //your matches: name, name@gmail.com
            string[] result;
            List<groups> groupa = new List<groups>();
            string selectwithalias = "";
            foreach (var item in splitfields)
            {
                if (item.Contains('{'))
                {
                    result = item.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                    //var id = Convert.ToInt32(result[0]);
                    int id = 0;
                    if (result.Count() > 0)
                    {
                        var sNewGroup = result[0];
                        id = oBO.BOGroups.Where(m => m.GroupName == sNewGroup).Select(m => m.ID).FirstOrDefault();
                    }
                    BOGroupFields groupname = GroupFields.Where(m => m.ID == id).FirstOrDefault();
                    if (groupname != null)
                    {
                        VisibleQuery = VisibleQuery.Replace('{' + result[0] + '}', groupname.BOSqlFieldNames);
                        res2 = res2.Replace('{' + result[0] + '}', groupname.GroupName);
                        SelForGroup = SelForGroup.Replace('{' + result[0] + '}', groupname.BOFieldNames);
                        res4 = res4.Replace('{' + result[0] + '}', groupname.BOSqlFieldNames);
                        selectwithalias = selectwithalias + "{" + id + "}, ";
                        groupa.Add(new groups
                        {
                            groupid = groupname.ID,
                            groupname = groupname.GroupName,
                            bofieldnames = groupname.BOFieldNames,
                            bosqlfieldnames = groupname.BOSqlFieldNames,
                            singlefield = item
                        });
                    }
                }
                else if (item.Contains("COUNT"))
                {
                    selectwithalias = selectwithalias + item + " AS '" + item + "', ";
                    groupa.Add(new groups
                    {
                        singlefield = item,
                        singlefieldtype = "INT",
                        singlealiasname = item,
                        groupid = null,
                        groupname = "",
                        bofieldnames = "",
                        bosqlfieldnames = ""
                    });
                }
                else
                {
                    var name = item.Split(new string[] { " AS " }, StringSplitOptions.RemoveEmptyEntries)[0];
					if (BoFields != null)
                    {
						int TypeID = BoFields.Where(m => m.Name.ToLower() == name.ToLower()).Select(m => m.TypeID).FirstOrDefault();
						string fieldname = BoFields.Where(m => m.Name.ToLower() == name.ToLower()).Select(m => m.LabelName).FirstOrDefault();
						string type = ((BODatatypes)TypeID).ToString();
						//fieldname = fieldname.Replace(" ", "_");
						selectwithalias = selectwithalias + name + " AS '" + fieldname + "', ";
						groupa.Add(new groups
						{
							singlefield = name,
							singlefieldtype = type,
							singlealiasname = fieldname,
							groupid = null,
							groupname = "",
							bofieldnames = "",
							bosqlfieldnames = ""
						});
					}
                }
            }

            if (VisibleQuery != null)
            {
                //Rep.generalquery = VisibleQuery.Replace("_", " ").Replace("\r\n", " ");
                Rep.generalquery = VisibleQuery.Replace("\r\n", " ");
            }
            else
            {
                string BoName = "";
                if (!string.IsNullOrEmpty(oBO.TableName))
                {
                    Rep.generalquery = " FROM " + oBO.TableName;
                    Rep.FromBos = oBO.TableName;
                }
                else
                {
                    Rep.generalquery = " FROM " + oBO.Name;
                    Rep.FromBos = oBO.Name;
                }

            }
            Rep.generalselectedfields = res2;
            Rep.groupallfields = groupa;
            if (SelForGroup != null && SelForGroup.Length > 0)
            {
                var GrFields = SelForGroup.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                SelForGroup = "";
                foreach (var items in GrFields)
                {
                    if (items.IndexOf(" AS ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        string fld = items.Split(new string[] { " AS " }, StringSplitOptions.RemoveEmptyEntries)[0];
                        SelForGroup = SelForGroup + fld + ", ";
                    }
                    else
                    {
                        SelForGroup = SelForGroup + items + ", ";
                    }
                }
                SelForGroup = SelForGroup.Substring(0, SelForGroup.Length - 2);
            }
            else
            {
                SelForGroup = Rep.GroupFields;
            }
            Rep.selectedgroupfields = SelForGroup;
            if (res4 != null)
            {
                if (res4.Length != 0)
                {
                    //Rep.selectedfields = res4.Replace('_', ' ');
                    if (!string.IsNullOrEmpty(selectwithalias))
                    {
                        Rep.SelectWithAlias = selectwithalias.Substring(0, selectwithalias.Length - 2);
                    }
                }
            }
            Rep.parent = dbContext.Reports.Where(m => m.ID == model.ParentID).Select(m => m.Name).FirstOrDefault();
            if (string.IsNullOrEmpty(Rep.FromBos))
            {
                Rep.FromBos = Rep.BO;
            }

            return Rep;
        }
        public VMQueryActions GetActionFeildsByID(string QueryID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMQueryActions ActionFields = new VMQueryActions();
            Reports model = new Reports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                model = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: GetActionFeildsByID: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                model = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            var BoFields = dbContext.BOFields.Where(m => m.BOID == model.BOID).ToList();
            var Groups = dbContext.BOGroupFields.Where(m => m.BOID == model.BOID).ToList();
            List<VMDropDown> RFields = new List<VMDropDown>();
            var grname = model.EditableFields;
            if (model.EditableFields != null)
            {
                var EFields = model.EditableFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in EFields)
                {
                    if (items.Contains('-'))
                    {
                        var name = items.Replace("NE-", "");
                        if (name.Contains('{'))
                        {
                            var result = name.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields Field = Groups.Where(m => m.ID == id).FirstOrDefault();
                            if (Field != null)
                            {
                                grname = grname.Replace('{' + result[0] + '}', Field.GroupName);
                                RFields.Add(new VMDropDown
                                {
                                    Value = Field.ID,
                                    text = Field.GroupName,
                                    Expression = "NonEditable",
                                    Type = "Group"
                                });
                            }
                        }
                        else
                        {
                            var id = items.Replace("NE-", "");
                            BOFields Field = BoFields.Where(m => m.Name == id && m.BOID == model.BOID).FirstOrDefault();
                            RFields.Add(new VMDropDown
                            {
                                text = name,
                                Expression = "NonEditable",
                                Value = Field.ID,
                                Type = "Field"
                            });
                        }
                    }
                    else
                    {
                        var name = items;
                        if (name.Contains('{'))
                        {
                            var result = name.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
                            var id = Convert.ToInt32(result[0]);
                            BOGroupFields Field = Groups.Where(m => m.ID == id).FirstOrDefault();
                            if (Field != null)
                            {
                                grname = grname.Replace('{' + result[0] + '}', Field.GroupName);
                                RFields.Add(new VMDropDown
                                {

                                    Value = Field.ID,
                                    text = Field.GroupName,
                                    Expression = "Editable",
                                    Type = "Group"
                                });
                            }
                        }
                        else
                        {
                            var id = items;
                            BOFields Field = BoFields.Where(m => m.Name == id && m.BOID == model.BOID).FirstOrDefault();
                            RFields.Add(new VMDropDown
                            {
                                Value = Field.ID,
                                text = name,
                                Expression = "Editable",
                                Type = "Field"
                            });
                        }
                    }
                }
            }
            ActionFields.RightEditFields = RFields;
            var Filtered = (from c in BoFields.Where(x => !RFields.Any(y => y.Value == x.ID)).ToList()
                            select new VMDropDown
                            {
                                Value = c.ID,
                                text = c.Name
                            }).ToList();
            //Filtered.AddRange((from c in Groups
            //                  select new VMDropDown { Value = c.ID, text = c.GroupName, Type = "Group" }
            //                      ));
            ActionFields.LeftEditFields = Filtered;
            return ActionFields;
        }

        public VMReports GetXiLinksList(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMReports Model = new VMReports();
            List<VMDropDown> XiLinksList = new List<VMDropDown>();
            XiLinksList = (from c in dbContext.XiLinks.Where(m => m.StatusTypeID == 10).ToList()
                           select new VMDropDown { text = c.Name, Value = c.XiLinkID }).ToList();
            Model.XiLinksList = XiLinksList;
            return Model;
        }

        public VMReports GetXiParametersList(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMReports Model = new VMReports();
            List<VMDropDown> XiParameterList = new List<VMDropDown>();
            XiParameterList = (from c in dbContext.XiParameters.Where(m => m.StatusTypeID == 10).ToList()
                               select new VMDropDown { text = c.Name, Value = c.XiParameterID }).ToList();
            Model.XiLinksList = XiParameterList;
            return Model;
        }

        public List<VMDropDown> GetGroupsByBOID(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> Groups = dbContext.BOGroupFields.Where(m => m.BOID == BOID).ToList().Select(m => new VMDropDown { text = m.GroupName, Value = m.ID }).ToList();
            return Groups;
        }

        public VMReports GetAllBos(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            VMReports Model = new VMReports();
            var Users = dbCore.XIAppUsers.ToList();
            var Master = dbContext.Types.ToList();
            DataContext SpDb = new DataContext(sOrgDB);
            if (OrgID > 0)
            {
                string database = Users.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                SpDb = new DataContext(sOrgDB);
            }
            var Templates = dbContext.ContentEditors.ToList();
            List<VMDropDown> AllBos = new List<VMDropDown>();
            //var list = dbContext.BOs.ToList();
            AllBos = (from c in dbContext.BOs.ToList()
                          //select new VMDropDown { Value = c.BOID, text = c.TableName }).ToList();//Naresh
                      select new VMDropDown { Value = c.BOID, text = c.Name }).ToList();
            Model.AllBOs = AllBos;
            Dictionary<string, string> allbo = new Dictionary<string, string>();
            foreach (var item in Model.AllBOs)
            {
                allbo[item.Value.ToString()] = item.text;
            }
            Model.AllBOss = allbo;
            List<VMDropDown> Classes = new List<VMDropDown>();
            if (OrgID == 0)
            {
                Classes = (from c in Master.Where(m => m.Name == "Class Type" && m.Status == 10).ToList()
                           select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            }
            else
            {
                Classes = (from c in SpDb.OrganizationClasses.Where(m => m.OrganizationID == OrgID).ToList()
                           select new VMDropDown { text = c.Class, Value = c.ClassID }).ToList();
            }

            Model.Classes = Classes;
            List<VMDropDown> ReportTypes = new List<VMDropDown>();
            ReportTypes = (from c in Master.Where(m => m.Name == "Report Type" && m.Status == 10).ToList()
                           select new VMDropDown { text = c.Expression, Value = c.TypeID }).ToList();
            Model.ReportTypes = ReportTypes;
            List<VMDropDown> InnerReports = new List<VMDropDown>();
            int Type = (Int32)Enum.Parse(typeof(EnumDisplayTypes), EnumDisplayTypes.ResultList.ToString());
            InnerReports = (from c in dbContext.Reports.Where(m => m.Query != null && m.StatusTypeID == 10 && m.DisplayAs == Type).ToList()
                            select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            Model.InnerReports = InnerReports;
            //List<VMDropDown> PopupsList = new List<VMDropDown>();
            //PopupsList = (from c in dbContext.Popups.Where(m => m.StatusTypeID == 10)
            //              select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            //Model.PopupsList = PopupsList;
            List<VMDropDown> XiLinksList = new List<VMDropDown>();
            XiLinksList = (from c in dbContext.XiLinks.Where(m => m.StatusTypeID == 10).ToList()
                           select new VMDropDown { text = c.Name, Value = c.XiLinkID }).ToList();
            Model.XiLinksList = XiLinksList;
            Model.XiLinksList.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            List<VMDropDown> XIComponentList = new List<VMDropDown>();
            XIComponentList = (from c in dbContext.XIComponents.Where(m => m.StatusTypeID == 10).ToList()
                               select new VMDropDown { text = c.sName, Value = c.ID }).ToList();
            Model.XIComponentList = XIComponentList;
            Model.XIComponentList.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            List<VMDropDown> UsersList = new List<VMDropDown>();
            if (OrgID > 0)
            {
                UsersList = (from c in Users.Where(m => m.FKiOrganisationID == OrgID).ToList()
                             select new VMDropDown { text = c.sFirstName, Value = c.UserID }).ToList();
            }
            else
            {
                UsersList = (from c in Users.ToList()
                             select new VMDropDown { text = c.sFirstName, Value = c.UserID }).ToList();
            }
            Model.TargetUsersList = UsersList;
            List<VMDropDown> EmailTemplates = new List<VMDropDown>();
            EmailTemplates = (from c in Templates.Where(m => m.OrganizationID == OrgID && m.Category == 1).ToList()
                              select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            Model.EmailTemplates = EmailTemplates;
            List<VMDropDown> SMSTemplates = new List<VMDropDown>();
            SMSTemplates = (from c in Templates.Where(m => m.OrganizationID == OrgID && m.Category == 2).ToList()
                            select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            Model.SMSTemplates = SMSTemplates;
            List<string> Parent1Clicks = new List<string>();
            Parent1Clicks = dbContext.Reports.Where(m => m.Query != null && m.StatusTypeID == 10 && m.DisplayAs == Type && m.IsParent == true).Select(m => m.Name).ToList();
            Model.Parent1Clicks = Parent1Clicks;
            Model.ddlRoles = dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).ToList().Select(m => new VMDropDown { Value = m.RoleID, text = m.sRoleName }).ToList();
            Model.ddlRoles.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            List<VMDropDown> ddlOneClicks = new List<VMDropDown>();
            ddlOneClicks = (from c in dbContext.Reports.Where(m => m.StatusTypeID == 10 || m.StatusTypeID == 36).ToList()
                            select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            ddlOneClicks.Insert(0, new VMDropDown { Value = 0, text = "--Select--" });
            Model.ddlOneClicks = ddlOneClicks;
            return Model;
        }
        public List<BOFields> GetAvailableFields(int BOID, int Type, int ClassType, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            List<BOFields> AllBos = new List<BOFields>();
            List<BOFields> list = new List<BOFields>();
            if (BOID > 0)
            {
                var oBO = dbContext.BOs.Find(BOID);
                if (Type == 1)
                {
                    list = oBO.BOFields.Where(m => m.FieldClassID == 0 || m.FieldClassID == ClassType).ToList();
                }
                else
                {
                    list = oBO.BOFields.Where(m => m.FieldClass == null && m.FieldClassID == 0).ToList();
                }

                list = list.Select(m => { m.Type = GetFieldType(m.TypeID); return m; }).ToList();
                var Groups = dbContext.BOGroupFields.Where(m => m.BOID == BOID).ToList();
                List<string> GroupNames = Groups.Select(m => m.GroupName).ToList();
                List<string> GroupSqlFields = Groups.Select(m => m.BOSqlFieldNames).ToList();
                List<string> GroupFields = Groups.Select(m => m.BOFieldNames).ToList();
                List<int> GroupIDs = Groups.Select(m => m.ID).ToList();
                if (GroupNames.Count > 0)
                {
                    list.FirstOrDefault().GroupNames = GroupNames;
                    list.FirstOrDefault().GroupSqlFields = GroupSqlFields;
                    list.FirstOrDefault().GroupFields = GroupFields;
                    list.FirstOrDefault().GroupIDs = GroupIDs;
                }

                foreach (var item in list)
                {
                    if (item.FKTableName != string.Empty && item.FKTableName != null)
                    {
                        item.PKColumnID = dbContext.BOs.Where(m => m.TableName == item.FKTableName).Select(s => s.sPrimaryKey).FirstOrDefault();
                    }
                }

                if (OrgID > 0)
                {
                    var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                    DataContext Spdb = new DataContext(sOrgDB);
                    var SubFields = Spdb.SubscriptionColumns.Where(m => m.OrganizationID == OrgID).ToList();
                    foreach (var items in SubFields)
                    {
                        BOFields Field = new BOFields();
                        Field.ID = 0;
                        Field.Name = items.FieldName;
                        Field.Type = "VARCHAR";
                        Field.FieldValue = items.FieldValue;
                        Field.IsWhere = true;
                        list.Add(Field);
                    }
                }
                if (list != null && list.Count() > 0)
                {
                    list.FirstOrDefault().TableName = oBO.TableName;
                }
            }
            return list;
        }

        private string GetFieldType(int m)
        {
            string type = ((BODatatypes)m).ToString();
            return type;
        }

        public BOFields GetWhereValues(int FieldID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<BOFields> AllBos = new List<BOFields>();
            BOFields list = dbContext.BOFields.Where(m => m.ID == FieldID).FirstOrDefault();
            return list;
        }

        public List<VMDropDown> GetDBValuesForField(string Query, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> results = new List<VMDropDown>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(new VMDropDown
                    {
                        Value = reader.GetInt32(0),
                        Expression = reader.GetString(1)
                    });
                }
                Con.Close();
            }
            return results;
        }

        public bool IsExistsQueryName(string QueryName, int ID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<Reports> AllReports = new List<Reports>();
            if (OrgID == 0)
            {
                AllReports = dbContext.Reports.ToList();
            }
            else
            {
                AllReports = dbContext.Reports.Where(m => m.OrganizationID == 0 || m.OrganizationID == OrgID).ToList();
            }
            var Report = AllReports.Where(m => m.Name.Equals(QueryName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (Report != null)
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
                if (Report != null)
                {
                    if (ID == Report.ID)
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

        public string SaveQuery(VMReports model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            Reports report = new Reports();
            if (string.IsNullOrEmpty(model.XIGUID.ToString()) || model.ID == 0)
            {
                int pid = dbContext.Reports.Where(m => m.Name == model.parent).Select(m => m.ID).FirstOrDefault();
                report.ID = dbContext.Reports.OrderByDescending(u => u.ID).Select(testc => testc.ID).FirstOrDefault() + 1;
                report.OrganizationID = iOrgID;
                report.FKiApplicationID = FKiAppID;
                report.BOID = model.BOID;
                report.Name = model.Name;
                report.Title = model.Title;
                report.bIsMultiBO = model.bIsMultiBO;
                report.FKiVisualisationID = model.FKiVisualisationID;
                report.TypeID = model.TypeID;
                report.iPaginationCount = model.iPaginationCount;
                if (model.TypeID == 2)
                {
                    report.Class = 0;
                }
                else
                {
                    report.Class = model.Class;
                }
                if (pid == 0)
                {
                    report.ParentID = null;
                }
                else
                {
                    report.ParentID = pid;
                }
                report.IsParent = model.IsParent;
                report.CategoryID = model.CategoryID;
                report.Description = model.Description;
                report.DisplayAs = model.DisplayAs;
                if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.ResultList.ToString())
                {
                    report.ResultListDisplayType = model.ResultListDisplayType;
                }
                else
                {
                    report.ResultListDisplayType = 1;
                }
                report.ResultIn = model.ResultIn;
                if (model.DisplayAs == 120)
                {
                    report.FKiComponentID = 0;
                    report.RepeaterType = 0;
                    report.RepeaterComponentID = model.FKiComponentID;
                }
                else
                {
                    report.FKiComponentID = 0;
                    report.RepeaterType = 0;
                    report.RepeaterComponentID = 0;
                }
                if (model.ResultIn == "Popup")
                {
                    report.PopupType = model.PopupType;
                    if (model.PopupType == "Default")
                    {
                        report.PopupLeft = 0;
                        report.PopupHeight = 0;
                        report.PopupTop = 0;
                        report.PopupWidth = 0;
                    }
                    else if (model.PopupType == "Max" || model.PopupType == "Medium" || model.PopupType == "Small")
                    {
                        report.PopupLeft = 0;
                        report.PopupHeight = model.PopupHeight;
                        report.PopupTop = 0;
                        report.PopupWidth = model.PopupWidth;
                    }
                    else
                    {
                        report.PopupLeft = model.PopupLeft;
                        report.PopupHeight = model.PopupHeight;
                        report.PopupTop = model.PopupTop;
                        report.PopupWidth = model.PopupWidth;
                    }
                }
                else if (model.ResultIn == "Dialog")
                {
                    report.PopupType = model.DialogType;
                    if (model.DialogType == "Specific")
                    {
                        report.DialogAt1 = model.DialogAt1;
                        report.DialogAt2 = model.DialogAt2;
                        report.DialogMy1 = model.DialogMy1;
                        report.DialogMy2 = model.DialogMy2;
                        report.PopupWidth = model.PopupWidth;
                        report.PopupHeight = model.PopupHeight;
                    }
                }
                else
                {
                    report.PopupType = null;
                    report.PopupLeft = 0;
                    report.PopupHeight = 0;
                    report.PopupTop = 0;
                    report.PopupWidth = 0;
                }
                report.Class = model.Class;
                report.Code = model.Code;
                report.IsNaturalSearch = model.IsNaturalSearch;
                report.IsFilterSearch = model.IsFilterSearch;
                report.IsExport = model.IsExport;
                report.IsDynamic = model.IsDynamic;
                report.IsStoredProcedure = model.IsStoredProcedure;
                report.Query = report.SelectFields = null;
                report.FromBos = report.WhereFields = report.GroupFields = null;
                report.OrderFields = null;
                report.StatusTypeID = 10;
                report.CreatedBy = report.UpdatedBy = 1;
                report.CreatedBySYSID = report.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                report.CreatedTime = report.UpdatedTime = DateTime.Now;
                report.SearchFields = null;
                report.bIsLockToUser = model.bIsLockToUser;
                report.sLog = model.sLog;
                dbContext.Reports.Add(report);
                dbContext.SaveChanges();
            }
            else
            {

                if (!string.IsNullOrEmpty(model.XIGUID.ToString()))
                {
                    report = dbContext.Reports.Where(x => x.XIGUID == model.XIGUID).Select(t => t).FirstOrDefault();
                }
                if (model.ID > 0)
                {
                    report = dbContext.Reports.Where(m => m.ID == model.ID).FirstOrDefault();
                    Common.SaveErrorLog("Error at Generatequeryrepository: SaveQuery: " + model.ID, "");
                }

                int pid = dbContext.Reports.Where(m => m.Name == model.parent).Select(m => m.ID).FirstOrDefault();
                report.BOID = model.BOID;
                report.Name = model.Name;
                report.Title = model.Title;
                report.TypeID = model.TypeID;
                report.FKiVisualisationID = model.FKiVisualisationID;
                report.iPaginationCount = model.iPaginationCount;
                if (model.TypeID == 2)
                {
                    report.Class = 0;
                }
                else
                {
                    report.Class = model.Class;
                }
                if (((EnumDisplayTypes)report.DisplayAs).ToString() == EnumDisplayTypes.ResultList.ToString())
                {
                    report.ResultListDisplayType = model.ResultListDisplayType;
                }
                else
                {
                    report.ResultListDisplayType = 1;
                }

                if (model.DisplayAs == 120)
                {
                    report.FKiComponentID = 0;
                    report.RepeaterType = 0;
                    report.RepeaterComponentID = model.FKiComponentID;
                }
                else
                {
                    report.FKiComponentID = 0;
                    report.RepeaterType = 0;
                    report.RepeaterComponentID = 0;
                }
                report.ResultIn = model.ResultIn;
                if (model.ResultIn == "Popup")
                {
                    report.PopupType = model.PopupType;
                    if (model.PopupType == "Default")
                    {
                        report.PopupLeft = 0;
                        report.PopupHeight = 0;
                        report.PopupTop = 0;
                        report.PopupWidth = 0;
                    }
                    else if (model.PopupType == "Max" || model.PopupType == "Medium" || model.PopupType == "Small")
                    {
                        report.PopupLeft = 0;
                        report.PopupHeight = model.PopupHeight;
                        report.PopupTop = 0;
                        report.PopupWidth = model.PopupWidth;
                    }
                    else
                    {
                        report.PopupLeft = model.PopupLeft;
                        report.PopupHeight = model.PopupHeight;
                        report.PopupTop = model.PopupTop;
                        report.PopupWidth = model.PopupWidth;
                    }
                }
                else if (model.ResultIn == "Dialog")
                {
                    report.PopupType = model.DialogType;
                    if (model.DialogType == "Specific")
                    {
                        report.DialogAt1 = model.DialogAt1;
                        report.DialogAt2 = model.DialogAt2;
                        report.DialogMy1 = model.DialogMy1;
                        report.DialogMy2 = model.DialogMy2;
                        report.PopupWidth = model.PopupWidth;
                        report.PopupHeight = model.PopupHeight;
                    }
                }
                else
                {
                    report.PopupType = null;
                    report.PopupLeft = 0;
                    report.PopupHeight = 0;
                    report.PopupTop = 0;
                    report.PopupWidth = 0;
                }
                report.DisplayAs = model.DisplayAs;
                report.Description = model.Description;
                report.ParentID = pid;
                report.IsParent = model.IsParent;
                report.CategoryID = model.CategoryID;
                report.Code = model.Code;
                report.IsNaturalSearch = model.IsNaturalSearch;
                report.IsFilterSearch = model.IsFilterSearch;
                report.IsExport = model.IsExport;
                report.IsDynamic = model.IsDynamic;
                report.bIsMultiBO = model.bIsMultiBO;
                report.IsStoredProcedure = model.IsStoredProcedure;
                report.bIsLockToUser = model.bIsLockToUser;
                report.OrganizationID = iOrgID;
                report.FKiApplicationID = FKiAppID;
                report.UpdatedBy = 1;
                report.UpdatedTime = DateTime.Now;
                report.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                report.StatusTypeID = model.StatusTypeID;
                report.sLog = model.sLog;
                report.XIGUID = model.XIGUID;
                dbContext.SaveChanges();
            }
            return report.XIGUID.ToString();
        }

        public string SaveQueryCopy(string ReportID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            Reports NewReport = new Reports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ReportID, out iQueryID);
            Guid.TryParse(ReportID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                NewReport = dbContext.Reports.Where(m => m.ID == iQueryID).FirstOrDefault();
                Common.SaveErrorLog("Error at Generatequeryrepository: SaveQueryCopy: " + iQueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                NewReport = dbContext.Reports.Where(m => m.XIGUID == QueryIDGUID).FirstOrDefault();
            }
            NewReport.Name = NewReport.Name + " Copy";
            List<string> totals = dbContext.Reports.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.OrganizationID == OrgID).Select(m => m.Name).ToList();
            if (totals.Contains(NewReport.Name))
            {
                return NewReport.XIGUID.ToString();
            }
            else
            {
                NewReport.OrganizationID = OrgID;
                NewReport.FKiApplicationID = FKiAppID;
                dbContext.Reports.Add(NewReport);
                dbContext.SaveChanges();
                if (NewReport.OrganizationID > 0)
                {
                    var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                    var Org = dbCore.Organization.Find(NewReport.OrganizationID);
                    DataContext Spdb = new DataContext(sOrgDB);
                    List<Targets> Targets = new List<Targets>();
                    if (iQueryID > 0)
                    {
                        Targets = Spdb.Targets.Where(m => m.ReportID == iQueryID).ToList();
                        Common.SaveErrorLog("Error at Generatequeryrepository: SaveQueryCopy: " + ReportID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Targets = Spdb.Targets.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).ToList();
                    }
                    foreach (var items in Targets)
                    {
                        items.FKi1ClickIDXIGUID = NewReport.XIGUID;
                        Spdb.Targets.Add(items);
                        Spdb.SaveChanges();
                    }
                    List<Schedulers> Schedulers = new List<Schedulers>();
                    if (iQueryID > 0)
                    {
                        Schedulers = Spdb.Schedulers.Where(m => m.ReportID == iQueryID).ToList();
                        Common.SaveErrorLog("Error at Generatequeryrepository: SaveQueryCopy: " + ReportID, "");
                    }
                    if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                    {
                        Schedulers = Spdb.Schedulers.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).ToList();
                    }
                    foreach (var items in Schedulers)
                    {
                        items.XIGUID = NewReport.XIGUID;
                        Spdb.Schedulers.Add(items);
                        Spdb.SaveChanges();
                    }
                }
                return NewReport.XIGUID.ToString();
            }
        }

        //public string SaveQuerySearchFields(string QueryID, string SearchFields, bool bIsMultiSearch, bool bIsXICreatedBy, bool bIsUpdatedBy, int FKiCrtd1ClickID, int FKiUpdtd1ClickID, string sDatabase)
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    Reports report = new Reports();
        //    int iQueryID = 0;
        //    Guid QueryIDGUID = Guid.Empty;
        //    int.TryParse(QueryID, out iQueryID);
        //    Guid.TryParse(QueryID, out QueryIDGUID);
        //    if (iQueryID > 0)
        //    {
        //        report = dbContext.Reports.Find(iQueryID);
        //        Common.SaveErrorLog("Error at Generatequeryrepository: SaveQuerySearchFields: " + QueryID, "");
        //    }
        //    if (QueryIDGUID != Guid.Empty && QueryIDGUID != null/*!string.IsNullOrEmpty(QueryIDGUID.ToString())*/)
        //    {
        //        report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
        //    }
        //    report.SearchFields = SearchFields;
        //    report.bIsMultiSearch = bIsMultiSearch;
        //    if (bIsXICreatedBy == true)
        //    {
        //        report.bIsXICreatedBy = true;
        //        report.FKiCrtd1ClickID = FKiCrtd1ClickID;
        //    }
        //    else
        //    {
        //        report.bIsXICreatedBy = false;
        //        report.FKiCrtd1ClickID = 0;
        //    }
        //    if (bIsUpdatedBy == true)
        //    {
        //        report.bIsXIUpdatedBy = true;
        //        report.FKiUpdtd1ClickID = FKiUpdtd1ClickID;
        //    }
        //    else
        //    {
        //        report.bIsXIUpdatedBy = false;
        //        report.FKiUpdtd1ClickID = 0;
        //    }
        //    dbContext.SaveChanges();
        //    return QueryID.ToString();
        //}

        public string SaveQuerySearchFields(string QueryID, string SearchFields, bool bIsMultiSearch, bool bIsXICreatedBy, bool bIsUpdatedBy, int FKiCrtd1ClickID, int FKiUpdtd1ClickID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Reports report = new Reports();
            VMReports Report = new VMReports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                report = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: SaveQuerySearchFields: " + QueryID, "");
            }
            if (QueryIDGUID != Guid.Empty && QueryIDGUID != null/*!string.IsNullOrEmpty(QueryIDGUID.ToString())*/)
            {
                report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            report.SearchFields = SearchFields;
            report.bIsMultiSearch = bIsMultiSearch;
            if (bIsXICreatedBy == true)
            {
                report.bIsXICreatedBy = true;
                report.FKiCrtd1ClickID = FKiCrtd1ClickID;
            }
            else
            {
                report.bIsXICreatedBy = false;
                report.FKiCrtd1ClickID = 0;
            }
            if (bIsUpdatedBy == true)
            {
                report.bIsXIUpdatedBy = true;
                report.FKiUpdtd1ClickID = FKiUpdtd1ClickID;
            }
            else
            {
                report.bIsXIUpdatedBy = false;
                report.FKiUpdtd1ClickID = 0;
            }
            dbContext.SaveChanges();
            return QueryID.ToString();
        }

        public int SaveQueryActions(VMQueryActions model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Reports report = dbContext.Reports.Where(x => x.XIGUID == model.XIGUID).Select(t => t).FirstOrDefault();
            if (model.IsRowClick)
            {
                report.IsRowClick = model.IsRowClick;
                report.RowXiLinkID = model.RowXiLinkID;
                if (!string.IsNullOrEmpty(model.sRowXiLinkType))
                    report.sRowXiLinkType = model.sRowXiLinkType.ToLower();
                else
                    report.sRowXiLinkType = model.sRowXiLinkType;
            }
            else
            {
                report.IsRowClick = false;
                report.RowXiLinkID = 0;
                report.sRowXiLinkType = "";
            }
            if (model.IsColumnClick)
            {
                report.IsColumnClick = model.IsColumnClick;
                report.OnClickColumn = model.OnClickColumn;
                report.OnClickParameter = model.OnClickParameter;
                report.ColumnXiLinkID = model.ColumnXiLinkID;
            }
            else
            {
                report.IsColumnClick = false;
                report.ColumnXiLinkID = 0;
                report.OnClickColumn = null;
                report.OnClickParameter = null;
            }
            if (model.IsCellClick)
            {
                report.IsCellClick = model.IsCellClick;
                report.OnClickCell = model.OnClickCell;
                report.CellXiLinkID = model.CellXiLinkID;
            }
            else
            {
                report.IsCellClick = false;
                report.CellXiLinkID = 0;
                report.OnClickCell = null;
            }
            if (model.IsRowTotal)
            {
                report.IsRowTotal = model.IsRowTotal;
            }
            else
            {
                report.IsRowTotal = false;
            }
            if (model.IsColumnTotal)
            {
                report.IsColumnTotal = model.IsColumnTotal;
            }
            else
            {
                report.IsColumnTotal = false;
            }
            if (model.IsCreate)
            {
                report.IsCreate = model.IsCreate;
                report.CreateRoleID = model.CreateRoleID;
                report.CreateGroupID = model.CreateGroupID;
                report.iCreateXILinkID = model.iCreateXILinkID;
                report.sAddLabel = model.sAddLabel;
                report.sCreateType = model.sCreateType;
                if (!string.IsNullOrEmpty(report.sCreateType) && report.sCreateType.ToLower() == "inlinetop")
                {
                    report.bIsAddTop = true;
                }
                else if (!string.IsNullOrEmpty(report.sCreateType) && report.sCreateType.ToLower() == "inlinebottom")
                {
                    report.bIsAddBottom = true;
                }
            }
            else
            {
                report.IsCreate = false;
                report.CreateRoleID = 0;
                report.CreateGroupID = 0;
                report.iCreateXILinkID = 0;
                report.sAddLabel = null;
                report.sCreateType = null;
            }
            if (model.IsEdit)
            {
                report.IsEdit = model.IsEdit;
                report.EditRoleID = model.EditRoleID;
                report.EditGroupID = model.EditGroupID;
            }
            else
            {
                report.IsEdit = false;
                report.EditRoleID = 0;
                report.EditGroupID = 0;
            }
            if (model.IsDelete)
            {
                report.IsDelete = model.IsDelete;
                report.DeleteRoleID = model.DeleteRoleID;
            }
            else
            {
                report.IsDelete = false;
                report.DeleteRoleID = 0;
            }
            if (model.IsRefresh)
            {
                report.IsRefresh = model.IsRefresh;
            }
            else
            {
                report.IsRefresh = false;
            }
            if (model.bIsCopy)
            {
                report.bIsCopy = model.bIsCopy;
            }
            else
            {
                report.bIsCopy = false;
            }
            if (model.bIsView)
            {
                report.bIsView = model.bIsView;
                report.iCreateXILinkID = model.iCreateXILinkID;
            }
            else
            {
                report.bIsView = false;
            }
            if (model.bIsCheckbox)
            {
                report.bIsCheckbox = model.bIsCheckbox;
            }
            else
            {
                report.bIsCheckbox = false;
            }
            if (model.bIsExport)
            {
                report.bIsExport = model.bIsExport;
                report.sFileExtension = model.sFileExtension;
            }
            else
            {
                report.bIsExport = false;
                report.sFileExtension = null;
            }
            report.iLayoutID = model.iLayoutID;
            report.InnerReportID = model.InnerReportID;
            report.EditableFields = model.EditableFields;
            dbContext.SaveChanges();
            return model.QueryID;
        }

        public int SaveQueryTargets(VMReports model, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            if (model.TargetID == 0)
            {
                Targets Target = new Targets();
                Target.UserID = model.TargetUsers;
                Target.ReportID = model.TarQueryID;
                Target.OrganizationID = model.OrganizationID;
                Target.ColumnID = model.TargetColumns;
                Target.Target = model.Targets;
                Target.Period = model.TargetPeriod;
                Target.Colour = model.Colour;
                Target.IsSMS = model.IsSMS;
                if (Target.IsSMS)
                {
                    Target.SMSTemplateID = model.SMSTemplateID;
                }
                Target.IsEmail = model.IsEmail;
                if (Target.IsEmail)
                {
                    Target.EmailTemplateID = model.EmailTemplateID;
                }
                Target.IsNotification = model.IsNotification;
                Spdb.Targets.Add(Target);
                Spdb.SaveChanges();
                return Target.ID;
            }
            else
            {
                Targets Target = new Targets();
                Target = Spdb.Targets.Find(model.TargetID);
                Target.ColumnID = model.TargetColumns;
                Target.Target = model.Targets;
                Target.Period = model.TargetPeriod;
                Target.Colour = model.Colour;
                Target.IsSMS = model.IsSMS;
                if (Target.IsSMS)
                {
                    Target.SMSTemplateID = model.SMSTemplateID;
                }
                else
                {
                    Target.SMSTemplateID = 0;
                }
                Target.IsEmail = model.IsEmail;
                if (Target.IsEmail)
                {
                    Target.EmailTemplateID = model.EmailTemplateID;
                }
                else
                {
                    Target.EmailTemplateID = 0;
                }
                Target.IsNotification = model.IsNotification;
                Spdb.SaveChanges();
                return model.TargetID;
            }
        }

        public int SaveQueryScheduler(VMReports model, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == model.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            if (model.SchedulerID == 0)
            {
                Schedulers Sch = new Schedulers();
                Sch.UserID = model.UserID;
                Sch.ReportID = model.SchQueryID;
                Sch.OrganizationID = model.OrganizationID;
                Sch.Period = model.TargetPeriod;
                Sch.Date = 0;
                if (model.TargetPeriod == "Monthly")
                {
                    Sch.Date = model.Date;
                    Sch.Day = null;
                }
                else if (model.TargetPeriod == "Weekly")
                {
                    Sch.Day = model.Day;
                    Sch.Date = 0;
                }
                Sch.Time = model.Time;
                Sch.Type = model.AlertType;
                if (model.AlertType == "Email")
                {
                    Sch.EmailTemplateID = model.SchEmailTemplateID;
                    Sch.SMSTemplateID = 0;
                }
                else if (model.AlertType == "SMS")
                {
                    Sch.SMSTemplateID = model.SchSMSTemplateID;
                    Sch.EmailTemplateID = 0;
                }
                else
                {
                    Sch.SMSTemplateID = 0;
                    Sch.EmailTemplateID = 0;
                }
                Sch.StatusTypeID = model.StatusTypeID;
                Spdb.Schedulers.Add(Sch);
                Spdb.SaveChanges();
                return Sch.ID;
            }
            else
            {
                Schedulers Sch = new Schedulers();
                Sch = Spdb.Schedulers.Find(model.SchedulerID);
                Sch.UserID = model.UserID;
                Sch.OrganizationID = model.OrganizationID;
                Sch.Period = model.TargetPeriod;
                Sch.Date = 0;
                if (model.TargetPeriod == "Weekly")
                {
                    Sch.Day = model.Day;
                    Sch.Date = 0;
                }
                else if (model.TargetPeriod == "Monthly")
                {
                    Sch.Date = model.Date;
                    Sch.Day = null;
                }
                Sch.Time = model.Time;
                Sch.Type = model.AlertType;
                if (model.AlertType == "Email")
                {
                    Sch.EmailTemplateID = model.SchEmailTemplateID;
                    Sch.SMSTemplateID = 0;
                }
                else if (model.AlertType == "SMS")
                {
                    Sch.SMSTemplateID = model.SchSMSTemplateID;
                    Sch.EmailTemplateID = 0;
                }
                else
                {
                    Sch.SMSTemplateID = 0;
                    Sch.EmailTemplateID = 0;
                }
                Sch.StatusTypeID = model.StatusTypeID;
                Spdb.SaveChanges();
                return Sch.ID;
            }
        }

        public string DeleteQuery(string QueryID, string sDatabase)
        {
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            ModelDbContext dbContext = new ModelDbContext();
            Reports report = new Reports();
            if (iQueryID > 0)
            {
                report = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: DeleteQuery: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            dbContext.Reports.Remove(report);
            dbContext.SaveChanges();
            return QueryID.ToString();
        }

        public List<DisplayName> GetOperators(string DataType, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<DisplayName> Operators = new List<DisplayName>();
            if (DataType == "INT" || DataType == "DATETIME" || DataType == "BIGINT" || DataType == "DECIMAL")
            {
                var intoperators = new IntOperatorOptions();
                foreach (var items in intoperators)
                {
                    Operators.Add(new DisplayName
                    {
                        Type = items.Type,
                        DisplayValue = items.DisplayValue,
                        Value = items.Value
                    });
                }
            }
            else if (DataType == "VARCHAR")
            {
                var stroperators = new StringOperatorOptions();
                foreach (var items in stroperators)
                {
                    Operators.Add(new DisplayName
                    {
                        Type = items.Type,
                        DisplayValue = items.DisplayValue,
                        Value = items.Value
                    });
                }
            }
            else if (DataType == "BIT")
            {
                var bitoperators = new BITOperatorOptions();
                foreach (var items in bitoperators)
                {
                    Operators.Add(new DisplayName
                    {
                        Type = items.Type,
                        DisplayValue = items.DisplayValue,
                        Value = items.Value
                    });
                }
            }

            return Operators;
        }
        public VMQueryPreview GetQueryPreview(string QueryID, int PageIndex, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            int PageSize = 30;
            int skip = 0;
            if (PageIndex == 2)
            {
                skip = 23;
                PageSize = 10;
            }
            else if (PageIndex >= 2)
            {
                skip = PageSize + 10 * (PageIndex - 2);
                PageSize = 10;
            }
            if (skip == 0)
            {
                PageSize = 23;
            }
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            Reports Report = new Reports();
            if (iQueryID > 0)
            {
                Report = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: GetQueryPreview: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                Report = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            Common Com = new Common();
            var FromIndex = Report.Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = Report.Query.Substring(0, FromIndex);
            List<VMDropDown> KeyPositions = new List<VMDropDown>();
            var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
            string UserIDs = Com.GetSubUsers(iUserID, OrganizationID, sDatabase, sOrgName);
            string Query = ServiceUtil.ReplaceQueryContent(Report.Query, UserIDs, iUserID, OrganizationID, 0, Report.BOID);
            Report.Query = Query;
            string Headings = Report.Query;
            VMQueryPreview Preview = new VMQueryPreview();
            List<string> AllHeadings = new List<string>();
            Reports model = new Reports();
            Report.OrganizationID = OrganizationID;
            VMReports Head = new VMReports();
            Head.ID = Report.ID;
            Head.OrganizationID = Report.OrganizationID;
            Head.FKiApplicationID = Report.FKiApplicationID;
            Head.Query = Report.Query;
            Head.BOID = Report.BOID;
            Head.UserID = iUserID;
            Head.AllBOs = Keys;
            if (Report.SelectFields != null)
            {
                Head.SelectFields = Report.SelectFields;
                Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                Query = Preview.Query;
                AllHeadings = Preview.Headings;
                KeyPositions = Preview.FKPositions;
            }
            else if (Headings.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                Headings = Query.Substring(0, Query.IndexOf(" FROM", StringComparison.InvariantCultureIgnoreCase));
                Headings = Headings.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                Head.SelectFields = Headings;
                Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                Query = Preview.Query;
                AllHeadings = Preview.Headings;
                KeyPositions = Preview.FKPositions;
            }
            string BOName = string.Empty;
            var oBO = dbContext.BOs.Find(Report.BOID);
            if (oBO.TableName == null)
            {
                BOName = oBO.Name;
            }
            else
            {
                BOName = oBO.TableName;
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
                int count = reader.FieldCount;
                string[] rows = new string[count];
                while (reader.Read())
                {
                    List<string> values = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        var pos = KeyPositions.Where(m => m.Value == i).FirstOrDefault();
                        if (pos != null)
                        {
                            var DbValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            if (DbValue != null)
                            {
                                var Value = ServiceUtil.ReplaceForeginKeyValues(pos, reader.GetValue(i).ToString(), sDatabase);
                                values.Add(Value);
                            }
                        }
                        else
                        {
                            values.Add(reader.IsDBNull(i) ? null : reader.GetValue(i).ToString());
                        }
                    }
                    string[] result = values.ToArray();
                    results.Add(result);
                }
                Con.Close();
            }
            VMQueryPreview vmquery = new VMQueryPreview();
            vmquery.Headings = AllHeadings;
            if (Report.ResultListDisplayType == 0)
            {
                vmquery.Rows = results.Skip(skip).Take(PageSize).ToList();
            }
            else
            {
                vmquery.Rows = results;
            }

            vmquery.ResultListDisplayType = Report.ResultListDisplayType;
            return vmquery;
        }

        public string CheckQueryStatus(string Query, int BOID, int iUserID, int OrgID, string sOrgName, string sDatabase, string sCurrentGuestUser)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            Common Com = new Common();
            var NewQuery = ServiceUtil.ReplaceQueryContent(Query, iUserID.ToString(), iUserID, OrgID, 0, BOID);
            NewQuery = ServiceUtil.ReplaceGuestUser(NewQuery, sCurrentGuestUser);
            string BoName = string.Empty;
            var oBO = dbContext.BOs.Find(BOID);
            if (oBO.TableName == null)
            {
                BoName = oBO.Name;
            }
            else
            {
                BoName = oBO.TableName;
            }
            var sBODataSource = oXIAPI.GetXIGUIDBODataSource(oBO.iDataSourceXIGUID.ToString(), UserDetails.FKiOrgID, sDatabase, sOrgDB, oBO.FKiApplicationID);
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = NewQuery;
                if (NewQuery.Contains("{"))
                {

                }
                else
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    List<string[]> results = new List<string[]>();
                    int count = reader.FieldCount;
                    string[] rows = new string[count];
                }
            }
            return "Success";
        }

        public DTResponse GetPreviewInForm(jQueryDataTableParamModel param, string ID, string Query, string Fields, int BOID, int iUserID, int OrgID, string sOrgName, string sDatabase, string sCurrentGuestUser)
        {
            var UserData = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserData.FKiApplicationID;
            var sOrgDB = UserData.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            var Role = dbCore.XIAppRoles.Find(UserRoleID);
            var oBO = dbContext.BOs.Find(BOID);
            string BoName = oBO.Name;
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ID, out iQueryID);
            Guid.TryParse(ID, out QueryIDGUID);
            Reports Rep = new Reports();
            if (iQueryID > 0)
            {
                Rep = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: GetPreviewInForm: " + ID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                Rep = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            Common Com = new Common();
            var VisibleQuery = Query;
            //var UserIDs = Com.GetSubUsers(iUserID, OrgID);
            var NewQuery = ServiceUtil.ReplaceQueryContent(Query, iUserID.ToString(), iUserID, OrgID, 0, BOID);
            NewQuery = ServiceUtil.ReplaceGuestUser(NewQuery, sCurrentGuestUser);
            var UserDetails = dbCore.XIAppUsers.Find(iUserID);
            if (((EnumDisplayTypes)Rep.DisplayAs).ToString() == EnumDisplayTypes.Summary.ToString())
            {
                if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && Role.sRoleName != EnumRoles.Admin.ToString() && BoName != EnumLeadTables.Reports.ToString() && BoName == EnumLeadTables.Leads.ToString())
                {
                    NewQuery = ServiceUtil.AddSearchParameters(NewQuery, "FKiOrgID=" + OrgID);
                    var LocCondition = "";
                    var Locs = UserDetails.sLocation.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    foreach (var items in Locs)
                    {
                        LocCondition = LocCondition + "OrgHeirarchyID='ORG" + OrgID + "_" + items + "' or ";
                    }
                    LocCondition = LocCondition.Substring(0, LocCondition.Length - 4);
                    LocCondition = "(" + LocCondition + ")";
                    NewQuery = ServiceUtil.AddSearchParameters(NewQuery, LocCondition);
                }
                else if (Role.sRoleName != EnumRoles.SuperAdmin.ToString() && BoName != EnumLeadTables.Reports.ToString() && BoName == EnumLeadTables.Leads.ToString())
                {
                    NewQuery = ServiceUtil.AddSearchParameters(NewQuery, "FKiOrgID=" + OrgID);
                    var LocCondition = "OrgHeirarchyID Like 'ORG" + OrgID + "_%'";
                    NewQuery = ServiceUtil.AddSearchParameters(NewQuery, LocCondition);
                }
            }
            string allfields = NewQuery;
            string Columns = "";
            if (Fields != null && Fields.Length > 0)
            {
                Columns = Fields.Replace("SELECT ", "");
            }
            List<string> AllHeadings = new List<string>();
            var FromIndex = Query.IndexOf(" FROM ", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = Query.Substring(0, FromIndex);
            List<VMDropDown> KeyPositions = new List<VMDropDown>();
            var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
            VMQueryPreview Preview = new VMQueryPreview();
            Reports model = new Reports();
            model.Query = NewQuery;
            model.SelectFields = Columns;
            model.BOID = BOID;
            model.OrganizationID = OrgID;
            model.FKiApplicationID = FKiAppID;
            model.ID = Rep.ID;
            model.XIGUID = QueryIDGUID;
            model.DisplayAs = Rep.DisplayAs;
            VMReports Head = new VMReports();
            Head.ID = Rep.ID;
            Head.XIGUID = QueryIDGUID;
            Head.OrganizationID = OrgID;
            Head.FKiApplicationID = FKiAppID;
            Head.Query = NewQuery;
            Head.BOID = BOID;
            Head.UserID = iUserID;
            Head.AllBOs = Keys;
            Head.SelectFields = Columns;
            Head.VisibleQuery = VisibleQuery;
            if (Columns.Length > 0)
            {
                Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                AllHeadings = Preview.Headings;
                NewQuery = Preview.Query;
                KeyPositions = Preview.FKPositions;
            }
            else
            {
                if (NewQuery.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    allfields = allfields.Substring(0, allfields.IndexOf(" FROM", StringComparison.InvariantCultureIgnoreCase));
                    allfields = allfields.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                    Head.SelectFields = allfields;
                    Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                    AllHeadings = Preview.Headings;
                    NewQuery = Preview.Query;
                    KeyPositions = Preview.FKPositions;
                }
            }
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserData.FKiOrgID, sDatabase, sOrgDB);
            List<string[]> results = new List<string[]>();
            List<object[]> TotalResult = new List<object[]>();
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = NewQuery;
                SqlDataReader reader = cmd.ExecuteReader();
                int count = reader.FieldCount;
                string[] rows = new string[count];
                DataTable data = new DataTable();
                data.Load(reader);
                TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                List<object[]> Res = new List<object[]>();
                Res = TotalResult.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                for (int i = 0; i < Res.Count(); i++)
                {
                    List<string> NewRes = new List<string>();
                    for (int j = 0; j < Res[i].Count(); j++)
                    {
                        var pos = KeyPositions.Where(m => m.Value == j).FirstOrDefault();
                        if (pos != null)
                        {
                            var DbValue = Res[i][j];
                            if (DbValue != null)
                            {
                                var Value = ServiceUtil.ReplaceForeginKeyValues(pos, Res[i][j].ToString(), UserDetails.sDatabaseName);
                                NewRes.Add(Value);
                            }
                            else
                            {
                                NewRes.Add(null);
                            }
                        }
                        else
                        {
                            NewRes.Add(Res[i][j].ToString());
                        }
                    }
                    results.Add(NewRes.ToArray());
                }
                Con.Close();
            }
            VMQueryPreview query = new VMQueryPreview();
            query.Headings = AllHeadings;
            query.Rows = results;
            query.Query = NewQuery;
            query.ShowAs = Rep.Title;
            if (!(allfields.IndexOf("SELECT") >= 0))
            {
                query.Select = "None";
            }
            if (!(allfields.IndexOf("WHERE") > 0))
            {
                query.Where = "None";
            }
            if (!(allfields.IndexOf("GROUP BY") > 0))
            {
                query.GroupBY = "None";
            }
            if (!(allfields.IndexOf("ORDER BY") > 0))
            {
                query.OrderBY = "None";
            }
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = TotalResult.Count(),
                iTotalDisplayRecords = TotalResult.Count(),
                aaData = results,
            };
        }

        public VMQueryPreview GetHeadingsOfQuery(VMReports Report, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            Reports Rep = new Reports();
            if (Report.XIGUID != Guid.Empty)
            {
                Rep = dbContext.Reports.Where(x => x.XIGUID == Report.XIGUID).Select(t => t).FirstOrDefault();
            }
            else
            {
                Rep = dbContext.Reports.Where(x => x.ID == Report.ID).Select(t => t).FirstOrDefault();
            }
            string OrgnQuery = Report.Query;
            if (Report.SelectFields == null || Report.SelectFields.Length == 0)
            {
                if (OrgnQuery.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    OrgnQuery = OrgnQuery.Substring(0, OrgnQuery.IndexOf(" FROM ", StringComparison.InvariantCultureIgnoreCase));
                    OrgnQuery = OrgnQuery.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                    Report.SelectFields = OrgnQuery;
                }
            }
            else
            {
                Report.SelectFields = Report.SelectFields.Replace("SELECT ", "");
                if (Report.SelectFields.Contains("{"))
                {
                    string sOrgQuery = Report.Query.Substring(0, Report.Query.IndexOf(" FROM ", StringComparison.InvariantCultureIgnoreCase));
                    sOrgQuery = sOrgQuery.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                    Report.SelectFields = sOrgQuery;
                }
            }
            Common Com = new Common();
            Report.Query = ServiceUtil.ReplaceQueryContent(Report.Query, Report.UserID.ToString(), Report.UserID, Report.OrganizationID, 0, Report.BOID);
            VMQueryPreview model = new VMQueryPreview();
            var GroupFileds = dbContext.BOGroupFields.ToList();
            var Bofields = dbContext.BOFields.Where(m => m.BOID == Report.BOID).ToList();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == Report.OrganizationID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext SpDb = new DataContext(sOrgDB);
            List<string> AllHeadings = new List<string>();
            string Query = Report.Query;
            var SelWithGroup = "";
            //var regx = new Regex("{.*?}");
            //var mathes = regx.Matches(Report.SelectFields);
            //if (mathes.Count > 0)
            //{
            //    List<string> SelectFields = Regex.Replace(Report.SelectFields, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            //    foreach (var items in SelectFields)
            //    {
            //        if (items.Contains('{'))
            //        {
            //            int id = Convert.ToInt32(items.Substring(1, items.Length - 2));
            //            var Grp = GroupFileds.Where(m => m.ID == id).FirstOrDefault();
            //            if (Grp.IsMultiColumnGroup)
            //            {
            //                SelWithGroup = SelWithGroup + Grp.BOFieldNames + ", ";
            //            }
            //            else
            //            {
            //                SelWithGroup = SelWithGroup + Grp.GroupName + ", ";
            //            }
            //        }
            //        else
            //        {
            //            SelWithGroup = SelWithGroup + items + ", ";
            //        }
            //    }
            //    SelWithGroup = SelWithGroup.Substring(0, SelWithGroup.Length - 2);
            //}
            //else
            //{
            //    SelWithGroup = Report.SelectFields;
            //}
            SelWithGroup = Report.SelectFields;
            var Keys = ServiceUtil.GetForeginkeyValues(" " + SelWithGroup);
            List<VMDropDown> KeyPositions = new List<VMDropDown>();
            if (Report.SelectFields.Length > 0)
            {
                var sGroupFileds = dbContext.BOGroupFields.Where(m => m.BOID == Rep.BOID).ToList();
                List<string> Headings = new List<string>();
                if (Report.SelectFields.IndexOf(", ") >= 0)
                {
                    Headings = Report.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                else
                {
                    Headings = Report.SelectFields.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                int FKPosition = 0;
                foreach (var items in Headings)
                {
                    if (items.Contains('{'))
                    {
                        var sCol = string.Empty;
                        if (items.Contains('.'))
                        {
                            sCol = items.Split('.')[1].Replace("{", "").Replace("}", "");
                        }
                        else
                        {
                            sCol = items.Replace("{", "").Replace("}", "");
                        }
                        //int id = 0;
                        //if (sGroupFileds != null && sGroupFileds.Any(m => m.GroupName.ToLower() == sCol.ToLower()))
                        //{
                        //    id = sGroupFileds.Where(m => m.GroupName.ToLower() == sCol.ToLower()).FirstOrDefault().ID;
                        //}
                        List<string> oGrpHeadS = new List<string>();
                        var oGrpD = sGroupFileds.Where(m => m.GroupName == sCol).FirstOrDefault();
                        if (oGrpD.IsMultiColumnGroup)
                        {
                            List<string> fieldnames = oGrpD.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                            foreach (var names in fieldnames)
                            {
                                string aliasname = Bofields.Where(m => m.Name.Equals(names, StringComparison.CurrentCultureIgnoreCase)).Select(m => m.LabelName).FirstOrDefault();
                                AllHeadings.Add(aliasname);
                                KeyPositions.AddRange((from c in Keys.Where(m => m.text == names).ToList() select new VMDropDown { text = names, Value = FKPosition }));
                                FKPosition++;
                            }
                        }
                        else
                        {
                            //var HeadName = oGrpD.BOSqlFieldNames + " AS " + "'" + oGrpD.GroupName + "'";
                            AllHeadings.Add(oGrpD.GroupName);
                            FKPosition++;
                        }
                    }
                    else if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                    {
                        var Ognl = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                        string aliasname = SpDb.MappedFields.Where(m => m.AddField == Ognl && m.OrganizationID == Report.OrganizationID).Select(m => m.FieldName).FirstOrDefault();
                        if (aliasname == null)
                        {
                            var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                            var regex = new Regex("'(?:''|[^']*)*'");
                            var matches = regex.Matches(fieldname); //your matches: name, name@gmail.com
                            if (matches.Count > 0)
                            {
                                fieldname = fieldname.Substring(1, fieldname.Length - 2);
                            }
                            AllHeadings.Add(fieldname);
                        }
                        else
                        {
                            AllHeadings.Add(aliasname);
                        }
                        KeyPositions.AddRange((from c in Keys.Where(m => m.text == Ognl) select new VMDropDown { text = Ognl, Value = FKPosition }));
                        FKPosition++;
                    }
                    else if (items.Contains('.'))
                    {
                        var fieldname = items.Split('.')[1];
                        string aliasname = Bofields.Where(m => m.Name.Equals(fieldname, StringComparison.CurrentCultureIgnoreCase)).Select(m => m.LabelName).FirstOrDefault();
                        if (aliasname == null)
                        {
                            AllHeadings.Add(fieldname);
                        }
                        else
                        {
                            AllHeadings.Add(aliasname);
                        }
                    }
                    else
                    {
                        string aliasname = "";
                        if (Report.OrganizationID != 0)
                        {
                            aliasname = SpDb.MappedFields.Where(m => m.AddField == items && m.OrganizationID == Report.OrganizationID).Select(m => m.FieldName).FirstOrDefault();
                        }
                        else
                        {
                            aliasname = Bofields.Where(m => m.Name.Equals(items, StringComparison.CurrentCultureIgnoreCase)).Select(m => m.LabelName).FirstOrDefault();
                        }
                        if (aliasname == null)
                        {
                            aliasname = Bofields.Where(m => m.Name.Equals(items, StringComparison.CurrentCultureIgnoreCase)).Select(m => m.LabelName).FirstOrDefault();
                        }
                        if (aliasname == null)
                        {
                            AllHeadings.Add(items);
                        }
                        else
                        {
                            AllHeadings.Add(aliasname);
                        }
                        KeyPositions.AddRange((from c in Keys.Where(m => m.text == items) select new VMDropDown { text = items, Value = FKPosition }));
                        FKPosition++;
                    }
                }
            }
            model.Query = Query;
            model.Headings = AllHeadings;
            model.FKPositions = KeyPositions;
            model.ShowAs = Rep.Title;
            model.XIGUID = Rep.XIGUID;
            model.ReportID = Rep.ID;
            return model;
        }

        public VMQueryPreview GetPreviewInFormEdited(string Query, int BOID, string sDatabase, int iUserID, int OrgID, string sOrgName)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            Common Com = new Common();
            var NewQuery = ServiceUtil.ReplaceQueryContent(Query, iUserID.ToString(), iUserID, OrgID, 0, BOID);
            string TotalQuery = NewQuery;
            string allfields = NewQuery;
            List<string> Headings = new List<string>();
            Reports model = new Reports();
            VMQueryPreview Preview = new VMQueryPreview();
            VMReports Head = new VMReports();
            Head.OrganizationID = OrgID;
            Head.Query = TotalQuery;
            Head.BOID = BOID;
            Head.UserID = iUserID;
            var oBO = dbContext.BOs.Find(BOID);
            if (TotalQuery.IndexOf("SELECT", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                allfields = allfields.Substring(0, allfields.IndexOf(" FROM", StringComparison.InvariantCultureIgnoreCase));
                allfields = allfields.Replace("SELECT ", "").Replace("Select ", "").Replace("select ", "");
                Head.SelectFields = allfields;
                Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                Headings = Preview.Headings;
                TotalQuery = Preview.Query;
            }
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            List<string[]> results = new List<string[]>();

            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = TotalQuery;
                SqlDataReader reader = cmd.ExecuteReader();
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
                }
                Con.Close();
            }
            VMQueryPreview query = new VMQueryPreview();
            query.Headings = Headings;
            query.Rows = results;
            if (!(allfields.IndexOf("SELECT") >= 0))
            {
                query.Select = "None";
            }
            if (!(allfields.IndexOf("WHERE") > 0))
            {
                query.Where = "None";
            }
            if (!(allfields.IndexOf("GROUP BY") > 0))
            {
                query.GroupBY = "None";
            }
            if (!(allfields.IndexOf("ORDER BY") > 0))
            {
                query.OrderBY = "None";
            }
            return query;
        }
        public VMQueryPreview GetQueryStatus(string ID, string Query, string Fields, int iUserID, string sOrgName, string sDatabase, int BOID)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            string allfields = Query;
            string Columns = Fields.Replace("SELECT ", "");
            List<string> AllHeadings = new List<string>();
            Reports model = new Reports();
            VMReports Head = new VMReports();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ID, out iQueryID);
            Guid.TryParse(ID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                model.ID = iQueryID;
                Head.ID = iQueryID;
                Common.SaveErrorLog("Error at Generatequeryrepository: GetQueryStatus: " + ID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                model.XIGUID = QueryIDGUID;
                Head.XIGUID = QueryIDGUID;
                model = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            Head.Query = allfields;
            Head.BOID = BOID;
            var oBO = dbContext.BOs.Find(BOID);
            VMQueryPreview query = new VMQueryPreview();
            if (Columns.Length > 0)
            {
                Head.SelectFields = Columns;
                Head.AllBOs = new List<VMDropDown>();
                query = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
                AllHeadings = query.Headings;
                allfields = query.Query;
            }
            else
            {
                allfields = Query;
            }


            List<string> conditions = new List<string>();
            if (allfields.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                conditions.Add("SELECT");
            }
            else
            {
                query.Select = "None";
            }
            if (allfields.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) > 0)
            {
                conditions.Add("WHERE");
            }
            else
            {
                query.Where = "None";
            }
            if (allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase) > 0)
            {
                conditions.Add("GROUP BY");
            }
            else
            {
                query.GroupBY = "None";
            }
            if (allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) > 0)
            {
                conditions.Add("ORDER BY");
            }
            else
            {
                query.OrderBY = "None";
            }

            string NewQuery = "", Select = "", Where = "", GroupBY = "", OrderBY = "";
            foreach (var items in conditions)
            {
                if (items == "SELECT")
                {
                    if (allfields.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        NewQuery = allfields.Substring(0, allfields.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase));
                    }
                    else if (allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        NewQuery = allfields.Substring(0, allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase));
                    }
                    else if (allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        NewQuery = allfields.Substring(0, allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        NewQuery = allfields;
                    }
                    Select = NewQuery;
                }
                if (items == "WHERE")
                {
                    if (allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        NewQuery = allfields.Substring(0, allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase));
                    }
                    else if (allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        NewQuery = allfields.Substring(0, allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        Where = allfields.Substring(allfields.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase));
                        NewQuery = Select + Where;
                    }
                }
                else if (items == "GROUP BY")
                {
                    if (allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase) > 0)
                    {
                        int start = allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
                        int end = allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
                        GroupBY = allfields.Substring(start, end - start);
                        NewQuery = Select + GroupBY;
                    }
                    else
                    {
                        NewQuery = allfields.Substring(allfields.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase));
                    }
                }
                else if (items == "ORDER BY")
                {
                    OrderBY = allfields.Substring(allfields.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase));
                    NewQuery = Select + OrderBY;
                }
                var sOrgDB = UserDetails.sUserDatabase;
                var sBODataSource = oXIAPI.GetXIGUIDBODataSource(oBO.iDataSourceXIGUID.ToString(), UserDetails.FKiOrgID, sDatabase, sOrgDB, oBO.FKiApplicationID);
               // var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSourceXIGUID.ToString(), UserDetails.FKiOrgID, sDatabase, sOrgDB);
                using (SqlConnection Con = new SqlConnection(sBODataSource))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    cmd.CommandText = NewQuery;
                    SqlDataReader reader = null;
                    try
                    {
                        reader = cmd.ExecuteReader();
                        if (items == "SELECT")
                        {
                            query.Select = "True";
                        }
                        if (items == "WHERE")
                        {
                            query.Where = "True";
                        }
                        else if (items == "GROUP BY")
                        {
                            query.GroupBY = "True";
                        }
                        else if (items == "ORDER BY")
                        {
                            query.OrderBY = "True";
                        }
                    }
                    catch (Exception)
                    {
                        if (items == "SELECT")
                        {
                            query.Select = "&#10005;";
                        }
                        if (items == "WHERE")
                        {
                            query.Where = "&#10005;";
                        }
                        else if (items == "GROUP BY")
                        {
                            query.GroupBY = "&#10005;";
                        }
                        else if (items == "ORDER BY")
                        {
                            query.OrderBY = "&#10005;";
                        }
                    }
                    Con.Close();
                }
            }
            return query;
        }

        public VMQueryPreview QueryDynamicForm(string QueryID, int iUserID, string sOrgName, string sDatabase, int OrganizationID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            Reports query = new Reports();
            if (iQueryID > 0)
            {
                query = dbContext.Reports.Find(iQueryID);
                Common.SaveErrorLog("Error at Generatequeryrepository: QueryDynamicForm: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                query = dbContext.Reports.Where(x => x.XIGUID == QueryIDGUID).Select(t => t).FirstOrDefault();
            }
            string Query = query.Query;
            List<string> AllHeadings = new List<string>();
            Reports model = new Reports();
            VMReports Head = new VMReports();
            Head.Query = Query;
            Head.BOID = query.BOID;
            Head.UserID = iUserID;
            var oBO = dbContext.BOs.Find(query.BOID);
            VMQueryPreview Preview = new VMQueryPreview();
            if (query.SelectFields.Length > 0)
            {
                Head.SelectFields = query.SelectFields;
                Head.AllBOs = new List<VMDropDown>();
                Preview = GetHeadingsOfQuery(Head, iUserID, sOrgName, sDatabase);
            }
            AllHeadings = Preview.Headings;
            Query = Preview.Query;
            int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            string icon = string.Empty;
            if (iQueryID > 0)
            {
                icon = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.ReportID == iQueryID).Select(m => m.Icon).FirstOrDefault();
                Common.SaveErrorLog("Error at Generatequeryrepository: QueryDynamicForm: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                icon = dbContext.UserReports.Where(m => m.RoleID == UserRoleID).Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).Select(m => m.Icon).FirstOrDefault();
            }
            var orgname = dbCore.Organization.Find(OrganizationID);
            Common Com = new Common();
            Query = ServiceUtil.ReplaceQueryContent(Query, iUserID.ToString(), iUserID, OrganizationID, 0, query.BOID);
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            List<string[]> results = new List<string[]>();
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = Query;
                SqlDataReader reader = cmd.ExecuteReader();
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
                }
                Con.Close();
            }
            Preview.Headings = AllHeadings;
            Preview.Rows = results;
            var AllBoFields = dbContext.BOFields.Where(m => m.BOID == query.BOID).ToList();
            List<SingleBOField> bofields = new List<SingleBOField>();
            if (query.SearchFields != null)
            {
                List<string> SearchFields = query.SearchFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (SearchFields != null)
                {
                    foreach (var items in SearchFields)
                    {
                        BOFields Field = AllBoFields.Where(m => m.Name == items).FirstOrDefault();
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
            Preview.Query = query.Query;
            Preview.QueryID = query.ID;
            Preview.XIGUID = QueryIDGUID;
            Preview.QueryName = query.Name;
            Preview.QueryIcon = icon;
            return Preview;
        }

        public List<Classes> GetClasses(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<Classes> Classes = new List<Classes>();
            var classnames = dbContext.Types.Where(m => m.Name == "Class Type").ToList();
            foreach (var items in classnames)
            {
                Classes.Add(new Classes
                {
                    text = items.Expression,
                    value = items.ID
                });
            }
            return Classes;
        }
        public bool IsPopupNameExists(string Name, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var name = dbContext.Reports.Where(m => m.ActionFieldValue == Name).FirstOrDefault();
            if (name != null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<Reports> GetAllOneClicks(int OrgID, int ParentID, string ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<Reports> OneClicks = new List<Reports>();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(ID, out iQueryID);
            Guid.TryParse(ID, out QueryIDGUID);
            if (iQueryID > 0)
            {
                OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.ID == iQueryID).ToList();
                Common.SaveErrorLog("Error at Generatequeryrepository: GetAllOneClicks: " + ID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {

                OneClicks = dbContext.Reports.Where(m => m.StatusTypeID == 10 && m.XIGUID == QueryIDGUID).ToList();
            }
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

        public DTResponse GetTargetsGrid(jQueryDataTableParamModel param, int ID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<Targets> AllQueries, FilteredBusinessObjects;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredBusinessObjects = Spdb.Targets.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).ToList();
                AllQueries = FilteredBusinessObjects.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredBusinessObjects.Count();
            }
            else
            {
                displyCount = Spdb.Targets.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).Count();
                AllQueries = Spdb.Targets.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = from c in AllQueries
                         select new[] {
                             (i++).ToString(),c.ID.ToString(), Convert.ToString(c.UserID+"-"+c.ColumnID+"-"+c.Target+"-"+c.Period+"-"+c.Colour+"-"+c.IsSMS+"-"+c.IsEmail+"-"+c.IsNotification+"-"+c.SMSTemplateID+"-"+c.EmailTemplateID), c.OrganizationID.ToString(),GetUserName(c.UserID,sDatabase), GetColumnName(c.ColumnID,sDatabase), c.Target.ToString(), c.Period, c.Colour, c.IsSMS.ToString(), c.IsEmail.ToString(), c.IsNotification.ToString(), "" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse GetSchedulersList(jQueryDataTableParamModel param, int ID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(sOrgDB);
            IEnumerable<Schedulers> AllQueries, FilteredBusinessObjects;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredBusinessObjects = Spdb.Schedulers.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).ToList();
                AllQueries = FilteredBusinessObjects.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredBusinessObjects.Count();
            }
            else
            {
                displyCount = Spdb.Schedulers.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).Count();
                AllQueries = Spdb.Schedulers.Where(m => m.OrganizationID == OrgID && m.ReportID == ID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }
            var result = from c in AllQueries
                         select new[] {
                             (i++).ToString(),c.ID.ToString(), GetUserName(c.UserID,sDatabase), Convert.ToString(c.UserID+"-"+c.ReportID+"-"+c.Period+"-"+c.Time+"-"+c.Type+"-"+c.EmailTemplateID+"-"+c.SMSTemplateID+"-"+c.StatusTypeID+"-"+c.Date+"-"+c.Day), c.ReportID.ToString(), c.Period, c.Time, c.Type, c.StatusTypeID.ToString(), "" };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetColumnName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var Column = dbContext.BOFields.Where(m => m.ID == p).Select(m => m.Name).FirstOrDefault();
            return Column;
        }

        private string GetUserName(int p, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var UserName = dbCore.XIAppUsers.Where(m => m.UserID == p).Select(m => m.sFirstName).FirstOrDefault();
            return UserName;
        }

        public List<VMDropDown> GetTargetUsers(int ID, int OrgID, string sDatabase)
        {
            //var UserList = dbContext.AspNetUsers.Where(m => m.OrganizationID == OrgID).ToList();
            //List<VMDropDown> AllUsers = new List<VMDropDown>();
            //var Users = dbContext.Reports.Find(ID);
            //var AssignedUsers = Users.TargetUsers;
            //if (AssignedUsers != null && AssignedUsers != "0")
            //{
            //    var User = new List<string>();
            //    User = AssignedUsers.Split(',').ToList();
            //    List<string> AUsers = UserList.Select(m => m.Id.ToString()).ToList();
            //    var Remaining = AUsers.Except(User).ToList();
            //    foreach (var items in Remaining)
            //    {
            //        var UID = Convert.ToInt32(items);
            //        var userdata = UserList.Where(m=>m.Id== UID).FirstOrDefault();
            //        AllUsers.Add(new VMDropDown
            //        {
            //            Value = userdata.Id,
            //            text = userdata.FirstName
            //        });
            //    }
            //}
            return null;
        }

        //SaveOneclickNameValuePairs
        public VMCustomResponse SaveOneclickNvs(int OneClickID, string[] NVPairs, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XI1ClickNVs oNVs = new XI1ClickNVs();
            var NVPairsList = dbContext.XI1ClickNVs.Where(m => m.FKi1ClickID == OneClickID).ToList();
            if (NVPairs != null && NVPairs.Count() > 0)
            {
                for (int i = 0; i < NVPairs.Count(); i++)
                {
                    var Pairs = NVPairs[i].ToString().Split('^').ToList();
                    int PairID = Convert.ToInt32(Pairs[0]);
                    if (NVPairsList.Where(m => m.ID == PairID).Select(m => m.ID).FirstOrDefault() != 0)
                    {
                        oNVs = dbContext.XI1ClickNVs.Where(m => m.ID == PairID).FirstOrDefault();
                        oNVs.sName = Pairs[1];
                        oNVs.sValue = Pairs[2];
                        //oNVs.UpdatedBy = iUserID;
                        //oNVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        //oNVs.UpdatedTime = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        oNVs.sName = Pairs[1];
                        oNVs.sValue = Pairs[2];
                        oNVs.FKi1ClickID = OneClickID;
                        //oNVs.CreatedBy = oNVs.UpdatedBy = iUserID;
                        //oNVs.CreatedBySYSID = oNVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        //oNVs.CreatedTime = oNVs.UpdatedTime = DateTime.Now;
                        dbContext.XI1ClickNVs.Add(oNVs);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oNVs.ID, Status = true };
        }

        //SaveParametersNameDefaultValuePairs
        public VMCustomResponse SaveParamerterNDVs(int OneClickID, string[] NDVPairs, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XI1ClickParameterNDVs oNDVs = new XI1ClickParameterNDVs();
            var NDVPairsList = dbContext.XI1ClickParameterNDVs.Where(m => m.FKi1ClickID == OneClickID).ToList();
            if (NDVPairs != null && NDVPairs.Count() > 0)
            {
                for (int i = 0; i < NDVPairs.Count(); i++)
                {
                    var Pairs = NDVPairs[i].ToString().Split('^').ToList();
                    int PairID = Convert.ToInt32(Pairs[0]);
                    if (NDVPairsList.Where(m => m.ID == PairID).Select(m => m.ID).FirstOrDefault() != 0)
                    {
                        oNDVs = dbContext.XI1ClickParameterNDVs.Where(m => m.ID == PairID).FirstOrDefault();
                        oNDVs.sName = Pairs[1];
                        oNDVs.sDefault = Pairs[2];
                        oNDVs.sValue = Pairs[3];
                        oNDVs.UpdatedBy = iUserID;
                        oNDVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oNDVs.UpdatedTime = DateTime.Now;
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        oNDVs.sName = Pairs[1];
                        oNDVs.sDefault = Pairs[2];
                        oNDVs.sValue = Pairs[3];
                        oNDVs.FKi1ClickID = OneClickID;
                        oNDVs.CreatedBy = oNDVs.UpdatedBy = iUserID;
                        oNDVs.CreatedBySYSID = oNDVs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oNDVs.CreatedTime = oNDVs.UpdatedTime = DateTime.Now;
                        dbContext.XI1ClickParameterNDVs.Add(oNDVs);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oNDVs.ID, Status = true };
        }

        //SaveXI1ClickLinkPairs
        public VMCustomResponse Save1ClickLinks(VMReports OneClickXILinks, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XI1ClickLinks oXI1CL = new XI1ClickLinks();
            var oXILinkDef = OneClickXILinks.XI1ClickLinks;
            var NDVPairsList = dbContext.XI1ClickLinks.Where(m => m.FKi1ClickID == OneClickXILinks.OneClickID).ToList();
            dbContext.XI1ClickLinks.RemoveRange(NDVPairsList);
            dbContext.SaveChanges();
            if (oXILinkDef != null && oXILinkDef.Count() > 0)
            {
                for (int i = 0; i < oXILinkDef.Count(); i++)
                {
                    var Pairs = oXILinkDef[i].ToString().Split('^').ToList();
                    int PairID = Convert.ToInt32(Pairs[0]);
                    var XiLinkName = Pairs[2];
                    oXI1CL.sName = Pairs[1];
                    var oXILink = dbContext.XiLinks.Where(m => m.Name == XiLinkName).FirstOrDefault();
                    if (oXILink != null)
                    {
                        oXI1CL.FKiXILinkID = oXILink.XiLinkID;
                    }
                    oXI1CL.FKi1ClickID = OneClickXILinks.OneClickID;
                    oXI1CL.sCode = Pairs[3];
                    oXI1CL.iType = Convert.ToInt32(Pairs[4]);
                    //oXI1CL.UpdatedBy = iUserID;
                    //oXI1CL.UpdatedBySYSID = oXI1CL.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    //oXI1CL.UpdatedTime = DateTime.Now;
                    dbContext.XI1ClickLinks.Add(oXI1CL);
                    dbContext.SaveChanges();
                }
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oXI1CL.ID, Status = true };
        }
        public List<VMXI1ClickLinks> XILinkValues(string QueryID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            List<VMXI1ClickLinks> XIOneClickLinks = new List<VMXI1ClickLinks>();
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(QueryID, out iQueryID);
            Guid.TryParse(QueryID, out QueryIDGUID);
            List<XI1ClickLinks> oFieldDef = new List<XI1ClickLinks>();
            if (iQueryID > 0)
            {
                oFieldDef = dbContext.XI1ClickLinks.Where(m => m.FKi1ClickID == iQueryID).ToList();
                Common.SaveErrorLog("Error at Generatequeryrepository: XILinkValues: " + QueryID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                oFieldDef = dbContext.XI1ClickLinks.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).ToList();
            }
            foreach (var item in oFieldDef)
            {
                var lXiFields = dbContext.XiLinks.Where(m => m.XiLinkID == item.FKiXILinkID).ToList();
                if (lXiFields != null && lXiFields.Count > 0)
                {
                    lXiFields.ForEach(m =>
                    {
                        VMXI1ClickLinks XIOneClickPairs = new VMXI1ClickLinks();
                        XIOneClickPairs.ID = item.ID;
                        XIOneClickPairs.sName = item.sName;
                        XIOneClickPairs.XILinkName = m.Name;
                        XIOneClickPairs.sCode = item.sCode;
                        XIOneClickPairs.iType = item.iType;
                        XIOneClickLinks.Add(XIOneClickPairs);
                    });
                }
            }
            return XIOneClickLinks;
        }

        public Dictionary<string, string> AllBusinessObjects(int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetais = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var iOrgID = UserDetais.FKiOrgID;
            var sOrgDB = UserDetais.sUserDatabase;
            if (iOrgID > 0)
            {
                sDatabase = sOrgDB;
            }
            ModelDbContext dbContext = new ModelDbContext();
            Dictionary<string, string> AllBos = new Dictionary<string, string>();
            var lAllBOs = dbContext.BOs.Where(m => m.FKiApplicationID == UserDetais.FKiApplicationID).ToList();
            foreach (var items in lAllBOs)
            {
                //AllBos[items.Name] = items.Name;
                AllBos[items.Name] = items.BOID.ToString();
            }
            return AllBos;
        }


        #region Save1ClickPermission
        //Naresh  
        public VMCustomResponse Save1ClickPermission(int[] NVPairs, string i1ClickID, string sType, int OrgID, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XI1ClickPermissions oCP = new XI1ClickPermissions();
            //var ExistList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickID == i1ClickID).ToList();
            //if (ExistList.Count() > 0 && ExistList != null)
            //{
            //    dbContext.XI1ClickPermissions.RemoveRange(ExistList);
            //    dbContext.SaveChanges();
            //}
            int iQueryID = 0;
            Guid QueryIDGUID = Guid.Empty;
            int.TryParse(i1ClickID, out iQueryID);
            Guid.TryParse(i1ClickID, out QueryIDGUID);
            string IsExist1ClickID = string.Empty;
            if (iQueryID > 0)
            {
                IsExist1ClickID = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickID == iQueryID).Select(m => m.FKi1ClickIDXIGUID).FirstOrDefault().ToString();
                Common.SaveErrorLog("Error at Generatequeryrepository: Save1ClickPermission: " + i1ClickID, "");
            }
            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
            {
                IsExist1ClickID = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).Select(m => m.FKi1ClickIDXIGUID).FirstOrDefault().ToString();
            }
            if (IsExist1ClickID != i1ClickID || sType == "Edit")
            {
                List<int> ExistList = new List<int>();
                if (iQueryID > 0)
                {
                    ExistList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickID == iQueryID).Select(m => m.FKiRoleID).ToList();
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    ExistList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).Select(m => m.FKiRoleID).ToList();
                }
                var oRolesList = dbContext.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).Select(m => m.RoleID).ToList();
                if (sType == "Add")
                {
                    ModelDbContext dbContexts = new ModelDbContext(sDatabase);
                    oRolesList = dbContexts.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).Select(m => m.RoleID).ToList();
                }
                else if (sType == "Edit")
                {
                    oRolesList = NVPairs.Except(ExistList).ToList();
                }
                if (oRolesList != null && oRolesList.Count > 0)
                {
                    oRolesList.ForEach(m =>
                    {
                        oCP.FKi1ClickID = iQueryID;
                        oCP.FKi1ClickIDXIGUID = QueryIDGUID;
                        oCP.FKiRoleID = m;
                        oCP.CreatedBy = oCP.UpdatedBy = iUserID;
                        oCP.CreatedTime = oCP.UpdatedTime = DateTime.Now;
                        oCP.CreatedBySYSID = oCP.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        oCP.StatusTypeID = 10;
                        dbContext.XI1ClickPermissions.Add(oCP);
                        dbContext.SaveChanges();
                    });
                }
                if (NVPairs != null)
                {
                    var oRemove = ExistList.Except(NVPairs).ToList();
                    if (oRemove != null && oRemove.Count() > 0)
                    {
                        for (int j = 0; j < oRemove.Count(); j++)
                        {
                            var RemoveRoleID = oRemove[j];
                            List<XI1ClickPermissions> RemoveList = new List<XI1ClickPermissions>();
                            if (iQueryID > 0)
                            {
                                RemoveList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickID == iQueryID && m.FKiRoleID == RemoveRoleID).ToList();
                                Common.SaveErrorLog("Error at Generatequeryrepository: Save1ClickPermission: " + iQueryID, "");
                            }
                            if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                            {
                                RemoveList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID && m.FKiRoleID == RemoveRoleID).ToList();
                            }
                            dbContext.XI1ClickPermissions.RemoveRange(RemoveList);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            if (sType == "Clear")
            {
                List<XI1ClickPermissions> RemoveList = new List<XI1ClickPermissions>();
                if (iQueryID > 0)
                {
                    RemoveList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickID == iQueryID).ToList();
                    Common.SaveErrorLog("Error at Generatequeryrepository: Save1ClickPermission: " + iQueryID, "");
                }
                if (!string.IsNullOrEmpty(QueryIDGUID.ToString()))
                {
                    RemoveList = dbContext.XI1ClickPermissions.Where(m => m.FKi1ClickIDXIGUID == QueryIDGUID).ToList();
                }
                dbContext.XI1ClickPermissions.RemoveRange(RemoveList);
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, sID = i1ClickID, Status = true };
        }

        #endregion Save1ClickPermission
    }
}
