namespace DevWinUI;

/// <summary>
/// A decorative field of drifting, twinkling particles rendered with Win2D.
/// </summary>
[TemplatePart(Name = PartHost, Type = typeof(Panel))]
public sealed partial class PlayStationParticles : Control
{
    private const string PartHost = "PART_Host";

    private Panel? _host;
    private CanvasAnimatedControl? _canvas;
    private PlayStationParticlesRenderer? _renderer;
    private SolidColorBrush? _trackedBrush;
    private long _brushToken;

    public PlayStationParticles()
    {
        DefaultStyleKey = typeof(PlayStationParticles);

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    public Brush ParticleBrush
    {
        get { return (Brush)GetValue(ParticleBrushProperty); }
        set { SetValue(ParticleBrushProperty, value); }
    }

    public static readonly DependencyProperty ParticleBrushProperty =
        DependencyProperty.Register(nameof(ParticleBrush), typeof(Brush), typeof(PlayStationParticles), new PropertyMetadata(null, OnParticleBrushChanged));

    public double Speed
    {
        get { return (double)GetValue(SpeedProperty); }
        set { SetValue(SpeedProperty, value); }
    }

    public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(nameof(Speed), typeof(double), typeof(PlayStationParticles), new PropertyMetadata(1.0, OnSpeedChanged));

    protected override void OnApplyTemplate()
    {
        Detach();

        base.OnApplyTemplate();

        _host = GetTemplateChild(PartHost) as Panel;

        if (IsLoaded)
        {
            Attach();
        }
    }

    private void OnLoaded(object sender, RoutedEventArgs e) => Attach();

    private void OnUnloaded(object sender, RoutedEventArgs e) => Detach();

    private void Attach()
    {
        if (_host is null || _canvas is not null) return;

        _canvas = new CanvasAnimatedControl
        {
            ClearColor = Colors.Transparent,
            IsHitTestVisible = false,
        };

        _host.Children.Add(_canvas);
        _renderer = new PlayStationParticlesRenderer(_canvas, ResolveColor(ParticleBrush), Speed);

        TrackBrush(ParticleBrush);
    }

    private void Detach()
    {
        UntrackBrush();

        if (_canvas is not null)
        {
            _host?.Children.Remove(_canvas);
            _canvas.RemoveFromVisualTree();
            _canvas = null;
        }

        _renderer?.Dispose();
        _renderer = null;
    }

    private void TrackBrush(Brush? brush)
    {
        UntrackBrush();

        if (brush is SolidColorBrush solid)
        {
            _trackedBrush = solid;
            _brushToken = solid.RegisterPropertyChangedCallback(SolidColorBrush.ColorProperty, OnBrushColorChanged);
        }
    }

    private void UntrackBrush()
    {
        _trackedBrush?.UnregisterPropertyChangedCallback(SolidColorBrush.ColorProperty, _brushToken);
        _trackedBrush = null;
    }

    private void OnBrushColorChanged(DependencyObject sender, DependencyProperty property) =>
        _renderer?.SetParticleColor(ResolveColor(ParticleBrush));

    private static Color ResolveColor(Brush? brush) =>
        brush is SolidColorBrush solid ? solid.Color : Colors.White;

    private static void OnParticleBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (PlayStationParticles)d;
        if (control._renderer is null) return;

        var brush = (Brush?)e.NewValue;
        control.TrackBrush(brush);
        control._renderer.SetParticleColor(ResolveColor(brush));
    }

    private static void OnSpeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((PlayStationParticles)d)._renderer?.SetSpeed((double)e.NewValue);
}
