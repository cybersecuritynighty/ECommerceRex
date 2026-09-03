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

    [HttpPost]
public async Task<IActionResult> FaceCheckIn([FromBody] FaceCheckInRequest request)
{
    // Get all users with face embeddings
    var users = await _context.Users
        .Where(u => !string.IsNullOrEmpty(u.FaceEmbeddings))
        .ToListAsync();

    var knownEncodings = users
        .Select(u => new UserFaceEncoding
        {
            UserId = u.Id,
            Encoding = JsonSerializer.Deserialize<float[]>(u.FaceEmbeddings)
        })
        .Where(e => e.Encoding != null && e.Encoding.Length > 0)
        .ToList();

    if (!knownEncodings.Any())
        return Json(new { success = false, error = "No registered faces found." });

    var userId = await _faceRecognitionService.RecognizeFaceAsync(request.ImageBase64, knownEncodings);
    if (userId == null)
        return Json(new { success = false, error = "Face not recognized." });

    var user = await _context.Users.FindAsync(userId);
    if (user == null)
        return Json(new { success = false, error = "User not found." });

    // Perform check-in
    var attendance = new Attendance
    {
        UserId = user.Id,
        CheckInTime = DateTime.UtcNow,
        ScanCode = "FACE_" + Guid.NewGuid().ToString().Substring(0, 8)
    };
    _context.Attendances.Add(attendance);
    await _context.SaveChangesAsync();

    return Json(new { success = true, username = user.Username });
}

public class FaceCheckInRequest
{
    public string ImageBase64 { get; set; } = "";
}
}
