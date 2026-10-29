namespace DevWinUI;

public sealed partial class GlowingBorder
{
    public GlowingBorderKind Kind
    {
        get { return (GlowingBorderKind)GetValue(KindProperty); }
        set { SetValue(KindProperty, value); }
    }

    public static readonly DependencyProperty KindProperty =
        DependencyProperty.Register(nameof(Kind), typeof(GlowingBorderKind), typeof(GlowingBorder), new PropertyMetadata(GlowingBorderKind.Cursor, OnPaletteChanged));

    public Brush GlowBrush1
    {
        get { return (Brush)GetValue(GlowBrush1Property); }
        set { SetValue(GlowBrush1Property, value); }
    }

    public static readonly DependencyProperty GlowBrush1Property =
        DependencyProperty.Register(nameof(GlowBrush1), typeof(Brush), typeof(GlowingBorder), new PropertyMetadata(null, OnPaletteChanged));

    public Brush GlowBrush2
    {
        get { return (Brush)GetValue(GlowBrush2Property); }
        set { SetValue(GlowBrush2Property, value); }
    }

    public static readonly DependencyProperty GlowBrush2Property =
        DependencyProperty.Register(nameof(GlowBrush2), typeof(Brush), typeof(GlowingBorder), new PropertyMetadata(null, OnPaletteChanged));

    public Brush GlowBrush3
    {
        get { return (Brush)GetValue(GlowBrush3Property); }
        set { SetValue(GlowBrush3Property, value); }
    }

    public static readonly DependencyProperty GlowBrush3Property =
        DependencyProperty.Register(nameof(GlowBrush3), typeof(Brush), typeof(GlowingBorder), new PropertyMetadata(null, OnPaletteChanged));

    public Brush GlowBrush4
    {
        get { return (Brush)GetValue(GlowBrush4Property); }
        set { SetValue(GlowBrush4Property, value); }
    }

    public static readonly DependencyProperty GlowBrush4Property =
        DependencyProperty.Register(nameof(GlowBrush4), typeof(Brush), typeof(GlowingBorder), new PropertyMetadata(null, OnPaletteChanged));

    public double Range
    {
        get { return (double)GetValue(RangeProperty); }
        set { SetValue(RangeProperty, value); }
    }

    public static readonly DependencyProperty RangeProperty =
        DependencyProperty.Register(nameof(Range), typeof(double), typeof(GlowingBorder), new PropertyMetadata(160.0, OnRedrawChanged));

    public double Spot
    {
        get { return (double)GetValue(SpotProperty); }
        set { SetValue(SpotProperty, value); }
    }

    public static readonly DependencyProperty SpotProperty =
        DependencyProperty.Register(nameof(Spot), typeof(double), typeof(GlowingBorder), new PropertyMetadata(340.0, OnRedrawChanged));

    public double RingThickness
    {
        get { return (double)GetValue(RingThicknessProperty); }
        set { SetValue(RingThicknessProperty, value); }
    }

    public static readonly DependencyProperty RingThicknessProperty =
        DependencyProperty.Register(nameof(RingThickness), typeof(double), typeof(GlowingBorder), new PropertyMetadata(1.5, OnLayoutChanged));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(GlowingBorder), new PropertyMetadata(16.0, OnLayoutChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(GlowingBorder), new PropertyMetadata(4.0, OnRedrawChanged));

    public double Glow
    {
        get { return (double)GetValue(GlowProperty); }
        set { SetValue(GlowProperty, value); }
    }

    public static readonly DependencyProperty GlowProperty =
        DependencyProperty.Register(nameof(Glow), typeof(double), typeof(GlowingBorder), new PropertyMetadata(0.0, OnRedrawChanged));

    public double GlowSize
    {
        get { return (double)GetValue(GlowSizeProperty); }
        set { SetValue(GlowSizeProperty, value); }
    }

    public static readonly DependencyProperty GlowSizeProperty =
        DependencyProperty.Register(nameof(GlowSize), typeof(double), typeof(GlowingBorder), new PropertyMetadata(10.0, OnLayoutChanged));

    public double InnerGlow
    {
        get { return (double)GetValue(InnerGlowProperty); }
        set { SetValue(InnerGlowProperty, value); }
    }

    public static readonly DependencyProperty InnerGlowProperty =
        DependencyProperty.Register(nameof(InnerGlow), typeof(double), typeof(GlowingBorder), new PropertyMetadata(0.0, OnRedrawChanged));

    private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GlowingBorder)d).RefreshPalette();

    private static void OnRedrawChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GlowingBorder)d).RequestRedraw();

    private static void OnLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((GlowingBorder)d).ApplyLayout();
}
