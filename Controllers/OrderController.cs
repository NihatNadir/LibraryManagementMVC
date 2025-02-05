using System.Diagnostics;
using LibraryManagementMVC.Entities;
using LibraryManagementMVC.Jwt;
using LibraryManagementMVC.Models;
using LibraryManagementMVC.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementMVC.Controllers;

public class OrderController : Controller
{
    private readonly ILogger<OrderController> _logger;
    private readonly IRepository _repository;
    private readonly IConfiguration _configuration;

    public OrderController(
        ILogger<OrderController> logger,
        IRepository repository,
        IConfiguration configuration
    )
    {
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult List()
    {
        var token = Request.Cookies["AuthToken"];
        if (token == null)
        {
            return RedirectToAction("Login", "User");
        }
        return View();
    }
}
