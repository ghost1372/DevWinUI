namespace DevWinUI;
public sealed partial class UnderlaySmokeLayerOptions
{
    public WindowedContentDialogSmokeLayerKind SmokeLayerKind { get; set; } = WindowedContentDialogSmokeLayerKind.Darken;
    public UIElement? CustomSmokeLayer { get; set; }

    public ScalarTransition OpacityTransition { get; set; } = new ScalarTransition { Duration = TimeSpan.FromSeconds(0.25) };
}
