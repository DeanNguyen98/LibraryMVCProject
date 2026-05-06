using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LibraryManagementSystem.ViewModels.Admin;

namespace LibraryManagementSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/Home")]
    public class AdminHomeController : Controller
    {
        [Route("")]
        public ActionResult Index()
        {
            var model = new AdminHomeViewModel
            {
                WelcomeMessage = "Welcome to the Admin Dashboard"
            };

            return View("~/Views/Admin/Home/Index.cshtml", model);
        }
    }
}
