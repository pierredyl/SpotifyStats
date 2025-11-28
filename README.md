# SpotifyStats - Spotify Listening Tracker

A full-stack web application that tracks and analyzes your Spotify listening habits with detailed statistics and playlist comparison tools.

![Tech Stack](https://img.shields.io/badge/ASP.NET%20Core-10.0-blue)
![React](https://img.shields.io/badge/React-19-61dafb)
![TypeScript](https://img.shields.io/badge/TypeScript-5.6-blue)

## Features

- 🎵 **Top Tracks & Artists** - View your most listened to tracks and artists (6-month period)
- 📊 **Playlist Comparison** - Compare any two playlists by tracks, artists, and genres
- 🔐 **Secure OAuth Authentication** - Industry-standard Spotify OAuth 2.0 flow
- 📱 **Responsive Design** - Works seamlessly on desktop and mobile
- ⚡ **Real-time Statistics** - Live data from your Spotify account

## Tech Stack

### Backend
- **ASP.NET Core 10.0** - Web API framework
- **SpotifyAPI.Web** - Official Spotify Web API wrapper
- **C# 13** - Modern language features

### Frontend
- **React 19** - UI framework
- **TypeScript 5.6** - Type-safe JavaScript
- **Vite** - Lightning-fast build tool
- **React Router** - Client-side routing

### Deployment
- **Backend**: Azure App Service
- **Frontend**: Vercel
- **Proxy Setup**: Vercel rewrites for cross-origin cookie support

## Project Structure

```
SpotifyListeningTracker/
├── Controllers/           # API endpoints
│   ├── AuthController.cs       # OAuth authentication
│   ├── UserController.cs       # User profile endpoints
│   ├── TrackController.cs      # Track statistics
│   ├── ArtistController.cs     # Artist statistics
│   └── PlaylistController.cs   # Playlist operations
├── Services/             # Business logic layer
│   ├── UserService.cs
│   ├── TrackService.cs
│   ├── ArtistService.cs
│   └── PlaylistService.cs
├── Models/               # Data models
│   ├── SpotifySettings.cs
│   └── SpotifyUser.cs
├── frontend/client/      # React application
│   ├── src/
│   │   ├── pages/        # Route components
│   │   │   ├── Home.tsx
│   │   │   ├── Dashboard.tsx
│   │   │   └── Unauthorized.tsx
│   │   ├── App.tsx       # Root component
│   │   └── main.tsx      # Entry point
│   ├── public/
│   ├── vite.config.ts    # Vite configuration
│   ├── vercel.json       # Vercel deployment config
│   └── package.json
├── Program.cs            # Application startup
└── appsettings.json      # Configuration
```

## Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/auth/login` | Get OAuth configuration |
| POST | `/api/auth/token` | Exchange authorization code for tokens |
| GET | `/api/user/me` | Get current user profile |
| GET | `/api/track/top` | Get top tracks (6 months) |
| GET | `/api/artist/top` | Get top artists (6 months) |
| GET | `/api/playlist` | Get user's playlists |
| GET | `/api/playlist/compare` | Compare two playlists |

## Authentication Flow

This application uses **OAuth 2.0 Authorization Code Flow**:

1. User clicks "Login with Spotify"
2. Frontend requests OAuth configuration from backend
3. Frontend redirects to Spotify authorization page
4. User authorizes the application
5. Spotify redirects back to frontend with authorization code
6. Frontend exchanges code for access token via backend
7. Backend sets HTTP-only cookies (access_token, refresh_token)
8. User is redirected to dashboard
9. Dashboard makes authenticated API requests (cookies sent automatically)

### Security Features

- ✅ **HTTP-only cookies** - Tokens not accessible to JavaScript (XSS protection)
- ✅ **Secure flag in production** - Cookies only sent over HTTPS
- ✅ **SameSite configuration** - CSRF protection
- ✅ **CORS properly configured** - Only allowed origins can make requests
- ✅ **No credentials in frontend code** - All secrets server-side

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- [Spotify Web API](https://developer.spotify.com/documentation/web-api/) - For providing access to Spotify data
- [SpotifyAPI.Web](https://github.com/JohnnyCrazy/SpotifyAPI-NET) - Excellent C# wrapper for Spotify API
- [React](https://react.dev/) - Frontend framework
- [Vite](https://vitejs.dev/) - Build tool

---

**Built with ❤️ using Spotify Web API**
