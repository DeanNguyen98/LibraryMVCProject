using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using LibraryManagementSystem.Models;

namespace LibraryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Library> Libraries { get; set; }
        public DbSet<BorrowSettings> BorrowSettings { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<BookAuthor> BookAuthors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<BookGenre> BookGenres { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Disable cascade delete globally to prevent cycle errors
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<System.Data.Entity.ModelConfiguration.Conventions.ManyToManyCascadeDeleteConvention>();

            // Composite PKs for junction tables
            modelBuilder.Entity<BookAuthor>()
                .HasKey(ba => new { ba.BookId, ba.AuthorId });

            modelBuilder.Entity<BookGenre>()
                .HasKey(bg => new { bg.BookId, bg.GenreId });

            // Unique constraints
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_User_Email") { IsUnique = true }));

            modelBuilder.Entity<Book>()
                .Property(b => b.Isbn)
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_Book_Isbn") { IsUnique = true }));

            modelBuilder.Entity<Genre>()
                .Property(g => g.Name)
                .HasColumnAnnotation("Index",
                    new IndexAnnotation(new IndexAttribute("IX_Genre_Name") { IsUnique = true }));

            // Decimal precision
            modelBuilder.Entity<BorrowSettings>()
                .Property(bs => bs.OverdueFinePerDay)
                .HasPrecision(10, 2);

            modelBuilder.Entity<BorrowTransaction>()
                .Property(bt => bt.FineAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<BorrowTransaction>()
                .Property(bt => bt.FinePaid)
                .HasPrecision(10, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}