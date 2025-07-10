using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class ShimmerTextBlock : Control
{
    private Compositor _compositor;
    private PointLight _pointLight;
    private Visual _textVisual;
    private ScalarKeyFrameAnimation _animation;

    public ShimmerTextBlock()
    {
        this.DefaultStyleKey = typeof(ShimmerTextBlock);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(TextBlock), typeof(ShimmerTextBlock),
            new PropertyMetadata(null, OnContentChanged));

    public TextBlock Content
    {
        get => (TextBlock)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(TimeSpan), typeof(ShimmerTextBlock),
            new PropertyMetadata(TimeSpan.FromSeconds(3.3), OnAnimationPropertyChanged));

    public TimeSpan Duration
    {
        get => (TimeSpan)GetValue(DurationProperty);
        set => SetValue(DurationProperty, value);
    }

    public static readonly DependencyProperty IterationBehaviorProperty =
        DependencyProperty.Register(nameof(IterationBehavior), typeof(AnimationIterationBehavior), typeof(ShimmerTextBlock),
            new PropertyMetadata(AnimationIterationBehavior.Forever, OnAnimationPropertyChanged));

    public AnimationIterationBehavior IterationBehavior
    {
        get => (AnimationIterationBehavior)GetValue(IterationBehaviorProperty);
        set => SetValue(IterationBehaviorProperty, value);
    }

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(ShimmerTextBlock),
            new PropertyMetadata(true));

    public bool AutoStart
    {
        get => (bool)GetValue(AutoStartProperty);
        set => SetValue(AutoStartProperty, value);
    }

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShimmerTextBlock control)
        {
            control.ApplyTemplate();
            control.Setup();
        }
    }

    private static void OnAnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ShimmerTextBlock control && control._pointLight != null)
        {
            control.StartAnimation();
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        Setup();
    }

    public void Setup()
    {
        if (Content == null) return;

        _textVisual = ElementCompositionPreview.GetElementVisual(Content);
        _compositor = _textVisual.Compositor;

        if (_pointLight == null)
        {
            _pointLight = _compositor.CreatePointLight();
            _pointLight.Color = Colors.White;
            _pointLight.CoordinateSpace = _textVisual;
            _pointLight.Targets.Add(_textVisual);
        }

        Content.SizeChanged -= (s, e) => StartAnimation();
        Content.SizeChanged += (s, e) => StartAnimation();

        if (AutoStart)
        {
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        if (_pointLight == null || Content == null) return;

        float width = (float)Content.ActualWidth;
        float height = (float)Content.ActualHeight;
        float fontSize = (float)Content.FontSize;

        _pointLight.Offset = new Vector3(-width, height / 2, fontSize);

        _animation = _compositor.CreateScalarKeyFrameAnimation();
        _animation.InsertKeyFrame(1.0f, 2 * width);
        _animation.Duration = Duration;
        _animation.IterationBehavior = IterationBehavior;

        _pointLight.StartAnimation("Offset.X", _animation);
    }
    public void StopAnimation()
    {
        if (_pointLight == null) return;

        _pointLight.StopAnimation("Offset.X");
    }
}
