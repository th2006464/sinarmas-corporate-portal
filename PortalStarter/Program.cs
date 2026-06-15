using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

await RunAsync();

static async Task RunAsync()
{
    var exeDir = AppContext.BaseDirectory;
    var portalPath = Path.Combine(exeDir, "Portal.exe");
    if (!File.Exists(portalPath))
    {
        Console.WriteLine("Portal.exe not found in the same folder.");
        return;
    }

    var psi = new ProcessStartInfo(portalPath)
    {
        UseShellExecute = false,
        WorkingDirectory = exeDir
    };
    psi.Environment["PORT"] = "5000";

    var proc = Process.Start(psi);
    Console.WriteLine($"Started Portal.exe (PID {proc?.Id}). Waiting for server at http://localhost:5000/");

    using var http = new HttpClient();
    var url = "http://localhost:5000/";
    var up = false;
    for (int i = 0; i < 60; i++)
    {
        try
        {
            var r = await http.GetAsync(url);
            if (r.IsSuccessStatusCode)
            {
                up = true; break;
            }
        }
        catch { }
        await Task.Delay(1000);
    }

    if (up)
    {
        Console.WriteLine("Server is up. Opening browser...");
        Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
    }
    else
    {
        Console.WriteLine("Server did not respond within timeout. You can open http://localhost:5000 manually.");
    }

    if (proc != null)
    {
        Console.WriteLine("Press Ctrl+C to stop. Waiting for Portal.exe to exit...");
        proc.WaitForExit();
    }
}
