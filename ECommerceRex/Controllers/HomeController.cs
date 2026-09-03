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

    // GET: /Home/Product
    public async Task<IActionResult> Product(string? category, string? brand)
    {
        var products = _context.Products.AsQueryable();

        // Category filter (case-insensitive)
        if (!string.IsNullOrEmpty(category) && category != "All")
        {
            products = products.Where(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        // Brand filter (we'll map to Supplier or a custom property; here we use Supplier as brand)
        if (!string.IsNullOrEmpty(brand) && brand != "All")
        {
            products = products.Where(p => p.Supplier != null && p.Supplier.Equals(brand, StringComparison.OrdinalIgnoreCase));
        }

        var productList = await products.ToListAsync();

        // Get distinct categories and brands for navigation
        ViewBag.Categories = await _context.Products.Select(p => p.Category).Distinct().Where(c => c != null).ToListAsync();
        ViewBag.Brands = await _context.Products.Select(p => p.Supplier).Distinct().Where(b => b != null).ToListAsync();
        ViewBag.SelectedCategory = category;
        ViewBag.SelectedBrand = brand;

        return View(productList);
    }

    // GET: /Home/ProductDetails/{id}
    public async Task<IActionResult> ProductDetails(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        return View(product);
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
