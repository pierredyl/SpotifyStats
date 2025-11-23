using SpotifyAPI.Web;

namespace SpotifyListeningTracker.Services
{
    public class PlaylistService
    {
        public async Task<object> GetUserPlaylists(string accessToken)
        {
            var spotify = new SpotifyClient(accessToken);
            var playlists = await spotify.PaginateAll(await spotify.Playlists.CurrentUsers());

            return playlists.Select(p => new
            {
                Id = p.Id,
                Name = p.Name,
                TracksCount = p.Tracks?.Total ?? 0,
                ImageUrl = p.Images?.FirstOrDefault()?.Url
            }).ToList();
        }

        public async Task<object> GetPlaylistTracks(string accessToken, string playlistId)
        {
            var spotify = new SpotifyClient(accessToken);
            var tracks = await spotify.PaginateAll(await spotify.Playlists.GetItems(playlistId));

            return tracks
                .Where(t => t.Track is FullTrack)
                .Select(t =>
                {
                    var track = (FullTrack)t.Track;
                    return new
                    {
                        Id = track.Id,
                        Name = track.Name,
                        Artist = string.Join(", ", track.Artists.Select(a => a.Name)),
                        Album = track.Album.Name,
                        ImageUrl = track.Album.Images?.FirstOrDefault()?.Url
                    };
                }).ToList();
        }

        public async Task<object> ComparePlaylists(string accessToken, string playlist1Id, string playlist2Id)
        {
            var spotify = new SpotifyClient(accessToken);

            // Fetch tracks from both playlists
            var tracks1 = await spotify.PaginateAll(await spotify.Playlists.GetItems(playlist1Id));
            var tracks2 = await spotify.PaginateAll(await spotify.Playlists.GetItems(playlist2Id));

            // Process playlist 1
            var playlist1Tracks = tracks1
                .Where(t => t.Track is FullTrack)
                .Select(t => (FullTrack)t.Track)
                .ToList();

            var playlist1Data = new
            {
                TrackCount = playlist1Tracks.Count,
                TotalDurationMs = playlist1Tracks.Sum(t => t.DurationMs),
                TrackIds = playlist1Tracks.Select(t => t.Id).ToList(),
                ArtistIds = playlist1Tracks.SelectMany(t => t.Artists.Select(a => a.Id)).Distinct().ToList(),
            };

            // Process playlist 2
            var playlist2Tracks = tracks2
                .Where(t => t.Track is FullTrack)
                .Select(t => (FullTrack)t.Track)
                .ToList();

            var playlist2Data = new
            {
                TrackCount = playlist2Tracks.Count,
                TotalDurationMs = playlist2Tracks.Sum(t => t.DurationMs),
                TrackIds = playlist2Tracks.Select(t => t.Id).ToList(),
                ArtistIds = playlist2Tracks.SelectMany(t => t.Artists.Select(a => a.Id)).Distinct().ToList(),
            };

            // Calculate track and artist similarities
            var commonTrackIds = playlist1Data.TrackIds.Intersect(playlist2Data.TrackIds).ToList();
            var commonArtistIds = playlist1Data.ArtistIds.Intersect(playlist2Data.ArtistIds).ToList();

            var trackSimilarity = playlist1Data.TrackIds.Count + playlist2Data.TrackIds.Count > 0
                ? (2.0 * commonTrackIds.Count) / (playlist1Data.TrackIds.Count + playlist2Data.TrackIds.Count) * 100
                : 0;

            var artistSimilarity = playlist1Data.ArtistIds.Count + playlist2Data.ArtistIds.Count > 0
                ? (2.0 * commonArtistIds.Count) / (playlist1Data.ArtistIds.Count + playlist2Data.ArtistIds.Count) * 100
                : 0;

            // Fetch genres for all unique artists
            var allArtistIds = playlist1Data.ArtistIds.Union(playlist2Data.ArtistIds).Distinct().ToList();
            var playlist1Genres = new List<string>();
            var playlist2Genres = new List<string>();

            // Batch fetch artists (max 50 per request)
            for (int i = 0; i < allArtistIds.Count; i += 50)
            {
                var batch = allArtistIds.Skip(i).Take(50).ToList();
                var artistsResponse = await spotify.Artists.GetSeveral(new ArtistsRequest(batch));

                foreach (var artist in artistsResponse.Artists)
                {
                    if (artist == null) continue;

                    if (playlist1Data.ArtistIds.Contains(artist.Id))
                    {
                        playlist1Genres.AddRange(artist.Genres);
                    }
                    if (playlist2Data.ArtistIds.Contains(artist.Id))
                    {
                        playlist2Genres.AddRange(artist.Genres);
                    }
                }
            }

            var uniqueGenres1 = playlist1Genres.Distinct().ToList();
            var uniqueGenres2 = playlist2Genres.Distinct().ToList();
            var commonGenres = uniqueGenres1.Intersect(uniqueGenres2).ToList();

            var genreSimilarity = uniqueGenres1.Count + uniqueGenres2.Count > 0
                ? (2.0 * commonGenres.Count) / (uniqueGenres1.Count + uniqueGenres2.Count) * 100
                : 0;

            return new
            {
                Playlist1 = new
                {
                    playlist1Data.TrackCount,
                    playlist1Data.TotalDurationMs,
                    playlist1Data.TrackIds,
                    playlist1Data.ArtistIds,
                    Genres = uniqueGenres1
                },
                Playlist2 = new
                {
                    playlist2Data.TrackCount,
                    playlist2Data.TotalDurationMs,
                    playlist2Data.TrackIds,
                    playlist2Data.ArtistIds,
                    Genres = uniqueGenres2
                },
                CommonTrackCount = commonTrackIds.Count,
                CommonArtistCount = commonArtistIds.Count,
                CommonGenreCount = commonGenres.Count,
                TrackSimilarityPercent = Math.Round(trackSimilarity, 1),
                ArtistSimilarityPercent = Math.Round(artistSimilarity, 1),
                GenreSimilarityPercent = Math.Round(genreSimilarity, 1)
            };
        }
    }
}
