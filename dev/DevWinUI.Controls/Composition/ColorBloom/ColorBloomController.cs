using Windows.Foundation.Collections;

namespace DevWinUI;
public partial class ColorBloomController : IDisposable
{
    private readonly UIElement _hostForVisual;
    private readonly Panel _uiCanvas;
    private readonly Action<Color> _onColorApplied;
    private readonly Queue<string> _pendingTransitions = new();
    private readonly PropertySet _colorsByKey;
    private ColorBloomTransitionHelper _transition;

    public ColorBloomController(UIElement hostForVisual, Panel panel, PropertySet colorsByKey, Action<Color> onColorApplied = null)
    {
        _hostForVisual = hostForVisual;
        _uiCanvas = panel;
        _onColorApplied = onColorApplied;
        _colorsByKey = colorsByKey ?? throw new ArgumentNullException(nameof(colorsByKey));

        InitializeTransitionHelper();
    }

    private void InitializeTransitionHelper()
    {
        ImageLoader.Initialize(ElementCompositionPreview.GetElementVisual(_hostForVisual).Compositor);
        _transition = new ColorBloomTransitionHelper(_hostForVisual);
        _transition.ColorBloomTransitionCompleted += OnTransitionCompleted;
    }

    public void StartTransition(string key, Rect initialBounds, Size finalSize)
    {
        if (!_colorsByKey.ContainsKey(key))
            return;

        var finalBounds = new Rect(0, 0, finalSize.Width, finalSize.Height);
        var color = (Color)_colorsByKey[key];

        _transition.Start(color, initialBounds, finalBounds);
        _pendingTransitions.Enqueue(key);
    }

    private void OnTransitionCompleted(object sender, EventArgs e)
    {
        if (_pendingTransitions.Count == 0) return;

        var key = _pendingTransitions.Dequeue();
        var color = (Color)_colorsByKey[key];

        if (_uiCanvas != null)
        {
            _uiCanvas.Background = new SolidColorBrush(color);
        }

        _onColorApplied?.Invoke(color);
    }

    public void ApplyClipping(Size newSize)
    {
        _uiCanvas.Clip = new RectangleGeometry { Rect = new Rect(0, 0, newSize.Width, newSize.Height) };
    }

    public void Dispose()
    {
        _transition?.Dispose();
    }
}
