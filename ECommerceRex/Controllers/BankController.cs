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
    // In BankController.cs

    // GET: Bank/Account
    public async Task<IActionResult> Account()
    {
    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    var user = await _context.Users.FindAsync(userId);
    var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);

    if (user == null) return NotFound();

    var model = new BankAccountViewModel
    {
        Username = user.Username,
        Email = user.Email,
        PhoneNumber = user.PhoneNumber,
        Address = user.Address,
        Balance = bankAccount?.Balance ?? 0,
        Currency = bankAccount?.Currency ?? "USD",
        // For credit card, we could store a masked version – we'll keep it empty for now
        CardHolderName = user.FullName
    };
    return View(model);
    }

    // POST: Bank/UpdateAccount
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateAccount(BankAccountViewModel model)
    {
    if (!ModelState.IsValid) return View("Account", model);

    var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
    var user = await _context.Users.FindAsync(userId);
    var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.UserId == userId);

    if (user == null) return NotFound();

    // Update user info
    user.Username = model.Username;
    user.Email = model.Email;
    user.PhoneNumber = model.PhoneNumber;
    user.Address = model.Address;
    user.FullName = model.CardHolderName; // optional

    // If bank account exists, update currency? Not card details (handled separately)
    // For card details, you might store them in a separate secure table.
    // For demonstration, we'll just update the user.

    await _context.SaveChangesAsync();

    TempData["Success"] = "Account updated successfully.";
    return RedirectToAction(nameof(Account));
    }
}
