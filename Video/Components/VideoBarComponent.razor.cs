namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public partial class VideoBarComponent<V>
    where V : class
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    [Parameter]
    public VideoMainLoaderViewModel<V>? DataContext { get; set; }
    [Inject]
    private IVideoPlayer? Player { get; set; }
    private bool _loaded = false;
    protected override async Task OnInitializedAsync()
    {
        await DataContext!.InitAsync();
        _loaded = true;
    }
}