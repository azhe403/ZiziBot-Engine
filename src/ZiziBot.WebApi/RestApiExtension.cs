using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using TIPC.Web.AutoWrapper;

namespace ZiziBot.WebApi;

public static class RestApiExtension
{
    public static IApplicationBuilder ConfigureAutoWrapper(this IApplicationBuilder app)
    {
        app.UseAutoWrapper(
            new AutoWrapperOptions()
            {
                WrapWhenApiPathStartsWith = "/api",
                IsApiOnly = false,
                ShowStatusCode = true,
                ShowIsErrorFlagForSuccessfulResponse = true,
                IgnoreNullValue = false,
                IgnoreWrapForOkRequests = false,
                ShouldLogRequestData = true,
                EnableResponseLogging = true,
                EnableExceptionLogging = true,
                LogRequestDataOnException = true
            }
        );

        return app;
    }

    public static IServiceCollection ConfigureApi(this IServiceCollection services)
    {
        services
            .Configure<ApiBehaviorOptions>(options => { options.SuppressInferBindingSourcesForParameters = true; })
            .AddControllers(options =>
                {
                    options.Conventions.Add(new ControllerHidingConvention());
                    options.Conventions.Add(new ActionHidingConvention());
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                }
            )
            .AddNewtonsoftJson();

        return services;
    }

    public static IServiceCollection AddAllMiddleware(this IServiceCollection services)
    {
        services.Scan(
            selector =>
            {
                selector.FromAssembliesOf(typeof(HeaderCheckMiddleware))
                    .AddClasses(filter => filter.InNamespaceOf<HeaderCheckMiddleware>())
                    .AsSelf()
                    .WithTransientLifetime();
            }
        );

        return services;
    }

    public static IApplicationBuilder UseAllMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<HeaderCheckMiddleware>();

        return app;
    }
}