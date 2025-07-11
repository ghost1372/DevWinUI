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

    public static readonly DependencyProperty SourceTypeProperty =
        DependencyProperty.Register(nameof(SourceType), typeof(BlurSourceType), typeof(BlurEffectControl), new PropertyMetadata(BlurSourceType.Backdrop, OnBlurPropertyChanged));

    public BlurSourceType SourceType
    {
        get => (BlurSourceType)GetValue(SourceTypeProperty);
        set => SetValue(SourceTypeProperty, value);
    }
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

        _blurManager.BlurAmount = BlurAmount;
        _blurManager.TintColor = TintColor;
        _blurManager.UseNoise = UseNoise;
        _blurManager.NoiseUri = NoiseUri;
        _blurManager.BorderMode = BorderMode;
        _blurManager.Optimization = Optimization;
        _blurManager.BlendMode = BlendMode;
        _blurManager.NoiseBlendMode = NoiseBlendMode;
        _blurManager.SourceType = SourceType;

        _blurManager.Refresh();
    }

    public BlurEffectManager GetBlurEffectManager() => _blurManager;
    public CompositionBrush GetBrush() => _blurManager?.GetBrush();
    public Compositor GetCompositor() => _blurManager?.GetCompositor();
}
