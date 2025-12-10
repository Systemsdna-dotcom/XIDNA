using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.Repository;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net.Mime;
using XIDNA.Models;
using XIDNA.ViewModels;
using System.Security.Claims;
using System.Threading;
using System.Text;
using System.Security.Cryptography;
using System.Configuration;
using System.Reflection;
using System.Web.Configuration;
using System.Net;

namespace XIDNA.Common
{
    public static class Util
    {
        
        public static int SendMail(string Email, string NewPassword, string FirstName)
        {
            int OrgID = 1;
            ModelDbContext dbcontext = new ModelDbContext();
            var sDetails = dbcontext.IOServerDetails.Where(m => m.Type == 1).Where(m => m.OrganizationID == OrgID).ToList();
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
            string messageBody = "Dear " + FirstName + ",<br/> We have received a request to reset the password for your account on XIDNA.<br/>Your temporary password is " + NewPassword;
            int n = 0;

            //string DateFields = field.Replace("((Today))", today.ToString()).Replace("((Tomorrow))", tomorrow.ToString()).Replace("((Yesterday))", yesterday.ToString());
            string Cc = "";
            List<string> ContentIds = new List<string>();
            List<string> Paths = new List<string>();
            string pattern = "(?<=src=\")[^,]+?(?=\")";
            string input = messageBody;
            if (messageBody.IndexOf("src=") >= 0)
            {
                int i = 1;
                string physicalPath = "", str = "", sFileName = "";
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
            if (Cc != "" && Cc != String.Empty && Cc != null)
            {
                msg.CC.Add(Cc);
            }
            //checking that if there exists BCC
            //if (BCC != "" && BCC != null)
            //{
            //    msg.Bcc.Add(BCC);
            //}
            msg.To.Add(Email);
            msg.From = new MailAddress(sender);
            msg.Subject = "Forgot Password";
            msg.Body = messageBody;
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
            return 0;
        }
        
        public static string SentOTPToUser(string PhoneNumber, string Message)
        {
            try
            {
                //string OTP = GenerateOTP();
                string BulkSMSPath = WebConfigurationManager.AppSettings["BulkSMSPath"];
                string SMSAPIKey = WebConfigurationManager.AppSettings["SMSAPIKey"];
                string Sender = WebConfigurationManager.AppSettings["isender"];
                string strResult = BulkSMSPath + "authkey=" + SMSAPIKey + "&mobiles=" + PhoneNumber + "&message=" + Message + "&sender=" + Sender + "&route = 6" + "&country = 0";
                WebClient WebClient = new WebClient();
                System.IO.StreamReader reader = new System.IO.StreamReader(WebClient.OpenRead(strResult));
                dynamic ResultHTML = reader.ReadToEnd();
                return PhoneNumber;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }    
}