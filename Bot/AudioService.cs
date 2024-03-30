using System.Diagnostics;
using Discord;
using Discord.Audio;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Bot;

public class AudioService
{
    private readonly ILogger<AudioService> _logger;
    private readonly DiscordSocketClient _client;

    public AudioService(DiscordSocketClient client, ILogger<AudioService> logger)
    {
        _client = client;
        _logger = logger;
    }

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
            _logger.LogError("Error playing audio: {message}", exception.Message);
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