using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ZiziBot.TelegramBot.Framework.Extensions;
using ZiziBot.TelegramBot.Framework.Models.Configs;

namespace ZiziBot.TelegramBot;

public static class TelegramExtension
{
    public static IServiceCollection ConfigureTelegramBot(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var environment = provider.GetRequiredService<IWebHostEnvironment>();
        var config = provider.GetRequiredService<IOptions<EngineConfig>>().Value;
        var appSettingDbContext = provider.GetRequiredService<MongoDbContextBase>();
        // var listBotOptions = provider.GetRequiredService<IOptions<List<SimpleTelegramBotClientOptions>>>().Value;


        // services.AddTelegramControllers();
        //
        // switch (config.TelegramEngineMode)
        // {
        //     case BotEngineMode.Webhook:
        //         services.AddTelegramWebHookManager();
        //         break;
        //
        //     case BotEngineMode.Polling:
        //         services.AddTelegramManager();
        //         break;
        //
        //     default: // or EngineMode.Auto
        //         if (environment.IsDevelopment())
        //             services.AddTelegramManager();
        //         else
        //             services.AddTelegramWebHookManager();
        //
        //         break;
        // }

        var listBotData = appSettingDbContext.BotSettings
            .Where(settings => settings.Status == (int)EventStatus.Complete)
            .ToList();

        services.AddZiziBotTelegramBot(new BotEngineConfig() {
            EngineMode = config.TelegramEngineMode,
            WebhookUrl = EnvUtil.GetEnv(Env.WEB_CONSOLE_URL),
            Bot = listBotData.Select(x => new BotTokenConfig() {
                Name = x.Name,
                Token = x.Token
            }).ToList()
        });

        return services;
    }

    // public async static Task<IApplicationBuilder> RunTelegramBot(this IApplicationBuilder app)
    // {
    //     var provider = app.ApplicationServices;
    //     var appSettingsDbContext = provider.GetRequiredService<MongoDbContextBase>();
    //
    //     var listBotOptions = provider.GetRequiredService<IOptions<List<SimpleTelegramBotClientOptions>>>().Value;
    //
    //     if (!listBotOptions.Any())
    //     {
    //         appSettingsDbContext.BotSettings.Add(new BotSettingsEntity() {
    //             Name = "BOT_NAME",
    //             Token = "BOT_TOKEN",
    //             Status = (int)EventStatus.InProgress
    //         });
    //
    //         await appSettingsDbContext.SaveChangesAsync();
    //
    //         throw new ApplicationException("No bot data found. Please ensure config for 'BotSettings'");
    //     }
    //
    //     var validBot = listBotOptions
    //         .Where(x => x.Name != "BOT_NAME")
    //         .ToList();
    //
    //     if (!validBot.Any())
    //     {
    //         throw new ApplicationException("Please add your Bot to 'BotSettings'");
    //     }
    //
    //     var telegramManager = provider.GetRequiredService<ITelegramManager>();
    //     await telegramManager.Start(validBot.Select(options => TelegramBotClientFactory.CreateClient(options)));
    //
    //     return app;
    // }
}