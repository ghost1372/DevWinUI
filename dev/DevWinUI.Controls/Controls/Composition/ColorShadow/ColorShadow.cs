using System.Linq.Expressions;

namespace DevWinUI;

[TemplatePart(Name = RenderGrid, Type = typeof(Grid))]
[TemplateVisualState(Name = "Initialized", GroupName = "ContentStates")]
[TemplateVisualState(Name = "Loading", GroupName = "ContentStates")]
[TemplateVisualState(Name = "Loaded", GroupName = "ContentStates")]
public partial class ColorShadow : Control
{
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

    private Compositor _compositor;
    private ICompositionGenerator _generator;
    private IImageSurface _imageSurface;
    private ContainerVisual _rootVisual;
    private SpriteVisual _colorShadowVisual;
    private SpriteVisual _imageVisual;
    private CompositionEffectBrush _colorShadowBrush;
    private DropShadow _dropShadow;
    private ScalarKeyFrameAnimation _colorShadowBlurAnimation;
    private readonly ImageSurfaceOptions _maskOptions = ImageSurfaceOptions.DefaultImageMaskOptions;

    private Grid _renderGrid;
    private bool _imageSourceLoaded;
    private readonly SemaphoreSlim _syncObject = new SemaphoreSlim(1);
    private bool _isLoading;
    private IImageMaskSurface _imageMaskSurface;
    private bool forceUpdateMask = false;
    public ColorShadow()
    {
        // Set the default Style Key
        DefaultStyleKey = typeof(ColorShadow);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _renderGrid = GetTemplateChild(RenderGrid) as Grid;
        // If the ImageUri property is already assigned a value and the image is already
        // loading, then go to the Loading state instead of the Initialized state.
        VisualStateManager.GoToState(this, _isLoading ? "Loading" : "Initialized", true);
    }

    private async Task<bool> OnImageUriChangedAsync(Uri imageUri)
    {
        return await LoadImageAsync(imageUri);
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
        var size = base.ArrangeOverride(finalSize);

        BaseArrangeOverride(finalSize);

        return size;
    }

    private async void BaseArrangeOverride(Size finalSize)
    {
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
            _imageVisual.Offset = new Vector3((float)ImageLayoutOffsetX, (float)ImageLayoutOffsetY, (float)ImageLayoutOffsetZ);

            var imageBrush = _compositor.CreateSurfaceBrush(_imageSurface);
            imageBrush.Stretch = ImageStretch;
            imageBrush.Offset = new Vector2((float)ImageOffsetX, (float)ImageOffsetY);

            IImageSurface maskSurface = await ApplyMaskAsync(imageBrush);

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

            void CreateShadowMask()
            {
                _imageMaskSurface?.Dispose();
                _imageMaskSurface = null;

                // Create the ImageMaskSurface 

                _imageMaskSurface = _generator.CreateImageMaskSurface(maskSurface ?? _imageSurface, _colorShadowVisual.Size.ToSize(), ColorMaskPadding, _maskOptions);

                // Create SurfaceBrush
                var mask = _compositor.CreateSurfaceBrush(_imageMaskSurface);
                // Set the Mask
                _colorShadowBrush.SetSourceParameter("mask", mask);
            }

            if (_imageMaskSurface == null)
            {
                CreateShadowMask();
            }
            else
            {
                if (forceUpdateMask)
                {
                    CreateShadowMask();
                    forceUpdateMask = false;
                }
                else
                {
                    // Update the ImageMaskSurface
                    _imageMaskSurface.Resize(_colorShadowVisual.Size.ToSize(), ColorMaskPadding, _maskOptions);
                }
            }

            _colorShadowVisual.Brush = _colorShadowBrush;
            _colorShadowVisual.Opacity = ColorShadowOpacity.ToSingle();

            _colorShadowBlurAnimation = _compositor.CreateScalarKeyFrameAnimation();
            _colorShadowBlurAnimation.Duration = TimeSpan.FromMilliseconds(100);

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
    }

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

    private async Task<IImageSurface> ApplyMaskAsync(CompositionBrush imageBrush)
    {
        if (IsRounded)
        {
            // Force circular mask, ignore user mask completely
            var circleSurface = await LoadCircleMaskAsync();

            var maskBrush = _compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = _compositor.CreateSurfaceBrush(circleSurface);

            _imageVisual.Brush = maskBrush;
            return circleSurface;
        }

        // Not rounded → check if user provided Mask
        if (Mask != null)
        {
            var userSurface = await LoadUserMaskAsync();

            var maskBrush = _compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = _compositor.CreateSurfaceBrush(userSurface);

            _imageVisual.Brush = maskBrush;
            return userSurface;
        }
        else
        {
            // No mask at all
            _imageVisual.Brush = imageBrush;
            return null;
        }
    }

    private async Task<IImageSurface> LoadCircleMaskAsync()
    {
        return await LoadMaskInternalAsync(new Uri("ms-appx:///Assets/Mask/CircleMask.png"));
    }

    private async Task<IImageSurface> LoadUserMaskAsync()
    {
        return await LoadMaskInternalAsync(Mask);
    }
    private async Task<IImageSurface> LoadMaskInternalAsync(Uri uri)
    {
        var assembly = typeof(ColorShadow).Assembly;

        var stream = FileHelper.GetFileFromEmbededResourcesOrUri(uri, assembly);

        CanvasBitmap canvasBitmap = null;
        var canvasDevice = CanvasDevice.GetSharedDevice();

        if (stream != null)
        {
            canvasBitmap = await CanvasBitmap.LoadAsync(canvasDevice, stream.AsRandomAccessStream());
        }
        else
        {
            canvasBitmap = await CanvasBitmap.LoadAsync(canvasDevice, uri);
        }

        return _generator.CreateImageSurface(canvasBitmap, canvasBitmap.Size, ImageSurfaceOptions.Default);
    }
}
