namespace DevWinUI;
public sealed partial class FlipSide : Control
{
    public static readonly DependencyProperty IsFlippedProperty =
        DependencyProperty.Register(nameof(IsFlipped), typeof(bool), typeof(FlipSide), new PropertyMetadata(false, (s, a) =>
        {
            if (a.NewValue != a.OldValue)
            {
                if (s is FlipSide sender)
                {
                    sender.OnIsFlippedChanged();
                }
            }
        }));

    public static readonly DependencyProperty Side1Property =
        DependencyProperty.Register(nameof(Side1), typeof(object), typeof(FlipSide), new PropertyMetadata(null));

    public static readonly DependencyProperty Side2Property =
        DependencyProperty.Register(nameof(Side2), typeof(object), typeof(FlipSide), new PropertyMetadata(null));

    public static readonly DependencyProperty AxisProperty =
        DependencyProperty.Register(nameof(Axis), typeof(Vector2), typeof(FlipSide), new PropertyMetadata(new Vector2(0, 1), (d, e) =>
        {
            var ctl = (FlipSide)d;
            if (ctl != null)
            {
                ctl.UpdateAxis(ctl.Side1Content, (Vector2)e.NewValue);
                ctl.UpdateAxis(ctl.Side2Content, (Vector2)e.NewValue);
            }
        }));


    public static readonly DependencyProperty FlipOrientationProperty =
        DependencyProperty.Register(nameof(FlipOrientation), typeof(FlipOrientationMode), typeof(FlipSide), new PropertyMetadata(FlipOrientationMode.Horizontal, (d, e) =>
        {
            var ctl = d as FlipSide;
            if (ctl != null)
            {
                switch ((FlipOrientationMode)e.NewValue)
                {
                    case FlipOrientationMode.Horizontal:
                        ctl.SetValue(AxisProperty, new Vector2(0, 1));
                        break;
                    case FlipOrientationMode.Vertical:
                        ctl.SetValue(AxisProperty, new Vector2(1, 0));
                        break;
                }
            }
        }));

    public bool IsFlipped
    {
        get { return (bool)GetValue(IsFlippedProperty); }
        set { SetValue(IsFlippedProperty, value); }
    }

    public object Side1
    {
        get { return (object)GetValue(Side1Property); }
        set { SetValue(Side1Property, value); }
    }

    public object Side2
    {
        get { return (object)GetValue(Side2Property); }
        set { SetValue(Side2Property, value); }
    }
    public Vector2 Axis
    {
        get { return (Vector2)GetValue(AxisProperty); }
        set { SetValue(AxisProperty, value); }
    }
    public FlipOrientationMode FlipOrientation
    {
        get { return (FlipOrientationMode)GetValue(FlipOrientationProperty); }
        set { SetValue(FlipOrientationProperty, value); }
    }

    private Grid LayoutRoot;

    private Visual s1Visual;

    private Visual s2Visual;

    private ContentPresenter Side1Content;

    private ContentPresenter Side2Content;

    private SpringScalarNaturalMotionAnimation springAnimation1;

    private SpringScalarNaturalMotionAnimation springAnimation2;

    public FlipSide()
    {
        this.DefaultStyleKey = typeof(FlipSide);
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        Side1Content = GetTemplateChild("Side1Content") as ContentPresenter;
        Side2Content = GetTemplateChild("Side2Content") as ContentPresenter;
        LayoutRoot = GetTemplateChild("LayoutRoot") as Grid;

        InitComposition();
    }

    private void InitComposition()
    {
        if (Side1Content == null || Side2Content == null || LayoutRoot == null) return;

        s1Visual = ElementCompositionPreview.GetElementVisual(Side1Content);
        s2Visual = ElementCompositionPreview.GetElementVisual(Side2Content);

        var compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;

        var opacity1Animation = compositor.CreateExpressionAnimation("this.Target.RotationAngleInDegrees > 90 ? 0f : 1f");
        var opacity2Animation = compositor.CreateExpressionAnimation("(this.Target.RotationAngleInDegrees - 180) > 90 ? 1f : 0f");

        s1Visual.StartAnimation("Opacity", opacity1Animation);
        s2Visual.StartAnimation("Opacity", opacity2Animation);

        OnIsFlippedChanged();

        springAnimation1 = compositor.CreateSpringScalarAnimation();
        springAnimation1.DampingRatio = 0.5f;
        springAnimation1.Period = TimeSpan.FromMilliseconds(200);
        springAnimation1.FinalValue = 180f;

        springAnimation2 = compositor.CreateSpringScalarAnimation();
        springAnimation2.DampingRatio = 0.5f;
        springAnimation2.Period = TimeSpan.FromMilliseconds(200);
        springAnimation2.FinalValue = 180f;

        UpdateAxis(Side1Content, Axis);
        UpdateAxis(Side2Content, Axis);
        UpdateTransformMatrix(LayoutRoot);

        LayoutRoot.SizeChanged += LayoutRoot_SizeChanged;
    }

    public void UpdateAnimations(SpringScalarNaturalMotionAnimation animation1, SpringScalarNaturalMotionAnimation animation2)
    {
        springAnimation1 = animation1;
        springAnimation2 = animation2;
    }

    private void LayoutRoot_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdateTransformMatrix(LayoutRoot);
        UpdateAxis(Side1Content, Axis);
        UpdateAxis(Side2Content, Axis);
    }

    private void OnIsFlippedChanged()
    {
        float f1 = 0f, f2 = 0f;
        if (IsFlipped)
        {
            f1 = 180f;
            f2 = 360f;
            VisualStateManager.GoToState(this, "Slide2", false);
        }
        else
        {
            f1 = 0f;
            f2 = 180f;
            VisualStateManager.GoToState(this, "Slide1", false);
        }
        if (springAnimation1 != null && springAnimation2 != null)
        {
            springAnimation1.FinalValue = f1;
            springAnimation2.FinalValue = f2;
            s1Visual.StartAnimation("RotationAngleInDegrees", springAnimation1);
            s2Visual.StartAnimation("RotationAngleInDegrees", springAnimation2);
        }
        else
        {
            s1Visual.RotationAngleInDegrees = f1;
            s2Visual.RotationAngleInDegrees = f2;
        }
    }

    private void UpdateAxis(FrameworkElement element, Vector2 vector2)
    {
        if (element == null)
        {
            return;
        }
        var visual = ElementCompositionPreview.GetElementVisual(element);
        var size = element.RenderSize.ToVector2();

        visual.CenterPoint = new Vector3(size.X / 2, size.Y / 2, 0f);
        visual.RotationAxis = new Vector3(vector2, 0f);
    }

    private void UpdateTransformMatrix(FrameworkElement element)
    {
        var host = ElementCompositionPreview.GetElementVisual(element);
        var size = element.RenderSize.ToVector2();
        if (size.X == 0 || size.Y == 0) return;
        var n = -1f / size.X;

        Matrix4x4 perspective = new Matrix4x4(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, 1.0f, n,
            0.0f, 0.0f, 0.0f, 1.0f);

        host.TransformMatrix =
            Matrix4x4.CreateTranslation(-size.X / 2, -size.Y / 2, 0f) *
            perspective *
            Matrix4x4.CreateTranslation(size.X / 2, size.Y / 2, 0f);
    }
}
