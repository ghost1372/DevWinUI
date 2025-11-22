namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasAnimatedControl))]
public partial class Countdown : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private CanvasAnimatedControl canvas;

    private ICanvasBrush backgroundDefaultBrush;
    private ICanvasBrush BackgroundSuccessBrush;
    private ICanvasBrush backgroundErrorBrush;
    private ICanvasBrush borderDefaultBrush;
    private CanvasLinearGradientBrush borderGradientBrush;
    private ICanvasBrush borderSuccessBrush;
    private ICanvasBrush borderErrorBrush;
    private const float size = 1000;
    private const float padding = 8;
    private const float radius = size / 2 - padding;
    private const float fullCircle = (float)Math.PI * 2;
    private const float startAngle = fullCircle * 0.75f;
    private static readonly Vector2 centerPoint = new Vector2(size / 2);
    private Vector2 textPosition;
    public Countdown()
    {
        DefaultStyleKey = typeof(Countdown);

        if (BackgroundGradientStops == null)
        {
            BackgroundGradientStops = new List<RadialGradientStopData>();
        }

        if (SuccessBackgroundGradientStops == null)
        {
            SuccessBackgroundGradientStops = new List<RadialGradientStopData>();
        }

        if (ErrorBackgroundGradientStops == null)
        {
            ErrorBackgroundGradientStops = new List<RadialGradientStopData>();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        canvas = GetTemplateChild(PART_Canvas) as CanvasAnimatedControl;

        UpdateClearColor();

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

        if (SuccessBackgroundGradientStops == null || SuccessBackgroundGradientStops.Count == 0)
        {
            var startColor = Color.FromArgb(0x00, 0x42, 0xC9, 0xC5);
            var endColor = Color.FromArgb(0x4D, 0x42, 0xC9, 0xC5);

            SuccessBackgroundGradientStops = new List<RadialGradientStopData>
            {
                new RadialGradientStopData() { Color = startColor, Position = 0.5d },
                new RadialGradientStopData() { Color = endColor, Position = 1 }
            };
        }

        if (ErrorBackgroundGradientStops == null || ErrorBackgroundGradientStops.Count == 0)
        {
            var startColor = Color.FromArgb(0x00, 0xDE, 0x01, 0x99);
            var endColor = Color.FromArgb(0x4D, 0xDE, 0x01, 0x99);

            ErrorBackgroundGradientStops = new List<RadialGradientStopData>
            {
                new RadialGradientStopData() { Color = startColor, Position = 0.5d },
                new RadialGradientStopData() { Color = endColor, Position = 1 }
            };
        }

        if (_textFormat == null)
        {
            _textFormat = new CanvasTextFormat()
            {
                FontFamily = "XamlAutoFontFamily",
                FontSize = size / 2,
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center
            };
        }

        canvas.CreateResources -= OnCanvasCreateResources;
        canvas.CreateResources += OnCanvasCreateResources;

        canvas.Draw -= OnCanvasDraw;
        canvas.Draw += OnCanvasDraw;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }
    private void UpdateClearColor()
    {
        if (canvas == null)
            return;

        canvas.ClearColor = ClearColor;
    }
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (canvas == null)
            return;

        canvas.RemoveFromVisualTree();
        canvas = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            backgroundDefaultBrush.Dispose();
            BackgroundSuccessBrush.Dispose();
            backgroundErrorBrush.Dispose();
            borderDefaultBrush.Dispose();
            borderGradientBrush.Dispose();
            borderSuccessBrush.Dispose();
            borderErrorBrush.Dispose();
            hairlineStrokeStyle.Dispose();
            _textFormat.Dispose();
        }
    }

    private void OnCanvasCreateResources(CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args)
    {
        // Initialize the brushes.
        backgroundDefaultBrush = CreateRadialGradientBrush(sender, BackgroundGradientStops.Select(g => new CanvasGradientStop()
        {
            Color = g.Color,
            Position = (float)g.Position
        }).ToArray());

        BackgroundSuccessBrush = CreateRadialGradientBrush(sender, SuccessBackgroundGradientStops.Select(g => new CanvasGradientStop()
        {
            Color = g.Color,
            Position = (float)g.Position
        }).ToArray());

        backgroundErrorBrush = CreateRadialGradientBrush(sender, ErrorBackgroundGradientStops.Select(g => new CanvasGradientStop()
        {
            Color = g.Color,
            Position = (float)g.Position
        }).ToArray());

        borderDefaultBrush = new CanvasSolidColorBrush(sender, BorderStrokeColor);

        if (BorderStrokeGradiantBrush == null)
        {
            var startColor = Color.FromArgb(0xFF, 0x0F, 0x56, 0xA4);
            var endColor = Color.FromArgb(0xFF, 0x6E, 0xEF, 0xF8);

            BorderStrokeGradiantBrush = new CanvasLinearGradientBrush(sender, startColor, endColor)
            {
                StartPoint = new Vector2(centerPoint.X - radius, centerPoint.Y),
                EndPoint = new Vector2(centerPoint.X + radius, centerPoint.Y)
            };
        }
        borderGradientBrush = BorderStrokeGradiantBrush;

        borderSuccessBrush = new CanvasSolidColorBrush(sender, SuccessBorderStrokeColor);
        borderErrorBrush = new CanvasSolidColorBrush(sender, ErrorBorderStrokeColor);

        // Calculate the text position for vertical centering to account for the
        // fact that text is not vertically centered within its layout bounds. 
        var textLayout = new CanvasTextLayout(sender, "0123456789", _textFormat, 0, 0);
        var drawMidpoint = (float)(textLayout.DrawBounds.Top + (textLayout.DrawBounds.Height / 2));
        var layoutMidpoint = (float)(textLayout.LayoutBounds.Top + (textLayout.LayoutBounds.Height / 2));
        var textPositionDelta = drawMidpoint - layoutMidpoint;
        textPosition = new Vector2(centerPoint.X, centerPoint.Y - textPositionDelta);
    }

    private static ICanvasBrush CreateRadialGradientBrush(ICanvasResourceCreator creator, CanvasGradientStop[] gradientStops)
    {
        return new CanvasRadialGradientBrush(creator, gradientStops, CanvasEdgeBehavior.Clamp, CanvasAlphaMode.Straight)
        {
            Center = centerPoint,
            RadiusX = radius,
            RadiusY = radius
        };
    }

    private void OnCanvasDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        var session = args.DrawingSession;

        // Scale to fit the current window size. 
        var scale = (float)Math.Min(sender.Size.Width, sender.Size.Height) / size;
        session.Transform = Matrix3x2.CreateScale(scale, scale);

        // Calculate key values affecting this draw pass. 
        var numberString = _countdownSeconds.ToString();// Math.Ceiling(secondsRemaining).ToString();

        var sweepAngle = fullCircle - (fullCircle * (_countdownSeconds) / _maxSeconds);
        ICanvasBrush backgroundBrush, borderBrush;
        switch (_state)
        {
            case CountdownState.Success:
                backgroundBrush = BackgroundSuccessBrush;
                borderBrush = borderSuccessBrush;
                break;
            case CountdownState.Error:
                backgroundBrush = backgroundErrorBrush;
                borderBrush = borderErrorBrush;
                break;
            case CountdownState.Normal:
            default:
                backgroundBrush = backgroundDefaultBrush;
                borderBrush = _text == null ? borderGradientBrush : borderDefaultBrush;
                break;
        }

        // Draw the background.
        using (var builder = new CanvasPathBuilder(session))
        {
            builder.BeginFigure(centerPoint);
            builder.AddArc(centerPoint, radius, radius, startAngle, sweepAngle);
            builder.EndFigure(CanvasFigureLoop.Closed);
            using (var geometry = CanvasGeometry.CreatePath(builder))
            {
                session.FillGeometry(geometry, backgroundBrush);
            }
        }

        // Draw the border.
        using (var builder = new CanvasPathBuilder(session))
        {
            builder.BeginFigure(centerPoint.X, centerPoint.Y - radius);
            builder.AddArc(centerPoint, radius, radius, startAngle, sweepAngle);
            builder.EndFigure(CanvasFigureLoop.Open);
            using (var geometry = CanvasGeometry.CreatePath(builder))
            {
                session.DrawGeometry(geometry, borderBrush, 10);
            }
        }

        // Draw the foreground.
        session.DrawText(_text ?? _countdownSeconds.ToString(), textPosition, _textForeground, _textFormat);
    }

    private void UpdateCanvas()
    {
        if (canvas == null)
            return;

        canvas.Invalidate();
    }

    private static readonly CanvasStrokeStyle hairlineStrokeStyle = new CanvasStrokeStyle()
    {
        TransformBehavior = CanvasStrokeTransformBehavior.Hairline
    };
}
