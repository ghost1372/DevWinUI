using Microsoft.UI.Xaml.Controls.Primitives;

namespace DevWinUI;
public partial class DropdownColorPicker
{
    public FlyoutPlacementMode FlyoutPlacement
    {
        get { return (FlyoutPlacementMode)GetValue(FlyoutPlacementProperty); }
        set { SetValue(FlyoutPlacementProperty, value); }
    }

    public static readonly DependencyProperty FlyoutPlacementProperty =
        DependencyProperty.Register(nameof(FlyoutPlacement), typeof(FlyoutPlacementMode), typeof(DropdownColorPicker), new PropertyMetadata(FlyoutPlacementMode.Top, OnFlyoutOptionsChanged));

    private static void OnFlyoutOptionsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DropdownColorPicker)d;
        if (ctl != null)
        {
            ctl.UpdateFlyoutOptions();
        }
    }

    public FlyoutShowMode FlyoutShowMode
    {
        get { return (FlyoutShowMode)GetValue(FlyoutShowModeProperty); }
        set { SetValue(FlyoutShowModeProperty, value); }
    }

    public static readonly DependencyProperty FlyoutShowModeProperty =
        DependencyProperty.Register(nameof(FlyoutShowMode), typeof(FlyoutShowMode), typeof(DropdownColorPicker), new PropertyMetadata(FlyoutShowMode.Auto, OnFlyoutOptionsChanged));

    public bool FlyoutShouldConstrainToRootBounds
    {
        get { return (bool)GetValue(FlyoutShouldConstrainToRootBoundsProperty); }
        set { SetValue(FlyoutShouldConstrainToRootBoundsProperty, value); }
    }

    public static readonly DependencyProperty FlyoutShouldConstrainToRootBoundsProperty =
        DependencyProperty.Register(nameof(FlyoutShouldConstrainToRootBounds), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true, OnFlyoutOptionsChanged));

    public LightDismissOverlayMode FlyoutLightDismissOverlayMode
    {
        get { return (LightDismissOverlayMode)GetValue(FlyoutLightDismissOverlayModeProperty); }
        set { SetValue(FlyoutLightDismissOverlayModeProperty, value); }
    }

    public static readonly DependencyProperty FlyoutLightDismissOverlayModeProperty =
        DependencyProperty.Register(nameof(FlyoutLightDismissOverlayMode), typeof(LightDismissOverlayMode), typeof(DropdownColorPicker), new PropertyMetadata(LightDismissOverlayMode.Auto, OnFlyoutOptionsChanged));


    public ColorPalette ColorPalette
    {
        get { return (ColorPalette)GetValue(ColorPaletteProperty); }
        set { SetValue(ColorPaletteProperty, value); }
    }

    public static readonly DependencyProperty ColorPaletteProperty =
        DependencyProperty.Register(nameof(ColorPalette), typeof(ColorPalette), typeof(DropdownColorPicker), new PropertyMetadata(null, OnColorPaletteChanged));

    private static void OnColorPaletteChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DropdownColorPicker)d;
        if (ctl != null)
        {
            if (e.OldValue is ColorPalette oldPalette)
            {
                oldPalette.ColorChanged -= ctl.OnColorPaletteColorChanged;
            }

            if (e.NewValue is ColorPalette newPalette)
            {
                newPalette.ColorChanged += ctl.OnColorPaletteColorChanged;
            }

            ctl.UpdateColorPalette();
        }
    }

    public bool IsColorCodeVisible
    {
        get { return (bool)GetValue(IsColorCodeVisibleProperty); }
        set { SetValue(IsColorCodeVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsColorCodeVisibleProperty =
        DependencyProperty.Register(nameof(IsColorCodeVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(false));

    public Color Color
    {
        get { return (Color)GetValue(MyPropertyProperty); }
        set { SetValue(MyPropertyProperty, value); }
    }

    public static readonly DependencyProperty MyPropertyProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(DropdownColorPicker), new PropertyMetadata(ColorHelper.GetColorFromHex("#FFFFFF"), OnColorChanged));

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (DropdownColorPicker)d;
        if (ctl != null)
        {
            ctl.OnColorChanged();
        }
    }

    public bool IsMoreButtonVisible
    {
        get { return (bool)GetValue(IsMoreButtonVisibleProperty); }
        set { SetValue(IsMoreButtonVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsMoreButtonVisibleProperty =
        DependencyProperty.Register(nameof(IsMoreButtonVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsColorSliderVisible
    {
        get { return (bool)GetValue(IsColorSliderVisibleProperty); }
        set { SetValue(IsColorSliderVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsColorSliderVisibleProperty =
        DependencyProperty.Register(nameof(IsColorSliderVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsColorChannelTextInputVisible
    {
        get { return (bool)GetValue(IsColorChannelTextInputVisibleProperty); }
        set { SetValue(IsColorChannelTextInputVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsColorChannelTextInputVisibleProperty =
        DependencyProperty.Register(nameof(IsColorChannelTextInputVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsHexInputVisible
    {
        get { return (bool)GetValue(IsHexInputVisibleProperty); }
        set { SetValue(IsHexInputVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsHexInputVisibleProperty =
        DependencyProperty.Register(nameof(IsHexInputVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsAlphaEnabled
    {
        get { return (bool)GetValue(IsAlphaEnabledProperty); }
        set { SetValue(IsAlphaEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsAlphaEnabledProperty =
        DependencyProperty.Register(nameof(IsAlphaEnabled), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsAlphaSliderVisible
    {
        get { return (bool)GetValue(IsAlphaSliderVisibleProperty); }
        set { SetValue(IsAlphaSliderVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsAlphaSliderVisibleProperty =
        DependencyProperty.Register(nameof(IsAlphaSliderVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsAlphaTextInputVisible
    {
        get { return (bool)GetValue(IsAlphaTextInputVisibleProperty); }
        set { SetValue(IsAlphaTextInputVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsAlphaTextInputVisibleProperty =
        DependencyProperty.Register(nameof(IsAlphaTextInputVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsColorPreviewVisible
    {
        get { return (bool)GetValue(IsColorPreviewVisibleProperty); }
        set { SetValue(IsColorPreviewVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsColorPreviewVisibleProperty =
        DependencyProperty.Register(nameof(IsColorPreviewVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public bool IsColorSpectrumVisible
    {
        get { return (bool)GetValue(IsColorSpectrumVisibleProperty); }
        set { SetValue(IsColorSpectrumVisibleProperty, value); }
    }

    public static readonly DependencyProperty IsColorSpectrumVisibleProperty =
        DependencyProperty.Register(nameof(IsColorSpectrumVisible), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(true));

    public ColorSpectrumShape ColorSpectrumShape
    {
        get { return (ColorSpectrumShape)GetValue(ColorSpectrumShapeProperty); }
        set { SetValue(ColorSpectrumShapeProperty, value); }
    }

    public static readonly DependencyProperty ColorSpectrumShapeProperty =
        DependencyProperty.Register(nameof(ColorSpectrumShape), typeof(ColorSpectrumShape), typeof(DropdownColorPicker), new PropertyMetadata(ColorSpectrumShape.Box));

    public bool IsCopyColorCodeOnSelectEnabled
    {
        get { return (bool)GetValue(IsCopyColorCodeOnSelectEnabledProperty); }
        set { SetValue(IsCopyColorCodeOnSelectEnabledProperty, value); }
    }

    public static readonly DependencyProperty IsCopyColorCodeOnSelectEnabledProperty =
        DependencyProperty.Register(nameof(IsCopyColorCodeOnSelectEnabled), typeof(bool), typeof(DropdownColorPicker), new PropertyMetadata(false));

    public Thickness TintBoxMargin
    {
        get { return (Thickness)GetValue(TintBoxMarginProperty); }
        set { SetValue(TintBoxMarginProperty, value); }
    }

    public static readonly DependencyProperty TintBoxMarginProperty =
        DependencyProperty.Register(nameof(TintBoxMargin), typeof(Thickness), typeof(DropdownColorPicker), new PropertyMetadata(new Thickness()));


    public CornerRadius TintBoxCornerRadius
    {
        get { return (CornerRadius)GetValue(TintBoxCornerRadiusProperty); }
        set { SetValue(TintBoxCornerRadiusProperty, value); }
    }

    public static readonly DependencyProperty TintBoxCornerRadiusProperty =
        DependencyProperty.Register(nameof(TintBoxCornerRadius), typeof(CornerRadius), typeof(DropdownColorPicker), new PropertyMetadata(new CornerRadius()));

    public double TintBoxWidth
    {
        get { return (double)GetValue(TintBoxWidthProperty); }
        set { SetValue(TintBoxWidthProperty, value); }
    }

    public static readonly DependencyProperty TintBoxWidthProperty =
        DependencyProperty.Register(nameof(TintBoxWidth), typeof(double), typeof(DropdownColorPicker), new PropertyMetadata(24.0));

    public double TintBoxHeight
    {
        get { return (double)GetValue(TintBoxHeightProperty); }
        set { SetValue(TintBoxHeightProperty, value); }
    }

    public static readonly DependencyProperty TintBoxHeightProperty =
        DependencyProperty.Register(nameof(TintBoxHeight), typeof(double), typeof(DropdownColorPicker), new PropertyMetadata(24.0));
}
