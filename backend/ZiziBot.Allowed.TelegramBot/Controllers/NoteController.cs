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
        await _mediatorService.EnqueueAsync(new GetNoteBotRequestModel()
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
        });
    }

    [Command("note")]
    [Command("renote")]
    public async Task CreateNote(MessageData data)
    {
        var query = data.Params.GetCommandParamAt<string>(0, separator: "\n");
        var rawButton = data.Params.Replace(query ?? "p", "").Trim();

        await _mediatorService.EnqueueAsync(new CreateNoteBotRequest()
        {
            BotToken = data.Options.Token,
            MinimumRole = RoleLevel.ChatAdminOrPrivate,
            Message = data.Message,
            ReplyMessage = true,
            Query = query,
            Content = data.Message.ReplyToMessage?.GetHtmlTextMarkup(),
            RawButton = rawButton,
            FileId = data.Message.ReplyToMessage?.GetFileId(),
            DataType = data.Message.ReplyToMessage != null ? (int)data.Message.ReplyToMessage.Type : -1,
            RefreshNote = data.Message.Text?.StartsWith("/renote"),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }

    [Command("dnote")]
    public async Task DeleteNote(MessageData data)
    {
        await _mediatorService.EnqueueAsync(new DeleteNoteRequest()
        {
            BotToken = data.Options.Token,
            Message = data.Message,
            Note = data.Params,
            ReplyMessage = true,
            DeleteAfter = TimeSpan.FromMinutes(1),
            CleanupTargets = new[]
            {
                CleanupTarget.FromBot,
                CleanupTarget.FromSender
            }
        });
    }
}