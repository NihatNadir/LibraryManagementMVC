using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Enums;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using LibraryManagementMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Controllers;

public class AuthorController : Controller
{
    private readonly LibraryManagementDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IRepository _repository;

    public AuthorController(
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

        var authorEntity = await _repository.GetAuthorById(id);

        if (authorEntity is null)
            return NotFound();

        return View(authorEntity);
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

        var _authorList = await _repository.GetAuthors();

        return View(_authorList);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorEntity formData)
    {
        if (!ModelState.IsValid)
        {
            return View(formData);
        }

        var newAuthor = new AuthorEntity { FullName = formData.FullName };

        _context.Authors.Add(newAuthor);
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    public IActionResult Delete()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var authorEntity = await _repository.GetAuthorById(id);

        if (authorEntity is null)
            return NotFound();

        return View(authorEntity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AuthorEntity formData)
    {
        var authorEntity = await _repository.GetAuthorById(id);

        if (authorEntity is null)
            return NotFound();

        authorEntity.FullName = formData.FullName;

        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }

    [HttpGet]
    public async Task<IActionResult> Remove(int id)
    {
        var authorEntity = await _repository.GetAuthorById(id);

        if (authorEntity is null)
            return NotFound();

        return View(authorEntity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int id, AuthorEntity formData)
    {
        var authorEntity = await _repository.GetAuthorById(id);

        if (authorEntity is null)
            return NotFound();

        var books = await _context
            .Books.Where(x => x.AuthorId == authorEntity.Id && !x.IsDeleted)
            .Include(b => b.BookGenres)
            .ToListAsync();

        foreach (var book in books)
        {
            foreach (var bookGenre in book.BookGenres.ToList())
            {
                bookGenre.IsDeleted = true;
                bookGenre.ModifiedDate = DateTime.Now;
            }
            book.IsDeleted = true;
        }

        authorEntity.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("List");
    }
}
