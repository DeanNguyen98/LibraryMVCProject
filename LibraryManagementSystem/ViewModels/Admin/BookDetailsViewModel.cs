using System;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class BookDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string LibraryName { get; set; }
        public int LibraryId { get; set; }
        public string Authors { get; set; }
        public string Genres { get; set; }
        public string Isbn { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Summary { get; set; }
        public string CoverImageUrl { get; set; }
    }
}