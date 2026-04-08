namespace DevWinUI;

[TemplatePart(Name = PART_Side1Content, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_Side2Content, Type = typeof(ContentPresenter))]
[TemplatePart(Name = PART_LayoutRoot, Type = typeof(Grid))]
public sealed partial class FlipSide : Control
{
    private const string PART_Side1Content = "Side1Content";
    private const string PART_Side2Content = "Side2Content";
    private const string PART_LayoutRoot = "LayoutRoot";

    public bool IsFlipped
    {
        get { return (bool)GetValue(IsFlippedProperty); }
        set { SetValue(IsFlippedProperty, value); }
    }
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

    public object Side1
    {
        get { return (object)GetValue(Side1Property); }
        set { SetValue(Side1Property, value); }
    }
    public static readonly DependencyProperty Side1Property =
        DependencyProperty.Register(nameof(Side1), typeof(object), typeof(FlipSide), new PropertyMetadata(null));

    public object Side2
    {
        get { return (object)GetValue(Side2Property); }
        set { SetValue(Side2Property, value); }
    }
    public static readonly DependencyProperty Side2Property =
        DependencyProperty.Register(nameof(Side2), typeof(object), typeof(FlipSide), new PropertyMetadata(null));

    public Vector2 Axis
    {
        get { return (Vector2)GetValue(AxisProperty); }
        set { SetValue(AxisProperty, value); }
    }
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

    public FlipOrientationMode FlipOrientation
    {
        get { return (FlipOrientationMode)GetValue(FlipOrientationProperty); }
        set { SetValue(FlipOrientationProperty, value); }
    }
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

    public float AnimationDampingRatio
    {
        get { return (float)GetValue(AnimationDampingRatioProperty); }
        set { SetValue(AnimationDampingRatioProperty, value); }
    }

    public static readonly DependencyProperty AnimationDampingRatioProperty =
        DependencyProperty.Register(nameof(AnimationDampingRatio), typeof(float), typeof(FlipSide), new PropertyMetadata(0.5f, OnAnimationChanged));

    public TimeSpan AnimationDuration
    {
        get { return (TimeSpan)GetValue(AnimationDurationProperty); }
        set { SetValue(AnimationDurationProperty, value); }
    }

    public static readonly DependencyProperty AnimationDurationProperty =
        DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(FlipSide), new PropertyMetadata(TimeSpan.FromMilliseconds(200), OnAnimationChanged));

    private static void OnAnimationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctl = (FlipSide)d;
        if (ctl != null)
        {
            ctl.InitComposition();
        }
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

        Side1Content = GetTemplateChild(PART_Side1Content) as ContentPresenter;
        Side2Content = GetTemplateChild(PART_Side2Content) as ContentPresenter;
        LayoutRoot = GetTemplateChild(PART_LayoutRoot) as Grid;

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
        springAnimation1.DampingRatio = AnimationDampingRatio;
        springAnimation1.Period = AnimationDuration;
        springAnimation1.FinalValue = 180f;

        springAnimation2 = compositor.CreateSpringScalarAnimation();
        springAnimation2.DampingRatio = AnimationDampingRatio;
        springAnimation2.Period = AnimationDuration;
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
            if (s1Visual != null)
            {
                s1Visual.RotationAngleInDegrees = f1;
            }
            if (s2Visual != null)
            {
                s2Visual.RotationAngleInDegrees = f2;
            }
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
