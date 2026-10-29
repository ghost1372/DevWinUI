namespace DevWinUI;

/// <summary>
/// Stacks its children and reports a height that springs toward the natural height of <see cref="Current"/>.
/// Children are measured with unbounded height, so the animated height never constrains their content.
/// </summary>
public sealed partial class MorphHost : Panel
{
    private const double SubstepSeconds = 0.004;
    private const double MaxFrameSeconds = 0.05;
    private const double ClipSlack = 12;

    private readonly RectangleGeometry _clip = new();
    private UIElement? _current;
    private double _height = double.NaN;
    private double _target;
    private double _velocity;
    private TimeSpan? _lastTime;
    private bool _running;

    public MorphHost()
    {
        Clip = _clip;
        Unloaded += OnUnloaded;
    }

    public double Stiffness
    {
        get { return (double)GetValue(StiffnessProperty); }
        set { SetValue(StiffnessProperty, value); }
    }

    public static readonly DependencyProperty StiffnessProperty =
        DependencyProperty.Register(nameof(Stiffness), typeof(double), typeof(MorphHost), new PropertyMetadata(300.0));

    public double Damping
    {
        get { return (double)GetValue(DampingProperty); }
        set { SetValue(DampingProperty, value); }
    }

    public static readonly DependencyProperty DampingProperty =
        DependencyProperty.Register(nameof(Damping), typeof(double), typeof(MorphHost), new PropertyMetadata(30.0));

    public bool AnimateHeight
    {
        get { return (bool)GetValue(AnimateHeightProperty); }
        set { SetValue(AnimateHeightProperty, value); }
    }

    public static readonly DependencyProperty AnimateHeightProperty =
        DependencyProperty.Register(nameof(AnimateHeight), typeof(bool), typeof(MorphHost), new PropertyMetadata(true));

    public UIElement? Current
    {
        get { return _current; }
        set
        {
            _current = value;
            InvalidateMeasure();
        }
    }

    public void SnapNext()
    {
        Stop();
        _height = double.NaN;
        InvalidateMeasure();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double maxWidth = 0;
        var probe = new Size(availableSize.Width, double.PositiveInfinity);
        foreach (UIElement child in Children)
        {
            child.Measure(probe);
            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
        }

        Retarget(_current?.DesiredSize.Height ?? 0);

        double width = double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width;
        return new Size(width, Math.Max(0, _height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (UIElement child in Children)
        {
            child.Arrange(new Rect(0, 0, finalSize.Width, child.DesiredSize.Height));
        }

        _clip.Rect = new Rect(-ClipSlack, -ClipSlack, finalSize.Width + ClipSlack * 2, finalSize.Height + ClipSlack * 2);
        return finalSize;
    }

    private void Retarget(double content)
    {
        if (double.IsNaN(_height) || !AnimateHeight)
        {
            Stop();
            _height = _target = content;
            _velocity = 0;
            return;
        }

        if (Math.Abs(content - _target) < 0.5)
        {
            return;
        }

        _target = content;
        if (_running)
        {
            return;
        }

        _running = true;
        _lastTime = null;
        CompositionTarget.Rendering += OnFrame;
    }

    private void Stop()
    {
        if (!_running)
        {
            return;
        }

        _running = false;
        CompositionTarget.Rendering -= OnFrame;
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        Stop();
        if (!double.IsNaN(_height))
        {
            _height = _target;
            _velocity = 0;
        }
    }

    private void OnFrame(object? sender, object e)
    {
        TimeSpan time = ((RenderingEventArgs)e).RenderingTime;
        if (_lastTime is not TimeSpan last)
        {
            _lastTime = time;
            return;
        }

        double dt = Math.Clamp((time - last).TotalSeconds, 0, MaxFrameSeconds);
        _lastTime = time;

        double stiffness = Stiffness;
        double damping = Damping;
        int steps = Math.Max(1, (int)Math.Ceiling(dt / SubstepSeconds));
        double h = dt / steps;

        for (int i = 0; i < steps; i++)
        {
            double accel = -stiffness * (_height - _target) - damping * _velocity;
            _velocity += accel * h;
            _height += _velocity * h;
        }

        if (Math.Abs(_height - _target) < 0.1 && Math.Abs(_velocity) < 1)
        {
            _height = _target;
            _velocity = 0;
            Stop();
        }

        InvalidateMeasure();
    }
}
