using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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
        var serviceProvider = services.BuildServiceProvider();
        var jwtConfig = serviceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;

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

        services.AddAuthorization()
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };

                o.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            new ApiResponseBase<bool>()
                            {
                                StatusCode = HttpStatusCode.Unauthorized,
                                Message = "Please ensure you have a valid token"
                            }
                        );
                    }
                };
            });

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
        app.UseMiddleware<InjectHeaderMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}