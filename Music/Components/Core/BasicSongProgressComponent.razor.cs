namespace MediaHelpers.BlazorCoreLibrary.Music.Components.Core;
public partial class BasicSongProgressComponent
{
    [Inject]
    private BasicSongProgressViewModel? DataContext { get; set; }
    [Parameter]
    public RenderFragment? ChildContent { get; set; }
    protected override void OnInitialized()
    {
        DataContext!.Start();
        DataContext!.StateChanged = () => InvokeAsync(StateHasChanged);
    }
}