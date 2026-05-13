using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.ViewModels.User
{
    public class UserBookDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Isbn { get; set; }
        public string Genre { get; set; }
        public string Summary { get; set; }
        public string CoverImageUrl { get; set; }
        public string Status { get; set; }
        public bool AtBorrowLimit { get; set; }
        public int LoanDurationDays { get; set; }
        public List<ReviewItem> Reviews { get; set; }

        public class ReviewItem
        {
            public string ReviewerName { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
        }
    }
}