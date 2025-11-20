using System.Linq.Expressions;

namespace DevWinUI;

[TemplatePart(Name = RenderGrid, Type = typeof(Grid))]
[TemplateVisualState(Name = "Initialized", GroupName = "ContentStates")]
[TemplateVisualState(Name = "Loading", GroupName = "ContentStates")]
[TemplateVisualState(Name = "Loaded", GroupName = "ContentStates")]
public partial class ColorShadow : Control
{
    #region Constants

    private const string RenderGrid = "RenderGrid";

    private const double DefaultColorShadowBlurRadius = 20;
    private static readonly Thickness DefaultColorShadowPadding = new Thickness(35d);
    private static readonly Thickness DefaultColorMaskPadding = new Thickness(25d);
    private const double DefaultColorShadowOpacity = 0.8;
    private const double DefaultMaskBlurRadius = 10;
    private const double DefaultShadowBlurRadius = 16;
    private const double DefaultShadowOffsetX = 10;
    private const double DefaultShadowOffsetY = 10;
    private const double DefaultShadowOffsetZ = 4;
    private const double DefaultShadowOpacity = 0.7f;
    private static readonly Color DefaultShadowColor = Colors.Black;

    #endregion

    #region Fields

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private IImageSurface _imageSurface;
    private ContainerVisual _rootVisual;
    private SpriteVisual _colorShadowVisual;
    private SpriteVisual _imageVisual;
    private CompositionEffectBrush _colorShadowBrush;
    private DropShadow _dropShadow;
    private KeyFrameAnimation<float> _colorShadowBlurAnimation;
    readonly ImageSurfaceOptions _maskOptions = ImageSurfaceOptions.DefaultImageMaskOptions;

    private Grid _renderGrid;
    private bool _imageSourceLoaded;
    private readonly SemaphoreSlim _syncObject = new SemaphoreSlim(1);
    private bool _isLoading;

    #endregion

    #region Dependency Properties

    #region ColorShadowBlurRadius

    /// <summary>
    /// ColorShadowBlurRadius Dependency Property
    /// </summary>
    public static readonly DependencyProperty ColorShadowBlurRadiusProperty =
        DependencyProperty.Register(nameof(ColorShadowBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowBlurRadius, OnColorShadowBlurRadiusChanged));

    /// <summary>
    /// Gets or sets the blur radius of the Gaussian Blur applied to the image in the ColorShadow layer.
    /// </summary>
    public double ColorShadowBlurRadius
    {
        get => (double)GetValue(ColorShadowBlurRadiusProperty);
        set => SetValue(ColorShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// Handles changes to the ColorShadowBlurRadius property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnColorShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowBlurRadiusChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ColorShadowBlurRadius property.
    /// </summary>
    private void OnColorShadowBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ColorShadowOpacity

    /// <summary>
    /// ColorShadowOpacity Dependency Property
    /// </summary>
    public static readonly DependencyProperty ColorShadowOpacityProperty =
        DependencyProperty.Register(nameof(ColorShadowOpacity), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowOpacity, OnColorShadowOpacityChanged));

    /// <summary>
    /// Gets or sets the opacity of the ColorShadow layer.
    /// </summary>
    public double ColorShadowOpacity
    {
        get => (double)GetValue(ColorShadowOpacityProperty);
        set => SetValue(ColorShadowOpacityProperty, value);
    }

    /// <summary>
    /// Handles changes to the ColorShadowOpacity property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnColorShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowOpacityChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ColorShadowOpacity property.
    /// </summary>
    private void OnColorShadowOpacityChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ColorShadowPadding

    /// <summary>
    /// ColorShadowPadding Dependency Property
    /// </summary>
    public static readonly DependencyProperty ColorShadowPaddingProperty =
        DependencyProperty.Register(nameof(ColorShadowPadding), typeof(Thickness), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorShadowPadding, OnColorShadowPaddingChanged));

    /// <summary>
    /// Gets or sets the padding between the color shadow bounds and the image bounds.
    /// </summary>
    public Thickness ColorShadowPadding
    {
        get => (Thickness)GetValue(ColorShadowPaddingProperty);
        set => SetValue(ColorShadowPaddingProperty, value);
    }

    /// <summary>
    /// Handles changes to the ColorShadowPadding property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnColorShadowPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorShadowPaddingChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ColorShadowPadding property.
    /// </summary>
    private void OnColorShadowPaddingChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ImageUri

    /// <summary>
    /// ImageUri Dependency Property
    /// </summary>
    public static readonly DependencyProperty ImageUriProperty =
        DependencyProperty.Register(nameof(ImageUri), typeof(object), typeof(ColorShadow),
            new PropertyMetadata(null, OnImageUriChanged));

    /// <summary>
    /// Gets or sets the Uri of the image to be displayed with its ColorShadow.
    /// </summary>
    public object ImageUri
    {
        get => (object)GetValue(ImageUriProperty);
        set => SetValue(ImageUriProperty, value);
    }

    /// <summary>
    /// Handles changes to the ImageUri property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
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

    /// <summary>
    /// Instance handler for the changes to the ImageUri property.
    /// </summary>
    /// <param name="imageUri">Uri of the image to be loaded</param>
    private async Task<bool> OnImageUriChangedAsync(Uri imageUri)
    {
        return await LoadImageAsync(imageUri);
    }

    #endregion

    #region IsShadowEnabled

    /// <summary>
    /// IsShadowEnabled Dependency Property
    /// </summary>
    public static readonly DependencyProperty IsShadowEnabledProperty =
        DependencyProperty.Register(nameof(IsShadowEnabled), typeof(bool), typeof(ColorShadow),
            new PropertyMetadata(false, OnIsShadowEnabledChanged));

    /// <summary>
    /// Gets or sets whether the image's normal shadow should be displayed.
    /// </summary>
    public bool IsShadowEnabled
    {
        get => (bool)GetValue(IsShadowEnabledProperty);
        set => SetValue(IsShadowEnabledProperty, value);
    }

    /// <summary>
    /// Handles changes to the IsShadowEnabled property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnIsShadowEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ColorShadow)d;
        target.OnIsShadowEnabledChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the IsShadowEnabled property.
    /// </summary>
    private void OnIsShadowEnabledChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ColorMaskBlurRadius

    /// <summary>
    /// ColorMaskBlurRadius Dependency Property
    /// </summary>
    public static readonly DependencyProperty ColorMaskBlurRadiusProperty =
        DependencyProperty.Register(nameof(ColorMaskBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultMaskBlurRadius, OnColorMaskBlurRadiusChanged));

    /// <summary>
    /// Gets or sets the blur radius of the IGaussianMaskSurface used to create the ColorShadow layer.
    /// </summary>
    public double ColorMaskBlurRadius
    {
        get => (double)GetValue(ColorMaskBlurRadiusProperty);
        set => SetValue(ColorMaskBlurRadiusProperty, value);
    }

    /// <summary>
    /// Handles changes to the ColorMaskBlurRadius property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnColorMaskBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorMaskBlurRadiusChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ColorMaskBlurRadius property.
    /// </summary>
    private void OnColorMaskBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ColorMaskPadding

    /// <summary>
    /// ColorMaskPadding Dependency Property
    /// </summary>
    public static readonly DependencyProperty ColorMaskPaddingProperty =
        DependencyProperty.Register(nameof(ColorMaskPadding), typeof(Thickness), typeof(ColorShadow),
            new PropertyMetadata(DefaultColorMaskPadding, OnColorMaskPaddingChanged));

    /// <summary>
    /// Gets or sets the padding between the ColorShadow layer bounds and the mask bounds.
    /// </summary>
    public Thickness ColorMaskPadding
    {
        get => (Thickness)GetValue(ColorMaskPaddingProperty);
        set => SetValue(ColorMaskPaddingProperty, value);
    }

    /// <summary>
    /// Handles changes to the ColorMaskPadding property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnColorMaskPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnColorMaskPaddingChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ColorMaskPadding property.
    /// </summary>
    private void OnColorMaskPaddingChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowBlurRadius

    /// <summary>
    /// ShadowBlurRadius Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowBlurRadiusProperty =
        DependencyProperty.Register(nameof(ShadowBlurRadius), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowBlurRadius, OnShadowBlurRadiusChanged));

    /// <summary>
    /// Gets or sets the blur radius of the normal shadow applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowBlurRadius property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowBlurRadiusChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowBlurRadius property.
    /// </summary>
    private void OnShadowBlurRadiusChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowColor

    /// <summary>
    /// ShadowColor Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowColor, OnShadowColorChanged));

    /// <summary>
    /// Gets or sets the color of the normal shadow applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowColor property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowColorChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowColor property.
    /// </summary>
    private void OnShadowColorChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowOffsetX

    /// <summary>
    /// ShadowOffsetX Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetXProperty =
        DependencyProperty.Register(nameof(ShadowOffsetX), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetX, OnShadowOffsetXChanged));

    /// <summary>
    /// Gets or sets the offset of the normal shadow on x-axis applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public double ShadowOffsetX
    {
        get => (double)GetValue(ShadowOffsetXProperty);
        set => SetValue(ShadowOffsetXProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetX property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetXChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowOffsetX property.
    /// </summary>
    private void OnShadowOffsetXChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowOffsetY

    /// <summary>
    /// ShadowOffsetY Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetYProperty =
        DependencyProperty.Register(nameof(ShadowOffsetY), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetY, OnShadowOffsetYChanged));

    /// <summary>
    /// Gets or sets the offset of the normal shadow on y-axis applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public double ShadowOffsetY
    {
        get => (double)GetValue(ShadowOffsetYProperty);
        set => SetValue(ShadowOffsetYProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetY property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetYChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowOffsetY property.
    /// </summary>
    private void OnShadowOffsetYChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowOffsetZ

    /// <summary>
    /// ShadowOffsetZ Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetZProperty =
        DependencyProperty.Register(nameof(ShadowOffsetZ), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOffsetZ, OnShadowOffsetZChanged));

    /// <summary>
    /// Gets or sets the offset of the normal shadow on z-axis applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public double ShadowOffsetZ
    {
        get => (double)GetValue(ShadowOffsetZProperty);
        set => SetValue(ShadowOffsetZProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetZ property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetZChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOffsetZChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowOffsetZ property.
    /// </summary>
    private void OnShadowOffsetZChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region ShadowOpacity

    /// <summary>
    /// ShadowOpacity Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(ColorShadow),
            new PropertyMetadata(DefaultShadowOpacity, OnShadowOpacityChanged));

    /// <summary>
    /// Gets or sets the opacity of the normal shadow applied to the Image.
    /// NOTE: This property is used only if IsShadowEnabled=True.
    /// </summary>
    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOpacity property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnShadowOpacityChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the ShadowOpacity property.
    /// </summary>
    private void OnShadowOpacityChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region Stretch

    /// <summary>
    /// Stretch Dependency Property
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ColorShadow),
            new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

    private IImageMaskSurface _imageMaskSurface;

    /// <summary>
    /// Gets or sets how the content is resized to fill the allocated spaced.
    /// </summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Handles changes to the Stretch property.
    /// </summary>
    /// <param name="d">ColorShadow</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var colorShadow = (ColorShadow)d;
        colorShadow.OnStretchChanged();
    }

    /// <summary>
    /// Instance handler for the changes to the Stretch property.
    /// </summary>
    private void OnStretchChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// ctor
    /// </summary>
    public ColorShadow()
    {
        // Set the default Style Key
        DefaultStyleKey = typeof(ColorShadow);
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Handler method when the Template is applied to the FluidToggleSwitch
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _renderGrid = GetTemplateChild(RenderGrid) as Grid;
        // If the ImageUri property is already assigned a value and the image is already
        // loading, then go to the Loading state instead of the Initialized state.
        VisualStateManager.GoToState(this, _isLoading ? "Loading" : "Initialized", true);
    }

    /// <summary>
    /// Handled for the Arrange Layout
    /// </summary>
    /// <param name="finalSize">Available size for the control to render itself.</param>
    /// <returns>Size</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);

        if (_compositor == null)
        {
            InitComposition();
        }

        if (_imageSourceLoaded && _imageSurface != null)
        {
            // Taking into account the BorderThickness Padding
            var padding = Padding;
            var paddingSize = padding.CollapseThickness();

            // Calculate the Dimensions of the frameVisual
            var srcWidth = _imageSurface.DecodedPhysicalSize.Width;
            var srcHeight = _imageSurface.DecodedPhysicalSize.Height;
            var destWidth = Math.Max(0, finalSize.Width - paddingSize.Width);
            var destHeight = Math.Max(0, finalSize.Height - paddingSize.Height);

            // Get the size that can fit in the destination size
            var targetRect = Utils.GetOptimumSize(srcWidth,
                                                  srcHeight,
                                                  destWidth,
                                                  destHeight,
                                                  Stretch,
                                                  AlignmentX.Center,
                                                  AlignmentY.Center);
            var targetWidth = targetRect.Width;
            var targetHeight = targetRect.Height;
            var targetSize = new Vector2(targetWidth.ToSingle(), targetHeight.ToSingle());

            // Center Alignment
            var left = targetRect.Left.ToSingle();
            var top = targetRect.Top.ToSingle();

            if (_rootVisual == null)
            {
                _rootVisual = _compositor.CreateContainerVisual();
            }

            _rootVisual.Size = targetSize;
            _rootVisual.Offset = new Vector3(left, top, 0f);

            // Image Layer
            if (_imageVisual == null)
            {
                _imageVisual = _compositor.CreateSpriteVisual();
            }

            _imageVisual.Size = targetSize;

            var imageBrush = _compositor.CreateSurfaceBrush(_imageSurface);
            imageBrush.Stretch = CompositionStretch.Fill;
            _imageVisual.Brush = imageBrush;

            // Color Shadow Layer
            if (_colorShadowVisual == null)
            {
                _colorShadowVisual = _compositor.CreateSpriteVisual();
            }

            var colorPaddingSize = ColorShadowPadding.CollapseThickness();
            _colorShadowVisual.Size = new Vector2(targetSize.X + colorPaddingSize.Width.ToSingle(), targetSize.Y + colorPaddingSize.Height.ToSingle());
            _colorShadowVisual.Offset = new Vector3(-ColorShadowPadding.Left.ToSingle(), -ColorShadowPadding.Top.ToSingle(), 0);

            if (_colorShadowBrush == null)
            {
                var blurEffect = new GaussianBlurEffect
                {
                    Name = "Blur",
                    BlurAmount = ColorShadowBlurRadius.ToSingle(),
                    BorderMode = EffectBorderMode.Soft,
                    Source = new CompositionEffectSourceParameter("source")
                };

                // Composite Effect
                var effect = new CompositeEffect
                {
                    Mode = CanvasComposite.DestinationIn,
                    Sources =
                        {
                            blurEffect,
                            new CompositionEffectSourceParameter("mask")
                        }
                };

                // Create Effect Factory
                var factory = _compositor.CreateEffectFactory(effect, new[] { "Blur.BlurAmount" });
                // Create Effect Brush
                _colorShadowBrush = factory.CreateBrush();
            }

            _colorShadowBrush.SetSourceParameter("source", imageBrush);

            _maskOptions.BlurRadius = ColorMaskBlurRadius.ToSingle();
            if (_imageMaskSurface == null)
            {
                // Create the ImageMaskSurface 
                _imageMaskSurface = _generator.CreateImageMaskSurface(_imageSurface, _colorShadowVisual.Size.ToSize(),
                                                                      ColorMaskPadding, _maskOptions);
                // Create SurfaceBrush
                var mask = _compositor.CreateSurfaceBrush(_imageMaskSurface);
                // Set the Mask
                _colorShadowBrush.SetSourceParameter("mask", mask);
            }
            else
            {
                // Update the ImageMaskSurface
                _imageMaskSurface.Resize(_colorShadowVisual.Size.ToSize(), ColorMaskPadding, _maskOptions);
            }

            _colorShadowVisual.Brush = _colorShadowBrush;
            _colorShadowVisual.Opacity = ColorShadowOpacity.ToSingle();

            _colorShadowBlurAnimation = _compositor.GenerateScalarKeyFrameAnimation().HavingDuration(TimeSpan.FromMilliseconds(100));
            Expression<CompositionExpression<float>> expr = c => ColorShadowBlurRadius.ToSingle();
            _colorShadowBlurAnimation.InsertExpressionKeyFrame(1f, expr, _compositor.CreateLinearEasingFunction());
            _colorShadowBrush.Properties.StartAnimation("Blur.BlurAmount", _colorShadowBlurAnimation);

            // Shadow for the Image Layer
            if (IsShadowEnabled)
            {
                _dropShadow.Offset = new Vector3(ShadowOffsetX.ToSingle(), ShadowOffsetY.ToSingle(), 0);
                _dropShadow.Opacity = ShadowOpacity.ToSingle();
                _dropShadow.BlurRadius = ShadowBlurRadius.ToSingle();
                _dropShadow.Color = ShadowColor;
                _imageVisual.Shadow = _dropShadow;
            }
            else
            {
                _imageVisual.Shadow = null;
            }

            if (!_rootVisual.Children.Any())
            {
                _rootVisual.Children.InsertAtTop(_colorShadowVisual);
                _rootVisual.Children.InsertAtTop(_imageVisual);
            }

            if (_renderGrid != null)
            {
                ElementCompositionPreview.SetElementChildVisual(_renderGrid, _rootVisual);
            }
        }

        return size;
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    private async Task<bool> LoadImageAsync(Uri uri)
    {
        if (uri == null)
        {
            return false;
        }

        await _syncObject.WaitAsync();

        try
        {
            VisualStateManager.GoToState(this, "Loading", true);
            _isLoading = true;
            if (_imageSourceLoaded)
            {
                _imageSourceLoaded = false;

                _rootVisual?.Children.RemoveAll();
                _rootVisual?.Dispose();
                _rootVisual = null;

                _imageVisual?.Dispose();
                _imageVisual = null;

                _colorShadowVisual?.Dispose();
                _colorShadowVisual = null;

                if (_imageSurface != null)
                {
                    _imageSurface.Dispose();
                    _imageSurface = null;
                }

                _imageSourceLoaded = false;

                if (_imageMaskSurface != null)
                {
                    _imageMaskSurface.Dispose();
                    _imageMaskSurface = null;
                }
            }

            if (_compositor == null)
            {
                InitComposition();
            }

            _imageSurface = await _generator.CreateImageSurfaceAsync(uri, new Size(), ImageSurfaceOptions.Default);
            _imageSourceLoaded = _imageSurface.Status == ImageSurfaceLoadStatus.Success;
        }
        catch
        {

        }
        finally
        {
            _syncObject.Release();
            VisualStateManager.GoToState(this, "Loaded", true);
            _isLoading = false;
        }

        if (_imageSourceLoaded)
        {
            InvalidateArrange();
        }

        return _imageSourceLoaded;
    }

    private void InitComposition()
    {
        _compositor = CompositionTarget.GetCompositorForCurrentThread();
        _generator = _compositor.CreateCompositionGenerator();
        _dropShadow = _compositor.CreateDropShadow();
        _dropShadow.SourcePolicy = CompositionDropShadowSourcePolicy.InheritFromVisualContent;
    }
}
