using ECommerceRex.Data;
using ECommerceRex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceRex.Controllers;

[Authorize]
public class BankController : Controller
{
    private readonly ApplicationDbContext _context;

    public BankController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var account = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);
        return View(account);
    }

    public async Task<IActionResult> Wallet()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var account = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);
        return View(account);
    }

    public async Task<IActionResult> History()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var transactions = await _context.Transactions
            .Where(t => t.SenderId == userId || (t.ReceiverId == userId))
            .OrderByDescending(t => t.TransactionDate)
            .ToListAsync();
        return View(transactions);
    }

    public IActionResult Account() => View();
}
