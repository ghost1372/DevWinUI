namespace DevWinUI;

[TemplatePart(Name = "PART_RootGrid", Type = typeof(Grid))]
public partial class FrostedGlass : Control
{
    #region Fields

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private SpriteVisual _glassVisual;
    private IMaskSurface _glassMaskSurface;
    private CompositionBackdropBrush _backdropBrush;
    private DropShadow _glassShadow;
    private Grid _rootGrid;

    #endregion

    #region Dependency Properties

    #region TintColor

    /// <summary>
    /// TintColor Dependency Property
    /// </summary>
    public static readonly DependencyProperty TintColorProperty =
        DependencyProperty.Register(nameof(TintColor), typeof(Color), typeof(FrostedGlass),
            new PropertyMetadata(Colors.White, OnTintColorChanged));

    /// <summary>
    /// Gets or sets the TintColor property. This dependency property 
    /// indicates the tint of the Frosted glass.
    /// </summary>
    public Color TintColor
    {
        get => (Color)GetValue(TintColorProperty);
        set => SetValue(TintColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the TintColor property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnTintColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnTintColorChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the TintColor property.
    /// </summary>
    private void OnTintColorChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region TintOpacity

    /// <summary>
    /// TintOpacity Dependency Property
    /// </summary>
    public static readonly DependencyProperty TintOpacityProperty =
        DependencyProperty.Register(nameof(TintOpacity), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(0.6, OnTintOpacityChanged));

    /// <summary>
    /// Gets or sets the TintOpacity property. This dependency property 
    /// indicates the opacity of the tint color.
    /// </summary>
    public double TintOpacity
    {
        get => (double)GetValue(TintOpacityProperty);
        set => SetValue(TintOpacityProperty, value.Clamp(0.0, 1.0));
    }

    /// <summary>
    /// Handles changes to the TintOpacity property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnTintOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnTintOpacityChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the TintOpacity property.
    /// </summary>
    private void OnTintOpacityChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region MultiplyAmount

    /// <summary>
    /// MultiplyAmount Dependency Property
    /// </summary>
    public static readonly DependencyProperty MultiplyAmountProperty =
        DependencyProperty.Register(nameof(MultiplyAmount), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(1.0, OnMultiplyAmountChanged));

    /// <summary>
    /// Gets or sets the MultiplyAmount property. This dependency property 
    /// indicates how much the multiplication result (Tint * Backdrop) should be included
    /// in the output image. Default value 1.
    /// </summary>
    public double MultiplyAmount
    {
        get => (double)GetValue(MultiplyAmountProperty);
        set => SetValue(MultiplyAmountProperty, value);
    }

    /// <summary>
    /// Handles changes to the MultiplyAmount property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnMultiplyAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnMultiplyAmountChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the MultiplyAmount property.
    /// </summary>
    private void OnMultiplyAmountChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region BlurAmount

    /// <summary>
    /// BlurAmount Dependency Property
    /// </summary>
    public static readonly DependencyProperty BlurAmountProperty =
        DependencyProperty.Register(nameof(BlurAmount), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(15.0, OnBlurAmountChanged));

    /// <summary>
    /// Gets or sets the BlurAmount property. This dependency property 
    /// indicates the amount of blurring that should be done. Default value is 15.
    /// </summary>`
    public double BlurAmount
    {
        get => (double)GetValue(BlurAmountProperty);
        set => SetValue(BlurAmountProperty, value);
    }

    /// <summary>
    /// Handles changes to the BlurAmount property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnBlurAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnBlurAmountChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the BlurAmount property.
    /// </summary>
    private void OnBlurAmountChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region BackdropAmount

    /// <summary>
    /// BackdropAmount Dependency Property
    /// </summary>
    public static readonly DependencyProperty BackdropAmountProperty =
        DependencyProperty.Register(nameof(BackdropAmount), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(0.75, OnBackdropAmountChanged));

    /// <summary>
    /// Gets or sets the BackdropAmount property. This dependency property 
    /// indicates the amount of backdrop brush that should be present in the 
    /// Frosted Glass. Value should be within 0 and 1 (inclusive).
    /// </summary>
    public double BackdropAmount
    {
        get => (double)GetValue(BackdropAmountProperty);
        set => SetValue(BackdropAmountProperty, value);
    }

    /// <summary>
    /// Handles changes to the BackdropAmount property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnBackdropAmountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (FrostedGlass)d;
        target.OnBackdropAmountChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the BackdropAmount property.
    /// </summary>
    private void OnBackdropAmountChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region DisplayShadow

    /// <summary>
    /// DisplayShadow Dependency Property
    /// </summary>
    public static readonly DependencyProperty DisplayShadowProperty =
        DependencyProperty.Register(nameof(DisplayShadow), typeof(bool), typeof(FrostedGlass),
            new PropertyMetadata(true, OnDisplayShadowChanged));

    /// <summary>
    /// Gets or sets the DisplayShadow property. This dependency property 
    /// indicates whether the shadow of the FrostedGlass should be displayed.
    /// </summary>
    public bool DisplayShadow
    {
        get => (bool)GetValue(DisplayShadowProperty);
        set => SetValue(DisplayShadowProperty, value);
    }

    /// <summary>
    /// Handles changes to the DisplayShadow property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnDisplayShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnDisplayShadowChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the DisplayShadow property.
    /// </summary>
    private void OnDisplayShadowChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ShadowColor

    /// <summary>
    /// ShadowColor Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(FrostedGlass),
            new PropertyMetadata(Colors.Black, OnShadowColorChanged));

    /// <summary>
    /// Gets or sets the ShadowColor property. This dependency property 
    /// indicates the color of the shadow.
    /// </summary>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowColor property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnShadowColorChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowColor property.
    /// </summary>
    private void OnShadowColorChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ShadowOpacity

    /// <summary>
    /// ShadowOpacity Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(0.5, OnShadowOpacityChanged));

    /// <summary>
    /// Gets or sets the ShadowOpacity property. This dependency property 
    /// indicates the opacity of the shadow.
    /// </summary>
    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, value.Clamp(0.0f, 1.0f));
    }

    /// <summary>
    /// Handles changes to the ShadowOpacity property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnShadowOpacityChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOpacity property.
    /// </summary>
    private void OnShadowOpacityChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ShadowBlurRadius

    /// <summary>
    /// ShadowBlurRadius Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowBlurRadiusProperty =
        DependencyProperty.Register(nameof(ShadowBlurRadius), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(16.0, OnShadowBlurRadiusChanged));

    /// <summary>
    /// Gets or sets the ShadowBlurRadius property. This dependency property 
    /// indicates the Blur Radius of the FrostedGlass shadow.
    /// </summary>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowBlurRadius property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnShadowBlurRadiusChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowBlurRadius property.
    /// </summary>
    private void OnShadowBlurRadiusChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ShadowOffsetX

    /// <summary>
    /// ShadowOffsetX Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetXProperty =
        DependencyProperty.Register(nameof(ShadowOffsetX), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(4.0, OnShadowOffsetXChanged));

    /// <summary>
    /// Gets or sets the ShadowOffsetX property. This dependency property 
    /// indicates the offset of the FrostedGlass shadow on the x-axis.
    /// </summary>
    public double ShadowOffsetX
    {
        get => (double)GetValue(ShadowOffsetXProperty);
        set => SetValue(ShadowOffsetXProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetX property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (FrostedGlass)d;
        target.OnShadowOffsetXChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOffsetX property.
    /// </summary>
    private void OnShadowOffsetXChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ShadowOffsetY

    /// <summary>
    /// ShadowOffsetY Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetYProperty =
        DependencyProperty.Register(nameof(ShadowOffsetY), typeof(double), typeof(FrostedGlass),
            new PropertyMetadata(4.0, OnShadowOffsetYChanged));

    /// <summary>
    /// Gets or sets the ShadowOffsetY property. This dependency property 
    /// indicates the offset of the FrostedGlass shadow on the y-axis.
    /// </summary>
    public double ShadowOffsetY
    {
        get => (double)GetValue(ShadowOffsetYProperty);
        set => SetValue(ShadowOffsetYProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetY property.
    /// </summary>
    /// <param name="d">FrostedGlass</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var glass = (FrostedGlass)d;
        glass.OnShadowOffsetYChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOffsetY property.
    /// </summary>
    private void OnShadowOffsetYChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public FrostedGlass()
    {
        // Set the style key
        DefaultStyleKey = typeof(FrostedGlass);

        // Handle the CornerRadius Change
        RegisterPropertyChangedCallback(ImageFrame.CornerRadiusProperty, OnCornerRadiusChanged);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the Template is first applied to the control
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Get the Root Grid within which the composition elements will be hosted
        _rootGrid = GetTemplateChild("PART_RootGrid") as Grid;
    }

    /// <summary>
    /// Handles the Measure pass during Layout
    /// </summary>
    /// <param name="availableSize">Available size</param>
    /// <returns>Total Size required to accommodate all the Children</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Initialize Composition elements for the the very first time
        if (_compositor == null)
        {
            _compositor = CompositionTarget.GetCompositorForCurrentThread();
            _generator = _compositor.CreateCompositionGenerator();
            _glassVisual = _compositor.CreateSpriteVisual();
            _glassMaskSurface = _generator.CreateMaskSurface(Size.Empty, null);
            _backdropBrush = _compositor.CreateBackdropBrush();
            _glassShadow = _compositor.CreateDropShadow();

            if (_rootGrid != null)
            {
                ElementCompositionPreview.SetElementChildVisual(_rootGrid, _glassVisual);
            }

        }

        return base.MeasureOverride(availableSize);
    }

    /// <summary>
    /// Handles the Arrange pass during Layout
    /// </summary>
    /// <param name="finalSize">Final Size of the control</param>
    /// <returns>Total size occupied by all the Children</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Taking into account the BorderThickness
        var borders = BorderThickness;
        var corners = CornerRadius;
        var borderSize = borders.CollapseThickness();

        // Calculate the Offset for the glassVisual
        var left = (float)borders.Left;
        var top = (float)borders.Top;

        // Calculate the Dimensions of the glassVisual
        var width = (float)Math.Max(0, finalSize.Width - borderSize.Width);
        var height = (float)Math.Max(0, finalSize.Height - borderSize.Height);
        _glassVisual.Size = new Vector2(width, height);
        _glassVisual.Offset = new Vector3(left, top, 0f);

        var rect = new CanvasRoundRect(Vector2.Zero, _glassVisual.Size, corners.ToVector4(), borders.ToVector4(), Vector4.Zero, false);

        using (var pathBuilder = new CanvasPathBuilder(_generator.Device))
        {
            pathBuilder.AddRoundedRectangleFigure(rect);
            using (var geometry = CanvasGeometry.CreatePath(pathBuilder))
            {
                _glassMaskSurface.Redraw(_glassVisual.Size.ToSize(), geometry);
                var frostedBrush = _compositor.CreateFrostedGlassBrush(_glassMaskSurface,
                                                                       TintColor,
                                                                       (float)BlurAmount,
                                                                       _backdropBrush,
                                                                       (float)MultiplyAmount,
                                                                       (float)TintOpacity,
                                                                       (float)BackdropAmount);

                // Update the glass shadow's mask
                _glassShadow.Mask = frostedBrush.GetSourceParameter("mask");
                _glassVisual.Brush = frostedBrush;
            }

            if (DisplayShadow)
            {
                _glassShadow.Color = ShadowColor;
                _glassShadow.Opacity = (float)ShadowOpacity;
                _glassShadow.Offset = new Vector3((float)ShadowOffsetX, (float)ShadowOffsetY, 0f);
                _glassShadow.BlurRadius = (float)ShadowBlurRadius;
                _glassVisual.Shadow = _glassShadow;
            }
            else
            {
                _glassVisual.Shadow = null;
            }
        }

        return finalSize;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the CornerRadius property.
    /// </summary>
    private void OnCornerRadiusChanged(DependencyObject d, DependencyProperty dp)
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion
}
