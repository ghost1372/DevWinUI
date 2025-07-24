namespace DevWinUI;

[TemplatePart(Name = nameof(PART_MainPanel), Type = typeof(Grid))]
public partial class FlipToReveal : Control
{
    private const string PART_MainPanel = "PART_MainPanel";
    private Grid _mainPanel;
    private Boolean IsFlipped = false;

    public FrameworkElement PrimaryContent
    {
        get { return (FrameworkElement)GetValue(PrimaryContentProperty); }
        set { SetValue(PrimaryContentProperty, value); }
    }

    public static readonly DependencyProperty PrimaryContentProperty =
        DependencyProperty.Register(nameof(PrimaryContent), typeof(FrameworkElement), typeof(FlipToReveal), new PropertyMetadata(null));

    public FrameworkElement SecondaryContent
    {
        get { return (FrameworkElement)GetValue(SecondaryContentProperty); }
        set { SetValue(SecondaryContentProperty, value); }
    }

    public static readonly DependencyProperty SecondaryContentProperty =
        DependencyProperty.Register(nameof(SecondaryContent), typeof(object), typeof(FlipToReveal), new PropertyMetadata(null));

    public TimeSpan RotationDuration
    {
        get { return (TimeSpan)GetValue(RotationDurationProperty); }
        set { SetValue(RotationDurationProperty, value); }
    }

    public static readonly DependencyProperty RotationDurationProperty =
        DependencyProperty.Register(nameof(RotationDuration), typeof(TimeSpan), typeof(FlipToReveal), new PropertyMetadata(TimeSpan.FromMilliseconds(800)));

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _mainPanel = GetTemplateChild(PART_MainPanel) as Grid;
        _mainPanel.Tapped -= OnMainPanelTapped;
        _mainPanel.Tapped += OnMainPanelTapped;
        SizeChanged -= FlipToReveal_SizeChanged;
        SizeChanged += FlipToReveal_SizeChanged;
    }

    private void OnMainPanelTapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        Visual visual = ElementCompositionPreview.GetElementVisual(SecondaryContent);
        Compositor compositor = visual.Compositor;

        visual.Size = new Vector2((float)SecondaryContent.ActualWidth / 2, (float)SecondaryContent.ActualHeight / 2);

        // Rotate around the X-axis
        visual.RotationAxis = new Vector3(1, 0, 0);

        // Start the rotation animation
        LinearEasingFunction linear = compositor.CreateLinearEasingFunction();
        ScalarKeyFrameAnimation rotationAnimation = compositor.CreateScalarKeyFrameAnimation();
        if (!IsFlipped) // default
        {
            rotationAnimation.InsertKeyFrame(0, 0, linear);
            rotationAnimation.InsertKeyFrame(1, 250f, linear); // flip over
        }
        else
        {
            rotationAnimation.InsertKeyFrame(0, 250f, linear);
            rotationAnimation.InsertKeyFrame(1, 0f, linear);   // flip back

        }
        rotationAnimation.Duration = RotationDuration;

        var transaction = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
        transaction.Completed += Animation_Completed;

        if (IsFlipped)
            SecondaryContent.Visibility = Microsoft.UI.Xaml.Visibility.Visible;

        visual.StartAnimation("RotationAngleInDegrees", rotationAnimation);

        transaction.End();
    }

    private void FlipToReveal_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        UpdatePerspective();
    }

    private void Animation_Completed(object sender, CompositionBatchCompletedEventArgs args)
    {
        IsFlipped = !IsFlipped;

        if (IsFlipped)
            SecondaryContent.Visibility = Microsoft.UI.Xaml.Visibility.Collapsed;
    }

    private void UpdatePerspective()
    {
        Visual visual = ElementCompositionPreview.GetElementVisual(_mainPanel);

        // Get the size of the area we are enabling perspective for
        Vector2 sizeList = new Vector2((float)_mainPanel.ActualWidth, (float)_mainPanel.ActualHeight);

        // Setup the perspective transform.
        Matrix4x4 perspective = new Matrix4x4(
                    1.0f, 0.0f, 0.0f, 0.0f,
                    0.0f, 1.0f, 0.0f, 0.0f,
                    0.0f, 0.0f, 1.0f, -1.0f / sizeList.X,
                    0.0f, 0.0f, 0.0f, 1.0f);

        // Set the parent transform to apply perspective to all children
        visual.TransformMatrix =
                           Matrix4x4.CreateTranslation(-sizeList.X / 2, -sizeList.Y / 2, 0f) *      // Translate to origin
                           perspective *                                                            // Apply perspective at origin
                           Matrix4x4.CreateTranslation(sizeList.X / 2, sizeList.Y / 2, 0f);         // Translate back to original position

    }
    public void SimulateFlip()
    {
        OnMainPanelTapped(_mainPanel, null);
    }
}
