using System.ComponentModel.DataAnnotations;
using LibraryManagementMVC.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Entities
{
    public class BookEntity : BaseEntity
    {
        public BookEntity()
        {
            CreatedDate = DateTime.Now;
        }

        [Required(ErrorMessage = "Title is required.")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Price is a required field.")]
        [Range(0.01, 1000000, ErrorMessage = "Price must be between 0.01 and 1,000,000.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Author selection is required.")]
        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Publish date is required.")]
        public DateTime PublishDate { get; set; }

        [Required(ErrorMessage = "ISBN is required.")]
        [StringLength(
            13,
            MinimumLength = 13,
            ErrorMessage = "ISBN number must be exactly 13 characters long."
        )]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int CopiesAvailable { get; set; }

        public AuthorEntity? Author { get; set; }

        public ICollection<BookOrderEntity>? BookOrders { get; set; }

        public ICollection<BookGenreEntity>? BookGenres { get; set; }
    }
}
