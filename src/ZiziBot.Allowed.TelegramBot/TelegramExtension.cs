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
        var env = provider.GetRequiredService<IWebHostEnvironment>();

        var listBotOptions = provider.GetRequiredService<IOptions<List<SimpleTelegramBotClientOptions>>>().Value;

        if (!listBotOptions.Any())
        {
            throw new ApplicationException("No bot data found. Please ensure config for 'BotSettings'");
        }

        services.AddTelegramClients(listBotOptions);

        if (env.IsDevelopment())
            services.AddTelegramManager();
        else
            services.AddTelegramWebHookManager();

        return services;
    }
}