using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Limilabs.Mail;
using Limilabs.Client.IMAP;
using Limilabs.Mail.MIME;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using XIDNA.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using XIDNA.ViewModels;
using System.Reflection;
using System.Net;
using System.IO;
using System.Web;

namespace XIDNA.Repository
{
    public class MailRepository : IMailRepository
    {
        CommonRepository Common = new CommonRepository();
        ModelDbContext dbContext = new ModelDbContext();
        //Gets the list of folders
        public List<string> SelectFoldersWithIMAP(int ID, int OrgID)
        {
            IOServerDetails Details = GetEmailCredentials(ID, OrgID);
            string UserName = Details.UserName;
            string Password = Details.Password;
            string Server = Details.ServerName;
            int Port = Details.Port;
            var lFolderList = new List<string>();
            using (Imap imap = new Imap())
            {
                imap.ConnectSSL(Server, Port);   // or ConnectSSL
                imap.UseBestLogin(UserName, Password);
                //Get list of folders...
                foreach (FolderInfo folder in imap.GetFolders())
                {
                    lFolderList.Add(folder.Name);
                }
                imap.Close();
            }
            return lFolderList;
        }

        public IOServerDetails GetEmailCredentials(int ID, int OrgID)
        {
            var ServerDetails = dbContext.IOServerDetails.Where(m => m.ID == ID).FirstOrDefault();
            return ServerDetails;
        }

        public List<string> sGetEmailSubjects(int ID, string sFolder, int OrgID)
        {
            IOServerDetails Details = GetEmailCredentials(ID, OrgID);
            string UserName = Details.UserName;
            string Password = Details.Password;
            string Server = Details.ServerName;
            int Port = Details.Port;
            var lListSubjects = new List<string>();
            string sFromAddress = "";
            string sFromName = "";
            using (Imap imap = new Imap())
            {
                imap.ConnectSSL(Server, Port);   // or ConnectSSL
                imap.UseBestLogin(UserName, Password);
                imap.Select(sFolder);
                List<long> uids = imap.GetAll();
                uids = uids.OrderByDescending(m => m).ToList();
                List<MessageInfo> infos = imap.GetMessageInfoByUID(uids);
                infos = infos.OrderByDescending(m => m.UID).ToList();
                foreach (MessageInfo info in infos)
                {
                    string uid = info.Envelope.UID.ToString();
                    string subject = info.Envelope.Subject;
                    string sEmailDate = (info.Envelope.Date).Value.ToString();
                    var dSplitToDateTime = sEmailDate.Split(' ');
                    var dGetDate = dSplitToDateTime[0];
                    foreach (var sAddress in info.Envelope.From)
                    {
                        sFromAddress = sAddress.Address;
                        sFromName = sAddress.Name;
                    }
                    lListSubjects.Add(uid + " <> " + sFolder + " <> " + subject + " <> " + sFromAddress + " <> " + dGetDate);
                }
                imap.Close();
            }
            return lListSubjects;
        }
        //Get the details of email based on the Subject selected(with UID and folder details)
        public List<string> GetEmailFullDetails(int ID, int iUID, string sFolder, int OrgID)
        {
            var lEmailDetails = new List<string>();
            string sFromAddress = "";
            string sFromName = "";
            string sEmailDate;
            var mMonth = "";
            var dDate = "";
            var yYear = "";
            int i = 0;
            //string database = dbcontext.AspNetUsers.Where(m => m.OrganizationID == OrgID).Select(m => m.DatabaseName).FirstOrDefault();
            //DataContext Spdb = new DataContext(database);
            //string SubscrptionID = 
            string sAttachmentName = "";
            int iAttachmentCount = 0;

            ArrayList sAttachmentDetails = new ArrayList();
            ArrayList aItemList = new ArrayList();

            string sCSVAttachmentName = "";
            IOServerDetails Details = GetEmailCredentials(ID, OrgID);
            string UserName = Details.UserName;
            string Password = Details.Password;
            string Server = Details.ServerName;
            int Port = Details.Port;
            using (Imap imap = new Imap())
            {
                imap.ConnectSSL(Server, Port);   // or ConnectSSL
                imap.UseBestLogin(UserName, Password);

                //Select folder
                imap.Select(sFolder);

                //Get details on selected UID
                IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(iUID));

                //Email UID
                int UID = iUID;

                //Email date for attachments
                sEmailDate = (email.Date).Value.ToString();
                var dSplitToDateTime = sEmailDate.Split(' ');
                var dGetDate = dSplitToDateTime[0];
                var dSplitToDate = dGetDate.Split('/');
                mMonth = dSplitToDate[0];
                dDate = dSplitToDate[1];
                yYear = dSplitToDate[2];

                //Email details
                foreach (var sAddress in email.From)
                {
                    sFromAddress = sAddress.Address;
                    sFromName = sAddress.Name;
                }
                string sSubject = email.Subject;
                string sText = email.Text;

                //Email Attachment
                //check for number of attachemnts in the mail..
                if (email.NonVisuals.Count == 0)
                {
                    sAttachmentName = "";
                }
                else
                {
                    //Get the count of attachments.
                    iAttachmentCount = email.NonVisuals.Count;

                    foreach (MimeData mime in email.Attachments)
                    {
                        sAttachmentName = mime.SafeFileName;

                        //Arraylist to insert values if tha email contains n attachments
                        aItemList.Insert(i, sAttachmentName);

                        //increment the i value to insert attachment name.
                        i = i + 1;
                    }

                    //Convert the output in the form of arraylist to CSV
                    sCSVAttachmentName = string.Join(",", (string[])aItemList.ToArray(Type.GetType("System.String")));
                }

                //Seperate method call to save to DB
                //SaveDetailsToDatabase(UID, sEmailDate, sFromAddress, sSubject, sText, sAttachmentPath, sCSVAttachmentName);
                //ExtractEmailData(sText);
                lEmailDetails.Add(sFromName + "<" + sFromAddress + "> <> " + sSubject + " <> " + sText + " <> " + email.Html + " <> " + sCSVAttachmentName);
                imap.Close();
            }

            return lEmailDetails;
        }

        //Parameterised query to Save to DB.
        public List<List<string>> SaveDetailsToDatabase(int ID, int iUID, string sFolder, int OrgID, int iUserID, string sOrgName, string sDatabase)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            var lEmailDetails = new List<string>();
            string sFromAddress = "";
            string sFromName = "";
            string sEmailDate;
            var mMonth = "";
            var dDate = "";
            var yYear = "";
            string sSubject = "";
            string sText = "";
            int i = 0;
            int sStatus = 0;
            string sAttachmentPath = "";
            string sAttachmentName = "";
            int iAttachmentCount = 0;
            var LeadDetails = new List<List<string>>(); ;
            ArrayList sAttachmentDetails = new ArrayList();
            ArrayList aItemList = new ArrayList();
            IOServerDetails Details = GetEmailCredentials(ID, OrgID);
            string UserName = Details.UserName;
            string Password = Details.Password;
            string Server = Details.ServerName;
            int Port = Details.Port;
            string sCSVAttachmentName = "";
            string SubscriptionID = "";
            DataContext Spdb = new DataContext(sOrgDB);
            try
            {
                using (Imap imap = new Imap())
                {
                    imap.ConnectSSL(Server, Port);   // or ConnectSSL
                    imap.UseBestLogin(UserName, Password);

                    //Select folder
                    imap.Select(sFolder);

                    //Get details on selected UID
                    IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(iUID));

                    //Email UID
                    int UID = iUID;

                    //Email date for attachments
                    sEmailDate = (email.Date).Value.ToString();
                    var dSplitToDateTime = sEmailDate.Split(' ');
                    var dGetDate = dSplitToDateTime[0];
                    var dSplitToDate = dGetDate.Split('/');
                    mMonth = dSplitToDate[0];
                    dDate = dSplitToDate[1];
                    yYear = dSplitToDate[2];

                    //Email details
                    foreach (var sAddress in email.From)
                    {
                        sFromAddress = sAddress.Address;
                        sFromName = sAddress.Name;
                    }
                    sSubject = email.Subject;
                    sText = email.Text;
                    string sHtmlText = email.Html;
                    //Email Attachment
                    //check for number of attachemnts in the mail..
                    if (email.NonVisuals.Count == 0)
                    {
                        sAttachmentPath = "";
                        sAttachmentName = "";
                    }
                    else
                    {
                        //Create a path to save the attachment...
                        if ((!System.IO.Directory.Exists("D:" + "\\" + yYear + " \\ " + mMonth + " \\ " + dDate + " \\ " + UID + "\\")))
                        {
                            System.IO.Directory.CreateDirectory("D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\");
                        }

                        //Get the count of attachments.
                        iAttachmentCount = email.NonVisuals.Count;

                        foreach (MimeData mime in email.Attachments)
                        {
                            mime.Save("D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\" + mime.SafeFileName);
                            sAttachmentPath = "D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\";
                            sAttachmentName = mime.SafeFileName;

                            //Arraylist to insert values if tha email contains n attachments
                            aItemList.Insert(i, sAttachmentName);

                            //increment the i value to insert attachment name.
                            i = i + 1;
                        }

                        //Convert the output in the form of arraylist to CSV
                        sCSVAttachmentName = string.Join(",", (string[])aItemList.ToArray(Type.GetType("System.String")));
                    }
                    //Get Subscription Details
                    var fromemail = email.From[0].Address.ToString();
                    var Subject = email.Subject;
                    //var sourceemail = Spdb.OrganizationSources.Where(m => m.EmailID == fromemail).FirstOrDefault();
                    SubscriptionID = Spdb.OrganizationSubscriptions.Where(m => m.Email == fromemail).Select(m => m.SubscriptionID).FirstOrDefault();
                    if (SubscriptionID != null)
                    {
                        SubscriptionID = Spdb.OrganizationSubscriptions.Where(m => m.Email == fromemail).Select(m => m.SubscriptionID).FirstOrDefault();
                    }
                    else
                    {
                        SubscriptionID = Subject.Split('-')[1].ToString();
                    }
                    if (SubscriptionID == null)
                    {
                        if (sText.IndexOf("Broker Ref ID", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            string Text = sText;
                            List<string> lines = Text.Replace("\r", "").Split('\n').ToList();
                            string EndLine = lines.Where(m => m.Contains("Broker Ref ID")).Select(m => m).FirstOrDefault();
                            int EndValue = EndLine.Length;
                            EndLine = EndLine.Replace("Broker Ref ID: ", "");
                            SubscriptionID = EndLine;
                        }
                    }
                }
                using (SqlConnection Con = new SqlConnection(""))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    string GetUID = "SELECT sEmailFrom,sEmailSubject FROM [dbo].[MailData] WHERE [UID] = '" + iUID + "'";
                    cmd.CommandText = GetUID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        string sQuery = string.Empty;
                        sQuery += " INSERT INTO MailData ([UID],[dEmailDate],[sEmailFrom],[sEmailSubject],[sEmailText],[sAttachmentPath],[sAttachmentName])";
                        sQuery += "VALUES (@UID,@dEmailDate,@EmailFrom, @EmailSubject, @EmailText, @EmailAttachmentFolder,@EmailAttachmentName)";
                        cmd = new SqlCommand(sQuery, Con);
                        cmd.CommandText = sQuery;
                        //cmd.Parameters.Add(new SqlParameter("@uid", System.Data.SqlDbType.NVarChar));
                        cmd.Parameters.AddWithValue("@UID", iUID);
                        cmd.Parameters.AddWithValue("@dEmailDate", sEmailDate);
                        cmd.Parameters.AddWithValue("@EmailFrom", sFromAddress);
                        cmd.Parameters.AddWithValue("@EmailSubject", sSubject);
                        cmd.Parameters.AddWithValue("@EmailText", sText);
                        cmd.Parameters.AddWithValue("@EmailAttachmentFolder", sAttachmentPath);
                        cmd.Parameters.AddWithValue("@EmailAttachmentName", sCSVAttachmentName);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        List<string> NoData = new List<string>();
                        NoData.Add("Lead Already Saved");
                        LeadDetails.Add(NoData);
                        return LeadDetails;
                    }
                    Con.Close();
                }
                sStatus = 1;
            }
            catch (Exception ex)
            {
                sStatus = 0;
                List<string> NoData = new List<string>();
                NoData.Add("Unable to extract data. Please check again.");
                LeadDetails.Add(NoData);
            }
            if (sStatus == 1)
            {
                int InboundID = ExtractEmailData(sText, OrgID, SubscriptionID, iUserID, sDatabase);
                if (InboundID > 0)
                {
                    LeadDetails = ExtractEmailData(InboundID, OrgID, sDatabase, iUserID, sOrgName);
                }
                else
                {

                }
            }
            return LeadDetails;
        }

        public List<List<string>> SaveClientMailToDatabase(int ID, int iUID, string sFolder, int OrgID, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            var lEmailDetails = new List<string>();
            string sFromAddress = "";
            string sFromName = "";
            string sEmailDate;
            var mMonth = "";
            var dDate = "";
            var yYear = "";
            string sSubject = "";
            string sText = "";
            int i = 0;
            string sAttachmentPath = "";
            string sAttachmentName = "";
            int iAttachmentCount = 0;
            var LeadDetails = new List<List<string>>(); ;
            ArrayList sAttachmentDetails = new ArrayList();
            ArrayList aItemList = new ArrayList();
            IOServerDetails Details = GetEmailCredentials(ID, OrgID);
            string UserName = Details.UserName;
            string Password = Details.Password;
            string Server = Details.ServerName;
            int Port = Details.Port;
            string sCSVAttachmentName = "";
            DataContext Spdb = new DataContext(database);
            try
            {
                using (Imap imap = new Imap())
                {
                    imap.ConnectSSL(Server, Port);   // or ConnectSSL
                    imap.UseBestLogin(UserName, Password);

                    //Select folder
                    imap.Select(sFolder);

                    //Get details on selected UID
                    IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(iUID));

                    //Email UID
                    int UID = iUID;

                    //Email date for attachments
                    sEmailDate = (email.Date).Value.ToString();
                    var dSplitToDateTime = sEmailDate.Split(' ');
                    var dGetDate = dSplitToDateTime[0];
                    var dSplitToDate = dGetDate.Split('/');
                    mMonth = dSplitToDate[0];
                    dDate = dSplitToDate[1];
                    yYear = dSplitToDate[2];

                    //Email details
                    foreach (var sAddress in email.From)
                    {
                        sFromAddress = sAddress.Address;
                        sFromName = sAddress.Name;
                    }
                    sSubject = email.Subject;
                    sText = email.Text;
                    string sHtmlText = email.Html;
                    //Email Attachment
                    //check for number of attachemnts in the mail..
                    if (email.NonVisuals.Count == 0)
                    {
                        sAttachmentPath = "";
                        sAttachmentName = "";
                    }
                    else
                    {
                        //Create a path to save the attachment...
                        if ((!System.IO.Directory.Exists("D:" + "\\" + yYear + " \\ " + mMonth + " \\ " + dDate + " \\ " + UID + "\\")))
                        {
                            System.IO.Directory.CreateDirectory("D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\");
                        }

                        //Get the count of attachments.
                        iAttachmentCount = email.NonVisuals.Count;

                        foreach (MimeData mime in email.Attachments)
                        {
                            mime.Save("D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\" + mime.SafeFileName);
                            sAttachmentPath = "D:" + "\\" + yYear + "\\" + mMonth + "\\" + dDate + "\\" + UID + "\\";
                            sAttachmentName = mime.SafeFileName;

                            //Arraylist to insert values if tha email contains n attachments
                            aItemList.Insert(i, sAttachmentName);

                            //increment the i value to insert attachment name.
                            i = i + 1;
                        }

                        //Convert the output in the form of arraylist to CSV
                        sCSVAttachmentName = string.Join(",", (string[])aItemList.ToArray(Type.GetType("System.String")));
                    }
                    //Get Subscription Details
                    var fromemail = email.From[0].Address.ToString();
                    var Subject = email.Subject;
                }
                List<string> NoData = new List<string>();
                using (SqlConnection Con = new SqlConnection(""))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(database);
                    cmd.Connection = Con;
                    string GetUID = "SELECT MailFrom,Subject FROM [dbo].[WalletMessages] WHERE [UID] = '" + iUID + "'";
                    cmd.CommandText = GetUID;
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows == false)
                    {
                        string sQuery = string.Empty;
                        sQuery += " INSERT INTO WalletMessages ([UID],[Sender],[MailType],[OrganizationID], [ReceivedOn],[EmailID],[Subject],[Message],[AttachmentsPath],[Attachments])";
                        sQuery += "VALUES (@UID,@Sender,@MailType,@OrganizationID, @ReceivedOn,@EmailID, @Subject, @Message, @AttachmentsPath,@Attachments)";
                        cmd = new SqlCommand(sQuery, Con);
                        cmd.CommandText = sQuery;
                        //cmd.Parameters.Add(new SqlParameter("@uid", System.Data.SqlDbType.NVarChar));
                        cmd.Parameters.AddWithValue("@UID", iUID);
                        cmd.Parameters.AddWithValue("@Sender", 1);
                        cmd.Parameters.AddWithValue("@MailType", "Message");
                        cmd.Parameters.AddWithValue("@OrganizationID", OrgID);
                        cmd.Parameters.AddWithValue("@ReceivedOn", sEmailDate);
                        cmd.Parameters.AddWithValue("@EmailID", sFromAddress);
                        cmd.Parameters.AddWithValue("@Subject", sSubject);
                        cmd.Parameters.AddWithValue("@Message", sText);
                        cmd.Parameters.AddWithValue("@AttachmentsPath", sAttachmentPath);
                        cmd.Parameters.AddWithValue("@Attachments", sCSVAttachmentName);
                        cmd.ExecuteNonQuery();
                    }
                    Con.Close();
                    NoData.Add("Success");
                    NoData.Add("Mail saved successfully");
                }
                LeadDetails.Add(NoData);
            }
            catch (Exception ex)
            {
                List<string> NoData = new List<string>();
                NoData.Add("Error while saving mail");
                LeadDetails.Add(NoData);
            }
            return LeadDetails;
        }
        //Extracting and saving to INBOUND table
        public int ExtractEmailData(string sText, int OrgID, string SubscriptionID, int UserID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            string sStartString = "";
            string sEndString = "";
            int SourceID = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubscriptionID).Select(m => m.SourceID).FirstOrDefault();
            MailExtractStrings Strings = new MailExtractStrings();
            Strings = Spdb.Database.SqlQuery<MailExtractStrings>("SELECT sStartString,sEndString FROM [dbo].[MailExtractStrings] WHERE [SubscriptionID] = '" + SubscriptionID + "'").FirstOrDefault();
            sStartString = Strings.sStartString;
            sEndString = Strings.sEndString;
            LeadInbounds Inbound = new LeadInbounds();
            //Parse EmailText based on strings..
            if (sStartString != "" && sEndString != "")
            {
                string sMAEmailTextBody = ParseTextLinePair(sStartString, sEndString, sText);
                //Insert the pased Email body to INBound
                if (sMAEmailTextBody != "" && sMAEmailTextBody != null)
                {

                    //Insert to Import History
                    ImportHistories History = new ImportHistories();
                    History.OrganizationID = OrgID;
                    History.FileType = "Email";
                    History.OriginalName = "Email";
                    History.ImportedOn = DateTime.Now;
                    History.StatusTypeID = 1;
                    History.UserID = UserID;
                    Spdb.ImportHistories.Add(History);
                    Spdb.SaveChanges();
                    //Insert to INBOUND
                    string UserName = dbCore.XIAppUsers.Where(m => m.UserID == UserID).Select(m => m.sFirstName).FirstOrDefault();
                    Inbound.SourceID = SourceID;
                    Inbound.FileID = History.ID;
                    Inbound.ImportedOn = DateTime.Now;
                    Inbound.ImportedBy = UserName;
                    Inbound.Content = sMAEmailTextBody;
                    Inbound.StatusTypeID = 10;
                    Inbound.SubscriptionID = SubscriptionID;
                    Spdb.LeadInbounds.Add(Inbound);
                    Spdb.SaveChanges();

                }
            }
            return Inbound.ID;
        }

        //Parse email body
        public string ParseTextLinePair(string sStartString, string sEndString, string sText)
        {

            int intLocLabel = 0;
            // int intLocCRLF = 0;
            int intLenLabel = 0;
            string strText = null;
            int intEndLabel = 0;
            int intLenEndLabel = 0;
            //--Get the position of the string to start extracting...
            intLocLabel = sText.IndexOf(sStartString);
            intLenLabel = sStartString.Length;
            //--Get the position of the end string...
            intEndLabel = sText.IndexOf(sEndString);
            intLenEndLabel = sEndString.Length;

            //string sEndString = "Broker Ref ID:";
            string Text = sText;
            List<string> lines = Text.Replace("\r", "").Split('\n').ToList();
            string EndLine = lines.Where(m => m.Contains(sEndString)).Select(m => m).FirstOrDefault();
            if (EndLine != null)
            {
                int EndValue = EndLine.Length;
                EndLine = EndLine.Replace(sEndString, "");
                EndValue = EndLine.Length;
                intLenEndLabel = intLenEndLabel + EndValue;
                if (intLocLabel >= 0)
                {
                    strText = sText.Substring(intLocLabel, intEndLabel - intLocLabel + intLenEndLabel);
                }
            }

            if (strText != null)
            {
                strText = strText.Trim();
            }
            return strText;
        }
        public List<List<string>> ExtractEmailData(int InboundID, int OrgID, string sDatabase, int iUserID, string sOrgName)
        {
            ModelDbContext dbContext = new ModelDbContext(sDatabase);
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext dbcontext = new DataContext(sOrgDB);
            ModelDbContext modeldb = new ModelDbContext();
            LeadInbounds inbound = new LeadInbounds();
            SqlConnection Con = new SqlConnection("");
            SqlCommand cmd = new SqlCommand();
            var lLeadID = new List<int>();
            var lSuccessDatas = new List<List<string>>();

            inbound = dbcontext.LeadInbounds.Where(m => m.ID == InboundID).FirstOrDefault();
            string SubscriptionID = inbound.SubscriptionID;
            var sEmailContent = inbound.Content;
            //int iInboundID = inbound.ID;
            //Replacing special characters to create the proper key value structure....
            //Add ":" to all the keys as many doesnt have...
            var ValuesRemoveNewLine = sEmailContent.Replace("\r\n\r\n", "\r\n");
            var ValuesRemoveNewLine1 = ValuesRemoveNewLine.Replace("\r\n  \r\n", "\r\n");
            var ValuesRemoveNewLine2 = ValuesRemoveNewLine1.Replace("\r\r\n", "\r\n");
            var ValuesRemoveNewLine3 = ValuesRemoveNewLine2.Replace(" \r\n", "\r\n");
            var ValuesRemoveNewLine4 = ValuesRemoveNewLine3.Replace("\r\n ", "\r\n");
            var ValuesRemoveDouble = ValuesRemoveNewLine4.Replace(",,", ",");
            //replace "'" single quotes with "''" to avoid database error..
            var ValuesReplaceQuote = ValuesRemoveDouble.Replace("'", "''");

            //Regex rgxSpace = new Regex("\\s\\s\\s+\r\n");
            Regex rgxSpace = new Regex(@"\s{2,}\r\n");
            string RemoveSpace = rgxSpace.Replace(ValuesReplaceQuote, "<>\r\n");

            Regex rgxSpace1 = new Regex("\t+\r\n");
            string RemoveSpace1 = rgxSpace1.Replace(RemoveSpace, "<>\r\n");

            Regex rgxn = new Regex(":\t+");
            string ValuesAddn = rgxn.Replace(RemoveSpace1, "\t");

            Regex rgx = new Regex("\\s\\s\\s+");
            string ValuesAdd = rgx.Replace(ValuesAddn, "::<>");

            var ValuesADD = ValuesAdd.Replace(":\r\n", "<>\r\n");

            Regex rgx1 = new Regex("::+");
            string ValuesAdd1 = rgx1.Replace(ValuesADD, "");

            Regex rgx2 = new Regex(":+\\s+");
            string ValuesAdd2 = rgx2.Replace(ValuesAdd1, "<>");
            //-------
            Regex rgx3 = new Regex(":>+? +");
            string ValuesAdd3 = rgx3.Replace(ValuesAdd2, "\t");

            var ValuesReplace1 = ValuesAdd3.Replace("\t", "<>");
            //var ValuesReplace2 = ValuesReplace1.Replace(":+", ":");
            var ValuesReplace3 = ValuesReplace1.Replace(" -\r\n", "-");
            var ValuesReplace4 = ValuesReplace3.Replace(" - ", "<>");
            var ValuesReplace5 = ValuesReplace4.Replace(" :>", ":>");
            var ValuesReplace6 = ValuesReplace5.Replace(" :<>", "<>");
            //statically assigning "<>" for Quotezone
            var ReplaceFinalQuote = ValuesReplace6.Replace(":", "<>");
            var FinalReplace = ReplaceFinalQuote.Replace("<><>", "<>");
            var ValuesReplace7 = FinalReplace.Replace("DETAILS\r\n", "DETAILS<>\r\n");

            //BgInsurance convicted driver static splitting
            //var ValuesStatic = ValuesReplace7.Replace("Convicton Type", "\r\nConvicton Type");
            //var ValuesStatic1 = ValuesStatic.Replace("Own This Vehicle", "\r\nOwn This Vehicle");
            //var ValuesStatic2 = ValuesStatic1.Replace("Overnight\r\nStorage", "\r\nOvernight Storage");
            //var ValuesStatic3 = ValuesStatic2.Replace("No Claims Bonus", "\r\nNo Claims Bonus");
            //var ValuesStatic4 = ValuesStatic3.Replace("Year Annual Mileage", "\r\nYear Annual Mileage");
            //var ValuesStatic5 = ValuesStatic4.Replace("Own Another Car", "\r\nOwn Another Car");
            //var ValuesStatic6 = ValuesStatic5.Replace("Policy Start Date", "\r\nPolicy Start Date");
            //var ValuesStatic7 = ValuesStatic6.Replace("Created When", "\r\nCreated When");
            //remove "-----" in quotesearcher
            Regex rgx6 = new Regex("------+");
            string NewValues = rgx6.Replace(ValuesReplace7, "");

            //New instance Table
            var Status = ValidateEmailLeadData(NewValues);
            List<List<string>> ValidStatus = new List<List<string>>();
            int FileID = inbound.FileID;
            string FileType = dbcontext.ImportHistories.Where(m => m.ID == FileID).Select(m => m.FileType).FirstOrDefault();
            //if (Status.Count() > 0)
            //{
            //    for (int i = 0; i < Status.Count(); i++)
            //    {
            //        ImportingErrorDetails Details = new ImportingErrorDetails();
            //        Details.FileID = FileID;
            //        Details.TypeOfData = FileType;
            //        Details.Message = Status[i];
            //        Details.LoggedOn = DateTime.Now;
            //        dbcontext.ImportingErrorDetails.Add(Details);
            //        dbcontext.SaveChanges();
            //        var History = dbcontext.ImportHistories.Find(FileID);
            //        History.StatusTypeID = 20;
            //        dbcontext.SaveChanges();
            //        var inbounds = dbcontext.LeadInbounds.Find(InboundID);
            //        inbounds.StatusTypeID = 20;
            //        dbcontext.SaveChanges();
            //    }
            //    Status.Insert(0, "Failure");
            //    ValidStatus.Add(Status);
            //    return ValidStatus;
            //}
            //else
            //{
            var SleadDetails = EmailExtractedIntoLead(NewValues, InboundID, OrgID, sDatabase, SubscriptionID, Status, iUserID, sOrgName);
            return SleadDetails;
            //string iResult = EmailExtractedIntoInstance(NewValues, InboundID, OrgID, database, SubscriptionID);
            //if (iResult == "Success")
            //{
            //    var SleadDetails = EmailExtractedIntoLead(NewValues, InboundID, OrgID, database, SubscriptionID, Status);
            //    var History = dbcontext.ImportHistories.Find(FileID);
            //    History.StatusTypeID = 10;
            //    dbcontext.SaveChanges();
            //    var inbounds = dbcontext.LeadInbounds.Find(InboundID);
            //    inbounds.StatusTypeID = 10;
            //    dbcontext.SaveChanges();
            //    return SleadDetails;
            //}
            //else
            //{
            //    var History = dbcontext.ImportHistories.Find(FileID);
            //    History.StatusTypeID = 20;
            //    dbcontext.SaveChanges();
            //    var inbounds = dbcontext.LeadInbounds.Find(InboundID);
            //    inbounds.StatusTypeID = 20;
            //    dbcontext.SaveChanges();
            //    List<List<string>> Errors = new List<List<string>>();
            //    List<string> Error = new List<string>();
            //    Error.Add("Failure");
            //    Error.Add(iResult);
            //    Errors.Add(Error);
            //    return Errors;
            //}
            //}
        }

        private List<string> ValidateEmailLeadData(string Values)
        {
            string sError = "";
            var aValidationErr = new List<string>();
            var lBOColWithExprs = new List<string>();
            var BOID = dbContext.BOs.Where(m => m.Name == EnumLeadTables.Leads.ToString()).Select(m => m.BOID).FirstOrDefault();
            var BoFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
            lBOColWithExprs = BoFields.Where(q => q.ExpressionValue != null).Select(q => q.Name).ToList();
            string sRegxE = "";
            var keyValuePairs = Values.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            IList<string> KeyValuePairInstance = new List<string>();
            KeyValuePairInstance = Values.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            try
            {
                foreach (var items in KeyValuePairInstance)
                {
                    var sKeyValues = Regex.Split(items, "<>");
                    var sKeyIn = sKeyValues[0];
                    var sValueIn = "";
                    if (sKeyValues[1].Contains(">"))
                    {
                        sValueIn = sKeyValues[1].Replace(">", " ");
                    }
                    else
                    {
                        sValueIn = sKeyValues[1];
                    }
                    if (sValueIn != "")
                    {
                        var sStringList = new List<string>();
                        string sGetDate = "";
                        string sColumnName = dbContext.MasterTemplates.Where(a => a.DataFieldName == sKeyIn).Select(a => a.FieldName).FirstOrDefault();
                        sRegxE = BoFields.Where(e => e.Name == sColumnName).Select(e => e.ExpressionValue).FirstOrDefault();
                        if (sRegxE != null)
                        {
                            for (int f = 0; f < lBOColWithExprs.Count; f++)
                            {
                                int iType = BoFields.Where(u => u.Name == sColumnName).Select(u => u.TypeID).FirstOrDefault();
                                //   List<BOFields> AssignedFields = new List<BOFields>();
                                int TypeID = iType;
                                string sDatatype = ((BODatatypes)TypeID).ToString().ToLower();
                                if (sDatatype == "datetime" && sColumnName == lBOColWithExprs[f])
                                {
                                    string[] formats = new string[50];
                                    var sSplitValues = sRegxE.Split(',').ToList();
                                    for (int i = 0; i < sSplitValues.Count; i++)
                                    {
                                        string sRemoveQuote = sSplitValues[i].Replace("\"", "");
                                        formats[i] = sRemoveQuote;
                                    }
                                    DateTime datetime;
                                    if (DateTime.TryParseExact(sValueIn.ToString(),
                                                                formats,
                                                                CultureInfo.InvariantCulture,
                                                                DateTimeStyles.AssumeLocal,
                                                                out datetime))
                                    {

                                        sGetDate = datetime.ToString("dd/MM/yyyy");
                                        sStringList.Add(sKeyIn + ":" + sGetDate);

                                    }
                                    else
                                    {
                                        sStringList.Add(sKeyIn + ":");
                                        //string sError = "";
                                        sError = "DOB Field is Invalid. Received data is " + sValueIn;
                                        aValidationErr.Add(sError);
                                    }
                                }
                                else if (sColumnName == lBOColWithExprs[f])
                                {
                                    var regexItem = new Regex(sRegxE);
                                    if (regexItem.IsMatch(sValueIn.ToLower().ToString()))
                                    {
                                        sStringList.Add(sKeyIn + ":" + sValueIn);
                                    }
                                    else
                                    {
                                        if (sColumnName == "sForeName" || sColumnName == "sLastName")
                                        {
                                            var newValue = Regex.Replace(sValueIn.ToString(), "[^a-zA-Z]+", "");
                                            sStringList.Add(sKeyIn + ":" + newValue);
                                            sError = "Special characters are not allowed.";
                                        }
                                        else
                                        {
                                            sStringList.Add(sKeyIn + ":");
                                            sError = sKeyIn + " is Invalid. Received data is " + sValueIn;
                                        }
                                        aValidationErr.Add(sError);
                                    }
                                }
                            }
                        }//regex is null
                        else
                        {
                            sStringList.Add(sKeyIn + ":" + sValueIn);
                        }
                    }
                    else
                    {
                        string sColumnName = dbContext.MasterTemplates.Where(a => a.DataFieldName == sKeyIn).Select(a => a.FieldName).FirstOrDefault();
                        var col = lBOColWithExprs.Where(m => m == sColumnName).FirstOrDefault();
                        if (col != null)
                        {
                            sError = sKeyIn + " is not Provided";
                            aValidationErr.Add(sError);
                        }
                    }

                }
            }
            catch
            {
                aValidationErr.Add("Not A Proper Format");
            }
            return aValidationErr;
        }

        public string EmailExtractedIntoInstance(string NewValues, int InboundID, int OrgID, string database, string SubscriptionID, long LeadID)
        {
            DataContext Spdb = new DataContext(database);
            var SubscrpDetails = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubscriptionID).FirstOrDefault();
            var MappedFields = Spdb.MappedFields.Where(u => u.OrganizationID == OrgID && u.ClassID == SubscrpDetails.ClassID).ToList();
            int ClassID = SubscrpDetails.ClassID;
            int SourceID = SubscrpDetails.SourceID;
            try
            {
                var keyValuePairs = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                IList<string> KeyValuePairInstance = new List<string>();
                KeyValuePairInstance = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //check if the key values is in the next line with out ":".....
                for (int key = 0; key < keyValuePairs.Count; key++)
                {

                    if (keyValuePairs[key].Contains("<>"))
                    {
                        //nothing to be done...
                    }
                    else
                    {
                        //int j;
                        if (keyValuePairs[key].Contains("<>"))
                        {
                            //Do not Add...
                        }
                        else
                        {
                            //changes made : var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";/////removed ">" as it appends
                            var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";
                            string GetKey = keyValuePairs[key - 1];
                            var KeyIndex = KeyValuePairInstance.IndexOf(GetKey);
                            //replace old with new one
                            KeyValuePairInstance[KeyIndex] = ValuesAppend;
                            KeyValuePairInstance.RemoveAt(KeyIndex + 1);
                        }
                    }
                }

                KeyValuePairInstance.Add("FKiLeadClassID<>" + ClassID);
                KeyValuePairInstance.Add("InBoundID<>" + InboundID);
                KeyValuePairInstance.Add("dImportedOn<>" + DateTime.Now);
                KeyValuePairInstance.Add("FKiOrgID<>" + OrgID);
                KeyValuePairInstance.Add("FKiSourceID<>" + SourceID);
                KeyValuePairInstance.Add("FKiLeadID<>" + LeadID);
                string sFirstLeadTable = EnumLeadTables.LeadInstances.ToString();
                long iInstanceID = Spdb.Database.SqlQuery<long>("INSERT INTO " + sFirstLeadTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST( SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                string UpdateStatements = "";
                foreach (var values in KeyValuePairInstance)
                {
                    if (values.Contains("<>"))
                    {
                        var sKeyValues = Regex.Split(values, "<>");
                        var sKeyIn = sKeyValues[0];
                        var sValueIn = "";
                        if (sKeyValues[1].Contains(">"))
                        {
                            sValueIn = sKeyValues[1].Replace(">", " ");
                        }
                        else
                        {
                            sValueIn = sKeyValues[1];
                        }

                        if (sValueIn != "")
                        {
                            var sColumnName = "";
                            var sDataType = "";
                            MasterTemplates TemplateColumnData = dbContext.MasterTemplates.Where(m => m.ClassID == ClassID && m.DataFieldName == sKeyIn).FirstOrDefault();
                            if (TemplateColumnData != null)
                            {
                                sColumnName = TemplateColumnData.FieldName;
                                sDataType = TemplateColumnData.FieldType;
                            }
                            else
                            {
                                var MappedColumnData = MappedFields.Where(u => u.FieldName == sKeyIn).FirstOrDefault();
                                if (MappedColumnData != null)
                                {
                                    sColumnName = MappedColumnData.AddField;
                                    sDataType = MappedColumnData.FieldType;
                                }
                            }
                            string sDataTypeValue = (sDataType).ToLower();
                            string sGetDate = "";
                            if (sColumnName != "" && sColumnName != null)
                            {
                                //CHECKS for the DATETIME DATATYPE
                                if (sDataTypeValue == "datetime")
                                {
                                    if (sValueIn != "1/1/0001" && sValueIn != "01/01/0001")
                                    {
                                        if (sColumnName == "dImportedOn")
                                        {
                                            UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";
                                        }
                                        else
                                        {
                                            if (sValueIn != "1/1/0001" && sValueIn != "01/01/0001")
                                            {
                                                DateTime datetime;
                                                string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                                foreach (var PatternType in Pattern)
                                                {
                                                    if (DateTime.TryParseExact(sValueIn, PatternType, null, DateTimeStyles.None, out datetime))
                                                    {
                                                        if (sColumnName == "dDOB")
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                        }
                                                        else
                                                        {
                                                            sGetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //error message
                                                    }
                                                }
                                                UpdateStatements = UpdateStatements + sColumnName + "='" + sGetDate + "', ";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    UpdateStatements = UpdateStatements + sColumnName + "='" + sValueIn + "', ";

                                }
                            }
                        }
                        else
                        {
                            //Save null
                        }
                    }
                }
                UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                UpdateStatements = "Update [dbo].[" + sFirstLeadTable + "] set " + UpdateStatements + " WHERE [ID]='" + iInstanceID + "'";
                Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public List<List<string>> EmailExtractedIntoLead(string NewValues, int InboundID, int OrgID, string sDatabase, string SubscriptionID, List<string> Status, int iUserID, string sOrgName)
        {
            var sOrgDB = Common.GetUserDetails(iUserID, sOrgName, sDatabase).sUserDatabase;
            DataContext Spdb = new DataContext(sOrgDB);
            var SubScrpDetails = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubscriptionID).FirstOrDefault();
            var SubScrpColumns = Spdb.SubscriptionColumns.Where(m => m.OrganizationID == OrgID && m.SubscriptionID == SubscriptionID).ToList();
            int BOID = dbContext.BOs.Where(m => m.Name == EnumLeadTables.Leads.ToString()).Select(m => m.BOID).FirstOrDefault();
            var BoFields = dbContext.BOFields.Where(m => m.BOID == BOID).ToList();
            var MasterFields = dbContext.MasterTemplates.Where(m => m.ClassID == SubScrpDetails.ClassID).ToList();
            var MappedFields = Spdb.MappedFields.Where(u => u.OrganizationID == OrgID && u.ClassID == SubScrpDetails.ClassID).ToList();
            int Class = SubScrpDetails.ClassID;
            int SourceID = SubScrpDetails.SourceID;
            var lLeadID = new List<long>();
            string ClientsTable = EnumLeadTables.LeadClients.ToString();
            var lSuccessDatas = new List<List<string>>();
            var lColValNull = new List<VMImproperData>();
            long LeadID = 0;
            try
            {
                var Key = "";
                var Value = "";

                //Leads....
                var keyValuePairs = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                List<string> NewKeyValuePair = new List<string>();
                NewKeyValuePair = NewValues.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

                //check if the key values is in the next line with out ":".....
                for (int key = 0; key < keyValuePairs.Count; key++)
                {
                    if (keyValuePairs[key].Contains("<>"))
                    {
                        //nothing to be done...
                    }
                    else
                    {
                        //int j;
                        if (keyValuePairs[key].Contains("<>"))
                        {
                            //Do not Add...
                        }
                        else
                        {
                            var ValuesAppend = keyValuePairs[key - 1] + " " + keyValuePairs[key] + "\r\n";
                            string GetKey = keyValuePairs[key - 1];
                            var KeyIndex = NewKeyValuePair.IndexOf(GetKey);
                            //NewKeyValuePair.RemoveAt(key);
                            //replace old with new one
                            NewKeyValuePair[KeyIndex] = ValuesAppend;
                            NewKeyValuePair.RemoveAt(KeyIndex + 1);
                        }
                    }
                }
                //====Add SourceID and LeadClassID to the keyValuesPair
                LeadEngine Engine = new LeadEngine();
                NewKeyValuePair.Add("FKiSourceID<>" + SourceID);
                VMLeadEngine result = Engine.GetEmailLeadDetails(NewKeyValuePair, OrgID, sDatabase, Class);
                string sMainTable = EnumLeadTables.Leads.ToString();
                if (result.LeadID > 0)
                {
                    if (result.InsertToInstance)
                    {
                        EmailExtractedIntoInstance(NewValues, InboundID, OrgID, sDatabase, SubscriptionID, result.LeadID);
                    }
                    string UpdateStatements = "";
                    LeadID = result.LeadID;
                    lLeadID.Add(result.LeadID);
                    NewKeyValuePair.Add("FKiLeadClassID<>" + Class);
                    foreach (var items in NewKeyValuePair)
                    {
                        if (items.Contains("<>"))
                        {
                            var keyvalues = Regex.Split(items, "<>");
                            Key = keyvalues[0].TrimStart().TrimEnd();
                            Value = keyvalues[1].TrimStart().TrimEnd();
                        }
                        //
                        if (Value == "")
                        {
                            var DValue = SubScrpColumns.Where(m => m.FieldName == Key).FirstOrDefault();
                            var DataType = BoFields.Where(m => m.Name == Key).Select(m => m.TypeID).FirstOrDefault();
                            string sDatatype = ((BODatatypes)DataType).ToString().ToLower();
                            if (DValue != null)
                            {
                                if (sDatatype == "datetime")
                                {
                                    DateTime Date = DateTime.ParseExact(DValue.FieldValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    //DateTime Date = Convert.ToDateTime(DValue.FieldValue);
                                    UpdateStatements = UpdateStatements + Key + "='" + Date + "', ";
                                }
                                else
                                {
                                    UpdateStatements = UpdateStatements + Key + "='" + DValue.FieldValue + "', ";
                                }
                            }
                        }
                        else
                        {
                            MappedFields MappedColumnData = new MappedFields();
                            var ColumnName = "";
                            var DataType = "";
                            MasterTemplates TemplateColumnData = MasterFields.Where(m => m.DataFieldName == Key).FirstOrDefault();
                            if (TemplateColumnData != null)
                            {
                                ColumnName = TemplateColumnData.FieldName;
                                DataType = TemplateColumnData.FieldType;
                            }
                            else
                            {
                                if (Key.IndexOf("AddField", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    MappedColumnData = MappedFields.Where(u => u.AddField == Key).FirstOrDefault();
                                }
                                else
                                {
                                    MappedColumnData = MappedFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                }

                                if (MappedColumnData != null)
                                {
                                    ColumnName = MappedColumnData.AddField;
                                    DataType = MappedColumnData.FieldType;
                                }
                            }

                            string DataTypeValue = (DataType).ToLower();
                            string GetDate = "";
                            if (ColumnName != "" && ColumnName != null)
                            {
                                //CHECKS for the DATETIME DATATYPE
                                if (DataTypeValue == "datetime")
                                {
                                    if (Value != "1/1/0001" && Value != "01/01/0001")
                                    {
                                        if (ColumnName == "dImportedOn")
                                        {
                                            UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                        }
                                        else
                                        {
                                            if (Value != "1/1/0001" && Value != "01/01/0001")
                                            {
                                                DateTime datetime;
                                                string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                                //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss tt", "M/d/yyyy" };
                                                foreach (var PatternType in Pattern)
                                                {
                                                    if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                    {
                                                        if (ColumnName == "dDOB")
                                                        {
                                                            GetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                        }
                                                        else
                                                        {
                                                            GetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //error message
                                                    }
                                                }
                                                UpdateStatements = UpdateStatements + ColumnName + "='" + GetDate + "', ";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                }
                            }
                        }
                    }
                    UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                    UpdateStatements = "Update [dbo].[" + sMainTable + "] set " + UpdateStatements + " WHERE [ID]='" + result.LeadID + "'";
                    Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                }
                else
                {
                    string UpdateStatements = "";
                    NewKeyValuePair.Add("InBoundID<>" + InboundID);
                    NewKeyValuePair.Add("dImportedOn<>" + DateTime.Now);
                    NewKeyValuePair.Add("FKiOrgID<>" + OrgID);
                    NewKeyValuePair.Add("FKiLeadClassID<>" + Class);
                    NewKeyValuePair.Add("iStatus<>" + 0);
                    string LeadsTable = EnumLeadTables.Leads.ToString();
                    LeadID = Spdb.Database.SqlQuery<long>("INSERT INTO " + LeadsTable + " (FKiSourceID) VALUES ('" + SourceID + "'); SELECT CAST( SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                    lLeadID.Add(LeadID);
                    foreach (var items in NewKeyValuePair)
                    {
                        if (items.Contains("<>"))
                        {
                            var keyvalues = Regex.Split(items, "<>");
                            Key = keyvalues[0].TrimStart().TrimEnd();
                            Value = keyvalues[1].TrimStart().TrimEnd();
                        }

                        if (Value != "")
                        {
                            var ColumnName = "";
                            var DataType = "";
                            MasterTemplates TemplateColumnData = MasterFields.Where(m => m.DataFieldName == Key).FirstOrDefault();
                            if (TemplateColumnData != null)
                            {
                                ColumnName = TemplateColumnData.FieldName;
                                DataType = TemplateColumnData.FieldType;
                            }
                            else
                            {
                                MappedFields MappedColumnData = new MappedFields();
                                if (Key.IndexOf("AddField", StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    MappedColumnData = MappedFields.Where(u => u.AddField == Key).FirstOrDefault();
                                }
                                else
                                {
                                    MappedColumnData = MappedFields.Where(u => u.FieldName == Key).FirstOrDefault();
                                }
                                if (MappedColumnData != null)
                                {
                                    ColumnName = MappedColumnData.AddField;
                                    DataType = MappedColumnData.FieldType;
                                }
                            }
                            string DataTypeValue = (DataType).ToLower();
                            string GetDate = "";
                            if (ColumnName != "" && ColumnName != null)
                            {
                                if (DataTypeValue == "datetime")
                                {
                                    if (Value != "1/1/0001" && Value != "01/01/0001")
                                    {
                                        if (ColumnName == "dImportedOn")
                                        {
                                            UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                        }
                                        else
                                        {
                                            if (Value != "1/1/0001" && Value != "01/01/0001")
                                            {
                                                DateTime datetime;
                                                //string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy", "M/d/yyyy hh:mm:ss", "M/d/yyyy" };
                                                string[] Pattern = { "d/M/yyyy hh:mm:ss", "d/M/yyyy" };
                                                foreach (var PatternType in Pattern)
                                                {
                                                    if (DateTime.TryParseExact(Value, PatternType, null, DateTimeStyles.None, out datetime))
                                                    {
                                                        if (ColumnName == "dDOB")
                                                        {
                                                            GetDate = datetime.ToString("MM/dd/yyyy 00:00:00.000");
                                                        }
                                                        else
                                                        {
                                                            GetDate = datetime.ToString("MM/dd/yyyy hh:mm:ss tt");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //error message
                                                    }
                                                }
                                                UpdateStatements = UpdateStatements + ColumnName + "='" + GetDate + "', ";
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    UpdateStatements = UpdateStatements + ColumnName + "='" + Value + "', ";
                                }
                            }
                        }
                        else
                        {
                            var DValue = SubScrpColumns.Where(m => m.FieldName == Key).FirstOrDefault();
                            var DataType = BoFields.Where(m => m.Name == Key).Select(m => m.TypeID).FirstOrDefault();
                            string sDatatype = ((BODatatypes)DataType).ToString().ToLower();
                            if (DValue != null)
                            {
                                if (sDatatype == "datetime")
                                {
                                    DateTime Date = DateTime.ParseExact(DValue.FieldValue, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    UpdateStatements = UpdateStatements + Key + "='" + Date + "', ";
                                }
                                else
                                {
                                    UpdateStatements = UpdateStatements + Key + "='" + DValue.FieldValue + "', ";
                                }
                            }
                            //Save Null value.
                        }
                    }
                    UpdateStatements = UpdateStatements.Substring(0, UpdateStatements.Length - 2);
                    UpdateStatements = "Update [dbo].[" + LeadsTable + "] set " + UpdateStatements + " WHERE [ID]='" + LeadID + "'";
                    Spdb.Database.ExecuteSqlCommand(UpdateStatements);
                    //insert into leadinstance table
                    EmailExtractedIntoInstance(NewValues, InboundID, OrgID, sDatabase, SubscriptionID, LeadID);
                    long iClientID = 0;
                    if (result.ClientID == 0)
                    {
                        VMLeads LeadData = Spdb.Database.SqlQuery<VMLeads>("SELECT sForeName,sLastName,sMob,sEmail,FKiLeadClassID FROM [dbo].[" + LeadsTable + "] WHERE [ID] = '" + LeadID + "'").FirstOrDefault();
                        iClientID = Spdb.Database.SqlQuery<long>("INSERT INTO " + ClientsTable + " (Name,Email,Mobile,ClassID,InBoundID,OrganizationID) VALUES ('" + LeadData.sForeName + " " + LeadData.sLastName + "','" + LeadData.sEmail + "','" + LeadData.sMob + "','" + Class + "','" + InboundID + "','" + OrgID + "'); SELECT CAST(SCOPE_IDENTITY() as bigint)").FirstOrDefault();
                    }
                    Spdb.Database.ExecuteSqlCommand("UPDATE " + LeadsTable + " SET FKiClientID =" + "'" + iClientID + "'" + " " + "WHERE" + " ID=" + LeadID + "");

                    //Priority
                    string sQueryPriority = dbContext.Reports.Where(m => m.Name == "Prority").Select(m => m.Query).FirstOrDefault();
                    string sPriority = sQueryPriority.Replace("ID = 0", "ID = " + LeadID);
                    Spdb.Database.ExecuteSqlCommand(sPriority);
                    //Users
                    AssignUserID(LeadID, OrgID, sDatabase);
                    //SendNotificationForAndroid("New lead posted into system");
                    //Finance
                    string sQueryFinance = "Update " + EnumLeadTables.Leads.ToString() + " Set iFinance =" + SubScrpDetails.LeadCost + ", OrgHeirarchyID ='ORG" + OrgID + "_" + SubScrpDetails.LocationCode.ToUpper() + "' Where ID =" + LeadID;
                    Spdb.Database.ExecuteSqlCommand(sQueryFinance);
                }
                if (Status.Count() > 0)
                {
                    var Alerts = string.Join("<BR>", Status);
                    Spdb.Database.ExecuteSqlCommand("Update " + EnumLeadTables.Leads.ToString() + " set sSystemAlert='" + Alerts + "' Where ID=" + LeadID);
                }
                Common Com = new Common();
                //change to LeadID
                Spdb.Database.ExecuteSqlCommand("UPDATE " + EnumLeadTables.LeadInbounds.ToString() + " SET StatusTypeID ='10'" + " " + "WHERE" + " ID=" + InboundID);
                string sBuildLeadID = "";
                for (int i = 0; i < lLeadID.Count; i++)
                {
                    sBuildLeadID = sBuildLeadID + " ID='" + lLeadID[i] + "' or ";
                }
                sBuildLeadID = sBuildLeadID.Substring(0, sBuildLeadID.Length - 4);
                using (SqlConnection Con = new SqlConnection(""))
                {
                    SqlCommand cmd = new SqlCommand();
                    Con.Open();
                    Con.ChangeDatabase(sOrgDB);
                    cmd.Connection = Con;
                    string sGetData = "SELECT [sName],[sForeName],[sLastName],[sMob],[sEmail],[dDOB],[sPostCode] FROM [dbo].[Leads] WHERE " + sBuildLeadID;
                    cmd.CommandText = sGetData;
                    SqlDataReader reader1 = cmd.ExecuteReader();
                    int count = reader1.FieldCount;
                    if (reader1.HasRows)
                    {
                        while (reader1.Read())
                        {
                            var lSuccessData = new List<string>();
                            for (int i = 0; i < count; i++)
                            {
                                //string EmailID = reader1.IsDBNull(i) ? null : reader1.GetValue(i).ToString();
                                lSuccessData.Add(reader1.IsDBNull(i) ? null : reader1.GetValue(i).ToString());
                                //if (i == 4)
                                //{
                                //    var UserExists = modeldb.AspNetUsers.Where(m => m.UserName == EmailID).FirstOrDefault();
                                //    Com.SendUserRegisterMail(EmailID, "Register", database, OrgID);
                                //}
                            }
                            lSuccessDatas.Add(lSuccessData);
                        }
                        List<string> Sucess = new List<string>();
                        Sucess.Add("Success");
                        lSuccessDatas.Insert(0, Sucess);
                        Con.Close();
                    }
                    else
                    {
                        List<string> Sucess = new List<string>();
                        Sucess.Add("Failure");
                        lSuccessDatas.Insert(0, Sucess);
                        Con.Close();
                    }
                }
                return lSuccessDatas;
            }
            catch (Exception ex)
            {
                //Change to LeadID
                Spdb.Database.ExecuteSqlCommand("UPDATE LeadInbounds SET StatusTypeID ='20'" + " " + "WHERE" + " ID=" + InboundID);
                List<string> Sucess = new List<string>();
                Sucess.Add("Failure");
                Sucess.Add(ex.Message);
                lSuccessDatas.Insert(0, Sucess);
                return lSuccessDatas;
            }
        }

        private void AssignUserID(long LeadID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            try
            {
                Common Com = new Common();
                int UserID = 0, NewUserID = 0, index = -1;
                var AllUsers = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).ToList();
                string database = AllUsers.FirstOrDefault().sDatabaseName;
                DataContext Spdb = new DataContext(database);
                var Users = AllUsers.Select(m => m.UserID).ToList();
                List<int> UserIDs = new List<int>();
                foreach (var items in Users)
                {
                    var RoleID = dbCore.XIAppUserRoles.Where(m => m.UserID == items).Select(m => m.RoleID).FirstOrDefault();
                    var RoleName = dbCore.XIAppRoles.Where(m => m.RoleID == RoleID).Select(m => m.sRoleName).FirstOrDefault();
                    if (RoleName != EnumRoles.Admin.ToString())
                    {
                        UserIDs.Add(items);
                    }
                }
                DateTime? LastAssignedTime = null;
                LastAssignedTime = Spdb.Database.SqlQuery<DateTime>("select max(assignedtime) from " + EnumLeadTables.Leads.ToString() + " where fkiorgid=" + OrgID).FirstOrDefault();
                if (LastAssignedTime != null)
                {
                    UserID = Spdb.Database.SqlQuery<int>("select userid from " + EnumLeadTables.Leads.ToString() + " where fkiorgid=" + OrgID + " and AssignedTime='" + LastAssignedTime + "'").FirstOrDefault();
                }
                if (UserIDs.Count() > 0)
                {
                    if (UserID > 0)
                    {
                        index = UserIDs.IndexOf(UserID);
                    }
                    else
                    {
                        index = 0;
                    }

                    if (index >= 0)
                    {
                        if (index == UserIDs.Count() - 1)
                        {
                            NewUserID = UserIDs[0];
                        }
                        else
                        {
                            NewUserID = UserIDs[index + 1];
                        }
                        if (NewUserID > 0)
                        {
                            int RLID = dbCore.XIAppUserRoles.Where(m => m.UserID == NewUserID).Select(m => m.RoleID).FirstOrDefault();
                            Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set userid=" + NewUserID + ", AssignedTime='" + DateTime.Now + "' where fkiorgid=" + OrgID + " and ID=" + LeadID + " and (userid is null or userid = 0)");
                        }
                    }
                    else
                    {
                        NewUserID = UserIDs[0];
                        int RLID = dbCore.XIAppUserRoles.Where(m => m.UserID == NewUserID).Select(m => m.RoleID).FirstOrDefault();
                        var AllRoles = Com.GetParentRoles(RLID, sDatabase);
                        Spdb.Database.ExecuteSqlCommand("update " + EnumLeadTables.Leads.ToString() + " set userid=" + NewUserID + ", AssignedTime='" + DateTime.Now + "' where fkiorgid=" + OrgID + " and ID=" + LeadID + " and (userid is null or userid = 0)");
                    }
                }
            }
            catch (Exception ex)
            {

            }

        }
        public MailExtractStrings GetMailExtractStringsRow(int ID, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            MailExtractStrings GetDetails = new MailExtractStrings();
            GetDetails = Spdb.MailExtractStrings.Find(ID);
            return GetDetails;
        }
        public List<VMDropDown> AddEditMailExtractStrings(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            var orgsource = Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID && m.StatusTypeID == 10).ToList();
            List<VMDropDown> AllSources = new List<VMDropDown>();
            AllSources = (from s in Spdb.OrganizationSubscriptions.Where(m => m.OrganizationID == OrgID).Where(m => m.StatusTypeID == 10)
                          join r in Spdb.OrganizationSources on s.SourceID equals r.ID
                          join c in Spdb.OrganizationClasses on s.ClassID equals c.ClassID
                          select new VMDropDown { text = r.Name + "-" + c.Class, Expression = s.SubscriptionID }).ToList();
            return AllSources;

        }
        public VMCustomResponse SaveMailExtractStrings(VMMailExtractStrings model, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            if (model.ID == 0)//ADD
            {
                var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                DataContext Spdb = new DataContext(database);
                MailExtractStrings sub = new MailExtractStrings();
                sub.SubscriptionID = model.SubscriptionID;
                sub.sStartString = model.sStartString;
                sub.sEndString = model.sEndString;
                sub.StatusTypeID = model.StatusTypeID;
                sub.OrganizationID = OrgID;
                Spdb.MailExtractStrings.Add(sub);
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = sub.ID, Status = true };
            }
            else//EDIT
            {
                var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
                DataContext Spdb = new DataContext(database);
                MailExtractStrings sub = new MailExtractStrings();
                sub = Spdb.MailExtractStrings.Find(model.ID);
                sub.sStartString = model.sStartString;
                sub.sEndString = model.sEndString;
                sub.StatusTypeID = model.StatusTypeID;
                sub.OrganizationID = OrgID;
                Spdb.SaveChanges();
                return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = model.ID, Status = true };
            }
        }

        public DTResponse MailExtractStringsGrid(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            DataContext Spdb = new DataContext(database);
            IQueryable<MailExtractStrings> AllStrings;
            List<MailExtractStrings> Strings = new List<MailExtractStrings>();
            if (OrgID == 0)
            {
                foreach (var items in dbs)
                {
                    DataContext AllDb = new DataContext(items);
                    Strings.AddRange(AllDb.MailExtractStrings.Where(x => x.OrganizationID == OrgID).ToList());
                }
                AllStrings = Strings.AsQueryable();
            }
            else
            {
                AllStrings = Spdb.MailExtractStrings.Where(x => x.OrganizationID == OrgID);
            }
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                //AllStrings = AllStrings.Where(m => m.Name.Contains(param.sSearch));
            }
            int displyCount = 0;
            displyCount = AllStrings.Count();
            AllStrings = QuerableUtil.GetResultsForDataTables(AllStrings, "", sortExpression, param);
            var clients = AllStrings.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = from c in clients
                     select new[] {
                                (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID, GetSubscriptionName(c.SubscriptionID,database),Convert.ToString(c.OrganizationID), c.sStartString,c.sEndString,c.StatusTypeID.ToString(), ""};
            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        public DTResponse MailExtractStringsPopUpGrid(jQueryDataTableParamModel param, int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            IEnumerable<MailExtractStrings> AllSubs, FilteredSubs;
            int displyCount = 0;
            var sortDirection = param.sSortDir;
            var sortColumnIndex = param.iSortCol;
            int i = param.iDisplayStart + 1;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                FilteredSubs = Spdb.MailExtractStrings.Where(x => x.OrganizationID == OrgID).ToList();
                AllSubs = FilteredSubs.OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
                displyCount = FilteredSubs.Count();

            }
            else
            {
                displyCount = Spdb.MailExtractStrings.Where(x => x.OrganizationID == OrgID).Count();
                AllSubs = Spdb.MailExtractStrings.Where(x => x.OrganizationID == OrgID).OrderByDescending(m => m.ID).Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            }


            var result = (from c in AllSubs
                          select new[] {
                             (i++).ToString(), Convert.ToString(c.ID), c.SubscriptionID, GetSubscriptionName(c.SubscriptionID,database), c.sStartString,c.sEndString}).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetSubscriptionName(string p, string database)
        {
            if (p != null)
            {
                DataContext Spdb = new DataContext(database);
                int sourceid, classid; string sourcename, classname, subIdtext;
                //int id = Convert.ToInt32(p);
                var orgsource = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == p);
                sourceid = orgsource.FirstOrDefault().SourceID;
                classid = orgsource.FirstOrDefault().ClassID;

                var sname = Spdb.OrganizationSources.Where(m => m.ID == sourceid).ToList();
                sourcename = sname.FirstOrDefault().Name;

                var cname = Spdb.OrganizationClasses.Where(m => m.ClassID == classid).ToList();
                classname = cname.FirstOrDefault().Class;

                subIdtext = sourcename + " - " + classname;

                return subIdtext;
            }
            else
            {
                return null;
            }

        }
        public List<VMDropDown> GetAllMailIDs(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            var database = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == OrgID).Select(m => m.sDatabaseName).FirstOrDefault();
            DataContext Spdb = new DataContext(database);
            List<VMDropDown> AllMailIDs = new List<VMDropDown>();
            AllMailIDs = (from c in dbContext.IOServerDetails.Where(m => m.OrganizationID == OrgID && m.Type == 20 && m.StatusTypeID == 10).ToList()
                          select new VMDropDown { text = c.UserName, Value = c.ID }).ToList();
            return AllMailIDs;
        }

        public List<List<string>> InsertAPIDataToInbound(Leads Model, int iUserID, string sOrgName, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<List<string>> Status = new List<List<string>>();
            string RawData = "";
            List<string> Values = new List<string>();
            Dictionary<string, string> myDict = new Dictionary<string, string>();
            var t = Model.GetType();
            foreach (PropertyInfo pi in t.GetProperties())
            {
                string type = pi.PropertyType.Name.ToString();
                if (type == "DateTime")
                {
                    DateTime Date = Convert.ToDateTime(pi.GetValue(Model, null));
                    myDict[pi.Name] = Date.ToString("d");
                }
                else
                {
                    if (pi.Name == "FirstName")
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(Model);
                            if (value != null)
                            {
                                myDict["First Name"] = pi.GetValue(Model, null).ToString();
                            }
                            else
                            {
                                myDict["First Name"] = null;
                            }
                        }
                    }
                    else if (pi.Name == "LastName")
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(Model);
                            if (value != null)
                            {
                                myDict["Last Name"] = pi.GetValue(Model, null).ToString();
                            }
                            else
                            {
                                myDict["Last Name"] = null;
                            }
                        }
                    }
                    else if (pi.Name == "MobileNumber")
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(Model);
                            if (value != null)
                            {
                                myDict["Mobile Number"] = pi.GetValue(Model, null).ToString();
                            }
                            else
                            {
                                myDict["Mobile Number"] = null;
                            }
                        }
                    }
                    else if (pi.Name == "TelephoneNumber")
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(Model);
                            if (value != null)
                            {
                                myDict["Telephone Number"] = pi.GetValue(Model, null).ToString();
                            }
                            else
                            {
                                myDict["Telephone Number"] = null;
                            }
                        }
                    }
                    else
                    {
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(Model);
                            if (value != null)
                            {
                                myDict[pi.Name] = pi.GetValue(Model, null).ToString();
                            }
                            else
                            {
                                myDict[pi.Name] = null;
                            }
                        }
                        else if (pi.PropertyType == typeof(int))
                        {
                            int value = (int)pi.GetValue(Model);
                            myDict[pi.Name] = pi.GetValue(Model, null).ToString();
                        }
                    }
                }
            }
            foreach (var items in myDict)
            {
                Values.Add(items.Key + ":" + items.Value);
            }
            string SubscriptionID = myDict.Where(m => m.Key == "SubscriptionID").Select(m => m.Value).FirstOrDefault();
            var Dbs = dbCore.XIAppUsers.Select(m => m.sDatabaseName).Distinct().ToList();
            OrganizationSubscriptions SUb = new OrganizationSubscriptions();
            OrganizationSubscriptions NewSUb = new OrganizationSubscriptions();
            foreach (var items in Dbs)
            {
                DataContext Spdb = new DataContext(items);
                SUb = Spdb.OrganizationSubscriptions.Where(m => m.SubscriptionID == SubscriptionID).FirstOrDefault();
                if (SUb != null)
                {
                    NewSUb = SUb;
                }
            }
            if (Values.Count() > 0)
            {
                RawData = string.Join("\r\n ", Values.ToArray());
            }
            if (NewSUb != null)
            {
                var User = dbCore.XIAppUsers.Where(m => m.FKiOrganisationID == NewSUb.OrganizationID).FirstOrDefault();
                DataContext Database = new DataContext(User.sDatabaseName);
                ImportHistories History = new ImportHistories();
                History.FileType = "WebApi";
                History.ImportedOn = DateTime.Now;
                History.OriginalName = "WebApi";
                History.OrganizationID = NewSUb.OrganizationID;
                History.UserID = User.UserID;
                History.StatusTypeID = 1;
                Database.ImportHistories.Add(History);
                Database.SaveChanges();
                LeadInbounds Inbound = new LeadInbounds();
                Inbound.SubscriptionID = NewSUb.SubscriptionID;
                Inbound.Content = RawData;
                Inbound.ImportedOn = DateTime.Now;
                Inbound.SourceID = NewSUb.SourceID;
                Inbound.ImportedBy = User.sFirstName;
                Inbound.FileID = History.ID;
                Database.LeadInbounds.Add(Inbound);
                Database.SaveChanges();
                Status = ExtractEmailData(Inbound.ID, NewSUb.OrganizationID, User.sDatabaseName, iUserID, sOrgName);
                return Status;
            }
            else
            {
                List<string> Error = new List<string>();
                Error.Add("Not A Valid Subscription");
                Status.Add(Error);
                return Status;
            }
        }

        #region AppNotifications

        public DTResponse AppNotificationsGrid(jQueryDataTableParamModel param, int OrgID, string database)
        {
            ModelDbContext dbCore = new ModelDbContext(database);
            var AllRoles = dbCore.XIAppRoles.ToList();
            var AllUsers = dbCore.XIAppUsers.ToList();
            DataContext Spdb = new DataContext(database);
            IQueryable<VMAppNotifications> AllReports;
            List<VMAppNotifications> Notify = new List<VMAppNotifications>();
            AllReports = (from r in Spdb.AppNotifications.Where(m => m.OrganizationID == OrgID)
                          select new VMAppNotifications
                          {
                              ID = r.ID,
                              OrganizationID = r.OrganizationID,
                              Icon = r.Icon,
                              RoleID = r.RoleID,
                              UserID = r.UserID,
                              Message = r.Message,
                              //RoleName = GetRoleName(r.RoleID),
                              //RoleName = dbcontext.AspNetGroups.Where(m => m.OrganizationID == OrgID && m.Id == r.RoleID).Select(m => m.RoleName).FirstOrDefault(),
                              //RoleName = GetRoleName(r.RoleID),
                              StatusTypeID = r.StatusTypeID
                          });
            //IEnumerable<VMAppNotifications> Nofify;
            //AllReports = AllReports.Select(x => new VMAppNotifications
            //              {
            //                  ID = x.ID,
            //                  OrganizationID = x.OrganizationID,
            //                  Icon = x.Icon,
            //                  RoleID = x.RoleID,
            //                  UserID = x.UserID,
            //                  Message = x.Message,
            //                  //RoleName = GetRoleName(x.RoleID),
            //                  //RoleName = dbcontext.AspNetGroups.Where(m => m.OrganizationID == OrgID && m.Id == r.RoleID).Select(m => m.RoleName).FirstOrDefault(),
            //                  //RoleName = GetRoleName(r.RoleID),
            //                  StatusTypeID = x.StatusTypeID
            //              });
            foreach (var items in AllReports)
            {
                if (items.RoleID > 0)
                {
                    items.RoleName = AllRoles.Where(m => m.RoleID == items.RoleID).Select(m => m.sRoleName).FirstOrDefault();
                }
                if (items.UserID > 0)
                {
                    items.UserName = AllUsers.Where(m => m.UserID == items.UserID).Select(m => m.sFirstName).FirstOrDefault();
                }
                Notify.Add(items);
            }
            string sortExpression = "ID";
            if (!string.IsNullOrWhiteSpace(param.sSearch))
            {
                Notify = Notify.Where(m => m.Message.Contains(param.sSearch)).ToList();
            }
            int displyCount = 0;
            displyCount = Notify.Count();
            AllReports = QuerableUtil.GetResultsForDataTables(Notify.AsQueryable(), "", sortExpression, param);
            var clients = AllReports.ToList();
            int i = param.iDisplayStart + 1;
            IEnumerable<string[]> result;
            result = (from d in clients
                      select new[] {
                                    (i++).ToString(), Convert.ToString(d.ID), d.OrganizationID.ToString() , d.Icon,d.RoleName, d.UserName , d.Message , d.StatusTypeID.ToString(),""}).ToList();

            return new DTResponse()
            {
                sEcho = param.sEcho,
                iTotalRecords = displyCount,
                iTotalDisplayRecords = displyCount,
                aaData = result
            };
        }

        private string GetRoleName(int p, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            return dbCore.XIAppRoles.Where(m => m.RoleID == p).Select(m => m.sRoleName).FirstOrDefault();
        }
        public VMAppNotifications EditAppNotifications(int ID, string database)
        {
            try
            {
                DataContext Spdb = new DataContext(database);
                var model = Spdb.AppNotifications.Find(ID);
                var obj = new VMAppNotifications();
                obj.ID = model.ID;
                obj.OrganizationID = model.OrganizationID;
                obj.Icon = model.Icon;
                obj.RoleID = model.RoleID;
                obj.UserID = model.UserID;
                obj.Message = model.Message;
                obj.StatusTypeID = model.StatusTypeID;
                return obj;
            }
            catch (Exception ex)
            {
                return new VMAppNotifications();
            }
        }

        public VMCustomResponse SaveAppNotification(VMAppNotifications model, int UserID, string database)
        {
            AppNotifications App = new AppNotifications();
            DataContext Spdb = new DataContext(database);
            if (model.ID == 0)
            {
                App.OrganizationID = model.OrganizationID;
                App.Icon = model.Icon;
                App.RoleID = model.RoleID;
                App.UserID = model.UserID;
                App.Message = model.Message;
                App.StatusTypeID = model.StatusTypeID;
                App.CreatedBy = UserID;
                App.CreatedTime = DateTime.Now;
                App.UpdatedBy = UserID;
                App.UpdatedTime = DateTime.Now;
                Spdb.AppNotifications.Add(App);
            }
            else
            {
                App = Spdb.AppNotifications.Find(model.ID);
                if (model.Icon == null)
                    App.Icon = App.Icon;
                else
                    App.Icon = model.Icon;
                App.RoleID = model.RoleID;
                App.UserID = model.UserID;
                App.Message = model.Message;
                App.StatusTypeID = model.StatusTypeID;
                App.UpdatedBy = UserID;
                App.UpdatedTime = DateTime.Now;
            }
            Spdb.SaveChanges();
            return new VMCustomResponse { ResponseMessage = ServiceConstants.SuccessMessage, ID = App.ID, Status = true };
        }

        public AppNotifications SendNotification(int ID, string database)
        {
            AppNotifications App = new AppNotifications();
            DataContext Spdb = new DataContext(database);
            App = Spdb.AppNotifications.Find(ID);
            var Response = SendNotificationForAndroid(App.Message, App.RoleID, App.UserID, App.Icon, database);
            //Spdb.SaveChanges();
            return App;
        }

        public string SendNotificationForAndroid(string Message, int RoleID, int UserID, string Image, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            string physicalPath = System.Web.Hosting.HostingEnvironment.MapPath("~");
            string ImagesPath = System.Configuration.ConfigurationManager.AppSettings["XIDNAPath"];
            if (Image != null)
            {
                ImagesPath = ImagesPath + "\\Content\\images\\AppImages\\" + Image;
            }
            List<string> devicekeys = new List<string>();
            var DeviceKeys = dbContext.DeviceKeys.ToList();
            if (RoleID > 0)
            {
                var AllUsers = dbCore.XIAppUserRoles.Where(m => m.RoleID == RoleID).ToList();
                var ActiveUsers = (from c in DeviceKeys.Where(x => AllUsers.Any(y => y.UserID == x.LoginId)) select c).ToList();
                var dkeys = ActiveUsers;
                foreach (var d in dkeys)
                {
                    devicekeys.Add(d.KeyDevice);
                }
            }
            if (UserID > 0)
            {
                var Keys = DeviceKeys.Where(m => m.LoginId == UserID).ToList();
                foreach (var d in Keys)
                {
                    devicekeys.Add(d.KeyDevice);
                }
            }
            try
            {
                //string url = "D:\\TfsProjects\\XIDNA\\XIDNA\\Content\\images\\advSearch.png";
                //string url = "http://www.appwellness.org/Content/dist/img/apbhadratha_logo.png";
                //var AndroidDeviceKey = "dNqF6aFSYdg:APA91bGSfi2EQajqSiKnU4Dy7wvm-50Xu9sRRCaXbzTMosmRqTfwT5Z2GhFLRZy_sDGwsAMb3AiPBbQoiulO61-MmzfQni3R8psz6tNozLlGzh8XDEv1DXg-QXG8CvDMcXjJGExQPJ9R";
                //string message = ukd.KpiName + " " + "Threshold Reached";
                string deviceId = "";
                foreach (var item in devicekeys)
                {
                    deviceId += item + "\",\"";
                }
                deviceId = deviceId.TrimEnd('"', ',', '"');
                string GoogleAppID = "AIzaSyCGSmewn4KiuZE8IrCHAphqVS4fBo4tar4";
                var SENDER_ID = "64138374419";
                WebRequest tRequest;
                tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
                tRequest.Headers.Add(string.Format("Authorization: key={0}", GoogleAppID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));
                tRequest.Method = "POST";
                tRequest.ContentType = "application/json";
                string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"title\" : " + "\"" + "XIDNA" + "\",\"message\" : " + "\"" + Message + "\",\"inbox\": " + "\"" + "picture" + "\",\"picture\" : " + "\"" + ImagesPath + "\",\"image\" : " + "\"" + ImagesPath + "\"},\"registration_ids\":[\"" + deviceId + "\"]}";
                //string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"title\" : " + "\"" + "XIDNA" + "\",\"message\" : " + "\"" + Message + "\",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + deviceId + "\"]}";
                Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return sResponseFromServer;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //public int SaveNotificationImage(int id, string FileName, string database)
        //{
        //    AppNotifications App = new AppNotifications();
        //    DataContext Spdb = new DataContext(database);
        //    App = Spdb.AppNotifications.Find(id);
        //    App.ImageName = FileName;
        //    Spdb.SaveChanges();
        //    var Response = SendNotificationForAndroid(App.Message, App.RoleID, FileName);
        //    return id;
        //}

        public List<VMDropDown> GetOrgRoles(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<VMDropDown> AllRoles = new List<VMDropDown>();
            AllRoles = (from c in dbCore.XIAppRoles.Where(m => m.FKiOrganizationID == OrgID).ToList()
                        select new VMDropDown { Value = c.RoleID, text = c.sRoleName }).ToList();
            return AllRoles;
        }
        public List<VMDropDown> GetUsers(int OrgID, string sDatabase)
        {
            ModelDbContext dbCore = new ModelDbContext(sDatabase);
            List<VMDropDown> AllUsers = new List<VMDropDown>();
            AllUsers = (from c in dbCore.XIAppUsers.ToList().Where(m => m.FKiOrganisationID == OrgID)
                        join d in dbCore.XIAppUserRoles on c.UserID equals d.UserID
                        select new VMDropDown { Value = d.UserID, text = c.sUserName }).ToList();
            return AllUsers;
        }
        #endregion AppNotifications
    }
}
