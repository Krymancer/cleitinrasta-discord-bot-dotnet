using Commands;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Handlers;

public class InteractionHandler(
    DiscordSocketClient discordClient,
    InteractionService interactionService,
    ILogger<InteractionHandler> logger,
    IServiceProvider serviceProvider)
{
    public async Task InitializeAsync()
    {
        var commandsAssembly = typeof(ICommandReference).Assembly;
        await interactionService.AddModulesAsync(commandsAssembly, serviceProvider);

        discordClient.InteractionCreated += HandleInteraction;

        interactionService.SlashCommandExecuted += SlashCommandExecuted;
        interactionService.ContextCommandExecuted += ContextCommandExecuted;
        interactionService.ComponentCommandExecuted += ComponentCommandExecuted;
    }

    private async Task HandleInteraction(SocketInteraction socketInteraction)
    {
        try
        {
            var context = new SocketInteractionContext(discordClient, socketInteraction);
            await interactionService.ExecuteCommandAsync(context, serviceProvider);
        }
        catch (Exception exception)
        {
            logger.LogError("Exception: {exception}", exception);

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