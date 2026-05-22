using System;
using System.Linq;
using System.Web.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [RoutePrefix("Admin/Transactions")]
    public class AdminTransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("")]
        public ActionResult Index()
        {
            var transactions = db.BorrowTransactions
                .Include("User")
                .Include("Book")
                .Include("Library")
                .OrderByDescending(t => t.BorrowedAt)
                .ToList();

            var reservations = db.Reservations
                .Include("User")
                .Include("Book")
                .OrderByDescending(r => r.ReservedAt)
                .ToList();

            ViewBag.Reservations = reservations;

            return View("~/Views/Admin/Transactions/Index.cshtml", transactions);
        }

        [Route("ConfirmReturn/{id}")]
        [HttpPost]
        public ActionResult ConfirmReturn(int id)
        {
            var transaction = db.BorrowTransactions.Find(id);
            if (transaction == null) return HttpNotFound();
            if (transaction.Status == BorrowStatus.Returned) return RedirectToAction("Index");

            transaction.Status = BorrowStatus.Returned;
            transaction.ReturnedAt = DateTime.UtcNow;

            var book = db.Books.Find(transaction.BookId);
            if (book != null) book.AvailableCopies += 1;

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Route("MarkFinePaid/{id}")]
        [HttpPost]
        public ActionResult MarkFinePaid(int id)
        {
            var transaction = db.BorrowTransactions.Find(id);
            if (transaction == null) return HttpNotFound();

            transaction.FinePaid = transaction.FineAmount;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Route("CancelReservation/{id}")]
        [HttpPost]
        public ActionResult CancelReservation(int id)
        {
            var reservation = db.Reservations.Find(id);
            if (reservation == null) return HttpNotFound();

            reservation.Status = ReservationStatus.Cancelled;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}