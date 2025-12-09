using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;
public partial class ColorSlideControl : ContentControl
{
    private Panel panel;
    public event EventHandler<Color> ColorSlideTransitionCompleted;
    private ColorSlideTransitionHelper _transition;

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorSlideControl), new PropertyMetadata(Colors.Transparent));

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

        _transition = new ColorSlideTransitionHelper(rectangle);

        _transition.ColorSlideTransitionCompleted -= OnTransitionCompleted;
        _transition.ColorSlideTransitionCompleted += OnTransitionCompleted;
    }

    public void StartTransition(FrameworkElement element)
    {
        StartTransition(element, new Size(ActualWidth, ActualHeight));
    }

    public void StartTransition(FrameworkElement element, Size containerSize)
    {
        var origin = element.TransformToVisual(panel).TransformPoint(new Point(0, 0));
        var initialBounds = new Rect(origin, element.RenderSize);

        var finalBounds = new Rect
        {
            Width = containerSize.Width,
            Height = containerSize.Height - initialBounds.Height,
            X = 0,
            Y = initialBounds.Y + initialBounds.Height
        };

        _transition.Start(Color, finalBounds);
    }
    private void OnTransitionCompleted(object sender, EventArgs e)
    {
        ColorSlideTransitionCompleted?.Invoke(this, Color);
    }

    public void ApplyClipping(Size newSize)
    {
        var clip = new RectangleGeometry
        {
            Rect = new Rect(0, 0, newSize.Width, newSize.Height)
        };

        panel.Clip = clip;
    }
}

