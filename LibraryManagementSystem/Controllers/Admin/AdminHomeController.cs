using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.Admin;

namespace LibraryManagementSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/Home")]
    public class AdminHomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("")]
        public ActionResult Index()
        {
            // Stat cards from real database
            var totalBooks = db.Books.Count();
            var borrowedBooks = db.BorrowTransactions
                .Count(t => t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed);
            var overdueBooks = db.BorrowTransactions
                .Count(t => t.Status == BorrowStatus.Overdue);
            var totalMembers = db.Users
                .Count(u => u.Role == UserRole.Member);
            var pendingReservations = db.Reservations
                .Count(r => r.Status == ReservationStatus.Pending);
            var totalFeedback = db.Feedbacks.Count();

            // Recent activity from last 10 borrow transactions
            var recentTransactions = db.BorrowTransactions
                .Include("User")
                .Include("Book")
                .OrderByDescending(t => t.BorrowedAt)
                .Take(10)
                .ToList();

            var recentActivity = new List<RecentActivityItem>();

            foreach (var t in recentTransactions)
            {
                if (t.Status == BorrowStatus.Returned)
                {
                    recentActivity.Add(new RecentActivityItem
                    {
                        Activity = "Book Returned",
                        UserName = t.User.FullName,
                        Details = "Returned \"" + t.Book.Title + "\"",
                        Time = t.ReturnedAt ?? t.BorrowedAt
                    });
                }
                else if (t.Status == BorrowStatus.Overdue)
                {
                    recentActivity.Add(new RecentActivityItem
                    {
                        Activity = "Overdue Notice",
                        UserName = t.User.FullName,
                        Details = "\"" + t.Book.Title + "\" is overdue",
                        Time = t.DueDate
                    });
                }
                else
                {
                    recentActivity.Add(new RecentActivityItem
                    {
                        Activity = "Book Borrowed",
                        UserName = t.User.FullName,
                        Details = "Borrowed \"" + t.Book.Title + "\"",
                        Time = t.BorrowedAt
                    });
                }
            }

            var model = new AdminHomeViewModel
            {
                WelcomeMessage = "Welcome, Admin",
                TotalBooks = totalBooks,
                BorrowedBooks = borrowedBooks,
                OverdueBooks = overdueBooks,
                TotalMembers = totalMembers,
                PendingReservations = pendingReservations,
                TotalFeedback = totalFeedback,
                RecentActivity = recentActivity
            };

            return View("~/Views/Admin/Home/Index.cshtml", model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
