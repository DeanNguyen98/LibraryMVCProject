using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.ViewModels.Admin
{
    public class LibraryCreateViewModel
    {
        [Required(ErrorMessage = "Library name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        public string OperatingHours { get; set; }

        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }

        public string Description { get; set; }
    }
}