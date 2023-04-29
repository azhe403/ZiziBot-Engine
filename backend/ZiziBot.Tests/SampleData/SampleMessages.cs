using Telegram.Bot.Types;

namespace ZiziBot.Tests.SampleData;

public static class SampleMessages
{
    public static Message CommonMessage => new Message
    {
        MessageId = 12345,
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