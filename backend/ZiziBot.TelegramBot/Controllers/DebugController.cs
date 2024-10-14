using Telegram.Bot.Types.Enums;
using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class DebugController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("about")]
    [Command("start")]
    public async Task GetAbout(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetAboutBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("id")]
    public async Task GetId(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetIdBotRequestModel() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("si")]
    public async Task GetSystemInfo(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetSystemInfoRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            MinimumRole = RoleLevel.Sudo,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("fid")]
    public async Task GetFileId(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetFileIdBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("dbg")]
    [Command("yaml")]
    [Command("json")]
    public async Task GetDebug(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetDebugBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            CleanupTarget = CleanupTarget.FromBot | CleanupTarget.FromSender,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("promote")]
    [Command("demote")]
    public async Task PromoteMember(CommandData data)
    {
        await mediatorService.EnqueueAsync(new PromoteMemberBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            Promote = data.Message.Text.IsCommand("/promote"),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [TypedCommand(MessageType.ForumTopicCreated)]
    [TypedCommand(MessageType.ForumTopicEdited)]
    public async Task OnTopicChange(CommandData data)
    {
        await mediatorService.EnqueueAsync(new ThreadUpdateBotRequest() {
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