using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
[ContentProperty(Name = nameof(Content))]
public partial class CircleIcon : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private CanvasControl canvas;

    public Color ClearColor
    {
        get { return (Color)GetValue(ClearColorProperty); }
        set { SetValue(ClearColorProperty, value); }
    }

    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(CircleIcon), new PropertyMetadata(Colors.Transparent, OnClearColorChanged));

    private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CircleIcon)d;
        if (ctl != null)
        {
            ctl.UpdateClearColor();
        }
    }
    public double BorderStrokeThickness
    {
        get { return (double)GetValue(BorderStrokeThicknessProperty); }
        set { SetValue(BorderStrokeThicknessProperty, value); }
    }

    public static readonly DependencyProperty BorderStrokeThicknessProperty =
        DependencyProperty.Register(nameof(BorderStrokeThickness), typeof(double), typeof(CircleIcon), new PropertyMetadata(2.0d, OnPropertyChanged));

    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(CircleIcon), new PropertyMetadata(null));

    public Color BorderStrokeColor
    {
        get { return (Color)GetValue(BorderStrokeColorProperty); }
        set { SetValue(BorderStrokeColorProperty, value); }
    }

    public static readonly DependencyProperty BorderStrokeColorProperty =
        DependencyProperty.Register(nameof(BorderStrokeColor), typeof(Color), typeof(CircleIcon), new PropertyMetadata(Color.FromArgb(0xFF, 0x6E, 0xEF, 0xF8), OnPropertyChanged));

    public IList<RadialGradientStopData> BackgroundGradientStops
    {
        get => (IList<RadialGradientStopData>)GetValue(BackgroundGradientStopsProperty);
        set => SetValue(BackgroundGradientStopsProperty, value);
    }

    public static readonly DependencyProperty BackgroundGradientStopsProperty =
        DependencyProperty.Register(nameof(BackgroundGradientStops), typeof(IList<RadialGradientStopData>), typeof(CircleIcon), new PropertyMetadata(null, OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CircleIcon)d;
        if (ctl != null)
        {
            ctl.UpdateCanvas();
        }
    }
    
    public CircleIcon()
    {
        DefaultStyleKey = typeof(CircleIcon);

        if (BackgroundGradientStops == null)
        {
            BackgroundGradientStops = new List<RadialGradientStopData>();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        canvas = GetTemplateChild(PART_Canvas) as CanvasControl;

        if (BackgroundGradientStops == null || BackgroundGradientStops.Count == 0)
        {
            var startColor = Color.FromArgb(0x00, 0x6E, 0xEF, 0xF8);
            var endColor = Color.FromArgb(0x4D, 0x6E, 0xEF, 0xF8);
            BackgroundGradientStops = new List<RadialGradientStopData>
            {
                new RadialGradientStopData() { Color = startColor, Position = 0.5d },
                new RadialGradientStopData() { Color = endColor, Position = 1 }
            };
        }

        UpdateClearColor();

        canvas.Draw -= OnCanvasDraw;
        canvas.Draw += OnCanvasDraw;
    }
    private void UpdateClearColor()
    {
        if (canvas == null)
            return;

        canvas.ClearColor = ClearColor;
    }

    private void UpdateCanvas()
    {
        if (canvas == null)
            return;

        canvas.Invalidate();
    }
    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var width = (float)sender.ActualWidth;
        var height = (float)sender.ActualHeight;
        var center = new Vector2(width / 2, height / 2);
        var radius = Math.Min(width, height) / 2 - (float)BorderStrokeThickness;

        var canvasStops = BackgroundGradientStops.Select(g => new CanvasGradientStop()
        {
            Color = g.Color,
            Position = (float)g.Position
        }).ToArray();
        var brush = new CanvasRadialGradientBrush(canvas, canvasStops, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Straight)
        {
            Center = center,
            RadiusX = radius,
            RadiusY = radius
        };
        args.DrawingSession.FillCircle(center, radius, brush);
        args.DrawingSession.DrawCircle(center, radius, BorderStrokeColor, (float)BorderStrokeThickness);
    }
}
