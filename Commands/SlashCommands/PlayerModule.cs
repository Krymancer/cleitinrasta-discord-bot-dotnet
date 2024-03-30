using Application.Services;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using VideoLibrary;

namespace Commands.SlashCommands;

public class PlayerModule(ILogger<PlayerModule> logger, DiscordSocketClient client, AudioService audioService)
    : InteractionModuleBase<SocketInteractionContext>
{
    public InteractionService? Commands { get; set; }

    private readonly DiscordSocketClient _discordClient = client;

    [SlashCommand("play", "Play a song")]
    public async Task Play([Summary("url", "The video url to play")] string url)
    {
        logger.LogInformation("User: {user}, Command: play, parameter: {url}", Context.User.Username, url);

        var streamUrl = await GetAudioStreamUrl(url);

        if (string.IsNullOrEmpty(streamUrl))
        {
            await RespondAsync("Failed to retrieve audio stream.");
            return;
        }

        var userVoiceState = Context.User as IVoiceState;
        var voiceChannel = userVoiceState?.VoiceChannel;

        if (voiceChannel is null)
        {
            await RespondAsync("You must be in a voice channel to use this command.");
            return;
        }

        await RespondAsync("Playing Now!");
        await audioService.PlayAudioAsync(voiceChannel, streamUrl);
    }

    private async Task<string> GetAudioStreamUrl(string url)
    {
        try
        {
            var youTube = YouTube.Default;
            var videos = await youTube.GetAllVideosAsync(url);

            var audio = videos
                .FirstOrDefault(i => i.AudioFormat == AudioFormat.Aac && i.AudioBitrate == 128);

            if (audio is null)
            {
                logger.LogError("Error trying to get audio from youtube video");
                return string.Empty;
            }
            
            return await audio.GetUriAsync();
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Error retrieving audio stream URL from YouTube: {exception}", exception);
            return string.Empty;
        }
    }
}