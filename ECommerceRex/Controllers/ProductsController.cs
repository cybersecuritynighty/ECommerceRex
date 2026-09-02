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

    public IActionResult CRUD() => View();
    public IActionResult Supply() => View();
    public IActionResult Category() => View();

    // Additional CRUD actions omitted for brevity (would include Create, Edit, Delete)
}
