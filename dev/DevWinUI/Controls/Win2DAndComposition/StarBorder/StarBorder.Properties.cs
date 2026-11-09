namespace DevWinUI;

public sealed partial class StarBorder
{
    public Color StarColor
    {
        get { return (Color)GetValue(StarColorProperty); }
        set { SetValue(StarColorProperty, value); }
    }

    public static readonly DependencyProperty StarColorProperty =
        DependencyProperty.Register(nameof(StarColor), typeof(Color), typeof(StarBorder), new PropertyMetadata(default(Color), OnPropertyChanged));

    public Color TrailColor
    {
        get { return (Color)GetValue(TrailColorProperty); }
        set { SetValue(TrailColorProperty, value); }
    }

    public static readonly DependencyProperty TrailColorProperty =
        DependencyProperty.Register(nameof(TrailColor), typeof(Color), typeof(StarBorder), new PropertyMetadata(default(Color), OnPropertyChanged));

    public double Duration
    {
        get { return (double)GetValue(DurationProperty); }
        set { SetValue(DurationProperty, value); }
    }

    public static readonly DependencyProperty DurationProperty =
        DependencyProperty.Register(nameof(Duration), typeof(double), typeof(StarBorder), new PropertyMetadata(4.0, OnPropertyChanged));

    public StarDirection Direction
    {
        get { return (StarDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(StarDirection), typeof(StarBorder), new PropertyMetadata(StarDirection.Clockwise, OnPropertyChanged));

    public int Stars
    {
        get { return (int)GetValue(StarsProperty); }
        set { SetValue(StarsProperty, value); }
    }

    public static readonly DependencyProperty StarsProperty =
        DependencyProperty.Register(nameof(Stars), typeof(int), typeof(StarBorder), new PropertyMetadata(1, OnPropertyChanged));

    public double TrailLength
    {
        get { return (double)GetValue(TrailLengthProperty); }
        set { SetValue(TrailLengthProperty, value); }
    }

    public static readonly DependencyProperty TrailLengthProperty =
        DependencyProperty.Register(nameof(TrailLength), typeof(double), typeof(StarBorder), new PropertyMetadata(0.3, OnPropertyChanged));

    public double LineThickness
    {
        get { return (double)GetValue(LineThicknessProperty); }
        set { SetValue(LineThicknessProperty, value); }
    }

    public static readonly DependencyProperty LineThicknessProperty =
        DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(StarBorder), new PropertyMetadata(1.0, OnPropertyChanged));

    public double Radius
    {
        get { return (double)GetValue(RadiusProperty); }
        set { SetValue(RadiusProperty, value); }
    }

    public static readonly DependencyProperty RadiusProperty =
        DependencyProperty.Register(nameof(Radius), typeof(double), typeof(StarBorder), new PropertyMetadata(12.0, OnPropertyChanged));

    public double Glow
    {
        get { return (double)GetValue(GlowProperty); }
        set { SetValue(GlowProperty, value); }
    }

    public static readonly DependencyProperty GlowProperty =
        DependencyProperty.Register(nameof(Glow), typeof(double), typeof(StarBorder), new PropertyMetadata(0.6, OnPropertyChanged));

    public bool Sparkle
    {
        get { return (bool)GetValue(SparkleProperty); }
        set { SetValue(SparkleProperty, value); }
    }

    public static readonly DependencyProperty SparkleProperty =
        DependencyProperty.Register(nameof(Sparkle), typeof(bool), typeof(StarBorder), new PropertyMetadata(false, OnPropertyChanged));

    public StarHoverMode HoverMode
    {
        get { return (StarHoverMode)GetValue(HoverModeProperty); }
        set { SetValue(HoverModeProperty, value); }
    }

    public static readonly DependencyProperty HoverModeProperty =
        DependencyProperty.Register(nameof(HoverMode), typeof(StarHoverMode), typeof(StarBorder), new PropertyMetadata(StarHoverMode.Lap, OnPropertyChanged));

    public bool ClickPulse
    {
        get { return (bool)GetValue(ClickPulseProperty); }
        set { SetValue(ClickPulseProperty, value); }
    }

    public static readonly DependencyProperty ClickPulseProperty =
        DependencyProperty.Register(nameof(ClickPulse), typeof(bool), typeof(StarBorder), new PropertyMetadata(true, OnPropertyChanged));

    public StarBorderTheme Theme
    {
        get { return (StarBorderTheme)GetValue(ThemeProperty); }
        set { SetValue(ThemeProperty, value); }
    }

    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.Register(nameof(Theme), typeof(StarBorderTheme), typeof(StarBorder), new PropertyMetadata(StarBorderTheme.Dark, OnPropertyChanged));

    public Color BackgroundColor
    {
        get { return (Color)GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }

    public static readonly DependencyProperty BackgroundColorProperty =
        DependencyProperty.Register(nameof(BackgroundColor), typeof(Color), typeof(StarBorder), new PropertyMetadata(default(Color), OnPropertyChanged));

    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly DependencyProperty TextColorProperty =
        DependencyProperty.Register(nameof(TextColor), typeof(Color), typeof(StarBorder), new PropertyMetadata(default(Color), OnPropertyChanged));

    public Color BorderColor
    {
        get { return (Color)GetValue(BorderColorProperty); }
        set { SetValue(BorderColorProperty, value); }
    }

    public static readonly DependencyProperty BorderColorProperty =
        DependencyProperty.Register(nameof(BorderColor), typeof(Color), typeof(StarBorder), new PropertyMetadata(default(Color), OnPropertyChanged));

    private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((StarBorder)d).OnPropsChanged();
}
