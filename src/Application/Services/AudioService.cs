using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AudioService(DiscordSocketClient client, ILogger<AudioService> logger)
{
    private readonly DiscordSocketClient _client = client;

    public async Task PlayAudioAsync(IVoiceChannel voiceChannel, string audioUrl)
    {
        try
        {
            var audioClient = await voiceChannel.ConnectAsync();
            
            using var ffmpeg = CreateStream(audioUrl);
            await using var output = ffmpeg.StandardOutput.BaseStream;
            await using var discord = audioClient.CreatePCMStream(AudioApplication.Mixed);
            try
            {
                await output.CopyToAsync(discord);
            }
            finally
            {
                await discord.FlushAsync();
            }
        }
        catch (Exception exception)
        {
            logger.LogError("Error playing audio: {message}", exception.Message);
        }
    }

    private Process CreateStream(string url)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ffmpeg",
            Arguments = $"-hide_banner -i {url} -ac 2 -f s16le -ar 48000 pipe:1",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }) ?? throw new InvalidOperationException();
    }
}