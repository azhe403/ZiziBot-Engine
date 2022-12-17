using Allowed.Telegram.Bot.Extensions;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Engine.Extensions;

public static class Telegram
{
    public static IServiceCollection ConfigureTelegramBot(this IServiceCollection services)
    {
        var provider = services.BuildServiceProvider();
        var configuration = provider.GetRequiredService<IConfiguration>();
        var env = provider.GetRequiredService<IWebHostEnvironment>();

        services.AddTelegramClients(configuration.GetSection("Telegram:Bots").Get<BotData[]>());

        if (env.IsDevelopment())
            services.AddTelegramManager();
        else
            services.AddTelegramWebHookManager();

        return services;
    }
}