using Microsoft.UI.Composition;
using Microsoft.UI.Xaml.Hosting;

namespace DevWinUIGallery.Views;

public sealed partial class CompositionShadowPage : Page
{
    private Compositor _compositor;
    private ManagedSurface _imageMaskSurface;
    private CompositionMaskBrush _maskBrush;
    public CompositionShadowPage()
    {
        InitializeComponent();
        Loaded += CompositionShadowPage_Loaded;
        Unloaded += CompositionShadowPage_Unloaded;
    }

    private void CompositionShadowPage_Unloaded(object sender, RoutedEventArgs e)
    {
        if (_imageMaskSurface != null)
        {
            _imageMaskSurface.Dispose();
        }
    }

    private void CompositionShadowPage_Loaded(object sender, RoutedEventArgs e)
    {
        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        _imageMaskSurface = ImageLoader.Instance.LoadCircle(200, Colors.White);

        CompositionSurfaceBrush source = ComImage.SurfaceBrush as CompositionSurfaceBrush;

        _maskBrush = _compositor.CreateMaskBrush();
        _maskBrush.Mask = _imageMaskSurface.Brush;
        _maskBrush.Source = source;
    }

    private void TGMask_Toggled(object sender, RoutedEventArgs e)
    {
        if (TGMask.IsOn)
        {
            ComImage.Brush = _maskBrush;
            RenderShadow.Mask = _maskBrush.Mask;
        }
        else
        {
            ComImage.Brush = _maskBrush.Source;
            RenderShadow.Mask = null;
        }
    }
}
