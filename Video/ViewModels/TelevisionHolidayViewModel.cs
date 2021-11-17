namespace MediaHelpers.BlazorCoreLibrary.Video.ViewModels;
public class TelevisionHolidayViewModel
{
    private readonly ITelevisionHolidayLogic _logic;
    private readonly ISystemError _error;
    public TelevisionHolidayViewModel(ITelevisionHolidayLogic logic, ISystemError error)
    {
        _logic = logic;
        _error = error;
    }
    public string NonHolidayText { get; set; } = "";
    public bool HolidayFullVisible { get; set; }
    public string HolidayFullText { get; set; } = "Full Hour";
    public bool HolidayHalfVisible { get; set; }
    public string HolidayHalfText { get; set; } = "Half Hour";
    public bool IsLoaded { get; private set; }
    internal bool WasHoliday { get; private set; }
    public IEpisodeTable? GetHolidayEpisode(EnumTelevisionLengthType lengthType)
    {
        var episodeList = _holidayList.GetConditionalItems(Items => Items.ShowTable.LengthType == lengthType);
        if (episodeList.Count == 0)
        {
            //can't show error here because it could be while skipping episode.
            //_error.ShowSystemError("No episodes left.  Should have made the option invisible");
            return null;
        }
        IEpisodeTable episode = episodeList.GetRandomItem();
        return episode;
    }
    private BasicList<IEpisodeTable> _holidayList = new();
    public async Task InitAsync(EnumTelevisionHoliday holiday) //has to send in.  so i can mock a holiday if needed to make sure holidays work before they happen.
    {
        IsLoaded = false;
        if (holiday == EnumTelevisionHoliday.None)
        {
            _error.ShowSystemError("Should have never shown the holiday view model because no holiday was chosene");
            return;
        }
        WasHoliday = true;
        try
        {
            _holidayList = await _logic.GetHolidayEpisodeListAsync(holiday);
            string p;
            NonHolidayText = $"Choose Shows With Non {holiday} Episodes";
            if (_holidayList.Exists(items => items.ShowTable.LengthType == EnumTelevisionLengthType.FullHour) == false)
            {
                HolidayFullVisible = false;
            }
            else
            {
                p = HolidayFullText;
                HolidayFullText = $"{p} For {holiday}";
                HolidayFullVisible = true;
            }
            if (_holidayList.Exists(items => items.ShowTable.LengthType == EnumTelevisionLengthType.HalfHour) == false)
            {
                HolidayHalfVisible = false;
            }
            else
            {
                p = HolidayHalfText;
                HolidayHalfText = $"{p} For {holiday}";
                HolidayHalfVisible = true;
            }
            IsLoaded = true;
        }
        catch (Exception ex)
        {
            _error.ShowSystemError(ex.Message);
            throw;
        }
    }
}