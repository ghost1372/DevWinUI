using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[TemplatePart(Name = CanvasElement, Type = typeof(CanvasControl))]
[ContentProperty(Name = nameof(Content))]
public partial class Hatch : Control
{
    public HatchStyle HatchStyle
    {
        get => (HatchStyle)GetValue(HatchStyleProperty);
        set => SetValue(HatchStyleProperty, value);
    }
    public static readonly DependencyProperty HatchStyleProperty =
        DependencyProperty.Register(nameof(HatchStyle), typeof(HatchStyle), typeof(Hatch), new PropertyMetadata(HatchStyle.Horizontal, OnHatchStyleChanged));
    private static void OnHatchStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Hatch control)
        {
            control.InvalidateCanvas();
        }
    }
    public object Content
    {
        get { return (object)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(object), typeof(Hatch), new PropertyMetadata(null));

    private const string CanvasElement = "PART_Canvas";
    private CanvasControl canvas;
    private CanvasRenderTarget patternTarget;
    private CanvasImageBrush patternBrush;
    private HatchStyle patternStyle;
    private Color patternColor;
    private float patternDpi;

    public Hatch()
    {
        this.DefaultStyleKey = typeof(Hatch);

        RegisterPropertyChangedCallback(ForegroundProperty, (s, dp) => InvalidateCanvas());
        RegisterPropertyChangedCallback(BackgroundProperty, (s, dp) => InvalidateCanvas());
        Unloaded += OnUnloaded;
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (canvas != null)
        {
            canvas.Draw -= OnCanvasDraw;
            canvas.CreateResources -= OnCanvasCreateResources;
        }

        canvas = GetTemplateChild(CanvasElement) as CanvasControl;

        if (canvas != null)
        {
            canvas.Draw += OnCanvasDraw;
            canvas.CreateResources += OnCanvasCreateResources;
        }
    }
    private void OnCanvasCreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        DisposePattern();
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        DisposePattern();
    }
    private void DisposePattern()
    {
        patternBrush?.Dispose();
        patternBrush = null;
        patternTarget?.Dispose();
        patternTarget = null;
    }
    private void InvalidateCanvas()
    {
        if (canvas != null)
        {
            canvas.Invalidate();
        }
    }
    private void OnCanvasDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var hatchStyle = HatchStyle;
        var foreground = (Foreground as SolidColorBrush)?.Color ?? Colors.Transparent;
        var background = (Background as SolidColorBrush)?.Color ?? Colors.Transparent;

        DrawHatchPattern(args.DrawingSession, hatchStyle, foreground, background);
    }
    public float GetAdjustedDpi()
    {
        var baseDpi = 96.0f;

        if (canvas == null)
        {
            return baseDpi;
        }

        var rasterizationScale = GeneralHelper.GetElementRasterizationScale(canvas);
        var adjustedDpi = baseDpi * rasterizationScale;

        return (float)adjustedDpi;
    }
    private void DrawHatchPattern(CanvasDrawingSession session, HatchStyle hatchStyle, Color foreColor, Color backColor)
    {
        // Fill the background
        session.FillRectangle(0, 0, (float)ActualWidth, (float)ActualHeight, backColor);

        if (foreColor.A == 0)
        {
            return;
        }

        session.FillRectangle(0, 0, (float)ActualWidth, (float)ActualHeight, GetPatternBrush(session.Device, hatchStyle, foreColor));
    }
    private CanvasImageBrush GetPatternBrush(CanvasDevice device, HatchStyle hatchStyle, Color foreColor)
    {
        var dpi = GetAdjustedDpi();

        if (patternBrush != null && patternStyle == hatchStyle && patternColor == foreColor && patternDpi == dpi)
        {
            return patternBrush;
        }

        DisposePattern();

        // Get the hatch pattern
        var hatchData = HatchGenerator.GetHatchData(hatchStyle);

        // Define the size of the pattern
        const int patternSize = 8;

        // Create a pixel grid using the hatch data
        patternTarget = new CanvasRenderTarget(device, patternSize, patternSize, dpi);
        using (var offscreenSession = patternTarget.CreateDrawingSession())
        {
            offscreenSession.Clear(Colors.Transparent);
            for (int y = 0; y < patternSize; y++)
            {
                byte row = hatchData[y];
                for (int x = 0; x < patternSize; x++)
                {
                    if ((row & (1 << (7 - x))) != 0) // Check if the bit is set
                    {
                        offscreenSession.FillRectangle(x, y, 1, 1, foreColor);
                    }
                }
            }
        }

        // Use the generated pattern as a tiled brush
        patternBrush = new CanvasImageBrush(device, patternTarget)
        {
            ExtendX = CanvasEdgeBehavior.Wrap,
            ExtendY = CanvasEdgeBehavior.Wrap
        };

        patternStyle = hatchStyle;
        patternColor = foreColor;
        patternDpi = dpi;

        return patternBrush;
    }
}
