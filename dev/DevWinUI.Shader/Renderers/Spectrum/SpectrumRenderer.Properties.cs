using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.Foundation;
using Windows.UI;

namespace DevWinUI;

public partial class SpectrumRenderer
{
    public float[]? SpectrumData { get; set; } = null;
    private int spectrumBarCount = 10;
    private bool isSpectrumGlowEffectEnabled = false;
    private int spectrumOpacity = 100;
    private SpectrumPlacement spectrumPlacement = SpectrumPlacement.Bottom;
    private SpectrumStyle spectrumStyle = SpectrumStyle.Curve;
    private Color spectrumColor = Colors.Blue;
    private Rect spectrumAlbumArtRect = default(Rect);
    private int spectrumCoverImageRadius = 12;

    public int BarCount
    {
        get { return (int)GetValue(BarCountProperty); }
        set { SetValue(BarCountProperty, value); }
    }

    public static readonly DependencyProperty BarCountProperty =
        DependencyProperty.Register(nameof(BarCount), typeof(int), typeof(SpectrumRenderer), new PropertyMetadata(10, OnBarCountChanged));

    private static void OnBarCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumBarCount = (int)e.NewValue;
    }

    public bool IsGlowEffectEnabled
    {
        get { return (bool)GetValue(IsGlowEffectEnabledProperty); }
        set { SetValue(IsGlowEffectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsGlowEffectEnabledProperty =
        DependencyProperty.Register(nameof(IsGlowEffectEnabled), typeof(bool), typeof(SpectrumRenderer), new PropertyMetadata(false, OnIsGlowEffectEnabledChanged));

    private static void OnIsGlowEffectEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.isSpectrumGlowEffectEnabled = (bool)e.NewValue;
    }

    public int Opacity
    {
        get { return (int)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register(nameof(Opacity), typeof(int), typeof(SpectrumRenderer), new PropertyMetadata(100, OnOpacityChanged));

    private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumOpacity = (int)e.NewValue;
    }

    public SpectrumPlacement Placement
    {
        get { return (SpectrumPlacement)GetValue(PlacementProperty); }
        set { SetValue(PlacementProperty, value); }
    }

    public static readonly DependencyProperty PlacementProperty =
        DependencyProperty.Register(nameof(Placement), typeof(SpectrumPlacement), typeof(SpectrumRenderer), new PropertyMetadata(SpectrumPlacement.Bottom, OnPlacementChanged));

    private static void OnPlacementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumPlacement = (SpectrumPlacement)e.NewValue;
    }
    public SpectrumStyle Style
    {
        get { return (SpectrumStyle)GetValue(StyleProperty); }
        set { SetValue(StyleProperty, value); }
    }

    public static readonly DependencyProperty StyleProperty =
        DependencyProperty.Register(nameof(Style), typeof(SpectrumStyle), typeof(SpectrumRenderer), new PropertyMetadata(SpectrumStyle.Curve, OnStyleChanged));

    private static void OnStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumStyle = (SpectrumStyle)e.NewValue;
    }

    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(SpectrumRenderer), new PropertyMetadata(Colors.Black, OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumColor = (Color)e.NewValue;
    }

    public Rect AlbumArtRect
    {
        get { return (Rect)GetValue(AlbumArtRectProperty); }
        set { SetValue(AlbumArtRectProperty, value); }
    }

    public static readonly DependencyProperty AlbumArtRectProperty =
        DependencyProperty.Register(nameof(AlbumArtRect), typeof(Rect), typeof(SpectrumRenderer), new PropertyMetadata(default(Rect), OnAlbumArtRectChanged));

    private static void OnAlbumArtRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumAlbumArtRect = (Rect)e.NewValue;
    }

    public int CoverImageRadius
    {
        get { return (int)GetValue(CoverImageRadiusProperty); }
        set { SetValue(CoverImageRadiusProperty, value); }
    }

    public static readonly DependencyProperty CoverImageRadiusProperty =
        DependencyProperty.Register(nameof(CoverImageRadius), typeof(int), typeof(SpectrumRenderer), new PropertyMetadata(12, OnCoverImageRadiusChanged));

    private static void OnCoverImageRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SpectrumRenderer)d;
        ctl.spectrumCoverImageRadius = (int)e.NewValue;
    }
}
