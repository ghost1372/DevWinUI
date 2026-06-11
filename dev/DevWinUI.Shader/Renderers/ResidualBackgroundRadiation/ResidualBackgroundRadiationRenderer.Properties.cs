using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class ResidualBackgroundRadiationRenderer
{
    private float mouseX;
    private float mouseY;
    public double MouseX
    {
        get { return (double)GetValue(MouseXProperty); }
        set { SetValue(MouseXProperty, value); }
    }

    public static readonly DependencyProperty MouseXProperty =
        DependencyProperty.Register(nameof(MouseX), typeof(double), typeof(ResidualBackgroundRadiationRenderer), new PropertyMetadata(0.0d, OnMouseXChanged));

    private static void OnMouseXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ResidualBackgroundRadiationRenderer)d;
        ctl.mouseX = (float)(double)e.NewValue;
    }
    public double MouseY
    {
        get { return (double)GetValue(MouseYProperty); }
        set { SetValue(MouseYProperty, value); }
    }

    public static readonly DependencyProperty MouseYProperty =
        DependencyProperty.Register(nameof(MouseY), typeof(double), typeof(ResidualBackgroundRadiationRenderer), new PropertyMetadata(0.0d, OnMouseYChanged));

    private static void OnMouseYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ResidualBackgroundRadiationRenderer)d;
        ctl.mouseY = (float)(double)e.NewValue;
    }
}
