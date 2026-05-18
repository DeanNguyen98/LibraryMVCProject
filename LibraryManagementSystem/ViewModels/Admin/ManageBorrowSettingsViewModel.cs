using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class ManageBorrowSettingsViewModel
    {
        public List<BorrowSettingRowViewModel> Settings { get; set; }
            = new List<BorrowSettingRowViewModel>();
    }

    public class BorrowSettingRowViewModel
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