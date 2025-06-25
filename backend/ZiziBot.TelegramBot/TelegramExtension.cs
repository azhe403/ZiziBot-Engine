using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using ZiziBot.Application.Facades;
using ZiziBot.Common.Configs;
using ZiziBot.Common.Constants;
using ZiziBot.DataSource.MongoEf.Entities;
using ZiziBot.TelegramBot.Framework.Extensions;
using ZiziBot.TelegramBot.Framework.Models.Configs;

namespace ZiziBot.TelegramBot;

public static class TelegramExtension
{
    public async static Task<IServiceCollection> ConfigureTelegramBot(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var config = provider.GetRequiredService<IOptions<EngineConfig>>().Value;
        var dataFacade = provider.GetRequiredService<DataFacade>();

        var listBotData = await dataFacade.AppSetting.ListBots();

        services.AddZiziBotTelegramBot(new BotEngineConfig() {
            EngineMode = config.TelegramEngineMode,
            WebhookUrl = EnvUtil.GetEnv(Env.TELEGRAM_WEBHOOK_URL),
            ExecutionStrategy = config.ExecutionStrategy,
            Bot = listBotData.Select(x => new BotTokenConfig() {
                Name = x.Name,
                Token = x.Token
            }).ToList()
        });

        return services;
    }

    public static async Task<IApplicationBuilder> RunTelegramBot(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices.CreateScope().ServiceProvider;
        var dataFacade = provider.GetRequiredService<DataFacade>();

        var listBotOptions = provider.GetRequiredService<List<BotTokenConfig>>();

        if (listBotOptions.Count == 0)
        {
            dataFacade.MongoEf.BotSettings.Add(new BotSettingsEntity() {
                Name = "Main",
                Token = "BOT_TOKEN_HERE",
                Status = EventStatus.InProgress
            });

            await dataFacade.MongoEf.SaveChangesAsync();

            throw new ApplicationException("No bot data found. Please ensure config for 'BotSettings'");
        }

        var validBot = listBotOptions
            .Where(x => x.Name != "BOT_NAME")
            .ToList();

        if (validBot.Count == 0)
        {
            throw new ApplicationException("Please add your Bot to 'BotSettings'");
        }

        await app.UseZiziBotTelegramBot();

        var bot = new TelegramBotClient(validBot.First(x => x.Name == "Main").Token);
        var me = await bot.GetMe();
        ValueConst.UniqueKey = me.GetFullName().Replace(" ", "_").ToLower() ?? string.Empty;

        return app;
    }
}