using Microsoft.AspNetCore.Mvc;

namespace ECommerceRex.Controllers;

public class HomeController : Controller
{
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
}
