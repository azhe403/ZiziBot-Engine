using Microsoft.Extensions.Hosting;
using Serilog;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    public static IHostBuilder InitSerilogBootstrapper(this IHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        hostBuilder.UseSerilog();

        return hostBuilder;
    }
}