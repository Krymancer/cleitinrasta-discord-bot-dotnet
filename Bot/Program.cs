using Application.Extensions;
using Application.Services;
using Bot;
using Configuration.Options;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Handlers;
using Handlers.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RunMode = Discord.Commands.RunMode;

var builder = Host.CreateDefaultBuilder(args);

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

builder
    .ConfigureServices(services =>
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.AddLogging(logging => { logging.AddConsole(); });
        
        services.AddHandlers();
        services.AddServices();
        
        services.AddHostedService<BotService>();

    });

await builder.Build().RunAsync();