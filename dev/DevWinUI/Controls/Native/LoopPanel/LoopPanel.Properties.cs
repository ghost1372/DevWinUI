namespace DevWinUI;

public partial class LoopPanel
{
    public double Spacing
    {
        get => (double)GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }
    public static readonly DependencyProperty SpacingProperty =
        DependencyProperty.Register(nameof(Spacing), typeof(double), typeof(LoopPanel), new PropertyMetadata(0.0, OnSpacingChanged));

    private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is LoopPanel panel)
        {
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
    }

    public double MouseWheelScrollFactor
    {
        get => (double)GetValue(MouseWheelScrollFactorProperty);
        set => SetValue(MouseWheelScrollFactorProperty, value);
    }
    public static readonly DependencyProperty MouseWheelScrollFactorProperty =
        DependencyProperty.Register(nameof(MouseWheelScrollFactor), typeof(double), typeof(LoopPanel), new PropertyMetadata(0.25));

    public double DragScrollFactor
    {
        get => (double)GetValue(DragScrollFactorProperty);
        set => SetValue(DragScrollFactorProperty, value);
    }
    public static readonly DependencyProperty DragScrollFactorProperty =
        DependencyProperty.Register(nameof(DragScrollFactor), typeof(double), typeof(LoopPanel), new PropertyMetadata(1.0));

    public bool IsInertiaEnabled
    {
        get => (bool)GetValue(IsInertiaEnabledProperty);
        set => SetValue(IsInertiaEnabledProperty, value);
    }
    public static readonly DependencyProperty IsInertiaEnabledProperty =
        DependencyProperty.Register(nameof(IsInertiaEnabled), typeof(bool), typeof(LoopPanel), new PropertyMetadata(true));

    public bool BringChildrenIntoView
    {
        get { return (bool)GetValue(BringChildrenIntoViewProperty); }
        set { SetValue(BringChildrenIntoViewProperty, value); }
    }
    public static readonly DependencyProperty BringChildrenIntoViewProperty =
        DependencyProperty.Register(nameof(BringChildrenIntoView),typeof(bool), typeof(LoopPanel), new PropertyMetadata(false));

    public double Offset
    {
        get { return (double)GetValue(OffsetProperty); }
        set { SetValue(OffsetProperty, value); }
    }
    public static readonly DependencyProperty OffsetProperty =
        DependencyProperty.Register(nameof(Offset), typeof(double), typeof(LoopPanel), new PropertyMetadata(0.5d));

    public Orientation Orientation
    {
        get { return (Orientation)GetValue(OrientationProperty); }
        set { SetValue(OrientationProperty, value); }
    }
    public static readonly DependencyProperty OrientationProperty =
        DependencyProperty.Register(nameof(Orientation), typeof(Orientation),typeof(LoopPanel), new PropertyMetadata(Orientation.Horizontal));

    public double RelativeOffset
    {
        get { return (double)GetValue(RelativeOffsetProperty); }
        set { SetValue(RelativeOffsetProperty, value); }
    }

    public static readonly DependencyProperty RelativeOffsetProperty =
        DependencyProperty.Register(nameof(RelativeOffset), typeof(double), typeof(LoopPanel), new PropertyMetadata(0.5d, IsRelativeOffsetValid));
    
    private static void IsRelativeOffsetValid(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        IsRelativeOffsetValid(e.NewValue);
    }
}
