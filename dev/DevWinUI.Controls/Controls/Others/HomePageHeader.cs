namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HeroImage), Type = typeof(Image))]
[TemplatePart(Name = nameof(PART_HeroTile), Type = typeof(TileControl))]
public partial class HomePageHeader : Control
{
    private const string PART_HeroImage = "PART_HeroImage";
    private const string PART_HeroTile = "PART_HeroTile";

    private Image heroImage;
    private TileControl heroTile;

    public double HeaderImageHeight
    {
        get => (double)GetValue(HeaderImageHeightProperty);
        set => SetValue(HeaderImageHeightProperty, value);
    }

    public static readonly DependencyProperty HeaderImageHeightProperty =
        DependencyProperty.Register(nameof(HeaderImageHeight), typeof(double), typeof(HomePageHeader), new PropertyMetadata(200.0));


    public object HeaderContent
    {
        get => (object)GetValue(HeaderContentProperty);
        set => SetValue(HeaderContentProperty, value);
    }

    public static readonly DependencyProperty HeaderContentProperty =
        DependencyProperty.Register(nameof(HeaderContent), typeof(object), typeof(HomePageHeader), new PropertyMetadata(null));

    public double HeaderSubtitleFontSize
    {
        get => (double)GetValue(HeaderSubtitleFontSizeProperty);
        set => SetValue(HeaderSubtitleFontSizeProperty, value);
    }

    public static readonly DependencyProperty HeaderSubtitleFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderSubtitleFontSize), typeof(double), typeof(HomePageHeader), new PropertyMetadata(18.0));

    public string HeaderSubtitleText
    {
        get => (string)GetValue(HeaderSubtitleTextProperty);
        set => SetValue(HeaderSubtitleTextProperty, value);
    }

    public static readonly DependencyProperty HeaderSubtitleTextProperty =
        DependencyProperty.Register(nameof(HeaderSubtitleText), typeof(string), typeof(HomePageHeader), new PropertyMetadata(default(string)));

    public double HeaderFontSize
    {
        get => (double)GetValue(HeaderFontSizeProperty);
        set => SetValue(HeaderFontSizeProperty, value);
    }

    public static readonly DependencyProperty HeaderFontSizeProperty =
        DependencyProperty.Register(nameof(HeaderFontSize), typeof(double), typeof(HomePageHeader), new PropertyMetadata(28.0));

    public string HeaderText
    {
        get => (string)GetValue(HeaderTextProperty);
        set => SetValue(HeaderTextProperty, value);
    }

    public static readonly DependencyProperty HeaderTextProperty =
        DependencyProperty.Register(nameof(HeaderText), typeof(string), typeof(HomePageHeader), new PropertyMetadata("All"));

    public string HeaderImage
    {
        get => (string)GetValue(HeaderImageProperty);
        set => SetValue(HeaderImageProperty, value);
    }

    public static readonly DependencyProperty HeaderImageProperty =
        DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(HomePageHeader), new PropertyMetadata(default(string), OnHeaderImageChanged));
    private static void OnHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeader)d;
        if (ctl != null)
        {
            ctl.ToggleTileOrImage();
        }
    }
    public Stretch Stretch
    {
        get { return (Stretch)GetValue(StretchProperty); }
        set { SetValue(StretchProperty, value); }
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(HomePageHeader), new PropertyMetadata(Stretch.UniformToFill));

    public bool IsTileImage
    {
        get { return (bool)GetValue(IsTileImageProperty); }
        set { SetValue(IsTileImageProperty, value); }
    }

    public static readonly DependencyProperty IsTileImageProperty =
        DependencyProperty.Register(nameof(IsTileImage), typeof(bool), typeof(HomePageHeader), new PropertyMetadata(false, OnIsTileImageChanged));
    private static void OnIsTileImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeader)d;
        if (ctl != null)
        {
            ctl.ToggleTileOrImage();
        }
    }

    private void ToggleTileOrImage()
    {
        if (heroImage == null || heroTile == null)
            return;

        if (IsTileImage)
        {
            heroImage.Visibility = Visibility.Collapsed;
            heroTile.Visibility = Visibility.Visible;
        }
        else
        {
            heroImage.Visibility = Visibility.Visible;
            heroTile.Visibility = Visibility.Collapsed;
        }
    }

    public HomePageHeader()
    {
        DefaultStyleKey = typeof(HomePageHeader);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        heroImage = GetTemplateChild(PART_HeroImage) as Image;
        heroTile = GetTemplateChild(PART_HeroTile) as TileControl;

        ToggleTileOrImage();
    }
}
