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
    private ScalarKeyFrameAnimation? _opacityAnimation;

    public Uri ImageUrl
    {
        get => (Uri)GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public static readonly DependencyProperty ImageUrlProperty =
        DependencyProperty.Register(nameof(ImageUrl), typeof(Uri), typeof(AnimatedImage), new PropertyMetadata(null, OnImageUrlChanged));

    public AnimatedImage()
    {
        this.DefaultStyleKey = typeof(AnimatedImage);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        _bottomImage = GetTemplateChild(PART_BottomImage) as Image;
        _topImage = GetTemplateChild(PART_TopImage) as Image;

        InitAnimations();
    }
    private static void OnImageUrlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is AnimatedImage control)
        {
            control.OnIsImageChanged();
        }
    }

    private void InitAnimations()
    {
        _opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _opacityAnimation.InsertKeyFrame(0.0f, 1.0f);
        _opacityAnimation.InsertKeyFrame(1.0f, 0.0f);
        _opacityAnimation.Duration = TimeSpan.FromMilliseconds(800);
    }

    private void OnIsImageChanged()
    {
        if (ImageUrl == null || _bottomImage == null || _topImage == null)
            return;

        _bottomImage.Source = new BitmapImage(ImageUrl);
        _bottomImage.Opacity = 1;

        var topVisual = ElementCompositionPreview.GetElementVisual(_topImage);
        topVisual.Opacity = 1.0f;

        topVisual.StartAnimation("Opacity", _opacityAnimation);

        // Set new image after fade-out
        _ = Task.Delay(800).ContinueWith(_ =>
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    _topImage.Source = new BitmapImage(ImageUrl);
                    topVisual.Opacity = 1.0f;
                }
                catch { }
            });
        });
    }
}
