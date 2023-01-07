using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ZiziBot.Allowed.TelegramBot;

public static class TelegramExtension
{
    public static IServiceCollection ConfigureTelegramBot(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var appSettingsDbContext = provider.GetRequiredService<AppSettingsDbContext>();
        var environment = provider.GetRequiredService<IWebHostEnvironment>();

        var listBotOptions = provider.GetRequiredService<IOptions<List<SimpleTelegramBotClientOptions>>>().Value;

        if (!listBotOptions.Any())
        {
            appSettingsDbContext.BotSettings.Add(new BotSettings()
            {
                Name = "BOT_NAME",
                Token = "BOT_TOKEN"
            });

            appSettingsDbContext.SaveChanges();

            throw new ApplicationException("No bot data found. Please ensure config for 'BotSettings'");
        }

        var validBot = listBotOptions
            .Where(x => x.Name != "BOT_NAME")
            .ToList();

        if (!validBot.Any())
        {
            throw new ApplicationException("Please add your Bot to 'BotSettings'");
        }

        services.AddTelegramClients(validBot);

        if (environment.IsDevelopment())
            services.AddTelegramManager();
        else
            services.AddTelegramWebHookManager();

        return services;
    }
}