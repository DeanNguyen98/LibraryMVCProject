using System.Collections.Generic;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class ManageBooksViewModel
    {
        public List<BookRowViewModel> Books { get; set; }
            = new List<BookRowViewModel>();

        public string SearchTerm { get; set; }
        public int? FilterLibraryId { get; set; }
    }

    public class BookRowViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string LibraryName { get; set; }
        public string Authors { get; set; }
        public string Genres { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
    }
}