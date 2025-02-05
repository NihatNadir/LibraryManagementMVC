using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Repositories
{
    public class Repository : IRepository
    {
        private readonly LibraryManagementDbContext _context;
        private readonly IConfiguration _configuration;

        public Repository(LibraryManagementDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public string? AuthenticateUser(SignInViewModel formData)
        {
            var user = _context.Users.FirstOrDefault(x =>
                x.Email == formData.Email && x.Password == formData.Password
            );

            if (user is null)
            {
                return null;
            }

            return JwtHelper.GenerateJwt(
                new JwtDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserType = user.UserRole,
                    SecretKey = _configuration["Jwt:SecretKey"]!,
                    Issuer = _configuration["Jwt:Issuer"]!,
                    Audience = _configuration["Jwt:Audience"]!,
                    ExpireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!),
                }
            );
        }

        public async Task<List<BookEntity>> GetBooks()
        {
            return _context.Books.Where(x => !x.IsDeleted).ToList();
        }

        public async Task<BookEntity> GetBookById(int id)
        {
            var bookEntity = await _context
                .Books.Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (bookEntity == null)
                return null;

            return bookEntity;
        }

        public void UpdateBook(BookViewModel book)
        {
            var updateBook = _context
                .Books.Include(b => b.BookGenres)
                .FirstOrDefault(b => b.Id == book.Book.Id);

            if (updateBook != null)
            {
                updateBook.Title = book.Book.Title;
                updateBook.AuthorId = book.Book.AuthorId;
                updateBook.PublishDate = book.Book.PublishDate;
                updateBook.CopiesAvailable = book.Book.CopiesAvailable;
                updateBook.ISBN = book.Book.ISBN;
                updateBook.Price = book.Book.Price;
                updateBook.ModifiedDate = DateTime.Now;

                if (updateBook.BookGenres != null)
                {
                    _context.BookGenres.RemoveRange(updateBook.BookGenres);
                }

                foreach (var genreId in book.Genres)
                {
                    _context.BookGenres.Add(
                        new BookGenreEntity { BookId = updateBook.Id, GenreId = genreId }
                    );
                }

                _context.SaveChanges();
            }
        }

        public void RemoveBookGenres(int bookId)
        {
            var bookGenres = _context.BookGenres.Where(bg => bg.BookId == bookId);
            _context.BookGenres.RemoveRange(bookGenres);
            _context.SaveChanges();
        }

        public async Task<List<AuthorEntity>> GetAuthors()
        {
            return _context.Authors.Where(x => !x.IsDeleted).ToList();
        }

        public async Task<AuthorEntity?> GetAuthorById(int id)
        {
            return await _context
                .Authors.Where(x => !x.IsDeleted)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<GenreEntity>> GetGenres()
        {
            return _context.Genres.Where(x => !x.IsDeleted).ToList();
        }
    }
}
