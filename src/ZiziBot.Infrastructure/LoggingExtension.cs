using Flurl.Http;
using HelpMate.Core.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    private const string TEMPLATE_BASE = $"[{{Level:u3}}]{{MemoryUsage}}{{ThreadId}} {{Message:lj}}{{NewLine}}{{Exception}}";
    private const string OUTPUT_TEMPLATE = $"{{Timestamp:HH:mm:ss.fff}} {TEMPLATE_BASE}";

    public static IHostBuilder InitSerilogBootstrapper(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, provider, config) =>
        {
            var appDbContext = provider.GetRequiredService<AppSettingsDbContext>();

            var appSettings = appDbContext.AppSettings.FirstOrDefault(entity => entity.Name == "EventLog:ChatId");
            var botToken = appDbContext.BotSettings.FirstOrDefault(entity => entity.Name == "Main");

            config
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .MinimumLevel.Debug()
                .Enrich.WithDemystifiedStackTraces()
                .WriteTo.Async(configuration => configuration
                    .Console(outputTemplate: OUTPUT_TEMPLATE)
                    .WriteTo.Sink(logEventSink: new TelegramSink()
                    {
                        BotToken = botToken?.Token,
                        ChatId = appSettings.Value.ToInt64()
                    }, LogEventLevel.Warning)
                );
        });

        return hostBuilder;
    }

    public static IApplicationBuilder ConfigureFlurlLogging(this IApplicationBuilder app)
    {
        FlurlHttp.Configure(
            settings =>
            {
                settings.BeforeCall = flurlCall =>
                {
                    var request = flurlCall.Request;
                    Log.Information("FlurlHttp: {Method} {url}", request.Verb, request.Url);
                };

                settings.AfterCall = flurlCall =>
                {
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