using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Cli.Extensions;

public static class ServiceExtension
{
    public static IServiceCollection AddTools(this IServiceCollection services)
    {
        services.Scan(selector => {
            selector.FromAssembliesOf(typeof(ProjectTool))
                .AddClasses(filter => filter.InNamespaceOf<ProjectTool>())
                .AsSelf()
                .WithScopedLifetime();
        });

        return services;
    }
}