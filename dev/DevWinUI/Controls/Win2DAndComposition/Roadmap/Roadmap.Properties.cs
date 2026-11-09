using Microsoft.UI.Text;
using Windows.UI.Text;

namespace DevWinUI;

public partial class Roadmap
{
    #region Mode and placement

    public RoadmapMode Mode
    {
        get { return (RoadmapMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(RoadmapMode), typeof(Roadmap), new PropertyMetadata(RoadmapMode.Simple, OnModeChanged));

    public RoadmapPlacement FirstItemPlacement
    {
        get { return (RoadmapPlacement)GetValue(FirstItemPlacementProperty); }
        set { SetValue(FirstItemPlacementProperty, value); }
    }

    public static readonly DependencyProperty FirstItemPlacementProperty =
        DependencyProperty.Register(nameof(FirstItemPlacement), typeof(RoadmapPlacement), typeof(Roadmap), new PropertyMetadata(RoadmapPlacement.Auto, OnLayoutPropertyChanged));

    public bool AlternatePlacement
    {
        get { return (bool)GetValue(AlternatePlacementProperty); }
        set { SetValue(AlternatePlacementProperty, value); }
    }

    public static readonly DependencyProperty AlternatePlacementProperty =
        DependencyProperty.Register(nameof(AlternatePlacement), typeof(bool), typeof(Roadmap), new PropertyMetadata(true, OnLayoutPropertyChanged));

    public RoadmapDistribution Distribution
    {
        get { return (RoadmapDistribution)GetValue(DistributionProperty); }
        set { SetValue(DistributionProperty, value); }
    }

    public static readonly DependencyProperty DistributionProperty =
        DependencyProperty.Register(nameof(Distribution), typeof(RoadmapDistribution), typeof(Roadmap), new PropertyMetadata(RoadmapDistribution.Even, OnLayoutPropertyChanged));

    #endregion

    #region Sizing (NaN = use the default of the current mode)

    public double ItemWidth
    {
        get { return (double)GetValue(ItemWidthProperty); }
        set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public double MarkerSize
    {
        get { return (double)GetValue(MarkerSizeProperty); }
        set { SetValue(MarkerSizeProperty, value); }
    }

    public static readonly DependencyProperty MarkerSizeProperty =
        DependencyProperty.Register(nameof(MarkerSize), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public double AxisThickness
    {
        get { return (double)GetValue(AxisThicknessProperty); }
        set { SetValue(AxisThicknessProperty, value); }
    }

    public static readonly DependencyProperty AxisThicknessProperty =
        DependencyProperty.Register(nameof(AxisThickness), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnVisualPropertyChanged));

    public double StemLength
    {
        get { return (double)GetValue(StemLengthProperty); }
        set { SetValue(StemLengthProperty, value); }
    }

    public static readonly DependencyProperty StemLengthProperty =
        DependencyProperty.Register(nameof(StemLength), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public RoadmapAxisCap StartCap
    {
        get { return (RoadmapAxisCap)GetValue(StartCapProperty); }
        set { SetValue(StartCapProperty, value); }
    }

    public static readonly DependencyProperty StartCapProperty =
        DependencyProperty.Register(nameof(StartCap), typeof(RoadmapAxisCap), typeof(Roadmap), new PropertyMetadata(RoadmapAxisCap.Auto, OnVisualPropertyChanged));

    public RoadmapAxisCap EndCap
    {
        get { return (RoadmapAxisCap)GetValue(EndCapProperty); }
        set { SetValue(EndCapProperty, value); }
    }

    public static readonly DependencyProperty EndCapProperty =
        DependencyProperty.Register(nameof(EndCap), typeof(RoadmapAxisCap), typeof(Roadmap), new PropertyMetadata(RoadmapAxisCap.Auto, OnVisualPropertyChanged));

    #endregion

    #region Brushes and fonts (defaults come from ThemeResource setters in the style)

    public Brush AxisBrush
    {
        get { return (Brush)GetValue(AxisBrushProperty); }
        set { SetValue(AxisBrushProperty, value); }
    }

    public static readonly DependencyProperty AxisBrushProperty =
        DependencyProperty.Register(nameof(AxisBrush), typeof(Brush), typeof(Roadmap), new PropertyMetadata(null, OnVisualPropertyChanged));

    public Brush StemBrush
    {
        get { return (Brush)GetValue(StemBrushProperty); }
        set { SetValue(StemBrushProperty, value); }
    }

    public static readonly DependencyProperty StemBrushProperty =
        DependencyProperty.Register(nameof(StemBrush), typeof(Brush), typeof(Roadmap), new PropertyMetadata(null, OnVisualPropertyChanged));

    public Brush MarkerFill
    {
        get { return (Brush)GetValue(MarkerFillProperty); }
        set { SetValue(MarkerFillProperty, value); }
    }

    public static readonly DependencyProperty MarkerFillProperty =
        DependencyProperty.Register(nameof(MarkerFill), typeof(Brush), typeof(Roadmap), new PropertyMetadata(null, OnVisualPropertyChanged));

    public double LabelFontSize
    {
        get { return (double)GetValue(LabelFontSizeProperty); }
        set { SetValue(LabelFontSizeProperty, value); }
    }

    public static readonly DependencyProperty LabelFontSizeProperty =
        DependencyProperty.Register(nameof(LabelFontSize), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public FontWeight LabelFontWeight
    {
        get { return (FontWeight)GetValue(LabelFontWeightProperty); }
        set { SetValue(LabelFontWeightProperty, value); }
    }

    public static readonly DependencyProperty LabelFontWeightProperty =
        DependencyProperty.Register(nameof(LabelFontWeight), typeof(FontWeight), typeof(Roadmap), new PropertyMetadata(FontWeights.Bold, OnVisualPropertyChanged));

    public FontFamily IconFontFamily
    {
        get { return (FontFamily)GetValue(IconFontFamilyProperty); }
        set { SetValue(IconFontFamilyProperty, value); }
    }

    public static readonly DependencyProperty IconFontFamilyProperty =
        DependencyProperty.Register(nameof(IconFontFamily), typeof(FontFamily), typeof(Roadmap), new PropertyMetadata(null, OnVisualPropertyChanged));

    #endregion

    #region Effects

    public double GlowRadius
    {
        get { return (double)GetValue(GlowRadiusProperty); }
        set { SetValue(GlowRadiusProperty, value); }
    }

    public static readonly DependencyProperty GlowRadiusProperty =
        DependencyProperty.Register(nameof(GlowRadius), typeof(double), typeof(Roadmap), new PropertyMetadata(double.NaN, OnVisualPropertyChanged));

    public double BootDuration
    {
        get { return (double)GetValue(BootDurationProperty); }
        set { SetValue(BootDurationProperty, value); }
    }

    public static readonly DependencyProperty BootDurationProperty =
        DependencyProperty.Register(nameof(BootDuration), typeof(double), typeof(Roadmap), new PropertyMetadata(1.6d));

    public bool AnimateActive
    {
        get { return (bool)GetValue(AnimateActiveProperty); }
        set { SetValue(AnimateActiveProperty, value); }
    }

    public static readonly DependencyProperty AnimateActiveProperty =
        DependencyProperty.Register(nameof(AnimateActive), typeof(bool), typeof(Roadmap), new PropertyMetadata(true, OnEffectChanged));

    public RoadmapToggle Scanlines
    {
        get { return (RoadmapToggle)GetValue(ScanlinesProperty); }
        set { SetValue(ScanlinesProperty, value); }
    }

    public static readonly DependencyProperty ScanlinesProperty =
        DependencyProperty.Register(nameof(Scanlines), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    public RoadmapToggle GridBackdrop
    {
        get { return (RoadmapToggle)GetValue(GridBackdropProperty); }
        set { SetValue(GridBackdropProperty, value); }
    }

    public static readonly DependencyProperty GridBackdropProperty =
        DependencyProperty.Register(nameof(GridBackdrop), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    public RoadmapToggle ScanBeam
    {
        get { return (RoadmapToggle)GetValue(ScanBeamProperty); }
        set { SetValue(ScanBeamProperty, value); }
    }

    public static readonly DependencyProperty ScanBeamProperty =
        DependencyProperty.Register(nameof(ScanBeam), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    public RoadmapToggle AxisFlow
    {
        get { return (RoadmapToggle)GetValue(AxisFlowProperty); }
        set { SetValue(AxisFlowProperty, value); }
    }

    public static readonly DependencyProperty AxisFlowProperty =
        DependencyProperty.Register(nameof(AxisFlow), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    public RoadmapToggle Flicker
    {
        get { return (RoadmapToggle)GetValue(FlickerProperty); }
        set { SetValue(FlickerProperty, value); }
    }

    public static readonly DependencyProperty FlickerProperty =
        DependencyProperty.Register(nameof(Flicker), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    public RoadmapToggle BootAnimation
    {
        get { return (RoadmapToggle)GetValue(BootAnimationProperty); }
        set { SetValue(BootAnimationProperty, value); }
    }

    public static readonly DependencyProperty BootAnimationProperty =
        DependencyProperty.Register(nameof(BootAnimation), typeof(RoadmapToggle), typeof(Roadmap), new PropertyMetadata(RoadmapToggle.Auto, OnEffectChanged));

    private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Roadmap roadmap)
        {
            roadmap.UpdateAnimation();
            roadmap.InvalidateCanvas();
        }
    }

    #endregion

    #region Lanes

    public ObservableCollection<RoadmapLane> Lanes
    {
        get { return (ObservableCollection<RoadmapLane>)GetValue(LanesProperty); }
        set { SetValue(LanesProperty, value); }
    }

    public static readonly DependencyProperty LanesProperty =
        DependencyProperty.Register(nameof(Lanes), typeof(ObservableCollection<RoadmapLane>), typeof(Roadmap), new PropertyMetadata(null, OnLanesChanged));

    public double LaneHeaderWidth
    {
        get { return (double)GetValue(LaneHeaderWidthProperty); }
        set { SetValue(LaneHeaderWidthProperty, value); }
    }

    public static readonly DependencyProperty LaneHeaderWidthProperty =
        DependencyProperty.Register(nameof(LaneHeaderWidth), typeof(double), typeof(Roadmap), new PropertyMetadata(120d));

    public Thickness LanesMargin
    {
        get { return (Thickness)GetValue(LanesMarginProperty); }
        set { SetValue(LanesMarginProperty, value); }
    }

    public static readonly DependencyProperty LanesMarginProperty =
        DependencyProperty.Register(nameof(LanesMargin), typeof(Thickness), typeof(Roadmap), new PropertyMetadata(new Thickness(0, 24, 0, 0)));

    #endregion

    #region Callbacks

    private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Roadmap roadmap)
        {
            roadmap.ApplyModeToContainers();
            roadmap.InvalidateLayoutAndCanvas();
            roadmap.UpdateAnimation();
        }
    }

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Roadmap roadmap)
        {
            roadmap.InvalidateLayoutAndCanvas();
        }
    }

    private static void OnVisualPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is Roadmap roadmap)
        {
            roadmap.InvalidateCanvas();
        }
    }

    private static void OnLanesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Roadmap roadmap)
        {
            return;
        }

        if (e.OldValue is ObservableCollection<RoadmapLane> oldLanes)
        {
            oldLanes.CollectionChanged -= roadmap.OnLanesCollectionChanged;
        }

        if (e.NewValue is ObservableCollection<RoadmapLane> newLanes)
        {
            newLanes.CollectionChanged += roadmap.OnLanesCollectionChanged;
            foreach (RoadmapLane lane in newLanes)
            {
                roadmap.BindLane(lane);
            }
        }

        roadmap.UpdateLanesVisibility();
    }

    #endregion
}
