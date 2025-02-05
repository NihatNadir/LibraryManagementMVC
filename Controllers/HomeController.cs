using System.Diagnostics;
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

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly LibraryManagementDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        IRepository repository,
        IConfiguration configuration,
        LibraryManagementDbContext context
    )
    {
        _context = context;
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
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

        var books = await _repository.GetBooks();
        var authors = await _repository.GetAuthors();

        var model = new HomeListViewModel
        {
            BookList = books,
            AuthorList = authors,
            IsAuthenticated = !string.IsNullOrEmpty(token),
        };

        return View(model);
    }

    public IActionResult Info()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }

    [HttpGet]
    public IActionResult Search(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            var searchResults = new SearchViewModel
            {
                Authors = new List<AuthorEntity>(),
                Books = new List<BookEntity>(),
                Genres = new List<GenreEntity>(),
            };

            return View(searchResults);
        }
        else
        {
            var authors = _context
                .Authors.Where(x =>
                    !x.IsDeleted && EF.Functions.Like(x.FullName, "%" + searchTerm + "%")
                )
                .ToList();

            var books = _context
                .Books.Where(x =>
                    !x.IsDeleted && EF.Functions.Like(x.Title, "%" + searchTerm + "%")
                )
                .ToList();

            var genres = _context
                .Genres.Where(x =>
                    !x.IsDeleted && EF.Functions.Like(x.Name, "%" + searchTerm + "%")
                )
                .ToList();

            var searchResults = new SearchViewModel
            {
                Authors = authors,
                Books = books,
                Genres = genres,
            };

            return View(searchResults);
        }
    }
}
