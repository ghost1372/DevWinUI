namespace DevWinUI;
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
    public static readonly DependencyProperty NormalizedCenterPointProperty = DependencyProperty.Register(nameof(NormalizedCenterPoint), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata("0.5", OnNormalizedCenterPointChanged));
    public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(HomePageHeaderImage), new PropertyMetadata(Stretch.UniformToFill));
    public static readonly DependencyProperty HeaderImageProperty = DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata(default(string), OnHeaderImageChanged));
    public static readonly DependencyProperty HeaderOverlayImageProperty = DependencyProperty.Register(nameof(HeaderOverlayImage), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata(default(string), OnHeaderImageChanged));

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
    private static void OnNormalizedCenterPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeaderImage)d;
        if (ctl != null)
        {
            ctl.UpdateNormalizedCenterPoint();
        }
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
        _imageGridBottomGradientBrush.StartAnimation(nameof(CompositionLinearGradientBrush.StartPoint), _bottomGradientStartPointAnimation);
        _imageGridBottomGradientBrush.StartAnimation(nameof(CompositionLinearGradientBrush.EndPoint), CreateExpressionAnimation(nameof(CompositionLinearGradientBrush.EndPoint), "Vector2(0.5, Visual.Size.Y)"));
        _imageGridBottomGradientBrush.CreateColorStopsWithEasingFunction(EasingMode.EaseInOut, 0f, 1f);

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
                GetVisual(HeroTile).Opacity = 0;
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
                GetVisual(HeroImage).Opacity = 0;
            }
            else
            {
                AnimateImage();
            }
        }
    }
    private Visual GetVisual(UIElement element)
    {
        return ElementCompositionPreview.GetElementVisual(element);
    }
    private void SetBottomGradientStartPoint()
    {
        _bottomGradientStartPointAnimation?.Properties.InsertScalar(GradientSizeKey, 180);
    }

    private void AnimateImage()
    {
        if (IsTileImage)
        {
            StartAnimation(HeroTile, 0, 1);
            StartAnimation(HeroOverlayTile, 0, 0.5f);
        }
        else
        {
            StartAnimation(HeroImage, 0, 1);
            StartAnimation(HeroOverlayImage, 0, 0.5f);
        }
    }

    private void StartAnimation(UIElement element, float startOpacity = 0f, float endOpacity = 1f, float startScale = 1.1f, float endScale = 1f)
    {
        if (element == null) return; // Safety check

        // Set initial values to avoid shrinkage
        var visual = ElementCompositionPreview.GetElementVisual(element);

        // Set the initial scale and opacity
        visual.Scale = new Vector3(startScale, startScale, 1f); // Start scaling from startScale
        visual.Opacity = startOpacity; // Set initial opacity

        // Fix shifting issue by setting the CenterPoint to the right edge
        visual.CenterPoint = new Vector3((float)element.RenderSize.Width / 2,
                                          (float)(element.RenderSize.Height / 2), 0);

        // Create easing functions
        var cubicBezierEasing = _compositor.CreateCubicBezierEasingFunction(
            new System.Numerics.Vector2(0.1f, 0.9f), // Control points for easing
            new System.Numerics.Vector2(0.2f, 1.0f)
        );

        // Opacity Animation with easing applied to keyframe
        var opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
        opacityAnimation.InsertKeyFrame(0f, startOpacity, cubicBezierEasing); // Apply easing to start opacity
        opacityAnimation.InsertKeyFrame(1f, endOpacity, cubicBezierEasing); // Apply easing to end opacity
        opacityAnimation.Duration = TimeSpan.FromMilliseconds(300);
        opacityAnimation.Target = "Opacity";

        // Scale Animation with easing applied to keyframe
        var scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
        scaleAnimation.InsertKeyFrame(0f, new Vector3(startScale, startScale, 1f), cubicBezierEasing); // Apply easing to start scale
        scaleAnimation.InsertKeyFrame(1f, new Vector3(endScale, endScale, 1f), cubicBezierEasing); // Apply easing to end scale
        scaleAnimation.Duration = TimeSpan.FromMilliseconds(400);
        scaleAnimation.Target = "Scale";

        // Start Animations
        visual.StartAnimation("Opacity", opacityAnimation);
        visual.StartAnimation("Scale", scaleAnimation);
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
    private void OnImageOpened(object sender, ImageExOpenedEventArgs e)
    {
        AnimateImage();
    }

    private void UpdateNormalizedCenterPoint()
    {
        if (IsTileImage)
        {
            UpdateNormalizedCenterPoint(HeroTile);
            UpdateNormalizedCenterPoint(HeroOverlayTile);
        }
        else
        {
            UpdateNormalizedCenterPoint(HeroImage);
            UpdateNormalizedCenterPoint(HeroOverlayImage);
        }
    }

    private void UpdateNormalizedCenterPoint(FrameworkElement element)
    {
        Vector2 center = NormalizedCenterPoint.ToVector2();
        Visual visual = GetVisual(element);
        const string expression = "Vector2(this.Target.Size.X * X, this.Target.Size.Y * Y)";
        ExpressionAnimation animation = visual.Compositor.CreateExpressionAnimation(expression);

        animation.SetScalarParameter("X", center.X);
        animation.SetScalarParameter("Y", center.Y);

        visual.StopAnimation("CenterPoint.XY");
        visual.StartAnimation("CenterPoint.XY", animation);
    }
}
