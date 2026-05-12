using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.Auth;

namespace LibraryManagementSystem.Controllers
{
    [RoutePrefix("Auth")]
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [Route("~/")]
        [HttpGet]
        public ActionResult Root()
        {
            if (!Request.IsAuthenticated)
                return Redirect("/Auth/SignIn");

            if (User.IsInRole("Admin"))
                return Redirect("/Admin/Home");

            return Redirect("/User/Home");
        }

        [Route("SignIn")]
        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [Route("SignIn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignIn(MemberSignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["SignInError"] = "Please fill in all fields.";
                return RedirectToAction("SignIn");
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Role == UserRole.Member);

            if (user == null || user.PasswordHash != model.Password)
            {
                TempData["SignInError"] = "Invalid email or password.";
                return RedirectToAction("SignIn");
            }

            SetAuthCookie(user);
            return Redirect("/User/Home");
        }

        [Route("SignUp")]
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        [Route("SignUp")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SignUp(MemberSignUpViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["SignUpError"] = "Please fill in all required fields.";
                return RedirectToAction("SignUp");
            }

            var emailExists = await db.Users.AnyAsync(u => u.Email == model.Email);
            if (emailExists)
            {
                TempData["SignUpError"] = "An account with this email already exists.";
                return RedirectToAction("SignUp");
            }

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = model.Password,
                Phone = model.Phone,
                Address = model.Address,
                Role = UserRole.Member
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            SetAuthCookie(user);
            return Redirect("/Admin/Home");
        }

        [Route("AdminSignIn")]
        [HttpGet]
        public ActionResult AdminSignIn()
        {
            return View();
        }

        [Route("AdminSignIn")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AdminSignIn(AdminSignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["AdminSignInError"] = "Please fill in all fields.";
                return RedirectToAction("AdminSignIn");
            }

            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Role == UserRole.Admin);

            if (user == null || user.PasswordHash != model.Password)
            {
                TempData["AdminSignInError"] = "Invalid email or password.";
                return RedirectToAction("AdminSignIn");
            }

            SetAuthCookie(user);
            return Redirect("/Admin/Home");
        }

        [Route("SignOut")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignOut()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("SignIn");
        }

        private void SetAuthCookie(User user)
        {
            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: user.Email,
                issueDate: DateTime.Now,
                expiration: DateTime.Now.AddMinutes(43200),
                isPersistent: false,
                userData: user.Role.ToString()
            );

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
            Response.Cookies.Add(cookie);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
