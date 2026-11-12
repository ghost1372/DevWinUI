namespace DevWinUI;

public sealed partial class ParticleGlobe
{
    #region Geometry / particles

    public int ParticleCount
    {
        get { return (int)GetValue(ParticleCountProperty); }
        set { SetValue(ParticleCountProperty, value); }
    }

    public static readonly DependencyProperty ParticleCountProperty =
        DependencyProperty.Register(nameof(ParticleCount), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(26000, OnRebuildPropertyChanged));

    public double ParticleSize
    {
        get { return (double)GetValue(ParticleSizeProperty); }
        set { SetValue(ParticleSizeProperty, value); }
    }

    public static readonly DependencyProperty ParticleSizeProperty =
        DependencyProperty.Register(nameof(ParticleSize), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.0, OnSettingsPropertyChanged));

    public double GlobeScale
    {
        get { return (double)GetValue(GlobeScaleProperty); }
        set { SetValue(GlobeScaleProperty, value); }
    }

    public static readonly DependencyProperty GlobeScaleProperty =
        DependencyProperty.Register(nameof(GlobeScale), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.74, OnSettingsPropertyChanged));

    public double TiltAngle
    {
        get { return (double)GetValue(TiltAngleProperty); }
        set { SetValue(TiltAngleProperty, value); }
    }

    public static readonly DependencyProperty TiltAngleProperty =
        DependencyProperty.Register(nameof(TiltAngle), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(18.0, OnSettingsPropertyChanged));

    public double LandThreshold
    {
        get { return (double)GetValue(LandThresholdProperty); }
        set { SetValue(LandThresholdProperty, value); }
    }

    public static readonly DependencyProperty LandThresholdProperty =
        DependencyProperty.Register(nameof(LandThreshold), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.52, OnRebuildPropertyChanged));

    public Uri LandMaskSource
    {
        get { return (Uri)GetValue(LandMaskSourceProperty); }
        set { SetValue(LandMaskSourceProperty, value); }
    }

    public static readonly DependencyProperty LandMaskSourceProperty =
        DependencyProperty.Register(nameof(LandMaskSource), typeof(Uri), typeof(ParticleGlobe), new PropertyMetadata(null, OnLandMaskSourceChanged));

    public Uri WorldMapDataPath
    {
        get { return (Uri)GetValue(WorldMapDataPathProperty); }
        set { SetValue(WorldMapDataPathProperty, value); }
    }

    public static readonly DependencyProperty WorldMapDataPathProperty =
        DependencyProperty.Register(nameof(WorldMapDataPath), typeof(Uri), typeof(ParticleGlobe), new PropertyMetadata(null, OnWorldMapSourceChanged));

    #endregion

    #region Motion / interaction

    public double RotationSpeed
    {
        get { return (double)GetValue(RotationSpeedProperty); }
        set { SetValue(RotationSpeedProperty, value); }
    }

    public static readonly DependencyProperty RotationSpeedProperty =
        DependencyProperty.Register(nameof(RotationSpeed), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.12, OnSettingsPropertyChanged));

    public bool IsAutoRotateEnabled
    {
        get { return (bool)GetValue(IsAutoRotateEnabledProperty); }
        set { SetValue(IsAutoRotateEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAutoRotateEnabledProperty =
        DependencyProperty.Register(nameof(IsAutoRotateEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsInteractive
    {
        get { return (bool)GetValue(IsInteractiveProperty); }
        set { SetValue(IsInteractiveProperty, value); }
    }

    public static readonly DependencyProperty IsInteractiveProperty =
        DependencyProperty.Register(nameof(IsInteractive), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public double ArcFrequency
    {
        get { return (double)GetValue(ArcFrequencyProperty); }
        set { SetValue(ArcFrequencyProperty, value); }
    }

    public static readonly DependencyProperty ArcFrequencyProperty =
        DependencyProperty.Register(nameof(ArcFrequency), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.6, OnSettingsPropertyChanged));

    public int OrbitRingCount
    {
        get { return (int)GetValue(OrbitRingCountProperty); }
        set { SetValue(OrbitRingCountProperty, value); }
    }

    public static readonly DependencyProperty OrbitRingCountProperty =
        DependencyProperty.Register(nameof(OrbitRingCount), typeof(int), typeof(ParticleGlobe), new PropertyMetadata(3, OnRebuildPropertyChanged));

    #endregion

    #region Layers

    public bool ShowStars
    {
        get { return (bool)GetValue(ShowStarsProperty); }
        set { SetValue(ShowStarsProperty, value); }
    }

    public static readonly DependencyProperty ShowStarsProperty =
        DependencyProperty.Register(nameof(ShowStars), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowGrid
    {
        get { return (bool)GetValue(ShowGridProperty); }
        set { SetValue(ShowGridProperty, value); }
    }

    public static readonly DependencyProperty ShowGridProperty =
        DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowArcs
    {
        get { return (bool)GetValue(ShowArcsProperty); }
        set { SetValue(ShowArcsProperty, value); }
    }

    public static readonly DependencyProperty ShowArcsProperty =
        DependencyProperty.Register(nameof(ShowArcs), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowCities
    {
        get { return (bool)GetValue(ShowCitiesProperty); }
        set { SetValue(ShowCitiesProperty, value); }
    }

    public static readonly DependencyProperty ShowCitiesProperty =
        DependencyProperty.Register(nameof(ShowCities), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowOrbits
    {
        get { return (bool)GetValue(ShowOrbitsProperty); }
        set { SetValue(ShowOrbitsProperty, value); }
    }

    public static readonly DependencyProperty ShowOrbitsProperty =
        DependencyProperty.Register(nameof(ShowOrbits), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowHud
    {
        get { return (bool)GetValue(ShowHudProperty); }
        set { SetValue(ShowHudProperty, value); }
    }

    public static readonly DependencyProperty ShowHudProperty =
        DependencyProperty.Register(nameof(ShowHud), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    #endregion

    #region Brushes (supplied through ThemeResource in Generic.xaml)

    public Brush LandBrush
    {
        get { return (Brush)GetValue(LandBrushProperty); }
        set { SetValue(LandBrushProperty, value); }
    }

    public static readonly DependencyProperty LandBrushProperty =
        DependencyProperty.Register(nameof(LandBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush OceanBrush
    {
        get { return (Brush)GetValue(OceanBrushProperty); }
        set { SetValue(OceanBrushProperty, value); }
    }

    public static readonly DependencyProperty OceanBrushProperty =
        DependencyProperty.Register(nameof(OceanBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush AtmosphereBrush
    {
        get { return (Brush)GetValue(AtmosphereBrushProperty); }
        set { SetValue(AtmosphereBrushProperty, value); }
    }

    public static readonly DependencyProperty AtmosphereBrushProperty =
        DependencyProperty.Register(nameof(AtmosphereBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush GridBrush
    {
        get { return (Brush)GetValue(GridBrushProperty); }
        set { SetValue(GridBrushProperty, value); }
    }

    public static readonly DependencyProperty GridBrushProperty =
        DependencyProperty.Register(nameof(GridBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush ArcBrush
    {
        get { return (Brush)GetValue(ArcBrushProperty); }
        set { SetValue(ArcBrushProperty, value); }
    }

    public static readonly DependencyProperty ArcBrushProperty =
        DependencyProperty.Register(nameof(ArcBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush AccentBrush
    {
        get { return (Brush)GetValue(AccentBrushProperty); }
        set { SetValue(AccentBrushProperty, value); }
    }

    public static readonly DependencyProperty AccentBrushProperty =
        DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    public Brush StarBrush
    {
        get { return (Brush)GetValue(StarBrushProperty); }
        set { SetValue(StarBrushProperty, value); }
    }

    public static readonly DependencyProperty StarBrushProperty =
        DependencyProperty.Register(nameof(StarBrush), typeof(Brush), typeof(ParticleGlobe), new PropertyMetadata(null, OnSettingsPropertyChanged));

    #endregion

    #region Countries

    public bool IsCountryHoverEnabled
    {
        get { return (bool)GetValue(IsCountryHoverEnabledProperty); }
        set { SetValue(IsCountryHoverEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCountryHoverEnabledProperty =
        DependencyProperty.Register(nameof(IsCountryHoverEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsCountrySelectionEnabled
    {
        get { return (bool)GetValue(IsCountrySelectionEnabledProperty); }
        set { SetValue(IsCountrySelectionEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCountrySelectionEnabledProperty =
        DependencyProperty.Register(nameof(IsCountrySelectionEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowCountryTooltip
    {
        get { return (bool)GetValue(ShowCountryTooltipProperty); }
        set { SetValue(ShowCountryTooltipProperty, value); }
    }

    public static readonly DependencyProperty ShowCountryTooltipProperty =
        DependencyProperty.Register(nameof(ShowCountryTooltip), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public string SelectedCountryCode
    {
        get { return (string)GetValue(SelectedCountryCodeProperty); }
        set { SetValue(SelectedCountryCodeProperty, value); }
    }

    public static readonly DependencyProperty SelectedCountryCodeProperty =
        DependencyProperty.Register(nameof(SelectedCountryCode), typeof(string), typeof(ParticleGlobe), new PropertyMetadata(null, OnSelectedCountryCodeChanged));

    public string HoveredCountryCode
    {
        get { return (string)GetValue(HoveredCountryCodeProperty); }
        private set { SetValue(HoveredCountryCodeProperty, value); }
    }

    public static readonly DependencyProperty HoveredCountryCodeProperty =
        DependencyProperty.Register(nameof(HoveredCountryCode), typeof(string), typeof(ParticleGlobe), new PropertyMetadata(null));

    public bool IsFlyToOnSelectEnabled
    {
        get { return (bool)GetValue(IsFlyToOnSelectEnabledProperty); }
        set { SetValue(IsFlyToOnSelectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsFlyToOnSelectEnabledProperty =
        DependencyProperty.Register(nameof(IsFlyToOnSelectEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public double FlyToDuration
    {
        get { return (double)GetValue(FlyToDurationProperty); }
        set { SetValue(FlyToDurationProperty, value); }
    }

    public static readonly DependencyProperty FlyToDurationProperty =
        DependencyProperty.Register(nameof(FlyToDuration), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(1.4, OnSettingsPropertyChanged));

    public double FlyToZoom
    {
        get { return (double)GetValue(FlyToZoomProperty); }
        set { SetValue(FlyToZoomProperty, value); }
    }

    public static readonly DependencyProperty FlyToZoomProperty =
        DependencyProperty.Register(nameof(FlyToZoom), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.0, OnSettingsPropertyChanged));

    public bool ShowCountryOutline
    {
        get { return (bool)GetValue(ShowCountryOutlineProperty); }
        set { SetValue(ShowCountryOutlineProperty, value); }
    }

    public static readonly DependencyProperty ShowCountryOutlineProperty =
        DependencyProperty.Register(nameof(ShowCountryOutline), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    #endregion

    #region Cursor

    public bool IsClickRippleEnabled
    {
        get { return (bool)GetValue(IsClickRippleEnabledProperty); }
        set { SetValue(IsClickRippleEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsClickRippleEnabledProperty =
        DependencyProperty.Register(nameof(IsClickRippleEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowCursorReticle
    {
        get { return (bool)GetValue(ShowCursorReticleProperty); }
        set { SetValue(ShowCursorReticleProperty, value); }
    }

    public static readonly DependencyProperty ShowCursorReticleProperty =
        DependencyProperty.Register(nameof(ShowCursorReticle), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsCursorForceFieldEnabled
    {
        get { return (bool)GetValue(IsCursorForceFieldEnabledProperty); }
        set { SetValue(IsCursorForceFieldEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCursorForceFieldEnabledProperty =
        DependencyProperty.Register(nameof(IsCursorForceFieldEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool IsClickToConnectEnabled
    {
        get { return (bool)GetValue(IsClickToConnectEnabledProperty); }
        set { SetValue(IsClickToConnectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsClickToConnectEnabledProperty =
        DependencyProperty.Register(nameof(IsClickToConnectEnabled), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(true, OnSettingsPropertyChanged));

    public bool ShowCountryBars
    {
        get { return (bool)GetValue(ShowCountryBarsProperty); }
        set { SetValue(ShowCountryBarsProperty, value); }
    }

    public static readonly DependencyProperty ShowCountryBarsProperty =
        DependencyProperty.Register(nameof(ShowCountryBars), typeof(bool), typeof(ParticleGlobe), new PropertyMetadata(false, OnSettingsPropertyChanged));

    public double BarMaxHeight
    {
        get { return (double)GetValue(BarMaxHeightProperty); }
        set { SetValue(BarMaxHeightProperty, value); }
    }

    public static readonly DependencyProperty BarMaxHeightProperty =
        DependencyProperty.Register(nameof(BarMaxHeight), typeof(double), typeof(ParticleGlobe), new PropertyMetadata(0.3, OnSettingsPropertyChanged));

    #endregion

    #region Callbacks

    private static void OnSettingsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ParticleGlobe)d).SyncSettings();
    }

    private static void OnRebuildPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var globe = (ParticleGlobe)d;
        globe.SyncSettings();
        globe._needsRebuild = true;
    }

    private static void OnSelectedCountryCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((ParticleGlobe)d).OnSelectedCodeChanged(e.OldValue as string, e.NewValue as string);
    }

    private static void OnLandMaskSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var globe = (ParticleGlobe)d;
        globe._maskUri = e.NewValue as Uri;
        globe.ReloadMask();
    }

    private static void OnWorldMapSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var globe = (ParticleGlobe)d;
        globe._geoSource = e.NewValue as Uri;
        globe.ReloadGeoMask();
    }

    #endregion
}
