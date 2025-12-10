using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using XIDNA.Repository;

namespace XIDNA.Common
{
    
    public class MultipleLogin : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool isAuthorized = base.AuthorizeCore(httpContext);

            string user = httpContext.User.Identity.Name;

            string access = httpContext.Session.SessionID;

            if (String.IsNullOrEmpty(user) || String.IsNullOrEmpty(access))
            {
                return isAuthorized;
            }

            SQLLogin sqlLogin = new SQLLogin();

            return sqlLogin.IsLoggedIn(user, access);
        }
    }
}