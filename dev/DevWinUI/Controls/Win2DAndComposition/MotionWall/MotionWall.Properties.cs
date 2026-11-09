namespace DevWinUI;

public sealed partial class MotionWall
{
    public object? ItemsSource
    {
        get { return (object?)GetValue(ItemsSourceProperty); }
        set { SetValue(ItemsSourceProperty, value); }
    }

    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(MotionWall), new PropertyMetadata(null, OnItemsSourceChanged));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(MotionWall), new PropertyMetadata(Orientation.Horizontal, OnSettingsChanged));

    public int Lanes
    {
        get { return (int)GetValue(LanesProperty); }
        set { SetValue(LanesProperty, value); }
    }

    public static readonly DependencyProperty LanesProperty =
        DependencyProperty.Register(nameof(Lanes), typeof(int), typeof(MotionWall), new PropertyMetadata(4, OnSettingsChanged));

    public bool AddEdgeLanes
    {
        get { return (bool)GetValue(AddEdgeLanesProperty); }
        set { SetValue(AddEdgeLanesProperty, value); }
    }

    public static readonly DependencyProperty AddEdgeLanesProperty =
        DependencyProperty.Register(nameof(AddEdgeLanes), typeof(bool), typeof(MotionWall), new PropertyMetadata(false, OnSettingsChanged));

    public int ItemsPerLane
    {
        get { return (int)GetValue(ItemsPerLaneProperty); }
        set { SetValue(ItemsPerLaneProperty, value); }
    }

    public static readonly DependencyProperty ItemsPerLaneProperty =
        DependencyProperty.Register(nameof(ItemsPerLane), typeof(int), typeof(MotionWall), new PropertyMetadata(0, OnSettingsChanged));

    public MotionWallDistribution ItemDistribution
    {
        get { return (MotionWallDistribution)GetValue(ItemDistributionProperty); }
        set { SetValue(ItemDistributionProperty, value); }
    }

    public static readonly DependencyProperty ItemDistributionProperty =
        DependencyProperty.Register(nameof(ItemDistribution), typeof(MotionWallDistribution), typeof(MotionWall), new PropertyMetadata(MotionWallDistribution.Shifted, OnSettingsChanged));

    public double Gap
    {
        get { return (double)GetValue(GapProperty); }
        set { SetValue(GapProperty, value); }
    }

    public static readonly DependencyProperty GapProperty =
        DependencyProperty.Register(nameof(Gap), typeof(double), typeof(MotionWall), new PropertyMetadata(16d, OnSettingsChanged));

    public double TileWidth
    {
        get { return (double)GetValue(TileWidthProperty); }
        set { SetValue(TileWidthProperty, value); }
    }

    public static readonly DependencyProperty TileWidthProperty =
        DependencyProperty.Register(nameof(TileWidth), typeof(double), typeof(MotionWall), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double TileHeight
    {
        get { return (double)GetValue(TileHeightProperty); }
        set { SetValue(TileHeightProperty, value); }
    }

    public static readonly DependencyProperty TileHeightProperty =
        DependencyProperty.Register(nameof(TileHeight), typeof(double), typeof(MotionWall), new PropertyMetadata(double.NaN, OnSettingsChanged));

    public double TileAspectRatio
    {
        get { return (double)GetValue(TileAspectRatioProperty); }
        set { SetValue(TileAspectRatioProperty, value); }
    }

    public static readonly DependencyProperty TileAspectRatioProperty =
        DependencyProperty.Register(nameof(TileAspectRatio), typeof(double), typeof(MotionWall), new PropertyMetadata(1.5d, OnSettingsChanged));

    public double TileCornerRadius
    {
        get { return (double)GetValue(TileCornerRadiusProperty); }
        set { SetValue(TileCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty TileCornerRadiusProperty =
        DependencyProperty.Register(nameof(TileCornerRadius), typeof(double), typeof(MotionWall), new PropertyMetadata(12d, OnCardChanged));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MotionWall), new PropertyMetadata(40d, OnSettingsChanged));

    public double LoopDuration
    {
        get { return (double)GetValue(LoopDurationProperty); }
        set { SetValue(LoopDurationProperty, value); }
    }

    public static readonly DependencyProperty LoopDurationProperty =
        DependencyProperty.Register(nameof(LoopDuration), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public MotionWallDirection Direction
    {
        get { return (MotionWallDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(MotionWallDirection), typeof(MotionWall), new PropertyMetadata(MotionWallDirection.Alternate, OnSettingsChanged));

    public double Variance
    {
        get { return (double)GetValue(VarianceProperty); }
        set { SetValue(VarianceProperty, value); }
    }

    public static readonly DependencyProperty VarianceProperty =
        DependencyProperty.Register(nameof(Variance), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public MotionWallVarianceStyle VarianceStyle
    {
        get { return (MotionWallVarianceStyle)GetValue(VarianceStyleProperty); }
        set { SetValue(VarianceStyleProperty, value); }
    }

    public static readonly DependencyProperty VarianceStyleProperty =
        DependencyProperty.Register(nameof(VarianceStyle), typeof(MotionWallVarianceStyle), typeof(MotionWall), new PropertyMetadata(MotionWallVarianceStyle.Random, OnSettingsChanged));

    public bool RandomStart
    {
        get { return (bool)GetValue(RandomStartProperty); }
        set { SetValue(RandomStartProperty, value); }
    }

    public static readonly DependencyProperty RandomStartProperty =
        DependencyProperty.Register(nameof(RandomStart), typeof(bool), typeof(MotionWall), new PropertyMetadata(true, OnSettingsChanged));

    public bool PauseOnHover
    {
        get { return (bool)GetValue(PauseOnHoverProperty); }
        set { SetValue(PauseOnHoverProperty, value); }
    }

    public static readonly DependencyProperty PauseOnHoverProperty =
        DependencyProperty.Register(nameof(PauseOnHover), typeof(bool), typeof(MotionWall), new PropertyMetadata(false, OnSettingsChanged));

    public int MaxFrameRate
    {
        get { return (int)GetValue(MaxFrameRateProperty); }
        set { SetValue(MaxFrameRateProperty, value); }
    }

    public static readonly DependencyProperty MaxFrameRateProperty =
        DependencyProperty.Register(nameof(MaxFrameRate), typeof(int), typeof(MotionWall), new PropertyMetadata(60, OnFrameRateChanged));

    public double Tilt
    {
        get { return (double)GetValue(TiltProperty); }
        set { SetValue(TiltProperty, value); }
    }

    public static readonly DependencyProperty TiltProperty =
        DependencyProperty.Register(nameof(Tilt), typeof(double), typeof(MotionWall), new PropertyMetadata(18d, OnSettingsChanged));

    public double Turn
    {
        get { return (double)GetValue(TurnProperty); }
        set { SetValue(TurnProperty, value); }
    }

    public static readonly DependencyProperty TurnProperty =
        DependencyProperty.Register(nameof(Turn), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public double Roll
    {
        get { return (double)GetValue(RollProperty); }
        set { SetValue(RollProperty, value); }
    }

    public static readonly DependencyProperty RollProperty =
        DependencyProperty.Register(nameof(Roll), typeof(double), typeof(MotionWall), new PropertyMetadata(-12d, OnSettingsChanged));

    public double Perspective
    {
        get { return (double)GetValue(PerspectiveProperty); }
        set { SetValue(PerspectiveProperty, value); }
    }

    public static readonly DependencyProperty PerspectiveProperty =
        DependencyProperty.Register(nameof(Perspective), typeof(double), typeof(MotionWall), new PropertyMetadata(1200d, OnSettingsChanged));

    public double Depth
    {
        get { return (double)GetValue(DepthProperty); }
        set { SetValue(DepthProperty, value); }
    }

    public static readonly DependencyProperty DepthProperty =
        DependencyProperty.Register(nameof(Depth), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public bool IsInteractive
    {
        get { return (bool)GetValue(IsInteractiveProperty); }
        set { SetValue(IsInteractiveProperty, value); }
    }

    public static readonly DependencyProperty IsInteractiveProperty =
        DependencyProperty.Register(nameof(IsInteractive), typeof(bool), typeof(MotionWall), new PropertyMetadata(false, OnInteractiveChanged));

    public double Parallax
    {
        get { return (double)GetValue(ParallaxProperty); }
        set { SetValue(ParallaxProperty, value); }
    }

    public static readonly DependencyProperty ParallaxProperty =
        DependencyProperty.Register(nameof(Parallax), typeof(double), typeof(MotionWall), new PropertyMetadata(0.5, OnSettingsChanged));

    public double PointerTilt
    {
        get { return (double)GetValue(PointerTiltProperty); }
        set { SetValue(PointerTiltProperty, value); }
    }

    public static readonly DependencyProperty PointerTiltProperty =
        DependencyProperty.Register(nameof(PointerTilt), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public double Spotlight
    {
        get { return (double)GetValue(SpotlightProperty); }
        set { SetValue(SpotlightProperty, value); }
    }

    public static readonly DependencyProperty SpotlightProperty =
        DependencyProperty.Register(nameof(Spotlight), typeof(double), typeof(MotionWall), new PropertyMetadata(0.6, OnSettingsChanged));

    public double Dim
    {
        get { return (double)GetValue(DimProperty); }
        set { SetValue(DimProperty, value); }
    }

    public static readonly DependencyProperty DimProperty =
        DependencyProperty.Register(nameof(Dim), typeof(double), typeof(MotionWall), new PropertyMetadata(0.35, OnSettingsChanged));

    public bool Grayscale
    {
        get { return (bool)GetValue(GrayscaleProperty); }
        set { SetValue(GrayscaleProperty, value); }
    }

    public static readonly DependencyProperty GrayscaleProperty =
        DependencyProperty.Register(nameof(Grayscale), typeof(bool), typeof(MotionWall), new PropertyMetadata(false, OnCardChanged));

    public double Lift
    {
        get { return (double)GetValue(LiftProperty); }
        set { SetValue(LiftProperty, value); }
    }

    public static readonly DependencyProperty LiftProperty =
        DependencyProperty.Register(nameof(Lift), typeof(double), typeof(MotionWall), new PropertyMetadata(48d, OnSettingsChanged));

    public Brush? OverlayColor
    {
        get { return (Brush?)GetValue(OverlayColorProperty); }
        set { SetValue(OverlayColorProperty, value); }
    }

    public static readonly DependencyProperty OverlayColorProperty =
        DependencyProperty.Register(nameof(OverlayColor), typeof(Brush), typeof(MotionWall), new PropertyMetadata(null, OnSettingsChanged));

    public double OverlayOpacity
    {
        get { return (double)GetValue(OverlayOpacityProperty); }
        set { SetValue(OverlayOpacityProperty, value); }
    }

    public static readonly DependencyProperty OverlayOpacityProperty =
        DependencyProperty.Register(nameof(OverlayOpacity), typeof(double), typeof(MotionWall), new PropertyMetadata(0d, OnSettingsChanged));

    public Brush? TileBackground
    {
        get { return (Brush?)GetValue(TileBackgroundProperty); }
        set { SetValue(TileBackgroundProperty, value); }
    }

    public static readonly DependencyProperty TileBackgroundProperty =
        DependencyProperty.Register(nameof(TileBackground), typeof(Brush), typeof(MotionWall), new PropertyMetadata(null, OnCardChanged));

    public Brush? TileForeground
    {
        get { return (Brush?)GetValue(TileForegroundProperty); }
        set { SetValue(TileForegroundProperty, value); }
    }

    public static readonly DependencyProperty TileForegroundProperty =
        DependencyProperty.Register(nameof(TileForeground), typeof(Brush), typeof(MotionWall), new PropertyMetadata(null, OnCardChanged));

    public bool ShowLines
    {
        get { return (bool)GetValue(ShowLinesProperty); }
        set { SetValue(ShowLinesProperty, value); }
    }

    public static readonly DependencyProperty ShowLinesProperty =
        DependencyProperty.Register(nameof(ShowLines), typeof(bool), typeof(MotionWall), new PropertyMetadata(false, OnSettingsChanged));

    public Brush? LineBrush
    {
        get { return (Brush?)GetValue(LineBrushProperty); }
        set { SetValue(LineBrushProperty, value); }
    }

    public static readonly DependencyProperty LineBrushProperty =
        DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(MotionWall), new PropertyMetadata(null, OnSettingsChanged));

    public bool FadeEdges
    {
        get { return (bool)GetValue(FadeEdgesProperty); }
        set { SetValue(FadeEdgesProperty, value); }
    }

    public static readonly DependencyProperty FadeEdgesProperty =
        DependencyProperty.Register(nameof(FadeEdges), typeof(bool), typeof(MotionWall), new PropertyMetadata(true, OnSettingsChanged));

    public double EdgeFadeSize
    {
        get { return (double)GetValue(EdgeFadeSizeProperty); }
        set { SetValue(EdgeFadeSizeProperty, value); }
    }

    public static readonly DependencyProperty EdgeFadeSizeProperty =
        DependencyProperty.Register(nameof(EdgeFadeSize), typeof(double), typeof(MotionWall), new PropertyMetadata(0.2d, OnSettingsChanged));

    public double EdgeFadeStrength
    {
        get { return (double)GetValue(EdgeFadeStrengthProperty); }
        set { SetValue(EdgeFadeStrengthProperty, value); }
    }

    public static readonly DependencyProperty EdgeFadeStrengthProperty =
        DependencyProperty.Register(nameof(EdgeFadeStrength), typeof(double), typeof(MotionWall), new PropertyMetadata(0.75, OnSettingsChanged));

    public Color? EdgeFadeColor
    {
        get { return (Color?)GetValue(EdgeFadeColorProperty); }
        set { SetValue(EdgeFadeColorProperty, value); }
    }

    public static readonly DependencyProperty EdgeFadeColorProperty =
        DependencyProperty.Register(nameof(EdgeFadeColor), typeof(Color?), typeof(MotionWall), new PropertyMetadata(null, OnSettingsChanged));

    private static void OnSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MotionWall)d;
        control.RefreshSettings();
        control.RequestRedraw();
    }

    private static void OnCardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MotionWall)d;
        control.RefreshSettings();
        control.InvalidateCards();
    }

    private static void OnFrameRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MotionWall)d).ApplyFrameRate();
    }

    private static void OnInteractiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (MotionWall)d;
        control.ApplyInteractive();
        control.RefreshSettings();
        control.RequestRedraw();
    }

    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((MotionWall)d).AttachItemsSource(e.OldValue, e.NewValue);
    }
}
