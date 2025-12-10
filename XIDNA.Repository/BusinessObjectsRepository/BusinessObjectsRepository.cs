using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Data;
using System.Data.SqlClient;
using System.Data.Entity;
using System.Configuration;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using System.Web.Caching;
using System.IO;
using System.Xml;

namespace XIDNA.Repository
{
    public class BusinessObjectsRepository : IBusinessObjectsRepository
    {
        CommonRepository Common = new CommonRepository();
        cXICache oCache = new cXICache();
        CXiAPI oXIAPI = new CXiAPI();
        XiLinkRepository obj = new XiLinkRepository();
        public DTResponse GetBusinessObjects(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int iOrgID = UserDetails.FKiOrgID;
            var FKiAppID = UserDetails.FKiApplicationID;
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<BOs> AllBOs;
            AllBOs = dbContext.BOs.Where(m => m.FKiApplicationID == FKiAppID);
            string sortExpression = "BOID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllBOs = AllBOs.Where(m => m.Name.Contains(param.sSearch) || m.BOID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllBOs.Count();
            AllBOs = QuerableUtil.GetResultsForDataTables(AllBOs, "", sortExpression, param);
            var clients = AllBOs.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.BOID), c.Name,c.TableName, c.Description, Convert.ToString(c.TypeID), Convert.ToString(c.BOFields.Count() +"/"+GetFieldCount(c.Name, iUserID,iOrgID, sOrgName, sDatabase)), c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public BOs GetBOByID(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BO = dbContext.BOs.Find(ID);
            return BO;
        }

        //Save BO changes made
        //public VMCustomResponse SaveBO(BOs model)
        public int SaveBO(BOs model, int iUserID, string sOrgName, string sDatabase)
        {
            int OrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            BOs Bo = new BOs();
            if (model.BOID == 0)
            {
                Bo.Name = model.Name;
                Bo.TableName = model.TableName;
                Bo.Description = model.Description;
                Bo.TypeID = 10;
                Bo.OrganizationID = OrgID;
                Bo.FieldCount = model.FieldCount;
                Bo.StatusTypeID = model.StatusTypeID;
                Bo.FKiApplicationID = model.FKiApplicationID;
                //1/6/2018
                Bo.sVersion = model.sVersion;
                Bo.sSize = model.sSize;
                Bo.bUID = model.bUID;
                Bo.sPrimaryKey = model.sPrimaryKey;
                Bo.sColumns = model.ColName;
                Bo.sTimeStamped = model.sTimeStamped;
                Bo.iTransactionEnable = model.iTransactionEnable;
                if (Bo.sTimeStamped == "10")
                {
                    //CreateTimeStampedDetails(Bo.BOID, Bo.CreatedBy, Bo.CreatedTime, sDatabase);
                }
                Bo.sDeleteRule = model.sDeleteRule;
                Bo.sHelpItem = model.sHelpItem;
                Bo.sNotes = model.sNotes;
                Bo.iDataSource = model.iDataSource;
                if (!string.IsNullOrEmpty(model.sUpdateVersion))
                {
                    Bo.sUpdateVersion = GetVersionForBO(model.sUpdateVersion);
                }
                Bo.iUpdateCount = model.iUpdateCount;
                Bo.sAudit = model.sAudit;
                Bo.sSearchType = model.sSearchType;
                if (model.AuditType == "Audit")
                {
                    Bo.sType = "Audit";
                }
                else
                {
                    Bo.sType = model.sType;
                }
                Bo.sDashBoardType = model.sDashBoardType;
                Bo.sSection = model.sSection;
                Bo.bIsAutoIncrement = model.bIsAutoIncrement;
                Bo.CreatedBy = iUserID;
                Bo.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Bo.CreatedTime = DateTime.Now;
                Bo.UpdatedBy = iUserID;
                Bo.UpdatedTime = DateTime.Now;
                Bo.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Bo.bIsAutoIncrement = model.bIsAutoIncrement;
                Bo.bIsHierarchy = model.bIsHierarchy;
                //1/6/2018
                dbContext.BOs.Add(Bo);
                dbContext.SaveChanges();
                //Create default 1-click
                int BOID = Bo.BOID;
                if (Bo.sType != null && Bo.sType.ToLower() == "masterentity")
                {
                }
                else
                {
                    CreateDefault1Click(BOID);
                }
            }
            else
            {
                Bo = dbContext.BOs.Find(model.BOID);
                Bo.Name = model.Name;
                Bo.sNameAttribute = model.sNameAttribute;
                Bo.Description = model.Description;
                Bo.FieldCount = model.FieldCount;
                Bo.FKiApplicationID = model.FKiApplicationID;
                Bo.StatusTypeID = model.StatusTypeID;
                //1/6/2018
                //Bo.sVersion = model.sVersion;
                Bo.sSize = model.sSize;
                Bo.bUID = model.bUID;
                Bo.sPrimaryKey = model.sPrimaryKey;
                Bo.sTimeStamped = model.sTimeStamped;
                if (Bo.sTimeStamped == "10")
                {
                    //CreateTimeStampedDetails(Bo.BOID, Bo.CreatedBy, DateTime.Now, sDatabase);
                }
                Bo.sDeleteRule = model.sDeleteRule;
                Bo.sHelpItem = model.sHelpItem;
                Bo.sNotes = model.sNotes;
                Bo.iDataSource = model.iDataSource;
                if (!string.IsNullOrEmpty(model.sUpdateVersion))
                {
                    Bo.sUpdateVersion = GetVersionForBO(model.sUpdateVersion);
                }
                Bo.iUpdateCount = model.iUpdateCount;
                Bo.sAudit = model.sAudit;
                Bo.sSearchType = model.sSearchType;
                Bo.sType = model.sType;
                Bo.sDashBoardType = model.sDashBoardType;
                Bo.sSection = model.sSection;
                Bo.bIsAutoIncrement = model.bIsAutoIncrement;
                Bo.UpdatedBy = iUserID;
                Bo.UpdatedTime = DateTime.Now;
                Bo.iTransactionEnable = model.iTransactionEnable;
                Bo.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                Bo.bIsAutoIncrement = model.bIsAutoIncrement;
                Bo.bIsHierarchy = model.bIsHierarchy;
                //1/6/2018
                dbContext.SaveChanges();
            }
            //return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Bo.BOID, Status = true };
            return Bo.BOID;
        }

        //Uncomment it 

        ////create default 1-click for all BOs
        //public string CreateDefault1Click()
        //{
        //    ModelDbContext dbContext = new ModelDbContext();
        //    var BODefs = new List<BOs>();
        //    BODefs = dbContext.BOs.ToList();
        //    var ErrorBOID = new List<string>();
        //    foreach(var BODetails in BODefs)
        //    {
        //        if (BODetails.BOID > 0)
        //        {
        //            //Get BO attributes:
        //            var sTableName = BODetails.TableName;
        //            string sGroupFields = dbContext.BOGroupFields.Where(m=>m.BOID==BODetails.BOID).Where(m => m.GroupName == "List").Select(m => m.BOFieldNames).FirstOrDefault();
        //            if (string.IsNullOrEmpty(sGroupFields))
        //            {
        //                List<BOFields> BOFields = new List<BOFields>();
        //                BOFields = dbContext.BOFields.Where(m => m.BOID == BODetails.BOID).ToList();
        //                var sSelectedFields = BOFields.Take(3);
        //                var sAttrNames = new List<string>();
        //                var sAttrIDs = new List<string>();
        //                var sAttrSQL = new List<string>();
        //                foreach (var sFields in sSelectedFields)
        //                {
        //                    sAttrNames.Add(sFields.LabelName);
        //                    sAttrIDs.Add(sFields.ID.ToString());
        //                    //Convert(varchar(256), Name)+' '+Convert(varchar(256), Description)+' '+Convert(varchar(256), TableName)+' '+Convert(varchar(256), sNotes)
        //                    sAttrSQL.Add(sFields.Name);
        //                }
        //                string sNewAttrSQL = "";
        //                foreach (var FieldSql in sAttrSQL)
        //                {
        //                    if (sNewAttrSQL == "")
        //                    {
        //                        sNewAttrSQL = "Convert(varchar(256), " + FieldSql + ")";
        //                    }
        //                    else
        //                    {
        //                        sNewAttrSQL = sNewAttrSQL + "+' '+ Convert(varchar(256), " + FieldSql + ")";
        //                    }
        //                }
        //                string csvAttrNames = String.Join(",", sAttrNames.Select(x => x.ToString()).ToArray());
        //                string csvAttrIDs = String.Join(",", sAttrIDs.Select(x => x.ToString()).ToArray());
        //                string sGroupName = "List";
        //                try
        //                {
        //                    BOGroupFields bogmodel = new BOGroupFields();
        //                    bogmodel.BOID = BODetails.BOID;
        //                    bogmodel.BOFieldIDs = csvAttrIDs;
        //                    bogmodel.BOSqlFieldNames = sNewAttrSQL;
        //                    bogmodel.BOFieldNames = csvAttrNames;
        //                    bogmodel.GroupName = sGroupName;
        //                    bogmodel.Description = sGroupName + " Description";
        //                    bogmodel.TypeID = 0;
        //                    bogmodel.StatusTypeID = 10;
        //                    bogmodel.CreatedByID = 1;
        //                    bogmodel.CreatedByName = "Admin";
        //                    bogmodel.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        //                    bogmodel.CreatedTime = DateTime.Now;
        //                    bogmodel.IsMultiColumnGroup = false;
        //                    dbContext.BOGroupFields.Add(bogmodel);
        //                    dbContext.SaveChanges();
        //                }
        //                catch (Exception Ex)
        //                {
        //                    ErrorBOID.Add(BODetails.BOID.ToString());
        //                }
        //            }

        //            //create default 1-click from the group
        //            //else
        //            //{
        //                try
        //                {
        //                    string s1ClickName= BODetails.Name + "Default1click";
        //                    int i1ClickID = dbContext.Reports.Where(m => m.BOID == BODetails.BOID).Where(m => m.Name.ToLower() == s1ClickName.ToLower()).Select(m => m.ID).FirstOrDefault();
        //                    if (i1ClickID == 0)
        //                    {
        //                        string sQuery = "Select " + sGroupFields + " from " + sTableName;
        //                        Reports report = new Reports();
        //                        report.OrganizationID = BODetails.OrganizationID;
        //                        report.BOID = BODetails.BOID;
        //                        report.Name = s1ClickName;
        //                        report.Title = "Default 1-Click";
        //                        report.TypeID = 1;
        //                        report.IsParent = false;
        //                        report.CategoryID = 1;
        //                        report.Description = "Default 1-Click for BO";
        //                        report.DisplayAs = 50;
        //                        report.ResultIn = "Inline";
        //                        report.FKiComponentID = 0;
        //                        report.RepeaterType = 0;
        //                        report.RepeaterComponentID = 0;
        //                        report.Class = 43;
        //                        report.Query = sQuery;
        //                        report.VisibleQuery = sQuery.ToString();
        //                        //validation error
        //                        report.PopupType = null;
        //                        report.PopupLeft = 0;
        //                        report.PopupHeight = 0;
        //                        report.PopupTop = 0;
        //                        report.PopupWidth = 0;
        //                        //Validation error including title
        //                        report.SelectFields = null;
        //                        report.FromBos = report.WhereFields = report.GroupFields = null;
        //                        report.OrderFields = null;
        //                        report.StatusTypeID = 10;
        //                        report.CreatedBy = report.UpdatedBy = 1;
        //                        report.CreatedBySYSID = report.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
        //                        report.CreatedTime = report.UpdatedTime = DateTime.Now;
        //                        report.SearchFields = null;
        //                        dbContext.Reports.Add(report);
        //                        dbContext.SaveChanges();
        //                    }
        //                }
        //                catch (Exception Ex)
        //                {

        //                }
        //           // }
        //        }
        //        }

        //    return null;
        //}

        ////Create default 1-click for single bo
        public string CreateDefault1Click(int BOID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            try
            {
                if (BOID > 0)
                {
                    //Get BO Definition
                    BOs BODef = dbContext.BOs.Find(BOID);
                    var sTableName = BODef.TableName;
                    string sGroupFields = dbContext.BOGroupFields.Where(m => m.BOID == BODef.BOID).Where(m => m.GroupName == "List").Select(m => m.BOFieldNames).FirstOrDefault();
                    string csvAttrNames = "";
                    string csvAttrIDs = "";
                    string sNewAttrSQL = "";
                    if (string.IsNullOrEmpty(sGroupFields))
                    {
                        List<BOFields> BOFields = new List<BOFields>();
                        BOFields = dbContext.BOFields.Where(m => m.BOID == BODef.BOID).ToList();
                        if (BOFields.Count > 0)
                        {
                            var sSelectedFields = BOFields.Take(3);
                            var sAttrNames = new List<string>();
                            var sAttrIDs = new List<string>();
                            var sAttrSQL = new List<string>();
                            foreach (var sFields in sSelectedFields)
                            {
                                sAttrNames.Add(sFields.LabelName);
                                sAttrIDs.Add(sFields.ID.ToString());
                                //Convert(varchar(256), Name)+' '+Convert(varchar(256), Description)+' '+Convert(varchar(256), TableName)+' '+Convert(varchar(256), sNotes)
                                sAttrSQL.Add(sFields.Name);
                            }
                            foreach (var FieldSql in sAttrSQL)
                            {
                                if (sNewAttrSQL == "")
                                {
                                    sNewAttrSQL = "Convert(varchar(256), " + FieldSql + ")";
                                }
                                else
                                {
                                    sNewAttrSQL = sNewAttrSQL + "+' '+ Convert(varchar(256), " + FieldSql + ")";
                                }
                            }
                            csvAttrNames = String.Join(",", sAttrNames.Select(x => x.ToString()).ToArray());
                            csvAttrIDs = String.Join(",", sAttrIDs.Select(x => x.ToString()).ToArray());
                        }
                        else
                        {
                            //add the static column name as the attributes for the BO are not made visible.
                            csvAttrNames = "ID";
                            csvAttrIDs = "";
                            sNewAttrSQL = "";
                        }
                    }
                    else
                    {
                        //nothing
                    }
                    string sGroupName = "List";
                    BOGroupFields bogmodel = new BOGroupFields();
                    bogmodel.BOID = BODef.BOID;
                    bogmodel.BOFieldIDs = csvAttrIDs;
                    bogmodel.BOSqlFieldNames = sNewAttrSQL;
                    bogmodel.BOFieldNames = csvAttrNames;
                    bogmodel.GroupName = sGroupName;
                    bogmodel.Description = sGroupName + " Description";
                    bogmodel.TypeID = 0;
                    bogmodel.StatusTypeID = 10;
                    bogmodel.CreatedByID = 1;
                    bogmodel.CreatedByName = "Admin";
                    bogmodel.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    bogmodel.CreatedTime = DateTime.Now;
                    bogmodel.IsMultiColumnGroup = false;
                    dbContext.BOGroupFields.Add(bogmodel);
                    dbContext.SaveChanges();

                    //create default 1-click
                    string s1ClickName = BODef.Name + " Default1click";
                    string sQuery = "";
                    //check 1-Click ID
                    int iCheck1ClickID = dbContext.Reports.Where(m => m.BOID == BODef.BOID).Where(m => m.Name == s1ClickName).Select(m => m.ID).FirstOrDefault();
                    if (iCheck1ClickID == 0)
                    {
                        string sCheckGroupFields = dbContext.BOGroupFields.Where(m => m.BOID == BODef.BOID).Where(m => m.GroupName == "List").Select(m => m.BOFieldNames).FirstOrDefault();
                        if (!string.IsNullOrEmpty(sCheckGroupFields))
                        {
                            sQuery = "Select " + sCheckGroupFields + " from " + sTableName;
                        }
                        else
                        {
                            //create query with static column which is common
                            sQuery = "Select ID from " + sTableName;
                        }
                        Reports report = new Reports();
                        report.OrganizationID = BODef.OrganizationID;
                        report.BOID = BODef.BOID;
                        report.Name = s1ClickName;
                        report.Title = "Default 1-Click";
                        report.TypeID = 1;
                        report.IsParent = false;
                        report.CategoryID = 1;
                        report.Description = "Default 1-Click for BO";
                        report.DisplayAs = 50;
                        report.ResultListDisplayType = 1;
                        report.ResultIn = "Inline";
                        report.FKiComponentID = 0;
                        report.RepeaterType = 0;
                        report.RepeaterComponentID = 0;
                        report.Class = 43;
                        report.VisibleQuery = sQuery.ToString();
                        report.Query = sQuery.ToString();
                        //validation error
                        report.PopupType = null;
                        report.PopupLeft = 0;
                        report.PopupHeight = 0;
                        report.PopupTop = 0;
                        report.PopupWidth = 0;
                        //Validation error including title
                        report.SelectFields = null;
                        report.FromBos = sTableName;
                        report.WhereFields = report.GroupFields = null;
                        report.OrderFields = null;
                        report.StatusTypeID = 10;
                        report.CreatedBy = report.UpdatedBy = 1;
                        report.CreatedBySYSID = report.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        report.CreatedTime = report.UpdatedTime = DateTime.Now;
                        report.SearchFields = null;
                        dbContext.Reports.Add(report);
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        //do nothing as default exists
                    }
                }
            }
            catch (Exception Ex)
            {

            }
            return null;
        }
        public string GetVersionForBO(string sUpdatedVersion)
        {
            float rUpdatedVersion = float.Parse(sUpdatedVersion, CultureInfo.InvariantCulture.NumberFormat);
            rUpdatedVersion += 0.1F;
            return rUpdatedVersion.ToString();
        }

        public string CreateTimeStampedDetails(int ID, int UserID, DateTime dDateTime, string sDatabase)
        {
            var sValues = "'" + dDateTime + "'," + UserID;

            using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                if (ID == 0)
                {
                    cmd.CommandText = "INSERT INTO XITimeStamped_XTS_T (dCreatedWhen,iCreatedByID ) VALUES (" + sValues + ")";
                    Con.Open();
                    //please check and confirm db with ravi
                    Con.ChangeDatabase(sDatabase);
                    cmd.ExecuteNonQuery();
                    Con.Dispose();
                }
                else
                {
                    cmd.CommandText = "INSERT INTO XITimeStamped_XTS_T (dUpdatedWhen,iUpdatedByID ) VALUES (" + sValues + ")";
                    Con.Open();
                    Con.ChangeDatabase(sDatabase);
                    cmd.ExecuteNonQuery();
                    Con.Dispose();
                }
            }
            return null;
        }

        //statically add the values in the table for now
        public List<VMDropDown> GetHelpItems(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllHelpItems = new List<VMDropDown>();
            var HelpDetails = dbContext.XIHelpItem.ToList();
            foreach (var item in HelpDetails)
            {
                AllHelpItems.Add(new VMDropDown
                {
                    text = item.Default,
                    Value = item.ID
                });
            }
            return AllHelpItems;
        }

        public int DeleteBO(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            try
            {
                BOs BODetails = dbContext.BOs.Find(BOID);
                dbContext.BOs.Remove(BODetails);
                dbContext.SaveChanges();
                iStatus = 1;
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }
            return iStatus;
        }

        public VMCustomResponse SaveExtractedBO(BOs model, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int OrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = UserDetails.sUserDatabase;
            var sBODataSource = string.Empty;
            sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            int IsExists = 0;
            string sPrimaryKey = "";
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "SELECT column_name FROM information_schema.key_column_usage WHERE TABLE_NAME = '" + model.TableName + "'";
                SqlDataReader Newreader = cmd.ExecuteReader();
                while (Newreader.Read())
                {
                    sPrimaryKey = Newreader.GetString(0);
                }
                model.sPrimaryKey = sPrimaryKey;
                Newreader.Close();
                Con.Close();
            }
            using (SqlConnection SC = new SqlConnection(sBODataSource))
            {
                string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + model.TableName + "') SELECT 1 ELSE SELECT 0";
                SC.Open();
                SqlCommand Query = new SqlCommand(cmdText, SC);
                IsExists = Convert.ToInt32(Query.ExecuteScalar());
                var lColDatatypes = new List<List<string>>();
                SC.Close();
            }
            if (IsExists == 1)
            {
                BOs Bo = new BOs();
                if (model.BOID == 0)
                {
                    Bo.Name = model.Name;
                    Bo.LabelName = model.Name;
                    Bo.Description = model.Description;
                    Bo.TableName = model.TableName;
                    Bo.sPrimaryKey = model.sPrimaryKey;
                    Bo.TypeID = 10;
                    Bo.OrganizationID = OrgID;
                    Bo.FieldCount = model.FieldCount;
                    Bo.iDataSource = model.iDataSource;
                    Bo.StatusTypeID = model.StatusTypeID;
                    Bo.FKiApplicationID = model.FKiApplicationID;
                    Bo.CreatedBy = model.CreatedBy;
                    Bo.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Bo.CreatedTime = DateTime.Now;
                    Bo.UpdatedBy = model.UpdatedBy;
                    Bo.UpdatedTime = DateTime.Now;
                    Bo.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    dbContext.BOs.Add(Bo);
                    dbContext.SaveChanges();
                    int BOID = Bo.BOID;
                    CreateDefault1Click(BOID);
                }
                else
                {
                    Bo = dbContext.BOs.Find(model.BOID);
                    Bo.Name = model.Name;
                    Bo.LabelName = model.Name;
                    Bo.TableName = model.TableName;
                    Bo.sPrimaryKey = model.sPrimaryKey;
                    Bo.Description = model.Description;
                    Bo.FieldCount = model.FieldCount;
                    Bo.iDataSource = model.iDataSource;
                    Bo.StatusTypeID = model.StatusTypeID;
                    Bo.FKiApplicationID = model.FKiApplicationID;
                    Bo.UpdatedBy = model.UpdatedBy;
                    Bo.UpdatedTime = DateTime.Now;
                    Bo.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    dbContext.SaveChanges();
                }
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = Bo.BOID, Status = true };
            }
            else
            {
                return new VMCustomResponse { ResponseMessage = ServiceConstants.TableDosntExist, Status = false };
            }

        }

        private int GetFieldCount(string BOName, int iUserID, int OrgID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            int fieldcount = 0;
            try
            {
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = Con;
                    string BoName = string.Empty;
                    var oBO = dbContext.BOs.Where(m => m.Name == BOName).FirstOrDefault();
                    if (oBO == null)
                    {
                        BoName = BOName;
                    }
                    else if (oBO.TableName == null)
                    {
                        BoName = oBO.Name;
                    }
                    else
                    {
                        BoName = oBO.TableName;
                    }

                    cmd.CommandText = "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='" + BoName + "'";
                    if (BoName != EnumLeadTables.Reports.ToString())
                    {
                        //Con.ChangeDatabase(sOrgDb);
                        var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, OrgID, sDatabase, sOrgDb);
                        if (!string.IsNullOrEmpty(sBODataSource))
                        {
                            SqlConnection SC = new SqlConnection(sBODataSource);
                            SC.Open();
                            cmd.Connection = SC;
                            SqlDataReader Newreader = cmd.ExecuteReader();
                            while (Newreader.Read())
                            {
                                fieldcount = Newreader.GetInt32(0);
                            }
                            Con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("Config Error: GetFieldCount() - Database not found while getting table columns count", sDatabase, "Config Error");
            }
            return fieldcount;
        }
        public List<BOFields> GetBOFields(int BOID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            List<BOFields> AssignedFields = new List<BOFields>();
            List<BOTreeViewModel> BOModel = new List<ViewModels.BOTreeViewModel>();
            var oBO = dbContext.BOs.Find(BOID);
            string BOName = string.Empty;
            //if (oBO.TableName == null)
            //{
            BOName = oBO.Name;
            //}
            //else
            //{
            //    BOName = oBO.TableName;
            //}
            AssignedFields = oBO.BOFields.Where(m => m.BOID == BOID).ToList();// && m.OrganizationID == OrgID

            int fieldcount = 0;
            List<BOFields> BOFields = new List<Models.BOFields>();
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME ='" + oBO.TableName + "'";
                SqlDataReader Newreader = cmd.ExecuteReader();
                while (Newreader.Read())
                {
                    fieldcount = Newreader.GetInt32(0);
                }
                Newreader.Close();
                if (AssignedFields.Count() <= fieldcount)
                {
                    cmd.CommandText = "SELECT table_name, column_name as 'Column Name', data_type as 'Data Type', character_maximum_length as 'Max Length' FROM information_schema.columns WHERE table_name LIKE '" + oBO.TableName + "'";
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);
                    BOFields = dt.AsEnumerable().Select(row =>
                    new BOFields
                    {
                        BOID = BOID,
                        TableName = row.Field<string>("table_name"),
                        Name = row.Field<string>("Column Name"),
                        LabelName = row.Field<string>("Column Name"),
                        DataType = row.Field<string>("Data Type"),
                        FieldMaxLength = row.Field<string>("Data Type") == "int" ? "0" : (row.Field<string>("Data Type") == "smalldatetime" ? "0" : (row.Field<string>("Data Type") == "decimal" ? "0" : (row.Field<string>("Data Type") == "tinyint" ? "0" : (row.Field<string>("Data Type") == "nvarchar" ? "max" : (row.Field<string>("Data Type") == "bigint" ? "0" : (row.Field<string>("Data Type") == "datetime" ? "0" : (row.Field<string>("Data Type") == "bit" ? "0" : (row.Field<string>("Data Type") == "float" ? "0" : row.Field<int?>("Max Length").ToString())))))))),
                    }).ToList();
                    Con.Close();
                    var MisMatch = BOFields.Select(m => m.Name.ToLower()).ToList().Except(AssignedFields.Select(m => m.Name.ToLower()).ToList()).ToList();
                    var Remaining = BOFields.Where(m => MisMatch.Contains(m.Name.ToLower())).ToList();

                    for (int i = 0; i < Remaining.Count(); i++)
                    {
                        if (Remaining[i].Name.ToLower() != "XICreatedBy".ToLower() && Remaining[i].Name.ToLower() != "XIUpdatedBy".ToLower() && Remaining[i].Name.ToLower() != "XIUpdatedWhen".ToLower() && Remaining[i].Name.ToLower() != "XICreatedWhen".ToLower())
                        {
                            bool IsFK = false;
                            bool IsCM = false;
                            string sFormat = "";
                            //Need to work on dt (datetime)
                            string sName = Remaining[i].Name;
                            var Char = sName.Select(c => char.IsUpper(c)).ToList();
                            var Position = Char.IndexOf(true);
                            if (Position == 1)
                            {
                                char FirstLetter = sName[0];
                                if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                                {
                                    sName = sName.Substring(1, sName.Length - 1);
                                }

                                //Get format by checking the substring
                                string sFrstLetter = Remaining[i].Name.Substring(0, 1);
                                if (sFrstLetter == "d")
                                {
                                    sFormat = "dd-MMM-yyyy";
                                }
                                else if (sFrstLetter == "r")
                                {
                                    sFormat = "en-GB";
                                }
                            }
                            else if (Position == 2)
                            {
                                string sFrstTwoLetters = sName.Substring(0, 2);
                                if (sFrstTwoLetters == "dt")
                                {
                                    sName = sName.Substring(2, sName.Length - 2);
                                    sFormat = "dd-MMM-yyyy HH:MM:SS";
                                }
                            }
                            else if (Position == 0)
                            {
                                if (sName.Length > 3)
                                {
                                    var Prefix = sName.Substring(0, 2);
                                    if (Prefix == "FK")
                                    {
                                        IsFK = true;
                                        sName = sName.Substring(2, sName.Length - 2);
                                        var Character = sName.Select(c => char.IsUpper(c)).ToList();
                                        var CharPosition = Character.IndexOf(true);
                                        if (CharPosition == 1)
                                        {
                                            char FirstLetter = sName[0];
                                            if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                                            {
                                                sName = sName.Substring(1, sName.Length - 1);
                                            }
                                        }
                                        if (sName.Contains("ID"))
                                        {
                                            sName = sName.Replace("ID", "");
                                        }
                                    }
                                    else if (Prefix == "CM")
                                    {
                                        IsCM = true;
                                        sName = sName.Substring(2, sName.Length - 2);
                                    }
                                }
                            }
                            else if (Position > 2)
                            {
                                if (sName.Length > 3)
                                {
                                    var Prefix = sName.Substring(0, 3);
                                    if (Prefix == "ref")
                                    {
                                        sName = sName.Substring(3, sName.Length - 3);
                                    }
                                }
                            }
                            Remaining[i].LabelName = sName;
                            Remaining[i].IsVisible = true;
                            Remaining[i].Format = sFormat;
                            if (IsFK)
                            {
                                Remaining[i].FKTableName = sName;
                            }
                            AssignedFields.Add(Remaining[i]);
                        }
                    }
                    return AssignedFields;
                }
                else
                {
                    foreach (var items in AssignedFields)
                    {
                        int TypeID = items.TypeID;
                        BODatatypes DataType = (BODatatypes)TypeID;
                        string type = DataType.ToString();
                        items.DataType = type;
                    }
                    return AssignedFields;
                }
            }
        }

        public int CreateAttributeForField(List<string> Labels, List<string> Checkbox, string FieldName, string sDatabase)
        {
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                BOFields model = new BOFields();
                List<string> Value = new List<string>();
                model.BOID = Convert.ToInt32(Labels[0]);
                model.Name = Labels[1];
                var types = Enum.GetValues(typeof(BODatatypes));
                string s = Labels[2];
                int key = 0;
                key = (int)Enum.Parse(typeof(BODatatypes), s);
                model.TypeID = key;
                model.LabelName = FieldName;
                model.Description = FieldName + " Description";
                if (model.TypeID == 180)
                {
                    model.MaxLength = "128";
                }
                else
                {
                    model.MaxLength = "0";
                }
                model.IsVisible = Convert.ToBoolean(Checkbox[0]);
                model.IsWhere = Convert.ToBoolean(Checkbox[1]);
                model.IsTotal = Convert.ToBoolean(Checkbox[2]);
                model.IsGroupBy = Convert.ToBoolean(Checkbox[3]);
                model.IsOrderBy = Convert.ToBoolean(Checkbox[4]);
                model.IsExpression = Convert.ToBoolean(Checkbox[5]);
                model.IsMail = Convert.ToBoolean(Checkbox[6]);
                model.WhereExpression = model.WhereExpreValue = null;
                model.DBQuery = model.ExpressionText = model.ExpressionValue = null;
                model.DateExpression = model.DateValue = model.Description = null;
                model.StatusTypeID = 10;
                model.CreatedByName = "Admin";
                model.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                model.CreatedByID = 1;
                model.CreatedTime = DateTime.Now;
                dbContext.BOFields.Add(model);
                dbContext.SaveChanges();
                return model.ID;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int AddBOAttributes(List<BOFields> model, string sDatabase, int iUserID)
        {
            var sdff = model.Where(m => m.LabelName == null).ToList();
            ModelDbContext dbContext = new ModelDbContext();
            foreach (var items in model)
            {
                if (items.ID == 0)
                {
                    BOFields oBOfield = new BOFields();
                    oBOfield.BOID = items.BOID;
                    oBOfield.Name = items.Name;
                    oBOfield.LabelName = items.LabelName;
                    oBOfield.Format = items.Format;
                    oBOfield.FKTableName = items.FKTableName;
                    oBOfield.Script = items.Script;
                    var values = Enum.GetValues(typeof(BODatatypes));
                    string s = items.DataType;
                    int key = 0;
                    key = (int)Enum.Parse(typeof(BODatatypes), s.ToUpper());
                    if (items.MaxLength == "-1" || items.MaxLength == "max")
                    {
                        oBOfield.MaxLength = "MAX";
                    }
                    else
                    {
                        if (items.MaxLength != null)
                        {
                            oBOfield.MaxLength = items.MaxLength;
                        }
                        else
                        {
                            oBOfield.MaxLength = "0";
                        }
                    }
                    oBOfield.TypeID = key;
                    oBOfield.IsVisible = items.IsVisible;
                    oBOfield.IsWhere = items.IsWhere;
                    oBOfield.IsTotal = items.IsTotal;
                    oBOfield.IsGroupBy = items.IsGroupBy;
                    oBOfield.IsOrderBy = items.IsOrderBy;
                    oBOfield.IsExpression = items.IsExpression;
                    oBOfield.IsMail = items.IsMail;
                    oBOfield.StatusTypeID = 10;
                    oBOfield.Description = items.Description;
                    oBOfield.CreatedByName = items.CreatedByName;
                    oBOfield.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oBOfield.CreatedByID = iUserID;
                    oBOfield.CreatedTime = DateTime.Now;
                    oBOfield.DBQuery = oBOfield.ExpressionText = oBOfield.ExpressionValue = null;
                    oBOfield.DateExpression = oBOfield.DateValue = null;
                    dbContext.BOFields.Add(oBOfield);
                    dbContext.SaveChanges();
                }
                else
                {
                    BOFields oBOField = dbContext.BOFields.Find(items.ID);
                    oBOField.LabelName = items.LabelName;
                    oBOField.Format = items.Format;
                    oBOField.FKTableName = items.FKTableName;
                    oBOField.Script = items.Script;
                    oBOField.Description = items.Description;
                    oBOField.IsVisible = items.IsVisible;
                    oBOField.IsWhere = items.IsWhere;
                    if (oBOField.IsWhere == false)
                    {
                        oBOField.IsRunTime = false;
                        oBOField.IsDate = false;
                        oBOField.DateExpression = null;
                        oBOField.DateValue = null;
                        oBOField.IsWhereExpression = false;
                        oBOField.WhereExpression = null;
                        oBOField.WhereExpreValue = null;
                        oBOField.IsDBValue = false;
                        oBOField.DBQuery = null;
                    }
                    oBOField.IsTotal = items.IsTotal;
                    oBOField.IsGroupBy = items.IsGroupBy;
                    oBOField.IsOrderBy = items.IsOrderBy;
                    oBOField.IsExpression = items.IsExpression;
                    if (oBOField.IsExpression == false)
                    {
                        oBOField.ExpressionText = null;
                        oBOField.ExpressionValue = null;
                    }
                    oBOField.IsMail = items.IsMail;
                    dbContext.SaveChanges();
                }
            }
            return 1;
        }

        public List<BOs> GetAllBos(int iUserID, string sOrgName, string sDatabase)
        {
            cXICache oCache = new cXICache();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var fkiApplicationID = UserDetails.FKiApplicationID;
            string sCacheKey = "allbos_" + fkiApplicationID;
            var oCacheObj = oCache.GetFromCache(sCacheKey);
            List<BOs> AllBos = new List<BOs>();
            if (oCacheObj == null)
            {
                ModelDbContext dbContext = new ModelDbContext();
                AllBos = dbContext.BOs.Where(m => m.FKiApplicationID == fkiApplicationID).ToList();
                oCache.InsertIntoCache(AllBos, sCacheKey);
            }
            else
            {
                AllBos = (List<BOs>)oCacheObj;
            }
            return AllBos;
        }

        public int AddAttributeGroup(BOGroupFields model, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sTableName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.TableName).FirstOrDefault();
            var sGUID = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.XIGUID).FirstOrDefault();
            if (model.ID == 0)
            {
                List<string> IDsNames = new List<string>();
                IDsNames = model.BOFieldNames.Split(',').ToList();
                string fieldids = "", fieldsqlnames = "", fieldnames = "";
                if (model.IsMultiColumnGroup == false)
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split('-').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldsqlnames = fieldsqlnames + "Convert(varchar(256), " + "[" + sTableName + "]." + IDs[1] + ")" + "+' '+";
                        fieldnames = fieldnames + IDs[1] + ", ";
                    }
                    string GroupName = model.GroupName;
                    //if (GroupName.Contains(' '))
                    //{
                    //    GroupName = GroupName.Replace(' ', '_');
                    //}
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 5);
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                }
                else
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split('-').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldnames = fieldnames + IDs[1] + ", ";
                        fieldsqlnames = fieldsqlnames + IDs[1] + ", ";
                    }
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 2);
                }
                fieldids = fieldids.Substring(0, fieldids.Length - 2);
                BOGroupFields bogmodel = new BOGroupFields();
                bogmodel.BOID = model.BOID;
                bogmodel.BOIDXIGUID = sGUID;
                bogmodel.BOFieldIDs = fieldids;
                bogmodel.BOSqlFieldNames = fieldsqlnames;
                bogmodel.BOFieldNames = fieldnames;
                bogmodel.GroupName = model.GroupName;
                bogmodel.Description = model.GroupName + " Description";
                bogmodel.TypeID = 0;
                bogmodel.StatusTypeID = 10;
                bogmodel.CreatedByID = iUserID;
                bogmodel.CreatedByName = model.CreatedByName;
                bogmodel.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                bogmodel.CreatedTime = DateTime.Now;
                bogmodel.IsMultiColumnGroup = model.IsMultiColumnGroup;
                dbContext.BOGroupFields.Add(bogmodel);
                dbContext.SaveChanges();
                return bogmodel.ID;
            }
            else
            {
                List<string> IDsNames = new List<string>();
                IDsNames = model.BOFieldNames.Split(',').ToList();
                string fieldids = "", fieldnames = "", fieldsqlnames = "";
                if (model.IsMultiColumnGroup == false)
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split('-').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldsqlnames = fieldsqlnames + "Convert(varchar(256), " + "[" + sTableName + "]." + IDs[1] + ")" + "+' '+";
                        fieldnames = fieldnames + IDs[1] + ", ";
                    }
                    string GroupName = model.GroupName;
                    //if (GroupName.Contains(' '))
                    //{
                    //    GroupName = GroupName.Replace(' ', '_');
                    //}
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 5);
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                }
                else
                {
                    foreach (var items in IDsNames)
                    {
                        List<string> IDs = items.Split('-').ToList();
                        fieldids = fieldids + IDs[0] + ", ";
                        fieldnames = fieldnames + IDs[1] + ", ";
                        fieldsqlnames = fieldsqlnames + IDs[1] + ", ";
                    }
                    fieldnames = fieldnames.Substring(0, fieldnames.Length - 2);
                    fieldsqlnames = fieldsqlnames.Substring(0, fieldsqlnames.Length - 2);
                }
                fieldids = fieldids.Substring(0, fieldids.Length - 2);

                BOGroupFields bogmodel = dbContext.BOGroupFields.Find(model.ID);
                bogmodel.BOFieldIDs = fieldids;
                bogmodel.BOSqlFieldNames = fieldsqlnames;
                bogmodel.BOFieldNames = fieldnames;
                bogmodel.GroupName = model.GroupName;
                bogmodel.IsMultiColumnGroup = model.IsMultiColumnGroup;
                dbContext.SaveChanges();
                return bogmodel.ID;
            }

        }
        public DTResponse GetAttributeGroups(jQueryDataTableParamModel param, int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IEnumerable<BOGroupFields> AllGroups;
            int displayCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                AllGroups = dbContext.BOGroupFields.Where(m => m.BOID == BOID).Where(m => m.GroupName.Contains(param.sSearch.ToUpper())).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displayCount = AllGroups.Count();
            }
            else
            {
                displayCount = dbContext.BOGroupFields.Where(m => m.BOID == BOID).Count();
                AllGroups = dbContext.BOGroupFields.Where(m => m.BOID == BOID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();

            }
            var result = (from bog in AllGroups
                          join bo in dbContext.BOs on bog.BOID equals bo.BOID
                          select new[] {
                             (i++).ToString(), Convert.ToString(bog.BOID), Convert.ToString(bog.ID),  bo.Name, bog.GroupName, bog.BOFieldNames, Convert.ToString(bog.IsMultiColumnGroup), "" }).ToList();
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displayCount,
                iTotalDisplayRecords = displayCount,
                aaData = result
            };
        }

        public BOAttributeVIewModel GetBOFieldsByID(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOs model = dbContext.BOs.Find(BOID);
            BOAttributeVIewModel Attr = new BOAttributeVIewModel();
            List<ListBoxItems> list = new List<ListBoxItems>();
            foreach (var items in model.BOFields)
            {
                ListBoxItems Items = new ListBoxItems();
                Items.ID = items.ID + "-" + items.Name;
                Items.FieldName = items.Name;
                list.Add(Items);
            }
            Attr.AvailableFields = list;
            Attr.BOID = BOID;
            return Attr;
        }

        public bool IsExistsBOName(string Name, int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var AllBOs = dbContext.BOs.ToList();
            BOs Bo = AllBOs.Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (Bo != null)
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
                if (Bo != null)
                {
                    if (ID == Bo.BOID)
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

        public bool IsExistsGroupName(string GroupName, int BOGroupId, int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BOGroupFields = dbContext.BOGroupFields.ToList();
            BOGroupFields Group = BOGroupFields.Where(m => m.BOID == BOID).Where(m => m.GroupName.Equals(GroupName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (BOGroupId == 0)
            {
                if (Group != null)
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
                if (Group != null)
                {
                    if (BOGroupId == Group.ID)
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

        public BOAttributeVIewModel EditBOAttributeGroup(int GroupID, int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
            BOGroupFields model = dbContext.BOGroupFields.Find(GroupID);
            BOAttributeVIewModel group = new BOAttributeVIewModel();
            group.BOID = model.BOID;
            group.BOGroupId = GroupID;
            group.GroupName = model.GroupName;
            List<string> ids = model.BOFieldIDs.Split(',').ToList();
            List<int> AssignedIds = new List<int>();
            if (model.BOFieldIDs == "0")
            {
                group.AssignedFields = new List<ListBoxItems>();
            }
            else
            {
                var newids = model.BOFieldIDs.Split(new string[] { ", " }, StringSplitOptions.None);

                List<ListBoxItems> AssignedFields = new List<ListBoxItems>();
                foreach (var items in newids)
                {
                    int ID = Convert.ToInt32(items);
                    BOFields bof = BOFields.Where(m => m.ID == ID).FirstOrDefault();
                    ListBoxItems Items = new ListBoxItems();
                    Items.ID = bof.ID + "-" + bof.Name;
                    Items.FieldName = bof.Name;
                    AssignedFields.Add(Items);
                    AssignedIds.Add(Convert.ToInt32(items));
                }
                group.AssignedFields = AssignedFields;
            }

            List<int> AllIds = BOFields.Where(m => m.BOID == BOID).Select(m => m.ID).ToList();
            IEnumerable<int> Remaining = AllIds.Except(AssignedIds).ToList();
            List<ListBoxItems> list = new List<ListBoxItems>();
            foreach (var field in Remaining)
            {
                BOFields bo = BOFields.Where(m => m.ID == field).FirstOrDefault();
                ListBoxItems Items = new ListBoxItems();
                Items.ID = bo.ID + "-" + bo.Name;
                Items.FieldName = bo.Name;
                list.Add(Items);
            }
            group.AvailableFields = list;
            group.IsMultiColumnGroup = model.IsMultiColumnGroup;
            return group;
        }

        public int RemoveGroup(int GroupID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOGroupFields model = dbContext.BOGroupFields.Find(GroupID);
            int BOID = model.BOID;
            dbContext.BOGroupFields.Remove(model);
            dbContext.SaveChanges();
            return BOID;
        }
        public int CreatePopUpItems(VMWherePopUP model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOFields popup = dbContext.BOFields.Where(m => m.ID == model.FieldID).SingleOrDefault();
            popup.IsRunTime = model.IsRuntimeValue;
            popup.IsDBValue = model.IsDBValue;
            if (model.DBQuery == null)
            {
                popup.DBQuery = null;
            }
            else
            {
                popup.DBQuery = model.DBQuery;
            }
            popup.IsWhereExpression = model.IsWhereExpression;
            if (model.IsWhereExpression)
            {
                popup.WhereExpression = model.WhereExpression;
                popup.WhereExpreValue = model.WhereExpressionValue;
            }
            popup.IsExpression = model.IsExpression;
            if (model.IsExpression)
            {
                popup.ExpressionText = model.ExpressionText;
                popup.ExpressionValue = model.ExpressionValue;
            }
            popup.IsDate = model.IsDate;
            if (model.IsDate == true)
            {
                popup.DateExpression = "Yesterday,Today,Tomorrow";
                popup.DateValue = "DATEADD(day,datediff(day,1,GETDATE()),0)-GETDATE()-DATEADD(day, 1, GETDATE())";
            }
            else
            {
                popup.DateExpression = null;
                popup.DateValue = null;
            }
            dbContext.SaveChanges();
            return model.FieldID;
        }
        public BOFields GetPopUpDataByID(int FieldID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOFields model = dbContext.BOFields.Find(FieldID);
            var BoName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
            model.BOName = BoName;
            int TypeID = model.TypeID;
            BODatatypes DataType = (BODatatypes)TypeID;
            string type = DataType.ToString();
            model.DataType = type;
            return model;
        }

        public int SaveScript(int ID, string Script, string ExecutionType, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOFields Field = new BOFields();
            Field = dbContext.BOFields.Find(ID);
            Field.Script = Script;
            Field.ScriiptExecutionType = ExecutionType;
            dbContext.SaveChanges();
            return Field.ID;
        }

        public List<string> ValidateScript(string Script)
        {
            Script = Script.Replace("\n", " ");
            Script = Script.Replace(@"\\\", @"\");
            try
            {
                using (Microsoft.CSharp.CSharpCodeProvider foo = new Microsoft.CSharp.CSharpCodeProvider())
                {
                    //Script = "if(50==50 && \"NR32 1DR\".Contains(\"DR\")){ return \"<TABLE>XYZ</TABLE>\";}else{ return \"<TABLE>PQR</TABLE>\";}";
                    var res = foo.CompileAssemblyFromSource(
                        new System.CodeDom.Compiler.CompilerParameters()
                        {
                            GenerateInMemory = true
                        },
                        "public class FooClass { public string Execute() {" + Script + "}}"
                    );
                    var type = res.CompiledAssembly.GetType("FooClass");
                    var obj = Activator.CreateInstance(type);
                    var ScrpResult = type.GetMethod("Execute").Invoke(obj, new object[] { }).ToString();
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        //poovanna 2/12/2017
        public BOFields GetBOAtrributesForm(string FieldName, int BOID, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int OrgID = UserDetails.FKiOrgID;
            ModelDbContext dbContext = new ModelDbContext();
            DataContext DBContext = new DataContext();
            BOFields oFieldDetails = new BOFields();
            List<BOTreeViewModel> BOModel = new List<ViewModels.BOTreeViewModel>();
            var oBO = dbContext.BOs.Find(BOID);
            string BOName = oBO.Name;
            string TableName = oBO.TableName;
            // int BOID = dbContext.BOs.Where(m => m.Name == FieldName).Select(m => m.BOID).FirstOrDefault();
            //When the BO's are created the OrgID used is zero manually stored a BO is not organisation depended so here orgID is statically entered.
            oFieldDetails = dbContext.BOFields.Where(m => m.BOID == BOID && m.Name.ToLower() == FieldName.ToLower()).FirstOrDefault();
            int TypeID = oFieldDetails.TypeID;
            BODatatypes DataType = (BODatatypes)TypeID;
            string type = DataType.ToString();
            oFieldDetails.DataType = type;
            var sOrgDB = UserDetails.sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            BOFields results = new BOFields();
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                //cmd.CommandText = "SELECT table_name, column_name as 'Column Name', data_type as 'Data Type', character_maximum_length as 'Max Length' FROM information_schema.columns WHERE table_name LIKE '" + BOName + "' AND COLUMN_NAME='" + FieldName + "'";
                cmd.CommandText = "SELECT table_name, column_name as 'Column Name', data_type as 'Data Type', character_maximum_length as 'Max Length' FROM information_schema.columns WHERE table_name LIKE '" + TableName + "' AND COLUMN_NAME='" + FieldName + "'";
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    results.TableName = reader.GetString(0);
                    results.Name = reader.GetString(1);
                    results.DataType = reader.GetString(2).ToUpper();
                    results.FieldMaxLength = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString();
                    results.BOID = BOID;
                    results.LabelName = reader.GetString(1);
                    results.BOName = BOName;
                }
                Con.Close();
            }
            bool IsCM = false;
            bool IsFK = false;
            string sFormat = "";
            //Need to work on dt (datetime)
            string sName = results.Name;

            //get the label name
            var Char = sName.Select(c => char.IsUpper(c)).ToList();
            var Position = Char.IndexOf(true);
            if (Position == 1)
            {
                char FirstLetter = sName[0];
                if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                {
                    sName = sName.Substring(1, sName.Length - 1);
                }

                //Get format by checking the substring
                string sFrstLetter = results.Name.Substring(0, 1);
                if (sFrstLetter == "d")
                {
                    sFormat = "dd/mm/yyyy";
                }
                else if (sFrstLetter == "r")
                {
                    sFormat = "£";
                }
            }
            else if (Position == 2)
            {
                string sFrstTwoLetters = sName.Substring(0, 2);
                if (sFrstTwoLetters == "dt")
                {
                    sName = sName.Substring(2, sName.Length - 2);
                    sFormat = "dd/mm/yyyy HH:MM:SS";
                }
            }
            else if (Position == 0)
            {
                if (sName.Length > 3)
                {
                    var Prefix = sName.Substring(0, 2);
                    if (Prefix == "FK")
                    {
                        IsFK = true;
                        sName = sName.Substring(2, sName.Length - 2);
                        var Character = sName.Select(c => char.IsUpper(c)).ToList();
                        var CharPosition = Character.IndexOf(true);
                        if (CharPosition == 1)
                        {
                            char FirstLetter = sName[0];
                            if (FirstLetter == 'i' || FirstLetter == 's' || FirstLetter == 'd' || FirstLetter == 'r' || FirstLetter == 'n')
                            {
                                sName = sName.Substring(1, sName.Length - 1);
                            }
                        }
                        if (sName.Contains("ID"))
                        {
                            sName = sName.Replace("ID", "");
                        }
                    }
                    else if (Prefix == "CM")
                    {
                        IsCM = true;
                        sName = sName.Substring(2, sName.Length - 2);
                    }
                }
            }
            else if (Position > 2)
            {
                if (sName.Length > 3)
                {
                    var Prefix = sName.Substring(0, 3);
                    if (Prefix == "ref")
                    {
                        sName = sName.Substring(3, sName.Length - 3);
                    }
                }
            }


            //get file type ID 
            int iFileType = dbContext.BOFields.Where(m => m.Name == FieldName).Where(m => m.BOID == BOID).Where(m => m.OrganizationID == 0).Select(m => m.FKiFileTypeID).FirstOrDefault();
            if (iFileType != 0)
            {
                results.FKiFileTypeID = iFileType;
            }

            oFieldDetails.TableName = results.TableName;
            //oFieldDetails.LabelName = sName;
            oFieldDetails.IsVisible = true;
            oFieldDetails.Format = sFormat;
            if (IsFK)
            {
                //oFieldDetails.FKTableName = sName;
            }

            //include max length in datatype.
            if (results.FieldMaxLength != "" || results.FieldMaxLength != "0")
            {
                oFieldDetails.DataType = results.DataType + "(" + results.FieldMaxLength + ")";
            }

            ////Add File settings Dropdown 5/2/2018
            XIFileTypes Files = new XIFileTypes();
            var FileDetails = dbContext.XIFileTypes.ToList().Select(m => new VMDropDown() { text = m.Name, Value = m.ID }).ToList();
            FileDetails.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            oFieldDetails.FileDetails = FileDetails;
            List<VMDropDown> SysTypes = new List<VMDropDown>();
            SysTypes = dbContext.Types.Where(m => m.Name == "Sys Type").ToList().Select(m => new VMDropDown { Value = m.ID, Expression = m.Expression }).ToList();
            oFieldDetails.SysType = SysTypes;
            var sOneClickName = dbContext.Reports.Where(m => m.ID == oFieldDetails.iOneClickID).Select(m => m.Name).FirstOrDefault();
            oFieldDetails.sOneClickName = sOneClickName;
            oFieldDetails.Type = oBO.sType;
            //oFieldDetails.AttrBOOptionList = dbContext.BOOptionLists.Where(m => m.BOFieldID == BOID).ToList();
            return oFieldDetails;
        }
        public Dictionary<string, string> GetOneClicks(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dictionary<string, string> XIFields = new Dictionary<string, string>();
            var lXiFields = dbContext.Reports.ToList();
            foreach (var items in lXiFields)
            {
                XIFields[items.Name] = items.ID.ToString();
            }
            return XIFields;
        }

        //Editing option list
        public List<List<string>> EditBoOptionList(int BOID, string AtrName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<BOOptionLists> OptionFields = new List<BOOptionLists>();
            var lOptionNames = new List<string>();
            var lOptionValues = new List<string>();
            var lOptionStatus = new List<string>();
            var lOptionCode = new List<string>();
            var lValues = new List<List<string>>();
            int iID = 0;
            OptionFields = dbContext.BOOptionLists.Where(m => m.BOID == BOID).Where(m => m.Name == AtrName).ToList();
            string[] sOpNames = { };
            string[] sOpValues = { };
            string[] sOpStatus = { };
            string[] sOpCode = { };
            for (var i = 0; i < OptionFields.Count(); i++)
            {
                lOptionNames = Regex.Split(OptionFields[i].sOptionName, ",").ToList();
                lOptionValues = Regex.Split(OptionFields[i].sValues, ",").ToList();
                if (OptionFields[i].sOptionCode != null && OptionFields[i].sOptionCode != "")
                {
                    lOptionCode = Regex.Split(OptionFields[i].sOptionCode, ",").ToList();
                }
                else
                {
                    lOptionCode = Regex.Split("", ",").ToList();
                }
                lOptionStatus = Regex.Split(OptionFields[i].StatusTypeID.ToString(), ",").ToList();
                iID = OptionFields[i].ID;
                if (lOptionNames.Count() == lOptionValues.Count())
                {
                    for (var j = 0; j < lOptionNames.Count(); j++)
                    {
                        var lVals = new List<string>();
                        lVals.Add(lOptionNames[j] + '^' + iID);
                        lVals.Add(lOptionValues[j]);
                        if (OptionFields[i].sOptionCode != null && OptionFields[i].sOptionCode.Count() >= 1)
                        {
                            lVals.Add(lOptionCode[j]);
                        }
                        else
                        {
                            lVals.Add("");
                        }
                        lVals.Add(lOptionStatus[j]);
                        lValues.Add(lVals);
                    }
                }
            }
            return lValues;
        }

        //Check if option list exists
        public bool CheckBoOptionList(int BOID, string AtrName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var OptionFields = dbContext.BOOptionLists.Where(m => m.BOID == BOID).Where(m => m.Name == AtrName).Select(m => m.sOptionName).FirstOrDefault();
            if (OptionFields != "" && OptionFields != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DelectCheckBoOptionList(int BOID, int BOFieldID, string AtrName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var ExistingUnCheckBoOptionList = dbContext.BOOptionLists.Where(m => m.BOID == BOID).Where(m => m.BOFieldID == BOFieldID).Where(m => m.Name == AtrName).ToList();
            dbContext.BOOptionLists.RemoveRange(ExistingUnCheckBoOptionList);
            dbContext.SaveChanges();
            return true;
        }

        //Delete option list
        public int DeleteBoOptionList(int BOID, int iID, string AtrName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOOptionLists OptDetails = dbContext.BOOptionLists.Find(iID);
            dbContext.BOOptionLists.Remove(OptDetails);
            dbContext.SaveChanges();
            return 1;
        }

        //save BO attributes form
        //public int SaveFormBOAttributes(List<BOFields> model)
        //{
        //    BOFields updateField = new BOFields();
        //    foreach (var items in model)
        //    {
        //        int ID = dbContext.BOFields.Where(m => m.BOID == items.BOID).Where(m => m.Name == items.Name).Select(m => m.ID).FirstOrDefault();
        //        updateField = dbContext.BOFields.Find(ID);
        //        updateField.LabelName = items.LabelName;
        //        updateField.Format = items.Format;
        //        updateField.FKTableName = items.FKTableName;
        //        updateField.Description = items.Description;
        //        updateField.IsVisible = items.IsVisible;
        //        updateField.IsWhere = items.IsWhere;
        //        if (updateField.IsWhere == false)
        //        {
        //            updateField.IsRunTime = false;
        //            updateField.IsDate = false;
        //            updateField.DateExpression = null;
        //            updateField.DateValue = null;
        //            updateField.IsWhereExpression = false;
        //            updateField.WhereExpression = null;
        //            updateField.WhereExpreValue = null;
        //            updateField.IsDBValue = false;
        //            updateField.DBQuery = null;
        //        }
        //        updateField.IsTotal = items.IsTotal;
        //        updateField.IsGroupBy = items.IsGroupBy;
        //        updateField.IsOrderBy = items.IsOrderBy;
        //        updateField.IsExpression = items.IsExpression;
        //        if (updateField.IsExpression == false)
        //        {
        //            updateField.ExpressionText = null;
        //            updateField.ExpressionValue = null;
        //        }
        //        updateField.IsMail = items.IsMail;
        //        updateField.IsOptionList = items.IsOptionList;
        //        updateField.DefaultValue = items.DefaultValue;
        //        dbContext.SaveChanges();
        //    }
        //    return 1;
        //}

        public VMCustomResponse SaveFormBOAttributes(BOFields model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOFields updateField = new BOFields();
            int iTypeID = 0;
            int ID = dbContext.BOFields.Where(m => m.BOID == model.BOID).Where(m => m.Name == model.Name).Select(m => m.ID).FirstOrDefault();
            updateField = dbContext.BOFields.Find(ID);
            //check for datatype 
            if (model.Name.Substring(0, 2) != "CM")
            {
                iTypeID = dbContext.BOFields.Where(m => m.ID == ID).Select(m => m.TypeID).FirstOrDefault();
                BODatatypes DataType = (BODatatypes)iTypeID;
                string sOldDataType = DataType.ToString();

                string sOldMaxLength = dbContext.BOFields.Where(m => m.BOID == model.BOID).Where(m => m.Name == model.Name).Select(m => m.MaxLength).FirstOrDefault();
                //Check if Old datatype is primary key in the table where it belongs.
                string sTableName = dbContext.BOs.Where(m => m.BOID == model.BOID).Select(m => m.Name).FirstOrDefault();
                using (SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                {
                    string sCheckPrimaryKey = @"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY' AND K.COLUMN_NAME = '" + model.Name + "' AND K.TABLE_NAME = '" + sTableName + "') SELECT 0 ELSE SELECT 1";
                    Conn.Open();
                    Conn.ChangeDatabase(sDatabase);
                    SqlCommand SqlcmdCheck = new SqlCommand(sCheckPrimaryKey, Conn);
                    int iIsPrimary = Convert.ToInt32(SqlcmdCheck.ExecuteScalar());
                    Conn.Close();
                    if (iIsPrimary == 1)
                    {
                        sOldDataType = sOldDataType + " IDENTITY(1, 1) PRIMARY KEY";
                    }
                    else
                    {
                        if (sOldMaxLength != null)
                        {
                            sOldDataType = sOldDataType + "(" + sOldMaxLength + ")";
                        }
                    }

                    //check if the new datatype has "(" to check max length.
                    var sNewDataType = model.DataType;
                    int DatatypeChanged = 0;
                    if (sOldDataType.ToUpper() != sNewDataType.ToUpper())
                    {
                        DatatypeChanged = CheckDatatypeConvertion(sTableName, sOldDataType, sNewDataType, ID, model.Name, iUserID, sOrgName, sDatabase);
                        if (DatatypeChanged == 0)
                        {
                            //sDTypeNotChngd.Add(sColumn);
                        }

                        //    }
                        //    if (sDTypeNotChngd.Count == 0)
                        //    {
                        //        ReturnStatus = ServiceConstants.DataTypeChangeSuccess;
                        //        bStatus = true;
                        //    }
                        //    else
                        //    {
                        //        ReturnStatus = ServiceConstants.DataTypeChangeError;
                        //        bStatus = false;
                        //    }
                        //}
                        Conn.Close();
                    }
                }
            }
            else
            {
                //if Computed volumn do nothing
            }
            if (updateField == null)
            {
                updateField = new BOFields();
                updateField.BOID = model.BOID;
                updateField.Name = model.Name;
                updateField.MaxLength = "0";
                updateField.CreatedTime = DateTime.Now;
                updateField.CreatedByName = model.CreatedByName;
                updateField.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                updateField.CreatedByID = model.CreatedByID;
                updateField.iAttributeType = model.iAttributeType;
                if (iTypeID == 0)
                {
                    updateField.TypeID = (int)Enum.Parse(typeof(BODatatypes), model.DataType.ToUpper());
                }
            }
            updateField.LabelName = model.LabelName;
            updateField.Format = model.Format;
            updateField.FKTableName = model.FKTableName;
            updateField.Description = model.Description;
            updateField.FKiType = model.FKiType;
            if (model.FKiType == 20)
            {
                updateField.iMasterDataID = model.iMasterDataID;
            }
            else
            {
                updateField.iMasterDataID = 0;
            }
            if (model.FKiType == 40)
            {
                updateField.iOneClickID = dbContext.Reports.Where(m => m.Name == model.sOneClickName).Select(m => m.ID).FirstOrDefault();
            }
            else
            {
                updateField.iOneClickID = 0;
            }
            updateField.IsVisible = model.IsVisible;
            updateField.IsTextArea = model.IsTextArea;
            updateField.IsWhere = model.IsWhere;
            if (updateField.IsWhere == false)
            {
                updateField.IsRunTime = false;
                updateField.IsDate = false;
                updateField.DateExpression = null;
                updateField.DateValue = null;
                updateField.IsWhereExpression = false;
                updateField.WhereExpression = null;
                updateField.WhereExpreValue = null;
                updateField.IsDBValue = false;
                updateField.DBQuery = null;
            }
            updateField.IsTotal = model.IsTotal;
            updateField.IsGroupBy = model.IsGroupBy;
            updateField.IsOrderBy = model.IsOrderBy;
            updateField.IsExpression = model.IsExpression;
            if (updateField.IsExpression == false)
            {
                updateField.ExpressionText = null;
                updateField.ExpressionValue = null;
            }
            updateField.IsMail = model.IsMail;
            updateField.IsOptionList = model.IsOptionList;
            if (model.IsOptionList == false)
            {
                var ExistingUnCheckBoOptionList = dbContext.BOOptionLists.Where(m => m.BOID == model.BOID).Where(m => m.BOFieldID == model.ID).Where(m => m.Name == model.Name).ToList();
                dbContext.BOOptionLists.RemoveRange(ExistingUnCheckBoOptionList);
                dbContext.SaveChanges();
            }
            updateField.DefaultValue = model.DefaultValue;
            updateField.Script = model.Script;
            updateField.sPlaceHolder = model.sPlaceHolder;
            updateField.sHelpText = model.sHelpText;
            updateField.FKiFileTypeID = model.FKiFileTypeID;
            updateField.bKPI = model.bKPI;
            updateField.iSystemType = model.iSystemType;
            if (updateField.ID == 0)
            {
                dbContext.BOFields.Add(updateField);
                dbContext.SaveChanges();
            }
            else
            {
                dbContext.SaveChanges();
            }

            return new VMCustomResponse { Status = true, ID = updateField.ID, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public int CheckDatatypeConvertion(string sTableName, string sOld_Datatype, string sNew_DataType, int iColID, string sColumnName, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            int iDatatypeStatus = 0;
            int iTableDtypChange = 0;
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            SqlConnection ConnA = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString);
            //remove empty
            string sOld_Type = "";
            string sOld_Length = "";
            string sNew_Type = "";
            string sNew_Length = "";
            if (sOld_Datatype.Contains("()"))
            {
                sOld_Datatype = sOld_Datatype.Replace("()", "");
            }
            else if (sNew_DataType.Contains("()"))
            {
                sNew_DataType = sNew_DataType.Replace("()", "");
            }

            //seperate length and type
            if (sOld_Datatype.Contains("PRIMARY"))
            {
                iDatatypeStatus = 0;
            }
            else
            {
                if (sOld_Datatype.Contains("("))
                {
                    string[] sSplitOld = sOld_Datatype.Split('(');
                    sOld_Type = sSplitOld[0];
                    sOld_Length = sSplitOld[1].Replace(")", "");
                }

                if (sNew_DataType.Contains("("))
                {
                    string[] sSplitOld = sNew_DataType.Split('('); ;
                    sNew_Type = sSplitOld[0];
                    sNew_Length = sSplitOld[1].Replace(")", "");
                }
            }
            string sQuery = "ALTER TABLE [" + sTableName + "] ";

            if (sOld_Type != sNew_Type)
            {

                if (sOld_Type == "INT" && (sNew_Type == "INT" || sNew_Type == "FLOAT"))
                {
                    iDatatypeStatus = 1;
                }
                else if (sOld_Type == "VARCHAR" && (sNew_Type == "VARCHAR" || sNew_Type == "NVARCHAR"))
                {
                    iDatatypeStatus = 1;
                }
                else if (sOld_Type.Contains("DATETIME"))
                {
                    iDatatypeStatus = 0;
                }
            }

            if (iDatatypeStatus == 1)
            {
                //new datatype can be used
                if (sNew_Length == "" && (sNew_Type != "VARCHAR" || sNew_Type != "NVARCHAR"))
                {
                    sQuery = sQuery + "ALTER COLUMN " + sColumnName + " " + sNew_Type;
                }
                else
                {
                    sQuery = sQuery + "ALTER COLUMN " + sColumnName + " " + sNew_Type + "(" + sNew_Length + ")";
                }
                Conn.Open();
                SqlCommand cmd = new SqlCommand(sQuery, Conn);
                try
                {
                    Conn.ChangeDatabase(sOrgDb);
                    cmd.ExecuteNonQuery();
                    iTableDtypChange = 1;
                    Conn.Close();
                }
                catch (Exception ex)
                {
                    //error
                }
                if (iTableDtypChange == 1)
                {
                    //get the type ID for the datatype.
                    int TypeID = (int)Enum.Parse(typeof(BODatatypes), sNew_Type);

                    if (sNew_Type == "")
                    {
                        sNew_Type = "0";
                    }

                    string sFieldsQuery = "UPDATE [BOFields] SET TypeID=" + TypeID + ",MaxLength=" + sNew_Length + " WHERE ID=" + iColID;
                    ConnA.Open();
                    ConnA.ChangeDatabase(sDatabase);
                    SqlCommand cmd1 = new SqlCommand(sFieldsQuery, ConnA);
                    cmd1.ExecuteNonQuery();
                    ConnA.Close();
                }
            }
            else
            {
                //retain the old datatype

            }
            return iDatatypeStatus;

        }
        //Save BO option list
        public int SaveBoOptionList(string Values, int BOID, int iID, string AtrName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOOptionLists Objects = new BOOptionLists();
            var BOFieldID = dbContext.BOFields.Where(m => m.Name == AtrName && m.BOID == BOID).Select(m => m.ID).FirstOrDefault();
            var lOptionNames = new List<string>();
            var lOptionValues = new List<string>();
            var lOptionStatus = new List<string>();
            var lOptionCode = new List<string>();
            string[] sValues = Regex.Split(Values, ",");
            int status = 0;
            foreach (var Val in sValues)
            {
                string[] sDetails = Regex.Split(Val, ":");
                string sOptionName = sDetails[0];
                string sOptionValue = sDetails[1];
                string sOptionCode = sDetails[2];
                string sOptionStatus = sDetails[3];
                lOptionNames.Add(sOptionName);
                lOptionValues.Add(sOptionValue);
                lOptionStatus.Add(sOptionStatus);
                lOptionCode.Add(sOptionCode);
            }
            if (lOptionNames.Count() > 0)
            {
                if (iID == 0)
                {
                    for (int i = 0; i < lOptionNames.Count(); i++)
                    {
                        Objects.BOID = BOID;
                        Objects.BOFieldID = BOFieldID;
                        Objects.Name = AtrName;
                        Objects.sOptionName = lOptionNames[i];
                        Objects.sValues = lOptionValues[i];
                        Objects.StatusTypeID = Convert.ToInt32(lOptionStatus[i]);
                        Objects.sOptionCode = lOptionCode[i];
                        dbContext.BOOptionLists.Add(Objects);
                        dbContext.SaveChanges();
                        status = 1;
                    }
                }
                else
                {
                    var ExistingOptionValues = dbContext.BOOptionLists.Where(m => m.BOID == BOID).Where(m => m.Name == AtrName).ToList();
                    dbContext.BOOptionLists.RemoveRange(ExistingOptionValues);
                    dbContext.SaveChanges();
                    for (int j = 0; j < lOptionNames.Count(); j++)
                    {
                        Objects.BOID = BOID;
                        Objects.BOFieldID = BOFieldID;
                        Objects.Name = AtrName;
                        Objects.sOptionName = lOptionNames[j];
                        Objects.sValues = lOptionValues[j];
                        Objects.StatusTypeID = Convert.ToInt32(lOptionStatus[j]);
                        Objects.sOptionCode = lOptionCode[j];
                        dbContext.BOOptionLists.Add(Objects);
                        dbContext.SaveChanges();
                        status = 1;
                    }
                }
            }
            return status;
        }

        //Delete attribute from BO
        public int DeleteAttribute(int BOID, string AttrName, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            ModelDbContext dbContext = new ModelDbContext();
            int iDeletedCol = 1;
            int iColCount = 0;
            string BOName = dbContext.BOs.Where(m => m.BOID == BOID).Select(m => m.Name).FirstOrDefault();
            //check the number of columns as the table cannot exist with no columns
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            using (SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
            {
                Conn.Open();
                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = Conn;
                Conn.ChangeDatabase(sOrgDb);
                SqlCmd.CommandText = "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME= '" + BOName + "'";
                SqlDataReader DReader = SqlCmd.ExecuteReader();
                while (DReader.Read())
                {
                    iColCount = DReader.GetInt32(0);
                }
                Conn.Close();
                if (iColCount > 1)
                {
                    string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + BOName + "') SELECT 1 ELSE SELECT 0";
                    Conn.Open();
                    SqlCommand DateCheck = new SqlCommand(cmdText, Conn);
                    int x = Convert.ToInt32(DateCheck.ExecuteScalar());
                    var lColDatatypes = new List<List<string>>();
                    Conn.Close();
                    if (x == 1)
                    {
                        string sDeleteCol = "ALTER TABLE dbo." + BOName + " DROP COLUMN " + AttrName;
                        try
                        {
                            Conn.Open();
                            Conn.ChangeDatabase(sOrgDb);
                            SqlCommand cmd = new SqlCommand(sDeleteCol, Conn);
                            cmd.ExecuteNonQuery();
                            Conn.Close();
                        }
                        catch (Exception ex)
                        {
                            iDeletedCol = 0;
                        }
                    }
                    else
                    {
                        //do nothing
                    }
                }
                else
                {
                    string sDeleteCol = "DROP TABLE " + BOName;
                    try
                    {
                        Conn.Open();
                        Conn.ChangeDatabase(sOrgDb);
                        SqlCommand cmd = new SqlCommand(sDeleteCol, Conn);
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    catch (Exception ex)
                    {
                        iDeletedCol = 0;
                    }
                    if (iDeletedCol == 1)
                    {
                        BOs model = dbContext.BOs.Find(BOID);
                        dbContext.BOs.Remove(model);
                        dbContext.SaveChanges();
                    }
                }
            }
            return iDeletedCol;
        }

        //Get the Attributes name related to BO
        public BOs CopyBOByID(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string sCol_Name = "";
            string Data_Type = "";
            string sMax_Length = "";
            string sIS_NULL = "";
            string Col_Details = "";
            var lCmptCol = new List<string>();
            var sColDetails = new List<string>();
            var oBO = dbContext.BOs.Find(ID);
            string BOName = oBO.Name;
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var sOrgDB = UserDetails.sUserDatabase;
            var sBODataSource = oXIAPI.GetBODataSource(oBO.iDataSource, UserDetails.FKiOrgID, sDatabase, sOrgDB);
            var sNewCol_Details = "";
            using (SqlConnection Con = new SqlConnection(sBODataSource))
            {
                Con.Open();

                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = Con;
                SqlCmd.CommandText = "SELECT COLUMN_NAME,DATA_TYPE,CHARACTER_MAXIMUM_LENGTH,IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME= '" + oBO.TableName + "'";
                SqlDataReader DReader = SqlCmd.ExecuteReader();
                while (DReader.Read())
                {
                    sCol_Name = DReader.IsDBNull(0) ? null : DReader.GetValue(0).ToString();
                    Data_Type = DReader.IsDBNull(1) ? null : DReader.GetValue(1).ToString();
                    sMax_Length = DReader.IsDBNull(2) ? null : DReader.GetValue(2).ToString();
                    sIS_NULL = DReader.IsDBNull(3) ? null : DReader.GetValue(3).ToString();

                    string sTwoLtrOfColumn = sCol_Name.Substring(0, 2);
                    string sNewDatatype = "";
                    string sNewColDetails = "";
                    if (sTwoLtrOfColumn == "CM")
                    {
                        string sGetCmptdColDef = "SELECT definition FROM sys.computed_columns WHERE [name] = '" + sCol_Name + "' AND [object_id] = OBJECT_ID('" + oBO.TableName + "')";
                        //Conn.Open();
                        SqlCommand SqlcmdColDef = new SqlCommand(sGetCmptdColDef, Con);
                        SqlDataReader DReaderChckDtype = SqlcmdColDef.ExecuteReader();
                        var sDefinition = "";
                        while (DReaderChckDtype.Read())
                        {
                            sDefinition = DReaderChckDtype.IsDBNull(0) ? null : DReaderChckDtype.GetValue(0).ToString();
                        }
                        //Conn.Close();
                        int iDefnLength = sDefinition.Length;
                        var sNewDefinition = sDefinition.Substring(1, iDefnLength - 2);
                        string sGetColumnUsed = String.Join(",", Regex.Matches(sNewDefinition, @"\[(.+?)\]").Cast<Match>().Select(m => m.Groups[1].Value));
                        var lColumnList = new List<string>();
                        var lColumnExists = new List<string>();
                        lColumnList = sGetColumnUsed.Split(',').ToList();
                        string sColumns = "";
                        foreach (var ColNmes in lColumnList)
                        {
                            string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='" + sDatabase + "' AND TABLE_NAME='" + oBO.TableName + "' AND COLUMN_NAME ='" + ColNmes + "') SELECT 0 ELSE SELECT 1";
                            // Conn.Open();
                            SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Con);
                            int iColExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            // Conn.Close();
                            if (iColExist == 1)
                            {
                                lColumnExists.Add(ColNmes);
                            }
                            sColumns = string.Join(",", lColumnExists.ToArray());
                        }
                        //Col_Details =Col_Details+ sCol_Name+",{"+ sColumns + "} AS " + sNewDefinition + "\n";
                        //take as a sperate string.
                        lCmptCol.Add(sCol_Name + ",{" + sColumns + "} AS " + sNewDefinition + "\n");
                    }
                    else if (sTwoLtrOfColumn == "FK")
                    {
                        var PK_Table = "";
                        var PK_ColName = "";
                        //get the details on foreign key here add extra PKcolumn name and table name 
                        //SqlConnection TempConn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);

                        SqlCommand SqlCmdTemp = new SqlCommand();
                        SqlCmdTemp.Connection = Con;
                        SqlCmdTemp.CommandText = "SELECT FK_Table = FK.TABLE_NAME,FK_Column = CU.COLUMN_NAME, PK_Table = PK.TABLE_NAME, PK_Column = PT.COLUMN_NAME, Constraint_Name = C.CONSTRAINT_NAME FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME INNER JOIN (SELECT i1.TABLE_NAME, i2.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY') PT ON PT.TABLE_NAME = PK.TABLE_NAME WHERE FK.TABLE_NAME= '" + oBO.TableName + "' AND CU.COLUMN_Name = '" + sCol_Name + "'";
                        SqlDataReader DReaderTemp = SqlCmdTemp.ExecuteReader();
                        while (DReaderTemp.Read())
                        {
                            PK_Table = DReaderTemp.IsDBNull(2) ? null : DReaderTemp.GetValue(2).ToString();
                            PK_ColName = DReaderTemp.IsDBNull(3) ? null : DReaderTemp.GetValue(3).ToString();
                        }
                        string NewNullVal = "NOT NULL";
                        if (sIS_NULL == "NO" || sIS_NULL == "no")
                        {
                            NewNullVal = "NOT NULL";
                        }
                        else
                        {
                            NewNullVal = "NULL";
                        }
                        if (PK_Table == "" && PK_ColName == "")
                        {
                            if (sMax_Length == "NULL")
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            }
                            else
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                            }
                        }
                        else
                        {
                            if (sMax_Length == "NULL")
                            {
                                // sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal + "," + PK_Table + "," + PK_ColName;
                                sNewColDetails = sCol_Name + "," + PK_Table + "," + PK_ColName;
                            }
                            else
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal + "," + PK_Table + "," + PK_ColName;
                                sNewColDetails = sCol_Name + "," + PK_Table + "," + PK_ColName;
                            }
                        }
                        sColDetails.Add(sNewColDetails);
                        if (Col_Details == "")
                        {
                            Col_Details = sNewColDetails + "\n";
                        }
                        else
                        {
                            Col_Details = Col_Details + sNewColDetails + "\n";
                        }
                    }
                    else
                    {
                        if (Data_Type == "int")
                        {
                            sNewDatatype = "i";

                        }
                        else if (Data_Type == "varchar")
                        {
                            sNewDatatype = "s";
                        }
                        else if (Data_Type == "nvarchar")
                        {
                            sNewDatatype = "n";
                        }
                        else if (Data_Type == "datetime")
                        {
                            sNewDatatype = "d";
                        }
                        else if (Data_Type == "float")
                        {
                            sNewDatatype = "r";
                        }

                        //string NewMaxLen = "0";
                        //if(sMax_Length=="NULL"&&sMax_Length=="null")
                        //{
                        //    NewMaxLen = "0";
                        //}
                        //else
                        //{
                        //    NewMaxLen = sMax_Length;
                        //}

                        //Is Null
                        string NewNullVal = "NOT NULL";
                        if (sIS_NULL == "NO" || sIS_NULL == "no")
                        {
                            NewNullVal = "NOT NULL";
                        }
                        else
                        {
                            NewNullVal = "NULL";
                        }
                        string sStartCharacter = sCol_Name.Substring(0, 1);
                        if (sCol_Name == "ID")
                        {
                            //sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            sNewColDetails = sCol_Name;
                        }
                        else if (sStartCharacter == sNewDatatype)
                        {
                            if (sMax_Length == "NULL")
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                                sNewColDetails = sCol_Name;
                            }
                            else
                            {
                                //sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                                sNewColDetails = sCol_Name;
                            }
                        }
                        else
                        {
                            //consider the column name directly
                            if (sMax_Length == "NULL")
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "," + NewNullVal;
                            }
                            else
                            {
                                sNewColDetails = sCol_Name + "," + Data_Type + "(" + sMax_Length + ")," + NewNullVal;
                            }
                            //sNewColDetails = sNewDatatype + sCol_Name + "_" + Data_Type + "_" + NewMaxLen + "_" + NewNullVal;
                        }
                        //  sNewColDetails = sNewDatatype + sCol_Name + "_" + Data_Type + "_" + sIS_NULL;
                        sColDetails.Add(sNewColDetails);
                        if (Col_Details == "")
                        {
                            Col_Details = sNewColDetails + "\n";
                        }
                        else
                        {
                            Col_Details = Col_Details + sNewColDetails + "\n";
                        }
                    }
                }
                var sRemoveNewLineAtEnd = Col_Details.TrimEnd('\n');
                var sRemoveEmptyParenthesis = sRemoveNewLineAtEnd.Replace("()", "");

                string sNewCmpCol = string.Join(",", lCmptCol.ToArray());
                string sCmptdCols = sNewCmpCol.Replace("\n,", "\n");
                string sFnlCmptdCol = sCmptdCols.TrimEnd('\n');
                if (sFnlCmptdCol != "")
                {
                    sNewCol_Details = sRemoveEmptyParenthesis + "\n" + sFnlCmptdCol;
                }
                else
                {
                    sNewCol_Details = sRemoveEmptyParenthesis;
                }

                Con.Close();
            }
            var BO = dbContext.BOs.Find(ID);
            // BO.ColumnDetails = sColDetails;
            BO.ColName = sNewCol_Details;
            return BO;
        }

        //create a new table and BO.
        //public VMCustomResponse CreateTableFromBO(BOs model, int CreatedByID, string CreatedByName, int iUserID, string sOrgName, string sDatabase)
        //{
        //    var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
        //    int SaveBOID = 0;
        //    bool bStatus = true;
        //    int iTableCreated = 1;
        //    int iTableUpdated = 1;
        //    // int DatatypeChanged = 0;
        //    string ReturnStatus = "";
        //    var sDTypeNotChngd = new List<string>();
        //    //Check if the table
        //    //SqlConnection SC = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
        //    SqlConnection SC = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
        //    string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + model.Name + "') SELECT 1 ELSE SELECT 0";
        //    SC.Open();
        //    SC.ChangeDatabase(sOrgDb);
        //    SqlCommand DateCheck = new SqlCommand(cmdText, SC);
        //    int x = Convert.ToInt32(DateCheck.ExecuteScalar());
        //    var lColDatatypes = new List<List<string>>();
        //    SC.Close();
        //    //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
        //    SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
        //    var lColList = new List<string>();
        //    string sSplitVal1 = "";
        //    string sSplitVal2 = "";
        //    string sSplitVal3 = "";
        //    string[] sColDetails = Regex.Split(model.ColName, "\r\n");
        //    foreach (string ColumnDetails in sColDetails)
        //    {
        //        var lColDatatypeList = new List<string>();
        //        //Check Computed Columns
        //        if (ColumnDetails.Contains(" AS ") || ColumnDetails.StartsWith(" CM "))
        //        {
        //            string[] sSplitColDetails = Regex.Split(ColumnDetails, " AS ");
        //            var sColNameDeatils = sSplitColDetails[0];
        //            var sColComputedFrml = sSplitColDetails[1];
        //            //the first part consists of column name, datatype and column name used in calculations
        //            var sSplitColmnDetails = Regex.Split(sColNameDeatils, ",");
        //            var sColName = sSplitColmnDetails[0];
        //            // var sColDType = sSplitColmnDetails[1];
        //            var sColUsed = sSplitColmnDetails[1];
        //            //int list to check if the column exists or computed
        //            List<int> liCheckUsedCol = new List<int>();
        //            //Remove braces as the format is "{column1.column2}
        //            var sColUsedRmv1 = sColUsed.Replace("{", "");
        //            var sColUsedRmv2 = sColUsedRmv1.Replace("}", "");
        //            if (sColUsedRmv2.Contains("."))
        //            {
        //                var sCSVColUsed = sColUsedRmv2.Replace("_", ",");
        //                List<string> lColUsed = sCSVColUsed.Split(',').ToList();

        //                for (var i = 0; i < lColUsed.Count(); i++)
        //                {
        //                    //check if column exists                           
        //                    string sCheckColComputd = @"IF NOT EXISTS(USE " + sOrgDb + " SELECT * FROM sys.computed_columns WHERE is_computed = 1 and name='" + lColUsed[i] + "'and object_id=OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";
        //                    Conn.Open();
        //                    Conn.ChangeDatabase(sOrgDb);
        //                    SqlCommand SqlcmdComp = new SqlCommand(sCheckColComputd, Conn);
        //                    int iColComptdExist = Convert.ToInt32(SqlcmdComp.ExecuteScalar());
        //                    Conn.Close();
        //                    if (iColComptdExist == 1)
        //                    {
        //                        liCheckUsedCol.Add(iColComptdExist);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                string sCheckColComputd = @"IF NOT EXISTS(SELECT * FROM sys.computed_columns WHERE is_computed = 1 and name='" + sColUsedRmv2 + "' and object_id=OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";
        //                Conn.Open();
        //                Conn.ChangeDatabase(sOrgDb);
        //                SqlCommand SqlcmdComp = new SqlCommand(sCheckColComputd, Conn);
        //                int iColComptdExist = Convert.ToInt32(SqlcmdComp.ExecuteScalar());
        //                Conn.Close();
        //                if (iColComptdExist == 1)
        //                {
        //                    liCheckUsedCol.Add(iColComptdExist);
        //                }
        //                else
        //                {
        //                    //if no donot add to list as the Computed column can be created.
        //                }
        //                //}
        //            }

        //            if (liCheckUsedCol.Count == 0)
        //            {
        //                lColDatatypeList.Add("COMPUTED COLUMN");
        //                lColDatatypeList.Add(sColName);
        //                lColDatatypeList.Add("AS " + sColComputedFrml);
        //                //lColDatatypes.Add(lColDatatypeList);
        //            }
        //            else
        //            {
        //                //donot create a computed column
        //                bStatus = false;
        //                ReturnStatus = ServiceConstants.ErrorMessageComptdColumn;
        //            }
        //        }
        //        else
        //        {
        //            //check if the foriegn key has references added.

        //            int iDetailsCount = Regex.Matches(ColumnDetails, ",").Count;
        //            if (iDetailsCount == 0)
        //            {
        //                var Char = ColumnDetails.Select(c => char.IsUpper(c)).ToList();
        //                var Position = Char.IndexOf(true);
        //                if (Position == 0)
        //                {
        //                    if (ColumnDetails == "ID")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("int IDENTITY(1,1) PRIMARY KEY");

        //                    }
        //                }
        //                else if (Position == 1)
        //                {
        //                    string sFirstLetter = ColumnDetails.Substring(0, 1);
        //                    if (sFirstLetter == "i")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("int NULL");
        //                    }
        //                    else if (sFirstLetter == "s")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("varchar(100) NULL");
        //                    }
        //                    else if (sFirstLetter == "d")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("datetime NULL");
        //                    }
        //                    else if (sFirstLetter == "r")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("float NULL");
        //                    }
        //                    else if (sFirstLetter == "n")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("nvarchar(MAX) NULL");
        //                    }
        //                }
        //                else if (Position == 2)
        //                {
        //                    string sFrstTwoLetter = ColumnDetails.Substring(0, 2);
        //                    if (sFrstTwoLetter == "dt")
        //                    {
        //                        lColDatatypeList.Add(ColumnDetails);
        //                        lColDatatypeList.Add("datetime NULL");
        //                    }
        //                }
        //                // lColDatatypes.Add(lColDatatypeList);
        //            }
        //            else if (iDetailsCount == 2)
        //            {
        //                //contains foreign key details ex "FKiOrgID,Orgnaization,ID
        //                string[] sSplitColDetails = Regex.Split(ColumnDetails, ",");
        //                var sColName = sSplitColDetails[0];
        //                var sPK_Table = sSplitColDetails[1];
        //                var sPK_Column = sSplitColDetails[2];

        //                string sFirstTwoLetters = sColName.Substring(0, 2);
        //                if (sFirstTwoLetters == "FK")
        //                {
        //                    string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='" + sDatabase + "' AND TABLE_NAME='" + sPK_Table + "' AND COLUMN_NAME ='" + sPK_Column + "') SELECT 0 ELSE SELECT 1";
        //                    Conn.Open();
        //                    Conn.ChangeDatabase(sOrgDb);
        //                    SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Conn);
        //                    int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
        //                    Conn.Close();
        //                    if (iExist == 1)
        //                    {
        //                        string ThirdLetter = sColName.Substring(2, 1);
        //                        if (ThirdLetter == "i")
        //                        {
        //                            lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
        //                            lColDatatypeList.Add("int_NULL");
        //                        }
        //                        else if (ThirdLetter == "s")
        //                        {
        //                            lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
        //                            lColDatatypeList.Add("varchar(64)_NULL");

        //                        }
        //                        else if (ThirdLetter == "d")
        //                        {
        //                            lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
        //                            lColDatatypeList.Add("datetime_NULL");
        //                        }
        //                        else if (ThirdLetter == "r")
        //                        {
        //                            lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
        //                            lColDatatypeList.Add("float_NULL");
        //                        }
        //                        else if (ThirdLetter == "n")
        //                        {
        //                            lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
        //                            lColDatatypeList.Add("nvarchar(MAX)_NULL");
        //                        }
        //                    }
        //                    else
        //                    {
        //                        //add normal column
        //                    }
        //                }
        //                else
        //                {
        //                    //Not foriegn key
        //                }
        //                //lColDatatypes.Add(lColDatatypeList);
        //            }
        //            else
        //            {
        //                //format is not supported.
        //            }
        //            lColDatatypes.Add(lColDatatypeList);
        //        }  //end of check
        //    }
        //    if (x == 0)
        //    //Table exixts
        //    {

        //        string sQuery = "create table [" + model.Name + "] (";
        //        for (var i = 0; i < lColDatatypes.Count(); i++)
        //        {
        //            //check if has computed column
        //            if (lColDatatypes[i][0] == "COMPUTED COLUMN")
        //            {
        //                if (sQuery.Contains(",,") == false)
        //                {
        //                    sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
        //                }
        //                else
        //                {
        //                    sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
        //                }
        //            }
        //            else if (lColDatatypes[i].Count == 2)
        //            {
        //                var sColumn = lColDatatypes[i][0];
        //                var sDataTyp = lColDatatypes[i][1];
        //                string sFirstTwoLtr = sColumn.Substring(0, 2);
        //                if (sFirstTwoLtr != "FK")
        //                {
        //                    if (sQuery.Contains(",,") == false)
        //                    {
        //                        if (sDataTyp.Contains("PRIMARY"))
        //                        {
        //                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
        //                        }
        //                        else
        //                        {
        //                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
        //                        }

        //                    }
        //                    else
        //                    {
        //                        if (sDataTyp.Contains("PRIMARY"))
        //                        {
        //                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
        //                        }
        //                        else
        //                        {
        //                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
        //                        }
        //                    }
        //                }//check FK
        //                else
        //                {
        //                    if (sColumn.Contains(","))
        //                    {
        //                        string[] sSplitVal = Regex.Split(sColumn, ",");
        //                        sSplitVal1 = sSplitVal[0];
        //                        sSplitVal2 = sSplitVal[1];
        //                        sSplitVal3 = sSplitVal[2];
        //                        if (sQuery.Contains(",,") == false)
        //                        {
        //                            //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
        //                            if (sSplitVal2 == "OrganizationClasses")
        //                            {
        //                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

        //                            }
        //                            else
        //                            {
        //                                string[] sSplitDtype = Regex.Split(sDataTyp, "_");
        //                                string sColmnDtype = sSplitDtype[0];
        //                                string sColmnNull = sSplitDtype[1];
        //                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (sSplitVal2 == "OrganizationClasses")
        //                            {
        //                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
        //                            }
        //                            else
        //                            {
        //                                //if (sDataTyp.Contains("_"))
        //                                //{
        //                                string[] sSplitDtype = Regex.Split(sDataTyp, "_");
        //                                string sColmnDtype = sSplitDtype[0];
        //                                string sColmnNull = sSplitDtype[1];
        //                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
        //                                //}
        //                            }
        //                        }

        //                    }
        //                    else // if column has no "," then use the column and datatype directly as no primary key or table
        //                    {
        //                        if (sQuery.Contains(",,") == false)
        //                        {
        //                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
        //                        }
        //                        else
        //                        {
        //                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
        //                        }
        //                    }


        //                }

        //            }
        //        }
        //        bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
        //        if (bStrTwoCm == true)
        //        {
        //            int position = sQuery.LastIndexOf(",,");
        //            sQuery = sQuery.Remove(position);
        //        }
        //        bool bStrCm = sQuery.Trim().EndsWith(@",");
        //        if (bStrCm == true)
        //        {
        //            int position = sQuery.LastIndexOf(',');
        //            sQuery = sQuery.Remove(position);
        //        }
        //        sQuery = sQuery + ")";
        //        if (sQuery.EndsWith("()"))
        //        {

        //        }
        //        else
        //        {
        //            Conn.Open();
        //            Conn.ChangeDatabase(sOrgDb);
        //            string sStructuredQryReplace = sQuery.Replace(",,,,", ",");
        //            string sStructuredQry = sStructuredQryReplace.Replace(",,", ",");
        //            SqlCommand cmd = new SqlCommand(sStructuredQry, Conn);
        //            try
        //            {
        //                cmd.ExecuteNonQuery();
        //                ReturnStatus = ServiceConstants.SuccessMessage;
        //            }
        //            catch (Exception Ex)
        //            {
        //                iTableCreated = 0;
        //                bStatus = false;
        //                ReturnStatus = ServiceConstants.ErrorMessage;
        //            }
        //            Conn.Close();
        //        }
        //        //Create BO for the table-----
        //        if (iTableCreated == 1)
        //        {
        //            model.CreatedBy = CreatedByID;
        //            SaveBOID = SaveBO(model, iUserID, sOrgName, sDatabase);
        //        }
        //    }
        //    else
        //    {
        //        string sQuery = "alter table [" + model.Name + "] add ";
        //        for (var i = 0; i < lColDatatypes.Count(); i++)
        //        {
        //            //check if has computed column
        //            if (lColDatatypes[i][0] == "COMPUTED COLUMN")
        //            {
        //                string sCheckComputdColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')) SELECT 0 ELSE SELECT 1";
        //                Conn.Open();
        //                Conn.ChangeDatabase(sOrgDb);
        //                SqlCommand Sqlcmd = new SqlCommand(sCheckComputdColmn, Conn);
        //                int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
        //                Conn.Close();
        //                if (iExist == 0)
        //                {
        //                    if (sQuery.Contains(",,") == false)
        //                    {
        //                        sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
        //                    }
        //                    else
        //                    {
        //                        sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
        //                    }
        //                }
        //                else
        //                {
        //                    //Get the defination for the computed column and check with the new definition
        //                    var sOldDefinition = "";
        //                    //Get the computed column definition
        //                    string sGetCmptdColDef = "SELECT definition FROM sys.computed_columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')";
        //                    Conn.Open();
        //                    Conn.ChangeDatabase(sOrgDb);
        //                    SqlCommand SqlcmdColDef = new SqlCommand(sGetCmptdColDef, Conn);
        //                    SqlDataReader DReaderChckDtype = SqlcmdColDef.ExecuteReader();
        //                    while (DReaderChckDtype.Read())
        //                    {
        //                        sOldDefinition = DReaderChckDtype.IsDBNull(0) ? null : DReaderChckDtype.GetValue(0).ToString();
        //                    }
        //                    Conn.Close();
        //                    var sNewDefinition = lColDatatypes[i][2].Replace("AS ", "");
        //                    var sAltrOldDefnEnd = "(" + sOldDefinition + ")";
        //                    //Compare old and new Definition
        //                    if (sAltrOldDefnEnd.ToLower() != sNewDefinition.ToLower())
        //                    {
        //                        //update to new definition but here we need to drop the column and re add the column.
        //                        var sDropComputedColumn = "ALTER TABLE [" + model.Name + "] DROP COLUMN " + lColDatatypes[i][1];
        //                        Conn.Open();
        //                        SqlCommand cmdDrop = new SqlCommand(sDropComputedColumn, Conn);
        //                        try
        //                        {
        //                            Conn.ChangeDatabase(sOrgDb);
        //                            cmdDrop.ExecuteNonQuery();
        //                            Conn.Close();
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }
        //                        var sUpdateComputedColumn = "ALTER TABLE [" + model.Name + "] ADD " + lColDatatypes[i][1] + " AS " + sNewDefinition;
        //                        Conn.Open();
        //                        SqlCommand cmd = new SqlCommand(sUpdateComputedColumn, Conn);
        //                        try
        //                        {
        //                            Conn.ChangeDatabase(sOrgDb);
        //                            cmd.ExecuteNonQuery();
        //                            Conn.Close();
        //                            bStatus = true;
        //                            ReturnStatus = ServiceConstants.SuccessMessageComptdColumn;
        //                        }
        //                        catch (Exception ex)
        //                        {

        //                        }
        //                    }
        //                    else
        //                    {
        //                        // do nothing as both are same
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                var sColumn = lColDatatypes[i][0];
        //                if (sColumn.Contains(','))
        //                {
        //                    string[] sSplitVal = Regex.Split(sColumn, ",");
        //                    sColumn = sSplitVal[0];
        //                }
        //                var sDataTyp = lColDatatypes[i][1];
        //                string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + sColumn + "' AND [object_id] = OBJECT_ID('" + model.Name + "')) SELECT 0 ELSE SELECT 1";
        //                Conn.Open();
        //                Conn.ChangeDatabase(sOrgDb);
        //                SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Conn);
        //                int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
        //                Conn.Close();
        //                if (iExist == 0)
        //                {
        //                    string sFirstTwoLtr = sColumn.Substring(0, 2);
        //                    if (sFirstTwoLtr != "FK")
        //                    {
        //                        if (sQuery.Contains(",,") == false)
        //                        {
        //                            if (sDataTyp.Contains("PRIMARY"))
        //                            {
        //                                sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
        //                            }
        //                            else
        //                            {
        //                                sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
        //                            }

        //                        }
        //                        else
        //                        {
        //                            if (sDataTyp.Contains("PRIMARY"))
        //                            {
        //                                sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
        //                            }
        //                            else
        //                            {
        //                                sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
        //                            }
        //                        }
        //                    }//check FK
        //                    else
        //                    {
        //                        //try if the foriegn key is present patterm Foreign Key, table name
        //                        if (lColDatatypes[i][0].Contains(","))
        //                        {
        //                            string[] sSplitVal = Regex.Split(lColDatatypes[i][0], ",");
        //                            sSplitVal1 = sSplitVal[0];
        //                            sSplitVal2 = sSplitVal[1];
        //                            sSplitVal3 = sSplitVal[2];
        //                            if (sQuery.Contains(",,") == false)
        //                            {
        //                                //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
        //                                if (sSplitVal2 == "OrganizationClasses")
        //                                {
        //                                    //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                    sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

        //                                }
        //                                else
        //                                {
        //                                    string[] sSplitDtype = Regex.Split(sDataTyp, "_");
        //                                    string sColmnDtype = sSplitDtype[0];
        //                                    string sColmnNull = sSplitDtype[1];
        //                                    //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                    sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (sSplitVal2 == "OrganizationClasses")
        //                                {
        //                                    //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                    sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
        //                                }
        //                                else
        //                                {
        //                                    string[] sSplitDtype = Regex.Split(sDataTyp, " ");
        //                                    string sColmnDtype = sSplitDtype[0];
        //                                    string sColmnNull = sSplitDtype[1];
        //                                    //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
        //                                    sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
        //                                }
        //                            }
        //                        }
        //                        else // if column has no "," then use the column and datatype directly as no primary key or table
        //                        {
        //                            if (sQuery.Contains(",,") == false)
        //                            {
        //                                sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
        //                            }
        //                            else
        //                            {
        //                                sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {//no datatype change for now
        //                    ////    var sOld_Datatype = "";
        //                    ////    var sOld_MxLength = "";
        //                    ////    //check is the datatype is same
        //                    ////    string sCheckColmnDatatype = "SELECT DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + model.Name + "' AND COLUMN_NAME = '" + sColumn + "'";
        //                    ////    Conn.Open();
        //                    ////    SqlCommand SqlcmdChckDtype = new SqlCommand(sCheckColmnDatatype, Conn);
        //                    ////    SqlDataReader DReaderCckDtype = SqlcmdChckDtype.ExecuteReader();
        //                    ////    while (DReaderCckDtype.Read())
        //                    ////    {
        //                    ////        sOld_Datatype = DReaderCckDtype.IsDBNull(0) ? null : DReaderCckDtype.GetValue(0).ToString();
        //                    ////        sOld_MxLength = DReaderCckDtype.IsDBNull(1) ? null : DReaderCckDtype.GetValue(1).ToString();
        //                    ////        //Check if Old datatype is primary key.
        //                    ////        string sCheckPrimaryKey = @"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY' AND K.COLUMN_NAME = '" + sColumn + "' AND K.TABLE_NAME = '" + model.Name + "') SELECT 0 ELSE SELECT 1";
        //                    ////        //Conn.Open();
        //                    ////        SqlCommand SqlcmdCheck = new SqlCommand(sCheckPrimaryKey, Conn);
        //                    ////        int iIsPrimary = Convert.ToInt32(SqlcmdCheck.ExecuteScalar());
        //                    ////        //Conn.Close();
        //                    ////        if (iIsPrimary == 1)
        //                    ////        {
        //                    ////            sOld_Datatype = sOld_Datatype + " IDENTITY(1, 1) PRIMARY KEY";
        //                    ////        }
        //                    ////        else
        //                    ////        {
        //                    ////            if (sOld_MxLength != null)
        //                    ////            {
        //                    ////                sOld_Datatype = sOld_Datatype + "(" + sOld_MxLength + ")";
        //                    ////            }
        //                    ////        }


        //                    ////        //check if the new datatype has "(" to check max length.
        //                    ////        var sNew_DataType = "";
        //                    ////        if (sDataTyp.Contains("PRIMARY") == true)
        //                    ////        {
        //                    ////            //string[] sSplitDType = sDataTyp.Split('(');
        //                    ////            //sNew_DataType = sSplitDType[0];
        //                    ////            sNew_DataType = sDataTyp;
        //                    ////        }
        //                    ////        else if (sDataTyp.Contains(" NOT NULL"))
        //                    ////        {
        //                    ////            sNew_DataType = sDataTyp.Replace(" NOT NULL", "");
        //                    ////        }
        //                    ////        else if (sDataTyp.Contains(" NULL"))
        //                    ////        {
        //                    ////            sNew_DataType = sDataTyp.Replace(" NULL", "");
        //                    ////        }
        //                    ////        else
        //                    ////        {
        //                    ////            sNew_DataType = sDataTyp;
        //                    ////        }
        //                    ////        //returns status
        //                    ////        if (sNew_DataType != sOld_Datatype)
        //                    ////        {
        //                    ////            DatatypeChanged = CheckDatatypeConvertion(model.Name, sOld_Datatype, sNew_DataType, sColumn, sDataTyp);
        //                    ////            if (DatatypeChanged == 0)
        //                    ////            {
        //                    ////                sDTypeNotChngd.Add(sColumn);
        //                    ////            }

        //                    ////        }
        //                    ////        if (sDTypeNotChngd.Count == 0)
        //                    ////        {
        //                    ////            ReturnStatus = ServiceConstants.DataTypeChangeSuccess;
        //                    ////            bStatus = true;
        //                    ////        }
        //                    ////        else
        //                    ////        {
        //                    ////            ReturnStatus = ServiceConstants.DataTypeChangeError;
        //                    ////            bStatus = false;
        //                    ////        }
        //                    ////    }
        //                    ////    Conn.Close();
        //                }
        //            }//Not computed column
        //        }
        //        if (sQuery.EndsWith("add "))
        //        {

        //        }
        //        else
        //        {
        //            Conn.Open();
        //            Conn.ChangeDatabase(sOrgDb);
        //            bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
        //            if (bStrTwoCm == true)
        //            {
        //                int position = sQuery.LastIndexOf(",,");
        //                sQuery = sQuery.Remove(position);
        //            }
        //            bool bStrCm = sQuery.Trim().EndsWith(@",");
        //            if (bStrCm == true)
        //            {
        //                int position = sQuery.LastIndexOf(',');
        //                sQuery = sQuery.Remove(position);
        //            }

        //            string sStructuredQryRemove = sQuery.Replace(",,,,", ",");
        //            string sStructuredQry = sStructuredQryRemove.Replace(",,", ",");
        //            SqlCommand cmd = new SqlCommand(sStructuredQry, Conn);
        //            try
        //            {
        //                cmd.ExecuteNonQuery();
        //                ReturnStatus = ServiceConstants.SuccessMessage;
        //                bStatus = true;
        //            }
        //            catch (Exception ex)
        //            {
        //                bStatus = false;
        //                iTableUpdated = 0;
        //                ReturnStatus = ServiceConstants.ErrorMessage;
        //            }
        //            if (iTableUpdated == 1)
        //            {
        //                SaveBOID = SaveBO(model, iUserID, sOrgName, sDatabase);
        //            }
        //            Conn.Close();
        //        }
        //    }
        //    // return the status based on the success;
        //    return new VMCustomResponse { ResponseMessage = ReturnStatus, ID = SaveBOID, Status = bStatus };
        //    //return ReturnStatus;        
        //}


        public VMCustomResponse CreateTableFromBO(BOs model, int OrgID, int CreatedByID, string CreatedByName, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            int SaveBOID = 0;
            bool bStatus = true;
            int iTableCreated = 1;
            int iTableUpdated = 1;
            // int DatatypeChanged = 0;
            string ReturnStatus = "";
            var sDTypeNotChngd = new List<string>();
            //Check if the table
            //SqlConnection SC = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            //SqlConnection SC = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + model.TableName + "') SELECT 1 ELSE SELECT 0";
            //SC.Open();
            //SC.ChangeDatabase(sOrgDb);
            var sBODataSources = oXIAPI.GetBODataSource(model.iDataSource, OrgID, sDatabase, sOrgDb);
            SqlConnection SC = new SqlConnection(sBODataSources);
            SC.Open();
            SqlCommand cmds = new SqlCommand();
            cmds.Connection = SC;
            cmds = new SqlCommand(cmdText, SC);
            SqlCommand DateCheck = new SqlCommand(cmdText, SC);
            int x = Convert.ToInt32(DateCheck.ExecuteScalar());
            var lColDatatypes = new List<List<string>>();
            SC.Close();
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            var lColList = new List<string>();
            string sSplitVal1 = "";
            string sSplitVal2 = "";
            string sSplitVal3 = "";
            //string[] sColDetails = Regex.Split(model.ColName, "\r\n");
            List<string> ColDetails = Regex.Split(model.ColName, "\r\n").ToList();
            List<string> sDefaultColumns = Regex.Split("ID\r\nsName\r\nsDescription\r\nsCode\r\niStatus\r\niType", "\r\n").ToList();
            // List<string> AttrDetails = new List<string>();
            ColDetails = ColDetails.Union(sDefaultColumns).ToList();
            if (ColDetails.Where(m => m.ToLower() == "id").FirstOrDefault() == null)
            {
                ColDetails.Add("ID");
            }
            foreach (string ColumnDetails in ColDetails)
            {
                var lColDatatypeList = new List<string>();
                //Check Computed Columns
                if (ColumnDetails.Contains(" AS ") || ColumnDetails.StartsWith(" CM "))
                {
                    string[] sSplitColDetails = Regex.Split(ColumnDetails, " AS ");
                    var sColNameDeatils = sSplitColDetails[0];
                    var sColComputedFrml = sSplitColDetails[1];
                    //the first part consists of column name, datatype and column name used in calculations
                    var sSplitColmnDetails = Regex.Split(sColNameDeatils, ",");
                    var sColName = sSplitColmnDetails[0];
                    // var sColDType = sSplitColmnDetails[1];
                    var sColUsed = sSplitColmnDetails[1];
                    //int list to check if the column exists or computed
                    List<int> liCheckUsedCol = new List<int>();
                    //Remove braces as the format is "{column1.column2}
                    var sColUsedRmv1 = sColUsed.Replace("{", "");
                    var sColUsedRmv2 = sColUsedRmv1.Replace("}", "");
                    if (sColUsedRmv2.Contains("."))
                    {
                        var sCSVColUsed = sColUsedRmv2.Replace("_", ",");
                        List<string> lColUsed = sCSVColUsed.Split(',').ToList();

                        for (var i = 0; i < lColUsed.Count(); i++)
                        {
                            //check if column exists                           
                            string sCheckColComputd = @"IF NOT EXISTS(USE " + sOrgDb + " SELECT * FROM sys.computed_columns WHERE is_computed = 1 and name='" + lColUsed[i] + "'and object_id=OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";
                            Conn.Open();
                            Conn.ChangeDatabase(sOrgDb);
                            SqlCommand SqlcmdComp = new SqlCommand(sCheckColComputd, Conn);
                            int iColComptdExist = Convert.ToInt32(SqlcmdComp.ExecuteScalar());
                            Conn.Close();
                            if (iColComptdExist == 1)
                            {
                                liCheckUsedCol.Add(iColComptdExist);
                            }
                        }
                    }
                    else
                    {
                        string sCheckColComputd = @"IF NOT EXISTS(SELECT * FROM sys.computed_columns WHERE is_computed = 1 and name='" + sColUsedRmv2 + "' and object_id=OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";
                        Conn.Open();
                        Conn.ChangeDatabase(sOrgDb);
                        SqlCommand SqlcmdComp = new SqlCommand(sCheckColComputd, Conn);
                        int iColComptdExist = Convert.ToInt32(SqlcmdComp.ExecuteScalar());
                        Conn.Close();
                        if (iColComptdExist == 1)
                        {
                            liCheckUsedCol.Add(iColComptdExist);
                        }
                        else
                        {
                            //if no donot add to list as the Computed column can be created.
                        }
                        //}
                    }

                    if (liCheckUsedCol.Count == 0)
                    {
                        lColDatatypeList.Add("COMPUTED COLUMN");
                        lColDatatypeList.Add(sColName);
                        lColDatatypeList.Add("AS " + sColComputedFrml);
                        //lColDatatypes.Add(lColDatatypeList);
                    }
                    else
                    {
                        //donot create a computed column
                        bStatus = false;
                        ReturnStatus = ServiceConstants.ErrorMessageComptdColumn;
                    }
                }
                else
                {
                    //check if the foriegn key has references added.

                    int iDetailsCount = Regex.Matches(ColumnDetails, ",").Count;
                    if (iDetailsCount == 0)
                    {
                        var Char = ColumnDetails.Select(c => char.IsUpper(c)).ToList();
                        var Position = Char.IndexOf(true);
                        if (Position == 0 || ColumnDetails.ToLower() == "id")
                        {
                            if (ColumnDetails.ToLower() == "ID".ToLower())
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("int IDENTITY(1,1) PRIMARY KEY");

                            }
                        }
                        else if (Position == 1)
                        {
                            string sFirstLetter = ColumnDetails.Substring(0, 1);
                            if (sFirstLetter == "i")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("int NULL");
                            }
                            else if (sFirstLetter == "s")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("varchar(256) NULL");
                            }
                            else if (sFirstLetter == "d")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("datetime NULL");
                            }
                            else if (sFirstLetter == "r")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("float NULL");
                            }
                            else if (sFirstLetter == "n")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("nvarchar(MAX) NULL");
                            }
                            else if (sFirstLetter == "b")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("bit NULL");
                            }
                        }
                        else if (Position == 2)
                        {
                            string sFrstTwoLetter = ColumnDetails.Substring(0, 2);
                            if (sFrstTwoLetter == "dt")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("datetime NULL");
                            }
                        }
                        // lColDatatypes.Add(lColDatatypeList);
                    }
                    else if (iDetailsCount == 2)
                    {
                        //contains foreign key details ex "FKiOrgID,Orgnaization,ID
                        string[] sSplitColDetails = Regex.Split(ColumnDetails, ",");
                        var sColName = sSplitColDetails[0];
                        var sPK_Table = sSplitColDetails[1];
                        var sPK_Column = sSplitColDetails[2];

                        string sFirstTwoLetters = sColName.Substring(0, 2);
                        if (sFirstTwoLetters == "FK")
                        {
                            string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='" + sDatabase + "' AND TABLE_NAME='" + sPK_Table + "' AND COLUMN_NAME ='" + sPK_Column + "') SELECT 0 ELSE SELECT 1";
                            Conn.Open();
                            Conn.ChangeDatabase(sOrgDb);
                            SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Conn);
                            int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            Conn.Close();
                            if (iExist == 1)
                            {
                                string ThirdLetter = sColName.Substring(2, 1);
                                if (ThirdLetter == "i")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("int_NULL");
                                }
                                else if (ThirdLetter == "s")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("varchar(64)_NULL");

                                }
                                else if (ThirdLetter == "d")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("datetime_NULL");
                                }
                                else if (ThirdLetter == "r")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("float_NULL");
                                }
                                else if (ThirdLetter == "n")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("nvarchar(MAX)_NULL");
                                }
                            }
                            else
                            {
                                //add normal column
                                string ThirdLetter = sColName.Substring(2, 1);
                                if (ThirdLetter == "i")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("int_NULL");
                                }
                                else if (ThirdLetter == "s")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("varchar(64)_NULL");

                                }
                                else if (ThirdLetter == "d")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("datetime_NULL");
                                }
                                else if (ThirdLetter == "r")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("float_NULL");
                                }
                                else if (ThirdLetter == "n")
                                {
                                    lColDatatypeList.Add(sColName + "," + sPK_Table + "," + sPK_Column);
                                    lColDatatypeList.Add("nvarchar(MAX)_NULL");
                                }
                            }
                        }
                        else
                        {
                            //Not foriegn key
                            string sFirstLetter = ColumnDetails.Substring(0, 1);
                            if (sFirstLetter == "i")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("int NULL");
                            }
                            else if (sFirstLetter == "s")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("varchar(256) NULL");
                            }
                            else if (sFirstLetter == "d")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("datetime NULL");
                            }
                            else if (sFirstLetter == "r")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("float NULL");
                            }
                            else if (sFirstLetter == "n")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("nvarchar(MAX) NULL");
                            }
                            else if (sFirstLetter == "b")
                            {
                                lColDatatypeList.Add(ColumnDetails);
                                lColDatatypeList.Add("bit NULL");
                            }
                        }
                        //lColDatatypes.Add(lColDatatypeList);
                    }
                    else
                    {
                        //format is not supported.
                    }
                    lColDatatypes.Add(lColDatatypeList);
                }  //end of check
            }
            if (x == 0)
            //Table exixts
            {

                string sQuery = "create table [" + model.TableName + "] (";
                for (var i = 0; i < lColDatatypes.Count(); i++)
                {
                    if (lColDatatypes[i].Count() > 0)
                    {
                        //check if has computed column
                        if (lColDatatypes[i][0] == "COMPUTED COLUMN")
                        {
                            if (sQuery.Contains(",,") == false)
                            {
                                sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
                            }
                            else
                            {
                                sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
                            }
                        }
                        else if (lColDatatypes[i].Count == 2)
                        {
                            var sColumn = lColDatatypes[i][0];
                            var sDataTyp = lColDatatypes[i][1];
                            string sFirstTwoLtr = sColumn.Substring(0, 2);
                            if (sFirstTwoLtr != "FK")
                            {
                                if (sQuery.Contains(",,") == false)
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                    }

                                }
                                else
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                    }
                                }
                            }//check FK
                            else
                            {
                                if (sColumn.Contains(","))
                                {
                                    string[] sSplitVal = Regex.Split(sColumn, ",");
                                    sSplitVal1 = sSplitVal[0];
                                    sSplitVal2 = sSplitVal[1];
                                    sSplitVal3 = sSplitVal[2];
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
                                        if (sSplitVal2 == "OrganizationClasses")
                                        {
                                            //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                            sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

                                        }
                                        else
                                        {
                                            string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                            string sColmnDtype = sSplitDtype[0];
                                            string sColmnNull = sSplitDtype[1];
                                            //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                            sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
                                        }
                                    }
                                    else
                                    {
                                        if (sSplitVal2 == "OrganizationClasses")
                                        {
                                            //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                            sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
                                        }
                                        else
                                        {
                                            //if (sDataTyp.Contains("_"))
                                            //{
                                            string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                            string sColmnDtype = sSplitDtype[0];
                                            string sColmnNull = sSplitDtype[1];
                                            //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                            sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
                                            //}
                                        }
                                    }
                                }
                                else // if column has no "," then use the column and datatype directly as no primary key or table
                                {
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                    }
                                }
                            }
                        }
                    }
                }
                bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
                if (bStrTwoCm == true)
                {
                    int position = sQuery.LastIndexOf(",,");
                    sQuery = sQuery.Remove(position);
                }
                bool bStrCm = sQuery.Trim().EndsWith(@",");
                if (bStrCm == true)
                {
                    int position = sQuery.LastIndexOf(',');
                    sQuery = sQuery.Remove(position);
                }
                sQuery = sQuery + ")";
                if (sQuery.EndsWith("()"))
                {

                }
                else
                {
                    var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, OrgID, sDatabase, sOrgDb);
                    using (SqlConnection Con = new SqlConnection(sBODataSource))
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        string sStructuredQryReplace = sQuery.Replace(",,,,", ",");
                        string sStructuredQry = sStructuredQryReplace.Replace(",,", ",");
                        cmd = new SqlCommand(sStructuredQry, Con);
                        //Conn.Open();
                        //Conn.ChangeDatabase(sOrgDb);
                        //string sStructuredQryReplace = sQuery.Replace(",,,,", ",");
                        //string sStructuredQry = sStructuredQryReplace.Replace(",,", ",");
                        //SqlCommand cmd = new SqlCommand(sStructuredQry, Conn);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            ReturnStatus = ServiceConstants.SuccessMessage;
                        }
                        catch (Exception Ex)
                        {
                            iTableCreated = 0;
                            bStatus = false;
                            ReturnStatus = ServiceConstants.ErrorMessage;
                        }
                        Conn.Close();
                    }
                }
            }
            else
            {
                string sQuery = "alter table [" + model.TableName + "] add ";
                for (var i = 0; i < lColDatatypes.Count(); i++)
                {
                    if (lColDatatypes[i].Count() > 0)
                    {
                        //check if has computed column
                        if (lColDatatypes[i][0] == "COMPUTED COLUMN")
                        {
                            string sCheckComputdColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')) SELECT 0 ELSE SELECT 1";
                            SC.Open();
                            cmds.Connection = SC;
                            SqlCommand Sqlcmd = new SqlCommand(sCheckComputdColmn, SC);
                            int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            SC.Close();
                            if (iExist == 0)
                            {
                                if (sQuery.Contains(",,") == false)
                                {
                                    sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
                                }
                                else
                                {
                                    sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
                                }
                            }
                            else
                            {
                                //Get the defination for the computed column and check with the new definition
                                var sOldDefinition = "";
                                //Get the computed column definition
                                string sGetCmptdColDef = "SELECT definition FROM sys.computed_columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')";
                                SC.Open();
                                cmds.Connection = SC;
                                SqlCommand SqlcmdColDef = new SqlCommand(sGetCmptdColDef, SC);
                                SqlDataReader DReaderChckDtype = SqlcmdColDef.ExecuteReader();
                                while (DReaderChckDtype.Read())
                                {
                                    sOldDefinition = DReaderChckDtype.IsDBNull(0) ? null : DReaderChckDtype.GetValue(0).ToString();
                                }
                                SC.Close();
                                var sNewDefinition = lColDatatypes[i][2].Replace("AS ", "");
                                var sAltrOldDefnEnd = "(" + sOldDefinition + ")";
                                //Compare old and new Definition
                                if (sAltrOldDefnEnd.ToLower() != sNewDefinition.ToLower())
                                {
                                    //update to new definition but here we need to drop the column and re add the column.
                                    var sDropComputedColumn = "ALTER TABLE [" + model.TableName + "] DROP COLUMN " + lColDatatypes[i][1];
                                    SC.Open();
                                    SqlCommand cmdDrop = new SqlCommand(sDropComputedColumn, SC);
                                    try
                                    {
                                        cmds.Connection = SC;
                                        cmdDrop.ExecuteNonQuery();
                                        Conn.Close();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    var sUpdateComputedColumn = "ALTER TABLE [" + model.TableName + "] ADD " + lColDatatypes[i][1] + " AS " + sNewDefinition;
                                    SC.Open();
                                    SqlCommand cmd = new SqlCommand(sUpdateComputedColumn, SC);
                                    try
                                    {
                                        cmds.Connection = SC;
                                        cmd.ExecuteNonQuery();
                                        Conn.Close();
                                        bStatus = true;
                                        ReturnStatus = ServiceConstants.SuccessMessageComptdColumn;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    // do nothing as both are same
                                }
                            }
                        }
                        else
                        {
                            var sColumn = lColDatatypes[i][0];
                            if (sColumn.Contains(','))
                            {
                                string[] sSplitVal = Regex.Split(sColumn, ",");
                                sColumn = sSplitVal[0];
                            }
                            var sDataTyp = lColDatatypes[i][1];
                            string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + sColumn + "' AND [object_id] = OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";

                            var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, OrgID, sDatabase, sOrgDb);
                            SqlConnection SCs = new SqlConnection(sBODataSources);
                            SCs.Open();
                            SqlCommand ncmd = new SqlCommand();
                            ncmd.Connection = SCs;
                            ncmd = new SqlCommand(sCheckColmn, SCs);
                            SqlCommand DateChecks = new SqlCommand(sCheckColmn, SCs);
                            int iExist = Convert.ToInt32(ncmd.ExecuteScalar());
                            SC.Close();



                            //SC.Open();
                            //SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, SC);
                            //Sqlcmd.Connection = SC;
                            //int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            //SC.Close();



                            if (iExist == 0)
                            {
                                string sFirstTwoLtr = sColumn.Substring(0, 2);
                                if (sFirstTwoLtr != "FK")
                                {
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        if (sDataTyp.Contains("PRIMARY"))
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                        }

                                    }
                                    else
                                    {
                                        if (sDataTyp.Contains("PRIMARY"))
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                        }
                                    }
                                }//check FK
                                else
                                {
                                    //try if the foriegn key is present patterm Foreign Key, table name
                                    if (lColDatatypes[i][0].Contains(","))
                                    {
                                        string[] sSplitVal = Regex.Split(lColDatatypes[i][0], ",");
                                        sSplitVal1 = sSplitVal[0];
                                        sSplitVal2 = sSplitVal[1];
                                        sSplitVal3 = sSplitVal[2];
                                        if (sQuery.Contains(",,") == false)
                                        {
                                            //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
                                            if (sSplitVal2 == "OrganizationClasses")
                                            {
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

                                            }
                                            else
                                            {
                                                string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                                string sColmnDtype = sSplitDtype[0];
                                                string sColmnNull = sSplitDtype[1];
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
                                            }
                                        }
                                        else
                                        {
                                            if (sSplitVal2 == "OrganizationClasses")
                                            {
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
                                            }
                                            else
                                            {
                                                string[] sSplitDtype = Regex.Split(sDataTyp, " ");
                                                string sColmnDtype = sSplitDtype[0];
                                                string sColmnNull = sSplitDtype[1];
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
                                            }
                                        }
                                    }
                                    else // if column has no "," then use the column and datatype directly as no primary key or table
                                    {
                                        if (sQuery.Contains(",,") == false)
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                        }
                                    }
                                }
                            }
                            else
                            {//no datatype change for now
                                var sOld_Datatype = "";
                                var sOld_MxLength = "";
                                //check is the datatype is same
                                string sCheckColmnDatatype = "SELECT DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + model.Name + "' AND COLUMN_NAME = '" + sColumn + "'";
                                SC.Open();
                                SqlCommand SqlcmdChckDtype = new SqlCommand(sCheckColmnDatatype, SC);
                                SqlDataReader DReaderCckDtype = SqlcmdChckDtype.ExecuteReader();
                                while (DReaderCckDtype.Read())
                                {
                                    sOld_Datatype = DReaderCckDtype.IsDBNull(0) ? null : DReaderCckDtype.GetValue(0).ToString();
                                    sOld_MxLength = DReaderCckDtype.IsDBNull(1) ? null : DReaderCckDtype.GetValue(1).ToString();
                                    //Check if Old datatype is primary key.
                                    string sCheckPrimaryKey = @"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY' AND K.COLUMN_NAME = '" + sColumn + "' AND K.TABLE_NAME = '" + model.Name + "') SELECT 0 ELSE SELECT 1";
                                    //Conn.Open();
                                    SqlCommand SqlcmdCheck = new SqlCommand(sCheckPrimaryKey, SC);
                                    int iIsPrimary = Convert.ToInt32(SqlcmdCheck.ExecuteScalar());
                                    //Conn.Close();
                                    if (iIsPrimary == 1)
                                    {
                                        sOld_Datatype = sOld_Datatype + " IDENTITY(1, 1) PRIMARY KEY";
                                    }
                                    else
                                    {
                                        if (sOld_MxLength != null)
                                        {
                                            sOld_Datatype = sOld_Datatype + "(" + sOld_MxLength + ")";
                                        }
                                    }


                                    //check if the new datatype has "(" to check max length.
                                    var sNew_DataType = "";
                                    if (sDataTyp.Contains("PRIMARY") == true)
                                    {
                                        //string[] sSplitDType = sDataTyp.Split('(');
                                        //sNew_DataType = sSplitDType[0];
                                        sNew_DataType = sDataTyp;
                                    }
                                    else if (sDataTyp.Contains(" NOT NULL"))
                                    {
                                        sNew_DataType = sDataTyp.Replace(" NOT NULL", "");
                                    }
                                    else if (sDataTyp.Contains(" NULL"))
                                    {
                                        sNew_DataType = sDataTyp.Replace(" NULL", "");
                                    }
                                    else
                                    {
                                        sNew_DataType = sDataTyp;
                                    }
                                    //returns status
                                    //if (sNew_DataType != sOld_Datatype)
                                    //{
                                    //    DatatypeChanged = CheckDatatypeConvertion(model.Name, sOld_Datatype, sNew_DataType, sColumn, sDataTyp);
                                    //    if (DatatypeChanged == 0)
                                    //    {
                                    //        sDTypeNotChngd.Add(sColumn);
                                    //    }

                                    //}
                                    if (sDTypeNotChngd.Count == 0)
                                    {
                                        ReturnStatus = ServiceConstants.DataTypeChangeSuccess;
                                        bStatus = true;
                                    }
                                    else
                                    {
                                        ReturnStatus = ServiceConstants.DataTypeChangeError;
                                        bStatus = false;
                                    }
                                }
                                Conn.Close();
                            }
                        }//Not computed column
                    }
                }
                if (sQuery.EndsWith("add "))
                {

                }
                else
                {
                    //Conn.Open();
                    //Conn.ChangeDatabase(sOrgDb);

                    var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, OrgID, sDatabase, sOrgDb);
                    SqlConnection SCs = new SqlConnection(sBODataSources);
                    SCs.Open();
                    SqlCommand ncmd = new SqlCommand();
                    ncmd.Connection = SCs;



                    bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
                    if (bStrTwoCm == true)
                    {
                        int position = sQuery.LastIndexOf(",,");
                        sQuery = sQuery.Remove(position);
                    }
                    bool bStrCm = sQuery.Trim().EndsWith(@",");
                    if (bStrCm == true)
                    {
                        int position = sQuery.LastIndexOf(',');
                        sQuery = sQuery.Remove(position);
                    }

                    string sStructuredQryRemove = sQuery.Replace(",,,,", ",");
                    string sStructuredQry = sStructuredQryRemove.Replace(",,", ",");
                    SqlCommand cmd = new SqlCommand(sStructuredQry, SCs);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        ReturnStatus = ServiceConstants.SuccessMessage;
                        bStatus = true;
                    }
                    catch (Exception ex)
                    {
                        bStatus = false;
                        iTableUpdated = 0;
                        ReturnStatus = ServiceConstants.ErrorMessage;
                    }
                    if (iTableUpdated == 1)
                    {

                    }
                    SCs.Close();
                }
            }
            SaveBOID = SaveBO(model, iUserID, sOrgName, sDatabase);
            // return the status based on the success;
            return new VMCustomResponse { ResponseMessage = ReturnStatus, ID = SaveBOID, Status = bStatus };
            //return ReturnStatus;        
        }
        public int CheckDatatypeConvertion(string TableName, string sOld_Datatype, string sNew_DataType, string sColumn, string sDataTyp, int iUserID, string sOrgName, string sDatabase)
        {

            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            string sQuery = "ALTER TABLE [" + TableName + "] ";
            int iDatatypeStatus = 0;
            if (sOld_Datatype.Contains("PRIMARY"))
            {
                iDatatypeStatus = 1;
            }
            else
            {
                if (sOld_Datatype == "int" && (sNew_DataType == "int" || sNew_DataType == "float"))
                {
                    iDatatypeStatus = 1;
                }
                else if (sOld_Datatype == "varchar" && (sNew_DataType == "varchar" || sNew_DataType == "nvarchar"))
                {
                    iDatatypeStatus = 1;
                }
                else if (sOld_Datatype.Contains("datetime"))
                {
                    iDatatypeStatus = 0;
                }
            }
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            if (iDatatypeStatus == 1)
            {
                //new datatype can be used
                sQuery = sQuery + "ALTER COLUMN " + sColumn + " " + sDataTyp;
                using (Conn)
                {
                    Conn.Open();
                    Conn.ChangeDatabase(sOrgDb);
                    SqlCommand cmd = new SqlCommand(sQuery, Conn);
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Conn.Close();
                    }
                    catch (Exception ex)
                    {
                        //error
                    }
                }
            }
            else
            {
                //retain the old datatype
            }
            return iDatatypeStatus;
        }

        #region DynamicForm
        public List<string> GetDefaultValues(string sAttrNames, string BOName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var lAttrAndDefault = new List<string>();
            int iBOID = dbContext.BOs.Where(m => m.Name == BOName).Select(m => m.BOID).FirstOrDefault();
            List<string> lAttrNames = sAttrNames.Split(',').ToList();
            for (var i = 0; i < lAttrNames.Count(); i++)
            {
                string sAttr = lAttrNames[i];
                string sDefault = dbContext.BOFields.Where(m => m.BOID == iBOID).Where(m => m.Name == sAttr).Select(m => m.DefaultValue).FirstOrDefault();
                string sOptionList = dbContext.BOOptionLists.Where(m => m.BOID == iBOID).Where(m => m.Name == sAttr).Select(m => m.sOptionName).FirstOrDefault();

                if (sOptionList != "" && sOptionList != null && sDefault != null)
                {
                    lAttrAndDefault.Add(sDefault + "_" + lAttrNames[i]);
                }
                else
                {

                }

            }
            return lAttrAndDefault;
        }



        public int SaveFormData(List<FormData> FormValues, string sTableName, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int iStatus = 0;
            var sColNme = new List<string>();
            var sColVal = new List<string>();
            try
            {
                foreach (var items in FormValues)
                {
                    sColNme.Add(items.Label);
                    sColVal.Add("'" + items.Value + "'");
                }
                var sColNames = string.Join(",", sColNme);
                var sValues = string.Join(",", sColVal);
                var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
                //var DataSource=dbContext.BOs.Where(s => s.TableName.ToLower() == sTableName.ToLower()).Select(s => s.sDataSource).FirstOrDefault();
                //SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
                using (SqlConnection Con = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
                {
                    SqlCommand cmd = new SqlCommand("", Con);
                    cmd.CommandText = "INSERT INTO " + sTableName + " (" + sColNames + ") VALUES (" + sValues + ")";
                    Con.Open();
                    Con.ChangeDatabase(sOrgDb);
                    cmd.ExecuteNonQuery();
                    Con.Dispose();
                }
                iStatus = 1;
            }
            catch (Exception ex)
            {
                iStatus = 0;
            }
            return iStatus;
        }

        //Dynamic From using Xilink
        public VMCreateForm CreateDynamicForm(int XiLinkID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMCreateForm FormVal = new VMCreateForm();
            var BOName = dbContext.XiLinkNVs.Where(m => m.Name == "BO").Where(m => m.XiLinkID == XiLinkID).Select(m => m.Value).FirstOrDefault();
            var GroupName = dbContext.XiLinkNVs.Where(m => m.Name == "GroupName").Where(m => m.XiLinkID == XiLinkID).Select(m => m.Value).FirstOrDefault();
            var FormName = dbContext.XiLinkNVs.Where(m => m.Name == "FormName").Where(m => m.XiLinkID == XiLinkID).Select(m => m.Value).FirstOrDefault();
            var sBOAttributes = new List<List<string>>();
            // var sDefaultVal = new List<string>();
            List<List<VMDropDown>> DropDownValues = new List<List<VMDropDown>>();
            List<List<VMDropDown>> OptionDropDownValues = new List<List<VMDropDown>>();
            var oBO = dbContext.BOs.Where(m => m.Name == BOName).FirstOrDefault();
            int iBOID = oBO.BOID;
            //Get the details based on the group name, GroupName "CreateForm" id for dynamic form.
            var FieldIDs = oBO.BOGroups.Where(m => m.GroupName == GroupName).Select(m => m.BOFieldIDs).FirstOrDefault();
            string[] values = FieldIDs.Split(',');
            for (int i = 0; i < values.Length; i++)
            {

                var sBOAttrs = new List<string>();
                int iID = Convert.ToInt32(values[i]);
                string sFields = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.Name).FirstOrDefault();
                bool IsOptional = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.IsOptionList).FirstOrDefault();
                if (IsOptional == true)
                {
                    string OPt_sFields = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Where(m => m.IsOptionList == true).Select(m => m.Name).FirstOrDefault();
                    if (OPt_sFields != "" && OPt_sFields != null)
                    {
                        string sOptionList = dbContext.BOOptionLists.Where(m => m.BOID == iBOID).Where(m => m.Name == OPt_sFields).Select(m => m.sOptionName).FirstOrDefault();

                        if (sOptionList != "" && sOptionList != null)
                        {
                            sBOAttrs.Add(OPt_sFields + "_OP");
                            sBOAttrs.Add(OPt_sFields + "_OP");
                        }
                        else
                        {
                            sBOAttrs.Add(OPt_sFields);
                            sBOAttrs.Add(OPt_sFields);
                        }
                    }
                }
                else
                {
                    string sOPt_sFields = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.Name).FirstOrDefault();
                    sBOAttrs.Add(sOPt_sFields);
                    sBOAttrs.Add(oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.LabelName).FirstOrDefault());

                }
                //Get Type of data
                int TypeID = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.TypeID).FirstOrDefault();
                BODatatypes DataType = (BODatatypes)TypeID;
                string type = DataType.ToString();
                sBOAttrs.Add(type);

                //Length
                string sMaxLen = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.MaxLength).FirstOrDefault();
                sBOAttrs.Add(sMaxLen);

                sBOAttributes.Add(sBOAttrs);

                string sFkTblName = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.FKTableName).FirstOrDefault();
                if (sFkTblName != null && sFkTblName.Length > 0)
                {
                    var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
                    //using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString))
                    using (SqlConnection Con = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
                    {
                        Con.Open();
                        Con.ChangeDatabase(sOrgDb);
                        SqlCommand cmd = new SqlCommand("", Con);
                        if (sFkTblName == "OrganizationClasses")
                        {
                            //cmd.CommandText = "Select ClassID, Class FROM " + sFkTblName + " WHERE OrganizationID=" + OrgID;
                            cmd.CommandText = "Select ClassID, Class FROM " + sFkTblName;
                        }
                        else
                        {
                            cmd.CommandText = "Select ID, Name FROM " + sFkTblName;
                        }
                        SqlDataReader reader = cmd.ExecuteReader();
                        DataTable data = new DataTable();
                        data.Load(reader);
                        List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                        var DDLVlaues = (from c in TotalResult
                                         select new VMDropDown { Type = sFields, Value = Convert.ToInt32(c[0]), text = c[1].ToString() }).ToList();
                        Con.Close();
                        DropDownValues.Add(DDLVlaues);
                    }
                    FormVal.sFKDropdwn = DropDownValues;
                }
                else
                {
                    DropDownValues.Add(null);
                    FormVal.sFKDropdwn = DropDownValues;
                }
                //add Option values from table 
                bool bOptionList = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.IsOptionList).FirstOrDefault();
                if (bOptionList == true)
                {
                    string attrName = oBO.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.Name).FirstOrDefault();
                    string SOptionList = dbContext.BOOptionLists.Where(m => m.BOID == iBOID).Where(m => m.Name == attrName).Select(m => m.sOptionName).FirstOrDefault();

                    if (SOptionList != "" && SOptionList != null)
                    {
                        var lOptions = new List<string>();
                        lOptions = SOptionList.Split(',').ToList();
                        string SValList = dbContext.BOOptionLists.Where(m => m.BOID == iBOID).Where(m => m.Name == attrName).Select(m => m.sValues).FirstOrDefault();
                        List<VMDropDown> OptionFields = new List<VMDropDown>();
                        List<string> Values = SValList.Split(',').ToList();
                        for (int a = 0; a < Values.Count(); a++)
                        {
                            OptionFields.Add(new VMDropDown
                            {
                                text = lOptions[a],
                                Expression = Values[a]
                            });

                        }
                        OptionDropDownValues.Add(OptionFields);
                        //using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                        //{
                        //    Con.Open();
                        //    SqlCommand cmd = new SqlCommand("", Con);
                        //    cmd.CommandText = "Select sValues FROM BOOptionLists";
                        //    SqlDataReader reader = cmd.ExecuteReader();
                        //    DataTable data = new DataTable();
                        //    data.Load(reader);
                        //    List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();

                        //    var DDLVlaues = (from c in TotalResult
                        //                     select new VMDropDown { Type = sFields, Value = 0, text = c[0].ToString() }).ToList();
                        //    //select new VMDropDown { Type = sFields, Value = Convert.ToInt32(c[0]), text = c[1].ToString() }).ToList();
                        //    Con.Close();

                        //    OptionDropDownValues.Add(DDLVlaues);
                        //}
                        FormVal.sOptionsDropdwn = OptionDropDownValues;
                    }
                    else
                    {
                        OptionDropDownValues.Add(null);
                        FormVal.sOptionsDropdwn = OptionDropDownValues;
                    }
                }
                else
                {
                    OptionDropDownValues.Add(null);
                    FormVal.sOptionsDropdwn = OptionDropDownValues;
                }

                ////Add default values
                //string sDefault = dbContext.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.DefaultValue).FirstOrDefault();
                //string sAttrName = dbContext.BOFields.Where(m => m.BOID == iBOID).Where(m => m.ID == iID).Select(m => m.Name).FirstOrDefault();
                //if (sDefault != null)
                //{
                //    sDefaultVal.Add(sDefault+"_"+ sAttrName);
                //}
                //else
                //{
                //    sDefaultVal.Add(null);
                //}
                //FormVal.sDefaultValues = sDefaultVal;



            }


            FormVal.sBOName = BOName;
            FormVal.sBOAttrDetails = sBOAttributes;
            FormVal.FormName = FormName;
            return FormVal;
        }
        #endregion DynamicForm


        #region Scripts

        public DTResponse GetBOScriptsList(jQueryDataTableParamModel param, int BOID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            IQueryable<BOScripts> AllBOScripts;
            if (param.bFromBO)
            {
                AllBOScripts = dbContext.BOScripts.Where(m => m.FKiApplicationID == FKiAppID).Where(m => m.FKiBOID == BOID);
            }
            else
            {
                AllBOScripts = dbContext.BOScripts.Where(m => m.FKiApplicationID == FKiAppID);
            }

            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllBOScripts = AllBOScripts.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllBOScripts.Count();
            AllBOScripts = QuerableUtil.GetResultsForDataTables(AllBOScripts, "", sortExpression, param);
            var clients = AllBOScripts.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName, c.sScript, c.sDescription, c.sType, c.sLanguage, Convert.ToString(c.StatusTypeID),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public VMCustomResponse SaveBOScript(BOScripts model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOScripts oBOSrpt = new BOScripts();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            float rScriptID = 0;
            if (model.ID == 0)
            {
                oBOSrpt.FKiBOID = model.FKiBOID;
                oBOSrpt.sName = model.sName;
                oBOSrpt.sDescription = model.sDescription;
                oBOSrpt.sType = model.sType;
                oBOSrpt.sLanguage = model.sLanguage;
                oBOSrpt.sScript = model.sScript;
                oBOSrpt.StatusTypeID = model.StatusTypeID;
                oBOSrpt.CreatedBy = model.CreatedBy;
                oBOSrpt.CreatedTime = DateTime.Now;
                oBOSrpt.UpdatedBy = model.UpdatedBy;
                oBOSrpt.UpdatedTime = DateTime.Now;
                oBOSrpt.sVersion = model.sVersion;
                oBOSrpt.sMethodName = model.sMethodName;
                oBOSrpt.FKiApplicationID = FKiAppID;
                oBOSrpt.OrganisationID = iOrgID;
                oBOSrpt.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oBOSrpt.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.BOScripts.Add(oBOSrpt);
                dbContext.SaveChanges();
                rScriptID = oBOSrpt.ID;
            }
            else
            {
                oBOSrpt = dbContext.BOScripts.Find(model.ID);
                oBOSrpt.sName = model.sName;
                oBOSrpt.sDescription = model.sDescription;
                oBOSrpt.sType = model.sType;
                oBOSrpt.sLanguage = model.sLanguage;
                oBOSrpt.sScript = model.sScript;
                oBOSrpt.sVersion = model.sVersion;
                oBOSrpt.sMethodName = model.sMethodName;
                oBOSrpt.StatusTypeID = model.StatusTypeID;
                oBOSrpt.UpdatedBy = model.UpdatedBy;
                oBOSrpt.UpdatedTime = DateTime.Now;
                oBOSrpt.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oBOSrpt.FKiApplicationID = FKiAppID;
                oBOSrpt.OrganisationID = iOrgID;
                dbContext.SaveChanges();
            }
            if (model.ScriptResults != null && model.ScriptResults.Count() > 0)
            {
                var ExistingResults = dbContext.BOScriptResults.Where(m => m.FKiScriptID == oBOSrpt.ID).ToList();
                if (ExistingResults.Count() > 0)
                {
                    dbContext.BOScriptResults.RemoveRange(ExistingResults);
                    dbContext.SaveChanges();
                }
                foreach (var script in model.ScriptResults)
                {
                    if (script.sResultCode != null && script.iAction > 0 && script.iType > 0)
                    {
                        BOScriptResults oResult = new BOScriptResults();
                        oResult.FKiScriptID = oBOSrpt.ID;
                        oResult.sResultCode = script.sResultCode;
                        oResult.iType = script.iType;
                        oResult.iAction = script.iAction;
                        oResult.sUserError = script.sUserError;
                        dbContext.BOScriptResults.Add(oResult);
                        dbContext.SaveChanges();
                    }
                }
            }
            return new VMCustomResponse { Status = true, ID = oBOSrpt.ID, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public BOScripts GetScriptByID(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOScripts oBOSrpt = new BOScripts();
            oBOSrpt = dbContext.BOScripts.Find(ID);
            return oBOSrpt;
        }
        public long CopyScript(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            long ReturnID = 0;
            try
            {
                var oScriptDetails = dbContext.BOScripts.Find(ID);
                if (oScriptDetails.ScriptResults.Count() == 0)
                {
                    var oScriptResults = dbContext.BOScriptResults.Where(m => m.FKiScriptID == ID);
                    if (oScriptResults.Count() > 0)
                    {
                        oScriptDetails.ScriptResults = oScriptResults.ToList();
                    }
                }
                if (oScriptDetails != null)
                {
                    oScriptDetails.ID = 0;
                    oScriptDetails.sName = oScriptDetails.sName + " Copy";
                    if (oScriptDetails.ScriptResults.Count() > 0)
                    {
                        oScriptDetails.ScriptResults.ToList().ForEach(m =>
                        {
                            m.FKiScriptID = 0;
                            m.ID = 0;
                        });
                    }
                    var oResponce = SaveBOScript(oScriptDetails, iUserID, sOrgName, sDatabase);

                    if (oResponce.ID > 0)
                    {
                        return ReturnID = oResponce.ID;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ReturnID;
        }
        #endregion Scripts

        #region BOClassAttributes

        public DTResponse GetClassAttributesGrid(jQueryDataTableParamModel param, int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<BOClassAttributes> AllTypes;
            AllTypes = dbContext.BOClassAttributes.Where(m => m.BOID == BOID);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTypes = AllTypes.Where(m => m.Class.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTypes.Count();
            AllTypes = QuerableUtil.GetResultsForDataTables(AllTypes, "", sortExpression, param);
            var clients = AllTypes.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     join b in dbContext.BOs.ToList() on c.BOID equals b.BOID
                     select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), b.Name, c.Class, Convert.ToString(c.StatusTypeID),""  };
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public BOClassAttributes EditClassAttribute(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOClassAttributes model = dbContext.BOClassAttributes.Find(ID);
            return model;
        }

        public bool IsExistsClassName(string Class, int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var AllBOs = dbContext.BOClassAttributes.ToList();
            //BOClassAttributes Classs = AllBOs.Where(m => m.Class.Equals(Class, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var result = AllBOs.Where(m => m.Class == Class).FirstOrDefault();
            if (ID == 0)
            {
                if (result != null)
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
                if (result != null)
                {
                    if (ID == result.ID)
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
        public VMCustomResponse SaveBOClassAttibute(BOClassAttributes model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOClassAttributes oBOClsAttr = new BOClassAttributes();
            if (model.ID == 0)
            {
                oBOClsAttr.BOID = model.BOID;
                oBOClsAttr.Class = model.Class;
                oBOClsAttr.StatusTypeID = model.StatusTypeID;
                dbContext.BOClassAttributes.Add(oBOClsAttr);
                dbContext.SaveChanges();
            }
            else
            {
                oBOClsAttr = dbContext.BOClassAttributes.Find(model.ID);
                oBOClsAttr.BOID = model.BOID;
                oBOClsAttr.Class = model.Class;
                oBOClsAttr.StatusTypeID = model.StatusTypeID;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse() { ID = oBOClsAttr.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        #endregion BOClassAttributes

        #region ImportBO

        public string ImportBOInXML(string sFilePath, int iUserID, string sOrgName, string sDatabase)
        {
            string sStatus = "failure";
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext dbContext = new ModelDbContext();
            var sXMLFileErrorFiles = new List<string>();
            ////Call the seperate method to read all the files in folder.
            //string sourcePath = "E:/XIDynaware-My folder/2017.11.15/Dan/Import BO in XML-2018.24.05/bo/bo";
            // string sourcePath = "E:/XIDynaware-My folder/2017.11.15/Dan/Import BO in XML-2018.24.05/boDemo/bo";
            //  string sourcePath = "E:/XIDynaware-My folder/2017.11.15/Dan/Import BO in XML-2018.24.05/Check/bo";
            List<List<string>> lFileDetails = CopyDirectoryFilesRecursively(sFilePath, sDatabase);
            var sErrorDetails = new List<string>();
            int iBOID = 0;
            foreach (var sFiles in lFileDetails[0])
            {
                try
                {
                    iBOID = ExtractXMLData(sFiles, iUserID, sOrgName, sDatabase);
                }
                catch (Exception Ex)
                {

                    if (iBOID > 0)
                    {
                        //Delete BO
                        BOs oBODetails = dbContext.BOs.Find(iBOID);
                        if (oBODetails != null)
                        {
                            dbContext.BOs.Remove(oBODetails);
                            dbContext.SaveChanges();
                        }
                        //Delete BO Fields
                        List<BOFields> oBOFields = dbContext.BOFields.Where(m => m.BOID == iBOID).ToList();
                        if (oBOFields.Count > 0)
                        {
                            dbContext.BOFields.RemoveRange(oBOFields);
                            dbContext.SaveChanges();
                        }
                        //Delete BO Group Fields
                        List<BOGroupFields> oBOGrpFields = dbContext.BOGroupFields.Where(m => m.BOID == iBOID).ToList();
                        if (oBOFields.Count > 0)
                        {
                            dbContext.BOGroupFields.RemoveRange(oBOGrpFields);
                            dbContext.SaveChanges();
                        }
                    }
                    sXMLFileErrorFiles.Add(sFiles);
                    sErrorDetails.Add(sFiles + "_:_" + Ex);
                    var sSplitFile = sFiles.Split('/');
                    using (SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNADbContext"].ConnectionString))
                    {
                        SqlCommand cmd = new SqlCommand("", Con);
                        //create a table to save the error details
                        string sBODetails = "INSERT INTO XIXmlImportFileError_XIF_T (FilePath,sError ) VALUES ('" + sFiles + "','" + Ex.ToString() + "')";
                        cmd.CommandText = sBODetails;
                        Con.Open();
                        cmd.ExecuteScalar();
                        //Con.ChangeDatabase(sOrgDb);
                        Con.Dispose();
                    }
                }
            }
            if (sErrorDetails.Count == 0 && iBOID > 0)
            {
                sStatus = "success";
            }
            else
            {
                sStatus = "Alert";
            }

            return sStatus;
        }

        public List<List<string>> CopyDirectoryFilesRecursively(string sourcePath, string sDatabase)
        {
            var lExcludedFileDetails = new List<string>();
            var lFileDetailsToExtract = new List<string>();
            var lFileDetails = new List<List<string>>();

            string[] ImageFormats = { ".XML", ".xml" };
            DirectoryInfo sourceinfo = new DirectoryInfo(sourcePath);
            foreach (var folder in sourceinfo.GetDirectories())
            {
                if (folder.Name != "prev")
                {
                    var AppPath = ConfigurationManager.AppSettings["ApplicationPath"];
                    string StrParent = folder.Parent.ToString();
                    //string targetPath = AppPath +"//Content//BOExtracted/" + folder.Name.ToString();
                    string targetPath = System.Web.HttpContext.Current.Server.MapPath("~/Content/BOExtracted/BOs/bo/bo/" + folder.Name.ToString());
                    Common.SaveErrorLog(targetPath, sDatabase);
                    DirectoryInfo target = new DirectoryInfo(targetPath);
                    //if (StrParent.ToLower().Trim() == ServiceConstants.ECGFolder.ToLower().Trim())
                    //{
                    if (!Directory.Exists(target.FullName))
                    {
                        Directory.CreateDirectory(target.FullName);
                    }
                    //}
                    foreach (FileInfo file in folder.GetFiles())
                    {
                        if (file.Length != 0 && ImageFormats.Contains(file.Extension.ToUpper()))
                        {
                            DirectoryInfo d = new DirectoryInfo(file.Directory.ToString());
                            if (d.Parent.Parent != null)
                            {
                                string tpath = folder.Name;
                                targetPath = System.Web.HttpContext.Current.Server.MapPath("~/Content/BOExtracted/BOs/bo/bo/" + tpath);
                                //targetPath = AppPath +"//Content//BOExtracted/" + tpath;
                                target = new DirectoryInfo(targetPath);
                                if (!Directory.Exists(target.FullName))
                                {
                                    Directory.CreateDirectory(target.FullName);
                                }
                                //exclude file with name "!Template"
                                if (file.Name.Contains("!Template"))
                                {
                                    //reference
                                    lExcludedFileDetails.Add(target.ToString() + "/" + file.Name);
                                }
                                else
                                {
                                    file.CopyTo(System.IO.Path.Combine(target.ToString(), file.Name), true);
                                    //add details for reference                                                  
                                    lFileDetailsToExtract.Add(target.ToString() + "/" + file.Name);

                                }
                            }
                        }
                    }
                    CopyDirectoryFilesRecursively(folder.FullName, sDatabase);
                }
            }
            lFileDetails.Add(lFileDetailsToExtract);
            lFileDetails.Add(lExcludedFileDetails);
            return lFileDetails;
        }


        public int ExtractXMLData(string FilePath, int iUserID, string sOrgName, string sDatabase)
        {
            //Check Size
            var SizeDictionary = new Dictionary<string, string>();
            SizeDictionary.Add("zSmall", "10");
            SizeDictionary.Add("zMedium", "20");
            SizeDictionary.Add("zLarge", "30");
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            int OrgID = UserDetails.FKiOrgID;
            int iBOID = 0;
            ModelDbContext dbContext = new ModelDbContext();
            string sBOName = "";
            //XDocument doc = XDocument.Load(FilePath);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(FilePath);
            XmlNodeList xnList = xmlDoc.SelectNodes("entity");
            //Getting BO Details
            BOs oBO = new BOs();
            string sTableName = string.Empty;
            foreach (XmlNode xn in xnList)
            {
                string sName = xn.SelectSingleNode("name").InnerText;
                if (sName.Contains("/"))
                {
                    string[] sSplitName = sName.Split('/');
                    oBO.Name = sSplitName[1];
                }
                else
                {
                    oBO.Name = sName;
                }
                oBO.sVersion = xn.SelectSingleNode("version").InnerText;
                XmlNodeList Label = xn.SelectNodes("label");
                foreach (XmlNode node in Label)
                {
                    XmlNode EN = node.SelectSingleNode("EN");
                    if (EN != null)
                    {
                        if (EN.SelectSingleNode("label") != null)
                        {
                            oBO.LabelName = EN.SelectSingleNode("label").InnerText;
                        }
                        oBO.Description = EN.SelectSingleNode("description") == null ? null : EN.SelectSingleNode("description").InnerText;
                    }
                }
                string sSize = xn.SelectSingleNode("size") == null ? "0" : xn.SelectSingleNode("size").InnerText;
                if (sSize != "0")
                {
                    oBO.sSize = SizeDictionary[sSize];
                }
                else
                {
                    oBO.sSize = "0";
                }
                oBO.TableName = xn.SelectSingleNode("table") == null ? null : xn.SelectSingleNode("table").InnerText;
                sTableName = oBO.TableName;
                if (string.IsNullOrEmpty(sTableName))
                {
                    sTableName = oBO.Name;
                }
                // oBO.sequence = xn.SelectSingleNode("sequence").InnerText;
                oBO.ClassName = xn.SelectSingleNode("classname") == null ? null : xn.SelectSingleNode("classname").InnerText;
                oBO.sPrimaryKey = xn.SelectSingleNode("primarykey") == null ? null : xn.SelectSingleNode("primarykey").InnerText;
                oBO.sTimeStamped = xn.SelectSingleNode("auditable") == null ? "20" : (xn.SelectSingleNode("auditable").InnerText == "auditable" ? "10" : "20");
                oBO.sDeleteRule = xn.SelectSingleNode("deleterule") == null ? "20" : (xn.SelectSingleNode("deleterule").InnerText == "allowed" ? "10" : "20");
                //oBO.deleterule = xn.SelectSingleNode("security").InnerText;
                oBO.sSearchType = xn.SelectSingleNode("searchtype") == null ? null : xn.SelectSingleNode("searchtype").InnerText;
                //oBO.iDataSource = xn.SelectSingleNode("datasource") == null ? "0" : xn.SelectSingleNode("datasource").InnerText;
                oBO.iDataSource = 0;
                oBO.sNotes = xn.SelectSingleNode("comment") == null ? null : xn.SelectSingleNode("comment").InnerText;
                oBO.sHelpItem = xn.SelectSingleNode("helpid") == null ? "0" : xn.SelectSingleNode("helpid").InnerText;
                if (string.IsNullOrEmpty(oBO.Description))
                {
                    oBO.Description = oBO.Name;
                }
                BODefinition BODefFields = new BODefinition();

                //Getting all BO Fields (or) attributes 
                XmlNode Attributes = xn.SelectSingleNode("attributes");
                BODefFields = GetAttibutes(Attributes);

                //Add BO details to BODefinition model
                BODefFields.BODetails = oBO;

                //AttributeGroups
                XmlNode AttributeGroups = xn.SelectSingleNode("attributegroups");
                var BOGroupFields = GetAttibuteGroups(AttributeGroups);
                BODefFields.BOGroups = BOGroupFields;

                //Save details to BO, BO fields, BO groups etc..   

                //Add details to BO table
                //TODO Check if BO already exists
                var BODetails = BODefFields.BODetails;
                BOs Bo = new BOs();
                int iCheckBOID = dbContext.BOs.Where(m => m.Name == BODetails.TableName && (m.OrganizationID == 0 || m.OrganizationID == OrgID)).Select(m => m.BOID).FirstOrDefault();
                if (iCheckBOID == 0)
                {
                    //BO name and table name has to be same in XI so add table name as BO name                    
                    Bo.Name = BODetails.TableName;
                    Bo.LabelName = BODetails.LabelName;
                    Bo.TableName = BODetails.TableName;
                    //Bo.Description = BODetails.Description;
                    //As the table and BO name are different save BO name in description
                    Bo.Description = BODetails.Description;
                    //Bo.TypeID = 10;
                    Bo.OrganizationID = OrgID;
                    Bo.TypeID = 10;
                    //Bo.FieldCount = model.FieldCount;
                    Bo.StatusTypeID = 10;
                    Bo.CreatedBy = iUserID;
                    Bo.UpdatedBy = iUserID;
                    Bo.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Bo.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Bo.CreatedTime = DateTime.Now;
                    Bo.UpdatedTime = DateTime.Now;
                    //1/6/2018
                    Bo.sVersion = BODetails.sVersion;
                    Bo.sSize = BODetails.sSize;
                    Bo.bUID = BODetails.bUID;
                    Bo.sPrimaryKey = BODetails.sPrimaryKey;
                    Bo.sTimeStamped = BODetails.sTimeStamped;
                    if (Bo.sTimeStamped == "10")
                    {
                        //CreateTimeStampedDetails(Bo.BOID, Bo.CreatedByID, Bo.CreatedTime, sDatabase);
                    }
                    Bo.sDeleteRule = BODetails.sDeleteRule;
                    Bo.sHelpItem = BODetails.sHelpItem;
                    Bo.sNotes = BODetails.sNotes;
                    Bo.iDataSource = BODetails.iDataSource;
                    Bo.sUpdateVersion = BODetails.sUpdateVersion;
                    Bo.sAudit = BODetails.sAudit;
                    Bo.sSearchType = BODetails.sSearchType;
                    Bo.iUpdateCount = 10;
                    Bo.bIsAutoIncrement = BODetails.bIsAutoIncrement;
                    dbContext.BOs.Add(Bo);
                    dbContext.SaveChanges();
                    iBOID = Bo.BOID;
                    sBOName = Bo.Name;
                }
                else
                {
                    //UpdateBOID
                    iBOID = iCheckBOID;
                    Bo = dbContext.BOs.Find(iCheckBOID);
                    //As the table and BO name are different save BO name in description
                    Bo.Description = BODetails.Description;
                    Bo.LabelName = BODetails.LabelName;
                    Bo.OrganizationID = OrgID;
                    Bo.CreatedBy = iUserID;
                    Bo.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    Bo.CreatedTime = DateTime.Now;
                    //1/6/2018
                    Bo.sVersion = BODetails.sVersion;
                    Bo.sSize = BODetails.sSize;
                    Bo.bUID = BODetails.bUID;
                    Bo.bIsAutoIncrement = BODetails.bIsAutoIncrement;
                    Bo.sPrimaryKey = BODetails.sPrimaryKey;
                    Bo.sTimeStamped = BODetails.sTimeStamped;
                    if (Bo.sTimeStamped == "10")
                    {
                        //CreateTimeStampedDetails(Bo.BOID, Bo.CreatedBy, Bo.CreatedTime, sDatabase);
                    }
                    Bo.sDeleteRule = BODetails.sDeleteRule;
                    Bo.sHelpItem = BODetails.sHelpItem;
                    Bo.sNotes = BODetails.sNotes;
                    Bo.iDataSource = BODetails.iDataSource;
                    Bo.sUpdateVersion = BODetails.sUpdateVersion;
                    Bo.sAudit = BODetails.sAudit;
                    Bo.sSearchType = BODetails.sSearchType;
                    dbContext.SaveChanges();
                }

                if (BODefFields.BOFieldsWithOpt.Count != 0)
                {
                    int i = 0;
                    try
                    {


                        //Seperate Attr with and without option list and Update BOID
                        foreach (var BOFlds in BODefFields.BOFieldsWithOpt)
                        {
                            i++;
                            if (i == 5)
                            {

                            }
                            BOFields oBOFlds = new BOFields();
                            oBOFlds = dbContext.BOFields.Where(m => m.BOID == iBOID && m.Name == BOFlds.Name).FirstOrDefault();
                            if (oBOFlds == null)
                            {
                                oBOFlds = new BOFields();
                            }
                            BOFlds.BOID = iBOID;
                            BOFlds.BOName = sBOName;
                            BOFlds.OrganizationID = OrgID;
                            BOFlds.CreatedByName = "";
                            BOFlds.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                            BOFlds.CreatedByID = iUserID;
                            BOFlds.CreatedTime = DateTime.Now;

                            oBOFlds.CreatedTime = BOFlds.CreatedTime;
                            oBOFlds.BOID = BOFlds.BOID;
                            oBOFlds.BOName = BOFlds.BOName;
                            oBOFlds.Name = BOFlds.Name;
                            oBOFlds.TypeID = BOFlds.TypeID;
                            oBOFlds.LabelName = BOFlds.LabelName;
                            oBOFlds.Format = BOFlds.Format;
                            //if(oBOFlds.FKTableName.Contains('/'))
                            //{

                            //}
                            oBOFlds.IsOptionList = true;
                            oBOFlds.FKTableName = BOFlds.FKTableName;
                            if (!string.IsNullOrEmpty(oBOFlds.FKTableName))
                            {
                                oBOFlds.FKiType = 10;
                            }
                            oBOFlds.Script = "";
                            oBOFlds.MaxLength = BOFlds.MaxLength;
                            oBOFlds.IsNull = BOFlds.IsNull;
                            if (oBOFlds.ID > 0)
                            {
                                dbContext.SaveChanges();
                            }
                            else
                            {
                                dbContext.BOFields.Add(oBOFlds);
                                dbContext.SaveChanges();
                            }
                            //List<BOFields> ListBODetails = dbContext.BOFields.Where(m => m.BOID == iBOID).ToList();
                            if (BOFlds.AttrBOOptionList != null)
                            {
                                var OptionList = dbContext.BOOptionLists.Where(m => m.BOFieldID == oBOFlds.ID).ToList();
                                if (OptionList != null && OptionList.Count() > 0)
                                {
                                    dbContext.BOOptionLists.RemoveRange(OptionList);
                                }
                                foreach (var Optn in BOFlds.AttrBOOptionList)
                                {
                                    string sAttrName = Optn.Name;
                                    BOOptionLists oBOLst = new BOOptionLists();
                                    oBOLst.BOID = iBOID;
                                    oBOLst.BOFieldID = oBOFlds.ID;// ListBODetails.Where(m => m.BOID == iBOID).Where(m => m.Name == sAttrName).Select(m => m.ID).FirstOrDefault();
                                    oBOLst.Name = sAttrName;
                                    oBOLst.sOptionName = Optn.sOptionName;
                                    oBOLst.sValues = Optn.sValues;
                                    dbContext.BOOptionLists.Add(oBOLst);
                                    dbContext.SaveChanges();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var k = i;
                    }
                    SyncTable(sTableName, BODefFields.BOFieldsWithOpt, UserDetails.sUserDatabase, sDatabase);
                }
                else
                {
                    //Do nothing
                }

                //add Bofields directly
                if (BODefFields.BOFields != null && BODefFields.BOFields.Count != 0)
                {
                    var AllFields = BODefFields.BOFields;
                    BODefFields.BOFields = new List<BOFields>();
                    var i = 0;
                    try
                    {


                        foreach (var field in AllFields)
                        {
                            i++;
                            var oBOfield = dbContext.BOFields.Where(m => m.BOID == iBOID && m.Name == field.Name).FirstOrDefault();
                            if (oBOfield != null)
                            {
                                dbContext.BOFields.Remove(oBOfield);
                                dbContext.SaveChanges();
                            }
                            oBOfield = new BOFields();
                            oBOfield = field;
                            if (!string.IsNullOrEmpty(oBOfield.FKTableName))
                            {
                                oBOfield.FKiType = 10;
                            }
                            oBOfield.BOID = iBOID;
                            oBOfield.BOName = sBOName;
                            oBOfield.OrganizationID = OrgID;
                            oBOfield.CreatedByID = iUserID;
                            oBOfield.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                            oBOfield.CreatedTime = DateTime.Now;
                            dbContext.BOFields.Add(oBOfield);
                            dbContext.SaveChanges();
                            BODefFields.BOFields.Add(oBOfield);
                        }
                    }
                    catch (Exception ex)
                    {
                        var k = i;
                    }

                    //BODefFields.BOFields.Select(c => { c.BOID = iBOID; return c; }).ToList();
                    //BODefFields.BOFields.Select(c => { c.BOName = sBOName; return c; }).ToList();
                    //BODefFields.BOFields.Select(c => { c.OrganizationID = OrgID; return c; }).ToList();
                    ////BODefFields.BOFields.Select(c => { c.CreatedByName = sCreatedByName; return c; }).ToList();
                    //BODefFields.BOFields.Select(c => { c.CreatedByID = iUserID; return c; }).ToList();
                    //BODefFields.BOFields.Select(c => { c.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString(); return c; }).ToList();
                    ////BODefFields.BOFields.Select(c => { c.CreatedByID = 1; return c; }).ToList();
                    //BODefFields.BOFields.Select(c => { c.CreatedTime = DateTime.Now; return c; }).ToList();
                    //dbContext.BOFields.AddRange(BODefFields.BOFields);
                    //dbContext.SaveChanges();
                    SyncTable(sTableName, BODefFields.BOFields, UserDetails.sUserDatabase, sDatabase);
                }
                else
                {
                    //Do Nothing
                }

                //Save details to BO Groups
                if (BODefFields.BOGroups.Count > 0)
                {

                    foreach (var Grp in BODefFields.BOGroups)
                    {
                        //Add BO field IDs, type etc
                        BOGroupFields BOGrp = new BOGroupFields();
                        BOGrp = dbContext.BOGroupFields.Where(m => m.BOID == iBOID && m.GroupName == Grp.GroupName).FirstOrDefault();
                        if (BOGrp == null)
                        {
                            BOGrp = new BOGroupFields();
                        }
                        BOGrp.BOID = iBOID;
                        BOGrp.GroupName = Grp.GroupName;
                        BOGrp.BOFieldNames = Grp.BOFieldNames;
                        var sAttrFld = Grp.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.None);
                        string sIDs = "";
                        var BOFieldList = new List<BOFields>();
                        BOFieldList = dbContext.BOFields.Where(m => m.BOID == iBOID).ToList();
                        //Static fields for now change it later
                        BOGrp.CreatedByID = iUserID;
                        BOGrp.CreatedByName = "";
                        BOGrp.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                        BOGrp.CreatedTime = DateTime.Now;
                        //u need to have List of BoFields of that BOID                        
                        foreach (var fld in sAttrFld)
                        {
                            string sID = BOFieldList.Where(m => m.Name.ToLower() == fld.ToLower()).Select(m => m.ID).FirstOrDefault().ToString();
                            if (sIDs == "")
                            {
                                sIDs = sID;
                            }
                            else
                            {
                                sIDs = sIDs + ", " + sID;
                            }
                        }

                        BOGrp.BOFieldIDs = sIDs;
                        if (BOGrp.ID > 0)
                        {
                            dbContext.SaveChanges();
                        }
                        else
                        {
                            dbContext.BOGroupFields.Add(BOGrp);
                            dbContext.SaveChanges();
                        }
                    }
                }
            }
            return iBOID;
        }

        private BODefinition GetAttibutes(XmlNode attributes)
        {
            BODefinition oBODef = new BODefinition();
            List<BOFields> BOFieldwithOption = new List<BOFields>();
            List<BOFields> BOFieldWithoutOption = new List<BOFields>();
            //DataType mapping Check
            var DataTypeDictionary = new Dictionary<string, string>();
            DataTypeDictionary.Add("zAutomatic", "INT");
            DataTypeDictionary.Add("zString", "VARCHAR");
            DataTypeDictionary.Add("zLong", "BIGINT");
            DataTypeDictionary.Add("zDouble", "FLOAT");
            DataTypeDictionary.Add("zBoolean", "BIT");
            DataTypeDictionary.Add("zDate", "DATETIME");
            DataTypeDictionary.Add("zTimestamp", "DATETIME");
            DataTypeDictionary.Add("zTime", "DATETIME");
            DataTypeDictionary.Add("zExpression", "VARCHAR");

            List<BOFields> BOFields = new List<BOFields>();
            XmlNodeList Attributes = attributes.SelectNodes("attribute");
            int i = 0;
            try
            {

                foreach (XmlNode Attr in Attributes)
                {
                    i++;
                    BOFields oBOField = new BOFields();
                    var Node = Attr.SelectSingleNode("name");
                    if (Node != null)
                    {
                        string sName = Attr.SelectSingleNode("name").InnerText;
                        if (i == 52)
                        {

                        }
                        if (sName == "id")
                        {
                            oBOField.Name = "ID";
                        }
                        else
                        {
                            oBOField.Name = sName;
                        }
                        XmlNodeList Label = Attr.SelectNodes("label");
                        foreach (XmlNode labl in Label)
                        {
                            XmlNode EN = labl.SelectSingleNode("EN");
                            if (EN != null)
                            {
                                oBOField.LabelName = EN.SelectSingleNode("label").InnerText;
                                oBOField.Description = EN.SelectSingleNode("description").InnerText;
                            }
                        }

                        // oBOField.helpid = Attr.SelectSingleNode("helpid") == null ? null : Attr.SelectSingleNode("helpid").InnerText;
                        oBOField.DefaultValue = Attr.SelectSingleNode("defaultvalue") == null ? null : Attr.SelectSingleNode("defaultvalue").InnerText;
                        oBOField.sXMLDataType = Attr.SelectSingleNode("datatype") == null ? null : Attr.SelectSingleNode("datatype").InnerText;
                        oBOField.DataType = DataTypeDictionary[oBOField.sXMLDataType];
                        var values = Enum.GetValues(typeof(BODatatypes));
                        string sDatatype = oBOField.DataType;
                        int key = 0;
                        key = (int)Enum.Parse(typeof(BODatatypes), sDatatype);
                        oBOField.TypeID = key;
                        oBOField.sPassword = Attr.SelectSingleNode("password") == null ? "20" : (Attr.SelectSingleNode("password").InnerText == "zYes" ? "10" : "20");
                        // oBOField.multiline = Attr.SelectSingleNode("multiline").InnerText;
                        oBOField.MaxLength = Attr.SelectSingleNode("length") == null ? null : (Convert.ToInt32(Attr.SelectSingleNode("length").InnerText) > 4000 ? "max" : Attr.SelectSingleNode("length").InnerText);
                        oBOField.sPrecision = Attr.SelectSingleNode("precision") == null ? null : Attr.SelectSingleNode("precision").InnerText;
                        // oeld.optional = Attr.SelectSingleNode("optional").InnerText;
                        //oBOField.searchmethod = Attr.SelectSingleNode(BOFi"searchmethod").InnerText;
                        //oBOField.column = Attr.SelectSingleNode("column") == null ? null : Attr.SelectSingleNode("column").InnerText; ;
                        oBOField.sVirtualColumn = Attr.SelectSingleNode("virtualcolumn") == null ? "20" : (Attr.SelectSingleNode("virtualcolumn").InnerText == "zYes" ? "10" : "20");
                        oBOField.IsNull = Attr.SelectSingleNode("null") == null ? false : (Attr.SelectSingleNode("null").InnerText == "zYes" ? true : false);

                        //oBOField.IsNull = false;
                        oBOField.sLock = Attr.SelectSingleNode("lock") == null ? "20" : Attr.SelectSingleNode("lock").InnerText == "zYes" ? "10" : "20";
                        oBOField.sCase = Attr.SelectSingleNode("case") == null ? "0" : (Attr.SelectSingleNode("case").InnerText == "zLower" ? "10" : Attr.SelectSingleNode("case").InnerText == "zUpper" ? "20" : "30");
                        oBOField.ForeignTable = Attr.SelectSingleNode("foreignkey") == null ? null : Attr.SelectSingleNode("foreignkey").InnerText;
                        string sForeignTable = Attr.SelectSingleNode("foreignkey") == null ? null : Attr.SelectSingleNode("foreignkey").InnerText;
                        oBOField.FKTableName = "";
                        if (sForeignTable != null)
                        {
                            string sTargetFile = System.Web.HttpContext.Current.Server.MapPath("~/Content/BOExtracted/" + sForeignTable + ".xml");
                            //var AppPath = ConfigurationManager.AppSettings["ApplicationPath"];
                            //string sTargetFile = AppPath+"//Content//BOExtracted//" + sForeignTable + ".xml";
                            if (File.Exists(sTargetFile))
                            {
                                XmlDocument xmlDoc = new XmlDocument();
                                xmlDoc.Load(sTargetFile);
                                XmlNodeList xnList = xmlDoc.SelectNodes("entity");
                                foreach (XmlNode xn in xnList)
                                {
                                    oBOField.FKTableName = xn.SelectSingleNode("table").InnerText;
                                }
                            }
                        }
                        oBOField.ForeignColumn = Attr.SelectSingleNode("column") == null ? null : Attr.SelectSingleNode("column").InnerText;
                        oBOField.iOutputLength = Convert.ToInt32(Attr.SelectSingleNode("outputlength") == null ? null : Attr.SelectSingleNode("outputlength").InnerText);
                        //Option list in the attributes
                        var sOptList = Attr.SelectSingleNode("list");
                        List<BOOptionLists> oBOOptnLst = new List<BOOptionLists>();
                        if (sOptList != null)
                        {
                            foreach (XmlNode Optn in sOptList)
                            {
                                BOOptionLists oBOOptn = new BOOptionLists();
                                oBOOptn.Name = oBOField.Name;
                                oBOOptn.sValues = Optn.SelectSingleNode("value").InnerText;
                                XmlNodeList OptLabel = Optn.SelectNodes("label");
                                foreach (XmlNode labl in OptLabel)
                                {
                                    XmlNode EN = labl.SelectSingleNode("EN");
                                    if (EN != null)
                                    {
                                        oBOOptn.sOptionName = EN.SelectSingleNode("label").InnerText;
                                        //oBOField.Description = EN.SelectSingleNode("description").InnerText;
                                    }
                                }
                                oBOOptnLst.Add(oBOOptn);
                                // oBOField.AttrBOOptionList.Add(oBOOptn);
                            }
                            oBOField.AttrBOOptionList = oBOOptnLst;
                        }
                        else
                        {
                            //Do nothing
                        }
                        oBOField.IsVisible = true;
                        if (oBOField.Name.ToLower() == "id")
                        {
                            oBOField.LabelName = "ID";
                        }
                        if (oBOField.Name.Contains(" "))
                        {
                            oBOField.Name = oBOField.Name.Replace(" ", "_");
                        }
                        if (oBOField.Name.Contains("/"))
                        {
                            oBOField.Name = oBOField.Name.Replace("/", "_");
                        }
                        //Seperate BOfeilds with and with out option list
                        if (oBOOptnLst.Count == 0)
                        {
                            if (!oBOField.Name.StartsWith("_"))
                            {
                                BOFieldWithoutOption.Add(oBOField);
                            }
                            else
                            {

                            }
                        }
                        else
                        {
                            if (!oBOField.Name.StartsWith("_"))
                            {
                                BOFieldwithOption.Add(oBOField);
                            }
                            else
                            {

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var k = i;
            }
            oBODef.BOFieldsWithOpt = BOFieldwithOption;
            oBODef.BOFields = BOFieldWithoutOption;
            return oBODef;
        }

        private List<BOGroupFields> GetAttibuteGroups(XmlNode AttributeGroups)
        {
            List<BOGroupFields> BOGrpFields = new List<BOGroupFields>();
            XmlNodeList AttributeGroup = AttributeGroups.SelectNodes("attributegroup");
            foreach (XmlNode AttrGrp in AttributeGroup)
            {
                BOGroupFields oBOGrp = new BOGroupFields();
                oBOGrp.GroupName = AttrGrp.SelectSingleNode("name").InnerText;
                XmlNodeList Attributes = AttrGrp.SelectNodes("attributes");
                foreach (XmlNode Attr in Attributes)
                {
                    string sAttrNms = "";
                    XmlNodeList AttrNme = Attr.SelectNodes("attribute");
                    foreach (XmlNode Att in AttrNme)
                    {

                        string sAttrNme = Att.InnerText;
                        if (!sAttrNme.StartsWith("_"))
                        {
                            if (sAttrNme == "XICreatedBy" || sAttrNme == "XICreatedWhen" || sAttrNme == "XIUpdatedBy" || sAttrNme == "XIUpdatedWhen")
                            {

                            }
                            else
                            {
                                if (sAttrNms == "")
                                {
                                    sAttrNms = sAttrNme;
                                }
                                else
                                {
                                    sAttrNms = sAttrNms + ", " + sAttrNme;
                                }
                            }
                        }
                    }
                    oBOGrp.BOFieldNames = sAttrNms;
                }

                BOGrpFields.Add(oBOGrp);

            }
            return BOGrpFields;
        }

        private string SyncTable(string sTableName, List<BOFields> BOFields, string sOrgDB, string sDatabase)
        {
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["XIDNAClientDbContext"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            var NewTableColDetails = new List<string>();
            var ExistingTblColDetails = new List<string>();
            //string sTableName = "";
            var bIsPrimaryAdded = false;
            foreach (var BOdetails in BOFields)
            {
                //sTableName = BOdetails.BOName;
                string sColumnName = BOdetails.Name;
                string sOldDatatype = BOdetails.sXMLDataType;
                string sNewDataType = "";
                //string sForgnColmn = "";
                //string sForgnTable = "";
                string sLength = BOdetails.MaxLength;
                bool isNull = BOdetails.IsNull;
                string sNull = "";
                if (isNull == true)
                {
                    sNull = "NULL";
                }
                else
                {
                    sNull = "NULL";
                }
                //Match the datatype details                                  
                if (sOldDatatype == "zAutomatic")
                {
                    if (!bIsPrimaryAdded)
                    {
                        sNewDataType = "INT IDENTITY(1,1) PRIMARY KEY";
                        sNull = "NOT NULL";
                        bIsPrimaryAdded = true;
                    }
                    else
                    {
                        sNewDataType = "INT";
                        sNull = "NULL";
                    }
                }
                else if (sOldDatatype == "zString")
                {
                    sNewDataType = "VARCHAR(" + sLength + ")";
                }
                else if (sOldDatatype == "zLong")
                {
                    sNewDataType = "BIGINT";
                }
                else if (sOldDatatype == "zDouble")
                {
                    sNewDataType = "FLOAT";
                }
                else if (sOldDatatype == "zBoolean")
                {
                    sNewDataType = "BIT";
                }
                else if (sOldDatatype == "zDate")
                {
                    sNewDataType = "DATETIME";
                }
                else if (sOldDatatype == "zTimestamp")
                {
                    sNewDataType = "DATETIME";
                }
                else if (sOldDatatype == "zTime")
                {
                    sNewDataType = "DATETIME";
                }
                else if (sOldDatatype == "zExpression")
                {
                    sNewDataType = "VARCHAR(MAX)";
                }
                //Check if the table exists
                int iExists = 0;
                using (SqlConnection SC = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
                {
                    string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + sTableName + "') SELECT 1 ELSE SELECT 0";
                    SC.Open();
                    SC.ChangeDatabase(sOrgDB);
                    SqlCommand DateCheck = new SqlCommand(cmdText, SC);
                    iExists = Convert.ToInt32(DateCheck.ExecuteScalar());
                    var lColDatatypes = new List<List<string>>();
                    SC.Close();
                }
                if (iExists == 0)
                {
                    NewTableColDetails.Add(sColumnName + " " + sNewDataType + " " + sNull);
                }
                else
                {
                    //check column added
                    string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG='" + sOrgDB + "' AND TABLE_NAME='" + sTableName + "' AND COLUMN_NAME ='" + sColumnName + "') SELECT 0 ELSE SELECT 1";
                    Conn.Open();
                    Conn.ChangeDatabase(sOrgDB);
                    SqlCommand Sqlcmd = new SqlCommand(sCheckColmn, Conn);
                    int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                    Conn.Close();
                    if (iExist == 0)
                    {
                        ExistingTblColDetails.Add(sColumnName + " " + sNewDataType + " " + sNull);
                    }
                }
            }
            //Build query
            if (NewTableColDetails.Count != 0)
            {
                string sColumnDetails = string.Join(",", NewTableColDetails).ToString();
                string sQuery = "create table [" + sTableName + "] (" + sColumnDetails + ")";
                Conn.Open();
                Conn.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand(sQuery, Conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception Ex)
                {

                }
                Conn.Close();
            }
            else
            {
                //do nothing
            }

            if (ExistingTblColDetails.Count != 0)
            {
                string sColumnDetails = string.Join(",", ExistingTblColDetails).ToString();
                string sQuery = "alter table [" + sTableName + "] add " + sColumnDetails;
                Conn.Open();
                Conn.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand(sQuery, Conn);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception Ex)
                {

                }
                Conn.Close();
            }
            else
            {
                //Do Nothing;
            }
            return null;
        }

        #endregion ImportBO
        #region EditableGrid
        public int DeleteFormData(int iBOID, string BOName, string Group, int iInstanceID, string sVisualisation, int iUserID, string sOrgName, string sDatabase, List<cNameValuePairs> nWCParams)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDb);//Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            string sTableName = dbContext.BOs.Where(m => m.BOID == iBOID).Select(m => m.TableName).FirstOrDefault();
            //check the number of columns as the table cannot exist with no columns
            int iColCount = 0; int iStatus = 0;
            using (SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase)))
            {
                Conn.Open();
                SqlCommand SqlCmd = new SqlCommand();
                SqlCmd.Connection = Conn;
                Conn.ChangeDatabase(sOrgDb);
                SqlCmd.CommandText = "SELECT COUNT(COLUMN_NAME) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME= '" + sTableName + "'";
                SqlDataReader DReader = SqlCmd.ExecuteReader();
                while (DReader.Read())
                {
                    iColCount = DReader.GetInt32(0);
                }
                Conn.Close();
                if (iColCount > 1)
                {
                    if (iInstanceID != 0)
                    {

                        string cmdText = @"IF EXISTS(SELECT * FROM " + sTableName + " where id=" + iInstanceID + ") SELECT 1 ELSE SELECT 0";
                        Conn.Open();
                        Conn.ChangeDatabase(sOrgDb);
                        SqlCommand DateCheck = new SqlCommand(cmdText, Conn);
                        int x = Convert.ToInt32(DateCheck.ExecuteScalar());
                        var lColDatatypes = new List<List<string>>();
                        Conn.Close();
                        if (x == 1)
                        {
                            try
                            {
                                string Query1 = "delete from " + sTableName + " where ID=" + iInstanceID + "";

                                Spdb.Database.ExecuteSqlCommand(Query1);
                                iStatus = 1;
                            }
                            catch (Exception ex)
                            {
                                iStatus = 0;
                            }
                        }
                        else
                        {
                            iStatus = 0;
                        }

                    }
                    else
                    {
                        iStatus = 0;
                    }
                }
                else
                {
                    iStatus = 0;
                }
            }
            return iStatus;
        }
        #endregion

        #region Data Source
        public DTResponse GetDataSource(jQueryDataTableParamModel param, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            var iOrgID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiOrgID;
            IQueryable<XIDataSources> AllXIDataSources;
            AllXIDataSources = dbContext.XIDataSources.Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0);
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllXIDataSources = AllXIDataSources.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllXIDataSources.Count();
            AllXIDataSources = QuerableUtil.GetResultsForDataTables(AllXIDataSources, "", sortExpression, param);
            var clients = AllXIDataSources.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sName,c.sDescription,c.sConnectionString, c.StatusTypeID.ToString(),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public XIDataSources GetDataSourceDetails(int ID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XIDataSources oDataSources = new XIDataSources();
            oDataSources = dbContext.XIDataSources.Where(m => m.ID == ID).FirstOrDefault();
            oDataSources.sConnectionString = oXIAPI.DecryptData(oDataSources.sConnectionString, true, oDataSources.ID.ToString());
            return oDataSources;
        }

        public VMCustomResponse CreateDataSource(XIDataSources model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext("XIEnvironment");
            XIDataSources oDataSource = new XIDataSources();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var FKiAppID = UserDetails.FKiApplicationID;
            var iOrgID = UserDetails.FKiOrgID;
            if (model.ID == 0)
            {
                oDataSource.sName = model.sName;
                oDataSource.sType = model.sType;
                oDataSource.FKiApplicationID = FKiAppID;
                oDataSource.FKiOrgID = iOrgID;
                oDataSource.sDescription = model.sDescription;
                oDataSource.StatusTypeID = model.StatusTypeID;
                oDataSource.CreatedBy = oDataSource.UpdatedBy = model.CreatedBy;
                oDataSource.CreatedTime = oDataSource.UpdatedTime = DateTime.Now;
                oDataSource.CreatedBySYSID = oDataSource.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.XIDataSources.Add(oDataSource);
                dbContext.SaveChanges();
                var sEnrypted = oXIAPI.EncryptData(model.sConnectionString, true, oDataSource.ID.ToString());
                oDataSource = dbContext.XIDataSources.Find(oDataSource.ID);
                oDataSource.sConnectionString = sEnrypted;
                dbContext.SaveChanges();
            }
            else
            {
                oDataSource = dbContext.XIDataSources.Find(model.ID);
                oDataSource.sName = model.sName;
                oDataSource.sType = model.sType;
                oDataSource.FKiApplicationID = model.FKiApplicationID;
                oDataSource.FKiOrgID = model.FKiOrgID;
                oDataSource.sDescription = model.sDescription;
                oDataSource.StatusTypeID = model.StatusTypeID;
                oDataSource.sConnectionString = model.sConnectionString;
                oDataSource.UpdatedBy = model.UpdatedBy;
                oDataSource.UpdatedTime = DateTime.Now;
                oDataSource.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.SaveChanges();
                var sEnrypted = oXIAPI.EncryptData(model.sConnectionString, true, oDataSource.ID.ToString());
                oDataSource = dbContext.XIDataSources.Find(model.ID);
                oDataSource.sConnectionString = sEnrypted;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = oDataSource.ID, Status = true };
            //return oDataSource.ID;
        }

        public List<VMDropDown> GetDataSources(int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllDataSources = new List<VMDropDown>();
            //var DataSourceDetails = dbContext.XIDataSources.Where(m => m.sType.ToLower() == "defined").ToList();
            var DataSourceDetails = dbContext.XIDataSources.ToList();
            foreach (var item in DataSourceDetails)
            {
                AllDataSources.Add(new VMDropDown
                {
                    text = item.sName,
                    Value = item.ID
                });
            }
            AllDataSources.Insert(0, new VMDropDown
            {
                text = "Organisation DB",
                Value = -2
            });
            AllDataSources.Insert(0, new VMDropDown
            {
                text = "Application DB",
                Value = -1
            });
            return AllDataSources;
        }
        public string CheckConnectionString(string sConnectionString)
        {
            string sStatus = "Success";
            using (SqlConnection conn = new SqlConnection(sConnectionString))
            {
                try
                {
                    conn.Open(); // throws if invalid
                    conn.Close();
                }
                catch (Exception Ex)
                {
                    sStatus = "Failure";
                }

            }
            return sStatus;
        }

        public List<VMDropDown> GetAppOrganisations(int iAppID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var oApp = dbContext.XIApplications.Find(iAppID);
            var sDatabase = oApp.sDatabaseName;
            List<VMDropDown> Orgs = new List<VMDropDown>();
            if (!string.IsNullOrEmpty(sDatabase))
            {
                dbContext = new ModelDbContext(sDatabase);
                Orgs = dbContext.Organization.ToList().Select(m => new VMDropDown { Value = m.ID, text = m.Name }).ToList();
            }
            return Orgs;
        }

        #endregion Data Source

        #region BOXIStructure

        public DTResponse GetBOStructuresList(jQueryDataTableParamModel param, int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            IQueryable<cXIStructure> AllBOStructures;
            AllBOStructures = dbContext.XIStructure.Where(m => m.BOID == BOID && m.FKiParentID == "#");
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllBOStructures = AllBOStructures.Where(m => m.sName.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllBOStructures.Count();
            AllBOStructures = QuerableUtil.GetResultsForDataTables(AllBOStructures, "", sortExpression, param);
            var clients = AllBOStructures.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID), c.sStructureName, c.sCode, Convert.ToString(c.StatusTypeID),""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<cXIStructure> SaveBODetailsToXIStructure(int iBOID, string BOName, int iUserID, string sOrgName, string sDatabase)
        {
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            ModelDbContext dbContext = new ModelDbContext();
            List<BOFields> BOFldList = new List<BOFields>();
            List<cXIStructure> OXIStrList = new List<cXIStructure>();
            OXIStrList = dbContext.XIStructure.Where(m => m.sBO == BOName).ToList();
            var oBODef = oXIAPI.Get_BOInstance(BOName, "", iUserID, sOrgName, sDatabase).Definition;
            iBOID = oBODef.BOID;
            BOFldList = dbContext.BOFields.Where(m => m.FKTableName == BOName).ToList();
            if (OXIStrList.Count() == 0)
            {
                long iXIStrID = 0;
                //Create new
                cXIStructure oXIs = new cXIStructure();
                oXIs.sName = oBODef.LabelName;
                oXIs.FKiParentID = "#";
                oXIs.sBO = oBODef.Name;
                oXIs.BOID = oBODef.BOID;
                oXIs.FKi1ClickID = 0;
                oXIs.FKiXIApplicationID = 0;
                oXIs.bMasterEntity = true;
                oXIs.StatusTypeID = 10;
                oXIs.CreatedBy = iUserID;
                oXIs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIs.CreatedTime = DateTime.Now;
                oXIs.UpdatedBy = iUserID;
                oXIs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIs.UpdatedTime = DateTime.Now;
                dbContext.XIStructure.Add(oXIs);
                dbContext.SaveChanges();
                iXIStrID = oXIs.ID;
                //Add Foriegn Key details

                foreach (var BOdetails in BOFldList)
                {
                    Countdata(iXIStrID, BOdetails, iUserID, sDatabase);
                }
            }
            else
            {
                //checked if foriegn key already added.
                long iXSID = dbContext.XIStructure.Where(m => m.sName == BOName).Where(m => m.FKiParentID == "#").Select(m => m.ID).FirstOrDefault();
                if (iXSID != 0)
                {
                    //Update or add
                    foreach (var BOdetails in BOFldList)
                    {
                        Countdata(iXSID, BOdetails, iUserID, sDatabase);
                    }
                }
            }
            List<cXIStructure> Tree = GetXIStructureTreeDetails(iBOID, "0", sDatabase);
            return Tree;
        }


        private string Countdata(long iXIStrID, BOFields BOdetails, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int BOID = BOdetails.BOID;
            BOs oBO = dbContext.BOs.Find(BOID);
            if (oBO != null)
            {
                string sPrntID = iXIStrID.ToString();
                cXIStructure XIs = dbContext.XIStructure.Where(m => m.sName == oBO.Name).Where(m => m.FKiParentID == sPrntID).FirstOrDefault();

                if (XIs == null)
                {
                    cXIStructure oXIs = new cXIStructure();
                    oXIs.sName = oBO.LabelName;
                    oXIs.FKiParentID = iXIStrID.ToString();
                    oXIs.sBO = oBO.Name;
                    oXIs.BOID = oBO.BOID;
                    oXIs.FKi1ClickID = 0;
                    oXIs.FKiXIApplicationID = 0;
                    oXIs.bMasterEntity = false;
                    oXIs.StatusTypeID = 10;
                    oXIs.CreatedBy = iUserID;
                    oXIs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oXIs.CreatedTime = DateTime.Now;
                    oXIs.UpdatedBy = iUserID;
                    oXIs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oXIs.UpdatedTime = DateTime.Now;
                    dbContext.XIStructure.Add(oXIs);
                    dbContext.SaveChanges();
                    iXIStrID = oXIs.ID;
                    //List<BOFields> BOFldList = dbContext.BOFields.Where(m => m.BOID == oBO.BOID).Where(m => m.FKTableName != null).ToList();
                    //if (BOFldList.Count() > 0)
                    //{
                    //    foreach (var BOdetail in BOFldList)
                    //    {
                    //        var XIStr = Countdata(iXIStrID, BOdetail, iUserID, sDatabase);
                    //    }
                    //}
                }
                else
                {
                    iXIStrID = XIs.ID;
                    //List<BOFields> BOFldList = dbContext.BOFields.Where(m => m.BOID == oBO.BOID).Where(m => m.FKTableName != null).ToList();
                    //if (BOFldList.Count() > 0)
                    //{
                    //    foreach (var BOdetail in BOFldList)
                    //    {
                    //        var XIStr = Countdata(iXIStrID, BOdetail, iUserID, sDatabase);
                    //    }
                    //}
                }
            }
            else
            {
                //BO dosnot exists for this
            }

            return null;
        }

        public List<cXIStructure> GetBOStructureTree(int iBOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<BOFields> BOFldList = new List<BOFields>();
            var oAllBOs = dbContext.BOs.ToList();
            var oBO = oAllBOs.Where(m => m.BOID == iBOID).FirstOrDefault();
            BOFldList = dbContext.BOFields.Where(m => m.FKTableName == oBO.TableName).ToList();
            List<cXIStructure> oTree = new List<cXIStructure>();
            oTree.Add(new cXIStructure { ID = iBOID, FKiParentID = "#", sName = oBO.LabelName, sBO = oBO.Name, BOID = iBOID });
            foreach (var items in BOFldList)
            {
                var BO = oAllBOs.Where(m => m.BOID == items.BOID).FirstOrDefault();
                if (BO != null)
                {
                    if (oTree.Where(m => m.ID == items.BOID).FirstOrDefault() == null)
                    {
                        if (BO.sType != EnumBOTypes.Reference.ToString() && BO.sType != EnumBOTypes.Enum.ToString() && BO.sType != EnumBOTypes.XISystem.ToString() && BO.sType != EnumBOTypes.Technical.ToString())
                        {
                            cXIStructure oStru = new cXIStructure();
                            oStru.ID = items.BOID;
                            oStru.sBO = BO.Name;
                            oStru.sName = BO.LabelName;
                            oStru.FKiParentID = iBOID.ToString();
                            oStru.BOID = BO.BOID;
                            oTree.Add(oStru);
                        }
                    }
                }
            }
            return oTree;
        }

        //Get details on node
        public List<cXIStructure> GetXIStructureTreeDetails(int BOID, string StructureUID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<cXIStructure> XIStrTrees = new List<cXIStructure>();
            List<cXIStructure> Tree = new List<cXIStructure>();
            int iStrID = 0;
            Guid StrXIGUID = Guid.Empty;
            int.TryParse(StructureUID, out iStrID);
            Guid.TryParse(StructureUID, out StrXIGUID);
            //XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == iMainID.ToString()).ToList();
            if (iStrID > 0)
            {
                XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == "#" && m.ID == iStrID).ToList();
            }
            else if(StrXIGUID != null && StrXIGUID != Guid.Empty)
            {
                XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == "#" && m.XIGUID == StrXIGUID).ToList();
            }
            else
            {
                XIStrTrees = dbContext.XIStructure.Where(m => m.FKiParentID == "#" && m.BOID == BOID).ToList();
            }
            foreach (var item in XIStrTrees)
            {
                item.FKiStepDefinitionName = dbContext.QSStepDefiniton.Where(m => m.ID == item.FKiStepDefinitionID).Select(m => m.sName).FirstOrDefault();
            }
            //XIStrTrees.FirstOrDefault().FKiStepDefinitionName = dbContext.QSStepDefiniton.Where(m => m.ID == XIStrTrees.FirstOrDefault().FKiStepDefinitionID).Select(m => m.sName).FirstOrDefault();
            //XIStrTrees.Add(XIParntTrees);
            if (XIStrTrees != null && XIStrTrees.Count() > 0)
            {
                Tree = XITree(XIStrTrees, sDatabase, new List<cXIStructure>());
            }
            else
            {
                Tree = new List<cXIStructure>();
            }
            // XIStrTrees.Add(XIParntTrees);
            return Tree;
        }

        public List<cXIStructure> XITree(List<cXIStructure> XIStrTrees, string sDatabase, List<cXIStructure> Tree)
        {
            ModelDbContext dbContext = new ModelDbContext();
            //List<cXIStructure> Strut = new List<cXIStructure>();
            foreach (var items in XIStrTrees)
            {
                var StrDetail = dbContext.XIStructureDetails.Where(m => m.FKiStructureIDXIGUID == items.XIGUID).FirstOrDefault();
                if(StrDetail != null)
                {
                    items.StructureDetail = StrDetail;
                }
                Tree.Add(items);
                var ID = items.ID;
                var SubXITreeNodes = dbContext.XIStructure.Where(m => m.FKiParentIDXIGUID == items.XIGUID).OrderBy(m => m.iOrder).ToList();
                if (SubXITreeNodes.Count() > 0)
                {
                    XITree(SubXITreeNodes, sDatabase, Tree);
                }
            }
            return Tree;
        }

        public VMCustomResponse SaveBOStructure(List<cXIStructure> model, int iStructureID, string sSavingType, int iUserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<cXIStructure> Tree = new List<cXIStructure>();
            string ParentID = "0";
            if (model.FirstOrDefault().sSavingType == "Edit")
            {
                Tree = GetXIStructureTreeDetails(model.FirstOrDefault().BOID, iStructureID.ToString(), sDatabase);
                if (Tree.Count() > 0)
                {
                    ParentID = Tree.FirstOrDefault().ID.ToString();
                }
            }
            var New = model.Select(m => m.BOID).ToList().Except(Tree.Select(m => m.BOID).ToList()).ToList();
            var Delete = Tree.Select(m => m.BOID).ToList().Except(model.Select(m => m.BOID).ToList()).ToList();

            foreach (var items in Delete)
            {
                var ID = Tree.Where(m => m.BOID == items).Select(m => m.ID).FirstOrDefault();
                var Node = dbContext.XIStructure.Find(ID);
                dbContext.XIStructure.Remove(Node);
                dbContext.SaveChanges();
            }
            var Add = model.Where(m => New.Contains(m.BOID)).ToList();
            var Update = model.Where(m => !New.Contains(m.BOID)).ToList();
            var MaxOrder = 0;
            foreach (var items in Add)
            {
                BOs oBO = new BOs();
                if (items.BOID > 0)
                {
                    oBO = dbContext.BOs.Find(items.BOID);
                }
                else
                {
                    oBO = dbContext.BOs.Where(m => m.Name == items.sName).FirstOrDefault();
                }
                //if (oBO != null)
                //{
                cXIStructure oXIs = new cXIStructure();
                oXIs.sStructureName = model.FirstOrDefault().sStructureName;
                oXIs.sCode = model.FirstOrDefault().sCode;
                oXIs.sName = items.sName;
                if (ParentID != "0")
                {
                    var Nodes = dbContext.XIStructure.Where(m => m.FKiParentID == ParentID).ToList();
                    if (Nodes != null && Nodes.Count() > 0)
                    {
                        MaxOrder = Nodes.Select(m => m.iOrder).Max();
                        MaxOrder = MaxOrder + 1;
                    }
                    else
                    {
                        MaxOrder = MaxOrder + 1;
                    }
                    //.Select(m => m.iOrder).Max();
                }
                else
                {
                    MaxOrder = MaxOrder + 1;
                }
                if (oBO != null)
                {
                    oXIs.sBO = oBO.Name;
                    oXIs.BOID = oBO.BOID;
                }
                oXIs.FKi1ClickID = 0;
                oXIs.FKiXIApplicationID = 0;
                if (items.FKiParentID == "#")
                {
                    oXIs.bMasterEntity = true;
                    oXIs.FKiParentID = items.FKiParentID;
                    oXIs.iOrder = 0;
                }
                else
                {
                    oXIs.bMasterEntity = false;
                    oXIs.FKiParentID = ParentID;
                    oXIs.iOrder = MaxOrder;
                }
                oXIs.sType = items.sType;
                oXIs.sMode = items.sMode;
                oXIs.bIsVisible = items.bIsVisible;
                oXIs.sLinkingType = items.sLinkingType;
                oXIs.sParentFKColumn = items.sParentFKColumn;
                oXIs.FKiStepDefinitionID = items.FKiStepDefinitionID;
                oXIs.sOutputArea = items.sOutputArea;
                oXIs.StatusTypeID = items.StatusTypeID;
                oXIs.CreatedBy = iUserID;
                oXIs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIs.CreatedTime = DateTime.Now;
                oXIs.UpdatedBy = iUserID;
                oXIs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oXIs.UpdatedTime = DateTime.Now;
                dbContext.XIStructure.Add(oXIs);
                dbContext.SaveChanges();
                if (items.FKiParentID == "#")
                {
                    ParentID = oXIs.ID.ToString();
                }
                //}
            }

            foreach (var items in Update)
            {
                BOs oBO = new BOs();
                if (items.BOID > 0)
                {
                    oBO = dbContext.BOs.Find(items.BOID);
                }
                else
                {
                    oBO = dbContext.BOs.Where(m => m.LabelName == items.sName).FirstOrDefault();
                }
                var oStruct = dbContext.XIStructure.Find(items.ID);
                oStruct.sName = items.sName;
                if (oBO != null)
                {
                    oStruct.sBO = oBO.Name;
                    oStruct.BOID = oBO.BOID;
                }
                oStruct.sStructureName = model.FirstOrDefault().sStructureName;
                oStruct.sCode = model.FirstOrDefault().sCode;
                oStruct.sType = items.sType;
                oStruct.sMode = items.sMode;
                oStruct.bIsVisible = items.bIsVisible;
                oStruct.sLinkingType = items.sLinkingType;
                oStruct.sParentFKColumn = items.sParentFKColumn;
                oStruct.FKiStepDefinitionID = items.FKiStepDefinitionID;
                oStruct.sOutputArea = items.sOutputArea;
                oStruct.StatusTypeID = items.StatusTypeID;
                dbContext.SaveChanges();

            }
            return new VMCustomResponse() { ID = 1, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        public long CreateAndSaveTreeNode(string ParentNode, string NodeID, string NodeTitle, int iUserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var Parent = dbContext.XIStructure.Find(Convert.ToInt64(ParentNode));
            cXIStructure oXIs = new cXIStructure();
            oXIs.sName = NodeTitle;
            oXIs.FKiParentID = ParentNode;
            oXIs.sStructureName = Parent.sStructureName;
            oXIs.sCode = Parent.sCode;
            oXIs.FKi1ClickID = 0;
            oXIs.FKiXIApplicationID = 0;
            oXIs.StatusTypeID = 10;
            oXIs.bMasterEntity = false;
            oXIs.CreatedTime = DateTime.Now;
            oXIs.CreatedBy = 1;
            oXIs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oXIs.UpdatedTime = DateTime.Now;
            oXIs.UpdatedBy = 1;
            oXIs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.XIStructure.Add(oXIs);
            dbContext.SaveChanges();
            return oXIs.ID;
        }
        public int DragAndDropNodes(string NodeID, string OldParentID, int UserID, string sDatabase, int Oldposition, int Newposition)
        {
            int TabID = 0;
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            int iGetid = Convert.ToInt32(OldParentID);
            cXIStructure DAndNodeTree = dbContext.XIStructure.Find(iGetid);
            int NoOfPostionsChanged = Oldposition - Newposition;
            if (DAndNodeTree != null)
            {
                int iNodeID = Convert.ToInt32(NodeID);
                cXIStructure tab = dbContext.XIStructure.Find(iNodeID);
                if (Oldposition > 0 && Newposition == 0)
                {
                    if (NoOfPostionsChanged > 0)
                    {
                        var NewPos = tab.iOrder - NoOfPostionsChanged;
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[XIBOStructure_T] SET [iOrder] = [iOrder] + 1  WHERE [iOrder] BETWEEN " + NewPos + " and " + tab.iOrder);
                        tab.iOrder = tab.iOrder - NoOfPostionsChanged;
                    }
                    else
                    {
                        var NewPos = tab.iOrder - NoOfPostionsChanged;
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[XIBOStructure_T] SET [iOrder] = [iOrder] - 1  WHERE [iOrder] BETWEEN " + tab.iOrder + " and " + NewPos);
                        tab.iOrder = tab.iOrder - NoOfPostionsChanged;
                    }
                }
                else
                {
                    if (NoOfPostionsChanged > 0)
                    {
                        var NewPos = tab.iOrder - NoOfPostionsChanged;
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[XIBOStructure_T] SET [iOrder] = [iOrder] + 1  WHERE [iOrder] BETWEEN " + NewPos + " and " + tab.iOrder);
                        tab.iOrder = tab.iOrder - NoOfPostionsChanged;
                    }
                    else
                    {
                        var NewPos = tab.iOrder - NoOfPostionsChanged;
                        dbContext.Database.ExecuteSqlCommand("UPDATE [dbo].[XIBOStructure_T] SET [iOrder] = [iOrder] - 1  WHERE [iOrder] BETWEEN " + tab.iOrder + " and " + NewPos);
                        tab.iOrder = tab.iOrder - NoOfPostionsChanged;
                    }
                }
                dbContext.SaveChanges();
                TabID = Convert.ToInt32(tab.ID);
            }
            return TabID;
        }
        public List<cXIStructure> DeleteNodeDetails(string ParentNode, string NodeID, string ChildrnIDs, string Type, int UserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIStructure oXIS = new cXIStructure();
            cXIStructure DelMainNode = dbContext.XIStructure.Find(Convert.ToInt32(NodeID));
            if (DelMainNode != null)
            {
                dbContext.XIStructure.Remove(DelMainNode);
                dbContext.SaveChanges();
            }
            //Delete Chidrens
            if (ChildrnIDs == "")
            {
                var GetChildIDs = new List<long>();
                GetChildIDs = dbContext.XIStructure.Where(m => m.FKiParentID == NodeID).Select(m => m.ID).ToList();
                string sChildIDs = String.Join(",", GetChildIDs.Select(x => x.ToString()).ToArray());
                ChildrnIDs = sChildIDs;

            }

            if (ChildrnIDs != "")
            {
                List<string> TargetdIDs = ChildrnIDs.Split(',').ToList();
                for (var i = 0; i < TargetdIDs.Count(); i++)
                {
                    string TrgID = TargetdIDs[i];
                    long IID = dbContext.XIStructure.Where(m => m.FKiParentID == TrgID).Select(m => m.ID).FirstOrDefault();
                    cXIStructure DelNode = dbContext.XIStructure.Find(IID);
                    dbContext.XIStructure.Remove(DelNode);
                    dbContext.SaveChanges();
                }
            }
            ////return the updated values
            List<cXIStructure> XIStructre = new List<cXIStructure>();
            XIStructre = dbContext.XIStructure.ToList();

            return XIStructre;
        }

        public int RenameTreeNode(string ParentNode, string NodeID, string NodeTitle, int UserID, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int Status = 0;
            //if (Type == "rename")
            //{
            int ID = Convert.ToInt32(NodeID);
            cXIStructure StructTree = dbContext.XIStructure.Find(ID);
            StructTree.sName = NodeTitle;
            var BO = dbContext.BOs.Where(m => m.LabelName.ToLower() == NodeTitle.ToLower()).FirstOrDefault();
            if (BO != null)
            {
                StructTree.sBO = BO.Name;
                StructTree.BOID = BO.BOID;
                dbContext.SaveChanges();
                Status = 1;
            }
            //}
            return Status;
        }
        public cXIStructure AddDetailsForStructure(string ParentNode, string NodeID, int OrgID, int UserID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIStructure oXIStrTree = new cXIStructure();
            //int RoleID = dbContext.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).FirstOrDefault();
            //List<VMDropDown> XILink = new List<VMDropDown>();
            //XILink = (from c in dbContext.XiLinks.ToList()
            //          select new VMDropDown { text = c.Name, Value = c.XiLinkID }).ToList();
            int ID = Convert.ToInt32(NodeID);
            oXIStrTree = dbContext.XIStructure.Where(m => m.ID == ID).FirstOrDefault();
            //MenuTree.VMXILink = XILink;
            //List<VMDropDown> MenuGroup = new List<VMDropDown>();
            //MenuGroup = (from c in dbContext.MenuGroups
            //             select new VMDropDown { text = c.sName, Value = c.ID }).ToList();

            //MenuTree.MenuGroup = MenuGroup;
            return oXIStrTree;
        }
        public VMCustomResponse SaveAddedDetails(int UserID, cXIStructure model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cXIStructure XISTree = dbContext.XIStructure.Find(model.ID);
            XISTree.sName = model.sName;
            XISTree.FKi1ClickID = model.FKi1ClickID;
            XISTree.sBO = model.sBO;
            XISTree.BOID = dbContext.BOs.Where(m => m.Name == model.sName).Select(m => m.BOID).FirstOrDefault();
            XISTree.FKiXIApplicationID = model.FKiXIApplicationID;
            XISTree.bMasterEntity = model.bMasterEntity;
            XISTree.StatusTypeID = model.StatusTypeID;
            dbContext.SaveChanges();
            return new VMCustomResponse() { Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }
        public cBOUIDetails GetBOUIDetails(int ID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cBOUIDetails oBOUI = new cBOUIDetails();
            var oStruct = dbContext.XIStructure.Find(ID);
            oBOUI = dbContext.BOUIDetails.Where(m => m.FKiBOID == oStruct.BOID).FirstOrDefault();
            if (oBOUI == null)
            {
                oBOUI = new cBOUIDetails();
                oBOUI.FKiBOID = oStruct.BOID;
                oBOUI.sBOName = oStruct.sBO;
            }
            else
            {
                oBOUI.FKiBOID = oBOUI.FKiBOID;
                oBOUI.sBOName = oStruct.sBO;
                oBOUI.i1ClickID = oBOUI.i1ClickID;
            }
            oBOUI.ddl1Clicks = Common.GetReportsDDL(sDatabase);
            oBOUI.ddlLayouts = Common.GetTemplateLayoutsDDL(sDatabase);
            oBOUI.ddlQSTemplates = Common.GetQSTemplatesDDL(sDatabase);
            oBOUI.ddlQSStepTemplates = Common.GetQSStepTemplatesDDL(0, sDatabase);
            return oBOUI;
        }

        //Save BO UI
        public VMCustomResponse SaveBOUI(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            VMCustomResponse Response = new VMCustomResponse();
            var Layout = dbContext.Layouts.Find(model.iLayoutID);
            if (Layout != null)
            {
                //if (Layout.LayoutName.ToLower() == "TabLayoutTemplate".ToLower())
                //{
                //    Response = SaveBOUIDetails(model, iUserID, sOrgName, sDatabase);
                //}
                if (Layout.LayoutName.ToLower() == "Left Tree Layout".ToLower())
                {
                    Response = SaveBOUIDetailsWithQuestionSet(model, iUserID, sOrgName, sDatabase);
                }
                if (Layout.LayoutName.ToLower() == "TabLayoutTemplate".ToLower())
                {
                    Response = SaveTabsBOUIDetailsWithQuestionSet(model, iUserID, sOrgName, sDatabase);
                }
            }
            else
            {
                Response = SaveBOUIDetails(model, iUserID, sOrgName, sDatabase);
            }
            return Response;
        }

        //Auto Create Popup without Question Set
        public VMCustomResponse SaveBOUIDetails(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var Tree = GetXIStructureTreeDetails(model.FKiBOID, model.FKiStructureID.ToString(), sDatabase).Where(m => m.BOID > 0).ToList();
            List<int> XILinkIDs = new List<int>();
            List<long> StructIDs = new List<long>();
            int i = 0;
            Guid iMain1Click = Guid.Empty;
            var bIsMainBODone = false;
            cLayouts oLayout = new cLayouts();
            var MainBO = Tree.FirstOrDefault().sBO;
            var RootBODef = oXIAPI.Get_BODefinition(MainBO, iUserID, sOrgName, sDatabase);
            var RootName = Tree.FirstOrDefault().sName;
            bIsMainBODone = Tree.FirstOrDefault().bIsAutoCreateDone;
            var ParentStructID = Tree.FirstOrDefault().ID;
            var BOStructIDs = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList();
            var StructDetails = dbContext.XIStructureDetails.Where(m => m.iParentStructureID == ParentStructID).ToList();
            if (model.sSavingType.ToLower() == "save" && StructDetails.Count() > 0)
            {
                var Delete = StructDetails.Select(m => m.FKiStructureID).ToList().Except(Tree.Select(m => m.ID).ToList()).ToList();
                var Add = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList().Except(StructDetails.Select(m => m.FKiStructureID).ToList()).ToList();

                if (Delete.Count() > 0)
                {
                    foreach (var items in Delete)
                    {
                        var Detail = StructDetails.Where(m => m.FKiStructureID == items).FirstOrDefault();
                        if (Detail != null)
                        {
                            //Delete Tab from Popup
                            var TabXiLink = dbContext.XiLinks.Find(Detail.iTabXiLinkID);
                            var TabXiLinkNVs = dbContext.XiLinkNVs.Where(m => m.Name == "XiLinkID" && m.Value == Detail.iTabXiLinkID.ToString()).FirstOrDefault();
                            dbContext.XiLinkNVs.Remove(TabXiLinkNVs);
                            dbContext.XiLinks.Remove(TabXiLink);

                            //Delete 1-click of tab and onclick XiLink
                            var o1Click = dbContext.Reports.Where(x => x.XIGUID == Detail.i1ClickIDXIGUID).Select(x => x).FirstOrDefault();
                            int RowXiLinkID = o1Click.RowXiLinkID;
                            var RowClickXiLink = dbContext.XiLinks.Find(RowXiLinkID);
                            var RowXiLinkNvs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == RowXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(RowXiLinkNvs);
                            dbContext.XiLinks.Remove(RowClickXiLink);
                            dbContext.Reports.Remove(o1Click);

                            //Delete Form Layouts and Details
                            var CreateLayout = dbContext.Layouts.Find(Detail.iCreateLayoutID);
                            var CreateLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == CreateLayout.ID).ToList();
                            dbContext.PopupLayoutDetails.RemoveRange(CreateLayoutDetails);
                            dbContext.Layouts.Remove(CreateLayout);
                            var EditLayout = dbContext.Layouts.Find(Detail.iEditLayoutID);
                            var EditLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == EditLayout.ID).ToList();
                            dbContext.PopupLayoutDetails.RemoveRange(EditLayoutDetails);
                            dbContext.Layouts.Remove(EditLayout);

                            //Delete Form Dialogs and Mappings
                            var CreateDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                            var CreateMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iCreateLayoutID && m.PopupID == Detail.iCreateDialogID).ToList();
                            dbContext.PopupLayoutMappings.RemoveRange(CreateMappings);
                            dbContext.Dialogs.Remove(CreateDlg);
                            var EditDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                            var EditMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iEditLayoutID && m.PopupID == Detail.iEditDialogID).ToList();
                            dbContext.PopupLayoutMappings.RemoveRange(EditMappings);
                            dbContext.Dialogs.Remove(EditDlg);

                            //Delete Form XiLinks
                            var CreateFormXiLink = dbContext.XiLinks.Find(Detail.iCreateFormXiLinkID);
                            var CreateFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iCreateFormXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(CreateFormNVs);
                            dbContext.XiLinks.Remove(CreateFormXiLink);
                            var EditFormXiLink = dbContext.XiLinks.Find(Detail.iEditFormXiLinkID);
                            var EditFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iEditFormXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(EditFormNVs);
                            dbContext.XiLinks.Remove(EditFormXiLink);

                            //Delete Structure Details
                            dbContext.XIStructureDetails.Remove(Detail);

                            dbContext.SaveChanges();
                        }
                    }

                }

                if (Add.Count() > 0 && bIsMainBODone)
                {
                    i = 1;
                    Tree = Tree.Where(m => Add.Contains(m.ID)).ToList();
                }
                else
                {
                    Tree = new List<cXIStructure>();
                }
            }

            //if (model.sSavingType == "Generate")
            //{
            foreach (var BO in Tree)
            {
                // Auto Create only if it is not created
                var oBODef = dbContext.BOs.Where(m => m.Name == BO.sBO).FirstOrDefault();//need to change to from cache
                if (oBODef != null)
                {
                    //Auto Create 1-Click with description group
                    Guid i1ClickID;//  AutoCreateOneClick(oBODef, UserDetails.FKiOrgID, sDatabase);
                    if (i == 0)
                    {
                        i1ClickID = AutoCreateOneClick(oBODef, false, null, null, BO.ID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                        iMain1Click = i1ClickID;

                    }
                    else
                    {
                        var RooTBOTable = string.Empty;
                        if (RootBODef != null)
                        {
                            RooTBOTable = RootBODef.TableName;
                        }
                        long iStructureID = BO.ID;
                        var BOFields = oBODef.BOFields.Where(m => m.FKTableName == RooTBOTable).FirstOrDefault();
                        var FKField = string.Empty;
                        if (BOFields != null)
                        {
                            FKField = BOFields.Name;
                        }
                        i1ClickID = AutoCreateOneClick(oBODef, true, FKField, MainBO, iStructureID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                    }
                    //Auto Create XiLink
                    int iXiLinkID = 0;
                    if (i == 0)
                    {
                        iXiLinkID = AutoCreateLeftPaneXiLink(oBODef.Name, ServiceConstants.Details1Group, true, sDatabase, Guid.Empty);
                    }
                    else
                    {
                        List<XiLinkNVs> NVs = new List<XiLinkNVs>();
                        NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "Search" });
                        NVs.Add(new XiLinkNVs { Name = "Output", Value = ServiceConstants.PopupOutputArea });
                        iXiLinkID = AutoCreateXiLink(oBODef.LabelName, i1ClickID, NVs, sDatabase);

                    }
                    if (i == 0)
                    {
                        XILinkIDs.Insert(0, iXiLinkID);
                        StructIDs.Insert(0, BO.ID);
                    }
                    else
                    {
                        XILinkIDs.Add(iXiLinkID);
                        StructIDs.Add(BO.ID);
                    }
                }

                i++;
                //Menu to Show Popups
            }

            if (model.sSavingType.ToLower() == "generate" || !bIsMainBODone)
            {
                //Create XiLink for Tabs with all XiLinks
                int TabXiLinkID = AutoCreateTabsXiLink(Tree.FirstOrDefault().sName, XILinkIDs, StructIDs, Tree.FirstOrDefault().ID, sDatabase);
                var Layout = dbContext.Layouts.Where(m => m.LayoutName == ServiceConstants.PopupLayoutName.ToString()).FirstOrDefault();
                //Create Dialog for Popup
                int iDialgID = AutoCreateDialog(Tree.FirstOrDefault().sName.Replace("_T", ""), Layout.ID, 0, sDatabase, UserDetails.FKiApplicationID);

                oLayout = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.PopupLayoutName.ToLower()).FirstOrDefault();
                //Map XiLink to Layout
                AutoMapXiLinkstoLayout(ServiceConstants.PopupLayoutName, ServiceConstants.PopupTabArea, iDialgID, TabXiLinkID, sDatabase);

                int LeftPaneXiLinkID = AutoCreateLeftPaneXiLink(Tree.FirstOrDefault().sBO, ServiceConstants.DescriptionGroup, false, sDatabase, Guid.Empty);


                AutoMapXiLinkstoLayout(ServiceConstants.PopupLayoutName, ServiceConstants.PopupLeftPaneArea, iDialgID, LeftPaneXiLinkID, sDatabase);
                // Create Dialog XILink and Map to 1-Click
                AutoCreateDialogXiLinkMapTOCLick(Tree.FirstOrDefault().sName, iMain1Click, iDialgID, sDatabase);
            }
            else
            {
                if (Tree.Count() > 0)
                {
                    var XILinkName = RootName + " Tabs XiLink".ToLower();
                    var TabXiLink = dbContext.XiLinks.Where(m => m.Name.ToLower() == XILinkName).FirstOrDefault();
                    if (XILinkIDs.Count() > 0)
                    {
                        foreach (var id in XILinkIDs)
                        {
                            XiLinkNVs oNv = new XiLinkNVs();
                            oNv.XiLinkID = TabXiLink.XiLinkID;
                            oNv.Name = "XiLinkID";
                            oNv.Value = id.ToString();
                            oNv.CreatedTime = DateTime.Now;
                            oNv.CreatedBy = 1;
                            oNv.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                            oNv.UpdatedTime = DateTime.Now;
                            oNv.UpdatedBy = 1;
                            oNv.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                            dbContext.XiLinkNVs.Add(oNv);
                            dbContext.SaveChanges();
                        }
                    }
                    for (int j = 0; j < StructIDs.Count(); j++)
                    {
                        long StructID = StructIDs[i];
                        var oStruct = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == StructID).FirstOrDefault();
                        if (oStruct != null)
                        {
                            oStruct.iParentStructureID = Tree.FirstOrDefault().ID;
                            oStruct.iTabXiLinkID = XILinkIDs[i];
                            dbContext.SaveChanges();
                        }
                    }
                }

            }


            //}

            foreach (var items in Tree.Where(m => m.bIsAutoCreateDone == false).ToList())
            {
                var oBO = dbContext.XIStructure.Find(items.ID);
                oBO.bIsAutoCreateDone = true;
                dbContext.SaveChanges();
            }

            cBOUIDetails oBOUI = new cBOUIDetails();
            oBOUI.FKi1ClickIDXIGUID = iMain1Click;
            oBOUI.FKiBOID = model.FKiBOID;
            oBOUI.FKiStructureID = Tree.FirstOrDefault().ID;
            if (oLayout != null)
            {
                oBOUI.iLayoutID = oLayout.ID;
            }
            oBOUI.StatusTypeID = 10;
            oBOUI.CreatedTime = DateTime.Now;
            oBOUI.CreatedBy = 1;
            oBOUI.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oBOUI.UpdatedTime = DateTime.Now;
            oBOUI.UpdatedBy = 1;
            oBOUI.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.BOUIDetails.Add(oBOUI);
            dbContext.SaveChanges();

            return new VMCustomResponse() { ID = oBOUI.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        //Auto Create Popup with Question Set Template and Step
        public VMCustomResponse SaveBOUIDetailsWithQuestionSet(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
            var Tree = GetXIStructureTreeDetails(model.FKiBOID, model.FKiStructureID.ToString(), sDatabase).Where(m => m.BOID > 0).ToList();
            List<int> XILinkIDs = new List<int>();
            List<long> StructIDs = new List<long>();
            int i = 0;
            int iDialgID = 0;
            Guid iMain1Click = Guid.Empty;
            int iQSDefinitionID = 0;
            var bIsMainBODone = false;
            cLayouts oLayout = new cLayouts();
            var MainBO = Tree.FirstOrDefault().sBO;
            var RootBODef = oXIAPI.Get_BODefinition(MainBO, iUserID, sOrgName, sDatabase);
            var RootName = Tree.FirstOrDefault().sName;
            bIsMainBODone = Tree.FirstOrDefault().bIsAutoCreateDone;
            var ParentStructID = Tree.FirstOrDefault().ID;
            var BOStructIDs = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList();
            var StructDetails = dbContext.XIStructureDetails.Where(m => m.iParentStructureID == ParentStructID).ToList();
            if (model.sSavingType.ToLower() == "save" && StructDetails.Count() > 0)
            {
                var Delete = StructDetails.Select(m => m.FKiStructureID).ToList().Except(Tree.Select(m => m.ID).ToList()).ToList();
                var Add = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList().Except(StructDetails.Select(m => m.FKiStructureID).ToList()).ToList();

                if (Delete.Count() > 0)
                {
                    foreach (var items in Delete)
                    {
                        var Detail = StructDetails.Where(m => m.FKiStructureID == items).FirstOrDefault();
                        if (Detail != null)
                        {
                            //Delete Tab from Popup
                            var TabXiLink = dbContext.XiLinks.Find(Detail.iTabXiLinkID);
                            var TabXiLinkNVs = dbContext.XiLinkNVs.Where(m => m.Name == "XiLinkID" && m.Value == Detail.iTabXiLinkID.ToString()).FirstOrDefault();
                            dbContext.XiLinkNVs.Remove(TabXiLinkNVs);
                            dbContext.XiLinks.Remove(TabXiLink);

                            //Delete 1-click of tab and onclick XiLink
                            var o1Click = dbContext.Reports.Where(x => x.XIGUID == Detail.i1ClickIDXIGUID).Select(t => t).FirstOrDefault();
                            int RowXiLinkID = o1Click.RowXiLinkID;
                            var RowClickXiLink = dbContext.XiLinks.Find(RowXiLinkID);
                            var RowXiLinkNvs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == RowXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(RowXiLinkNvs);
                            dbContext.XiLinks.Remove(RowClickXiLink);
                            dbContext.Reports.Remove(o1Click);

                            //Delete Form Layouts and Details
                            var CreateLayout = dbContext.Layouts.Find(Detail.iCreateLayoutID);
                            var CreateLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == CreateLayout.ID).ToList();
                            dbContext.PopupLayoutDetails.RemoveRange(CreateLayoutDetails);
                            dbContext.Layouts.Remove(CreateLayout);
                            var EditLayout = dbContext.Layouts.Find(Detail.iEditLayoutID);
                            var EditLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == EditLayout.ID).ToList();
                            dbContext.PopupLayoutDetails.RemoveRange(EditLayoutDetails);
                            dbContext.Layouts.Remove(EditLayout);

                            //Delete Form Dialogs and Mappings
                            var CreateDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                            var CreateMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iCreateLayoutID && m.PopupID == Detail.iCreateDialogID).ToList();
                            dbContext.PopupLayoutMappings.RemoveRange(CreateMappings);
                            dbContext.Dialogs.Remove(CreateDlg);
                            var EditDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                            var EditMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iEditLayoutID && m.PopupID == Detail.iEditDialogID).ToList();
                            dbContext.PopupLayoutMappings.RemoveRange(EditMappings);
                            dbContext.Dialogs.Remove(EditDlg);

                            //Delete Form XiLinks
                            var CreateFormXiLink = dbContext.XiLinks.Find(Detail.iCreateFormXiLinkID);
                            var CreateFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iCreateFormXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(CreateFormNVs);
                            dbContext.XiLinks.Remove(CreateFormXiLink);
                            var EditFormXiLink = dbContext.XiLinks.Find(Detail.iEditFormXiLinkID);
                            var EditFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iEditFormXiLinkID).ToList();
                            dbContext.XiLinkNVs.RemoveRange(EditFormNVs);
                            dbContext.XiLinks.Remove(EditFormXiLink);

                            //Delete Structure Details
                            dbContext.XIStructureDetails.Remove(Detail);

                            dbContext.SaveChanges();
                        }
                    }

                }

                if (Add.Count() > 0 && bIsMainBODone)
                {
                    i = 1;
                    Tree = Tree.Where(m => Add.Contains(m.ID)).ToList();
                }
                else
                {
                    Tree = new List<cXIStructure>();
                }
            }

            //if (model.sSavingType == "Generate")
            //{
            foreach (var BO in Tree)
            {
                // Auto Create only if it is not created
                var oBODef = dbContext.BOs.Where(m => m.Name == BO.sBO).FirstOrDefault();//need to change to from cache
                if (oBODef != null)
                {
                    //Auto Create 1-Click with description group
                    Guid i1ClickID;//  AutoCreateOneClick(oBODef, UserDetails.FKiOrgID, sDatabase);
                    if (i == 0)
                    {
                        i1ClickID = AutoCreateOneClick(oBODef, false, null, null, BO.ID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                        iMain1Click = i1ClickID;
                        iQSDefinitionID = AutoCreateQuestionSet(oBODef.LabelName, UserDetails.FKiApplicationID);
                    }
                    else
                    {
                        var RooTBOTable = string.Empty;
                        if (RootBODef != null)
                        {
                            RooTBOTable = RootBODef.TableName;
                        }
                        long iStructureID = BO.ID;
                        var BOFields = oBODef.BOFields.Where(m => m.FKTableName == RooTBOTable).FirstOrDefault();
                        var FKField = string.Empty;
                        if (BOFields != null)
                        {
                            FKField = BOFields.Name;
                        }
                        i1ClickID = AutoCreateOneClick(oBODef, true, FKField, MainBO, iStructureID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                    }
                    //Auto Create XiLink
                    int iXiLinkID = 0;
                    if (i == 0)
                    {
                        if (iQSDefinitionID > 0)
                        {
                            //Create QS Component
                            var QSComponent = dbContext.XIComponents.Where(m => m.sName == ServiceConstants.QSComponent).FirstOrDefault();
                            // Creating Level1 Layout mapping
                            var Layout = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.Level1Layout.ToString().ToLower()).FirstOrDefault();

                            if (Layout != null)
                            {
                                var CopyLayout = CopyLayoutAndCreateNew(Layout, false);
                                iDialgID = AutoCreateDialog(Tree.FirstOrDefault().sName.Replace("_T", ""), CopyLayout.ID, iQSDefinitionID, sDatabase, UserDetails.FKiApplicationID);
                                var PlaceHolder = CopyLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.QSComponentHolder.ToLower()).FirstOrDefault();
                                if (PlaceHolder != null)
                                {
                                    int MappingID = AutoCreateLayoutMappings(CopyLayout.ID, PlaceHolder.PlaceHolderID, QSComponent.ID, iDialgID, "XIComponent", sDatabase);
                                    if (MappingID > 0)
                                    {
                                        List<cNameValuePairs> nQSParams = new List<cNameValuePairs>();
                                        nQSParams.Add(new cNameValuePairs { sName = "iQSDID", sValue = iQSDefinitionID.ToString() });
                                        nQSParams.Add(new cNameValuePairs { sName = "sMode", sValue = "Popup" });
                                        if (QSComponent != null)
                                        {
                                            foreach (var items in nQSParams)
                                            {
                                                cXIComponentParams oParam = new cXIComponentParams();
                                                oParam.FKiComponentID = QSComponent.ID;
                                                oParam.iLayoutMappingID = PlaceHolder.PlaceHolderID;
                                                oParam.sName = items.sName;
                                                oParam.sValue = items.sValue;
                                                dbContext.XIComponentParams.Add(oParam);
                                                dbContext.SaveChanges();
                                            }
                                        }
                                    }
                                }
                            }

                            //Step With Layout contains left tree, Default Details Form and Summery Group
                            var Layout2 = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.LeftTreeLayout.ToString().ToLower()).FirstOrDefault();
                            var NewStepLayout = new cLayouts();
                            if (Layout2 != null)
                            {
                                NewStepLayout = CopyLayoutAndCreateNew(Layout2, false);
                            }
                            var oStep1 = AutoCreateQuestionSetStep("Step1 with Layout", iQSDefinitionID, null, null, NewStepLayout.ID);

                            //Creating Tree Structure Component and adding it to XiLink
                            List<cNameValuePairs> nParams = new List<cNameValuePairs>();
                            nParams.Add(new cNameValuePairs { sName = "iBODID", sValue = RootBODef.BOID.ToString() });
                            nParams.Add(new cNameValuePairs { sName = "sCode", sValue = Tree.FirstOrDefault().sCode });
                            nParams.Add(new cNameValuePairs { sName = "sMode", sValue = "Popup" });
                            nParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-listinstance" });
                            nParams.Add(new cNameValuePairs { sName = "Nodeclickparamname", sValue = "-oneclickid" });
                            nParams.Add(new cNameValuePairs { sName = "register", sValue = "yes" });
                            var oLeftTreeStep = AutoCreateQuestionSetStep("Step2 Left Tree", iQSDefinitionID, ServiceConstants.TreeStructureComponet, nParams, 0);
                            if (NewStepLayout != null)
                            {
                                var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.LeftTreeArea.ToLower()).FirstOrDefault();
                                if (PlaceHolder != null)
                                {
                                    int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oLeftTreeStep.ID, 0, "Step", sDatabase);
                                }
                            }

                            //Creating BO Comopnent and assigning it to Step
                            List<cNameValuePairs> nBOParams = new List<cNameValuePairs>();
                            nBOParams.Add(new cNameValuePairs { sName = "BO", sValue = MainBO });
                            nBOParams.Add(new cNameValuePairs { sName = "Group", sValue = "Details1" });
                            nBOParams.Add(new cNameValuePairs { sName = "LockGroup", sValue = "Lock" });
                            nBOParams.Add(new cNameValuePairs { sName = "iInstanceID", sValue = "{-iInstanceID}" });
                            var oBOStep = AutoCreateQuestionSetStep("Step3 with Main BO Form", iQSDefinitionID, ServiceConstants.FormComponent, nBOParams, 0);
                            if (NewStepLayout != null)
                            {
                                var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.LeftTreeOutput.ToLower()).FirstOrDefault();
                                if (PlaceHolder != null)
                                {
                                    int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oBOStep.ID, 0, "Step", sDatabase);
                                }
                            }

                            //Creating Summery BO Comopnent and assigning it to step
                            List<cNameValuePairs> nBOSummaryParams = new List<cNameValuePairs>();
                            nBOSummaryParams.Add(new cNameValuePairs { sName = "BO", sValue = MainBO });
                            nBOSummaryParams.Add(new cNameValuePairs { sName = "Group", sValue = "Description" });
                            nBOSummaryParams.Add(new cNameValuePairs { sName = "DisplayMode", sValue = "View" });
                            nBOSummaryParams.Add(new cNameValuePairs { sName = "iInstanceID", sValue = "{-iInstanceID}" });
                            var oBOSummeryStep = AutoCreateQuestionSetStep("Step4 with Summary BO Form", iQSDefinitionID, ServiceConstants.FormComponent, nBOSummaryParams, 0);
                            if (NewStepLayout != null)
                            {
                                var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.SummeryHolder.ToLower()).FirstOrDefault();
                                if (PlaceHolder != null)
                                {
                                    int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oBOSummeryStep.ID, 0, "Step", sDatabase);
                                }
                            }
                        }
                        else
                        {
                            iXiLinkID = AutoCreateLeftPaneXiLink(oBODef.Name, ServiceConstants.Details1Group, true, sDatabase, Guid.Empty);
                        }
                    }
                    else
                    {
                        if (iQSDefinitionID > 0)
                        {
                            if (i == 1)
                            {
                                //Step With sub Layout contains List and BO Form
                                var Layout3 = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.SubEntityLayout.ToString().ToLower()).FirstOrDefault();
                                var NewStepLayout = new cLayouts();
                                if (Layout3 != null)
                                {
                                    NewStepLayout = CopyLayoutAndCreateNew(Layout3, false);
                                }
                                var oStep4 = AutoCreateQuestionSetStep("Step5 with Layout", iQSDefinitionID, null, null, NewStepLayout.ID);

                                //Creating One Click Component and adding it to step
                                List<cNameValuePairs> nListParams = new List<cNameValuePairs>();
                                nListParams.Add(new cNameValuePairs { sName = "1ClickID", sValue = "{XIP|i1ClickID}" });
                                nListParams.Add(new cNameValuePairs { sName = "Listclickparamname", sValue = "-listinstance" });
                                nListParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-oneclickid" });
                                nListParams.Add(new cNameValuePairs { sName = "register", sValue = "yes" });
                                var oOneClickStep = AutoCreateQuestionSetStep("Step6 with 1-click Component", iQSDefinitionID, ServiceConstants.OneClickComponent, nListParams, 0);
                                if (NewStepLayout != null)
                                {
                                    var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.SubEntityListArea.ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oOneClickStep.ID, 0, "Step", sDatabase);
                                    }
                                }

                                //Creating BO Component for Sub entity
                                List<cNameValuePairs> nSubBOParams = new List<cNameValuePairs>();
                                nSubBOParams.Add(new cNameValuePairs { sName = "BO", sValue = "{XIP|ActiveBO}" });
                                nSubBOParams.Add(new cNameValuePairs { sName = "Group", sValue = "Details1" });
                                nSubBOParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-listinstance" });
                                nSubBOParams.Add(new cNameValuePairs { sName = "register", sValue = "yes" });
                                var oSubBOStep = AutoCreateQuestionSetStep("Step7 with BO Component", iQSDefinitionID, ServiceConstants.FormComponent, nSubBOParams, 0);
                                if (NewStepLayout != null)
                                {
                                    var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.SubEntityDetailsArea.ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oSubBOStep.ID, 0, "Step", sDatabase);
                                    }
                                }
                            }
                        }
                        else
                        {
                            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
                            NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "Search" });
                            NVs.Add(new XiLinkNVs { Name = "Output", Value = ServiceConstants.PopupOutputArea });
                            iXiLinkID = AutoCreateXiLink(oBODef.LabelName, i1ClickID, NVs, sDatabase);
                        }
                    }
                    if (i == 0)
                    {
                        XILinkIDs.Insert(0, iXiLinkID);
                        StructIDs.Insert(0, BO.ID);
                    }
                    else
                    {
                        XILinkIDs.Add(iXiLinkID);
                        StructIDs.Add(BO.ID);
                    }
                }
                i++;
                //Menu to Show Popups
            }

            if (model.sSavingType.ToLower() == "generate" || !bIsMainBODone)
            {
                //Create XiLink for Tabs with all XiLinks
                //int TabXiLinkID = AutoCreateTabsXiLink(Tree.FirstOrDefault().sName, XILinkIDs, StructIDs, Tree.FirstOrDefault().ID, sDatabase);
                //Create Dialog for Popup


                //oLayout = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.LeftTreeLayout.ToLower()).FirstOrDefault();
                //Map XiLink to Layout
                //AutoMapXiLinkstoLayout(ServiceConstants.LeftTreeLayout, null, iDialgID, TabXiLinkID, sDatabase);
                //AutoCreateLayoutMappings(int iLayoutID, int iPlaceHolderID, int iXiLinkID, int iDialogID, string ContentType, string sDatabase)
                //int LeftPaneXiLinkID = AutoCreateLeftPaneXiLink(Tree.FirstOrDefault().sBO, ServiceConstants.DescriptionGroup, false, sDatabase);


                //AutoMapXiLinkstoLayout(ServiceConstants.LeftTreeLayout, ServiceConstants.PopupLeftPaneArea, iDialgID, LeftPaneXiLinkID, sDatabase);
                // Create Dialog XILink and Map to 1-Click
                AutoCreateDialogXiLinkMapTOCLick(Tree.FirstOrDefault().sName, iMain1Click, iDialgID, sDatabase);
            }


            //}

            foreach (var items in Tree.Where(m => m.bIsAutoCreateDone == false).ToList())
            {
                var oBO = dbContext.XIStructure.Find(items.ID);
                oBO.bIsAutoCreateDone = true;
                dbContext.SaveChanges();
            }

            cBOUIDetails oBOUI = new cBOUIDetails();
            oBOUI.FKi1ClickIDXIGUID = iMain1Click;
            oBOUI.FKiBOID = model.FKiBOID;
            oBOUI.FKiStructureID = Tree.FirstOrDefault().ID;
            if (oLayout != null)
            {
                oBOUI.iLayoutID = oLayout.ID;
            }
            oBOUI.StatusTypeID = 10;
            oBOUI.CreatedTime = DateTime.Now;
            oBOUI.CreatedBy = 1;
            oBOUI.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oBOUI.UpdatedTime = DateTime.Now;
            oBOUI.UpdatedBy = 1;
            oBOUI.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.BOUIDetails.Add(oBOUI);
            dbContext.SaveChanges();

            return new VMCustomResponse() { ID = oBOUI.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
        }

        private cLayouts CopyLayoutAndCreateNew(cLayouts oLayout, bool bUseParent)
        {
            cLayouts oNewLay = new cLayouts();
            List<PopupLayoutDetails> oDetails = new List<PopupLayoutDetails>();
            if (oLayout != null)
            {
                oNewLay.FKiApplicationID = oLayout.FKiApplicationID;
                oNewLay.LayoutName = oLayout.LayoutName + " copy";
                oNewLay.LayoutType = oLayout.LayoutType;
                oNewLay.LayoutCode = oLayout.LayoutCode;
                oNewLay.XiParameterID = oLayout.XiParameterID;
                oNewLay.LayoutLevel = oLayout.LayoutLevel;
                oNewLay.Authentication = oLayout.Authentication;
                oNewLay.iThemeID = oLayout.iThemeID;
                oNewLay.StatusTypeID = oLayout.StatusTypeID;
                oNewLay.UpdatedBy = oLayout.UpdatedBy;
                oNewLay.CreatedBy = oLayout.CreatedBy;
                oNewLay.UpdatedTime = oLayout.UpdatedTime;
                oNewLay.UpdatedBySYSID = oLayout.UpdatedBySYSID;
                oNewLay.CreatedBySYSID = oLayout.CreatedBySYSID;
                oNewLay.CreatedTime = oLayout.CreatedTime;
                oNewLay.ID = 0;
                oNewLay.bUseParentGUID = bUseParent;
                ModelDbContext dbContext = new ModelDbContext();
                dbContext.Layouts.Add(oNewLay);
                dbContext.SaveChanges();
                if (oLayout.LayoutDetails != null && oLayout.LayoutDetails.Count() > 0)
                {
                    foreach (var items in oLayout.LayoutDetails)
                    {
                        PopupLayoutDetails oDetail = new PopupLayoutDetails();
                        //oDetail = items;
                        oDetail.PlaceHolderID = 0;
                        oDetail.PlaceholderArea = items.PlaceholderArea;
                        oDetail.PlaceholderName = items.PlaceholderName;
                        oDetail.LayoutID = oNewLay.ID;
                        oDetail.PlaceholderUniqueName = items.PlaceholderUniqueName.Replace(" ", "");
                        dbContext.PopupLayoutDetails.Add(oDetail);
                        dbContext.SaveChanges();
                        oDetails.Add(oDetail);
                    }

                }
            }
            //oNewLay.LayoutDetails.Clear();
            oNewLay.LayoutDetails = oDetails;
            return oNewLay;
        }

        public int AutoCreateXiLink(string sName, Guid i1ClickID, List<XiLinkNVs> NVPairs, string sDatabase, bool isFromDialog = false)
        {
            ModelDbContext dbContext = new ModelDbContext();
            XiLinks oXiLink = new XiLinks();
            oXiLink.Name = sName;
            oXiLink.URL = "XiLink";
            //oXiLink.OneClickID = i1ClickID;
            if (!isFromDialog)
            {
                oXiLink.FKiComponentID = 2;
                if (!string.IsNullOrEmpty(i1ClickID.ToString()))
                {
                    oXiLink.FKiComponentID = 3;
                }
            }

            oXiLink.FKi1ClickIDXIGUID = i1ClickID;
            oXiLink.StatusTypeID = 10;
            oXiLink.CreatedTime = DateTime.Now;
            oXiLink.CreatedBy = 1;
            oXiLink.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oXiLink.UpdatedTime = DateTime.Now;
            oXiLink.UpdatedBy = 1;
            oXiLink.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.XiLinks.Add(oXiLink);
            dbContext.SaveChanges();
            if (isFromDialog)
            {
                foreach (var NV in NVPairs)
                {
                    XiLinkNVs oNvs = new XiLinkNVs();
                    oNvs.XiLinkID = oXiLink.XiLinkID;
                    oNvs.Name = NV.Name;
                    oNvs.Value = NV.Value;
                    oNvs.CreatedTime = DateTime.Now;
                    oNvs.CreatedBy = 1;
                    oNvs.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    oNvs.UpdatedTime = DateTime.Now;
                    oNvs.UpdatedBy = 1;
                    oNvs.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                    dbContext.XiLinkNVs.Add(oNvs);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                foreach (var nv in NVPairs)
                {
                    cXIComponentParams xiparams = new cXIComponentParams();
                    xiparams.iXiLinkID = oXiLink.XiLinkID;
                    xiparams.sName = nv.Name;
                    xiparams.sValue = nv.Value;
                    xiparams.FKiComponentID = 2;
                    if (!string.IsNullOrEmpty(i1ClickID.ToString()))
                    {
                        xiparams.FKiComponentID = 3;
                    }
                    dbContext.XIComponentParams.Add(xiparams);
                    dbContext.SaveChanges();
                }
            }

            return oXiLink.XiLinkID;
        }

        private void AutoCreateDialogXiLinkMapTOCLick(string sBO, Guid iMain1Click, int iDialgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
            NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "Dialog" });
            NVs.Add(new XiLinkNVs { Name = "DialogID", Value = iDialgID.ToString() });
            int iXiLinkID = AutoCreateXiLink(sBO + " 1Click Dialog XiLink", Guid.Empty, NVs, sDatabase, true);
            if (!string.IsNullOrEmpty(iMain1Click.ToString()))
            {
                var o1Click = dbContext.Reports.Where(x => x.XIGUID == iMain1Click).Select(t => t).FirstOrDefault();
                o1Click.IsRowClick = true;
                o1Click.RowXiLinkID = iXiLinkID;
                dbContext.SaveChanges();
            }
        }

        private int AutoCreateDialog(string sBO, int iLayoutID, int iQSID, string sDatabase, int iApplicationID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var PopupDetails = dbContext.Layouts.Where(m => m.ID == iLayoutID).FirstOrDefault();
            if (PopupDetails.ID > 0)
            {
                int iDlagID = AutoCreateDialogNew(sBO, PopupDetails.ID, iQSID, sDatabase, iApplicationID);
                return iDlagID;
            }
            return 0;
        }

        private int AutoCreateTabsXiLink(string sBO, List<int> XILinkIDs, List<long> StructIDs, long iParentStructID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
            NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "Tabs" });
            foreach (var ID in XILinkIDs)
            {
                NVs.Add(new XiLinkNVs { Name = "XiLinkID", Value = ID.ToString() });
            }
            int iXiLinkID = AutoCreateXiLink(sBO + " Tabs XiLink", Guid.Empty, NVs, sDatabase, true);
            for (int i = 0; i < StructIDs.Count(); i++)
            {
                long StructID = StructIDs[i];
                var oStruct = dbContext.XIStructureDetails.Where(m => m.FKiStructureID == StructID).FirstOrDefault();
                if (oStruct != null)
                {
                    oStruct.iParentStructureID = iParentStructID;
                    oStruct.iTabXiLinkID = XILinkIDs[i];
                    dbContext.SaveChanges();
                }
            }
            return iXiLinkID;
        }

        private void AutoMapXiLinkstoLayout(string popupLayoutName, string sLayoutArea, int iDialgID, int TabXiLinkID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var PopupDetails = dbContext.Layouts.Where(m => m.LayoutName == popupLayoutName).FirstOrDefault();

            var Layout = Common.GetLayoutDetails(PopupDetails.ID, 0, 0, 0, null, 0, null, sDatabase);
            var PlaceHolderID = Layout.Details.Where(m => m.PlaceholderName == sLayoutArea).Select(m => m.PlaceHolderID).FirstOrDefault();
            if (PlaceHolderID > 0)
            {
                AutoCreateLayoutMappings(PopupDetails.ID, PlaceHolderID, TabXiLinkID, iDialgID, "XiLink", sDatabase);
            }
        }

        private Guid AutoCreateOneClick(BOs oBODef, bool bIsSubNode, string sFKField, string sFKBO, long iStructureID, string sSavingType, int iOrgID, string sDatabase, int iApplicationID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            int CreateLayoutID = 0;
            int EditLayoutID = 0;
            int CreatePlaceHolderID = 0;
            int EditPlaceHolderID = 0;
            int CreateFormDlgID = 0;
            int EditFormDlgID = 0;
            int CreateFormXiLinkID = 0;
            int EditFormXiLinkID = 0;
            if (bIsSubNode)
            {
                //Create Layouts
                CreateLayoutID = AutoCreateLayout(oBODef.LabelName, "Create", sDatabase);
                EditLayoutID = AutoCreateLayout(oBODef.LabelName, "Edit", sDatabase);

                //Create Layout Details
                CreatePlaceHolderID = AutoCreateLayoutDetails(oBODef.LabelName, CreateLayoutID, "Create", sDatabase);
                EditPlaceHolderID = AutoCreateLayoutDetails(oBODef.LabelName, EditLayoutID, "Edit", sDatabase);

                //Create Dialogs
                CreateFormDlgID = AutoCreateDialogNew(oBODef.LabelName, CreateLayoutID, 0, sDatabase, iApplicationID);
                EditFormDlgID = AutoCreateDialogNew(oBODef.LabelName, EditLayoutID, 0, sDatabase, iApplicationID);

                //Create XiLinks
                CreateFormXiLinkID = AutoCreateFormXiLink(oBODef.Name, ServiceConstants.CreateGroup, sDatabase);
                EditFormXiLinkID = AutoCreateFormXiLink(oBODef.Name, ServiceConstants.EditGroup, sDatabase);

                //Map XiLink to Dialog placeholder
                AutoCreateLayoutMappings(CreateLayoutID, CreatePlaceHolderID, CreateFormXiLinkID, CreateFormDlgID, "XiLink", sDatabase);
                AutoCreateLayoutMappings(EditLayoutID, EditPlaceHolderID, EditFormXiLinkID, EditFormDlgID, "XiLink", sDatabase);


            }
            string sWhereClause = string.Empty;
            if (!string.IsNullOrEmpty(sFKBO) && !string.IsNullOrEmpty(sFKField))
            {
                sWhereClause = " where " + sFKField + "={XIP|" + sFKBO + ".id}";
            }

            var DescriptionFields = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == ServiceConstants.DescriptionGroup.ToLower()).Select(m => m.BOFieldNames).FirstOrDefault();
            if (string.IsNullOrEmpty(DescriptionFields))
            {
                DescriptionFields = oBODef.sPrimaryKey;
            }
            Reports o1Click = new Reports();
            o1Click.Name = oBODef.LabelName.Replace("_T", "");
            o1Click.BOID = oBODef.BOID;
            var sPrimaryKey = string.Empty;
            if (!string.IsNullOrEmpty(oBODef.sPrimaryKey))
            {
                sPrimaryKey = oBODef.sPrimaryKey;
            }
            else
            {
                sPrimaryKey = "id";
            }
            if (!string.IsNullOrEmpty(sWhereClause))
            {
                o1Click.Query = "Select " + DescriptionFields + " from " + oBODef.TableName + sWhereClause + " order by " + sPrimaryKey + " desc";
            }
            else
            {
                o1Click.Query = "Select " + DescriptionFields + " from " + oBODef.TableName + " order by " + sPrimaryKey + " desc";
            }

            o1Click.Description = o1Click.Name;
            o1Click.Title = o1Click.Name;
            o1Click.OrganizationID = iOrgID;
            o1Click.PopupLeft = o1Click.PopupHeight = o1Click.PopupTop = o1Click.PopupWidth = 0;
            o1Click.StatusTypeID = 10;
            o1Click.DisplayAs = (int)Enum.Parse(typeof(EnumDisplayTypes), "ResultList");
            o1Click.ResultListDisplayType = 1;
            o1Click.IsFilterSearch = true;
            var sSearchFields = string.Empty;
            var oGrpD = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == ServiceConstants.SearchGroup.ToLower()).FirstOrDefault();
            if (oGrpD != null)
            {
                if (!string.IsNullOrEmpty(oGrpD.BOFieldNames))
                {
                    sSearchFields = oGrpD.BOFieldNames;
                }
                else
                {
                    sSearchFields = oBODef.sPrimaryKey;
                }
            }
            else
            {
                sSearchFields = oBODef.sPrimaryKey;
            }
            o1Click.SearchFields = sSearchFields;
            //sSearchFields = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == ServiceConstants.CreateGroup.ToLower()).Select(m => m.ID).FirstOrDefault();
            //o1Click.sSystemType = "XISystem";            
            o1Click.IsCreate = true;
            int GroupID = oBODef.BOGroups.Where(m => m.GroupName.ToLower() == ServiceConstants.CreateGroup.ToLower()).Select(m => m.ID).FirstOrDefault();
            if (GroupID > 0)
            {
                o1Click.CreateGroupID = GroupID;
            }
            o1Click.iLayoutID = CreateLayoutID;
            o1Click.sCreateType = "inlinetop";
            o1Click.CreatedTime = DateTime.Now;
            o1Click.CreatedBy = 1;
            o1Click.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            o1Click.UpdatedTime = DateTime.Now;
            o1Click.UpdatedBy = 1;
            o1Click.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.Reports.Add(o1Click);
            dbContext.SaveChanges();



            if (bIsSubNode)
            {

                //Create XiLink For Form on 1-Click
                //Create Layout 
                AutoCreateDialogXiLinkMapTOCLick(oBODef.Name.Replace("_T", ""), o1Click.XIGUID, EditFormDlgID, sDatabase);
                //Save Node Details
                if (sSavingType.ToLower() == "save")
                {
                    var oStruct = dbContext.XIStructure.Find(iStructureID);
                    cXIStructureDetails oStructDet = new cXIStructureDetails();
                    oStructDet.FKiStructureID = iStructureID;
                    if (oStruct.FKiParentID != "#")
                    {
                        oStructDet.iParentStructureID = Convert.ToInt64(oStruct.FKiParentID);
                    }
                    oStructDet.i1ClickID = o1Click.ID;
                    oStructDet.iCreateLayoutID = CreateLayoutID;
                    oStructDet.iEditLayoutID = EditLayoutID;
                    oStructDet.iCreateDialogID = CreateFormDlgID;
                    oStructDet.iEditDialogID = EditFormDlgID;
                    oStructDet.iCreateFormXiLinkID = CreateFormXiLinkID;
                    oStructDet.iEditFormXiLinkID = EditFormXiLinkID;
                    dbContext.XIStructureDetails.Add(oStructDet);
                    dbContext.SaveChanges();
                }
            }
            else
            {
                if (sSavingType.ToLower() == "save")
                {
                    cXIStructureDetails oStructDet = new cXIStructureDetails();
                    oStructDet.FKiStructureID = iStructureID;
                    oStructDet.i1ClickID = o1Click.ID;
                    dbContext.XIStructureDetails.Add(oStructDet);
                    dbContext.SaveChanges();
                }
            }

            return o1Click.XIGUID;
        }

        private int AutoCreateFormXiLink(string sBO, string sGroup, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string sName = string.Empty;
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();

            if (sGroup == ServiceConstants.CreateGroup)
            {
                sName = sBO + " Create Form XiLink";
                // NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "CreateForm" });
            }
            else
            {
                sName = sBO + "Edit Form XiLink";
                // NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "inlineEdit" });
            }


            NVs.Add(new XiLinkNVs { Name = "BO", Value = sBO });
            NVs.Add(new XiLinkNVs { Name = "Group", Value = sGroup });
            int iXiLinkID = AutoCreateXiLink(sName, Guid.Empty, NVs, sDatabase, false);
            return iXiLinkID;

        }

        private int AutoCreateLeftPaneXiLink(string sBO, string sGroup, bool bIsOutput, string sDatabase, Guid i1ClickID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            string sName = string.Empty;
            if (bIsOutput)
            {
                sName = "Details";
            }
            else
            {
                sName = sBO.Replace("_T", "") + " Form XiLink";
            }
            List<XiLinkNVs> NVs = new List<XiLinkNVs>();
            //if (sGroup == ServiceConstants.DescriptionGroup)
            //{
            //    NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "PopupLeftContent" });
            //}
            //else
            //{
            //    NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "inlineView" });
            //}
            if (!string.IsNullOrEmpty(i1ClickID.ToString()))
            {
                NVs.Add(new XiLinkNVs { Name = "1ClickID", Value = i1ClickID.ToString() });
                NVs.Add(new XiLinkNVs { Name = "ListClickparamname", Value = "-listinstance" });
            }
            else
            {
                NVs.Add(new XiLinkNVs { Name = "BO", Value = sBO });
                NVs.Add(new XiLinkNVs { Name = "Group", Value = sGroup });
            }

            //if (bIsOutput)
            //{
            //    NVs.Add(new XiLinkNVs { Name = "Output", Value = ServiceConstants.PopupOutputArea });
            //}
            int iXiLinkID = AutoCreateXiLink(sName + " Form ", i1ClickID, NVs, sDatabase);

            return iXiLinkID;
        }

        private int AutoCreateLayout(string sBO, string sType, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cLayouts oLay = new cLayouts();
            oLay.LayoutName = sBO + " Form " + sType + " Layout Template";
            oLay.LayoutCode = ServiceConstants.FormLayoutTemplate;
            oLay.LayoutType = "Dialog";
            oLay.XiParameterID = 1;
            oLay.LayoutLevel = "OrganisationLevel";
            oLay.Authentication = "Authenticated";
            oLay.iThemeID = 0;
            oLay.bUseParentGUID = true;
            oLay.StatusTypeID = 10;
            oLay.CreatedTime = DateTime.Now;
            oLay.CreatedBy = 1;
            oLay.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oLay.UpdatedTime = DateTime.Now;
            oLay.UpdatedBy = 1;
            oLay.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.Layouts.Add(oLay);
            dbContext.SaveChanges();

            return oLay.ID;
        }

        public int AutoCreateLayoutDetails(string sBO, int iLayoutID, string sType, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            PopupLayoutDetails oDetail = new PopupLayoutDetails();
            oDetail.LayoutID = iLayoutID;
            oDetail.PlaceholderName = sType + "Content";
            oDetail.PlaceholderUniqueName = sBO.Replace(" ", "") + sType.Replace(" ", "") + "Content";
            oDetail.PlaceholderArea = "div1";
            dbContext.PopupLayoutDetails.Add(oDetail);
            dbContext.SaveChanges();
            return oDetail.PlaceHolderID;
        }

        public int AutoCreateDialogNew(string sName, int iLayoutID, int iQSID, string sDatabase, int iApplicationID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            Dialogs oCreateDlg = new Dialogs();
            oCreateDlg.DialogName = sName + " Dialog";
            oCreateDlg.FKiApplicationID = iApplicationID;
            oCreateDlg.LayoutID = iLayoutID;
            oCreateDlg.StatusTypeID = 10;
            //oCreateDlg.DialogWidth = 1100;
            //oCreateDlg.DialogHeight = 800;
            oCreateDlg.PopupSize = "Default";
            //oCreateDlg.DialogMy1 = "left";
            //oCreateDlg.DialogMy2 = "top";
            //oCreateDlg.DialogAt1 = "left";
            //oCreateDlg.DialogAt2 = "top";
            oCreateDlg.IsMaximiseIcon = true;
            oCreateDlg.IsMinimiseIcon = true;
            oCreateDlg.IsResizable = true;
            oCreateDlg.IsResizable = true;
            oCreateDlg.FKiQSDefinitionID = iQSID;
            oCreateDlg.CreatedTime = DateTime.Now;
            oCreateDlg.CreatedBy = 1;
            oCreateDlg.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oCreateDlg.UpdatedTime = DateTime.Now;
            oCreateDlg.UpdatedBy = 1;
            oCreateDlg.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.Dialogs.Add(oCreateDlg);
            dbContext.SaveChanges();
            return oCreateDlg.ID;
        }

        public int AutoCreateLayoutMappings(int iLayoutID, int iPlaceHolderID, int iXiLinkID, int iDialogID, string ContentType, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            PopupLayoutMappings oMap = new PopupLayoutMappings();
            oMap.PopupLayoutID = iLayoutID;
            oMap.PlaceHolderID = iPlaceHolderID;
            oMap.XiLinkID = iXiLinkID;
            oMap.PopupID = iDialogID;
            oMap.Type = "Dialog";
            oMap.ContentType = ContentType;
            oMap.StatusTypeID = 10;
            oMap.CreatedTime = DateTime.Now;
            oMap.CreatedBy = 1;
            oMap.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oMap.UpdatedTime = DateTime.Now;
            oMap.UpdatedBy = 1;
            oMap.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.PopupLayoutMappings.Add(oMap);
            dbContext.SaveChanges();
            return oMap.ID;
        }

        //Auto Create Question Set
        public int AutoCreateQuestionSet(string sBO, int FKiApplicationID)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cQSDefinition oQS = new cQSDefinition();
            oQS.FKiApplicationID = FKiApplicationID;
            oQS.sName = sBO + " Popup Question Set";
            oQS.sDescription = sBO + " Popup Question Set";
            oQS.iVisualisationID = 0;
            oQS.SaveType = "Save at End";
            oQS.bIsTemplate = true;
            oQS.bInMemoryOnly = true;
            oQS.StatusTypeID = 10;
            oQS.CreatedTime = DateTime.Now;
            oQS.CreatedBy = 1;
            oQS.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            oQS.UpdatedTime = DateTime.Now;
            oQS.UpdatedBy = 1;
            oQS.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            dbContext.QSDefinition.Add(oQS);
            dbContext.SaveChanges();
            return oQS.ID;
        }

        public cQSStepDefiniton AutoCreateQuestionSetStep(string Name, int iQSID, string sComponentName, List<cNameValuePairs> nParams, int iLayoutID)
        {
            try
            {

                ModelDbContext dbContext = new ModelDbContext();
                cQSStepDefiniton oStep = new cQSStepDefiniton();
                oStep.FKiQSDefintionID = iQSID;
                oStep.sName = Name;
                oStep.iDisplayAs = 0;
                oStep.iLayoutID = iLayoutID;
                oStep.bInMemoryOnly = true;
                oStep.iXIComponentID = 0;
                oStep.StatusTypeID = 10;
                oStep.CreatedTime = DateTime.Now;
                oStep.CreatedBy = 1;
                oStep.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oStep.UpdatedTime = DateTime.Now;
                oStep.UpdatedBy = 1;
                oStep.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.QSStepDefiniton.Add(oStep);
                dbContext.SaveChanges();
                var Component = dbContext.XIComponents.Where(m => m.sName.ToLower() == sComponentName.ToLower()).FirstOrDefault();
                if (Component != null)
                {
                    oStep.iDisplayAs = 20;
                    dbContext.SaveChanges();
                    AutoCreateStepSection(oStep.ID, Component.ID, nParams);
                }
                return oStep;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private void AutoCreateStepSection(int iStepID, int iComponentD, List<cNameValuePairs> nParams)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cStepSectionDefinition oSection = new cStepSectionDefinition();
            oSection.FKiStepDefinitionID = iStepID;
            oSection.iXIComponentID = iComponentD;
            oSection.iDisplayAs = 40;
            oSection.iOrder = 0;
            oSection.sIsHidden = "off";
            dbContext.StepSectionDefinition.Add(oSection);
            dbContext.SaveChanges();
            if (nParams != null)
            {
                foreach (var items in nParams)
                {
                    cXIComponentParams oParam = new cXIComponentParams();
                    oParam.FKiComponentID = iComponentD;
                    oParam.iStepSectionID = oSection.ID;
                    oParam.sName = items.sName;
                    oParam.sValue = items.sValue;
                    dbContext.XIComponentParams.Add(oParam);
                    dbContext.SaveChanges();
                }
            }

        }


        //Auto Create Popup with Question Set Template and Step
        public VMCustomResponse SaveTabsBOUIDetailsWithQuestionSet(cBOUIDetails model, int iUserID, string sOrgName, string sDatabase)
        {
            try
            {
                ModelDbContext dbContext = new ModelDbContext();
                var UserDetails = Common.GetUserDetails(iUserID, sOrgName, sDatabase);
                var Tree = GetXIStructureTreeDetails(model.FKiBOID, model.FKiStructureID.ToString(), sDatabase).Where(m => m.BOID > 0).ToList();
                List<int> XILinkIDs = new List<int>();
                List<long> StructIDs = new List<long>();
                int i = 0;
                int iDialgID = 0;
                Guid iMain1Click = Guid.Empty;
                int iQSDefinitionID = 0;
                var bIsMainBODone = false;
                cLayouts oLayout = new cLayouts();
                var MainBO = Tree.FirstOrDefault().sBO;
                var RootBODef = dbContext.BOs.Where(m => m.Name.ToLower() == MainBO.ToLower()).FirstOrDefault(); //oXIAPI.Get_BODefinition(MainBO, iUserID, sOrgName, sDatabase);
                var RootName = Tree.FirstOrDefault().sName;
                bIsMainBODone = Tree.FirstOrDefault().bIsAutoCreateDone;
                var ParentStructID = Tree.FirstOrDefault().ID;
                var BOStructIDs = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList();
                var StructDetails = dbContext.XIStructureDetails.Where(m => m.iParentStructureID == ParentStructID).ToList();
                if (model.sSavingType.ToLower() == "save" && StructDetails.Count() > 0)
                {
                    var Delete = StructDetails.Select(m => m.FKiStructureID).ToList().Except(Tree.Select(m => m.ID).ToList()).ToList();
                    var Add = Tree.Where(m => m.FKiParentID != "#").Select(m => m.ID).ToList().Except(StructDetails.Select(m => m.FKiStructureID).ToList()).ToList();

                    if (Delete.Count() > 0)
                    {
                        foreach (var items in Delete)
                        {
                            var Detail = StructDetails.Where(m => m.FKiStructureID == items).FirstOrDefault();
                            if (Detail != null)
                            {
                                //Delete Tab from Popup
                                var TabXiLink = dbContext.XiLinks.Find(Detail.iTabXiLinkID);
                                var TabXiLinkNVs = dbContext.XiLinkNVs.Where(m => m.Name == "XiLinkID" && m.Value == Detail.iTabXiLinkID.ToString()).FirstOrDefault();
                                dbContext.XiLinkNVs.Remove(TabXiLinkNVs);
                                dbContext.XiLinks.Remove(TabXiLink);

                                //Delete 1-click of tab and onclick XiLink
                                var o1Click = dbContext.Reports.Where(x => x.XIGUID == Detail.i1ClickIDXIGUID).Select(t => t).FirstOrDefault();
                                int RowXiLinkID = o1Click.RowXiLinkID;
                                var RowClickXiLink = dbContext.XiLinks.Find(RowXiLinkID);
                                var RowXiLinkNvs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == RowXiLinkID).ToList();
                                dbContext.XiLinkNVs.RemoveRange(RowXiLinkNvs);
                                dbContext.XiLinks.Remove(RowClickXiLink);
                                dbContext.Reports.Remove(o1Click);

                                //Delete Form Layouts and Details
                                var CreateLayout = dbContext.Layouts.Find(Detail.iCreateLayoutID);
                                var CreateLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == CreateLayout.ID).ToList();
                                dbContext.PopupLayoutDetails.RemoveRange(CreateLayoutDetails);
                                dbContext.Layouts.Remove(CreateLayout);
                                var EditLayout = dbContext.Layouts.Find(Detail.iEditLayoutID);
                                var EditLayoutDetails = dbContext.PopupLayoutDetails.Where(m => m.LayoutID == EditLayout.ID).ToList();
                                dbContext.PopupLayoutDetails.RemoveRange(EditLayoutDetails);
                                dbContext.Layouts.Remove(EditLayout);

                                //Delete Form Dialogs and Mappings
                                var CreateDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                                var CreateMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iCreateLayoutID && m.PopupID == Detail.iCreateDialogID).ToList();
                                dbContext.PopupLayoutMappings.RemoveRange(CreateMappings);
                                dbContext.Dialogs.Remove(CreateDlg);
                                var EditDlg = dbContext.Dialogs.Find(Detail.iCreateDialogID);
                                var EditMappings = dbContext.PopupLayoutMappings.Where(m => m.PopupLayoutID == Detail.iEditLayoutID && m.PopupID == Detail.iEditDialogID).ToList();
                                dbContext.PopupLayoutMappings.RemoveRange(EditMappings);
                                dbContext.Dialogs.Remove(EditDlg);

                                //Delete Form XiLinks
                                var CreateFormXiLink = dbContext.XiLinks.Find(Detail.iCreateFormXiLinkID);
                                var CreateFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iCreateFormXiLinkID).ToList();
                                dbContext.XiLinkNVs.RemoveRange(CreateFormNVs);
                                dbContext.XiLinks.Remove(CreateFormXiLink);
                                var EditFormXiLink = dbContext.XiLinks.Find(Detail.iEditFormXiLinkID);
                                var EditFormNVs = dbContext.XiLinkNVs.Where(m => m.XiLinkID == Detail.iEditFormXiLinkID).ToList();
                                dbContext.XiLinkNVs.RemoveRange(EditFormNVs);
                                dbContext.XiLinks.Remove(EditFormXiLink);

                                //Delete Structure Details
                                dbContext.XIStructureDetails.Remove(Detail);

                                dbContext.SaveChanges();
                            }
                        }

                    }

                    if (Add.Count() > 0 && bIsMainBODone)
                    {
                        i = 1;
                        Tree = Tree.Where(m => Add.Contains(m.ID)).ToList();
                    }
                    else
                    {
                        Tree = new List<cXIStructure>();
                    }
                }

                //if (model.sSavingType == "Generate")
                //{
                foreach (var BO in Tree)
                {
                    // Auto Create only if it is not created
                    var oBODef = dbContext.BOs.Where(m => m.Name == BO.sBO).FirstOrDefault();//need to change to from cache
                    if (oBODef != null)
                    {
                        //Auto Create 1-Click with description group
                        Guid i1ClickID = Guid.Empty;//  AutoCreateOneClick(oBODef, UserDetails.FKiOrgID, sDatabase);
                        if (i == 0)
                        {
                            i1ClickID = AutoCreateOneClick(oBODef, false, null, null, BO.ID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                            iMain1Click = i1ClickID;
                            iQSDefinitionID = AutoCreateQuestionSet(oBODef.LabelName, UserDetails.FKiApplicationID);
                        }
                        else
                        {
                            var RooTBOTable = string.Empty;
                            if (RootBODef != null)
                            {
                                RooTBOTable = RootBODef.TableName;
                            }
                            long iStructureID = BO.ID;
                            var BOFields = oBODef.BOFields.Where(m => m.FKTableName == RooTBOTable).FirstOrDefault();
                            var FKField = string.Empty;
                            if (BOFields != null)
                            {
                                FKField = BOFields.Name;
                            }
                            i1ClickID = AutoCreateOneClick(oBODef, true, FKField, MainBO, iStructureID, model.sSavingType, UserDetails.FKiOrgID, sDatabase, UserDetails.FKiApplicationID);
                        }
                        //Auto Create XiLink
                        int iXiLinkID = 0;
                        if (i == 0)
                        {
                            if (iQSDefinitionID > 0)
                            {
                                //Create QS Component
                                var QSComponent = dbContext.XIComponents.Where(m => m.sName == ServiceConstants.QSComponent).FirstOrDefault();
                                // Creating Level1 Layout mapping
                                var Layout = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.Level1Layout.ToLower()).FirstOrDefault();
                                int iTopLayoutOutputArea = 0;
                                if (Layout != null)
                                {
                                    var CopyLayout = CopyLayoutAndCreateNew(Layout, false);

                                    iDialgID = AutoCreateDialog(Tree.FirstOrDefault().sName.Replace("_T", ""), CopyLayout.ID, iQSDefinitionID, sDatabase, UserDetails.FKiApplicationID);
                                    var PlaceHolder = CopyLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.QSComponentHolder.ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(CopyLayout.ID, PlaceHolder.PlaceHolderID, QSComponent.ID, iDialgID, "XIComponent", sDatabase);
                                        if (MappingID > 0)
                                        {
                                            List<cNameValuePairs> nQSParams = new List<cNameValuePairs>();
                                            nQSParams.Add(new cNameValuePairs { sName = "iQSDID", sValue = iQSDefinitionID.ToString() });
                                            nQSParams.Add(new cNameValuePairs { sName = "sMode", sValue = "Popup" });
                                            if (QSComponent != null)
                                            {
                                                foreach (var items in nQSParams)
                                                {
                                                    cXIComponentParams oParam = new cXIComponentParams();
                                                    oParam.FKiComponentID = QSComponent.ID;
                                                    oParam.iLayoutMappingID = PlaceHolder.PlaceHolderID;
                                                    oParam.sName = items.sName;
                                                    oParam.sValue = items.sValue;
                                                    dbContext.XIComponentParams.Add(oParam);
                                                    dbContext.SaveChanges();
                                                }
                                            }
                                        }
                                    }
                                }

                                //Step With Layout contains left tree, Default Details Form and Summery Group
                                var Layout2 = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == "New Policy Tab Layout".ToLower()).FirstOrDefault();
                                var NewStepLayout = new cLayouts();
                                if (Layout2 != null)
                                {
                                    NewStepLayout = CopyLayoutAndCreateNew(Layout2, true);
                                    iTopLayoutOutputArea = NewStepLayout.ID;
                                }
                                var oStep1 = AutoCreateQuestionSetStep("Step1 with Layout", iQSDefinitionID, null, null, NewStepLayout.ID);

                                //Creating Tree Structure Component and adding it to XiLink
                                List<cNameValuePairs> nParams = new List<cNameValuePairs>();
                                nParams.Add(new cNameValuePairs { sName = "BO", sValue = MainBO });
                                nParams.Add(new cNameValuePairs { sName = "Group", sValue = "Description" });
                                nParams.Add(new cNameValuePairs { sName = "iInstanceID", sValue = "{-iInstanceID}" });
                                nParams.Add(new cNameValuePairs { sName = "Visualisation", sValue = "Mode" });
                                var oLeftTreeStep = AutoCreateQuestionSetStep("Step with BO Component View", iQSDefinitionID, ServiceConstants.FormComponent, nParams, 0);
                                if (NewStepLayout != null)
                                {
                                    var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == "Description".ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oLeftTreeStep.ID, 0, "Step", sDatabase);
                                    }
                                }

                                //Creating BO Comopnent and assigning it to Step
                                List<cNameValuePairs> nBOParams = new List<cNameValuePairs>();
                                nBOParams.Add(new cNameValuePairs { sName = "StructureCode", sValue = Tree.FirstOrDefault().sCode });
                                nBOParams.Add(new cNameValuePairs { sName = "iBODID", sValue = Tree.FirstOrDefault().BOID.ToString() });
                                nBOParams.Add(new cNameValuePairs { sName = "OutputContent", sValue = "MainContent-" + iTopLayoutOutputArea });
                                nBOParams.Add(new cNameValuePairs { sName = "Tabclickparamname", sValue = "-TabInstance" });
                                var oBOStep = AutoCreateQuestionSetStep("Step with Tab Component", iQSDefinitionID, "Tab Component", nBOParams, 0);
                                if (NewStepLayout != null)
                                {
                                    var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == "TabArea".ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oBOStep.ID, 0, "Step", sDatabase);
                                    }
                                }

                                //Creating Summery BO Comopnent and assigning it to step
                                List<cNameValuePairs> nBOSummaryParams = new List<cNameValuePairs>();
                                nBOSummaryParams.Add(new cNameValuePairs { sName = "BO", sValue = MainBO });
                                nBOSummaryParams.Add(new cNameValuePairs { sName = "Group", sValue = "Details1" });
                                nBOSummaryParams.Add(new cNameValuePairs { sName = "iInstanceID", sValue = "{-iInstanceID}" });
                                var oBOSummeryStep = AutoCreateQuestionSetStep("Step with BO Component", iQSDefinitionID, ServiceConstants.FormComponent, nBOSummaryParams, 0);
                                if (NewStepLayout != null)
                                {
                                    var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == "Main Content".ToLower()).FirstOrDefault();
                                    if (PlaceHolder != null)
                                    {
                                        int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oBOSummeryStep.ID, 0, "Step", sDatabase);
                                    }
                                }
                                //Creating 1-Click Comopnent and assigning it to step
                                List<cNameValuePairs> oneClickParams = new List<cNameValuePairs>();
                                oneClickParams.Add(new cNameValuePairs { sName = "1ClickID", sValue = "{XIP|i1ClickID}" });
                                oneClickParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-TabInstance" });
                                var o1ClickStep = AutoCreateQuestionSetStep("Step with 1-click Component", iQSDefinitionID, ServiceConstants.OneClickComponent, oneClickParams, 0);

                            }
                            else
                            {
                                iXiLinkID = AutoCreateLeftPaneXiLink(oBODef.Name, ServiceConstants.Details1Group, true, sDatabase, Guid.Empty);
                            }
                        }
                        else
                        {
                            if (iQSDefinitionID > 0)
                            {
                                if (i == 1)
                                {
                                    //Step With sub Layout contains List and BO Form
                                    //    var Layout3 = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.SubEntityLayout.ToString().ToLower()).FirstOrDefault();
                                    //    var NewStepLayout = new cLayouts();
                                    //    if (Layout3 != null)
                                    //    {
                                    //        NewStepLayout = CopyLayoutAndCreateNew(Layout3);
                                    //    }
                                    //    var oStep4 = AutoCreateQuestionSetStep("Step5 with Layout", iQSDefinitionID, null, null, NewStepLayout.ID);

                                    //    //Creating One Click Component and adding it to step
                                    //    List<cNameValuePairs> nListParams = new List<cNameValuePairs>();
                                    //    nListParams.Add(new cNameValuePairs { sName = "1ClickID", sValue = "{XIP|i1ClickID}" });
                                    //    nListParams.Add(new cNameValuePairs { sName = "Listclickparamname", sValue = "-listinstance" });
                                    //    nListParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-oneclickid" });
                                    //    nListParams.Add(new cNameValuePairs { sName = "register", sValue = "yes" });
                                    //    var oOneClickStep = AutoCreateQuestionSetStep("Step6 with 1-click Component", iQSDefinitionID, ServiceConstants.OneClickComponent, nListParams, 0);
                                    //    if (NewStepLayout != null)
                                    //    {
                                    //        var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.SubEntityListArea.ToLower()).FirstOrDefault();
                                    //        if (PlaceHolder != null)
                                    //        {
                                    //            int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oOneClickStep.ID, 0, "Step", sDatabase);
                                    //        }
                                    //    }

                                    //    //Creating BO Component for Sub entity
                                    //    List<cNameValuePairs> nSubBOParams = new List<cNameValuePairs>();
                                    //    nSubBOParams.Add(new cNameValuePairs { sName = "BO", sValue = "{XIP|BO}" });
                                    //    nSubBOParams.Add(new cNameValuePairs { sName = "Group", sValue = "Details1" });
                                    //    nSubBOParams.Add(new cNameValuePairs { sName = "watchparam1", sValue = "-listinstance" });
                                    //    nSubBOParams.Add(new cNameValuePairs { sName = "register", sValue = "yes" });
                                    //    var oSubBOStep = AutoCreateQuestionSetStep("Step7 with BO Component", iQSDefinitionID, ServiceConstants.FormComponent, nSubBOParams, 0);
                                    //    if (NewStepLayout != null)
                                    //    {
                                    //        var PlaceHolder = NewStepLayout.LayoutDetails.Where(m => m.PlaceholderName.ToLower() == ServiceConstants.SubEntityDetailsArea.ToLower()).FirstOrDefault();
                                    //        if (PlaceHolder != null)
                                    //        {
                                    //            int MappingID = AutoCreateLayoutMappings(NewStepLayout.ID, PlaceHolder.PlaceHolderID, oSubBOStep.ID, 0, "Step", sDatabase);
                                    //        }
                                    //    }
                                }
                            }
                            else
                            {
                                List<XiLinkNVs> NVs = new List<XiLinkNVs>();
                                NVs.Add(new XiLinkNVs { Name = "StartAction", Value = "Search" });
                                NVs.Add(new XiLinkNVs { Name = "Output", Value = ServiceConstants.PopupOutputArea });
                                iXiLinkID = AutoCreateXiLink(oBODef.LabelName, i1ClickID, NVs, sDatabase);
                            }
                        }
                        if (i == 0)
                        {
                            XILinkIDs.Insert(0, iXiLinkID);
                            StructIDs.Insert(0, BO.ID);
                        }
                        else
                        {
                            XILinkIDs.Add(iXiLinkID);
                            StructIDs.Add(BO.ID);
                        }
                    }
                    i++;
                    //Menu to Show Popups
                }

                if (model.sSavingType.ToLower() == "generate" || !bIsMainBODone)
                {
                    //Create XiLink for Tabs with all XiLinks
                    //int TabXiLinkID = AutoCreateTabsXiLink(Tree.FirstOrDefault().sName, XILinkIDs, StructIDs, Tree.FirstOrDefault().ID, sDatabase);
                    //Create Dialog for Popup


                    //oLayout = dbContext.Layouts.Where(m => m.LayoutName.ToLower() == ServiceConstants.LeftTreeLayout.ToLower()).FirstOrDefault();
                    //Map XiLink to Layout
                    //AutoMapXiLinkstoLayout(ServiceConstants.LeftTreeLayout, null, iDialgID, TabXiLinkID, sDatabase);
                    //AutoCreateLayoutMappings(int iLayoutID, int iPlaceHolderID, int iXiLinkID, int iDialogID, string ContentType, string sDatabase)
                    //int LeftPaneXiLinkID = AutoCreateLeftPaneXiLink(Tree.FirstOrDefault().sBO, ServiceConstants.DescriptionGroup, false, sDatabase);


                    //AutoMapXiLinkstoLayout(ServiceConstants.LeftTreeLayout, ServiceConstants.PopupLeftPaneArea, iDialgID, LeftPaneXiLinkID, sDatabase);
                    // Create Dialog XILink and Map to 1-Click
                    AutoCreateDialogXiLinkMapTOCLick(Tree.FirstOrDefault().sName, iMain1Click, iDialgID, sDatabase);
                }


                //}

                foreach (var items in Tree.Where(m => m.bIsAutoCreateDone == false).ToList())
                {
                    var oBO = dbContext.XIStructure.Find(items.ID);
                    oBO.bIsAutoCreateDone = true;
                    dbContext.SaveChanges();
                }

                cBOUIDetails oBOUI = new cBOUIDetails();
                oBOUI.FKi1ClickIDXIGUID = iMain1Click;
                oBOUI.FKiBOID = model.FKiBOID;
                oBOUI.FKiStructureID = Tree.FirstOrDefault().ID;
                if (oLayout != null)
                {
                    oBOUI.iLayoutID = oLayout.ID;
                }
                oBOUI.StatusTypeID = 10;
                oBOUI.CreatedTime = DateTime.Now;
                oBOUI.CreatedBy = 1;
                oBOUI.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                oBOUI.UpdatedTime = DateTime.Now;
                oBOUI.UpdatedBy = 1;
                oBOUI.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.BOUIDetails.Add(oBOUI);
                dbContext.SaveChanges();

                return new VMCustomResponse() { ID = oBOUI.ID, Status = true, ResponseMessage = ServiceConstants.SuccessMessage };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion BOXIStructure

        #region BOUIDefaults

        public cBOUIDefaults GetBODefaults(int iBOID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cBOUIDefaults oDef = new cBOUIDefaults();
            oDef = dbContext.BOUIDefaults.Where(m => m.FKiBOID == iBOID).FirstOrDefault();
            if (oDef == null)
            {
                oDef = new cBOUIDefaults();
            }
            oDef.FKiBOID = iBOID;
            oDef.ddl1Clicks = Common.GetReportsDDL(sDatabase);
            oDef.ddlLayouts = Common.GetLayoutsDDL(iUserID, sOrgName, sDatabase);
            oDef.ddlStructures = Common.GetBOStructuresDDL(iBOID, sDatabase);
            oDef.ddlXIComponents = Common.GetXIComponentsDDL(iUserID, sOrgName, sDatabase);
            oDef.ddlPopups = Common.GetDialogsDDL(sDatabase);
            return oDef;
        }


        public VMCustomResponse SaveBODefaults(cBOUIDefaults model, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            cBOUIDefaults oDefaults = new cBOUIDefaults();
            if (model.ID > 0)
            {
                oDefaults = dbContext.BOUIDefaults.Find(model.ID);
            }
            oDefaults.FKiBOID = model.FKiBOID;
            oDefaults.iXIComponentID = model.iXIComponentID;

            List<XiLinkNVs> NVPairs = new List<XiLinkNVs>();
            if (model.iPopupID > 0)
            {
                XiLinkNVs oNV = new XiLinkNVs();
                oNV.Name = "StartAction";
                oNV.Value = "Dialog";
                NVPairs.Add(oNV);
                XiLinkNVs oNV1 = new XiLinkNVs();
                oNV1.Name = "DialogID";
                oNV1.Value = model.iPopupID.ToString();
                NVPairs.Add(oNV1);
                var XILinkID = AutoCreateXiLink(model.FKiBOID + " Default Popup", Guid.Empty, NVPairs, null, true);
                oDefaults.iPopupID = XILinkID;
            }
            oDefaults.iStructureID = model.iStructureID;
            oDefaults.i1ClickID = model.i1ClickID;
            oDefaults.iLayoutID = model.iLayoutID;
            oDefaults.StatusTypeID = model.StatusTypeID;
            oDefaults.UpdatedTime = DateTime.Now;
            oDefaults.UpdatedBy = model.UpdatedBy;
            oDefaults.UpdatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
            if (model.ID == 0)
            {
                oDefaults.CreatedBy = model.CreatedBy;
                oDefaults.CreatedTime = DateTime.Now;
                oDefaults.CreatedBySYSID = Dns.GetHostAddresses(Dns.GetHostName())[1].ToString();
                dbContext.BOUIDefaults.Add(oDefaults);
            }
            dbContext.SaveChanges();
            return new VMCustomResponse() { ID = oDefaults.ID, ResponseMessage = ServiceConstants.SuccessMessage, Status = true };
        }
        #endregion BOUIDefaults

        #region Audit
        public string CreateAuditTable(int BOID, int iOrgID, int iUserID, string sOrgName, string sDatabase, string CreatedByName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            BOs model = dbContext.BOs.Find(BOID);
            var sOrgDb = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase; //Common.GetUserDetails(iUserID, sOrgName, sDatabase).sAppName +"_Shared";
            int NewBOID = 0;
            bool bStatus = true;
            int iTableCreated = 1;
            int iTableUpdated = 1;
            string ReturnStatus = "";
            string SaveBOType = "Create";
            var sDTypeNotChngd = new List<string>();
            model.TableName = model.TableName + "_Audit";
            string cmdText = @"IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + model.TableName + "') SELECT 1 ELSE SELECT 0";
            var sBODataSources = oXIAPI.GetBODataSource(model.iDataSource, iOrgID, sDatabase, sOrgDb);
            SqlConnection SC = new SqlConnection(sBODataSources);
            SC.Open();
            SqlCommand cmds = new SqlCommand();
            cmds.Connection = SC;
            cmds = new SqlCommand(cmdText, SC);
            SqlCommand DateCheck = new SqlCommand(cmdText, SC);
            int x = Convert.ToInt32(DateCheck.ExecuteScalar());
            var lColDatatypes = new List<List<string>>();
            SC.Close();
            SqlConnection Conn = new SqlConnection(ModelDbContext.ConnectionString(sDatabase));
            var lColList = new List<string>();
            string sSplitVal1 = "";
            string sSplitVal2 = "";
            string sSplitVal3 = "";
            //string[] sColDetails = Regex.Split(model.ColName, "\r\n");
            List<string> ColDetails = new List<string>();
            foreach (var item in model.BOFields)
            {
                var ColDetailss = Regex.Split(item.Name, "\r\n");
                ColDetails.AddRange(ColDetailss);
            }

            if (ColDetails.Where(m => m.ToLower() == "id").FirstOrDefault() == null)
            {
                ColDetails.Add("ID");
            }
            foreach (string ColumnDetails in ColDetails)
            {
                var lColDatatypeList = new List<string>();

                if (ColumnDetails.ToLower() == "id")
                {
                    if (ColumnDetails.ToLower() == "ID".ToLower())
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("int IDENTITY(1,1) PRIMARY KEY");
                    }
                }
                else
                {
                    string sFirstLetter = ColumnDetails.Substring(0, 1);
                    if (sFirstLetter == "i")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("int NULL");
                    }
                    else if (sFirstLetter == "s")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("varchar(256) NULL");
                    }
                    else if (sFirstLetter == "d")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("datetime NULL");
                    }
                    else if (sFirstLetter == "r")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("float NULL");
                    }
                    else if (sFirstLetter == "n")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("nvarchar(MAX) NULL");
                    }
                    else if (sFirstLetter == "b")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("bit NULL");
                    }
                }
                string sFrstTwoLetter = ColumnDetails.Substring(0, 2);
                if (sFrstTwoLetter == "dt")
                {
                    lColDatatypeList.Add(ColumnDetails);
                    lColDatatypeList.Add("datetime NULL");
                }
                else if (sFrstTwoLetter == "FK")
                {
                    //contains foreign key details ex "FKiOrgID,Orgnaization,ID
                    string[] sSplitColDetails = Regex.Split(ColumnDetails, ",");
                    var sColName = sSplitColDetails[0];
                    var sPK_DataType = 0;
                    var sPK_Column = string.Empty;
                    foreach (var item in model.BOFields.Where(s => s.Name == sColName).ToList())
                    {
                        sPK_DataType = item.TypeID;
                        break;
                    }
                    string sFirstTwoLetters = sColName.Substring(0, 2);
                    if (sFirstTwoLetters == "FK")
                    {

                        if (sPK_DataType == 60 || sPK_DataType == 10)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("int NULL");
                        }
                        else if (sPK_DataType == 180)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("varchar(256) NULL");
                        }
                        else if (sPK_DataType == 150)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("datetime NULL");
                        }
                        else if (sPK_DataType == 90)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("float NULL");
                        }
                        else if (sPK_DataType == 210)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("nvarchar(MAX) NULL");
                        }
                        else if (sPK_DataType == 20)
                        {
                            lColDatatypeList.Add(ColumnDetails);
                            lColDatatypeList.Add("bit NULL");
                        }
                    }
                    else if (sFirstTwoLetters == "dt")
                    {
                        lColDatatypeList.Add(ColumnDetails);
                        lColDatatypeList.Add("datetime NULL");
                    }
                }
                lColDatatypes.Add(lColDatatypeList);
            }
            if (x == 0)
            //Table exixts
            {
                var lColDatatypeList = new List<string>();
                if (model.sAuditBOName == null)
                {
                    lColDatatypeList.Add("FKiInstanceID");
                    lColDatatypeList.Add("int NULL");
                    lColDatatypes.Add(lColDatatypeList);
                }
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("XICreatedBy");
                lColDatatypeList.Add("int NULL");
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("XICreatedWhen");
                lColDatatypeList.Add("datetime NULL");
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("XIUpdatedBy");
                lColDatatypeList.Add("int NULL");
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("XIUpdatedWhen");
                lColDatatypeList.Add("datetime NULL");
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("zXAuditCrtdBy");
                lColDatatypeList.Add("varchar(15) NULL");
                lColDatatypes.Add(lColDatatypeList);
                lColDatatypeList = new List<string>();
                lColDatatypeList.Add("zXAuditCrtdWhn");
                lColDatatypeList.Add("datetime NULL");
                lColDatatypes.Add(lColDatatypeList);

                string sQuery = "create table [" + model.TableName + "] (";
                for (var i = 0; i < lColDatatypes.Count(); i++)
                {
                    if (lColDatatypes[i].Count() > 0)
                    {
                        //check if has computed column
                        if (lColDatatypes[i][0] == "COMPUTED COLUMN")
                        {
                            if (sQuery.Contains(",,") == false)
                            {
                                sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
                            }
                            else
                            {
                                sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
                            }
                        }
                        else if (lColDatatypes[i].Count == 2)
                        {
                            var sColumn = lColDatatypes[i][0];
                            var sDataTyp = lColDatatypes[i][1];
                            string sFirstTwoLtr = sColumn.Substring(0, 2);
                            if (sFirstTwoLtr != "FK")
                            {
                                if (sQuery.Contains(",,") == false)
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                    }

                                }
                                else
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                    }
                                }
                            }//check FK
                            else
                            {
                                if (sColumn.Contains(","))
                                {
                                    string[] sSplitVal = Regex.Split(sColumn, ",");
                                    sSplitVal1 = sSplitVal[0];
                                    sSplitVal2 = sSplitVal[1];
                                    sSplitVal3 = sSplitVal[2];
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
                                        if (sSplitVal2 == "OrganizationClasses")
                                        {
                                            sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

                                        }
                                        else
                                        {
                                            string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                            string sColmnDtype = sSplitDtype[0];
                                            string sColmnNull = sSplitDtype[1];
                                            sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
                                        }
                                    }
                                    else
                                    {
                                        if (sSplitVal2 == "OrganizationClasses")
                                        {
                                            sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
                                        }
                                        else
                                        {
                                            string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                            string sColmnDtype = sSplitDtype[0];
                                            string sColmnNull = sSplitDtype[1];
                                            sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
                                        }
                                    }
                                }
                                else // if column has no "," then use the column and datatype directly as no primary key or table
                                {
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                    }
                                }
                            }
                        }
                        else if (lColDatatypes[i].Count == 4)
                        {
                            var sColumn = lColDatatypes[i][0];
                            var sDataTyp = lColDatatypes[i][1];
                            string sFirstTwoLtr = sColumn.Substring(0, 2);
                            if (sFirstTwoLtr == "dt")
                            {
                                if (sQuery.Contains(",,") == false)
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                    }

                                }
                                else
                                {
                                    if (sDataTyp.Contains("PRIMARY"))
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
                                    }
                                    else
                                    {
                                        sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                    }
                                }
                            }
                        }
                    }
                }
                bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
                if (bStrTwoCm == true)
                {
                    int position = sQuery.LastIndexOf(",,");
                    sQuery = sQuery.Remove(position);
                }
                bool bStrCm = sQuery.Trim().EndsWith(@",");
                if (bStrCm == true)
                {
                    int position = sQuery.LastIndexOf(',');
                    sQuery = sQuery.Remove(position);
                }
                sQuery = sQuery + ")";
                if (sQuery.EndsWith("()"))
                {

                }
                else
                {
                    var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, iOrgID, sDatabase, sOrgDb);
                    using (SqlConnection Con = new SqlConnection(sBODataSource))
                    {
                        Con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Connection = Con;
                        string sStructuredQryReplace = sQuery.Replace(",,,,", ",");
                        string sStructuredQry = sStructuredQryReplace.Replace(",,", ",");
                        cmd = new SqlCommand(sStructuredQry, Con);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            ReturnStatus = ServiceConstants.SuccessMessage;
                        }
                        catch (Exception Ex)
                        {
                            iTableCreated = 0;
                            bStatus = false;
                            ReturnStatus = ServiceConstants.ErrorMessage;
                        }
                        Conn.Close();
                    }
                }
            }
            else
            {
                SaveBOType = "Update";
                string sQuery = "alter table [" + model.TableName + "] add ";
                for (var i = 0; i < lColDatatypes.Count(); i++)
                {
                    if (lColDatatypes[i].Count() > 0)
                    {
                        //check if has computed column
                        if (lColDatatypes[i][0] == "COMPUTED COLUMN")
                        {
                            string sCheckComputdColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')) SELECT 0 ELSE SELECT 1";
                            SC.Open();
                            cmds.Connection = SC;
                            SqlCommand Sqlcmd = new SqlCommand(sCheckComputdColmn, SC);
                            int iExist = Convert.ToInt32(Sqlcmd.ExecuteScalar());
                            SC.Close();
                            if (iExist == 0)
                            {
                                if (sQuery.Contains(",,") == false)
                                {
                                    sQuery = sQuery + " " + lColDatatypes[i][1] + " " + lColDatatypes[i][2] + ",,";
                                }
                                else
                                {
                                    sQuery = sQuery + ",," + lColDatatypes[i][1] + " " + lColDatatypes[i][2];
                                }
                            }
                            else
                            {
                                //Get the defination for the computed column and check with the new definition
                                var sOldDefinition = "";
                                //Get the computed column definition
                                string sGetCmptdColDef = "SELECT definition FROM sys.computed_columns WHERE [name] = '" + lColDatatypes[i][1] + "' AND [object_id] = OBJECT_ID('" + model.Name + "')";
                                SC.Open();
                                cmds.Connection = SC;
                                SqlCommand SqlcmdColDef = new SqlCommand(sGetCmptdColDef, SC);
                                SqlDataReader DReaderChckDtype = SqlcmdColDef.ExecuteReader();
                                while (DReaderChckDtype.Read())
                                {
                                    sOldDefinition = DReaderChckDtype.IsDBNull(0) ? null : DReaderChckDtype.GetValue(0).ToString();
                                }
                                SC.Close();
                                var sNewDefinition = lColDatatypes[i][2].Replace("AS ", "");
                                var sAltrOldDefnEnd = "(" + sOldDefinition + ")";
                                //Compare old and new Definition
                                if (sAltrOldDefnEnd.ToLower() != sNewDefinition.ToLower())
                                {
                                    //update to new definition but here we need to drop the column and re add the column.
                                    var sDropComputedColumn = "ALTER TABLE [" + model.TableName + "] DROP COLUMN " + lColDatatypes[i][1];
                                    SC.Open();
                                    SqlCommand cmdDrop = new SqlCommand(sDropComputedColumn, SC);
                                    try
                                    {
                                        cmds.Connection = SC;
                                        cmdDrop.ExecuteNonQuery();
                                        Conn.Close();
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    var sUpdateComputedColumn = "ALTER TABLE [" + model.TableName + "] ADD " + lColDatatypes[i][1] + " AS " + sNewDefinition;
                                    SC.Open();
                                    SqlCommand cmd = new SqlCommand(sUpdateComputedColumn, SC);
                                    try
                                    {
                                        cmds.Connection = SC;
                                        cmd.ExecuteNonQuery();
                                        Conn.Close();
                                        bStatus = true;
                                        ReturnStatus = ServiceConstants.SuccessMessageComptdColumn;
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                                else
                                {
                                    // do nothing as both are same
                                }
                            }
                        }
                        else
                        {
                            var sColumn = lColDatatypes[i][0];
                            if (sColumn.Contains(','))
                            {
                                string[] sSplitVal = Regex.Split(sColumn, ",");
                                sColumn = sSplitVal[0];
                            }
                            var sDataTyp = lColDatatypes[i][1];
                            string sCheckColmn = @"IF NOT EXISTS(SELECT * FROM sys.columns WHERE [name] = '" + sColumn + "' AND [object_id] = OBJECT_ID('" + model.TableName + "')) SELECT 0 ELSE SELECT 1";

                            var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, iOrgID, sDatabase, sOrgDb);
                            SqlConnection SCs = new SqlConnection(sBODataSources);
                            SCs.Open();
                            SqlCommand ncmd = new SqlCommand();
                            ncmd.Connection = SCs;
                            ncmd = new SqlCommand(sCheckColmn, SCs);
                            SqlCommand DateChecks = new SqlCommand(sCheckColmn, SCs);
                            int iExist = Convert.ToInt32(ncmd.ExecuteScalar());
                            SC.Close();

                            if (iExist == 0)
                            {
                                string sFirstTwoLtr = sColumn.Substring(0, 2);
                                if (sFirstTwoLtr != "FK")
                                {
                                    if (sQuery.Contains(",,") == false)
                                    {
                                        if (sDataTyp.Contains("PRIMARY"))
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + " NOT NULL,,";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                        }

                                    }
                                    else
                                    {
                                        if (sDataTyp.Contains("PRIMARY"))
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp + " NOT NULL";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                        }
                                    }
                                }//check FK
                                else
                                {
                                    //try if the foriegn key is present patterm Foreign Key, table name
                                    if (lColDatatypes[i][0].Contains(","))
                                    {
                                        string[] sSplitVal = Regex.Split(lColDatatypes[i][0], ",");
                                        sSplitVal1 = sSplitVal[0];
                                        sSplitVal2 = sSplitVal[1];
                                        sSplitVal3 = sSplitVal[2];
                                        if (sQuery.Contains(",,") == false)
                                        {
                                            //classes comes from master data... while mapping classes to org.. i took masterdataid as classid
                                            if (sSplitVal2 == "OrganizationClasses")
                                            {
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + " " + sSplitVal1 + " " + sDataTyp + ",,";

                                            }
                                            else
                                            {
                                                string[] sSplitDtype = Regex.Split(sDataTyp, "_");
                                                string sColmnDtype = sSplitDtype[0];
                                                string sColmnNull = sSplitDtype[1];
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + " " + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull + ",,";
                                            }
                                        }
                                        else
                                        {
                                            if (sSplitVal2 == "OrganizationClasses")
                                            {
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + ",," + sSplitVal1 + " " + sDataTyp;
                                            }
                                            else
                                            {
                                                string[] sSplitDtype = Regex.Split(sDataTyp, " ");
                                                string sColmnDtype = sSplitDtype[0];
                                                string sColmnNull = sSplitDtype[1];
                                                //PersonID int FOREIGN KEY REFERENCES Persons(PersonID)
                                                sQuery = sQuery + ",," + sSplitVal1 + " " + sColmnDtype + " FOREIGN KEY REFERENCES " + sSplitVal2 + "(" + sSplitVal3 + ") " + sColmnNull;
                                            }
                                        }
                                    }
                                    else // if column has no "," then use the column and datatype directly as no primary key or table
                                    {
                                        if (sQuery.Contains(",,") == false)
                                        {
                                            sQuery = sQuery + " " + sColumn + " " + sDataTyp + ",,";
                                        }
                                        else
                                        {
                                            sQuery = sQuery + ",," + sColumn + " " + sDataTyp;
                                        }
                                    }
                                }
                            }
                            else
                            {//no datatype change for now
                                var sOld_Datatype = "";
                                var sOld_MxLength = "";
                                //check is the datatype is same
                                string sCheckColmnDatatype = "SELECT DATA_TYPE,CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + model.Name + "' AND COLUMN_NAME = '" + sColumn + "'";
                                SC.Open();
                                SqlCommand SqlcmdChckDtype = new SqlCommand(sCheckColmnDatatype, SC);
                                SqlDataReader DReaderCckDtype = SqlcmdChckDtype.ExecuteReader();
                                while (DReaderCckDtype.Read())
                                {
                                    sOld_Datatype = DReaderCckDtype.IsDBNull(0) ? null : DReaderCckDtype.GetValue(0).ToString();
                                    sOld_MxLength = DReaderCckDtype.IsDBNull(1) ? null : DReaderCckDtype.GetValue(1).ToString();
                                    //Check if Old datatype is primary key.
                                    string sCheckPrimaryKey = @"IF NOT EXISTS(SELECT* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS C JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE AS K ON C.TABLE_NAME = K.TABLE_NAME AND C.CONSTRAINT_CATALOG = K.CONSTRAINT_CATALOG AND C.CONSTRAINT_SCHEMA = K.CONSTRAINT_SCHEMA AND C.CONSTRAINT_NAME = K.CONSTRAINT_NAME WHERE C.CONSTRAINT_TYPE = 'PRIMARY KEY' AND K.COLUMN_NAME = '" + sColumn + "' AND K.TABLE_NAME = '" + model.Name + "') SELECT 0 ELSE SELECT 1";
                                    //Conn.Open();
                                    SqlCommand SqlcmdCheck = new SqlCommand(sCheckPrimaryKey, SC);
                                    int iIsPrimary = Convert.ToInt32(SqlcmdCheck.ExecuteScalar());
                                    //Conn.Close();
                                    if (iIsPrimary == 1)
                                    {
                                        sOld_Datatype = sOld_Datatype + " IDENTITY(1, 1) PRIMARY KEY";
                                    }
                                    else
                                    {
                                        if (sOld_MxLength != null)
                                        {
                                            sOld_Datatype = sOld_Datatype + "(" + sOld_MxLength + ")";
                                        }
                                    }

                                    var sNew_DataType = "";
                                    if (sDataTyp.Contains("PRIMARY") == true)
                                    {
                                        sNew_DataType = sDataTyp;
                                    }
                                    else if (sDataTyp.Contains(" NOT NULL"))
                                    {
                                        sNew_DataType = sDataTyp.Replace(" NOT NULL", "");
                                    }
                                    else if (sDataTyp.Contains(" NULL"))
                                    {
                                        sNew_DataType = sDataTyp.Replace(" NULL", "");
                                    }
                                    else
                                    {
                                        sNew_DataType = sDataTyp;
                                    }
                                    if (sDTypeNotChngd.Count == 0)
                                    {
                                        ReturnStatus = ServiceConstants.DataTypeChangeSuccess;
                                        bStatus = true;
                                    }
                                    else
                                    {
                                        ReturnStatus = ServiceConstants.DataTypeChangeError;
                                        bStatus = false;
                                    }
                                }
                                Conn.Close();
                            }
                        }//Not computed column
                    }
                }
                if (sQuery.EndsWith("add "))
                {

                }
                else
                {
                    var sBODataSource = oXIAPI.GetBODataSource(model.iDataSource, iOrgID, sDatabase, sOrgDb);
                    using (SqlConnection SCs = new SqlConnection(sBODataSources))
                    {
                        SCs.Open();
                        SqlCommand ncmd = new SqlCommand();
                        ncmd.Connection = SCs;
                        bool bStrTwoCm = sQuery.Trim().EndsWith(@",,");
                        if (bStrTwoCm == true)
                        {
                            int position = sQuery.LastIndexOf(",,");
                            sQuery = sQuery.Remove(position);
                        }
                        bool bStrCm = sQuery.Trim().EndsWith(@",");
                        if (bStrCm == true)
                        {
                            int position = sQuery.LastIndexOf(',');
                            sQuery = sQuery.Remove(position);
                        }
                        string sStructuredQryRemove = sQuery.Replace(",,,,", ",");
                        string sStructuredQry = sStructuredQryRemove.Replace(",,", ",");
                        SqlCommand cmd = new SqlCommand(sStructuredQry, SCs);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            ReturnStatus = ServiceConstants.SuccessMessage;
                            bStatus = true;
                        }
                        catch (Exception ex)
                        {
                            bStatus = false;
                            iTableUpdated = 0;
                            ReturnStatus = ServiceConstants.ErrorMessage;
                        }
                        if (iTableUpdated == 1)
                        {

                        }
                        SCs.Close();
                    }
                }
            }
            int ExistBOID = 0;
            int OldBOID = model.BOID;
            if (SaveBOType == "Update")
            {
                ExistBOID = dbContext.BOs.Where(m => m.Name == model.Name + "_Audit").Select(m => m.BOID).FirstOrDefault();
                model.BOID = ExistBOID;
                model.Name = model.Description = model.Name + "_Audit";
            }
            else
            {
                model.Name = model.Description = model.Name + "_Audit";
                model.BOID = 0;
            }
            model.AuditType = "Audit";
            model.FieldCount = lColDatatypes.Count();
            if (model.AuditType == "Audit")
            {
                ModelDbContext NewdbContext = new ModelDbContext();
                BOs bo = NewdbContext.BOs.Find(OldBOID);
                bo.sAuditBOName = model.Name;
                NewdbContext.SaveChanges();
            }
            if (model.sAuditBOName == null)
            {
                NewBOID = SaveBO(model, iUserID, sOrgName, sDatabase);
            }
            model.BOFields.Add(new BOFields { Name = "FKiInstanceID", LabelName = "FKiInstanceID", TypeID = 60, IsVisible = true, sXMLDataType = "zAutomatic", sPlaceHolder = "Please Enter ID", iOutputLength = 9, ForeignColumn = "id", sCase = "20", sLock = "20", sVirtualColumn = "20" });
            List<BOFields> ResolveBOFields = ResolveDataTypes(model.BOFields.ToList());
            if (SaveBOType == "Update")
            {
                ResolveBOFields.ToList().ForEach(m => { m.BOID = ExistBOID; m.CreatedByName = CreatedByName; });
            }
            else
            {
                ResolveBOFields.ToList().ForEach(m => { m.ID = 0; m.BOID = NewBOID; m.CreatedByName = CreatedByName; });
            }
            AddBOAttributes(ResolveBOFields, sDatabase, iUserID);
            return null;
        }

        public List<BOFields> ResolveDataTypes(List<BOFields> model)
        {
            foreach (var Type in model)
            {
                var TypeID = Type.TypeID;
                if (TypeID == 10)
                {
                    Type.DataType = "bigint";
                }
                else if (TypeID == 20)
                {
                    Type.DataType = "bit";
                }
                else if (TypeID == 40)
                {
                    Type.DataType = "decimal";
                }
                else if (TypeID == 60)
                {
                    Type.DataType = "int";
                }
                else if (TypeID == 90)
                {
                    Type.DataType = "float";
                }
                else if (TypeID == 130)
                {
                    Type.DataType = "datetime2";
                }
                else if (TypeID == 150)
                {
                    Type.DataType = "datetime";
                }
                else if (TypeID == 180)
                {
                    Type.DataType = "varchar";
                }
                else if (TypeID == 210)
                {
                    Type.DataType = "nvarchar";
                }
            }
            return model;
        }
        #endregion Audit

    }
}

