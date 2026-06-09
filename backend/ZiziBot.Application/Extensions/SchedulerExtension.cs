using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Application.Features.Hangfire;

namespace ZiziBot.Application.Extensions;

public static class SchedulerExtension
{
    public static IServiceCollection ConfigureScheduler(this IServiceCollection services)
    {
        services.AddScoped<SchedulerService>();
        services.ConfigureHangfire();
        // services.ConfigureTickerQ();

        return services;
    }

    public static IApplicationBuilder UseScheduler(this IApplicationBuilder app)
    {
        app.UseHangfire();
        // app.EnableTickerQ();

        return app;
    }
}