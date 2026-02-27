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
    public DropShadow DropShadow => _dropShadow;

    public SpriteVisual Visual => _shadowVisual;
    public Compositor compositor;

    private bool _isCustomMask;
    public CompositionBrush Mask
    {
        get => _dropShadow?.Mask;
        set
        {
            if (_dropShadow != null)
            {
                _dropShadow.Mask = value;
                _isCustomMask = value != null; // mark that user provided a mask
            }
        }
    }
    public CompositionShadow()
    {
        this.DefaultStyleKey = typeof(CompositionShadow);
        this.SizeChanged += CompositionShadow_SizeChanged;
        this.Loaded += CompositionShadow_Loaded;
        compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

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
        if (IsRounded)
        {
            UpdateIsRounded();
        }
        else
        {
            UpdateShadowMask();
        }

        UpdateShadowSize();

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

        // If IsRounded is true, always use rounded mask
        if (IsRounded)
            return; // Mask is already set in UpdateIsRounded

        // Only auto-generate mask if user hasn't set a custom mask
        if (!_isCustomMask && Content != null)
        {
            CompositionBrush mask = null;
            if (Content is Image image)
                mask = image.GetAlphaMask();
            else if (Content is Shape shape)
                mask = shape.GetAlphaMask();
            else if (Content is TextBlock textBlock)
                mask = textBlock.GetAlphaMask();

            _dropShadow.Mask = mask;
        }
        else if (!_isCustomMask)
        {
            // fallback if Content is null or unsupported
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

    private async Task UpdateColorFromImageAsync()
    {
        if (!UseEdgeColorFromImage || ImageSourceForEdgeColor == null)
        {
            Color = Colors.Black;
            return;
        }

        Uri uri = GeneralHelper.GetUriFromObjectSource(ImageSourceForEdgeColor);
        if (uri != null)
        {
            var device = CanvasDevice.GetSharedDevice();
            var color = await ColorHelperEx.GetImageEdgeColorWithWin2DAsync(device, uri);
            Color = color;
        }
        else
        {
            Color = Colors.Black;
        }
    }

    private void UpdateIsRounded()
    {
        if (IsRounded)
        {
            float width = (float)this.ActualWidth;
            float height = (float)this.ActualHeight;
            float radius = MathF.Min(width, height) / 2f;

            var ellipse = compositor.CreateEllipseGeometry();
            ellipse.Center = new Vector2(width / 2f, height / 2f);
            ellipse.Radius = new Vector2(radius, radius);

            var shape = compositor.CreateSpriteShape(ellipse);
            shape.FillBrush = compositor.CreateColorBrush(Colors.White);

            var shapeVisual = compositor.CreateShapeVisual();
            shapeVisual.Size = new Vector2(width, height);
            shapeVisual.Shapes.Add(shape);

            var visualSurface = compositor.CreateVisualSurface();
            visualSurface.SourceVisual = shapeVisual;
            visualSurface.SourceSize = new Vector2(width, height);

            var maskBrush = compositor.CreateSurfaceBrush(visualSurface);

            _dropShadow.Mask = maskBrush;
        }
        else
        {
            UpdateShadowMask();
        }
    }
}
