using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyListeningTracker.Models;
using SpotifyListeningTracker.Services;

namespace SpotifyListeningTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var spotify = new SpotifyClient(accessToken);
                var profile = await spotify.UserProfile.Current();

                var user = new User
                {
                    Id = profile.Id,
                    DisplayName = profile.DisplayName ?? string.Empty,
                    Email = profile.Email ?? string.Empty,
                    country = profile.Country ?? string.Empty,
                    imageUrl = profile.Images?.FirstOrDefault()?.Url ?? string.Empty,
                    Followers = profile.Followers?.Total ?? 0
                };

                return Ok(user);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _userService.GetUserProfile(accessToken, userId);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
