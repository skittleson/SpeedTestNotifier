using CommunityToolkit.WinUI.Notifications;
using SpeedTestNotifier;
using System.Text.Json;
using static SimpleExec.Command;

Console.WriteLine("Speed Test Notifier for Windows 10!");
var (standardOutput1, standardError1) = await ReadAsync("speedtest-cli", "--json --no-upload");
if (!string.IsNullOrEmpty(standardOutput1))
{
    var options = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    var speedTestResponse = JsonSerializer.Deserialize<SpeedTestResponse>(standardOutput1, options);
    var downloadBytes = speedTestResponse?.Download;
    if (downloadBytes > 0)
    {
        var downloadMbps = downloadBytes / 1024 / 1024;
        Console.WriteLine($"{downloadMbps}Mbps");
        var toast = new ToastContentBuilder()
            .AddHeader("speedtest-cli", "Speed Test", "")
            .AddText($"{downloadMbps:F2}Mbps Download")
            .SetToastDuration(downloadMbps < 30 ? ToastDuration.Long : ToastDuration.Short);

        toast.Show();

        // give some time to send it to the windows api
        Thread.Sleep(TimeSpan.FromSeconds(3));
    }
    Environment.Exit(0);
}
else
{
    Console.WriteLine("Failed to run speed test.  Install `pip install speedtest-cli`");
    Console.WriteLine(standardError1);
    Thread.Sleep(TimeSpan.FromSeconds(3));
    Environment.Exit(1);
}


