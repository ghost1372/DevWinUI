namespace DevWinUI;

public sealed partial class MorphingWizard
{
    public ObservableCollection<MorphingWizardStep> Steps => _steps;

    public int CurrentIndex => _index;

    public event EventHandler? Completed;

    public event EventHandler<int>? StepChanged;

    public int DefaultIndex
    {
        get { return (int)GetValue(DefaultIndexProperty); }
        set { SetValue(DefaultIndexProperty, value); }
    }

    public static readonly DependencyProperty DefaultIndexProperty =
        DependencyProperty.Register(nameof(DefaultIndex), typeof(int), typeof(MorphingWizard), new PropertyMetadata(0));

    public bool ResetOnCompleted
    {
        get { return (bool)GetValue(ResetOnCompletedProperty); }
        set { SetValue(ResetOnCompletedProperty, value); }
    }

    public static readonly DependencyProperty ResetOnCompletedProperty =
        DependencyProperty.Register(nameof(ResetOnCompleted), typeof(bool), typeof(MorphingWizard), new PropertyMetadata(true));

    public string BackText
    {
        get { return (string)GetValue(BackTextProperty); }
        set { SetValue(BackTextProperty, value); }
    }

    public static readonly DependencyProperty BackTextProperty =
        DependencyProperty.Register(nameof(BackText), typeof(string), typeof(MorphingWizard), new PropertyMetadata("Back"));

    public string ContinueText
    {
        get { return (string)GetValue(ContinueTextProperty); }
        set { SetValue(ContinueTextProperty, value); }
    }

    public static readonly DependencyProperty ContinueTextProperty =
        DependencyProperty.Register(nameof(ContinueText), typeof(string), typeof(MorphingWizard), new PropertyMetadata("Continue", OnChromeChanged));

    public string FinishText
    {
        get { return (string)GetValue(FinishTextProperty); }
        set { SetValue(FinishTextProperty, value); }
    }

    public static readonly DependencyProperty FinishTextProperty =
        DependencyProperty.Register(nameof(FinishText), typeof(string), typeof(MorphingWizard), new PropertyMetadata("Finish", OnChromeChanged));

    public string CounterFormat
    {
        get { return (string)GetValue(CounterFormatProperty); }
        set { SetValue(CounterFormatProperty, value); }
    }

    public static readonly DependencyProperty CounterFormatProperty =
        DependencyProperty.Register(nameof(CounterFormat), typeof(string), typeof(MorphingWizard), new PropertyMetadata("Step {0} of {1}", OnChromeChanged));

    public Brush? CounterForeground
    {
        get { return (Brush?)GetValue(CounterForegroundProperty); }
        set { SetValue(CounterForegroundProperty, value); }
    }

    public static readonly DependencyProperty CounterForegroundProperty =
        DependencyProperty.Register(nameof(CounterForeground), typeof(Brush), typeof(MorphingWizard), new PropertyMetadata(null));

    public Brush? TrackBrush
    {
        get { return (Brush?)GetValue(TrackBrushProperty); }
        set { SetValue(TrackBrushProperty, value); }
    }

    public static readonly DependencyProperty TrackBrushProperty =
        DependencyProperty.Register(nameof(TrackBrush), typeof(Brush), typeof(MorphingWizard),
            new PropertyMetadata(null, (d, e) => ((MorphingWizard)d).ApplyBrushes()));

    public Brush? ProgressBrush
    {
        get { return (Brush?)GetValue(ProgressBrushProperty); }
        set { SetValue(ProgressBrushProperty, value); }
    }

    public static readonly DependencyProperty ProgressBrushProperty =
        DependencyProperty.Register(nameof(ProgressBrush), typeof(Brush), typeof(MorphingWizard),
            new PropertyMetadata(null, (d, e) => ((MorphingWizard)d).ApplyBrushes()));

    public Brush? SegmentForeground
    {
        get { return (Brush?)GetValue(SegmentForegroundProperty); }
        set { SetValue(SegmentForegroundProperty, value); }
    }

    public static readonly DependencyProperty SegmentForegroundProperty =
        DependencyProperty.Register(nameof(SegmentForeground), typeof(Brush), typeof(MorphingWizard),
            new PropertyMetadata(null, (d, e) => ((MorphingWizard)d).ApplyBrushes()));

    public Brush? ActiveSegmentForeground
    {
        get { return (Brush?)GetValue(ActiveSegmentForegroundProperty); }
        set { SetValue(ActiveSegmentForegroundProperty, value); }
    }

    public static readonly DependencyProperty ActiveSegmentForegroundProperty =
        DependencyProperty.Register(nameof(ActiveSegmentForeground), typeof(Brush), typeof(MorphingWizard),
            new PropertyMetadata(null, (d, e) => ((MorphingWizard)d).ApplyBrushes()));

    private static void OnChromeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
        ((MorphingWizard)d).UpdateChrome();
}
