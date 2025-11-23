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
        private static EmbedIOAuthServer? _server;

        public AuthController(IOptions<SpotifySettings> spotifySettings)
        {
            _spotifySettings = spotifySettings.Value;
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login()
        {
            // Authorization URL
            var loginRequest = new LoginRequest(
                new Uri(_spotifySettings.RedirectUri),
                _spotifySettings.ClientId,
                LoginRequest.ResponseType.Code
            )
            {
                Scope = new[]
                {
                    Scopes.UserReadPrivate,
                    Scopes.UserReadEmail,
                    Scopes.PlaylistReadPrivate,
                    Scopes.UserReadRecentlyPlayed,
                    Scopes.UserTopRead,
                    Scopes.UserLibraryRead,
                }
            };

            var uri = loginRequest.ToUri();
            return Ok(new { authUrl = uri.ToString() });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code, [FromQuery] string? error)
        {
            if (!string.IsNullOrEmpty(error))
            {
                return BadRequest(new { error });
            }

            try
            {
                // Exchange code for access token
                var tokenResponse = await new OAuthClient().RequestToken(
                    new AuthorizationCodeTokenRequest(
                        _spotifySettings.ClientId,
                        _spotifySettings.ClientSecret,
                        code,
                        new Uri(_spotifySettings.RedirectUri)
                    )
                );

                if (string.IsNullOrEmpty(tokenResponse.AccessToken))
                {
                    return Redirect("http://localhost:5173/?error=token_exchange_failed");
                }

                // Set tokens in HTTP-only cookies
                Response.Cookies.Append("access_token", tokenResponse.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None, // Required for cross-origin
                    Expires = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn),
                    Path = "/"
                });

                if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
                {
                    Response.Cookies.Append("refresh_token", tokenResponse.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddDays(30),
                        Path = "/"
                    });
                }

                return Redirect("http://localhost:5173/dashboard");
            }
            catch (Exception ex)
            {
                return Redirect($"http://localhost:5173/?error={Uri.EscapeDataString(ex.Message)}");
            }
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