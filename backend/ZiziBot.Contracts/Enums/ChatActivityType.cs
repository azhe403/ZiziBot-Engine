namespace ZiziBot.Contracts.Enums;

public enum ChatActivityType
{
    BotBanUser,
    BotUnbanUser,

    BotSendMessage,
    BotEditMessage,

    BotWarnUsername,
    BotKickMember,

    BotSendWebHook,
    BotEditWebHook,

    NewChatMember,
    KickMember,

    UserDeleteMessage,
    UserForwardMessage,
    UserPinMessage,
    UserUnpinMessage,

    UserSendMessage,
    UserEditMessage,
}