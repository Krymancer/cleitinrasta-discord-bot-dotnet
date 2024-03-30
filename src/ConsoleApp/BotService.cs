using Commands.PrefixCommands;
using Configuration.Options;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot;

public class BotService : IHostedService
{
    private readonly DiscordSocketClient _discordClient;
    private readonly ILogger<BotService> _logger;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly InteractionService _interactionService;
    private readonly InteractionHandler _interactionHandler;
    private readonly PrefixHandler _prefixHandler;

    public BotService(DiscordSocketClient discordClient, ILogger<BotService> logger, IOptions<AppSettings> appSettings,
        InteractionService interactionService, PrefixHandler prefixHandler, InteractionHandler interactionHandler)
    {
        _discordClient = discordClient;
        _logger = logger;
        _appSettings = appSettings;
        _interactionService = interactionService;
        _prefixHandler = prefixHandler;
        _interactionHandler = interactionHandler;

        _discordClient.Log += Log;
        _discordClient.Ready += ClientReady;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _interactionHandler.InitializeAsync();
        _prefixHandler.AddModule<PrefixModule>();
        await _prefixHandler.InitializeAsync();

        await _discordClient.LoginAsync(TokenType.Bot, _appSettings.Value.Token);
        await _discordClient.StartAsync();
    }

    private async Task ClientReady()
    {
        await _interactionService.RegisterCommandsToGuildAsync(413094045384966144);
        _logger.LogInformation("ConsoleApp Ready!");
    }

    private Task Log(LogMessage logMessage)
    {
        _logger.LogInformation("Discord.Net Log: {message}", logMessage.ToString());
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discordClient.LogoutAsync();
        await _discordClient.StopAsync();
    }
}