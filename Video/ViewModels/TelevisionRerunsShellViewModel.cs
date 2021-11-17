namespace MediaHelpers.BlazorCoreLibrary.Video.ViewModels;
public class TelevisionRerunsShellViewModel : ITelevisionShellViewModel
{
    private readonly ITelevisionShellLogic _logic;
    private readonly IDateOnlyPicker _datePicker;
    public TelevisionRerunsShellViewModel(ITelevisionShellLogic logic, IDateOnlyPicker datePicker)
    {
        _logic = logic;
        _datePicker = datePicker;
    }
    public EnumTelevisionHoliday CurrentHoliday { get; private set; } = EnumTelevisionHoliday.None;
    public IEpisodeTable? PreviousEpisode { get; private set; }
    public bool IsLoaded { get; private set; }
    public bool DidReset { get; private set; }

    public async Task InitAsync()
    {
        PreviousEpisode = await _logic.GetPreviousShowAsync();
        if (PreviousEpisode is null)
        {
            CurrentHoliday = _datePicker.GetCurrentDate.WhichHoliday();
        }
        IsLoaded = true;
    }
    public void ResetHoliday()
    {
        CurrentHoliday = EnumTelevisionHoliday.None;
        DidReset = true; //this means will not be holiday because you chose no holiday.
    }
}