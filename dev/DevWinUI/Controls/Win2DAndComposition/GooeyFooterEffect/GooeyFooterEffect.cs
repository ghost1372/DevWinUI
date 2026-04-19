// https://github.com/cnbluefire

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
public partial class GooeyFooterEffect : Control
{
    private string PART_Canvas = "PART_Canvas";
    private CanvasControl _Canvas;
    private static Random random = new Random();

    private static int unit = 16;

    private GaussianBlurEffect _blurEffect;
    private ICanvasBrush _brush;
    private List<GooeyBubble> _bubbles;
    private Vector2 _centerPoint;
    private ICanvasImage _image;
    private DateTime _startTime;

    private static double _timeLineFromDefaultValue = -(6 + random.NextDouble() * 4) * unit;
    private static double _sizeTimeLineFromDefaultValue = (2 + random.NextDouble() * 4) * unit;
    private static double _timeLineToDefaultValue = 10 * unit;
    private static double _durationDefaultValue = 2 + random.NextDouble() * 2;
    private static double _sizeDurationDefaultValue = 2 + random.NextDouble() * 2;

    private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyFooterEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.Invalidate();
        }
    }

    private static void OnResourcePropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyFooterEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.CreateResources -= ctl.OnCreateResource;
            ctl._Canvas.CreateResources += ctl.OnCreateResource;
            ctl._Canvas.Invalidate();
        }
    }

    private static void OnBubbleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (GooeyFooterEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl.UpdateBubble();
            ctl._Canvas.Invalidate();
        }
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
        UpdateBubble();
    }

    private void UpdateBubble()
    {
        _bubbles = new List<GooeyBubble>();
        for (var i = 0; i < Bubble; i++)
        {
            var delayRnd = random.NextDouble();
            var delay = BeginTime.TotalSeconds + delayRnd * 2;
            var sizeDelay = SizeBeginTime.TotalSeconds + delayRnd * 2;

            var rnd = random.NextDouble();
            var second = Convert.ToDouble(Duration + rnd * 2);
            var sizeSecond = Convert.ToDouble(SizeDuration + rnd * 2);

            var offsetTimeline = new DoubleTimeline(TimeLineFrom, TimeLineTo, second, TimeSpan.FromSeconds(delay), false);
            var sizeTimeline = new DoubleTimeline(SizeTimeLineFrom, SizeTimeLineTo, sizeSecond, TimeSpan.FromSeconds(sizeDelay), false);
            var x = random.NextDouble();
            _bubbles.Add(new GooeyBubble { X = x, OffsetTimeline = offsetTimeline, SizeTimeline = sizeTimeline });
        }
    }
    public CanvasControl GetCanvas()
    {
        return _Canvas;
    }
    private void OnCanvasSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _centerPoint = _Canvas.ActualSize / 2;
    }

    private void OnCreateResource(CanvasControl sender, CanvasCreateResourcesEventArgs args)
    {
        _startTime = DateTime.Now;
        _brush = new CanvasSolidColorBrush(sender, Fill);
        _blurEffect = new GaussianBlurEffect
        {
            BlurAmount = Convert.ToSingle(this.BlurAmount)
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
                M44 = 19,
                M51 = 0,
                M52 = 0,
                M53 = 0,
                M54 = -9
            },
            ClampOutput = true,
            Source = _blurEffect
        };
    }

    private void OnDraw(CanvasControl sender, CanvasDrawEventArgs args)
    {
        var source = new CanvasCommandList(sender);
        var totalTime = DateTime.Now - _startTime;
        using (var darwingSession = source.CreateDrawingSession())
        {
            darwingSession.FillRectangle(Convert.ToSingle(X), _centerPoint.Y, _centerPoint.X * 2 + Convert.ToSingle(W), _centerPoint.Y + Convert.ToSingle(H), _brush);

            foreach (var bubble in _bubbles)
            {
                var x = bubble.X * _centerPoint.X * 2;
                var y = _centerPoint.Y - bubble.OffsetTimeline.GetCurrentProgress(totalTime);
                var size = bubble.SizeTimeline.GetCurrentProgress(totalTime);
                darwingSession.FillCircle(new Vector2((float)x, (float)y), (float)size, _brush);
            }
        }

        _blurEffect.Source = source;
        args.DrawingSession.DrawImage(_image);
        _Canvas.Invalidate();
    }
}
