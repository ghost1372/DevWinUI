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

    private Compositor? _compositor;
    private ICompositionGenerator? _generator;
    private IImageSurface? _imageSurface;
    private ContainerVisual? _rootVisual;
    private SpriteVisual? _colorShadowVisual;
    private SpriteVisual? _imageVisual;
    private CompositionEffectBrush? _colorShadowBrush;
    private DropShadow? _dropShadow;
    private ScalarKeyFrameAnimation? _colorShadowBlurAnimation;
    private readonly ImageSurfaceOptions _maskOptions = ImageSurfaceOptions.DefaultImageMaskOptions;

    private Grid? _renderGrid;
    private bool _imageSourceLoaded;
    private readonly SemaphoreSlim _syncObject = new SemaphoreSlim(1);
    private bool _isLoading;
    private IImageMaskSurface? _imageMaskSurface;
    private bool forceUpdateMask = false;
    private ScalarKeyFrameAnimation? _breathAnimation;

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

        UpdateIsBreathing();
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
        var (compositor, generator, dropShadow) = EnsureComposition();

        var imageSurface = _imageSurface;
        if (!_imageSourceLoaded || imageSurface == null)
        {
            return;
        }

        // Taking into account the BorderThickness Padding
        var padding = Padding;
        var paddingSize = padding.CollapseThickness();

        // Calculate the Dimensions of the frameVisual
        var srcWidth = imageSurface.DecodedPhysicalSize.Width;
        var srcHeight = imageSurface.DecodedPhysicalSize.Height;
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

        var rootVisual = _rootVisual ??= compositor.CreateContainerVisual();
        rootVisual.Size = targetSize;
        rootVisual.Offset = new Vector3(left, top, 0f);

        // Image Layer
        var imageVisual = _imageVisual ??= compositor.CreateSpriteVisual();
        imageVisual.Size = targetSize;
        imageVisual.Offset = new Vector3((float)ImageLayoutOffsetX, (float)ImageLayoutOffsetY, (float)ImageLayoutOffsetZ);

        var imageBrush = compositor.CreateSurfaceBrush(imageSurface);
        imageBrush.Stretch = ImageStretch;
        imageBrush.Offset = new Vector2((float)ImageOffsetX, (float)ImageOffsetY);

        IImageSurface? maskSurface = await ApplyMaskAsync(compositor, generator, imageVisual, imageBrush);

        // Color Shadow Layer
        var colorShadowVisual = _colorShadowVisual ??= compositor.CreateSpriteVisual();

        var colorPaddingSize = ColorShadowPadding.CollapseThickness();
        colorShadowVisual.Size = new Vector2(targetSize.X + colorPaddingSize.Width.ToSingle(), targetSize.Y + colorPaddingSize.Height.ToSingle());
        colorShadowVisual.Offset = new Vector3(-ColorShadowPadding.Left.ToSingle(), -ColorShadowPadding.Top.ToSingle(), 0);

        var colorShadowBrush = _colorShadowBrush;
        if (colorShadowBrush == null)
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
            var factory = compositor.CreateEffectFactory(effect, new[] { "Blur.BlurAmount" });
            // Create Effect Brush
            colorShadowBrush = factory.CreateBrush();
            _colorShadowBrush = colorShadowBrush;
        }

        colorShadowBrush.SetSourceParameter("source", imageBrush);

        _maskOptions.BlurRadius = ColorMaskBlurRadius.ToSingle();

        void CreateShadowMask()
        {
            _imageMaskSurface?.Dispose();
            _imageMaskSurface = null;

            // Create the ImageMaskSurface
            var newMaskSurface = generator.CreateImageMaskSurface(maskSurface ?? imageSurface, colorShadowVisual.Size.ToSize(), ColorMaskPadding, _maskOptions);
            _imageMaskSurface = newMaskSurface;

            // Create SurfaceBrush
            var mask = compositor.CreateSurfaceBrush(newMaskSurface);
            // Set the Mask
            colorShadowBrush.SetSourceParameter("mask", mask);
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
                _imageMaskSurface.Resize(colorShadowVisual.Size.ToSize(), ColorMaskPadding, _maskOptions);
            }
        }

        colorShadowVisual.Brush = colorShadowBrush;
        colorShadowVisual.Opacity = ColorShadowOpacity.ToSingle();

        var blurAnimation = compositor.CreateScalarKeyFrameAnimation();
        blurAnimation.Duration = TimeSpan.FromMilliseconds(100);

        Expression<CompositionExpression<float>> expr = c => ColorShadowBlurRadius.ToSingle();
        blurAnimation.InsertExpressionKeyFrame(1f, expr, compositor.CreateLinearEasingFunction());
        colorShadowBrush.Properties.StartAnimation("Blur.BlurAmount", blurAnimation);
        _colorShadowBlurAnimation = blurAnimation;

        // Shadow for the Image Layer
        if (IsShadowEnabled)
        {
            dropShadow.Offset = new Vector3(ShadowOffsetX.ToSingle(), ShadowOffsetY.ToSingle(), 0);
            dropShadow.Opacity = ShadowOpacity.ToSingle();
            dropShadow.BlurRadius = ShadowBlurRadius.ToSingle();
            dropShadow.Color = ShadowColor;
            imageVisual.Shadow = dropShadow;
        }
        else
        {
            imageVisual.Shadow = null;
        }

        if (!rootVisual.Children.Any())
        {
            rootVisual.Children.InsertAtTop(colorShadowVisual);
            rootVisual.Children.InsertAtTop(imageVisual);
        }

        if (_renderGrid != null)
        {
            ElementCompositionPreview.SetElementChildVisual(_renderGrid, rootVisual);
        }
    }

    private async Task<bool> LoadImageAsync(Uri? uri)
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

            var (_, generator, _) = EnsureComposition();

            var imageSurface = await generator.CreateImageSurfaceAsync(uri, new Size(), ImageSurfaceOptions.Default);
            _imageSurface = imageSurface;
            _imageSourceLoaded = imageSurface.Status == ImageSurfaceLoadStatus.Success;
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

    private (Compositor Compositor, ICompositionGenerator Generator, DropShadow DropShadow) EnsureComposition()
    {
        if (_compositor == null || _generator == null || _dropShadow == null)
        {
            var compositor = CompositionTarget.GetCompositorForCurrentThread();
            var generator = compositor.CreateCompositionGenerator();
            var dropShadow = compositor.CreateDropShadow();
            dropShadow.SourcePolicy = CompositionDropShadowSourcePolicy.InheritFromVisualContent;

            _compositor = compositor;
            _generator = generator;
            _dropShadow = dropShadow;
        }

        return (_compositor, _generator, _dropShadow);
    }

    private async Task<IImageSurface?> ApplyMaskAsync(
        Compositor compositor,
        ICompositionGenerator generator,
        SpriteVisual imageVisual,
        CompositionBrush imageBrush)
    {
        if (IsRounded)
        {
            // Force circular mask, ignore user mask completely
            var circleSurface = await LoadMaskInternalAsync(generator, new Uri("ms-appx:///Assets/Mask/CircleMask.png"));

            var maskBrush = compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = compositor.CreateSurfaceBrush(circleSurface);

            imageVisual.Brush = maskBrush;
            return circleSurface;
        }

        // Not rounded → check if user provided Mask
        if (Mask is Uri userMask)
        {
            var userSurface = await LoadMaskInternalAsync(generator, userMask);

            var maskBrush = compositor.CreateMaskBrush();
            maskBrush.Source = imageBrush;
            maskBrush.Mask = compositor.CreateSurfaceBrush(userSurface);

            imageVisual.Brush = maskBrush;
            return userSurface;
        }

        // No mask at all
        imageVisual.Brush = imageBrush;
        return null;
    }

    private async Task<IImageSurface> LoadMaskInternalAsync(ICompositionGenerator generator, Uri uri)
    {
        var assembly = typeof(ColorShadow).Assembly;

        var stream = FileHelper.GetFileFromEmbededResourcesOrUri(uri, assembly);

        CanvasBitmap canvasBitmap;
        var canvasDevice = CanvasDevice.GetSharedDevice();

        if (stream != null)
        {
            canvasBitmap = await CanvasBitmap.LoadAsync(canvasDevice, stream.AsRandomAccessStream());
        }
        else
        {
            canvasBitmap = await CanvasBitmap.LoadAsync(canvasDevice, uri);
        }

        return generator.CreateImageSurface(canvasBitmap, canvasBitmap.Size, ImageSurfaceOptions.Default);
    }

    public void StartBreath(CompositionEasingFunction? breathEasing = null, TimeSpan? breathDuration = null)
    {
        if (_compositor == null || _colorShadowBrush == null || _breathAnimation != null)
            return;

        var easing = breathEasing ?? _compositor.CreateEaseInOutSineEasingFunction();

        var animation = _compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(0f, (float)ColorShadowBlurRadius, easing);
        animation.InsertKeyFrame(0.5f, (float)(ColorShadowBlurRadius * 3.5), easing); // expand
        animation.InsertKeyFrame(1f, (float)ColorShadowBlurRadius, easing);           // shrink
        animation.Duration = breathDuration ?? TimeSpan.FromSeconds(2);
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.Direction = AnimationDirection.Alternate;

        _colorShadowBrush.Properties.StartAnimation("Blur.BlurAmount", animation);

        _breathAnimation = animation;
    }

    public void StopBreath()
    {
        if (_breathAnimation == null || _colorShadowBrush == null)
            return;

        _colorShadowBrush.Properties.StopAnimation("Blur.BlurAmount");
        _breathAnimation = null;
    }

    private void UpdateIsBreathing()
    {
        if (IsBreathing)
        {
            StartBreath();
        }
        else
        {
            StopBreath();
        }
    }
}
