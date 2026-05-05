using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibraryManagementSystem.Models
{
    public class BookGenre
    {
        public int BookId { get; set; }

        public int GenreId { get; set; }

        // Navigation properties
        public virtual Book Book { get; set; }
        public virtual Genre Genre { get; set; }
    }
}