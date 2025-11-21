namespace DevWinUI;

/// <summary>
/// Control which can be used for displaying images
/// </summary>
[TemplatePart(Name = RenderBorder, Type = typeof(Border))]
public sealed partial class ImageFrame : Control, IDisposable
{
    private Color _shadowColor = Colors.Transparent;

    #region Enums

    private enum ImageEngineState
    {
        Idle = 0,
        Loading = 1,
        Scheduled = 2
    }

    #endregion

    #region Events and Delegates

    /// <summary>
    /// Fired when Image has been loaded successfully
    /// </summary>
    public event RoutedEventHandler ImageOpened;
    /// <summary>
    /// Fired when loading of the Image failed
    /// </summary>
    public event RoutedEventHandler ImageFailed;

    #endregion

    #region Constants

    private const string RenderBorder = "RenderBorder";

    private static readonly TimeSpan MinimumTransitionDuration = TimeSpan.FromMilliseconds(1);
    private static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromMilliseconds(700);
    private static readonly TimeSpan AlignmentTransitionDuration = TimeSpan.FromMilliseconds(500);
    private static readonly Size PlaceholderSize = new Size(48, 48);
    private const float MinScaleFactor = 0.1f;

    #endregion

    #region Fields

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private IImageSurface _imageSurface;
    private IImageSurface _nextImageSurface;
    private IMaskSurface _frameLayerMask;
    private IGeometrySurface _placeholderContentMask;
    private CompositionSurfaceBrush _placeholderContentBrush;
    private CompositionSurfaceBrush _nextSurfaceBrush;
    private ContainerVisual _rootContainer;
    private SpriteVisual _shadowVisual;
    private LayerVisual _frameLayer;
    private SpriteVisual _frameBackgroundVisual;
    private SpriteVisual _placeholderBackgroundVisual;
    private SpriteVisual _placeholderContentVisual;
    private SpriteVisual _frameContentVisual;
    private SpriteVisual _nextVisualContent;
    private DropShadow _shadow;
    private CompositionEffectBrush _layerEffectBrush;
    private ImageSurfaceOptions _imageOptions;
    private ScalarKeyFrameAnimation _fadeOutAnimation;
    private ScalarKeyFrameAnimation _fadeInAnimation;
    private ColorKeyFrameAnimation _colorAnimation;
    private ScalarKeyFrameAnimation _alignXAnimation;
    private ScalarKeyFrameAnimation _alignYAnimation;
    private Vector3KeyFrameAnimation _offsetAnimation;
    private Vector3KeyFrameAnimation _scaleAnimation;
    private CompositionAnimationGroup _zoomInAnimationGroup;

    private ImageEngineState _engineState;
    private Uri _scheduledUri;
    private Uri _currentUri;
    private Border _renderBorder;

    #endregion

    #region Dependency Properties

    public bool UseAutoShadowColor
    {
        get { return (bool)GetValue(UseAutoShadowColorProperty); }
        set { SetValue(UseAutoShadowColorProperty, value); }
    }

    public static readonly DependencyProperty UseAutoShadowColorProperty =
        DependencyProperty.Register(nameof(UseAutoShadowColor), typeof(bool), typeof(ImageFrame), new PropertyMetadata(false, OnUseAutoShadowColor));

    private async static void OnUseAutoShadowColor(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ImageFrame)d;
        if (ctl != null && ctl.DisplayShadow)
        {
            ctl.UpdateShadowColor();
            ctl.InvalidateArrange();
        }
    }

    private async void UpdateShadowColor()
    {
        if (UseAutoShadowColor)
        {
            var uri = GetUriFromSource();
            if (uri != null)
            {
                var device = CanvasDevice.GetSharedDevice();
                _shadowColor = await ColorHelperEx.GetImageEdgeColorWithWin2DAsync(device, uri);
            }
        }
        else
        {
            _shadowColor = ShadowColor;
        }
    }


    #region AlignX

    /// <summary>
    /// AlignX Dependency Property
    /// </summary>
    public static readonly DependencyProperty AlignXProperty =
        DependencyProperty.Register(nameof(AlignX), typeof(AlignmentX), typeof(ImageFrame),
            new PropertyMetadata(AlignmentX.Center, OnAlignXChanged));

    /// <summary>
    /// Gets or sets the AlignX property. This dependency property 
    /// indicates how the image is positioned horizontally in the ImageFrame.
    /// </summary>
    public AlignmentX AlignX
    {
        get => (AlignmentX)GetValue(AlignXProperty);
        set => SetValue(AlignXProperty, value);
    }

    /// <summary>
    /// Handles changes to the AlignX property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnAlignXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnAlignXChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the AlignX property.
    /// </summary>
    private void OnAlignXChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region AlignY

    /// <summary>
    /// AlignY Dependency Property
    /// </summary>
    public static readonly DependencyProperty AlignYProperty =
        DependencyProperty.Register(nameof(AlignY), typeof(AlignmentY), typeof(ImageFrame),
            new PropertyMetadata(AlignmentY.Center, OnAlignYChanged));

    /// <summary>
    /// Gets or sets the AlignY property. This dependency property 
    /// indicates how the image is positioned vertically in the ImageFrame.
    /// </summary>
    public AlignmentY AlignY
    {
        get => (AlignmentY)GetValue(AlignYProperty);
        set => SetValue(AlignYProperty, value);
    }

    /// <summary>
    /// Handles changes to the AlignY property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnAlignYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnAlignYChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the AlignY property.
    /// </summary>
    private void OnAlignYChanged()
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
        DependencyProperty.Register(nameof(DisplayShadow), typeof(bool), typeof(ImageFrame),
            new PropertyMetadata(false, OnDisplayShadowChanged));

    /// <summary>
    /// Gets or sets the DisplayShadow property. This dependency property 
    /// indicates whether the shadow for this image should be displayed.
    /// </summary>
    public bool DisplayShadow
    {
        get => (bool)GetValue(DisplayShadowProperty);
        set => SetValue(DisplayShadowProperty, value);
    }

    /// <summary>
    /// Handles changes to the DisplayShadow property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnDisplayShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnDisplayShadowChanged();
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

    #region FrameBackground

    /// <summary>
    /// FrameBackground Dependency Property
    /// </summary>
    public static readonly DependencyProperty FrameBackgroundProperty =
        DependencyProperty.Register(nameof(FrameBackground), typeof(Color), typeof(ImageFrame),
            new PropertyMetadata(Colors.Black, OnFrameBackgroundChanged));

    /// <summary>
    /// Gets or sets the FrameBackground property. This dependency property 
    /// indicates the background color of the ImageFrame to fill the area 
    /// where image is not rendered.
    /// </summary>
    public Color FrameBackground
    {
        get => (Color)GetValue(FrameBackgroundProperty);
        set => SetValue(FrameBackgroundProperty, value);
    }

    /// <summary>
    /// Handles changes to the FrameBackground property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnFrameBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnFrameBackgroundChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the FrameBackground property.
    /// </summary>
    private void OnFrameBackgroundChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region Interpolation

    /// <summary>
    /// Interpolation Dependency Property
    /// </summary>
    public static readonly DependencyProperty InterpolationProperty =
        DependencyProperty.Register(nameof(Interpolation), typeof(CanvasImageInterpolation), typeof(ImageFrame),
            new PropertyMetadata(CanvasImageInterpolation.HighQualityCubic, OnInterpolationChanged));

    /// <summary>
    /// Gets or sets the Interpolation property. This dependency property 
    /// indicates the interpolation used for rendering the image.
    /// </summary>
    public CanvasImageInterpolation Interpolation
    {
        get => (CanvasImageInterpolation)GetValue(InterpolationProperty);
        set => SetValue(InterpolationProperty, value);
    }

    /// <summary>
    /// Handles changes to the Interpolation property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnInterpolationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnInterpolationChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the Interpolation property.
    /// </summary>
    private void OnInterpolationChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region OptimizeShadow

    /// <summary>
    /// OptimizeShadow Dependency Property
    /// </summary>
    public static readonly DependencyProperty OptimizeShadowProperty =
        DependencyProperty.Register(nameof(OptimizeShadow), typeof(bool), typeof(ImageFrame),
            new PropertyMetadata(false, OnOptimizeShadowChanged));

    /// <summary>
    /// Gets or sets the OptimizeShadow property. This dependency property 
    /// indicates whether the ImageFrame should use a shared shadow object to display
    /// the shadow.
    /// </summary>
    public bool OptimizeShadow
    {
        get => (bool)GetValue(OptimizeShadowProperty);
        set => SetValue(OptimizeShadowProperty, value);
    }

    /// <summary>
    /// Handles changes to the OptimizeShadow property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnOptimizeShadowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnOptimizeShadowChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the OptimizeShadow property.
    /// </summary>
    private void OnOptimizeShadowChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region PlaceholderColor

    /// <summary>
    /// PlaceholderColor Dependency Property
    /// </summary>
    public static readonly DependencyProperty PlaceholderColorProperty =
        DependencyProperty.Register(nameof(PlaceholderColor), typeof(Color), typeof(ImageFrame),
            new PropertyMetadata(Color.FromArgb(255, 224, 224, 224), OnPlaceholderColorChanged));

    /// <summary>
    /// Gets or sets the PlaceholderColor property. This dependency property 
    /// indicates the color with which the rendered placeholder geometry should be filled.
    /// </summary>
    public Color PlaceholderColor
    {
        get => (Color)GetValue(PlaceholderColorProperty);
        set => SetValue(PlaceholderColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the PlaceholderColor property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnPlaceholderColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnPlaceholderColorChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the PlaceholderColor property.
    /// </summary>
    private void OnPlaceholderColorChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region PlaceholderBackground

    /// <summary>
    /// PlaceholderBackground Dependency Property
    /// </summary>
    public static readonly DependencyProperty PlaceholderBackgroundProperty =
        DependencyProperty.Register(nameof(PlaceholderBackground), typeof(Color), typeof(ImageFrame),
            new PropertyMetadata(Colors.Black, OnPlaceholderBackgroundChanged));

    /// <summary>
    /// Gets or sets the PlaceholderBackground property. This dependency property 
    /// indicates the background color of the Placeholder.
    /// </summary>
    public Color PlaceholderBackground
    {
        get => (Color)GetValue(PlaceholderBackgroundProperty);
        set => SetValue(PlaceholderBackgroundProperty, value);
    }

    /// <summary>
    /// Handles changes to the PlaceholderBackground property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnPlaceholderBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnPlaceholderBackgroundChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the PlaceholderBackground property.
    /// </summary>
    private void OnPlaceholderBackgroundChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region RenderFast

    /// <summary>
    /// RenderFast Dependency Property
    /// </summary>
    public static readonly DependencyProperty RenderFastProperty =
        DependencyProperty.Register(nameof(RenderFast), typeof(bool), typeof(ImageFrame),
            new PropertyMetadata(false));

    /// <summary>
    /// Gets or sets the RenderFast property. This dependency property 
    /// indicates whether some of the transitions need to be switched off
    /// if the ImageFrame is being used in scenarios where it is 
    /// being rapidly updated with new Source (for example - like in a ListBox 
    /// containing numerous ImageFrames which is being scrolled very fast).
    /// </summary>
    public bool RenderFast
    {
        get => (bool)GetValue(RenderFastProperty);
        set => SetValue(RenderFastProperty, value);
    }

    #endregion

    #region RenderOptimized

    /// <summary>
    /// RenderOptimized Dependency Property
    /// </summary>
    public static readonly DependencyProperty RenderOptimizedProperty =
        DependencyProperty.Register(nameof(RenderOptimized), typeof(bool), typeof(ImageFrame),
            new PropertyMetadata(false, OnRenderOptimizedChanged));

    /// <summary>
    /// Gets or sets the RenderOptimized property. This dependency property 
    /// indicates whether optimization must be used to render the image.
    /// Set this property to True if the ImageFrame is very small
    /// compared to the actual image size. This will optimize memory usage.
    /// </summary>
    public bool RenderOptimized
    {
        get => (bool)GetValue(RenderOptimizedProperty);
        set => SetValue(RenderOptimizedProperty, value);
    }

    /// <summary>
    /// Handles changes to the RenderOptimized property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnRenderOptimizedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnRenderOptimizedChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the RenderOptimized property.
    /// </summary>
    private void OnRenderOptimizedChanged()
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
        DependencyProperty.Register(nameof(ShadowBlurRadius), typeof(double), typeof(ImageFrame),
            new PropertyMetadata(0.0, OnShadowBlurRadiusChanged));

    /// <summary>
    /// Gets or sets the ShadowBlurRadius property. This dependency property 
    /// indicates the blur radius of the ImageFrame shadow.
    /// </summary>
    public double ShadowBlurRadius
    {
        get => (double)GetValue(ShadowBlurRadiusProperty);
        set => SetValue(ShadowBlurRadiusProperty, CoerceShadowBlurRadius(value));
    }

    /// <summary>
    /// Coerces the ShadowBlurRadius to a more acceptable value
    /// </summary>
    /// <param name="newShadowBlurRadius">ShadowBlurRadius value to be coerced.</param>
    /// <returns>Coerced ShadowBlurRadius value</returns>
    private object CoerceShadowBlurRadius(double newShadowBlurRadius)
    {
        return Math.Max(0.0, newShadowBlurRadius);
    }

    /// <summary>
    /// Handles changes to the ShadowBlurRadius property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowBlurRadiusChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShadowBlurRadiusChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowBlurRadius property.
    /// </summary>
    private void OnShadowBlurRadiusChanged()
    {
        // Refresh Layout if shadow is displayed
        if (DisplayShadow)
        {
            InvalidateArrange();
        }
    }

    #endregion

    #region ShadowColor

    /// <summary>
    /// ShadowColor Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowColorProperty =
        DependencyProperty.Register(nameof(ShadowColor), typeof(Color), typeof(ImageFrame),
            new PropertyMetadata(Colors.Transparent, OnShadowColorChanged));

    /// <summary>
    /// Gets or sets the ShadowColor property. This dependency property 
    /// indicates the color of the ImageFrame shadow.
    /// </summary>
    public Color ShadowColor
    {
        get => (Color)GetValue(ShadowColorProperty);
        set => SetValue(ShadowColorProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowColor property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShadowColorChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowColor property.
    /// </summary>
    private void OnShadowColorChanged()
    {
        // Refresh Layout if shadow is displayed
        if (DisplayShadow)
        {
            UpdateShadowColor();
            InvalidateArrange();
        }
    }

    #endregion

    #region ShadowOffsetX

    /// <summary>
    /// ShadowOffsetX Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetXProperty =
        DependencyProperty.Register(nameof(ShadowOffsetX), typeof(double), typeof(ImageFrame),
            new PropertyMetadata(0.0, OnShadowOffsetXChanged));

    /// <summary>
    /// Gets or sets the ShadowOffsetX property. This dependency property 
    /// indicates the horizontal offset of the ImageFrame shadow.
    /// </summary>
    public double ShadowOffsetX
    {
        get => (double)GetValue(ShadowOffsetXProperty);
        set => SetValue(ShadowOffsetXProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetX property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShadowOffsetXChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOffsetX property.
    /// </summary>
    private void OnShadowOffsetXChanged()
    {
        // Refresh Layout if shadow is displayed
        if (DisplayShadow)
        {
            InvalidateArrange();
        }
    }

    #endregion

    #region ShadowOffsetY

    /// <summary>
    /// ShadowOffsetY Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOffsetYProperty =
        DependencyProperty.Register(nameof(ShadowOffsetY), typeof(double), typeof(ImageFrame),
            new PropertyMetadata(0.0, OnShadowOffsetYChanged));

    /// <summary>
    /// Gets or sets the ShadowOffsetY property. This dependency property 
    /// indicates the vertical offset of the ImageFrame shadow.
    /// </summary>
    public double ShadowOffsetY
    {
        get => (double)GetValue(ShadowOffsetYProperty);
        set => SetValue(ShadowOffsetYProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShadowOffsetY property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOffsetYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShadowOffsetYChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOffsetY property.
    /// </summary>
    private void OnShadowOffsetYChanged()
    {
        // Refresh Layout if shadow is displayed
        if (DisplayShadow)
        {
            InvalidateArrange();
        }
    }

    #endregion

    #region ShadowOpacity

    /// <summary>
    /// ShadowOpacity Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShadowOpacityProperty =
        DependencyProperty.Register(nameof(ShadowOpacity), typeof(double), typeof(ImageFrame),
            new PropertyMetadata(1.0, OnShadowOpacityChanged));

    /// <summary>
    /// Gets or sets the ShadowOpacity property. This dependency property 
    /// indicates the opacity of the ImageFrame shadow.
    /// </summary>
    public double ShadowOpacity
    {
        get => (double)GetValue(ShadowOpacityProperty);
        set => SetValue(ShadowOpacityProperty, CoerceShadowOpacity(value));
    }

    /// <summary>
    /// Coerces the ShadowOpacity to a more acceptable value.
    /// </summary>
    /// <param name="newShadowOpacity">ShadowOpacity value to be coerced.</param>
    /// <returns>Coerced ShadowOpacity value</returns>
    private object CoerceShadowOpacity(double newShadowOpacity)
    {
        // Opacity value should be between 0 and 1 inclusive
        var opacity = Math.Max(0, newShadowOpacity);
        return Math.Min(1.0, opacity);
    }

    /// <summary>
    /// Handles changes to the ShadowOpacity property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShadowOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShadowOpacityChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShadowOpacity property.
    /// </summary>
    private void OnShadowOpacityChanged()
    {
        // Refresh Layout if shadow is displayed
        if (DisplayShadow)
        {
            InvalidateArrange();
        }
    }

    #endregion

    #region ShowPlaceholder

    /// <summary>
    /// ShowPlaceholder Dependency Property
    /// </summary>
    public static readonly DependencyProperty ShowPlaceholderProperty =
        DependencyProperty.Register(nameof(ShowPlaceholder), typeof(bool), typeof(ImageFrame),
            new PropertyMetadata(true, OnShowPlaceholderChanged));

    /// <summary>
    /// Gets or sets the ShowPlaceholder property. This dependency property 
    /// indicates whether the placeholder needs to be displayed during image load or
    /// when no image is loaded in the ImageFrame.
    /// </summary>
    public bool ShowPlaceholder
    {
        get => (bool)GetValue(ShowPlaceholderProperty);
        set => SetValue(ShowPlaceholderProperty, value);
    }

    /// <summary>
    /// Handles changes to the ShowPlaceholder property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnShowPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnShowPlaceholderChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ShowPlaceholder property.
    /// </summary>
    private void OnShowPlaceholderChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region Source

    /// <summary>
    /// Source Dependency Property
    /// </summary>
    public static readonly DependencyProperty SourceProperty =
        DependencyProperty.Register(nameof(Source), typeof(object), typeof(ImageFrame),
            new PropertyMetadata(null, OnSourceChanged));

    /// <summary>
    /// Gets or sets the Source property. This dependency property 
    /// indicates Uri or StorageFile or IRandomAccessStream of the image 
    /// to be loaded into the ImageFrame.
    /// </summary>
    public object Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    /// <summary>
    /// Handles changes to the Source property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var imageFrame = (ImageFrame)d;
        imageFrame.OnSourceChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the Source property.
    /// </summary>
    private void OnSourceChanged()
    {
        // When the Source value is obtained from a Binding, often this value is set before
        // the ImageFrame has done its first Layout pass (in that case compositor will
        // be null). No need to proceed in this scenario. The Source will be scheduled for loading
        // during the Arrange Pass.
        if (_compositor == null)
            return;

        // If the ImageFrame is properly initialized, then we can schedule this Uri
        // to be loaded next.

        UpdateShadowColor();
        ScheduleNextLoad();
    }

    #endregion

    #region Stretch

    /// <summary>
    /// Stretch Dependency Property
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register(nameof(Stretch), typeof(Stretch), typeof(ImageFrame),
            new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

    /// <summary>
    /// Gets or sets the Stretch property. This dependency property 
    /// indicates how the image is resized to fill its allocated space 
    /// in the ImageFrame.
    /// </summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Handles changes to the Stretch property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var frame = (ImageFrame)d;
        frame.OnStretchChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the Stretch property.
    /// </summary>
    private void OnStretchChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region TransitionDuration

    /// <summary>
    /// TransitionDuration Dependency Property
    /// </summary>
    public static readonly DependencyProperty TransitionDurationProperty =
        DependencyProperty.Register(nameof(TransitionDuration), typeof(TimeSpan), typeof(ImageFrame),
            new PropertyMetadata(DefaultTransitionDuration, OnTransitionDurationChanged));

    /// <summary>
    /// Gets or sets the TransitionDuration property. This dependency property 
    /// indicates the duration of the cross fade animation for transitioning 
    /// from one image to another.
    /// </summary>
    public TimeSpan TransitionDuration
    {
        get => (TimeSpan)GetValue(TransitionDurationProperty);
        set => SetValue(TransitionDurationProperty, CoerceTransitionDuration(value));
    }

    /// <summary>
    /// Coerces the TransitionDuration to a more acceptable value
    /// </summary>
    /// <param name="duration">Transition Duration</param>
    /// <returns>Coerced Transition Duration</returns>
    private object CoerceTransitionDuration(TimeSpan duration)
    {
        return (duration.TotalMilliseconds.IsZero() || (duration.TotalMilliseconds < 0)) ? MinimumTransitionDuration : duration;
    }

    /// <summary>
    /// Handles changes to the TransitionDuration property.
    /// </summary>
    /// <param name="d">ImageFrame</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnTransitionDurationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var target = (ImageFrame)d;
        var newTransitionDuration = target.TransitionDuration;
        target.OnTransitionDurationChanged(newTransitionDuration);
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the TransitionDuration property.
    /// </summary>
    /// <param name="newTransitionDuration">New Value</param>
    private void OnTransitionDurationChanged(TimeSpan newTransitionDuration)
    {
        // Update the animations if they are already created
        UpdateAnimationsDuration(newTransitionDuration);
    }

    #endregion

    #region TransitionMode

    /// <summary>
    /// TransitionMode Dependency Property
    /// </summary>
    public static readonly DependencyProperty TransitionModeProperty =
        DependencyProperty.Register(nameof(TransitionMode), typeof(ImageFrameTransitionMode), typeof(ImageFrame),
            new PropertyMetadata(ImageFrameTransitionMode.FadeIn));

    /// <summary>
    /// Gets or sets the TransitionMode property. This dependency property 
    /// indicates the type of transition animation to employ for displaying
    /// an image after it has been loaded.
    /// </summary>
    public ImageFrameTransitionMode TransitionMode
    {
        get => (ImageFrameTransitionMode)GetValue(TransitionModeProperty);
        set => SetValue(TransitionModeProperty, value);
    }

    #endregion

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Ctor
    /// </summary>
    public ImageFrame()
    {
        // Set the default Style Key
        DefaultStyleKey = typeof(ImageFrame);

        // Handle the CornerRadius Change
        RegisterPropertyChangedCallback(ImageFrame.CornerRadiusProperty, OnCornerRadiusChanged);

        // Setting the background to transparent so that it will
        // receive Pointer events
        Background = new SolidColorBrush(Colors.Transparent);
        // Initialize the ImageOptions
        _imageOptions = ImageSurfaceOptions.Default;
    }

    #endregion

    #region APIs

    /// <summary>
    /// Disposes the resources
    /// </summary>
    public void Dispose()
    {
        // Clean up resources
        _compositor = null;
        Source = null;
        DataContext = null;
        Foreground = null;
        Background = null;
        _scheduledUri = null;
        _currentUri = null;

        // Clean up Composition Objects
        _imageSurface?.Dispose();
        _imageSurface = null;
        _nextImageSurface?.Dispose();
        _nextImageSurface = null;
        _frameLayerMask?.Dispose();
        _frameLayerMask = null;
        _placeholderContentMask?.Dispose();
        _placeholderContentMask = null;
        _placeholderContentBrush?.Dispose();
        _placeholderContentBrush = null;
        _nextSurfaceBrush?.Dispose();
        _nextSurfaceBrush = null;
        _rootContainer?.Dispose();
        _rootContainer = null;
        _shadowVisual?.Dispose();
        _shadowVisual = null;
        _frameLayer?.Dispose();
        _frameLayer = null;
        _frameBackgroundVisual?.Dispose();
        _frameBackgroundVisual = null;
        _placeholderBackgroundVisual?.Dispose();
        _placeholderBackgroundVisual = null;
        _placeholderContentVisual?.Dispose();
        _placeholderContentVisual = null;
        _frameContentVisual?.Dispose();
        _frameContentVisual = null;
        _nextVisualContent?.Dispose();
        _nextVisualContent = null;
        _shadow?.Dispose();
        _shadow = null;
        _layerEffectBrush?.Dispose();
        _layerEffectBrush = null;
        _imageOptions = null;
        _zoomInAnimationGroup?.Dispose();
        _zoomInAnimationGroup = null;
        _fadeOutAnimation?.Dispose();
        _fadeOutAnimation = null;
        _fadeInAnimation?.Dispose();
        _fadeInAnimation = null;
        _colorAnimation?.Dispose();
        _colorAnimation = null;
        _alignXAnimation?.Dispose();
        _alignXAnimation = null;
        _alignYAnimation?.Dispose();
        _alignYAnimation = null;
        _offsetAnimation?.Dispose();
        _offsetAnimation = null;
        _scaleAnimation?.Dispose();
        _scaleAnimation = null;

        // Dispose the generator at the end to allow the 
        // dependent composition objects to unsubscribe from
        // generator events
        _generator?.Dispose();
        _generator = null;
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Called when the template is applied to the control
    /// </summary>
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _renderBorder = GetTemplateChild(RenderBorder) as Border;
    }

    /// <summary>
    /// Handles the Measure pass during Layout
    /// </summary>
    /// <param name="availableSize">Available size</param>
    /// <returns>Total Size required to accommodate the Children</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Initialize Composition elements for the the very first time
        if (_compositor == null)
        {
            InitComposition();
        }

        // If the width or the height of the availableSize is Infinity, then calculate the size required by the
        // image to display itself.
        var hasInvalidAvailableWidth = Double.IsNaN(availableSize.Width) || Double.IsInfinity(availableSize.Width);
        var hasInvalidAvailableHeight = Double.IsNaN(availableSize.Height) || Double.IsInfinity(availableSize.Height);
        var hasInvalidLength = hasInvalidAvailableWidth || hasInvalidAvailableHeight;

        if ((_imageSurface?.Uri != null) && hasInvalidLength)
        {
            if ((hasInvalidAvailableWidth && hasInvalidAvailableHeight) || (Stretch == Stretch.None))
            {
                return _imageSurface.Size;
            }

            if (hasInvalidAvailableWidth)
            {
                var finalHeight = availableSize.Height;
                var finalWidth = (_imageSurface.Size.Width * finalHeight) / _imageSurface.Size.Height;
                return new Size(finalWidth, finalHeight);
            }
            else
            {
                var finalWidth = availableSize.Width;
                var finalHeight = (_imageSurface.Size.Height * finalWidth) / _imageSurface.Size.Width;
                return new Size(finalWidth, finalHeight);
            }
        }

        return base.MeasureOverride(availableSize);
    }

    /// <summary>
    /// Handles the Arrange pass during Layout
    /// </summary>
    /// <param name="finalSize">Final Size of the control</param>
    /// <returns>Total size occupied by the Children</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        // Taking into account the BorderThickness and Padding
        var borders = BorderThickness;
        var padding = Padding;
        var corners = CornerRadius;
        var borderSize = borders.CollapseThickness();
        var paddingSize = padding.CollapseThickness();

        // Calculate the Offset for the frameVisual
        var left = (borders.Left + padding.Left).ToSingle();
        var top = (borders.Top + padding.Top).ToSingle();

        // Calculate the Dimensions of the frameVisual
        var width = Math.Max(0, finalSize.Width - borderSize.Width - paddingSize.Width).ToSingle();
        var height = Math.Max(0, finalSize.Height - borderSize.Height - paddingSize.Height).ToSingle();

        // Set the Size and Offset of visuals in the ImageFrame
        var frameSize = new Vector2(width, height);
        _rootContainer.Size = frameSize;
        _frameLayer.Size = frameSize;
        _frameBackgroundVisual.Size = frameSize;
        _frameContentVisual.Size = frameSize;
        _placeholderContentVisual.Size = frameSize;
        _placeholderBackgroundVisual.Size = frameSize;
        _shadowVisual.Size = frameSize;
        _nextVisualContent.Size = frameSize;
        _rootContainer.Offset = new Vector3(left, top, 0);

        // Update the frameLayerMask in case the CornerRadius or 
        // BorderThickness or Padding has changed
        var rect = new CanvasRoundRect(Vector2.Zero, frameSize, corners.ToVector4(),
            borders.ToVector4(), padding.ToVector4(), false);

        using (var pathBuilder = new CanvasPathBuilder(_generator.Device))
        {
            pathBuilder.AddRoundedRectangleFigure(rect);
            using (var geometry = CanvasGeometry.CreatePath(pathBuilder))
            {
                _frameLayerMask.Redraw(_frameLayer.Size.ToSize(), geometry);
            }
        }

        // If the FrameBackground has changed since the last time it was
        // applied to the frameBackgroundVisual, then animate the brush's
        // color to the new color.
        if (_frameBackgroundVisual.Brush is CompositionColorBrush brush)
        {
            if (!brush.Color.Equals(FrameBackground))
            {
                // If we are rendering fast, then no need to animate.
                // Just set the final value.
                if (RenderFast)
                {
                    brush.Color = FrameBackground;
                }
                else
                {
                    _colorAnimation.InsertKeyFrame(1f, FrameBackground);
                    brush.StartAnimation("Color", _colorAnimation);
                }
            }
        }

        // If the PlaceholderBackground has changed since the last time it was
        // applied to the _placeholderBackgroundVisual, then update the brush's
        // color to the new color.
        brush = _placeholderBackgroundVisual.Brush as CompositionColorBrush;
        if (brush != null)
        {
            if (!brush.Color.Equals(PlaceholderBackground))
            {
                brush.Color = PlaceholderBackground;
            }
        }

        // Redraw the placeholder content with the latest placeholder color
        _placeholderContentMask.Redraw(PlaceholderColor, PlaceholderBackground);

        // Set the stretch property of placeholder content's brush according to its size
        if ((width > PlaceholderSize.Width) && (height > PlaceholderSize.Height))
        {
            _placeholderContentBrush.Stretch = CompositionStretch.None;
        }
        else
        {
            _placeholderContentBrush.Stretch = CompositionStretch.Uniform;
        }

        // Update the imageOptions
        _imageOptions.Stretch = Stretch;
        _imageOptions.HorizontalAlignment = AlignX;
        _imageOptions.VerticalAlignment = AlignY;
        _imageOptions.Interpolation = Interpolation;
        _imageOptions.SurfaceBackgroundColor = Colors.Transparent;
        _imageOptions.AutoResize = !RenderOptimized;

        // If Source is valid then try loading/refreshing the ImageSurface
        if (Source != null)
        {
            if (_imageSurface != null)
            {
                // Resize the ImageSurface to the new size
                _imageSurface.Resize(frameSize.ToSize(), _imageOptions);
                // Update the surface brush based on the Stretch and Alignment options
                if (RenderFast)
                {
                    // Use no animations to update alignment if we are rendering fast
                    (_frameContentVisual.Brush as CompositionSurfaceBrush)?.UpdateSurfaceBrushOptions(Stretch,
                        AlignX, AlignY);
                }
                else
                {
                    // Update stretch and alignment using animation
                    (_frameContentVisual.Brush as CompositionSurfaceBrush)?.UpdateSurfaceBrushOptions(Stretch,
                        AlignX, AlignY, _alignXAnimation, _alignYAnimation);
                }
            }
            else
            {
                // Load the Source to the ImageSurface
                ScheduleNextLoad();
            }
        }
        else
        {
            // If the frameContentVisual had any previous brush, fade it out
            if (_imageSurface == null)
            {
                // Make the frameVisualContent transparent
                _frameContentVisual.Brush = _compositor.CreateColorBrush(Colors.Transparent);
                DisplayPlaceHolder();
                _engineState = ImageEngineState.Idle;
            }
        }

        // Handle shadow
        if (DisplayShadow)
        {
            // If OptimizeShadow is True then use the sharedShadow otherwise use the instance shadow
            var shadow = OptimizeShadow
                         ? ShadowProvider.GetSharedShadow(_compositor)
                         : (_shadow ?? (_shadow = _compositor.CreateDropShadow()));

            shadow.BlurRadius = ShadowBlurRadius.ToSingle();
            shadow.Color = _shadowColor;
            shadow.Offset = new Vector3(ShadowOffsetX.ToSingle(), ShadowOffsetY.ToSingle(), 0);
            shadow.Opacity = ShadowOpacity.ToSingle();
            shadow.Mask = _layerEffectBrush.GetSourceParameter("mask");

            _shadowVisual.Shadow = shadow;
        }
        else
        {
            _shadowVisual.Shadow = null;
        }

        return base.ArrangeOverride(finalSize);
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Initializes the Composition elements
    /// </summary>
    private void InitComposition()
    {
        // Compositor
        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        // CompositionGenerator
        _generator = _compositor.CreateCompositionGenerator();

        // Fade Out Animation
        _fadeOutAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _fadeOutAnimation.InsertKeyFrame(1f, 0);
        _fadeOutAnimation.Duration = TransitionDuration;
        // Fade In Animation
        _fadeInAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _fadeInAnimation.InsertKeyFrame(1f, 1);
        _fadeInAnimation.Duration = TransitionDuration;
        // Color Animation
        _colorAnimation = _compositor.CreateColorKeyFrameAnimation();
        _colorAnimation.Duration = TransitionDuration;
        // Offset Animation
        _offsetAnimation = _compositor.CreateVector3KeyFrameAnimation();
        _offsetAnimation.Target = "Offset";
        _offsetAnimation.Duration = TransitionDuration;
        _offsetAnimation.InsertKeyFrame(1f, Vector3.Zero);
        // Alignment animations
        _alignXAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _alignXAnimation.Duration = AlignmentTransitionDuration;
        _alignYAnimation = _compositor.CreateScalarKeyFrameAnimation();
        _alignYAnimation.Duration = AlignmentTransitionDuration;

        // ZoomIn Animation Group
        _scaleAnimation = _compositor.CreateVector3KeyFrameAnimation();
        _scaleAnimation.Target = "Scale";
        _scaleAnimation.InsertKeyFrame(1f, Vector3.One);
        _scaleAnimation.Duration = TransitionDuration;
        _zoomInAnimationGroup = _compositor.CreateAnimationGroup();
        _zoomInAnimationGroup.Add(_scaleAnimation);
        _zoomInAnimationGroup.Add(_offsetAnimation);

        // Visuals
        _rootContainer = _compositor.CreateContainerVisual();
        _frameLayer = _compositor.CreateLayerVisual();
        _frameBackgroundVisual = _compositor.CreateSpriteVisual();
        _frameContentVisual = _compositor.CreateSpriteVisual();
        _placeholderContentVisual = _compositor.CreateSpriteVisual();
        _placeholderBackgroundVisual = _compositor.CreateSpriteVisual();
        _nextVisualContent = _compositor.CreateSpriteVisual();

        _frameLayer.Children.InsertAtTop(_frameBackgroundVisual);
        _frameLayer.Children.InsertAtTop(_frameContentVisual);
        _frameLayer.Children.InsertAtTop(_placeholderBackgroundVisual);
        _frameLayer.Children.InsertAtTop(_placeholderContentVisual);
        _frameLayer.Children.InsertAtTop(_nextVisualContent);

        // Placeholder content
        _placeholderContentMask = _generator.CreateGeometrySurface(PlaceholderSize, GetPlaceHolderGeometry(),
            PlaceholderColor, PlaceholderBackground);
        _placeholderContentBrush = _compositor.CreateSurfaceBrush(_placeholderContentMask.Surface);
        _placeholderContentVisual.Brush = _placeholderContentBrush;
        // Placeholder background
        _placeholderBackgroundVisual.Brush = _compositor.CreateColorBrush(PlaceholderBackground);

        // By default placeholder visual will not be visible
        HidePlaceholder();

        // Shadow visual
        _shadowVisual = _compositor.CreateSpriteVisual();

        _rootContainer.Children.InsertAtBottom(_shadowVisual);
        _rootContainer.Children.InsertAtTop(_frameLayer);

        _frameBackgroundVisual.Brush = _compositor.CreateColorBrush(FrameBackground);

        // Create the effect to create the opacity mask
        var layerEffect = new CompositeEffect
        {
            // CanvasComposite.DestinationIn - Intersection of source and mask. 
            // Equation: O = MA * S
            // where O - Output pixel, MA - Mask Alpha, S - Source pixel.
            Mode = CanvasComposite.DestinationIn,
            Sources =
                        {
                            new CompositionEffectSourceParameter("source"),
                            new CompositionEffectSourceParameter("mask")
                        }
        };

        var layerEffectFactory = _compositor.CreateEffectFactory(layerEffect);
        _layerEffectBrush = layerEffectFactory.CreateBrush();

        // The mask for the imageFrame
        _frameLayerMask = _generator.CreateMaskSurface(new Size(0, 0), null);
        _layerEffectBrush.SetSourceParameter("mask", _compositor.CreateSurfaceBrush(_frameLayerMask.Surface));
        // Apply the mask effect to the frameLayer
        _frameLayer.Effect = _layerEffectBrush;

        if (_renderBorder != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_renderBorder, _rootContainer);
        }
    }

    /// <summary>
    /// Updates the duration of animations with the given duration.
    /// </summary>
    /// <param name="newTransitionDuration">New duration to set</param>
    private void UpdateAnimationsDuration(TimeSpan newTransitionDuration)
    {
        if (_fadeOutAnimation != null)
        {
            _fadeOutAnimation.Duration = newTransitionDuration;
        }

        if (_fadeInAnimation != null)
        {
            _fadeInAnimation.Duration = newTransitionDuration;
        }

        if (_colorAnimation != null)
        {
            _colorAnimation.Duration = newTransitionDuration;
        }

        if (_offsetAnimation != null)
        {
            _offsetAnimation.Duration = newTransitionDuration;
        }

        if (_scaleAnimation != null)
        {
            _scaleAnimation.Duration = newTransitionDuration;
        }
    }

    /// <summary>
    /// Creates the geometry for the placeholder
    /// </summary>
    /// <param name="progress">Progress percentage</param>
    /// <returns>CanvasGeometry</returns>
    private CanvasGeometry GetPlaceHolderGeometry(int progress = -1)
    {
        using (var pathBuilder = new CanvasPathBuilder(_generator.Device))
        using (var pathBuilder2 = new CanvasPathBuilder(_generator.Device))
        {
            pathBuilder.BeginFigure(33.690f, 18.000f);
            pathBuilder.AddLine(30.082f, 22.793f);
            pathBuilder.AddLine(39.174f, 39.616f);
            pathBuilder.AddLine(28.297f, 25.166f);
            pathBuilder.AddLine(17.250f, 10.490f);
            pathBuilder.AddLine(3.750f, 27.596f);
            pathBuilder.AddLine(3.750f, 3.750f);
            pathBuilder.AddLine(44.250f, 3.750f);
            pathBuilder.AddLine(44.250f, 30.523f);
            pathBuilder.AddLine(33.690f, 18.000f);
            pathBuilder.EndFigure(CanvasFigureLoop.Closed);

            pathBuilder2.BeginFigure(0.000f, 0.000f);
            pathBuilder2.AddLine(0.000f, 32.348f);
            pathBuilder2.AddLine(0.000f, 48.000f);
            pathBuilder2.AddLine(0.000f, 48.000f);
            pathBuilder2.AddLine(48.000f, 48.000f);
            pathBuilder2.AddLine(48.000f, 48.000f);
            pathBuilder2.AddLine(48.000f, 34.971f);
            pathBuilder2.AddLine(48.000f, 0.000f);
            pathBuilder2.AddLine(0.000f, 0.000f);
            pathBuilder2.EndFigure(CanvasFigureLoop.Closed);
            using (var geom1 = CanvasGeometry.CreatePath(pathBuilder))
            using (var geom2 = CanvasGeometry.CreatePath(pathBuilder2))
            {
                var geom3 = geom2.CombineWith(geom1, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
                if ((progress < 0) || (progress >= 100))
                {
                    // No need to display the progress
                    return geom3;
                }
                else
                {
                    // Create the progress geometry
                    using (var geom4 = CanvasGeometry.CreateRectangle(_generator.Device, 2, 40, 44, 6))
                    {
                        var geom5 = geom3.CombineWith(geom4, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
                        var width = progress * 0.40f;
                        using (var geom6 = CanvasGeometry.CreateRectangle(_generator.Device, 4, 42, width, 2))
                        {
                            return geom5.CombineWith(geom6, Matrix3x2.Identity, CanvasGeometryCombine.Union);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the Placeholder geometry in the PlaceholderMask
    /// based on the progress value.
    /// </summary>
    /// <param name="progress">Progress value</param>
    private void ProgressHandler(int progress)
    {
        if (ShowPlaceholder)
        {
            _placeholderContentMask.Redraw(GetPlaceHolderGeometry(progress));
        }
    }

    /// <summary>
    /// Updates the state of the the ImageLoadEngine before loading an image
    /// </summary>
    private async void ScheduleNextLoad()
    {
        _scheduledUri = GetUriFromSource();

        switch (_engineState)
        {
            case ImageEngineState.Idle:
                _engineState = ImageEngineState.Loading;
                // Load the image
                await LoadImageAsync(_scheduledUri, _frameLayer.Size.ToSize());
                break;
            case ImageEngineState.Loading:
                _engineState = ImageEngineState.Scheduled;
                break;
            case ImageEngineState.Scheduled:
                break;
        }
    }

    /// <summary>
    /// Converts the Source value to appropriate Uri object
    /// </summary>
    /// <returns>Uri</returns>
    private Uri GetUriFromSource()
    {
        switch (Source)
        {
            case Uri uri:
                return uri;
            case string uriString:
                // Try creating the Uri from the uriString
                return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out var imgUri) ? imgUri : null;
            default:
                return null;
        }
    }

    /// <summary>
    /// Loads an image asynchronously from the given object for the given size
    /// </summary>
    /// <param name="scheduledUri">The next object to load</param>
    /// <param name="size">Render size of the image</param>
    /// <returns>Task</returns>
    private async Task LoadImageAsync(Uri scheduledUri, Size size)
    {
        try
        {
            bool raiseEvent = false;
            // Does the ImageFrame contain no previously rendered image?
            if (_imageSurface == null)
            {
                // Since a new object is being loaded, ImageOpened event must be raised on successful load 
                raiseEvent = true;
                // Show the placeholder, if required
                DisplayPlaceHolder();
                // Create the ImageSurface and get the latest Uri
                var latestUri = await LoadNextScheduledUriAsync(scheduledUri, size, false);

                // Uri was successfully loaded
                if ((latestUri != null) && (_imageSurface != null))
                {
                    // Set initial opacity to 0 so that the contentVisual can be faded in
                    _frameContentVisual.Opacity = 0;
                    // Apply the surfaceBrush to the visual
                    var surfaceBrush = _compositor.CreateSurfaceBrush(_imageSurface.Surface);
                    // Update the surface brush based on the Stretch and Alignment options
                    surfaceBrush.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY);
                    _frameContentVisual.Brush = surfaceBrush;
                    // Report 100% progress
                    ProgressHandler(100);
                    // Hide the placeholder
                    HidePlaceholder();

                    // If we are rendering fast, no need to animate
                    if (RenderFast)
                    {
                        _frameContentVisual.Opacity = 1;
                    }
                    else
                    {
                        // Start transition animation
                        StartTransition(true);
                    }
                }
                // Caching or Loading of the object failed
                else
                {
                    // Clear the existing image
                    ClearImageFrame();
                }
            }
            else
            {
                if (scheduledUri == null)
                {
                    // Clear the existing image
                    ClearImageFrame();
                }
                else
                {
                    // Check whether the object to load is same as the existing image
                    // loaded in the ImageSurface
                    if (_imageSurface.Uri.IsEqualTo(scheduledUri))
                    {
                        // Since the Uri has not changed, no need to raise the ImageOpened event
                        // Just resize the ImageSurface with the given imageOptions and
                        // update the frameContentVisual's brush
                        _imageSurface.Resize(size, _imageOptions);
                        // Update the surface brush based on the Stretch and Alignment options
                        var frameBrush = (_frameContentVisual.Brush as CompositionSurfaceBrush);
                        if (RenderFast)
                        {
                            // Use no animations to update alignment if we are rendering fast
                            frameBrush?.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY);
                        }
                        else
                        {
                            // Update stretch and alignment using animation
                            frameBrush?.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY, _alignXAnimation, _alignYAnimation);
                        }
                    }
                    else
                    {
                        // Since a different Uri is being loaded, then ImageOpened event
                        // must be raised on successful load
                        raiseEvent = true;
                        // Show the placeholder, if required
                        DisplayPlaceHolder();

                        // Create a temporary visual which loads the new Uri
                        _nextVisualContent.Opacity = 0;
                        // Load the object scheduled for load
                        var latestUri = await LoadNextScheduledUriAsync(scheduledUri, size, true);

                        // Object was successfully cached and loaded
                        if (latestUri != null)
                        {
                            // Create the surface brush for the next image
                            _nextSurfaceBrush = _compositor.CreateSurfaceBrush(_nextImageSurface.Surface);
                            // Update the surface brush based on the Stretch and Alignment options
                            _nextSurfaceBrush.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY);
                            _nextVisualContent.Brush = _nextSurfaceBrush;

                            // Report 100% progress
                            ProgressHandler(100);

                            // If we are rendering fast, then no need to animate.
                            if (RenderFast)
                            {
                                _frameContentVisual.Opacity = 0;
                                _nextVisualContent.Opacity = 1;
                                // apply the new brush to the frameVisualContent
                                _frameContentVisual.Brush = _nextSurfaceBrush;
                                // Update the surface image
                                _imageSurface = _nextImageSurface;
                                // Make the frameVisualContent visible again
                                _frameContentVisual.Opacity = 1;
                                // Hide the placeholder
                                HidePlaceholder();
                                // Hide the nextVisualContent
                                _nextVisualContent.Opacity = 0;
                            }
                            else
                            {
                                _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
                                    () =>
                                    {
                                        // Start transition animation
                                        StartTransition(false);
                                    },
                                    () =>
                                    {
                                        // apply the new brush to the frameVisualContent
                                        _frameContentVisual.Brush = _nextSurfaceBrush;
                                        // Update the surface image
                                        _imageSurface = _nextImageSurface;
                                        // Make the frameVisualContent visible again
                                        _frameContentVisual.Opacity = 1;
                                        // Hide the placeholder
                                        HidePlaceholder();
                                        // Hide the nextVisualContent
                                        _nextVisualContent.Opacity = 0;
                                    });
                            }
                        }
                        // Caching or Loading of the object failed
                        else
                        {
                            // Clear the existing image
                            ClearImageFrame();
                        }
                    }
                }
            }

            if (raiseEvent)
            {
                // Notify to subscribers that the image has been successfully loaded
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                () =>
                {
                    ImageOpened?.Invoke(this, new ImageFrameEventArgs(_imageSurface.Uri, string.Empty));
                });
            }
        }
        catch (Exception ex)
        {
            // Notify to subscribers that loading of the image failed
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                () =>
                {
                    ImageFailed?.Invoke(this, new ImageFrameEventArgs(_currentUri, ex.ToString()));
                });
        }

        // Update the engineState after load is completed
        PostLoadImage();
    }

    /// <summary>
    /// Loads the Uri scheduled for load. If after loading there is another
    /// Uri scheduled it loads that and keeps on repeating this until there
    /// are no more Uris scheduled for load.
    /// </summary>
    /// <param name="nextUri">Object scheduled to be loaded in the ImageFrame</param>
    /// <param name="size">Size of the ImageSurface</param>
    /// <param name="isLoadingNext">Whether loading in the imageSurface or the 
    /// nextImageSurface</param>
    /// <returns>Uri</returns>
    private async Task<Uri> LoadNextScheduledUriAsync(Uri nextUri, Size size, bool isLoadingNext)
    {
        bool loadNext;
        Uri lastUri;

        do
        {
            _currentUri = nextUri;

            if (isLoadingNext)
            {
                _nextImageSurface = await _generator.CreateImageSurfaceAsync(nextUri, size, _imageOptions);
            }
            else
            {
                _imageSurface = await _generator.CreateImageSurfaceAsync(nextUri, size, _imageOptions);
            }

            if ((isLoadingNext && _nextImageSurface.Status == ImageSurfaceLoadStatus.Error)
                || (!isLoadingNext && _imageSurface.Status == ImageSurfaceLoadStatus.Error))
            {
                // Notify to subscribers that loading of the image failed
                DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                    () =>
                    {
                        ImageFailed?.Invoke(this, new ImageFrameEventArgs(_currentUri, $"Failed to load image {_currentUri}"));
                    });
            }

            // Report 99% progress
            ProgressHandler(99);
            lastUri = nextUri;

            // Since the loading of the object takes some time, it could be possible
            // that a new object has been scheduled for load. In that case, discard
            // the current object and load the scheduled object
            if (_engineState == ImageEngineState.Scheduled)
            {
                loadNext = true;
                nextUri = _scheduledUri;
                _engineState = ImageEngineState.Loading;
            }
            else
            {
                loadNext = false;
            }
        } while (loadNext);

        return lastUri;
    }

    /// <summary>
    /// Updates the state of the the ImageLoadEngine after loading an image
    /// </summary>
    private async void PostLoadImage()
    {
        switch (_engineState)
        {
            case ImageEngineState.Idle:
                // Do Nothing
                _scheduledUri = null;
                break;
            case ImageEngineState.Loading:
                // Loading is complete. No image pending load.
                _engineState = ImageEngineState.Idle;
                _scheduledUri = null;
                // Now that the load has completed, Invalidate the Measure
                InvalidateMeasure();
                break;
            case ImageEngineState.Scheduled:
                // New image waiting in the pipeline to be rendered.
                _engineState = ImageEngineState.Loading;
                // Load the image
                await LoadImageAsync(_scheduledUri, _frameLayer.Size.ToSize());
                break;
        }
    }

    private void ClearImageFrame()
    {
        // Clear the progress in the Placeholder
        ProgressHandler(-1);

        if (RenderFast)
        {
            // Make the frameVisualContent transparent
            _frameContentVisual.Brush = _compositor.CreateColorBrush(Colors.Transparent);
            // Dispose the ImageSurface
            _imageSurface?.Dispose();
            _imageSurface = null;
            // Engine is now idle
            _engineState = ImageEngineState.Idle;
        }
        else
        {
            _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
                () =>
                {
                    // Fade out the frameVisualContent
                    _frameContentVisual.StartAnimation("Opacity", _fadeOutAnimation);
                },
                () =>
                {
                    // Make the frameVisualContent transparent
                    _frameContentVisual.Brush = _compositor.CreateColorBrush(Colors.Transparent);
                    //await _imageSurface.RedrawAsync(cachedUri, size, _imageOptions);
                    _frameContentVisual.Opacity = 1;
                    // Dispose the ImageSurface
                    _imageSurface?.Dispose();
                    _imageSurface = null;
                    // Engine is now idle
                    _engineState = ImageEngineState.Idle;
                });
        }
    }

    /// <summary>
    /// Begins the transition animation based on the TransitionMode
    /// </summary>
    /// <param name="isFirstLoad">Indicates whether the image is being
    /// loaded for the first time.</param>
    private void StartTransition(bool isFirstLoad)
    {
        SpriteVisual prevContent;
        SpriteVisual nextContent;

        if (isFirstLoad)
        {
            prevContent = null;
            nextContent = _frameContentVisual;
        }
        else
        {
            prevContent = _frameContentVisual;
            nextContent = _nextVisualContent;
        }

        switch (TransitionMode)
        {
            // New content fades into view
            case ImageFrameTransitionMode.FadeIn:
                nextContent.StartAnimation("Opacity", _fadeInAnimation);
                break;
            // New content slides from right to left
            case ImageFrameTransitionMode.SlideLeft:
                nextContent.Offset = new Vector3(nextContent.Size.X, 0, 0);
                nextContent.Opacity = 1;
                nextContent.StartAnimation("Offset", _offsetAnimation);
                break;
            // New content slides from left to right
            case ImageFrameTransitionMode.SlideRight:
                nextContent.Offset = new Vector3(-nextContent.Size.X, 0, 0);
                nextContent.Opacity = 1;
                nextContent.StartAnimation("Offset", _offsetAnimation);
                break;
            // New content slides up from bottom to top
            case ImageFrameTransitionMode.SlideUp:
                nextContent.Offset = new Vector3(0, nextContent.Size.Y, 0);
                nextContent.Opacity = 1;
                nextContent.StartAnimation("Offset", _offsetAnimation);
                break;
            // New content slides down from top to bottom
            case ImageFrameTransitionMode.SlideDown:
                nextContent.Offset = new Vector3(0, -nextContent.Size.Y, 0);
                nextContent.Opacity = 1;
                nextContent.StartAnimation("Offset", _offsetAnimation);
                break;
            // New content zooms into view
            case ImageFrameTransitionMode.ZoomIn:
                nextContent.Scale = new Vector3(MinScaleFactor, MinScaleFactor, 1);
                nextContent.Offset = new Vector3(nextContent.Size.X * (1 - MinScaleFactor) / 2f,
                    nextContent.Size.Y * (1 - MinScaleFactor) / 2f, 0);
                nextContent.Opacity = 1;
                nextContent.StartAnimationGroup(_zoomInAnimationGroup);
                break;
        }

        // Fade out the previous content (if any)
        prevContent?.StartAnimation("Opacity", _fadeOutAnimation);
    }

    /// <summary>
    /// If ShowPlaceholder option is enabled then displays the placeholder
    /// </summary>
    private void DisplayPlaceHolder()
    {
        if (!ShowPlaceholder)
            return;

        _placeholderContentVisual.Opacity = 1;
        // If we are rendering fast, then no need to animate 
        // placeholderBackgroundVisual's Opacity.
        // Just set the final value.
        if (RenderFast)
        {
            _placeholderBackgroundVisual.Opacity = 1;
        }
        else if (TransitionMode == ImageFrameTransitionMode.FadeIn)
        {
            _placeholderBackgroundVisual.StartAnimation("Opacity", _fadeInAnimation);
        }
    }

    /// <summary>
    /// Hides the Placeholder
    /// </summary>
    private void HidePlaceholder()
    {
        _placeholderContentVisual.Opacity = 0;
        _placeholderBackgroundVisual.Opacity = 0;
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
