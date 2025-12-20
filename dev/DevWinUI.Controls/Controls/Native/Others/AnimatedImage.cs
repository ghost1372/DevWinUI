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
        DependencyProperty.Register(nameof(ImageUrl), typeof(Uri), typeof(AnimatedImage), new PropertyMetadata(null, OnImageSourceChanged));

    public BitmapImage ImageSource
    {
        get { return (BitmapImage)GetValue(ImageSourceProperty); }
        set { SetValue(ImageSourceProperty, value); }
    }

    public static readonly DependencyProperty ImageSourceProperty =
        DependencyProperty.Register(nameof(ImageSource), typeof(BitmapImage), typeof(AnimatedImage), new PropertyMetadata(null, OnImageSourceChanged));

    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(AnimatedImage), new PropertyMetadata(Stretch.UniformToFill));

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

        OnIsImageChanged();
    }
    private static void OnImageSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        if (_bottomImage == null || _topImage == null)
            return;

        BitmapImage bitmapImage = null;

        if (ImageSource != null)
        {
            bitmapImage = ImageSource;
        }
        else if (ImageUrl != null)
        {
            bitmapImage = new BitmapImage(ImageUrl);
        }
        else
        {
            return;
        }

        _bottomImage.Source = bitmapImage;

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
                    _topImage.Source = bitmapImage;
                    topVisual.Opacity = 1.0f;
                }
                catch { }
            });
        });
    }
}
