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
                if (userOverdueBookIds.Contains(b.Id))
                    status = "Overdue";
                else if (userActiveBookIds.Contains(b.Id))
                    status = "Borrowed";
                else if (b.AvailableCopies > 0)
                    status = "Available";
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

            var settings = await db.BorrowSettings.FirstOrDefaultAsync(bs => bs.Status == "Active");
            var activeCount = await db.BorrowTransactions.CountAsync(
                t => t.UserId == user.Id &&
                (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed));

            var model = new UserBooksViewModel
            {
                Books = books,
                Genres = genres,
                SearchQuery = search,
                SelectedGenre = genre,
                AtBorrowLimit = settings != null && activeCount >= settings.MaxBorrowableItems,
                LoanDurationDays = settings?.LoanDurationDays ?? 14
            };

            return View("~/Views/User/Books.cshtml", model);
        }

        [Route("BookDetails")]
        public async Task<ActionResult> BookDetails(int id)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var book = await db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .Include(b => b.BookGenres.Select(bg => bg.Genre))
                .Include(b => b.Feedbacks.Select(f => f.User))
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            var userHasActive = await db.BorrowTransactions.AnyAsync(
                t => t.UserId == user.Id && t.BookId == id &&
                (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed));

            var userHasOverdue = await db.BorrowTransactions.AnyAsync(
                t => t.UserId == user.Id && t.BookId == id && t.Status == BorrowStatus.Overdue);

            string status;
            if (userHasOverdue)
                status = "Overdue";
            else if (userHasActive)
                status = "Borrowed";
            else if (book.AvailableCopies > 0)
                status = "Available";
            else
                status = "Unavailable";

            var settings = await db.BorrowSettings.FirstOrDefaultAsync(bs => bs.Status == "Active");
            var activeCount = await db.BorrowTransactions.CountAsync(
                t => t.UserId == user.Id &&
                (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed));

            var model = new UserBookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Authors = string.Join(", ", book.BookAuthors.Select(ba => ba.Author.Name)),
                Isbn = book.Isbn,
                Genre = string.Join(", ", book.BookGenres.Select(bg => bg.Genre.Name)),
                Summary = book.Summary,
                CoverImageUrl = book.CoverImageUrl,
                Status = status,
                AtBorrowLimit = settings != null && activeCount >= settings.MaxBorrowableItems,
                LoanDurationDays = settings?.LoanDurationDays ?? 14,
                Reviews = book.Feedbacks
                    .OrderByDescending(f => f.CreatedAt)
                    .Take(3)
                    .Select(f => new UserBookDetailsViewModel.ReviewItem
                    {
                        ReviewerName = f.User.FullName,
                        Rating = f.Rating,
                        Comment = f.Comment
                    })
                    .ToList()
            };

            return View("~/Views/User/BookDetails.cshtml", model);
        }

        [Route("BorrowBook")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BorrowBook(int bookId, string returnUrl)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return Redirect(returnUrl ?? "/User/Books");
            }

            if (book.AvailableCopies <= 0)
            {
                TempData["Error"] = "This book is no longer available.";
                return Redirect(returnUrl ?? "/User/Books");
            }

            var settings = await db.BorrowSettings.FirstOrDefaultAsync(bs => bs.Status == "Active");
            var activeCount = await db.BorrowTransactions.CountAsync(
                t => t.UserId == user.Id &&
                (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed));

            if (settings != null && activeCount >= settings.MaxBorrowableItems)
            {
                TempData["Error"] = $"You have reached your borrowing limit of {settings.MaxBorrowableItems} books.";
                return Redirect(returnUrl ?? "/User/Books");
            }

            var loanDays = settings?.LoanDurationDays ?? 14;
            var dueDate = System.DateTime.UtcNow.AddDays(loanDays);

            db.BorrowTransactions.Add(new BorrowTransaction
            {
                UserId = user.Id,
                BookId = bookId,
                LibraryId = book.LibraryId,
                Status = BorrowStatus.Borrowed,
                BorrowedAt = System.DateTime.UtcNow,
                DueDate = dueDate
            });

            book.AvailableCopies--;
            book.BorrowedTimes++;

            await db.SaveChangesAsync();

            TempData["Success"] = $"You have borrowed \"{book.Title}\". Due date: {dueDate:d MMM yyyy}.";
            return Redirect(returnUrl ?? "/User/Books");
        }

        [Route("ReserveBook")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ReserveBook(int bookId, string returnUrl)
        {
            var email = User.Identity.Name;
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);

            var book = await db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
            {
                TempData["Error"] = "Book not found.";
                return Redirect(returnUrl ?? "/User/Books");
            }

            var alreadyReserved = await db.Reservations.AnyAsync(
                r => r.UserId == user.Id && r.BookId == bookId && r.Status == ReservationStatus.Pending);
            if (alreadyReserved)
            {
                TempData["Error"] = "You already have a pending reservation for this book.";
                return Redirect(returnUrl ?? "/User/Books");
            }

            db.Reservations.Add(new Reservation
            {
                UserId = user.Id,
                BookId = bookId,
                Status = ReservationStatus.Pending,
                ReservedAt = System.DateTime.UtcNow,
                ExpiresAt = System.DateTime.UtcNow.AddDays(7)
            });

            await db.SaveChangesAsync();

            TempData["Success"] = $"You have reserved \"{book.Title}\". Your reservation expires in 7 days.";
            return Redirect(returnUrl ?? "/User/Books");
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

            var txSettings = await db.BorrowSettings.FirstOrDefaultAsync(bs => bs.Status == "Active");
            var loanDays = txSettings?.LoanDurationDays ?? 14;
            var renewalLimit = txSettings?.RenewalLimit ?? 3;

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
                    ReturnLocation = t.Library != null ? t.Library.Name : "Sydney Library",
                    LoanDurationDays = loanDays,
                    NewDueDateAfterExtension = t.DueDate.AddDays(loanDays).ToString("dd MMM yyyy"),
                    RenewalsRemaining = Math.Max(0, renewalLimit - t.RenewalsUsed)
                });
            }

            var finePerDay = txSettings?.OverdueFinePerDay ?? 0;
            bool finesChanged = false;
            foreach (var t in overdueItems)
            {
                var daysOverdue = (int)(DateTime.Today - t.DueDate.Date).TotalDays;
                var outstandingFine = Math.Max(0, finePerDay * daysOverdue - t.FinePaid);
                if (t.FineAmount != outstandingFine)
                {
                    t.FineAmount = outstandingFine;
                    finesChanged = true;
                }
            }
            if (finesChanged)
                await db.SaveChangesAsync();

            var overdue = new List<UserTransactionsViewModel.OverdueItem>();
            foreach (var t in overdueItems)
            {
                overdue.Add(new UserTransactionsViewModel.OverdueItem
                {
                    TransactionId = t.Id,
                    BookTitle = t.Book.Title,
                    DueDate = t.DueDate,
                    DaysOverdue = (int)(DateTime.Today - t.DueDate.Date).TotalDays,
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
        [ValidateAntiForgeryToken]
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
                return HttpNotFound();

            var settings = await db.BorrowSettings.FirstOrDefaultAsync(bs => bs.Status == "Active");
            var renewalLimit = settings?.RenewalLimit ?? 3;
            var loanDays = settings?.LoanDurationDays ?? 7;

            if (transaction.RenewalsUsed >= renewalLimit)
            {
                TempData["Error"] = "You have reached the maximum number of renewals for this book.";
                return RedirectToAction("Transactions");
            }

            transaction.DueDate = transaction.DueDate.AddDays(loanDays);
            transaction.Status = BorrowStatus.Renewed;
            transaction.RenewalsUsed++;

            await db.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }

        [Route("PayFine")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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

            transaction.FinePaid += transaction.FineAmount;
            transaction.FineAmount = 0;

            await db.SaveChangesAsync();

            return RedirectToAction("Transactions");
        }

        [Route("SubmitFeedback")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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