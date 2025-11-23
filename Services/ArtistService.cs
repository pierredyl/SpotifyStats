using SpotifyAPI.Web;

namespace SpotifyListeningTracker.Services
{
    public class ArtistService
    {
        public async Task<object> GetTopArtists(string accessToken)
        {
            var spotify = new SpotifyClient(accessToken);
            var topArtists = await spotify.Personalization.GetTopArtists(new PersonalizationTopRequest
            {
                Limit = 50,
                TimeRangeParam = PersonalizationTopRequest.TimeRange.MediumTerm
            });

            return topArtists.Items?.Select(a => new
            {
                Id = a.Id,
                Name = a.Name,
                Genres = a.Genres,
                ImageUrl = a.Images?.FirstOrDefault()?.Url
            }).ToList() ?? [];
        }

        public async Task<object> GetArtistGenres(string accessToken, string artistId)
        {
            var spotify = new SpotifyClient(accessToken);
            var artist = await spotify.Artists.Get(artistId);

            return new
            {
                Id = artist.Id,
                Name = artist.Name,
                Genres = artist.Genres
            };
        }
    }
}
