using System;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class BorrowSettingDetailsViewModel
    {
        public int Id { get; set; }
        public string SettingName { get; set; }
        public int LoanDurationDays { get; set; }
        public int RenewalLimit { get; set; }
        public decimal OverdueFinePerDay { get; set; }
        public int MaxBorrowableItems { get; set; }
        public string Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}