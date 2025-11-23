using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyListeningTracker.Services;

namespace SpotifyListeningTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrackController : ControllerBase
    {
        private readonly TrackService _trackService;

        public TrackController(TrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopTracks()
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _trackService.GetTopTracks(accessToken);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
