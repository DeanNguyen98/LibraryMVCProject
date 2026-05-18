using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.Admin;

namespace LibraryManagementSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/BorrowSettings")]
    public class AdminBorrowSettingsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Admin/BorrowSettings
        [Route("")]
        public async Task<ActionResult> Index()
        {
            var settings = await db.BorrowSettings
                .OrderByDescending(b => b.Status == "Active")
                .ThenBy(b => b.SettingName)
                .ToListAsync();

            var vm = new ManageBorrowSettingsViewModel
            {
                Settings = settings.Select(s => new BorrowSettingRowViewModel
                {
                    Id = s.Id,
                    SettingName = s.SettingName,
                    LoanDurationDays = s.LoanDurationDays,
                    RenewalLimit = s.RenewalLimit,
                    OverdueFinePerDay = s.OverdueFinePerDay,
                    MaxBorrowableItems = s.MaxBorrowableItems,
                    Status = s.Status,
                    UpdatedAt = s.UpdatedAt
                }).ToList()
            };

            return View("~/Views/Admin/BorrowSettings/Index.cshtml", vm);
        }

        // GET: /Admin/BorrowSettings/Details/5
        [Route("Details/{id:int}")]
        public async Task<ActionResult> Details(int id)
        {
            var setting = await db.BorrowSettings.FindAsync(id);

            if (setting == null)
                return HttpNotFound();

            var vm = new BorrowSettingDetailsViewModel
            {
                Id = setting.Id,
                SettingName = setting.SettingName,
                LoanDurationDays = setting.LoanDurationDays,
                RenewalLimit = setting.RenewalLimit,
                OverdueFinePerDay = setting.OverdueFinePerDay,
                MaxBorrowableItems = setting.MaxBorrowableItems,
                Status = setting.Status,
                UpdatedAt = setting.UpdatedAt
            };

            return View("~/Views/Admin/BorrowSettings/Details.cshtml", vm);
        }

        // GET: /Admin/BorrowSettings/Create
        [Route("Create")]
        [HttpGet]
        public ActionResult Create()
        {
            var vm = new BorrowSettingCreateViewModel
            {
                LoanDurationDays = 14,
                RenewalLimit = 2,
                OverdueFinePerDay = 0.10m,
                MaxBorrowableItems = 5,
                Status = "Inactive"
            };

            return View("~/Views/Admin/BorrowSettings/Create.cshtml", vm);
        }

        // POST: /Admin/BorrowSettings/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BorrowSettingCreateViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/BorrowSettings/Create.cshtml", vm);

            // If setting this as Active, deactivate all others
            if (vm.Status == "Active")
            {
                var allSettings = await db.BorrowSettings.ToListAsync();
                foreach (var setting in allSettings)
                {
                    setting.Status = "Inactive";
                }
            }

            var borrowSetting = new BorrowSettings
            {
                SettingName = vm.SettingName,
                LoanDurationDays = vm.LoanDurationDays,
                RenewalLimit = vm.RenewalLimit,
                OverdueFinePerDay = vm.OverdueFinePerDay,
                MaxBorrowableItems = vm.MaxBorrowableItems,
                Status = vm.Status,
                UpdatedAt = DateTime.UtcNow
            };

            db.BorrowSettings.Add(borrowSetting);
            await db.SaveChangesAsync();

            TempData["Success"] = "Borrow setting created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/BorrowSettings/Edit/5
        [Route("Edit/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var setting = await db.BorrowSettings.FindAsync(id);

            if (setting == null)
                return HttpNotFound();

            var vm = new BorrowSettingEditViewModel
            {
                Id = setting.Id,
                SettingName = setting.SettingName,
                LoanDurationDays = setting.LoanDurationDays,
                RenewalLimit = setting.RenewalLimit,
                OverdueFinePerDay = setting.OverdueFinePerDay,
                MaxBorrowableItems = setting.MaxBorrowableItems,
                Status = setting.Status
            };

            return View("~/Views/Admin/BorrowSettings/Edit.cshtml", vm);
        }

        // POST: /Admin/BorrowSettings/Edit/5
        [Route("Edit/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, BorrowSettingEditViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Admin/BorrowSettings/Edit.cshtml", vm);

            var setting = await db.BorrowSettings.FindAsync(id);

            if (setting == null)
                return HttpNotFound();

            // Block deactivating the only active setting without replacing it
            if (setting.Status == "Active" && vm.Status != "Active")
            {
                TempData["Error"] = "Cannot deactivate this setting — it is the only active setting. Activate another setting first.";
                return RedirectToAction("Edit", new { id });
            }

            // If setting this as Active, deactivate all others
            if (vm.Status == "Active")
            {
                var otherSettings = await db.BorrowSettings
                    .Where(b => b.Id != id)
                    .ToListAsync();

                foreach (var other in otherSettings)
                {
                    other.Status = "Inactive";
                }
            }

            setting.SettingName = vm.SettingName;
            setting.LoanDurationDays = vm.LoanDurationDays;
            setting.RenewalLimit = vm.RenewalLimit;
            setting.OverdueFinePerDay = vm.OverdueFinePerDay;
            setting.MaxBorrowableItems = vm.MaxBorrowableItems;
            setting.Status = vm.Status;
            setting.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            TempData["Success"] = "Borrow setting updated successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/BorrowSettings/Delete/5
        [Route("Delete/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var setting = await db.BorrowSettings.FindAsync(id);

            if (setting == null)
                return HttpNotFound();

            var vm = new BorrowSettingDetailsViewModel
            {
                Id = setting.Id,
                SettingName = setting.SettingName,
                LoanDurationDays = setting.LoanDurationDays,
                RenewalLimit = setting.RenewalLimit,
                OverdueFinePerDay = setting.OverdueFinePerDay,
                MaxBorrowableItems = setting.MaxBorrowableItems,
                Status = setting.Status,
                UpdatedAt = setting.UpdatedAt
            };

            return View("~/Views/Admin/BorrowSettings/Delete.cshtml", vm);
        }

        // POST: /Admin/BorrowSettings/Delete/5
        [Route("Delete/{id:int}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var setting = await db.BorrowSettings.FindAsync(id);

            if (setting == null)
                return HttpNotFound();

            // Prevent deleting the active setting
            if (setting.Status == "Active")
            {
                TempData["Error"] = "Cannot delete the active borrow setting. Please activate another setting first.";
                return RedirectToAction("Index");
            }

            db.BorrowSettings.Remove(setting);
            await db.SaveChangesAsync();

            TempData["Success"] = "Borrow setting deleted successfully.";
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