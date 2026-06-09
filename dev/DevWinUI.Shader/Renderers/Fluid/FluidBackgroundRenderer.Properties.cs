using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class FluidBackgroundRenderer
{
    private bool isFluidOverlayLightWaveEnabled = false;
    private bool isColorDitheringEnabled = true;
    public bool isStatic { get; set; } = false;
    private double fluidOverlayOpacity = 100.0;
    private Color fluidAccentColor1 = Colors.Blue;
    private Color fluidAccentColor2 = Colors.AliceBlue;
    private Color fluidAccentColor3 = Colors.DarkBlue;
    private Color fluidAccentColor4 = Colors.Orange;

    public bool IsLightWaveEnabled
    {
        get { return (bool)GetValue(IsLightWaveEnabledProperty); }
        set { SetValue(IsLightWaveEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsLightWaveEnabledProperty =
        DependencyProperty.Register(nameof(IsLightWaveEnabled), typeof(bool), typeof(FluidBackgroundRenderer), new PropertyMetadata(false, OnIsLightWaveEnabledChanged));

    private static void OnIsLightWaveEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.isFluidOverlayLightWaveEnabled = (bool)e.NewValue;
    }

    public bool IsColorDitheringEnabled
    {
        get { return (bool)GetValue(IsColorDitheringEnabledProperty); }
        set { SetValue(IsColorDitheringEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsColorDitheringEnabledProperty =
        DependencyProperty.Register(nameof(IsColorDitheringEnabled), typeof(bool), typeof(FluidBackgroundRenderer), new PropertyMetadata(true, OnIsColorDitheringEnabledChanged));

    private static void OnIsColorDitheringEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.isColorDitheringEnabled = (bool)e.NewValue;
    }

    public bool IsStatic
    {
        get { return (bool)GetValue(IsStaticProperty); }
        set { SetValue(IsStaticProperty, value); }
    }

    public static readonly DependencyProperty IsStaticProperty =
        DependencyProperty.Register(nameof(IsStatic), typeof(bool), typeof(FluidBackgroundRenderer), new PropertyMetadata(false, OnIsStaticChanged));

    private static void OnIsStaticChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.isStatic = (bool)e.NewValue;
    }

    public double Opacity
    {
        get { return (double)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register(nameof(Opacity), typeof(double), typeof(FluidBackgroundRenderer), new PropertyMetadata(100.0, OnOpacityChanged));

    private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.fluidOverlayOpacity = (double)e.NewValue;
    }
    public Color AccentColor1
    {
        get { return (Color)GetValue(AccentColor1Property); }
        set { SetValue(AccentColor1Property, value); }
    }

    public static readonly DependencyProperty AccentColor1Property =
        DependencyProperty.Register(nameof(AccentColor1), typeof(Color), typeof(FluidBackgroundRenderer), new PropertyMetadata(default(Color), OnAccentColor1Changed));

    private static void OnAccentColor1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.fluidAccentColor1 = (Color)e.NewValue;
        ctl.UpdatePalette();
    }
    public Color AccentColor2
    {
        get { return (Color)GetValue(AccentColor2Property); }
        set { SetValue(AccentColor2Property, value); }
    }

    public static readonly DependencyProperty AccentColor2Property =
        DependencyProperty.Register(nameof(AccentColor2), typeof(Color), typeof(FluidBackgroundRenderer), new PropertyMetadata(default(Color), OnAccentColor2Changed));

    private static void OnAccentColor2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.fluidAccentColor2 = (Color)e.NewValue;
        ctl.UpdatePalette();
    }
    public Color AccentColor3
    {
        get { return (Color)GetValue(AccentColor3Property); }
        set { SetValue(AccentColor3Property, value); }
    }

    public static readonly DependencyProperty AccentColor3Property =
        DependencyProperty.Register(nameof(AccentColor3), typeof(Color), typeof(FluidBackgroundRenderer), new PropertyMetadata(default(Color), OnAccentColor3Changed));

    private static void OnAccentColor3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.fluidAccentColor3 = (Color)e.NewValue;
        ctl.UpdatePalette();
    }
    public Color AccentColor4
    {
        get { return (Color)GetValue(AccentColor4Property); }
        set { SetValue(AccentColor4Property, value); }
    }

    public static readonly DependencyProperty AccentColor4Property =
        DependencyProperty.Register(nameof(AccentColor4), typeof(Color), typeof(FluidBackgroundRenderer), new PropertyMetadata(default(Color), OnAccentColor4Changed));

    private static void OnAccentColor4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FluidBackgroundRenderer)d;
        ctl.fluidAccentColor4 = (Color)e.NewValue;
        ctl.UpdatePalette();
    }
}
