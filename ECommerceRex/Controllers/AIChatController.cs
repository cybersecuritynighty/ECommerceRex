using ECommerceRex.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace ECommerceRex.Controllers;

public class AIChatController : Controller
{
    private readonly IAIChatService _chatService;
    private readonly IDistributedCache _cache; // optional for persisting history across requests

    public AIChatController(IAIChatService chatService, IDistributedCache cache)
    {
        _chatService = chatService;
        _cache = cache;
    }

    public IActionResult Index()
    {
        // Load chat history from session (or cache) and pass to view
        var history = HttpContext.Session.GetString("ChatHistory");
        ViewBag.History = string.IsNullOrEmpty(history) ? new List<ChatMessage>() : JsonSerializer.Deserialize<List<ChatMessage>>(history);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Send(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return BadRequest("Message cannot be empty.");

        // Get current history
        var historyJson = HttpContext.Session.GetString("ChatHistory");
        var history = string.IsNullOrEmpty(historyJson) ? new List<ChatMessage>() : JsonSerializer.Deserialize<List<ChatMessage>>(historyJson);

        // Add user message
        history.Add(new ChatMessage { Role = "user", Content = message });

        // Get AI response
        var aiReply = await _chatService.GetResponseAsync(message);

        // Add AI response
        history.Add(new ChatMessage { Role = "assistant", Content = aiReply });

        // Save history (limit to last 20 messages to avoid session bloat)
        if (history.Count > 20)
            history = history.Skip(history.Count - 20).ToList();

        HttpContext.Session.SetString("ChatHistory", JsonSerializer.Serialize(history));

        return Json(new { reply = aiReply, history });
    }

    public IActionResult ClearHistory()
    {
        HttpContext.Session.Remove("ChatHistory");
        return RedirectToAction("Index");
    }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}
