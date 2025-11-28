using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using Microsoft.Extensions.Options;
using SpotifyListeningTracker.Models;

namespace SpotifyListeningTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly SpotifySettings _spotifySettings;
        private readonly string _frontendUrl;
        private readonly bool _isDevelopment;

        public AuthController(IOptions<SpotifySettings> spotifySettings, IConfiguration configuration, IWebHostEnvironment env)
        {
            _spotifySettings = spotifySettings.Value;
            _frontendUrl = Environment.GetEnvironmentVariable("frontendUrl")
                ?? configuration["frontendUrl"]
                ?? "http://127.0.0.1:5173";
            _isDevelopment = env.IsDevelopment();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Return configuration for frontend to handle OAuth
            return Ok(new
            {
                clientId = _spotifySettings.ClientId,
                redirectUri = _spotifySettings.RedirectUri,
                scopes = new[]
                {
                    Scopes.UserReadPrivate,
                    Scopes.UserReadEmail,
                    Scopes.PlaylistReadPrivate,
                    Scopes.UserReadRecentlyPlayed,
                    Scopes.UserTopRead,
                    Scopes.UserLibraryRead,
                }
            });
        }

        [HttpPost("token")]
        public async Task<IActionResult> ExchangeToken([FromBody] TokenExchangeRequest request)
        {
            if (string.IsNullOrEmpty(request.Code))
            {
                return BadRequest(new { error = "Code is required" });
            }

            try
            {

                // Create the request object
                var tokenRequest = new AuthorizationCodeTokenRequest(
                    _spotifySettings.ClientId,
                    _spotifySettings.ClientSecret,
                    request.Code,
                    new Uri(_spotifySettings.RedirectUri)
                );
                // Exchange code for access token
                var tokenResponse = await new OAuthClient().RequestToken(tokenRequest);

                // Set tokens in HTTP-only cookies
                // In development: SameSite=None with Secure=false (allows cross-origin in dev)
                // In production: SameSite=None with Secure=true (HTTPS required)
                Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = !_isDevelopment,
                    SameSite = SameSiteMode.None,
                    Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    Path = "/"
                });

                if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                {
                    Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = !_isDevelopment,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                        Path = "/"
                    });
                }

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        public class TokenExchangeRequest
        {
            public string Code { get; set; } = string.Empty;
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var newResponse = await new OAuthClient().RequestToken(
                    new AuthorizationCodeRefreshRequest(
                        _spotifySettings.ClientId,
                        _spotifySettings.ClientSecret,
                        request.RefreshToken
                    )
                );

                return Ok(new
                {
                    access_token = newResponse.AccessToken,
                    expires_in = newResponse.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        public class RefreshTokenRequest
        {
            public string RefreshToken { get; set; } = string.Empty;
        }
    }
}