﻿using Microsoft.Extensions.Logging;
using MongoFramework.Linq;
using Telegram.Bot.Types;

namespace ZiziBot.Application.Handlers.Telegram.Group;

public class NewChatMembersRequest : RequestBase
{
    public User[] NewUser { get; set; } = null!;
}

[UsedImplicitly]
public class NewChatMembersHandler : IRequestHandler<NewChatMembersRequest, ResponseBase>
{
    private readonly ILogger<NewChatMembersHandler> _logger;
    private readonly GroupDbContext _groupDbContext;
    private readonly TelegramService _telegramService;

    public NewChatMembersHandler(ILogger<NewChatMembersHandler> logger, GroupDbContext groupDbContext, TelegramService telegramService)
    {
        _logger = logger;
        _groupDbContext = groupDbContext;
        _telegramService = telegramService;
    }

    public async Task<ResponseBase> Handle(NewChatMembersRequest request, CancellationToken cancellationToken)
    {
        _telegramService.SetupResponse(request);
        _logger.LogInformation("New Chat Members. ChatId: {ChatId}", request.ChatId);

        var chatTitle = request.ChatTitle;
        var newMemberCount = request.NewUser.Length;
        var allNewMember = request.NewUser.Select(user => user.GetFullMention()).Aggregate((s, next) => s + ", " + next);
        var greet = TimeUtil.GetTimeGreet();
        var memberCount = await _telegramService.GetMemberCount();

        var messageTemplate = $"Hai {allNewMember}\n" +
                              $"Selamat datang di Kontrakan {request.ChatTitle}";

        var welcomeMessage = await _groupDbContext.WelcomeMessage
            .Where(x => x.ChatId == request.ChatIdentifier)
            .Where(x => x.Status == (int)EventStatus.Complete)
            .FirstOrDefaultAsync(cancellationToken);

        if (welcomeMessage != null)
        {
            messageTemplate = welcomeMessage.Text;
        }

        var messageText = messageTemplate.ResolveVariable(new List<(string placeholder, string value)>()
        {
            ("AllNewMember", allNewMember),
            // ("AllNoUsername", allNoUsername),
            // ("AllNewBot", allNewBot),
            ("ChatTitle", chatTitle),
            ("Greet", greet),
            ("NewMemberCount", newMemberCount.ToString()),
            ("MemberCount", memberCount.ToString())
        });

        if (welcomeMessage?.DataType > (int)CommonMediaType.Text)
        {
            return await _telegramService.SendMediaAsync(welcomeMessage.Media, (CommonMediaType)welcomeMessage.DataType, messageText);
        }

        return await _telegramService.SendMessageText(messageText);
    }
}