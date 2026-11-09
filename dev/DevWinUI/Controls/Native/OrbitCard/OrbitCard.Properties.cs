namespace DevWinUI;

public sealed partial class OrbitCard
{
    public OrbitCardItem? Item
    {
        get { return (OrbitCardItem?)GetValue(ItemProperty); }
        set { SetValue(ItemProperty, value); }
    }

    public static readonly DependencyProperty ItemProperty =
        DependencyProperty.Register(nameof(Item), typeof(OrbitCardItem), typeof(OrbitCard), new PropertyMetadata(null, (d, _) => ((OrbitCard)d).Apply()));

    public int PaletteIndex
    {
        get { return (int)GetValue(PaletteIndexProperty); }
        set { SetValue(PaletteIndexProperty, value); }
    }

    public static readonly DependencyProperty PaletteIndexProperty =
        DependencyProperty.Register(nameof(PaletteIndex), typeof(int), typeof(OrbitCard), new PropertyMetadata(0, (d, _) => ((OrbitCard)d).Apply()));

    public string LinkLabel
    {
        get { return (string)GetValue(LinkLabelProperty); }
        set { SetValue(LinkLabelProperty, value); }
    }

    public static readonly DependencyProperty LinkLabelProperty =
        DependencyProperty.Register(nameof(LinkLabel), typeof(string), typeof(OrbitCard), new PropertyMetadata("Open", (d, _) => ((OrbitCard)d).Apply()));

    public bool ShowDescription
    {
        get { return (bool)GetValue(ShowDescriptionProperty); }
        set { SetValue(ShowDescriptionProperty, value); }
    }

    public static readonly DependencyProperty ShowDescriptionProperty =
        DependencyProperty.Register(nameof(ShowDescription), typeof(bool), typeof(OrbitCard), new PropertyMetadata(true, (d, _) => ((OrbitCard)d).Apply()));

    public double NameFontSize
    {
        get { return (double)GetValue(NameFontSizeProperty); }
        set { SetValue(NameFontSizeProperty, value); }
    }

    public static readonly DependencyProperty NameFontSizeProperty =
        DependencyProperty.Register(nameof(NameFontSize), typeof(double), typeof(OrbitCard), new PropertyMetadata(14.0));

    public double BodyFontSize
    {
        get { return (double)GetValue(BodyFontSizeProperty); }
        set { SetValue(BodyFontSizeProperty, value); }
    }

    public static readonly DependencyProperty BodyFontSizeProperty =
        DependencyProperty.Register(nameof(BodyFontSize), typeof(double), typeof(OrbitCard), new PropertyMetadata(11.5));

    public double InitialsFontSize
    {
        get { return (double)GetValue(InitialsFontSizeProperty); }
        set { SetValue(InitialsFontSizeProperty, value); }
    }

    public static readonly DependencyProperty InitialsFontSizeProperty =
        DependencyProperty.Register(nameof(InitialsFontSize), typeof(double), typeof(OrbitCard), new PropertyMetadata(32.0));
}
