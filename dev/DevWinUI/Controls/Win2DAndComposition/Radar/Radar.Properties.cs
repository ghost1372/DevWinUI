namespace DevWinUI;

public sealed partial class Radar
{
    public RadarMode Mode
    {
        get { return (RadarMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(RadarMode), typeof(Radar), new PropertyMetadata(RadarMode.Sweep, OnChanged));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(Radar), new PropertyMetadata(1.0, OnChanged));

    public double Trail
    {
        get { return (double)GetValue(TrailProperty); }
        set { SetValue(TrailProperty, value); }
    }

    public static readonly DependencyProperty TrailProperty =
        DependencyProperty.Register(nameof(Trail), typeof(double), typeof(Radar), new PropertyMetadata(0.35, OnChanged));

    public double RadarScale
    {
        get { return (double)GetValue(RadarScaleProperty); }
        set { SetValue(RadarScaleProperty, value); }
    }

    public static readonly DependencyProperty RadarScaleProperty =
        DependencyProperty.Register(nameof(RadarScale), typeof(double), typeof(Radar), new PropertyMetadata(0.9, OnChanged));

    public int RingCount
    {
        get { return (int)GetValue(RingCountProperty); }
        set { SetValue(RingCountProperty, value); }
    }

    public static readonly DependencyProperty RingCountProperty =
        DependencyProperty.Register(nameof(RingCount), typeof(int), typeof(Radar), new PropertyMetadata(6, OnChanged));

    public int SpokeCount
    {
        get { return (int)GetValue(SpokeCountProperty); }
        set { SetValue(SpokeCountProperty, value); }
    }

    public static readonly DependencyProperty SpokeCountProperty =
        DependencyProperty.Register(nameof(SpokeCount), typeof(int), typeof(Radar), new PropertyMetadata(12, OnChanged));

    public bool Ticks
    {
        get { return (bool)GetValue(TicksProperty); }
        set { SetValue(TicksProperty, value); }
    }

    public static readonly DependencyProperty TicksProperty =
        DependencyProperty.Register(nameof(Ticks), typeof(bool), typeof(Radar), new PropertyMetadata(true, OnChanged));

    public int Targets
    {
        get { return (int)GetValue(TargetsProperty); }
        set { SetValue(TargetsProperty, value); }
    }

    public static readonly DependencyProperty TargetsProperty =
        DependencyProperty.Register(nameof(Targets), typeof(int), typeof(Radar), new PropertyMetadata(6, OnChanged));

    public RadarVisibility TargetVisibility
    {
        get { return (RadarVisibility)GetValue(TargetVisibilityProperty); }
        set { SetValue(TargetVisibilityProperty, value); }
    }

    public static readonly DependencyProperty TargetVisibilityProperty =
        DependencyProperty.Register(nameof(TargetVisibility), typeof(RadarVisibility), typeof(Radar), new PropertyMetadata(RadarVisibility.Always, OnChanged));

    public RadarVisibility BlipVisibility
    {
        get { return (RadarVisibility)GetValue(BlipVisibilityProperty); }
        set { SetValue(BlipVisibilityProperty, value); }
    }

    public static readonly DependencyProperty BlipVisibilityProperty =
        DependencyProperty.Register(nameof(BlipVisibility), typeof(RadarVisibility), typeof(Radar), new PropertyMetadata(RadarVisibility.Always, OnChanged));

    public double ScanFade
    {
        get { return (double)GetValue(ScanFadeProperty); }
        set { SetValue(ScanFadeProperty, value); }
    }

    public static readonly DependencyProperty ScanFadeProperty =
        DependencyProperty.Register(nameof(ScanFade), typeof(double), typeof(Radar), new PropertyMetadata(2.0, OnChanged));

    public Brush? TargetBrush
    {
        get { return (Brush?)GetValue(TargetBrushProperty); }
        set { SetValue(TargetBrushProperty, value); }
    }

    public static readonly DependencyProperty TargetBrushProperty =
        DependencyProperty.Register(nameof(TargetBrush), typeof(Brush), typeof(Radar), new PropertyMetadata(null, OnChanged));

    public IList<RadarBlip> Blips
    {
        get { return (IList<RadarBlip>)GetValue(BlipsProperty); }
        set { SetValue(BlipsProperty, value); }
    }

    public static readonly DependencyProperty BlipsProperty =
        DependencyProperty.Register(nameof(Blips), typeof(IList<RadarBlip>), typeof(Radar), new PropertyMetadata(null, OnBlipsChanged));

    public double Clutter
    {
        get { return (double)GetValue(ClutterProperty); }
        set { SetValue(ClutterProperty, value); }
    }

    public static readonly DependencyProperty ClutterProperty =
        DependencyProperty.Register(nameof(Clutter), typeof(double), typeof(Radar), new PropertyMetadata(0.35, OnChanged));

    public double Glow
    {
        get { return (double)GetValue(GlowProperty); }
        set { SetValue(GlowProperty, value); }
    }

    public static readonly DependencyProperty GlowProperty =
        DependencyProperty.Register(nameof(Glow), typeof(double), typeof(Radar), new PropertyMetadata(1.0, OnChanged));

    public double Brightness
    {
        get { return (double)GetValue(BrightnessProperty); }
        set { SetValue(BrightnessProperty, value); }
    }

    public static readonly DependencyProperty BrightnessProperty =
        DependencyProperty.Register(nameof(Brightness), typeof(double), typeof(Radar), new PropertyMetadata(1.0, OnChanged));

    public double GridOpacity
    {
        get { return (double)GetValue(GridOpacityProperty); }
        set { SetValue(GridOpacityProperty, value); }
    }

    public static readonly DependencyProperty GridOpacityProperty =
        DependencyProperty.Register(nameof(GridOpacity), typeof(double), typeof(Radar), new PropertyMetadata(1.0, OnChanged));

    public double LineWidth
    {
        get { return (double)GetValue(LineWidthProperty); }
        set { SetValue(LineWidthProperty, value); }
    }

    public static readonly DependencyProperty LineWidthProperty =
        DependencyProperty.Register(nameof(LineWidth), typeof(double), typeof(Radar), new PropertyMetadata(1.0, OnChanged));

    public double Tilt
    {
        get { return (double)GetValue(TiltProperty); }
        set { SetValue(TiltProperty, value); }
    }

    public static readonly DependencyProperty TiltProperty =
        DependencyProperty.Register(nameof(Tilt), typeof(double), typeof(Radar), new PropertyMetadata(0.0, OnChanged));

    public double CenterX
    {
        get { return (double)GetValue(CenterXProperty); }
        set { SetValue(CenterXProperty, value); }
    }

    public static readonly DependencyProperty CenterXProperty =
        DependencyProperty.Register(nameof(CenterX), typeof(double), typeof(Radar), new PropertyMetadata(0.5, OnChanged));

    public double CenterY
    {
        get { return (double)GetValue(CenterYProperty); }
        set { SetValue(CenterYProperty, value); }
    }

    public static readonly DependencyProperty CenterYProperty =
        DependencyProperty.Register(nameof(CenterY), typeof(double), typeof(Radar), new PropertyMetadata(0.5, OnChanged));

    public bool MouseInteraction
    {
        get { return (bool)GetValue(MouseInteractionProperty); }
        set { SetValue(MouseInteractionProperty, value); }
    }

    public static readonly DependencyProperty MouseInteractionProperty =
        DependencyProperty.Register(nameof(MouseInteraction), typeof(bool), typeof(Radar), new PropertyMetadata(true, OnChanged));

    public bool Intro
    {
        get { return (bool)GetValue(IntroProperty); }
        set { SetValue(IntroProperty, value); }
    }

    public static readonly DependencyProperty IntroProperty =
        DependencyProperty.Register(nameof(Intro), typeof(bool), typeof(Radar), new PropertyMetadata(true, OnChanged));

    public double Grain
    {
        get { return (double)GetValue(GrainProperty); }
        set { SetValue(GrainProperty, value); }
    }

    public static readonly DependencyProperty GrainProperty =
        DependencyProperty.Register(nameof(Grain), typeof(double), typeof(Radar), new PropertyMetadata(0.03, OnChanged));

    public double Fade
    {
        get { return (double)GetValue(FadeProperty); }
        set { SetValue(FadeProperty, value); }
    }

    public static readonly DependencyProperty FadeProperty =
        DependencyProperty.Register(nameof(Fade), typeof(double), typeof(Radar), new PropertyMetadata(0.0, OnChanged));

    public bool LightMode
    {
        get { return (bool)GetValue(LightModeProperty); }
        set { SetValue(LightModeProperty, value); }
    }

    public static readonly DependencyProperty LightModeProperty =
        DependencyProperty.Register(nameof(LightMode), typeof(bool), typeof(Radar), new PropertyMetadata(false, OnChanged));

    public bool Paused
    {
        get { return (bool)GetValue(PausedProperty); }
        set { SetValue(PausedProperty, value); }
    }

    public static readonly DependencyProperty PausedProperty =
        DependencyProperty.Register(nameof(Paused), typeof(bool), typeof(Radar), new PropertyMetadata(false, OnChanged));

    public double MaxDevicePixelRatio
    {
        get { return (double)GetValue(MaxDevicePixelRatioProperty); }
        set { SetValue(MaxDevicePixelRatioProperty, value); }
    }

    public static readonly DependencyProperty MaxDevicePixelRatioProperty =
        DependencyProperty.Register(nameof(MaxDevicePixelRatio), typeof(double), typeof(Radar), new PropertyMetadata(2.0, OnChanged));

    public object? ItemsSource
    {
        get { return GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(Radar), new PropertyMetadata(null, OnItemsSourceChanged));

    public IRadarBlipMapper? ItemMapper
    {
        get { return (IRadarBlipMapper?)GetValue(ItemMapperProperty); }
        set { SetValue(ItemMapperProperty, value); }
    }

    public static readonly DependencyProperty ItemMapperProperty =
        DependencyProperty.Register(nameof(ItemMapper), typeof(IRadarBlipMapper), typeof(Radar), new PropertyMetadata(null, OnItemMapperChanged));

    public IList<RadarZone> Zones
    {
        get { return (IList<RadarZone>)GetValue(ZonesProperty); }
        set { SetValue(ZonesProperty, value); }
    }

    public static readonly DependencyProperty ZonesProperty =
        DependencyProperty.Register(nameof(Zones), typeof(IList<RadarZone>), typeof(Radar), new PropertyMetadata(null, OnZonesChanged));

    public IList<RadarJam> Jams
    {
        get { return (IList<RadarJam>)GetValue(JamsProperty); }
        set { SetValue(JamsProperty, value); }
    }

    public static readonly DependencyProperty JamsProperty =
        DependencyProperty.Register(nameof(Jams), typeof(IList<RadarJam>), typeof(Radar), new PropertyMetadata(null, OnJamsChanged));

    public bool ShowRangeLabels
    {
        get { return (bool)GetValue(ShowRangeLabelsProperty); }
        set { SetValue(ShowRangeLabelsProperty, value); }
    }

    public static readonly DependencyProperty ShowRangeLabelsProperty =
        DependencyProperty.Register(nameof(ShowRangeLabels), typeof(bool), typeof(Radar), new PropertyMetadata(false, OnChanged));

    public bool ShowCompass
    {
        get { return (bool)GetValue(ShowCompassProperty); }
        set { SetValue(ShowCompassProperty, value); }
    }

    public static readonly DependencyProperty ShowCompassProperty =
        DependencyProperty.Register(nameof(ShowCompass), typeof(bool), typeof(Radar), new PropertyMetadata(false, OnChanged));

    public bool ShowReadout
    {
        get { return (bool)GetValue(ShowReadoutProperty); }
        set { SetValue(ShowReadoutProperty, value); }
    }

    public static readonly DependencyProperty ShowReadoutProperty =
        DependencyProperty.Register(nameof(ShowReadout), typeof(bool), typeof(Radar), new PropertyMetadata(false, OnChanged));

    public double MaxRange
    {
        get { return (double)GetValue(MaxRangeProperty); }
        set { SetValue(MaxRangeProperty, value); }
    }

    public static readonly DependencyProperty MaxRangeProperty =
        DependencyProperty.Register(nameof(MaxRange), typeof(double), typeof(Radar), new PropertyMetadata(100.0, OnChanged));

    public string RangeUnit
    {
        get { return (string)GetValue(RangeUnitProperty); }
        set { SetValue(RangeUnitProperty, value); }
    }

    public static readonly DependencyProperty RangeUnitProperty =
        DependencyProperty.Register(nameof(RangeUnit), typeof(string), typeof(Radar), new PropertyMetadata(string.Empty, OnChanged));

    public double Phosphor
    {
        get { return (double)GetValue(PhosphorProperty); }
        set { SetValue(PhosphorProperty, value); }
    }

    public static readonly DependencyProperty PhosphorProperty =
        DependencyProperty.Register(nameof(Phosphor), typeof(double), typeof(Radar), new PropertyMetadata(0.0, OnChanged));

    private static void OnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).OnPropsChanged();
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).ApplyItemsSource(e.OldValue, e.NewValue);
    }

    private static void OnItemMapperChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).RebuildSource();
    }

    private static void OnZonesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).OnZonesChanged(e.OldValue as IList<RadarZone>, e.NewValue as IList<RadarZone>);
    }

    private static void OnJamsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).OnJamsChanged(e.OldValue as IList<RadarJam>, e.NewValue as IList<RadarJam>);
    }

    private static void OnBlipsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Radar)d).OnBlipsChanged(e.OldValue as IList<RadarBlip>, e.NewValue as IList<RadarBlip>);
    }
}
