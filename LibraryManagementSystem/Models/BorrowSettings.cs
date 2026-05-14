using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace LibraryManagementSystem.Models
{
    public class BorrowSettings
    {
        public int Id { get; set; }

        [Required]
        public string SettingName { get; set; }

        public int LoanDurationDays { get; set; } = 14;

        public int RenewalLimit { get; set; } = 2;

        public decimal OverdueFinePerDay { get; set; }

        public int MaxBorrowableItems { get; set; } = 5;

        public string Status { get; set; } = "Active";

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}