using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    private const string TEMPLATE_BASE = $"[{{Level:u3}}]{{MemoryUsage}}{{ThreadId}} {{Message:lj}}{{NewLine}}{{Exception}}";
    private const string OUTPUT_TEMPLATE = $"{{Timestamp:HH:mm:ss.fff}} {TEMPLATE_BASE}";

    public static IHostBuilder InitSerilogBootstrapper(this IHostBuilder hostBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithDemystifiedStackTraces()
            .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
            .CreateBootstrapLogger();

        hostBuilder.UseSerilog();

        return hostBuilder;
    }

    public static IApplicationBuilder ConfigureFlurlLogging(this IApplicationBuilder app)
    {
        FlurlHttp.Configure(
            settings => {
                settings.BeforeCall = flurlCall => {
                    var request = flurlCall.Request;
                    Log.Information("FlurlHttp: {Method} {url}", request.Verb, request.Url);
                };

                settings.AfterCall = flurlCall => {
                    var request = flurlCall.Request;
                    var response = flurlCall.Response;
                    Log.Information(
                        "FlurlHttp: {Method} {Url} {StatusCode}. Elapsed: {Elapsed}",
                        request.Verb,
                        request.Url,
                        response?.StatusCode,
                        flurlCall.Duration
                    );
                };
            }
        );

        return app;
    }
}