using System.Diagnostics;
using LibraryManagementMVC.Data;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using LibraryManagementMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Controllers;

public class UserController : Controller
{
    private readonly LibraryManagementDbContext _context;
    private readonly IConfiguration _configuration;

    private readonly IRepository _repository;

    public UserController(
        LibraryManagementDbContext context,
        IConfiguration configuration,
        IRepository repository
    )
    {
        _context = context;
        _configuration = configuration;
        _repository = repository;
    }

    public IActionResult Logout()
    {
        Response.Cookies.Delete("AuthToken");

        return RedirectToAction("Login", "User");
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(SignInViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            return View(formData);
        }

        var token = _repository.AuthenticateUser(formData);

        if (token is null)
        {
            ModelState.AddModelError(string.Empty, "Invalid email or password.");
            return View(formData);
        }

        Response.Cookies.Append(
            "AuthToken",
            token,
            new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(2),
            }
        );

        ViewData["LoginMessage"] = "Login successful.";
        return RedirectToAction("List", "Home");
    }

    [HttpGet]
    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel formData)
    {
        if (!ModelState.IsValid)
        {
            return View(formData);
        }

        var user = _context.Users.FirstOrDefault(x => x.Email == formData.Email);

        if (user is not null)
        {
            ViewData["EmailExists"] = "This email is already registered. Please log in.";
            return View(formData);
        }
        ;
        var newUser = new UserEntity
        {
            FirstName = formData.FirstName,
            LastName = formData.LastName,
            Email = formData.Email,
            Password = formData.Password,
            PhoneNumber = formData.PhoneNumber,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now,
        };

        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        ViewData["SuccessMessage"] = "Registration successful. You can now log in.";

        return RedirectToAction("Login");
    }
}
