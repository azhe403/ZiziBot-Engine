using Cortex.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace ZiziBot.Application.Extensions;

public static class CortexExtension
{
    internal static IServiceCollection AddApplicationCortexMediator(this IServiceCollection services)
    {
        services.AddCortexMediator([typeof(PingRequestHandler)], options =>
        {
            options.AddOpenQueryPipelineBehavior(typeof(LoggingPipeline<,>));
            options.AddOpenQueryPipelineBehavior(typeof(PreProcessPipeline<,>));
            options.AddOpenQueryPipelineBehavior(typeof(PostProcessPipeline<,>));
        });

        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(FeatureControlPipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(CheckUserRolePipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(CheckRestrictionPipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(CheckMessagePipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(CheckAntispamPipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(CheckUsernamePipeline<,>));
        services.AddTransient(typeof(IPreProcessPipeline<,>), typeof(ActionResultPipeline<,>));
        services.AddTransient(typeof(IPostProcessPipeline<,>), typeof(CheckAfkSessionPipeline<,>));
        services.AddTransient(typeof(IPostProcessPipeline<,>), typeof(EnsureChatAdminPipeline<,>));
        services.AddTransient(typeof(IPostProcessPipeline<,>), typeof(EnsureChatSettingPipeline<,>));
        services.AddTransient(typeof(IPostProcessPipeline<,>), typeof(UpsertBotUserPipeline<,>));
        services.AddTransient(typeof(IPostProcessPipeline<,>), typeof(InsertChatActivityPipeline<,>));
        services.AddScoped<IAppMediator, CortexAppMediator>();

        return services;
    }
}
