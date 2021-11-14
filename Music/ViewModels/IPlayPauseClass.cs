namespace MediaHelpers.BlazorCoreLibrary.Music.ViewModels;
public interface IPlayPauseClass
{
    void PlayPause();
    bool CanPause { get; }
}