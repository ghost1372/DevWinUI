using Windows.Foundation.Collections;

namespace DevWinUI;
public partial class ColorSlideController
{
    private readonly UIElement _hostForVisual;
    private readonly Panel _uiCanvas;
    private readonly PropertySet _colorsByKey;
    private readonly ColorSlideTransitionHelper _transition;

    public ColorSlideController(UIElement hostForVisual, Panel panel, PropertySet colorsByKey)
    {
        _hostForVisual = hostForVisual ?? throw new ArgumentNullException(nameof(hostForVisual));
        _uiCanvas = panel ?? throw new ArgumentNullException(nameof(panel));
        _colorsByKey = colorsByKey ?? throw new ArgumentNullException(nameof(colorsByKey));

        _transition = new ColorSlideTransitionHelper(_hostForVisual);
    }

    public void StartTransition(string key, Rect headerBounds, Size containerSize)
    {
        if (!_colorsByKey.ContainsKey(key))
            return;

        var color = (Color)_colorsByKey[key];

        // Compute final bounds excluding the header height
        var finalBounds = new Rect
        {
            Width = containerSize.Width,
            Height = containerSize.Height - headerBounds.Height,
            X = 0,
            Y = headerBounds.Y + headerBounds.Height
        };

        _transition.Start(color, finalBounds);
    }

    public void ApplyClipping(Size newSize)
    {
        var clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, newSize.Width, newSize.Height)
        };

        _uiCanvas.Clip = clip;
    }
}

