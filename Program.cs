using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Portal.Services;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddSingleton<NewsService>();

// Cookie authentication for admin
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/admin/login";
        options.LogoutPath = "/admin/logout";
        options.AccessDeniedPath = "/admin/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Support sub-path deployment (e.g., /portalwebsite)
var pathBase = Environment.GetEnvironmentVariable("PATH_BASE") ?? "";
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Determine the listen URL
var portEnv = Environment.GetEnvironmentVariable("PORT");
int port = 5000;
if (!string.IsNullOrEmpty(portEnv) && int.TryParse(portEnv, out var parsedPort))
{
    port = parsedPort;
    app.Urls.Clear();
    app.Urls.Add($"http://*:{port}");
}
else
{
    if (app.Environment.IsDevelopment())
    {
        app.Urls.Clear();
        app.Urls.Add($"http://localhost:{port}");
    }
}

// Auto-open browser when server is ready
var url = $"http://localhost:{port}";
app.Lifetime.ApplicationStarted.Register(() =>
{
    // Delay to ensure server is ready
    Task.Run(async () =>
    {
        await Task.Delay(1000);
        try
        {
            using var proc = Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
            // Don't wait - let the browser run independently
        }
        catch { }
    });
});

// Ensure clean shutdown
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    app.StopAsync().Wait();
    Environment.Exit(0);
};

app.Run();
