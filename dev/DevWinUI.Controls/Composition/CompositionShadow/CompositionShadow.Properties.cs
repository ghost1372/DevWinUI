namespace DevWinUI;

public partial class CompositionShadow
{
    public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }
    public static readonly DependencyProperty BlurRadiusProperty =
        DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(CompositionShadow), new PropertyMetadata(9.0, OnBlurRadiusChanged));
    private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.BlurRadius = (float)(double)e.NewValue;
    }
    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(CompositionShadow), new PropertyMetadata(Colors.Black, OnColorChanged));
    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.Color = (Color)e.NewValue;
    }
    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }
    public static readonly DependencyProperty OffsetXProperty =
        DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetXChanged));
    private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset((float)(double)e.NewValue, control._dropShadow.Offset.Y, control._dropShadow.Offset.Z);
    }
    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }
    public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetYChanged));
    private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset(control._dropShadow.Offset.X, (float)(double)e.NewValue, control._dropShadow.Offset.Z);
    }
    public double OffsetZ
    {
        get => (double)GetValue(OffsetZProperty);
        set => SetValue(OffsetZProperty, value);
    }
    public static readonly DependencyProperty OffsetZProperty =
        DependencyProperty.Register(nameof(OffsetZ), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetZChanged));
    private static void OnOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset(control._dropShadow.Offset.X, control._dropShadow.Offset.Y, (float)(double)e.NewValue);
    }
    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, value);
    }
    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(CompositionShadow), new PropertyMetadata(1.0, OnShadowOpacityChanged));
    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.Opacity = (float)(double)e.NewValue;
    }
    public FrameworkElement Content
    {
        get => (FrameworkElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }
    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(CompositionShadow), new PropertyMetadata(null, OnContentChanged));
    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        if (e.OldValue is FrameworkElement oldElement)
        {
            oldElement.SizeChanged -= control.OnContentSizeChanged;
        }
        if (e.NewValue is FrameworkElement newElement)
        {
            newElement.SizeChanged += control.OnContentSizeChanged;
        }
        control.ConfigureShadowVisualForContent();
    }

    public bool UseEdgeColorFromImage
    {
        get => (bool)GetValue(UseEdgeColorFromImageProperty);
        set => SetValue(UseEdgeColorFromImageProperty, value);
    }

    public static readonly DependencyProperty UseEdgeColorFromImageProperty =
        DependencyProperty.Register(nameof(UseEdgeColorFromImage), typeof(bool), typeof(CompositionShadow), new PropertyMetadata(false, OnUseEdgeColorFromImageChanged));
    private static async void OnUseEdgeColorFromImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CompositionShadow)d;
        if (ctl != null)
        {
            await ctl.UpdateColorFromImageAsync();
        }
    }

    public object ImageSourceForEdgeColor
    {
        get => (object)GetValue(ImageSourceForEdgeColorProperty);
        set => SetValue(ImageSourceForEdgeColorProperty, value);
    }

    public static readonly DependencyProperty ImageSourceForEdgeColorProperty =
        DependencyProperty.Register(nameof(ImageSourceForEdgeColor), typeof(object), typeof(CompositionShadow), new PropertyMetadata(null, OnImageSourceForEdgeColorChanged));
    private static async void OnImageSourceForEdgeColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (CompositionShadow)d;
        if (ctl != null)
        {
            await ctl.UpdateColorFromImageAsync();
        }
    }
}
