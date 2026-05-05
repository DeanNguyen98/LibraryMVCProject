using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public enum BorrowStatus
    {
        Borrowed,
        Returned,
        Overdue,
        Renewed
    }

    public class BorrowTransaction
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int LibraryId { get; set; }

        public BorrowStatus Status { get; set; } = BorrowStatus.Borrowed;

        public DateTime BorrowedAt { get; set; } = DateTime.UtcNow;

        public DateTime DueDate { get; set; }

        public DateTime? ReturnedAt { get; set; }

        public int RenewalsUsed { get; set; } = 0;

        public decimal FineAmount { get; set; } = 0;

        public decimal FinePaid { get; set; } = 0;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
        public virtual Library Library { get; set; }
    }
}