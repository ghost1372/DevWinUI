namespace DevWinUI;
public partial class BlurEffectManager
{
    private readonly FrameworkElement _targetElement;
    private SpriteVisual _blurVisual;
    private CompositionEffectBrush _blurBrush;
    private Compositor _compositor;

    public BlurEffectManager(FrameworkElement targetElement)
    {
        _targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
        _targetElement.SizeChanged -= OnSizeChanged;
        _targetElement.SizeChanged += OnSizeChanged;

        // Delay blur setup until fully loaded
        if (_targetElement.IsLoaded)
            Initialize();
        else
            _targetElement.Loaded += (s, e) => Initialize();
    }

    private void Initialize()
    {
        _compositor = ElementCompositionPreview.GetElementVisual(_targetElement).Compositor;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_blurVisual != null)
        {
            _blurVisual.Size = new System.Numerics.Vector2(
                (float)e.NewSize.Width, (float)e.NewSize.Height);
        }
    }
    public void EnableBlur()
    {
        EnableBlur(100f, EffectBorderMode.Hard);
    }
    public void EnableBlur(float blurAmount)
    {
        EnableBlur(blurAmount, EffectBorderMode.Hard);
    }
    public void EnableBlur(float blurAmount, EffectBorderMode effectBorderMode)
    {
        if (_compositor == null)
            Initialize();

        // Always recreate brush and visual
        var blurEffect = new GaussianBlurEffect
        {
            Name = "Blur",
            Source = new CompositionEffectSourceParameter("Backdrop"),
            BlurAmount = blurAmount,
            BorderMode = effectBorderMode
        };

        var effectFactory = _compositor.CreateEffectFactory(blurEffect, new[] { "Blur.BlurAmount" });
        _blurBrush = effectFactory.CreateBrush();

        var backdropBrush = _compositor.CreateBackdropBrush();
        _blurBrush.SetSourceParameter("Backdrop", backdropBrush);

        _blurVisual = _compositor.CreateSpriteVisual();
        _blurVisual.Brush = _blurBrush;

        // Handle possible zero sizes
        var width = Math.Max(1, _targetElement.ActualWidth);
        var height = Math.Max(1, _targetElement.ActualHeight);
        _blurVisual.Size = new System.Numerics.Vector2((float)width, (float)height);

        ElementCompositionPreview.SetElementChildVisual(_targetElement, _blurVisual);
        _blurBrush.Properties.InsertScalar("Blur.BlurAmount", blurAmount);
    }


    public void DisableBlur()
    {
        if (_blurBrush != null)
        {
            _blurBrush.Properties.InsertScalar("Blur.BlurAmount", 0f);
        }

        ElementCompositionPreview.SetElementChildVisual(_targetElement, null);

        _blurBrush?.Dispose();
        _blurBrush = null;

        _blurVisual?.Dispose();
        _blurVisual = null;
    }

    public void UpdateBlurAmount(float amount)
    {
        if (_blurBrush != null && _blurVisual != null)
        {
            _blurBrush.Properties.InsertScalar("Blur.BlurAmount", amount);
        }
        else
        {
            EnableBlur(amount);
        }
    }
}
