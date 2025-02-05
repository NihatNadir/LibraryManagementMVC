using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Enums;

namespace LibraryManagementMVC.Models
{
    public class SearchViewModel
    {
        public List<AuthorEntity> Authors { get; set; }
        public List<BookEntity> Books { get; set; }
        public List<GenreEntity> Genres { get; set; }
    }
}
