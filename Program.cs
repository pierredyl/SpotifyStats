using SpotifyListeningTracker.Models;
using SpotifyListeningTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

// Register services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<PlaylistService>();
builder.Services.AddScoped<TrackService>();
builder.Services.AddScoped<ArtistService>();

//Configure Spotify settings
builder.Services.Configure<SpotifySettings>(builder.Configuration.GetSection("Spotify"));

//CORS
var frontendUrl = Environment.GetEnvironmentVariable("frontendUrl")
    ?? builder.Configuration["frontendUrl"]
    ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
        // policy.WithOrigins(frontendUrl, "http://localhost:5173", "http://127.0.0.1:5173")
        //       .AllowAnyHeader()
        //       .AllowAnyMethod()
        //       .AllowCredentials();
    });
});

// Configure port
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
if (builder.Environment.IsDevelopment())
{
    builder.WebHost.UseUrls("http://127.0.0.1:5000");
}
else
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

