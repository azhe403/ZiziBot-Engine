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
        var listBotData = provider.GetRequiredService<IOptions<List<BotData>>>().Value;

        services.AddTelegramClients(listBotData);

        if (env.IsDevelopment())
            services.AddTelegramManager();
        else
            services.AddTelegramWebHookManager();

        return services;
    }
}