using Microsoft.Extensions.Hosting;
using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.Commands;
using NetCord.Services.ApplicationCommands;
using NetCord.Services.Commands;
using Lavalink4NET.NetCord;

var builder = Host.CreateDefaultBuilder(args)
    .UseDiscordGateway(options =>
    {
        options.Configuration = new()
        {
            Intents = GatewayIntents.GuildMessages
                      | GatewayIntents.DirectMessages
                      | GatewayIntents.MessageContent
                      | GatewayIntents.DirectMessageReactions
                      | GatewayIntents.GuildMessageReactions,
        };
    })
    .UseLavalink()
    .UseApplicationCommands<SlashCommandInteraction, SlashCommandContext>()
    .UseApplicationCommands<UserCommandInteraction, UserCommandContext>()
    .UseApplicationCommands<MessageCommandInteraction, MessageCommandContext>()
    .UseCommands<CommandContext>();

var host = builder.Build()
    .AddModules(typeof(Program).Assembly)
    .UseGatewayEventHandlers();

await host.RunAsync();