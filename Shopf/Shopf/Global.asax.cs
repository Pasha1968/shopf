using Shopf.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Shopf
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        //25 auth
        protected void Application_AuthentificateRequest()
        {
            if (User == null)
            {
                return;
            }
            string userName = Context.User.Identity.Name;
            string[] roles = null;
            using (DB db = new DB())
            {
                UserDTO dto = db.Users.FirstOrDefault(x => x.UserName == userName);
                if (dto == null)
                    return;
                roles = db.UserRoles.Where(x => x.UserId == dto.Id).Select(x => x.Role.Name).ToArray();
            }
            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity,roles);

            Context.User = newUserObj; 
        }
    }
}
