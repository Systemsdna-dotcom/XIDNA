using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Compression;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using XIDNA.Repository;

namespace XIDNA.Common
{
    public class SessionTimeoutAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            CommonRepository Common = new CommonRepository();
            try
            {
                HttpContext ctx = HttpContext.Current;
                if (HttpContext.Current.Session["CoreDatabase"] == null)
                {
                    var ApplicationPath = System.Configuration.ConfigurationManager.AppSettings["ApplicationPath"];
                    var sURL = filterContext.HttpContext.Request.Url.ToString();
                    //Common.SaveErrorLog("Session End URL:" + sURL, "XIDNA");
                    var sCurrentURL = sURL.Replace(ApplicationPath, "");
                    var URLContent = sCurrentURL.Split('/').ToList();
                    if (URLContent.Count() > 1)
                    {
                        //Common.SaveErrorLog("Session End URL Content:" + sCurrentURL, "XIDNA");
                        FormsAuthentication.SignOut();
                        filterContext.Result = new RedirectResult("~/Account/SessionHandler");
                        Common.SaveErrorLog("Error in OnActionExecuting on Case1", "XIDNA");
                        return;
                    }
                    else
                    {
                        //Common.SaveErrorLog("Session End URL Content QuestionSet:" + sCurrentURL, "XIDNA");
                        FormsAuthentication.SignOut();
                        filterContext.Result = new RedirectResult(sURL);
                        Common.SaveErrorLog("Error in OnActionExecuting  on Case2", "XIDNA");
                        return;
                    }                    
                }
            }
            catch (Exception ex)
            {
                Common.SaveErrorLog("Error in OnActionExecuting while fetching URL: " + ex.ToString(), "XIDNA");
                FormsAuthentication.SignOut();
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
            base.OnActionExecuting(filterContext);
        }
    }

    public class CompressAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted)) return;

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = filterContext.HttpContext.Response;

            if (encodingsAccepted.Contains("deflate"))
            {
                response.AppendHeader("Content-encoding", "deflate");
                response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
            }
            else if (encodingsAccepted.Contains("gzip"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
            }
        }
    }
}