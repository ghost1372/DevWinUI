namespace DevWinUI;
public partial class ColorPaletteItem
{
    internal string InternalColorName
    {
        get { return (string)GetValue(InternalColorNameProperty); }
        set { SetValue(InternalColorNameProperty, value); }
    }

    internal static readonly DependencyProperty InternalColorNameProperty =
        DependencyProperty.Register(nameof(InternalColorName), typeof(string), typeof(ColorPaletteItem), new PropertyMetadata(null));


    internal ObservableCollection<ColorPaletteItem> InternalColors
    {
        get { return (ObservableCollection<ColorPaletteItem>)GetValue(InternalColorsProperty); }
        set { SetValue(InternalColorsProperty, value); }
    }

    internal static readonly DependencyProperty InternalColorsProperty =
        DependencyProperty.Register(nameof(InternalColors), typeof(ObservableCollection<ColorPaletteItem>), typeof(ColorPaletteItem), new PropertyMetadata(null));

    public bool ShowHexCode
    {
        get { return (bool)GetValue(ShowHexCodeProperty); }
        set { SetValue(ShowHexCodeProperty, value); }
    }

    public static readonly DependencyProperty ShowHexCodeProperty =
        DependencyProperty.Register(nameof(ShowHexCode), typeof(bool), typeof(ColorPaletteItem), new PropertyMetadata(false, OnShowHexCodeChanged));

    private static void OnShowHexCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPaletteItem)d;
        if (ctl != null)
        {
            ctl.UpdateColorName();
        }
    }

    public bool ShowColorName
    {
        get { return (bool)GetValue(ShowColorNameProperty); }
        set { SetValue(ShowColorNameProperty, value); }
    }

    public static readonly DependencyProperty ShowColorNameProperty =
        DependencyProperty.Register(nameof(ShowColorName), typeof(bool), typeof(ColorPaletteItem), new PropertyMetadata(true, OnShowColorNameChanged));
    private static void OnShowColorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPaletteItem)d;
        if (ctl != null)
        {
            ctl.UpdateColorName();
        }
    }
    public bool ShowToolTip
    {
        get { return (bool)GetValue(ShowToolTipProperty); }
        set { SetValue(ShowToolTipProperty, value); }
    }

    public static readonly DependencyProperty ShowToolTipProperty =
        DependencyProperty.Register(nameof(ShowToolTip), typeof(bool), typeof(ColorPaletteItem), new PropertyMetadata(true, OnShowToolTipChanged));

    private static void OnShowToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPaletteItem)d;
        if (ctl != null)
        {
            ctl.UpdateToolTip();
        }
    }

    public ColorItemShape ItemShape
    {
        get { return (ColorItemShape)GetValue(ItemShapeProperty); }
        set { SetValue(ItemShapeProperty, value); }
    }

    public static readonly DependencyProperty ItemShapeProperty =
        DependencyProperty.Register(nameof(ItemShape), typeof(ColorItemShape), typeof(ColorPaletteItem), new PropertyMetadata(ColorItemShape.Rectangle, OnItemShapeChanged));

    private static void OnItemShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPaletteItem)d;
        if (ctl != null)
        {
            ctl.UpdateTemplate();
        }
    }

    public string ColorName
    {
        get { return (string)GetValue(ColorNameProperty); }
        set { SetValue(ColorNameProperty, value); }
    }

    public static readonly DependencyProperty ColorNameProperty =
        DependencyProperty.Register(nameof(ColorName), typeof(string), typeof(ColorPaletteItem), new PropertyMetadata(null));

    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(ColorPaletteItem), new PropertyMetadata(ColorHelper.GetColorFromHex("#FFFFFF"), OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPaletteItem)d;
        if (ctl != null)
        {
            ctl.ColorBrush = new SolidColorBrush((Color)e.NewValue);
            ctl.UpdateColorName();
        }
    }

    internal SolidColorBrush ColorBrush
    {
        get { return (SolidColorBrush)GetValue(ColorBrushProperty); }
        set { SetValue(ColorBrushProperty, value); }
    }

    internal static readonly DependencyProperty ColorBrushProperty =
        DependencyProperty.Register(nameof(ColorBrush), typeof(SolidColorBrush), typeof(ColorPaletteItem), new PropertyMetadata(null));
}
