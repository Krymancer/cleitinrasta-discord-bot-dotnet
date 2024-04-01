using Lavalink4NET;
using Lavalink4NET.NetCord;
using Lavalink4NET.Players;
using Lavalink4NET.Rest.Entities.Tracks;
using NetCord.Services.ApplicationCommands;

namespace App.Modules;

public class MusicModule(IAudioService audioService) : ApplicationCommandModule<SlashCommandContext>
{
    [SlashCommand("play", "Plays a track!")]
    public async Task<string> PlayAsync([SlashCommandParameter(Description = "The query to search for")] string query)
    {
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.Join);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions);

        if (!result.IsSuccess) return GetErrorMessage(result.Status);

        var player = result.Player;

        var track = await audioService.Tracks
            .LoadTrackAsync(query, TrackSearchMode.YouTube);

        if (track is null) return "No tracks found.";

        await player.PlayAsync(track);

        return $"Now playing: {track.Title}";
    }

    [SlashCommand("stop", description: "Stops the current track")]
    public async Task<string> Stop()
    {
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.None);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions);

        if (!result.IsSuccess) return GetErrorMessage(result.Status);

        var player = result.Player;

        if (player.CurrentTrack is null) return ("Nothing playing!");

        await player.StopAsync();
        return "Stopped playing.";
    }

    [SlashCommand("position", description: "Shows the track position")]
    public async Task<string> Position()
    {
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.Join);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions);

        if (!result.IsSuccess) return GetErrorMessage(result.Status);

        var player = result.Player;

        if (player.CurrentTrack is null) return "Nothing playing!";

        return $"Position: {player.Position?.Position} / {player.CurrentTrack.Duration}.";
    }

    [SlashCommand("pause", description: "Pauses the player.")]
    public async Task<string> PauseAsync()
    {
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.Join);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions);

        if (!result.IsSuccess) return GetErrorMessage(result.Status);

        var player = result.Player;

        if (player.State is PlayerState.Paused)
        {
            return "Player is already paused.";
        }

        await player.PauseAsync();
        return "Paused.";
    }

    [SlashCommand("skip", description: "Skips the current track")]
    public async Task<string> Skip()
    {
        var retrieveOptions = new PlayerRetrieveOptions(ChannelBehavior: PlayerChannelBehavior.Join);

        var result = await audioService.Players
            .RetrieveAsync(Context, playerFactory: PlayerFactory.Queued, retrieveOptions);

        if (!result.IsSuccess) return GetErrorMessage(result.Status);

        var player = result.Player;

        if (player.CurrentTrack is null) return "Nothing playing!";

        await player.SkipAsync().ConfigureAwait(false);

        var track = player.CurrentTrack;

        return track is not null ? $"Skipped. Now playing: {track.Uri}" : "Skipped. Stopped playing because the queue is now empty.";
    }

    private static string GetErrorMessage(PlayerRetrieveStatus retrieveStatus) => retrieveStatus switch
    {
        PlayerRetrieveStatus.UserNotInVoiceChannel => "You are not connected to a voice channel.",
        PlayerRetrieveStatus.BotNotConnected => "The bot is currently not connected.",
        _ => "Unknown error.",
    };
}