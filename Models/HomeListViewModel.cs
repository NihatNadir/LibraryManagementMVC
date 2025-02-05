using LibraryManagementMVC.Entities;

namespace LibraryManagementMVC.Models
{
    public class HomeListViewModel()
    {
        public List<BookEntity>? BookList { get; set; }
        public List<AuthorEntity>? AuthorList { get; set; }
        public bool IsAuthenticated { get; set; }
    }
}
