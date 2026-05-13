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

            // Same thing with trending books
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
                OverdueNotices = overdueNotices
            };

            return View("~/Views/User/Index.cshtml", model);
        }

        [Route("Books")]
        public async Task<ActionResult> Books(string search = null, string genre = null)
        {
            var query = db.Books
                .Include(b => b.BookAuthors.Select(ba => ba.Author))
                .Include(b => b.BookGenres.Select(bg => bg.Genre))
                .Include(b => b.BorrowTransactions)
                .Include(b => b.Reservations)
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
                else if (b.BorrowTransactions.Any(t => t.Status == BorrowStatus.Overdue))
                    status = "Overdue";
                else if (b.Reservations.Any(r => r.Status == ReservationStatus.Pending))
                    status = "Reserved";
                else
                    status = "Borrowed";

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
        public ActionResult Transactions()
        {
            return View("~/Views/User/Transactions.cshtml");
        }
    }
}
