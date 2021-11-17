﻿namespace MediaHelpers.BlazorCoreLibrary.Video.ViewModels;
public class TelevisionLoaderViewModel : VideoMainLoaderViewModel<IEpisodeTable>
{
    private readonly ITelevisionLoaderLogic _loadLogiclogic;
    private readonly TelevisionHolidayViewModel _holidayViewModel;
    private readonly ITelevisionRemoteControlHostService _hostService;
    private readonly ITelevisionListLogic _listLogic;
    private readonly ISystemError _error;
    private readonly IToast _toast;
    private readonly bool _wasHoliday;
    public TelevisionLoaderViewModel(IVideoPlayer player,
        ITelevisionLoaderLogic loadLogic,
        TelevisionHolidayViewModel holidayViewModel,
        TelevisionContainerClass containerClass,
        ITelevisionRemoteControlHostService hostService,
        ITelevisionListLogic listLogic,
        ISystemError error,
        IToast toast,
        IExit exit

        ) : base(player, error, exit)
    {
        _loadLogiclogic = loadLogic;
        _holidayViewModel = holidayViewModel;
        _hostService = hostService;
        _listLogic = listLogic;
        _error = error;
        _toast = toast;
        if (containerClass.EpisodeChosen is null)
        {
            throw new CustomBasicException("There was no episode chosen.  Rethink");
        }
        _wasHoliday = containerClass.EpisodeChosen.Holiday != EnumTelevisionHoliday.None;
        _hostService.NewClient = SendOtherDataAsync;
        _hostService.SkipEpisodeForever = async () =>
        {
            await SkipEpisodeForeverAsync();
        };
        _hostService.ModifyHoliday = async (item) =>
        {
            await ModifyHolidayAsync(item);
        };
        SelectedItem = containerClass.EpisodeChosen;
    }
    private async Task ModifyHolidayAsync(EnumTelevisionHoliday holiday)
    {
        if (holiday == SelectedItem!.Holiday)
        {
            _toast.ShowInfoToast("No holiday change");
            return;
        }
        var tempItem = StopEpisode();
        await _loadLogiclogic.ModifyHolidayAsync(tempItem, holiday);
        await StartNextEpisodeAsync(tempItem);
    }
    private async Task SkipEpisodeForeverAsync()
    {
        var tempItem = StopEpisode();
        await _loadLogiclogic.ForeverSkipEpisodeAsync(tempItem);
        await StartNextEpisodeAsync(tempItem);
    }
    private IEpisodeTable StopEpisode()
    {
        ResumeSecs = 0;
        VideoPosition = 0;
        if (SelectedItem is null)
        {
            throw new CustomBasicException("No episode was even chosen");
        }
        var tempItem = SelectedItem;
        SelectedItem = null;
        if (tempItem is null)
        {
            throw new CustomBasicException("The temp item is null.  Wrong");
        }
        Player.StopPlay();
        return tempItem;
    }
    private async Task StartNextEpisodeAsync(IEpisodeTable tempItem) //try this way.
    {
        IShowTable show = tempItem.ShowTable;
        SelectedItem = _wasHoliday ? await _listLogic.GetNextEpisodeAsync(show) : _holidayViewModel.GetHolidayEpisode(show.LengthType);
        if (SelectedItem is null)
        {
            return;
        }
        BeforeInitEpisode();
        await _loadLogiclogic.AddToHistoryAsync(SelectedItem!); 
        await ProcessSkipsAsync();
        await ShowVideoLoadedAsync();
    }
    public override Task SaveProgressAsync()
    {
        return _loadLogiclogic.UpdateTVShowProgressAsync(SelectedItem!, VideoPosition);
    }
    public override Task VideoFinishedAsync()
    {
        return _loadLogiclogic.FinishTVEpisodeAsync(SelectedItem!);
    }
    private bool _hasIntro;
    private void BeforeInitEpisode()
    {
        int secs = _loadLogiclogic.GetSeconds(SelectedItem!);
        ResumeSecs = secs;
        VideoPosition = ResumeSecs;
        VideoPath = SelectedItem!.FullPath();
        _hasIntro = SelectedItem.BeginAt > 0;
    }
    protected override async Task BeforePlayerInitAsync()
    {
        try
        {
            await base.BeforePlayerInitAsync();
            BeforeInitEpisode();
            await _loadLogiclogic.AddToHistoryAsync(SelectedItem!);
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
        }
    }
    private (int startTime, int howLong) GetSkipData()
    {
        return (SelectedItem!.BeginAt, SelectedItem!.OpeningLength!.Value);
    }
    private async Task ProcessSkipsAsync()
    {
        if (_hasIntro)
        {
            var (StartTime, HowLong) = GetSkipData();
            SkipSceneClass skip = new()
            {
                StartTime = StartTime,
                HowLong = HowLong
            };
            var list = new BasicList<SkipSceneClass> { skip };
            Player.AddScenesToSkip(list);
        }
        var tvLength = Player.Length();
        await CalculateDurationAsync(tvLength);
    }
    protected override async Task AfterPlayerInitAsync()
    {
        try
        {
            await ProcessSkipsAsync();

            await _hostService.InitializeAsync();
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
        }
    }
    private async Task CalculateDurationAsync(int tvLength)
    {
        int newLength;
        TimeSpan thisSpan = TimeSpan.FromSeconds(tvLength);
        if (thisSpan.Minutes >= 20 && SelectedItem!.ClosingLength.HasValue == true)
        {
            newLength = tvLength - SelectedItem.ClosingLength!.Value;
        }
        else
        {
            newLength = tvLength;
        }
        VideoLength = newLength;
        await ShowVideoLoadedAsync();
    }
    protected override Task SendOtherDataAsync()
    {
        return _hostService.SendProgressAsync(new TelevisionModel(SelectedItem!.ShowTable.ShowName, ProgressText, SelectedItem.Holiday!));
    }
}