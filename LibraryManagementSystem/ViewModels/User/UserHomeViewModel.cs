using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.ViewModels.User
{
    public class UserHomeViewModel
    {
        public string UserFullName { get; set; }
        public int CurrentlyBorrowedCount { get; set; }
        public int AvailableBooksCount { get; set; }
        public int OverdueBooksCount { get; set; }
        public List<TrendingBookItem> TrendingBooks { get; set; }
        public List<NewArrivalItem> NewArrivals { get; set; }
        public List<OverdueNoticeItem> OverdueNotices { get; set; }

        public class TrendingBookItem
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public bool IsAvailable { get; set; }
        }

        public class NewArrivalItem
        {
            public string Title { get; set; }
            public string Authors { get; set; }
            public bool IsAvailable { get; set; }
        }

        public class OverdueNoticeItem
        {
            public string BookTitle { get; set; }
            public DateTime DueDate { get; set; }
            public decimal FineAmount { get; set; }
        }
    }
}