using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.ViewModels.User;

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
            var trending = trendingList.Select(b => new UserHomeViewModel.TrendingBookItem
            {
                Title = b.Title,
                Authors = string.Join(", ", b.BookAuthors.Select(ba => ba.Author.Name)),
                IsAvailable = b.AvailableCopies > 0
            }).ToList();

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
        public ActionResult Books()
        {
            return View("~/Views/User/Books.cshtml");
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
