using XIDNA.Models;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web;
using System.Linq;
using System.Web.Security;
using System;
using System.Security.Claims;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using XIDNA.Common;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IdentityModel.Services;
using XICore;
//using Microsoft.AspNet.SignalR;
using XIDNA.Controllers;
using System.Data.SqlClient;
using XIDNA.Repository;
using System.Web.Helpers;
using XISystem;

namespace XIDNA
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868

    public class MvcApplication : System.Web.HttpApplication
    {
        readonly string _connString = ServiceUtil.GetClientConnectionString();
        string sDatabase = string.Empty;
        Boolean isDBDependencySignalRStarted = false;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));
            System.Web.Helpers.AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Newtonsoft.Json.Formatting.Indented,
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            };

            //try
            //{
            //    Boolean isDBDependencySignalREnabled = false;
            //    XIInfraCache oCache = new XIInfraCache();
            //    string sIDRef = (string)oCache.GetObjectFromCache(XIConstant.CacheConfig, "isDBDependencySignalREnabled");

            //    if (!String.IsNullOrEmpty(sIDRef) && (sIDRef.ToLower().Equals("true") || sIDRef.ToLower().Equals("yes"))) 
            //    { 
            //        isDBDependencySignalREnabled = true; 
            //    }

            //    if (isDBDependencySignalREnabled)
            //    {
            //        SqlDependency.Start(_connString);
            //        isDBDependencySignalRStarted = true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    //Log error to DB - The SQL Server Service Broker for the current database is not enabled.
            //}
            //GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);
            //GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(30);
            //GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(10);
        }
        protected void Application_End()
        {
            // Shut down SignalR Dependencies
            if (isDBDependencySignalRStarted) { 
                SqlDependency.Stop(_connString);
            }
        }

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            try
            {
                var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    if (authTicket != null && !authTicket.Expired)
                    {
                        var roles = authTicket.UserData.Split(',');
                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(new FormsIdentity(authTicket), roles);
                    }
                    else
                    {

                    }
                }
            }
            catch (CryptographicException cex)
            {
                FormsAuthentication.SignOut();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var error = Server.GetLastError();
            var cryptoEx = error as CryptographicException;
            if (cryptoEx != null)
            {
                FederatedAuthentication.WSFederationAuthenticationModule.SignOut();
                Server.ClearError();
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {
            try
            {
                string sessionId = this.Session.SessionID;
                /* CLEAR USER CACHE HERE*/

                /* REMOVE CACHE FOR LOGOUT SESSION */
                var oUserCache = HttpRuntime.Cache["XICache"];
                var oCacheobj = (XICacheInstance)oUserCache;
                if (oCacheobj != null)
                    oCacheobj.RemoveUserCache(sessionId);
                /*REMOVE USER CACHE USERNAME AND ROLENAME*/
                XIInfraCache.RemoveCacheWithKey("SSS_" + sessionId);
            }
            catch (Exception ex)
            {

            }
        }
        //protected void Application_AuthenticateRequest(object sender, EventArgs e)
        //{
        //    var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];

        //    if (authCookie != null)
        //    {
        //        var ticket = FormsAuthentication.Decrypt(authCookie.Value);

        //        FormsIdentity formsIdentity = new FormsIdentity(ticket);

        //        ClaimsIdentity claimsIdentity = new ClaimsIdentity(formsIdentity);

        //        Application_AcquireRequestState(sender, e);

        //        var Roles = GetUserRoles(ticket.Name);

        //        //foreach (var role in Roles)
        //        //{
        //        //    claimsIdentity.AddClaim(
        //        //        new Claim(ClaimTypes.Role, role.sRoleName));
        //        //}
        //        var UserDetails = GetUserDetails(ticket.Name);
        //        if (UserDetails != null)
        //        {
        //            claimsIdentity.AddClaim(
        //            new Claim(ClaimTypes.Rsa, UserDetails.sDatabaseName));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.SerialNumber, UserDetails.UserID.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.NameIdentifier, UserDetails.sUserName.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.Surname, UserDetails.sAppName.ToString()));
        //        }
        //        else
        //        {

        //        }
        //        if (Roles != null)
        //        {
        //            //claimsIdentity.AddClaim(
        //            //    new Claim(ClaimTypes.Name, UserDetails.sUserName.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.Role, Roles.sRoleName));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.GroupSid, Roles.iLayoutID.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.Sid, Roles.iThemeID.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.PrimarySid, Roles.FKiOrganizationID.ToString()));
        //            claimsIdentity.AddClaim(
        //                new Claim(ClaimTypes.PrimaryGroupSid, Roles.RoleID.ToString()));
        //        }
        //        else
        //        {

        //        }

        //        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //        Thread.CurrentPrincipal = claimsPrincipal;

        //        HttpContext.Current.User = claimsPrincipal;
        //    }
        //}

        //private cXIAppUsers GetUserDetails(string UserName)
        //{
        //    ModelDbContext dbContext;
        //    if (!string.IsNullOrEmpty(sDatabase))
        //    {
        //        dbContext = new ModelDbContext(sDatabase);
        //    }
        //    else
        //    {
        //        dbContext = new ModelDbContext();
        //    }
        //    var UserDetails = dbContext.XIAppUsers.Where(m => m.sUserName == UserName).FirstOrDefault();
        //    return UserDetails;
        //}

        //protected void Application_AcquireRequestState(object sender, EventArgs e)
        //{
        //    //Some code here
        //    if (HttpContext.Current != null && HttpContext.Current.Session != null && HttpContext.Current.SessionManager.CoreDatabase != null)
        //    {
        //        sDatabase = HttpContext.Current.SessionManager.CoreDatabase;
        //    }
        //}

        //private cXIAppRoles GetUserRoles(string UserName)
        //{
        //    ModelDbContext dbContext;
        //    if (!string.IsNullOrEmpty(sDatabase))
        //    {
        //        dbContext = new ModelDbContext(sDatabase);
        //    }
        //    else
        //    {
        //        dbContext = new ModelDbContext();
        //    }
        //    cXIAppRoles Roles = new cXIAppRoles();
        //    var UserID = dbContext.XIAppUsers.Where(m => m.sUserName == UserName).Select(m => m.UserID).FirstOrDefault();
        //    List<int> RoleIDs = dbContext.XIAppUserRoles.Where(m => m.UserID == UserID).Select(m => m.RoleID).ToList();
        //    Roles = dbContext.XIAppRoles.Where(m => RoleIDs.Contains(m.RoleID)).FirstOrDefault();
        //    return Roles;
        //}
    }
}
