using Microsoft.AspNetCore.Mvc;

namespace ClientApp.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}