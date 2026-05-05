using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace LibraryManagementSystem.Models
{
    public enum UserRole
    {
        Member,
        Admin
    }

    public class User
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; } = UserRole.Member;

        public string Phone { get; set; }

        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<BorrowTransaction> BorrowTransactions { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
    }
}