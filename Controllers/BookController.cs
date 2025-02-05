using System.Linq;
using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Enums;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using LibraryManagementMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Controllers;

public class BookController : Controller
{
    private readonly LibraryManagementDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRepository _repository;

    public BookController(
        LibraryManagementDbContext context,
        IConfiguration configuration,
        IRepository repository
    )
    {
        _repository = repository;
        _context = context;
        _configuration = configuration;
    }

    public async Task<IActionResult> Details(int id)
    {
        var token = Request.Cookies["AuthToken"];
        if (token == null)
        {
            return RedirectToAction("Login", "User");
        }

        var claimsPrincipal = JwtHelper.GetPrincipalFromExpiredToken(
            token,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!
        );

        if (claimsPrincipal == null)
        {
            return RedirectToAction("Login", "User");
        }

        var userEmailClaim = claimsPrincipal.FindFirst("Email");
        if (userEmailClaim == null)
        {
            return RedirectToAction("Login", "User");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmailClaim.Value);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }

        var userRoleClaim = claimsPrincipal.FindFirst("UserRole");
        if (userRoleClaim == null)
        {
            return BadRequest("User role not found in the token.");
        }

        if (!Enum.TryParse<UserRole>(userRoleClaim.Value, out UserRole userRole))
        {
            return BadRequest("Invalid user role format in the token.");
        }

        ViewBag.UserRole = userRole;
        ViewBag.User = user;

        var bookEntity = await _repository.GetBookById(id);
        var book = await _context
            .Books.Include(b => b.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(b => b.Id == id);
        var author = await _repository.GetAuthorById(book.AuthorId);
        if (book is null || author is null)
        {
            return NotFound();
        }

        var genreIds = book.BookGenres.Select(bg => bg.GenreId).ToList();
        var genres = await _context.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

        var model = new BookViewModel
        {
            Book = book,
            GenreList = genres,
            Genres = genreIds.ToArray(),
        };

        ViewBag.Author = author;
        return View(model);
    }

    public async Task<IActionResult> List()
    {
        var token = Request.Cookies["AuthToken"];
        if (token == null)
        {
            return RedirectToAction("Login", "User");
        }

        var claimsPrincipal = JwtHelper.GetPrincipalFromExpiredToken(
            token,
            _configuration["Jwt:SecretKey"]!,
            _configuration["Jwt:Issuer"]!,
            _configuration["Jwt:Audience"]!
        );

        if (claimsPrincipal == null)
        {
            return RedirectToAction("Login", "User");
        }

        var userEmailClaim = claimsPrincipal.FindFirst("Email");
        if (userEmailClaim == null)
        {
            return RedirectToAction("Login", "User");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == userEmailClaim.Value);
        if (user == null)
        {
            return RedirectToAction("Login", "User");
        }

        var userRoleClaim = claimsPrincipal.FindFirst("UserRole");
        if (userRoleClaim == null)
        {
            return BadRequest("User role not found in the token.");
        }

        if (!Enum.TryParse<UserRole>(userRoleClaim.Value, out UserRole userRole))
        {
            return BadRequest("Invalid user role format in the token.");
        }

        ViewBag.UserRole = userRole;
        ViewBag.User = user;

        var _bookList = await _repository.GetBooks();

        return View(_bookList);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Authors = await _repository.GetAuthors();
        ViewBag.GenreList = await _repository.GetGenres();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Authors = await _repository.GetAuthors();
            ViewBag.GenreList = await _repository.GetGenres();
            return View(formData);
        }

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var newBook = new BookEntity
            {
                Title = formData.Book.Title,
                Price = formData.Book.Price,
                AuthorId = formData.Book.AuthorId,
                PublishDate = formData.Book.PublishDate,
                ISBN = formData.Book.ISBN,
                CopiesAvailable = formData.Book.CopiesAvailable,
            };

            _context.Books.Add(newBook);
            await _context.SaveChangesAsync();

            foreach (var genreId in formData.Genres)
            {
                var bookGenre = new BookGenreEntity
                {
                    BookId = newBook.Id,
                    GenreId = genreId,
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                };
                _context.BookGenres.Add(bookGenre);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
        }

        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _context
            .Books.Include(a => a.Author)
            .Include(b => b.BookGenres)
            .ThenInclude(g => g.Genre)
            .Where(x => !x.IsDeleted)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound();

        var viewModel = new BookViewModel
        {
            Book = book,
            GenreList = book.BookGenres.Select(bg => bg.Genre).ToList(),
            Genres = book.BookGenres.Select(bg => bg.GenreId).ToArray(),
        };

        ViewBag.Authors = await _repository.GetAuthors();
        ViewBag.GenreList = await _repository.GetGenres();

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Authors = await _repository.GetAuthors();
            ViewBag.GenreList = await _repository.GetGenres();
            return View(formData);
        }

        var bookEntity = await _repository.GetBookById(formData.Book.Id);
        if (bookEntity == null)
            return NotFound();

        bookEntity.Title = formData.Book.Title;
        bookEntity.Price = formData.Book.Price;
        bookEntity.PublishDate = formData.Book.PublishDate;
        bookEntity.ISBN = formData.Book.ISBN;
        bookEntity.CopiesAvailable = formData.Book.CopiesAvailable;
        bookEntity.AuthorId = formData.Book.AuthorId;
        bookEntity.ModifiedDate = DateTime.Now;

        var bookGenres = await _context
            .BookGenres.Where(x => x.BookId == bookEntity.Id && !x.IsDeleted)
            .ToListAsync();

        if (bookGenres == null)
            return BadRequest();

        foreach (var genre in bookGenres)
        {
            _context.BookGenres.Remove(genre);
        }

        foreach (var genreId in formData.Genres)
        {
            var newBookGenre = new BookGenreEntity
            {
                BookId = bookEntity.Id,
                GenreId = genreId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };
            _context.BookGenres.Add(newBookGenre);
        }

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return RedirectToAction("List");
        }

        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> Remove(int id)
    {
        var bookEntity = await _repository.GetBookById(id);

        if (bookEntity == null)
        {
            return NotFound("Book not found");
        }

        var bookGenres = await _context
            .BookGenres.Where(x => x.BookId == bookEntity.Id && !x.IsDeleted)
            .ToListAsync();

        var viewModel = new BookViewModel
        {
            Book = bookEntity,
            GenreList = bookGenres.Select(bg => bg.Genre).ToList(),
            Genres = bookGenres.Select(bg => bg.GenreId).ToArray(),
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, BookViewModel formData)
    {
        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var bookEntity = await _repository.GetBookById(id);
            if (bookEntity is null)
                return NotFound("Book not found");

            bookEntity.IsDeleted = true;
            bookEntity.ModifiedDate = DateTime.Now;

            var bookGenres = await _context
                .BookGenres.Where(x => x.BookId == bookEntity.Id && !x.IsDeleted)
                .ToListAsync();

            if (bookGenres is null || bookGenres.Count == 0)
                return BadRequest("No genres found for this book.");

            foreach (var genre in bookGenres)
            {
                genre.IsDeleted = true;
                genre.ModifiedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return RedirectToAction("List");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
        }
        return RedirectToAction("List");
    }
}
