using ZiziBot.TelegramBot.Framework.Attributes;
using ZiziBot.TelegramBot.Framework.Models;

namespace ZiziBot.TelegramBot.Controllers;

// [BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class NotesController(
    MediatorService mediatorService
) : BotCommandController
{
    [Command("notes")]
    [Command("tags")]
    public async Task GetNotes(CommandData data)
    {
        await mediatorService.EnqueueAsync(new GetNoteBotRequestModel() {
            BotToken = data.BotToken,
            Message = data.Message,
            ReplyMessage = true,
            DeleteAfter = TimeSpan.FromHours(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("note")]
    [Command("renote")]
    public async Task CreateNote(CommandData data)
    {
        var query = data.CommandParam.GetCommandParamAt<string>(0, separator: "\n");
        var rawButton = data.CommandParam.TrimStart(query);

        await mediatorService.EnqueueAsync(new CreateNoteBotRequest() {
            BotToken = data.BotToken,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            Message = data.Message,
            ReplyMessage = true,
            Query = query,
            Content = data.Message.ReplyToMessage?.GetHtmlTextMarkup(),
            RawButton = rawButton,
            FileId = data.Message.ReplyToMessage?.GetFileId(),
            DataType = data.Message.ReplyToMessage != null ? (int)data.Message.ReplyToMessage.Type : -1,
            RefreshNote = data.Message.Text?.StartsWith("/renote"),
            DeleteAfter = TimeSpan.FromHours(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("dnote")]
    public async Task DeleteNote(CommandData data)
    {
        await mediatorService.EnqueueAsync(new DeleteNoteRequest() {
            BotToken = data.BotToken,
            Message = data.Message,
            Note = data.CommandParam,
            ReplyMessage = true,
            DeleteAfter = TimeSpan.FromMinutes(1),
            CleanupTargets = new[] {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}