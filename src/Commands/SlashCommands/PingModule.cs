using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Commands.SlashCommands;

public class PingModule(ILogger<PingModule> logger) : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService? Commands { get; set; }

    [SlashCommand("ping", "Receive a pong!")]
    public async Task Ping()
    {
        logger.LogInformation("User: {user}, Command: ping", Context.User.Username);
        await RespondAsync("Pong!");
    }
}