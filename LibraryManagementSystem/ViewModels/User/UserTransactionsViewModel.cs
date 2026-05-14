using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels.User
{
    public class UserTransactionsViewModel
    {
        public List<ActiveBorrowItem> ActiveBorrows { get; set; }
        public List<OverdueItem> OverdueItems { get; set; }
        public List<HistoryItem> HistoryItems { get; set; }

        public class ActiveBorrowItem
        {
            public int TransactionId { get; set; }
            public string BookTitle { get; set; }
            public int BookId { get; set; }
            public DateTime BorrowedAt { get; set; }
            public DateTime DueDate { get; set; }
            public string ReturnLocation { get; set; }
        }

        public class OverdueItem
        {
            public int TransactionId { get; set; }
            public string BookTitle { get; set; }
            public DateTime DueDate { get; set; }
            public int DaysOverdue { get; set; }
            public decimal FineAmount { get; set; }
        }

        public class HistoryItem
        {
            public int BookId { get; set; }
            public string BookTitle { get; set; }
            public bool ReturnedOnTime { get; set; }
            public DateTime ReturnedAt { get; set; }
            public decimal FinePaid { get; set; }
            public bool HasFeedback { get; set; }
            public int FeedbackRating { get; set; }
            public string FeedbackComment { get; set; }
        }
    }
}
