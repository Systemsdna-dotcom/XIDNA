using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using XIDNA.Models;
using System.Reflection;
using XICore;

namespace XIDNA
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapMvcAttributeRoutes();
          //  routes.MapRoute(
          //    name: "UserRoute",
          //    url: "{Application}/{Org}/{Layout}",
          //    defaults: new { controller = "QuestionSet", action = "QuestionSet", id = UrlParameter.Optional },
          //    constraints: new { Application = new ApplicationRouteConstraint() }
          //);
            routes.MapRoute(
               name: "ApplicationRoute",
               url: "{Application}",
               defaults: new { controller = "QuestionSet", action = "StartUP", id = UrlParameter.Optional }
           );

            // routes.MapRoute(
            //    name: "ApplicationRoute",
            //    url: "{Application}",
            //    defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
            //    constraints: new { Application = new ApplicationRouteConstraint() }
            //);


            routes.MapRoute(null, "{controller}/{action}");

            routes.MapRoute(
               "Default",
               "{controller}/{action}/{id}",
               new { controller = "Account", action = "Login", id = UrlParameter.Optional },
               new[] { "XIDNA.Controllers" }
           );

        }
    }
    public class ApplicationRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            //these would usually come from a database, or cache.
            string CoreDatabase = System.Configuration.ConfigurationManager.AppSettings["CoreApplicatoinDatabase"];
            using (XICoreDbContext dbcontext = new XICoreDbContext(CoreDatabase))
            {
                List<string> lstOrg = dbcontext.XIApplications.Select(m => m.sApplicationName.ToLower()).ToList();
                lstOrg = lstOrg ?? new List<string>();

                //lstOrg.Add(EnumDefaultUrls.CorporateAdmin.ToString().ToLower());

                string[] Applications = lstOrg.ToArray();

                //string[] corporates = new[] { "restaurants", "cafes", "bistros", "welfare" };
                if (values[parameterName] == null)
                    return false;

                //get the category passed in to the route
                var category = values[parameterName].ToString();

                //now we check our corporates, and see if it exists
                var Apps = Applications.Any(x => x == category.ToLower());
                return Applications.Any(x => x == category.ToLower());
            }

            // url such as /restaurants/Camberley--Surrey will match
            // url such as /pubs/Camberley--Surrey will not
        }
    }

    public class UserRouteConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            asm.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type)) //filter controllers
                .SelectMany(type => type.GetMethods())
                .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)));
            return false;
            //these would usually come from a database, or cache.

            //return true;
            // url such as /restaurants/Camberley--Surrey will match
            // url such as /pubs/Camberley--Surrey will not
        }
    }
}