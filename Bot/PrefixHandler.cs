using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Bot;

public class PrefixHandler
{
    private readonly DiscordSocketClient _discordClient;
    private readonly CommandService _commandService;
    private readonly IOptions<AppSettings> _appSettings;
    private readonly IServiceProvider _serviceProvider;

    public PrefixHandler(DiscordSocketClient discordClient, CommandService commandService,
        IOptions<AppSettings> appSettings, IServiceProvider serviceProvider)
    {
        _discordClient = discordClient;
        _commandService = commandService;
        _appSettings = appSettings;
        _serviceProvider = serviceProvider;
    }

    public void AddModule<T>()
    {
        _commandService.AddModuleAsync<T>(_serviceProvider);
    }

    public Task InitializeAsync()
    {
        _discordClient.MessageReceived += HandleCommandAsync;
        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return;

        var position = 0;

        if (!(message.HasStringPrefix(_appSettings.Value.Prefix, ref position) ||
              message.HasMentionPrefix(_discordClient.CurrentUser, ref position) || message.Author.IsBot)) return;

        var context = new SocketCommandContext(_discordClient, message);

        await _commandService.ExecuteAsync(
            context: context,
            argPos: position,
            services: _serviceProvider);
    }
}