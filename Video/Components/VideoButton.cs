namespace MediaHelpers.BlazorCoreLibrary.Video.Components;
public class VideoButton : ButtonComponentBase
{
    public override string BackColor => cs.Blue.ToWebColor();
    public override string TextColor => cs.LavenderBlush.ToWebColor();
    public override string DisabledColor => cs.Gray.ToWebColor();
    public override string FontSize => "40px";
}