using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class ManageLibraryViewModel
    {
        public List<LibraryRowViewModel> Libraries { get; set; }
            = new List<LibraryRowViewModel>();

        public string SearchTerm { get; set; }
    }
    public class LibraryRowViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string OperatingHours { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public int TotalBooks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}