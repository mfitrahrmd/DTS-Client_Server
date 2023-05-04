using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Microsoft.Net.Http.Headers;
using MediaTypeHeaderValue = System.Net.Http.Headers.MediaTypeHeaderValue;

namespace Client.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ClientId = "1edced7946c04732b1fb519d8a09cc3b";
    private const string ClientSecret = "c307f20f535549c2abfce5090c4c0e86";
    private const string RedirectUri = "http://localhost:5209";
    private const string State = "abcdefghijklmnop";

    private const string Scope =
        "user-library-read playlist-read-private user-read-recently-played user-read-playback-position user-follow-read";


    public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index()
    {
        return View();
    }

    public IActionResult Login()
    {
        return Redirect(
            $"https://accounts.spotify.com/authorize?response_type=code&client_id={ClientId}&scope={Scope}&redirect_uri={RedirectUri}&state={State}");
    }
  
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}