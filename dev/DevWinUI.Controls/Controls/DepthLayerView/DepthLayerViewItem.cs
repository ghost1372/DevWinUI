namespace DevWinUI;

public partial class DepthLayerViewItem : IDepthLayerViewItem
{
    internal ContainerVisual ItemContainerVisual { get; set; }
    internal LayerVisual LayerVisual { get; set; }
    public Uri ImageUri { get; set; }
}
