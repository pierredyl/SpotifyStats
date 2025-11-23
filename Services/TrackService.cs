using SpotifyAPI.Web;

namespace SpotifyListeningTracker.Services
{
    public class TrackService
    {
        public async Task<object> GetTopTracks(string accessToken)
        {
            var spotify = new SpotifyClient(accessToken);
            var topTracks = await spotify.Personalization.GetTopTracks(new PersonalizationTopRequest
            {
                Limit = 50,
                TimeRangeParam = PersonalizationTopRequest.TimeRange.MediumTerm
            });

            return topTracks.Items?.Select(t => new
            {
                Id = t.Id,
                Name = t.Name,
                Artist = string.Join(", ", t.Artists.Select(a => a.Name)),
                Album = t.Album.Name,
                ImageUrl = t.Album.Images?.FirstOrDefault()?.Url
            }).ToList() ?? [];
        }
    }
}
