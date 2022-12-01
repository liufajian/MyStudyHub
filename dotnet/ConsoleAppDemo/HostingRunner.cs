using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using System.Net;

namespace ConsoleAppDemo
{
    public static class HostingRunner
    {
        static IServiceProvider ServiceProvider;

        public static T GetService<T>()
        {
            return ServiceProvider.GetService<T>();
        }

        public static async Task Run(string[] args)
        {
            var config = new ConfigurationBuilder().Build();

            var logger = LogManager.Setup()
                                   .SetupExtensions(ext => ext.RegisterConfigSettings(config))
                                   .GetCurrentClassLogger();

            try
            {
                var host = Host.CreateDefaultBuilder(args)
                    .ConfigureLogging(builder => builder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace))
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHttpClient("")
                              .ConfigurePrimaryHttpMessageHandler(messageHandler =>
                              {
                                  var handler = new HttpClientHandler();
                                  if (handler.SupportsAutomaticDecompression)
                                  {
                                      handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                                  }
                                  return handler;
                              });
                        services.AddHostedService<TestWorker1>();
                        services.AddHostedService<ConsoleHostedService>();
                    })
                    .UseNLog()
                    .UseConsoleLifetime()
                    .Build();

                ServiceProvider = host.Services;

                // Build and run the host in one go; .RCA is specialized for running it in a console.
                // It registers SIGTERM(Ctrl-C) to the CancellationTokenSource that's shared with all services in the container.
                await host.RunAsync();

                Console.WriteLine("The host container has terminated. Press ANY key to exit the console.");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                // NLog: catch setup errors (exceptions thrown inside of any containers may not necessarily be caught)
                logger.Fatal(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }

        public class ConsoleHostedService : BackgroundService
        {
            private readonly ILogger<ConsoleHostedService> _logger;

            public ConsoleHostedService(ILogger<ConsoleHostedService> logger)
            {
                _logger = logger;
                _logger.LogInformation("ConsoleHostedService instance created...");
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                _logger.LogInformation("Hello from your hosted service thread!");
                _logger.LogTrace("I may or may not return for a long time depending on what I do.");
                _logger.LogDebug("In this example, I return right away, but my host will continue to run until");
                _logger.LogInformation("its CancellationToken is Cancelled (SIGTERM(Ctrl-C) or a Lifetime Event )");
                await Task.CompletedTask;
            }
        }

        class TestWorker1 : BackgroundService
        {
            private readonly ILogger<TestWorker1> _logger;
            private HttpClient httpClient;

            public TestWorker1(ILogger<TestWorker1> logger)
            {
                _logger = logger;
            }

            public override Task StartAsync(CancellationToken cancellationToken)
            {
                httpClient = GetService<IHttpClientFactory>().CreateClient();

                return base.StartAsync(cancellationToken);
            }

            public override Task StopAsync(CancellationToken cancellationToken)
            {
                httpClient.Dispose();
                return base.StopAsync(cancellationToken);
            }

            protected override async Task ExecuteAsync(CancellationToken stoppingToken)
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var result = await httpClient.GetAsync("https://www.c-sharpcorner.com/members/atul-warade");
                        if (result.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("The Website is Up.Status Code {StatusCode}", result.StatusCode);
                        }
                        else
                        {
                            _logger.LogError("The Website is Down.Status Code {StatusCode}", result.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("The Website is Down {0}.", ex.Message);
                    }
                    finally
                    {
                        await Task.Delay(1000 * 5, stoppingToken);
                    }

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                }
            }
        }
    }
}