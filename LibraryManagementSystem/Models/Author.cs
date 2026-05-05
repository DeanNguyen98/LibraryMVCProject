using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace LibraryManagementSystem.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Bio { get; set; }

        // Navigation property
        public virtual ICollection<BookAuthor> BookAuthors { get; set; }
    }
}