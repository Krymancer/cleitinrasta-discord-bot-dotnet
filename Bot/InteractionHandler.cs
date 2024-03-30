using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Interactions;
using Microsoft.Extensions.Logging;

namespace Bot;

public class InteractionHandler
{
    private readonly ILogger<InteractionHandler> _logger;
    private readonly DiscordSocketClient _discordClient;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _serviceProvider;

    public InteractionHandler(DiscordSocketClient discordClient, InteractionService interactionService,
        ILogger<InteractionHandler> logger, IServiceProvider serviceProvider)
    {
        _discordClient = discordClient;
        _interactionService = interactionService;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeAsync()
    {
        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider);

        _discordClient.InteractionCreated += HandleInteraction;
        
        _interactionService.SlashCommandExecuted += SlashCommandExecuted;
        _interactionService.ContextCommandExecuted += ContextCommandExecuted;
        _interactionService.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var context = new SocketInteractionContext(_discordClient, socketInteraction);
            await _interactionService.ExecuteCommandAsync(context, _serviceProvider);
        }
        catch (Exception exception)
        {
            _logger.LogError("Exception: {exception}", exception);

            if (socketInteraction.Type == InteractionType.ApplicationCommand)
                await socketInteraction.GetOriginalResponseAsync()
                    .ContinueWith(async (message) => await message.Result.DeleteAsync());
        }
    }

    private Task ComponentCommandExecuted(ComponentCommandInfo componentCommandInfo,
        Discord.IInteractionContext interactionContext, IResult result)
    {
        return Task.CompletedTask;
    }

    private Task ContextCommandExecuted(ContextCommandInfo contextCommandInfo,
        Discord.IInteractionContext interactionContext, IResult result)
    {
        return Task.CompletedTask;
    }

    private Task SlashCommandExecuted(SlashCommandInfo slashCommandInfo, Discord.IInteractionContext interactionContext,
        IResult result)
    {
        return Task.CompletedTask;
    }
}