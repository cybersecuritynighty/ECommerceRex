using ECommerceRex.Data;
using ECommerceRex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ECommerceRex.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
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

    [HttpGet]
public IActionResult RegisterFace(int userId)
{
    ViewBag.UserId = userId;
    return View();
}

[HttpPost]
public async Task<IActionResult> RegisterFace(int userId, string imageBase64)
{
    var user = await _context.Users.FindAsync(userId);
    if (user == null) return NotFound();

    var encoding = await _faceRecognitionService.RegisterFaceAsync(userId, imageBase64);
    // Store as JSON
    user.FaceEmbeddings = JsonSerializer.Serialize(encoding);
    await _context.SaveChangesAsync();

    return Json(new { success = true });
}

    public IActionResult CRM() { return View(); }
    

// ... inside AdminController

// ========== USER CRUD ==========

// List – already exists (UserList)

// Create GET
[HttpGet]
public IActionResult UserCreate() => View();

// Create POST
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UserCreate(UserCreateViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    if (await _context.Users.AnyAsync(u => u.Email == model.Email))
    {
        ModelState.AddModelError("Email", "Email already registered.");
        return View(model);
    }

    var user = new User
    {
        Username = model.Username,
        Email = model.Email,
        FullName = model.FullName,
        Role = model.Role ?? "User"
    };
    user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return RedirectToAction(nameof(UserList));
}

// Edit GET
[HttpGet]
public async Task<IActionResult> UserEdit(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null) return NotFound();

    var model = new UserEditViewModel
    {
        Id = user.Id,
        Username = user.Username,
        Email = user.Email,
        FullName = user.FullName,
        Role = user.Role
    };
    return View(model);
}

// Edit POST
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UserEdit(UserEditViewModel model)
{
    if (!ModelState.IsValid) return View(model);

    var user = await _context.Users.FindAsync(model.Id);
    if (user == null) return NotFound();

    // Update fields
    user.Username = model.Username;
    user.Email = model.Email;
    user.FullName = model.FullName;
    user.Role = model.Role ?? "User";

    // If password is provided, update it
    if (!string.IsNullOrEmpty(model.Password))
    {
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);
    }

    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(UserList));
}

// Delete (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UserDelete(int id)
{
    // Prevent deleting the admin user (optional)
    var user = await _context.Users.FindAsync(id);
    if (user == null) return NotFound();

    // Optionally prevent deleting yourself
    var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    if (currentUserId == id)
    {
        TempData["Error"] = "You cannot delete your own account.";
        return RedirectToAction(nameof(UserList));
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();
    return RedirectToAction(nameof(UserList));
}

// ========== PRODUCT CRUD ==========

// List – already exists (ProdList)

// Create GET
[HttpGet]
public IActionResult ProductCreate() => View();

// Create POST
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ProductCreate(Product model)
{
    if (!ModelState.IsValid) return View(model);

    _context.Products.Add(model);
    await _context.SaveChangesAsync();
    // Invalidate cache
    await _cache.RemoveAsync("all_products");
    return RedirectToAction(nameof(ProdList));
}

// Edit GET
[HttpGet]
public async Task<IActionResult> ProductEdit(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null) return NotFound();
    return View(product);
}

// Edit POST
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ProductEdit(Product model)
{
    if (!ModelState.IsValid) return View(model);

    var product = await _context.Products.FindAsync(model.Id);
    if (product == null) return NotFound();

    // Update fields
    product.Name = model.Name;
    product.Description = model.Description;
    product.Price = model.Price;
    product.StockQuantity = model.StockQuantity;
    product.Category = model.Category;
    product.Supplier = model.Supplier;

    await _context.SaveChangesAsync();
    await _cache.RemoveAsync("all_products");
    return RedirectToAction(nameof(ProdList));
}

// Delete (POST)
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> ProductDelete(int id)
{
    var product = await _context.Products.FindAsync(id);
    if (product == null) return NotFound();

    _context.Products.Remove(product);
    await _context.SaveChangesAsync();
    await _cache.RemoveAsync("all_products");
    return RedirectToAction(nameof(ProdList));
}
}
