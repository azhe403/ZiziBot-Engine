using Telegram.Bot.Types.Enums;
using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class ChatController(
    MediatorService mediatorServiceService
) : BotCommandController
{
    [Command("fch")]
    public async Task AddForwardChannelSource(CommandData data)
    {
        await mediatorServiceService.EnqueueAsync(new ConnectChannelPostRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            ChannelId = data.CommandParam.GetCommandParamAt<long>(0),
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("pin")]
    public async Task PinMessage(CommandData data)
    {
        await mediatorServiceService.EnqueueAsync(new PinMessageRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [UpdateCommand(UpdateType.ChatJoinRequest)]
    public async Task JoinRequest(CommandData data)
    {
        await mediatorServiceService.EnqueueAsync(new ChatJoinBotRequest() {
            BotToken = data.BotToken,
            Update = data.Update,
            DeleteAfter = TimeSpan.FromMinutes(10),
            CleanupTargets = new[] {
                CleanupTarget.FromBot
            }
        });
    }

    [TypedCommand(MessageType.NewChatMembers)]
    public async Task NewChatMembers(CommandData data)
    {
        await mediatorServiceService.EnqueueAsync(new NewChatMembersBotRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            NewUser = data.Message.NewChatMembers!,
            DeleteAfter = TimeSpan.FromMinutes(10),
            CleanupTargets = new[] {
                CleanupTarget.FromBot
            }
        });
    }

    [UpdateCommand(UpdateType.ChannelPost)]
    [UpdateCommand(UpdateType.EditedChannelPost)]
    public async Task ChannelPost(CommandData data)
    {
        await mediatorServiceService.EnqueueAsync(new ForwardChannelPostRequest() {
            BotToken = data.BotToken,
            Update = data.Update
        });
    }
}