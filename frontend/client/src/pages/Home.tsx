import { useEffect, useRef } from "react";
import "../App.css";

const API_URL = import.meta.env.VITE_API_URL || "http://127.0.0.1:5000";

function Home() {
  const exchangingCode = useRef(false);

  useEffect(() => {
    // Handle OAuth callback
    const urlParams = new URLSearchParams(window.location.search);
    const code = urlParams.get("code");
    const error = urlParams.get("error");

    if (error) {
      console.error("OAuth error:", error);
      alert(`Authentication failed: ${error}`);
      // Clean URL
      window.history.replaceState({}, document.title, window.location.pathname);
      return;
    }

    if (code && !exchangingCode.current) {
      exchangingCode.current = true;
      // Exchange code for tokens
      fetch(`${API_URL}/api/auth/token`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ code }),
      })
        .then((res) => res.json())
        .then((data) => {
          if (data.success) {
            window.location.href = "/dashboard";
          } else {
            alert(`Token exchange failed: ${data.error}`);
          }
        })
        .catch((err) => {
          console.error("Token exchange error:", err);
          alert("Failed to complete authentication");
        })
        .finally(() => {
          // Clean URL
          window.history.replaceState(
            {},
            document.title,
            window.location.pathname
          );
        });
    }
  }, []);

  const handleLogin = async () => {
    try {
      const response = await fetch(`${API_URL}/api/auth/login`);
      const data = await response.json();

      // Build Spotify authorization URL
      const authUrl = new URL("https://accounts.spotify.com/authorize");
      authUrl.searchParams.append("client_id", data.clientId);
      authUrl.searchParams.append("response_type", "code");
      authUrl.searchParams.append("redirect_uri", data.redirectUri);
      authUrl.searchParams.append("scope", data.scopes.join(" "));

      window.location.href = authUrl.toString();
    } catch (error) {
      console.error("Login failed:", error);
    }
  };

  return (
    <div className="container">
      <h1 className="title">Welcome to SpotifyStats</h1>
      <button className="login-button" onClick={handleLogin}>
        Login with Spotify
      </button>
    </div>
  );
}

export default Home;
