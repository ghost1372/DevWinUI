using System.Collections.ObjectModel;

namespace DevWinUI;
public sealed partial class ColorPalette : UserControl
{
    public CornerRadius RectangleCornerRadius
    {
        get { return (CornerRadius)GetValue(RectangleCornerRadiusProperty); }
        set { SetValue(RectangleCornerRadiusProperty, value); }
    }
    public ColorType Color
    {
        get { return (ColorType)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }
    public PaletteType Palette
    {
        get { return (PaletteType)GetValue(ColorPaleteTypeProperty); }
        set { SetValue(ColorPaleteTypeProperty, value); }
    }

    public bool ShowHeader
    {
        get { return (bool)GetValue(ShowHeaderProperty); }
        set { SetValue(ShowHeaderProperty, value); }
    }

    public double ItemWidth
    {
        get { return (double)GetValue(ItemWidthProperty); }
        set { SetValue(ItemWidthProperty, value); }
    }

    public static readonly DependencyProperty RectangleCornerRadiusProperty =
        DependencyProperty.Register(nameof(RectangleCornerRadius), typeof(CornerRadius), typeof(ColorPalette), new PropertyMetadata(new CornerRadius(0)));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(ColorType), typeof(ColorType), new PropertyMetadata(ColorType.Normal, OnColorChanged));

    public static readonly DependencyProperty ColorPaleteTypeProperty =
        DependencyProperty.Register(nameof(Palette), typeof(PaletteType), typeof(ColorPalette), new PropertyMetadata(PaletteType.TabView, OnPaletteChanged));

    public static readonly DependencyProperty ShowHeaderProperty =
        DependencyProperty.Register(nameof(ShowHeader), typeof(bool), typeof(ColorPalette), new PropertyMetadata(true));

    public static readonly DependencyProperty ItemWidthProperty =
        DependencyProperty.Register(nameof(ItemWidth), typeof(double), typeof(ColorPalette), new PropertyMetadata(120.0));


    public event SelectionChangedEventHandler SelectionChanged;
    public event ItemClickEventHandler ItemClick;

    public object ItemsSource
    {
        get => gridView.ItemsSource;
        set => gridView.ItemsSource = value;
    }

    public object SelectedItem
    {
        get => gridView.SelectedItem;
        set => gridView.SelectedItem = value;
    }
    public int SelectedIndex
    {
        get => gridView.SelectedIndex;
        set => gridView.SelectedIndex = value;
    }

    public DataTemplate ItemTemplate
    {
        get => gridView.ItemTemplate;
        set => gridView.ItemTemplate = value;
    }

    
    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.LoadColorPallete();
        }
    }

    private void LoadColorPallete()
    {
        switch (Color)
        {
            case ColorType.Normal:
                gridView.ItemsSource = ColorPaletteNormalResources();
                break;
            case ColorType.NormalLarge:
                gridView.ItemsSource = ColorPaletteNormalLargeResources();
                break;
        }
    }
    private static void OnPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorPalette)d;
        if (ctl != null)
        {
            ctl.ChangeItemTemplate();
        }
    }

    private void ChangeItemTemplate()
    {
        DataTemplate dataTemplate = null;
        switch (Palette)
        {
            case PaletteType.TabView:
                dataTemplate = Resources["ColorPaletteTabViewTemplate"] as DataTemplate;
                break;
            case PaletteType.Circle:
                dataTemplate = Resources["ColorPaletteCircleTemplate"] as DataTemplate;
                break;
            case PaletteType.Rectangle:
                dataTemplate = Resources["ColorPaletteRectangleTemplate"] as DataTemplate;
                break;
        }
        gridView.ItemTemplate = dataTemplate;
    }
    
    public ObservableCollection<ColorPaletteItem> ColorPaletteNormalResources()
    {
        var list = new ObservableCollection<ColorPaletteItem>();
        for (int i = 0; i < TintColorsList.Count; i++)
        {
            list.Add(new ColorPaletteItem
            {
                Hex = TintColorsList[i],
                ActualHex = TintColorsList[i],
                Color = ColorHelper.GetColorFromHex(TintColorsList[i]),
            });
        }
        return list;
    }
    public ObservableCollection<ColorPaletteItem> ColorPaletteNormalLargeResources()
    {
        var list = new ObservableCollection<ColorPaletteItem>();
        list.Add(new ColorPaletteItem
        {
            Hex = TintColorsList[0],
            ActualHex = TintColorsList[0],
            Color = ColorHelper.GetColorFromHex(TintColorsList[0]),
        });
        list.Add(new ColorPaletteItem
        {
            Hex = TintColorsList[1],
            ActualHex = TintColorsList[1],
            Color = ColorHelper.GetColorFromHex(TintColorsList[1]),
        });
        for (int i = 0; i < TintColorsListLarge.Count; i++)
        {
            list.Add(new ColorPaletteItem
            {
                Hex = ColorHelper.GetHexFromColor(TintColorsListLarge[i]),
                ActualHex = ColorHelper.GetHexFromColor(TintColorsListLarge[i]),
                Color = TintColorsListLarge[i],
            });
        }
        return list;
    }

    public ColorPalette()
    {
        this.InitializeComponent();
        DataContext = this;
        Color = ColorType.Normal;
        if (ItemsSource != null)
        {
            gridView.ItemsSource = ItemsSource;
        }

        Palette = PaletteType.TabView;
    }

    private readonly ObservableCollection<string> TintColorsList = new()
    {
        "#00000000",
        "#000000",
        "#f44336",
        "#e91e63",
        "#9c27b0",
        "#673ab7",
        "#3f51b5",
        "#2196f3",
        "#03a9f4",
        "#00bcd4",
        "#009688",
        "#4caf50",
        "#8bc34a",
        "#cddc39",
        "#ffeb3b",
        "#ffc107",
        "#ff9800",
        "#ff5722",
        "#795548",
        "#9e9e9e"
    };
    public ObservableCollection<Color> TintColorsListLarge { get; } = new()
    {
        Windows.UI.Color.FromArgb(255, 255, 185, 0),
        Windows.UI.Color.FromArgb(255, 255, 140, 0),
        Windows.UI.Color.FromArgb(255, 247, 99, 12),
        Windows.UI.Color.FromArgb(255, 202, 80, 16),
        Windows.UI.Color.FromArgb(255, 218, 59, 1),
        Windows.UI.Color.FromArgb(255, 239, 105, 80),
        Windows.UI.Color.FromArgb(255, 209, 52, 56),
        Windows.UI.Color.FromArgb(255, 255, 67, 67),
        Windows.UI.Color.FromArgb(255, 231, 72, 86),
        Windows.UI.Color.FromArgb(255, 232, 17, 35),
        Windows.UI.Color.FromArgb(255, 234, 0, 94),
        Windows.UI.Color.FromArgb(255, 195, 0, 82),
        Windows.UI.Color.FromArgb(255, 227, 0, 140),
        Windows.UI.Color.FromArgb(255, 191, 0, 119),
        Windows.UI.Color.FromArgb(255, 194, 57, 179),
        Windows.UI.Color.FromArgb(255, 154, 0, 137),
        Windows.UI.Color.FromArgb(255, 0, 120, 212),
        Windows.UI.Color.FromArgb(255, 0, 99, 177),
        Windows.UI.Color.FromArgb(255, 142, 140, 216),
        Windows.UI.Color.FromArgb(255, 107, 105, 214),
        Windows.UI.Color.FromArgb(255, 135, 100, 184),
        Windows.UI.Color.FromArgb(255, 116, 77, 169),
        Windows.UI.Color.FromArgb(255, 177, 70, 194),
        Windows.UI.Color.FromArgb(255, 136, 23, 152),
        Windows.UI.Color.FromArgb(255, 0, 153, 188),
        Windows.UI.Color.FromArgb(255, 45, 125, 154),
        Windows.UI.Color.FromArgb(255, 0, 183, 195),
        Windows.UI.Color.FromArgb(255, 3, 131, 135),
        Windows.UI.Color.FromArgb(255, 0, 178, 148),
        Windows.UI.Color.FromArgb(255, 1, 133, 116),
        Windows.UI.Color.FromArgb(255, 0, 204, 106),
        Windows.UI.Color.FromArgb(255, 16, 137, 62),
        Windows.UI.Color.FromArgb(255, 122, 117, 116),
        Windows.UI.Color.FromArgb(255, 93, 90, 88),
        Windows.UI.Color.FromArgb(255, 104, 118, 138),
        Windows.UI.Color.FromArgb(255, 81, 92, 107),
        Windows.UI.Color.FromArgb(255, 86, 124, 115),
        Windows.UI.Color.FromArgb(255, 72, 104, 96),
        Windows.UI.Color.FromArgb(255, 73, 130, 5),
        Windows.UI.Color.FromArgb(255, 16, 124, 16),
        Windows.UI.Color.FromArgb(255, 118, 118, 118),
        Windows.UI.Color.FromArgb(255, 76, 74, 72),
        Windows.UI.Color.FromArgb(255, 105, 121, 126),
        Windows.UI.Color.FromArgb(255, 74, 84, 89),
        Windows.UI.Color.FromArgb(255, 100, 124, 100),
        Windows.UI.Color.FromArgb(255, 82, 94, 84),
        Windows.UI.Color.FromArgb(255, 132, 117, 69),
        Windows.UI.Color.FromArgb(255, 126, 115, 95)
    };

    private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SelectionChanged?.Invoke(sender, e);
    }

    private void GridView_ItemClick(object sender, ItemClickEventArgs e)
    {
        ItemClick?.Invoke(sender, e);
    }
}

public enum PaletteType
{
    TabView,
    Circle,
    Rectangle
}

public enum ColorType
{
    Normal,
    NormalLarge,
}
