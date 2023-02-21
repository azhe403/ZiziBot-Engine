using Flurl.Http;
using Microsoft.AspNetCore.Builder;
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