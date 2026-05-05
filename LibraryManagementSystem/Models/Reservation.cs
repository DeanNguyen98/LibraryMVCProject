using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace LibraryManagementSystem.Models
{
    public enum ReservationStatus
    {
        Pending,
        Fulfilled,
        Cancelled,
        Expired
    }

    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public DateTime ReservedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
    }
}