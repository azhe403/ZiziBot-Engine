using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class CityController(
    MediatorService mediator
) : BotCommandController
{
    [Command("ac")]
    [Command("addcity")]
    [Command("add_city")]
    public async Task AddCity(CommandData data)
    {
        await mediator.EnqueueAsync(new AddCityBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CityName = data.CommandParam,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("city")]
    [Command("lc")]
    public async Task GetCity(CommandData data)
    {
        await mediator.EnqueueAsync(new GetCityListBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("salat")]
    [Command("shalat")]
    [Command("sholat")]
    public async Task GetShalatTime(CommandData data)
    {
        await mediator.EnqueueAsync(new GetShalatTimeBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}