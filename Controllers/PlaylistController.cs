using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web;
using SpotifyListeningTracker.Services;

namespace SpotifyListeningTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlaylistController : ControllerBase
    {
        private readonly PlaylistService _playlistService;

        public PlaylistController(PlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlaylists()
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _playlistService.GetUserPlaylists(accessToken);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("{playlistId}/tracks")]
        public async Task<IActionResult> GetPlaylistTracks(string playlistId)
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            try
            {
                var result = await _playlistService.GetPlaylistTracks(accessToken, playlistId);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("compare")]
        public async Task<IActionResult> ComparePlaylists([FromQuery] string playlist1Id, [FromQuery] string playlist2Id)
        {
            var accessToken = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(accessToken))
            {
                return Unauthorized(new { error = "Access token required" });
            }

            if (string.IsNullOrEmpty(playlist1Id) || string.IsNullOrEmpty(playlist2Id))
            {
                return BadRequest(new { error = "Both playlist IDs are required" });
            }

            try
            {
                var result = await _playlistService.ComparePlaylists(accessToken, playlist1Id, playlist2Id);
                return Ok(result);
            }
            catch (APIException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
