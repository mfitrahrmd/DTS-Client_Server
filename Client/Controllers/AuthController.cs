using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace Client.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<HomeController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string ClientId = "1edced7946c04732b1fb519d8a09cc3b";
    private const string ClientSecret = "c307f20f535549c2abfce5090c4c0e86";
    private const string RedirectUri = "http://localhost:5209";
    private const string State = "abcdefghijklmnop";

    private const string Scope =
        "user-library-read playlist-read-private user-read-recently-played user-read-playback-position user-follow-read";

    public AuthController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }
    
    [HttpGet("Login")]
    public IActionResult Login()
    {
        return Redirect(
            $"https://accounts.spotify.com/authorize?response_type=code&client_id={ClientId}&scope={Scope}&redirect_uri={RedirectUri}&state={State}");
    }


    [HttpPost("AccessToken")]
    public async Task<IActionResult> AccessToken(AccessTokenRequest req)
    {
        var res = await _httpClientFactory.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Headers =
                {
                    {
                        HeaderNames.Authorization,
                        $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"))}"
                    }
                },
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", req.Code),
                    new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                    new KeyValuePair<string, string>("grant_type", "authorization_code")
                })
            });

        var fullResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(await res.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString
            });

        return Ok(fullResponse);
    }

    [HttpPost("RefreshToken")]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest req)
    {
        var res = await _httpClientFactory.CreateClient()
            .SendAsync(new HttpRequestMessage(HttpMethod.Post, "https://accounts.spotify.com/api/token")
            {
                Headers =
                {
                    {
                        HeaderNames.Authorization,
                        $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ClientId}:{ClientSecret}"))}"
                    }
                },
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("refresh_token", req.RefreshToken),
                    new KeyValuePair<string, string>("grant_type", "refresh_token")
                })
            });

        var fullResponse = JsonSerializer.Deserialize<Dictionary<string, object>>(await res.Content.ReadAsStreamAsync(),
            new JsonSerializerOptions
            {
                NumberHandling = JsonNumberHandling.WriteAsString
            });

        return Ok(fullResponse);
    }

    public class RefreshTokenRequest
    {
        [Required] public string RefreshToken { get; set; }
    };

    public class AccessTokenRequest
    {
        [Required] public string Code { get; set; }
    };
}