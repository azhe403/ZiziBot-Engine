using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ZiziBot.Utils;

public static class TelegramUtil
{
    public static string GetFullName(this User user)
    {
        var fullName = (user.FirstName + " " + user.LastName).Trim();
        return fullName;
    }

    public static string GetFullMention(this User user)
    {
        var fullName = (user.FirstName + " " + user.LastName).Trim();
        var mention = "<a href=\"tg://user?id=" + user.Id + "\">" + fullName + "</a>";
        return mention;
    }

    public static string? GetFileId(this Message message)
    {
        var fileId = message.Type switch
        {
            MessageType.Photo => message.Photo?.LastOrDefault()?.FileId,
            MessageType.Audio => message.Audio?.FileId,
            MessageType.Video => message.Video?.FileId,
            MessageType.Voice => message.Voice?.FileId,
            MessageType.Document => message.Document?.FileId,
            MessageType.Sticker => message.Sticker?.FileId,
            MessageType.VideoNote => message.VideoNote?.FileId,
            _ => null
        };

        return fileId;
    }
}