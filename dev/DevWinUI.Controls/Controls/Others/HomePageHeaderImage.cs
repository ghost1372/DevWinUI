namespace DevWinUI;

[TemplatePart(Name = nameof(PART_HeroImage), Type = typeof(Image))]
[TemplatePart(Name = nameof(PART_HeroTile), Type = typeof(TileControl))]
public partial class HomePageHeaderImage : Control
{
    private const string PART_HeroImage = "PART_HeroImage";
    private const string PART_HeroTile = "PART_HeroTile";

    private Image heroImage;
    private TileControl heroTile;

    public string HeaderImage
    {
        get => (string)GetValue(HeaderImageProperty);
        set => SetValue(HeaderImageProperty, value);
    }

    public static readonly DependencyProperty HeaderImageProperty =
        DependencyProperty.Register(nameof(HeaderImage), typeof(string), typeof(HomePageHeaderImage), new PropertyMetadata(default(string), OnHeaderImageChanged));
    private static void OnHeaderImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeaderImage)d;
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
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(HomePageHeaderImage), new PropertyMetadata(Stretch.UniformToFill));

    public bool IsTileImage
    {
        get { return (bool)GetValue(IsTileImageProperty); }
        set { SetValue(IsTileImageProperty, value); }
    }

    public static readonly DependencyProperty IsTileImageProperty =
        DependencyProperty.Register(nameof(IsTileImage), typeof(bool), typeof(HomePageHeaderImage), new PropertyMetadata(false, OnIsTileImageChanged));
    private static void OnIsTileImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (HomePageHeaderImage)d;
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

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        heroImage = GetTemplateChild(PART_HeroImage) as Image;
        heroTile = GetTemplateChild(PART_HeroTile) as TileControl;

        ToggleTileOrImage();
    }
}
