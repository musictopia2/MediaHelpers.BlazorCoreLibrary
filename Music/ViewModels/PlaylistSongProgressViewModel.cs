namespace MediaHelpers.BlazorCoreLibrary.Music.ViewModels;
public class PlaylistSongProgressViewModel : BasicSongProgressViewModel
{
    private readonly IPlaylistSongProgressPlayer _player;
    public PlaylistSongProgressViewModel(IMP3Player mp3,
        IPlaylistSongProgressPlayer player,
        IPrepareSong prepare,
        ChangeSongContainer container,
        IToast toast
        ) : base(mp3, player, prepare, container,toast)
    {
        _player = player;
        _player.UpdateProgress = UpdateProgress;
    }
    private void UpdateProgress()
    {
        SongsLeft = _player.SongsLeft;
        UpTo = _player.UpTo;
        StateChanged?.Invoke();
    }
    public int UpTo { get; set; }
    public int SongsLeft { get; set; }
}