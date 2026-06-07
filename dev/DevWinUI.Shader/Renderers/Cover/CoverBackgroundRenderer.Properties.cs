using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class CoverBackgroundRenderer
{
    private int coverOverlayOpacity = 100;
    private int coverOverlayBlurAmount = 50;
    private int coverOverlaySpeed = 100;
    private string coverImage = null;

    public int Opacity
    {
        get { return (int)GetValue(OpacityProperty); }
        set { SetValue(OpacityProperty, value); }
    }

    public static readonly DependencyProperty OpacityProperty =
        DependencyProperty.Register(nameof(Opacity), typeof(int), typeof(CoverBackgroundRenderer), new PropertyMetadata(100, OnOpacityChanged));

    private static void OnOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CoverBackgroundRenderer)d;
        ctl.coverOverlayOpacity = (int)e.NewValue;
    }
    public int BlurAmount
    {
        get { return (int)GetValue(BlurAmountProperty); }
        set { SetValue(BlurAmountProperty, value); }
    }

    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(int), typeof(CoverBackgroundRenderer), new PropertyMetadata(50, OnBlurAmountChanged));

    private static void OnBlurAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CoverBackgroundRenderer)d;
        var value = (int)e.NewValue;

        if (ctl.coverOverlayBlurAmount != value)
        {
            ctl.coverOverlayBlurAmount = value;
            ctl._needsCacheUpdate = true;
        }
    }
    public int Speed
    {
        get { return (int)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(int), typeof(CoverBackgroundRenderer), new PropertyMetadata(100, OnSpeedChanged));

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CoverBackgroundRenderer)d;
        var value = (int)e.NewValue;
        if (ctl.coverOverlaySpeed != value)
        {
            ctl.coverOverlaySpeed = value;
            ctl._needsCacheUpdate = true;
        }
    }
   
    public string CoverImage
    {
        get { return (string)GetValue(CoverImageProperty); }
        set { SetValue(CoverImageProperty, value); }
    }

    public static readonly DependencyProperty CoverImageProperty =
        DependencyProperty.Register(nameof(CoverImage), typeof(byte[]), typeof(CoverBackgroundRenderer), new PropertyMetadata(null, OnCoverImageChanged));

    private static void OnCoverImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CoverBackgroundRenderer)d;
        if (e.NewValue is string value && ctl.coverImage != value)
        {
            ctl.coverImage = value;
            _ = ctl.LoadCover();
        }
    }
}
