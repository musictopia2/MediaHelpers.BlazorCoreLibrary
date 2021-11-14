﻿namespace MediaHelpers.BlazorCoreLibrary.Video.ViewModels;
public abstract class VideoMainLoaderViewModel<V> : IVideoPlayerViewModel
    where V : class
{
    public VideoMainLoaderViewModel(IVideoPlayer player, ISystemError error, IExit exit)
    {
        Player = player;
        _error = error;
        _exit = exit;
    }
    protected IVideoPlayer Player { get; }
    public V? SelectedItem { get; protected set; }
    public async Task InitAsync()
    {
        await BeforePlayerInitAsync();
        Player.Init();
        await AfterPlayerInitAsync();
    }
    protected virtual Task AfterPlayerInitAsync()
    {
        return Task.CompletedTask;
    }
    protected virtual Task BeforePlayerInitAsync()
    {
        Player.ErrorRaised += ThisPlayer_ErrorRaised;
        Player.SaveResume += ThisPlayer_SaveResume;
        Player.Progress += ThisPlayer_Progress;
        Player.MediaEnded += ThisPlayer_MediaEnded;
        return Task.CompletedTask;
    }
    public int VideoLength { get; set; }
    public bool FullScreen
    {
        get
        {
            return Player.FullScreen;
        }
        set
        {
            if (VideoGlobal.IsTesting)
            {
                value = false;
            }
            Player.FullScreen = value;
        }
    }
    public int VideoPosition { get; set; }
    public string VideoPath
    {
        get => Player.Path;
        set => Player.Path = value;
    }
    public int ResumeSecs { get; set; }
    public string ProgressText { get; set; } = "";
    protected async Task ShowVideoLoadedAsync()
    {
        if (VideoLength == 0)
        {
            VideoLength = Player.Length();
        }
        if (VideoLength == -1)
        {
            _error.ShowSystemError("Movie Length can't be -1");
            return;
        }
        if (VideoPosition > VideoLength)
        {
            await VideoFinishedAsync();
            return;
        }
        FullScreen = true;
        await Player.PlayAsync(VideoLength, VideoPosition);
    }
    public bool PlayButtonVisible { get; set; }
    public bool CloseButtonVisible { get; set; }
    public Action? StateHasChanged { get; set; }
    public void PlayPause()
    {
        Player.Pause();
    }
    public async Task CloseScreenAsync()
    {
        await SaveProgressAsync();
        _exit.ExitApp();
    }
    public abstract Task SaveProgressAsync();
    public abstract Task VideoFinishedAsync();
    private async void ThisPlayer_MediaEnded()
    {
        await VideoFinishedAsync();
    }
    private int _attempts;
    private readonly ISystemError _error;
    private readonly IExit _exit;
    private async void ThisPlayer_Progress(string timeElapsed, string totalTime)
    {
        try
        {
            int els = Player!.TimeElapsed();
            els += 3;
            if (els < ResumeSecs && ResumeSecs > 5 && els <= 5)
            {
                _attempts++;
                if (_attempts > 10)
                {
                    _error.ShowSystemError("Somehow; its failing to autoresume no matter what.  Already tried 10 times.  Therefore; its being closed out");
                    return;
                }
                await Player.PlayAsync(VideoLength, ResumeSecs);
            }
            VideoPosition = Player.TimeElapsed();
            ProgressText = $"{timeElapsed}/{totalTime}";
            if (Player.IsPaused() == false)
            {
                await SendOtherDataAsync();
            }
            StateHasChanged?.Invoke();
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
        }
    }
    protected virtual Task SendOtherDataAsync() { return Task.CompletedTask; }
    private async void ThisPlayer_SaveResume(int newSecs)
    {
        VideoPosition = newSecs;
        await SaveProgressAsync();
    }
    private void ThisPlayer_ErrorRaised(string message)
    {
        _error.ShowSystemError(message);
    }
}