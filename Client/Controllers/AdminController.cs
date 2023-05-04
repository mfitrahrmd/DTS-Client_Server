using Microsoft.AspNetCore.Mvc;

namespace Client.Controllers;

public class AdminController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}