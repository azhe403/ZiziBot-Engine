using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ZiziBot.Common.Types;

namespace ZiziBot.Application.Tasks;

public class StartupNotificationTask(IServiceScopeFactory serviceScope) : IStartupTask
{
    public async Task ExecuteAsync()
    {
        using var service = serviceScope.CreateScope();
        var appSettingRepository = service.ServiceProvider.GetRequiredService<AppSettingRepository>();

        var config = await appSettingRepository.GetBotMain();
        var eventConfig = await appSettingRepository.GetRequiredConfigSectionAsync<EventLogConfig>();
        var engineConfig = await appSettingRepository.GetRequiredConfigSectionAsync<EngineConfig>();

        var bot = new TelegramBotClient(config.Token);

        var message = HtmlMessage.Empty
            .Bold("Startup Notification").Br()
            .Bold("Date: ").Code(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss zz")).Br()
            .Bold("ExecutionStrategy: ").Code(engineConfig.ExecutionStrategy.ToString()).Br()
            .Bold("EngineMode: ").Code(engineConfig.TelegramEngineMode.ToString()).Br()
            .Text("#task #startup");

        await bot.SendMessage(eventConfig.ChatId, message.ToString(), parseMode: ParseMode.Html);
    }
}