namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public partial class TelevisionBarComponent
{
    [Inject]
    private TelevisionLoaderViewModel? DataContext { get; set; }
    protected override void OnInitialized()
    {
        DataContext!.StateHasChanged = () => InvokeAsync(StateHasChanged);
    }
}