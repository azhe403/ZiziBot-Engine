using Flurl.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    // ReSharper disable InconsistentNaming
    private const string TEMPLATE_BASE = $"[{{Level:u3}}]{{MemoryUsage}}{{ThreadId}} {{Message:lj}}{{NewLine}}{{Exception}}";

    private const string OUTPUT_TEMPLATE = $"{{Timestamp:HH:mm:ss.fff}} {TEMPLATE_BASE}";
    // ReSharper restore InconsistentNaming

    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder, bool fullMode = false)
    {
        hostBuilder.UseSerilog((context, provider, config) => {
            config.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .MinimumLevel.Debug()
                .Enrich.WithDemystifiedStackTraces();

            config.WriteTo.Async(configuration => configuration.Console(outputTemplate: OUTPUT_TEMPLATE));

            if (!fullMode)
                return;

            var appSettingRepository = provider.GetRequiredService<AppSettingRepository>();
            var sinkConfig = appSettingRepository.GetTelegramSinkConfig();

            config.WriteTo.Async(configuration => configuration.Telegram(sinkConfig.BotToken, sinkConfig.ChatId, sinkConfig.ThreadId));
        });

        return hostBuilder;
    }

    public static IApplicationBuilder ConfigureFlurlLogging(this IApplicationBuilder app)
    {
        FlurlHttp.Configure(
            settings => {
                settings.BeforeCall = flurlCall => {
                    var request = flurlCall.Request;
                    Log.Information("FlurlHttp: {Method} {Url}", request.Verb, request.Url);
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