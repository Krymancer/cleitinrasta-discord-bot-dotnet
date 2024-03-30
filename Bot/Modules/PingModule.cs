using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Bot.Modules;

public class PingModule : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService Commands { get; set; }

    private readonly ILogger<PingModule> _logger;

    public PingModule(ILogger<PingModule> logger)
    {
        _logger = logger;
    }

    [SlashCommand("ping", "Receive a pong!")]
    public async Task Ping()
    {
        _logger.LogInformation("User: {user}, Command: ping", Context.User.Username);
        await RespondAsync("Pong!");
    }
}