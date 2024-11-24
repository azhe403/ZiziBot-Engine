using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using ZiziBot.Contracts.Constants;

namespace ZiziBot.Console.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddConsole(this IServiceCollection services)
    {
        if (Flag.IsEnabled(Flag.CONSOLE_BLAZOR))
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddBlazoredLocalStorage();
            services.AddAuthorizationCore();
            services.AddRadzenComponents();
            services.AddReactiveViewModels();

            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
        }

        return services;
    }

    public static WebApplication ConfigureConsole(this WebApplication app)
    {
        if (Flag.IsEnabled(Flag.CONSOLE_BLAZOR))
        {
            app.UseBlazorFrameworkFiles();
            app.MapRazorPages();
            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");
        }

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

    private static IServiceCollection AddReactiveViewModels(this IServiceCollection services)
    {
        services.Scan(selector => selector.FromAssemblyOf<ChatSelectorViewModel>());

        return services;
    }
}