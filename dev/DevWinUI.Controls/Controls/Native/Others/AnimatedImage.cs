namespace DevWinUI;

[TemplatePart(Name = nameof(PART_BottomImage), Type = typeof(Image))]
[TemplatePart(Name = nameof(PART_TopImage), Type = typeof(Image))]
public partial class AnimatedImage : Control
{
    private const string PART_BottomImage = "PART_BottomImage";
    private const string PART_TopImage = "PART_TopImage";

    private Image _bottomImage;
    private Image _topImage;
    private Compositor _compositor;
    private ScalarKeyFrameAnimation _opacityAnimation;
    private CompositionScopedBatch _currentBatch;

    public Uri ImageUrl
    {
        get => (Uri)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public static readonly DependencyProperty ImageUrlProperty =
        DependencyProperty.Register(nameof(ImageUrl), typeof(Uri), typeof(AnimatedImage), new PropertyMetadata(null, OnImageChanged));

    public BitmapImage ImageSource
    {
        get => (BitmapImage)GetValue(ImageSourceProperty);
        set => SetValue(ImageSourceProperty, value);
    }

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(BitmapImage), typeof(AnimatedImage), new PropertyMetadata(null, OnImageChanged));

    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(AnimatedImage), new PropertyMetadata(Stretch.UniformToFill));

    public TimeSpan OpacityAnimationDuration
    {
        get { return (TimeSpan)GetValue(OpacityAnimationDurationProperty); }
        set { SetValue(OpacityAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty OpacityAnimationDurationProperty =
        DependencyProperty.Register(nameof(OpacityAnimationDuration), typeof(TimeSpan), typeof(AnimatedImage), new PropertyMetadata(TimeSpan.FromMilliseconds(600)));

    public AnimatedImage()
    {
        DefaultStyleKey = typeof(AnimatedImage);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _bottomImage = GetTemplateChild(PART_BottomImage) as Image;
        _topImage = GetTemplateChild(PART_TopImage) as Image;

        if (_bottomImage == null || _topImage == null)
            return;

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        InitAnimation();
        UpdateImage();
    }

    private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((AnimatedImage)d).UpdateImage();
    }

    private void InitAnimation()
    {
        _opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _opacityAnimation.InsertKeyFrame(0f, 1f);
        _opacityAnimation.InsertKeyFrame(1f, 0f);
        _opacityAnimation.Duration = OpacityAnimationDuration;
    }

    private void UpdateImage()
    {
        if (_bottomImage == null || _topImage == null)
            return;

        ImageSource newSource = ImageSource;

        if (newSource == null && ImageUrl != null)
            newSource = new BitmapImage(ImageUrl);

        if (newSource == null)
            return;

        if (_bottomImage.Source == newSource)
            return;

        _bottomImage.Source = newSource;

        var topVisual = ElementCompositionPreview.GetElementVisual(_topImage);

        _currentBatch?.Dispose();

        topVisual.Opacity = 1f;

        _currentBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        topVisual.StartAnimation("Opacity", _opacityAnimation);

        _currentBatch.Completed += (s, e) =>
        {
            _topImage.Source = newSource;
            topVisual.Opacity = 1f;
        };

        _currentBatch.End();
    }
}
