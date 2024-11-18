namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
public partial class GooeyEffect : Control
{
    private string PART_Canvas = "PART_Canvas";
    private CanvasControl _Canvas;

    private GaussianBlurEffect _blurEffect;
    private Vector2 _centerPoint;
    private ICanvasImage _image;
    private ICanvasBrush _leftBrush;
    private Vector2Timeline _leftTimeline;
    private ICanvasBrush _rightBrush;
    private Vector2Timeline _rightTimeline;
    private DateTime _startTime;

    private static void OnPropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.Invalidate();
        }
    }
    private static void OnResourcePropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.CreateResources -= ctl.OnCreateResource;
            ctl._Canvas.CreateResources += ctl.OnCreateResource;
            ctl._Canvas.Invalidate();
        }
    }
    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyEffect)d;
        ctl.UpdateAnimation();
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _Canvas = GetTemplateChild(PART_Canvas) as CanvasControl;
        if (_Canvas != null)
        {
            _Canvas.Draw -= OnDraw;
            _Canvas.Draw += OnDraw;
            _Canvas.CreateResources -= OnCreateResource;
            _Canvas.CreateResources += OnCreateResource;
            _Canvas.SizeChanged -= OnCanvasSizeChanged;
            _Canvas.SizeChanged += OnCanvasSizeChanged;
        }
        UpdateAnimation();
    }

    public CanvasControl GetCanvas()
    {
        return _Canvas;
    }
    private void UpdateAnimation()
    {
        var easingFunction = new ExponentialEase { EasingMode = this.Easing };

        _leftTimeline =
            new Vector2Timeline(new Vector2(Convert.ToSingle(SecondaryTimeLineFromX), Convert.ToSingle(SecondaryTimeLineFromY)), new Vector2(Convert.ToSingle(SecondaryTimeLineToX), Convert.ToSingle(SecondaryTimeLineToY)), Duration, null, true, true, easingFunction);
        _rightTimeline =
            new Vector2Timeline(new Vector2(Convert.ToSingle(PrimaryTimeLineFromX), Convert.ToSingle(PrimaryTimeLineFromY)), new Vector2(Convert.ToSingle(PrimaryTimeLineToX), Convert.ToSingle(PrimaryTimeLineToY)), Duration, null, true, true, easingFunction);
    }

    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _centerPoint = _Canvas.ActualSize / 2;
    }

    private void OnCreateResource(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        _startTime = DateTime.Now;
        _leftBrush = new CanvasSolidColorBrush(sender, SecondaryFill);
        _rightBrush = new CanvasSolidColorBrush(sender, PrimaryFill);
        _blurEffect = new GaussianBlurEffect
        {
            BlurAmount = Convert.ToSingle(this.BlurAmount),
        };

        _image = new ColorMatrixEffect
        {
            ColorMatrix = new Matrix5x4
            {
                M11 = 1,
                M12 = 0,
                M13 = 0,
                M14 = 0,
                M21 = 0,
                M22 = 1,
                M23 = 0,
                M24 = 0,
                M31 = 0,
                M32 = 0,
                M33 = 1,
                M34 = 0,
                M41 = 0,
                M42 = 0,
                M43 = 0,
                M44 = 18,
                M51 = 0,
                M52 = 0,
                M53 = 0,
                M54 = -7
            },
            Source = _blurEffect
        };
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var source = new CanvasCommandList(sender);
        var totalTime = DateTime.Now - _startTime;
        using (var drawingSession = source.CreateDrawingSession())
        {
            drawingSession.FillCircle(_centerPoint + _leftTimeline.GetCurrentValue(totalTime), Convert.ToSingle(SecondaryRadius), _leftBrush);
            drawingSession.FillCircle(_centerPoint + _rightTimeline.GetCurrentValue(totalTime), Convert.ToSingle(PrimaryRadius), _rightBrush);
        }

        _blurEffect.Source = source;
        args.DrawingSession.DrawImage(_image);
        _Canvas.Invalidate();
    }
}
