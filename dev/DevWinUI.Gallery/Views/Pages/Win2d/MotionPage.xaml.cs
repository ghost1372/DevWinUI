namespace DevWinUIGallery.Views;

public sealed partial class MotionPage : Page
{
    private static readonly DependencyProperty[] ResettableProperties =
    {
        MotionWall.OrientationProperty,
        MotionWall.LanesProperty,
        MotionWall.AddEdgeLanesProperty,
        MotionWall.ItemsPerLaneProperty,
        MotionWall.ItemDistributionProperty,
        MotionWall.GapProperty,
        MotionWall.TileWidthProperty,
        MotionWall.TileHeightProperty,
        MotionWall.TileAspectRatioProperty,
        MotionWall.TileCornerRadiusProperty,
        MotionWall.SpeedProperty,
        MotionWall.LoopDurationProperty,
        MotionWall.DirectionProperty,
        MotionWall.VarianceProperty,
        MotionWall.VarianceStyleProperty,
        MotionWall.RandomStartProperty,
        MotionWall.PauseOnHoverProperty,
        MotionWall.MaxFrameRateProperty,
        MotionWall.TiltProperty,
        MotionWall.TurnProperty,
        MotionWall.RollProperty,
        MotionWall.PerspectiveProperty,
        MotionWall.DepthProperty,
        MotionWall.IsInteractiveProperty,
        MotionWall.ParallaxProperty,
        MotionWall.PointerTiltProperty,
        MotionWall.SpotlightProperty,
        MotionWall.DimProperty,
        MotionWall.GrayscaleProperty,
        MotionWall.LiftProperty,
        MotionWall.OverlayOpacityProperty,
        MotionWall.ShowLinesProperty,
        MotionWall.FadeEdgesProperty,
        MotionWall.EdgeFadeSizeProperty,
        MotionWall.EdgeFadeStrengthProperty
    };
    public ObservableCollection<Orientation> OrientationItems { get; set; } = new ObservableCollection<Orientation>(Enum.GetValues<Orientation>());
    public ObservableCollection<MotionWallDirection> DirectionItems { get; set; } = new ObservableCollection<MotionWallDirection>(Enum.GetValues<MotionWallDirection>());
    public ObservableCollection<MotionWallDistribution> DistributionItems { get; set; } = new ObservableCollection<MotionWallDistribution>(Enum.GetValues<MotionWallDistribution>());
    public ObservableCollection<MotionWallVarianceStyle> VarianceStyleItems { get; set; } = new ObservableCollection<MotionWallVarianceStyle>(Enum.GetValues<MotionWallVarianceStyle>());

    public MotionPage()
    {
        InitializeComponent();

        Wall.ItemsSource = CreateItems();
        Wall.TileInvoked += OnTileInvoked;

        Sync();
    }

    private static List<object?> CreateItems()
    {
        var items = new List<object?>
        {
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/1.jpeg", Title = "Photo 1" },
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/2.png", Title = "Photo 2" },
            new MotionWallItem
            {
                Text = "Open docs",
                Title = "Opens the WinUI docs",
                Href = new Uri("https://ghost1372.github.io")
            },
            new MotionWallItem { Text = "Text tile", Title = "Plain text tile" },
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/3.jpeg", Title = "Photo 3" },
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/4.jpeg", Title = "Photo 4" },
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/7.jpeg", Title = "Photo 5" },
            new MotionWallItem { Source = "ms-appx:///Assets/BannerView/8.jpeg", Title = "Photo 6" }
        };

        return items;
    }

    private void OnTileInvoked(object? sender, MotionWallTileEventArgs e)
    {
        string title = e.Item is MotionWallItem item ? item.Title ?? item.Text ?? "tile" : "tile";
        StatusText.Text = $"Invoked #{e.Index}: {title}";
    }

    private void Sync()
    {
        LanesSlider.Value = Wall.Lanes;
        ItemsPerLaneBox.Value = Wall.ItemsPerLane;
        OrientationBox.SelectedItem = Wall.Orientation;
        DirectionBox.SelectedItem = Wall.Direction;
        DistributionBox.SelectedItem = Wall.ItemDistribution;
        VarianceStyleBox.SelectedItem = Wall.VarianceStyle;
    }

    private void Preset_Click(object sender, RoutedEventArgs e)
    {
        foreach (DependencyProperty property in ResettableProperties)
        {
            Wall.ClearValue(property);
        }

        switch ((string)((Button)sender).Tag)
        {
            case "marquee":
                Wall.Lanes = 4;
                Wall.AddEdgeLanes = true;
                Wall.ItemsPerLane = 6;
                Wall.Gap = 24;
                Wall.TileAspectRatio = 16d / 9d;
                Wall.LoopDuration = 40;
                Wall.Variance = 0.15;
                Wall.VarianceStyle = MotionWallVarianceStyle.Stepped;
                Wall.Tilt = 18;
                Wall.Roll = -22;
                Wall.Perspective = 1000;
                Wall.Spotlight = 0;
                Wall.Parallax = 0;
                Wall.Dim = 0;
                Wall.ShowLines = true;
                Wall.EdgeFadeStrength = 1;
                break;

            case "drift":
                Wall.Orientation = Orientation.Vertical;
                Wall.Lanes = 5;
                Wall.TileWidth = 200;
                Wall.TileHeight = 132;
                Wall.Gap = 18;
                Wall.TileCornerRadius = 14;
                Wall.Speed = 42;
                Wall.Variance = 0.45;
                Wall.ItemDistribution = MotionWallDistribution.Interleaved;
                Wall.Tilt = 16;
                Wall.Turn = -14;
                Wall.Roll = 0;
                Wall.Depth = 120;
                Wall.IsInteractive = true;
                Wall.Lift = 64;
                Wall.PointerTilt = 0.6;
                Wall.Spotlight = 0;
                Wall.Parallax = 0;
                Wall.Dim = 0.45;
                Wall.OverlayOpacity = 0.42;
                Wall.EdgeFadeStrength = 0.6;
                break;

            case "grid":
                Wall.Lanes = 4;
                Wall.TileAspectRatio = 1.33;
                Wall.Gap = 16;
                Wall.TileCornerRadius = 14;
                Wall.Speed = 24;
                Wall.Tilt = 0;
                Wall.Roll = -12;
                Wall.Parallax = 0.5;
                Wall.Spotlight = 0.6;
                Wall.Dim = 0.35;
                Wall.Grayscale = true;
                Wall.EdgeFadeStrength = 0.5;
                break;
        }

        Sync();
    }

    private void LanesSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        Wall.Lanes = (int)e.NewValue;
    }

    private void ItemsPerLaneBox_ValueChanged(NumberBox sender, NumberBoxValueChangedEventArgs args)
    {
        if (double.IsNaN(args.NewValue))
        {
            return;
        }

        Wall.ItemsPerLane = (int)args.NewValue;
    }
}
