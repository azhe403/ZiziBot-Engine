using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Flurl.Http;
using Flurl.Http.Configuration;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.AspNetCore.SignalR.Extensions;
using ZiziBot.Common.Utils;
using IHub = Serilog.Sinks.AspNetCore.SignalR.Interfaces.IHub;

namespace ZiziBot.Infrastructure;

public static class LoggingExtension
{
    // ReSharper disable InconsistentNaming
    private const string TEMPLATE_BASE = $"[{{Level:u3}}]{{MemoryUsage}}{{ThreadId}}{{Message:lj}}{{NewLine}}{{Exception}}";

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
                    return $" MEM {mem} ";
                }).Enrich.WithDynamicProperty("ThreadId", () => {
                    var threadId = Environment.CurrentManagedThreadId.ToString();
                    return $" Thread {threadId} ";
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

    public static IWebHostBuilder ConfigureSentry(this IWebHostBuilder hostBuilder)
    {
        if (Env.SentryDsn.IsNotNullOrWhiteSpace())
        {
            hostBuilder.UseSentry((context, options) => {
                options.Dsn = Env.SentryDsn;
                options.TracesSampleRate = 1.0;
                options.ProfilesSampleRate = 1.0;
                options.Release = VersionUtil.GetVersion();
                options.AddProfilingIntegration();
            });

            SentrySdk.Init(options => {
                options.Dsn = Env.SentryDsn;
                options.TracesSampleRate = 1.0;
                options.ProfilesSampleRate = 1.0;
                options.Release = VersionUtil.GetVersion();
                options.AddProfilingIntegration();
            });
        }

        return hostBuilder;
    }

    public static IServiceCollection AddSerilog(this IServiceCollection services, WebApplicationBuilder applicationBuilder)
    {
        services.AddSerilog((provider, config) => {
            using var scope = provider.CreateScope();

            var appSettingRepository = scope.ServiceProvider.GetRequiredService<AppSettingRepository>();
            var sinkConfig = appSettingRepository.GetTelegramSinkConfig();

            var logConfig = appSettingRepository.GetRequiredConfigSection<EventLogConfig>();

            config.ReadFrom
                .Configuration(applicationBuilder.Configuration)
                .ReadFrom.Services(provider)
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Debug()
                .WriteTo.Console(outputTemplate: OUTPUT_TEMPLATE)
                .Enrich.WithDemystifiedStackTraces();

            if (logConfig.ProcessEnrich)
            {
                config.Enrich.WithDynamicProperty("MemoryUsage", () => {
                    var mem = Process.GetCurrentProcess().PrivateMemorySize64.Bytes().ToString("0.00");
                    return $" {mem} ";
                }).Enrich.WithDynamicProperty("ThreadId", () => {
                    var threadId = Environment.CurrentManagedThreadId.ToString("000000").Replace("0", " ");
                    return $" {threadId} ";
                });
            }

            if (logConfig.WriteToFile)
            {
                config.WriteTo.Async(cfg => cfg.File($"{PathConst.LOG}/log-.log",
                    outputTemplate: OUTPUT_TEMPLATE,
                    rollingInterval: RollingInterval.Day,
                    flushToDiskInterval: TimeSpan.FromSeconds(2),
                    shared: true
                ));
            }

            if (logConfig.WriteToSignalR)
            {
                config.WriteTo.Async(cfg => cfg.SignalRSink<LogHub, IHub>(LogEventLevel.Debug, provider));
            }

            if (logConfig.WriteToTelegram)
            {
                config.WriteTo.Async(cfg => cfg.Telegram(sinkConfig.BotToken, sinkConfig.ChatId, sinkConfig.ThreadId));
            }

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
        });

        return services;
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

                    Log.Information("Flurl request {Method}: {Url}", request.Verb, request.Url);
                });

                builder.AfterCall(flurlCall => {
                    var request = flurlCall.Request;
                    var response = flurlCall.Response;

                    Log.Information("Flurl response {Method}: {Url}: {StatusCode}. Elapsed: {Elapsed}",
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