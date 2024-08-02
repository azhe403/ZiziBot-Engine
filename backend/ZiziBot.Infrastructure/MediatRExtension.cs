using System.Reflection;
using MediatR.Extensions.AttributedBehaviors;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class MediatRExtension
{
    internal static IServiceCollection AddMediator(this IServiceCollection services)
    {
        var assembly = typeof(PingRequestHandler).GetTypeInfo().Assembly;

        services.AddMediatR(
            configuration =>
                configuration.RegisterServicesFromAssemblyContaining<PingRequestHandler>()
                    .AddBehavior(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>))
                    .AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>))
                    .AddOpenBehavior(typeof(CheckUserRolePipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckRestrictionPipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckUsernamePipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckMessagePipelineBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(CheckAfkSessionBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(EnsureChatAdminRequestHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(EnsureChatSettingBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(FindNoteRequestHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(UpsertBotUserHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(InsertChatActivityHandler<,>))
        );

        // services.AddTransient(typeof(IRequestExceptionHandler<,,>), typeof(GlobalExceptionHandler<,,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehaviour<,>));

        services.AddMediatRAttributedBehaviors(assembly);

        return services;
    }
}