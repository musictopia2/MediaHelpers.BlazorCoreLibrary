using BasicBlazorLibrary.Components.BaseClasses;
namespace MediaHelpers.BlazorCoreLibrary.Music.Components.Playlists;
public class BarButton : ButtonComponentBase
{
    public override string BackColor => cs.Aqua.ToWebColor();
    public override string TextColor => cs.Navy.ToWebColor();
    public override string DisabledColor => cs.Gray.ToWebColor();
    public override string FontSize => "40px"; //try 40 to start with.
}