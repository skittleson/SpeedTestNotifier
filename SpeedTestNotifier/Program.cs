using CommandLine;
using CommunityToolkit.WinUI.Notifications;
using SpeedTestNotifier;
using System.Text.Json;
using static SimpleExec.Command;

Console.WriteLine("Speed Test Notifier for Windows 10!");
var argsOptions = Parser.Default.ParseArguments<ArgsOptions>(args);

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
        var downloadMin = 30u;
        argsOptions.WithParsed<ArgsOptions>(o => {
            if (o.DownloadMin.HasValue) {
                downloadMin = o.DownloadMin.Value;
            }
        });
        var toast = new ToastContentBuilder()
            .AddHeader("speedtest-cli", "Speed Test", "")
            .AddText($"{downloadMbps:F2}Mbps Download")
            .SetToastDuration(downloadMbps < downloadMin ? ToastDuration.Long : ToastDuration.Short);

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



public class ArgsOptions
{

    [Option('d', "download-min", Required = false, HelpText = "Set Download Minimal Threshold")]
    public uint? DownloadMin { get; set; }
}


