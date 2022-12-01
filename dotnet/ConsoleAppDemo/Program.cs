using ConsoleAppDemo;
using Microsoft.Extensions.Logging;

internal class Program
{
    static async Task Main(string[] args)
    {
        //.NET Core 3 introduced LoggerFactory.Create that can be useful for application startup logging,
        // before the dependency injection system is fully initialized
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("StartupLogger");
        logger.LogInformation("Starting...");

        if (args != null)
        {
            for (var i = 0; i < args.Length; i++)
            {
                logger.LogInformation("启动参数{index}:{value}", i + 1, args[i]);
            }
        }

        var runMode = Environment.GetEnvironmentVariable("DemoRunMode");

        if (runMode == "simple")
        {
            SimpleRunner.Run();
        }
        else
        {
            await HostingRunner.Run(args);
        }
    }
}
