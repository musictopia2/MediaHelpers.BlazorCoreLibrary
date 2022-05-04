namespace MediaHelpers.BlazorCoreLibrary.Music.ViewModels;
public class PlaylistSongProgressViewModel : BasicSongProgressViewModel
{
    private readonly IPlaylistSongProgressPlayer _player;
    private readonly IMusicRemoteControlDataAccess _remoteData;
    private readonly IMusicRemoteControlHostService _hostService;
    public PlaylistSongProgressViewModel(IMP3Player mp3,
        IPlaylistSongProgressPlayer player,
        IPrepareSong prepare,
        ChangeSongContainer container,
        IMusicRemoteControlHostService hostService,
        IMusicRemoteControlDataAccess remoteData,
        IToast toast
        ) : base(mp3, player, prepare, container,toast)
    {
        _player = player;
        _hostService = hostService;
        _remoteData = remoteData;
        _player.UpdateProgress = UpdateProgress;
        _hostService.DeleteSong = async () =>
        {
            await remoteData.DeleteSongAsync(CurrentSong!);
        };
        _hostService.IncreaseWeight = async () =>
        {
            await remoteData.IncreaseWeightAsync(CurrentSong!);
        };
        _hostService.DecreaseWeight = async () =>
        {
            await remoteData.DecreaseWeightAsync(CurrentSong!);
        };
    }
    protected override async Task InitPossibleRemoteControl()
    {
        await _hostService.InitializeAsync();
    }
    private async void UpdateProgress()
    {
        SongsLeft = _player.SongsLeft;
        UpTo = _player.UpTo;
        StateChanged?.Invoke(); //hopefully this works (?)
        int weight = _remoteData.GetWeight(CurrentSong!);
        SongModel model = new(CurrentSong!.SongName, CurrentSong.ArtistName, weight, ProgressText);
        await _hostService.SendProgressAsync(model);
    }
    public int UpTo { get; set; }
    public int SongsLeft { get; set; }
}