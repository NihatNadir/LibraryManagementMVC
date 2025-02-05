using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Entities
{
    public class AuthorEntity : BaseEntity
    {
        [Required(ErrorMessage = "Author name is required.")]
        public string? FullName { get; set; }

        public ICollection<BookEntity>? Books { get; set; }
    }
}
