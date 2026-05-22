using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class BookCreateViewModel
    {
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [MaxLength(20, ErrorMessage = "ISBN cannot exceed 20 characters.")]
        public string Isbn { get; set; }

        public string Summary { get; set; }

        public string CoverImageUrl { get; set; }

        [Required(ErrorMessage = "Please select a library.")]
        public int LibraryId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Total copies must be at least 1.")]
        public int TotalCopies { get; set; } = 1;

        [Range(0, int.MaxValue, ErrorMessage = "Available copies cannot be negative.")]
        public int AvailableCopies { get; set; } = 1;

        // Comma-separated author names entered by admin
        public string AuthorNames { get; set; }

        // Comma-separated genre names entered by admin
        public string GenreNames { get; set; }

        // Dropdown for Library
        public List<SelectListItem> LibraryOptions { get; set; }
            = new List<SelectListItem>();
    }
}