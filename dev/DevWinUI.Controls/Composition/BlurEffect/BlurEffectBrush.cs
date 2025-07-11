namespace DevWinUI;
public partial class BlurEffectBrush : XamlCompositionBrushBase
{
    private BlurEffectManager _manager;
    private FrameworkElement _dummyElement;

    #region Dependency Properties
    public bool IsBlurEnabled
    {
        get { return (bool)GetValue(IsBlurEnabledProperty); }
        set { SetValue(IsBlurEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsBlurEnabledProperty =
        DependencyProperty.Register(nameof(IsBlurEnabled), typeof(bool), typeof(BlurEffectBrush), new PropertyMetadata(true, OnBlurPropertyChanged));

    public bool IsTintEnabled
    {
        get { return (bool)GetValue(IsTintEnabledProperty); }
        set { SetValue(IsTintEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsTintEnabledProperty =
        DependencyProperty.Register(nameof(IsTintEnabled), typeof(bool), typeof(BlurEffectBrush), new PropertyMetadata(false, OnBlurPropertyChanged));

    public BlurTintTarget TintTargetMode
    {
        get { return (BlurTintTarget)GetValue(TintTargetModeProperty); }
        set { SetValue(TintTargetModeProperty, value); }
    }

    public static readonly DependencyProperty TintTargetModeProperty =
        DependencyProperty.Register(nameof(TintTargetMode), typeof(BlurTintTarget), typeof(BlurEffectBrush), new PropertyMetadata(BlurTintTarget.Foreground, OnBlurPropertyChanged));

    public ICompositionSurface SurfaceSource
    {
        get { return (ICompositionSurface)GetValue(SurfaceSourceProperty); }
        set { SetValue(SurfaceSourceProperty, value); }
    }

    public static readonly DependencyProperty SurfaceSourceProperty =
        DependencyProperty.Register(nameof(SurfaceSource), typeof(ICompositionSurface), typeof(BlurEffectBrush), new PropertyMetadata(null, OnBlurPropertyChanged));

    public CompositionSurfaceBrush SurfaceBrushSource
    {
        get { return (CompositionSurfaceBrush)GetValue(SurfaceBrushSourceProperty); }
        set { SetValue(SurfaceBrushSourceProperty, value); }
    }

    public static readonly DependencyProperty SurfaceBrushSourceProperty =
        DependencyProperty.Register(nameof(SurfaceBrushSource), typeof(CompositionSurfaceBrush), typeof(BlurEffectBrush), new PropertyMetadata(null, OnBlurPropertyChanged));

    public Visual VisualSource
    {
        get { return (Visual)GetValue(VisualSourceProperty); }
        set { SetValue(VisualSourceProperty, value); }
    }

    public static readonly DependencyProperty VisualSourceProperty =
        DependencyProperty.Register(nameof(VisualSource), typeof(Visual), typeof(BlurEffectBrush), new PropertyMetadata(null, OnBlurPropertyChanged));

    public CompositionBrush CustomSourceBrush
    {
        get { return (CompositionBrush)GetValue(CustomSourceBrushProperty); }
        set { SetValue(CustomSourceBrushProperty, value); }
    }

    public static readonly DependencyProperty CustomSourceBrushProperty =
        DependencyProperty.Register(nameof(CustomSourceBrush), typeof(CompositionBrush), typeof(BlurEffectBrush), new PropertyMetadata(null, OnBlurPropertyChanged));

    public double BlurAmount
    {
        get => (double)GetValue(BlurAmountProperty);
        set => SetValue(BlurAmountProperty, value);
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(BlurEffectBrush), new PropertyMetadata(10.0, OnBlurPropertyChanged));

    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    public static readonly DependencyProperty TintColorProperty =
        DependencyProperty.Register(nameof(TintColor), typeof(Color), typeof(BlurEffectBrush), new PropertyMetadata(Colors.Transparent, OnBlurPropertyChanged));

    public bool UseNoise
    {
        get => (bool)GetValue(UseNoiseProperty);
        set => SetValue(UseNoiseProperty, value);
    }

    public static readonly DependencyProperty UseNoiseProperty =
        DependencyProperty.Register(nameof(UseNoise), typeof(bool), typeof(BlurEffectBrush), new PropertyMetadata(false, OnBlurPropertyChanged));

    public string NoiseUri
    {
        get => (string)GetValue(NoiseUriProperty);
        set => SetValue(NoiseUriProperty, value);
    }

    public static readonly DependencyProperty NoiseUriProperty =
        DependencyProperty.Register(nameof(NoiseUri), typeof(string), typeof(BlurEffectBrush), new PropertyMetadata("ms-appx:///Assets/Noise/Noise.jpg", OnBlurPropertyChanged));

    public EffectBorderMode BorderMode
    {
        get => (EffectBorderMode)GetValue(BorderModeProperty);
        set => SetValue(BorderModeProperty, value);
    }

    public static readonly DependencyProperty BorderModeProperty =
        DependencyProperty.Register(nameof(BorderMode), typeof(EffectBorderMode), typeof(BlurEffectBrush), new PropertyMetadata(EffectBorderMode.Hard, OnBlurPropertyChanged));

    public EffectOptimization Optimization
    {
        get => (EffectOptimization)GetValue(OptimizationProperty);
        set => SetValue(OptimizationProperty, value);
    }

    public static readonly DependencyProperty OptimizationProperty =
        DependencyProperty.Register(nameof(Optimization), typeof(EffectOptimization), typeof(BlurEffectBrush), new PropertyMetadata(EffectOptimization.Balanced, OnBlurPropertyChanged));

    public BlendEffectMode BlendMode
    {
        get => (BlendEffectMode)GetValue(BlendModeProperty);
        set => SetValue(BlendModeProperty, value);
    }

    public static readonly DependencyProperty BlendModeProperty =
        DependencyProperty.Register(nameof(BlendMode), typeof(BlendEffectMode), typeof(BlurEffectBrush), new PropertyMetadata(BlendEffectMode.SoftLight, OnBlurPropertyChanged));

    public BlendEffectMode NoiseBlendMode
    {
        get => (BlendEffectMode)GetValue(NoiseBlendModeProperty);
        set => SetValue(NoiseBlendModeProperty, value);
    }

    public static readonly DependencyProperty NoiseBlendModeProperty =
        DependencyProperty.Register(nameof(NoiseBlendMode), typeof(BlendEffectMode), typeof(BlurEffectBrush), new PropertyMetadata(BlendEffectMode.Screen, OnBlurPropertyChanged));

    public BlurSourceType SourceType
    {
        get => (BlurSourceType)GetValue(SourceTypeProperty);
        set => SetValue(SourceTypeProperty, value);
    }
    public static readonly DependencyProperty SourceTypeProperty =
        DependencyProperty.Register(nameof(SourceType), typeof(BlurSourceType), typeof(BlurEffectBrush), new PropertyMetadata(BlurSourceType.Backdrop, OnBlurPropertyChanged));

    public CompositionStretch NoiseStretch
    {
        get { return (CompositionStretch)GetValue(NoiseStretchProperty); }
        set { SetValue(NoiseStretchProperty, value); }
    }

    public static readonly DependencyProperty NoiseStretchProperty =
        DependencyProperty.Register(nameof(NoiseStretch), typeof(CompositionStretch), typeof(BlurEffectBrush), new PropertyMetadata(CompositionStretch.UniformToFill, OnBlurPropertyChanged));

    #endregion
    protected override void OnConnected()
    {
        if (CompositionBrush == null)
        {
            _dummyElement = new Grid(); // Not used visually, just for compositor access
            _manager = new BlurEffectManager(_dummyElement)
            {
                IsTintEnabled = this.IsTintEnabled,
                TintTargetMode = this.TintTargetMode,
                TintColor = this.TintColor,
                SurfaceSource = this.SurfaceSource,
                SurfaceBrushSource = this.SurfaceBrushSource,
                VisualSource = this.VisualSource,
                CustomSourceBrush = this.CustomSourceBrush,
                NoiseUri = this.NoiseUri,
                SourceType = this.SourceType,
                BlurAmount = this.BlurAmount,
                UseNoise = this.UseNoise,
                BorderMode = this.BorderMode,
                Optimization = this.Optimization,
                BlendMode = this.BlendMode,
                NoiseBlendMode = this.NoiseBlendMode
            };

            _manager.Refresh();
            CompositionBrush = _manager.GetBrush();
        }
    }

    protected override void OnDisconnected()
    {
        _manager?.DisableBlur();
        _manager = null;

        CompositionBrush?.Dispose();
        CompositionBrush = null;
    }

    private static void OnBlurPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlurEffectBrush b && b._manager != null)
        {
            b._manager.IsTintEnabled = b.IsTintEnabled;
            b._manager.TintTargetMode = b.TintTargetMode;
            b._manager.TintColor = b.TintColor;
            b._manager.SurfaceSource = b.SurfaceSource;
            b._manager.SurfaceBrushSource = b.SurfaceBrushSource;
            b._manager.VisualSource = b.VisualSource;
            b._manager.CustomSourceBrush = b.CustomSourceBrush;
            b._manager.NoiseUri = b.NoiseUri;
            b._manager.SourceType = b.SourceType;
            b._manager.BlurAmount = b.BlurAmount;
            b._manager.UseNoise = b.UseNoise;
            b._manager.BorderMode = b.BorderMode;
            b._manager.Optimization = b.Optimization;
            b._manager.BlendMode = b.BlendMode;
            b._manager.NoiseBlendMode = b.NoiseBlendMode;

            b._manager.Refresh();
        }
    }
}
