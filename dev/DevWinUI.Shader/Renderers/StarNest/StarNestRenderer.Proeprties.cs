using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class StarNestRenderer
{
    private float speed = 0.005f;
    private float zoom = 0.8f;
    private float tile = 0.85f;

    private float rotationSpeed1 = 0.5f;
    private float rotationSpeed2 = 0.8f;

    private float stepScale = 0.001f;
    private float fadePower = 0.73f;

    private float colorIntensity = 0.01f;

    private Color color = Colors.SaddleBrown;

    public double Speed
    {
        get => speed;
        set => SetValue(SpeedProperty, value);
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.005f, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.speed = (float)(double)e.NewValue;
    }

    public double Zoom
    {
        get => zoom;
        set => SetValue(ZoomProperty, value);
    }

    public static readonly DependencyProperty ZoomProperty =
        DependencyProperty.Register(nameof(Zoom), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.8d, OnZoomChanged));

    private static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.zoom = (float)(double)e.NewValue;
    }

    public double Tile
    {
        get => tile;
        set => SetValue(TileProperty, value);
    }

    public static readonly DependencyProperty TileProperty =
        DependencyProperty.Register(nameof(Tile), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.85d, OnTileChanged));

    private static void OnTileChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.tile = (float)(double)e.NewValue;
    }

    public double RotationSpeed1
    {
        get => rotationSpeed1;
        set => SetValue(RotationSpeed1Property, value);
    }

    public static readonly DependencyProperty RotationSpeed1Property =
        DependencyProperty.Register(nameof(RotationSpeed1), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.5d, OnRotationSpeed1Changed));

    private static void OnRotationSpeed1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.rotationSpeed1 = (float)(double)e.NewValue;
    }

    public double RotationSpeed2
    {
        get => rotationSpeed2;
        set => SetValue(RotationSpeed2Property, value);
    }

    public static readonly DependencyProperty RotationSpeed2Property =
        DependencyProperty.Register(nameof(RotationSpeed2), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.8d, OnRotationSpeed2Changed));

    private static void OnRotationSpeed2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.rotationSpeed2 = (float)(double)e.NewValue;
    }

    public double StepScale
    {
        get => stepScale;
        set => SetValue(StepScaleProperty, value);
    }

    public static readonly DependencyProperty StepScaleProperty =
        DependencyProperty.Register(nameof(StepScale), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.001d, OnStepScaleChanged));

    private static void OnStepScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.stepScale = (float)(double)e.NewValue;
    }

    public double FadePower
    {
        get => fadePower;
        set => SetValue(FadePowerProperty, value);
    }

    public static readonly DependencyProperty FadePowerProperty =
        DependencyProperty.Register(nameof(FadePower), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.73d, OnFadePowerChanged));

    private static void OnFadePowerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.fadePower = (float)(double)e.NewValue;
    }

    public double ColorIntensity
    {
        get => colorIntensity;
        set => SetValue(ColorIntensityProperty, value);
    }

    public static readonly DependencyProperty ColorIntensityProperty =
        DependencyProperty.Register(nameof(ColorIntensity), typeof(double), typeof(StarNestRenderer), new PropertyMetadata(0.01d, OnColorIntensityChanged));

    private static void OnColorIntensityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.colorIntensity = (float)(double)e.NewValue;
    }

    public Color Color
    {
        get => color;
        set => SetValue(ColorProperty, value);
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(StarNestRenderer), new PropertyMetadata(Colors.SaddleBrown, OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (StarNestRenderer)d;
        ctl.color = (Color)e.NewValue;
    }
}
