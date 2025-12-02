namespace DevWinUI;

public partial class ColorShadow
{
    public static readonly DependencyProperty ColorShadowBlurRadiusProperty =
        DependencyProperty.Register(nameof(ColorShadowBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowBlurRadius, OnColorShadowBlurRadiusChanged));

    public double ColorShadowBlurRadius
    {
        get => (double)GetValue(ColorShadowBlurRadiusProperty);
        set => SetValue(ColorShadowBlurRadiusProperty, value);
    }

    private static void OnColorShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowBlurRadiusChanged();
    }

    private void OnColorShadowBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ColorShadowOpacityProperty =
        DependencyProperty.Register(nameof(ColorShadowOpacity), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowOpacity, OnColorShadowOpacityChanged));

    public double ColorShadowOpacity
    {
        get => (double)GetValue(ColorShadowOpacityProperty);
        set => SetValue(ColorShadowOpacityProperty, value);
    }

    private static void OnColorShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowOpacityChanged();
    }

    private void OnColorShadowOpacityChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ColorShadowPaddingProperty =
        DependencyProperty.Register(nameof(ColorShadowPadding), typeof(Thickness), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowPadding, OnColorShadowPaddingChanged));

    public Thickness ColorShadowPadding
    {
        get => (Thickness)GetValue(ColorShadowPaddingProperty);
        set => SetValue(ColorShadowPaddingProperty, value);
    }

    private static void OnColorShadowPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowPaddingChanged();
    }

    private void OnColorShadowPaddingChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ImageUriProperty =
        DependencyProperty.Register(nameof(ImageUri), typeof(object), typeof(ColorShadow),
            new PropertyMetadata(null, OnImageUriChanged));

    public object ImageUri
    {
        get => (object)GetValue(ImageUriProperty);
        set => SetValue(ImageUriProperty, value);
    }

    private static async void OnImageUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ColorShadow)d;
        Uri uri = null;

        switch (e.NewValue)
        {
            case Uri u:
                uri = u;
                break;

            case string s when Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var uriFromString):
                uri = uriFromString;
                break;
        }
        await target.OnImageUriChangedAsync(uri);
    }

    public static readonly DependencyProperty IsShadowEnabledProperty =
        DependencyProperty.Register(nameof(IsShadowEnabled), typeof(bool), typeof(ColorShadow),
            new PropertyMetadata(false, OnIsShadowEnabledChanged));

    public bool IsShadowEnabled
    {
        get => (bool)GetValue(IsShadowEnabledProperty);
        set => SetValue(IsShadowEnabledProperty, value);
    }

    private static void OnIsShadowEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ColorShadow)d;
        target.OnIsShadowEnabledChanged();
    }

    private void OnIsShadowEnabledChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ColorMaskBlurRadiusProperty =
        DependencyProperty.Register(nameof(ColorMaskBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultMaskBlurRadius, OnColorMaskBlurRadiusChanged));

    public double ColorMaskBlurRadius
    {
        get => (double)GetValue(ColorMaskBlurRadiusProperty);
        set => SetValue(ColorMaskBlurRadiusProperty, value);
    }

    private static void OnColorMaskBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorMaskBlurRadiusChanged();
    }

    private void OnColorMaskBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ColorMaskPaddingProperty =
        DependencyProperty.Register(nameof(ColorMaskPadding), typeof(Thickness), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorMaskPadding, OnColorMaskPaddingChanged));

    public Thickness ColorMaskPadding
    {
        get => (Thickness)GetValue(ColorMaskPaddingProperty);
        set => SetValue(ColorMaskPaddingProperty, value);
    }

    private static void OnColorMaskPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorMaskPaddingChanged();
    }

    private void OnColorMaskPaddingChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowBlurRadiusProperty =
        DependencyProperty.Register(nameof(ShadowBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowBlurRadius, OnShadowBlurRadiusChanged));

    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    private static void OnShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowBlurRadiusChanged();
    }

    private void OnShadowBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowColor, OnShadowColorChanged));

    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    private static void OnShadowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowColorChanged();
    }

    private void OnShadowColorChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowOffsetXProperty =
        DependencyProperty.Register(nameof(ShadowOffsetX), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetX, OnShadowOffsetXChanged));

    public double ShadowOffsetX
    {
        get => (double)GetValue(ShadowOffsetXProperty);
        set => SetValue(ShadowOffsetXProperty, value);
    }

    private static void OnShadowOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetXChanged();
    }

    private void OnShadowOffsetXChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowOffsetYProperty =
        DependencyProperty.Register(nameof(ShadowOffsetY), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetY, OnShadowOffsetYChanged));

    public double ShadowOffsetY
    {
        get => (double)GetValue(ShadowOffsetYProperty);
        set => SetValue(ShadowOffsetYProperty, value);
    }

    private static void OnShadowOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetYChanged();
    }

    private void OnShadowOffsetYChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowOffsetZProperty =
        DependencyProperty.Register(nameof(ShadowOffsetZ), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetZ, OnShadowOffsetZChanged));

    public double ShadowOffsetZ
    {
        get => (double)GetValue(ShadowOffsetZProperty);
        set => SetValue(ShadowOffsetZProperty, value);
    }

    private static void OnShadowOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetZChanged();
    }

    private void OnShadowOffsetZChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOpacity, OnShadowOpacityChanged));

    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, value);
    }

    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOpacityChanged();
    }

    private void OnShadowOpacityChanged()
    {
        InvalidateArrange();
    }

    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ColorShadow),
            new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnStretchChanged();
    }

    private void OnStretchChanged()
    {
        InvalidateArrange();
    }

    public bool IsRounded
    {
        get { return (bool)GetValue(IsRoundedProperty); }
        set { SetValue(IsRoundedProperty, value); }
    }

    public static readonly DependencyProperty IsRoundedProperty =
        DependencyProperty.Register(nameof(IsRounded), typeof(bool), typeof(ColorShadow), new PropertyMetadata(false, OnIsRoundedChanged));

    private static void OnIsRoundedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorShadow)d;
        if (ctl != null)
        {
            ctl.OnIsRoundedChanged();
        }
    }

    private void OnIsRoundedChanged()
    {
        forceUpdateMask = true;
        InvalidateArrange();
    }

    public Uri Mask
    {
        get { return (Uri)GetValue(MaskProperty); }
        set { SetValue(MaskProperty, value); }
    }

    public static readonly DependencyProperty MaskProperty =
        DependencyProperty.Register(nameof(Mask), typeof(Uri), typeof(ColorShadow), new PropertyMetadata(null, OnMaskChanged));

    private static void OnMaskChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorShadow)d;
        if (ctl != null)
        {
            ctl.OnMaskChanged();
        }
    }

    private void OnMaskChanged()
    {
        InvalidateArrange();
    }

    public CompositionStretch ImageStretch
    {
        get { return (CompositionStretch)GetValue(ImageStretchProperty); }
        set { SetValue(ImageStretchProperty, value); }
    }

    public static readonly DependencyProperty ImageStretchProperty =
        DependencyProperty.Register(nameof(ImageStretch), typeof(CompositionStretch), typeof(ColorShadow), new PropertyMetadata(CompositionStretch.Fill, OnStretchChanged));

    public double ImageLayoutOffsetX
    {
        get { return (double)GetValue(ImageLayoutOffsetXProperty); }
        set { SetValue(ImageLayoutOffsetXProperty, value); }
    }

    public static readonly DependencyProperty ImageLayoutOffsetXProperty =
        DependencyProperty.Register(nameof(ImageLayoutOffsetX), typeof(double), typeof(ColorShadow), new PropertyMetadata(0d, OnOffsetChanged));

    public double ImageLayoutOffsetY
    {
        get { return (double)GetValue(ImageLayoutOffsetYProperty); }
        set { SetValue(ImageLayoutOffsetYProperty, value); }
    }

    public static readonly DependencyProperty ImageLayoutOffsetYProperty =
        DependencyProperty.Register(nameof(ImageLayoutOffsetY), typeof(double), typeof(ColorShadow), new PropertyMetadata(0d, OnOffsetChanged));

    public double ImageLayoutOffsetZ
    {
        get { return (double)GetValue(ImageLayoutOffsetZProperty); }
        set { SetValue(ImageLayoutOffsetZProperty, value); }
    }

    public static readonly DependencyProperty ImageLayoutOffsetZProperty =
        DependencyProperty.Register(nameof(ImageLayoutOffsetZ), typeof(double), typeof(ColorShadow), new PropertyMetadata(0d, OnOffsetChanged));

    public double ImageOffsetX
    {
        get { return (double)GetValue(ImageOffsetXProperty); }
        set { SetValue(ImageOffsetXProperty, value); }
    }

    public static readonly DependencyProperty ImageOffsetXProperty =
        DependencyProperty.Register(nameof(ImageOffsetX), typeof(double), typeof(ColorShadow), new PropertyMetadata(0d, OnOffsetChanged));

    public double ImageOffsetY
    {
        get { return (double)GetValue(ImageOffsetYProperty); }
        set { SetValue(ImageOffsetYProperty, value); }
    }

    public static readonly DependencyProperty ImageOffsetYProperty =
        DependencyProperty.Register(nameof(ImageOffsetY), typeof(double), typeof(ColorShadow), new PropertyMetadata(0d, OnOffsetChanged));

    private static void OnOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ColorShadow)d;
        if (ctl != null)
        {
            ctl.OnOffsetChanged();
        }
    }

    private void OnOffsetChanged()
    {
        InvalidateArrange();
    }
}
