using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class BorrowSettingCreateViewModel
    {
        [Required(ErrorMessage = "Setting name is required.")]
        [MaxLength(100, ErrorMessage = "Setting name cannot exceed 100 characters.")]
        public string SettingName { get; set; }

        [Required(ErrorMessage = "Loan duration is required.")]
        [Range(1, 365, ErrorMessage = "Loan duration must be between 1 and 365 days.")]
        public int LoanDurationDays { get; set; }

        [Required(ErrorMessage = "Renewal limit is required.")]
        [Range(0, 10, ErrorMessage = "Renewal limit must be between 0 and 10.")]
        public int RenewalLimit { get; set; }

        [Required(ErrorMessage = "Overdue fine per day is required.")]
        [Range(0, 100, ErrorMessage = "Fine must be between 0 and 100.")]
        public decimal OverdueFinePerDay { get; set; }

        [Required(ErrorMessage = "Maximum borrowable items is required.")]
        [Range(1, 50, ErrorMessage = "Maximum items must be between 1 and 50.")]
        public int MaxBorrowableItems { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; }
    }
}