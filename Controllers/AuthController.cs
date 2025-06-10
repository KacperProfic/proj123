using Microsoft.AspNetCore.Mvc;

namespace proj123.Controllers;

public class AuthController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}