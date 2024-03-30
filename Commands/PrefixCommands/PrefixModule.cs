using Discord;
using Discord.Commands;

namespace Commands.PrefixCommands;

public abstract class PrefixModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    public async Task Pong()
    {
        await Context.Message.ReplyAsync("Pong!");
    }
}