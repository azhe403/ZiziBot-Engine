using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class NotesController : CommandController
{
    private readonly MediatorService _mediatorService;

    public NotesController(MediatorService mediatorService)
    {
        _mediatorService = mediatorService;
    }

    [Command("notes")]
    public async Task GetNotes(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new GetNoteRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                ReplyMessage = true,
                DeleteAfter = TimeSpan.FromHours(1),
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }

    [Command("note")]
    [Command("renote")]
    public async Task CreateNote(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new CreateNoteRequestModel()
            {
                BotToken = data.Options.Token,
                MinimumRole = RoleLevel.ChatAdminOrPrivate,
                Message = data.Message,
                ReplyMessage = true,
                Query = data.Params.GetCommandParamAt<string>(0, separator: "\n"),
                Content = data.Message.ReplyToMessage?.Text,
                RawButton = data.Params.GetCommandParamAt<string>(1, separator: "\n"),
                FileId = data.Message.ReplyToMessage?.GetFileId(),
                RefreshNote = data.Message.Text?.StartsWith("/renote"),
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }
}