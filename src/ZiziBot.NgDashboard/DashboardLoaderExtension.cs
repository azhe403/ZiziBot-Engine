using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.NgDashboard;

public static class DashboardLoaderExtension
{
    public static IServiceCollection AddNgDashboard(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAny", builder => builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
            );
        });

        // services.AddSpaStaticFiles(configuration =>
        // {
        //     configuration.RootPath = "./wwwroot";
        // });

        return services;
    }

    public static WebApplication UseNgDashboard(this WebApplication app)
    {
        app.UseStaticFiles();

        // if (!app.Environment.IsDevelopment())
        // {
        //     app.UseSpaStaticFiles();
        // }
        //
        // app.UseSpa(spa =>
        // {
        //     spa.Options.SourcePath = Env.DASHBOARD_PROJECT_PATH;
        //     if (app.Environment.IsDevelopment())
        //     {
        //         spa.UseAngularCliServer(npmScript: "start");
        //     }
        // });

        app.UseCors();

        return app;
    }
}