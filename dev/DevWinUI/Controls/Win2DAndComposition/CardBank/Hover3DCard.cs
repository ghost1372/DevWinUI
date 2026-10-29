namespace DevWinUI;

public partial class Hover3DCard : ContentControl
{
    public double MaxTilt
    {
        get { return (double)GetValue(MaxTiltProperty); }
        set { SetValue(MaxTiltProperty, value); }
    }

    public static readonly DependencyProperty MaxTiltProperty =
        DependencyProperty.Register(nameof(MaxTilt), typeof(double), typeof(Hover3DCard), new PropertyMetadata(8.0));

    public double HoverScale
    {
        get { return (double)GetValue(HoverScaleProperty); }
        set { SetValue(HoverScaleProperty, value); }
    }

    public static readonly DependencyProperty HoverScaleProperty =
        DependencyProperty.Register(nameof(HoverScale), typeof(double), typeof(Hover3DCard), new PropertyMetadata(1.05));

    public double Perspective
    {
        get { return (double)GetValue(PerspectiveProperty); }
        set { SetValue(PerspectiveProperty, value); }
    }

    public static readonly DependencyProperty PerspectiveProperty =
        DependencyProperty.Register(nameof(Perspective), typeof(double), typeof(Hover3DCard), new PropertyMetadata(1200.0));

    public double SmoothingMs
    {
        get { return (double)GetValue(SmoothingMsProperty); }
        set { SetValue(SmoothingMsProperty, value); }
    }

    public static readonly DependencyProperty SmoothingMsProperty =
        DependencyProperty.Register(nameof(SmoothingMs), typeof(double), typeof(Hover3DCard), new PropertyMetadata(60.0));

    private readonly Visual _visual;
    private TimeSpan? _lastTime;
    private bool _rendering;

    private float _cx, _cy, _cs = 1f;
    private float _tx, _ty, _ts = 1f;

    public Hover3DCard()
    {
        DefaultStyleKey = typeof(Hover3DCard);
        _visual = ElementCompositionPreview.GetElementVisual(this);

        PointerEntered += (s, e) => SetTarget(e.GetCurrentPoint(this).Position);
        PointerMoved += (s, e) => SetTarget(e.GetCurrentPoint(this).Position);
        PointerExited += (s, e) => Reset();
        PointerCanceled += (s, e) => Reset();
        Unloaded += (s, e) => StopRendering();
    }

    protected virtual void OnTiltUpdated(float nx, float ny, float hoverProgress) { }

    private void SetTarget(Windows.Foundation.Point p)
    {
        float w = (float)ActualWidth, h = (float)ActualHeight;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        _tx = Math.Clamp((float)p.X / w * 2f - 1f, -1f, 1f);
        _ty = Math.Clamp((float)p.Y / h * 2f - 1f, -1f, 1f);
        _ts = (float)HoverScale;
        StartRendering();
    }

    private void Reset()
    {
        _tx = 0f;
        _ty = 0f;
        _ts = 1f;
        StartRendering();
    }

    private void StartRendering()
    {
        if (_rendering)
        {
            return;
        }

        _rendering = true;
        _lastTime = null;
        CompositionTarget.Rendering += OnRendering;
    }

    private void StopRendering()
    {
        if (!_rendering)
        {
            return;
        }

        _rendering = false;
        CompositionTarget.Rendering -= OnRendering;
    }

    private void OnRendering(object? sender, object e)
    {
        TimeSpan time = ((RenderingEventArgs)e).RenderingTime;
        if (_lastTime is not TimeSpan last)
        {
            _lastTime = time;
            return;
        }

        float dt = (float)Math.Max(0.0, (time - last).TotalMilliseconds);
        _lastTime = time;

        float k = 1f - MathF.Exp(-dt / (float)Math.Max(1.0, SmoothingMs));
        _cx += (_tx - _cx) * k;
        _cy += (_ty - _cy) * k;
        _cs += (_ts - _cs) * k;

        bool settled = MathF.Abs(_tx - _cx) < 0.001f
                    && MathF.Abs(_ty - _cy) < 0.001f
                    && MathF.Abs(_ts - _cs) < 0.0005f;
        if (settled)
        {
            _cx = _tx;
            _cy = _ty;
            _cs = _ts;
        }

        Apply();

        if (settled)
        {
            StopRendering();
        }
    }

    private void Apply()
    {
        float w = (float)ActualWidth, h = (float)ActualHeight;
        if (w <= 0 || h <= 0)
        {
            return;
        }

        float deg2rad = MathF.PI / 180f;
        float rotY = _cx * (float)MaxTilt * deg2rad;
        float rotX = -_cy * (float)MaxTilt * deg2rad;

        var persp = Matrix4x4.Identity;
        persp.M34 = -1f / (float)Math.Max(1.0, Perspective);

        _visual.TransformMatrix =
              Matrix4x4.CreateTranslation(-w / 2, -h / 2, 0)
            * Matrix4x4.CreateScale(_cs, _cs, 1f)
            * Matrix4x4.CreateRotationX(rotX)
            * Matrix4x4.CreateRotationY(rotY)
            * persp
            * Matrix4x4.CreateTranslation(w / 2, h / 2, 0);

        float hs = (float)HoverScale;
        float progress = hs > 1f ? Math.Clamp((_cs - 1f) / (hs - 1f), 0f, 1f) : 0f;
        OnTiltUpdated(_cx, _cy, progress);
    }
}
