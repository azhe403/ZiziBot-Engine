using Blazored.LocalStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Radzen;

namespace ZiziBot.Console.Extensions;

public static class ServiceExtension
{

    public static IServiceCollection AddConsole(this IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor();
        services.AddBlazoredLocalStorage();
        services.AddRadzenComponents();

        return services;
    }

    public static WebApplication ConfigureConsole(this WebApplication app)
    {
        app.UseBlazorFrameworkFiles();
        app.MapRazorPages();
        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        return app;
    }

    private static IServiceCollection AddRadzenComponents(this IServiceCollection services)
    {
        services.AddScoped<DialogService>();
        services.AddScoped<NotificationService>();
        services.AddScoped<TooltipService>();
        services.AddScoped<ContextMenuService>();

        return services;
    }
}