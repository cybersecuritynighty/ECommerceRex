using Microsoft.AspNetCore.Mvc;

namespace ECommerceRex.Controllers;

public class HomeController : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.ProductCount = await _context.Products.CountAsync();
        ViewBag.UserCount = await _context.Users.CountAsync();
        ViewBag.TransactionCount = await _context.Transactions.CountAsync();
        ViewBag.FeaturedProducts = await _context.Products.OrderByDescending(p => p.Id).Take(4).ToListAsync();
        return View();
    }
    
    public IActionResult Index() {
        return View();
    }
    
    public IActionResult About() {
        return View();
    }
    
    public IActionResult Product() {
        return View();
    }
    
    public IActionResult Bank() {
        return View();
    }

    [HttpGet]
    public IActionResult Error(string? id = null)
    {
        ViewData["Error"] = id switch
        {
            "404" => "Page not found.",
            "403" => "Forbidden.",
            "500" => "Internal server error.",
            _ => "An unexpected error occurred."
        };
        return View();
    }

[HttpGet]
public IActionResult NotFound()
{
    ViewData["RequestedUrl"] = Request.Query["requestedUrl"].FirstOrDefault() ?? Request.Path;
    return View();
}

[HttpGet]
public IActionResult AccessDenied()
{
    return View();
}
    
}
