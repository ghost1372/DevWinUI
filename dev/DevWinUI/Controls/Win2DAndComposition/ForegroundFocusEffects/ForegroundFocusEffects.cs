using Microsoft.UI.Xaml.Markup;
using Windows.Graphics.Effects;

namespace DevWinUI;

[ContentProperty(Name = nameof(Content))]
public partial class ForegroundFocusEffects : Control
{
    private SpriteVisual _destinationSprite;
    private Compositor _compositor;
    private CompositionScopedBatch _scopeBatch;
    private ManagedSurface _maskSurface;
    private CompositionBackdropBrush _backdropBrush;

    public FrameworkElement Content
    {
        get { return (FrameworkElement)GetValue(ContentProperty); }
        set { SetValue(ContentProperty, value); }
    }

    public static readonly DependencyProperty ContentProperty =
        DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(ForegroundFocusEffects), new PropertyMetadata(null, OnContentChanged));

    private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ForegroundFocusEffects)d;
        if (ctl != null)
        {
            if (e.OldValue is FrameworkElement oldContent)
            {
                ElementCompositionPreview.SetElementChildVisual(oldContent, null);
            }

            ctl.Init();
        }
    }

    public ForegroundFocusEffectTypes Effect
    {
        get { return (ForegroundFocusEffectTypes)GetValue(EffectProperty); }
        set { SetValue(EffectProperty, value); }
    }

    public static readonly DependencyProperty EffectProperty =
        DependencyProperty.Register(nameof(Effect), typeof(ForegroundFocusEffectTypes), typeof(ForegroundFocusEffects), new PropertyMetadata(ForegroundFocusEffectTypes.Blur, OnEffectChanged));

    private static void OnEffectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ForegroundFocusEffects)d;
        if (ctl != null)
        {
            ctl.UpdateEffect();
        }
    }

    public Uri MaskSource
    {
        get { return (Uri)GetValue(MaskSourceProperty); }
        set { SetValue(MaskSourceProperty, value); }
    }

    public static readonly DependencyProperty MaskSourceProperty =
        DependencyProperty.Register(nameof(MaskSource), typeof(Uri), typeof(ForegroundFocusEffects), new PropertyMetadata(new Uri("ms-appx:///Assets/Mask/ForegroundFocusMask.png"), OnMaskSourceChanged));

    private static void OnMaskSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ForegroundFocusEffects)d;
        if (ctl != null && (ctl._maskSurface != null || ctl.Effect == ForegroundFocusEffectTypes.Mask) && e.NewValue != null)
        {
            ctl.LoadMaskSurface();
        }
    }

    public TimeSpan ApplyEffectDuration
    {
        get { return (TimeSpan)GetValue(ApplyEffectDurationProperty); }
        set { SetValue(ApplyEffectDurationProperty, value); }
    }

    public static readonly DependencyProperty ApplyEffectDurationProperty =
        DependencyProperty.Register(nameof(ApplyEffectDuration), typeof(TimeSpan), typeof(ForegroundFocusEffects), new PropertyMetadata(TimeSpan.FromMilliseconds(0)));

    public TimeSpan RemoveEffectDuration
    {
        get { return (TimeSpan)GetValue(RemoveEffectDurationProperty); }
        set { SetValue(RemoveEffectDurationProperty, value); }
    }

    public static readonly DependencyProperty RemoveEffectDurationProperty =
        DependencyProperty.Register(nameof(RemoveEffectDuration), typeof(TimeSpan), typeof(ForegroundFocusEffects), new PropertyMetadata(TimeSpan.FromMilliseconds(0)));

    public TimeSpan ShowAnimationDuration
    {
        get { return (TimeSpan)GetValue(ShowAnimationDurationProperty); }
        set { SetValue(ShowAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty ShowAnimationDurationProperty =
        DependencyProperty.Register(nameof(ShowAnimationDuration), typeof(TimeSpan), typeof(ForegroundFocusEffects), new PropertyMetadata(TimeSpan.FromMilliseconds(1500)));

    public TimeSpan HideAnimationDuration
    {
        get { return (TimeSpan)GetValue(HideAnimationDurationProperty); }
        set { SetValue(HideAnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty HideAnimationDurationProperty =
        DependencyProperty.Register(nameof(HideAnimationDuration), typeof(TimeSpan), typeof(ForegroundFocusEffects), new PropertyMetadata(TimeSpan.FromMilliseconds(1000)));

    public ForegroundFocusEffects()
    {
        DefaultStyleKey = typeof(ForegroundFocusEffects);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        InitializeVisual();

        SizeChanged -= ForegroundFocusEffects_SizeChanged;
        SizeChanged += ForegroundFocusEffects_SizeChanged;
        Loaded -= ForegroundFocusEffects_Loaded;
        Loaded += ForegroundFocusEffects_Loaded;
        Unloaded -= ForegroundFocusEffects_Unloaded;
        Unloaded += ForegroundFocusEffects_Unloaded;
    }

    private void InitializeVisual()
    {
        if (_destinationSprite != null)
        {
            return;
        }

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        ImageLoader.Initialize(_compositor);
        _destinationSprite = _compositor.CreateSpriteVisual();
        _backdropBrush = _compositor.CreateBackdropBrush();

        Init();
        UpdateEffect();
    }

    private async void LoadMaskSurface()
    {
        var sprite = _destinationSprite;
        var source = MaskSource;

        if (sprite == null || source == null)
        {
            return;
        }

        var surface = await ImageLoader.Instance.LoadFromUriAsync(source);

        if (surface == null)
        {
            return;
        }

        if (sprite != _destinationSprite || source != MaskSource)
        {
            surface.Dispose();
            return;
        }

        var oldSurface = _maskSurface;
        _maskSurface = surface;
        UpdateEffect();
        oldSurface?.Dispose();
    }

    private void ForegroundFocusEffects_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeVisual();
    }

    private void ForegroundFocusEffects_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_destinationSprite != null)
        {
            _destinationSprite.Size = e.NewSize.ToVector2();

            switch (this.Effect)
            {
                case ForegroundFocusEffectTypes.Mask:
                    {
                        if (_destinationSprite.Brush is CompositionEffectBrush brush && brush.GetSourceParameter("SecondSource") is CompositionSurfaceBrush surfaceBrush)
                        {
                            surfaceBrush.CenterPoint = e.NewSize.ToVector2() * .5f;
                        }

                        break;
                    }
                default:
                    break;
            }
        }
    }

    private void ForegroundFocusEffects_Unloaded(object sender, RoutedEventArgs e)
    {
        CleanupScopeBatch();

        if (Content != null)
        {
            ElementCompositionPreview.SetElementChildVisual(Content, null);
        }

        if (_destinationSprite != null)
        {
            _destinationSprite.Dispose();
            _destinationSprite = null;
        }

        if (_backdropBrush != null)
        {
            _backdropBrush.Dispose();
            _backdropBrush = null;
        }

        if (_maskSurface != null)
        {
            _maskSurface.Dispose();
            _maskSurface = null;
        }
    }

    private void Init()
    {
        if (Content == null || _destinationSprite == null)
        {
            return;
        }

        _destinationSprite.Size = new Vector2((float)ActualWidth, (float)ActualHeight);

        _destinationSprite.IsVisible = false;

        ElementCompositionPreview.SetElementChildVisual(Content, _destinationSprite);
    }

    private void UpdateEffect()
    {
        if (_compositor != null && _destinationSprite != null)
        {
            IGraphicsEffect graphicsEffect = null;
            CompositionBrush secondaryBrush = null;
            string[] animatableProperties = null;

            //
            // Create the appropriate effect graph and resources
            //

            switch (this.Effect)
            {
                case ForegroundFocusEffectTypes.Desaturation:
                    {
                        graphicsEffect = new SaturationEffect()
                        {
                            Saturation = 0.0f,
                            Source = new CompositionEffectSourceParameter("ImageSource")
                        };
                    }
                    break;

                case ForegroundFocusEffectTypes.Hue:
                    {
                        graphicsEffect = new HueRotationEffect()
                        {
                            Name = "Hue",
                            Angle = 3.14f,
                            Source = new CompositionEffectSourceParameter("ImageSource")
                        };
                        animatableProperties = new[] { "Hue.Angle" };
                    }
                    break;

                case ForegroundFocusEffectTypes.VividLight:
                    {
                        graphicsEffect = new BlendEffect()
                        {
                            Mode = BlendEffectMode.VividLight,
                            Foreground = new ColorSourceEffect()
                            {
                                Name = "Base",
                                Color = Color.FromArgb(255, 80, 40, 40)
                            },
                            Background = new CompositionEffectSourceParameter("ImageSource"),
                        };
                        animatableProperties = new[] { "Base.Color" };
                    }
                    break;
                case ForegroundFocusEffectTypes.Mask:
                    {
                        if (_maskSurface == null)
                        {
                            _destinationSprite.Brush?.Dispose();
                            _destinationSprite.Brush = null;
                            LoadMaskSurface();
                            return;
                        }

                        graphicsEffect = new CompositeEffect()
                        {
                            Mode = CanvasComposite.DestinationOver,
                            Sources =
                                {
                                    new CompositeEffect()
                                    {
                                        Mode = CanvasComposite.DestinationIn,
                                        Sources =
                                        {

                                            new CompositionEffectSourceParameter("ImageSource"),
                                            new CompositionEffectSourceParameter("SecondSource")
                                        }
                                    },
                                    new ColorSourceEffect()
                                    {
                                        Color = Color.FromArgb(200,255,255,255)
                                    },
                                }
                        };

                        _maskSurface.Brush.Stretch = CompositionStretch.UniformToFill;
                        _maskSurface.Brush.CenterPoint = _destinationSprite.Size * .5f;
                        secondaryBrush = _maskSurface.Brush;
                    }
                    break;
                case ForegroundFocusEffectTypes.Blur:
                    {
                        graphicsEffect = new GaussianBlurEffect()
                        {
                            BlurAmount = 20,
                            Source = new CompositionEffectSourceParameter("ImageSource"),
                            Optimization = EffectOptimization.Balanced,
                            BorderMode = EffectBorderMode.Hard,
                        };
                    }
                    break;
                case ForegroundFocusEffectTypes.LightenBlur:
                    {
                        graphicsEffect = new ArithmeticCompositeEffect()
                        {
                            Source1Amount = .4f,
                            Source2Amount = .6f,
                            MultiplyAmount = 0,
                            Source1 = new ColorSourceEffect()
                            {
                                Name = "Base",
                                Color = Color.FromArgb(255, 255, 255, 255),
                            },
                            Source2 = new GaussianBlurEffect()
                            {
                                BlurAmount = 20,
                                Source = new CompositionEffectSourceParameter("ImageSource"),
                                Optimization = EffectOptimization.Balanced,
                                BorderMode = EffectBorderMode.Hard,
                            }
                        };
                    }
                    break;
                case ForegroundFocusEffectTypes.DarkenBlur:
                    {
                        graphicsEffect = new ArithmeticCompositeEffect()
                        {
                            Source1Amount = .4f,
                            Source2Amount = .6f,
                            MultiplyAmount = 0,
                            Source1 = new ColorSourceEffect()
                            {
                                Name = "Base",
                                Color = Color.FromArgb(255, 0, 0, 0),
                            },
                            Source2 = new GaussianBlurEffect()
                            {
                                BlurAmount = 20,
                                Source = new CompositionEffectSourceParameter("ImageSource"),
                                Optimization = EffectOptimization.Balanced,
                                BorderMode = EffectBorderMode.Hard,
                            }
                        };
                    }
                    break;
                case ForegroundFocusEffectTypes.RainbowBlur:
                    {
                        graphicsEffect = new ArithmeticCompositeEffect()
                        {
                            Source1Amount = .3f,
                            Source2Amount = .7f,
                            MultiplyAmount = 0,
                            Source1 = new ColorSourceEffect()
                            {
                                Name = "Base",
                                Color = Color.FromArgb(255, 0, 0, 0),
                            },
                            Source2 = new GaussianBlurEffect()
                            {
                                BlurAmount = 20,
                                Source = new CompositionEffectSourceParameter("ImageSource"),
                                Optimization = EffectOptimization.Balanced,
                                BorderMode = EffectBorderMode.Hard,
                            }
                        };
                        animatableProperties = new[] { "Base.Color" };
                    }
                    break;
                default:
                    break;
            }

            if (graphicsEffect == null)
            {
                return;
            }

            // Create the effect factory and instantiate a brush
            CompositionEffectFactory _effectFactory = _compositor.CreateEffectFactory(graphicsEffect, animatableProperties);
            CompositionEffectBrush brush = _effectFactory.CreateBrush();
            _effectFactory.Dispose();

            // Set the destination brush as the source of the image content
            brush.SetSourceParameter("ImageSource", _backdropBrush);

            // If his effect uses a secondary brush, set it now
            if (secondaryBrush != null)
            {
                brush.SetSourceParameter("SecondSource", secondaryBrush);
            }

            // Update the destination layer with the fully configured brush
            CompositionBrush oldBrush = _destinationSprite.Brush;
            _destinationSprite.Brush = brush;
            oldBrush?.Dispose();
        }
    }

    private void CleanupScopeBatch()
    {
        if (_scopeBatch != null)
        {
            _scopeBatch.Completed -= ScopeBatch_Completed;
            _scopeBatch.Dispose();
            _scopeBatch = null;
        }
    }
    private void ScopeBatch_Completed(object sender, CompositionBatchCompletedEventArgs args)
    {
        if (_destinationSprite != null)
        {
            // Scope batch completion event has fired, hide the destination sprite and cleanup
            // the batch
            _destinationSprite.IsVisible = false;

            if (Effect == ForegroundFocusEffectTypes.RainbowBlur && _destinationSprite.Brush is CompositionEffectBrush rainbowBrush)
            {
                rainbowBrush.StopAnimation("Base.Color");
            }
        }

        CleanupScopeBatch();
    }
    public void ApplyEffect()
    {
        if (_destinationSprite?.Brush == null)
        {
            return;
        }

        // If we're in the middle of an animation, cancel it now
        if (_scopeBatch != null)
        {
            CleanupScopeBatch();
        }

        // We're starting our transition, show the destination sprite
        _destinationSprite.IsVisible = true;

        // Animate from transparent to fully opaque
        ScalarKeyFrameAnimation showAnimation = _compositor.CreateScalarKeyFrameAnimation();
        showAnimation.InsertKeyFrame(0f, 0f);
        showAnimation.InsertKeyFrame(1f, 1f);
        showAnimation.Duration = ShowAnimationDuration;
        _destinationSprite.StartAnimation("Opacity", showAnimation);

        // Use whichever effect is currently selected
        switch (this.Effect)
        {
            case ForegroundFocusEffectTypes.Mask:
                {
                    CompositionSurfaceBrush brush = ((CompositionEffectBrush)_destinationSprite.Brush).GetSourceParameter("SecondSource") as CompositionSurfaceBrush;
                    Vector2KeyFrameAnimation scaleAnimation = _compositor.CreateVector2KeyFrameAnimation();
                    scaleAnimation.InsertKeyFrame(0f, new Vector2(1.25f, 1.25f));
                    scaleAnimation.InsertKeyFrame(1f, new Vector2(0f, 0f));
                    scaleAnimation.Duration = ApplyEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(2000) : ApplyEffectDuration;
                    brush.StartAnimation("Scale", scaleAnimation);
                    break;
                }
            case ForegroundFocusEffectTypes.VividLight:
                {
                    CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
                    ColorKeyFrameAnimation coloAnimation = _compositor.CreateColorKeyFrameAnimation();
                    coloAnimation.InsertKeyFrame(0f, Color.FromArgb(255, 255, 255, 255));
                    coloAnimation.InsertKeyFrame(1f, Color.FromArgb(255, 30, 30, 30));
                    coloAnimation.Duration = ApplyEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(4000) : ApplyEffectDuration;
                    brush.StartAnimation("Base.Color", coloAnimation);
                    break;
                }
            case ForegroundFocusEffectTypes.Hue:
                {
                    CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
                    ScalarKeyFrameAnimation rotateAnimation = _compositor.CreateScalarKeyFrameAnimation();
                    rotateAnimation.InsertKeyFrame(0f, 0f);
                    rotateAnimation.InsertKeyFrame(1f, (float)Math.PI);
                    rotateAnimation.Duration = ApplyEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(4000) : ApplyEffectDuration;
                    brush.StartAnimation("Hue.Angle", rotateAnimation);
                    break;
                }
            case ForegroundFocusEffectTypes.RainbowBlur:
                {
                    CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
                    ColorKeyFrameAnimation colorAnimation = _compositor.CreateColorKeyFrameAnimation();
                    colorAnimation.InsertKeyFrame(0, Colors.Red);
                    colorAnimation.InsertKeyFrame(.16f, Colors.Orange);
                    colorAnimation.InsertKeyFrame(.32f, Colors.Yellow);
                    colorAnimation.InsertKeyFrame(.48f, Colors.Green);
                    colorAnimation.InsertKeyFrame(.64f, Colors.Blue);
                    colorAnimation.InsertKeyFrame(.80f, Colors.Purple);
                    colorAnimation.InsertKeyFrame(1, Colors.Red);
                    colorAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
                    colorAnimation.Duration = ApplyEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(5000) : ApplyEffectDuration;
                    brush.StartAnimation("Base.Color", colorAnimation);
                    break;
                }
            default:
                break;
        }
    }

    public void RemoveEffect()
    {
        if (_destinationSprite?.Brush == null)
        {
            return;
        }

        CleanupScopeBatch();

        // Start a scoped batch so we can register to completion event and hide the destination layer
        _scopeBatch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);

        // Start the hide animation to fade out the destination effect
        ScalarKeyFrameAnimation hideAnimation = _compositor.CreateScalarKeyFrameAnimation();
        hideAnimation.InsertKeyFrame(0f, 1f);
        hideAnimation.InsertKeyFrame(1.0f, 0f);
        hideAnimation.Duration = HideAnimationDuration;
        _destinationSprite.StartAnimation("Opacity", hideAnimation);

        // Use whichever effect is currently selected
        switch (this.Effect)
        {
            case ForegroundFocusEffectTypes.Mask:
                {
                    CompositionSurfaceBrush brush = ((CompositionEffectBrush)_destinationSprite.Brush).GetSourceParameter("SecondSource") as CompositionSurfaceBrush;
                    Vector2KeyFrameAnimation scaleAnimation = _compositor.CreateVector2KeyFrameAnimation();
                    scaleAnimation.InsertKeyFrame(1f, new Vector2(2.0f, 2.0f));
                    scaleAnimation.Duration = RemoveEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(1000) : RemoveEffectDuration;
                    brush.StartAnimation("Scale", scaleAnimation);
                    break;
                }
            case ForegroundFocusEffectTypes.VividLight:
                {
                    CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
                    ColorKeyFrameAnimation coloAnimation = _compositor.CreateColorKeyFrameAnimation();
                    coloAnimation.InsertKeyFrame(1f, Color.FromArgb(255, 100, 100, 100));
                    coloAnimation.Duration = RemoveEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(1500) : RemoveEffectDuration;
                    brush.StartAnimation("Base.Color", coloAnimation);
                    break;
                }
            case ForegroundFocusEffectTypes.Hue:
                {
                    CompositionEffectBrush brush = (CompositionEffectBrush)_destinationSprite.Brush;
                    ScalarKeyFrameAnimation rotateAnimation = _compositor.CreateScalarKeyFrameAnimation();
                    rotateAnimation.InsertKeyFrame(1f, 0f);
                    rotateAnimation.Duration = RemoveEffectDuration.TotalMilliseconds == 0 ? TimeSpan.FromMilliseconds(1500) : RemoveEffectDuration;
                    brush.StartAnimation("Hue.Angle", rotateAnimation);
                    break;
                }
            default:
                break;
        }

        //Scoped batch completed event
        _scopeBatch.Completed += ScopeBatch_Completed;
        _scopeBatch.End();
    }
}
