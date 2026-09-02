using ECommerceRex.Data;
using ECommerceRex.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerceRex.Controllers;

[Authorize]
public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _context;

    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var attendances = await _context.Attendances
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CheckInTime)
            .ToListAsync();
        return View(attendances);
    }

    public IActionResult Scan() => View();

    [HttpPost]
    public async Task<IActionResult> CheckIn(string scanCode)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var attendance = new Attendance
        {
            UserId = userId,
            CheckInTime = DateTime.UtcNow,
            ScanCode = scanCode
        };
        _context.Attendances.Add(attendance);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CheckOut(int attendanceId)
    {
        var attendance = await _context.Attendances.FindAsync(attendanceId);
        if (attendance != null && attendance.UserId == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"))
        {
            attendance.CheckOutTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction("Index");
    }
}
