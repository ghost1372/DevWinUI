namespace DevWinUI;

public sealed partial class Timeline2
{
    public Timeline2Size Size
    {
        get { return (Timeline2Size)GetValue(SizeProperty); }
        set { SetValue(SizeProperty, value); }
    }

    public static readonly DependencyProperty SizeProperty =
        DependencyProperty.Register(nameof(Size), typeof(Timeline2Size), typeof(Timeline2), new PropertyMetadata(Timeline2Size.MD, OnLayoutPropertyChanged));

    public Timeline2Color Color
    {
        get { return (Timeline2Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Timeline2Color), typeof(Timeline2), new PropertyMetadata(Timeline2Color.Primary, OnLayoutPropertyChanged));

    public Timeline2Kind Kind
    {
        get { return (Timeline2Kind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(Timeline2Kind), typeof(Timeline2), new PropertyMetadata(Timeline2Kind.Solid, OnLayoutPropertyChanged));

    public Timeline2MarkerShape MarkerShape
    {
        get { return (Timeline2MarkerShape)GetValue(MarkerShapeProperty); }
        set { SetValue(MarkerShapeProperty, value); }
    }

    public static readonly DependencyProperty MarkerShapeProperty =
        DependencyProperty.Register(nameof(MarkerShape), typeof(Timeline2MarkerShape), typeof(Timeline2), new PropertyMetadata(Timeline2MarkerShape.Circle, OnLayoutPropertyChanged));

    public double MarkerSize
    {
        get { return (double)GetValue(MarkerSizeProperty); }
        set { SetValue(MarkerSizeProperty, value); }
    }

    public static readonly DependencyProperty MarkerSizeProperty =
        DependencyProperty.Register(nameof(MarkerSize), typeof(double), typeof(Timeline2), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }

    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Timeline2), new PropertyMetadata(Orientation.Vertical, OnLayoutPropertyChanged));

    public Timeline2Align Align
    {
        get { return (Timeline2Align)GetValue(AlignProperty); }
        set { SetValue(AlignProperty, value); }
    }

    public static readonly DependencyProperty AlignProperty =
        DependencyProperty.Register(nameof(Align), typeof(Timeline2Align), typeof(Timeline2), new PropertyMetadata(Timeline2Align.Start, OnLayoutPropertyChanged));

    public Timeline2DatePlacement DatePlacement
    {
        get { return (Timeline2DatePlacement)GetValue(DatePlacementProperty); }
        set { SetValue(DatePlacementProperty, value); }
    }

    public static readonly DependencyProperty DatePlacementProperty =
        DependencyProperty.Register(nameof(DatePlacement), typeof(Timeline2DatePlacement), typeof(Timeline2), new PropertyMetadata(Timeline2DatePlacement.Content, OnLayoutPropertyChanged));

    public bool Reverse
    {
        get { return (bool)GetValue(ReverseProperty); }
        set { SetValue(ReverseProperty, value); }
    }

    public static readonly DependencyProperty ReverseProperty =
        DependencyProperty.Register(nameof(Reverse), typeof(bool), typeof(Timeline2), new PropertyMetadata(false, OnLayoutPropertyChanged));

    public double BarSpacing
    {
        get { return (double)GetValue(BarSpacingProperty); }
        set { SetValue(BarSpacingProperty, value); }
    }

    public static readonly DependencyProperty BarSpacingProperty =
        DependencyProperty.Register(nameof(BarSpacing), typeof(double), typeof(Timeline2), new PropertyMetadata(12d, OnLayoutPropertyChanged));

    public double ItemSpacing
    {
        get { return (double)GetValue(ItemSpacingProperty); }
        set { SetValue(ItemSpacingProperty, value); }
    }

    public static readonly DependencyProperty ItemSpacingProperty =
        DependencyProperty.Register(nameof(ItemSpacing), typeof(double), typeof(Timeline2), new PropertyMetadata(double.NaN, OnLayoutPropertyChanged));

    public bool IsItemClickEnabled
    {
        get { return (bool)GetValue(IsItemClickEnabledProperty); }
        set { SetValue(IsItemClickEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsItemClickEnabledProperty =
        DependencyProperty.Register(nameof(IsItemClickEnabled), typeof(bool), typeof(Timeline2), new PropertyMetadata(false, OnLayoutPropertyChanged));

    public bool ActivateOnClick
    {
        get { return (bool)GetValue(ActivateOnClickProperty); }
        set { SetValue(ActivateOnClickProperty, value); }
    }

    public static readonly DependencyProperty ActivateOnClickProperty =
        DependencyProperty.Register(nameof(ActivateOnClick), typeof(bool), typeof(Timeline2), new PropertyMetadata(false));

    public int ActiveIndex
    {
        get { return (int)GetValue(ActiveIndexProperty); }
        set { SetValue(ActiveIndexProperty, value); }
    }

    public static readonly DependencyProperty ActiveIndexProperty =
        DependencyProperty.Register(nameof(ActiveIndex), typeof(int), typeof(Timeline2), new PropertyMetadata(-1, OnActiveIndexPropertyChanged));

    public object? ActiveValue
    {
        get { return GetValue(ActiveValueProperty); }
        set { SetValue(ActiveValueProperty, value); }
    }

    public static readonly DependencyProperty ActiveValueProperty =
        DependencyProperty.Register(nameof(ActiveValue), typeof(object), typeof(Timeline2), new PropertyMetadata(null, OnActiveValuePropertyChanged));

    public DataTemplate? MarkerTemplate
    {
        get { return (DataTemplate?)GetValue(MarkerTemplateProperty); }
        set { SetValue(MarkerTemplateProperty, value); }
    }

    public static readonly DependencyProperty MarkerTemplateProperty =
        DependencyProperty.Register(nameof(MarkerTemplate), typeof(DataTemplate), typeof(Timeline2), new PropertyMetadata(null, OnLayoutPropertyChanged));

    public DataTemplate? OppositeTemplate
    {
        get { return (DataTemplate?)GetValue(OppositeTemplateProperty); }
        set { SetValue(OppositeTemplateProperty, value); }
    }

    public static readonly DependencyProperty OppositeTemplateProperty =
        DependencyProperty.Register(nameof(OppositeTemplate), typeof(DataTemplate), typeof(Timeline2), new PropertyMetadata(null, OnLayoutPropertyChanged));

    public DataTemplate? DateTemplate
    {
        get { return (DataTemplate?)GetValue(DateTemplateProperty); }
        set { SetValue(DateTemplateProperty, value); }
    }

    public static readonly DependencyProperty DateTemplateProperty =
        DependencyProperty.Register(nameof(DateTemplate), typeof(DataTemplate), typeof(Timeline2), new PropertyMetadata(null, OnLayoutPropertyChanged));

    public DataTemplate? TitleTemplate
    {
        get { return (DataTemplate?)GetValue(TitleTemplateProperty); }
        set { SetValue(TitleTemplateProperty, value); }
    }

    public static readonly DependencyProperty TitleTemplateProperty =
        DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(Timeline2), new PropertyMetadata(null, OnLayoutPropertyChanged));

    public DataTemplate? DescriptionTemplate
    {
        get { return (DataTemplate?)GetValue(DescriptionTemplateProperty); }
        set { SetValue(DescriptionTemplateProperty, value); }
    }

    public static readonly DependencyProperty DescriptionTemplateProperty =
        DependencyProperty.Register(nameof(DescriptionTemplate), typeof(DataTemplate), typeof(Timeline2), new PropertyMetadata(null, OnLayoutPropertyChanged));

    public Brush? PrimaryColorBrush
    {
        get { return (Brush?)GetValue(PrimaryColorBrushProperty); }
        set { SetValue(PrimaryColorBrushProperty, value); }
    }

    public static readonly DependencyProperty PrimaryColorBrushProperty =
        DependencyProperty.Register(nameof(PrimaryColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? SecondaryColorBrush
    {
        get { return (Brush?)GetValue(SecondaryColorBrushProperty); }
        set { SetValue(SecondaryColorBrushProperty, value); }
    }

    public static readonly DependencyProperty SecondaryColorBrushProperty =
        DependencyProperty.Register(nameof(SecondaryColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? SuccessColorBrush
    {
        get { return (Brush?)GetValue(SuccessColorBrushProperty); }
        set { SetValue(SuccessColorBrushProperty, value); }
    }

    public static readonly DependencyProperty SuccessColorBrushProperty =
        DependencyProperty.Register(nameof(SuccessColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? InfoColorBrush
    {
        get { return (Brush?)GetValue(InfoColorBrushProperty); }
        set { SetValue(InfoColorBrushProperty, value); }
    }

    public static readonly DependencyProperty InfoColorBrushProperty =
        DependencyProperty.Register(nameof(InfoColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? WarningColorBrush
    {
        get { return (Brush?)GetValue(WarningColorBrushProperty); }
        set { SetValue(WarningColorBrushProperty, value); }
    }

    public static readonly DependencyProperty WarningColorBrushProperty =
        DependencyProperty.Register(nameof(WarningColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? ErrorColorBrush
    {
        get { return (Brush?)GetValue(ErrorColorBrushProperty); }
        set { SetValue(ErrorColorBrushProperty, value); }
    }

    public static readonly DependencyProperty ErrorColorBrushProperty =
        DependencyProperty.Register(nameof(ErrorColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? NeutralColorBrush
    {
        get { return (Brush?)GetValue(NeutralColorBrushProperty); }
        set { SetValue(NeutralColorBrushProperty, value); }
    }

    public static readonly DependencyProperty NeutralColorBrushProperty =
        DependencyProperty.Register(nameof(NeutralColorBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? OnColorForeground
    {
        get { return (Brush?)GetValue(OnColorForegroundProperty); }
        set { SetValue(OnColorForegroundProperty, value); }
    }

    public static readonly DependencyProperty OnColorForegroundProperty =
        DependencyProperty.Register(nameof(OnColorForeground), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? OnNeutralForeground
    {
        get { return (Brush?)GetValue(OnNeutralForegroundProperty); }
        set { SetValue(OnNeutralForegroundProperty, value); }
    }

    public static readonly DependencyProperty OnNeutralForegroundProperty =
        DependencyProperty.Register(nameof(OnNeutralForeground), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? ElevatedBrush
    {
        get { return (Brush?)GetValue(ElevatedBrushProperty); }
        set { SetValue(ElevatedBrushProperty, value); }
    }

    public static readonly DependencyProperty ElevatedBrushProperty =
        DependencyProperty.Register(nameof(ElevatedBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? SurfaceBrush
    {
        get { return (Brush?)GetValue(SurfaceBrushProperty); }
        set { SetValue(SurfaceBrushProperty, value); }
    }

    public static readonly DependencyProperty SurfaceBrushProperty =
        DependencyProperty.Register(nameof(SurfaceBrush), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? HighlightedForeground
    {
        get { return (Brush?)GetValue(HighlightedForegroundProperty); }
        set { SetValue(HighlightedForegroundProperty, value); }
    }

    public static readonly DependencyProperty HighlightedForegroundProperty =
        DependencyProperty.Register(nameof(HighlightedForeground), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? MutedForeground
    {
        get { return (Brush?)GetValue(MutedForegroundProperty); }
        set { SetValue(MutedForegroundProperty, value); }
    }

    public static readonly DependencyProperty MutedForegroundProperty =
        DependencyProperty.Register(nameof(MutedForeground), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    public Brush? DimmedForeground
    {
        get { return (Brush?)GetValue(DimmedForegroundProperty); }
        set { SetValue(DimmedForegroundProperty, value); }
    }

    public static readonly DependencyProperty DimmedForegroundProperty =
        DependencyProperty.Register(nameof(DimmedForeground), typeof(Brush), typeof(Timeline2), new PropertyMetadata(null, OnBrushPropertyChanged));

    private static void OnLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline2)d).Refresh();
    }

    private static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline2)d).QueueRefresh();
    }

    private static void OnActiveIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline2)d).OnActiveIndexChanged();
    }

    private static void OnActiveValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((Timeline2)d).OnActiveValueChanged();
    }
}
