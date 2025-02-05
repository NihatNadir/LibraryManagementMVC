using LibraryManagementMVC.Entities;

namespace LibraryManagementMVC.Models
{
    public class BookViewModel
    {
        public BookEntity? Book { get; set; }
        public List<GenreEntity>? GenreList { get; set; }
        public int[] Genres { get; set; }
    }
}
