// https://github.com/DinoChan

using Windows.Foundation;

namespace DevWinUI;
public partial class WaveCircle : FrameworkElement
{
    public static readonly DependencyProperty ColorBrushProperty =
        DependencyProperty.Register(nameof(ColorBrush), typeof(Color), typeof(WaveCircle), new PropertyMetadata(Colors.Red, OnColorChanged));
    public Color ColorBrush
    {
        get { return (Color)GetValue(ColorBrushProperty); }
        set { SetValue(ColorBrushProperty, value); }
    }

    public static readonly DependencyProperty IsAnimateProperty =
        DependencyProperty.Register(nameof(IsAnimate), typeof(bool), typeof(WaveCircle), new PropertyMetadata(false, OnAnimationChanged));
    public bool IsAnimate
    {
        get { return (bool)GetValue(IsAnimateProperty); }
        set { SetValue(IsAnimateProperty, value); }
    }
    
    public static readonly DependencyProperty IsReverseAnimationProperty =
        DependencyProperty.Register(nameof(IsReverseAnimation), typeof(bool), typeof(WaveCircle), new PropertyMetadata(false, OnIsReverseAnimationChanged));

    public bool IsReverseAnimation
    {
        get { return (bool)GetValue(IsReverseAnimationProperty); }
        set { SetValue(IsReverseAnimationProperty, value); }
    }

    private static void OnIsReverseAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveCircle)d;
        if (ctl != null)
        {
            ctl.IsAnimate = !ctl.IsAnimate;
            ctl.IsAnimate = !ctl.IsAnimate;
        }
    }

    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveCircle)d;
        if (ctl != null)
        {
            ctl.DrawWaveCircle(ctl.RenderSize, (bool)e.NewValue);
        }
    }

    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (WaveCircle)d;
        if (ctl != null)
        {
            ctl.DrawWaveCircle(ctl.RenderSize, ctl.IsAnimate);
        }
    }

    private ContainerVisual _containerVisual;
    private Visual _visual;
    public WaveCircle()
    {
        SizeChanged -= OnSizeChanged;
        SizeChanged += OnSizeChanged;
    }

    public void StartAnimation(Visual visual)
    {
        ContainerVisual containerVisual = visual.Compositor.CreateContainerVisual();
        containerVisual.Children.InsertAtTop(visual);
        containerVisual.CenterPoint = new Vector3((float)(RenderSize.Width / 2), (float)(RenderSize.Height / 2), 0);
        ElementCompositionPreview.SetElementChildVisual(this, containerVisual);

        var rotationAnimation = visual.Compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation = visual.Compositor.CreateScalarKeyFrameAnimation();
        rotationAnimation.InsertKeyFrame(0.0f, 0.0f);
        //rotationAnimation.InsertKeyFrame(0.5f, 180.0f); // Rotate halfway

        var circleAngle = 360.0f;
        if (IsReverseAnimation)
        {
            circleAngle = -360.0f;
        }
        rotationAnimation.InsertKeyFrame(1.0f, circleAngle);
        rotationAnimation.Duration = TimeSpan.FromSeconds(2); // Set the duration of the rotation
        rotationAnimation.IterationBehavior = AnimationIterationBehavior.Forever; // Repeat the animation indefinitely

        // Start the rotation animation
        containerVisual.StartAnimation("RotationAngleInDegrees", rotationAnimation);
        _containerVisual = containerVisual;
    }
    public void StopAnimation()
    {
        if (_containerVisual != null)
        {
            _containerVisual.StopAnimation("RotationAngleInDegrees");
        }
    }

    public Visual GetVisual()
    {
        return _visual;
    }
    private void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        DrawWaveCircle(e.NewSize, IsAnimate);
    }

    public void DrawWaveCircle(Size size, bool isAnimation)
    {
        var length = (float)Math.Min(size.Width, size.Height) * 0.95;
        var centerX = (float)size.Width / 2;
        var centerY = (float)size.Height / 2;

        var points = new List<Vector2>();
        var r = length / 2;
        var r2 = r * 1.06;
        var r3 = r * 0.951;
        int index = 0;
        int segments = 100;
        for (int i = 0; i < segments; i += 2)
        {
            var x = r * Math.Cos(i * 2 * Math.PI / segments) + centerX;
            var y = r * Math.Sin(i * 2 * Math.PI / segments) + centerY;

            points.Add(new Vector2((float)x, (float)y));
            var currentR = index++ % 2 == 0 ? r2 : r3;
            x = currentR * Math.Cos((i + 1) * 2 * Math.PI / segments) + centerX;
            y = currentR * Math.Sin((i + 1) * 2 * Math.PI / segments) + centerY;

            points.Add(new Vector2((float)x, (float)y));
        }

        points.Add(points[0]);

        CanvasGeometry result;
        using (var builder = new CanvasPathBuilder(null))
        {
            builder.BeginFigure(points[0]);
            for (int i = 0; i < points.Count - 2; i += 2)
            {
                var currentPoint = points[i];
                var centerPoint = points[i + 1];
                var nextPoint = points[i + 2];
                builder.AddCubicBezier(currentPoint, centerPoint, nextPoint);
            }
            builder.EndFigure(CanvasFigureLoop.Open);

            result = CanvasGeometry.CreatePath(builder);
        }
        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        var path = new CompositionPath(result);
        var line3 = compositor.CreatePathGeometry();
        line3.Path = path;
        var shape3 = compositor.CreateSpriteShape(line3);
        shape3.FillBrush = compositor.CreateColorBrush(ColorBrush);
        var visual = compositor.CreateShapeVisual();
        visual.Shapes.Add(shape3);
        visual.Size = size.ToVector2();
        ElementCompositionPreview.SetElementChildVisual(this, visual);
        _visual = visual;

        if (isAnimation)
        {
            StartAnimation(visual);
        }
    }
}
