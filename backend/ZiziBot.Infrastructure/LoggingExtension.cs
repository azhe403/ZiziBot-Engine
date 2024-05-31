using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AspNetCore.SignalR.Extensions;
using IHub = Serilog.Sinks.AspNetCore.SignalR.Interfaces.IHub;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    // ReSharper disable InconsistentNaming
    private const string TEMPLATE_BASE =
        $"[{{Level:u3}}] {{MemoryUsage}} {{ThreadId}} {{Message:lj}}{{NewLine}}{{Exception}}";

    private const string OUTPUT_TEMPLATE = $"{{Timestamp:HH:mm:ss.fff}} {TEMPLATE_BASE}";
    // ReSharper restore InconsistentNaming

    public static IHostBuilder ConfigureSerilogLite(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseSerilog((context, provider, config) => {
            config.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
                .Enrich.WithDemystifiedStackTraces();

            config.Enrich.WithDynamicProperty("MemoryUsage", () => {
                var mem = Process.GetCurrentProcess().PrivateMemorySize64.Bytes().ToString("0.00");
                return $"{mem}";
            }).Enrich.WithDynamicProperty("ThreadId", () => {
                var threadId = Environment.CurrentManagedThreadId.ToString();
                return $"{threadId}";
            });
        });

        return hostBuilder;
    }

    public static IHostBuilder ConfigureSerilog(this IHostBuilder hostBuilder, bool fullMode = false)
    {
        hostBuilder.UseSerilog((context, provider, config) => {
            var appSettingRepository = provider.GetRequiredService<AppSettingRepository>();
            var sinkConfig = appSettingRepository.GetTelegramSinkConfig();

            var logConfig = appSettingRepository.GetRequiredConfigSection<LogConfig>();

            config.ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(provider)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Debug()
                .Enrich.WithDemystifiedStackTraces();

            if (logConfig.ProcessEnrich)
            {
                config.Enrich.WithDynamicProperty("MemoryUsage", () => {
                    var mem = Process.GetCurrentProcess().PrivateMemorySize64.Bytes().ToString("0.00");
                    return $"{mem}";
                }).Enrich.WithDynamicProperty("ThreadId", () => {
                    var threadId = Environment.CurrentManagedThreadId.ToString();
                    return $"{threadId}";
                });
            }

            config.WriteTo.Async(cfg => cfg
                .Console(outputTemplate: OUTPUT_TEMPLATE)
                .WriteTo.SignalRSink<LogHub, IHub>(LogEventLevel.Debug, provider));

            if (!fullMode)
                return;

            var sentryConfig = appSettingRepository.GetConfigSection<SentryConfig>();

            if (sentryConfig?.IsEnabled ?? false)
            {
                config.WriteTo.Async(cfg => {
                    cfg.Sentry(options => {
                        options.Dsn = sentryConfig.Dsn;
                        options.StackTraceMode = StackTraceMode.Enhanced;
                        options.Release = VersionUtil.GetVersion();
                    });
                });
            }

            config.WriteTo.Async(configuration =>
                configuration.Telegram(sinkConfig.BotToken, sinkConfig.ChatId, sinkConfig.ThreadId));
        });

        return hostBuilder;
    }

    public static IApplicationBuilder ConfigureFlurl(this IApplicationBuilder app)
    {
        FlurlHttp.Clients.WithDefaults(builder => {
                builder.Settings.JsonSerializer = new DefaultJsonSerializer(new JsonSerializerOptions() {
                    NumberHandling = JsonNumberHandling.AllowReadingFromString
                });

                builder.BeforeCall(call => {
                    var request = call.Request;
                    call.Request.Headers.Add("User-Agent", Env.COMMON_UA);

                    Log.Information("FlurlHttp: {Method} {Url}", request.Verb, request.Url);
                });

                builder.AfterCall(flurlCall => {
                    var request = flurlCall.Request;
                    var response = flurlCall.Response;

                    Log.Information("FlurlHttp: {Method} {Url}: {StatusCode}. Elapsed: {Elapsed}",
                        request.Verb,
                        request.Url,
                        response?.StatusCode,
                        flurlCall.Duration
                    );
                });
            }
        );

        return app;
    }
}