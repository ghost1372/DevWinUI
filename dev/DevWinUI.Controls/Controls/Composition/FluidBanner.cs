using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml.Input;
using WinRT.Interop;

namespace DevWinUI;

public sealed partial class FluidBanner : Panel
{
    #region Constants

    private const float ScaleDownFactor = 0.7f;
    private const float HoverScaleFactor = 1.1f;
    private const double DefaultItemGap = 30;
    private const float TargetOpacity = 0f;
    private static readonly TimeSpan DefaultOpacityAnimationDuration = TimeSpan.FromMilliseconds(900);
    private static readonly TimeSpan DefaultScaleAnimationDuration = TimeSpan.FromMilliseconds(900);
    private static readonly TimeSpan InsetClipAnimationDuration = TimeSpan.FromMilliseconds(600);
    private static readonly TimeSpan InsetAnimationDuration = TimeSpan.FromMilliseconds(500);
    private static readonly TimeSpan InsetAnimationDelayDuration = TimeSpan.FromMilliseconds(500);
    private static readonly Color DefaultItemBackground = Colors.Black;

    #endregion

    #region Fields

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private ContainerVisual _rootContainer;
    private LayerVisual _bgLayer;
    private LayerVisual _topLayer;
    private ImplicitAnimationCollection _implicitAnimationCollection;
    private ContainerVisual _selectedVisual;
    private ContainerVisual _hoverVisual;
    private Dictionary<IImageSurface, SpriteVisual> _surfaceVisuals;
    private Dictionary<ContainerVisual, Rect> _fluidItems;
    private List<IImageSurface> _surfaceImages;
    private Rect _bannerBounds;
    private int _itemCount;
    private float _itemWidth;
    private float _itemHeight;
    private float _availableWidth;
    private float _availableHeight;
    private bool _isExpanding;
    private bool _isCollapsing;
    private bool _surfaceImagesCreated;
    // Animations
    private ScalarKeyFrameAnimation _expandLeftInset;
    private ScalarKeyFrameAnimation _expandRightInset;
    private ScalarKeyFrameAnimation _expandInsetClip;
    private ScalarKeyFrameAnimation _collapseLeftInset;
    private ScalarKeyFrameAnimation _collapseRightInset;
    private ScalarKeyFrameAnimation _collapseInsetClip;

    #endregion

    #region Events

    /// <summary>
    /// Event which indicates that the items in the ItemsSource have started loading
    /// </summary>
    public event EventHandler ItemsLoading;
    /// <summary>
    /// Event which indicates that the items in the ItemsSource have completed loading
    /// </summary>
    public event EventHandler ItemsLoaded;

    #endregion

    #region Dependency Properties

    #region AlignX

    /// <summary>
    /// AlignX Dependency Property
    /// </summary>
    public static readonly DependencyProperty AlignXProperty =
        DependencyProperty.Register("AlignX", typeof(AlignmentX), typeof(FluidBanner),
            new PropertyMetadata(AlignmentX.Center, OnAlignXChanged));

    /// <summary>
    /// Gets or sets the AlignX property. This dependency property 
    /// indicates the horizontal alignment of the image within each item.
    /// </summary>
    public AlignmentX AlignX
    {
        get => (AlignmentX)GetValue(AlignXProperty);
        set => SetValue(AlignXProperty, value);
    }

    /// <summary>
    /// Handles changes to the AlignX property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnAlignXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var banner = (FluidBanner)d;
        banner.OnAlignXChanged();
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
        DependencyProperty.Register("AlignY", typeof(AlignmentY), typeof(FluidBanner),
            new PropertyMetadata(AlignmentY.Center, OnAlignYChanged));

    /// <summary>
    /// Gets or sets the AlignY property. This dependency property 
    /// indicates the vertical alignment of the image within each item.
    /// </summary>
    public AlignmentY AlignY
    {
        get => (AlignmentY)GetValue(AlignYProperty);
        set => SetValue(AlignYProperty, value);
    }

    /// <summary>
    /// Handles changes to the AlignY property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnAlignYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var banner = (FluidBanner)d;
        banner.OnAlignYChanged();
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

    #region ItemBackground

    /// <summary>
    /// ItemBackground Dependency Property
    /// </summary>
    public static readonly DependencyProperty ItemBackgroundProperty =
        DependencyProperty.Register("ItemBackground", typeof(Color), typeof(FluidBanner),
            new PropertyMetadata(DefaultItemBackground, OnItemBackgroundChanged));

    /// <summary>
    /// Gets or sets the ItemBackground property. This dependency property 
    /// indicates the color for the background of the item.
    /// </summary>
    public Color ItemBackground
    {
        get => (Color)GetValue(ItemBackgroundProperty);
        set => SetValue(ItemBackgroundProperty, value);
    }

    /// <summary>
    /// Handles changes to the ItemBackground property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnItemBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (FluidBanner)d;
        panel.OnItemBackgroundChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ItemBackground property.
    /// </summary>
    private void OnItemBackgroundChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ItemGap

    /// <summary>
    /// ItemGap Dependency Property
    /// </summary>
    public static readonly DependencyProperty ItemGapProperty =
        DependencyProperty.Register("ItemGap", typeof(double), typeof(FluidBanner),
            new PropertyMetadata(DefaultItemGap, OnItemGapChanged));

    /// <summary>
    /// Gets or sets the ItemGap property. This dependency property 
    /// indicates the gap between adjacent items in the banner.
    /// </summary>
    public double ItemGap
    {
        get => (double)GetValue(ItemGapProperty);
        set => SetValue(ItemGapProperty, value);
    }

    /// <summary>
    /// Handles changes to the ItemGap property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnItemGapChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (FluidBanner)d;
        panel.OnItemGapChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ItemGap property.
    /// </summary>
    private void OnItemGapChanged()
    {
        // Refresh Layout
        InvalidateArrange();
    }

    #endregion

    #region ItemsSource

    /// <summary>
    /// ItemsSource Dependency Property
    /// </summary>
    public static readonly DependencyProperty ItemsSourceProperty =
        DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Uri>), typeof(FluidBanner),
            new PropertyMetadata(null, OnItemsSourceChanged));

    /// <summary>
    /// Gets or sets the ItemsSource property. This dependency property 
    /// indicates the collection of Uris of images to be shown in the banner.
    /// </summary>
    public IEnumerable<Uri> ItemsSource
    {
        get => (IEnumerable<Uri>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    /// <summary>
    /// Handles changes to the ItemsSource property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (FluidBanner)d;
        panel.OnItemsSourceChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the ItemsSource property.
    /// </summary>
    private async void OnItemsSourceChanged()
    {
        _surfaceImagesCreated = false;

        ItemsLoading?.Invoke(this, null);

        // Create SurfaceImages
        await CreateSurfaceImagesAsync();

        // Refresh Layout
        InvalidateArrange();

        ItemsLoaded?.Invoke(this, null);
    }

    #endregion

    #region Padding

    /// <summary>
    /// Padding Dependency Property
    /// </summary>
    public static readonly DependencyProperty PaddingProperty =
        DependencyProperty.Register("Padding", typeof(Thickness), typeof(FluidBanner),
            new PropertyMetadata(new Thickness(), OnPaddingChanged));

    /// <summary>
    /// Gets or sets the Padding property. This dependency property 
    /// indicates the Padding thickness for the content.
    /// </summary>
    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }

    /// <summary>
    /// Handles changes to the Padding property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnPaddingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var panel = (FluidBanner)d;
        panel.OnPaddingChanged();
    }

    /// <summary>
    /// Provides the class instance an opportunity to handle changes to the Padding property.
    /// </summary>
    private void OnPaddingChanged()
    {
        InvalidateArrange();
    }

    #endregion

    #region Stretch

    /// <summary>
    /// Stretch Dependency Property
    /// </summary>
    public static readonly DependencyProperty StretchProperty =
        DependencyProperty.Register("Stretch", typeof(Stretch), typeof(FluidBanner),
            new PropertyMetadata(Stretch.Uniform, OnStretchChanged));

    /// <summary>
    /// Gets or sets the Stretch property. This dependency property 
    /// indicates how the image is resized to fill the item.
    /// </summary>
    public Stretch Stretch
    {
        get => (Stretch)GetValue(StretchProperty);
        set => SetValue(StretchProperty, value);
    }

    /// <summary>
    /// Handles changes to the Stretch property.
    /// </summary>
    /// <param name="d">FluidBanner</param>
    /// <param name="e">DependencyProperty changed event arguments</param>
    private static void OnStretchChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var banner = (FluidBanner)d;
        banner.OnStretchChanged();
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

    #endregion

    #region Construction / Initialization

    /// <summary>
    /// Ctor
    /// </summary>
    public FluidBanner()
    {
        _fluidItems = new Dictionary<ContainerVisual, Rect>();
        _surfaceImages = new List<IImageSurface>();
        _surfaceVisuals = new Dictionary<IImageSurface, SpriteVisual>();
        Loaded += OnBannerLoaded;
    }

    #endregion

    #region Overrides

    /// <summary>
    /// Handles the Measure pass during Layout
    /// </summary>
    /// <param name="availableSize">Available size</param>
    /// <returns>Total Size required to accommodate all the Children</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        // Compositor will be null the very first time
        if (_compositor == null)
        {
            InitializeComposition();
        }

        return availableSize;
    }

    /// <summary>
    /// Handles the Arrange pass during Layout
    /// </summary>
    /// <param name="finalSize">Final Size of the control</param>
    /// <returns>Total size occupied by all the Children</returns>
    protected override Size ArrangeOverride(Size finalSize)
    {
        if ((ItemsSource == null) || !ItemsSource.Any())
            return finalSize;

        // Resize the rootContainer
        _rootContainer.Size = finalSize.ToVector2();

        var padding = Padding;

        var paddingSize = padding.CollapseThickness();
        var width = finalSize.Width;
        var height = finalSize.Height;
        _itemCount = ItemsSource.Count();

        _availableWidth = (width - paddingSize.Width).ToSingle();
        _availableHeight = (height - paddingSize.Height).ToSingle();
        var gapCount = _itemCount - 1;
        var totalGap = (gapCount * ItemGap).ToSingle();
        var availableNonGapWidth = _availableWidth - totalGap;
        var availableNonGapHeight = _availableHeight;

        _itemWidth = availableNonGapWidth / _itemCount;
        _itemHeight = availableNonGapHeight;
        _bannerBounds = new Rect(padding.Left, padding.Top, _availableWidth, _availableHeight);

        // Update Visual sizes and surfaceImages
        var availableSize = new Vector2(_availableWidth, _availableHeight);
        _rootContainer.Size = availableSize;
        _bgLayer.Size = availableSize;
        _bgLayer.CenterPoint = new Vector3(_bgLayer.Size * 0.5f, 0);
        _topLayer.Size = availableSize;

        // Check if any visual is currently selected. If yes then obtain its index.
        // The new visual at that index should be selected and added to the player
        var selectedVisualIndex = -1;
        if (_selectedVisual != null)
        {
            foreach (var item in _fluidItems)
            {
                selectedVisualIndex++;
                if (ReferenceEquals(item.Key, _selectedVisual))
                    break;
            }
        }

        for (var index = 0; index < _surfaceImages.Count; index++)
        {
            // Get the surfaceImage
            var surfaceImage = _surfaceImages.ElementAt(index);

            // Get the visual corresponding to this surfaceImage
            var itemVisual = _surfaceVisuals[surfaceImage];

            if (itemVisual == null)
                continue;

            itemVisual.Size = availableSize;
            itemVisual.CenterPoint = new Vector3(itemVisual.Size * 0.5f, 0);
            if (!(itemVisual.Children.ElementAt(0) is SpriteVisual contentVisual))
                continue;

            contentVisual.Size = availableSize;
            contentVisual.CenterPoint = itemVisual.CenterPoint;
            (contentVisual.Brush as CompositionSurfaceBrush)?.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY);

            // Calculate the Inset Clip
            var left = index * (_itemWidth + ItemGap.ToSingle());
            var right = _availableWidth - (left + _itemWidth);
            itemVisual.Properties.InsertScalar("LeftInset", left);
            itemVisual.Properties.InsertScalar("RightInset", right);
            itemVisual.Clip = (index == selectedVisualIndex)
                ? _compositor.CreateInsetClip(0, 0, 0, 0)
                : _compositor.CreateInsetClip(left, 0, right, 0);

            // Center Point
            itemVisual.CenterPoint = new Vector3(left + (_itemWidth / 2f), (float)padding.Top + (_itemHeight / 2f), 0);
            contentVisual.CenterPoint = itemVisual.CenterPoint;

            // Update Fluid Items
            _fluidItems[itemVisual] = new Rect(padding.Left + left, padding.Top, _itemWidth, _itemHeight);
        }

        // Update Animations' keyframes
        _expandRightInset.InsertKeyFrame(1f, _availableWidth - _itemWidth);
        _collapseInsetClip.InsertKeyFrame(1f, _availableWidth - _itemWidth);

        return finalSize;
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Handle the event when the control is loaded.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">RoutedEventArgs</param>
    private void OnBannerLoaded(object sender, RoutedEventArgs e)
    {
        // Track the pointer pressed event
        PointerPressed += OnPointerPressed;
        PointerMoved += OnPointerMoved;
    }

    /// <summary>
    /// Handles the PointerMoved event
    /// </summary>
    /// <param name="sender">FluidBanner</param>
    /// <param name="e">PointerRoutedEventArgs</param>
    private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
    {
        if ((ItemsSource == null) || (!ItemsSource.Any()) ||
            _isExpanding || _isCollapsing || _selectedVisual != null)
            return;

        var point = e.GetCurrentPoint(this).Position;

        HandlePointerPosition(point);
    }

    /// <summary>
    /// Handles the PointerPressed event
    /// </summary>
    /// <param name="sender">FluidBanner</param>
    /// <param name="e">PointerRoutedEventArgs</param>
    private async void OnPointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if ((ItemsSource == null) || (!ItemsSource.Any()) ||
            _isExpanding || _isCollapsing)
            return;

        var point = e.GetCurrentPoint(this).Position;

        if (_selectedVisual == null)
        {
            _selectedVisual = _fluidItems.Where(t => t.Value.Contains(point))
                                         .Select(t => t.Key)
                                         .FirstOrDefault();

            if (_selectedVisual != null)
            {
                ResetHover(_hoverVisual);
                _hoverVisual = null;
                await SelectBannerItem();
            }
        }
        else
        {
            if (_bannerBounds.Contains(point))
            {
                ResetBannerItems(e.GetCurrentPoint(this).Position);
            }
        }
    }

    #endregion

    #region Helpers

    /// <summary>
    /// Initialize all Composition related stuff here (Compositor, Animations etc)
    /// </summary>
    private void InitializeComposition()
    {
        var rootVisual = ElementCompositionPreview.GetElementVisual(this);
        // Compositor
        _compositor = rootVisual.Compositor;
        // Composition Generator
        _generator = _compositor.CreateCompositionGenerator();

        // Final Value Expressions
        var vector3Expr = _compositor.CreateFinalValueExpression<Vector3>();
        var scalarExpr = _compositor.CreateFinalValueExpression<float>();

        // Opacity Animation
        var opacityAnimation = _compositor.CreateKeyFrameAnimation<float>()
                                          .HavingDuration(DefaultOpacityAnimationDuration)
                                          .ForTarget(() => rootVisual.Opacity);
        opacityAnimation.InsertExpressionKeyFrame(1f, scalarExpr);

        // Scale Animation
        var scaleAnimation = _compositor.CreateKeyFrameAnimation<Vector3>()
                                        .HavingDuration(DefaultScaleAnimationDuration)
                                        .ForTarget(() => rootVisual.Scale);
        scaleAnimation.InsertExpressionKeyFrame(1f, vector3Expr);

        // ImplicitAnimation
        _implicitAnimationCollection = _compositor.CreateImplicitAnimationCollection();
        _implicitAnimationCollection["Opacity"] = opacityAnimation.Animation;
        _implicitAnimationCollection["Scale"] = scaleAnimation.Animation;

        // Expand Animations
        _expandLeftInset = _compositor.CreateScalarKeyFrameAnimation();
        _expandLeftInset.Duration = InsetAnimationDuration;
        _expandLeftInset.DelayTime = InsetAnimationDelayDuration;

        _expandLeftInset.InsertKeyFrame(1f, 0);

        _expandRightInset = _compositor.CreateScalarKeyFrameAnimation();
        _expandRightInset.Duration = InsetAnimationDuration;
        _expandRightInset.DelayTime = InsetAnimationDelayDuration;

        _expandInsetClip = _compositor.CreateScalarKeyFrameAnimation();
        _expandInsetClip.Duration = InsetClipAnimationDuration;

        _expandInsetClip.InsertKeyFrame(1f, 0);

        // Collapse Animations
        _collapseLeftInset = _compositor.CreateScalarKeyFrameAnimation();
        _collapseLeftInset.Duration = InsetAnimationDuration;

        _collapseRightInset = _compositor.CreateScalarKeyFrameAnimation();
        _collapseRightInset.Duration = InsetAnimationDuration;

        _collapseInsetClip = _compositor.CreateScalarKeyFrameAnimation();
        _collapseInsetClip.Duration = InsetClipAnimationDuration;

        // Root Container
        _rootContainer = _compositor.CreateContainerVisual();

        // Background Layer
        _bgLayer = _compositor.CreateLayerVisual();
        _bgLayer.Size = _rootContainer.Size;
        _bgLayer.CenterPoint = new Vector3(_bgLayer.Size * 0.5f, 0);

        // Top Layer
        _topLayer = _compositor.CreateLayerVisual();
        _topLayer.Size = _rootContainer.Size;

        _rootContainer.Children.InsertAtBottom(_bgLayer);
        _rootContainer.Children.InsertAtTop(_topLayer);

        // Add the rootContainer to the visual tree
        ElementCompositionPreview.SetElementChildVisual(this, _rootContainer);
    }

    /// <summary>
    /// Creates the SurfaceImages based on the ItemsSource
    /// </summary>
    /// <returns>Task</returns>
    private async Task CreateSurfaceImagesAsync()
    {
        if (_surfaceImagesCreated || _bgLayer == null)
            return;

        var padding = Padding;
        // Clear previous visuals
        _surfaceVisuals.Clear();
        _fluidItems.Clear();
        _bgLayer.Children.RemoveAll();
        _topLayer.Children.RemoveAll();
        _selectedVisual = null;

        // Dispose the surface images
        for (var i = 0; i < _surfaceImages.Count; i++)
        {
            var surfaceImage = _surfaceImages.ElementAt(i);
            surfaceImage.Dispose();
        }
        _surfaceImages.Clear();

        if ((ItemsSource == null) || !ItemsSource.Any())
            return;

        var options = ImageSurfaceOptions.Default;
        options.SurfaceBackgroundColor = Colors.Transparent;

        var availableSize = new Size(_availableWidth, _availableHeight);
        for (var i = 0; i < ItemsSource.Count(); i++)
        {
            // Create the surface image
            var surfaceImage = await _generator.CreateImageSurfaceAsync(ItemsSource.ElementAt(i), availableSize, options);
            _surfaceImages.Add(surfaceImage);

            // Add a visual for the background
            var containerVisual = _compositor.CreateSpriteVisual();
            containerVisual.Size = availableSize.ToVector2();
            containerVisual.Brush = _compositor.CreateColorBrush(ItemBackground);
            containerVisual.Offset = new Vector3((float)padding.Left, (float)padding.Top, 0);

            // Create the visual for the content
            var contentVisual = _compositor.CreateSpriteVisual();
            contentVisual.Size = availableSize.ToVector2();
            var surfaceBrush = _compositor.CreateSurfaceBrush(surfaceImage.Surface);
            surfaceBrush.UpdateSurfaceBrushOptions(Stretch, AlignX, AlignY);
            contentVisual.Brush = surfaceBrush;

            // Calculate the Inset Clip
            var left = i * (_itemWidth + ItemGap.ToSingle());
            var right = _availableWidth - (left + _itemWidth);
            containerVisual.Properties.InsertScalar("LeftInset", left);
            containerVisual.Properties.InsertScalar("RightInset", right);
            containerVisual.Clip = _compositor.CreateInsetClip(left, 0, right, 0);

            // Center Point
            containerVisual.CenterPoint = new Vector3(left + (_itemWidth / 2f), (float)padding.Top + (_itemHeight / 2f), 0);
            contentVisual.CenterPoint = containerVisual.CenterPoint;
            containerVisual.ImplicitAnimations = _implicitAnimationCollection;
            contentVisual.ImplicitAnimations = _implicitAnimationCollection;

            // Set the container content
            containerVisual.Children.InsertAtTop(contentVisual);

            // Add to Background Layer
            _bgLayer.Children.InsertAtTop(containerVisual);

            // Add Visual to the fluid items
            _fluidItems[containerVisual] = new Rect(padding.Left + left, padding.Top, _itemWidth, _itemHeight);
            _surfaceVisuals[surfaceImage] = containerVisual;
        }
    }

    /// <summary>
    /// Checks if the current Pointer location is above any of the FluidBanner items
    /// and takes appropriate action
    /// </summary>
    /// <param name="point">Current Pointer location</param>
    private void HandlePointerPosition(Point point)
    {
        var visual = _fluidItems.Where(t => t.Value.Contains(point))
                                            .Select(t => t.Key)
                                            .FirstOrDefault();

        // Pointer not on any visual
        if (visual == null)
        {
            if (_hoverVisual != null)
            {
                ResetHover(_hoverVisual);
                _hoverVisual = null;
            }
        }
        else
        {
            if (ReferenceEquals(_hoverVisual, visual))
                return;

            if (_hoverVisual != null)
            {
                ResetHover(_hoverVisual);
            }

            _hoverVisual = visual;
            Hover(_hoverVisual);
        }
    }

    /// <summary>
    /// Handles the Hover animation for the given visual
    /// </summary>
    /// <param name="visual">Visual to animate</param>
    private void Hover(ContainerVisual visual)
    {
        if (visual == null)
            return;

        var topIndex = visual.Children.Count - 1;
        var child = visual.Children.ElementAt(topIndex);
        if (child != null)
        {
            child.Scale = new Vector3(HoverScaleFactor, HoverScaleFactor, 1);
        }
    }

    /// <summary>
    /// Handles the Reset Hover animation for the given visual
    /// </summary>
    /// <param name="visual">Visual to animate</param>
    private void ResetHover(ContainerVisual visual)
    {
        if (visual == null)
            return;

        var topIndex = visual.Children.Count - 1;
        var child = visual.Children.ElementAt(topIndex);
        if (child != null)
        {
            child.Scale = Vector3.One;
        }
    }

    /// <summary>
    /// Performs the required animations on the selected Visual
    /// </summary>
    /// <returns>Task</returns>
    private async Task SelectBannerItem()
    {
        if (_selectedVisual == null)
            return;
        _isExpanding = true;

        // Bring the selected visual from background layer to the top layer
        _bgLayer.Children.Remove(_selectedVisual);
        _topLayer.Children.InsertAtTop(_selectedVisual);

        // Scale back and fade the remaining visuals in the banner
        foreach (var child in _bgLayer.Children)
        {
            child.Scale = new Vector3(ScaleDownFactor, ScaleDownFactor, 1);
            child.Opacity = TargetOpacity;
        }

        // If the first visual is selected, then no need to move it to the leftmost
        // position as it is already there. Just do the expand animation.
        if (ReferenceEquals(_selectedVisual, _fluidItems.Keys.ElementAt(0)))
        {
            // Delay the expand animation by the DefaultScaleAnimationDuration 
            // so that the other visuals in the Banner scale down
            await Task.Delay((int)DefaultScaleAnimationDuration.TotalMilliseconds);

            _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
                () =>
                {
                    // Expand the visual to occupy the full size of the banner
                    _selectedVisual.Clip.StartAnimation("RightInset", _expandInsetClip);
                },
                () =>
                {
                    _isExpanding = false;
                });

        }
        else
        {
            _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
                () =>
                {
                    // Animate the visual to move to the left most position
                    _selectedVisual.Clip.StartAnimation("LeftInset", _expandLeftInset);
                    _selectedVisual.Clip.StartAnimation("RightInset", _expandRightInset);
                },
                () =>
                {
                    _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
                        () =>
                        {
                            // Expand the visual to occupy the full size of the banner
                            _selectedVisual.Clip.StartAnimation("RightInset", _expandInsetClip);
                        },
                        () =>
                        {
                            _isExpanding = false;
                        });

                });
        }
    }

    /// <summary>
    /// Performs to the required animation to reset selection on the Selected Visual
    /// </summary>
    private void ResetBannerItems(Point point)
    {
        if (_selectedVisual == null)
            return;

        _isCollapsing = true;

        // Get the value to set for the LeftInset of the InsetClip
        _selectedVisual.Properties.TryGetScalar("LeftInset", out var left);
        _collapseLeftInset.InsertKeyFrame(1f, left);

        // Get the value to set for the RightInset of the InsetClip
        _selectedVisual.Properties.TryGetScalar("RightInset", out var right);
        _collapseRightInset.InsertKeyFrame(1f, right);

        // Use a scoped batch helper to queue up animations one after the other
        _compositor.CreateScopedBatch(CompositionBatchTypes.Animation,
        () =>
        {
            // Collapse the visual
            _selectedVisual.Clip.StartAnimation("RightInset", _collapseInsetClip);
        },
        async () =>
        {
            // If the first visual is selected, then no need to move it to the leftmost
            // position as it is already there.
            if (!ReferenceEquals(_selectedVisual, _fluidItems.Keys.ElementAt(0)))
            {
                // Move the visual back to its original location in the banner
                _selectedVisual.Clip.StartAnimation("LeftInset", _collapseLeftInset);
                _selectedVisual.Clip.StartAnimation("RightInset", _collapseRightInset);
            }

            // Fade in the other visuals and scale them to normal size
            foreach (var child in _bgLayer.Children)
            {
                child.Scale = Vector3.One;
                child.Opacity = 1;
            }

            // Delay the exchange, of selected visual, between the top and background layers
            // by the DefaultScaleAnimationDuration so that other visuals in 
            // the Banner scale back to normal
            await Task.Delay((int)DefaultScaleAnimationDuration.TotalMilliseconds);

            // Bring the selected visual from top layer to the background layer
            _topLayer.Children.Remove(_selectedVisual);
            _bgLayer.Children.InsertAtTop(_selectedVisual);

            _selectedVisual = null;
            _isCollapsing = false;

            // Check the current location of the Pointer and whether it is hovering
            // over any of the FluidBanner items
            HandlePointerPosition(GetPointerPosition(point));
        });
    }

    /// <summary>
    /// Gets the current location of the Pointer
    /// </summary>
    /// <returns>Point</returns>
    private Point GetPointerPosition(Point screenPoint)
    {
        // 1. If screenPoint is invalid (e.g., computer locked)
        if (double.IsNegativeInfinity(screenPoint.X) ||
            double.IsNegativeInfinity(screenPoint.Y))
        {
            return new Point(double.NegativeInfinity, double.NegativeInfinity);
        }

        // 2. Get the Window for this control
        var window = WindowHelper.GetWindowForElement(this);
        if (window is null)
            return new Point(double.NegativeInfinity, double.NegativeInfinity);

        // 3. Get HWND safely
        var hwnd = WindowNative.GetWindowHandle(window);
        if (hwnd == IntPtr.Zero)
            return new Point(double.NegativeInfinity, double.NegativeInfinity);

        // 4. Get AppWindow
        var windowId = Win32Interop.GetWindowIdFromWindow(hwnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        if (appWindow is null)
            return new Point(double.NegativeInfinity, double.NegativeInfinity);

        // 5. Bounds from AppWindow
        var bounds = appWindow.Position;

        // 6. Transform to visual (this can be null after template reapply!)
        var transformToWindow = this.TransformToVisual(null);
        if (transformToWindow is null)
            return new Point(double.NegativeInfinity, double.NegativeInfinity);

        var controlOffset = transformToWindow.TransformPoint(new Point(0, 0));

        // 7. Convert screen → control
        var localX = screenPoint.X - bounds.X - controlOffset.X;
        var localY = screenPoint.Y - bounds.Y - controlOffset.Y;

        return new Point(localX, localY);
    }



    #endregion
}
