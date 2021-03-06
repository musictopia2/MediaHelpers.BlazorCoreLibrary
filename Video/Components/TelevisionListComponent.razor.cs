namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public partial class TelevisionListComponent
{
    [Inject]
    private TelevisionListViewModel? DataContext { get; set; }
    [Inject]
    private ITelevisionVideoLoader? Loader { get; set; }
    private async Task DoChooseShowAsync()
    {
        IEpisodeTable? ee = await DataContext!.GetEpisodeChosenAsync();
        if (ee is null)
        {
            throw new CustomBasicException("There was no episode chosen");
        }
        Loader!.ChoseEpisode(ee);
    }
}