namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Container), Type = typeof(Grid))]
public partial class WanderingParticles : Control
{
    private const string PART_Container = "PART_Container";
    private Grid grid;

    private Compositor _compositor;
    private CompositionGraphicsDevice _graphicsDevice;
    private VisualCollection _particles;
    private Random _random = new();
    private bool _isDisposed = false;

    public int ParticleCount
    {
        get { return (int)GetValue(ParticleCountProperty); }
        set { SetValue(ParticleCountProperty, value); }
    }

    public static readonly DependencyProperty ParticleCountProperty =
        DependencyProperty.Register(nameof(ParticleCount), typeof(int), typeof(WanderingParticles), new PropertyMetadata(2000, OnParticleCountChanged));

    private static void OnParticleCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WanderingParticles)d;
        if (ctl != null)
        {
            ctl.Stop();
            ctl.Start();
        }
    }

    public bool AutoStart
    {
        get { return (bool)GetValue(AutoStartProperty); }
        set { SetValue(AutoStartProperty, value); }
    }

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(WanderingParticles), new PropertyMetadata(true, OnAutoStartChanged));

    private static void OnAutoStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WanderingParticles)d;
        if (ctl != null)
        {
            if ((bool)e.NewValue)
            {
                ctl.Start();
            }
            else
            {
                ctl.Stop();
            }
        }
    }

    public WanderingParticles()
    {
        DefaultStyleKey = typeof(WanderingParticles);
    }
    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        grid = GetTemplateChild(PART_Container) as Grid;

        _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        Unloaded -= OnUnloaded;
        Unloaded += OnUnloaded;

        if (AutoStart)
        {
            Start();
        }
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Stop();
    }

    public void Stop()
    {
        _isDisposed = true;
        CleanupParticles();
    }

    public void Start()
    {
        if (grid == null)
            return;

        _isDisposed = false;

        CleanupParticles();

        var canvasDevice = CanvasDevice.GetSharedDevice();
        _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, canvasDevice);

        var container = _compositor.CreateContainerVisual();
        _particles = container.Children;

        ElementCompositionPreview.SetElementChildVisual(grid, container);

        StartParticles();
    }


    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_particles == null)
            return;

        float newWidth = (float)e.NewSize.Width;
        float newHeight = (float)e.NewSize.Height;

        foreach (var visual in _particles)
        {
            float newX = (float)_random.NextDouble() * newWidth;
            float newY = (float)_random.NextDouble() * newHeight;
            ((SpriteVisual)visual).Offset = new Vector3(newX, newY, 0);
        }
    }

    private void StartParticles()
    {
        float width = (float)ActualWidth;
        float height = (float)ActualHeight;

        for (int i = 0; i < ParticleCount; i++)
        {
            var brush = CreateCircleBrushAsync(_graphicsDevice, 1f, Colors.White);

            var particle = _compositor.CreateSpriteVisual();
            particle.Size = new Vector2(2, 2);
            particle.Brush = brush;

            float startX = (float)_random.NextDouble() * width;
            float startY = (float)_random.NextDouble() * height;
            particle.Offset = new Vector3(startX, startY, 0);

            _particles.InsertAtTop(particle);

            AnimateWandering(particle);
        }
    }
    private void AnimateWandering(SpriteVisual particle)
    {
        void StartNextDrift()
        {
            if (_isDisposed)
                return;

            try
            {
                Vector3 current = particle.Offset;

                float dx = (float)(_random.NextDouble() * 40 - 20);
                float dy = (float)(_random.NextDouble() * 40 - 20);
                Vector3 target = current + new Vector3(dx, dy, 0);

                var animation = _compositor.CreateVector3KeyFrameAnimation();
                animation.Duration = TimeSpan.FromSeconds(3 + _random.NextDouble() * 2);
                animation.InsertKeyFrame(1.0f, target, _compositor.CreateLinearEasingFunction());

                var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                batch.Completed += (s, e) =>
                {
                    StartNextDrift();
                };

                particle.StartAnimation("Offset", animation);
                batch.End();

                var opacityAnimation = _compositor.CreateScalarKeyFrameAnimation();
                opacityAnimation.InsertKeyFrame(0.0f, 0.8f);
                opacityAnimation.InsertKeyFrame(0.5f, 0.3f);
                opacityAnimation.InsertKeyFrame(1.0f, 0.8f);
                opacityAnimation.Duration = TimeSpan.FromSeconds(3 + _random.NextDouble() * 2);
                opacityAnimation.IterationBehavior = AnimationIterationBehavior.Forever;
                opacityAnimation.Direction = AnimationDirection.Alternate;

                opacityAnimation.DelayTime = TimeSpan.FromSeconds(_random.NextDouble() * opacityAnimation.Duration.TotalSeconds);
                particle.StartAnimation("Opacity", opacityAnimation);
            }
            catch (ObjectDisposedException)
            {

            }
        }

        if (!_isDisposed)
            StartNextDrift();
    }

    private CompositionSurfaceBrush CreateCircleBrushAsync(CompositionGraphicsDevice graphicsDevice, float radius, Color color)
    {
        var size = new Size(radius * 2, radius * 2);
        var surface = graphicsDevice.CreateDrawingSurface(
            size,
            DirectXPixelFormat.B8G8R8A8UIntNormalized,
            DirectXAlphaMode.Premultiplied);

        using (var ds = CanvasComposition.CreateDrawingSession(surface))
        {
            ds.Clear(Colors.Transparent);
            ds.FillCircle(new Vector2(radius, radius), radius, color);
        }

        var brush = _compositor.CreateSurfaceBrush(surface);
        brush.Stretch = CompositionStretch.None;
        return brush;
    }

    private void CleanupParticles()
    {
        if (_particles != null)
        {
            foreach (var visual in _particles)
            {
                visual?.StopAnimation("Offset");
                visual?.StopAnimation("Opacity");

                if (visual is SpriteVisual sprite && sprite.Brush is CompositionSurfaceBrush brush)
                {
                    brush?.Dispose();
                }
            }

            _particles.RemoveAll();
            _particles.Dispose();
            _particles = null;
        }

        _graphicsDevice?.Dispose();
        _graphicsDevice = null;
    }
}

