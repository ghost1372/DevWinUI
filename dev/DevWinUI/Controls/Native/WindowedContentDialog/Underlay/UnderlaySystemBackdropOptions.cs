namespace DevWinUI;
public sealed partial class UnderlaySystemBackdropOptions
{
    public BackdropType Backdrop { get; set; } = BackdropType.Mica;
    public UnderlayCoverMode CoverMode { get; set; } = UnderlayCoverMode.ClientArea;
    public ScalarTransition OpacityTransition { get; set; } = new ScalarTransition { Duration = TimeSpan.FromSeconds(0.15) };
}
