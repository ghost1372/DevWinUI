namespace DevWinUI;
public partial class ColorPalette
{
    public Color SelectedColor
    {
        get { return (Color)GetValue(SelectedColorProperty); }
        set { SetValue(SelectedColorProperty, value); }
    }

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(nameof(SelectedColor), typeof(Color), typeof(ColorPalette), new PropertyMetadata(ColorHelper.GetColorFromHex("#FFFFFF"), OnSelectedColorChanged));

    private static void OnSelectedColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateSelectedColor();
        }
    }

    public double ItemWidth
    {
        get { return (double)GetValue(ItemWidthProperty); }
        set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(ColorPalette), new PropertyMetadata(32.0, OnItemSizeChanged));


    public double ItemHeight
    {
        get { return (double)GetValue(ItemHeightProperty); }
        set { SetValue(ItemHeightProperty, value); }
    }

    public static readonly DependencyProperty ItemHeightProperty =
        DependencyProperty.Register(nameof(ItemHeight), typeof(double), typeof(ColorPalette), new PropertyMetadata(32.0, OnItemSizeChanged));


    private static void OnItemSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateItemTemplateAndSize();
        }
    }

    public ObservableCollection<ColorPaletteItem> Colors
    {
        get { return (ObservableCollection<ColorPaletteItem>)GetValue(ColorsProperty); }
        set { SetValue(ColorsProperty, value); }
    }

    public static readonly DependencyProperty ColorsProperty =
        DependencyProperty.Register(nameof(Colors), typeof(ObservableCollection<ColorPaletteItem>), typeof(ColorPalette), new PropertyMetadata(new ObservableCollection<ColorPaletteItem>(), OnColorsChanged));

    private static void OnColorsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateItemsSource();
        }
    }

    public bool IsCopyColorCodeOnClickEnabled
    {
        get { return (bool)GetValue(IsCopyColorCodeOnClickEnabledProperty); }
        set { SetValue(IsCopyColorCodeOnClickEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCopyColorCodeOnClickEnabledProperty =
        DependencyProperty.Register(nameof(IsCopyColorCodeOnClickEnabled), typeof(bool), typeof(ColorPalette), new PropertyMetadata(false));

    public bool ShowHexCode
    {
        get { return (bool)GetValue(ShowHexCodeProperty); }
        set { SetValue(ShowHexCodeProperty, value); }
    }

    public static readonly DependencyProperty ShowHexCodeProperty =
        DependencyProperty.Register(nameof(ShowHexCode), typeof(bool), typeof(ColorPalette), new PropertyMetadata(false, OnShowHexCodeChanged));

    private static void OnShowHexCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateShowHexCode();
        }
    }

    public bool ShowColorName
    {
        get { return (bool)GetValue(ShowColorNameProperty); }
        set { SetValue(ShowColorNameProperty, value); }
    }

    public static readonly DependencyProperty ShowColorNameProperty =
        DependencyProperty.Register(nameof(ShowColorName), typeof(bool), typeof(ColorPalette), new PropertyMetadata(true, OnShowColorNameChanged));

    private static void OnShowColorNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateShowColorName();
        }
    }

    public bool ShowToolTip
    {
        get { return (bool)GetValue(ShowToolTipProperty); }
        set { SetValue(ShowToolTipProperty, value); }
    }

    public static readonly DependencyProperty ShowToolTipProperty =
        DependencyProperty.Register(nameof(ShowToolTip), typeof(bool), typeof(ColorPalette), new PropertyMetadata(true, OnShowToolTipChanged));

    private static void OnShowToolTipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateToolTip();
        }
    }

    public ColorSetType ColorSet
    {
        get { return (ColorSetType)GetValue(ColorSetProperty); }
        set { SetValue(ColorSetProperty, value); }
    }

    public static readonly DependencyProperty ColorSetProperty =
        DependencyProperty.Register(nameof(ColorSet), typeof(ColorSetType), typeof(ColorPalette), new PropertyMetadata(ColorSetType.Basic, OnColorSetChanged));

    private static void OnColorSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateItemsSource();
        }
    }
    public ColorItemShape ItemShape
    {
        get { return (ColorItemShape)GetValue(ItemShapeProperty); }
        set { SetValue(ItemShapeProperty, value); }
    }

    public static readonly DependencyProperty ItemShapeProperty =
        DependencyProperty.Register(nameof(ItemShape), typeof(ColorItemShape), typeof(ColorPalette), new PropertyMetadata(ColorItemShape.Rectangle, OnItemShapeChanged));

    private static void OnItemShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.UpdateItemTemplateAndSize();
        }
    }
}

