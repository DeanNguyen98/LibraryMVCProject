using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public int LibraryId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [MaxLength(20)]
        public string Isbn { get; set; }

        public string Summary { get; set; }

        public string CoverImageUrl { get; set; }

        public int TotalCopies { get; set; } = 1;

        public int AvailableCopies { get; set; } = 1;

        public int BorrowedTimes { get; set; } = 0;

        public int BorrowSettingsId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Library Library { get; set; }
        public virtual BorrowSettings BorrowSettings { get; set; }
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
        public virtual ICollection<BookGenre> BookGenres { get; set; }
        public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}