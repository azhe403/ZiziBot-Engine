using Telegram.Bot.Types;

namespace ZiziBot.Tests.SampleData;

public static class SampleMessages
{
    public static Message CommonMessage => new Message
    {
        MessageId = 316001,
        MessageThreadId = 297986,
        ForumTopicCreated = new ForumTopicCreated()
        {
            Name = "Playground 1"
        },
        ForumTopicEdited = new ForumTopicEdited()
        {
            Name = "Playground II"
        },
        Date = DateTime.UtcNow,
        EditDate = DateTime.UtcNow,
        ForwardDate = DateTime.UtcNow,
        Chat = new Chat
        {
            Id = -1001404591750,
            Title = "🇮🇩 Telegram Bot API🔥🔥"
        },
        From = new User
        {
            Id = 1025424321,
            FirstName = "Sandal",
            LastName = "Jepit"
        }
    };

    public static Message NewChatMembers => new()
    {
        MessageId = 12345,
        MessageThreadId = 297986,
        Chat = new Chat()
        {
            Id = -1001404591750,
            Title = "🇮🇩 Telegram Bot API🔥🔥"
        },
        From = new User()
        {
            Id = 552609163,
            FirstName = "I'am",
            LastName = "Groot"
        },
        NewChatMembers = new[]
        {
            new User()
            {
                Id = 1025424321,
                FirstName = "Sandal",
                LastName = "Jepit"
            },
            new User()
            {
                Id = 1025424321,
                FirstName = "Fulan",
                LastName = "Ahmad"
            },
            new User()
            {
                Id = 552609163,
                FirstName = "I'm",
                LastName = "Groot"
            }
        }
    };
}