namespace LibraryManagementMVC.Entities
{
    public class BookGenreEntity : BaseEntity
    {
        public int BookId { get; set; }
        public int GenreId { get; set; }

        public BookEntity? Book { get; set; }
        public GenreEntity? Genre { get; set; }
    }
}
