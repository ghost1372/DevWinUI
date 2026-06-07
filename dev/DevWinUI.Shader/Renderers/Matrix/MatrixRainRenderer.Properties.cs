using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class MatrixRainRenderer
{
    private double matrixRainSpeed = 100.0;
    private double matrixRainDensity = 70.0;
    private double matrixRainGlyphSize = 14.0;
    private Color matrixRainColor = Color.FromArgb(255, 0, 255, 70);
    private string matrixRainFontFamily = "Consolas";
    private double matrixRainFontSize = 12.0;
    private string matrixRainGlyphs = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(MatrixRainRenderer), new PropertyMetadata(100.0, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainSpeed = (double)e.NewValue;
    }

    public double Density
    {
        get { return (double)GetValue(DensityProperty); }
        set { SetValue(DensityProperty, value); }
    }

    public static readonly DependencyProperty DensityProperty =
        DependencyProperty.Register(nameof(Density), typeof(double), typeof(MatrixRainRenderer), new PropertyMetadata(70.0, OnDensityChanged));

    private static void OnDensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainDensity = (double)e.NewValue;
    }

    public double GlyphSize
    {
        get { return (double)GetValue(GlyphSizeProperty); }
        set { SetValue(GlyphSizeProperty, value); }
    }

    public static readonly DependencyProperty GlyphSizeProperty =
        DependencyProperty.Register(nameof(GlyphSize), typeof(double), typeof(MatrixRainRenderer), new PropertyMetadata(14.0, OnGlyphSizeChanged));

    private static void OnGlyphSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainGlyphSize = (double)e.NewValue;
    }

    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(MatrixRainRenderer), new PropertyMetadata(Color.FromArgb(255, 0, 255, 70), OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainColor = (Color)e.NewValue;
    }

    public string FontFamily
    {
        get { return (string)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public static readonly DependencyProperty FontFamilyProperty =
        DependencyProperty.Register(nameof(FontFamily), typeof(string), typeof(MatrixRainRenderer), new PropertyMetadata("Consolas", OnFontFamilyChanged));

    private static void OnFontFamilyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainFontFamily = (string)e.NewValue;
    }

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly DependencyProperty FontSizeProperty =
        DependencyProperty.Register(nameof(FontSize), typeof(double), typeof(MatrixRainRenderer), new PropertyMetadata(12.0, OnFontSizeChanged));

    private static void OnFontSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainFontSize = (double)e.NewValue;
    }

    public string Glyphs
    {
        get { return (string)GetValue(MatrixRainGlyphsProperty); }
        set { SetValue(MatrixRainGlyphsProperty, value); }
    }

    public static readonly DependencyProperty MatrixRainGlyphsProperty =
        DependencyProperty.Register(nameof(Glyphs), typeof(string), typeof(MatrixRainRenderer), new PropertyMetadata("0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ", OnGlyphsChanged));

    private static void OnGlyphsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (MatrixRainRenderer)d;
        ctl.matrixRainGlyphs = (string)e.NewValue;
    }
}
