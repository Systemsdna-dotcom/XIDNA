using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XIDNA.Mailer
{
    public interface IUserMailer
    {
        MvcMailMessage SendUserAccountTemporaryPassword(string sUserName, string sEmaiID, string sTemporayPassWord);
        MvcMailMessage SendMailTemplateToUser(string sUserName, string sEmaiID, string sHtmlContent);
        MvcMailMessage SendPassword(string email, string UserName);
    }
}