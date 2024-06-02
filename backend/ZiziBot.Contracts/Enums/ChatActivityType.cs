namespace ZiziBot.Contracts.Enums;

public enum ChatActivityType
{
    BanUser,
    UnbanUser,

    NewChatMember,
    KickMember,

    DeleteMessage,
    ForwardMessage,
    PinMessage,
    UnpinMessage,

    BotSendMessage,
    BotEditMessage,
    BotSentWebHook,

    UserSendMessage,
    UserEditMessage
}