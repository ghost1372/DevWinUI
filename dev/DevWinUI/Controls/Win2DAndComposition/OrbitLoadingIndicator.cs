namespace DevWinUI;

public partial class OrbitLoadingIndicator : Control
{
    private Compositor compositor;
    private SpriteVisual ellipse_0;
    private SpriteVisual ellipse_1;
    private SpriteVisual ellipse_2;
    private SpriteVisual ellipse_3;
    private ICompositionGenerator generator;
    private ContainerVisual container;
    private List<CompositionAnimation> animations = new List<CompositionAnimation>();

    public bool IsReady = false;

    public bool IsBusy
    {
        get { return (bool)GetValue(IsBusyProperty); }
        set { SetValue(IsBusyProperty, value); }
    }

    public static readonly DependencyProperty IsBusyProperty =
        DependencyProperty.Register(nameof(IsBusy), typeof(bool), typeof(OrbitLoadingIndicator), new PropertyMetadata(false, (d, e) => (d as OrbitLoadingIndicator).Invalidate()));
    public Color? IndicatorColor
    {
        get => (Color?)GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public static readonly DependencyProperty IndicatorColorProperty =
        DependencyProperty.Register(nameof(IndicatorColor), typeof(Color?), typeof(OrbitLoadingIndicator), new PropertyMetadata(null, OnIndicatorColorChanged));

    private static void OnIndicatorColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (OrbitLoadingIndicator)d;
        control.UpdateIndicatorColor(e.NewValue);
    }
    private void UpdateIndicatorColor(object newColor)
    {
        if (!IsReady) return;

        Color color = newColor == null ? GetDefaultColor() : (Color)newColor;

        UpdateBrushColor(ellipse_0, color);
        UpdateBrushColor(ellipse_1, color);
        UpdateBrushColor(ellipse_2, color);
        UpdateBrushColor(ellipse_3, color);
    }

    private void UpdateBrushColor(SpriteVisual visual, Color color)
    {
        if (visual.Brush is CompositionMaskBrush maskBrush &&
            maskBrush.Source is CompositionColorBrush colorBrush)
        {
            colorBrush.Color = color;
        }
    }

    public OrbitLoadingIndicator()
    {
        this.SizeChanged += OrbitLoadingIndicator_SizeChanged;
        this.Loaded += BusyIndicator_Loaded;
    }
    private async void OrbitLoadingIndicator_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (!IsReady)
            return;

        await RebuildForNewSize();
    }
    private async Task RebuildForNewSize()
    {
        float width = (float)ActualWidth;
        float height = (float)ActualHeight;

        container.Size = new Vector2(width, height);

        float radius = width / 2;

        container.Children.RemoveAll();

        if (IndicatorColor == null)
        {
            IndicatorColor = GetDefaultColor();
        }

        ellipse_0 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_1 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_2 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_3 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);

        container.Children.InsertAtBottom(ellipse_0);
        container.Children.InsertAtBottom(ellipse_1);
        container.Children.InsertAtBottom(ellipse_2);
        container.Children.InsertAtBottom(ellipse_3);

        if (IsBusy)
            Invalidate();
    }

    private Color GetDefaultColor()
    {
        return (Color)Application.Current.Resources["SystemAccentColor"];
    }
    private async void BusyIndicator_Loaded(object sender, RoutedEventArgs e)
    {
        if (IndicatorColor == null)
        {
            IndicatorColor = GetDefaultColor();
        }

        compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
        generator = compositor.CreateCompositionGenerator();
        container = compositor.CreateContainerVisual();
        container.Size = new Vector2((float)ActualWidth, (float)ActualHeight);

        float radius = (float)ActualWidth / 2;

        ellipse_0 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_1 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_2 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);
        ellipse_3 = await CreateEllipseVisual((Color)IndicatorColor, radius, radius);


        container.Children.InsertAtBottom(ellipse_0);
        container.Children.InsertAtBottom(ellipse_1);
        container.Children.InsertAtBottom(ellipse_2);
        container.Children.InsertAtBottom(ellipse_3);

        ElementCompositionPreview.SetElementChildVisual(this, container);

        animations.Add(AnimateScale(ellipse_0));
        animations.Add(AnimateScale(ellipse_1, true));
        animations.Add(AnimateScale(ellipse_2));
        animations.Add(AnimateScale(ellipse_3, true));

        animations.Add(AnimateRotation(ellipse_0, 0));
        animations.Add(AnimateRotation(ellipse_1, 0.25f * (float)Math.PI));
        animations.Add(AnimateRotation(ellipse_2, 0.5f * (float)Math.PI));
        animations.Add(AnimateRotation(ellipse_3, 0.75f * (float)Math.PI));

        IsReady = true;
    }

    private async void Invalidate()
    {
        while (!IsReady)
        {
            await Task.Delay(200);
        }

        if (IsBusy)
        {
            ellipse_0.StartAnimation("Scale", animations[0]);
            ellipse_0.StartAnimation("RotationAngle", animations[0 + 4]);

            ellipse_1.StartAnimation("Scale", animations[1]);
            ellipse_1.StartAnimation("RotationAngle", animations[1 + 4]);

            ellipse_2.StartAnimation("Scale", animations[2]);
            ellipse_2.StartAnimation("RotationAngle", animations[2 + 4]);

            ellipse_3.StartAnimation("Scale", animations[3]);
            ellipse_3.StartAnimation("RotationAngle", animations[3 + 4]);
        }
        else
        {
            ellipse_0.StopAnimation("Scale");
            ellipse_1.StopAnimation("Scale");
            ellipse_2.StopAnimation("Scale");
            ellipse_3.StopAnimation("Scale");


            ellipse_0.StopAnimation("RotationAngle");
            ellipse_1.StopAnimation("RotationAngle");
            ellipse_2.StopAnimation("RotationAngle");
            ellipse_3.StopAnimation("RotationAngle");
        }
    }

    private CompositionAnimation AnimateScale(SpriteVisual visual, bool horizontal = true)
    {
        float max = 1.1f;
        float min = 0.5f;
        var linear = compositor.CreateLinearEasingFunction();
        var animation = compositor.CreateVector3KeyFrameAnimation();
        animation.InsertKeyFrame(0.0f, new Vector3(1, 1, 1), linear);
        animation.InsertKeyFrame(0.5f, new Vector3(horizontal ? max : min, horizontal ? min : max, 1), linear);
        animation.InsertKeyFrame(1.0f, new Vector3(1, 1, 1), linear);
        animation.Duration = TimeSpan.FromMilliseconds(2000);
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.StopBehavior = AnimationStopBehavior.SetToInitialValue;
        return animation;
    }

    private CompositionAnimation AnimateRotation(SpriteVisual visual, float offset)
    {
        var linear = compositor.CreateLinearEasingFunction();
        var animation = compositor.CreateScalarKeyFrameAnimation();
        animation.InsertKeyFrame(0.0f, 0f + offset, linear);
        animation.InsertKeyFrame(1.0f, 6.2831853f + offset, linear);
        animation.Duration = TimeSpan.FromMilliseconds(2000);
        animation.IterationBehavior = AnimationIterationBehavior.Forever;
        animation.StopBehavior = AnimationStopBehavior.SetToInitialValue;
        return animation;
    }

    private async Task<SpriteVisual> CreateEllipseVisual(Color color, float radiusX = 20, float radiusY = 20, float strokeWidth = 2)
    {
        var ellipse = compositor.CreateSpriteVisual();
        ellipse.Size = new Vector2(2 * (radiusX + strokeWidth), 2 * (radiusY + strokeWidth));

        var ellipseGeometry = CreateOutlinedEllipseGeometry(new Vector2(radiusX + strokeWidth, radiusY + strokeWidth), radiusX, radiusY, strokeWidth);

        ellipse.Brush = await CreateMask(color, ellipseGeometry, ellipse.Size);
        ellipse.CenterPoint = new Vector3(radiusX + strokeWidth, radiusY + strokeWidth, 0);
        return ellipse;
    }

    private async Task<CompositionMaskBrush> CreateMask(Color color, CanvasGeometry geometry, Vector2 size)
    {
        var compositionMask = generator.CreateMaskSurface(size.ToSize(), geometry);

        var mask = compositor.CreateSurfaceBrush(compositionMask.Surface);
        var source = compositor.CreateColorBrush(color);

        var maskBrush = compositor.CreateMaskBrush();
        maskBrush.Mask = mask;
        maskBrush.Source = source;

        return maskBrush;
    }

    private CanvasGeometry CreateOutlinedEllipseGeometry(Vector2 center, float radiusX, float radiusY, float strokewidth)
    {
        var inner = CanvasGeometry.CreateEllipse(generator.Device, center, radiusX, radiusY);
        var outer = CanvasGeometry.CreateEllipse(generator.Device, center, radiusX + strokewidth, radiusY + strokewidth);

        return outer.CombineWith(inner, Matrix3x2.Identity, CanvasGeometryCombine.Exclude);
    }
}
