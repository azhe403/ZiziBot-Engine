using Allowed.Telegram.Bot.Attributes;
using Allowed.Telegram.Bot.Controllers;
using Allowed.Telegram.Bot.Models;

namespace ZiziBot.Allowed.TelegramBot.Controllers;

[BotName("Main")]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public class NotesController : CommandController
{
    private readonly IMediator _mediator;

    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Command("notes")]
    public async Task GetNotes(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new GetNoteRequestModel()
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
    public async Task CreateNote(MessageData data)
    {
        await _mediator.EnqueueAsync(
            new CreateNoteRequestModel()
            {
                BotToken = data.Options.Token,
                Message = data.Message,
                ReplyMessage = true,
                Query = data.Params.GetCommandParamAt<string>(0, separator: "\n"),
                Content = data.Message.ReplyToMessage?.Text,
                FileId = data.Message.ReplyToMessage?.GetFileId(),
                CleanupTargets = new[]
                {
                    CleanupTarget.FromBot,
                    CleanupTarget.FromSender
                }
            }
        );
    }
}