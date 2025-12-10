using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using XICore;

namespace XIDNA.Mailer
{
    public class UserMailer : MailerBase, IUserMailer
    {
        public virtual MvcMailMessage SendUserAccountTemporaryPassword(string sHtmlContent, string sEmaiID, string sTemporayPassWord)
        {
            ViewBag.sHtmlContent = sHtmlContent;
            ViewBag.sTemporayPassWord = sTemporayPassWord;
            ViewBag.EmailID = sEmaiID;
            return Populate(x =>
            {
                x.Subject = "Account Was Created Successfully";
                x.ViewName = "SendUserAccountTemporaryPassword";
                x.To.Add(sEmaiID);
            });
        }
        public virtual MvcMailMessage SendMailTemplateToUser(string sUserName, string sEmaiID, string sHtmlContent)
        {
            ViewBag.UserName = sUserName;
            ViewBag.HtmlContent = sHtmlContent;
            ViewBag.EmailID = sEmaiID;
            return Populate(x =>
            {
                x.Subject = "Welcome Mail";
                x.ViewName = "SendMailTemplate";
                x.To.Add(sEmaiID);
            });
        }
        public virtual MvcMailMessage SendPassword(string email, string UserName)
        {

            try
            {
                XIInfraEncryption xiinfra = new XIInfraEncryption();
                string EncryptedHash = xiinfra.EncryptData(UserName + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm tt"), true, "URP");
                ViewBag.Email = email;
                ViewBag.UserName = UserName;
                ViewBag.name = "http://localhost:53996/Account/ResetPasswordLink?key=" + EncryptedHash;
                return Populate(x =>
                {
                    x.Subject = "Zeeinsurance Password reset link";
                    x.ViewName = "PasswordResetLink";
                    x.To.Add(email);
                });

            }
            catch (Exception ex)
            {
                // log.Error("Error", ex);
                return null;
            }
        }
    }
}