using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Helpers;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            SeedAdminUser();
        }

        private void SeedAdminUser()
        {
            using (var db = new ApplicationDbContext())
            {
                // skip if an admin already exists
                if (db.Users.Any(u => u.Role == UserRole.Admin))
                    return;

                db.Users.Add(new User
                {
                    FullName = "Admin",
                    Email = "admin@library.com",
                    PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                db.SaveChanges();
            }
        }

        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            var authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null) return;

            var ticket = FormsAuthentication.Decrypt(authCookie.Value);
            if (ticket == null || ticket.Expired) return;

            var roles = new[] { ticket.UserData };
            var identity = new FormsIdentity(ticket);
            var principal = new GenericPrincipal(identity, roles);

            HttpContext.Current.User = principal;
            System.Threading.Thread.CurrentPrincipal = principal;
        }
    }
}
