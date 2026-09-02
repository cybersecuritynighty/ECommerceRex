using ECommerceRex.Data;
using ECommerceRex.Models;
using ECommerceRex.Models.ViewModels;
using ECommerceRex.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceRex.Controllers;

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserController(ApplicationDbContext context, IJwtService jwtService, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
        if (result != PasswordVerificationResult.Success)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // Generate JWT
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role ?? "User"),
            new(ClaimTypes.Name, user.Username)
        };
        var token = _jwtService.GenerateToken(claims);

        // Store token in cookie or return as response; here we set a cookie
        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // plain HTTP
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult SignUp() => View();

    [HttpPost]
    public async Task<IActionResult> SignUp(RegisterViewModel model)
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
            Role = "User"
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Auto-login: generate token
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new(ClaimTypes.Name, user.Username)
        };
        var token = _jwtService.GenerateToken(claims);
        Response.Cookies.Append("auth_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = DateTime.UtcNow.AddHours(1)
        });

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Index() {
        return View(); // Profile view
    }

    // Challenge external provider
[HttpGet]
public IActionResult ExternalLogin(string provider, string returnUrl = "/")
{
    // Redirect to external provider
    var redirectUrl = Url.Action("ExternalLoginCallback", "User", new { returnUrl });
    var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
    return Challenge(properties, provider);
}

// Callback after external login
[HttpGet]
public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/", string? remoteError = null)
{
    if (remoteError != null)
    {
        ModelState.AddModelError("", $"Error from external provider: {remoteError}");
        return View("Login");
    }

    // Get external login info
    var info = await _signInManager.GetExternalLoginInfoAsync();
    if (info == null)
    {
        ModelState.AddModelError("", "Error loading external login information.");
        return View("Login");
    }

    // Check if user already exists
    var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == info.Principal.FindFirst(ClaimTypes.Email)?.Value);
    if (user == null)
    {
        // Register new user
        var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;
        var name = info.Principal.FindFirst(ClaimTypes.Name)?.Value ?? email;
        user = new User
        {
            Username = name,
            Email = email ?? "unknown@example.com",
            PasswordHash = "EXTERNAL_AUTH", // not used
            Role = "User"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    // Create JWT token
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Email, user.Email),
        new(ClaimTypes.Role, user.Role),
        new(ClaimTypes.Name, user.Username)
    };
    var token = _jwtService.GenerateToken(claims);
    Response.Cookies.Append("auth_token", token, new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Lax,
        Expires = DateTime.UtcNow.AddHours(1)
    });

    return LocalRedirect(returnUrl);
}

[HttpGet]
public IActionResult TelegramLogin(string returnUrl = "/")
{
    // Render a view with the Telegram login widget
    ViewBag.ReturnUrl = returnUrl;
    return View();
}

    // Add this method to Controllers/UserController.cs
    [HttpPost]
    [Authorize]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("auth_token");
        return RedirectToAction("Index", "Home");
    }
}
