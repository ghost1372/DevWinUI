using Windows.Foundation;
using Windows.Foundation.Collections;

namespace DevWinUIGallery.Views;

public sealed partial class ColorBloomTransitionPage : Page
{
    private ColorBloomController _bloomController;
    private PropertySet _colorsByKey;
    public ColorBloomTransitionPage()
    {
        this.InitializeComponent();

        InitializeColors();

        _bloomController = new ColorBloomController(HostForVisual, MainGrid, _colorsByKey);
        Unloaded += (_, _) => _bloomController.Dispose();
    }

    private void InitializeColors()
    {
        _colorsByKey = new PropertySet
        {
            { "Pictures", Colors.Orange },
            { "ContactInfo", Colors.Lavender },
            { "Download", Colors.GreenYellow },
            { "Comment", Colors.DeepSkyBlue }
        };
    }
    private void OnClick(object sender, RoutedEventArgs e)
    {
        var button = (FrameworkElement)sender;
        string key = button.Name;

        var origin = button.TransformToVisual(MainGrid).TransformPoint(new Point(0, 0));
        var initialBounds = new Rect(origin, button.RenderSize);

        _bloomController.StartTransition(key, initialBounds, this.RenderSize);
    }


    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _bloomController.ApplyClipping(e.NewSize);
    }
}
