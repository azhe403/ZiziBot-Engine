using Discord.Commands;

namespace ZiziBot.DiscordNet.DiscordBot;

public class StartModule : ModuleBase<SocketCommandContext>
{
	[Command("ping")]
	[Alias("pong", "hello")]
	public Task PingAsync()
		=> ReplyAsync("pong!");
}