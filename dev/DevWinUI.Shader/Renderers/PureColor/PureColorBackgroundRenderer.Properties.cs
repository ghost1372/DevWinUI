using Microsoft.UI;
using Microsoft.UI.Xaml;
using Windows.UI;

namespace DevWinUI;

public partial class PureColorBackgroundRenderer
{
    private int pureColorOverlayOpacity = 100;
    private Color overlayColor = Colors.Transparent;

    public int Opacity
    {
        get { return (int)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register(nameof(Opacity), typeof(int), typeof(PureColorBackgroundRenderer), new PropertyMetadata(100, OnOpacityChanged));

    private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PureColorBackgroundRenderer)d;
        ctl.pureColorOverlayOpacity = (int)e.NewValue;
    }

    public Color Color
    {
        get { return (Color)GetValue(ColorProperty); }
        set { SetValue(ColorProperty, value); }
    }

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(PureColorBackgroundRenderer), new PropertyMetadata(Colors.Transparent, OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (PureColorBackgroundRenderer)d;
        ctl.overlayColor = (Color)e.NewValue;
    }
}
