using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Cli;

public static class ServiceExtension
{
    public static IServiceCollection AddTools(this IServiceCollection services)
    {
        services.Scan(selector => {
            selector.FromAssembliesOf(typeof(ProjectTool))
                .AddClasses(filter => filter.InNamespaceOf<ProjectTool>())
                .AsSelf()
                .WithTransientLifetime();
        });

        return services;
    }
}