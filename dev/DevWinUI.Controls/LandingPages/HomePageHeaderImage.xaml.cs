using CommunityToolkit.WinUI;
using CommunityToolkit.WinUI.Animations;

namespace DevWinUI;
// ATTRIBUTION: @RykenApps
public sealed partial class HomePageHeaderImage : UserControl
{
    public string HeaderImage
    {
        get => (string)GetValue(HeaderImageProperty);
        set => SetValue(HeaderImageProperty, value);
    }
    public string HeaderOverlayImage
    {
        get => (string)GetValue(HeaderOverlayImageProperty);
        set => SetValue(HeaderOverlayImageProperty, value);
    }
    public ImageSource PlaceholderSource
    {
        get => (ImageSource)GetValue(PlaceholderSourceProperty);
        set => SetValue(PlaceholderSourceProperty, value);
    }
    public bool IsCacheEnabled
    {
        get => (bool)GetValue(IsCacheEnabledProperty);
        set => SetValue(IsCacheEnabledProperty, value);
    }
    public bool EnableLazyLoading
    {
        get => (bool)GetValue(EnableLazyLoadingProperty);
        set => SetValue(EnableLazyLoadingProperty, value);
    }
    public double LazyLoadingThreshold
    {
        get => (double)GetValue(LazyLoadingThresholdProperty);
        set => SetValue(LazyLoadingThresholdProperty, value);
    }

    public double OverlayOpacity
    {
        get { return (double)GetValue(OverlayOpacityProperty); }
        set { SetValue(OverlayOpacityProperty, value); }
    }

    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }
    public string NormalizedCenterPoint
    {
        get { return (string)GetValue(NormalizedCenterPointProperty); }
        set { SetValue(NormalizedCenterPointProperty, value); }
    }
    public bool IsTileImage
    {
        get { return (bool)GetValue(IsTileImageProperty); }
        set { SetValue(IsTileImageProperty, value); }
    }

    public static readonly DependencyProperty IsTileImageProperty = DependencyProperty.Register(nameof(IsTileImage), typeof(bool), typeof(HomePageHeaderImage), new PropertyMetadata(false, OnIsTileImageChanged));
    public static readonly DependencyProperty NormalizedCenterPointProperty = DependencyProperty.Register(nameof(NormalizedCenterPoint), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata("0.5"));
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(HomePageHeaderImage), new PropertyMetadata(Stretch.UniformToFill));
    public static readonly DependencyProperty OverlayOpacityProperty = DependencyProperty.Register(nameof(OverlayOpacity), typeof(double), typeof(HomePageHeaderImage), new PropertyMetadata(0.5));
    public static readonly DependencyProperty HeaderImageProperty = DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata(default(string), OnHeaderImageChanged));
    public static readonly DependencyProperty HeaderOverlayImageProperty = DependencyProperty.Register(nameof(HeaderOverlayImage), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata(default(string), OnHeaderImageChanged));
    public static readonly DependencyProperty PlaceholderSourceProperty = DependencyProperty.Register(nameof(PlaceholderSource), typeof(ImageSource), typeof(HomePageHeaderImage), new PropertyMetadata(default(ImageSource)));
    public static readonly DependencyProperty IsCacheEnabledProperty = DependencyProperty.Register(nameof(IsCacheEnabled), typeof(bool), typeof(HomePageHeaderImage), new PropertyMetadata(true));
    public static readonly DependencyProperty EnableLazyLoadingProperty = DependencyProperty.Register(nameof(EnableLazyLoading), typeof(bool), typeof(HomePageHeaderImage), new PropertyMetadata(true));
    public static readonly DependencyProperty LazyLoadingThresholdProperty = DependencyProperty.Register(nameof(LazyLoadingThreshold), typeof(double), typeof(HomePageHeaderImage), new PropertyMetadata(300.0));

    private Compositor _compositor;
    private CompositionLinearGradientBrush _imageGridBottomGradientBrush;
    private CompositionEffectBrush _imageGridEffectBrush;
    private ExpressionAnimation _imageGridSizeAnimation;
    private ExpressionAnimation _bottomGradientStartPointAnimation;
    private SpriteVisual _imageGridSpriteVisual;
    private CompositionSurfaceBrush _imageGridSurfaceBrush;
    private Visual _imageGridVisual;
    private CompositionVisualSurface _imageGridVisualSurface;
    private const string GradientSizeKey = "GradientSize";
    public HomePageHeaderImage()
    {
        this.InitializeComponent();
    }
    private static void OnHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeaderImage)d;
        if (ctl != null)
        {
            ctl.ToggleTileOrImageEx();
        }
    }
    private static void OnIsTileImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeaderImage)d;
        if (ctl != null)
        {
            ctl.ToggleTileOrImageEx();
        }
    }
    private void ToggleTileOrImageEx()
    {
        if (IsTileImage)
        {
            HeroImage.Visibility = Visibility.Collapsed;
            HeroOverlayImage.Visibility = Visibility.Collapsed;
            HeroTile.Visibility = Visibility.Visible;
            HeroOverlayTile.Visibility = Visibility.Visible;
        }
        else
        {
            HeroImage.Visibility = Visibility.Visible;
            HeroOverlayImage.Visibility = Visibility.Visible;
            HeroTile.Visibility = Visibility.Collapsed;
            HeroOverlayTile.Visibility = Visibility.Collapsed;
        }
    }
    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(HeaderOverlayImage))
        {
            HeaderOverlayImage = HeaderImage;
        }
        _imageGridVisual = ElementCompositionPreview.GetElementVisual(ImageGrid);
        _compositor = _imageGridVisual.Compositor;

        _imageGridSizeAnimation = _compositor.CreateExpressionAnimation("Visual.Size");
        _imageGridSizeAnimation.SetReferenceParameter("Visual", _imageGridVisual);

        _imageGridVisualSurface = _compositor.CreateVisualSurface();
        _imageGridVisualSurface.SourceVisual = _imageGridVisual;
        _imageGridVisualSurface.StartAnimation(nameof(CompositionVisualSurface.SourceSize), _imageGridSizeAnimation);

        _imageGridSurfaceBrush = _compositor.CreateSurfaceBrush(_imageGridVisualSurface);
        _imageGridSurfaceBrush.Stretch = CompositionStretch.UniformToFill;

        _bottomGradientStartPointAnimation = CreateExpressionAnimation(nameof(CompositionLinearGradientBrush.StartPoint), $"Vector2(0.5, Visual.Size.Y - this.{GradientSizeKey})");
        SetBottomGradientStartPoint();

        _imageGridBottomGradientBrush = _compositor.CreateLinearGradientBrush();
        _imageGridBottomGradientBrush.MappingMode = CompositionMappingMode.Absolute;
        _imageGridBottomGradientBrush.StartAnimation(_bottomGradientStartPointAnimation);
        _imageGridBottomGradientBrush.StartAnimation(CreateExpressionAnimation(nameof(CompositionLinearGradientBrush.EndPoint), "Vector2(0.5, Visual.Size.Y)"));
        _imageGridBottomGradientBrush.CreateColorStopsWithEasingFunction(EasingType.Sine, EasingMode.EaseInOut, 0f, 1f);

        var alphaMask = new AlphaMaskEffect
        {
            Source = new CompositionEffectSourceParameter("ImageGrid"),
            AlphaMask = new CompositionEffectSourceParameter("Gradient")
        };

        var effectFactory = _compositor.CreateEffectFactory(alphaMask);
        _imageGridEffectBrush = effectFactory.CreateBrush();
        _imageGridEffectBrush.SetSourceParameter("ImageGrid", _imageGridSurfaceBrush);
        _imageGridEffectBrush.SetSourceParameter("Gradient", _imageGridBottomGradientBrush);

        _imageGridSpriteVisual = _compositor.CreateSpriteVisual();
        _imageGridSpriteVisual.RelativeSizeAdjustment = Vector2.One;
        _imageGridSpriteVisual.Brush = _imageGridEffectBrush;

        ElementCompositionPreview.GetElementVisual(ImageGridSurfaceRec).ParentForTransform = _imageGridVisual;

        ElementCompositionPreview.SetElementChildVisual(ImageGridSurfaceRec, _imageGridSpriteVisual);
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        ElementCompositionPreview.SetElementChildVisual(ImageGridSurfaceRec, null);
        _imageGridSpriteVisual?.Dispose();
        _imageGridEffectBrush?.Dispose();
        _imageGridSurfaceBrush?.Dispose();
        _imageGridVisualSurface?.Dispose();
        _imageGridSizeAnimation?.Dispose();
        _bottomGradientStartPointAnimation?.Dispose();
        _bottomGradientStartPointAnimation = null;
    }
    private void OnLoading(FrameworkElement sender, object args)
    {
        if (IsTileImage)
        {
            if (HeroTile.ImageSource == null)
            {
                HeroTile.GetVisual().Opacity = 0;
            }
            else
            {
                AnimateImage();
            }
        }
        else
        {
            if (HeroImage.Source == null)
            {
                HeroImage.GetVisual().Opacity = 0;
            }
            else
            {
                AnimateImage();
            }
        }
    }
    private void SetBottomGradientStartPoint()
    {
        _bottomGradientStartPointAnimation?.Properties.InsertScalar(GradientSizeKey, 180);
    }

    private void OnImageOpened(object sender, ImageExOpenedEventArgs e)
    {
        AnimateImage();
    }

    private void AnimateImage()
    {
        if (IsTileImage)
        {
            AnimationBuilder.Create()
                        .Opacity(1, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
                        .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
                        .Start(HeroTile);

            AnimationBuilder.Create()
            .Opacity(0.5, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
            .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
            .Start(HeroOverlayTile);
        }
        else
        {
            AnimationBuilder.Create()
            .Opacity(1, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
            .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
            .Start(HeroImage);

            AnimationBuilder.Create()
                .Opacity(0.5, 0, duration: TimeSpan.FromMilliseconds(300), easingMode: EasingMode.EaseOut)
                .Scale(1, 1.1f, duration: TimeSpan.FromMilliseconds(400), easingMode: EasingMode.EaseOut)
                .Start(HeroOverlayImage);
        }
    }

    private ExpressionAnimation CreateExpressionAnimation(string target, string expression)
    {
        var ani = _compositor.CreateExpressionAnimation(expression);
        ani.SetReferenceParameter("Visual", _imageGridVisual);
        ani.Target = target;
        return ani;
    }

    private void HeroTile_ImageLoaded(object sender, EventArgs e)
    {
        AnimateImage();
    }
}
