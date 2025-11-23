using SpotifyAPI.Web;

namespace SpotifyListeningTracker.Services
{
    public class UserService
    {
        public async Task<object> GetCurrentUserProfile(string accessToken)
        {
            var spotify = new SpotifyClient(accessToken);
            var user = await spotify.UserProfile.Current();

            return new
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Followers = user.Followers?.Total ?? 0,
                ImageUrl = user.Images?.FirstOrDefault()?.Url
            };
        }

        public async Task<object> GetUserProfile(string accessToken, string userId)
        {
            var spotify = new SpotifyClient(accessToken);
            var user = await spotify.UserProfile.Get(userId);

            return new
            {
                Id = user.Id,
                DisplayName = user.DisplayName,
                Followers = user.Followers?.Total ?? 0,
                ImageUrl = user.Images?.FirstOrDefault()?.Url
            };
        }
    }
}
