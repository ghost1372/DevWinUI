using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Shapes;

namespace DevWinUI;

[TemplatePart(Name = nameof(PART_Canvas), Type = typeof(Canvas))]
public partial class SnowFlakeEffect : Control
{
    private const string PART_Canvas = "PART_Canvas";
    private Canvas _canvas;

    private readonly Random _random = new(); 
    private readonly List<SnowFlake> _snowFlakes = []; 
    private double mX = -100;
    private double mY = -100;

    public bool AutoStart
    {
        get { return (bool)GetValue(AutoStartProperty); }
        set { SetValue(AutoStartProperty, value); }
    }

    public static readonly DependencyProperty AutoStartProperty =
        DependencyProperty.Register(nameof(AutoStart), typeof(bool), typeof(SnowFlakeEffect), new PropertyMetadata(true));

    public int FlakeCount
    {
        get { return (int)GetValue(FlakeCountProperty); }
        set { SetValue(FlakeCountProperty, value); }
    }

    public static readonly DependencyProperty FlakeCountProperty =
        DependencyProperty.Register(nameof(FlakeCount), typeof(int), typeof(SnowFlakeEffect), new PropertyMetadata(188, OnFlakeCountChanged));

    private static void OnFlakeCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (SnowFlakeEffect)d;
        if (ctl != null)
        {
            ctl.Stop();
            ctl.Start();
        }
    }

    public SnowFlakeEffect()
    {
        DefaultStyleKey = typeof(SnowFlakeEffect);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _canvas = GetTemplateChild(PART_Canvas) as Canvas;

        PointerMoved -= OnPointerMove;
        PointerMoved += OnPointerMove;

        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;

        Unloaded -= OnUnLoaded;
        Unloaded += OnUnLoaded;

        if (AutoStart)
        {
            LayoutUpdated += SnowFlakeEffect_LayoutUpdated;
        }
    }

    private void SnowFlakeEffect_LayoutUpdated(object sender, object e)
    {
        LayoutUpdated -= SnowFlakeEffect_LayoutUpdated;

        Start();
    }

    private void OnUnLoaded(object sender, RoutedEventArgs e)
    {
        Stop();
        PointerMoved -= OnPointerMove;
        SizeChanged -= OnSizeChanged;
        Unloaded -= OnUnLoaded;
    }

    public void Start()
    {
        InitSnowFlakes();
    }

    public void Stop()
    {
        if (_canvas == null)
            return;

        ClearSnowFlakes();

        _canvas.Children.Clear();
    }

    private void InitSnowFlakes()
    {
        if (_canvas == null)
            return;

        for (int i = 0; i < FlakeCount; i++)
        {
            CreateSnowFlake();
        }

        CompositionTarget.Rendering -= UpdateSnowFlakes;
        CompositionTarget.Rendering += UpdateSnowFlakes;
    }

    private void CreateSnowFlake()
    {
        double size = (_random.NextDouble() * 3) + 2; // Snowflake size
        double speed = (_random.NextDouble() * 1) + 0.5; // Falling speed
        double opacity = (_random.NextDouble() * 0.5) + 0.3; // Opacity
        double x = _random.NextDouble() * _canvas.ActualWidth; // Initial X position
        double y = _random.NextDouble() * _canvas.ActualHeight; // Initial Y position

        Ellipse flakeShape = new()
        {
            Width = size,
            Height = size,
            Fill = new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), 255, 255, 255)),
        };

        var transform = new TranslateTransform();
        transform.X = x;
        transform.Y = y;
        flakeShape.RenderTransform = transform;

        _canvas.Children.Add(flakeShape);

        SnowFlake flake = new()
        {
            Shape = flakeShape,
            X = x,
            Y = y,
            Size = size,
            Speed = speed,
            Opacity = opacity,
            VelX = 0,
            VelY = speed,
            StepSize = _random.NextDouble() / 30 * 1,
            Step = 0,
            Angle = 180,
            Transform = transform,
        };

        _snowFlakes.Add(flake);
    }

    private void UpdateSnowFlakes(object sender, object e)
    {
        if (_canvas.ActualWidth == 0 || _canvas.ActualHeight == 0)
        {
            return;
        }

        foreach (SnowFlake flake in _snowFlakes)
        {
            double x = mX;
            double y = mY;
            double minDist = 150;
            double x2 = flake.X;
            double y2 = flake.Y;

            double dist = Math.Sqrt(((x2 - x) * (x2 - x)) + ((y2 - y) * (y2 - y)));

            if (dist < minDist)
            {
                double force = minDist / (dist * dist);
                double xcomp = (x - x2) / dist;
                double ycomp = (y - y2) / dist;
                double deltaV = force / 2;

                flake.VelX -= deltaV * xcomp;
                flake.VelY -= deltaV * ycomp;
            }
            else
            {
                flake.VelX *= 0.98;
                if (flake.VelY <= flake.Speed)
                {
                    flake.VelY = flake.Speed;
                }

                flake.VelX += Math.Cos(flake.Step += 0.05) * flake.StepSize;
            }

            flake.Y += flake.VelY;
            flake.X += flake.VelX;

            if (flake.Y >= _canvas.ActualHeight || flake.Y <= 0)
            {
                ResetFlake(flake);
            }

            if (flake.X >= _canvas.ActualWidth || flake.X <= 0)
            {
                ResetFlake(flake);
            }

            flake.Transform!.SetValue(TranslateTransform.XProperty, flake.X);
            flake.Transform!.SetValue(TranslateTransform.YProperty, flake.Y);
        }
    }

    private void ResetFlake(SnowFlake flake)
    {
        flake.X = _random.NextDouble() * _canvas.ActualWidth;
        flake.Y = 0;
        flake.Size = (_random.NextDouble() * 3) + 2;
        flake.Speed = (_random.NextDouble() * 1) + 0.5;
        flake.VelY = flake.Speed;
        flake.VelX = 0;
        flake.Opacity = (_random.NextDouble() * 0.5) + 0.3;

        if (flake.Shape == null)
        {
            return;
        }

        flake.Shape.SetValue(FrameworkElement.WidthProperty, flake.Size);
        flake.Shape.SetValue(FrameworkElement.HeightProperty, flake.Size);
        flake.Shape.SetValue(
            Shape.FillProperty,
            new SolidColorBrush(Color.FromArgb((byte)(flake.Opacity * 255), 255, 255, 255))
        );
    }

    private void ClearSnowFlakes()
    {
        foreach (SnowFlake flake in _snowFlakes)
        {
            _canvas.Children.Remove(flake.Shape);
        }

        _snowFlakes.Clear();
    }

    private void OnPointerMove(object sender, PointerRoutedEventArgs e)
    {
        var pos = e.GetCurrentPoint(_canvas);
        mX = pos.Position.X;
        mY = pos.Position.Y;
    }

    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        _canvas.SetValue(FrameworkElement.WidthProperty, e.NewSize.Width);
        _canvas.SetValue(FrameworkElement.HeightProperty, e.NewSize.Height);
    }
}
