import "../App.css";

function Unauthorized() {
  return (
    <div className="container">
      <h1 className="title">Unauthorized</h1>
      <p style={{ textAlign: "center", marginBottom: "20px" }}>
        Your session has expired or authentication failed.
      </p>
      <button
        className="login-button"
        onClick={() => window.location.href = "/"}
      >
        Return to Login
      </button>
    </div>
  );
}

export default Unauthorized;
