using Microsoft.UI.Dispatching;
using Microsoft.UI.Text;
using Windows.UI.Text;

namespace DevWinUI;

[TemplatePart(Name = "ContentBorder", Type = typeof(Border))]
[TemplatePart(Name = "AnimatedCanvas", Type = typeof(CanvasAnimatedControl))]
public sealed partial class AnimatedTextBlock : Control
{
    private CanvasAnimatedControl _animatedCanvas = null;

    private string _oldText = string.Empty;
    private string _newText = string.Empty;

    private AnimatedTextBlockRedrawState _currentState = AnimatedTextBlockRedrawState.Idle;
    private TimeSpan _animationBeginTime;

    private List<TextDiffResult> _diffResults = null;

    private CanvasTextFormat _textFormat = new CanvasTextFormat();
    private CanvasLinearGradientBrush _textBrush;
    private Color _textColor = Colors.Black;

    private CanvasTextLayout _oldTextLayout;
    private CanvasTextLayout _newTextLayout;

    private ITextEffect _textEffect;

    private float _fontSize = 14;
    private string _fontFamily = FontFamily.XamlAutoFontFamily.Source;
    private FontStretch _fontStretch = FontStretch.Normal;
    private FontStyle _fontStyle = FontStyle.Normal;
    private FontWeight _fontWeight = FontWeights.Normal;

    private TextAlignment _textAlignment = TextAlignment.Left;
    private AnimatedTextBlockTextDirection _textDirection = AnimatedTextBlockTextDirection.LeftToRightThenTopToBottom;
    private TextTrimming _textTrimming = TextTrimming.None;
    private TextWrapping _textWrapping = TextWrapping.NoWrap;

    #region Properties

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(AnimatedTextBlock), new PropertyMetadata(default(string)));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set
        {
            _oldText = _newText ?? string.Empty;
            _newText = value ?? string.Empty;

            SetRedrawState(AnimatedTextBlockRedrawState.TextChanged, false);

            SetValue(TextProperty, value);
        }
    }

    public static readonly DependencyProperty TextEffectProperty = DependencyProperty.Register(
        nameof(TextEffect), typeof(ITextEffect), typeof(AnimatedTextBlock), new PropertyMetadata(default(ITextEffect)));

    public ITextEffect TextEffect
    {
        get { return (ITextEffect)GetValue(TextEffectProperty); }
        set
        {
            _textEffect = value;
            SetValue(TextEffectProperty, value);
        }
    }

    public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register(
        nameof(TextAlignment), typeof(TextAlignment), typeof(AnimatedTextBlock), new PropertyMetadata(default(TextAlignment)));

    public TextAlignment TextAlignment
    {
        get { return (TextAlignment)GetValue(TextAlignmentProperty); }
        set
        {
            _textAlignment = value;
            SetValue(TextAlignmentProperty, value);
        }
    }

    public static readonly DependencyProperty TextDirectionProperty = DependencyProperty.Register(
        nameof(TextDirection), typeof(AnimatedTextBlockTextDirection), typeof(AnimatedTextBlock), new PropertyMetadata(default(AnimatedTextBlockTextDirection)));

    public AnimatedTextBlockTextDirection TextDirection
    {
        get { return (AnimatedTextBlockTextDirection)GetValue(TextDirectionProperty); }
        set
        {
            _textDirection = value;
            SetValue(TextDirectionProperty, value);
        }
    }

    public static readonly DependencyProperty TextTrimmingProperty = DependencyProperty.Register(
        nameof(TextTrimming), typeof(TextTrimming), typeof(AnimatedTextBlock), new PropertyMetadata(default(TextTrimming)));

    public TextTrimming TextTrimming
    {
        get { return (TextTrimming)GetValue(TextTrimmingProperty); }
        set
        {
            _textTrimming = value;
            SetValue(TextTrimmingProperty, value);
        }
    }

    public static readonly DependencyProperty TextWrappingProperty = DependencyProperty.Register(
        nameof(TextWrapping), typeof(TextWrapping), typeof(AnimatedTextBlock), new PropertyMetadata(default(TextWrapping)));

    public TextWrapping TextWrapping
    {
        get { return (TextWrapping)GetValue(TextWrappingProperty); }
        set
        {
            _textWrapping = value;
            SetValue(TextWrappingProperty, value);
        }
    }

    public bool IsAnimating => _currentState != AnimatedTextBlockRedrawState.Idle;

    #endregion

    public event EventHandler<AnimatedTextBlockRedrawState> RedrawStateChanged;

    public AnimatedTextBlock()
    {
        this.DefaultStyleKey = typeof(AnimatedTextBlock);

        this.Loaded += OnLoaded;
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.ForegroundProperty, ForegroundChangedCallback);
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.FontFamilyProperty, FontFamilyChangedCallback);
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.FontSizeProperty, FontSizeChangedCallback);
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.FontStretchProperty, FontStretchChangedCallback);
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.FontStyleProperty, FontStyleChangedCallback);
        this.RegisterPropertyChangedCallback(AnimatedTextBlock.FontWeightProperty, FontWeightChangedCallback);

        _textFormat.TrimmingSign = CanvasTrimmingSign.Ellipsis;
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _animatedCanvas = GetTemplateChild("AnimatedCanvas") as CanvasAnimatedControl;
        this.SizeChanged += OnSizeChanged;

        ApplyTextFormat();
        ApplyTextForeground();

        if (_animatedCanvas != null)
        {
            _animatedCanvas.CreateResources += AnimatedCanvas_CreateResources;
            _animatedCanvas.Update += AnimatedCanvas_Update;
            _animatedCanvas.Draw += AnimatedCanvas_Draw;
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        _newText = Text ?? string.Empty;

        SetRedrawState(AnimatedTextBlockRedrawState.TextChanged, false);
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        SetRedrawState(AnimatedTextBlockRedrawState.LayoutChanged);
    }

    #region Property Changed Callbacks

    private void ForegroundChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        ApplyTextForeground();
    }
    private void FontFamilyChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        _fontFamily = FontFamily.Source;
    }
    private void FontSizeChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        _fontSize = (float)FontSize;
    }
    private void FontStretchChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        _fontStretch = FontStretch;
    }

    private void FontStyleChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        _fontStyle = FontStyle;
    }
    private void FontWeightChangedCallback(DependencyObject sender, DependencyProperty dp)
    {
        _fontWeight = FontWeight;
    }
    #endregion

    #region Canvas Events

    private void AnimatedCanvas_CreateResources(CanvasAnimatedControl sender,
        CanvasCreateResourcesEventArgs args)
    {
        if (Foreground is LinearGradientBrush linearGradientBrush)
        {
            var stops = new CanvasGradientStop[linearGradientBrush.GradientStops.Count];

            for (int i = 0; i < linearGradientBrush.GradientStops.Count; i++)
            {
                var gradientStop = linearGradientBrush.GradientStops[i];
                stops[i].Color = gradientStop.Color;
                stops[i].Position = (float)gradientStop.Offset;
            }

            _textBrush = new CanvasLinearGradientBrush(_animatedCanvas, stops);
        }
    }

    private void AnimatedCanvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        if (_textEffect == null)
        {
            SetRedrawState(AnimatedTextBlockRedrawState.Idle);
            return;
        }

        if (_currentState == AnimatedTextBlockRedrawState.LayoutChanged)
        {
            ApplyTextFormat();

            if (_newTextLayout == null)
            {
                SetRedrawState(AnimatedTextBlockRedrawState.Idle);
            }
            else
            {
                _oldText = _newText;
                _oldTextLayout = _newTextLayout;

                GenerateNewTextLayout(sender);

                GenerateDiffResults();

                _animationBeginTime = args.Timing.TotalTime;

                SetRedrawState(AnimatedTextBlockRedrawState.Animating);
            }
        }

        if (_currentState == AnimatedTextBlockRedrawState.TextChanged)
        {
            ApplyTextFormat();

            GenerateOldTextLayout(sender);

            GenerateNewTextLayout(sender);

            GenerateDiffResults();

            _animationBeginTime = args.Timing.TotalTime;

            SetRedrawState(AnimatedTextBlockRedrawState.Animating);
        }

        if (_currentState == AnimatedTextBlockRedrawState.Animating)
        {
            UpdateAllClusterProgress(args.Timing);
        }

        _textEffect.Update(_oldText,
            _newText,
            _diffResults,
            _oldTextLayout,
            _newTextLayout,
            _currentState,
            sender,
            args);
    }

    private void AnimatedCanvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        args.DrawingSession.Clear(Colors.Transparent);

        if (_textEffect == null)
        {
            CanvasTextLayout ctl = new CanvasTextLayout(sender,
                _newText,
                _textFormat,
                (float)sender.Size.Width,
                (float)sender.Size.Height);
            ctl.Options = CanvasDrawTextOptions.EnableColorFont;

            args.DrawingSession.DrawTextLayout(ctl, 0, 0, _textColor);
        }
        else
        {
            _textEffect?.DrawText(_oldText,
                _newText,
                _diffResults,
                _oldTextLayout,
                _newTextLayout,
                _textFormat, _textColor,
                _textBrush,
                _currentState,
                args.DrawingSession,
                args);
        }
    }

    #endregion

    private void ApplyTextFormat()
    {
        _textFormat.FontSize = _fontSize;
        _textFormat.FontFamily = _fontFamily;
        _textFormat.FontStretch = _fontStretch;
        _textFormat.FontStyle = _fontStyle;
        _textFormat.FontWeight = _fontWeight;
        _textFormat.Options = CanvasDrawTextOptions.EnableColorFont | CanvasDrawTextOptions.NoPixelSnap;
        _textFormat.HorizontalAlignment = Win2dHelpers.MapCanvasHorizontalAlignment(_textAlignment);
        _textFormat.VerticalAlignment = CanvasVerticalAlignment.Center;
        _textFormat.Direction = Win2dHelpers.MapTextDirection(_textDirection);
        _textFormat.TrimmingGranularity = Win2dHelpers.MapTrimmingGranularity(_textTrimming);
        _textFormat.WordWrapping = Win2dHelpers.MapWordWrapping(_textWrapping);
    }

    private void ApplyTextForeground()
    {
        if (Foreground is SolidColorBrush colorBrush)
        {
            _textColor = colorBrush.Color;
            _textBrush = null;
        }
        else if (Foreground is LinearGradientBrush linearGradientBrush)
        {
            if (_animatedCanvas != null)
            {
                var stops = new CanvasGradientStop[linearGradientBrush.GradientStops.Count];

                foreach (var gradientStop in linearGradientBrush.GradientStops)
                {
                    var stop = new CanvasGradientStop()
                    {
                        Color = gradientStop.Color,
                        Position = (float)gradientStop.Offset
                    };
                }

                _textBrush = new CanvasLinearGradientBrush(_animatedCanvas, stops);
            }
        }
        else
        {
            if (Application.Current.Resources["TextFillColorPrimaryBrush"] is SolidColorBrush defaultForegroundBrush)
            {
                _textColor = defaultForegroundBrush.Color;
                _textBrush = null;
            }
        }
    }

    private void GenerateOldTextLayout(ICanvasAnimatedControl resourceCreator)
    {
        _oldTextLayout = new CanvasTextLayout(resourceCreator, _oldText, _textFormat,
            (float)(resourceCreator.Size.Width),
            (float)(resourceCreator.Size.Height));
        _oldTextLayout.Options = CanvasDrawTextOptions.EnableColorFont | CanvasDrawTextOptions.NoPixelSnap;
        _oldTextLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
    }

    private void GenerateNewTextLayout(ICanvasAnimatedControl resourceCreator)
    {
        _newTextLayout = new CanvasTextLayout(resourceCreator, _newText, _textFormat,
            (float)(resourceCreator.Size.Width),
            (float)(resourceCreator.Size.Height));
        _newTextLayout.Options = CanvasDrawTextOptions.EnableColorFont | CanvasDrawTextOptions.NoPixelSnap;
        _newTextLayout.VerticalAlignment = CanvasVerticalAlignment.Center;
    }

    private void GenerateDiffResults()
    {
        var oldGraphemeClusters = TextRenderingHelper.GenerateGraphemeClusters(_oldText, _oldTextLayout);
        var newGraphemeClusters = TextRenderingHelper.GenerateGraphemeClusters(_newText, _newTextLayout);

        _diffResults = GraphemeClusterDiff.Diff(oldGraphemeClusters, newGraphemeClusters);
    }

    private void UpdateAllClusterProgress(CanvasTimingInformation timing)
    {
        var animationDuration = _textEffect?.AnimationDuration ?? TimeSpan.FromMilliseconds(600);
        var delayPerCluster = _textEffect?.DelayPerCluster ?? TimeSpan.FromMilliseconds(0);

        float step = (float)(1 / (animationDuration.TotalMilliseconds / timing.ElapsedTime.TotalMilliseconds));

        var delay = delayPerCluster <= animationDuration ? delayPerCluster : animationDuration;

        int insertDelayOffset = 0;
        int moveDelayOffset = 0;
        int removeDelayOffset = 0;
        int updateDelayOffset = 0;

        int ongoingAnimations = 0;

        for (int i = 0; i < _diffResults.Count; i++)
        {
            var diffResult = _diffResults[i];
            var oldCluster = diffResult.OldGlyphCluster;
            var newCluster = diffResult.NewGlyphCluster;

            int delayOffset = 0;

            switch (diffResult.Type)
            {
                default:
                case AnimatedTextBlockDiffOperationType.Move:
                    delayOffset = removeDelayOffset;
                    removeDelayOffset += 1;
                    break;
                case AnimatedTextBlockDiffOperationType.Insert:
                    delayOffset = insertDelayOffset;
                    insertDelayOffset += 1;
                    break;
                case AnimatedTextBlockDiffOperationType.Remove:
                    delayOffset = moveDelayOffset;
                    moveDelayOffset += 1;
                    break;
                case AnimatedTextBlockDiffOperationType.Update:
                    delayOffset = updateDelayOffset;
                    updateDelayOffset += 1;
                    break;
            }

            if (!UpdateClusterProgress(oldCluster, delayOffset, step, delay, timing))
            {
                ongoingAnimations += 1;
            }

            if (!UpdateClusterProgress(newCluster, delayOffset, step, delay, timing))
            {
                ongoingAnimations += 1;
            }
        }

        if (ongoingAnimations < 1)
        {
            SetRedrawState(AnimatedTextBlockRedrawState.Idle);
        }
    }

    /// <summary>
    /// Update progress of every cluster.
    /// </summary>
    /// <param name="cluster">Target cluster.</param>
    /// <param name="offset">Index of target cluster</param>
    /// <param name="step">Incremental step of the progress</param>
    /// <param name="delay">Duration of delay</param>
    /// <param name="timing">Timing info</param>
    /// <returns>If the animation of the cluster is finished</returns>
    private bool UpdateClusterProgress(GraphemeCluster cluster,
        int offset,
        float step,
        TimeSpan delay,
        CanvasTimingInformation timing)
    {
        if (cluster == null)
            return true;

        var duration = _textEffect?.AnimationDuration ?? TimeSpan.FromMilliseconds(0);

        bool isFinished = timing.TotalTime.TotalMilliseconds >=
                          (_animationBeginTime.TotalMilliseconds +
                           delay.TotalMilliseconds * offset +
                           duration.TotalMilliseconds);

        if (isFinished)
        {
            cluster.Progress = 1.0f;
            cluster.IsAnimationFinished = true;
            return true;
        }

        float progress = cluster.Progress + step;

        if ((timing.TotalTime.TotalMilliseconds - _animationBeginTime.TotalMilliseconds <
             delay.TotalMilliseconds * offset))
        {
            progress = 0;
        }

        progress = Math.Clamp(progress, 0, 1.0f);

        cluster.Progress = progress;

        return false;
    }

    private void ResetAllClusterProgress()
    {
        foreach (var diffResult in _diffResults)
        {
            var oldCluster = diffResult.OldGlyphCluster;
            var newCluster = diffResult.NewGlyphCluster;

            oldCluster.Progress = 0;
            newCluster.Progress = 0;
        }
    }

    private void SetRedrawState(AnimatedTextBlockRedrawState state, bool fireEvent = true)
    {
        _currentState = state;

        if (fireEvent)
        {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => RedrawStateChanged?.Invoke(this, _currentState));
        }
    }
}
