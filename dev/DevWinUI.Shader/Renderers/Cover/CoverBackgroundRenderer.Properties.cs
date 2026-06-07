using Microsoft.UI.Xaml;

namespace DevWinUI;

public partial class CoverBackgroundRenderer
{
    private int coverOverlayOpacity = 100;
    private int coverOverlayBlurAmount = 50;
    private int coverOverlaySpeed = 100;
    private byte[] coverAlbumArtBytes = null;

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
   
    public byte[] AlbumArtBytes
    {
        get { return (byte[])GetValue(AlbumArtBytesProperty); }
        set { SetValue(AlbumArtBytesProperty, value); }
    }

    public static readonly DependencyProperty AlbumArtBytesProperty =
        DependencyProperty.Register(nameof(AlbumArtBytes), typeof(byte[]), typeof(CoverBackgroundRenderer), new PropertyMetadata(null, OnAlbumArtBytesChanged));

    private static void OnAlbumArtBytesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CoverBackgroundRenderer)d;
        ctl.coverAlbumArtBytes = (byte[])e.NewValue;
        _ = ctl.ReloadCoverBackgroundResourcesAsync();
    }
}
