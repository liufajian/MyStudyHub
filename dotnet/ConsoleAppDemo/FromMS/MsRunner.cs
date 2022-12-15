using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

namespace ConsoleAppDemo.FromMS
{
    internal class MsRunner
    {
        public static async Task Run(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .UseWindowsService(options =>
                {
                    options.ServiceName = ".NET Joke Service";
                })
                .ConfigureServices(services =>
                {
                    LoggerProviderOptions.RegisterProviderOptions<
                        EventLogSettings, EventLogLoggerProvider>(services);

                    services.AddSingleton<JokeService>();
                    services.AddHostedService<WindowsBackgroundService>();
                })
                .ConfigureLogging((context, logging) =>
                {
                    // See: https://github.com/dotnet/runtime/issues/47303
                    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
                })
                .Build();

            await host.RunAsync();
        }
    }
}
