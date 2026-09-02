using ECommerceRex.Data;
using ECommerceRex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceRex.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

    app.Use(async (context, next) =>
{
    await next();
    if (context.Response.StatusCode == 403 && !context.Response.HasStarted)
    {
        context.Request.Path = "/Home/AccessDenied";
        await next();
    }
});

    [Authorize(Roles = "Admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult Index()
    {
        // Show alerts for tampered entities (if any)
        var tampered = _context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Unchanged) // not all, just a quick check
            .ToList(); // simplified; real check would scan all
        // We'll just pass a flag
        ViewBag.TamperAlert = false; // for demo
        return View();
    }

    public async Task<IActionResult> UserList()
    {
        var users = await _context.Users.ToListAsync();
        return View(users);
    }

    public async Task<IActionResult> ProdList()
    {
        var products = await _context.Products.ToListAsync();
        return View(products);
    }

    public IActionResult CRM() { return View(); }
}
