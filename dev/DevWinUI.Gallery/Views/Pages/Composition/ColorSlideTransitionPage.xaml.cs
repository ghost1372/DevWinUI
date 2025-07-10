using Windows.Foundation;
using Windows.Foundation.Collections;

namespace DevWinUIGallery.Views;

public sealed partial class ColorSlideTransitionPage : Page
{
    private PropertySet _colorsByKey;
    private ColorSlideController _slideController;
    public ColorSlideTransitionPage()
    {
        InitializeComponent();

        InitializeColors();

        _slideController = new ColorSlideController(HostForVisual, MainGrid, _colorsByKey);
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

    private void Header_Click(object sender, RoutedEventArgs e)
    {
        var header = (FrameworkElement)sender;

        var headerPosition = header.TransformToVisual(MainGrid).TransformPoint(new Point(0, 0));
        var headerBounds = new Rect(headerPosition, header.RenderSize);

        _slideController.StartTransition(header.Name, headerBounds, this.RenderSize);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _slideController.ApplyClipping(e.NewSize);
    }
}
