using ECommerceRex.Data;
using ECommerceRex.Models;
using ECommerceRex.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceRex.Controllers;

[Authorize]
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IRedisCacheService _cache;

    public ProductsController(ApplicationDbContext context, IRedisCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<IActionResult> Index()
    {
        // Cache products for 5 minutes
        const string cacheKey = "all_products";
        var products = await _cache.GetAsync<List<Product>>(cacheKey);
        if (products == null)
        {
            products = await _context.Products.ToListAsync();
            await _cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
        }
        return View(products);
    }

    public IActionResult CRUD() { 
        return View(); 
    }
    
    public IActionResult Supply() {
        return View();
    }
    
    public IActionResult Category() {
        return View();
    }

    // Additional CRUD actions omitted for brevity (would include Create, Edit, Delete)
    [HttpPost]
    [Authorize(Roles = "Admin")] // Optional: restrict to admins
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            // Invalidate cache
            await _cache.RemoveAsync("all_products");
        }
        return RedirectToAction(nameof(Index));
    }
}
