using Microsoft.UI.Xaml.Media.Animation;
using Windows.Graphics.Effects;

namespace DevWinUI;

public partial class BlurEffectManager : IDisposable
{
    private readonly FrameworkElement _targetElement;
    private readonly Compositor _compositor;

    private SpriteVisual _blurVisual;
    private CompositionEffectBrush _blurBrush;
    private ManagedSurface _noiseSurface;

    private double _blurAmount = 10.0;
    private Color _tintColor = Colors.Transparent;
    private bool _useNoise;
    private bool _isBrushWithTint = false;

    private EffectBorderMode _borderMode = EffectBorderMode.Hard;
    private EffectOptimization _optimization = EffectOptimization.Balanced;
    private BlendEffectMode _blendMode = BlendEffectMode.SoftLight;
    private BlendEffectMode _noiseBlendMode = BlendEffectMode.Screen;
    private string _noiseUri = "ms-appx:///Assets/Noise/Noise.jpg";

    public ICompositionSurface SurfaceSource { get; set; }
    public CompositionSurfaceBrush SurfaceBrushSource { get; set; }
    public Visual VisualSource { get; set; }
    public CompositionBrush CustomSourceBrush { get; set; }

    public string NoiseUri
    {
        get => _noiseUri;
        set
        {
            if (_noiseUri != value)
            {
                _noiseUri = value;
                if (_useNoise)
                    Refresh();
            }
        }
    }
    private BlurSourceType _sourceType = BlurSourceType.Backdrop;

    public BlurSourceType SourceType
    {
        get => _sourceType;
        set
        {
            if (_sourceType != value)
            {
                _sourceType = value;
                Refresh();
            }
        }
    }

    public double BlurAmount
    {
        get => _blurAmount;
        set
        {
            if (_blurAmount != value)
            {
                _blurAmount = Math.Max(0, value);
                if (_blurBrush != null)
                    _blurBrush.Properties.InsertScalar("Blur.BlurAmount", (float)_blurAmount);
            }
        }
    }

    private bool IsTintEnabled => _tintColor.A > 0;

    public Color TintColor
    {
        get => _tintColor;
        set
        {
            if (_tintColor != value)
            {
                bool wasTintEnabled = _isBrushWithTint;
                _tintColor = value;
                bool isTintEnabledNow = IsTintEnabled;

                if (_blurBrush != null)
                {
                    if (wasTintEnabled != isTintEnabledNow)
                    {
                        Refresh();
                    }
                    else if (isTintEnabledNow)
                    {
                        _blurBrush.Properties.InsertColor("Tint.Color", _tintColor);
                    }
                }
            }
        }
    }

    public bool UseNoise
    {
        get => _useNoise;
        set
        {
            if (_useNoise != value)
            {
                _useNoise = value;
                Refresh();
            }
        }
    }

    public EffectBorderMode BorderMode
    {
        get => _borderMode;
        set
        {
            if (_borderMode != value)
            {
                _borderMode = value;
                Refresh();
            }
        }
    }

    public EffectOptimization Optimization
    {
        get => _optimization;
        set
        {
            if (_optimization != value)
            {
                _optimization = value;
                Refresh();
            }
        }
    }

    public BlendEffectMode BlendMode
    {
        get => _blendMode;
        set
        {
            if (_blendMode != value)
            {
                _blendMode = value;
                Refresh();
            }
        }
    }

    public BlendEffectMode NoiseBlendMode
    {
        get => _noiseBlendMode;
        set
        {
            if (_noiseBlendMode != value)
            {
                _noiseBlendMode = value;
                Refresh();
            }
        }
    }

    public BlurEffectManager(FrameworkElement targetElement)
    {
        _targetElement = targetElement ?? throw new ArgumentNullException(nameof(targetElement));
        var visual = ElementCompositionPreview.GetElementVisual(_targetElement);
        _compositor = visual.Compositor ?? throw new InvalidOperationException("Compositor is not available.");
        _targetElement.SizeChanged += OnSizeChanged;

        EnableBlur();
    }

    public void EnableBlur()
    {
        if (_blurVisual != null)
            return;

        _blurVisual = _compositor.CreateSpriteVisual();
        _blurVisual.Size = new System.Numerics.Vector2(
            (float)Math.Max(1, _targetElement.ActualWidth),
            (float)Math.Max(1, _targetElement.ActualHeight));

        _blurBrush = BuildBrush();
        _blurVisual.Brush = _blurBrush;

        ElementCompositionPreview.SetElementChildVisual(_targetElement, _blurVisual);
    }

    public void DisableBlur()
    {
        ElementCompositionPreview.SetElementChildVisual(_targetElement, null);
        _blurBrush?.Dispose();
        _blurBrush = null;
        _blurVisual?.Dispose();
        _blurVisual = null;
    }

    public void Refresh()
    {
        DisableBlur();
        EnableBlur();
    }

    private CompositionEffectBrush BuildBrush()
    {
        var blurEffect = new GaussianBlurEffect
        {
            Name = "Blur",
            BlurAmount = (float)BlurAmount,
            BorderMode = BorderMode,
            Optimization = Optimization,
            Source = new CompositionEffectSourceParameter("ImageSource")
        };

        IGraphicsEffect currentEffect = blurEffect;

        if (IsTintEnabled)
        {
            currentEffect = new BlendEffect
            {
                Background = blurEffect,
                Foreground = new ColorSourceEffect
                {
                    Name = "Tint",
                    Color = TintColor
                },
                Mode = BlendMode
            };
        }

        if (UseNoise)
        {
            currentEffect = new BlendEffect
            {
                Background = currentEffect,
                Foreground = new CompositionEffectSourceParameter("NoiseImage"),
                Mode = NoiseBlendMode
            };
        }

        var effectFactory = _compositor.CreateEffectFactory(currentEffect, GetAnimatableProperties());

        var brush = effectFactory.CreateBrush();

        // Resolve Source
        switch (SourceType)
        {
            case BlurSourceType.Backdrop:
                brush.SetSourceParameter("ImageSource", _compositor.CreateBackdropBrush());
                break;

            case BlurSourceType.Surface:
                if (SurfaceSource != null)
                    brush.SetSourceParameter("ImageSource", _compositor.CreateSurfaceBrush(SurfaceSource));
                else if (SurfaceBrushSource != null)
                    brush.SetSourceParameter("ImageSource", SurfaceBrushSource);
                break;

            case BlurSourceType.Visual:
                if (VisualSource != null)
                {
                    var visualSurface = _compositor.CreateVisualSurface();
                    visualSurface.SourceVisual = VisualSource;
                    brush.SetSourceParameter("ImageSource", _compositor.CreateSurfaceBrush(visualSurface));
                }
                break;

            case BlurSourceType.Custom:
                if (CustomSourceBrush != null)
                    brush.SetSourceParameter("ImageSource", CustomSourceBrush);
                break;
        }

        brush.Properties.InsertScalar("Blur.BlurAmount", (float)BlurAmount);

        if (IsTintEnabled)
            brush.Properties.InsertColor("Tint.Color", TintColor);

        if (UseNoise)
        {
            ImageLoader.Initialize(_compositor);
            _noiseSurface?.Brush?.Dispose();
            _noiseSurface = ImageLoader.Instance.LoadFromUri(new Uri(NoiseUri));
            _noiseSurface.Brush.Stretch = CompositionStretch.UniformToFill;
            brush.SetSourceParameter("NoiseImage", _noiseSurface.Brush);
        }

        _isBrushWithTint = IsTintEnabled;

        return brush;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_blurVisual != null)
        {
            _blurVisual.Size = new System.Numerics.Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
        }
    }

    public List<string> GetAnimatableProperties()
    {
        var list = new List<string> { "Blur.BlurAmount" };
        if (IsTintEnabled)
            list.Add("Tint.Color");
        return list;
    }

    public CompositionBrush GetBrush() => _blurBrush;
    public Compositor GetCompositor() => _compositor;

    public void StartBlurAnimation() => StartBlurAnimation(BlurAmount, TimeSpan.FromMilliseconds(300));
    public void StartBlurAnimation(TimeSpan duration) => StartBlurAnimation(BlurAmount, duration);

    public void StartBlurAnimation(double targetBlurAmount, TimeSpan duration)
    {
        if (_blurBrush == null) return;

        var blurAnimation = _compositor.CreateScalarKeyFrameAnimation();
        blurAnimation.Duration = duration;
        blurAnimation.InsertKeyFrame(0f, 0f);
        blurAnimation.InsertKeyFrame(1f, (float)targetBlurAmount);

        _blurBrush.Properties.StartAnimation("Blur.BlurAmount", blurAnimation);

        if (IsTintEnabled)
        {
            var colorAnimation = _compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = duration;
            colorAnimation.InsertKeyFrame(0f, Colors.Transparent);
            colorAnimation.InsertKeyFrame(1f, TintColor);
            _blurBrush.Properties.StartAnimation("Tint.Color", colorAnimation);
        }
    }

    public void StartBlurReverseAnimation() => StartBlurReverseAnimation(BlurAmount, TimeSpan.FromMilliseconds(300));
    public void StartBlurReverseAnimation(TimeSpan duration) => StartBlurReverseAnimation(BlurAmount, duration);

    public void StartBlurReverseAnimation(double currentBlurAmount, TimeSpan duration)
    {
        if (_blurBrush == null) return;

        var blurAnimation = _compositor.CreateScalarKeyFrameAnimation();
        blurAnimation.Duration = duration;
        blurAnimation.InsertKeyFrame(0f, (float)currentBlurAmount);
        blurAnimation.InsertKeyFrame(1f, 0f);
        _blurBrush.Properties.StartAnimation(propertyName: "Blur.BlurAmount", blurAnimation);

        if (IsTintEnabled)
        {
            var colorAnimation = _compositor.CreateColorKeyFrameAnimation();
            colorAnimation.Duration = duration;
            colorAnimation.InsertKeyFrame(0f, TintColor);
            colorAnimation.InsertKeyFrame(1f, Colors.Transparent);
            _blurBrush.Properties.StartAnimation("Tint.Color", colorAnimation);
        }
    }
    public void StopBlurAnimation()
    {
        if (_blurBrush == null) return;

        _blurBrush.Properties.StopAnimation(propertyName: "Blur.BlurAmount");
        if (IsTintEnabled)
        {
            _blurBrush.Properties.StopAnimation("Tint.Color");
        }
    }
    public void Dispose()
    {
        _targetElement.SizeChanged -= OnSizeChanged;
        DisableBlur();
    }
}
