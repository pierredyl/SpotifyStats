import "../App.css";

const API_URL = import.meta.env.VITE_API_URL || "http://127.0.0.1:5000";

function Home() {
  const handleLogin = async () => {
    try {
      const response = await fetch(`${API_URL}/api/auth/login`);
      const data = await response.json();
      window.location.href = data.authUrl;
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
