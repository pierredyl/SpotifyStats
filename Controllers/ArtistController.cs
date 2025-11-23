using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyListeningTracker.Services;

namespace SpotifyListeningTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistService _artistService;

        public ArtistController(ArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("top")]
        public async Task<IActionResult> GetTopArtists()
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _artistService.GetTopArtists(accessToken);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{artistId}/genres")]
        public async Task<IActionResult> GetArtistGenres(string artistId)
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _artistService.GetArtistGenres(accessToken, artistId);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
