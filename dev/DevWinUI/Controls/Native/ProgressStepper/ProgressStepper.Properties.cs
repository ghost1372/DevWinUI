namespace DevWinUI;

public sealed partial class ProgressStepper
{
    public Brush? PrimaryForeground
    {
        get { return (Brush?)GetValue(PrimaryForegroundProperty); }
        set { SetValue(PrimaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty PrimaryForegroundProperty =
        DependencyProperty.Register(nameof(PrimaryForeground), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? SecondaryForeground
    {
        get { return (Brush?)GetValue(SecondaryForegroundProperty); }
        set { SetValue(SecondaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty SecondaryForegroundProperty =
        DependencyProperty.Register(nameof(SecondaryForeground), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? TertiaryForeground
    {
        get { return (Brush?)GetValue(TertiaryForegroundProperty); }
        set { SetValue(TertiaryForegroundProperty, value); }
    }

    public static readonly DependencyProperty TertiaryForegroundProperty =
        DependencyProperty.Register(nameof(TertiaryForeground), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? RailBrush
    {
        get { return (Brush?)GetValue(RailBrushProperty); }
        set { SetValue(RailBrushProperty, value); }
    }

    public static readonly DependencyProperty RailBrushProperty =
        DependencyProperty.Register(nameof(RailBrush), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? SuccessBrush
    {
        get { return (Brush?)GetValue(SuccessBrushProperty); }
        set { SetValue(SuccessBrushProperty, value); }
    }

    public static readonly DependencyProperty SuccessBrushProperty =
        DependencyProperty.Register(nameof(SuccessBrush), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? SuccessFillBrush
    {
        get { return (Brush?)GetValue(SuccessFillBrushProperty); }
        set { SetValue(SuccessFillBrushProperty, value); }
    }

    public static readonly DependencyProperty SuccessFillBrushProperty =
        DependencyProperty.Register(nameof(SuccessFillBrush), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? AccentBrush
    {
        get { return (Brush?)GetValue(AccentBrushProperty); }
        set { SetValue(AccentBrushProperty, value); }
    }

    public static readonly DependencyProperty AccentBrushProperty =
        DependencyProperty.Register(nameof(AccentBrush), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    public Brush? AccentTrackBrush
    {
        get { return (Brush?)GetValue(AccentTrackBrushProperty); }
        set { SetValue(AccentTrackBrushProperty, value); }
    }

    public static readonly DependencyProperty AccentTrackBrushProperty =
        DependencyProperty.Register(nameof(AccentTrackBrush), typeof(Brush), typeof(ProgressStepper), new PropertyMetadata(null, BrushChanged));

    private bool _rebuildQueued;

    private static void BrushChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is ProgressStepper control)
        {
            control.QueueBuild();
        }
    }

    private void QueueBuild()
    {
        if (_rebuildQueued) return;

        _rebuildQueued = true;
        DispatcherQueue.TryEnqueue(() =>
        {
            _rebuildQueued = false;
            Build();
        });
    }
}
