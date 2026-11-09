namespace DevWinUI;

[TemplatePart(Name = PART_CanvasAnimation, Type = typeof(CanvasAnimatedControl))]
public partial class ConfettiCannon : Control
{
    private const string PART_CanvasAnimation = "PART_CanvasAnimated";
    private CanvasAnimatedControl _canvas;
    private readonly ConfettiEngine _engine = new();
    public event EventHandler AnimationStarted;
    public event EventHandler AnimationCompleted;
    private bool _isAnimating;

    public bool IsFixedTimeStep
    {
        get { return (bool)GetValue(IsFixedTimeStepProperty); }
        set { SetValue(IsFixedTimeStepProperty, value); }
    }

    public static readonly DependencyProperty IsFixedTimeStepProperty =
        DependencyProperty.Register(nameof(IsFixedTimeStep), typeof(bool), typeof(ConfettiCannon), new PropertyMetadata(false, OnIsFixedTimeStepChanged));

    private static void OnIsFixedTimeStepChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ConfettiCannon)d;
        if (ctl != null && ctl._canvas != null)
        {
            ctl._canvas.IsFixedTimeStep = (bool)e.NewValue;
        }
    }

    public Color ClearColor
    {
        get { return (Color)GetValue(ClearColorProperty); }
        set { SetValue(ClearColorProperty, value); }
    }

    public static readonly DependencyProperty ClearColorProperty =
        DependencyProperty.Register(nameof(ClearColor), typeof(Color), typeof(ConfettiCannon), new PropertyMetadata(Colors.Transparent, OnClearColorChanged));

    private static void OnClearColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (ConfettiCannon)d;
        if (ctl != null && ctl._canvas != null)
        {
            ctl._canvas.ClearColor = (Color)e.NewValue;
        }
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        if (_canvas != null)
        {
            _canvas.Draw -= OnDraw;
            _canvas.Update -= OnUpdate;
        }

        _canvas = GetTemplateChild(PART_CanvasAnimation) as CanvasAnimatedControl;

        if (_canvas == null)
            return;

        _canvas.ClearColor = ClearColor;
        _canvas.IsFixedTimeStep = IsFixedTimeStep;

        _canvas.Draw += OnDraw;
        _canvas.Update += OnUpdate;

        Loaded -= OnLoaded;
        Loaded += OnLoaded;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        if (_canvas != null)
            _canvas.Paused = _engine.ParticleCount == 0;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        if (_canvas == null)
            return;

        _canvas.Draw -= OnDraw;
        _canvas.Update -= OnUpdate;
        _canvas.RemoveFromVisualTree();
    }

    private void OnUpdate(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
    {
        _engine.Update((float)args.Timing.ElapsedTime.TotalSeconds, sender.Size);
        int count = _engine.ParticleCount;

        // Animation start
        if (!_isAnimating && count > 0)
        {
            _isAnimating = true;
            DispatcherQueue.TryEnqueue(() => AnimationStarted?.Invoke(this, EventArgs.Empty));
        }
        // Animation complete
        else if (_isAnimating && count == 0)
        {
            _isAnimating = false;
            DispatcherQueue.TryEnqueue(() =>
            {
                // Stop rendering while idle
                if (_canvas != null && _engine.ParticleCount == 0)
                    _canvas.Paused = true;

                AnimationCompleted?.Invoke(this, EventArgs.Empty);
            });
        }
    }

    private void OnDraw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
    {
        _engine.Draw(args.DrawingSession);
    }

    public void Fire()
    {
        Fire(new ConfettiCannonOptions());
    }

    public void Fire(ConfettiCannonOptions options)
    {
        _engine.Enqueue(options);

        if (_canvas != null)
            _canvas.Paused = false;
    }

    public void FireBasic()
    {
        Fire(new ConfettiCannonOptions
        {
            ParticleCount = 100,
            Spread = 70,
            Origin = new Windows.Foundation.Point(0.5, 0.6)
        });
    }
    public void FireRandomDirection()
    {
        var random = new Random();
        Fire(new ConfettiCannonOptions
        {
            Angle = random.Next(55, 125),
            Spread = random.Next(50, 70),
            ParticleCount = random.Next(50, 100),
            Origin = new Windows.Foundation.Point(0.5, 0.6)
        });
    }
    public void FireRealistic()
    {
        const int count = 200;

        Fire(0.25, new ConfettiCannonOptions { Spread = 26, StartVelocity = 55 });
        Fire(0.2, new ConfettiCannonOptions { Spread = 60 });
        Fire(0.35, new ConfettiCannonOptions { Spread = 100, Decay = 0.91, Scalar = 0.8 });
        Fire(0.1, new ConfettiCannonOptions { Spread = 120, StartVelocity = 25, Decay = 0.92, Scalar = 1.2 });
        Fire(0.1, new ConfettiCannonOptions { Spread = 120, StartVelocity = 45 });

        void Fire(double particleRatio, ConfettiCannonOptions options)
        {
            options.ParticleCount = (int)Math.Floor(count * particleRatio);
            this.Fire(options);
        }
    }
    public void FireFireworks()
    {
        const int duration = 15 * 1000;
        var random = new Random();
        var animationEnd = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };
        timer.Tick += (_, __) =>
        {
            double timeLeft = (animationEnd - DateTime.UtcNow).TotalMilliseconds;

            if (timeLeft <= 0)
            {
                timer.Stop();
                return;
            }

            var particleCount = (int)(50 * (timeLeft / duration));
            Fire(new ConfettiCannonOptions
            {
                StartVelocity = 30,
                Spread = 360,
                Ticks = 60,
                ParticleCount = particleCount,
                Origin = new Windows.Foundation.Point(RandomInRange(0.1, 0.3), random.NextDouble() - 0.2)
            });
            Fire(new ConfettiCannonOptions
            {
                StartVelocity = 30,
                Spread = 360,
                Ticks = 60,
                ParticleCount = particleCount,
                Origin = new Windows.Foundation.Point(RandomInRange(0.7, 0.9), random.NextDouble() - 0.2)
            });
        };
        timer.Start();
    }

    public void FireStars()
    {
        var colors = new List<Color>
        {
            ColorHelper.GetColorFromHex("#FFE400"),
            ColorHelper.GetColorFromHex("#FFBD00"),
            ColorHelper.GetColorFromHex("#E89400"),
            ColorHelper.GetColorFromHex("#FFCA6C"),
            ColorHelper.GetColorFromHex("#FDFFB8")
        };

        void Shoot()
        {
            Fire(new ConfettiCannonOptions
            {
                Spread = 360,
                Ticks = 50,
                Gravity = 0,
                Decay = 0.94,
                StartVelocity = 30,
                Colors = colors,
                ParticleCount = 40,
                Scalar = 1.2,
                Shapes = new() { "star" }
            });
            Fire(new ConfettiCannonOptions
            {
                Spread = 360,
                Ticks = 50,
                Gravity = 0,
                Decay = 0.94,
                StartVelocity = 30,
                Colors = colors,
                ParticleCount = 10,
                Scalar = 0.75,
                Shapes = new() { "circle" }
            });
        }

        SetTimeout(Shoot, 0);
        SetTimeout(Shoot, 100);
        SetTimeout(Shoot, 200);
    }
    public void FireSnow()
    {
        const int duration = 15 * 1000;

        var random = new Random();
        DateTime animationEnd = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        double skew = 1;
        var colors = new List<Color> { ColorHelper.GetColorFromHex("#ffffff") };

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        timer.Tick += (_, __) =>
        {
            double timeLeft = (animationEnd - DateTime.UtcNow).TotalMilliseconds;

            if (timeLeft <= 0)
            {
                timer.Stop();
                return;
            }

            int ticks = (int)Math.Max(200, 500 * (timeLeft / duration));
            skew = Math.Max(0.8, skew - 0.001);

            Fire(new ConfettiCannonOptions
            {
                ParticleCount = 1,
                StartVelocity = 0,
                Ticks = ticks,
                Origin = new Windows.Foundation.Point(random.NextDouble(), random.NextDouble() * skew - 0.2),
                Colors = colors,
                Shapes = new() { "circle" },
                Gravity = RandomInRange(0.4, 0.6),
                Scalar = RandomInRange(0.4, 1),
                Drift = RandomInRange(-0.4, 0.4),
            });
        };
        timer.Start();
    }

    public void FireSchoolPride()
    {
        const int duration = 15 * 1000;

        DateTime animationEnd = DateTime.UtcNow + TimeSpan.FromMilliseconds(duration);
        var colors = new List<Color> { ColorHelper.GetColorFromHex("#bb0000"), ColorHelper.GetColorFromHex("#ffffff") };

        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(16) };
        timer.Tick += (_, __) =>
        {
            double timeLeft = (animationEnd - DateTime.UtcNow).TotalMilliseconds;
            if (timeLeft <= 0)
            {
                timer.Stop();
                return;
            }

            Fire(new ConfettiCannonOptions
            {
                ParticleCount = 2,
                Angle = 60,
                Spread = 55,
                Origin = new Windows.Foundation.Point(0, 0.5),
                Colors = colors,
            });
            Fire(new ConfettiCannonOptions
            {
                ParticleCount = 2,
                Angle = 120,
                Spread = 55,
                Origin = new Windows.Foundation.Point(1, 0.5),
                Colors = colors,
            });
        };
        timer.Start();
    }

    private double RandomInRange(double min, double max)
    {
        return Random.Shared.NextDouble() * (max - min) + min;
    }

    private static void SetTimeout(Action action, int delayMilliseconds)
    {
        var timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(delayMilliseconds) };
        timer.Tick += (_, __) =>
        {
            action();
            timer.Stop();
        };
        timer.Start();
    }
}
