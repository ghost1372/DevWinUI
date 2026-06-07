using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class SnowRenderer
{
    private double snowFlakeOverlayAmount = 10.0;
    private double snowFlakeOverlaySpeed = 1.0;

    public double Amount
    {
        get { return (double)GetValue(AmountProperty); }
        set { SetValue(AmountProperty, value); }
    }

    public static readonly DependencyProperty AmountProperty =
        DependencyProperty.Register(nameof(Amount), typeof(double), typeof(SnowRenderer), new PropertyMetadata(10.0, OnAmountChanged));

    private static void OnAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SnowRenderer)d;
        ctl.snowFlakeOverlayAmount = (double)e.NewValue;
    }
    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(SnowRenderer), new PropertyMetadata(1.0, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SnowRenderer)d;
        ctl.snowFlakeOverlaySpeed = (double)e.NewValue;
    }
 
}
