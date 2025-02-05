using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LibraryManagementMVC.Repositories
{
    public interface IRepository
    {
        string? AuthenticateUser(SignInViewModel formData);
        Task<List<BookEntity>> GetBooks();
        Task<List<AuthorEntity>> GetAuthors();
        Task<List<GenreEntity>> GetGenres();
        Task<BookEntity> GetBookById(int id);
        Task<AuthorEntity> GetAuthorById(int id);
        void UpdateBook(BookViewModel book);
        void RemoveBookGenres(int bookId);
    }
}
