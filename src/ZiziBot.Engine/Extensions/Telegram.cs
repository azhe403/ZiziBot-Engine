using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;
using Microsoft.Extensions.Options;

namespace ZiziBot.Engine.Extensions;

public static class Telegram
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