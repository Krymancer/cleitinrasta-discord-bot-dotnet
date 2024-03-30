using Bot.Modules;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Discord.Interactions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bot;

public class BotService : BackgroundService
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

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        await _interactionHandler.InitializeAsync();
        _prefixHandler.AddModule<PrefixModule>();
        await _prefixHandler.InitializeAsync();

        await _discordClient.LoginAsync(TokenType.Bot, _appSettings.Value.Token);
        await _discordClient.StartAsync();

        while (!cancellationToken.IsCancellationRequested)
        {
        }

        await _discordClient.StopAsync();
    }

    private async Task ClientReady()
    {
        await _interactionService.RegisterCommandsToGuildAsync(413094045384966144, true);
        _logger.LogInformation("Bot Ready!");
    }

    private Task Log(LogMessage logMessage)
    {
        _logger.LogInformation("Discord.Net Log: {message}", logMessage.ToString());
        return Task.CompletedTask;
    }
}