using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class AbstractMovementBackgroundRenderer
{
    private Color accentColor1 = ColorHelper.GetColorFromHex("#EAE2D3");
    private Color accentColor2 = ColorHelper.GetColorFromHex("#F34729");
    private Color accentColor3 = ColorHelper.GetColorFromHex("#1C1F47");
    private Color accentColor4 = ColorHelper.GetColorFromHex("#303380");
    private AbstractMovementDirection direction = AbstractMovementDirection.BottomLeft;

    public Color AccentColor1
    {
        get => accentColor1;
        set => SetValue(AccentColor1Property, value);
    }
    public static readonly DependencyProperty AccentColor1Property =
        DependencyProperty.Register(nameof(AccentColor1), typeof(Color), typeof(AbstractMovementBackgroundRenderer), new PropertyMetadata(ColorHelper.GetColorFromHex("#EAE2D3"), OnAccentColor1Changed));
    private static void OnAccentColor1Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AbstractMovementBackgroundRenderer)d;
        ctl.accentColor1 = (Color)e.NewValue;
    }

    public Color AccentColor2
    {
        get => accentColor2;
        set => SetValue(AccentColor2Property, value);
    }
    public static readonly DependencyProperty AccentColor2Property =
        DependencyProperty.Register(nameof(AccentColor2), typeof(Color), typeof(AbstractMovementBackgroundRenderer), new PropertyMetadata(ColorHelper.GetColorFromHex("#F34729"), OnAccentColor2Changed));
    private static void OnAccentColor2Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AbstractMovementBackgroundRenderer)d;
        ctl.accentColor2 = (Color)e.NewValue;
    }

    public Color AccentColor3
    {
        get => accentColor3;
        set => SetValue(AccentColor3Property, value);
    }
    public static readonly DependencyProperty AccentColor3Property =
        DependencyProperty.Register(nameof(AccentColor3), typeof(Color), typeof(AbstractMovementBackgroundRenderer), new PropertyMetadata(ColorHelper.GetColorFromHex("#1C1F47"), OnAccentColor3Changed));
    private static void OnAccentColor3Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AbstractMovementBackgroundRenderer)d;
        ctl.accentColor3 = (Color)e.NewValue;
    }

    public Color AccentColor4
    {
        get => accentColor4;
        set => SetValue(AccentColor4Property, value);
    }
    public static readonly DependencyProperty AccentColor4Property =
        DependencyProperty.Register(nameof(AccentColor4), typeof(Color), typeof(AbstractMovementBackgroundRenderer), new PropertyMetadata(ColorHelper.GetColorFromHex("#303380"), OnAccentColor4Changed));
    private static void OnAccentColor4Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AbstractMovementBackgroundRenderer)d;
        ctl.accentColor4 = (Color)e.NewValue;
    }

    public AbstractMovementDirection Direction
    {
        get { return (AbstractMovementDirection)GetValue(DirectionProperty); }
        set { SetValue(DirectionProperty, value); }
    }

    public static readonly DependencyProperty DirectionProperty =
        DependencyProperty.Register(nameof(Direction), typeof(AbstractMovementDirection), typeof(AbstractMovementBackgroundRenderer), new PropertyMetadata(AbstractMovementDirection.BottomLeft, OnDirectionChanged));

    private static void OnDirectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (AbstractMovementBackgroundRenderer)d;
        ctl.direction = (AbstractMovementDirection)e.NewValue;
    }
}
