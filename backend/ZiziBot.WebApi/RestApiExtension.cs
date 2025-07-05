using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ZiziBot.Common.Configs;
using ZiziBot.Common.Converters.SystemTextJson;

namespace ZiziBot.WebApi;

public static class RestApiExtension
{
    private const string ALLOW_ANY_ORIGIN_POLICY = "AllowAnyOriginPolicy";

    public static IServiceCollection AddRestApi(this IServiceCollection services)
    {
        using var serviceProvider = services.BuildServiceProvider();
        var jwtConfig = serviceProvider.GetRequiredService<IOptions<JwtConfig>>().Value;

        services.AddSignalR();
        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<PostGlobalBanApiValidator>();

        services
            .Configure<ApiBehaviorOptions>(options => {
                options.SuppressInferBindingSourcesForParameters = true;
            })
            .AddControllers(options => {
                    options.Conventions.Add(new ControllerHidingConvention());
                    options.Conventions.Add(new ActionHidingConvention());
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                }
            )
            .AddJsonOptions(options => {
                options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options => {
                options.InvalidModelStateResponseFactory = context => {
                    var transactionId = context.HttpContext.GetTransactionId();

                    var errorDetails = context.ModelState
                        .Where(entry => entry.Value?.ValidationState == ModelValidationState.Invalid)
                        .Where(x => x.Key != "request")
                        .Select(key => new {
                            Id = key.Key,
                            Field = key.Key.Split('.').LastOrDefault(),
                            Message = key.Value?.Errors.Select(e => e.ErrorMessage)
                        }).ToList();

                    var errors = errorDetails.SelectMany(x => x.Message ?? []).ToList();

                    return new BadRequestObjectResult(new ApiResponseBase<object>() {
                        StatusCode = HttpStatusCode.BadRequest,
                        TransactionId = transactionId,
                        Message = errors.Aggregate((a, b) => $"{a}\n{b}"),
                        Result = new {
                            Error = errors.Aggregate((a, b) => $"{a}\n{b}"),
                            Errors = errors,
                            ErrorDetails = errorDetails,
                        }
                    });
                };
            });

        services.AddAuthorization()
            .AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o => {
                o.TokenValidationParameters = new TokenValidationParameters {
                    ValidIssuer = jwtConfig.Issuer,
                    ValidAudience = jwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };

                o.Events = new JwtBearerEvents {
                    OnChallenge = async context => {
                        context.HandleResponse();

                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsJsonAsync(
                            new ApiResponseBase<bool>() {
                                StatusCode = HttpStatusCode.Unauthorized,
                                Message = "Please ensure you have a valid token"
                            }
                        );
                    }
                };
            });

        services.AddCorsConfiguration();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options => {
                options.DocumentFilter<HidePathDocumentFilter>();
                options.SchemaFilter<SwaggerIgnoreFilter>();
            }
        );

        services.ConfigureRateLimiter();

        return services;
    }

    private static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options => {
            options.AddPolicy(ALLOW_ANY_ORIGIN_POLICY, builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });

        return services;
    }

    public static IServiceCollection AddAllMiddleware(this IServiceCollection services)
    {
        services.Scan(
            selector => {
                selector.FromAssembliesOf(typeof(GlobalExceptionMiddleware))
                    .AddClasses(filter => filter.InNamespaceOf<GlobalExceptionMiddleware>())
                    .AsSelf()
                    .WithScopedLifetime();
            }
        );

        return services;
    }

    public static WebApplication ConfigureApi(this WebApplication app)
    {
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseMiddleware<RequestBodyGuardMiddleware>();
        app.UseMiddleware<InjectRequestMiddleware>();
        app.MapHub<LogHub>("/api/logging");

        app.UseStaticFiles();
        app.MapControllers();

        app.UseRouting();
        app.UseCors(ALLOW_ANY_ORIGIN_POLICY);
        app.UseAuthentication();
        app.UseAuthorization();

        app.ConfigureRateLimiter();

        app.UseSwagger();
        app.UseSwaggerUI(options => {
            options.DefaultModelsExpandDepth(-1);
        });

        return app;
    }
}