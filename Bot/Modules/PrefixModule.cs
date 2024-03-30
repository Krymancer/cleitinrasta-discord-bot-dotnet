using Discord.Commands;
using Discord;

namespace Bot.Modules;

public class PrefixModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task Pong()
    {
        await Context.Message.ReplyAsync("Pong!");
    }
}