using Bot;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RunMode = Discord.Commands.RunMode;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var builder = new HostBuilder()
    .ConfigureServices(services =>
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddLogging(logging => { logging.AddConsole(); });

        services.AddSingleton<DiscordSocketClient>(_ =>
        {
            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true
            });
            return client;
        });

        services.AddSingleton<CommandService>(_ =>
        {
            var service = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info,
                DefaultRunMode = RunMode.Async
            });

            return service;
        });

        services.AddSingleton<AudioService>();
        services.AddSingleton<PrefixHandler>();
        services.AddSingleton<InteractionService>();
        services.AddSingleton<InteractionHandler>();
        services.AddHostedService<BotService>();
    });

await builder.RunConsoleAsync();