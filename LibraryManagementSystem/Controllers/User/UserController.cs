using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.User;
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
            var user = await db.Users.FirstOrDefaultAsync(u =>  u.Email == email);
            
            var borrowedCount = await db.BorrowTransactions.CountAsync( t => t.UserId == user.Id
                && (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed)
            );
            var availableCount = await db.Books.CountAsync(b => b.AvailableCopies > 0);

            var overdueCount = await db.BorrowTransactions.CountAsync(t => t.UserId == user.Id && t.Status == BorrowStatus.Overdue);

            //Get 3 top books based on borrowed times
            var trendingList = await db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .OrderByDescending(b => b.BorrowedTimes)
                .Take(3)
                .ToListAsync();

            // Create an object to store data for each book. Need to check if toList() is good or need to be ToListAsync()?
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

            //compile ViewModel class
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
                .Where(t => t.UserId == user.Id && (t.Status == BorrowStatus.Borrowed || t.Status == BorrowStatus.Renewed))
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
        public ActionResult Transactions()
        {
            return View("~/Views/User/Transactions.cshtml");
        }
    }
}
