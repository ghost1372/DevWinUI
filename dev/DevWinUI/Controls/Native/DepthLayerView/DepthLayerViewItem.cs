namespace DevWinUI;

public partial class DepthLayerViewItem : IDepthLayerViewItem
{
    internal ContainerVisual ItemContainerVisual { get; set; } = null!;
    internal LayerVisual LayerVisual { get; set; } = null!;
    public Uri ImageUri { get; set; } = null!;
}
