using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XIDNA.Models;
using System.Data.SqlClient;
using System.Configuration;
using XIDNA.ViewModels;

namespace XIDNA.Repository
{
    public class ContentRepository : IContentRepository
    {
        CommonRepository Common = new CommonRepository();
        public VMCustomResponse PostContent(ContentEditors model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            ContentEditors content = new ContentEditors();
            if (model.ID == 0)
            {
                string Content = "";
                if (model.Category == 2)
                {
                    Content = model.SMSContent;
                }
                else
                {
                    Content = model.Content;
                }
                if (model.Category != 3)
                {
                    Content = Content.Replace("&lt;", "<");
                    Content = Content.Replace("&gt;", ">");
                    Content = Content.Replace("&quot;", "\"");
                }
                content.Content = Content.ToString();
                content.OrganizationID = model.OrganizationID;
                content.Name = model.Name;
                content.Category = model.Category;
                content.Type = model.Type;
                content.BO = model.BO;
                content.iParentID = model.iParentID;
                content.bIsHavingAttachments = model.bIsHavingAttachments;
                content.bIsPaswordProtected = model.bIsPaswordProtected;
                content.iSurNamePasswordRange = model.iSurNamePasswordRange;
                content.iDOBPasswordRange = model.iDOBPasswordRange;
                content.sTemplateHeader = model.sTemplateHeader;
                content.iTypeofPDF = model.iTypeofPDF;
                content.sCSSFileName = model.sCSSFileName;
                content.FKiApplicationID = model.FKiApplicationID;
                content.sFrom = model.sFrom;
                content.sCC = model.sCC;
                content.sBCC = model.sBCC;
                content.FkiServerID = model.FkiServerID;
                content.bIsBCCOnly = model.bIsBCCOnly;
                content.dActiveFrom = model.dActiveFrom;
                content.dActiveTo = model.dActiveTo;
                dbContext.ContentEditors.Add(content);
                dbContext.SaveChanges();
                var ISs = model.ID;
                var news = content.ID;
            }
            else
            {
                string Content = model.Content;
                content = dbContext.ContentEditors.Find(model.ID);
                if (content.Category != 3)
                {
                    Content = Content.Replace("&lt;", "<");
                    Content = Content.Replace("&gt;", ">");
                    Content = Content.Replace("&quot;", "\"");
                }
                if (content.Category == 2)
                {
                    content.Content = model.SMSContent;
                }
                else
                {
                    content.Content = Content.ToString();
                }
                //content.Content = model.Content;
                content.sTemplateHeader = model.sTemplateHeader;
                content.bIsHavingAttachments = model.bIsHavingAttachments;
                content.bIsPaswordProtected = model.bIsPaswordProtected;
                content.iSurNamePasswordRange = model.iSurNamePasswordRange;
                content.iDOBPasswordRange = model.iDOBPasswordRange;
                content.iParentID = model.iParentID;
                content.iTypeofPDF = model.iTypeofPDF;
                content.sCSSFileName = model.sCSSFileName;
                content.Type = model.Type;
                content.Name = model.Name;
                content.BO = model.BO;
                content.sFrom = model.sFrom;
                content.sCC = model.sCC;
                content.sBCC = model.sBCC;
                content.FkiServerID = model.FkiServerID;
                content.FKiApplicationID = model.FKiApplicationID;
                content.bIsBCCOnly = model.bIsBCCOnly;
                content.dActiveFrom = model.dActiveFrom;
                content.dActiveTo = model.dActiveTo;
                dbContext.SaveChanges();
            }
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = model.ID, Status = true };
        }

        public int SaveTemplateCopy(int ID, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            ContentEditors NewReport = new ContentEditors();
            NewReport = Spdb.ContentEditors.Where(m => m.ID == ID).FirstOrDefault();
            NewReport.Name = NewReport.Name + " Copy";
            NewReport.OrganizationID = OrgID;
            Spdb.ContentEditors.Add(NewReport);
            Spdb.SaveChanges();
            if (NewReport.OrganizationID > 0)
            {
                var Org = dbCore.Organization.Find(NewReport.OrganizationID);
                var Targets = Spdb.Targets.Where(m => m.ID == ID).ToList();
                foreach (var items in Targets)
                {
                    items.ReportID = NewReport.ID;
                    Spdb.Targets.Add(items);
                    Spdb.SaveChanges();
                }
                var Schedulers = Spdb.Schedulers.Where(m => m.ID == ID).ToList();
                foreach (var items in Schedulers)
                {
                    items.ID = NewReport.ID;
                    Spdb.Schedulers.Add(items);
                    Spdb.SaveChanges();
                }
            }
            return NewReport.ID;
        }


        public ContentEditors GetContent(int ID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            ContentEditors content = new ContentEditors();
            try
            {
                content = dbContext.ContentEditors.Find(ID);
                if (content.Category == 2)
                {
                    content.SMSContent = content.Content;
                }
            }
            catch (Exception)
            {
            }
            return content;
        }
        public List<VMDropdowns> ContentList(string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var result = dbContext.ContentEditors.Where(m => m.OrganizationID == OrgID).Select(x => new VMDropdowns { ID = x.ID, Value = x.Name }).ToList();
            return result;
        }

        public List<VMDropDown> Contentdropdown(int Category, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<VMDropDown> Classes = new List<VMDropDown>();
            Classes = (from c in dbContext.ContentEditors.Where(m => m.OrganizationID == OrgID).Where(m => m.Category == Category).ToList()
                       select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            Classes.Insert(0, new VMDropDown
            {
                text = "--Select--",
                Value = 0
            });
            return Classes;
        }
        public List<VMDropDown> LeadFieldsList(int BOID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> EmailFields = new List<VMDropDown>();
            EmailFields = (from c in dbContext.BOFields.Where(m => m.BOID == BOID).Where(m => m.IsMail == true).ToList()
                           select new VMDropDown { text = c.Name, Value = c.ID }).ToList();
            return EmailFields;
        }
        //Getting Values from the Types Table 

        public List<VMDropDown> TypesList(string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> TypeFields = new List<VMDropDown>();
            TypeFields = (from c in dbContext.Types.Where(m => m.Name == "Template Type").ToList()
                          select new VMDropDown { text = c.Expression, Value = c.ID }).ToList();
            return TypeFields;
        }
        public List<VMNewLeads> GetUsersList(int Type, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            string TableName = EnumLeadTables.Leads.ToString();
            List<VMNewLeads> results = new List<VMNewLeads>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                if (Type == 0)
                {
                    cmd.CommandText = "select ID,sForeName,sLastName,sMob,sEmail from " + TableName;
                }
                else if (Type == 1)
                {
                    cmd.CommandText = "select ID,sForeName,sLastName,sMob,sEmail from " + TableName + " where sEmail is not null";
                }
                else
                {
                    cmd.CommandText = "select ID,sForeName,sLastName,sMob,sEmail from " + TableName + " where sMob is not null";
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    VMNewLeads model = new VMNewLeads();
                    model.ID = reader.GetInt32(0);
                    model.FirstName = (reader.IsDBNull(1) ? null : reader.GetValue(1).ToString()) + " " + (reader.IsDBNull(2) ? null : reader.GetValue(2).ToString());
                    model.Mobile = reader.IsDBNull(3) ? null : reader.GetValue(3).ToString();
                    model.Email = reader.IsDBNull(4) ? null : reader.GetValue(4).ToString();
                    results.Add(model);
                }
                Con.Close();
            }
            return results;
        }
        public List<VMDropDown> GetOrgImages(int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            if (Spdb.OrganizationImages != null && Spdb.OrganizationImages.Local.Count() > 0)
            {
                var Images = Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).ToList();
            }
            List<VMDropDown> AllImages = new List<VMDropDown>();
            if (Spdb.OrganizationImages != null && Spdb.OrganizationImages.Local.Count() > 0)
            {
                AllImages = (from c in Spdb.OrganizationImages.Where(m => m.OrganizationID == OrgID).ToList()
                             select new VMDropDown { text = c.FileName, Value = c.ID }).ToList();
            }
            var Logo = dbCore.Organization.Where(m => m.ID == OrgID).FirstOrDefault();
            if (Logo.Logo != null)
            {
                AllImages.Insert(0, new VMDropDown
                {
                    text = Logo.Logo,
                    Value = Logo.ID
                });
            }

            return AllImages;
        }

        public DTResponse GetListOfTemplates(jQueryDataTableParamModel param, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var fkiApplicationID = Common.GetUserDetails(iUserID, sOrgName, sDatabase).FKiApplicationID;
            DataContext Spdb = new DataContext(sOrgDB);
            IQueryable<ContentEditors> AllTemplates;
            AllTemplates = dbContext.ContentEditors.Where(m => m.OrganizationID == OrgID).Where(m => m.FKiApplicationID == fkiApplicationID || fkiApplicationID == 0);
            string sortExpression = ServiceConstants.SortExpression;
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                AllTemplates = AllTemplates.Where(m => m.Name.Contains(param.sSearch) || m.ID.ToString().Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllTemplates.Count();
            AllTemplates = QuerableUtil.GetResultsForDataTables(AllTemplates, "", sortExpression, param);
            var clients = AllTemplates.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            var TypesListContent = TypesList(sDatabase);
            result = from c in clients
                     select new[] {
                                (i++).ToString(),Convert.ToString(c.ID),c.Name, c.Category.ToString(), TypesListContent.Where(m=>m.Value == c.Type).Select(m=>m.text).FirstOrDefault(), "","",""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public List<ContentEditors> GetContentList(string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            List<ContentEditors> users = new List<ContentEditors>();
            return dbContext.ContentEditors.ToList();
        }
        public List<VMLeadEmail> GetLeadsData(string Users, List<string> Columns, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            List<VMLeadEmail> Emails = new List<VMLeadEmail>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                var Cols = "";
                var sColData = new List<string>();
                if (Columns.Count() > 0)
                {
                    foreach (var items in Columns)
                    {
                        string ColName = dbContext.BOGroupFields.Where(m => m.GroupName == items).Select(m => m.BOFieldNames).FirstOrDefault();
                        sColData.Add(ColName);
                        //Cols = Cols + ColName + ", ";
                    }
                    //Cols = Cols.Substring(0, Cols.Length - 2);
                }
                List<int> leadids = new List<int>();
                var data = (Users.Split(',').ToList());
                string IDs = "";
                foreach (var res in data)
                {
                    VMLeadEmail mail = new VMLeadEmail();
                    List<List<string>> Data = new List<List<string>>();
                    foreach (var items in sColData)
                    {
                        IDs = IDs + "ID=" + res + " OR ";
                        leadids.Add(Convert.ToInt32(res));
                        cmd.CommandText = "select sEmail from " + EnumLeadTables.Leads.ToString() + " WHERE ID=" + res;
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            mail.Email = reader.GetString(0);
                        }
                        reader.Close();
                        if (items != null)
                        {
                            if (items.Length != 0)
                            {
                                cmd.CommandText = "select " + items + " from " + EnumLeadTables.Leads.ToString() + " WHERE ID=" + res;
                                reader = cmd.ExecuteReader();
                                List<string> results = new List<string>();
                                int count = reader.FieldCount;
                                string[] rows = new string[count];
                                while (reader.Read())
                                {
                                    List<string> values = new List<string>();
                                    for (int i = 0; i < count; i++)
                                    {
                                        results.Add(reader.IsDBNull(i) ? null : reader.GetValue(i).ToString());
                                    }
                                }
                                Data.Add(results);
                            }
                        }
                        reader.Close();
                    }
                    mail.Data = Data;
                    Emails.Add(mail);

                }
                Con.Close();
            }
            return Emails;

        }
        public List<VMLeadEmail> GetLead(int? LeadID, string Users, List<string> Columns, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            List<VMLeadEmail> lDatas = new List<VMLeadEmail>();
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                var Cols = "";
                var sColData = new List<string>();
                if (Columns.Count() > 0)
                {
                    foreach (var items in Columns)
                    {
                        string ColName = dbContext.BOGroupFields.Where(m => m.GroupName == items).Select(m => m.BOFieldNames).FirstOrDefault();
                        sColData.Add(ColName);
                        //Cols = Cols + ColName + ", ";
                    }
                    //Cols = Cols.Substring(0, Cols.Length - 2);
                }
                List<int> leadids = new List<int>();
                var data = (Users.Split(',').ToList());
                string IDs = "";
                List<List<string>> Data = new List<List<string>>();
                VMLeadEmail MData = new VMLeadEmail();
                foreach (var res in data)
                {
                    foreach (var items in sColData)
                    {

                        IDs = IDs + "ID=" + res + " OR ";
                        leadids.Add(Convert.ToInt32(res));
                        if (items != null)
                        {
                            if (items.Length != 0)
                            {
                                cmd.CommandText = "select " + items + " from " + EnumLeadTables.Leads.ToString() + " WHERE ID=" + res;
                                SqlDataReader reader = cmd.ExecuteReader();
                                List<string> results = new List<string>();
                                int count = reader.FieldCount;
                                string[] rows = new string[count];
                                while (reader.Read())
                                {
                                    List<string> values = new List<string>();
                                    for (int i = 0; i < count; i++)
                                    {
                                        if (reader.GetValue(i).ToString() != "")
                                        {
                                            results.Add(reader.GetValue(i).ToString());
                                        }

                                    }
                                }
                                Data.Add(results);
                                reader.Close();
                            }
                        }
                    }
                    MData.Data = Data;
                    lDatas.Add(MData);
                }
                Con.Close();
            }
            return lDatas;
        }

        //saving the Email and sms Values

        public int SaveOutbounds(Outbounds Content, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            string EmailID = ""; string MobileNumber = "";
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(sOrgDB);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "select sEmail, sMob from " + EnumLeadTables.Leads.ToString() + " Where ID =" + Content.Users;
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    EmailID = reader.IsDBNull(0) ? null : reader.GetValue(0).ToString();
                    MobileNumber = reader.IsDBNull(1) ? null : reader.GetValue(1).ToString();
                }
                Con.Close();
            }
            Outbounds Ob = new Outbounds();

            if (Content.ID != 0)
            {
                Ob.Type = Content.Type;
                Ob.LeadID = Content.Users;
                Ob.TemplateID = Content.ID;
                Ob.Attachment = Content.Attachment;
                if (Content.Cc.Length > 0)
                {
                    Ob.Cc = Content.Cc;
                }
                else
                {
                    Ob.Cc = null;
                }
                Ob.Email = EmailID;
                Ob.Mobile = MobileNumber;
                Ob.OrganizationID = OrgID;
                Ob.SourceID = 0;
                Spdb.Outbounds.Add(Ob);
                Spdb.SaveChanges();
                return Content.ID;
            }
            else
            {
                return 0;
            }
        }

        //Getting the Attachment Values

        public Outbounds GetAttachment(int ID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var result = Spdb.Outbounds.Where(m => m.ID == ID).SingleOrDefault();
            return result;
        }

        //Getting the Server Deatils from the database

        public List<IOServerDetails> ServerDetails(int Type, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var SDetails = dbContext.IOServerDetails.Where(m => m.Type == Type).Where(m => m.OrganizationID == OrgID).ToList();
            return SDetails;
        }

        //Checking the Title

        public bool IsExistsTitle(string Name, int ID, int Category, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var ContentEditors = dbContext.ContentEditors.ToList();
            ContentEditors CE = ContentEditors.Where(m => m.OrganizationID == OrgID).Where(m => m.Category == Category).Where(m => m.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (ID == 0)
            {
                if (CE != null)
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
                if (CE != null)
                {
                    if (ID == CE.ID)
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
        public Organizations GetOrganizations(int ID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            ModelDbContext Spdb = new ModelDbContext(sOrgDB);
            Organizations OrgList = new Organizations();
            ID = 5;
            OrgList = dbContext.Organization.Find(ID);
            OrgList.AggregationsList = Spdb.Aggregations.Where(X => X.FKiQSInstanceID == 258).ToList();
            return OrgList;
        }
    }
}
