using System.Diagnostics;
using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Enums;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using LibraryManagementMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Controllers;

public class GenreController : Controller
{
    private readonly LibraryManagementDbContext _context;
    private readonly IRepository _repository;
    private readonly IConfiguration _configuration;

    public GenreController(
        LibraryManagementDbContext context,
        IRepository repository,
        IConfiguration configuration
    )
    {
        _context = context;
        _repository = repository;
        _configuration = configuration;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
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

        ViewBag.GenreList = await _repository.GetGenres();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(GenreEntity formData)
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

        if (!ModelState.IsValid)
        {
            ViewBag.GenreList = await _repository.GetGenres();
            return View(formData);
        }

        var genreNames = formData
            .Name.Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(name => name.Trim())
            .Distinct()
            .ToList();

        foreach (var genreName in genreNames)
        {
            var genre = new GenreEntity
            {
                Name = genreName,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
            };

            _context.Genres.Add(genre);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction("List", "Home");
    }
}
