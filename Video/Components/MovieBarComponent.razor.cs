namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public partial class MovieBarComponent
{
    [Inject]
    private MovieLoaderViewModel? DataContext { get; set; }
    [Inject]
    private IVideoPlayer? Player { get; set; }
    protected override void OnInitialized()
    {
        DataContext!.StateHasChanged = () => InvokeAsync(StateHasChanged);
    }
}