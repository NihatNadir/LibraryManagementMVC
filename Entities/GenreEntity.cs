namespace LibraryManagementMVC.Entities
{
    public class GenreEntity : BaseEntity
    {
        public string? Name { get; set; }
        public ICollection<BookGenreEntity>? BookGenres { get; set; }
    }
}
