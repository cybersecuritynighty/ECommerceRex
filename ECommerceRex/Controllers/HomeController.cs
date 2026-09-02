using Microsoft.AspNetCore.Mvc;

namespace ECommerceRex.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
    public IActionResult About() => View();
    public IActionResult Product() => View();
    public IActionResult Bank() => View();
}
