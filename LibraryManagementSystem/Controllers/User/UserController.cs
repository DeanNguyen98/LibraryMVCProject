using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.User;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace LibraryManagementSystem.Controllers.UserArea
{
    [Authorize(Roles = "Member")]
    [RoutePrefix("User")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [Route("Home")]
        public async Task<ActionResult> Index()
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var borrowedCount = await db.BorrowTransactions.CountAsync(t => t.UserId == user.Id
                && (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed)
            );
            var availableCount = await db.Books.CountAsync(b => b.AvailableCopies > 0);

            var overdueCount = await db.BorrowTransactions.CountAsync(t => t.UserId == user.Id
                && t.Status == BorrowStatus.Overdue);

            var trendingList = await db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .OrderByDescending(b => b.BorrowedTimes)
                .Take(3)
                .ToListAsync();

            var trending = new List<UserHomeViewModel.TrendingBookItem>();
            foreach (var b in trendingList)
            {
                trending.Add(new UserHomeViewModel.TrendingBookItem
                {
                    Title = b.Title,
                    Authors = string.Join(", ", b.BookAuthors.Select(ba => ba.Author.Name)),
                    IsAvailable = b.AvailableCopies > 0
                });
            }

            var newArrivalsList = await db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .OrderByDescending(b => b.CreatedAt)
                .Take(4)
                .ToListAsync();

            var newArrivals = new List<UserHomeViewModel.NewArrivalItem>();
            foreach (var b in newArrivalsList)
            {
                newArrivals.Add(new UserHomeViewModel.NewArrivalItem
                {
                    Title = b.Title,
                    Authors = string.Join(", ", b.BookAuthors.Select(ba => ba.Author.Name)),
                    IsAvailable = b.AvailableCopies > 0
                });
            }

            var overdueNotices = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id && t.Status == BorrowStatus.Overdue)
                .Select(t => new UserHomeViewModel.OverdueNoticeItem
                {
                    BookTitle = t.Book.Title,
                    DueDate = t.DueDate,
                    FineAmount = t.FineAmount
                })
                .ToListAsync();

            var model = new UserHomeViewModel
            {
                UserFullName = user.FullName,
                CurrentlyBorrowedCount = borrowedCount,
                AvailableBooksCount = availableCount,
                OverdueBooksCount = overdueCount,
                TrendingBooks = trending,
                NewArrivals = newArrivals,
                OverdueNotices = overdueNotices
            };

            return View("~/Views/User/Index.cshtml", model);
        }

        [Route("Books")]
        public async Task<ActionResult> Books(string search = null, string genre = null)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var userActiveBookIds = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id
                    && (t.Status == BorrowStatus.Borrowed
                        || t.Status == BorrowStatus.Renewed))
                .Select(t => t.BookId)
                .ToListAsync();

            var userOverdueBookIds = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id && t.Status == BorrowStatus.Overdue)
                .Select(t => t.BookId)
                .ToListAsync();

            var query = db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .Include(b => b.BookGenres.Select(bg => bg.Genre))
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(b => b.Title.Contains(search) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.Contains(search)));

            if (!string.IsNullOrWhiteSpace(genre))
                query = query.Where(b => b.BookGenres.Any(bg => bg.Genre.Name == genre));

            var booksRaw = await query.ToListAsync();

            var books = new List<UserBooksViewModel.BookListItem>();
            foreach (var b in booksRaw)
            {
                string status;
                if (b.AvailableCopies > 0)
                    status = "Available";
                else if (userOverdueBookIds.Contains(b.Id))
                    status = "Overdue";
                else if (userActiveBookIds.Contains(b.Id))
                    status = "Borrowed";
                else
                    status = "Unavailable";

                books.Add(new UserBooksViewModel.BookListItem
                {
                    Id = b.Id,
                    Title = b.Title,
                    Authors = string.Join(", ", b.BookAuthors.Select(ba => ba.Author.Name)),
                    Genre = string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name)),
                    Status = status
                });
            }

            var genres = await db.Genres.Select(g => g.Name).OrderBy(n => n).ToListAsync();

            var model = new UserBooksViewModel
            {
                Books = books,
                Genres = genres,
                SearchQuery = search,
                SelectedGenre = genre
            };

            return View("~/Views/User/Books.cshtml", model);
        }

        [Route("BookDetails")]
        public ActionResult BookDetails()
        {
            return View("~/Views/User/BookDetails.cshtml");
        }

        [Route("Transactions")]
        public async Task<ActionResult> Transactions()
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var activeBorrows = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id
                    && (t.Status == BorrowStatus.Borrowed
                        || t.Status == BorrowStatus.Renewed))
                .Include(t => t.Book)
                .Include(t => t.Library)
                .ToListAsync();

            var overdueItems = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id
                    && t.Status == BorrowStatus.Overdue)
                .Include(t => t.Book)
                .ToListAsync();

            var historyItems = await db.BorrowTransactions
                .Where(t => t.UserId == user.Id
                    && t.Status == BorrowStatus.Returned)
                .Include(t => t.Book)
                .ToListAsync();

            var userFeedbacks = await db.Feedbacks
                .Where(f => f.UserId == user.Id)
                .ToListAsync();

            var active = new List<UserTransactionsViewModel.ActiveBorrowItem>();
            foreach (var t in activeBorrows)
            {
                active.Add(new UserTransactionsViewModel.ActiveBorrowItem
                {
                    TransactionId = t.Id,
                    BookTitle = t.Book.Title,
                    BookId = t.BookId,
                    BorrowedAt = t.BorrowedAt,
                    DueDate = t.DueDate,
                    ReturnLocation = t.Library != null ? t.Library.Name : "Sydney Library"
                });
            }

            var overdue = new List<UserTransactionsViewModel.OverdueItem>();
            foreach (var t in overdueItems)
            {
                overdue.Add(new UserTransactionsViewModel.OverdueItem
                {
                    TransactionId = t.Id,
                    BookTitle = t.Book.Title,
                    DueDate = t.DueDate,
                    DaysOverdue = (int)(DateTime.Today - t.DueDate).TotalDays,
                    FineAmount = t.FineAmount
                });
            }

            var history = new List<UserTransactionsViewModel.HistoryItem>();
            foreach (var t in historyItems)
            {
                var feedback = userFeedbacks
                    .FirstOrDefault(f => f.BookId == t.BookId);

                history.Add(new UserTransactionsViewModel.HistoryItem
                {
                    BookId = t.BookId,
                    BookTitle = t.Book.Title,
                    ReturnedOnTime = t.ReturnedAt.HasValue
                        && t.ReturnedAt.Value <= t.DueDate,
                    ReturnedAt = t.ReturnedAt ?? t.DueDate,
                    FinePaid = t.FinePaid,
                    HasFeedback = feedback != null,
                    FeedbackRating = feedback != null ? feedback.Rating : 0,
                    FeedbackComment = feedback != null ? feedback.Comment : ""
                });
            }

            var model = new UserTransactionsViewModel
            {
                ActiveBorrows = active,
                OverdueItems = overdue,
                HistoryItems = history
            };

            return View("~/Views/User/Transactions.cshtml", model);
        }

        [Route("ExtendBorrow")]
        [HttpPost]
        public async Task<ActionResult> ExtendBorrow(int transactionId)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var transaction = await db.BorrowTransactions
                .FirstOrDefaultAsync(t => t.Id == transactionId
                    && t.UserId == user.Id
                    && (t.Status == BorrowStatus.Borrowed
                        || t.Status == BorrowStatus.Renewed));

            if (transaction == null)
            {
                return HttpNotFound();
            }

            transaction.DueDate = transaction.DueDate.AddDays(7);
            transaction.Status = BorrowStatus.Renewed;

            await db.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }

        [Route("PayFine")]
        [HttpPost]
        public async Task<ActionResult> PayFine(int transactionId)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var transaction = await db.BorrowTransactions
                .FirstOrDefaultAsync(t => t.Id == transactionId
                    && t.UserId == user.Id
                    && t.Status == BorrowStatus.Overdue);

            if (transaction == null)
            {
                return HttpNotFound();
            }

            transaction.FinePaid = transaction.FineAmount;
            transaction.FineAmount = 0;
            transaction.Status = BorrowStatus.Borrowed;

            await db.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }

        [Route("SubmitFeedback")]
        [HttpPost]
        public async Task<ActionResult> SubmitFeedback(int bookId, int rating, string comment)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var existing = await db.Feedbacks
                .FirstOrDefaultAsync(f => f.UserId == user.Id
                    && f.BookId == bookId);

            if (existing != null)
            {
                existing.Rating = rating;
                existing.Comment = comment;
                existing.CreatedAt = DateTime.UtcNow;
            }
            else
            {
                var feedback = new Feedback
                {
                    UserId = user.Id,
                    BookId = bookId,
                    Rating = rating,
                    Comment = comment,
                    CreatedAt = DateTime.UtcNow
                };
                db.Feedbacks.Add(feedback);
            }

            await db.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }
    }
}