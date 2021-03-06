namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public partial class TelevisionHolidayComponent
{
    [Inject]
    private ISystemError? Error { get; set; }
    [Parameter]
    public EventCallback OnBackToMain { get; set; }
    [Parameter]
    public EnumTelevisionHoliday Holiday { get; set; }
    [Inject]
    private TelevisionHolidayViewModel? DataContext { get; set; }
    [Inject]
    private ITelevisionVideoLoader? Loader { get; set; }
    private void ChoseHolidayEpisode(EnumTelevisionLengthType lengthType)
    {
        IEpisodeTable? episode = DataContext?.GetHolidayEpisode(lengthType);
        if (episode is null)
        {
            Error!.ShowSystemError("No episode was chosen even though I chose a holiday episode");
            return;
        }
        Loader!.ChoseEpisode(episode);
    }
    protected override async Task OnInitializedAsync()
    {
        await DataContext!.InitAsync(Holiday);
        if (DataContext.HolidayFullVisible == false && DataContext.HolidayHalfVisible == false)
        {
            await OnBackToMain.InvokeAsync();
        }
    }
}