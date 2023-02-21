using System.Reflection;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class MediatRExtension
{
    internal static IServiceCollection AddMediator(this IServiceCollection services)
    {
        var assembly = typeof(PingRequestHandler).GetTypeInfo().Assembly;

        services.AddMediatR(
            configuration => configuration
                .RegisterServicesFromAssemblyContaining<PingRequestHandler>()
                .AddOpenBehavior(typeof(BotMiddlewarePipelineBehaviour<,>))
        );

        services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        services.AddMediatRBehaviors();

        return services;
    }

    private static IServiceCollection AddMediatRBehaviors(this IServiceCollection services)
    {
        services.AddTransient(typeof(IPipelineBehavior<CreateNoteRequestModel, ResponseBase>), typeof(CheckChatAdminBehavior));
        return services;
    }

    #region MediatR attributed behaviors

    // source: https://github.com/ITIXO/MediatR.Extensions.AttributedBehaviors/blob/main/src/MediatR.Extensions.AttributedBehaviors/ServiceCollectionExtensions.cs
    public static IServiceCollection AddMediatRAttributedBehaviors(this IServiceCollection services, Assembly assembly)
    {
        return services.AddMediatRAttributedBehaviors(new[] { assembly });
    }

    public static IServiceCollection AddMediatRAttributedBehaviors(this IServiceCollection services, IEnumerable<Assembly> assemblies)
    {
        var queriesWithAttributes = assemblies
            .Distinct()
            .SelectMany(a => a.DefinedTypes)
            .Where(
                ti =>
                    // (ti.ImplementedInterfaces.Contains(typeof(IRequest)) ||
                    // ti.IsAssignableToGenericType(typeof(IRequest<>))) &&
                    Attribute.IsDefined(ti, typeof(MediatRBehaviorAttribute))
            ).ToList();

        queriesWithAttributes.ForEach(
            typeInfo => {
                foreach (var implementedInterface in queriesWithAttributes
                             .Where(implementedInterface => implementedInterface.IsGenericType))
                {
                    services.Add(new ServiceDescriptor(implementedInterface, typeInfo, ServiceLifetime.Transient));
                }
            }
        );

        // foreach (var query in queriesWithAttributes)
        // {
        //     foreach (var attr in query
        //                  .GetCustomAttributes<MediatRBehaviorAttribute>(false)
        //                  .OrderBy(attr => attr.Order))
        //     {
        // services.Add(new ServiceDescriptor(attr.InterfaceType, attr.BehaviorType, attr.ServiceLifetime));
        // services.AddTransient(attr.InterfaceType, typeof(CheckChatAdminBehavior));
        // }
        // }

        return services;
    }

    private static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        if (givenType.GetInterfaces().Any(it => it.IsGenericType && it.GetGenericTypeDefinition() == genericType))
        {
            return true;
        }

        if (givenType.IsGenericType &&
            givenType.GetGenericTypeDefinition() == genericType)
        {
            return true;
        }

        var baseType = givenType.BaseType;
        return baseType != null && IsAssignableToGenericType(baseType, genericType);
    }

    #endregion
}