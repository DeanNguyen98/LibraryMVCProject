using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class BookAuthor
    {
        public int BookId { get; set; }

        public int AuthorId { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual Author Author { get; set; }
    }
}