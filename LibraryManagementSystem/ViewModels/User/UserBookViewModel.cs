using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.ViewModels.User
{
    public class UserBooksViewModel
    {
        public List<BookListItem> Books { get; set; }
        public List<string> Genres { get; set; }
        public string SearchQuery { get; set; }
        public string SelectedGenre { get; set; }
        public bool AtBorrowLimit { get; set; }
        public int LoanDurationDays { get; set; }

        public class BookListItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Authors { get; set; }
            public string Genre { get; set; }
            public string Status { get; set; }
        }
    }
}