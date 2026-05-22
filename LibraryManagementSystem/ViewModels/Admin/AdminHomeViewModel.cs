using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class AdminHomeViewModel
    {
        public string WelcomeMessage { get; set; }

        // Stat cards
        public int TotalBooks { get; set; }
        public int BorrowedBooks { get; set; }
        public int OverdueBooks { get; set; }
        public int TotalMembers { get; set; }
        public int PendingReservations { get; set; }
        public int TotalFeedback { get; set; }

        // Recent activity
        public List<RecentActivityItem> RecentActivity { get; set; }
    }

    public class RecentActivityItem
    {
        public string Activity { get; set; }
        public string UserName { get; set; }
        public string Details { get; set; }
        public DateTime Time { get; set; }

        public string TimeAgo
        {
            get
            {
                var diff = DateTime.UtcNow - Time;
                if (diff.TotalMinutes < 1) return "just now";
                if (diff.TotalMinutes < 60) return (int)diff.TotalMinutes + " minutes ago";
                if (diff.TotalHours < 24) return (int)diff.TotalHours + " hours ago";
                return (int)diff.TotalDays + " days ago";
            }
        }
    }
}
