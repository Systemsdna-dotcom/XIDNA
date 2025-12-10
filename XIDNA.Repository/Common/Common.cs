using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Web;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
namespace XIDNA.Repository
{
    public class Common
    {
        CommonRepository Commons = new CommonRepository();
        public string GetSubUsers(int iUserID, int OrganizationID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (iUserID > 0)
            {
                var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                int UserRoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
                string Locations = dbCore.XIAppUsers.Where(m => m.UserID == iUserID).Select(m => m.sLocation).FirstOrDefault();
                var rolename = dbCore.XIAppRoles.Where(m => m.RoleID == UserRoleID).Select(m => m.sRoleName).FirstOrDefault();
                if (rolename != EnumRoles.Admin.ToString())
                {
                    Locations = dbCore.XIAppUsers.Where(m => m.UserID == iUserID).Select(m => m.sLocation).FirstOrDefault();
                }
                else
                {
                    var allLocations = Spdb.OrganizationLocations.Where(m => m.OrganizationID == OrganizationID).Select(m => m.ID).ToList();
                    if (allLocations.Count > 0)
                    {
                        foreach (var items in allLocations)
                        {
                            Locations = Locations + items + ", ";
                        }
                        Locations = Locations.Substring(0, Locations.Length - 2);
                    }
                }
                var roles = GetSubRoles(UserRoleID, sDatabase);
                string UserIDs = Convert.ToString(iUserID) + ", ";
                foreach (var role in roles)
                {
                    List<int> ChildUsers = dbCore.XIAppUserRoles.Where(m => m.RoleID == role).Select(m => m.UserID).ToList();
                    foreach (var user in ChildUsers)
                    {
                        UserIDs = UserIDs + user + ", ";
                        //string userloc = dbContext.AspNetUsers.Where(m => m.Id == user).Select(m => m.Location).FirstOrDefault();
                        //if (rolename == "Admin")
                        //{
                        //    if (Locations != null)
                        //    {
                        //        if (Locations.Contains(userloc))
                        //        {

                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    if (Locations != null)
                        //    {
                        //        if (Locations.Contains(userloc))
                        //        {
                        //            UserIDs = UserIDs + user + ", ";
                        //        }
                        //    }
                        //}
                    }
                }
                UserIDs = UserIDs.Substring(0, UserIDs.Length - 2);
                return UserIDs;
            }
            else
            {
                return null;
            }
        }
        public List<int> GetSubRoles(int RoleID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<int> SubRolesIDs = new List<int>();
            cXIAppRoles Role = dbCore.XIAppRoles.Where(s => s.RoleID == RoleID).FirstOrDefault();
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
        public List<int> GetParentRoles(int RoleID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<int> lst = new List<int>();
            //lst.Add(RoleID);
            int? pid = dbCore.XIAppRoles.Find(RoleID).iParentID;
            if (pid != 2)
            {
                lst.Add(Convert.ToInt32(pid));
                do
                {
                    pid = dbCore.XIAppRoles.Find(pid).iParentID;
                    if (pid != 2)
                    {
                        lst.Add(Convert.ToInt32(pid));
                    }
                    else
                    {
                        lst.Add(Convert.ToInt32(pid));
                    }
                } while (pid != 2);

            }
            return lst.OrderBy(m => m).ToList();
        }

        public int SendUserRegisterMail(int iUserID, string EmailID, string Type, string sDatabase, int OrgID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var res = GetContent(iUserID, sOrgName, sDatabase, OrgID, Type);
            var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
            //string field = res.Content;            
            //var today = System.DateTime.Now;
            //var tomorrow = System.DateTime.Now.AddDays(1);
            //var yesterday = System.DateTime.Now.AddDays(-1);
            List<string> Columns = new List<string>();
            foreach (Match m in Regex.Matches(res.Content, GetFieldsRgx))
            {
                Columns.Add(m.Value);
            }
            var ClientID = GetClientID(iUserID, OrgID, EmailID, sDatabase, sOrgName);

            //var leadsdata = ContentRepository.GetLeadsData(Users, Columns, User.Identity.GetDatabaseName());
            //string[] s = Users.Split(',');
            //path to save the attachment in the project folder
            string physicalPath = "", str = "", sFileName = "";
            //if (UploadFile != null)
            //{
            //    physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            //    str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\Attachment";
            //    //logger.Info(str);
            //    UploadFile.SaveAs(str + "\\" + "Attachment" + ".pdf");
            //    sFileName = "Attachment" + ".pdf";
            //    Content.Attachment = sFileName;
            //}
            //for (int i = 0; i < s.Count(); i++)
            //{
            //    Content.Cc = Cc;
            //    Content.Users = Convert.ToInt32(s[i]);
            //    //var outbounds = ContentRepository.SaveOutbounds(Content, User.Identity.GetDatabaseName(), User.Identity.GetOrganizationID());
            //}

            try
            {
                string Link = "";

                //Getting the Server Details from the Database                    
                var sDetails = GetServerDetails(1, OrgID, sDatabase);
                string usern = "", pass = "", sender = "", security = "", serverName = "";
                int port = 0;
                foreach (var items in sDetails)
                {
                    usern = items.UserName;
                    pass = items.Password;
                    sender = items.FromAddress;
                    serverName = items.ServerName;
                    port = items.Port;
                    security = items.Security;
                }
                string username = HttpUtility.UrlEncode(usern);
                string password = HttpUtility.UrlEncode(pass);
                string emailSubject = "Welcome Email";
                string messageBody = "";
                string field = res.Content;
                foreach (var items in Columns)
                {
                    if (items == "Registeration Link")
                    {
                        Link = "<a href=\"http://192.168.7.7/InsuranceWallet/account/register\">Register</a>";
                        field = field.Replace("{{" + items + "}}", Link);
                    }
                    else
                    {
                        Link = "<a href=\"http://192.168.7.7/InsuranceWallet/account/register\">Activate</a>";
                        field = field.Replace("{{" + items + "}}", Link);
                    }
                }
                //foreach (var user in leadsdata)
                //{

                //int n = 0;
                //foreach (var items in user.Data)
                //{
                //    string Con = "";
                //    foreach (var item in items)
                //    {
                //        if (item != null)
                //        {
                //            Con = Con + item + ",  ";
                //        }
                //    }
                //    Con = Con.Substring(0, Con.Length - 2);
                //    Con = Con.TrimEnd(',');

                //    n++;

                //    //field = field.Replace("{{" + Columns[n] + "}}", items);
                //    //n++;
                //}
                //string DateFields = field.Replace("((Today))", today.ToString()).Replace("((Tomorrow))", tomorrow.ToString()).Replace("((Yesterday))", yesterday.ToString());
                messageBody = field;
                List<string> ContentIds = new List<string>();
                List<string> Paths = new List<string>();
                string pattern = "(?<=src=\")[^,]+?(?=\")";
                string input = messageBody;
                if (messageBody.IndexOf("src=") >= 0)
                {
                    int i = 1;
                    foreach (Match m in Regex.Matches(input, pattern))
                    {
                        physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                        Paths.Add(physicalPath + m.Value);
                        ContentIds.Add("ContentID" + i);
                        messageBody = messageBody.Replace(m.Value, "cid:ContentID" + i);
                        i++;
                    }
                }
                MailMessage msg = new MailMessage();
                //checking that if there exist a Cc                                                     
                //if (Cc != "" && Cc != String.Empty && Cc != null)
                //{
                //    msg.CC.Add(Cc);
                //}
                //checking that if there exists BCC
                //if (BCC != "" && BCC != null)
                //{
                //    msg.Bcc.Add(BCC);
                //}
                msg.To.Add(EmailID);
                msg.From = new MailAddress(sender);
                msg.Subject = "Welcome Mail";
                string html = @"<html><body>" + messageBody + "</body></html>";
                AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
                //for (int j = 0; j < Paths.Count(); j++)
                //{
                //    string Path = Paths[j];
                //    string CntID = ContentIds[j];
                //    LinkedResource yourPictureRes = new LinkedResource(Path, MediaTypeNames.Image.Jpeg);
                //    yourPictureRes.ContentId = CntID;
                //    altView.LinkedResources.Add(yourPictureRes);
                //}
                msg.AlternateViews.Add(altView);
                string Attach = "";
                //if (UploadFile != null)
                //{
                //    //Getting Attachment file
                //    var att = ContentRepository.GetAttachment(154);
                //    physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                //    str = physicalPath.Substring(0, physicalPath.Length) + "\\Content\\Attachment";
                //    sFileName = (str + "\\" + "Attachment" + ".pdf");

                //    Attach = sFileName;
                //    msg.Attachments.Add(new Attachment(Attach));
                //}
                msg.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = serverName;
                smtp.Port = port;
                //for gmail
                smtp.EnableSsl = false;
                smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
                smtp.Send(msg);
                //}
            }
            catch (Exception e)
            {

            }
            return 0;
        }

        private int GetClientID(int iUserID, int OrgID, string EmailID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var ClientID = Spdb.LeadClients.Where(m => m.Email == EmailID).Select(m => m.ID).FirstOrDefault();
            return ClientID;
        }

        private List<IOServerDetails> GetServerDetails(int Type, int OrgID, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var SDetails = dbContext.IOServerDetails.Where(m => m.Type == Type && m.Category == 1).Where(m => m.OrganizationID == OrgID).ToList();
            return SDetails;
        }

        private ContentEditors GetContent(int iUserID, string sOrgName, string sDatabase, int OrgID, string Type)
        {
            ModelDbContext dbContext = new ModelDbContext();
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            ContentEditors content = new ContentEditors();
            try
            {
                if (Type == "Register")
                {
                    content = Spdb.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Name == "User Registration").FirstOrDefault();
                }
                else
                {
                    content = Spdb.ContentEditors.Where(m => m.OrganizationID == OrgID && m.Name == "User Activation").FirstOrDefault();
                }

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

        //Policies...Start
        public List<VMDropDown> ProductList(int iUserID, int OrgID, int ddlVal, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            List<VMDropDown> AllProducts = new List<VMDropDown>();
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            AllProducts = (from c in Spdb.WalletProducts.Where(m => m.OrganizationID == OrgID && m.Type == ddlVal).ToList()
                           select new VMDropDown { Value = c.ID, text = c.Name }).ToList();
            return AllProducts;
        }
        //Policies...End
        public VMQueryPreview GetHeadings(Guid ReportID, string sDatabase, int OrgID, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            VMResultList vmquery = new VMResultList();
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            Reports query = dbContext.Reports.Where(x => x.XIGUID == ReportID).Select(t => t).FirstOrDefault();
            int BOID = query.BOID;
            List<VMDropDown> KeyPositions = new List<VMDropDown>();
            Common Com = new Common();
            var FromIndex = query.Query.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            var SelectQuery = query.Query.Substring(0, FromIndex);
            var Keys = ServiceUtil.GetForeginkeyValues(SelectQuery);
            List<string> Headings = new List<string>();
            List<string> Formatting = new List<string>();
            List<string> Scripts = new List<string>();
            if (query.SelectFields == null)
            {
                List<string> SelectFields = Regex.Replace(SelectQuery, "select ", "", RegexOptions.IgnoreCase).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in SelectFields)
                {
                    if (items.IndexOf(" as ", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        string Head = Regex.Split(items, " as ", RegexOptions.IgnoreCase)[0];
                        Headings.Add(Head);
                    }
                    else
                    {
                        Headings.Add(items);
                    }
                }
            }
            else
            {
                Headings = query.SelectFields.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
            }
            List<string> AllHeadings = new List<string>();
            var str1 = "";
            if (Headings.Contains("ID") == false)
            {
                //str1 = "No";
                //Headings.Insert(0, "ID");
                vmquery.IDExists = false;
            }
            else
            {
                vmquery.IDExists = true;
            }
            string allfields = "";
            var groupfieldseditquery2 = "";
            var groupfieldseditquery5 = "";
            if (str1 == "No")
            {
                //var allfields1 = (query.Query).Insert(7, " ID, ");
                //allfields = (query.Query).Insert(7, " ID, ");
                //if (allfields.Contains("ORDER BY") == true && allfields.Contains("GROUP BY") == true)
                //{
                //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY", "ORDER BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //}
                //else if (allfields.Contains("GROUP BY") == false)
                //{
                //    allfields = allfields1;
                //}
                //else
                //{
                //    groupfieldseditquery2 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //    groupfieldseditquery5 = allfields.Split(new[] { "GROUP BY" }, StringSplitOptions.RemoveEmptyEntries)[1];
                //}
                //if (groupfieldseditquery2 != "")
                //{
                //    groupfieldseditquery2 = groupfieldseditquery2 + ", " + "ID" + " ";
                //    allfields = allfields.Replace(groupfieldseditquery5, groupfieldseditquery2);
                //}
                allfields = query.Query;
            }
            else
                allfields = query.Query;
            int FKPosition = 0;
            var BoFileds = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
            var MapFields = Spdb.MappedFields.Where(m => m.ClassID == query.Class && m.OrganizationID == OrgID).ToList();
            List<string> IsMouseOver = new List<string>();
            List<string> MouseOverColumns = new List<string>();
            foreach (var items in Headings)
            {
                if (items.Contains('{'))
                {
                    string id = items.Substring(1, items.Length - 2);
                    int gid = Convert.ToInt32(id);
                    string groupid = Convert.ToString(gid);
                    BOGroupFields fields = dbContext.BOGroupFields.Find(gid);
                    allfields = allfields.Replace("{" + groupid + "}", fields.BOSqlFieldNames);
                    if (fields.IsMultiColumnGroup)
                    {
                        List<string> fieldnames = fields.BOFieldNames.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        foreach (var names in fieldnames)
                        {
                            var Fild = BoFileds.Where(m => m.Name.Equals(names, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                            AllHeadings.Add(Fild.LabelName);
                            Formatting.Add(Fild.Format);
                            Scripts.Add(Fild.Script);
                            MouseOverColumns.Add(Fild.FKTableName);
                            KeyPositions.AddRange((from c in Keys.Where(m => m.text == names) select new VMDropDown { text = names, Value = FKPosition }));
                            FKPosition++;
                        }
                    }
                    else
                    {
                        AllHeadings.Add(fields.GroupName);
                        MouseOverColumns.Add("");
                        Formatting.Add(null);
                        Scripts.Add(null);
                        FKPosition++;
                    }
                }
                else if (items.IndexOf(" AS ", StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    var Ognl = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[0];
                    string aliasname = MapFields.Where(m => m.AddField == Ognl).Select(m => m.FieldName).FirstOrDefault();
                    var fieldname = Regex.Split(items, " AS ", RegexOptions.IgnoreCase)[1];
                    if (aliasname == null)
                    {
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
                    var BoFiled = BoFileds.Where(m => m.Name == Ognl).FirstOrDefault();
                    Formatting.Add(BoFiled.Format);
                    Scripts.Add(BoFiled.Script);
                    MouseOverColumns.Add(BoFiled.FKTableName);
                    KeyPositions.AddRange((from c in Keys.Where(m => m.text == Ognl) select new VMDropDown { text = Ognl, Value = FKPosition }));
                    FKPosition++;
                }
                else
                {
                    string aliasname = "";
                    if (OrgID != 0)
                    {
                        aliasname = MapFields.Where(m => m.AddField == items).Select(m => m.FieldName).FirstOrDefault();
                        if (aliasname != null)
                        {
                            MouseOverColumns.Add("");
                            AllHeadings.Add(aliasname);
                            Formatting.Add(null);
                            Scripts.Add(null);
                        }
                    }
                    else
                    {
                        var BoFiled = BoFileds.Where(m => m.Name == items).FirstOrDefault();
                        aliasname = BoFiled.LabelName;
                        Formatting.Add(BoFiled.Format);
                        Scripts.Add(BoFiled.Script);
                        MouseOverColumns.Add(BoFiled.FKTableName);
                        AllHeadings.Add(aliasname);
                    }
                    if (aliasname == null)
                    {
                        var BoFiled = BoFileds.Where(m => m.Name == items).FirstOrDefault();
                        aliasname = BoFiled.LabelName;
                        Formatting.Add(BoFiled.Format);
                        Scripts.Add(BoFiled.Script);
                        MouseOverColumns.Add(BoFiled.FKTableName);
                        AllHeadings.Add(aliasname);
                    }
                    KeyPositions.AddRange((from c in Keys.Where(m => m.text == items) select new VMDropDown { text = items, Value = FKPosition }));
                    FKPosition++;
                }
            }
            VMQueryPreview Preview = new VMQueryPreview();
            Preview.Headings = AllHeadings;
            Preview.IsPopup = query.IsRowClick;
            Preview.ActionType = query.OnRowClickType;
            Preview.ActionReportID = query.OnRowClickValue;
            Preview.ResultListDisplayType = query.ResultListDisplayType;
            int RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == iUserID).Select(m => m.RoleID).FirstOrDefault();
            Preview.QueryIcon = dbContext.UserReports.Where(m => m.FKi1ClickIDXIGUID == ReportID).Where(m => m.RoleID == RoleID).Select(m => m.Icon).FirstOrDefault();
            Preview.QueryName = query.Name;
            Preview.Query = allfields;
            Preview.FKPositions = KeyPositions;
            var Group = dbContext.BOGroupFields.Where(m => m.BOID == BOID && m.GroupName == "Filter Group").Select(m => m.BOFieldNames).FirstOrDefault();
            if (Group != null)
            {
                var FilterGroup = Group.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                var BOFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
                List<string> FilterFileds = new List<string>();
                Dictionary<string, string> myDict = new Dictionary<string, string>();
                foreach (var items in FilterGroup)
                {
                    myDict[BOFields.Where(m => m.Name == items).Select(m => m.LabelName).FirstOrDefault()] = items;
                }
                Preview.FilterGroup = myDict;
            }
            Preview.IsFilterSearch = query.IsFilterSearch;
            Preview.Rows = new List<string[]>();
            Preview.IsMouseOverColumn = IsMouseOver;
            Preview.MouserOverColum = MouseOverColumns;
            Preview.Formats = Formatting;
            Preview.Scripts = Scripts;
            Preview.XIGUID = ReportID;
            return Preview;
        }
        public string GetReportColumnsWhereCondition(int iUserID, int? ReportID, string ReportColumns, int OrgID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            string WhereString = "";
            //var Report = dbContext.Reports.Find(ReportID);
            //var CellClickCols = Report.OnClickCell;
            if (ReportColumns.IndexOf("and", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var Cols = ReportColumns.Split(new string[] { " AND " }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (var items in Cols)
                {
                    var ColsValues = items.Split('=').ToList();
                    if (ColsValues[0] == "FKiSourceID")
                    {
                        var Name = ColsValues[1];
                        var SorID = Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID && m.Name == Name).Select(m => m.ID).FirstOrDefault();
                        WhereString = WhereString + "FKiSourceID=" + SorID + " AND ";
                    }
                    else if (ColsValues[0] == "FKiLeadClassID")
                    {
                        var classs = ColsValues[1];
                        var ClassID = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID && m.Class == classs).Select(m => m.ClassID).FirstOrDefault();
                        WhereString = WhereString + "FKiLeadClassID=" + ClassID + " AND ";
                    }
                    else if (ColsValues[0] == "iStatus")
                    {
                        var status = ColsValues[1];
                        var StatusID = Spdb.Stages.Where(m => m.Name == status).Select(m => m.ID).FirstOrDefault();
                        WhereString = WhereString + "iStatus=" + StatusID + " AND ";
                    }
                    else if (ColsValues[0] == "UserID")
                    {
                        var UserName = ColsValues[1];
                        var UserID = dbCore.XIAppUsers.Where(m => m.sFirstName == UserName).Select(m => m.UserID).FirstOrDefault();
                        WhereString = WhereString + "UserID=" + UserID + " AND ";
                    }
                }
                WhereString = WhereString.Substring(0, WhereString.Length - 5);
            }
            else
            {
                var ColsValues = ReportColumns.Split('=').ToList();
                if (ColsValues[0] == "FKiSourceID")
                {
                    var Name = ColsValues[1];
                    var SorID = Spdb.OrganizationSources.Where(m => m.OrganizationID == OrgID && m.Name == Name).Select(m => m.ID).FirstOrDefault();
                    WhereString = WhereString + "FKiSourceID=" + SorID;
                }
                else if (ColsValues[0] == "FKiLeadClassID")
                {
                    var classs = ColsValues[1];
                    var ClassID = Spdb.OrganizationClasses.Where(m => m.OrganizationID == OrgID && m.Class == classs).Select(m => m.ClassID).FirstOrDefault();
                    WhereString = WhereString + "FKiLeadClassID=" + ClassID;
                }
                else if (ColsValues[0] == "iStatus")
                {
                    var status = ColsValues[1];
                    var StatusID = Spdb.Stages.Where(m => m.Name == status).Select(m => m.ID).FirstOrDefault();
                    WhereString = WhereString + "iStatus=" + StatusID;
                }
                else if (ColsValues[0] == "UserID")
                {
                    var UserName = ColsValues[1];
                    var UserID = dbCore.XIAppUsers.Where(m => m.sFirstName == UserName).Select(m => m.UserID).FirstOrDefault();
                    WhereString = WhereString + "UserID=" + UserID;
                }
            }
            return WhereString;
        }

        public int SendMail(int iUserID, int OrgID, string Attachment, int TemplateID, string MsgBody, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var UserDetails = dbCore.XIAppUsers.Find(iUserID);
            var sDetails = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID && m.Type == 1 && m.Category == 1).FirstOrDefault();
            string usern = "", pass = "", sender = "", security = "", serverName = "";
            int port = 0;
            usern = sDetails.UserName;
            pass = sDetails.Password;
            sender = sDetails.FromAddress;
            serverName = sDetails.ServerName;
            port = sDetails.Port;
            security = sDetails.Security;
            string OrgiMesg = "";
            string username = HttpUtility.UrlEncode(usern);
            string password = HttpUtility.UrlEncode(pass);
            string messageBody = "";
            if (TemplateID > 0)
            {
                var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
                DataContext Spdb = new DataContext(sOrgDB);
                var Template = Spdb.ContentEditors.Where(m => m.ID == TemplateID).FirstOrDefault();
                messageBody = Template.Content;
                OrgiMesg = Template.Content;
            }
            if (MsgBody.Length > 0)
            {
                messageBody = messageBody + MsgBody;
            }
            List<string> ContentIds = new List<string>();
            List<string> Paths = new List<string>();
            string pattern = "(?<=src=\")[^,]+?(?=\")";
            string input = messageBody;
            if (messageBody.IndexOf("src=") >= 0)
            {
                int i = 1;
                foreach (Match m in Regex.Matches(input, pattern))
                {
                    string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    Paths.Add(physicalPath + m.Value);
                    ContentIds.Add("ContentID" + i);
                    messageBody = messageBody.Replace(m.Value, "cid:ContentID" + i);
                    i++;
                }
            }
            MailMessage msg = new MailMessage();
            msg.To.Add("raviteja.m@inativetech.com");
            //msg.To.Add("ravitejamachineni@gmail.com");
            //msg.CC.Add("srihari@inativetech.com");
            //msg.CC.Add("8020systems@gmail.com");
            //msg.CC.Add("");
            //if (Cc != "" && Cc != String.Empty && Cc != null)
            //{
            //    msg.CC.Add(Cc);
            //}
            //if (BCC != "" && BCC != null)
            //{
            //    msg.Bcc.Add(BCC);
            //}
            msg.From = new MailAddress(sender);
            msg.Subject = "XIDNA Mail";
            string html = @"<html><body>" + messageBody + "</body></html>";
            AlternateView altView = AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html);
            for (int j = 0; j < Paths.Count(); j++)
            {
                string Path = Paths[j];
                string CntID = ContentIds[j];
                LinkedResource yourPictureRes = new LinkedResource(Path, MediaTypeNames.Image.Jpeg);
                yourPictureRes.ContentId = CntID;
                altView.LinkedResources.Add(yourPictureRes);
            }
            msg.AlternateViews.Add(altView);
            if (Attachment != null)
            {
                //msg.Attachments.Add(new Attachment(Attachment));
            }
            msg.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = serverName;
            smtp.Port = port;
            //for gmail
            smtp.EnableSsl = false;
            smtp.Credentials = new System.Net.NetworkCredential(usern, pass);
            smtp.Send(msg);
            var Org = dbCore.Organization.Find(OrgID);
            DataContext Sdb = new DataContext(Org.DatabaseName);
            WalletMessages Mesg = new WalletMessages();
            Mesg.MailType = "Message";
            Mesg.Sender = 0;
            Mesg.ReceivedOn = DateTime.Now;
            Mesg.Subject = msg.Subject;
            Mesg.Message = OrgiMesg;
            Mesg.MailFrom = sender;
            Mesg.EmailID = UserDetails.sEmail;
            Mesg.OrganizationID = OrgID;
            Sdb.WalletMessages.Add(Mesg);
            Sdb.SaveChanges();
            return 100;
        }

        public int SendSMS(int iUserID, int OrgID, int TemplateID, string sDatabase, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var UserDetais = dbCore.XIAppUsers.Find(iUserID);
            var sOrgDB = Commons.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var SMSDetails = dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID && m.Type == 2).FirstOrDefault();
            string PhNo = UserDetais.sPhoneNumber;
            string Message = "";
            if (TemplateID > 0)
            {
                DataContext Spdb = new DataContext(sOrgDB);
                var Template = Spdb.ContentEditors.Where(m => m.ID == TemplateID).FirstOrDefault();
                Message = Template.Content;
            }
            string BulkSMSPath = SMSDetails.SMSPath; //"http://login.bulksmsglobal.in/api/sendhttp.php?";
            string SMSAPIKey = SMSDetails.SMSAPIKey; //"4411Ac9v78SjO9b57adb887";
            string strResult = BulkSMSPath + "authkey=" + SMSAPIKey + "&mobiles=" + PhNo + "&message=" + Message + "&sender=TARGET" + "&route=6" + "&country=0";
            WebClient WebClient = new WebClient();
            System.IO.StreamReader reader = new System.IO.StreamReader(WebClient.OpenRead(strResult));
            dynamic ResultHTML = reader.ReadToEnd();
            //return ResultHTML;
            var Org = dbCore.Organization.Find(OrgID);
            DataContext Sdb = new DataContext(Org.DatabaseName);
            WalletMessages Mesg = new WalletMessages();
            Mesg.MailType = "Message";
            Mesg.Sender = 0;
            Mesg.ReceivedOn = DateTime.Now;
            Mesg.Type = "SMS";
            Mesg.Message = Message;
            Mesg.OrganizationID = OrgID;
            Sdb.WalletMessages.Add(Mesg);
            Sdb.SaveChanges();
            return 0;
        }
        public string ReplacementQueryApp(string Query, int CurrentUserID)
        {
            if (Query.IndexOf("FROM {Leads}", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Query.Replace("FROM {Leads}", "FROM Leads");
            }
            if (Query.IndexOf("UserID = {CurrentUser}", StringComparison.OrdinalIgnoreCase) > 0)
            {
                Query = Regex.Replace(Query, "UserID = {CurrentUser}", "UserID IN(" + CurrentUserID + ")", RegexOptions.IgnoreCase);
            }
            return Query;
        }

        public string ReplaceTemplateContent(ContentEditors template, int ID, string Table, string sDatabase)
        {
            ModelDbContext dbContext = new ModelDbContext();
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var Content = template.Content;
            var Database = dbCore.Organization.Where(m => m.ID == template.OrganizationID).Select(m => m.DatabaseName).FirstOrDefault();
            string pattern = "(?<=src=\")[^,]+?(?=\")";
            string input = Content;
            if (Content.IndexOf("src=") >= 0)
            {
                int i = 1;
                foreach (Match m in Regex.Matches(input, pattern))
                {
                    var ConImgPath = System.Configuration.ConfigurationManager.AppSettings["XIDNAImgPath"];
                    //string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
                    var path = m.Value.Replace("..", "");
                    Content = Content.Replace("src=\"" + m.Value + "\"", "src=" + ConImgPath + path);
                }
            }
            var Cols = "";
            var GetFieldsRgx = @"(?<=\{{)[^}]*(?=\}})";
            List<string> Columns = new List<string>();
            foreach (Match m in Regex.Matches(template.Content, GetFieldsRgx))
            {
                Cols = Cols + m.Value + ",";
                Columns.Add(m.Value);
            }
            Cols = Cols.Substring(0, Cols.Length - 1);
            Content = @"<html><body>" + Content + "</body></html>";
            using (SqlConnection Con = new SqlConnection(ServiceUtil.GetClientConnectionString()))
            {
                Con.Open();
                Con.ChangeDatabase(Database);
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = Con;
                cmd.CommandText = "Select " + Cols + " from " + Table + " where id=" + ID;
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);
                List<object[]> TotalResult = data.AsEnumerable().Select(m => m.ItemArray).ToList();
                var DBVlaues = (from c in TotalResult[0]
                                select c.ToString()).ToList();

                for (int i = 0; i < Columns.Count(); i++)
                {
                    Content = Content.Replace("{{" + Columns[i] + "}}", DBVlaues[i]);
                }
            }
            return Content;
        }

    }
}
