using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Infrastructure;

public static class MediatRExtension
{
    internal static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.AddMediatR(
            configuration =>
                configuration.RegisterServicesFromAssemblyContaining<PingRequestHandler>()
                    .AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>))
                    .AddOpenBehavior(typeof(FeatureControlPipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckUserRolePipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckRestrictionPipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckMessagePipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckAntispamPipelineBehavior<,>))
                    .AddOpenBehavior(typeof(CheckUsernamePipelineBehavior<,>))
                    .AddOpenBehavior(typeof(ActionResultPipelineBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(FindNoteRequestHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(CheckAfkSessionBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(EnsureChatAdminRequestHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(EnsureChatSettingBehavior<,>))
                    .AddOpenRequestPostProcessor(typeof(UpsertBotUserHandler<,>))
                    .AddOpenRequestPostProcessor(typeof(InsertChatActivityHandler<,>))
        );

        return services;
    }
}