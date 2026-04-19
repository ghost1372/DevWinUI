// https://github.com/DinoChan

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(CanvasControl))]
public partial class TextMorphEffect : Control
{
    private string PART_Canvas = "PART_Canvas";
    private CanvasControl _Canvas;

    private GaussianBlurEffect _blurEffect;
    private Vector2 _centerPoint;
    private ColorMatrixEffect _colorMatrixEffect;
    private List<TextMorphItem> _morphItems;

    private CanvasTextFormat _textFormat;
    private string[] _texts;

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextMorphEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            if (double.IsNaN(ctl.BlurAmount))
            {
                ctl.BlurAmount = 0f;
            }
            ctl._Canvas.Invalidate();
        }
    }

    private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextMorphEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl.UpdateTextMorph();
            ctl._Canvas.Invalidate();
        }
    }
    private static void OnResourcePropertyValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextMorphEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl._Canvas.CreateResources -= ctl.OnCreateResource;
            ctl._Canvas.CreateResources += ctl.OnCreateResource;
            ctl._Canvas.Invalidate();
        }
    }
    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (TextMorphEffect)d;
        if (ctl != null && ctl._Canvas != null)
        {
            ctl.UpdateTextMorph();
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

        UpdateTextMorph();
    }
    private void UpdateTextMorph()
    {
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        _texts = Text.Split(Delimiter);

        var i = 0;
        var easingFunction = new QuarticEase { EasingMode = this.Easing };
        _morphItems = _texts.Select(t =>
        {
            return new TextMorphItem
            {
                Text = t,
                Timeline = new DoubleTimeline(TimeLineFrom, TimeLineTo, Duration, TimeSpan.FromSeconds(BeginTime.TotalSeconds + i++ * 1.7), this.AutoReverse, false,
                    easingFunction)
            };
        }).Reverse().ToList();

        _textFormat = new CanvasTextFormat
        {
            FontSize = Convert.ToSingle(FontSize),
            Direction = this.Direction,
            VerticalAlignment = this.VerticalAlignment,
            HorizontalAlignment = this.HorizontalAlignment,
            FontWeight = this.FontWeight,
            FontFamily = FontFamily.Source
        };
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
        if (double.IsNaN(BlurAmount))
        {
            BlurAmount = 0f;
        }

        _blurEffect = new GaussianBlurEffect
        {
            BlurAmount = Convert.ToSingle(this.BlurAmount)
        };

        _colorMatrixEffect = new ColorMatrixEffect
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
        if (string.IsNullOrEmpty(Text))
        {
            return;
        }

        var source = new CanvasCommandList(sender);
        var totalTime = TimeSpan.FromSeconds((double)Environment.TickCount / MorphSpeed % 15);

        double maxProgress = 0;
        using (var drawingSession = source.CreateDrawingSession())
        {
            foreach (var item in _morphItems)
            {
                var progress = item.Timeline.GetCurrentProgress(totalTime);
                maxProgress = Math.Max(maxProgress, progress);
                drawingSession.DrawText(item.Text, _centerPoint, new CanvasSolidColorBrush(sender, ColorBrush)
                {
                    Opacity = Convert.ToSingle(progress)
                }, _textFormat);
            }
        }

        _blurEffect.BlurAmount = Convert.ToSingle(20 * (1 - maxProgress));
        _blurEffect.Source = source;

        args.DrawingSession.DrawImage(_colorMatrixEffect);
        _Canvas.Invalidate();
    }
}
