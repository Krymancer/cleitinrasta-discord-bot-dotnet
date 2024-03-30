using Configuration.Options;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Options;

namespace Handlers;

public class PrefixHandler(
    DiscordSocketClient discordClient,
    CommandService commandService,
    IOptions<AppSettings> appSettings,
    IServiceProvider serviceProvider)
{
    public void AddModule<T>()
    {
        commandService.AddModuleAsync<T>(serviceProvider);
    }

    public Task InitializeAsync()
    {
        discordClient.MessageReceived += HandleCommandAsync;
        return Task.CompletedTask;
    }

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        if (messageParam is not SocketUserMessage message) return;

        var position = 0;

        if (!(message.HasStringPrefix(appSettings.Value.Prefix, ref position) ||
              message.HasMentionPrefix(discordClient.CurrentUser, ref position) || message.Author.IsBot)) return;

        var context = new SocketCommandContext(discordClient, message);

        await commandService.ExecuteAsync(
            context: context,
            argPos: position,
            services: serviceProvider);
    }
}