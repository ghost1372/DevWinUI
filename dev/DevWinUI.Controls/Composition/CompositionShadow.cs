using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
[TemplatePart(Name = nameof(PART_ShadowElement), Type = typeof(Border))]
public partial class CompositionShadow : Control
{
    private const string PART_ShadowElement = "PART_ShadowElement";
    private Border _shadowElement;

    private DropShadow _dropShadow;
    private SpriteVisual _shadowVisual;

    public CompositionShadow()
    {
        this.DefaultStyleKey = typeof(CompositionShadow);
        this.SizeChanged += CompositionShadow_SizeChanged;
        this.Loaded += CompositionShadow_Loaded;
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        _shadowVisual = compositor.CreateSpriteVisual();
        _dropShadow = compositor.CreateDropShadow();
        _shadowVisual.Shadow = _dropShadow;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _shadowElement = GetTemplateChild(PART_ShadowElement) as Border;

        if (_shadowElement != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_shadowElement, _shadowVisual);
        }

        ConfigureShadowVisualForContent();
    }

    private void CompositionShadow_Loaded(object sender, RoutedEventArgs e)
    {
        ConfigureShadowVisualForContent();
    }

    public static readonly DependencyProperty BlurRadiusProperty =
        DependencyProperty.Register(nameof(BlurRadius), typeof(double), typeof(CompositionShadow), new PropertyMetadata(9.0, OnBlurRadiusChanged));

    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register(nameof(Color), typeof(Color), typeof(CompositionShadow), new PropertyMetadata(Colors.Black, OnColorChanged));

    public static readonly DependencyProperty OffsetXProperty =
        DependencyProperty.Register(nameof(OffsetX), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetXChanged));

    public static readonly DependencyProperty OffsetYProperty =
        DependencyProperty.Register(nameof(OffsetY), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetYChanged));

    public static readonly DependencyProperty OffsetZProperty =
        DependencyProperty.Register(nameof(OffsetZ), typeof(double), typeof(CompositionShadow), new PropertyMetadata(0.0, OnOffsetZChanged));

    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(CompositionShadow), new PropertyMetadata(1.0, OnShadowOpacityChanged));

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(CompositionShadow),
            new PropertyMetadata(null, OnContentChanged));

    public double BlurRadius
    {
        get => (double)GetValue(BlurRadiusProperty);
        set => SetValue(BlurRadiusProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    public double OffsetX
    {
        get => (double)GetValue(OffsetXProperty);
        set => SetValue(OffsetXProperty, value);
    }

    public double OffsetY
    {
        get => (double)GetValue(OffsetYProperty);
        set => SetValue(OffsetYProperty, value);
    }

    public double OffsetZ
    {
        get => (double)GetValue(OffsetZProperty);
        set => SetValue(OffsetZProperty, value);
    }

    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, value);
    }

    /// <summary>
    /// The element used as the mask for the shadow.
    /// </summary>
    public FrameworkElement Content
    {
        get => (FrameworkElement)GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    /// <summary>
    /// Expose underlying DropShadow for animations etc.
    /// </summary>
    public DropShadow DropShadow => _dropShadow;

    /// <summary>
    /// Expose underlying SpriteVisual.
    /// </summary>
    public SpriteVisual Visual => _shadowVisual;

    /// <summary>
    /// CompositionBrush mask of the DropShadow.
    /// </summary>
    public CompositionBrush Mask
    {
        get => _dropShadow?.Mask;
        set
        {
            if (_dropShadow != null)
            {
                _dropShadow.Mask = value;
            }
        }
    }
    private static void OnBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.BlurRadius = (float)(double)e.NewValue;
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.Color = (Color)e.NewValue;
    }

    private static void OnOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset((float)(double)e.NewValue, control._dropShadow.Offset.Y, control._dropShadow.Offset.Z);
    }

    private static void OnOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset(control._dropShadow.Offset.X, (float)(double)e.NewValue, control._dropShadow.Offset.Z);
    }

    private static void OnOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control.UpdateShadowOffset(control._dropShadow.Offset.X, control._dropShadow.Offset.Y, (float)(double)e.NewValue);
    }

    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (CompositionShadow)d;
        control._dropShadow.Opacity = (float)(double)e.NewValue;
    }

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

    private void OnContentSizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateShadowSize();
    }

    private void CompositionShadow_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateShadowSize();
    }

    private void ConfigureShadowVisualForContent()
    {
        UpdateShadowMask();
        UpdateShadowSize();

        // Update other properties to reflect current DependencyProperty values
        if (_dropShadow != null)
        {
            _dropShadow.BlurRadius = (float)BlurRadius;
            _dropShadow.Color = Color;
            _dropShadow.Opacity = (float)ShadowOpacity;
            UpdateShadowOffset((float)OffsetX, (float)OffsetY, (float)OffsetZ);
        }
    }

    private void UpdateShadowMask()
    {
        if (_dropShadow == null)
            return;

        if (Content != null)
        {
            CompositionBrush mask = null;
            if (Content is Image image)
            {
                mask = image.GetAlphaMask();
            }
            else if (Content is Shape shape)
            {
                mask = shape.GetAlphaMask();
            }
            else if (Content is TextBlock textBlock)
            {
                mask = textBlock.GetAlphaMask();
            }
            _dropShadow.Mask = mask;
        }
        else
        {
            _dropShadow.Mask = null;
        }
    }

    private void UpdateShadowOffset(float x, float y, float z)
    {
        if (_dropShadow != null)
        {
            _dropShadow.Offset = new Vector3(x, y, z);
        }
    }

    private void UpdateShadowSize()
    {
        if (_shadowVisual == null)
            return;

        Vector2 newSize;
        if (Content != null)
        {
            newSize = new Vector2((float)Content.ActualWidth, (float)Content.ActualHeight);
        }
        else
        {
            newSize = new Vector2((float)ActualWidth, (float)ActualHeight);
        }

        _shadowVisual.Size = newSize;
    }
}
