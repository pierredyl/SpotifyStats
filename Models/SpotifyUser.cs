namespace SpotifyListeningTracker.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenExpiration { get; set; }
        public string country { get; set; } = string.Empty;
        public string imageUrl { get; set; } = string.Empty;

        public int Followers { get; set; }
    }
}