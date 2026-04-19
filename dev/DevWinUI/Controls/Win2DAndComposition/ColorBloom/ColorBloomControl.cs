using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
public partial class ColorBloomControl : ContentControl, IDisposable
{
    private Panel panel;
    public event EventHandler<Color> ColorBloomTransitionCompleted;
    private ColorBloomTransitionHelper _transition;

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorBloomControl), new PropertyMetadata(Colors.Transparent));

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        var rectangle = new Rectangle();

        this.panel = new Grid();
        this.panel.Children.Add(rectangle);
        this.Content = panel;

        ImageLoader.Initialize(ElementCompositionPreview.GetElementVisual(rectangle).Compositor);
        _transition = new ColorBloomTransitionHelper(rectangle);
        _transition.ColorBloomTransitionCompleted -= OnTransitionCompleted;
        _transition.ColorBloomTransitionCompleted += OnTransitionCompleted;
    }

    public void StartTransition(FrameworkElement element)
    {
        StartTransition(element, new Size(ActualWidth, ActualHeight));
    }

    public void StartTransition(FrameworkElement element, Size finalSize)
    {
        var origin = element.TransformToVisual(panel).TransformPoint(new Point(0, 0));
        var initialBounds = new Rect(origin, element.RenderSize);

        var finalBounds = new Rect(0, 0, finalSize.Width, finalSize.Height);

        _transition.Start(Color, initialBounds, finalBounds);
    }
    private void OnTransitionCompleted(object sender, EventArgs e)
    {
        ColorBloomTransitionCompleted?.Invoke(this, Color);
    }

    public void ApplyClipping(Size newSize)
    {
        panel.Clip = new RectangleGeometry { Rect = new Rect(0, 0, newSize.Width, newSize.Height) };
    }

    public void Dispose()
    {
        _transition?.Dispose();
    }
}
