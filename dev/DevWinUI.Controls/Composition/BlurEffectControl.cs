namespace DevWinUI;
public partial class BlurEffectControl : Control
{
    private readonly BlurEffectManager _blurManager;

    public BlurEffectControl()
    {
        this.DefaultStyleKey = typeof(BlurEffectControl);

        _blurManager = new BlurEffectManager(this);

        UpdateManagerProperties();
    }

    #region Dependency Properties
    public bool IsBlurEnabled
    {
        get { return (bool)GetValue(IsBlurEnabledProperty); }
        set { SetValue(IsBlurEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsBlurEnabledProperty =
        DependencyProperty.Register(nameof(IsBlurEnabled), typeof(bool), typeof(BlurEffectControl), new PropertyMetadata(true, OnBlurPropertyChanged));

    public bool IsTintEnabled
    {
        get { return (bool)GetValue(IsTintEnabledProperty); }
        set { SetValue(IsTintEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsTintEnabledProperty =
        DependencyProperty.Register(nameof(IsTintEnabled), typeof(bool), typeof(BlurEffectControl), new PropertyMetadata(false, OnBlurPropertyChanged));

    public BlurTintTarget TintTargetMode
    {
        get { return (BlurTintTarget)GetValue(TintTargetModeProperty); }
        set { SetValue(TintTargetModeProperty, value); }
    }

    public static readonly DependencyProperty TintTargetModeProperty =
        DependencyProperty.Register(nameof(TintTargetMode), typeof(BlurTintTarget), typeof(BlurEffectControl), new PropertyMetadata(BlurTintTarget.Foreground, OnBlurPropertyChanged));

    public ICompositionSurface SurfaceSource
    {
        get { return (ICompositionSurface)GetValue(SurfaceSourceProperty); }
        set { SetValue(SurfaceSourceProperty, value); }
    }

    public static readonly DependencyProperty SurfaceSourceProperty =
        DependencyProperty.Register(nameof(SurfaceSource), typeof(ICompositionSurface), typeof(BlurEffectControl), new PropertyMetadata(null, OnBlurPropertyChanged));

    public CompositionSurfaceBrush SurfaceBrushSource
    {
        get { return (CompositionSurfaceBrush)GetValue(SurfaceBrushSourceProperty); }
        set { SetValue(SurfaceBrushSourceProperty, value); }
    }

    public static readonly DependencyProperty SurfaceBrushSourceProperty =
        DependencyProperty.Register(nameof(SurfaceBrushSource), typeof(CompositionSurfaceBrush), typeof(BlurEffectControl), new PropertyMetadata(null, OnBlurPropertyChanged));

    public Visual VisualSource
    {
        get { return (Visual)GetValue(VisualSourceProperty); }
        set { SetValue(VisualSourceProperty, value); }
    }

    public static readonly DependencyProperty VisualSourceProperty =
        DependencyProperty.Register(nameof(VisualSource), typeof(Visual), typeof(BlurEffectControl), new PropertyMetadata(null, OnBlurPropertyChanged));

    public CompositionBrush CustomSourceBrush
    {
        get { return (CompositionBrush)GetValue(CustomSourceBrushProperty); }
        set { SetValue(CustomSourceBrushProperty, value); }
    }

    public static readonly DependencyProperty CustomSourceBrushProperty =
        DependencyProperty.Register(nameof(CustomSourceBrush), typeof(CompositionBrush), typeof(BlurEffectControl), new PropertyMetadata(null, OnBlurPropertyChanged));

    public double BlurAmount
    {
        get => (double)GetValue(BlurAmountProperty);
        set => SetValue(BlurAmountProperty, value);
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(BlurEffectControl), new PropertyMetadata(10.0, OnBlurPropertyChanged));

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    public static readonly DependencyProperty TintColorProperty =
        DependencyProperty.Register(nameof(TintColor), typeof(Color), typeof(BlurEffectControl), new PropertyMetadata(Colors.Transparent, OnBlurPropertyChanged));

    public bool UseNoise
    {
        get => (bool)GetValue(UseNoiseProperty);
        set => SetValue(UseNoiseProperty, value);
    }

    public static readonly DependencyProperty UseNoiseProperty =
        DependencyProperty.Register(nameof(UseNoise), typeof(bool), typeof(BlurEffectControl), new PropertyMetadata(false, OnBlurPropertyChanged));

    public string NoiseUri
    {
        get => (string)GetValue(NoiseUriProperty);
        set => SetValue(NoiseUriProperty, value);
    }

    public static readonly DependencyProperty NoiseUriProperty =
        DependencyProperty.Register(nameof(NoiseUri), typeof(string), typeof(BlurEffectControl), new PropertyMetadata("ms-appx:///Assets/Other/Noise.jpg", OnBlurPropertyChanged));

    public EffectBorderMode BorderMode
    {
        get => (EffectBorderMode)GetValue(BorderModeProperty);
        set => SetValue(BorderModeProperty, value);
    }

    public static readonly DependencyProperty BorderModeProperty =
        DependencyProperty.Register(nameof(BorderMode), typeof(EffectBorderMode), typeof(BlurEffectControl), new PropertyMetadata(EffectBorderMode.Hard, OnBlurPropertyChanged));

    public EffectOptimization Optimization
    {
        get => (EffectOptimization)GetValue(OptimizationProperty);
        set => SetValue(OptimizationProperty, value);
    }

    public static readonly DependencyProperty OptimizationProperty =
        DependencyProperty.Register(nameof(Optimization), typeof(EffectOptimization), typeof(BlurEffectControl), new PropertyMetadata(EffectOptimization.Balanced, OnBlurPropertyChanged));

    public BlendEffectMode BlendMode
    {
        get => (BlendEffectMode)GetValue(BlendModeProperty);
        set => SetValue(BlendModeProperty, value);
    }

    public static readonly DependencyProperty BlendModeProperty =
        DependencyProperty.Register(nameof(BlendMode), typeof(BlendEffectMode), typeof(BlurEffectControl), new PropertyMetadata(BlendEffectMode.SoftLight, OnBlurPropertyChanged));

    public BlendEffectMode NoiseBlendMode
    {
        get => (BlendEffectMode)GetValue(NoiseBlendModeProperty);
        set => SetValue(NoiseBlendModeProperty, value);
    }

    public static readonly DependencyProperty NoiseBlendModeProperty =
        DependencyProperty.Register(nameof(NoiseBlendMode), typeof(BlendEffectMode), typeof(BlurEffectControl), new PropertyMetadata(BlendEffectMode.Screen, OnBlurPropertyChanged));

    public BlurSourceType SourceType
    {
        get => (BlurSourceType)GetValue(SourceTypeProperty);
        set => SetValue(SourceTypeProperty, value);
    }
    public static readonly DependencyProperty SourceTypeProperty =
        DependencyProperty.Register(nameof(SourceType), typeof(BlurSourceType), typeof(BlurEffectControl), new PropertyMetadata(BlurSourceType.Backdrop, OnBlurPropertyChanged));

    public CompositionStretch NoiseStretch
    {
        get { return (CompositionStretch)GetValue(NoiseStretchProperty); }
        set { SetValue(NoiseStretchProperty, value); }
    }

    public static readonly DependencyProperty NoiseStretchProperty =
        DependencyProperty.Register(nameof(NoiseStretch), typeof(CompositionStretch), typeof(BlurEffectControl), new PropertyMetadata(CompositionStretch.UniformToFill, OnBlurPropertyChanged));

    #endregion

    private static void OnBlurPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (BlurEffectControl)d;
        control?.UpdateManagerProperties();
    }

    private void UpdateManagerProperties()
    {
        if (_blurManager == null)
            return;

        if (IsBlurEnabled)
        {
            _blurManager.IsTintEnabled = IsTintEnabled;
            _blurManager.TintTargetMode = TintTargetMode;
            _blurManager.TintColor = TintColor;
            _blurManager.SurfaceSource = SurfaceSource;
            _blurManager.SurfaceBrushSource = SurfaceBrushSource;
            _blurManager.VisualSource = VisualSource;
            _blurManager.CustomSourceBrush = CustomSourceBrush;
            _blurManager.NoiseUri = NoiseUri;
            _blurManager.SourceType = SourceType;
            _blurManager.BlurAmount = BlurAmount;
            _blurManager.UseNoise = UseNoise;
            _blurManager.BorderMode = BorderMode;
            _blurManager.Optimization = Optimization;
            _blurManager.BlendMode = BlendMode;
            _blurManager.NoiseBlendMode = NoiseBlendMode;

            _blurManager.Refresh();
        }
        else
        {
            _blurManager.DisableBlur();
        }
    }

    public BlurEffectManager GetBlurEffectManager() => _blurManager;
    public CompositionBrush GetBrush() => _blurManager?.GetBrush();
    public Compositor GetCompositor() => _blurManager?.GetCompositor();
}
