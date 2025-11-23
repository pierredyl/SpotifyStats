import { useEffect, useState } from "react";
import "../App.css";

interface User {
  id: string;
  displayName: string;
  email: string;
  imageUrl: string;
  followers: number;
}

interface Track {
  id: string;
  name: string;
  artist: string;
  album: string;
  durationMs: number;
  imageUrl: string;
}

interface Artist {
  id: string;
  name: string;
  genres: string[];
  imageUrl: string;
}

interface Playlist {
  id: string;
  name: string;
  tracksCount: number;
  imageUrl: string;
}

interface ComparisonResult {
  playlist1: {
    trackCount: number;
    totalDurationMs: number;
    trackIds: string[];
    artistIds: string[];
    genres: string[];
  };
  playlist2: {
    trackCount: number;
    totalDurationMs: number;
    trackIds: string[];
    artistIds: string[];
    genres: string[];
  };
  commonTrackCount: number;
  commonArtistCount: number;
  commonGenreCount: number;
  trackSimilarityPercent: number;
  artistSimilarityPercent: number;
  genreSimilarityPercent: number;
}

const API_URL = import.meta.env.VITE_API_URL || "http://127.0.0.1:5000";

function Dashboard() {
  const [user, setUser] = useState<User | null>(null);
  const [topTracks, setTopTracks] = useState<Track[]>([]);
  const [topArtists, setTopArtists] = useState<Artist[]>([]);
  const [loading, setLoading] = useState(true);
  const [visibleTracks, setVisibleTracks] = useState(5);
  const [visibleArtists, setVisibleArtists] = useState(5);
  const [playlists, setPlaylists] = useState<Playlist[]>([]);
  const [selectedPlaylist, setSelectedPlaylist] = useState("");
  const [comparePlaylistUrl, setComparePlaylistUrl] = useState("");
  const [comparisonResult, setComparisonResult] = useState<ComparisonResult | null>(null);
  const [comparing, setComparing] = useState(false);

  const extractPlaylistId = (url: string): string | null => {
    const match = url.match(/playlist\/([a-zA-Z0-9]+)/);
    return match ? match[1] : null;
  };

  const formatDuration = (ms: number): string => {
    const hours = Math.floor(ms / 3600000);
    const minutes = Math.floor((ms % 3600000) / 60000);
    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    }
    return `${minutes}m`;
  };

  const handleCompare = async () => {
    const playlist2Id = extractPlaylistId(comparePlaylistUrl);
    if (!playlist2Id) {
      alert("Invalid playlist URL");
      return;
    }

    setComparing(true);
    try {
      const res = await fetch(
        `${API_URL}/api/playlist/compare?playlist1Id=${selectedPlaylist}&playlist2Id=${playlist2Id}`,
        { credentials: "include" }
      );
      if (!res.ok) throw new Error("Failed to compare playlists");
      const data = await res.json();
      setComparisonResult(data);
    } catch (error) {
      console.error("Comparison error:", error);
      alert("Failed to compare playlists");
    } finally {
      setComparing(false);
    }
  };

  useEffect(() => {
    // Fetch user data (cookies sent automatically)
    fetch(`${API_URL}/api/user/me`, {
      credentials: "include",
    })
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch user");
        return res.json();
      })
      .then((data) => {
        setUser(data);
        // Fetch top tracks
        return fetch(`${API_URL}/api/track/top`, {
          credentials: "include",
        });
      })
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch top tracks");
        return res.json();
      })
      .then((data) => {
        setTopTracks(data || []);
        // Fetch top artists
        return fetch(`${API_URL}/api/artist/top`, {
          credentials: "include",
        });
      })
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch top artists");
        return res.json();
      })
      .then((data) => {
        setTopArtists(data || []);
        // Fetch playlists
        return fetch(`${API_URL}/api/playlist`, {
          credentials: "include",
        });
      })
      .then((res) => {
        if (!res.ok) throw new Error("Failed to fetch playlists");
        return res.json();
      })
      .then((data) => {
        setPlaylists(data || []);
        setLoading(false);
      })
      .catch((error) => {
        console.error("Error:", error);
        // Redirect to login if auth fails
        if (error.message.includes("user")) {
          window.location.href = "/";
        } else {
          setLoading(false);
        }
      });
  }, []);

  if (loading) {
    return (
      <div className="container">
        <p className="loading-text">Loading...</p>
      </div>
    );
  }

  return (
    <div className="container dashboard">
      <div className="profile-card">
        {user?.imageUrl && (
          <img src={user.imageUrl} alt="Profile" className="profile-image" />
        )}
        <div className="profile-info">
          <h1 className="profile-name">{user?.displayName}</h1>
          <h2 className="profile-name">Followers: {user?.followers}</h2>
        </div>
      </div>

      <div className="dashboard-content">
        <div className="left-section">
          <div className="top-tracks-section">
            <h2 className="section-title">Top Tracks (6 months)</h2>
            <div className="tracks-list">
              {topTracks.slice(0, visibleTracks).map((track, index) => (
                <div key={track.id} className="track-item">
                  <span className="track-rank">{index + 1}</span>
                  <img
                    src={track.imageUrl}
                    alt={track.name}
                    className="track-image"
                  />
                  <div className="track-info">
                    <p className="track-name">{track.name}</p>
                    <p className="track-artist">{track.artist}</p>
                  </div>
                </div>
              ))}
            </div>
            {visibleTracks < topTracks.length && (
              <button
                className="show-more-button"
                onClick={() => setVisibleTracks((prev) => prev + 10)}
              >
                Show More
              </button>
            )}
          </div>

          <div className="top-artists-section">
            <h2 className="section-title">Top Artists (6 months)</h2>
            <div className="tracks-list">
              {topArtists.slice(0, visibleArtists).map((artist, index) => (
                <div key={artist.id} className="track-item">
                  <span className="track-rank">{index + 1}</span>
                  <img
                    src={artist.imageUrl}
                    alt={artist.name}
                    className="track-image"
                  />
                  <div className="track-info">
                    <p className="track-name">{artist.name}</p>
                    <p className="track-artist">{artist.genres.slice(0, 3).join(", ")}</p>
                  </div>
                </div>
              ))}
            </div>
            {visibleArtists < topArtists.length && (
              <button
                className="show-more-button"
                onClick={() => setVisibleArtists((prev) => prev + 10)}
              >
                Show More
              </button>
            )}
          </div>
        </div>

        <div className="right-section">
          <div className="comparison-tool">
            <h2 className="section-title">Playlist Comparison</h2>

            <div className="comparison-input-group">
              <label className="comparison-label">Your Playlist</label>
              <select
                className="comparison-select"
                value={selectedPlaylist}
                onChange={(e) => setSelectedPlaylist(e.target.value)}
              >
                <option value="">Select a playlist</option>
                {playlists.map((playlist) => (
                  <option key={playlist.id} value={playlist.id}>
                    {playlist.name} ({playlist.tracksCount} tracks)
                  </option>
                ))}
              </select>
            </div>

            <div className="comparison-input-group">
              <label className="comparison-label">Compare with</label>
              <input
                type="text"
                className="comparison-input"
                placeholder="Paste Spotify playlist link..."
                value={comparePlaylistUrl}
                onChange={(e) => setComparePlaylistUrl(e.target.value)}
              />
            </div>

            <button
              className="compare-button"
              disabled={!selectedPlaylist || !comparePlaylistUrl || comparing}
              onClick={handleCompare}
            >
              {comparing ? "Comparing..." : "Compare Playlists"}
            </button>

            {comparisonResult && (
              <div className="comparison-results">
                <h3 className="results-title">Results</h3>

                <div className="results-grid">
                  <div className="result-item">
                    <span className="result-label">Your Playlist</span>
                    <span className="result-value">{comparisonResult.playlist1.trackCount} tracks</span>
                    <span className="result-sub">{formatDuration(comparisonResult.playlist1.totalDurationMs)}</span>
                  </div>

                  <div className="result-item">
                    <span className="result-label">Compare Playlist</span>
                    <span className="result-value">{comparisonResult.playlist2.trackCount} tracks</span>
                    <span className="result-sub">{formatDuration(comparisonResult.playlist2.totalDurationMs)}</span>
                  </div>
                </div>

                <div className="similarity-section">
                  <div className="similarity-item">
                    <span className="similarity-label">Track Similarity</span>
                    <span className="similarity-value">{comparisonResult.trackSimilarityPercent}%</span>
                    <span className="similarity-detail">{comparisonResult.commonTrackCount} common tracks</span>
                  </div>

                  <div className="similarity-item">
                    <span className="similarity-label">Artist Similarity</span>
                    <span className="similarity-value">{comparisonResult.artistSimilarityPercent}%</span>
                    <span className="similarity-detail">{comparisonResult.commonArtistCount} common artists</span>
                  </div>

                  <div className="similarity-item">
                    <span className="similarity-label">Genre Similarity</span>
                    <span className="similarity-value">{comparisonResult.genreSimilarityPercent}%</span>
                    <span className="similarity-detail">{comparisonResult.commonGenreCount} common genres</span>
                  </div>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}

export default Dashboard;
