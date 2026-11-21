using Microsoft.UI.Xaml.Markup;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class ShimmerTextBlock : Control
{
    private Compositor? _compositor;
    private PointLight? _pointLight;
    private Visual? _textVisual;
    private ScalarKeyFrameAnimation? _animation;

    public ShimmerTextBlock()
    {
        this.DefaultStyleKey = typeof(ShimmerTextBlock);
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(TextBlock), typeof(ShimmerTextBlock),
            new PropertyMetadata(null, OnContentChanged));

    public TextBlock? Content
    {
        get => (TextBlock?)GetValue(ContentProperty);
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
        var content = Content;
        if (content == null) return;

        var textVisual = ElementCompositionPreview.GetElementVisual(content);
        var compositor = textVisual.Compositor;
        _textVisual = textVisual;
        _compositor = compositor;

        if (_pointLight == null)
        {
            _pointLight = compositor.CreatePointLight();
            _pointLight.Color = Colors.White;
            _pointLight.CoordinateSpace = textVisual;
            _pointLight.Targets.Add(textVisual);
        }

        content.SizeChanged -= OnContentSizeChanged;
        content.SizeChanged += OnContentSizeChanged;

        if (AutoStart)
        {
            StartAnimation();
        }
    }

    private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        StartAnimation();
    }

    public void StartAnimation()
    {
        var content = Content;
        var compositor = _compositor;
        var pointLight = _pointLight;
        if (pointLight == null || compositor == null || content == null) return;

        float width = (float)content.ActualWidth;
        float height = (float)content.ActualHeight;
        float fontSize = (float)content.FontSize;

        pointLight.Offset = new Vector3(-width, height / 2, fontSize);

        var animation = compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(1.0f, 2 * width);
        animation.Duration = Duration;
        animation.IterationBehavior = IterationBehavior;
        _animation = animation;

        pointLight.StartAnimation("Offset.X", animation);
    }
    public void StopAnimation()
    {
        if (_pointLight == null) return;

        _pointLight.StopAnimation("Offset.X");
    }
}
