using NetCord.Services.ApplicationCommands;

namespace App.Modules;

public class SlashCommandModule : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("pong", "Pong!")]
    public static string Pong() => "Ping!";
}