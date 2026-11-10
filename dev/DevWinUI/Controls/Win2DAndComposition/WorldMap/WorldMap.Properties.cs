namespace DevWinUI;

public partial class WorldMap
{
    public static readonly DependencyProperty CountryFillProperty =
        DependencyProperty.Register(nameof(CountryFill), typeof(Brush), typeof(WorldMap), new PropertyMetadata(null, OnVisualChanged));

    public static readonly DependencyProperty CountryHoverFillProperty =
        DependencyProperty.Register(nameof(CountryHoverFill), typeof(Brush), typeof(WorldMap), new PropertyMetadata(null, OnVisualChanged));

    public static readonly DependencyProperty CountrySelectedFillProperty =
        DependencyProperty.Register(nameof(CountrySelectedFill), typeof(Brush), typeof(WorldMap), new PropertyMetadata(null, OnVisualChanged));

    public static readonly DependencyProperty CountryStrokeProperty =
        DependencyProperty.Register(nameof(CountryStroke), typeof(Brush), typeof(WorldMap), new PropertyMetadata(null, OnVisualChanged));

    public static readonly DependencyProperty CountryStrokeThicknessProperty =
        DependencyProperty.Register(nameof(CountryStrokeThickness), typeof(double), typeof(WorldMap), new PropertyMetadata(0.75, OnVisualChanged));

    public static readonly DependencyProperty SelectionModeProperty =
        DependencyProperty.Register(nameof(SelectionMode), typeof(WorldMapSelectionMode), typeof(WorldMap), new PropertyMetadata(WorldMapSelectionMode.Single, OnSelectionModeChanged));

    public static readonly DependencyProperty SelectedCountryCodeProperty =
        DependencyProperty.Register(nameof(SelectedCountryCode), typeof(string), typeof(WorldMap), new PropertyMetadata(null, OnSelectedCountryCodeChanged));

    public static readonly DependencyProperty IsPanEnabledProperty =
        DependencyProperty.Register(nameof(IsPanEnabled), typeof(bool), typeof(WorldMap), new PropertyMetadata(true));

    public static readonly DependencyProperty IsZoomEnabledProperty =
        DependencyProperty.Register(nameof(IsZoomEnabled), typeof(bool), typeof(WorldMap), new PropertyMetadata(true));

    public static readonly DependencyProperty MinZoomProperty =
        DependencyProperty.Register(nameof(MinZoom), typeof(double), typeof(WorldMap), new PropertyMetadata(1.0));

    public static readonly DependencyProperty MaxZoomProperty =
        DependencyProperty.Register(nameof(MaxZoom), typeof(double), typeof(WorldMap), new PropertyMetadata(40.0));

    public static readonly DependencyProperty ShowToolTipProperty =
        DependencyProperty.Register(nameof(ShowToolTip), typeof(bool), typeof(WorldMap), new PropertyMetadata(true));

    public static readonly DependencyProperty ShowZoomControlsProperty =
        DependencyProperty.Register(nameof(ShowZoomControls), typeof(bool), typeof(WorldMap), new PropertyMetadata(true, OnShowZoomControlsChanged));

    public static readonly DependencyProperty ClearSelectionOnEmptyClickProperty =
        DependencyProperty.Register(nameof(ClearSelectionOnEmptyClick), typeof(bool), typeof(WorldMap), new PropertyMetadata(true));

    public static readonly DependencyProperty DataSourceProperty =
        DependencyProperty.Register(nameof(DataSource), typeof(Uri), typeof(WorldMap), new PropertyMetadata(null));

    public static readonly DependencyProperty HighlightColorProperty =
        DependencyProperty.Register(nameof(HighlightColor), typeof(WorldMapHighlightColor), typeof(WorldMap), new PropertyMetadata(WorldMapHighlightColor.Default, OnVisualChanged));

    public static readonly DependencyProperty CustomHighlightColorProperty =
        DependencyProperty.Register(nameof(CustomHighlightColor), typeof(Windows.UI.Color), typeof(WorldMap), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 0, 120, 212), OnVisualChanged));

    public static readonly DependencyProperty CountryColorProperty =
        DependencyProperty.Register(nameof(CountryColor), typeof(WorldMapHighlightColor), typeof(WorldMap), new PropertyMetadata(WorldMapHighlightColor.Default, OnVisualChanged));

    public static readonly DependencyProperty CustomCountryColorProperty =
        DependencyProperty.Register(nameof(CustomCountryColor), typeof(Windows.UI.Color), typeof(WorldMap), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 100, 116, 139), OnVisualChanged));

    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register(nameof(SelectedColor), typeof(WorldMapHighlightColor), typeof(WorldMap), new PropertyMetadata(WorldMapHighlightColor.Default, OnVisualChanged));

    public static readonly DependencyProperty CustomSelectedColorProperty =
        DependencyProperty.Register(nameof(CustomSelectedColor), typeof(Windows.UI.Color), typeof(WorldMap), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 0, 120, 212), OnVisualChanged));

    public static readonly DependencyProperty CountryBorderStyleProperty =
        DependencyProperty.Register(nameof(CountryBorderStyle), typeof(WorldMapBorderStyle), typeof(WorldMap), new PropertyMetadata(WorldMapBorderStyle.Solid, OnBorderStyleChanged));

    public static readonly DependencyProperty BorderColorProperty =
        DependencyProperty.Register(nameof(BorderColor), typeof(WorldMapHighlightColor), typeof(WorldMap), new PropertyMetadata(WorldMapHighlightColor.Default, OnVisualChanged));

    public static readonly DependencyProperty CustomBorderColorProperty =
        DependencyProperty.Register(nameof(CustomBorderColor), typeof(Windows.UI.Color), typeof(WorldMap), new PropertyMetadata(Windows.UI.Color.FromArgb(255, 148, 163, 184), OnVisualChanged));

    public WorldMapHighlightColor BorderColor
    {
        get => (WorldMapHighlightColor)GetValue(BorderColorProperty);
        set => SetValue(BorderColorProperty, value);
    }

    public Windows.UI.Color CustomBorderColor
    {
        get => (Windows.UI.Color)GetValue(CustomBorderColorProperty);
        set => SetValue(CustomBorderColorProperty, value);
    }

    public WorldMapHighlightColor CountryColor
    {
        get => (WorldMapHighlightColor)GetValue(CountryColorProperty);
        set => SetValue(CountryColorProperty, value);
    }

    public Windows.UI.Color CustomCountryColor
    {
        get => (Windows.UI.Color)GetValue(CustomCountryColorProperty);
        set => SetValue(CustomCountryColorProperty, value);
    }

    public WorldMapHighlightColor SelectedColor
    {
        get => (WorldMapHighlightColor)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    public Windows.UI.Color CustomSelectedColor
    {
        get => (Windows.UI.Color)GetValue(CustomSelectedColorProperty);
        set => SetValue(CustomSelectedColorProperty, value);
    }

    public WorldMapBorderStyle CountryBorderStyle
    {
        get => (WorldMapBorderStyle)GetValue(CountryBorderStyleProperty);
        set => SetValue(CountryBorderStyleProperty, value);
    }

    public WorldMapHighlightColor HighlightColor
    {
        get => (WorldMapHighlightColor)GetValue(HighlightColorProperty);
        set => SetValue(HighlightColorProperty, value);
    }

    public Windows.UI.Color CustomHighlightColor
    {
        get => (Windows.UI.Color)GetValue(CustomHighlightColorProperty);
        set => SetValue(CustomHighlightColorProperty, value);
    }

    public Brush? CountryFill
    {
        get => (Brush?)GetValue(CountryFillProperty);
        set => SetValue(CountryFillProperty, value);
    }

    public Brush? CountryHoverFill
    {
        get => (Brush?)GetValue(CountryHoverFillProperty);
        set => SetValue(CountryHoverFillProperty, value);
    }

    public Brush? CountrySelectedFill
    {
        get => (Brush?)GetValue(CountrySelectedFillProperty);
        set => SetValue(CountrySelectedFillProperty, value);
    }

    public Brush? CountryStroke
    {
        get => (Brush?)GetValue(CountryStrokeProperty);
        set => SetValue(CountryStrokeProperty, value);
    }

    public double CountryStrokeThickness
    {
        get => (double)GetValue(CountryStrokeThicknessProperty);
        set => SetValue(CountryStrokeThicknessProperty, value);
    }

    public WorldMapSelectionMode SelectionMode
    {
        get => (WorldMapSelectionMode)GetValue(SelectionModeProperty);
        set => SetValue(SelectionModeProperty, value);
    }

    public string? SelectedCountryCode
    {
        get => (string?)GetValue(SelectedCountryCodeProperty);
        set => SetValue(SelectedCountryCodeProperty, value);
    }

    public bool IsPanEnabled
    {
        get => (bool)GetValue(IsPanEnabledProperty);
        set => SetValue(IsPanEnabledProperty, value);
    }

    public bool IsZoomEnabled
    {
        get => (bool)GetValue(IsZoomEnabledProperty);
        set => SetValue(IsZoomEnabledProperty, value);
    }

    public double MinZoom
    {
        get => (double)GetValue(MinZoomProperty);
        set => SetValue(MinZoomProperty, value);
    }

    public double MaxZoom
    {
        get => (double)GetValue(MaxZoomProperty);
        set => SetValue(MaxZoomProperty, value);
    }

    public bool ShowToolTip
    {
        get => (bool)GetValue(ShowToolTipProperty);
        set => SetValue(ShowToolTipProperty, value);
    }

    public bool ShowZoomControls
    {
        get => (bool)GetValue(ShowZoomControlsProperty);
        set => SetValue(ShowZoomControlsProperty, value);
    }

    public bool ClearSelectionOnEmptyClick
    {
        get => (bool)GetValue(ClearSelectionOnEmptyClickProperty);
        set => SetValue(ClearSelectionOnEmptyClickProperty, value);
    }

    public Uri DataSource
    {
        get => (Uri)GetValue(DataSourceProperty);
        set => SetValue(DataSourceProperty, value);
    }

    public Func<WorldMapCountry, string>? ToolTipFormatter { get; set; }

    private static void OnBorderStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WorldMap)d).ResetStrokeCaches();
    }

    private static void OnVisualChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WorldMap)d).Invalidate();
    }

    private static void OnSelectionModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var map = (WorldMap)d;
        var mode = (WorldMapSelectionMode)e.NewValue;

        if (mode == WorldMapSelectionMode.None)
        {
            map.ClearSelection();
        }
        else if (mode == WorldMapSelectionMode.Single && map._selected.Count > 1)
        {
            map.SelectCountry(map.SelectedCountryCode ?? map._selected.First());
        }
    }

    private static void OnSelectedCountryCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var map = (WorldMap)d;
        if (map._updatingSelection)
        {
            return;
        }

        if (e.NewValue is string code && code.Length > 0)
        {
            map.SelectCountry(code);
        }
        else
        {
            map.ClearSelection();
        }
    }

    private static void OnShowZoomControlsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((WorldMap)d).UpdateZoomControlsVisibility();
    }
}
