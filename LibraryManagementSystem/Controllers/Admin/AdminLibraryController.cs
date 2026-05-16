using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace LibraryManagementSystem.Controllers.Admin
{

    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/Library")]
    public class AdminLibraryController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Admin/Library
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var libraries = await db.Libraries
                .Include(l => l.Books)
                .OrderBy(l => l.Name)
                .ToListAsync();

            var vm = new ManageLibraryViewModel
            {
                Libraries = libraries.Select(l => new LibraryRowViewModel
                {
                    Id = l.Id,
                    Name = l.Name,
                    Location = l.Location,
                    OperatingHours = l.OperatingHours,
                    ContactEmail = l.ContactEmail,
                    ContactPhone = l.ContactPhone,
                    TotalBooks = l.Books != null ? l.Books.Count : 0,
                    CreatedAt = l.CreatedAt
                }).ToList()
            };

            return View("~/Views/Admin/Library/Index.cshtml", vm);
        }

        // GET: /Admin/Library/Details/5
        [Route("Details/{id:int}")]
        public async Task<ActionResult> Details(int id)
        {
            var library = await db.Libraries
                .Include(l => l.Books)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (library == null)
                return HttpNotFound();

            var vm = new LibraryDetailsViewModel
            {
                Id = library.Id,
                Name = library.Name,
                Location = library.Location,
                OperatingHours = library.OperatingHours,
                ContactEmail = library.ContactEmail,
                ContactPhone = library.ContactPhone,
                Description = library.Description,
                TotalBooks = library.Books != null ? library.Books.Count : 0,
                CreatedAt = library.CreatedAt
            };

            return View("~/Views/Admin/Library/Details.cshtml", vm);
        }

        // GET: /Admin/Library/Create
        [Route("Create")]
        [HttpGet]
        public ActionResult Create()
        {
            var vm = new LibraryCreateViewModel();
            return View("~/Views/Admin/Library/Create.cshtml", vm);
        }

        // POST: /Admin/Library/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(LibraryCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Library/Create.cshtml", vm);

            var library = new Library
            {
                Name = vm.Name,
                Location = vm.Location,
                OperatingHours = vm.OperatingHours,
                ContactEmail = vm.ContactEmail,
                ContactPhone = vm.ContactPhone,
                Description = vm.Description,
                CreatedAt = DateTime.UtcNow
            };

            db.Libraries.Add(library);
            await db.SaveChangesAsync();

            TempData["Success"] = "Library created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Library/Edit/5
        [Route("Edit/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var library = await db.Libraries.FindAsync(id);

            if (library == null)
                return HttpNotFound();

            var vm = new LibraryEditViewModel
            {
                Id = library.Id,
                Name = library.Name,
                Location = library.Location,
                OperatingHours = library.OperatingHours,
                ContactEmail = library.ContactEmail,
                ContactPhone = library.ContactPhone,
                Description = library.Description
            };

            return View("~/Views/Admin/Library/Edit.cshtml", vm);
        }

        // POST: /Admin/Library/Edit/5
        [Route("Edit/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, LibraryEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/Library/Edit.cshtml", vm);

            var library = await db.Libraries.FindAsync(id);

            if (library == null)
                return HttpNotFound();

            library.Name = vm.Name;
            library.Location = vm.Location;
            library.OperatingHours = vm.OperatingHours;
            library.ContactEmail = vm.ContactEmail;
            library.ContactPhone = vm.ContactPhone;
            library.Description = vm.Description;

            await db.SaveChangesAsync();

            TempData["Success"] = "Library updated successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Library/Delete/5
        [Route("Delete/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var library = await db.Libraries
                .Include(l => l.Books)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (library == null)
                return HttpNotFound();

            var vm = new LibraryDetailsViewModel
            {
                Id = library.Id,
                Name = library.Name,
                Location = library.Location,
                TotalBooks = library.Books != null ? library.Books.Count : 0,
                CreatedAt = library.CreatedAt
            };

            return View("~/Views/Admin/Library/Delete.cshtml", vm);
        }

        // POST: /Admin/Library/Delete/5
        [Route("Delete/{id:int}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var library = await db.Libraries.FindAsync(id);

            if (library == null)
                return HttpNotFound();

            db.Libraries.Remove(library);
            await db.SaveChangesAsync();

            TempData["Success"] = "Library deleted successfully.";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }


    }
}