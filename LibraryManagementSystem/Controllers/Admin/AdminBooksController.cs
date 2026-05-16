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
    [RoutePrefix("Admin/Books")]
    public class AdminBooksController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: /Admin/Books
        [Route("")]
        public async Task<ActionResult> Index(string searchTerm = null)
        {
            var query = db.Books
                .Include(b => b.Library)
                .Include("BookAuthors.Author")
                .Include("BookGenres.Genre")
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
                query = query.Where(b => b.Title.Contains(searchTerm)
                                      || b.Isbn.Contains(searchTerm));

            var books = await query.OrderBy(b => b.Title).ToListAsync();

            var vm = new ManageBooksViewModel
            {
                SearchTerm = searchTerm,
                Books = books.Select(b => new BookRowViewModel
                {
                    Id = b.Id,
                    Title = b.Title,
                    Isbn = b.Isbn,
                    LibraryName = b.Library != null ? b.Library.Name : "—",
                    Authors = b.BookAuthors != null && b.BookAuthors.Any()
                                          ? string.Join(", ", b.BookAuthors.Select(ba => ba.Author.Name))
                                          : "—",
                    Genres = b.BookGenres != null && b.BookGenres.Any()
                                          ? string.Join(", ", b.BookGenres.Select(bg => bg.Genre.Name))
                                          : "—",
                    TotalCopies = b.TotalCopies,
                    AvailableCopies = b.AvailableCopies
                }).ToList()
            };

            return View("~/Views/Admin/Books/Index.cshtml", vm);
        }

        // GET: /Admin/Books/Details/5
        [Route("Details/{id:int}")]
        public async Task<ActionResult> Details(int id)
        {
            var book = await db.Books
                .Include(b => b.Library)
                .Include("BookAuthors.Author")
                .Include("BookGenres.Genre")
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            var vm = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                LibraryName = book.Library != null ? book.Library.Name : "—",
                LibraryId = book.LibraryId,
                Authors = book.BookAuthors != null && book.BookAuthors.Any()
                                      ? string.Join(", ", book.BookAuthors.Select(ba => ba.Author.Name))
                                      : "—",
                Genres = book.BookGenres != null && book.BookGenres.Any()
                                      ? string.Join(", ", book.BookGenres.Select(bg => bg.Genre.Name))
                                      : "—",
                Isbn = book.Isbn,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                CreatedAt = book.CreatedAt,
                Summary = book.Summary,
                CoverImageUrl = book.CoverImageUrl
            };

            return View("~/Views/Admin/Books/Details.cshtml", vm);
        }

        // GET: /Admin/Books/Create
        [Route("Create")]
        [HttpGet]
        public async Task<ActionResult> Create()
        {
            var libraries = await db.Libraries.OrderBy(l => l.Name).ToListAsync();

            var vm = new BookCreateViewModel
            {
                LibraryOptions = libraries.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList()
            };

            return View("~/Views/Admin/Books/Create.cshtml", vm);
        }

        // POST: /Admin/Books/Create
        [Route("Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BookCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var libraries = await db.Libraries.OrderBy(l => l.Name).ToListAsync();
                vm.LibraryOptions = libraries.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList();

                return View("~/Views/Admin/Books/Create.cshtml", vm);
            }

            var book = new Book
            {
                Title = vm.Title,
                Isbn = vm.Isbn,
                Summary = vm.Summary,
                CoverImageUrl = vm.CoverImageUrl,
                LibraryId = vm.LibraryId,
                TotalCopies = vm.TotalCopies,
                AvailableCopies = vm.AvailableCopies,
                CreatedAt = DateTime.UtcNow
            };

            db.Books.Add(book);
            await db.SaveChangesAsync();

            // Handle authors
            if (!string.IsNullOrWhiteSpace(vm.AuthorNames))
            {
                var authorNames = vm.AuthorNames.Split(',')
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a));

                foreach (var name in authorNames)
                {
                    var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == name)
                                 ?? new Author { Name = name };

                    if (author.Id == 0)
                    {
                        db.Authors.Add(author);
                        await db.SaveChangesAsync();
                    }

                    db.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = author.Id });
                }
            }

            // Handle genres
            if (!string.IsNullOrWhiteSpace(vm.GenreNames))
            {
                var genreNames = vm.GenreNames.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g));

                foreach (var name in genreNames)
                {
                    var genre = await db.Genres.FirstOrDefaultAsync(g => g.Name == name)
                                ?? new Genre { Name = name };

                    if (genre.Id == 0)
                    {
                        db.Genres.Add(genre);
                        await db.SaveChangesAsync();
                    }

                    db.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = genre.Id });
                }
            }

            await db.SaveChangesAsync();

            TempData["Success"] = "Book created successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Books/Edit/5
        [Route("Edit/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var book = await db.Books
                .Include("BookAuthors.Author")
                .Include("BookGenres.Genre")
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            var libraries = await db.Libraries.OrderBy(l => l.Name).ToListAsync();

            var vm = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Isbn = book.Isbn,
                Summary = book.Summary,
                CoverImageUrl = book.CoverImageUrl,
                LibraryId = book.LibraryId,
                TotalCopies = book.TotalCopies,
                AvailableCopies = book.AvailableCopies,
                AuthorNames = book.BookAuthors != null
                                    ? string.Join(", ", book.BookAuthors.Select(ba => ba.Author.Name))
                                    : "",
                GenreNames = book.BookGenres != null
                                    ? string.Join(", ", book.BookGenres.Select(bg => bg.Genre.Name))
                                    : "",
                LibraryOptions = libraries.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList()
            };

            return View("~/Views/Admin/Books/Edit.cshtml", vm);
        }

        // POST: /Admin/Books/Edit/5
        [Route("Edit/{id:int}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, BookEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var libraries = await db.Libraries.OrderBy(l => l.Name).ToListAsync();
                vm.LibraryOptions = libraries.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList();

                return View("~/Views/Admin/Books/Edit.cshtml", vm);
            }

            var book = await db.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            book.Title = vm.Title;
            book.Isbn = vm.Isbn;
            book.Summary = vm.Summary;
            book.CoverImageUrl = vm.CoverImageUrl;
            book.LibraryId = vm.LibraryId;
            book.TotalCopies = vm.TotalCopies;
            book.AvailableCopies = vm.AvailableCopies;

            // Clear and re-add authors
            db.BookAuthors.RemoveRange(book.BookAuthors);
            if (!string.IsNullOrWhiteSpace(vm.AuthorNames))
            {
                var authorNames = vm.AuthorNames.Split(',')
                    .Select(a => a.Trim())
                    .Where(a => !string.IsNullOrEmpty(a));

                foreach (var name in authorNames)
                {
                    var author = await db.Authors.FirstOrDefaultAsync(a => a.Name == name)
                                 ?? new Author { Name = name };

                    if (author.Id == 0)
                    {
                        db.Authors.Add(author);
                        await db.SaveChangesAsync();
                    }

                    db.BookAuthors.Add(new BookAuthor { BookId = book.Id, AuthorId = author.Id });
                }
            }

            // Clear and re-add genres
            db.BookGenres.RemoveRange(book.BookGenres);
            if (!string.IsNullOrWhiteSpace(vm.GenreNames))
            {
                var genreNames = vm.GenreNames.Split(',')
                    .Select(g => g.Trim())
                    .Where(g => !string.IsNullOrEmpty(g));

                foreach (var name in genreNames)
                {
                    var genre = await db.Genres.FirstOrDefaultAsync(g => g.Name == name)
                                ?? new Genre { Name = name };

                    if (genre.Id == 0)
                    {
                        db.Genres.Add(genre);
                        await db.SaveChangesAsync();
                    }

                    db.BookGenres.Add(new BookGenre { BookId = book.Id, GenreId = genre.Id });
                }
            }

            await db.SaveChangesAsync();

            TempData["Success"] = "Book updated successfully.";
            return RedirectToAction("Index");
        }

        // GET: /Admin/Books/Delete/5
        [Route("Delete/{id:int}")]
        [HttpGet]
        public async Task<ActionResult> Delete(int id)
        {
            var book = await db.Books
                .Include(b => b.Library)
                .Include("BookAuthors.Author")
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            var vm = new BookDetailsViewModel
            {
                Id = book.Id,
                Title = book.Title,
                LibraryName = book.Library != null ? book.Library.Name : "—",
                Authors = book.BookAuthors != null && book.BookAuthors.Any()
                                ? string.Join(", ", book.BookAuthors.Select(ba => ba.Author.Name))
                                : "—",
                Isbn = book.Isbn,
                CreatedAt = book.CreatedAt
            };

            return View("~/Views/Admin/Books/Delete.cshtml", vm);
        }

        // POST: /Admin/Books/Delete/5
        [Route("Delete/{id:int}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var book = await db.Books
                .Include(b => b.BookAuthors)
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return HttpNotFound();

            db.BookAuthors.RemoveRange(book.BookAuthors);
            db.BookGenres.RemoveRange(book.BookGenres);
            db.Books.Remove(book);

            await db.SaveChangesAsync();

            TempData["Success"] = "Book deleted successfully.";
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