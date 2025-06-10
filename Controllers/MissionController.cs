using Microsoft.AspNetCore.Mvc;

namespace Projekt_zaliczenie.Controllers;

public class MissionController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}