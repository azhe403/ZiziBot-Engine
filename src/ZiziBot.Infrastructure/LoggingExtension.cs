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
                    var url = request.Url;
                    var method = request.Verb;
                    Log.Information("FlurlHttp: {Method} {url}", method, url);
                };

                settings.AfterCall = flurlCall => {
                    var request = flurlCall.Request;
                    var response = flurlCall.Response;
                    var statusCode = response.StatusCode;
                    Log.Information(
                        "FlurlHttp: {Method} {Url} {StatusCode}. Elapsed: {Elapsed}",
                        request.Verb,
                        request.Url,
                        statusCode,
                        flurlCall.Duration
                    );
                };
            }
        );

        return app;
    }
}