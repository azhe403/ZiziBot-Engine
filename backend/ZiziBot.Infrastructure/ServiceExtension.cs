﻿using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using CloudCraic.Hosting.BackgroundQueue.DependencyInjection;
using Flurl.Http;
using Flurl.Http.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using ZiziBot.Application.Facades;
using ZiziBot.Database.Extension;
using ZiziBot.Services.Rest;

namespace ZiziBot.Infrastructure;

public static class ServiceExtension
{
    public static IServiceProvider GlobalServiceProvider { get; set; }

    public static async Task<IServiceCollection> ConfigureServices(this IServiceCollection services)
    {
        services.AddAllService();
        await services.ConfigureSettings();
        services.AddCacheTower();
        services.AddMongoMigration();
        await services.PrefetchRepository();
        services.AddMediator();
        services.AddBackgroundQueue();
        services.ConfigureFlurl();

        return services;
    }

    private static IServiceCollection ConfigureFlurl(this IServiceCollection services)
    {
        services.AddSingleton<IFlurlClientCache>(sp => new FlurlClientCache()
            .WithDefaults(builder => {
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
            ));

        return services;
    }

    private static IServiceCollection AddAllService(this IServiceCollection services)
    {
        var assembly = Assembly.Load("ZiziBot.Application");

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        services.Scan(selector => selector.FromAssembliesOf(typeof(TaskRunnerHostedService))
            .AddClasses(filter => filter.InNamespaceOf<TaskRunnerHostedService>())
            .As<IHostedService>()
            .WithSingletonLifetime());

        services.Scan(selector => selector.FromAssemblies(assembly)
            .AddClasses(x => x.InNamespaces(
                "ZiziBot.Application.Services",
                "ZiziBot.Application.UseCases"
            ))
            .AsSelf()
            .WithScopedLifetime());

        services.Scan(selector => selector.FromAssembliesOf(typeof(RegisterRssJobTask))
            .AddClasses(filter => filter.InNamespaceOf<RegisterRssJobTask>())
            .As<IStartupTask>()
            .WithScopedLifetime());

        services.Scan(selector => selector.FromAssembliesOf(typeof(MirrorPaymentRestService))
            .AddClasses()
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.AddScoped<ServiceFacade>();

        return services;
    }

    private static IServiceCollection AddBackgroundQueue(this IServiceCollection services)
    {
        services.AddBackgroundQueue(maxConcurrentCount: 3, millisecondsToWaitBeforePickingUpTask: 1000, onException: exception => { });

        return services;
    }
}