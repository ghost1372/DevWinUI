using Windows.Graphics.Effects;

namespace DevWinUI;

public partial class BlurEffectManager : IDisposable
{
    private readonly FrameworkElement _targetElement;
    private readonly Compositor _compositor;
    private SpriteVisual _blurVisual;
    private CompositionEffectBrush _blurBrush;
    private ManagedSurface _noiseSurface;


    private bool _isTintEnabled = false;
    public bool IsTintEnabled
    {
        get => _isTintEnabled;
        set
        {
            if (_isTintEnabled != value)
            {
                _isTintEnabled = value;
                Refresh();
            }
        }
    }


    private BlurTintTarget _tintTarget = BlurTintTarget.Foreground;
    public BlurTintTarget TintTargetMode
    {
        get => _tintTarget;
        set
        {
            if (_tintTarget != value)
            {
                _tintTarget = value;
                Refresh();
            }
        }
    }


    private Color _tintColor = Colors.Transparent;
    public Color TintColor
    {
        get => _tintColor;
        set
        {
            var corrected = value;
            corrected.A = 255; // force full opacity

            if (_tintColor != corrected)
            {
                _tintColor = corrected;
                if (_blurBrush != null && IsTintEnabled)
                {
                    _blurBrush.Properties.InsertColor("Tint.Color", _tintColor);
                }
            }
        }
    }


    private ICompositionSurface _surfaceSource;
    public ICompositionSurface SurfaceSource
    {
        get => _surfaceSource;
        set
        {
            if (_surfaceSource != value)
            {
                _surfaceSource = value;
                Refresh();
            }
        }
    }

    private CompositionSurfaceBrush _surfaceBrushSource;
    public CompositionSurfaceBrush SurfaceBrushSource
    {
        get => _surfaceBrushSource;
        set
        {
            if (_surfaceBrushSource != value)
            {
                _surfaceBrushSource = value;
                Refresh();
            }
        }
    }

    private Visual _visualSource;
    public Visual VisualSource
    {
        get => _visualSource;
        set
        {
            if (_visualSource != value)
            {
                _visualSource = value;
                Refresh();
            }
        }
    }

    private CompositionBrush _customSourceBrush;
    public CompositionBrush CustomSourceBrush
    {
        get => _customSourceBrush;
        set
        {
            if (_customSourceBrush != value)
            {
                _customSourceBrush = value;
                Refresh();
            }
        }
    }


    private string _noiseUri = "ms-appx:///Assets/Noise/Noise.jpg";
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


    private double _blurAmount = 10.0;
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


    private bool _useNoise;
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


    private EffectBorderMode _borderMode = EffectBorderMode.Hard;
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


    private EffectOptimization _optimization = EffectOptimization.Balanced;
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


    private BlendEffectMode _blendMode = BlendEffectMode.SoftLight;
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


    private BlendEffectMode _noiseBlendMode = BlendEffectMode.Screen;
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


    private CompositionStretch _noiseStretch = CompositionStretch.UniformToFill;
    public CompositionStretch NoiseStretch
    {
        get => _noiseStretch;
        set
        {
            if (_noiseStretch != value)
            {
                _noiseStretch = value;
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
        ImageLoader.Initialize(_compositor);

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
            if (TintTargetMode == BlurTintTarget.Foreground)
            {
                // Tint color on top of blur (foreground)
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
            else // Background
            {
                // Tint color behind the blur
                currentEffect = new BlendEffect
                {
                    Background = new ColorSourceEffect
                    {
                        Name = "Tint",
                        Color = TintColor
                    },
                    Foreground = blurEffect,
                    Mode = BlendMode
                };
            }
        }

        if (UseNoise)
        {
            currentEffect = new BlendEffect
            {
                Background = new CompositionEffectSourceParameter("NoiseImage"),
                Foreground = currentEffect,
                Mode = NoiseBlendMode
            };
        }

        var effectFactory = _compositor.CreateEffectFactory(currentEffect, GetAnimatableProperties());

        var brush = effectFactory.CreateBrush();

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
            _noiseSurface?.Brush?.Dispose();
            _noiseSurface = ImageLoader.Instance.LoadFromUri(new Uri(NoiseUri));
            _noiseSurface.Brush.Stretch = _noiseStretch;
            brush.SetSourceParameter("NoiseImage", _noiseSurface.Brush);
        }

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
